using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
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
            WarehouseRepo warehouseRepo, InventoryProjectRepo inventoryProjectRepo, ProductSaleRepo productSaleRepo, Repo.GenericEntity.AddressStockRepo addressStockRepo, CustomerRepo customerRepo, SupplierSaleRepo supplierSaleRepo, UserRepo userRepo)
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
        }
        [HttpGet("get-all-project")]
        public IActionResult getAllProject()
        {
            try
            {
                var result = _projectRepo.GetAll().OrderByDescending(x => x.ID);
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("")]
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
        private async Task<(bool IsValid, string ErrorMessage)> ValidateBillExport(BillExportDTO dto)
        {
            // Validate Code
            if (string.IsNullOrWhiteSpace(dto.billExport?.Code))
            {
                return (false, "Xin hãy điền số phiếu.");
            }

            // Check for duplicate code
            var existingBill = _billexportRepo.GetAll(p => p.Code == dto.billExport.Code).FirstOrDefault();
            if (dto.billExport.ID > 0)
            {
                // For update: check if code exists for a different ID
                if (existingBill != null && existingBill.ID != dto.billExport.ID)
                {
                    return (false, "Số phiếu này đã tồn tại. Vui lòng chọn số phiếu khác!");
                }
            }
            else
            {
                // For create: check if code already exists
                if (existingBill != null)
                {
                    // Optionally generate new code here if loadBillNumber equivalent is needed
                    return (false, $"Phiếu đã tồn tại. Vui lòng chọn số phiếu khác hoặc tải lại số phiếu!");
                }
            }

            // Validate Customer or Supplier
            if ((dto.billExport.CustomerID <= 0 || dto.billExport.CustomerID == null) &&
                (dto.billExport.SupplierID <= 0 || dto.billExport.SupplierID == null))
            {
                return (false, "Xin hãy chọn Khách hàng hoặc Nhà cung cấp!");
            }

            // Validate Warehouse Type
            if (string.IsNullOrWhiteSpace(dto.billExport.WarehouseType))
            {
                return (false, "Xin hãy chọn kho quản lý.");
            }

            /*  // Validate Group
              if (string.IsNullOrWhiteSpace(dto.billExport.GroupID))
              {
                  return (false, "Xin hãy chọn nhóm.");
              }*/

            // Validate Sender
            if (dto.billExport.SenderID == null)
            {
                return (false, "Xin hãy chọn người giao.");
            }

            // Validate Status
            if (dto.billExport.Status < 0)
            {
                return (false, "Xin hãy chọn trạng thái.");
            }

            /*   // Validate Creation Date
               if (dto.billExport.Status != 6 && !dto.billExport.CreatedDate.HasValue)
               {
                   return (false, "Xin hãy chọn Ngày xuất!");
               }*/

            return (true, string.Empty);
        }
        [HttpPost("recheck-qty")]
        public IActionResult RecheckQty([FromBody] List<BillExportDetail> details)
        {
            if (details == null || !details.Any())
                return BadRequest(new { status = 0, message = "Không có dữ liệu chi tiết." });

            // Nhóm theo ProductID và tính tổng
            var grouped = details
                .GroupBy(x => x.ProductID)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Qty ?? 0));

            // Cập nhật TotalQty cho từng dòng
            foreach (var item in details)
            {
                if (item.ProductID != 0 && grouped.ContainsKey(item.ProductID))
                {
                    item.TotalQty = grouped[item.ProductID];
                }
            }

            return Ok(ApiResponseFactory.Success(details, "Đã tính lại TotalQty thành công"));
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveDataBillExport([FromBody] BillExportDTO dto)
        {
            try
            {
                // 1. Validate cơ bản
                var (isValid, errorMessage) = await ValidateBillExport(dto);
                if (!isValid)
                    return BadRequest(new { status = 0, message = errorMessage });

                if ((dto.billExportDetail == null || !dto.billExportDetail.Any()) && (dto.DeletedDetailIDs == null || !dto.DeletedDetailIDs.Any()))
                {
                    // Trường hợp chỉ xóa phiếu
                    dto.billExport.UpdatedDate = DateTime.Now;
                    _billexportRepo.Update(dto.billExport);
                    return Ok(new { status = 1, message = $"Đã xóa thành công phiếu {dto.billExport.Code}" });
                }

                var inventoryList = _inventoryRepo.GetAll();

                // 2. Tính TotalQty chung theo ProductID / Project / POKH
                var groupedQuantities = dto.billExportDetail
                    .GroupBy(d => new { d.ProductID, ProjectID = d.POKHDetailID > 0 ? 0 : d.ProjectID, POKHDetailID = d.POKHDetailID })
                    .ToDictionary(g => g.Key, g => g.Sum(d => d.Qty));

                foreach (var detail in dto.billExportDetail)
                {
                    var key = new { detail.ProductID, ProjectID = detail.POKHDetailID > 0 ? 0 : detail.ProjectID, POKHDetailID = detail.POKHDetailID };
                    if (groupedQuantities.ContainsKey(key))
                        detail.TotalQty = groupedQuantities[key];
                }

                // 3. Validate Keep (tồn kho tổng)
                foreach (var detail in dto.billExportDetail)
                {
                    int productId = detail.ProductID ?? 0;
                    ProductSale product = _productSaleRepo.GetByID(productId);
                    string productCode = product.ProductNewCode ?? "";
                    int projectId = detail.POKHDetailID > 0 ? 0 : detail.ProjectID ?? 0;
                    int pokhDetailId = detail.POKHDetailID ?? 0;

                    decimal totalQty = detail.TotalQty ?? 0;

                    // Lấy tồn kho theo sp, project, POKH
                    var ds = SQLHelper<dynamic>.ProcedureToList("spGetInventoryProjectImportExport",
                        new string[] { "@WarehouseID", "@ProductID", "@ProjectID", "@POKHDetailID", "@BillExportDetailID" },
                        new object[] { dto.billExport?.WarehouseID ?? 0, productId, projectId, pokhDetailId, detail.ID });

                    var inventoryProjects = ds[0];
                    var dtImport = ds[1];
                    var dtExport = ds[2];
                    var dtStock = ds[3];

                    decimal totalQuantityKeep = inventoryProjects.Count > 0 ? Convert.ToDecimal(inventoryProjects[0].TotalQuantity) : 0;
                    decimal totalQuantityLast = dtStock.Count > 0 ? Convert.ToDecimal(dtStock[0].TotalQuantityLast) : 0;
                    decimal totalImport = dtImport.Count > 0 ? Convert.ToDecimal(dtImport[0].TotalImport) : 0;
                    decimal totalExport = dtExport.Count > 0 ? Convert.ToDecimal(dtExport[0].TotalExport) : 0;

                    decimal totalQuantityRemain = Math.Max(totalImport - totalExport, 0);
                    decimal totalStock = Math.Max(totalQuantityKeep, 0) + totalQuantityRemain + Math.Max(totalQuantityLast, 0);
                    //decimal totalQuantityRemain = Math.Max(totalImport - totalExport, 0);
                    //decimal totalStock = Math.Max(totalQuantityKeep, 0) + totalQuantityRemain + Math.Max(totalQuantityLast, 0);



                    if (totalStock < totalQty)
                    {
                        return BadRequest(new
                        {
                            status = 0,
                            message = $"Số lượng còn lại sản phẩm [{product.ProductNewCode}] không đủ! SL xuất: {totalQty}, SL giữ: {totalQuantityKeep}, Tồn CK: {totalQuantityLast}, Tổng: {totalStock}"
                        });
                    }
                }

                // 4. Tạo / Cập nhật phiếu xuất
                int billExportId;
                if (dto.billExport.ID <= 0) // Thêm mới
                {
                    dto.billExport.IsMerge = false;
                    dto.billExport.UnApprove = 0;
                    dto.billExport.IsDeleted = false;
                    await _billexportRepo.CreateAsync(dto.billExport);
                    billExportId = dto.billExport.ID;
                }
                else // Cập nhật
                {
                    dto.billExport.UpdatedDate = DateTime.Now;
                    _billexportRepo.Update(dto.billExport);
                    billExportId = dto.billExport.ID;
                }

                // 5. Xử lý chi tiết phiếu xuất
                foreach (var detail in dto.billExportDetail)
                {
                    detail.BillID = billExportId;

                    if (detail.ID <= 0)
                    {
                        detail.IsDeleted = false;
                        await _billExportDetailRepo.CreateAsync(detail);
                    }
                    else
                    {
                        var existingDetail = _billExportDetailRepo.GetByID(detail.ID);
                        if (existingDetail != null)
                            _billExportDetailRepo.Update(detail);
                    }

                    // 6. Save InventoryProjectExport for status 2 (Exported) or 6 (Request Export)
                    // Matches desktop logic: if (billExport.Status == 2 || billExport.Status == 6)
                    if (dto.billExport.Status == 2 || dto.billExport.Status == 6)
                    {
                        // Get ChosenInventoryProject string from detail
                        // Format: "inventoryProjectID1-quantity1;inventoryProjectID2-quantity2"
                        string chosenInventoryProject = detail.ChosenInventoryProject ?? "";

                        if (!string.IsNullOrWhiteSpace(chosenInventoryProject))
                        {
                            string[] chosenInventoryProjects = chosenInventoryProject.Split(';');

                            // First, soft delete existing records for this detail (matching desktop logic)
                            var existingRecords = _inventoryprojectexportRepo.GetAll(x =>
                                x.BillExportDetailID == detail.ID && x.IsDeleted != true);

                            foreach (var existing in existingRecords)
                            {
                                existing.IsDeleted = true;
                                existing.UpdatedBy = dto.billExport.UpdatedBy;
                                existing.UpdatedDate = DateTime.Now;
                                await _inventoryprojectexportRepo.UpdateAsync(existing);
                            }

                            // Then create new records
                            foreach (string item in chosenInventoryProjects)
                            {
                                if (string.IsNullOrWhiteSpace(item)) continue;

                                var parts = item.Split('-');
                                if (parts.Length < 2) continue;

                                if (!int.TryParse(parts[0], out int inventoryProjectID)) continue;
                                if (!decimal.TryParse(parts[1], out decimal quantity)) continue;

                                var projectExport = new InventoryProjectExport
                                {
                                    BillExportDetailID = detail.ID,
                                    InventoryProjectID = inventoryProjectID,
                                    Quantity = quantity,
                                    CreatedDate = DateTime.Now,
                                    UpdatedDate = DateTime.Now,
                                    CreatedBy = dto.billExport.CreatedBy,
                                    IsDeleted = false
                                };

                                await _inventoryprojectexportRepo.CreateAsync(projectExport);
                            }
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(dto, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
        [HttpPost("delete-bill-export")]
        public async Task<IActionResult> deleteBillExport([FromBody] BillExport billExport)
        {
            try
            {
                if (billExport.IsApproved == true)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không thể xóa {" + billExport.Code + "} do đã nhận chứng từ!"));
                }
                else
                {
                    billExport.IsDeleted = true;
                    await _billexportRepo.UpdateAsync(billExport);
                    HistoryDeleteBill historyDeleteBill = new HistoryDeleteBill
                    {
                        BillID = billExport.ID,
                        UserID = billExport.UserID,
                        DeleteDate = DateTime.Now,
                        TypeBill = billExport.Code,
                    };
                    await _historyDeleteBillRepo.CreateAsync(historyDeleteBill);
                }
                return Ok(ApiResponseFactory.Success(billExport, "Đã xóa thành công mã phiếu {" + billExport.Code + "}!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("approved")]

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
            string billCode = string.Empty;
            string date = DateTime.Now.ToString("yyMMdd");

            if (billId.HasValue && billId.Value > 0 && !string.IsNullOrEmpty(currentCode) && currentStatus.HasValue)
            {
                string newPrefix = GetPrefix(billTypeId);
                string oldPrefix = GetPrefix(currentStatus.Value);
                billCode = currentCode.Replace(oldPrefix, newPrefix);
                return Ok(new { data = billCode });
            }

            string prefix = GetPrefix(billTypeId);
            int serial = 1;

            while (true)
            {
                string nextSerial = serial.ToString().PadLeft(3, '0');
                string candidateCode = prefix + date + nextSerial;

                // Kiểm tra trùng mã trong DB
                bool exists = _billexportRepo.GetAll().Any(x => x.Code == candidateCode && x.IsDeleted == false);
                if (!exists)
                {
                    billCode = candidateCode;
                    break;
                }

                serial++; // nếu trùng, tăng tiếp
            }

            return Ok(new { data = billCode });
        }

        // Xác định tiền tố mã phiếu dựa vào trạng thái
        private string GetPrefix(int billType)
        {
            return billType switch
            {
                0 => "PM",
                3 => "PCT",
                4 => "PMNB",
                5 => "PXM",
                _ => "PXK",
            };
        }

        // Truy vấn CSDL để lấy mã phiếu gần nhất trong ngày
        private string GetLastBillCodeToday()
        {
            // TODO: Bạn có thể thay đoạn này bằng truy vấn CSDL thật bằng Dapper/EF
            /*
            string sql = @"SELECT TOP 1 Code 
                           FROM BillExport 
                           WHERE DAY(CreatedDate) = @day 
                             AND MONTH(CreatedDate) = @month 
                             AND YEAR(CreatedDate) = @year 
                           ORDER BY ID DESC";
            */
            return ""; // giả lập chưa có mã nào trong ngày
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

        [HttpGet("export-excel")]
        public IActionResult ExportExcel(int id, int type)
        {
            try
            {
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
                var masterData = allData.FirstOrDefault();


                if (masterData == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy dữ liệu master"));
                }

                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "PhieuXuatSALE.xlsx");
                if (!System.IO.File.Exists(templatePath))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy file mẫu Excel"));
                }

                using (var workbook = new XLWorkbook(templatePath))
                {
                    var sheet = workbook.Worksheet(1);

                    // Mapping dữ liệu Master
                    sheet.Cell(6, 1).Value = $"Số: {masterData.Code}";
                    sheet.Cell(9, 4).Value = string.IsNullOrWhiteSpace((string)masterData.FullName)
                        ? masterData.FullName
                        : $"{masterData.FullName}";

                    string customerName = masterData.CustomerName?.ToString()?.Trim() ?? "";
                    string supplierName = masterData.NameNCC?.ToString()?.Trim() ?? "";

                    sheet.Cell(10, 3).Value = "'- Khách hàng/Nhà cung cấp:";
                    sheet.Cell(10, 4).Value = !string.IsNullOrEmpty(customerName) ? customerName : supplierName;

                    sheet.Cell(11, 4).Value = masterData.Address?.ToString() ?? "";
                    sheet.Cell(12, 4).Value = masterData.AddressStock?.ToString() ?? "";

                    DateTime? creatDate = masterData.CreatDate != null ? Convert.ToDateTime(masterData.CreatDate) : null;
                    if (creatDate.HasValue)
                    {
                        sheet.Cell(18, 9).Value = $"Ngày {creatDate.Value:dd} tháng {creatDate.Value:MM} năm {creatDate.Value:yyyy}";
                    }

                    sheet.Cell(25, 3).Value = masterData.FullNameSender?.ToString() ?? "";
                    sheet.Cell(25, 9).Value = masterData.FullName?.ToString() ?? "";

                    List<List<dynamic>> resultDetail = SQLHelper<dynamic>.ProcedureToList(
                  "spGetBillExportDetail",
                  new string[] { "@BillID" },
                  new object[] { id }
              );
                    var detailList = resultDetail[0]
                            .Cast<IDictionary<string, object>>()
                             .ToList();

                    // Mapping dữ liệu chi tiết
                    int startRow = 15;
                    int currentRow = startRow;
                    int stt = 1;
                    // Chỉ chèn thêm dòng nếu có nhiều hơn 1 mục chi tiết,
                    // vì file mẫu đã có sẵn 1 dòng để sử dụng.
                    if (detailList.Count > 1)
                    {
                        // Ví dụ: có 5 mục chi tiết -> chèn thêm 4 dòng.
                        sheet.Row(startRow).InsertRowsBelow(detailList.Count - 1);
                    }
                    if (detailList.Any())
                    {
                        foreach (var item in detailList)
                        {
                            int parentId = Convert.ToInt32(item["ParentID"] ?? 0);

                            if (type == 1 && parentId != 0) continue; // giống Winform: nếu type=1 thì bỏ qua dòng con

                            sheet.Cell(currentRow, 1).Value = stt++;
                            sheet.Cell(currentRow, 2).Value = item["ProductNewCode"]?.ToString() ?? "";
                            sheet.Cell(currentRow, 3).Value = item["ProductCode"]?.ToString() ?? "";
                            sheet.Cell(currentRow, 4).Value = item["ProductFullName"]?.ToString() ?? "";
                            sheet.Cell(currentRow, 5).Value = item["ProductName"]?.ToString() ?? "";
                            sheet.Cell(currentRow, 6).Value = item["Unit"]?.ToString() ?? "";
                            sheet.Cell(currentRow, 7).Value = Convert.ToDecimal(item["Qty"] ?? 0);
                            sheet.Cell(currentRow, 8).Value = item["ProjectCodeText"]?.ToString() ?? "";
                            sheet.Cell(currentRow, 9).Value = item["ProjectNameText"]?.ToString() ?? "";
                            sheet.Cell(currentRow, 10).Value = item["ProductTypeText"]?.ToString() ?? "";

                            sheet.Cell(currentRow, 11).Value = masterData.WarehouseID?.ToString() == "1"
                                ? item["ProductGroupName"]?.ToString() ?? ""
                                : item["WarehouseName"]?.ToString() ?? "";

                            string note = item["Note"]?.ToString() ?? "";
                            sheet.Cell(currentRow, 12).Value = note.StartsWith("=") ? $"'{note}" : note;

                            currentRow++;
                        }
                    }
                    else // Nếu không có dòng chi tiết nào
                    {
                        // Xoá dòng mẫu đi
                        sheet.Row(startRow).Delete();
                    }

                    using (var stream = new MemoryStream())
                    {

                        // --- TẠO QR CODE bằng ZXing.Net ---
                        string qrCodeText = masterData?.Code?.ToString() ?? "Unknown";

                        var writer = new BarcodeWriterPixelData
                        {
                            Format = BarcodeFormat.QR_CODE,
                            Options = new EncodingOptions
                            {
                                Height = 250,
                                Width = 250,
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

                            // Lưu vào tệp tạm
                            string tempPath = Path.Combine(Path.GetTempPath(), $"qr_{Guid.NewGuid()}.png");
                            bitmap.Save(tempPath, ImageFormat.Png);

                            // Thêm ảnh vào Excel
                            sheet.AddPicture(tempPath)
                           .MoveTo(sheet.Cell(1, 10), 20, 10)  // Đặt tại ô (1,10), lệch phải 20px, xuống 10px
                           .WithSize(120, 120);

                            System.IO.File.Delete(tempPath);
                        }

                        workbook.SaveAs(stream);
                        stream.Position = 0;

                        string fileName = $"{masterData.Code}_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.xlsx";
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
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
                    return NotFound(new { status = 0, message = $"Không tìm thấy phiếu với mã {code}." });

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
                var warehouses = _warehouseRepo.GetAll();
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
                    return NotFound(new
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
    }
}