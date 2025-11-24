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
    public class BillImportTechnicalController : ControllerBase
    {
        private readonly HistoryDeleteBillRepo _historyDeleteBillRepo;
        private readonly BillImportTechnicalRepo _billImportTechnicalRepo;
        private readonly BillImportTechnicalDetailRepo _billImportTechnicalDetailRepo;
        private readonly BillImportTechDetailSerialRepo _billImportTechDetailSerialRepo;
        private readonly RulePayRepo _rulePayRepo;
        public BillImportTechnicalController(HistoryDeleteBillRepo historyDeleteBillRepo, BillImportTechnicalRepo billImportTechnicalRepo, BillImportTechnicalDetailRepo billImportTechnicalDetailRepo, BillImportTechDetailSerialRepo billImportTechDetailSerialRepo, RulePayRepo rulePayRepo)
        {
            _historyDeleteBillRepo = historyDeleteBillRepo;
            _billImportTechnicalRepo = billImportTechnicalRepo;
            _billImportTechnicalDetailRepo = billImportTechnicalDetailRepo;
            _billImportTechDetailSerialRepo = billImportTechDetailSerialRepo;
            _rulePayRepo = rulePayRepo;
        }
        [HttpGet("get-rulepay")]
        public IActionResult GetRulepay()
        {
            try
            {
                List<RulePay> rulepays = _rulePayRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = rulepays
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
        [HttpGet("get-bill-import-by-code")]
        public IActionResult GetBillImportByCode(string billCode)
        {
            try
            {
                List<BillImportTechnical> masterBillImports = _billImportTechnicalRepo.GetAll(x => x.BillCode == billCode);
                var importID = masterBillImports[0].ID;
                var billDetail = SQLHelper<dynamic>.ProcedureToList("spGetBillImportDetailTechnical", new string[] { "@ID" }, new object[] { importID });
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
        [HttpGet("get-serialbyID")]
        public IActionResult GetSerialByID(int id)
        {
            try
            {
                var serial = _billImportTechDetailSerialRepo.GetByID(id);
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
        [HttpPost("get-bill-import-technical")]
        public async Task<ActionResult> GetBillImportTechnical([FromBody] BillImportTechnicalRequestParam request)
        {
            try
            {
                var billImportTechnical = SQLHelper<dynamic>.ProcedureToList(
                    "spGetBillImportTechnical",
                    new string[] { "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@Status", "@FilterText", "@WarehouseID" },
                    new object[] { request.Page, request.Size, request.DateStart, request.DateEnd, request.Status, request.FilterText, request.WarehouseID });

                return Ok(new
                {
                    status = 1,
                    billImportTechnical = SQLHelper<dynamic>.GetListData(billImportTechnical, 0),
                    TotalPage = SQLHelper<dynamic>.GetListData(billImportTechnical, 1)
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
        [HttpGet("get-bill-import-technical-detail")]
        public IActionResult GetbillImportTechnicalDetail(string? ID)
        {
            try
            {
                var billDetail = SQLHelper<dynamic>.ProcedureToList("spGetBillImportDetailTechnical", new string[] { "@ID" }, new object[] { ID });
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
        [HttpGet("get-bill-code")]
        public async Task<IActionResult> GenerateBillCode([FromQuery] int billtype)
        {
            string billCode = _billImportTechnicalRepo.GetBillCode(billtype);
            return Ok(new
            {
                status = 1,
                data = billCode
            });
        }
        [HttpGet("get-user")]
        public IActionResult GetUser()
        {
            try
            {
                var dt = SQLHelper<dynamic>.ProcedureToList("spGetUsersHistoryProductRTC", ["@UserID"], [0]);
                var data = SQLHelper<dynamic>.GetListData(dt, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-document-bill-import")]
        public IActionResult GetDocumentBillImport(int poNCCId, int billImportID)
        {
            try
            {
                var document = SQLHelper<dynamic>.ProcedureToList("spGetAllDocumentImportByPONCCID", new string[] { "@PONCCID", "@BillImportID" }, new object[] { poNCCId, billImportID });
                return Ok(new
                {
                    status = 1,
                    document = SQLHelper<dynamic>.GetListData(document, 0)

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

        //[HttpPost("save-data")]
        //public async Task<IActionResult> SaveData([FromBody] BillimporttechnicalFullDTO product)
        //{
        //    try
        //    {
        //        if (product == null)
        //        {
        //            return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." });
        //        }

        //        if (product.historyDeleteBill != null)
        //        {
        //            if (product.historyDeleteBill.ID <= 0)
        //                await _historyDeleteBillRepo.CreateAsync(product.historyDeleteBill);
        //            else
        //                _historyDeleteBillRepo.UpdateFieldsByID(product.historyDeleteBill.ID, product.historyDeleteBill);
        //        }

        //        if (product.billImportTechnical != null)
        //        {
        //            if (product.billImportTechnical.ID <= 0)
        //                await _billImportTechnicalRepo.CreateAsync(product.billImportTechnical);
        //            else
        //                _billImportTechnicalRepo.UpdateFieldsByID(product.billImportTechnical.ID, product.billImportTechnical);
        //        }

        //        if (product.billImportDetailTechnicals != null && product.billImportDetailTechnicals.Any())
        //        {
        //            foreach (var item in product.billImportDetailTechnicals)
        //            {
        //                item.BillImportTechID = product.billImportTechnical.ID;

        //                if (item.ID <= 0)
        //                {
        //                    await _billImportTechnicalDetailRepo.CreateAsync(item);
        //                    string serial = "10,11,12,13";
        //                    string[] data = serial.Split(',');
        //                    foreach(string i in data)
        //                    {
        //                        BillImportTechDetailSerial model = _billImportTechDetailSerialRepo.GetByID(int.Parse(i));

        //                    }
        //                }
        //                else
        //                    _billImportTechnicalDetailRepo.UpdateFieldsByID(item.ID, item);
        //            }
        //        }
        //        List<BillImportTechDetailSerial> savedSerials = new();

        //        if (product.billImportTechDetailSerials != null && product.billImportTechDetailSerials.Any())
        //        {
        //            foreach (var item in product.billImportTechDetailSerials)
        //            {
        //                if (item.ID <= 0)
        //                {

        //                    var id = await _billImportTechDetailSerialRepo.CreateAsync(item);
        //                }
        //                else
        //                {
        //                    _billImportTechDetailSerialRepo.UpdateFieldsByID(item.ID, item);
        //                }
        //                savedSerials.Add(item);
        //            }
        //        }

        //        return Ok(new
        //        {
        //            status = 1,
        //            data = savedSerials 
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new
        //        {
        //            status = 0,
        //            message = ex.Message,
        //            error = ex.ToString()
        //        });
        //    }
        //}
        [HttpPost("export-bill-import-technical")]
        public IActionResult ExportBillImportTechnical([FromBody] BillExportTechnicallExcelFullDTO dto)
        {
            try
            {
                var master = dto.Master;
                var details = dto.Details;

                if (master == null || details == null || details.Count == 0)
                    return BadRequest("Dữ liệu phiếu nhập kỹ thuật không hợp lệ.");

                //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage.License.SetNonCommercialOrganization("RTC Technology Viet Nam");
                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "BillImportTechnicalNew.xlsx");

                if (!System.IO.File.Exists(templatePath))
                    return NotFound("File mẫu không tồn tại.");

                string fileName = $"PNKT_{master.Code}_{DateTime.Now:ddMMyyyy_HHmmss}.xlsx";
                using var package = new ExcelPackage(new FileInfo(templatePath));
                var ws = package.Workbook.Worksheets[0];

                // --- DỮ LIỆU MASTER ---
                DateTime createDate = master.CreatedDate ?? DateTime.Now;
                string locationDate = $"Hà Nội, Ngày {createDate.Day} tháng {createDate.Month} năm {createDate.Year}";
                string supplier = master.SupplierName?.Trim() ?? "";
                string customer = master.CustomerName?.Trim() ?? "";
                string deliver = master.Deliver?.Trim() ?? "";
                string receiver = master.Receiver?.Trim() ?? "";
                string dept = master.DepartmentName?.Trim() ?? "";
                string address = master.Addres?.Trim() ?? "";

                ws.Cells[14, 2].Value = locationDate;
                ws.Cells[15, 2].Value = $"Số: {master.Code}";
                ws.Cells[16, 3].Value = string.IsNullOrEmpty(supplier) ? customer : supplier;
                ws.Cells[17, 3].Value = string.IsNullOrEmpty(dept) ? deliver : $"{deliver} / Phòng {dept}";
                ws.Cells[16, 7].Value = receiver;
                ws.Cells[20, 3].Value = address;
                ws.Cells[35, 5].Value = deliver;
                ws.Cells[35, 10].Value = receiver;

                // --- CHÈN ẢNH NẾU CÓ ---
                //if (!string.IsNullOrEmpty(master.Image) && System.IO.File.Exists(master.Image))
                //{
                //    var image = Image.FromFile(master.Image);
                //    var picture = ws.Drawings.AddPicture("DeviceImage", image);
                //    picture.SetPosition(26, 100, 1, 100); // chỉnh vị trí nếu cần
                //    picture.SetSize(200, 100);
                //}

                //// --- CHÈN QR CODE ---
                //using (var qrGen = new QRCodeGenerator())
                //using (var qrData = qrGen.CreateQrCode(master.Code, QRCodeGenerator.ECCLevel.Q))
                //using (var qrCode = new QRCode(qrData))
                //using (var qrBitmap = qrCode.GetGraphic(20))
                //using (var qrStream = new MemoryStream())
                //{
                //    qrBitmap.Save(qrStream, ImageFormat.Png);
                //    qrStream.Position = 0;
                //    var qrPicture = ws.Drawings.AddPicture("QRCode", qrStream, ePictureType.Png);
                //    qrPicture.SetPosition(0, 0, 6, 0); // chỉnh vị trí nếu cần
                //    qrPicture.SetSize(90, 90);
                //}

                // --- DỮ LIỆU CHI TIẾT ---
                int insertRow = 24;
                int templateRow = 23;

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
                return StatusCode(500, new
                {
                    status = 0,
                    message = "Lỗi khi xuất phiếu nhập kỹ thuật.",
                    error = ex.Message
                });
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] BillimporttechnicalFullDTO product)
        {
            try
            {
                if (product == null)
                {
                    return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." });
                }

                // Lưu lịch sử xóa nếu có
                if (product.historyDeleteBill != null)
                {
                    if (product.historyDeleteBill.ID <= 0)
                        await _historyDeleteBillRepo.CreateAsync(product.historyDeleteBill);
                    else
                        _historyDeleteBillRepo.UpdateAsync(product.historyDeleteBill);
                }

                // Lưu phiếu nhập
                if (product.billImportTechnical != null)
                {
                    if (product.billImportTechnical.ID <= 0)
                        await _billImportTechnicalRepo.CreateAsync(product.billImportTechnical);
                    else
                        _billImportTechnicalRepo.UpdateAsync(product.billImportTechnical);
                }

                // Map STT -> ID sau khi insert chi tiết phiếu
                Dictionary<int, int> sttToDetailIdMap = new();
                int? singleDetailId = null;

                if (product.billImportDetailTechnicals != null && product.billImportDetailTechnicals.Any())
                {
                    foreach (var item in product.billImportDetailTechnicals)
                    {
                        item.BillImportTechID = product.billImportTechnical.ID;

                        if (item.ID <= 0)
                        {
                            await _billImportTechnicalDetailRepo.CreateAsync(item);

                            if (product.billImportDetailTechnicals.Count == 1)
                                singleDetailId = item.ID;

                            if (item.STT.HasValue && item.STT.Value > 0 && !sttToDetailIdMap.ContainsKey(item.STT.Value))
                            {
                                sttToDetailIdMap[item.STT.Value] = item.ID;
                            }
                        }
                        else
                        {
                            _billImportTechnicalDetailRepo.UpdateAsync(item);

                            if (product.billImportDetailTechnicals.Count == 1)
                                singleDetailId = item.ID;
                        }
                    }
                }
                List<BillImportTechDetailSerial> savedSerials = new();

                if (product.billImportTechDetailSerials != null && product.billImportTechDetailSerials.Any())
                {
                    foreach (var item in product.billImportTechDetailSerials)
                    {
                        // Nếu chỉ có 1 detail, gán trực tiếp
                        if (singleDetailId.HasValue)
                        {
                            item.BillImportTechDetailID = singleDetailId.Value;
                        }
                        // Nếu nhiều detail, gán theo STT
                        else if (item.STT.HasValue && item.STT.Value > 0 && sttToDetailIdMap.TryGetValue(item.STT.Value, out int detailId))
                        {
                            item.BillImportTechDetailID = detailId;
                        }

                        if (item.ID <= 0)
                        {
                            await _billImportTechDetailSerialRepo.CreateAsync(item); // item.ID sẽ tự cập nhật
                        }
                        else
                        {
                            _billImportTechDetailSerialRepo.UpdateAsync(item);
                        }

                        savedSerials.Add(item);
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
