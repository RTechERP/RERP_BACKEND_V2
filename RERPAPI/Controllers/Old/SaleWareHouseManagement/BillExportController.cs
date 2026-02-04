using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;
using RERPAPI.Repo.GenericEntity.Technical;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
using ZXing.Common;


namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BillExportController : ControllerBase
    {
        private readonly ProductGroupRepo _productgroupRepo;
        private readonly BillDocumentExportRepo _billdocumentexportRepo;
        private readonly BillExportDetailRepo _billExportDetailRepo;
        private readonly BillExportRepo _billexportRepo;
        private readonly InventoryRepo _inventoryRepo;
        private readonly InventoryProjectExportRepo _inventoryprojectexportRepo;
        private readonly BillExportDetailSerialNumberRepo _billexportdetailserialnumberRepo;
        private readonly DocumentExportRepo _documentexportRepo;
        private readonly BillExportLogRepo _billexportlogRepo;
        private readonly ProjectRepo _projectRepo;
        private readonly HistoryDeleteBillRepo _historyDeleteBillRepo;
        private readonly BillExportDetailSerialNumberRepo _billExportDetailSerialNumberRepo;
        private readonly WarehouseRepo _warehouseRepo;
        private readonly BillExportDetailSerialNumberRepo billExportDetailSerialNumberRepo;
        private readonly InventoryProjectRepo _inventoryProjectRepo;
        private readonly ProductSaleRepo _productSaleRepo;
        private readonly CustomerRepo _customerRepo;
        private readonly Repo.GenericEntity.AddressStockRepo _addressStockRepo;
        private readonly SupplierSaleRepo _supplierSaleRepo;
        private readonly UserRepo _userRepo;
        private readonly EmployeeRepo _employeeRepo;
        private readonly DepartmentRepo _departmentRepo;
        private readonly IConfiguration _configuration;
        private readonly POKHRepo _pokhRepo;
        private readonly POKHFilesRepo _pokhFilesRepo;


        public BillExportController(
            ProductGroupRepo productgroupRepo,
            BillDocumentExportRepo billdocumentexportRepo,
            BillExportDetailRepo billExportDetailRepo,
            BillExportRepo billexportRepo,
            InventoryRepo inventoryRepo,
            InventoryProjectExportRepo inventoryprojectexportRepo,
            BillExportDetailSerialNumberRepo billExportDetailSerialNumberRepoInjected,
            DocumentExportRepo documentexportRepo,
            BillExportLogRepo billexportlogRepo,
            ProjectRepo projectRepo,
            HistoryDeleteBillRepo historyDeleteBillRepo,
            WarehouseRepo warehouseRepo, InventoryProjectRepo inventoryProjectRepo, ProductSaleRepo productSaleRepo, Repo.GenericEntity.AddressStockRepo addressStockRepo, CustomerRepo customerRepo, SupplierSaleRepo supplierSaleRepo, UserRepo userRepo, IConfiguration configuration, DepartmentRepo departmentRepo, EmployeeRepo employeeRepo, POKHRepo pokhRepo, POKHFilesRepo pokhFilesRepo)
        {
            _productgroupRepo = productgroupRepo;
            _billdocumentexportRepo = billdocumentexportRepo;
            _billExportDetailRepo = billExportDetailRepo;
            _billexportRepo = billexportRepo;
            _inventoryRepo = inventoryRepo;
            _inventoryprojectexportRepo = inventoryprojectexportRepo;
            _billexportdetailserialnumberRepo = billExportDetailSerialNumberRepoInjected;
            _documentexportRepo = documentexportRepo;
            _billexportlogRepo = billexportlogRepo;
            _projectRepo = projectRepo;
            _historyDeleteBillRepo = historyDeleteBillRepo;
            _billExportDetailSerialNumberRepo = billExportDetailSerialNumberRepoInjected;
            _warehouseRepo = warehouseRepo;
            billExportDetailSerialNumberRepo = billExportDetailSerialNumberRepoInjected;
            _inventoryProjectRepo = inventoryProjectRepo;
            _productSaleRepo = productSaleRepo;
            _addressStockRepo = addressStockRepo;
            _customerRepo = customerRepo;
            _supplierSaleRepo = supplierSaleRepo;
            _userRepo = userRepo;
            _configuration = configuration;
            _departmentRepo = departmentRepo;
            _employeeRepo = employeeRepo;
            _pokhRepo = pokhRepo;
            _pokhFilesRepo = pokhFilesRepo;
        }
        [HttpGet("get-all-project")]
        public IActionResult getAllProject()
        {
            try
            {
                var result = _projectRepo.GetAll(x => x.IsDeleted == false).OrderByDescending(x => x.ID);
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("")]
        [RequiresPermission("N27,N29,N50,N1,N36,N52,N35,N33,N34,N69")]
        public IActionResult getBillExport([FromBody] BillExportParamRequest filter)
        {
            try
            {

                if (filter.checkedAll == true)
                {
                    filter.DateStart = new DateTime(1990, 01, 01);
                    filter.DateEnd = new DateTime(9999, 01, 01);
                }
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                       "spGetBillExport_New", new string[] { "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@Status", "@KhoType", "@FilterText", "@WarehouseCode" },
                    new object[] { filter.PageNumber, filter.PageSize, filter.DateStart, filter.DateEnd, filter.Status, filter.KhoType, filter.FilterText, filter.WarehouseCode }
                   );
                /* List<dynamic> billList = result[0]; // dữ liệu hóa đơn*/
                int totalPage = 0;
                if (result.Count > 0)
                {
                    if (result[0].Count > 0)
                    {
                        totalPage = result[0][0].TotalPage;
                    }
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
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-product")]
        public IActionResult getOptionProduct(string warehouseCode, int productGroupID)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                       "spGetInventory", new string[] { "@ID", "@Find", "@WarehouseCode" },
                    new object[] { productGroupID, "", warehouseCode }
                   );
                var data = SQLHelper<object>.GetListData(result, 0);

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-product-group")]
        public IActionResult getProductGroup(bool isAdmin, int departmentID)
        {
            try
            {
                List<ProductGroup> listPG;

                //if (isAdmin)
                //{
                //    if (departmentID == 6)
                //    {
                //        listPG = _productgroupRepo.GetAll(x => x.ProductGroupID == "C" && x.IsVisible == true);
                //    }
                //    else
                //    {
                //        listPG = _productgroupRepo.GetAll(x => x.ProductGroupID != "C" && x.IsVisible == true);
                //    }
                //}
                //else
                //{
                // Nếu không phải admin, có thể xử lý mặc định hoặc trả toàn bộ danh sách chẳng hạn
                listPG = _productgroupRepo.GetAll(x => x.IsVisible == true);
                //}

                return Ok(ApiResponseFactory.Success(listPG, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        //[RequiresPermission("N27,N1,N33,N34,N69")]
        public async Task<IActionResult> SaveDataBillExport([FromBody] BillExportDTO dto)
        {
            try
            {
                var (success, message, billExportId) = await _billexportRepo.SaveBillExportWithDetails(dto);

                if (success)
                {
                    return Ok(new
                    {
                        status = 1,
                        message = message,
                        data = new { BillExportID = billExportId }
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = message
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = $"Lỗi server: {ex.Message}"
                });
            }
        }

        [HttpGet("{id}")]
        public IActionResult getBillExportByID(int id)
        {
            try
            {
                BillExport result = _billexportRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("by-billexport/{billExportID}")]
        public async Task<IActionResult> GetByBillExportID(int billExportID)
        {
            try
            {
                var billImport = _billexportRepo.GetBillImportByBillExportID(billExportID);
                if (billImport == null) return BadRequest(new { status = 0, message = "Không tìm thấy phiếu nhập được link!" });
                return Ok(ApiResponseFactory.Success(billImport));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));

            }
        }
        [HttpPost("delete-bill-export")]
        [RequiresPermission("N27,N1,N33,N34,N69")]
        public async Task<IActionResult> DeleteBillExport([FromBody] BillExport billExport)
        {
            try
            {
                var rs = await _billexportRepo.HandleDeleteBill(billExport);

                if (!rs.Success)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, rs.Message));
                }

                return Ok(ApiResponseFactory.Success(billExport, rs.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("approved")]
        [RequiresPermission("N11,N50,N1,N18")]
        public async Task<IActionResult> Approved([FromBody] BillExport billExport, bool isapproved)
        {
            try
            {
                if (billExport.IsApproved == false && isapproved == false)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"{billExport.Code} chưa nhận chứng từ!"));
                }
                billExport.IsApproved = isapproved;
                await _billexportRepo.UpdateAsync(billExport);

                SQLHelper<dynamic>.ExcuteProcedure("spCalculateExport_New", new string[] { "@ID", "@WarehouseID" }, new object[] { billExport.ID, billExport.WarehouseID });
                SQLHelper<dynamic>.ExcuteProcedure("spCalculatePOKH", new string[] { "@IDMaster" }, new object[] { billExport.ID });

                BillExportLog log = new BillExportLog()
                {
                    BillExportID = billExport.ID,
                    StatusBill = isapproved,
                    DateStatus = DateTime.Now,
                };
                await _billexportlogRepo.CreateAsync(log);
                return Ok(ApiResponseFactory.Success(billExport, $"{billExport.Code} {(isapproved ? "đã được nhận chứng từ thành công!" : "đã được hủy nhận chứng từ!")}"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #region
        [HttpGet("get-bill-code")]

        public ActionResult<string> LoadBillNumber(int billTypeId, int? billId = null, int? currentStatus = null, string currentCode = null)
        {
            string billCode = _billexportRepo.GetBillCode(billTypeId);

            return Ok(new { data = billCode });
        }
        #endregion
        //đã xuất kho 
        [HttpPost("shipped-out")]
        public async Task<IActionResult> ExportWareHouse([FromBody] BillExport billExport)
        {
            try
            {
                if (billExport.Status == 0 || billExport.Status == 1)
                {
                    billExport.Status = 2;
                    await _billexportRepo.UpdateAsync(billExport);
                }
                else
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = $"Vui lòng kiểm tra lại trạng thái phiếu xuất {billExport.Code} "

                    });
                }
                return Ok(new
                {
                    status = 1,
                    message = $"{billExport.Code} Đã cập nhật!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("BillImportID/{billIDs}")]

        public IActionResult GetdetailByIDS(string billIDs)
        {
            try
            {
                var data = SQLHelper<dynamic>.ProcedureToList("spGetBillImportDetail", ["@ID"], [billIDs]);
                var dataDetail = SQLHelper<dynamic>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(dataDetail, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        private static string GetString(IDictionary<string, object> row, string key)
        {
            if (!row.ContainsKey(key) || row[key] == null || row[key] == DBNull.Value)
                return "";
            return row[key].ToString();
        }

        private static int GetInt(IDictionary<string, object> row, string key)
        {
            if (!row.ContainsKey(key) || row[key] == null || row[key] == DBNull.Value)
                return 0;
            return Convert.ToInt32(row[key]);
        }

        private static decimal GetDecimal(IDictionary<string, object> row, string key)
        {
            if (!row.ContainsKey(key) || row[key] == null || row[key] == DBNull.Value)
                return 0;
            return Convert.ToDecimal(row[key]);
        }


        [HttpPost("export-excel")]
        public IActionResult ExportExcel([FromBody] List<int> listId, int type)
        {
            try
            {
                if (listId == null || !listId.Any())
                    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách Phiếu rỗng"));

                string rootPath = _configuration.GetValue<string>("PathTemplate");
                string templatePath = Path.Combine(rootPath, "ExportExcel", "PhieuXuatSale.xlsx");

                using var zipStream = new MemoryStream();
                using (var archive = new System.IO.Compression.ZipArchive(zipStream, System.IO.Compression.ZipArchiveMode.Create, true))
                {
                    foreach (var id in listId)
                    {
                        #region ===== LOAD DATA =====
                        var masterSets = SQLHelper<dynamic>.ProcedureToList(
                            "spGetExportExcel",
                            new[] { "@ID" },
                            new object[] { id }
                        );

                        if (masterSets == null || masterSets[0].Count == 0)
                            continue;

                        var master = (IDictionary<string, object>)masterSets[0][0];

                        var detailSets = SQLHelper<dynamic>.ProcedureToList(
                            "spGetBillExportDetail",
                            new[] { "@BillID" },
                            new object[] { id }
                        );

                        if (detailSets == null || detailSets.Count == 0)
                            continue;

                        var details = detailSets[0].Cast<IDictionary<string, object>>().ToList();
                        #endregion

                        #region ===== CREATE WORKBOOK =====
                        using var workbook = new XLWorkbook(templatePath);
                        var sheet = workbook.Worksheet(1);
                        #endregion

                        #region ===== MAP MASTER =====
                        sheet.Cell(6, 1).Value = "Số: " + GetString(master, "Code");

                        string fullName = GetString(master, "FullName").Trim();
                        int userId = GetInt(master, "UserID");

                        int departmentID = _employeeRepo.GetAll(x => x.UserID == userId).FirstOrDefault()?.DepartmentID ?? 0;
                        string department = _departmentRepo.GetByID(departmentID)?.Name ?? "";

                        sheet.Cell(9, 4).Value = string.IsNullOrWhiteSpace(department)
                            ? fullName
                            : $"{fullName} / Phòng {department}";

                        string customer = GetString(master, "CustomerName").Trim();
                        string supplier = GetString(master, "NameNCC").Trim();

                        sheet.Cell(10, 3).Value = "'- Khách hàng/Nhà cung cấp:";
                        sheet.Cell(10, 4).Value = string.IsNullOrEmpty(customer) ? supplier : customer;

                        sheet.Cell(11, 4).Value = GetString(master, "Address");
                        sheet.Cell(12, 4).Value = GetString(master, "AddressStock");

                        sheet.Cell(25, 3).Value = GetString(master, "FullNameSender");
                        sheet.Cell(25, 9).Value = GetString(master, "FullName");

                        //if (GetInt(master, "WarehouseID") == 1)
                        //    sheet.Cell(15, 10).Value = "Loại vật tư";

                        if (DateTime.TryParse(GetString(master, "CreatDate"), out var d))
                            sheet.Cell(18, 9).Value = $"Ngày {d:dd} tháng {d:MM} năm {d:yyyy}";
                        #endregion

                        #region ===== MAP DETAIL =====
                        int excelRow = 15;
                        int stt = 1;

                        for (int i = details.Count - 1; i >= 0; i--)
                        {
                            int parentId = GetInt(details[i], "ParentID");
                            if (type == 1 && parentId != 0) continue;

                            sheet.Cell(excelRow, 1).Value = stt++;
                            sheet.Cell(excelRow, 2).Value = GetString(details[i], "ProductNewCode");
                            sheet.Cell(excelRow, 3).Value = GetString(details[i], "ProductCode");
                            sheet.Cell(excelRow, 4).Value = GetString(details[i], "ProductFullName");
                            sheet.Cell(excelRow, 5).Value = GetString(details[i], "ProductName");
                            sheet.Cell(excelRow, 6).Value = GetString(details[i], "Unit");
                            sheet.Cell(excelRow, 7).Value = GetDecimal(details[i], "Qty");
                            sheet.Cell(excelRow, 8).Value = GetString(details[i], "ProjectCodeText");
                            sheet.Cell(excelRow, 9).Value = GetString(details[i], "ProjectNameText");
                            sheet.Cell(excelRow, 10).Value = GetString(details[i], "ProductTypeText");
                            sheet.Cell(excelRow, 11).Value = GetDecimal(details[i], "UnitPricePOKH");
                            sheet.Cell(excelRow, 12).Value = GetDecimal(details[i], "UnitPricePurchase");

                            sheet.Cell(excelRow, 13).Value =
                                GetInt(master, "WarehouseID") == 1
                                    ? GetString(details[i], "ProductGroupName")
                                    : GetString(details[i], "WarehouseName");

                            string note = GetString(details[i], "Note");
                            sheet.Cell(excelRow, 14).Value = note.StartsWith("=") ? $"'{note}" : note;

                            sheet.Row(excelRow).InsertRowsBelow(1);
                            excelRow++;
                        }
                        //sheet.Row(excelRow + details.Count).Delete();
                        sheet.Row(excelRow).Delete();
                        sheet.Row(excelRow).Delete();
                        #endregion
                        //#region ===== QR CODE =====
                        string qrText = master["Code"]?.ToString();

                        var writer = new BarcodeWriterPixelData
                        {
                            Format = BarcodeFormat.QR_CODE,
                            Options = new EncodingOptions { Width = 250, Height = 250 }
                        };

                        var pixelData = writer.Write(qrText);
                        using var bmp = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb);

                        var data = bmp.LockBits(
                            new Rectangle(0, 0, bmp.Width, bmp.Height),
                            ImageLockMode.WriteOnly,
                            bmp.PixelFormat);

                        System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, data.Scan0, pixelData.Pixels.Length);
                        bmp.UnlockBits(data);

                        string tempPath = Path.GetTempFileName();
                        bmp.Save(tempPath, ImageFormat.Png);

                        sheet.AddPicture(tempPath)
                             .MoveTo(sheet.Cell(1, 11), 20, 10)
                             .WithSize(150, 150);

                        System.IO.File.Delete(tempPath);
                        using var excelStream = new MemoryStream();
                        workbook.SaveAs(excelStream);
                        excelStream.Position = 0;

                        string fileName = $"{GetString(master, "Code")}.xlsx";
                        var entry = archive.CreateEntry(fileName);

                        using var entryStream = entry.Open();
                        excelStream.CopyTo(entryStream);
                    }
                }

                zipStream.Position = 0;
                return File(
                    zipStream.ToArray(),
                    "application/zip",
                    $"PhieuXuat_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.zip"
                );
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("bill-export-synthetic")]
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
                  "spGetBillExportSynthetic",
                 new string[] { "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@Status", "@KhoType", "@FilterText", "@WarehouseCode", "@IsDeleted" },
                    new object[] { filter.PageNumber, filter.PageSize, filter.DateStart, filter.DateEnd, filter.Status, filter.KhoType, filter.FilterText, filter.WarehouseCode, filter.IsDeleted }
                   );
                var data = SQLHelper<dynamic>.GetListData(result, 0);
                return Ok(new
                {
                    status = 1,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

        [HttpGet("scan")]
        public IActionResult ScanQRCode(string code, int warehouseId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                    return BadRequest(ApiResponseFactory.Fail(null, "Mã QR không được để trống."));

                // 1. Tìm phiếu có mã code
                var bills = _billexportRepo.GetAll()
                    .Where(b => b.Code == code && b.IsDeleted != true)
                    .ToList();
                var bill = bills.FirstOrDefault();

                if (bill == null)
                    return BadRequest(new { status = 0, message = $"Không tìm thấy phiếu với mã {code}." });

                // 2. Kiểm tra nếu là phiếu không được quét
                string tableName = typeof(BillExport).Name.Replace("Model", "");
                int billTypeNew = tableName == "BillImport" ? 4 : tableName == "BillImportTechnical" ? 5 : 0;

                if (billTypeNew != 0)
                {
                    var check = _billexportRepo.GetAll().Where(b => b.Code == code && b.IsDeleted != true).ToList()
                        .Where(x => x.Status == billTypeNew)
                        .FirstOrDefault();

                    if (check != null)
                        return BadRequest(ApiResponseFactory.Fail(null, "Không thể quét phiếu loại Yêu cầu nhập kho."));
                }

                // 3. Gọi store procedure lấy chi tiết phiếu
                var result = SQLHelper<BillExport>.ProcedureToList(
                    "spGetBillExportScanQR",
                    new string[] { "@FilterText", "@WareHouseId" },
                    new object[] { code, warehouseId }
                );

                var detailList = SQLHelper<BillExport>.GetListData(result, 0); // lấy bảng đầu tiên

                if (detailList == null || detailList.Count == 0)
                {
                    return BadRequest(new { status = 0, message = "Không có chi tiết phiếu trong kho." });
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
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // hàm lấy danh sách vật tư theo dự án 

        [HttpPost("get-product-project")]

        public IActionResult getproductProject(GetListProductByProjectPram filter)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                    "spGetListProductImportExportByProjectID_New",
                    new string[] { "@projectId", "@projectCode", "@WarehouseCode" },
                    new object[] { filter.projectID, filter.projectCode, filter.WarehouseCode }
                    );
                var dt = SQLHelper<object>.GetListData(result, 0);
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(result, 0)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // GET: api/BillExport/get-customers
        [HttpGet("get-customers")]
        public IActionResult GetCustomers()
        {
            try
            {
                var customers = _customerRepo.GetAll(x => x.IsDeleted != true);
                return Ok(new
                {
                    status = 1,
                    data = customers
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // GET: api/BillExport/get-users
        [HttpGet("get-users")]
        public IActionResult GetUsers()
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                    "spGetUsersHistoryProductRTC",
                    new string[] { "@UsersID" },
                    new object[] { 0 }
                );
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(result, 0)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // GET: api/BillExport/get-senders
        [HttpGet("get-senders")]
        public IActionResult GetSenders()
        {
            try
            {
                var senders = _userRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = senders
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // GET: api/BillExport/get-suppliers
        [HttpGet("get-suppliers")]
        public IActionResult GetSuppliers()
        {
            try
            {
                var suppliers = _supplierSaleRepo.GetAll(x => x.IsDeleted != true);
                return Ok(new
                {
                    status = 1,
                    data = suppliers
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // GET: api/BillExport/get-address-stock/{customerId}
        [HttpGet("get-address-stock/{customerId}")]
        public IActionResult GetAddressStock(int customerId)
        {
            try
            {
                var addressStock = _addressStockRepo.GetAll(x => x.CustomerID == customerId).FirstOrDefault();
                if (addressStock == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy địa chỉ khách hàng"));
                }
                return Ok(ApiResponseFactory.Success(addressStock, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // GET: api/BillExport/get-bill-detail/{billId}
        [HttpGet("get-bill-detail/{billId}")]
        public IActionResult GetBillExportDetail(int billId)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                    "spGetBillExportDetail",
                    new string[] { "@BillID" },
                    new object[] { billId }
                );
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(result, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // POST: api/BillExport/get-bill-import-detail
        [HttpPost("get-bill-import-detail")]
        public IActionResult GetBillImportDetail([FromBody] List<int> billImportIds)
        {
            try
            {
                if (billImportIds == null || !billImportIds.Any())
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Không có phiếu nhập được trả về!"
                    });
                }

                string ids = string.Join(",", billImportIds);
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                    "spGetBillImportDetail",
                    new string[] { "@ID" },
                    new object[] { ids }
                );

                var data = SQLHelper<object>.GetListData(result, 0);

                // Reset IDs to 0 for conversion to export details
                foreach (var item in data)
                {
                    var dict = item as IDictionary<string, object>;
                    if (dict != null && dict.ContainsKey("ID"))
                    {
                        dict["ID"] = 0;
                    }
                }

                return Ok(new
                {
                    status = 1,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-all-inventorProjects")]
        public IActionResult GetInventoryProject(int warehouseId, int productId, int projectId = 0, int pokhDetailId = 0)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList("spGetInventoryProject",
                        new string[] { "@ProjectID", "@EmployeeID", "@ProductSaleID", "@Keyword", "@WarehouseID", "@POKHDetailID" },
                        new object[] { projectId, 0, productId, "", warehouseId, pokhDetailId }
                );

                return Ok(new
                {
                    status = 1,
                    inventoryProjects = SQLHelper<object>.GetListData(result, 0),
                    imports = result.Count > 1 ? SQLHelper<object>.GetListData(result, 1) : new List<object>(),
                    exports = result.Count > 2 ? SQLHelper<object>.GetListData(result, 2) : new List<object>(),
                    stock = result.Count > 3 ? SQLHelper<object>.GetListData(result, 3) : new List<object>()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // GET: api/BillExport/get-inventory-project
        [HttpGet("get-inventory-project")]
        public IActionResult GetInventoryProject(int warehouseId, int productId, int projectId = 0, int pokhDetailId = 0, int billExportDetailId = 0)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                    "spGetInventoryProjectImportExport",
                    new string[] { "@WarehouseID", "@ProductID", "@ProjectID", "@POKHDetailID", "@BillExportDetailID" },
                    new object[] { warehouseId, productId, projectId, pokhDetailId, billExportDetailId }
                );

                return Ok(new
                {
                    status = 1,
                    inventoryProjects = SQLHelper<object>.GetListData(result, 0),
                    imports = result.Count > 1 ? SQLHelper<object>.GetListData(result, 1) : new List<object>(),
                    exports = result.Count > 2 ? SQLHelper<object>.GetListData(result, 2) : new List<object>(),
                    stock = result.Count > 3 ? SQLHelper<object>.GetListData(result, 3) : new List<object>()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // GET: api/BillExport/get-product-group-warehouse
        [HttpGet("get-product-group-warehouse")]
        public IActionResult GetProductGroupWarehouse(int warehouseId, int productGroupId)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                    "spGetProductGroupWarehouse",
                    new string[] { "@WarehouseID", "@ProductGroupID" },
                    new object[] { warehouseId, productGroupId }
                );

                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(result, 0)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // GET: api/BillExport/get-warehouses
        [HttpGet("get-warehouses")]
        public IActionResult GetWarehouses()
        {
            try
            {
                var warehouses = _warehouseRepo.GetAll(x => x.IsDeleted == false || x.IsDeleted == null);
                return Ok(new
                {
                    status = 1,
                    data = warehouses
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // GET: api/BillExport/get-warehouse-by-code/{code}
        [HttpGet("get-warehouse-by-code/{code}")]
        public IActionResult GetWarehouseByCode(string code)
        {
            try
            {
                var warehouse = _warehouseRepo.GetAll(w => w.WarehouseCode == code.Trim()).FirstOrDefault();
                if (warehouse == null)
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Warehouse not found"
                    });
                }

                return Ok(new
                {
                    status = 1,
                    data = warehouse
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("excel-kt")]
        public IActionResult ExportExcelKT(int id, string warehouseCode)
        {
            try
            {
                // Validate warehouseCode
                if (string.IsNullOrEmpty(warehouseCode))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng truyền mã kho (warehouseCode)"));
                }

                // Lấy dữ liệu từ stored procedure
                List<List<dynamic>> resultSets = SQLHelper<dynamic>.ProcedureToList(
                    "spGetExportExcel",
                    new string[] { "@ID" },
                    new object[] { id }
                );

                if (resultSets == null || resultSets.Count == 0 || resultSets[0].Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu từ spGetExportExcel"));
                }

                var allData = resultSets[0];

                if (allData.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy dữ liệu"));
                }

                // Lấy đường dẫn template
                var path = _configuration.GetValue<string>("PathTemplate");
                if (path == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy đường dẫn server để lấy mẫu xuất!"));
                }

                string templatePath = Path.Combine(path, "ExportExcel", "FormXuatKho.xlsx");
                if (!System.IO.File.Exists(templatePath))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy file mẫu Excel"));
                }

                // Set license context cho EPPlus
                ExcelPackage.License.SetNonCommercialOrganization("RTC Technology VietNam");

                using (var package = new ExcelPackage(new FileInfo(templatePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Sheet đầu tiên

                    var firstRow = allData.FirstOrDefault();
                    string phieuCode = firstRow?.Code?.ToString() ?? "Unknown";

                    if (warehouseCode.ToUpper() == "HN")
                    {
                        worksheet.Cells[2, 14].Value = "Loại vật tư (*)";
                    }

                    int startRow = 3;
                    int dataCount = allData.Count;

                    if (dataCount > 1)
                    {
                        worksheet.InsertRow(startRow + 1, dataCount - 1, startRow);
                    }

                    int currentRow = startRow + dataCount - 1;
                    for (int i = dataCount - 1; i >= 0; i--)
                    {
                        var item = allData[i] as IDictionary<string, object>;
                        if (item == null) continue;

                        worksheet.Cells[currentRow, 1].Value = i + 1; // STT
                        worksheet.Cells[currentRow, 3].Value = item.ContainsKey("CreatDate")
                            ? item["CreatDate"]?.ToString() ?? ""
                            : "";
                        worksheet.Cells[currentRow, 8].Value = item.ContainsKey("CustomerCode")
                            ? item["CustomerCode"]?.ToString() ?? ""
                            : "";
                        worksheet.Cells[currentRow, 9].Value = item.ContainsKey("CustomerName")
                            ? item["CustomerName"]?.ToString() ?? ""
                            : "";
                        worksheet.Cells[currentRow, 12].Value = item.ContainsKey("Note")
                            ? item["Note"]?.ToString() ?? ""
                            : "";
                        worksheet.Cells[currentRow, 13].Value = item.ContainsKey("FullName")
                            ? item["FullName"]?.ToString() ?? ""
                            : "";

                        // Cột 14: Loại vật tư hoặc Kho
                        if (warehouseCode.ToUpper() == "HN")
                        {
                            worksheet.Cells[currentRow, 14].Value = item.ContainsKey("ProductGroupName")
                                ? item["ProductGroupName"]?.ToString() ?? ""
                                : "";
                        }
                        else
                        {
                            worksheet.Cells[currentRow, 14].Value = item.ContainsKey("WarehouseName")
                                ? item["WarehouseName"]?.ToString() ?? ""
                                : "";
                        }

                        worksheet.Cells[currentRow, 24].Value = item.ContainsKey("ProductCode")
                            ? item["ProductCode"]?.ToString() ?? ""
                            : "";
                        worksheet.Cells[currentRow, 25].Value = item.ContainsKey("ProductName")
                            ? item["ProductName"]?.ToString() ?? ""
                            : "";
                        worksheet.Cells[currentRow, 31].Value = item.ContainsKey("Unit")
                            ? item["Unit"]?.ToString() ?? ""
                            : "";
                        worksheet.Cells[currentRow, 32].Value = item.ContainsKey("Qty")
                            ? Convert.ToDecimal(item["Qty"] ?? 0)
                            : 0;

                        currentRow--;
                    }

                    // Xóa 2 dòng đầu
                    worksheet.DeleteRow(2);
                    //worksheet.DeleteRow(1);

                    // Auto-fit columns
                    worksheet.Cells.AutoFitColumns();

                    using (var stream = new MemoryStream())
                    {
                        package.SaveAs(stream);
                        stream.Position = 0;

                        string fileName = $"{phieuCode}_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.xlsx";
                        return File(
                            stream.ToArray(),
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileName
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //[HttpGet("{id:int}")]
        //public IActionResult GetWarehouses(int id)
        //{
        //    try
        //    {
        //        var billExport = _billexportRepo.GetByID(id);
        //        return Ok(ApiResponseFactory.Success(billExport));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}

        // GET: api/BillExport/download-pokh-file/{poNumber}/{fileName}
        [HttpGet("download-pokh-file/{poNumber}/{fileName}")]
        public IActionResult DownloadPOKHFile(string poNumber, string fileName)
        {
            try
            {
                // Find POKH by PONumber
                var pokh = _pokhRepo.GetAll(p => p.PONumber == poNumber && p.IsDeleted != true).FirstOrDefault();
                if (pokh == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy PO với số {poNumber}"));
                }

                // Find file by POKHID and FileName
                var pokhFile = _pokhFilesRepo.GetAll(f => f.POKHID == pokh.ID && f.FileName == fileName && f.IsDeleted != true).FirstOrDefault();
                if (pokhFile == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy file {fileName} trong PO {poNumber}"));
                }

                // Get file path from ServerPath
                string filePath = pokhFile.ServerPath;
                string path = Path.Combine(filePath, fileName);
                if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(path))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"File không tồn tại trên server: {fileName}"));
                }

                // Read file and return
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                string contentType = GetContentType(fileName);

                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-inventory-project-import-export")]
        public IActionResult GetInventoryProjectImportExport(
    int warehouseId,
    int productId,
    int projectId = 0,
    int pokhDetailId = 0,
    string billExportDetailIds = "")
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                    "spGetInventoryProjectImportExport",
                    new string[] { "@WarehouseID", "@ProductID", "@ProjectID", "@POKHDetailID", "@BillExportDetailID" },
                    new object[] { warehouseId, productId, projectId, pokhDetailId, billExportDetailIds }
                );

                return Ok(new
                {
                    status = 1,
                    inventoryProjects = SQLHelper<object>.GetListData(result, 0),
                    import = result.Count > 1 ? SQLHelper<object>.GetListData(result, 1) : new List<object>(),
                    export = result.Count > 2 ? SQLHelper<object>.GetListData(result, 2) : new List<object>(),
                    stock = result.Count > 3 ? SQLHelper<object>.GetListData(result, 3) : new List<object>()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // GET: api/BillExport/get-pokh-files/{poNumber}
        [HttpGet("get-pokh-files/{poNumber}")]
        public IActionResult GetPOKHFiles(string poNumber)
        {
            try
            {
                // Find POKH by PONumber
                var pokh = _pokhRepo.GetAll(p => p.PONumber == poNumber && p.IsDeleted != true).FirstOrDefault();
                if (pokh == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy PO với số {poNumber}"));
                }

                // Get all files for this POKH
                var files = _pokhFilesRepo.GetAll(f => f.POKHID == pokh.ID && f.IsDeleted != true).ToList();

                return Ok(ApiResponseFactory.Success(files, $"Tìm thấy {files.Count} file"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        private string GetContentType(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".txt" => "text/plain",
                ".zip" => "application/zip",
                ".rar" => "application/x-rar-compressed",
                _ => "application/octet-stream"
            };
        }
        [HttpPost("check-bill-code")]
        public IActionResult CheckBillCode([FromBody] string billCode)
        {
            try
            {
                bool isExists = _billexportRepo.GetAll(b => b.Code.ToLower().Trim() == billCode.ToLower().Trim() && b.IsDeleted == false).Any();
                return Ok(ApiResponseFactory.Success(isExists));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpGet("export-excel-file")]
        public async Task<IActionResult> ExportFile(int billExportId)
        {
            try
            {
                string rootPath = _configuration.GetValue<string>("PathTemplate");
                string templatePath = Path.Combine(rootPath, "ExportExcel", "PhieuXuatSale.xlsx");
                var paramMaster = new { ID = billExportId };
                var paramDetail = new { BillID = billExportId };

                var masters = await SqlDapper<object>.ProcedureToListTAsync("spGetExportExcel", paramMaster);
                //var master = _billexportRepo.GetByID(billExportId);
                if (masters == null || masters.Count() <= 0) return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy dữ liệu phiếu"));

                dynamic master = masters[0];

                var details = await SqlDapper<object>.ProcedureToListTAsync("spGetBillExportDetail", paramDetail);

                if (details == null || details.Count() <= 0) return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy dữ liệu chi tiết phiếu"));

                using (var workbook = new XLWorkbook(templatePath))
                {
                    var sheet = workbook.Worksheet(1);

                    #region ===== MAP MASTER =====
                    sheet.Cell(6, 1).Value = "Số: " + master.Code ?? "";

                    string fullname = master.FullName ?? "";
                    int userId;
                    int.TryParse(master.UserID?.ToString(), out userId);

                    int departmentid = _employeeRepo.GetByID(userId)?.DepartmentID ?? 0;
                    string department = _departmentRepo.GetByID(departmentid)?.Name ?? "";

                    sheet.Cell(9, 4).Value = string.IsNullOrWhiteSpace(department)
                        ? fullname
                        : $"{fullname} / phòng {department}";

                    string customer = master.CustomerName;
                    string supplier = master.NameNCC;

                    sheet.Cell(10, 3).Value = "'- khách hàng/nhà cung cấp:";
                    sheet.Cell(10, 4).Value = string.IsNullOrWhiteSpace(customer) ? supplier : customer;

                    sheet.Cell(11, 4).Value = master.Address ?? "";
                    sheet.Cell(12, 4).Value = master.AddressStock ?? "";

                    sheet.Cell(25, 3).Value = master.FullNameSender ?? "";
                    sheet.Cell(25, 9).Value = fullname;

                    //if (master.WarehouseID == 1)
                    //    sheet.Cell(15, 10).Value = "loại vật tư";

                    DateTime d;
                    if (DateTime.TryParse(GetString(master, "CreatDate"), out d))
                    {
                        sheet.Cell(18, 9).Value = $"ngày {d:dd} tháng {d:MM} năm {d:yyyy}";
                    }
                    #endregion

                    #region ===== MAP DETAIL =====
                    int excelRow = 15;
                    int stt = 1;

                    for (int i = details.Count - 1; i >= 0; i--)
                    {
                        dynamic dt = details[i];
                        sheet.Cell(excelRow, 1).Value = stt++;
                        sheet.Cell(excelRow, 2).Value = dt.ProductNewCode ?? "";
                        sheet.Cell(excelRow, 3).Value = dt.ProductCode ?? "";
                        sheet.Cell(excelRow, 4).Value = dt.ProductFullName ?? "";
                        sheet.Cell(excelRow, 5).Value = dt.ProductName ?? "";
                        sheet.Cell(excelRow, 6).Value = dt.Unit ?? "";
                        sheet.Cell(excelRow, 7).Value = dt.Qty ?? 0;
                        sheet.Cell(excelRow, 8).Value = dt.ProjectCodeText ?? "";
                        sheet.Cell(excelRow, 9).Value = dt.ProjectNameText ?? "";
                        sheet.Cell(excelRow, 10).Value = dt.ProductTypeText ?? "";
                        sheet.Cell(excelRow, 11).Value = dt.UnitPricePOKH ?? 0;
                        sheet.Cell(excelRow, 12).Value = dt.UnitPricePurchase ?? 0;

                        sheet.Cell(excelRow, 13).Value =
                            master.WarehouseID == 1
                                ? dt.ProductGroupName ?? ""
                                : dt.WarehouseName ?? "";

                        string note = dt.Note ?? "";
                        sheet.Cell(excelRow, 14).Value = note.StartsWith("=") ? $"'{note}" : note;

                        sheet.Row(excelRow).InsertRowsBelow(1);
                        excelRow++;
                    }
                    sheet.Row(excelRow).Delete();
                    sheet.Row(excelRow).Delete();
                    //sheet.Row(excelRow + details.Count - 1).Delete();
                    //sheet.Row(excelRow + details.Count - 1).Delete();
                    #endregion

                    #region ===== QR CODE =====
                    string qrText = master.Code ?? "";

                    var writer = new BarcodeWriterPixelData
                    {
                        Format = BarcodeFormat.QR_CODE,
                        Options = new EncodingOptions { Width = 250, Height = 250 }
                    };

                    var pixelData = writer.Write(qrText);
                    using var bmp = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb);

                    var data = bmp.LockBits(
                        new Rectangle(0, 0, bmp.Width, bmp.Height),
                        ImageLockMode.WriteOnly,
                        bmp.PixelFormat);

                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, data.Scan0, pixelData.Pixels.Length);
                    bmp.UnlockBits(data);

                    string tempPath = Path.GetTempFileName();
                    bmp.Save(tempPath, ImageFormat.Png);

                    sheet.AddPicture(tempPath)
                         .MoveTo(sheet.Cell(1, 11), 20, 10)
                         .WithSize(150, 150);
                    #endregion
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Position = 0;

                        string fileName = $"Phiếu xuất.xlsx";
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }

                }

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }


}