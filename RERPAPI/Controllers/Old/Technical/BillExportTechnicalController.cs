using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using RERPAPI.Attributes;
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
        private readonly IConfiguration _configuration;
        private readonly ProjectRepo _projectRepo;
        private readonly BillExportTechnicalLogRepo _billExportTechnicalLogRepo;
        private readonly ProductRTCRepo _productRTCRepo;
        private readonly BillExportDetailSerialNumberModulaLocationRepo _billExportDetailSerialNumberModulaLocationRepo;
        public BillExportTechnicalController(ProductRTCQRCodeRepo productRTCQRCodeRepo, BillExportTechnicalRepo billExportTechnicalRepo, BillExportDetailTechnicalRepo billExportDetailTechnicalRepo, BillExportTechDetailSerialRepo billExportTechDetailSerialRepo, HistoryDeleteBillRepo historyDeleteBillRepo, HistoryProductRTCRepo historyProductRTCRepo, InventoryDemoRepo inventoryDemoRepo, IConfiguration configuration, ProjectRepo projectRepo, BillExportTechnicalLogRepo billExportTechnicalLogRepo, ProductRTCRepo productRTCRepo, BillExportDetailSerialNumberModulaLocationRepo billExportDetailSerialNumberModulaLocationRepo)
        {
            _productRTCQRCodeRepo = productRTCQRCodeRepo;
            _billExportTechnicalRepo = billExportTechnicalRepo;
            _billExportDetailTechnicalRepo = billExportDetailTechnicalRepo;
            _billExportTechDetailSerialRepo = billExportTechDetailSerialRepo;
            _historyDeleteBillRepo = historyDeleteBillRepo;
            _historyProductRTCRepo = historyProductRTCRepo;
            _inventoryDemoRepo = inventoryDemoRepo;
            _configuration = configuration;
            _projectRepo = projectRepo;
            _billExportTechnicalLogRepo = billExportTechnicalLogRepo;
            _productRTCRepo = productRTCRepo;
            _billExportDetailSerialNumberModulaLocationRepo = billExportDetailSerialNumberModulaLocationRepo;
        }
        [HttpPost("get-bill-export-technical")]
        //[RequiresPermission("N19,N18,N26,N36,N29,N50,N54,N1")]
        public ActionResult GetBillExportTechnical([FromBody] BillExportTechnicalRequestParam request)
        {
            try
            {
                DateTime? dateStart = request.DateStart?.Date;
                DateTime? dateEnd = request.DateEnd?.Date.AddDays(1).AddSeconds(-1);

                var billExportTechnical = SQLHelper<dynamic>.ProcedureToList(
                    "spGetBillExportTechnical",
                    new string[] { "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@Status", "@FilterText", "@WarehouseID", "@WarehouseTypeBill" },
                    new object[] { request.Page, request.Size, dateStart, dateEnd, request.Status, request.FilterText, request.WarehouseID, request.WarehouseTypeBill });

                return Ok(new
                {
                    status = 1,
                    billExportTechnical = SQLHelper<dynamic>.GetListData(billExportTechnical, 0),
                    TotalPage = SQLHelper<dynamic>.GetListData(billExportTechnical, 1)
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
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-bill-export-by-id")]
        public IActionResult GetBillExportByCode(int id)
        {
            try
            {
                var billxport = _billExportTechnicalRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(billxport, ""));
            }

            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-bill-export-by-code")]
        public IActionResult GetBillExportByCode(string billCode)
        {
            try
            {
                List<BillExportTechnical> masterBillImports = _billExportTechnicalRepo.GetAll(x => x.Code == billCode);
                if (masterBillImports.Count <= 0) return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy phiếu xuất có mã {billCode}!"));
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
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-all-project")]
        public IActionResult GetAllProject()
        {
            var rs = _projectRepo.GetAll(x => x.IsDeleted == false || x.IsDeleted == null);
            return Ok(ApiResponseFactory.Success(rs, ""));
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
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
                ExcelPackage.License.SetNonCommercialOrganization("RTC Technology VietNam");

                string pahtServer = _configuration.GetValue<string>("PathTemplate");
                string templatePath = Path.Combine(pahtServer, "ExportExcel", "BillExportTechnical.xlsx");
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
                int startRow = 27;
                int templateRow = 27;

                if (details.Count > 1)
                {
                    ws.InsertRow(startRow + 1, details.Count - 1);
                }

                for (int i = 0; i < details.Count; i++)
                {
                    int currentRow = startRow + i;
                    var row = details[i];
                    if (i > 0)
                    {
                        for (int col = 1; col <= 10; col++)
                        {
                            ws.Cells[currentRow, col].StyleID = ws.Cells[templateRow, col].StyleID;
                        }
                    }

                    ws.Cells[currentRow, 2].Value = i + 1;
                    ws.Cells[currentRow, 3].Value = row.ProductCode?.Trim() ?? "";
                    ws.Cells[currentRow, 4].Value = row.ProductName?.Trim() ?? "";
                    ws.Cells[currentRow, 5].Value = row.Quantity;
                    ws.Cells[currentRow, 6].Value = row.UnitName?.Trim() ?? "";
                    ws.Cells[currentRow, 7].Value = row.Maker?.Trim() ?? "";
                    ws.Cells[currentRow, 8].Value = row.WarehouseType?.Trim() ?? "";
                    ws.Cells[currentRow, 9].Value = row.ProductCodeRTC?.Trim() ?? "";
                    ws.Cells[currentRow, 10].Value = row.Note?.Trim() ?? "";
                }

                ws.DeleteRow(startRow + details.Count);

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
        //[HttpPost("save-export-data")]
        //public async Task<IActionResult> SaveExportData([FromBody] BillExportTechnicalFullDTO product)
        //{
        //    try
        //    {
        //        if (product == null || product.billExportTechnical == null)
        //        {
        //            return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." });
        //        }

        //        var header = product.billExportTechnical;

        //        // Check rỗng
        //        if (string.IsNullOrWhiteSpace(header.Code))
        //            return BadRequest(new { status = 0, message = "Xin hãy điền số phiếu." });

        //        if (string.IsNullOrWhiteSpace(header.Deliver)) // txtLienHe
        //            return BadRequest(new { status = 0, message = "Xin hãy chọn người liên hệ." });

        //        if (string.IsNullOrWhiteSpace(header.Receiver)) // txtNguoiNhan
        //            return BadRequest(new { status = 0, message = "Chưa có thông tin người nhận." });

        //        // Check Loại phiếu
        //        if (header.BillType < 0)
        //            return BadRequest(new { status = 0, message = "Xin hãy chọn loại phiếu." });

        //        if ((header.SupplierSaleID ?? 0) <= 0 && (header.CustomerID ?? 0) <= 0)
        //        {
        //            return BadRequest(new { status = 0, message = "Vui lòng chọn Nhà cung cấp hoặc Khách hàng!" });
        //        }

        //        if ((header.ApproverID ?? 0) <= 0)
        //            return BadRequest(new { status = 0, message = "Xin hãy chọn Người duyệt." });

        //        var duplicateItem = _billExportTechnicalRepo.GetAll(x =>
        //            x.Code == header.Code.Trim() &&
        //            x.ID != header.ID &&
        //            x.WarehouseID == header.WarehouseID
        //        ).FirstOrDefault();

        //        if (duplicateItem != null)
        //        {
        //            return BadRequest(new { status = 0, message = $"Số phiếu {header.Code} đã tồn tại trong kho này." });
        //        }
        //        if (product.historyDeleteBill != null)
        //        {
        //            if (product.historyDeleteBill.ID <= 0)
        //                await _historyDeleteBillRepo.CreateAsync(product.historyDeleteBill);
        //            else
        //                await _historyDeleteBillRepo.UpdateAsync(product.historyDeleteBill);
        //        }

        //        header.Code = header.Code.Trim(); // Trim code
        //        if (header.ID <= 0)
        //        {
        //            header.Status = 0; // Mặc định
        //            header.BillDocumentExportType = 2; // Gán mặc định theo logic cũ
        //            await _billExportTechnicalRepo.CreateAsync(header);
        //        }
        //        else
        //        {
        //            await _billExportTechnicalRepo.UpdateAsync(header);
        //        }

        //        Dictionary<int, int> sttToDetailIdMap = new();
        //        int? singleDetailId = null;

        //        if (product.billExportDetailTechnicals != null && product.billExportDetailTechnicals.Any())
        //        {
        //            foreach (var item in product.billExportDetailTechnicals)
        //            {
        //                item.BillExportTechID = header.ID;
        //                item.WarehouseID = header.WarehouseID; // Đồng bộ kho

        //                if (item.IsDeleted && item.ID > 0)
        //                {
        //                    var detailToDelete = _billExportDetailTechnicalRepo.GetByID(item.ID);
        //                    if (detailToDelete != null)
        //                    {
        //                        detailToDelete.IsDelete = true;
        //                        // detailToDelete.UpdatedDate = DateTime.Now; 
        //                        await _billExportDetailTechnicalRepo.UpdateAsync(detailToDelete);
        //                    }
        //                    continue; // Bỏ qua các bước bên dưới, chuyển sang item tiếp theo
        //                }

        //                // --- LOGIC THÊM / SỬA ---
        //                if (!item.IsDeleted)
        //                {
        //                    if (item.ID <= 0)
        //                    {
        //                        await _billExportDetailTechnicalRepo.CreateAsync(item);

        //                        if (product.billExportDetailTechnicals.Count(x => !x.IsDeleted) == 1)
        //                            singleDetailId = item.ID;

        //                        // Map STT
        //                        if (item.STT.HasValue && item.STT.Value > 0 && !sttToDetailIdMap.ContainsKey(item.STT.Value))
        //                        {
        //                            sttToDetailIdMap[item.STT.Value] = item.ID;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        await _billExportDetailTechnicalRepo.UpdateAsync(item);

        //                        if (product.billExportDetailTechnicals.Count(x => !x.IsDeleted) == 1)
        //                            singleDetailId = item.ID;
        //                    }

        //                    // Logic InventoryDemo (Cập nhật tồn kho ảo - Logic từ WinForms)
        //                    var existInv = _inventoryDemoRepo.GetAll(x => x.ProductRTCID == item.ProductID && x.WarehouseID == item.WarehouseID).FirstOrDefault();
        //                    if (existInv == null)
        //                    {
        //                        var newInv = new Model.Entities.InventoryDemo // Hoặc Entity tương ứng
        //                        {
        //                            ProductRTCID = item.ProductID,
        //                            WarehouseID = item.WarehouseID,
        //                            // NumberInStore logic nếu cần
        //                        };
        //                        await _inventoryDemoRepo.CreateAsync(newInv);
        //                    }
        //                }
        //            }
        //        }

        //        // Lưu Serial
        //        List<BillExportTechDetailSerial> savedSerials = new();
        //        if (product.billExportTechDetailSerials != null && product.billExportTechDetailSerials.Any())
        //        {
        //            foreach (var item in product.billExportTechDetailSerials)
        //            {
        //                // Map ID chi tiết vào Serial
        //                if (singleDetailId.HasValue)
        //                {
        //                    item.BillExportTechDetailID = singleDetailId.Value;
        //                }
        //                else if (item.STT.HasValue && item.STT.Value > 0 && sttToDetailIdMap.TryGetValue(item.STT.Value, out int detailId))
        //                {
        //                    item.BillExportTechDetailID = detailId;
        //                }

        //                if (item.ID <= 0)
        //                {
        //                    await _billExportTechDetailSerialRepo.CreateAsync(item);
        //                }
        //                else
        //                {
        //                    await _billExportTechDetailSerialRepo.UpdateAsync(item);
        //                }
        //                savedSerials.Add(item);

        //                // Logic cập nhật trạng thái QR Code (Thường là update bảng ProductRTCQRCode status = 3)
        //                // if (!string.IsNullOrEmpty(item.SerialNumber)) 
        //                //    await _productRTCQRCodeRepo.UpdateStatusAsync(item.SerialNumber, 3);
        //            }
        //        }

        //        // Lưu HistoryProductRTC (Logic phiếu mượn)
        //        if (product.historyProductRTCs != null && product.historyProductRTCs.Any())
        //        {
        //            foreach (var item in product.historyProductRTCs)
        //            {
        //                // Gán BillID nếu chưa có
        //                if (item.BillExportTechnicalID <= 0) item.BillExportTechnicalID = header.ID;

        //                if (item.ID <= 0)
        //                    await _historyProductRTCRepo.CreateAsync(item);
        //                else
        //                    await _historyProductRTCRepo.UpdateAsync(item);

        //                if (ProductRTCQRCodeID > 0)
        //                {
        //                    TextUtils.ExcuteProcedure(StoreProcedures.spUpdateStatusProductRTCQRCode,
        //                                                new string[] { "@ProductRTCQRCodeID", "@Status", "@ProductRTCQRCode" },
        //                                                new object[] { ProductRTCQRCodeID, 3, oHistoryModel.ProductRTCQRCode });
        //                }
        //        }

        //        return Ok(new
        //        {
        //            status = 1,
        //            data = savedSerials,
        //            headerID = header.ID // Trả về ID phiếu để client biết
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}
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
                        new string[] { "@WarehouseID", "@WarehouseType" },
                        new object[] { warehouseID, warehouseType }
                    );
                }
                else
                {
                    dtProduct = SQLHelper<dynamic>.ProcedureToList(
                        "spGetInventoryDemo",
                        new string[] { "@ProductGroupID", "@Keyword", "@CheckAll", "@WarehouseID", "@WarehouseType" },
                        new object[] { 0, "", 0, warehouseID, warehouseType }
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
        [HttpPost("save-data")]
        [RequiresPermission("N26,N1,N73,N80")]
        public async Task<IActionResult> SaveData([FromBody] BillExportTechnicalFullDTO product)
        {
            try
            {
                if (product == null)
                {
                    return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." });
                }
                //// Lưu lịch sử xóa
                //if (product.historyDeleteBill != null)
                //{
                //    if (product.historyDeleteBill.ID <= 0)
                //        await _historyDeleteBillRepo.CreateAsync(product.historyDeleteBill);
                //    else
                //        await _historyDeleteBillRepo.UpdateAsync(product.historyDeleteBill);
                //})
                // Lưu phiếu xuất
                if (product.billExportTechnical != null)
                {
                    if (product.billExportTechnical.IsDeleted == true)
                    {
                        await _billExportTechnicalRepo.UpdateAsync(product.billExportTechnical);
                        List<BillExportDetailTechnical> lst = _billExportDetailTechnicalRepo.GetAll(x => x.BillExportTechID == product.billExportTechnical.ID);
                        foreach (var item in lst)
                        {
                            item.IsDeleted = true;
                            await _billExportDetailTechnicalRepo.UpdateAsync(item);
                        }
                        return Ok(ApiResponseFactory.Success(product, "Xóa dữ liệu thành công"));
                    }
                    product.billExportTechnical.CheckAddHistoryProductRTC = product.billExportTechnical.BillType == 1;
                    if (product.billExportTechnical.ID <= 0)
                        await _billExportTechnicalRepo.CreateAsync(product.billExportTechnical);
                    else
                    {
                        var history = _historyProductRTCRepo.GetAll(x => x.BillExportTechnicalID == product.billExportTechnical.ID).FirstOrDefault();
                        if (history != null)
                        {
                            history.IsDelete = true;
                            _historyProductRTCRepo.Update(history);
                        }

                        await _billExportTechnicalRepo.UpdateAsync(product.billExportTechnical);
                    }
                }
                var existHistory = _historyProductRTCRepo.GetAll(x => x.IsDelete != true && x.BillExportTechnicalID == product.billExportTechnical.ID);
                foreach (var item in existHistory)
                {
                    item.IsDelete = true;
                }
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

                            //if (product.billExportDetailTechnicals.Count == 1)
                            singleDetailId = item.ID;

                            if (item.STT.HasValue && item.STT.Value > 0 && !sttToDetailIdMap.ContainsKey(item.STT.Value))
                            {
                                sttToDetailIdMap[item.STT.Value] = item.ID;
                            }
                        }
                        else
                        {
                            await _billExportDetailTechnicalRepo.UpdateAsync(item);

                            //if (product.billExportDetailTechnicals.Count == 1)
                            singleDetailId = item.ID;
                        }
                        var inventorydemo = _inventoryDemoRepo.GetAll(x => x.ProductRTCID == item.ProductID && (x.WarehouseID == item.WarehouseID || x.WarehouseID == product.billExportTechnical.WarehouseID)).FirstOrDefault() ?? new Model.Entities.InventoryDemo();
                        inventorydemo.ProductRTCID = item.ProductID;
                        inventorydemo.WarehouseID = product.billExportTechnical.WarehouseID;

                        if (inventorydemo.ID <= 0) await _inventoryDemoRepo.CreateAsync(inventorydemo);
                        else await _inventoryDemoRepo.UpdateAsync(inventorydemo);

                        List<BillExportTechDetailSerial> savedSerials = new();

                        if (product.billExportTechDetailSerials != null && product.billExportTechDetailSerials.Any())
                        {
                            foreach (var serial in product.billExportTechDetailSerials)
                            {
                                // Nếu chỉ có 1 detail, gán trực tiếp
                                if (singleDetailId.HasValue)
                                {
                                    serial.BillExportTechDetailID = singleDetailId.Value;
                                }
                                // Nếu nhiều detail, gán theo STT
                                else if (item.STT.HasValue && item.STT.Value > 0 && sttToDetailIdMap.TryGetValue(item.STT.Value, out int detailId))
                                {
                                    serial.BillExportTechDetailID = detailId;
                                }

                                if (serial.ID <= 0)
                                {
                                    await _billExportTechDetailSerialRepo.CreateAsync(serial);
                                }
                                else
                                {
                                    await _billExportTechDetailSerialRepo.UpdateAsync(serial);
                                }

                                savedSerials.Add(serial);
                                SQLHelper<dynamic>.ExcuteProcedure("spUpdateStatusProductRTCQRCode", new string[] { "@ProductRTCQRCode", "@Status" }, new object[] { serial.SerialNumber, 3 });
                                if (serial.ModulaLocationDetailID > 0)
                                {
                                    var locations = _billExportDetailSerialNumberModulaLocationRepo.GetAll(x => x.ModulaLocationDetailID == serial.ModulaLocationDetailID && x.BillExportTechDetailSerialID == serial.ID);
                                    if (locations.Count > 0) continue;
                                    BillExportDetailSerialNumberModulaLocation location = new BillExportDetailSerialNumberModulaLocation();
                                    location.ModulaLocationDetailID = serial.ModulaLocationDetailID;
                                    location.Quantity = 1;
                                    location.BillExportTechDetailSerialID = serial.ID;
                                    await _billExportDetailSerialNumberModulaLocationRepo.CreateAsync(location);
                                }
                            }
                        }

                        if (product.billExportTechnical.CheckAddHistoryProductRTC == true)
                        {

                            var dt = SQLHelper<dynamic>.ProcedureToList("spGetBillExportTechDetailSerial", new string[] { "@BillExportTechDetailID", "@WarehouseID" }, new object[] { singleDetailId, product.billExportTechnical.WarehouseID ?? 1 });


                            var productrtc = _productRTCRepo.GetByID(item.ProductID ?? 0);
                            var data = SQLHelper<dynamic>.GetListData(dt, 0);
                            if (data.Count > 0)
                            {
                                foreach (var d in data)
                                {
                                    var dict = (IDictionary<string, object>)d;

                                    string serialNumber = dict.ContainsKey("SerialNumber")
                                        ? dict["SerialNumber"]?.ToString() ?? ""
                                        : "";

                                    if (string.IsNullOrWhiteSpace(serialNumber))
                                        continue;

                                    var productQRCode = _productRTCQRCodeRepo.GetAll(x => x.ProductQRCode.Trim() == serialNumber.Trim()).FirstOrDefault();
                                    if (productQRCode == null) continue;
                                    HistoryProductRTC historyProduct = _historyProductRTCRepo.GetAll(x => x.ProductRTCID == item.ProductID && x.BillExportTechnicalID == product.billExportTechnical.ID && x.ProductRTCQRCode == serialNumber).FirstOrDefault() ?? new HistoryProductRTC();
                                    if (historyProduct.Status == 0) continue;
                                    historyProduct.ProductRTCQRCode = serialNumber;
                                    historyProduct.ProductRTCQRCodeID = productQRCode.ID;
                                    historyProduct.ProductRTCID = item.ProductID;
                                    historyProduct.DateBorrow = product.billExportTechnical.CreatedDate;
                                    historyProduct.DateReturnExpected = product.billExportTechnical.ExpectedDate;
                                    historyProduct.PeopleID = product.billExportTechnical.ReceiverID;
                                    historyProduct.Note = "Phiếu xuất" + product.billExportTechnical.Code + (string.IsNullOrWhiteSpace(item.Note) ? "" : ":\n" + item.Note);
                                    historyProduct.Project = product.billExportTechnical.ProjectName;
                                    historyProduct.NumberBorrow = 1;
                                    historyProduct.Status = 1;
                                    historyProduct.BillExportTechnicalID = product.billExportTechnical.ID;
                                    historyProduct.WarehouseID = product.billExportTechnical.WarehouseID;
                                    historyProduct.IsDelete = false;
                                    if (historyProduct.ID <= 0) await _historyProductRTCRepo.CreateAsync(historyProduct);
                                    else await _historyProductRTCRepo.UpdateAsync(historyProduct);
                                }

                            }
                            else
                            {
                                if (savedSerials.Count > 0)
                                {
                                    foreach (var s in savedSerials)
                                    {
                                        var productRtcQRCode = _productRTCQRCodeRepo.GetAll(x => x.ProductQRCode == s.SerialNumber).FirstOrDefault();
                                        if (productRtcQRCode == null) continue;
                                        HistoryProductRTC oHistoryModel = new HistoryProductRTC();
                                        var his = _historyProductRTCRepo.GetAll(x => x.ProductRTCID == item.ProductID && x.BillExportTechnicalID == product.billExportTechnical.ID && x.IsDelete != true && x.ProductRTCQRCode == s.SerialNumber).FirstOrDefault() ?? new HistoryProductRTC();
                                        if (his.ID > 0)
                                        {
                                            oHistoryModel = his;
                                            if (oHistoryModel.Status == 0) continue;
                                        }
                                        oHistoryModel.ProductRTCQRCodeID = productRtcQRCode.ID;
                                        oHistoryModel.ProductRTCQRCode = s.SerialNumber;
                                        oHistoryModel.ProductRTCID = item.ProductID;
                                        oHistoryModel.DateBorrow = product.billExportTechnical.CreatedDate;
                                        oHistoryModel.DateReturnExpected = product.billExportTechnical.ExpectedDate;
                                        oHistoryModel.PeopleID = product.billExportTechnical.ReceiverID;
                                        oHistoryModel.Note = "Phiếu xuất" + product.billExportTechnical.Code + (string.IsNullOrWhiteSpace(item.Note) ? "" : ":\n" + item.Note);
                                        oHistoryModel.Project = product.billExportTechnical.ProjectName;
                                        oHistoryModel.Status = 1;
                                        oHistoryModel.BillExportTechnicalID = product.billExportTechnical.ID; ;
                                        oHistoryModel.NumberBorrow = item.Quantity;
                                        oHistoryModel.WarehouseID = product.billExportTechnical.WarehouseID;
                                        oHistoryModel.IsDelete = false;
                                        if (oHistoryModel.ID <= 0) await _historyProductRTCRepo.CreateAsync(oHistoryModel);
                                        else await _historyProductRTCRepo.UpdateAsync(oHistoryModel);
                                    }
                                }
                                else
                                {
                                    HistoryProductRTC oHistoryModel = new HistoryProductRTC();
                                    var his = _historyProductRTCRepo.GetAll(x => x.ProductRTCID == item.ProductID && x.IsDelete != true && x.BillExportTechnicalID == item.BillExportTechID).FirstOrDefault() ?? new HistoryProductRTC();
                                    if (his.ID > 0)
                                    {
                                        oHistoryModel = his;
                                        if (oHistoryModel.Status == 0) continue;
                                    }
                                    oHistoryModel.ProductRTCQRCodeID = 0;
                                    //oHistoryModel.ProductRTCQRCode = s.SerialNumber;
                                    oHistoryModel.ProductRTCID = item.ProductID;
                                    oHistoryModel.DateBorrow = product.billExportTechnical.CreatedDate;
                                    oHistoryModel.DateReturnExpected = product.billExportTechnical.ExpectedDate;
                                    oHistoryModel.PeopleID = product.billExportTechnical.ReceiverID;
                                    oHistoryModel.Note = "Phiếu xuất" + product.billExportTechnical.Code + (string.IsNullOrWhiteSpace(item.Note) ? "" : ":\n" + item.Note);
                                    oHistoryModel.Project = product.billExportTechnical.ProjectName;
                                    oHistoryModel.Status = 1;
                                    oHistoryModel.BillExportTechnicalID = product.billExportTechnical.ID; ;
                                    oHistoryModel.NumberBorrow = item.Quantity;
                                    oHistoryModel.WarehouseID = product.billExportTechnical.WarehouseID;
                                    oHistoryModel.IsDelete = false;
                                    if (oHistoryModel.ID <= 0) await _historyProductRTCRepo.CreateAsync(oHistoryModel);
                                    else await _historyProductRTCRepo.UpdateAsync(oHistoryModel);
                                }

                            }
                        }
                    }
                }

                //List<BillExportTechDetailSerial> savedSerials = new();

                //if (product.billExportTechDetailSerials != null && product.billExportTechDetailSerials.Any())
                //{
                //    foreach (var item in product.billExportTechDetailSerials)
                //    {
                //        // Nếu chỉ có 1 detail, gán trực tiếp
                //        if (singleDetailId.HasValue)
                //        {
                //            item.BillExportTechDetailID = singleDetailId.Value;
                //        }
                //        // Nếu nhiều detail, gán theo STT
                //        else if (item.STT.HasValue && item.STT.Value > 0 && sttToDetailIdMap.TryGetValue(item.STT.Value, out int detailId))
                //        {
                //            item.BillExportTechDetailID = detailId;
                //        }

                //        if (item.ID <= 0)
                //        {
                //            await _billExportTechDetailSerialRepo.CreateAsync(item);
                //        }
                //        else
                //        {
                //            await _billExportTechDetailSerialRepo.UpdateAsync(item);
                //        }

                //        savedSerials.Add(item);


                //        SQLHelper<dynamic>.ExcuteProcedure("spUpdateStatusProductRTCQRCode", new string[] { "@ProductRTCQRCode", "@Status" }, new object[] { item.SerialNumber, 3 });

                //    }
                //}
                //if (product.inentoryDemos != null && product.inentoryDemos.Any())
                //{
                //    foreach (var item in product.inentoryDemos)
                //    {

                //        if (item.ID <= 0)
                //            await _inventoryDemoRepo.CreateAsync(item);
                //        else
                //            await _inventoryDemoRepo.UpdateAsync(item);
                //    }
                //}

                //if (product.historyProductRTCs != null && product.historyProductRTCs.Any())
                //{

                //    foreach (var item in product.historyProductRTCs)
                //    {
                //        item.BillExportTechnicalID = product.billExportTechnical.ID;
                //        if (item.ID <= 0)
                //        {
                //            await _historyProductRTCRepo.CreateAsync(item);
                //        }

                //        else
                //            await _historyProductRTCRepo.UpdateAsync(item);
                //    }
                //}

                return Ok(new
                {
                    status = 1,
                    data = product
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("approve-bill")]
        // [Authorize] // Bắt buộc phải có Token
        [RequiresPermission("N18,N19,N50,N52,N1,N80")]
        public async Task<IActionResult> ApproveBill([FromBody] ApproveBillDTO req)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUserInfo = ObjectMapper.GetCurrentUser(claims);
                int currentUserID = currentUserInfo.EmployeeID;

                if (currentUserID <= 0)
                    return Unauthorized(new { status = 0, message = "Bạn chưa đăng nhập hoặc Token không hợp lệ." });

                if (req.BillID <= 0)
                    return BadRequest(new { status = 0, message = "ID phiếu không hợp lệ." });

                var bill = await _billExportTechnicalRepo.GetByIDAsync(req.BillID);
                if (bill == null)
                    return BadRequest(new { status = 0, message = "Không tìm thấy phiếu xuất." });

                int approverID = bill.ApproverID ?? 0;
                if (approverID != currentUserID && currentUserID != 1)
                {
                    return BadRequest(new { status = 0, message = "Bạn không có quyền duyệt phiếu này!" });
                }
                bool currentStatus = (bill.Status == 1);
                if (req.IsApproved && currentStatus)
                {
                    return Ok(new { status = 1, message = "Phiếu này đã được duyệt trước đó." });
                }
                if (!req.IsApproved && !currentStatus)
                {
                    return Ok(new { status = 1, message = "Phiếu này chưa được duyệt." });
                }

                bill.Status = req.IsApproved ? 1 : 0;
                bill.UpdatedBy = currentUserInfo.LoginName; // Lưu người thao tác là người gọi API
                bill.UpdatedDate = DateTime.Now;

                // Lưu xuống DB ngay lập tức
                await _billExportTechnicalRepo.UpdateAsync(bill);

                var log = new BillExportTechnicalLog
                {
                    BillExportTechnicalID = bill.ID,
                    StatusBill = req.IsApproved,
                    DateStatus = DateTime.Now,
                };

                await _billExportTechnicalLogRepo.CreateAsync(log);

                return Ok(new
                {
                    status = 1,
                    message = req.IsApproved ? "Duyệt phiếu thành công." : "Đã hủy duyệt phiếu."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
