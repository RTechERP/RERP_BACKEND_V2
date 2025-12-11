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
        private readonly IConfiguration _configuration;
        private readonly BillImportTechnicalLogRepo _billImportTechnicalLogRepo;
        private readonly BillDocumentImportTechnicalRepo _billDocumentImportTechnicalRepo;
        private readonly BillDocumentImportTechnicalLogRepo _billDocumentImportTechnicalLogRepo;
        private readonly InventoryDemoRepo _inventoryDemoRepo;
        private readonly PONCCRepo _pONCCRepo;
        public BillImportTechnicalController(HistoryDeleteBillRepo historyDeleteBillRepo, BillImportTechnicalRepo billImportTechnicalRepo, BillImportTechnicalDetailRepo billImportTechnicalDetailRepo, BillImportTechDetailSerialRepo billImportTechDetailSerialRepo, RulePayRepo rulePayRepo, IConfiguration configuration, BillImportTechnicalLogRepo billImportTechnicalLogRepo, BillDocumentImportTechnicalRepo billDocumentImportTechnicalRepo, BillDocumentImportTechnicalLogRepo billDocumentImportTechnicalLogRepo, InventoryDemoRepo inventoryDemoRepo, PONCCRepo pONCCRepo)
        {
            _historyDeleteBillRepo = historyDeleteBillRepo;
            _billImportTechnicalRepo = billImportTechnicalRepo;
            _billImportTechnicalDetailRepo = billImportTechnicalDetailRepo;
            _billImportTechDetailSerialRepo = billImportTechDetailSerialRepo;
            _rulePayRepo = rulePayRepo;
            _configuration = configuration;
            _billImportTechnicalLogRepo = billImportTechnicalLogRepo;
            _billDocumentImportTechnicalRepo = billDocumentImportTechnicalRepo;
            _billDocumentImportTechnicalLogRepo = billDocumentImportTechnicalLogRepo;
            _inventoryDemoRepo = inventoryDemoRepo;
            _pONCCRepo = pONCCRepo;
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
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("get-bill-import-technical")]
        public async Task<ActionResult> GetBillImportTechnical([FromBody] BillImportTechnicalRequestParam request)
        {
            try
            {
                // Chuẩn hóa thời gian
                DateTime? dateStart = request.DateStart?.Date; // 00:00:00
                DateTime? dateEnd = request.DateEnd?.Date.AddDays(1).AddSeconds(-1); // 23:59:59

                var billImportTechnical = SQLHelper<dynamic>.ProcedureToList(
                    "spGetBillImportTechnical",
                    new string[] { "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@Status", "@FilterText", "@WarehouseID", "@BillType" },
                    new object[] { request.Page, request.Size, dateStart, dateEnd, request.Status, request.FilterText, request.WarehouseID, request.BillType });

                return Ok(new
                {
                    status = 1,
                    billImportTechnical = SQLHelper<dynamic>.GetListData(billImportTechnical, 0),
                    TotalPage = SQLHelper<dynamic>.GetListData(billImportTechnical, 1)
                });

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
        [HttpGet("load-product")]
        public IActionResult LoadProduct([FromQuery] int status, [FromQuery] int warehouseID, int warehouseType)
        {
            try
            {
                List<List<object>> dtProduct;

                if (status == 1)
                {
                    dtProduct = SQLHelper<dynamic>.ProcedureToList(
                        "spGetProductRTC",
                        new string[] { "@ProductGroupID", "@Keyword", "@CheckAll", "@WarehouseID", "@WarehouseType" },
                        new object[] { 0, "", 1, warehouseID, warehouseType }
                    );
                }
                else if (warehouseID == 1)
                {
                    dtProduct = SQLHelper<dynamic>.ProcedureToList(
                        "spGetProductRTCQRCode",
                        new string[] { "@WarehouseID" },
                        new object[] { warehouseID }
                    );
                }
                else
                {
                    dtProduct = SQLHelper<dynamic>.ProcedureToList(
                        "spGetInventoryDemo",
                        new string[] { "@ProductGroupID", "@Keyword", "@CheckAll", "@WarehouseID" },
                        new object[] { 0, "", 0, warehouseID }
                    );
                }

                var data = SQLHelper<dynamic>.GetListData(dtProduct, 0);

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(null, "Error: " + ex.Message));
            }
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
        public IActionResult GetDocumentBillImport(int billImportID)
        {
            try
            {
                var document = SQLHelper<dynamic>.ProcedureToList("spGetAllDocumentImportPONCCTechnical", new string[] { "@BillImportTechnicalID" }, new object[] { billImportID });
                return Ok(new
                {
                    status = 1,
                    document = SQLHelper<dynamic>.GetListData(document, 0)

                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("export-bill-import-technical")]
        public IActionResult ExportBillImportTechnical([FromBody] BillExportTechnicallExcelFullDTO dto)
        {
            try
            {
                var master = dto.Master;
                var details = dto.Details;

                if (master == null || details == null || details.Count == 0)
                    return BadRequest("Dữ liệu phiếu nhập Demo không hợp lệ.");

                ExcelPackage.License.SetNonCommercialOrganization("RTC Technology Viet Nam");
                //string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "BillImportTechnicalNew.xlsx");
                string path = _configuration.GetValue<string>("PathTemplate") ?? "";
                if (string.IsNullOrWhiteSpace(path)) return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy đường dẫn thư mục {path} trên server!"));
                string templatePath = Path.Combine(path, "ExportExcel", "BillImportTechnicalNew.xlsx");
                if (!System.IO.File.Exists(templatePath))
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy File mẫu!"));

                string fileName = $"PNKT_{master.Code}_{DateTime.Now:ddMMyyyy_HHmmss}.xlsx";
                using var package = new ExcelPackage(new FileInfo(templatePath));
                var ws = package.Workbook.Worksheets[0];

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
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
                        await _historyDeleteBillRepo.UpdateAsync(product.historyDeleteBill);
                }

                // Lưu phiếu nhập
                if (product.billImportTechnical != null)
                {
                    if (product.billImportTechnical.IsDeleted == true)
                    {
                        _billImportTechnicalRepo.Update(product.billImportTechnical);
                        var lst = _billImportTechnicalDetailRepo.GetAll(x => x.BillImportTechID == product.billImportTechnical.ID && x.IsDeleted == false);
                        foreach (var item in lst)
                        {
                            item.IsDeleted = true;
                            await _billImportTechnicalDetailRepo.UpdateAsync(item);
                        }
                        return Ok(ApiResponseFactory.Success(lst, "Cập nhật dữ liệu thành công!"));
                    }
                    if (product.billImportTechnical.ID <= 0)
                        await _billImportTechnicalRepo.CreateAsync(product.billImportTechnical);
                    else
                        await _billImportTechnicalRepo.UpdateAsync(product.billImportTechnical);
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
                            await _billImportTechnicalDetailRepo.UpdateAsync(item);

                            if (product.billImportDetailTechnicals.Count == 1)
                                singleDetailId = item.ID;
                        }
                        var exist = _inventoryDemoRepo.GetAll(x => x.ProductRTCID == item.ProductID && x.WarehouseID == item.WarehouseID).FirstOrDefault() ?? new Model.Entities.InventoryDemo();
                        if (exist.ID <= 0)
                        {
                            exist.ProductRTCID = item.ProductID;
                            exist.WarehouseID = item.WarehouseID;
                            await _inventoryDemoRepo.CreateAsync(exist);
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
                // Lưu documents (PONCC)
                List<BillDocumentImportTechnical> savedDocuments = new();
                if (product.documentImportPONCCs != null && product.documentImportPONCCs.Any())
                {
                    foreach (var document in product.documentImportPONCCs)
                    {
                        document.BillImportTechnicalID = product.billImportTechnical.ID;
                        document.UpdatedDate = DateTime.Now;
                        // document.UpdatedBy = User.Identity.Name; // hoặc lấy từ claims/context

                        if (document.ID <= 0)
                        {
                            await _billDocumentImportTechnicalRepo.CreateAsync(document);
                        }
                        else
                        {
                            _billDocumentImportTechnicalRepo.UpdateAsync(document);
                        }

                        // Tạo log cho document
                        var log = new BillDocumentImportTechnicalLog
                        {
                            BillDocumentImportTechnicalID = document.ID,
                            Status = document.Status,
                            LogDate = DateTime.Now, // hoặc document.DateReceive nếu có
                            Note = $"LÝ DO HUỶ: {document.ReasonCancel}\nGHI CHÚ: {document.Note}",
                            DocumentImportID = document.DocumentImportID
                        };

                        await _billDocumentImportTechnicalLogRepo.CreateAsync(log);
                        savedDocuments.Add(document);
                    }

                }
                if (product.PonccID > 0)
                {
                    PONCC po = _pONCCRepo.GetByID(product.PonccID ?? 0);
                    po.Status = 5;
                    await _pONCCRepo.UpdateAsync(po);
                }
                return Ok(new
                {
                    status = 1,
                    data = savedSerials
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("approve")]
        public async Task<IActionResult> Approved([FromBody] List<BillImportTechnical> bills)
        {
            if (bills == null || bills.Count == 0)
            {
                return BadRequest(ApiResponseFactory.Fail(null, "Danh sách phiếu cần duyệt trống."));
            }

            var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
            var currentUserInfo = ObjectMapper.GetCurrentUser(claims);
            int currentUserId = currentUserInfo.EmployeeID;

            int successCount = 0;

            try
            {
                foreach (var item in bills)
                {
                    var billInDb = await _billImportTechnicalRepo.GetByIDAsync(item.ID);

                    if (billInDb == null) continue;

                    if (billInDb.Status == true) continue;

                    if (!currentUserInfo.IsAdmin && billInDb.ApproverID != currentUserId) continue;

                    await _billImportTechnicalRepo.UpdateStatusAsync(billInDb, true);

                    successCount++;
                }

                var result = new
                {
                    SuccessCount = successCount,
                    TotalProcessed = bills.Count
                };

                if (successCount == 0)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Không có phiếu nào được duyệt.", result));
                }

                return Ok(ApiResponseFactory.Success(result, $"Duyệt thành công {successCount}/{bills.Count} phiếu!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("unapprove")]
        public async Task<IActionResult> Unapprove([FromBody] List<BillImportTechnical> bills)
        {
            if (bills == null || bills.Count == 0)
            {
                return BadRequest(ApiResponseFactory.Fail(null, "Danh sách phiếu cần hủy duyệt trống."));
            }

            var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
            var currentUserInfo = ObjectMapper.GetCurrentUser(claims);
            int currentUserId = currentUserInfo.EmployeeID;

            int successCount = 0;

            try
            {
                foreach (var item in bills)
                {
                    var billInDb = await _billImportTechnicalRepo.GetByIDAsync(item.ID);

                    if (billInDb == null)
                    {
                        continue;
                    }
                    if (billInDb.Status != true)
                    {
                        continue;
                    }
                    if (!currentUserInfo.IsAdmin && billInDb.ApproverID != currentUserId)
                    {
                        continue;
                    }

                    await _billImportTechnicalRepo.UpdateStatusAsync(billInDb, false);

                    successCount++;
                }

                var result = new
                {
                    SuccessCount = successCount,
                    TotalProcessed = bills.Count,
                };

                if (successCount == 0)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Không có phiếu nào được hủy duyệt."));
                }

                return Ok(ApiResponseFactory.Success(result, $"Hủy duyệt thành công {successCount}/{bills.Count} phiếu!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
