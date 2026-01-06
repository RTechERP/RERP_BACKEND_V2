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
using System;

namespace RERPAPI.Controllers.Old.Asset
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetTranferController : ControllerBase
    {
        TSAssetTransferRepo _tSAssetTransferRepo;
        TSAllocationEvictionAssetRepo _tSAllocationEvictionRepo;
        TSAssetManagementRepo _tsAssetManagementRepo;
        TSAssetTransferDetailRepo _tSAssetTransferDetailRepo;
        private IConfiguration _configuration;
        vUserGroupLinksRepo _vUserGroupLinksRepo;
         
        public AssetTranferController(TSAssetTransferRepo tSAssetTransferRepo, TSAllocationEvictionAssetRepo tSAllocationEvictionAssetRepo, TSAssetManagementRepo TSAssetManagementRepo, TSAssetTransferDetailRepo tSAssetTransferDetailRepo, vUserGroupLinksRepo vUserGroupLinksRepo, IConfiguration configuration)
        {
            _tSAssetTransferRepo = tSAssetTransferRepo;
            _tSAllocationEvictionRepo = tSAllocationEvictionAssetRepo;
            _tsAssetManagementRepo = TSAssetManagementRepo;
            _tSAssetTransferDetailRepo = tSAssetTransferDetailRepo;
            _vUserGroupLinksRepo = vUserGroupLinksRepo;
            _configuration = configuration;
        }

        [HttpPost("get-asset-tranfer")]
        public IActionResult GetListAssetTransfer([FromBody] AssetTransferRequestParam request)
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
                    employeeID = request.ReceiverID.Value;
                }
                else
                {
                    employeeID = currentUser.EmployeeID;
                }
                var assetTranfer = SQLHelper<dynamic>.ProcedureToList("spGetTranferAssetMaster",
                   new string[] { "@DateStart", "@DateEnd", "@IsApproved", "@DeliverID", "@ReceiverID", "TextFilter", "@PageSize", "@PageNumber" },
                                            new object[] { request.DateStart, request.DateEnd, request.IsApproved, request.DeliverID, employeeID, request.TextFilter, request.PageSize, request.PageNumber });
                return Ok(new
                {
                    status = 1,

                    assetTranfer = SQLHelper<dynamic>.GetListData(assetTranfer, 0)

                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-asset-tranfer-detail")]
        public IActionResult GetAssetTranferDetail(string? id)
        {
            try
            {
                var assetTranferDetail = SQLHelper<dynamic>.ProcedureToList(
            "spGetTranferAssetDetail",
            new string[] { "@TranferAssetID" },
            new object[] { id });
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        assetTransferDetail = SQLHelper<dynamic>.GetListData(assetTranferDetail, 0)
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-asset-tranfer-code")]
        public async Task<IActionResult> GenerateTransferCode([FromQuery] DateTime? transferDate)
        {
            if (transferDate == null)
                return BadRequest("allocationDate is required.");

            string newCode = _tSAssetTransferRepo.GenTransferCode(transferDate);
            return Ok(new
            {
                status = 1,
                data = newCode
            });
        }
        [HttpPost("export-transfer-asset-report")]
        public IActionResult ExportTransferAssetReport([FromBody] TranferExportFullDto dto)
        {
            var master = dto.Master;
            var details = dto.Details;

            if (master == null || details == null || !details.Any())
                return BadRequest("Dữ liệu bàn giao không hợp lệ.");
            try
            {
                ExcelPackage.License.SetNonCommercialOrganization("RTC Technology Viet Nam");
                var templateFolder = _configuration.GetValue<string>("PathTemplate");

                if (string.IsNullOrWhiteSpace(templateFolder))
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy đường dẫn thư mục {templateFolder} trên sever!!"));
                string templatePath = Path.Combine(templateFolder, "ExportExcel", "BienBanBanGiao.xlsx");
                if (!System.IO.File.Exists(templatePath))
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy File mẫu!"));
                string fileName = $"BBBG_{master.CodeReport}_{DateTime.Now:ddMMyyyy}.xlsx";
                using var package = new ExcelPackage(new FileInfo(templatePath));
                var ws = package.Workbook.Worksheets[0];

                string headerText = $"Hà Nội, Ngày {master.TranferDate.Day} tháng {master.TranferDate.Month} năm {master.TranferDate.Year} tại Văn phòng Công ty Cổ phần RTC Technology Việt Nam. Chúng tôi gồm các bên sau:";

                ws.Cells[5, 1].Value = master.CodeReport;
                ws.Cells[6, 1].Value = headerText;

                ws.Cells[8, 3].Value = master.DeliverName;
                ws.Cells[9, 3].Value = master.PossitionDeliver;
                ws.Cells[10, 3].Value = master.DepartmentDeliver;

                ws.Cells[13, 3].Value = master.ReceiverName;
                ws.Cells[14, 3].Value = master.PossitionReceiver;
                ws.Cells[15, 3].Value = master.DepartmentReceiver;
                ws.Cells[17, 3].Value = master.Reason;

                //if (master.CreatedDate.HasValue)
                //    ws.Cells[32, 1].Value = master.CreatedDate.Value.ToString("dd/MM/yyyy HH:mm");

                //if (master.DateApprovedPersonalProperty.HasValue)
                //    ws.Cells[32, 8].Value = master.DateApprovedPersonalProperty.Value.ToString("dd/MM/yyyy HH:mm");
                ws.Cells[27, 1].Value = master.DeliverName ?? "";
                ws.Cells[27, 8].Value = master.ReceiverName ?? "";
              
                ws.Cells[28, 1].Value = master.DateApprovedHR?.ToString("dd/MM/yyyy HH:mm") ?? "";
                ws.Cells[28, 8].Value = master.DateApprovedPersonalProperty?.ToString("dd/MM/yyyy HH:mm") ?? "";
              
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




                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                return File(stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [RequiresPermission("N1,N23")]
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] AssetTranferFullDTO assetTransfer)
        {
            try
            {
                if (assetTransfer == null)
                {
                    return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." });
                }
                if (assetTransfer.tSAssetManagements != null && assetTransfer.tSAssetManagements.Any())
                {
                    foreach (var item in assetTransfer.tSAssetManagements)
                    {
                        if (item.ID <= 0)
                            await _tsAssetManagementRepo.CreateAsync(item);
                        else
                            await _tsAssetManagementRepo.UpdateAsync(item);
                    }
                }
                if (assetTransfer.tSAllocationEvictionAssets != null && assetTransfer.tSAllocationEvictionAssets.Any())
                {
                    foreach (var item in assetTransfer.tSAllocationEvictionAssets)
                    {
                        if (item.ID <= 0)
                            await _tSAllocationEvictionRepo.CreateAsync(item);
                        else
                            await _tSAllocationEvictionRepo.UpdateAsync(item);
                    }
                }
                if (assetTransfer.tSTranferAsset != null)
                {
                    if (assetTransfer.tSTranferAsset.ID <= 0)
                        await _tSAssetTransferRepo.CreateAsync(assetTransfer.tSTranferAsset);
                    else
                        await _tSAssetTransferRepo.UpdateAsync(assetTransfer.tSTranferAsset);
                }
                if (assetTransfer.tSTranferAssetDetails != null && assetTransfer.tSTranferAssetDetails.Any())
                {
                    foreach (var item in assetTransfer.tSTranferAssetDetails)
                    {
                        item.TSTranferAssetID = assetTransfer.tSTranferAsset.ID;
                        if (item.ID <= 0)
                            await _tSAssetTransferDetailRepo.CreateAsync(item);
                        else
                            await _tSAssetTransferDetailRepo.UpdateAsync(item);
                    }
                }

                return Ok(new { status = 1, message = "Lưu dữ liệu thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data-personal")]
        public async Task<IActionResult> SaveDataPersonal([FromBody] AssetTranferFullDTO assetTransfer)
        {
            try
            {
                if (assetTransfer == null)
                {
                    return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." });
                }
                if (assetTransfer.tSAssetManagements != null && assetTransfer.tSAssetManagements.Any())
                {
                    foreach (var item in assetTransfer.tSAssetManagements)
                    {
                        if (item.ID <= 0)
                            await _tsAssetManagementRepo.CreateAsync(item);
                        else
                            await _tsAssetManagementRepo.UpdateAsync(item);
                    }
                }
                if (assetTransfer.tSAllocationEvictionAssets != null && assetTransfer.tSAllocationEvictionAssets.Any())
                {
                    foreach (var item in assetTransfer.tSAllocationEvictionAssets)
                    {
                        if (item.ID <= 0)
                            await _tSAllocationEvictionRepo.CreateAsync(item);
                        else
                            await _tSAllocationEvictionRepo.UpdateAsync(item);
                    }
                }
                if (assetTransfer.tSTranferAsset != null)
                {
                    if (assetTransfer.tSTranferAsset.ID <= 0)
                        await _tSAssetTransferRepo.CreateAsync(assetTransfer.tSTranferAsset);
                    else
                        await _tSAssetTransferRepo.UpdateAsync(assetTransfer.tSTranferAsset);
                }
                if (assetTransfer.tSTranferAssetDetails != null && assetTransfer.tSTranferAssetDetails.Any())
                {
                    foreach (var item in assetTransfer.tSTranferAssetDetails)
                    {
                        item.TSTranferAssetID = assetTransfer.tSTranferAsset.ID;
                        if (item.ID <= 0)
                            await _tSAssetTransferDetailRepo.CreateAsync(item);
                        else
                            await _tSAssetTransferDetailRepo.UpdateAsync(item);
                    }
                }

                return Ok(new { status = 1, message = "Lưu dữ liệu thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N67")]
        [HttpPost("save-data-kt")]
        public async Task<IActionResult> SaveDataKT([FromBody] AssetTranferFullDTO assetTransfer)
        {
            try
            {
                if (assetTransfer == null)
                {
                    return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." });
                }
                if (assetTransfer.tSAssetManagements != null && assetTransfer.tSAssetManagements.Any())
                {
                    foreach (var item in assetTransfer.tSAssetManagements)
                    {
                        if (item.ID <= 0)
                            await _tsAssetManagementRepo.CreateAsync(item);
                        else
                            await _tsAssetManagementRepo.UpdateAsync(item);
                    }
                }
                if (assetTransfer.tSAllocationEvictionAssets != null && assetTransfer.tSAllocationEvictionAssets.Any())
                {
                    foreach (var item in assetTransfer.tSAllocationEvictionAssets)
                    {
                        if (item.ID <= 0)
                            await _tSAllocationEvictionRepo.CreateAsync(item);
                        else
                            await _tSAllocationEvictionRepo.UpdateAsync(item);
                    }
                }
                if (assetTransfer.tSTranferAsset != null)
                {
                    if (assetTransfer.tSTranferAsset.ID <= 0)
                        await _tSAssetTransferRepo.CreateAsync(assetTransfer.tSTranferAsset);
                    else
                        await _tSAssetTransferRepo.UpdateAsync(assetTransfer.tSTranferAsset);
                }
                if (assetTransfer.tSTranferAssetDetails != null && assetTransfer.tSTranferAssetDetails.Any())
                {
                    foreach (var item in assetTransfer.tSTranferAssetDetails)
                    {
                        item.TSTranferAssetID = assetTransfer.tSTranferAsset.ID;
                        if (item.ID <= 0)
                            await _tSAssetTransferDetailRepo.CreateAsync(item);
                        else
                            await _tSAssetTransferDetailRepo.UpdateAsync(item);
                    }
                }

                return Ok(new { status = 1, message = "Lưu dữ liệu thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
