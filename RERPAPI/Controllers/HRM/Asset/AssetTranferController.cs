using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.Asset;
using RERPAPI.Model.Param.Asset;
using RERPAPI.Repo.GenericEntity.Asset;
using RERPAPI.Model.Entities;
using OfficeOpenXml;

namespace RERPAPI.Controllers.Old.Asset
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetTranferController : ControllerBase
    {
        private readonly TSAssetTransferRepo _tSAssetTransferRepo;
        private readonly TSAllocationEvictionAssetRepo _tSAllocationEvictionRepo;
        private readonly TSAssetManagementRepo _tsAssetManagementRepo;
        private readonly TSAssetTransferDetailRepo _tSAssetTransferDetailRepo;
        TSTranferAsset tSTranferAsset = new TSTranferAsset();
        TSTranferAssetDetail tranferAssetDetail = new TSTranferAssetDetail();
        [HttpPost("get-asset-tranfer")]
        public IActionResult GetListAssetTransfer([FromBody] AssetTransferRequestParam request)
        {
            try
            {
                var assetTranfer = SQLHelper<dynamic>.ProcedureToList("spGetTranferAssetMaster",
                   new string[] { "@DateStart", "@DateEnd", "@IsApproved", "@DeliverID", "@ReceiverID", "TextFilter", "@PageSize", "@PageNumber" },
                                            new object[] { request.DateStart, request.DateEnd, request.IsApproved, request.DeliverID, request.ReceiverID, request.TextFilter, request.PageSize, request.PageNumber });
                return Ok(new
                {
                    status = 1,

                    assetTranfer = SQLHelper<dynamic>.GetListData(assetTranfer, 0)

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
                return Ok(new
                {

                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
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
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "BienBanBanGiao.xlsx");
                if (!System.IO.File.Exists(templatePath))
                    return NotFound("File mẫu không tồn tại.");

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

                int insertRow = 21;
                int templateRow = 19;

                for (int i = 0; i < details.Count; i++)
                {
                    var row = details[i];
                    ws.InsertRow(insertRow, 1);
                    for (int col = 1; col <= 8; col++)
                    {
                        ws.Cells[insertRow, col].StyleID = ws.Cells[templateRow, col].StyleID;
                    }
                    ws.Cells[insertRow, 1].Value = i + 1;
                    ws.Cells[insertRow, 2].Value = row.TSCodeNCC;
                    ws.Cells[insertRow, 3].Value = row.TSAssetName;
                    ws.Cells[insertRow, 5].Value = row.UnitName;
                    ws.Cells[insertRow, 6].Value = row.Quantity;
                    ws.Cells[insertRow, 7].Value = row.Status;
                    ws.Cells[insertRow, 8].Value = row.Note;

                    insertRow++;
                }
                ws.DeleteRow(insertRow);



                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                return File(stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 0,
                    message = "Lỗi khi xuất Excel.",
                    error = ex.Message
                });
            }
        }



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
                            _tsAssetManagementRepo.UpdateAsync( item);
                    }
                }
                if (assetTransfer.tSAllocationEvictionAssets != null && assetTransfer.tSAllocationEvictionAssets.Any())
                {
                    foreach (var item in assetTransfer.tSAllocationEvictionAssets)
                    {
                        if (item.ID <= 0)
                            await _tSAllocationEvictionRepo.CreateAsync(item);
                        else
                            _tSAllocationEvictionRepo.UpdateAsync( item);
                    }
                }
                if (assetTransfer.tSTranferAsset != null)
                {
                    if (assetTransfer.tSTranferAsset.ID <= 0)
                        await _tSAssetTransferRepo.CreateAsync(assetTransfer.tSTranferAsset);
                    else
                        _tSAssetTransferRepo.UpdateAsync( assetTransfer.tSTranferAsset);
                }
                if (assetTransfer.tSTranferAssetDetails != null && assetTransfer.tSTranferAssetDetails.Any())
                {
                    foreach (var item in assetTransfer.tSTranferAssetDetails)
                    {
                        item.TSTranferAssetID = assetTransfer.tSTranferAsset.ID;
                        if (item.ID <= 0)
                            await _tSAssetTransferDetailRepo.CreateAsync(item);
                        else
                            _tSAssetTransferDetailRepo.UpdateAsync( item);
                    }
                }

                return Ok(new { status = 1, message = "Lưu dữ liệu thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 0,
                    message = "Lỗi xảy ra khi lưu dữ liệu.",
                    detail = ex.Message.ToString(),
                });
            }
        }
    }
}
