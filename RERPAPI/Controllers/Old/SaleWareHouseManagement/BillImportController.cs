using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
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
            BillImportDetailSerialNumberRepo billImportDetailSerialNumberRepo)
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
                string message = isapproved ? "nhận chứng từ" : "hủy nhận chứng từ";
                if (billImport.Status == false && isapproved == false)
                {
                    return Ok(new
                    {
                        status = 0,
                        message = $"{billImport.BillImportCode} chưa nhận chứng từ!",
                    });
                }
                if (billImport.BillTypeNew == 4)
                {
                    return Ok(new
                    {
                        status = 0,
                        message = $"Bạn không thể {message} cho phiếu Yêu cầu nhập kho!",
                    });
                }
                // Cập nhật trạng thái duyệt phiếu
                billImport.Status = isapproved;
                billImport.UnApprove = isapproved ? 1 : 2;
                await _billImportRepo.UpdateAsync(billImport);

                // Tính lại tồn kho và tình hình đơn hàng 
                SQLHelper<dynamic>.ExcuteProcedure("spCalculateImport_New", new string[] { "@ID", "@WarehouseID" }, new object[] { billImport.ID, billImport.WarehouseID });
                //SQLHelper<dynamic>.ExcuteProcedure("spUpdateTinhHinhDonHang",
                //         new string[] { "@BillImportID", "@IsApproved" },
                //         new object[] { billImport.ID, isapproved });
                //ghi log
                BillImportLog log = new BillImportLog()
                {
                    BillImportID = billImport.ID,
                    StatusBill = isapproved,
                    DateStatus = DateTime.Now,
                };
                await _billImportLogRepo.CreateAsync(log);
                return Ok(new
                {
                    status = 1,
                    message = $"Phiếu {billImport.BillImportCode} đã được {message} thành công!"
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
        [HttpPost("save-data")]
        public async Task<IActionResult> saveDataBillImport([FromBody] BillImportDTO dto)
        {
            try
            {
                // Perform validation
                if (dto.billImport != null)
                {
                    var (isValid, errorMessage) = await ValidateBillImport(dto);
                    if (!isValid)
                    {
                        return BadRequest(new
                        {
                            status = 0,
                            message = errorMessage
                        });
                    }
                }


                //xóa phiếu nhập: update 02/11/25
                if (dto.billImportDetail == null && dto.DeletedDetailIDs == null)
                {
                    await _billImportRepo.SaveBillImport(dto.billImport);
                    return Ok(new
                    {
                        status = 1,
                        message = "Đã xóa thành công phiếu " + dto.billImport.BillImportCode
                    });
                }

                var inventoryList = _inventoryRepo.GetAll().ToList();

                // Lưu phiếu nhập: update 02/11/25
                int billImportId = await _billImportRepo.SaveBillImport(dto.billImport);

                // Xóa chi tiết cũ nếu có : update 02/11/25
                if (dto.DeletedDetailIDs?.Any() == true)
                {
                    await _billImportDetailRepo.DeleteBillImportDetail(dto.DeletedDetailIDs);
                }

                // Lưu chi tiết: update 02/11/25
                await _billImportDetailRepo.SaveBillImportDetail(dto.billImportDetail, billImportId);

                //kiểm tra tồn kho 
                await _inventoryRepo.CheckInventoryForImport(dto.billImportDetail, dto.billImport);

                return Ok(new
                {
                    status = 1,
                    message = "Xử lý thành công",
                    data = dto
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
                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "BillImportSale.xlsx");
                if (!System.IO.File.Exists(templatePath))
                {
                    return BadRequest(new { status = 0, message = "Không tìm thấy file mẫu Excel" });
                }

                using (var workbook = new XLWorkbook(templatePath))
                {
                    var sheet = workbook.Worksheet(1);

                    // Mapping master data
                    sheet.Cell(6, 1).Value = $"Số: {masterData["BillImportCode"]?.ToString()?.Trim() ?? ""}";
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

                        string fileName = $"Phiếu nhập-{masterData["BillImportCode"]}_{DateTime.Now:dd_MM_yyyy}.xlsx";
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
    }
}
