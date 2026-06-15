using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.Asset;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Asset;

namespace RERPAPI.Controllers.Old.Asset
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsAllocationController : ControllerBase
    {
        private vUserGroupLinksRepo _vUserGroupLinksRepo;

        private TSAssetManagementRepo _tsAssetManagementRepo;
        private TSAssetAllocationRepo _tSAssetAllocationRepo;
        private TSAssetAllocationDetailRepo _tSAssetAllocationDetailRepo;
        private TSAllocationEvictionAssetRepo _tSAllocationEvictionAssetRepo;
        private AssetAllocationLogRepo _assetAllocationLogRepo;
        private IConfiguration _configuration;

        public AssetsAllocationController(TSAssetManagementRepo tSAssetManagementRepo, TSAssetAllocationRepo tSAssetAllocationRepo, TSAssetAllocationDetailRepo tSAssetAllocationDetailRepo, TSAllocationEvictionAssetRepo tSAllocationEvictionAssetRepo, vUserGroupLinksRepo vUserGroupLinksRepo, AssetAllocationLogRepo assetAllocationLogRepo, IConfiguration configuration)
        {
            _vUserGroupLinksRepo = vUserGroupLinksRepo;
            _tsAssetManagementRepo = tSAssetManagementRepo;
            _tSAssetAllocationRepo = tSAssetAllocationRepo;
            _tSAssetAllocationDetailRepo = tSAssetAllocationDetailRepo;
            _tSAllocationEvictionAssetRepo = tSAllocationEvictionAssetRepo;
            _assetAllocationLogRepo = assetAllocationLogRepo;
            _configuration = configuration;
        }

        [HttpPost("get-allocation")]
        public async Task<ActionResult> GetAssetAllocationnn([FromBody] AssetAllocationRequestParam request)

        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                var vUserHR = _vUserGroupLinksRepo
      .GetAll()
      .FirstOrDefault(x =>
          (x.Code == "N23" || x.Code == "N1" || x.Code == "N67") &&
          x.UserID == currentUser.ID);

                int employeeID;
                if (vUserHR != null)
                {
                    employeeID = request.EmployeeID.Value;
                }
                else
                {
                    employeeID = currentUser.EmployeeID;
                }
                var assetAllocation = SQLHelper<dynamic>.ProcedureToList(
                    "spGetTSAssetAllocation",
                    new string[] { "@DateStart", "@DateEnd", "@EmployeeID", "@Status", "@FilterText", "@PageSize", "@PageNumber" },
                    new object[] { request.DateStart, request.DateEnd, employeeID, request.Status, request.FilterText ?? string.Empty, request.PageSize, request.PageNumber });

                return Ok(new
                {
                    status = 1,
                    assetAllocation = SQLHelper<dynamic>.GetListData(assetAllocation, 0)
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        [HttpGet("get-asset-allocation-detail")]
        public IActionResult GetAssetAllocationDetail(string? id)
        {
            try
            {
                var assetsAllocationDetail = SQLHelper<dynamic>.ProcedureToList("spGetTSAssetAllocationDetail", new string[] { "@TSAssetAllocationID" }, new object[] { id });
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        assetsAllocationDetail = SQLHelper<dynamic>.GetListData(assetsAllocationDetail, 0)
                    }
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        [HttpGet("get-allocation-code")]
        public async Task<IActionResult> GenerateAllocationCode([FromQuery] DateTime? allocationDate)
        {
            if (allocationDate == null)
                return BadRequest("allocationDate is required.");

            string newCode = _tSAssetAllocationRepo.generateAllocationCode(allocationDate);
            return Ok(new
            {
                status = 1,
                data = newCode
            });
        }

        [HttpGet("get-asset-allocation-log/{assetId}")]
        public IActionResult GetAllocationLog(int assetId)
        {
            try
            {
                var assetLogs = _assetAllocationLogRepo.GetAll(x => x.AssetAllocationID == assetId).OrderByDescending(x => x.CreatedDate).ToList();
                return Ok(ApiResponseFactory.Success(assetLogs));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("export-allocation-asset-report")]
        public IActionResult ExportAllocationAssetReport([FromBody] AssetAllocationExportFullDto dto)
        {
            var master = dto.Master;
            var details = dto.Details;

            if (master == null || details == null || !details.Any())
                return BadRequest("Dữ liệu cấp phát không hợp lệ.");

            try
            {
                ExcelPackage.License.SetNonCommercialOrganization("RTC");

                //string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "BienBanBanGiao.xlsx");

                var templateFolder = _configuration.GetValue<string>("PathTemplate");

                if (string.IsNullOrWhiteSpace(templateFolder))
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy đường dẫn thư mục {templateFolder} trên sever!!"));
                string templatePath = Path.Combine(templateFolder, "ExportExcel", "BienBanBanGiao.xlsx");
                if (!System.IO.File.Exists(templatePath))
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy File mẫu!"));

                string fileName = $"PhieuCapPhat_{master.Code}.xlsx";

                using var stream = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var package = new ExcelPackage(stream);
                var ws = package.Workbook.Worksheets[0];

                string date = $"Hà Nội, Ngày {master.DateAllocation.Day} tháng {master.DateAllocation.Month} năm {master.DateAllocation.Year} tại Văn phòng Công ty Cổ phần RTC Technology Việt Nam. Chúng tôi gồm các bên sau:";

                // --- Ghi thông tin master ---
                ws.Cells[5, 1].Value = master.Code;
                ws.Cells[6, 1].Value = date;

                // Danh sách không trùng lặp
                var names = new HashSet<string>();
                var positions = new HashSet<string>();
                var departments = new HashSet<string>();

                foreach (var detail in details)
                {
                    if (!string.IsNullOrEmpty(detail.FullName)) names.Add(detail.FullName);
                    if (!string.IsNullOrEmpty(detail.PositionName)) positions.Add(detail.PositionName);
                    if (!string.IsNullOrEmpty(detail.DepartmentName)) departments.Add(detail.DepartmentName);
                }

                ws.Cells[8, 3].Value = master.EmployeeAllocationName;
                ws.Cells[9, 3].Value = master.PosittionAllocation;
                ws.Cells[10, 3].Value = master.DepartmentAllocation;

                ws.Cells[13, 3].Value = string.Join(", ", names);
                ws.Cells[14, 3].Value = string.Join(", ", positions);
                ws.Cells[15, 3].Value = string.Join(", ", departments);
                ws.Cells[17, 3].Value = master.Note;

                ws.Cells[27, 1].Value = master.EmployeeName ?? "";
                ws.Cells[27, 8].Value = master.EmployeeName ?? "";
                ws.Cells[28, 1].Value = master.DateApprovedHR?.ToString("dd/MM/yyyy HH:mm") ?? "";
                ws.Cells[28, 8].Value = master.DateApprovedPersonalProperty?.ToString("dd/MM/yyyy HH:mm") ?? "";

                ws.DeleteRow(20, 1);

                int startRow = 20;
                for (int i = 0; i < details.Count; i++)
                {
                    var item = details[i];
                    int row = startRow + i;
                    ws.InsertRow(row, 1);

                    ws.Cells[row, 1].Value = i + 1;
                    ws.Cells[row, 2].Value = item.TSCodeNCC ?? "";
                    ws.Cells[row, 3].Value = item.TSAssetName ?? "";
                    ws.Cells[row, 5].Value = item.UnitName ?? "";
                    ws.Cells[row, 6].Value = item.Quantity;
                    ws.Cells[row, 7].Value = item.Status ?? ""; // Tình trạng không có trong DTO
                    ws.Cells[row, 8].Value = item.Note;
                }

                var outputStream = new MemoryStream();
                package.SaveAs(outputStream);
                outputStream.Position = 0;

                return File(outputStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [RequiresPermission("N23,N1,N67")]
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] TSAssetAllocationFullDTO allocations)
        {
            try
            {
                int assetId = 0;
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                if (allocations == null) { return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." }); }
                if (allocations.tSAssetAllocation != null)
                {
                    if (allocations.tSAssetAllocation.ID <= 0)
                    {
                        allocations.tSAssetAllocation.AllocationID = currentUser.EmployeeID;
                        await _tSAssetAllocationRepo.CreateAsync(allocations.tSAssetAllocation);
                    }
                    else
                    {
                        var existingMaster = _tSAssetAllocationRepo.GetSingleNoTracking(x => x.ID == allocations.tSAssetAllocation.ID);
                        allocations.tSAssetAllocation.AllocationID = existingMaster?.AllocationID;

                        await _tSAssetAllocationRepo.UpdateAsync(allocations.tSAssetAllocation);
                        assetId = allocations.tSAssetAllocation.ID;

                        List<string> changeDetails = _assetAllocationLogRepo.GetEntityChanges(existingMaster, allocations.tSAssetAllocation);
                        if (changeDetails.Any())
                        {
                            await _assetAllocationLogRepo.CreateAsync(new AssetAllocationLog
                            {
                                AssetAllocationID = assetId,
                                EmployeeID = currentUser.EmployeeID,
                                TypeLog = "CẬP NHẬT TÀI SẢN CẤP PHÁT",
                                ContentLog = $"Cập nhật tài sản cấp phát: {string.Join(", ", changeDetails)}",
                                CreatedBy = currentUser.LoginName,
                                CreatedDate = DateTime.Now
                            });
                        }
                    }
                }

                if (allocations.tSAssetAllocationDetails != null && allocations.tSAssetAllocationDetails.Any())
                {
                    foreach (var item in allocations.tSAssetAllocationDetails)
                    {
                        item.TSAssetAllocationID = allocations.tSAssetAllocation.ID;

                        var existingDetail = item.ID > 0 ? _tSAssetAllocationDetailRepo.GetSingleNoTracking(x => x.ID == item.ID) : null;
                        int? assetIdLookup = item.AssetManagementID > 0 ? item.AssetManagementID : existingDetail?.AssetManagementID;
                        var master = assetIdLookup > 0 ? _tsAssetManagementRepo.GetSingleNoTracking(x => x.ID == assetIdLookup) : null;
                        string code = master?.TSCodeNCC ?? "Trống";

                        if (item.ID <= 0)
                        {
                            await _tSAssetAllocationDetailRepo.CreateAsync(item);
                            await _assetAllocationLogRepo.CreateAsync(new AssetAllocationLog
                            {
                                AssetAllocationID = allocations.tSAssetAllocation.ID,
                                EmployeeID = currentUser.EmployeeID,
                                TypeLog = "THÊM CHI TIẾT CẤP PHÁT",
                                ContentLog = $"Thêm chi tiết cấp phát (Mã tài sản: {code}). Số lượng: {item.Quantity}. Ghi chú: {item.Note}",
                                CreatedBy = currentUser.LoginName,
                                CreatedDate = DateTime.Now
                            });
                        }
                        else
                        {
                            await _tSAssetAllocationDetailRepo.UpdateAsync(item);

                            if (item.IsDeleted == true && existingDetail?.IsDeleted != true)
                            {
                                await _assetAllocationLogRepo.CreateAsync(new AssetAllocationLog
                                {
                                    AssetAllocationID = allocations.tSAssetAllocation.ID,
                                    EmployeeID = currentUser.EmployeeID,
                                    TypeLog = "XÓA CHI TIẾT CẤP PHÁT",
                                    ContentLog = $"Xóa tài sản khỏi cấp phát (Mã tài sản: {code})",
                                    CreatedBy = currentUser.LoginName,
                                    CreatedDate = DateTime.Now
                                });
                            }
                            else if (existingDetail != null)
                            {
                                List<string> changeDetails = _assetAllocationLogRepo.GetEntityChanges(existingDetail, item);
                                if (changeDetails.Any())
                                {
                                    await _assetAllocationLogRepo.CreateAsync(new AssetAllocationLog
                                    {
                                        AssetAllocationID = allocations.tSAssetAllocation.ID,
                                        EmployeeID = currentUser.EmployeeID,
                                        TypeLog = "CẬP NHẬT CHI TIẾT CẤP PHÁT",
                                        ContentLog = $"Cập nhật chi tiết cấp phát (Mã tài sản: {code}): {string.Join(", ", changeDetails)}",
                                        CreatedBy = currentUser.LoginName,
                                        CreatedDate = DateTime.Now
                                    });
                                }
                            }
                        }
                    }
                }
                if (allocations.tSAssetManagements != null && allocations.tSAssetManagements.Any())
                {
                    foreach (var item in allocations.tSAssetManagements)
                    {
                        if (item.ID <= 0)
                        {
                            await _tsAssetManagementRepo.CreateAsync(item);
                        }
                        else
                        {
                            await _tsAssetManagementRepo.UpdateAsync(item);
                        }
                    }
                }
                if (allocations.tSAllocationEvictionAssets != null && allocations.tSAllocationEvictionAssets.Any())
                {
                    foreach (var item in allocations.tSAllocationEvictionAssets)
                    {
                        bool isNew = item.ID <= 0;
                        if (isNew)
                        {
                            await _tSAllocationEvictionAssetRepo.CreateAsync(item);
                        }
                        else
                        {
                            var existingEviction = _tSAllocationEvictionAssetRepo.GetSingleNoTracking(x => x.ID == item.ID);
                            await _tSAllocationEvictionAssetRepo.UpdateAsync(item);

                            List<string> changeDetails = _assetAllocationLogRepo.GetEntityChanges(existingEviction, item);
                            if (changeDetails.Any())
                            {
                                await _assetAllocationLogRepo.CreateAsync(new AssetAllocationLog
                                {
                                    AssetAllocationID = allocations.tSAssetAllocation.ID,
                                    EmployeeID = currentUser.EmployeeID,
                                    TypeLog = "CẬP NHẬT CẤP PHÁT / THU HỒI",
                                    ContentLog = $"Cập nhật trạng thái cấp phát/thu hồi: {string.Join(", ", changeDetails)}",
                                    CreatedBy = currentUser.LoginName,
                                    CreatedDate = DateTime.Now
                                });
                            }
                        }

                        if (isNew)
                        {
                            var master = _tsAssetManagementRepo.GetSingleNoTracking(x => x.ID == item.AssetManagementID);
                            string code = master?.TSCodeNCC ?? "Trống";
                            string action = item.Status == "Đang sử dụng" ? "Cấp phát" : "Thu hồi";

                            await _assetAllocationLogRepo.CreateAsync(new AssetAllocationLog
                            {
                                AssetAllocationID = allocations.tSAssetAllocation.ID,
                                EmployeeID = currentUser.EmployeeID,
                                TypeLog = $"{action.ToUpper()} TÀI SẢN",
                                ContentLog = $"{action} tài sản. Mã: {code}. Ghi chú: {item.Note}",
                                CreatedBy = currentUser.LoginName,
                                CreatedDate = DateTime.Now
                            });
                        }
                    }
                }
                return Ok(ApiResponseFactory.Success(allocations, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-appropve-personal")]
        public async Task<IActionResult> SaveAppropvePersonal([FromBody] TSAssetAllocationFullDTO allocations)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                if (allocations == null) { return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." }); }
                if (allocations.tSAssetAllocation != null)
                {
                    if (allocations.tSAssetAllocation.ID <= 0)
                    {
                        allocations.tSAssetAllocation.AllocationID = currentUser.ID;
                        await _tSAssetAllocationRepo.CreateAsync(allocations.tSAssetAllocation);
                    }
                    else
                        await _tSAssetAllocationRepo.UpdateAsync(allocations.tSAssetAllocation);
                }

                if (allocations.tSAssetAllocationDetails != null && allocations.tSAssetAllocationDetails.Any())
                {
                    foreach (var item in allocations.tSAssetAllocationDetails)
                    {
                        item.TSAssetAllocationID = allocations.tSAssetAllocation.ID;
                        if (item.ID <= 0)
                            await _tSAssetAllocationDetailRepo.CreateAsync(item);
                        else
                            await _tSAssetAllocationDetailRepo.UpdateAsync(item);
                    }
                }
                if (allocations.tSAssetManagements != null && allocations.tSAssetManagements.Any())
                {
                    foreach (var item in allocations.tSAssetManagements)
                    {
                        if (item.ID <= 0)
                            await _tsAssetManagementRepo.CreateAsync(item);
                        else
                            await _tsAssetManagementRepo.UpdateAsync(item);
                    }
                }
                if (allocations.tSAllocationEvictionAssets != null && allocations.tSAllocationEvictionAssets.Any())
                {
                    foreach (var item in allocations.tSAllocationEvictionAssets)
                    {
                        if (item.ID <= 0)
                            await _tSAllocationEvictionAssetRepo.CreateAsync(item);
                        else
                            await _tSAllocationEvictionAssetRepo.UpdateAsync(item);
                    }
                }
                return Ok(ApiResponseFactory.Success(allocations, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [RequiresPermission("N67,N1")]
        [HttpPost("save-appropve-kt")]
        public async Task<IActionResult> SaveAppropveKT([FromBody] TSAssetAllocationFullDTO allocations)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                if (allocations == null) { return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." }); }
                if (allocations.tSAssetAllocation != null)
                {
                    if (allocations.tSAssetAllocation.ID <= 0)
                    {
                        allocations.tSAssetAllocation.AllocationID = currentUser.ID;
                        await _tSAssetAllocationRepo.CreateAsync(allocations.tSAssetAllocation);
                    }
                    else
                        await _tSAssetAllocationRepo.UpdateAsync(allocations.tSAssetAllocation);
                }

                if (allocations.tSAssetAllocationDetails != null && allocations.tSAssetAllocationDetails.Any())
                {
                    foreach (var item in allocations.tSAssetAllocationDetails)
                    {
                        item.TSAssetAllocationID = allocations.tSAssetAllocation.ID;
                        if (item.ID <= 0)
                            await _tSAssetAllocationDetailRepo.CreateAsync(item);
                        else
                            await _tSAssetAllocationDetailRepo.UpdateAsync(item);
                    }
                }
                if (allocations.tSAssetManagements != null && allocations.tSAssetManagements.Any())
                {
                    foreach (var item in allocations.tSAssetManagements)
                    {
                        if (item.ID <= 0)
                            await _tsAssetManagementRepo.CreateAsync(item);
                        else
                            await _tsAssetManagementRepo.UpdateAsync(item);
                    }
                }
                if (allocations.tSAllocationEvictionAssets != null && allocations.tSAllocationEvictionAssets.Any())
                {
                    foreach (var item in allocations.tSAllocationEvictionAssets)
                    {
                        if (item.ID <= 0)
                            await _tSAllocationEvictionAssetRepo.CreateAsync(item);
                        else
                            await _tSAllocationEvictionAssetRepo.UpdateAsync(item);
                    }
                }
                return Ok(ApiResponseFactory.Success(allocations, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}