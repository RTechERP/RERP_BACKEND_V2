    using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.Asset;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.Asset;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Asset;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RERPAPI.Controllers.Old.Asset
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsRecoveryController : ControllerBase
    {
        TSAssetRecoveryRepo _tSAssetRecoveryRepo;
        TSAssetRecoveryDetailRepo _tSAssetRecoveryDetailRepo;
        TSAssetManagementRepo _tsAssetManagementRepo;
        TSAllocationEvictionAssetRepo _tSAllocationEvictionAssetRepo;
        vUserGroupLinksRepo _vUserGroupLinksRepo;
        private readonly RoleConfig _roleConfig;
        private IConfiguration _configuration;
        public AssetsRecoveryController(TSAssetRecoveryRepo tSAssetRecoveryRepo, TSAssetRecoveryDetailRepo tSAssetRecoveryDetailRepo, TSAssetManagementRepo tSAssetManagementRepo, TSAllocationEvictionAssetRepo tSAllocationEvictionAssetRepo, vUserGroupLinksRepo vUserGroupLinksRepo, IConfiguration configuration)
        {
            _tSAssetRecoveryRepo = tSAssetRecoveryRepo;
            _tSAllocationEvictionAssetRepo = tSAllocationEvictionAssetRepo;
            _tsAssetManagementRepo = tSAssetManagementRepo;
            _tSAssetRecoveryDetailRepo = tSAssetRecoveryDetailRepo;
            _vUserGroupLinksRepo = vUserGroupLinksRepo;
            _configuration = configuration;

        }

        [HttpPost("get-asset-recovery")]
        public async Task<ActionResult> GetAssetsRecovery(AssetRecoveryRequestParam request)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                var vUserHR = _vUserGroupLinksRepo.GetAll().FirstOrDefault(x =>
                 (x.Code == "N23" || x.Code == "N1" || x.Code == "N67") &&
                x.UserID == currentUser.ID);

                int employeeID;
                if (vUserHR != null)
                {
                    employeeID = request.EmployeeReturnID.Value;
                }
                else
                {
                    employeeID = currentUser.EmployeeID;
                }
                var assetRecovery = SQLHelper<dynamic>.ProcedureToList("spGetTSAssetRecovery",
                    new string[] { "@DateStart", "@DateEnd", "@EmployeeReturnID", "@EmployeeRecoveryID", "@Status", "@FilterText", "@PageSize", "@PageNumber" },
                    new object[] { request.DateStart, request.DateEnd, employeeID, request.EmployeeRecoveryID ?? 0, request.Status ?? -1, request.Filtertext ?? "", request.PageSize, request.PageNumber, });
                return Ok(new
                {
                    status = 1,

                    assetsrecovery = SQLHelper<dynamic>.GetListData(assetRecovery, 0)

                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-asset-recovery-detail")]
        public IActionResult GetAssetsRecoveryDetail(int? id)
        {
            try
            {
                var result = SQLHelper<dynamic>.ProcedureToList(
             "spGetTSAssetRecoveryDetail",
             new string[] { "@TSAssetRecoveryID" },
             new object[] { id }
         );
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        assetsRecoveryDetail = SQLHelper<dynamic>.GetListData(result, 0)
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-recovery-code")]
        public async Task<IActionResult> GenerateRecoveryCode([FromQuery] DateTime? recoveryDate)
        {
            if (recoveryDate == null)
                return BadRequest("recoveryDate is required.");

            string newCode = _tSAssetRecoveryRepo.genCodeRecovery(recoveryDate);

            return Ok(new
            {
                status = 1,
                data = newCode
            });
        }
        [HttpGet("get-recovery-by-employee")]
        public IActionResult GetRecoveryByEmployee(int? recoveID, int? employeeID)
        {
            try
            {
                var assetsRecoveryByEmployee = SQLHelper<dynamic>.ProcedureToList(
                    "spGetTSAssetByEmployee",
                    new string[] { "@EmployeeID", "@StatusID" },
                    new object[] { employeeID, 0 });
                if (recoveID > 0)
                {
                    var mergedAssets = SQLHelper<dynamic>.ProcedureToList(
                        "spGetTSAssetByEmployeeMerge",
                        new string[] { "@TSAssetID", "@TSAssetType" },
                        new object[] { recoveID, 2 });
                    assetsRecoveryByEmployee.AddRange(mergedAssets);
                }
                return Ok(new
                {
                    status = 1,

                    assetsRecoveryByEmployee = SQLHelper<dynamic>.GetListData(assetsRecoveryByEmployee, 0)

                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("export-recovery-asset-report")]
        public IActionResult ExportRecoveryAssetReport([FromBody] AssetRecoveryExportFullDto dto)
        {
            var master = dto.Master;
                var details = dto.Details;

            if (master == null || details == null || !details.Any())
                return BadRequest("Dữ liệu thu hồi không hợp lệ.");
            try
            {
                ExcelPackage.License.SetNonCommercialOrganization("RTC Technology Viet Nam");


                var templateFolder = _configuration.GetValue<string>("PathTemplate");

                if (string.IsNullOrWhiteSpace(templateFolder))
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy đường dẫn thư mục {templateFolder} trên sever!!"));
                string templatePath = Path.Combine(templateFolder, "ExportExcel", "BienBanBanGiao.xlsx");
                if (!System.IO.File.Exists(templatePath))
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy File mẫu!"));

                var fileName = $"PhieuThuHoi_{master.Code}.xlsx";
                var exportPath = Path.Combine(Path.GetTempPath(), fileName);

                using (var stream = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var package = new ExcelPackage(stream))
                {
                    var ws = package.Workbook.Worksheets[0];

                    // Ghi dữ liệu vào file Excel giống như trong Interop
                    string location = $"Hà Nội, Ngày {master.DateRecovery.Day} tháng {master.DateRecovery.Month} năm {master.DateRecovery.Year} tại Văn phòng Công ty Cổ phần RTC Technology Việt Nam. Chúng tôi gồm các bên sau:";
                    ws.Cells[4, 1].Value = "BIÊN BẢN THU HỒI TÀI SẢN";
                    ws.Cells[5, 1].Value = master.Code;
                    ws.Cells[6, 1].Value = location;

                    ws.Cells[8, 3].Value = master.EmployeeReturnName;
                    ws.Cells[9, 3].Value = master.PossitionReturn;
                    ws.Cells[10, 3].Value = master.DepartmentReturn;

                    ws.Cells[13, 3].Value = master.EmployeeRecoveryName;
                    ws.Cells[14, 3].Value = master.PossitionRecovery;
                    ws.Cells[15, 3].Value = master.DepartmentRecovery;

                    ws.Cells[17, 3].Value = master.Note;


                   
                    ws.Cells[27, 1].Value = master.EmployeeReturnName??"";
                    ws.Cells[27, 8].Value = master.EmployeeRecoveryName ?? "";
                    ws.Cells[28, 1].Value = master.DateApprovedHR?.ToString("dd/MM/yyyy HH:mm") ??"";
                    ws.Cells[28, 8].Value = master.DateApprovedPersonalProperty?.ToString("dd/MM/yyyy HH:mm") ?? "";


                    // Xoá dòng template sẵn (dòng 20 + 21)
                    ws.DeleteRow(20, 1);

                    // Ghi dữ liệu chi tiết từ dòng 21 trở đi
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
                        ws.Cells[row, 7].Value = item.Status ?? "";
                        ws.Cells[row, 8].Value = item.Note ?? "";
                    }
                        //ws.Cells[27, 1].Value = master.EmployeeReturnName ?? "";
                        //ws.Cells[27, 8].Value = master.EmployeeRecoveryName?? "";
                        //ws.Cells[28, 1].Value = master.DateApprovedPersonalProperty?.ToString("dd/MM/yyyy HH:mm") ?? "";
                        //ws.Cells[28, 8].Value = master.DateApprovedHR?.ToString("dd/MM/yyyy HH:mm") ?? "";
                    // Lưu file tạm rồi trả về
                    var outStream = new MemoryStream();
                    package.SaveAs(outStream);
                    outStream.Position = 0;

                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    return File(outStream, contentType, fileName);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N23")]
            
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData2([FromBody] AssetRecoveryDTO asset)
        {
            try
            {
                if (asset == null)
                {
                    return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." });
                }
                if (asset.tSAssetRecovery != null)
                {
                    if (asset.tSAssetRecovery.ID <= 0)
                    {
                        await _tSAssetRecoveryRepo.CreateAsync(asset.tSAssetRecovery);
                    }
                    else
                    {
                        await _tSAssetRecoveryRepo.UpdateAsync(asset.tSAssetRecovery);
                    }
                }
                if (asset.TSAssetRecoveryDetails != null && asset.TSAssetRecoveryDetails.Any())
                {
                    foreach (var item in asset.TSAssetRecoveryDetails)
                    {
                        item.TSAssetRecoveryID = asset.tSAssetRecovery.ID;
                        if (item.ID <= 0)
                            await _tSAssetRecoveryDetailRepo.CreateAsync(item);
                        else
                            await _tSAssetRecoveryDetailRepo.UpdateAsync(item);
                    }
                }
                if (asset.tSAssetManagements != null && asset.tSAssetManagements.Any())
                {
                    foreach (var item in asset.tSAssetManagements)
                    {

                        if (item.ID <= 0)
                            await _tsAssetManagementRepo.CreateAsync(item);
                        else
                            await _tsAssetManagementRepo.UpdateAsync(item);
                    }
                }
                if (asset.tSAllocationEvictionAssets != null && asset.tSAllocationEvictionAssets.Any())
                {
                    foreach (var item in asset.tSAllocationEvictionAssets)
                    {

                        if (item.ID <= 0)
                            await _tSAllocationEvictionAssetRepo.CreateAsync(item);
                        else
                            await _tSAllocationEvictionAssetRepo.UpdateAsync(item);
                    }
                }
                return Ok(new
                {
                    status = 1,
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
        [HttpPost("save-data-personal")]
        public async Task<IActionResult> SaveDataPersonal([FromBody] AssetRecoveryDTO asset)
        {
            try
            {
                if (asset == null)
                {
                    return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." });
                }
                if (asset.tSAssetRecovery != null)
                {
                    if (asset.tSAssetRecovery.ID <= 0)
                    {
                        await _tSAssetRecoveryRepo.CreateAsync(asset.tSAssetRecovery);
                    }
                    else
                    {
                        await _tSAssetRecoveryRepo.UpdateAsync(asset.tSAssetRecovery);
                    }
                }
                if (asset.TSAssetRecoveryDetails != null && asset.TSAssetRecoveryDetails.Any())
                {
                    foreach (var item in asset.TSAssetRecoveryDetails)
                    {
                        item.TSAssetRecoveryID = asset.tSAssetRecovery.ID;
                        if (item.ID <= 0)
                            await _tSAssetRecoveryDetailRepo.CreateAsync(item);
                        else
                            await _tSAssetRecoveryDetailRepo.UpdateAsync(item);
                    }
                }
                if (asset.tSAssetManagements != null && asset.tSAssetManagements.Any())
                {
                    foreach (var item in asset.tSAssetManagements)
                    {

                        if (item.ID <= 0)
                            await _tsAssetManagementRepo.CreateAsync(item);
                        else
                            await _tsAssetManagementRepo.UpdateAsync(item);
                    }
                }
                if (asset.tSAllocationEvictionAssets != null && asset.tSAllocationEvictionAssets.Any())
                {
                    foreach (var item in asset.tSAllocationEvictionAssets)
                    {

                        if (item.ID <= 0)
                            await _tSAllocationEvictionAssetRepo.CreateAsync(item);
                        else
                            await _tSAllocationEvictionAssetRepo.UpdateAsync(item);
                    }
                }
                return Ok(new
                {
                    status = 1,
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
        [RequiresPermission("N1,N67")]
        [HttpPost("save-data-kt")]
        public async Task<IActionResult> SaveDataKT([FromBody] AssetRecoveryDTO asset)
        {
            try
            {
                if (asset == null)
                {
                    return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." });
                }
                if (asset.tSAssetRecovery != null)
                {
                    if (asset.tSAssetRecovery.ID <= 0)
                    {
                        await _tSAssetRecoveryRepo.CreateAsync(asset.tSAssetRecovery);
                    }
                    else
                    {
                        await _tSAssetRecoveryRepo.UpdateAsync(asset.tSAssetRecovery);
                    }
                }
                if (asset.TSAssetRecoveryDetails != null && asset.TSAssetRecoveryDetails.Any())
                {
                    foreach (var item in asset.TSAssetRecoveryDetails)
                    {
                        item.TSAssetRecoveryID = asset.tSAssetRecovery.ID;
                        if (item.ID <= 0)
                            await _tSAssetRecoveryDetailRepo.CreateAsync(item);
                        else
                            await _tSAssetRecoveryDetailRepo.UpdateAsync(item);
                    }
                }
                if (asset.tSAssetManagements != null && asset.tSAssetManagements.Any())
                {
                    foreach (var item in asset.tSAssetManagements)
                    {

                        if (item.ID <= 0)
                            await _tsAssetManagementRepo.CreateAsync(item);
                        else
                            await _tsAssetManagementRepo.UpdateAsync(item);
                    }
                }
                if (asset.tSAllocationEvictionAssets != null && asset.tSAllocationEvictionAssets.Any())
                {
                    foreach (var item in asset.tSAllocationEvictionAssets)
                    {

                        if (item.ID <= 0)
                            await _tSAllocationEvictionAssetRepo.CreateAsync(item);
                        else
                            await _tSAllocationEvictionAssetRepo.UpdateAsync(item);
                    }
                }
                return Ok(new
                {
                    status = 1,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
