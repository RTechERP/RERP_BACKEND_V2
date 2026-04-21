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
					return BadRequest("Dữ liệu phiếu xuất kỹ thuật không hợp lệ.");

				ExcelPackage.License.SetNonCommercialOrganization("RTC Technology VietNam");
				string pahtServer = _configuration.GetValue<string>("PathTemplate");
				string templatePath = Path.Combine(pahtServer, "ExportExcel", "BillExportTechnical.xlsx");
				if (!System.IO.File.Exists(templatePath))
					return NotFound("File mẫu không tồn tại.");

				string fileName = $"PXKT_{master.Code}_{DateTime.Now:ddMMyyyy_HHmmss}.xlsx";

				using var package = new ExcelPackage(new FileInfo(templatePath));
				var ws = package.Workbook.Worksheets[0];

				// ── Dữ liệu chung ──────────────────────────────────────────────
				DateTime createDate = master.CreatedDate ?? DateTime.Now;
				string locationDate = $"Hà Nội, Ngày {createDate.Day} tháng {createDate.Month} năm {createDate.Year}";

				string supplierCode = master.SupplierName?.Trim() ?? "";
				string customerName =  "";
				string customerFullName = master.CustomerName?.Trim() ?? "";
				string deliver = master.Deliver?.Trim() ?? "";
				string receiver = master.Receiver?.Trim() ?? "";
				int receiverID = master.ReceiverID ?? 0;
				string dept = master.DepartmentName?.Trim() ?? "";
				string address = master.Addres?.Trim() ?? "";
                string projectName = master.ProjectName?.Trim() ?? "";

				// ── Ghi header ─────────────────────────────────────────────────
				// [14,2]  Địa danh + ngày
				ws.Cells[16, 2].Value = locationDate;

				// [16,4]  Số phiếu
				ws.Cells[18, 4].Value = master.Code;

				// [17,3]  Nhà cung cấp hoặc tên khách hàng đầy đủ
				ws.Cells[19, 3].Value = string.IsNullOrEmpty(supplierCode) ? customerFullName : supplierCode;

				// [17,7] + [39,5]  Người giao
				ws.Cells[19, 5].Value = deliver;
				ws.Cells[37, 3].Value = deliver;

				// [18,3]  Tên dự án
				//ws.Cells[18, 3].Value = projectName;

				// [19,3]  Người nhận (kèm phòng ban hoặc tên khách hàng)
				if (!string.IsNullOrEmpty(customerName))
				{
					ws.Cells[20, 3].Value = $"{receiver} - {customerName}";
				}
				else
				{
					ws.Cells[20, 3].Value = string.IsNullOrEmpty(dept)
						? receiver
						: $"{receiver} / Phòng {dept}";
				}

				// [20,3]  Địa chỉ
				ws.Cells[21, 3].Value = address;

				// [39,10] Người nhận (chữ ký cuối)
				ws.Cells[37, 8].Value = receiver;
				// ── Ghi chi tiết (logic giống code chuẩn: insert-from-bottom, delete template) ──
				int startRow = 27; // dòng template detail

				// Duyệt ngược từ cuối lên đầu, mỗi vòng insert 1 dòng rồi ghi
				for (int i = details.Count - 1; i >= 0; i--)
				{
					var row = details[i];

					ws.Cells[startRow, 2].Value = i + 1;
					ws.Cells[startRow, 3].Value = row.ProductCode?.Trim() ?? "";
					ws.Cells[startRow, 4].Value = row.ProductName?.Trim() ?? "";
					ws.Cells[startRow, 5].Value = row.Quantity;
					ws.Cells[startRow, 6].Value = row.UnitName?.Trim() ?? "";
					ws.Cells[startRow, 7].Value = row.Maker?.Trim() ?? "";
					ws.Cells[startRow, 8].Value = row.WarehouseType?.Trim() ?? "";
					ws.Cells[startRow, 9].Value = row.ProductCodeRTC?.Trim() ?? "";
					ws.Cells[startRow, 10].Value = row.Note?.Trim() ?? "";

					ws.InsertRow(startRow, 1, startRow); // insert + copy style từ dòng vừa ghi
				}

				// Xóa dòng template thừa (dòng startRow bị đẩy xuống sau các lần insert)
				ws.DeleteRow(startRow);
                ws.DeleteRow(startRow+ details.Count);

                // ── Xuất file ──────────────────────────────────────────────────
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

                if (product.billExportDetailTechnicals != null && product.billExportDetailTechnicals.Any())
                {
                    foreach (var item in product.billExportDetailTechnicals)
                    {
                        item.BillExportTechID = product.billExportTechnical.ID;

                        if (item.ID <= 0)
                            await _billExportDetailTechnicalRepo.CreateAsync(item);
                        else
                            await _billExportDetailTechnicalRepo.UpdateAsync(item);

                        // Cập nhật inventory demo
                        var inventorydemo = _inventoryDemoRepo.GetAll(x =>
                            x.ProductRTCID == item.ProductID &&
                            (x.WarehouseID == item.WarehouseID || x.WarehouseID == product.billExportTechnical.WarehouseID))
                            .FirstOrDefault() ?? new Model.Entities.InventoryDemo();

                        inventorydemo.ProductRTCID = item.ProductID;
                        inventorydemo.WarehouseID = product.billExportTechnical.WarehouseID;

                        if (inventorydemo.ID <= 0) await _inventoryDemoRepo.CreateAsync(inventorydemo);
                        else await _inventoryDemoRepo.UpdateAsync(inventorydemo);

                        // ✅ Serial nằm trực tiếp trong từng detail, không cần map/filter gì thêm
                        List<BillExportTechDetailSerial> savedSerials = new();

                        if (item.billExportTechDetailSerials != null && item.billExportTechDetailSerials.Any())
                        {
                            foreach (var serial in item.billExportTechDetailSerials)
                            {
                                serial.BillExportTechDetailID = item.ID;

                                // ✅ Check trong DB theo SerialNumber + BillExportTechDetailID
                                var existingSerial = _billExportTechDetailSerialRepo
                                    .GetAll(x =>
                                    //x.SerialNumber == serial.SerialNumber &&
                                    x.BillExportTechDetailID == item.ID && x.IsDeleted == false)
                                    .FirstOrDefault();

                                if (existingSerial != null)
                                {
                                    // Đã tồn tại → update
                                    serial.ID = existingSerial.ID;
                                    await _billExportTechDetailSerialRepo.UpdateAsync(serial);
                                }
                                else
                                {
                                    // Chưa có → create
                                    serial.ID = 0;
                                    await _billExportTechDetailSerialRepo.CreateAsync(serial);
                                }


                                savedSerials.Add(serial);

                                SQLHelper<dynamic>.ExcuteProcedure(
                                    "spUpdateStatusProductRTCQRCode",
                                    new string[] { "@ProductRTCQRCode", "@Status" },
                                    new object[] { serial.SerialNumber, 3 });

                                if (serial.ModulaLocationDetailID > 0)
                                {
                                    var locations = _billExportDetailSerialNumberModulaLocationRepo.GetAll(x =>
                                        x.ModulaLocationDetailID == serial.ModulaLocationDetailID &&
                                        x.BillExportTechDetailSerialID == serial.ID);

                                    if (locations.Count > 0) continue;

                                    BillExportDetailSerialNumberModulaLocation location = new BillExportDetailSerialNumberModulaLocation();
                                    location.ModulaLocationDetailID = serial.ModulaLocationDetailID;
                                    location.Quantity = 1;
                                    location.BillExportTechDetailSerialID = serial.ID;
                                    await _billExportDetailSerialNumberModulaLocationRepo.CreateAsync(location);
                                }
                            }
                        }

                        // Lưu lịch sử mượn trả
                        if (product.billExportTechnical.CheckAddHistoryProductRTC == true)
                        {
                            var dt = SQLHelper<dynamic>.ProcedureToList(
                                "spGetBillExportTechDetailSerial",
                                new string[] { "@BillExportTechDetailID", "@WarehouseID" },
                                new object[] { item.ID, product.billExportTechnical.WarehouseID ?? 1 });

                            var c = _productRTCRepo.GetByID(item.ProductID ?? 0);
                            var data = SQLHelper<dynamic>.GetListData(dt, 0);

                            if (data.Count > 0)
                            {
                                foreach (var d in data)
                                {
                                    var dict = (IDictionary<string, object>)d;

                                    string serialNumber = dict.ContainsKey("SerialNumber")
                                        ? dict["SerialNumber"]?.ToString() ?? ""
                                        : "";

                                    if (string.IsNullOrWhiteSpace(serialNumber)) continue;

                                    var productQRCode = _productRTCQRCodeRepo.GetAll(x => x.ProductQRCode.Trim() == serialNumber.Trim()).FirstOrDefault();
                                    if (productQRCode == null) continue;

                                    HistoryProductRTC historyProduct = _historyProductRTCRepo.GetAll(x =>
                                        x.ProductRTCID == item.ProductID &&
                                        x.BillExportTechnicalID == product.billExportTechnical.ID &&
                                        x.ProductRTCQRCode == serialNumber && x.IsDelete==false)
                                        .FirstOrDefault() ?? new HistoryProductRTC();

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
                                        var his = _historyProductRTCRepo.GetAll(x =>
                                            x.ProductRTCID == item.ProductID &&
                                            x.BillExportTechnicalID == product.billExportTechnical.ID &&
                                            x.IsDelete != true &&
                                            x.ProductRTCQRCode == s.SerialNumber)
                                            .FirstOrDefault() ?? new HistoryProductRTC();

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
                                        oHistoryModel.BillExportTechnicalID = product.billExportTechnical.ID;
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
                                    var his = _historyProductRTCRepo.GetAll(x =>
                                        x.ProductRTCID == item.ProductID &&
                                        x.IsDelete != true &&
                                        x.BillExportTechnicalID == item.BillExportTechID)
                                        .FirstOrDefault() ?? new HistoryProductRTC();

                                    if (his.ID > 0)
                                    {
                                        oHistoryModel = his;
                                        if (oHistoryModel.Status == 0) continue;
                                    }

                                    oHistoryModel.ProductRTCQRCodeID = 0;
                                    oHistoryModel.ProductRTCID = item.ProductID;
                                    oHistoryModel.DateBorrow = product.billExportTechnical.CreatedDate;
                                    oHistoryModel.DateReturnExpected = product.billExportTechnical.ExpectedDate;
                                    oHistoryModel.PeopleID = product.billExportTechnical.ReceiverID;
                                    oHistoryModel.Note = "Phiếu xuất" + product.billExportTechnical.Code + (string.IsNullOrWhiteSpace(item.Note) ? "" : ":\n" + item.Note);
                                    oHistoryModel.Project = product.billExportTechnical.ProjectName;
                                    oHistoryModel.Status = 1;
                                    oHistoryModel.BillExportTechnicalID = product.billExportTechnical.ID;
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
