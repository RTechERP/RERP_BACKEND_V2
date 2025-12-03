using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;
using RERPAPI.Repo.GenericEntity.Technical;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
using ZXing.Common;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BillImportController : ControllerBase
    {
        private readonly BillImportRepo _billImportRepo;
        private readonly BillImportLogRepo _billImportLogRepo;
        private readonly DocumentImportRepo _documentImportRepo;
        private readonly BillDocumentImportRepo _billDocumentImportRepo;
        private readonly BillImportDetailRepo _billImportDetailRepo;
        private readonly InvoiceLinkRepo _invoiceLinkRepo;
        private readonly InventoryProjectRepo _inventoryProjectRepo;
        private readonly InventoryRepo _inventoryRepo;
        private readonly BillImportDetailSerialNumberRepo _billImportDetailSerialNumberRepo;
        private readonly IConfiguration _configuration;
        private readonly DocumentImportPONCCRepo _documentImportPONCCRepo;
        private readonly EmployeeRepo _employeeRepo;
        //private readonly PONCCDetailRepo _pONCCDetailRepo;
        private readonly PONCCRepo _pONCCRepo;

        private List<InvoiceDTO> listInvoice = new List<InvoiceDTO>();

        public BillImportController(
            BillImportRepo billImportRepo,
            BillImportLogRepo billImportLogRepo,
            DocumentImportRepo documentImportRepo,
            BillDocumentImportRepo billDocumentImportRepo,
            BillImportDetailRepo billImportDetailRepo,
            InvoiceLinkRepo invoiceLinkRepo,
            InventoryProjectRepo inventoryProjectRepo,
            InventoryRepo inventoryRepo,
            BillImportDetailSerialNumberRepo billImportDetailSerialNumberRepo,
            IConfiguration configuration,
            DocumentImportPONCCRepo documentImportPONCCRepo, EmployeeRepo employeeRepo
            , PONCCRepo pONCCRepo)
        {
            _billImportRepo = billImportRepo;
            _billImportLogRepo = billImportLogRepo;
            _documentImportRepo = documentImportRepo;
            _billDocumentImportRepo = billDocumentImportRepo;
            _billImportDetailRepo = billImportDetailRepo;
            _invoiceLinkRepo = invoiceLinkRepo;
            _inventoryProjectRepo = inventoryProjectRepo;
            _inventoryRepo = inventoryRepo;
            _billImportDetailSerialNumberRepo = billImportDetailSerialNumberRepo;
            _configuration = configuration;
            _documentImportPONCCRepo = documentImportPONCCRepo;
            _employeeRepo = employeeRepo;
            _pONCCRepo = pONCCRepo;
        }
        /// <summary>
        /// lấy danh sách phiếu nhập
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost("get-all")]
        public IActionResult getBillImport([FromBody] BillImportParamRequest filter)
        {
            try
            {
                if (filter.checkedAll == true)
                {
                    filter.DateStart = new DateTime(1990, 01, 01);
                    filter.DateEnd = new DateTime(9999, 01, 01);
                }
                if (filter.DateEnd.HasValue)
                {
                    filter.DateEnd = filter.DateEnd.Value.Date
                        .AddDays(1).AddTicks(-1);
                }
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                       "spGetBillImport_New", new string[] { "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@Status", "@KhoType", "@FilterText", "@WarehouseCode" },
                    new object[] { filter.PageNumber, filter.PageSize, filter.DateStart, filter.DateEnd, filter.Status, filter.KhoType, filter.FilterText, filter.WarehouseCode }
                   );
                /* List<dynamic> billList = result[0]; // dữ liệu hóa đơn*/
                int totalPage = 0;

                if (result.Count > 1 && result[1].Count > 0)
                {
                    totalPage = (int)result[1][0].TotalPage;
                }

                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(result, 0),
                    totalPage
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

        //get-option-product-sale
        [HttpGet("get-product")]
        public IActionResult getOptionProduct(int warehouseID, int productGroupID)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                       "spGetProductImportSale", new string[] { "@GroupProductID", "@WarehouseCode", },
                    new object[] { productGroupID, warehouseID }
                   );
                /* List<dynamic> billList = result[0]; // dữ liệu hóa đơn*/
                int totalPage = 0;

                if (result.Count > 1 && result[1].Count > 0)
                {
                    totalPage = (int)result[1][0].TotalPage;
                }

                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(result, 0),
                    totalPage
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

        /// <summary>
        /// lấy dữ liệu phiếu nhâp theo id 
        /// </summary>
        /// <param mã phiếu nhập="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult getBillImportByID(int id)
        {
            try
            {
                BillImport result = _billImportRepo.GetByID(id);
                /*   var newCode = _billexportRepo.GetBillCode()*/
                return Ok(new
                {
                    status = 1,
                    data = result,
                    /*  newCode = newCode,*/
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
        ///// <summary>
        ///// duyệt,hủy duyệt phiếu nhập
        ///// </summary>
        ///// <param name="billExport"></param>
        ///// <param name="isapproved"></param>
        ///// <returns></returns>
        //[HttpPost("approved")]
        //public async Task<IActionResult> Approved([FromBody] BillImport billImport, bool isapproved)
        //{
        //    try
        //    {
        //        string messageError;
        //        string message = isapproved ? "nhận chứng từ" : "hủy nhận chứng từ";
        //        if (billImport.Status == false && isapproved == false)
        //        {
        //            messageError = $"{billImport.BillImportCode} chưa nhận chứng từ!";
        //            throw new Exception(messageError);
        //        }
        //        if (billImport.BillTypeNew == 4)
        //        {
        //            messageError = $"Bạn không thể {message} cho phiếu Yêu cầu nhập kho!";
        //            throw new Exception(messageError);
        //        }
        //        // Cập nhật trạng thái duyệt phiếu
        //        billImport.Status = isapproved;
        //        billImport.UnApprove = isapproved ? 1 : 2;
        //        await _billImportRepo.UpdateAsync(billImport);

        //        // Tính lại tồn kho và tình hình đơn hàng 
        //        SQLHelper<dynamic>.ExcuteProcedure("spCalculateImport_New", new string[] { "@ID", "@WarehouseID" }, new object[] { billImport.ID, billImport.WarehouseID });
        //        SQLHelper<dynamic>.ExcuteProcedure("spUpdateTinhHinhDonHang",
        //                 new string[] { "@BillImportID", "@IsApproved" },
        //                 new object[] { billImport.ID, isapproved });
        //        //ghi log
        //        BillImportLog log = new BillImportLog()
        //        {
        //            BillImportID = billImport.ID,
        //            StatusBill = isapproved,
        //            DateStatus = DateTime.Now,
        //        };
        //        await _billImportLogRepo.CreateAsync(log);
        //        return Ok(ApiResponseFactory.Success(null, $"Phiếu {billImport.BillImportCode} đã được {message} thành công!"));

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}
        /// <summary>
        /// duyệt,hủy duyệt phiếu nhập
        /// </summary>
        /// <param name="billImports">Danh sách phiếu nhập</param>
        /// <param name="isapproved">Trạng thái duyệt</param>
        /// <returns></returns>
        [HttpPost("approved")]
        public async Task<IActionResult> Approved([FromBody] List<BillImportApprovedDTO> billImports, bool isapproved)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                string messageError;
                string message = isapproved ? "nhận chứng từ" : "hủy nhận chứng từ";

                // Kiểm tra điều kiện cho từng phiếu
                foreach (var billImport in billImports)
                {
                    if (billImport.Status == false && isapproved == false)
                    {
                        messageError = $"{billImport.BillImportCode} chưa nhận chứng từ!";
                        return BadRequest(ApiResponseFactory.Fail(null, messageError));
                    }
                    if (billImport.BillTypeNew == 4)
                    {
                        messageError = $"Bạn không thể {message} cho phiếu Yêu cầu nhập kho {billImport.BillImportCode}!";
                        return BadRequest(ApiResponseFactory.Fail(null, messageError));
                    }
                    if (isapproved)
                    {
                        string currencyText = billImport.CurrencyList ?? "";
                        if (string.IsNullOrEmpty(currencyText)) continue;
                        int[] lstCurrency = currencyText.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                        string billCode = billImport.PONCCCodeList ?? "";
                        if (lstCurrency.Length > 1)
                        {
                            messageError = $"Phiếu nhập kho [{billImport.BillImportCode}] có nhiều hơn 1 loại tiền tệ từ đơn mua hàng [{billCode}]. Vui lòng kiểm tra lại!";
                            return BadRequest(ApiResponseFactory.Fail(null, messageError));
                        }
                    }
                }

                // Cập nhật trạng thái cho tất cả phiếu
                foreach (var billImport in billImports)
                {
                    // Cập nhật trạng thái duyệt phiếu
                    billImport.Status = isapproved;
                    billImport.UnApprove = isapproved ? 1 : 2;
                    //billImport.StatusDocumentImport = isapproved;
                    await _billImportRepo.UpdateAsync(billImport);

                    // Tính lại tồn kho và tình hình đơn hàng 
                    SQLHelper<dynamic>.ExcuteProcedure("spCalculateImport_New", new string[] { "@ID", "@WarehouseID" }, new object[] { billImport.ID, billImport.WarehouseID });
                    //SQLHelper<dynamic>.ExcuteProcedure("spUpdateTinhHinhDonHang",
                    //         new string[] { "@BillImportID", "@IsApproved" },
                    //         new object[] { billImport.ID, isapproved });


                    if (isapproved)
                    {
                        string currencyText = billImport.CurrencyList ?? "";
                        if (!string.IsNullOrWhiteSpace(currencyText))
                        {
                            int[] lstCurrency = currencyText.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                            decimal vat = billImport.VAT ?? 0;
                            if (lstCurrency.Length == 1 && Convert.ToInt32(currencyText) != 1 && vat == 0)
                            {
                                billImport.StatusDocumentImport = isapproved;
                                billImport.UpdatedBy = currentUser.LoginName;
                                billImport.UpdatedDate = DateTime.Now;
                                await _billImportRepo.UpdateAsync(billImport);
                            }
                        }
                    }
                    else
                    {
                        billImport.StatusDocumentImport = isapproved;
                        await _billImportRepo.UpdateAsync(billImport);
                    }

                    //ghi log
                    BillImportLog log = new BillImportLog()
                    {
                        BillImportID = billImport.ID,
                        StatusBill = isapproved,
                        DateStatus = DateTime.Now,
                    };
                    await _billImportLogRepo.CreateAsync(log);
                    await _pONCCRepo.UpdateTinhHinhDonHang(billImport, isapproved);
                }
                var billCodes = string.Join(", ", billImports.Select(x => x.BillImportCode));
                return Ok(ApiResponseFactory.Success(null, $"{billImports.Count} phiếu ({billCodes}) đã được {message} thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("approve-document-import")]
        public IActionResult ApproveDocumentImport([FromBody] List<BillImportApproveDocumentDTO> models, [FromQuery] bool status)
        {
            if (models == null || models.Count == 0)
            {
                return BadRequest(ApiResponseFactory.Fail(null, "Danh sách phiếu truyền rỗng!"));
            }

            string message;
            bool result = _billImportRepo.ApproveDocument(models, status, out message);

            if (!result)
            {
                return BadRequest(ApiResponseFactory.Fail(null, message));
            }

            return Ok(ApiResponseFactory.Success(models, message));
        }

        ///// <summary>
        ///// duyệt,hủy duyệt phiếu nhập
        ///// </summary>
        ///// <param name="billExport"></param>
        ///// <param name="isapproved"></param>
        ///// <returns></returns>
        //[HttpPost("approved")]
        //public async Task<IActionResult> Approved([FromBody] BillImport billImport, bool isapproved)
        //{
        //    try
        //    {
        //        string message = isapproved ? "nhận chứng từ" : "hủy nhận chứng từ";
        //        if (billImport.Status == false && isapproved == false)
        //        {
        //            return Ok(new
        //            {
        //                status = 0,
        //                message = $"{billImport.BillImportCode} chưa nhận chứng từ!",
        //            });
        //        }
        //        if (billImport.BillTypeNew == 4)
        //        {
        //            return Ok(new
        //            {
        //                status = 0,
        //                message = $"Bạn không thể {message} cho phiếu Yêu cầu nhập kho!",
        //            });
        //        }
        //        // Cập nhật trạng thái duyệt phiếu
        //        billImport.Status = isapproved;
        //        billImport.UnApprove = isapproved ? 1 : 2;
        //        await _billImportRepo.UpdateAsync(billImport);

        //        // Tính lại tồn kho và tình hình đơn hàng 
        //        SQLHelper<dynamic>.ExcuteProcedure("spCalculateImport_New", new string[] { "@ID", "@WarehouseID" }, new object[] { billImport.ID, billImport.WarehouseID });
        //        //SQLHelper<dynamic>.ExcuteProcedure("spUpdateTinhHinhDonHang",
        //        //         new string[] { "@BillImportID", "@IsApproved" },
        //        //         new object[] { billImport.ID, isapproved });
        //        //ghi log
        //        BillImportLog log = new BillImportLog()
        //        {
        //            BillImportID = billImport.ID,
        //            StatusBill = isapproved,
        //            DateStatus = DateTime.Now,
        //        };
        //        await _billImportLogRepo.CreateAsync(log);
        //        return Ok(new
        //        {
        //            status = 1,
        //            message = $"Phiếu {billImport.BillImportCode} đã được {message} thành công!"
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
        private async Task<(bool IsValid, string ErrorMessage)> ValidateBillImport(BillImportDTO dto)
        {
            if (dto.billImport.IsDeleted == true && dto.billImport.ID > 0)
            {
                return (true, string.Empty);
            }
            // Validate Supplier
            if (dto.billImport.SupplierID == null)
            {
                return (false, "Xin hãy điền thông tin nhà cung cấp.");
            }

            // Validate Receiver
            if (string.IsNullOrWhiteSpace(dto.billImport.Reciver))
            {
                return (false, "Xin hãy điền thông tin người nhập.");
            }

            // Validate Warehouse Type
            if (string.IsNullOrWhiteSpace(dto.billImport.KhoType))
            {
                return (false, "Xin hãy chọn kho quản lý.");
            }

            // Validate Deliver
            if (string.IsNullOrWhiteSpace(dto.billImport.Deliver))
            {
                return (false, "Xin hãy điền thông tin người giao.");
            }

            // Validate Bill Type and Creation Date
            if (dto.billImport.BillTypeNew != 4 && !dto.billImport.CreatedDate.HasValue)
            {
                return (false, "Vui lòng nhập Ngày nhập!");
            }

            // Validate Payment Rule
            if (dto.billImport.RulePayID <= 0)
            {
                return (false, "Vui lòng nhập Điều khoản TT!");
            }

            return (true, string.Empty);
        }


        [HttpGet("get-bill-code")]
        public ActionResult<string> getBillCode(int billType)
        {
            var newCode = _billImportRepo.GetBillCode(billType);
            return Ok(new { data = newCode }); // <-- Đây là điểm quan trọng
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> saveDataBillImport([FromBody] List<BillImportDTO> dtos)
        {

            try
            {
                foreach (var dto in dtos)
                {
                    var (isValid, errorMessage) = await ValidateBillImport(dto);
                    if (!isValid)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, errorMessage));
                    }
                    if (dto.billImportDetail == null && dto.DeletedDetailIDs == null)
                    {
                        _billImportRepo.Update(dto.billImport);
                        return Ok(ApiResponseFactory.Success(null, "Đã xóa thành công phiếu " + dto.billImport.BillImportCode));
                    }
                    var inventoryList = _inventoryRepo.GetAll();

                    var groupedQuantities = dto.billImportDetail.GroupBy(d => d.ProductID)
                        .ToDictionary(g => g.Key, g => g.Sum(d => d.Qty));

                    foreach (var detail in dto.billImportDetail)
                    {
                        if (groupedQuantities.ContainsKey(detail.ProductID))
                        {
                            detail.TotalQty = groupedQuantities[detail.ProductID];
                        }
                    }

                    int billImportId;
                    if (dto.billImport.ID <= 0)
                    {
                        dto.billImport.BillDocumentImportType = 2;
                        dto.billImport.IsDeleted = false;
                        dto.billImport.Status = false;
                        await _billImportRepo.CreateAsync(dto.billImport);
                        billImportId = dto.billImport.ID;

                        // Thêm chứng từ
                        var listID = _documentImportRepo.GetAll(x => x.IsDeleted == false).ToList();
                        foreach (var item in listID)
                        {
                            BillDocumentImport billDocumentImport = new BillDocumentImport
                            {
                                BillImportID = billImportId,
                                DocumentImportID = item.ID,
                                DocumentStatus = 0,
                                Note = ""
                            };
                            await _billDocumentImportRepo.CreateAsync(billDocumentImport);
                        }
                    }
                    else
                    {
                        await _billImportRepo.UpdateAsync(dto.billImport);
                        billImportId = dto.billImport.ID;
                    }

                    // Xóa chi tiết cũ
                    if (dto.DeletedDetailIDs != null && dto.DeletedDetailIDs.Any())
                    {
                        foreach (var id in dto.DeletedDetailIDs)
                        {
                            var item = _billImportDetailRepo.GetByID(id);
                            if (item != null)
                            {
                                item.IsDeleted = true;
                                _billImportDetailRepo.Update(item);
                                var invoiceLink = _invoiceLinkRepo.GetAll(p => p.BillImportDetailID == id);
                                if (invoiceLink.Any())
                                {
                                    await _invoiceLinkRepo.DeleteByAttributeAsync("BillImportDetailID", id);
                                }
                            }
                        }
                    }
                    foreach (var detail in dto.billImportDetail)
                    {
                        detail.BillImportID = billImportId;

                        if (detail.ID <= 0)
                        {
                            detail.IsDeleted = false;
                            await _billImportDetailRepo.CreateAsync(detail);
                        }
                        else
                        {
                            // Cập nhật
                            var existingDetail = _billImportDetailRepo.GetByID(detail.ID);
                            if (existingDetail != null)
                            {
                                _billImportDetailRepo.Update(detail);
                            }
                        }
                        await UpdateInventoryProject(detail, dto.billImport);

                        // Kiểm tra tồn kho
                        bool exists = inventoryList.Any(x => x.WarehouseID == dto.billImport.WarehouseID && x.ProductSaleID == detail.ProductID);
                        if (!exists)
                        {
                            Inventory inventory = new Inventory
                            {
                                WarehouseID = dto.billImport.WarehouseID,
                                ProductSaleID = detail.ProductID,
                                TotalQuantityFirst = 0,
                                TotalQuantityLast = 0,
                                Import = 0,
                                Export = 0
                            };
                            await _inventoryRepo.CreateAsync(inventory);
                        }
                        List<InvoiceDTO> lst = listInvoice.Where(p => p.IdMapping == detail.STT).ToList();
                        // await _invoiceLinkRepo.DeleteByAttributeAsync("BillImportDetailID", (int?)detail.ID);
                        var invoicelink = _invoiceLinkRepo.GetAll(p => p.BillImportDetailID == detail.ID).FirstOrDefault();
                        if (invoicelink != null)
                        {
                            invoicelink.IsDeleted = true;
                            _invoiceLinkRepo.Update(invoicelink);
                        }
                        foreach (InvoiceDTO item in lst)
                        {
                            foreach (InvoiceLink model in item.Details)
                            {
                                model.BillImportDetailID = detail.ID;
                                //InvoiceBO.Instance.Insert(model);
                                _invoiceLinkRepo.Create(model);
                            }
                        }

                        // Cập nhật trạng thái
                        SQLHelper<dynamic>.ExcuteProcedure("spUpdateReturnedStatusForBillExportDetail",
                            new string[] { "@BillImportID", "@Approved" },
                            new object[] { detail.BillImportID ?? billImportId, 0 });
                        var listDetails = _billImportDetailRepo.GetAll(x => x.BillImportID == detail.BillImportID);
                        string poNCCDetailID = string.Join(",", listDetails.Select(x => x.PONCCDetailID));
                        SQLHelper<dynamic>.ExcuteProcedure("spUpdateStatusPONCC",
                            new string[] { "@PONCCDetailID" },
                            new object[] { poNCCDetailID });
                    }
                    if (dto.pONCCID != null && dto.pONCCID > 0)
                    {
                        PONCC po = _pONCCRepo.GetByID(dto.pONCCID ?? 0);

                        po.Status = 5;
                        await _pONCCRepo.UpdateAsync(po);


                    }
                    await UpdateDocumentImport(billImportId, dto.billDocumentImports);

                }

                return Ok(ApiResponseFactory.Success(dtos, "Cập nhật dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //test

        [HttpGet("test")]
        public IActionResult test(int id)
        {
            List<List<dynamic>> resultSets = SQLHelper<dynamic>.ProcedureToList(
                    "spGetBillImportDetail",
                    new string[] { "@ID" },
                    new object[] { id }
                );
            return Ok(new
            {
                status = 1,
                data = SQLHelper<object>.GetListData(resultSets, 0),
            });
        }

        [HttpGet("import-excel")]
        public IActionResult ImportExcel(int id)
        {
            try
            {
                // Fetch data from stored procedure
                List<List<dynamic>> resultDetail = SQLHelper<dynamic>.ProcedureToList(
                    "spGetBillImportDetail",
                    new string[] { "@ID" },
                    new object[] { id }
                );

                var detailList = resultDetail[0]
                    .Cast<IDictionary<string, object>>()
                    .ToList();

                if (!detailList.Any())
                {
                    return BadRequest(new { status = 0, message = "Không tìm thấy dữ liệu từ spGetBillImportDetail" });
                }

                // Use the first record for master data
                var masterData = detailList[0];

                // Load Excel template
                //string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "BillImportSale.xlsx");
                //if (!System.IO.File.Exists(templatePath))
                //{
                //    return BadRequest(new { status = 0, message = "Không tìm thấy file mẫu Excel" });
                //}
                string templatePath = Path.Combine(@"\\192.168.1.190\Software\Template\ExportExcel", "BillImportSale.xlsx");
                if (!System.IO.File.Exists(templatePath))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy file template: {templatePath}"));
                }
                BillImport billImport = _billImportRepo.GetByID(id);
                using (var workbook = new XLWorkbook(templatePath))
                {
                    var sheet = workbook.Worksheet(1);

                    // Mapping master data

                    sheet.Cell(6, 1).Value = billImport.BillImportCode ?? "";
                    sheet.Cell(9, 4).Value = masterData["NameNCC"]?.ToString()?.Trim() ?? "";
                    sheet.Cell(11, 4).Value = masterData["RulePayName"]?.ToString()?.Trim() ?? "";

                    // Warehouse logic
                    if (masterData["WarehouseName"]?.ToString()?.Trim() != "KHO HN")
                    {
                        sheet.Cell(10, 3).Value = "- Kho";
                        sheet.Cell(10, 4).Value = masterData["WarehouseName"]?.ToString()?.Trim() ?? "";
                    }
                    else
                    {
                        sheet.Cell(10, 4).Value = masterData["ProductGroupName"]?.ToString()?.Trim() ?? "";
                    }

                    // Department and Deliver logic
                    string departmentName = masterData["CustomerFullName"]?.ToString()?.Trim() ?? "";
                    sheet.Cell(8, 4).Value = string.IsNullOrEmpty(departmentName)
                        ? masterData["Deliver"]?.ToString()?.Trim() ?? ""
                        : $"{masterData["Deliver"]?.ToString()?.Trim() ?? ""} / Phòng {departmentName}";

                    // Date formatting
                    DateTime? creatDate = masterData["CreatDate"] != null ? Convert.ToDateTime(masterData["CreatDate"]) : null;
                    if (creatDate.HasValue)
                    {
                        sheet.Cell(17, 8).Value = $"Ngày {creatDate.Value:dd} Tháng {creatDate.Value:MM} Năm {creatDate.Value:yyyy}";
                    }

                    // Footer signatures
                    sheet.Cell(19, 3).Value = masterData["Deliver"]?.ToString()?.Trim() ?? "";
                    sheet.Cell(19, 8).Value = masterData["Reciver"]?.ToString()?.Trim() ?? "";

                    // Generate QR code
                    string qrCodeText = masterData["BillImportID"]?.ToString()?.Trim() ?? "Unknown";
                    var writer = new BarcodeWriterPixelData
                    {
                        Format = BarcodeFormat.QR_CODE,
                        Options = new EncodingOptions
                        {
                            Height = 100,
                            Width = 100,
                            Margin = 1
                        }
                    };

                    var pixelData = writer.Write(qrCodeText);
                    using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb))
                    {
                        var bitmapData = bitmap.LockBits(
                            new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                            ImageLockMode.WriteOnly,
                            PixelFormat.Format32bppRgb);

                        try
                        {
                            System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                        }
                        finally
                        {
                            bitmap.UnlockBits(bitmapData);
                        }

                        string tempPath = Path.Combine(Path.GetTempPath(), $"qr_{Guid.NewGuid()}.png");
                        bitmap.Save(tempPath, ImageFormat.Png);

                        // Add QR code to Excel (positioned at top-right, matching original coordinates)
                        sheet.AddPicture(tempPath)
                            .MoveTo(sheet.Cell(1, 11), 10, 5) // Approximates left=1000, top=5
                            .WithSize(100, 100);

                        System.IO.File.Delete(tempPath);
                    }

                    // Mapping detail data
                    int startRow = 15;
                    int currentRow = startRow;
                    int stt = 1;

                    if (detailList.Any())
                    {
                        // Insert additional rows if needed
                        if (detailList.Count > 1)
                        {
                            sheet.Row(startRow).InsertRowsBelow(detailList.Count - 1);
                        }

                        foreach (var item in detailList.OrderByDescending(x => x["ID"])) // Match original reverse order
                        {
                            sheet.Cell(currentRow, 1).Value = stt++;
                            sheet.Cell(currentRow, 2).Value = item["ProductNewCode"]?.ToString()?.Trim() ?? "";
                            sheet.Cell(currentRow, 3).Value = item["ProductCode"]?.ToString()?.Trim() ?? "";
                            sheet.Cell(currentRow, 4).Value = item["ProductName"]?.ToString()?.Trim() ?? "";
                            sheet.Cell(currentRow, 5).Value = item["Unit"]?.ToString()?.Trim() ?? "";
                            sheet.Cell(currentRow, 6).Value = item["ProjectCode"]?.ToString()?.Trim() ?? "";
                            sheet.Cell(currentRow, 7).Value = Convert.ToDecimal(item["Qty"] ?? 0);
                            sheet.Cell(currentRow, 8).Value = item["SomeBill"]?.ToString()?.Trim() ?? "";
                            sheet.Cell(currentRow, 9).Value = item["ProjectCodeText"]?.ToString()?.Trim() ?? "";
                            sheet.Cell(currentRow, 10).Value = item["ProjectNameText"]?.ToString()?.Trim() ?? "";
                            sheet.Cell(currentRow, 11).Value = item["BillCodePO"]?.ToString()?.Trim() ?? "";

                            string note = item["Note"]?.ToString()?.Trim() ?? "";
                            string codePM = item["CodeMaPhieuMuon"]?.ToString()?.Trim() ?? "";
                            note = note.StartsWith("=") ? $"'{note}" : note;
                            sheet.Cell(currentRow, 12).Value = $"{note}\n{codePM}".Trim();

                            currentRow++;
                        }
                    }
                    else
                    {
                        // Delete placeholder rows if no details
                        sheet.Row(14).Delete();
                        sheet.Row(14).Delete();
                    }

                    // Save to stream and return file
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Position = 0;

                        string fileName = $"Phiếu nhập-{billImport.BillImportCode}_{DateTime.Now:dd_MM_yyyy}.xlsx";
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = "Lỗi khi xuất Excel",
                    error = ex.ToString()
                });
            }
        }

        /// <summary>
        /// tổng hợp phiếu nhập
        /// </summary>
        [HttpPost("bill-import-synthetic")]
        public IActionResult getBillExportSynthetic([FromBody] BillExportSyntheticParamRequest filter)
        {
            try
            {

                if (filter.checkedAll == true)
                {
                    filter.DateStart = new DateTime(1990, 01, 01);
                    filter.DateEnd = new DateTime(9999, 01, 01);
                }
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                  "spGetBillImportSynthetic",
                 new string[] { "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@Status", "@KhoType", "@FilterText", "@WarehouseCode", "@IsDeleted" },
                    new object[] { filter.PageNumber, filter.PageSize, filter.DateStart, filter.DateEnd, filter.Status, filter.KhoType, filter.FilterText, filter.WarehouseCode, filter.IsDeleted }
                   );

                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(result, 0)
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
        // qr code phieu
        [HttpGet("scan-import")]
        public IActionResult ScanQRCode(string code, int warehouseId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                    return BadRequest(new { status = 0, message = "Mã QR không được để trống." });

                // 1. Tìm phiếu có mã code
                var bills = _billImportRepo.GetAll(x => x.BillImportCode == code);
                var bill = bills.FirstOrDefault();

                if (bill == null)
                    return NotFound(new { status = 0, message = $"Không tìm thấy phiếu với mã {code}." });

                /*   // 2. Kiểm tra nếu là phiếu không được quét
                   string tableName = typeof(BillImport).Name.Replace("Model", "");
                   int billTypeNew = tableName == "BillImport" ? 4 : (tableName == "BillImportTechnical" ? 5 : 0);

                   if (billTypeNew != 0)
                   {
                       var check = SQLHelper<BillImport>.FindByAttribute("BillImportCode", $"'{code}'")
                           .Where(x => x.Status == billTypeNew)
                           .FirstOrDefault();

                               if (check != null)
                           return BadRequest(new { status = 0, message = "Không thể quét phiếu loại Yêu cầu nhập kho." });
                   }*/

                // 3. Gọi store procedure lấy chi tiết phiếu
                var result = SQLHelper<BillImport>.ProcedureToList(
                    "spGetBillImportScanQR",
                    new string[] { "@FilterText", "@WareHouseId" },
                    new object[] { code, warehouseId }
                );

                var detailList = SQLHelper<BillImport>.GetListData(result, 0); // lấy bảng đầu tiên

                if (detailList == null || detailList.Count == 0)
                {
                    return NotFound(new { status = 0, message = "Không có chi tiết phiếu trong kho." });
                }

                return Ok(new
                {
                    status = 1,
                    message = "Thành công",
                    data = detailList
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 0, message = "Lỗi hệ thống", detail = ex.Message });
            }
        }

        [HttpGet("export-excel-kt")]
        public IActionResult ExportExcelKT(int id)
        {
            try
            {
                if (id == 0)
                    return BadRequest(new { status = 0, message = "ID không hợp lệ" });

                // Lấy dữ liệu
                var resultSets = SQLHelper<dynamic>.ProcedureToList(
                    "spGetBillImportDetail",
                    new[] { "@ID" },
                    new object[] { id }
                );

                var detailList = resultSets[0]
                    .Cast<IDictionary<string, object>>()
                    .ToList();

                if (!detailList.Any())
                    return BadRequest(new { status = 0, message = "Không tìm thấy dữ liệu phiếu nhập" });

                BillImport billImport = _billImportRepo.GetByID(id);
                string billImportCode = billImport.BillImportCode ?? "";

                string warehouseName = detailList[0]["WarehouseName"]?.ToString()?.Trim() ?? "";
                string warehouseCode = warehouseName.Contains("HN") ? "HN" : "HCM";

                string templatePath = @"\\192.168.1.190\Software\Template\ExportExcel\FormNhapKho.xlsx";

                if (!System.IO.File.Exists(templatePath))
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy file template."));

                // ĐỌC TEMPLATE – tránh lỗi closed stream
                byte[] templateBytes = System.IO.File.ReadAllBytes(templatePath);
                ExcelPackage.License.SetNonCommercialOrganization("RTC Technology Viet Nam");

                using (var package = new ExcelPackage(new MemoryStream(templateBytes)))
                {
                    var sheet = package.Workbook.Worksheets[0];

                    // Nếu kho HN sửa header
                    if (warehouseCode == "HN")
                        sheet.Cells[2, 14].Value = "Loại vật tư (*)"; // Row 2, Column 14 (NPOI 13)

                    int startRow = 3;       // Excel row 3
                    int currentRow = startRow;

                    // EPPlus không có ShiftRows → ta chuyển thủ công
                    int insertCount = detailList.Count;
                    if (insertCount > 1)
                        sheet.InsertRow(startRow, insertCount - 1, startRow);

                    // Fill data
                    for (int i = detailList.Count - 1; i >= 0; i--)
                    {
                        var detail = detailList[i];

                        // STT
                        sheet.Cells[currentRow, 1].Value = i + 1;

                        // Ngày
                        DateTime? creatDate = detail["CreatDate"] != null
                            ? Convert.ToDateTime(detail["CreatDate"])
                            : null;

                        sheet.Cells[currentRow, 3].Value = creatDate?.ToString("dd/MM/yyyy") ?? "";

                        // Mã NCC
                        sheet.Cells[currentRow, 6].Value = detail["CodeNCC"]?.ToString()?.Trim();

                        // Tên NCC
                        sheet.Cells[currentRow, 7].Value = detail["NameNCC"]?.ToString()?.Trim();

                        // Ghi chú
                        sheet.Cells[currentRow, 9].Value = detail["Note"]?.ToString()?.Trim();

                        // Product
                        sheet.Cells[currentRow, 12].Value = detail["ProductCode"]?.ToString()?.Trim();
                        sheet.Cells[currentRow, 13].Value = detail["ProductName"]?.ToString()?.Trim();

                        // Cột 14
                        if (warehouseCode == "HN")
                            sheet.Cells[currentRow, 14].Value = detail["ProductGroupName"]?.ToString()?.Trim();
                        else
                            sheet.Cells[currentRow, 14].Value = detail["WarehouseName"]?.ToString()?.Trim();

                        // Đơn vị
                        sheet.Cells[currentRow, 18].Value = detail["Unit"]?.ToString()?.Trim();

                        // Số lượng
                        int qty = int.TryParse(detail["Qty"]?.ToString(), out int q1) ? q1 : 0;
                        sheet.Cells[currentRow, 19].Value = qty;

                        currentRow++;
                    }

                    // XÓA 2 ROW TEMPLATE ban đầu
                    sheet.DeleteRow(startRow - 1, 1);

                    // AutoFit
                    sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

                    // TRẢ FILE
                    byte[] fileBytes = package.GetAsByteArray();
                    string fileName = $"{billImportCode}_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.xlsx";

                    return File(
                        fileBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName
                    );
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }
        [HttpGet("get-bill-import-detail")]
        public IActionResult GetdetailByIDS(List<int> bids)
        {
            try
            {

                string ids = string.Join(",", bids);
                var data = SQLHelper<dynamic>.ProcedureToList("spGetBillImportDetail", ["@ID"], [ids]);
                var dataDetail = SQLHelper<dynamic>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(dataDetail, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));

            }
        }
        private async Task UpdateInventoryProject(BillImportDetailDTO detail, BillImport billImport)
        {
            Employee em = _employeeRepo.GetAll(x => x.UserID == billImport.DeliverID).FirstOrDefault() ?? new Employee();
            if (detail.IsNotKeep == true)
            {
                //không giữ=> xóa khỏi kho giữ
                if (detail.InventoryProjectID.HasValue && detail.InventoryProjectID > 0)
                {
                    var existingInv = await _inventoryProjectRepo.GetByIDAsync(detail.InventoryProjectID.Value);
                    if (existingInv != null && existingInv.ID > 0)
                    {
                        existingInv.IsDeleted = true;
                        await _inventoryProjectRepo.UpdateAsync(existingInv);
                    }
                }
                return;
            }

            // Chỉ áp dụng cho phiếu nhập (BillTypeNew == 0)
            if (billImport.BillTypeNew != 0) return;

            // Nếu không có Project hoặc POKHList thì bỏ qua
            if ((detail.ProjectID ?? 0) <= 0 && (detail.POKHList?.Count ?? 0) == 0) return;

            decimal quantityReal = detail.Qty ?? 0;
            decimal quantityRequest = detail.QuantityRequestBuy ?? 0;
            decimal quantityKeep = quantityReal;

            // Nếu có quantityRequest, lấy min giữa thực nhập và yêu cầu
            if (quantityReal > 0 && quantityRequest > 0)
            {
                quantityKeep = Math.Min(quantityReal, quantityRequest);
            }

            // Nếu có POKHList, phân bổ quantityKeep theo tỷ lệ, trả nợ parent
            if (detail.POKHList != null && detail.POKHList.Count > 0)
            {
                decimal totalPokhQty = detail.POKHList.Sum(x => x.QuantityRequest);
                foreach (var item in detail.POKHList)
                {
                    decimal proportion = totalPokhQty > 0 ? item.QuantityRequest / totalPokhQty : 0;
                    decimal thisQuantityKeep = proportion * quantityKeep;

                    if (thisQuantityKeep <= 0) continue;

                    // Trả nợ parent trước khi tạo/update mới
                    decimal adjustedQty = await UpdateReturnQuantityLoan(item.POKHDetailID, thisQuantityKeep);

                    // Tìm InventoryProject root theo POKHDetailID
                    var existingRoot = _inventoryProjectRepo.GetAll(x =>
                            x.POKHDetailID == item.POKHDetailID &&
                            x.ParentID == 0 &&
                            x.IsDeleted == false)
                        .FirstOrDefault();

                    InventoryProject invProject;
                    if (existingRoot == null)
                    {
                        // Tạo mới
                        invProject = new InventoryProject
                        {
                            ProjectID = detail.ProjectID,
                            ProductSaleID = detail.ProductID,
                            WarehouseID = billImport.WarehouseID,
                            Quantity = adjustedQty,
                            QuantityOrigin = adjustedQty,
                            Note = detail.Note,
                            POKHDetailID = item.POKHDetailID,
                            CustomerID = detail.CustomerID,
                            EmployeeID = em.ID,
                            IsDeleted = false,
                            ParentID = 0
                        };

                        await _inventoryProjectRepo.CreateAsync(invProject);
                    }
                    else
                    {
                        // Update root
                        existingRoot.Quantity = adjustedQty;
                        existingRoot.QuantityOrigin = adjustedQty;
                        existingRoot.Note = detail.Note;
                        existingRoot.EmployeeID = em.ID;
                        existingRoot.CustomerID = detail.CustomerID;
                        existingRoot.IsDeleted = adjustedQty <= 0;

                        await _inventoryProjectRepo.UpdateAsync(existingRoot);
                        invProject = existingRoot;
                    }

                    if (invProject.ID > 0 && adjustedQty > 0)
                    {
                        detail.InventoryProjectID ??= invProject.ID;
                    }
                }
            }
            else
            {
                // Nếu không có POKHList, tạo/update duy nhất 1 InventoryProject
                decimal thisQuantityKeep = quantityKeep;
                if (thisQuantityKeep <= 0) return;

                // Fix: Chỉ query nếu InventoryProjectID > 0
                var existing = detail.InventoryProjectID > 0
                    ? await _inventoryProjectRepo.GetByIDAsync(detail.InventoryProjectID.Value)
                    : null;

                InventoryProject invProject;
                if (existing == null || existing.ID <= 0)
                {

                    invProject = new InventoryProject
                    {
                        ProjectID = detail.ProjectID,
                        ProductSaleID = detail.ProductID,
                        WarehouseID = billImport.WarehouseID,
                        Quantity = thisQuantityKeep,
                        QuantityOrigin = thisQuantityKeep,
                        Note = detail.Note,
                        CustomerID = detail.CustomerID,
                        EmployeeID = em.ID,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    await _inventoryProjectRepo.CreateAsync(invProject);
                }
                else
                {
                    existing.Quantity = thisQuantityKeep;
                    existing.QuantityOrigin = thisQuantityKeep;
                    existing.Note = detail.Note;
                    existing.EmployeeID = em.ID;
                    existing.CustomerID = detail.CustomerID;
                    existing.IsDeleted = thisQuantityKeep <= 0;
                    existing.UpdatedDate = DateTime.Now;
                    await _inventoryProjectRepo.UpdateAsync(existing);
                    invProject = existing;
                }

                detail.InventoryProjectID ??= invProject.ID;
            }
        }

        private async Task<decimal> UpdateReturnQuantityLoan(int pokhDetailId, decimal quantityKeep)
        {
            if (pokhDetailId <= 0) return quantityKeep;

            var inventoryProjects = _inventoryProjectRepo.GetAll(x =>
                x.POKHDetailID == pokhDetailId &&
                x.IsDeleted == false &&
                x.ParentID > 0)
                .OrderBy(x => x.ID)
                .ToList();

            foreach (var item in inventoryProjects)
            {
                if (quantityKeep <= 0) break;

                var parent = await _inventoryProjectRepo.GetByIDAsync(item.ParentID ?? 0);
                if (parent == null) continue;

                decimal quantityLoan = (parent.QuantityOrigin ?? 0) - (parent.Quantity ?? 0);
                if (quantityLoan <= 0) continue;

                decimal returnQty = Math.Min(quantityLoan, quantityKeep);
                parent.Quantity += returnQty;

                await _inventoryProjectRepo.UpdateAsync(parent);
                quantityKeep -= returnQty;
            }

            return quantityKeep;
        }
        private async Task UpdateDocumentImport(int billImportID, List<DocumentImportPONCC> docImport)
        {
            foreach (var item in docImport)
            {

                if (item.ID <= 0)
                {
                    item.BillImportID = billImportID;
                    await _documentImportPONCCRepo.CreateAsync(item);
                }
                else await _documentImportPONCCRepo.UpdateAsync(item);
            }
        }
        [HttpGet("get-phieu-tra")]
        public IActionResult GetPhieuTra(int productID)
        {
            var dt = SQLHelper<dynamic>.ProcedureToList("spGetBillReturn", ["@ProductID"], [productID]);
            var data = SQLHelper<dynamic>.GetListData(dt, 0);
            return Ok(ApiResponseFactory.Success(data, ""));
        }
    }
}
