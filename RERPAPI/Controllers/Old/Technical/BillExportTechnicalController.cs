using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.Technical;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Technical;
using RTCApi.Repo.GenericRepo;

namespace RERPAPI.Controllers.Old.Technical
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillExportTechnicalController : ControllerBase
    {
        private readonly HistoryDeleteBillRepo _historyDeleteBillRepo;
        private readonly BillExportTechnicalRepo _billExportTechnicalRepo;
        private readonly BillExportDetailTechnicalRepo _billExportDetailTechnicalRepo;
        private readonly BillExportTechDetailSerialRepo _billExportTechDetailSerialRepo;
        private readonly InventoryDemoRepo _inventoryDemoRepo;
        private readonly HistoryProductRTCRepo _historyProductRTCRepo;
        private readonly ProductRTCQRCodeRepo _productRTCQRCodeRepo;
        public BillExportTechnicalController(ProductRTCQRCodeRepo productRTCQRCodeRepo, BillExportTechnicalRepo billExportTechnicalRepo, BillExportDetailTechnicalRepo billExportDetailTechnicalRepo, BillExportTechDetailSerialRepo billExportTechDetailSerialRepo, HistoryDeleteBillRepo historyDeleteBillRepo, HistoryProductRTCRepo historyProductRTCRepo, InventoryDemoRepo inventoryDemoRepo)
        {
            _productRTCQRCodeRepo = productRTCQRCodeRepo;
            _billExportTechnicalRepo = billExportTechnicalRepo;
            _billExportDetailTechnicalRepo = billExportDetailTechnicalRepo;
            _billExportTechDetailSerialRepo = billExportTechDetailSerialRepo;
            _historyDeleteBillRepo = historyDeleteBillRepo;
            _historyProductRTCRepo = historyProductRTCRepo;
            _inventoryDemoRepo = inventoryDemoRepo;
        }
        [HttpPost("get-bill-export-technical")]
        public async Task<ActionResult> GetBillExportTechnical([FromBody] BillExportTechnicalRequestParam request)
        {
            try
            {
                var billExportTechnical = SQLHelper<dynamic>.ProcedureToList(
                    "spGetBillExportTechnical",
                    new string[] { "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@Status", "@FilterText", "@WarehouseID" },
                    new object[] { request.Page, request.Size, request.DateStart, request.DateEnd, request.Status, request.FilterText, request.WarehouseID });

                return Ok(new
                {
                    status = 1,
                    billExportTechnical = SQLHelper<dynamic>.GetListData(billExportTechnical, 0),
                    TotalPage = SQLHelper<dynamic>.GetListData(billExportTechnical, 1)
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
        [HttpGet("get-bill-code")]
        public async Task<IActionResult> GenerateBillCode([FromQuery] int billtype)
        {
            string billCode = _billExportTechnicalRepo.GetBillCode(billtype);
            return Ok(new
            {
                status = 1,
                data = billCode
            });
        }
        [HttpGet("get-serialbyID")]
        public IActionResult GetSerialByID(int id)
        {
            try
            {
                var serial = _billExportTechDetailSerialRepo.GetByID(id);
                return Ok(new
                {
                    status = 1,
                    data = serial
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpGet("get-bill-export-by-code")]
        public IActionResult GetBillExportByCode(string billCode)
        {
            try
            {
                List<BillExportTechnical> masterBillImports = _billExportTechnicalRepo.GetAll(x => x.Code == billCode);
                var importID = masterBillImports[0].ID;
                var billDetail = SQLHelper<dynamic>.ProcedureToList("spGetBillExportTechDetail_New", new string[] { "@ID" }, new object[] { importID });
                return Ok(new
                {
                    status = 1,
                    master = masterBillImports,
                    detail = SQLHelper<dynamic>.GetListData(billDetail, 0),
                });
            }

            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpGet("get-bill-export-technical-detail")]
        public IActionResult GetbillImportTechnicalDetail(string? ID)
        {
            try
            {
                var billDetail = SQLHelper<dynamic>.ProcedureToList("spGetBillExportTechDetail_New", new string[] { "@ID" }, new object[] { ID });
                return Ok(new
                {
                    status = 1,
                    billDetail = SQLHelper<dynamic>.GetListData(billDetail, 0)

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
        [HttpPost("export-bill-export-technical")]
        public IActionResult ExportBillImportTechnical([FromBody] BillExportTechnicallExcelFullDTO dto)
        {
            try
            {
                var master = dto.Master;
                var details = dto.Details;

                if (master == null || details == null || details.Count == 0)
                    return BadRequest("Dữ liệu phiếu nhập kỹ thuật không hợp lệ.");
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "BillExportTechnical.xlsx");
                if (!System.IO.File.Exists(templatePath))
                    return NotFound("File mẫu không tồn tại.");

                string fileName = $"PNKT_{master.Code}_{DateTime.Now:ddMMyyyy_HHmmss}.xlsx";
                using var package = new ExcelPackage(new FileInfo(templatePath));
                var ws = package.Workbook.Worksheets[0];

                // Dữ liệu chung
                DateTime createDate = master.CreatedDate ?? DateTime.Now;
                string locationDate = $"Hà Nội, Ngày {createDate.Day} tháng {createDate.Month} năm {createDate.Year}";
                string suplier = master.SupplierName?.Trim() ?? "";
                string customer = master.CustomerName?.Trim() ?? "";
                string deliver = master.Deliver?.Trim() ?? "";
                string receiver = master.Receiver?.Trim() ?? "";
                string dept = master.DepartmentName?.Trim() ?? "";
                // Ghi dữ liệu vào file Excel
                ws.Cells[16, 2].Value = locationDate;
                ws.Cells[18, 4].Value = $"Số: {master.Code}";
                ws.Cells[19, 3].Value = string.IsNullOrEmpty(suplier) ? customer : suplier;
                ws.Cells[20, 7].Value = receiver;
                ws.Cells[35, 5].Value = deliver;
                // Ghi chi tiết
                int insertRow = 27;
                int templateRow = 28;

                for (int i = 0; i < details.Count; i++)
                {
                    var row = details[i];
                    ws.InsertRow(insertRow, 1);
                    for (int col = 1; col <= 10; col++)
                    {
                        ws.Cells[insertRow, col].StyleID = ws.Cells[templateRow, col].StyleID;
                    }

                    ws.Cells[insertRow, 2].Value = i + 1;
                    ws.Cells[insertRow, 3].Value = row.ProductCode;
                    ws.Cells[insertRow, 4].Value = row.ProductName;
                    ws.Cells[insertRow, 5].Value = row.Quantity;
                    ws.Cells[insertRow, 6].Value = row.UnitName;
                    ws.Cells[insertRow, 7].Value = row.Maker;
                    ws.Cells[insertRow, 8].Value = row.WarehouseType;
                    ws.Cells[insertRow, 9].Value = row.ProductCodeRTC;
                    ws.Cells[insertRow, 10].Value = row.Note;
                    insertRow++;
                }
                ws.DeleteRow(templateRow);

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                return File(stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] BillExportTechnicalFullDTO product)
        {
            try
            {
                if (product == null)
                {
                    return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." });
                }
                // Lưu lịch sử xóa
                if (product.historyDeleteBill != null)
                {
                    if (product.historyDeleteBill.ID <= 0)
                        await _historyDeleteBillRepo.CreateAsync(product.historyDeleteBill);
                    else
                        _historyDeleteBillRepo.UpdateAsync(product.historyDeleteBill);
                }

                // Lưu phiếu xuất
                if (product.billExportTechnical != null)
                {
                    if (product.billExportTechnical.ID <= 0)
                        await _billExportTechnicalRepo.CreateAsync(product.billExportTechnical);
                    else
                        _billExportTechnicalRepo.UpdateAsync(product.billExportTechnical);
                }

                // Map STT -> ID sau khi insert chi tiết phiếu
                Dictionary<int, int> sttToDetailIdMap = new();
                int? singleDetailId = null;

                if (product.billExportDetailTechnicals != null && product.billExportDetailTechnicals.Any())
                {
                    foreach (var item in product.billExportDetailTechnicals)
                    {
                        item.BillExportTechID = product.billExportTechnical.ID;

                        if (item.ID <= 0)
                        {
                            await _billExportDetailTechnicalRepo.CreateAsync(item);

                            if (product.billExportDetailTechnicals.Count == 1)
                                singleDetailId = item.ID;

                            if (item.STT.HasValue && item.STT.Value > 0 && !sttToDetailIdMap.ContainsKey(item.STT.Value))
                            {
                                sttToDetailIdMap[item.STT.Value] = item.ID;
                            }
                        }
                        else
                        {
                            _billExportDetailTechnicalRepo.UpdateAsync(item);

                            if (product.billExportDetailTechnicals.Count == 1)
                                singleDetailId = item.ID;
                        }
                    }
                }

                List<BillExportTechDetailSerial> savedSerials = new();

                if (product.billExportTechDetailSerials != null && product.billExportTechDetailSerials.Any())
                {
                    foreach (var item in product.billExportTechDetailSerials)
                    {
                        // Nếu chỉ có 1 detail, gán trực tiếp
                        if (singleDetailId.HasValue)
                        {
                            item.BillExportTechDetailID = singleDetailId.Value;
                        }
                        // Nếu nhiều detail, gán theo STT
                        else if (item.STT.HasValue && item.STT.Value > 0 && sttToDetailIdMap.TryGetValue(item.STT.Value, out int detailId))
                        {
                            item.BillExportTechDetailID = detailId;
                        }

                        if (item.ID <= 0)
                        {
                            await _billExportTechDetailSerialRepo.CreateAsync(item);
                        }
                        else
                        {
                            _billExportTechDetailSerialRepo.UpdateAsync(item);
                        }

                        savedSerials.Add(item);
                    }
                }
                if (product.inentoryDemos != null && product.inentoryDemos.Any())
                {
                    foreach (var item in product.inentoryDemos)
                    {

                        if (item.ID <= 0)
                            await _inventoryDemoRepo.CreateAsync(item);
                        else
                            _inventoryDemoRepo.UpdateAsync(item);
                    }
                }
                if (product.historyProductRTCs != null && product.historyProductRTCs.Any())
                {

                    foreach (var item in product.historyProductRTCs)
                    {
                        if (item.ID <= 0)
                            await _historyProductRTCRepo.CreateAsync(item);
                        else
                            _historyProductRTCRepo.UpdateAsync(item);
                    }
                }

                return Ok(new
                {
                    status = 1,
                    data = savedSerials
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

    }
}
