using ClosedXML.Excel;
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
            IConfiguration configuration)
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
        /// <summary>
        /// duyệt,hủy duyệt phiếu nhập
        /// </summary>
        /// <param name="billExport"></param>
        /// <param name="isapproved"></param>
        /// <returns></returns>
        [HttpPost("approved")]
        public async Task<IActionResult> Approved([FromBody] BillImport billImport, bool isapproved)
        {
            try
            {
                string messageError;
                string message = isapproved ? "nhận chứng từ" : "hủy nhận chứng từ";
                if (billImport.Status == false && isapproved == false)
                {
                    messageError = $"{billImport.BillImportCode} chưa nhận chứng từ!";
                    throw new Exception(messageError);
                }
                if (billImport.BillTypeNew == 4)
                {
                    messageError = $"Bạn không thể {message} cho phiếu Yêu cầu nhập kho!";
                    throw new Exception(messageError);
                }
                // Cập nhật trạng thái duyệt phiếu
                billImport.Status = isapproved;
                billImport.UnApprove = isapproved ? 1 : 2;
                await _billImportRepo.UpdateAsync(billImport);

                // Tính lại tồn kho và tình hình đơn hàng 
                SQLHelper<dynamic>.ExcuteProcedure("spCalculateImport_New", new string[] { "@ID", "@WarehouseID" }, new object[] { billImport.ID, billImport.WarehouseID });
                SQLHelper<dynamic>.ExcuteProcedure("spUpdateTinhHinhDonHang",
                         new string[] { "@BillImportID", "@IsApproved" },
                         new object[] { billImport.ID, isapproved });
                //ghi log
                BillImportLog log = new BillImportLog()
                {
                    BillImportID = billImport.ID,
                    StatusBill = isapproved,
                    DateStatus = DateTime.Now,
                };
                await _billImportLogRepo.CreateAsync(log);
                return Ok(ApiResponseFactory.Success(null, $"Phiếu {billImport.BillImportCode} đã được {message} thành công!"));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
        //thêm sửa dữ liệu 
        //[HttpPost("save-data")]
        //public async Task<IActionResult> saveDataBillImport([FromBody] BillImportDTO dto)
        //{
        //    try
        //    {
        //        // Perform validation
        //        if (dto.billImport != null)
        //        {
        //            var (isValid, errorMessage) = await ValidateBillImport(dto);
        //            if (!isValid)
        //            {
        //                return BadRequest(new
        //                {
        //                    status = 0,
        //                    message = errorMessage
        //                });
        //            }
        //        }


        //        //xóa phiếu nhập: update 02/11/25
        //        if (dto.billImportDetail == null && dto.DeletedDetailIDs == null)
        //        {
        //            await _billImportRepo.SaveBillImport(dto.billImport);
        //            return Ok(new
        //            {
        //                status = 1,
        //                message = "Đã xóa thành công phiếu " + dto.billImport.BillImportCode
        //            });
        //        }

        //        var inventoryList = _inventoryRepo.GetAll().ToList();

        //        // Lưu phiếu nhập: update 02/11/25
        //        int billImportId = await _billImportRepo.SaveBillImport(dto.billImport);

        //        // Xóa chi tiết cũ nếu có : update 02/11/25
        //        if (dto.DeletedDetailIDs?.Any() == true)
        //        {
        //            await _billImportDetailRepo.DeleteBillImportDetail(dto.DeletedDetailIDs);
        //        }

        //        // Lưu chi tiết: update 02/11/25
        //        await _billImportDetailRepo.SaveBillImportDetail(dto.billImportDetail, billImportId);

        //        //kiểm tra tồn kho 
        //        await _inventoryRepo.CheckInventoryForImport(dto.billImportDetail, dto.billImport);

        //        return Ok(new
        //        {
        //            status = 1,
        //            message = "Xử lý thành công",
        //            data = dto
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

        //thêm sửa dữ liệu 
        [HttpPost("save-data")]
        public async Task<IActionResult> saveDataBillImport([FromBody] BillImportDTO dto)
        {

            try
            {
                // Perform validation
                var (isValid, errorMessage) = await ValidateBillImport(dto);
                if (!isValid)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, errorMessage));
                }
                if (dto.billImportDetail == null && dto.DeletedDetailIDs == null)
                {
                    // Cập nhật phiếu nhập
                    dto.billImport.UpdatedDate = DateTime.Now;
                    _billImportRepo.Update(dto.billImport);
                    return Ok(ApiResponseFactory.Success(null, "Đã xóa thành công phiếu " + dto.billImport.BillImportCode));
                }
                var inventoryList = _inventoryRepo.GetAll();

                // Tính TotalQty
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
                    // Thêm mới phiếu nhập
                    dto.billImport.BillDocumentImportType = 2;
                    dto.billImport.CreatedDate = DateTime.Now;
                    dto.billImport.BillTypeNew = 0;
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
                            CreatedDate = DateTime.Now,
                            Note = ""
                        };
                        await _billDocumentImportRepo.CreateAsync(billDocumentImport);
                    }
                }
                else
                {
                    // Cập nhật phiếu nhập
                    dto.billImport.UpdatedDate = DateTime.Now;
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
                            item.UpdatedDate = DateTime.Now;
                            _billImportDetailRepo.Update(item);
                            var invoiceLink = _invoiceLinkRepo.GetAll(p => p.BillImportDetailID == id);
                            if (invoiceLink.Any())
                            {
                                await _invoiceLinkRepo.DeleteByAttributeAsync("BillImportDetailID", id);
                            }
                        }
                    }
                }

                // Xử lý chi tiết (thêm mới hoặc cập nhật)
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
                    //serial
                    //// Update serial với BillExportDetailID
                    //if (detail.SerialNumber != null && detail.SerialNumber.Any())
                    //{
                    //    // Lấy serial từ SerialNumbers, kiểm tra CreatedDate gần nhất để tránh xung đột
                    //    var serialIds = detail.SerialNumber
                    //            .Split(',', StringSplitOptions.RemoveEmptyEntries) // tách theo dấu phẩy
                    //            .Select(x => int.Parse(x.Trim())) // chuyển sang int
                    //            .ToList();
                    //    var serials = _billImportDetailSerialNumberRepo.GetAll(s => serialIds.Contains(s.ID) &&
                    //                    s.IsDeleted != true &&
                    //                    s.BillImportDetailID == null)
                    //        ;

                    //    if (detail.Qty.HasValue && serials.Count != (int)detail.Qty)
                    //    {
                    //        return BadRequest(ApiResponseFactory.Fail(null, $"Số serial ({serials.Count}) không khớp Qty ({detail.Qty})"));
                    //    }
                    //    // Update SerialNumber nếu chưa có
                    //    if (string.IsNullOrEmpty(detail.SerialNumber))
                    //    {
                    //        detail.SerialNumber = serials.Any() ? string.Join(",", serials.Select(s => s.SerialNumber)) : null;
                    //        _billImportDetailRepo.Update(detail);
                    //    }

                    //    // Cập nhật BillExportDetailID
                    //    foreach (var serial in serials)
                    //    {
                    //        //.BillExportDetailID = detail.ID;
                    //        serial.UpdatedDate = DateTime.Now;
                    //        _billImportDetailSerialNumberRepo.Update(serial);
                    //    }
                    //}
                    // Xử lý tồn kho dự án
                    if (detail.ProjectID > 0)
                    {
                        var inv = _inventoryProjectRepo.GetByID(detail.InventoryProjectID ?? 0);
                        if (inv.ID > 0)
                        {
                            inv.Quantity += detail.Qty;
                            await _inventoryProjectRepo.UpdateAsync(inv);
                        }
                        else
                        {
                            await _inventoryProjectRepo.CreateAsync(new InventoryProject
                            {
                                ProjectID = detail.ProjectID,
                                ProductSaleID = detail.ProductID,
                                Quantity = detail.Qty,
                                CreatedDate = DateTime.Now
                            });
                        }
                    }

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

                return Ok(ApiResponseFactory.Success(dto, "Xử lý dữ liệu thành công!"));
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
    }
}
