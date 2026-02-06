using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.Technical;

namespace RERPAPI.Repo.GenericEntity.AddNewBillExport
{
    public class BillExportRepo : GenericRepo<BillExport>
    {
        private readonly BillExportDetailRepo _billExportDetailRepo;
        private readonly BillExportDetailSerialNumberRepo _serialNumberRepo;
        private readonly InventoryRepo _inventoryRepo;
        private readonly BillDocumentExportRepo _billDocumentExportRepo;
        private readonly DocumentExportRepo _documentExportRepo;
        private readonly InventoryProjectExportRepo _inventoryProjectExportRepo;
        private readonly BillImportRepo _billImportRepo;
        private readonly BillImportDetailRepo _billImportDetailRepo;
        private readonly BillImportDetailSerialNumberRepo _billImportSerialNumberRepo;
        private readonly POKHDetailRepo _pokhDetailRepo;
        private readonly POKHRepo _pokhRepo;
        private readonly ProductSaleRepo _productSaleRepo;
        private readonly ProjectRepo _projectRepo;
        private readonly BillImportDetailSerialNumberRepo _billImportDetailSerialNumberRepo;
        private readonly HistoryDeleteBillRepo _historyDeleteBillRepo;
        private readonly CurrentUser _currentUser;
        private readonly UserRepo _userRepo;
        private readonly SupplierRepo _supplierRepo;
        private readonly ProductGroupRepo _productGroupRepo;

        public BillExportRepo(
            CurrentUser currentUser,
            BillExportDetailRepo billExportDetailRepo,
            BillExportDetailSerialNumberRepo serialNumberRepo,
            InventoryRepo inventoryRepo,
            BillDocumentExportRepo billDocumentExportRepo,
            DocumentExportRepo documentExportRepo,
            InventoryProjectExportRepo inventoryProjectExportRepo,
            BillImportRepo billImportRepo,
            BillImportDetailRepo billImportDetailRepo,
            BillImportDetailSerialNumberRepo billImportSerialNumberRepo,
            POKHDetailRepo pokhDetailRepo,
            POKHRepo pokhRepo,
            ProductSaleRepo productSaleRepo,
            ProjectRepo projectRepo,
            BillImportDetailSerialNumberRepo billImportDetailSerialNumberRepo,
            HistoryDeleteBillRepo historyDeleteBillRepo,
            UserRepo userRepo,
            SupplierRepo supplierRepo,
            ProductGroupRepo productGroupRepo
        ) : base(currentUser)
        {
            _currentUser = currentUser;
            _billExportDetailRepo = billExportDetailRepo;
            _serialNumberRepo = serialNumberRepo;
            _inventoryRepo = inventoryRepo;
            _billDocumentExportRepo = billDocumentExportRepo;
            _documentExportRepo = documentExportRepo;
            _inventoryProjectExportRepo = inventoryProjectExportRepo;
            _billImportRepo = billImportRepo;
            _billImportDetailRepo = billImportDetailRepo;
            _billImportSerialNumberRepo = billImportSerialNumberRepo;
            _pokhDetailRepo = pokhDetailRepo;
            _pokhRepo = pokhRepo;
            _productSaleRepo = productSaleRepo;
            _projectRepo = projectRepo;
            _billImportDetailSerialNumberRepo = billImportDetailSerialNumberRepo;
            _historyDeleteBillRepo = historyDeleteBillRepo;
            _userRepo = userRepo;
            _supplierRepo = supplierRepo;
            _productGroupRepo = productGroupRepo;
        }

        #region Bill Code Generation
        public string GetBillCode(int billtype)
        {
            string billCode = "";
            DateTime billDate = DateTime.Now;
            string preCode = "PXK";

            if (billtype == 0 || billtype == 7) preCode = "PM";
            else if (billtype == 3) preCode = "PCT";
            else if (billtype == 4) preCode = "PMNB";
            else if (billtype == 5) preCode = "PXM";
            else preCode = "PXK";

            List<BillExport> billExports = GetAll(x => (x.Code ?? "").Contains(billDate.ToString("yyMMdd")));

            var listCode = billExports.Select(x => new
            {
                ID = x.ID,
                Code = x.Code,
                STT = string.IsNullOrWhiteSpace(x.Code) ? 0 : Convert.ToInt32(x.Code.Substring(x.Code.Length - 3)),
            }).ToList();

            string numberCodeText = "000";
            int numberCode = listCode.Count <= 0 ? 0 : listCode.Max(x => x.STT);
            numberCodeText = (++numberCode).ToString();

            while (numberCodeText.Length < 3)
            {
                numberCodeText = "0" + numberCodeText;
            }

            billCode = $"{preCode}{billDate.ToString("yyMMdd")}{numberCodeText}";
            return billCode;
        }
        public BillImport GetBillImportByBillExportID(int billExportID)
        {

            var billImport = _billImportRepo.GetAll(b => b.BillExportID == billExportID && b.IsDeleted == false).FirstOrDefault();
            return billImport;
        }
        public string GetBillImportCode(int billtype)
        {
            string billCode = "";
            DateTime billDate = DateTime.Now;
            string preCode = "";

            if (billtype == 0 || billtype == 4) preCode = "PNK";
            else if (billtype == 1) preCode = "PT";
            else if (billtype == 3) preCode = "PNM";
            else preCode = "PTNB";

            List<BillImport> billImports = _billImportRepo.GetAll(x => (x.BillImportCode ?? "").Contains(billDate.ToString("yyMMdd")));

            var listCode = billImports.Select(x => new
            {
                ID = x.ID,
                Code = x.BillImportCode,
                STT = string.IsNullOrWhiteSpace(x.BillImportCode) ? 0 : Convert.ToInt32(x.BillImportCode.Substring(x.BillImportCode.Length - 3)),
            }).ToList();

            string numberCodeText = "000";
            int numberCode = listCode.Count <= 0 ? 0 : listCode.Max(x => x.STT);
            numberCodeText = (++numberCode).ToString();

            while (numberCodeText.Length < 3)
            {
                numberCodeText = "0" + numberCodeText;
            }

            billCode = $"{preCode}{billDate.ToString("yyMMdd")}{numberCodeText}";
            return billCode;
        }
        #endregion
        //#region Auto Allocate Inventory Project
        //private async Task AutoAllocateInventoryProjects(BillExportDTO dto)
        //{
        //    int status = dto.billExport.Status ?? 0;
        //    if (status != 2 && status != 6)
        //        return;

        //    foreach (var detail in dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>())
        //    {
        //        // Nếu đã có ChosenInventoryProject từ frontend, skip
        //        if (!string.IsNullOrWhiteSpace(detail.ChosenInventoryProject))
        //            continue;

        //        int productId = detail.ProductID ?? 0;
        //        int projectId = detail.POKHDetailID > 0 ? 0 : detail.ProjectID ?? 0;
        //        int pokhDetailId = detail.POKHDetailID ?? 0;
        //        decimal qty = detail.Qty ?? 0;

        //        if (qty <= 0 || productId <= 0 || (projectId <= 0 && pokhDetailId <= 0))
        //            continue;

        //        // Tự động phân bổ
        //        detail.ChosenInventoryProject = await AutoAllocateInventoryProject(
        //            detail,
        //            dto.billExport.WarehouseID ?? 0,
        //            dto.billExportDetail.ToList()
        //        );
        //    }
        //}

        //private async Task<string> AutoAllocateInventoryProject(
        //    BillExportDetailExtendedDTO currentDetail,
        //    int warehouseId,
        //    List<BillExportDetailExtendedDTO> allDetails)
        //{
        //    int productId = currentDetail.ProductID ?? 0;
        //    int projectId = currentDetail.POKHDetailID > 0 ? 0 : currentDetail.ProjectID ?? 0;
        //    int pokhDetailId = currentDetail.POKHDetailID ?? 0;
        //    decimal qty = currentDetail.Qty ?? 0;

        //    // 1. Lấy danh sách inventory projects
        //    var ds = SQLHelper<dynamic>.ProcedureToList("spGetInventoryProjectImportExport",
        //        new string[] { "@WarehouseID", "@ProductID", "@ProjectID", "@POKHDetailID", "@BillExportDetailID" },
        //        new object[] { warehouseId, productId, projectId, pokhDetailId, currentDetail.ID ?? 0 });

        //    var inventoryProjects = ds[0]
        //        .Where(x => Convert.ToDecimal(x.TotalQuantityRemain) > 0)
        //        .OrderBy(x => Convert.ToDateTime(x.CreatedDate))
        //        .ToList();

        //    var dtStock = ds[3];
        //    decimal totalStockAvailable = Math.Max(0,
        //        dtStock.Count > 0 ? Convert.ToDecimal(dtStock[0].TotalQuantityLast) : 0);

        //    // Nếu không có kho giữ
        //    if (inventoryProjects.Count == 0)
        //    {
        //        // Có đủ tồn kho -> return empty (lấy từ tồn)
        //        if (totalStockAvailable >= qty)
        //            return "";
        //        // Không đủ -> return empty (để validation bắt lỗi)
        //        return "";
        //    }

        //    // 2. Tính toán số lượng đã sử dụng từ các detail khác
        //    var usedQuantityByInventoryID = CalculateUsedInventoryQuantities(
        //        allDetails,
        //        currentDetail.ID ?? 0,
        //        productId,
        //        projectId,
        //        pokhDetailId
        //    );

        //    // 3. Tính tổng available từ kho giữ
        //    decimal availableFromKeep = 0;
        //    foreach (var inv in inventoryProjects)
        //    {
        //        int id = Convert.ToInt32(inv.ID);
        //        decimal totalRemain = Convert.ToDecimal(inv.TotalQuantityRemain);
        //        decimal used = usedQuantityByInventoryID.ContainsKey(id) ? usedQuantityByInventoryID[id] : 0;
        //        decimal available = Math.Max(0, totalRemain - used);
        //        availableFromKeep += available;
        //    }
        //    availableFromKeep = Math.Max(0, availableFromKeep);

        //    decimal remainingQty = qty;
        //    var selectedInventory = new Dictionary<int, decimal>();

        //    // 4. Phân bổ
        //    if (availableFromKeep >= qty)
        //    {
        //        // Đủ kho giữ - Lấy từ kho giữ
        //        foreach (var inv in inventoryProjects)
        //        {
        //            if (remainingQty <= 0) break;

        //            int id = Convert.ToInt32(inv.ID);
        //            decimal totalRemain = Convert.ToDecimal(inv.TotalQuantityRemain);
        //            decimal used = usedQuantityByInventoryID.ContainsKey(id) ? usedQuantityByInventoryID[id] : 0;
        //            decimal available = Math.Max(0, totalRemain - used);

        //            if (available > 0)
        //            {
        //                decimal allocateQty = Math.Min(available, remainingQty);
        //                selectedInventory[id] = allocateQty;
        //                remainingQty -= allocateQty;
        //            }
        //        }

        //        if (selectedInventory.Any())
        //        {
        //            return string.Join(";", selectedInventory.Select(kv => $"{kv.Key}-{kv.Value}"));
        //        }
        //    }
        //    else
        //    {
        //        // Không đủ kho giữ - Kiểm tra tồn
        //        if (totalStockAvailable >= qty)
        //        {
        //            // Lấy từ tồn kho
        //            return "";
        //        }
        //        // Không đủ cả 2 -> return empty (validation sẽ bắt)
        //        return "";
        //    }

        //    return "";
        //}

        //private Dictionary<int, decimal> CalculateUsedInventoryQuantities(
        //    List<BillExportDetailExtendedDTO> allDetails,
        //    int currentDetailId,
        //    int productId,
        //    int projectId,
        //    int pokhDetailId)
        //{
        //    var usedQuantities = new Dictionary<int, decimal>();

        //    var relatedDetails = allDetails.Where(d =>
        //        d.ID != currentDetailId &&
        //        d.ProductID == productId &&
        //        (pokhDetailId > 0 ? d.POKHDetailID == pokhDetailId : d.ProjectID == projectId)
        //    );

        //    foreach (var detail in relatedDetails)
        //    {
        //        if (string.IsNullOrWhiteSpace(detail.ChosenInventoryProject))
        //            continue;

        //        foreach (var item in detail.ChosenInventoryProject.Split(';'))
        //        {
        //            if (string.IsNullOrWhiteSpace(item)) continue;

        //            var parts = item.Split('-');
        //            if (parts.Length < 2) continue;

        //            if (int.TryParse(parts[0], out int id) && decimal.TryParse(parts[1], out decimal qty))
        //            {
        //                if (!usedQuantities.ContainsKey(id))
        //                    usedQuantities[id] = 0;
        //                usedQuantities[id] += qty;
        //            }
        //        }
        //    }

        //    return usedQuantities;
        //}
        //#endregion
        //#region Main Save Method
        //private void RecalculateTotalQty(BillExportDTO dto)
        //{
        //    // Tính TotalQty theo ProductID
        //    var groupedQty = (dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>())
        //        .GroupBy(d => d.ProductID)
        //        .ToDictionary(g => g.Key, g => g.Sum(d => d.Qty ?? 0));

        //    // Gán lại TotalQty cho từng detail
        //    foreach (var detail in dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>())
        //    {
        //        if (groupedQty.ContainsKey(detail.ProductID))
        //        {
        //            detail.TotalQty = groupedQty[detail.ProductID];
        //        }
        //    }
        //}
        //public async Task<(bool Success, string Message, int BillExportId)> SaveBillExportWithDetails(BillExportDTO dto)
        //{
        //    try
        //    {
        //        RecalculateTotalQty(dto);
        //        // 1. Validate
        //        var (isValid, errorMessage) = await ValidateBillExport(dto);
        //        if (!isValid)
        //            return (false, errorMessage, 0);

        //        // 2. Validate phiếu mượn
        //        var borrowValidation = await ValidateBorrowBill(dto);
        //        if (!borrowValidation.Success)
        //            return (false, borrowValidation.Message, 0);

        //        // 3. Validate Keep
        //        var keepValidation = await ValidateKeepInventory(dto);
        //        if (!keepValidation.Success)
        //            return (false, keepValidation.Message, 0);

        //        // 4. Lưu BillExport
        //        int billExportId = await SaveBillExport(dto);

        //        // 5. Xử lý xóa detail
        //        if (dto.DeletedDetailIDs != null && dto.DeletedDetailIDs.Any())
        //            await HandleDeletedDetails(dto.DeletedDetailIDs);

        //        // 6. Lưu chi tiết
        //        await SaveBillExportDetails(dto, billExportId);

        //        // 7. Tạo chứng từ (nếu phiếu mới)
        //        if (dto.billExport.ID <= 0)
        //            await CreateDocumentExports(billExportId);

        //        // 8. Xử lý chuyển kho
        //        if (dto.billExport.IsTransfer == true && dto.billExport.Status == 2)
        //            await HandleTransferWarehouse(dto, billExportId);

        //        //// 9. Xử lý POKH
        //        //if (dto.POKHDetailIDs != null && dto.POKHDetailIDs.Any())
        //        //    await HandlePOKH(dto);

        //        return (true, "Lưu thành công", billExportId);
        //    }
        //    catch (Exception ex)
        //    {
        //        return (false, $"Lỗi: {ex.Message}", 0);
        //    }
        //}
        //#endregion

        //#region Validation
        //private async Task<(bool Success, string Message)> ValidateBillExport(BillExportDTO dto)
        //{
        //    // Kiểm tra trùng mã phiếu
        //    if (dto.billExport.ID > 0)
        //    {
        //        var existingCode = GetAll(x =>
        //            x.Code == dto.billExport.Code &&
        //            x.ID != dto.billExport.ID
        //        ).FirstOrDefault();

        //        if (existingCode != null)
        //        {
        //            dto.billExport.Code = GetBillCode(dto.billExport.Status ?? 0);
        //            return (true, $"Phiếu đã tồn tại. Phiếu được đổi tên thành: {dto.billExport.Code}");
        //        }
        //    }
        //    else
        //    {
        //        var existingCode = GetAll(x => x.Code == dto.billExport.Code).FirstOrDefault();
        //        if (existingCode != null)
        //        {
        //            dto.billExport.Code = GetBillCode(dto.billExport.Status ?? 0);
        //            return (true, $"Phiếu đã tồn tại. Phiếu được đổi tên thành: {dto.billExport.Code}");
        //        }
        //    }

        //    // Validate required fields
        //    if (string.IsNullOrWhiteSpace(dto.billExport.Code))
        //        return (false, "Xin hãy điền số phiếu.");

        //    if ((dto.billExport.CustomerID ?? 0) <= 0 && (dto.billExport.SupplierID ?? 0) <= 0)
        //        return (false, "Xin hãy chọn Khách hàng hoặc Nhà cung cấp!");

        //    if ((dto.billExport.UserID ?? 0) <= 0)
        //        return (false, "Xin hãy chọn nhân viên.");

        //    if ((dto.billExport.KhoTypeID ?? 0) <= 0)
        //        return (false, "Xin hãy chọn kho quản lý.");

        //    if (dto.billExport.KhoTypeID <= 0)
        //        return (false, "Xin hãy chọn nhóm.");

        //    if ((dto.billExport.SenderID ?? 0) <= 0)
        //        return (false, "Xin hãy chọn người giao.");

        //    if ((dto.billExport.Status ?? -1) < 0)
        //        return (false, "Xin hãy chọn trạng thái.");

        //    int billStatus = dto.billExport.Status ?? 0;
        //    if (billStatus != 6 && dto.billExport.CreatDate == null)
        //        return (false, "Xin hãy chọn Ngày xuất!");

        //    if (dto.billExport.IsTransfer == true && (dto.billExport.WareHouseTranferID ?? 0) <= 0)
        //        return (false, "Xin hãy chọn kho nhận!");

        //    return (true, string.Empty);
        //}

        //private async Task<(bool Success, string Message)> ValidateBorrowBill(BillExportDTO dto)
        //{
        //    int status = dto.billExport.Status ?? 0;

        //    if (status != 7 && status != 0)
        //        return (true, string.Empty);

        //    foreach (var detail in dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>())
        //    {
        //        if ((detail.Qty ?? 0) <= 0)
        //            return (false, $"Vui lòng nhập SL xuất dòng [{detail.STT}]");

        //        if (detail.ExpectReturnDate == null)
        //            return (false, $"Vui lòng nhập Ngày dự kiến trả dòng [{detail.STT}]");

        //        if ((detail.ProjectID ?? 0) <= 0)
        //            return (false, $"Vui lòng nhập Dự án dòng [{detail.STT}]");
        //    }

        //    return (true, string.Empty);
        //}

        //private async Task<(bool Success, string Message)> ValidateKeepInventory(BillExportDTO dto)
        //{
        //    int status = dto.billExport.Status ?? 0;
        //    if (status != 2 && status != 6)
        //        return (true, string.Empty);

        //    // Tính TotalQty chung
        //    var groupedQuantities = (dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>())
        //        .GroupBy(d => new
        //        {
        //            d.ProductID,
        //            ProjectID = d.POKHDetailID > 0 ? 0 : d.ProjectID,
        //            POKHDetailID = d.POKHDetailID
        //        })
        //        .ToDictionary(g => g.Key, g => g.Sum(d => d.Qty ?? 0));

        //    foreach (var detail in dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>())
        //    {
        //        var key = new
        //        {
        //            detail.ProductID,
        //            ProjectID = detail.POKHDetailID > 0 ? 0 : detail.ProjectID,
        //            POKHDetailID = detail.POKHDetailID
        //        };

        //        if (groupedQuantities.ContainsKey(key))
        //            detail.TotalQty = groupedQuantities[key];

        //        int productId = detail.ProductID ?? 0;
        //        int projectId = detail.POKHDetailID > 0 ? 0 : detail.ProjectID ?? 0;
        //        int pokhDetailId = detail.POKHDetailID ?? 0;
        //        decimal totalQty = detail.TotalQty ?? 0;

        //        // Lấy tồn kho
        //        var ds = SQLHelper<dynamic>.ProcedureToList("spGetInventoryProjectImportExport",
        //            new string[] { "@WarehouseID", "@ProductID", "@ProjectID", "@POKHDetailID", "@BillExportDetailID" },
        //            new object[] { dto.billExport?.WarehouseID ?? 0, productId, projectId, pokhDetailId, detail.ID });

        //        var inventoryProjects = ds[0];
        //        var dtImport = ds[1];
        //        var dtExport = ds[2];
        //        var dtStock = ds[3];

        //        decimal totalQuantityKeep = inventoryProjects.Count > 0 ? Convert.ToDecimal(inventoryProjects[0].TotalQuantity) : 0;
        //        decimal totalQuantityLast = dtStock.Count > 0 ? Convert.ToDecimal(dtStock[0].TotalQuantityLast) : 0;
        //        decimal totalImport = dtImport.Count > 0 ? Convert.ToDecimal(dtImport[0].TotalImport) : 0;
        //        decimal totalExport = dtExport.Count > 0 ? Convert.ToDecimal(dtExport[0].TotalExport) : 0;

        //        decimal totalQuantityRemain = Math.Max(totalImport - totalExport, 0);
        //        decimal totalStock = Math.Max(totalQuantityKeep, 0) + totalQuantityRemain + Math.Max(totalQuantityLast, 0);

        //        if (totalStock < totalQty)
        //        {
        //            var product = await _productSaleRepo.GetByIDAsync(productId);
        //            return (false, $"Số lượng còn lại sản phẩm [{product?.ProductNewCode}] không đủ! " +
        //                           $"SL xuất: {totalQty}, SL giữ: {totalQuantityKeep}, Tồn CK: {totalQuantityLast}, Tổng: {totalStock}");
        //        }
        //    }

        //    return (true, string.Empty);
        //}
        //#endregion

        //#region Save Operations
        //private async Task<int> SaveBillExport(BillExportDTO dto)
        //{
        //    dto.billExport.UpdatedDate = DateTime.Now;
        //    dto.billExport.UpdatedBy = _currentUser.LoginName;

        //    if (dto.billExport.ID <= 0)
        //    {
        //        dto.billExport.IsMerge = false;
        //        dto.billExport.UnApprove = 0;
        //        dto.billExport.IsDeleted = false;
        //        dto.billExport.BillDocumentExportType = 2;
        //        dto.billExport.CreatedDate = DateTime.Now;
        //        dto.billExport.CreatedBy = _currentUser.LoginName;

        //        await CreateAsync(dto.billExport);
        //        return dto.billExport.ID;
        //    }
        //    else
        //    {
        //        await UpdateAsync(dto.billExport);
        //        return dto.billExport.ID;
        //    }
        //}

        //private async Task HandleDeletedDetails(List<int> deletedDetailIDs)
        //{
        //    foreach (var detailId in deletedDetailIDs)
        //    {
        //        var detail = await _billExportDetailRepo.GetByIDAsync(detailId);
        //        if (detail != null)
        //        {
        //            detail.IsDeleted = true;
        //            detail.UpdatedBy = _currentUser.LoginName;
        //            detail.UpdatedDate = DateTime.Now;
        //            await _billExportDetailRepo.UpdateAsync(detail);
        //        }

        //        // Xóa SerialNumber
        //        var serialNumbers = _serialNumberRepo.GetAll(x => x.BillExportDetailID == detailId);
        //        foreach (var sn in serialNumbers)
        //        {
        //            await _serialNumberRepo.DeleteAsync(sn.ID);
        //        }
        //    }
        //}

        //private async Task SaveBillExportDetails(BillExportDTO dto, int billExportId)
        //{
        //    foreach (var detail in dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>())
        //    {
        //        detail.BillID = billExportId;

        //        if (detail.ID <= 0)
        //        {
        //            detail.IsDeleted = false;
        //            await _billExportDetailRepo.CreateAsync(detail);
        //        }
        //        else
        //        {
        //            var existingDetail = await _billExportDetailRepo.GetByIDAsync(detail.ID);
        //            if (existingDetail != null)
        //                await _billExportDetailRepo.UpdateAsync(detail);
        //        }

        //        // Kiểm tra và tạo Inventory
        //        await EnsureInventoryExists(dto.billExport.WarehouseID ?? 0, detail.ProductID ?? 0);

        //        // Lưu InventoryProjectExport
        //        if (dto.billExport.Status == 2 || dto.billExport.Status == 6)
        //        {
        //            await SaveInventoryProjectExport(detail);
        //        }
        //    }
        //}

        //private async Task EnsureInventoryExists(int warehouseId, int productId)
        //{
        //    var existingInventory = _inventoryRepo.GetAll(x =>
        //        x.WarehouseID == warehouseId &&
        //        x.ProductSaleID == productId
        //    ).FirstOrDefault();

        //    if (existingInventory == null)
        //    {
        //        var newInventory = new Inventory
        //        {
        //            WarehouseID = warehouseId,
        //            ProductSaleID = productId,
        //            TotalQuantityFirst = 0,
        //            TotalQuantityLast = 0,
        //            Import = 0,
        //            Export = 0
        //        };
        //        await _inventoryRepo.CreateAsync(newInventory);
        //    }
        //}

        //private async Task SaveInventoryProjectExport(BillExportDetailExtendedDTO detail)
        //{
        //    string chosenInventoryProject = detail.ChosenInventoryProject ?? "";
        //    if (string.IsNullOrWhiteSpace(chosenInventoryProject))
        //        return;

        //    // Xóa soft các record cũ
        //    var existingRecords = _inventoryProjectExportRepo.GetAll(x =>
        //        x.BillExportDetailID == detail.ID && x.IsDeleted != true);

        //    foreach (var existing in existingRecords)
        //    {
        //        existing.IsDeleted = true;
        //        existing.UpdatedBy = _currentUser.LoginName;
        //        existing.UpdatedDate = DateTime.Now;
        //        await _inventoryProjectExportRepo.UpdateAsync(existing);
        //    }

        //    // Tạo mới
        //    string[] chosenInventoryProjects = chosenInventoryProject.Split(';');
        //    foreach (string item in chosenInventoryProjects)
        //    {
        //        if (string.IsNullOrWhiteSpace(item)) continue;

        //        var parts = item.Split('-');
        //        if (parts.Length < 2) continue;

        //        if (!int.TryParse(parts[0], out int inventoryProjectID)) continue;
        //        if (!decimal.TryParse(parts[1], out decimal quantity)) continue;

        //        var projectExport = new InventoryProjectExport
        //        {
        //            BillExportDetailID = detail.ID,
        //            InventoryProjectID = inventoryProjectID,
        //            Quantity = quantity,
        //            IsDeleted = false,
        //            CreatedBy = _currentUser.LoginName,
        //            CreatedDate = DateTime.Now
        //        };

        //        await _inventoryProjectExportRepo.CreateAsync(projectExport);
        //    }
        //}

        //private async Task CreateDocumentExports(int billExportId)
        //{
        //    var documentExports = _documentExportRepo.GetAll(x => x.IsDeleted != true);

        //    foreach (var doc in documentExports)
        //    {
        //        var billDocument = new BillDocumentExport
        //        {
        //            BillExportID = billExportId,
        //            DocumentExportID = doc.ID
        //        };
        //        await _billDocumentExportRepo.CreateAsync(billDocument);
        //    }
        //}
        //#endregion

        //#region Transfer Warehouse
        //private async Task HandleTransferWarehouse(BillExportDTO dto, int billExportId)
        //{
        //    var existingImport = _billImportRepo.GetAll(x =>
        //        x.BillExportID == billExportId && x.IsDeleted != true
        //    ).FirstOrDefault();

        //    BillImport billImport;

        //    if (existingImport == null)
        //    {
        //        billImport = CreateNewBillImport(dto, billExportId);
        //        await _billImportRepo.CreateAsync(billImport);

        //        await CreateDocumentImport(billImport.ID);
        //    }
        //    else
        //    {
        //        if (existingImport.BillTypeNew != 4)
        //        {
        //            throw new Exception("Phiếu nhập đã thay đổi trạng thái, không thể sửa!");
        //        }

        //        billImport = UpdateBillImport(existingImport, dto);
        //        await _billImportRepo.UpdateAsync(billImport);
        //    }

        //    await SaveBillImportDetails(billImport.ID, dto);
        //}

        //private BillImport CreateNewBillImport(BillExportDTO dto, int billExportId)
        //{
        //    return new BillImport
        //    {
        //        BillExportID = billExportId,
        //        //Deliver = dto.billExport.SenderID,
        //        //Reciver = dto.billExport.User,
        //        //KhoType = dto.billExport.KhoType,
        //        KhoTypeID = dto.billExport.KhoTypeID,
        //        DeliverID = dto.billExport.SenderID,
        //        ReciverID = dto.billExport.UserID,
        //        SupplierID = 16677,
        //        GroupID = dto.billExport.GroupID,
        //        DateRequestImport = DateTime.Now,
        //        WarehouseID = dto.billExport.WareHouseTranferID,
        //        BillTypeNew = 4,
        //        CreatDate = dto.billExport.CreatDate,
        //        BillImportCode = GetBillImportCode(4),
        //        Status = false,
        //        IsDeleted = false,
        //        CreatedDate = DateTime.Now,
        //        CreatedBy = _currentUser.LoginName
        //    };
        //}

        //private BillImport UpdateBillImport(BillImport existingImport, BillExportDTO dto)
        //{
        //    existingImport.DeliverID = dto.billExport.SenderID;
        //    existingImport.ReciverID = dto.billExport.UserID;
        //    existingImport.KhoType = "Sale";
        //    existingImport.KhoTypeID = dto.billExport.ProductType;
        //    //existingImport.DeliverID = dto.billExport.SenderID;
        //    //existingImport.ReciverID = dto.billExport.UserID;
        //    existingImport.GroupID = dto.billExport.GroupID;
        //    existingImport.WarehouseID = dto.billExport.WareHouseTranferID;
        //    existingImport.CreatDate = dto.billExport.CreatDate;
        //    existingImport.UpdatedDate = DateTime.Now;
        //    existingImport.UpdatedBy = _currentUser.LoginName;
        //    return existingImport;
        //}

        //private async Task CreateDocumentImport(int billImportId)
        //{


        //    //SQLHelper<dynamic>.ExcuteProcedure("spCreateDocumentImport", ["BillImportID"])
        //    //await db.Database.ExecuteSqlRawAsync(
        //    //    "EXEC spCreateDocumentImport @BillImportID = {0}, @CreatedBy = {1}",
        //    //    billImportId, _currentUser.LoginName
        //    //);
        //}

        //private async Task SaveBillImportDetails(int billImportId, BillExportDTO dto)
        //{
        //    // Xóa chi tiết cũ
        //    var existingDetails = _billImportDetailRepo.GetAll(x => x.BillImportID == billImportId);
        //    foreach (var detail in existingDetails)
        //    {
        //        var serialNumbers = _billImportSerialNumberRepo.GetAll(x => x.BillImportDetailID == detail.ID);
        //        foreach (var sn in serialNumbers)
        //        {
        //            await _billImportSerialNumberRepo.DeleteAsync(sn.ID);
        //        }
        //        await _billImportDetailRepo.DeleteAsync(detail.ID);
        //    }

        //    // Tạo mới
        //    foreach (var exportDetail in dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>())
        //    {
        //        if ((exportDetail.ProductID ?? 0) <= 0) continue;

        //        var importDetail = new BillImportDetail
        //        {
        //            BillImportID = billImportId,
        //            ProductID = exportDetail.ProductID,
        //            Qty = exportDetail.Qty,
        //            Price = 0,
        //            TotalPrice = 0,
        //            ProjectName = exportDetail.ProjectName,
        //            ProjectCode = exportDetail.ProjectName,
        //            Note = dto.billExport.Code,
        //            STT = exportDetail.STT,
        //            TotalQty = exportDetail.TotalQty,
        //            ProjectID = exportDetail.ProjectID,
        //            SerialNumber = exportDetail.SerialNumber,
        //            BillExportDetailID = exportDetail.ID,
        //            ReturnedStatus = false,
        //            PONCCDetailID = exportDetail.POKHDetailID,
        //            QtyRequest = exportDetail.Qty,
        //            IsDeleted = false,
        //            CreatedDate = DateTime.Now,
        //            CreatedBy = _currentUser.LoginName
        //        };

        //        await _billImportDetailRepo.CreateAsync(importDetail);
        //        await EnsureInventoryExists(dto.billExport.WareHouseTranferID ?? 0, exportDetail.ProductID ?? 0);
        //    }
        //    SQLHelper<dynamic>.ExcuteProcedure("spUpdateReturnedStatusForBillExportDetail", ["@BillImportID", "@Approved"], [billImportId, 0]);
        //    // Cập nhật trạng thái trả
        //    //await db.Database.ExecuteSqlRawAsync(
        //    //    "EXEC spUpdateReturnedStatusForBillExportDetail @BillImportID = {0}, @Approved = {1}",
        //    //    billImportId, 0
        //    //);
        //}
        //#endregion
        bool HasPermission(params string[] requiredPermissions)
        {
            if (string.IsNullOrWhiteSpace(_currentUser.Permissions))
                return false;

            var userPermissions = _currentUser.Permissions
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim());

            return requiredPermissions.Any(rp => userPermissions.Contains(rp));
        }

        #region Main Save Method
        public async Task<(bool Success, string Message, int BillExportId)> SaveBillExportWithDetails(BillExportDTO dto)
        {
            try
            {
                if (dto.billExport != null && dto.billExport.ID > 0)
                {
                    if (!HasPermission("N1", "N27", "N33", "N34", "N69") && !_currentUser.IsAdmin)
                    {
                        return (false, "Bạn không có quyền để lưu phiếu này!", dto.billExport.ID);
                    }
                }
                if (dto.billExport.ID > 0)
                {
                    await LoadExistingInventoryProjectData(dto);
                }
                // 0. Tính lại TotalQty
                RecalculateTotalQty(dto);

                // 1. Validate form
                var (isValid, errorMessage) = await ValidateBillExport(dto);
                if (!isValid)
                    return (false, errorMessage, 0);

                // 2. Validate phiếu mượn
                var borrowValidation = await ValidateBorrowBill(dto);
                if (!borrowValidation.Success)
                    return (false, borrowValidation.Message, 0);

                // 3. Tự động phân bổ kho giữ (nếu frontend chưa làm)
                await AutoAllocateInventoryProjects(dto);

                // 4. Validate Keep (sau khi đã phân bổ)
                var keepValidation = await ValidateKeepInventory(dto);
                if (!keepValidation.Success)
                    return (false, keepValidation.Message, 0);

                // 5. Lưu BillExport
                int billExportId = await SaveBillExport(dto);

                // 6. Xử lý xóa detail
                if (dto.DeletedDetailIDs != null && dto.DeletedDetailIDs.Any())
                    await HandleDeletedDetails(dto.DeletedDetailIDs);

                // 7. Lưu chi tiết
                await SaveBillExportDetails(dto, billExportId);

                // 8. Tạo chứng từ (nếu phiếu mới)
                if (dto.billExport.ID <= 0)
                    await CreateDocumentExports(billExportId);

                // 9. Xử lý chuyển kho
                //if (dto.billExport.IsTransfer == true)
                //    await HandleTransferWarehouse(dto, billExportId);

                return (true, "Lưu thành công", billExportId);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}", 0);
            }
        }


        private void RecalculateTotalQty(BillExportDTO dto)
        {
            var groupedQty = (dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>())
                .GroupBy(d => d.ProductID)
                .ToDictionary(g => g.Key, g => g.Sum(d => d.Qty ?? 0));

            foreach (var detail in dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>())
            {
                if (groupedQty.ContainsKey(detail.ProductID))
                {
                    detail.TotalQty = groupedQty[detail.ProductID];
                }
            }
        }
        #endregion

        #region Validation
        private async Task<(bool Success, string Message)> ValidateBillExport(BillExportDTO dto)
        {
            // Kiểm tra trùng mã phiếu
            if (dto.billExport.ID > 0)
            {
                BillExport billExport = GetByID(dto.billExport.ID);
                BillImport billImports = _billImportRepo.GetAll(x => x.BillExportID == dto.billExport.ID && x.IsDeleted == false).FirstOrDefault();
                if (billImports != null)
                {
                    if (billImports.Status == true)
                    {
                        return (false, $"Không thể sửa phiếu xuất {dto.billExport.Code} vì phiếu nhập {billImports.BillImportCode} đã được duyệt!");
                    }

                    if (billImports.BillTypeNew != 4 && billExport.Status != 6)
                    {
                        return (false, $"Phiếu nhập {billImports.BillImportCode} đã thay đổi trạng thái, không thể sửa!");
                    }
                    else if (billImports.BillTypeNew != 0 && billExport.Status != 6)
                    {
                        return (false, $"Phiếu nhập {billImports.BillImportCode} chưa chuyển sang trạng thái [Nhập kho], không thể sửa!");
                    }

                    if (dto.billExport.IsTransfer == false && billImports.Status == true)
                    {
                        return (false, $"Không thể bỏ chuyển kho vì phiếu nhập {billImports.BillImportCode} đã được duyệt!");
                    }
                }

                var existingCode = GetAll(x => x.Code == dto.billExport.Code && x.ID != dto.billExport.ID).FirstOrDefault();
                if (existingCode != null)
                {
                    dto.billExport.Code = GetBillCode(dto.billExport.Status ?? 0);
                    return (true, $"Phiếu đã tồn tại. Phiếu được đổi tên thành: {dto.billExport.Code}");
                }
            }
            else
            {
                var existingCode = GetAll(x => x.Code == dto.billExport.Code).FirstOrDefault();
                if (existingCode != null)
                {
                    dto.billExport.Code = GetBillCode(dto.billExport.Status ?? 0);
                    return (true, $"Phiếu đã tồn tại. Phiếu được đổi tên thành: {dto.billExport.Code}");
                }
            }
            if (string.IsNullOrWhiteSpace(dto.billExport.Code))
                return (false, "Xin hãy điền số phiếu.");

            if ((dto.billExport.CustomerID ?? 0) <= 0 && (dto.billExport.SupplierID ?? 0) <= 0)
                return (false, "Xin hãy chọn Khách hàng hoặc Nhà cung cấp!");

            if ((dto.billExport.UserID ?? 0) <= 0)
                return (false, "Xin hãy chọn nhân viên.");

            if ((dto.billExport.KhoTypeID ?? 0) <= 0)
                return (false, "Xin hãy chọn kho quản lý.");

            if ((dto.billExport.SenderID ?? 0) <= 0)
                return (false, "Xin hãy chọn người giao.");

            if ((dto.billExport.Status ?? -1) < 0)
                return (false, "Xin hãy chọn trạng thái.");

            int billStatus = dto.billExport.Status ?? 0;
            if (billStatus != 6 && dto.billExport.CreatDate == null)
                return (false, "Xin hãy chọn Ngày xuất!");

            if (dto.billExport.IsTransfer == true && (dto.billExport.WareHouseTranferID ?? 0) <= 0)
                return (false, "Xin hãy chọn kho nhận!");

            return (true, string.Empty);
        }

        private async Task<(bool Success, string Message)> ValidateBorrowBill(BillExportDTO dto)
        {
            int status = dto.billExport.Status ?? 0;

            if (status != 7 && status != 0)
                return (true, string.Empty);

            foreach (var detail in dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>())
            {
                if ((detail.Qty ?? 0) <= 0)
                    return (false, $"Vui lòng nhập SL xuất dòng [{detail.STT}]");

                if (detail.ExpectReturnDate == null)
                    return (false, $"Vui lòng nhập Ngày dự kiến trả dòng [{detail.STT}]");

                if ((detail.ProjectID ?? 0) <= 0)
                    return (false, $"Vui lòng nhập Dự án dòng [{detail.STT}]");
            }

            return (true, string.Empty);
        }

        private async Task<(bool Success, string Message)> ValidateKeepInventory(BillExportDTO dto)
        {
            int status = dto.billExport.Status ?? 0;
            if (status != 2 && status != 6)
                return (true, string.Empty);

            // Skip units
            var skipUnitNames = new[] { "m", "mét", "met" };

            // Tính TotalQty chung
            var groupedQuantities = (dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>())
                .GroupBy(d => new
                {
                    d.ProductID,
                    ProjectID = (d.POKHDetailID ?? 0) > 0 ? 0 : d.ProjectID,
                    POKHDetailID = d.POKHDetailID ?? 0
                })
                .ToDictionary(g => g.Key, g => g.Sum(d => d.Qty ?? 0));

            foreach (var detail in dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>())
            {
                // Skip validation theo Unit
                if (!string.IsNullOrWhiteSpace(detail.Unit) &&
                    skipUnitNames.Contains(detail.Unit.Trim().ToLower()))
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(detail.UnitName) &&
                    skipUnitNames.Contains(detail.UnitName.Trim().ToLower()))
                {
                    continue;
                }

                var key = new
                {
                    detail.ProductID,
                    ProjectID = (detail.POKHDetailID ?? 0) > 0 ? 0 : detail.ProjectID,
                    POKHDetailID = detail.POKHDetailID ?? 0
                };

                if (groupedQuantities.ContainsKey(key))
                    detail.TotalQty = groupedQuantities[key];

                int productId = detail.ProductID ?? 0;
                int projectId = (detail.POKHDetailID ?? 0) > 0 ? 0 : detail.ProjectID ?? 0;
                int pokhDetailId = detail.POKHDetailID ?? 0;
                decimal totalQty = detail.TotalQty ?? 0;

                // Lấy tồn kho
                var ds = GetInventoryProjectImportExport(
                    dto.billExport?.WarehouseID ?? 0,
                    productId,
                    projectId,
                    pokhDetailId,
                    detail.ID
                );

                var inventoryProjects = ds[0];
                var dtImport = ds[1];
                var dtExport = ds[2];
                var dtStock = ds[3];

                decimal totalQuantityKeep = inventoryProjects.Count > 0 ? Convert.ToDecimal(((dynamic)inventoryProjects[0]).TotalQuantity) : 0;
                decimal totalQuantityLast = dtStock.Count > 0 ? Convert.ToDecimal(((dynamic)dtStock[0]).TotalQuantityLast) : 0;
                decimal totalImport = dtImport.Count > 0 ? Convert.ToDecimal(((dynamic)dtImport[0]).TotalImport) : 0;
                decimal totalExport = dtExport.Count > 0 ? Convert.ToDecimal(((dynamic)dtExport[0]).TotalExport) : 0;

                decimal totalQuantityRemain = Math.Max(totalImport - totalExport, 0);
                decimal totalStock = Math.Max(totalQuantityKeep, 0) + totalQuantityRemain + Math.Max(totalQuantityLast, 0);

                if (totalStock < totalQty)
                {
                    string productDisplay = detail.ProductNewCode ?? detail.ProductCode ?? $"ID:{productId}";

                    return (false, $"Số lượng còn lại sản phẩm [{productDisplay}] không đủ! " +
                                   $"SL xuất: {totalQty}, SL giữ: {totalQuantityKeep}, Tồn CK: {totalQuantityLast}, Tổng: {totalStock}");
                }
            }

            return (true, string.Empty);
        }
        #endregion

        #region Auto Allocate Inventory Project
        private async Task AutoAllocateInventoryProjects(BillExportDTO dto)
        {
            int status = dto.billExport.Status ?? 0;
            if (status != 2 && status != 6)
                return;

            foreach (var detail in dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>())
            {
                // Nếu đã có ChosenInventoryProject từ frontend, skip
                if (!string.IsNullOrWhiteSpace(detail.ChosenInventoryProject))
                    continue;

                int productId = detail.ProductID ?? 0;
                int projectId = detail.ProjectID ?? 0;
                int pokhDetailId = detail.POKHDetailID ?? 0;
                //detail.POKHDetailIDActual ?? detail.POKHDetailID ?? 0;
                if (pokhDetailId > 0) projectId = 0;
                decimal qty = detail.Qty ?? 0;

                if (qty <= 0 || productId <= 0 || (projectId <= 0 && pokhDetailId <= 0))
                    continue;

                // Tự động phân bổ
                var result = await AutoAllocateInventoryProject(
                    detail,
                    dto.billExport.WarehouseID ?? 0,
                    dto.billExportDetail.ToList()
                );

                detail.ChosenInventoryProject = result.ChosenInventoryProject;
                detail.ProductCodeExport = result.ProductCodeExport;
            }
        }
        /// <summary>
        /// Lấy dữ liệu tổng hợp import/export/stock (từ spGetInventoryProjectImportExport)
        /// Trả về 4 DataSets: [0] InventoryProjects Summary, [1] Import, [2] Export, [3] Stock
        /// </summary>
        public List<List<dynamic>> GetInventoryProjectImportExport(
            int warehouseId,
            int productId,
            int projectId,
            int pokhDetailId,
            int billExportDetailId)
        {
            var result = SQLHelper<dynamic>.ProcedureToList(
                "spGetInventoryProjectImportExport",
                new string[] { "@WarehouseID", "@ProductID", "@ProjectID", "@POKHDetailID", "@BillExportDetailID" },
                new object[] { warehouseId, productId, projectId, pokhDetailId, billExportDetailId }
            );

            return result;
        }
        /// <summary>
        /// Lấy danh sách kho giữ có thể phân bổ (từ spGetInventoryProject)
        /// </summary>
        public List<dynamic> GetInventoryProjectList(
            int warehouseId,
            int productId,
            int projectId,
            int pokhDetailId)
        {
            var result = SQLHelper<dynamic>.ProcedureToList(
                "spGetInventoryProject",
                new string[] { "@ProjectID", "@EmployeeID", "@ProductSaleID", "@Keyword", "@WarehouseID", "@POKHDetailID" },
                new object[] { 0, 0, productId, "", warehouseId, pokhDetailId }
            );

            // spGetInventoryProject chỉ trả về 1 result set (danh sách inventory projects)
            return result.Count > 0 ? result[0] : new List<dynamic>();
        }

        //    private async Task<(string ChosenInventoryProject, string ProductCodeExport)> AutoAllocateInventoryProject(
        //BillExportDetailExtendedDTO currentDetail,
        //int warehouseId,
        //List<BillExportDetailExtendedDTO> allDetails)
        //    {
        //        int productId = currentDetail.ProductID ?? 0;
        //        int projectId = (currentDetail.POKHDetailIDActual ?? currentDetail.POKHDetailID ?? 0) > 0 ? 0 : currentDetail.ProjectID ?? 0;
        //        int pokhDetailId = currentDetail.POKHDetailID ?? 0;
        //        decimal qty = currentDetail.Qty ?? 0;

        //        // ✅ 1. Lấy danh sách kho giữ CÓ THỂ PHÂN BỔ (từ spGetInventoryProject)
        //        var inventoryProjectsRaw = GetInventoryProjectList(warehouseId, productId, projectId, pokhDetailId);

        //        // Filter và sort
        //        var inventoryProjects = inventoryProjectsRaw
        //            .Where(x =>
        //            {
        //                var dict = x as IDictionary<string, object>;
        //                if (dict != null && dict.ContainsKey("TotalQuantityRemain"))
        //                {
        //                    return Convert.ToDecimal(dict["TotalQuantityRemain"]) > 0;
        //                }
        //                return false;
        //            })
        //            .OrderBy(x =>
        //            {
        //                var dict = x as IDictionary<string, object>;
        //                if (dict != null && dict.ContainsKey("CreatedDate"))
        //                {
        //                    return Convert.ToDateTime(dict["CreatedDate"]);
        //                }
        //                return DateTime.MinValue;
        //            })
        //            .ToList();

        //        // ✅ 2. Lấy tổng tồn kho (từ spGetInventoryProjectImportExport)
        //        var ds = GetInventoryProjectImportExport(warehouseId, productId, projectId, pokhDetailId, currentDetail.ID);
        //        var dtStock = ds.Count > 3 ? ds[3] : new List<dynamic>();

        //        decimal totalStockAvailable = 0;
        //        if (dtStock.Count > 0)
        //        {
        //            var stockDict = dtStock[0] as IDictionary<string, object>;
        //            if (stockDict != null && stockDict.ContainsKey("TotalQuantityLast"))
        //            {
        //                totalStockAvailable = Math.Max(0, Convert.ToDecimal(stockDict["TotalQuantityLast"]));
        //            }
        //        }

        //        // ✅ 3. Nếu không có kho giữ
        //        if (inventoryProjects.Count == 0)
        //        {
        //            if (totalStockAvailable >= qty)
        //                return ("", ""); // Lấy từ tồn kho
        //            return ("", ""); // Không đủ, để validation bắt
        //        }

        //        // ✅ 4. Tính toán số lượng đã sử dụng
        //        var usedQuantityByInventoryID = CalculateUsedInventoryQuantities(
        //            allDetails,
        //            currentDetail.ChildID ?? currentDetail.ID,
        //            productId,
        //            projectId,
        //            pokhDetailId
        //        );

        //        // ✅ 5. Tính tổng available từ kho giữ
        //        decimal availableFromKeep = 0;
        //        foreach (var inv in inventoryProjects)
        //        {
        //            var invDict = inv as IDictionary<string, object>;
        //            if (invDict == null) continue;

        //            int id = Convert.ToInt32(invDict["ID"]);
        //            decimal totalRemain = Convert.ToDecimal(invDict["TotalQuantityRemain"]);
        //            decimal used = usedQuantityByInventoryID.ContainsKey(id) ? usedQuantityByInventoryID[id] : 0;
        //            decimal available = Math.Max(0, totalRemain - used);
        //            availableFromKeep += available;
        //        }
        //        availableFromKeep = Math.Max(0, availableFromKeep);

        //        decimal remainingQty = qty;
        //        var selectedInventory = new Dictionary<int, decimal>();

        //        // ✅ 6. Phân bổ
        //        if (availableFromKeep >= qty)
        //        {
        //            // Đủ kho giữ - Lấy từ kho giữ
        //            foreach (var inv in inventoryProjects)
        //            {
        //                if (remainingQty <= 0) break;

        //                var invDict = inv as IDictionary<string, object>;
        //                if (invDict == null) continue;

        //                int id = Convert.ToInt32(invDict["ID"]);
        //                decimal totalRemain = Convert.ToDecimal(invDict["TotalQuantityRemain"]);
        //                decimal used = usedQuantityByInventoryID.ContainsKey(id) ? usedQuantityByInventoryID[id] : 0;
        //                decimal available = Math.Max(0, totalRemain - used);

        //                if (available > 0)
        //                {
        //                    decimal allocateQty = Math.Min(available, remainingQty);
        //                    selectedInventory[id] = allocateQty;
        //                    remainingQty -= allocateQty;
        //                }
        //            }

        //            if (selectedInventory.Any())
        //            {
        //                string result = string.Join(";", selectedInventory.Select(kv => $"{kv.Key}-{kv.Value}"));

        //                var codes = new List<string>();
        //                foreach (var inv in inventoryProjects)
        //                {
        //                    var invDict = inv as IDictionary<string, object>;
        //                    if (invDict == null) continue;

        //                    int id = Convert.ToInt32(invDict["ID"]);
        //                    if (selectedInventory.ContainsKey(id) && invDict.ContainsKey("ProductCode"))
        //                    {
        //                        codes.Add(invDict["ProductCode"]?.ToString() ?? "");
        //                    }
        //                }

        //                return (result, string.Join(";", codes));
        //            }
        //        }
        //        else
        //        {
        //            // Không đủ kho giữ - Kiểm tra tồn
        //            if (totalStockAvailable >= qty)
        //            {
        //                return ("", ""); // Lấy từ tồn kho
        //            }
        //            return ("", ""); // Không đủ, để validation bắt
        //        }

        //        return ("", "");
        //    }

        private async Task<(string ChosenInventoryProject, string ProductCodeExport)> AutoAllocateInventoryProject(
    BillExportDetailExtendedDTO currentDetail,
    int warehouseId,
    List<BillExportDetailExtendedDTO> allDetails)
        {
            int productId = currentDetail.ProductID ?? 0;
            int projectId = currentDetail.ProjectID ?? 0;
            int pokhDetailId = currentDetail.POKHDetailID ?? 0;
            if (pokhDetailId > 0) projectId = 0;
            decimal qty = currentDetail.Qty ?? 0;

            // 1. Lấy danh sách kho giữ
            var inventoryProjectsRaw = GetInventoryProjectList(warehouseId, productId, projectId, pokhDetailId);

            // 2. Filter và sort
            var inventoryProjects = inventoryProjectsRaw
                .Where(x => GetDecimalFromDynamic(x, "TotalQuantityRemain") > 0)
                .OrderBy(x => GetDateTimeFromDynamic(x, "CreatedDate"))
                .ToList();

            // 3. Lấy tổng tồn kho
            var ds = GetInventoryProjectImportExport(warehouseId, productId, projectId, pokhDetailId, currentDetail.ID);
            var dtStock = ds.Count > 3 ? ds[3] : new List<dynamic>();

            decimal totalStockAvailable = dtStock.Count > 0
                ? Math.Max(0, GetDecimalFromDynamic(dtStock[0], "TotalQuantityLast"))
                : 0;

            // 4. Nếu không có kho giữ
            if (inventoryProjects.Count == 0)
            {
                if (totalStockAvailable >= qty)
                {
                    // Đủ tồn kho, để trống để lấy từ tồn kho
                    return ("", "");
                }
                else
                {
                    // Không đủ tồn kho, để validation bắt
                    return ("", "");
                }
            }

            // 5. Tính toán số lượng đã sử dụng
            var usedQuantityByInventoryID = CalculateUsedInventoryQuantities(
                allDetails,
                currentDetail.ChildID ?? currentDetail.ID,
                productId,
                projectId,
                pokhDetailId
            );

            // 6. Tính tổng available từ kho giữ
            decimal availableFromKeep = 0;
            foreach (var inv in inventoryProjects)
            {
                int id = GetIntFromDynamic(inv, "ID");
                decimal totalRemain = GetDecimalFromDynamic(inv, "TotalQuantityRemain");
                decimal used = usedQuantityByInventoryID.ContainsKey(id) ? usedQuantityByInventoryID[id] : 0;
                availableFromKeep += Math.Max(0, totalRemain - used);
            }
            availableFromKeep = Math.Max(0, availableFromKeep);

            // ✅ 7. LOGIC PHÂN BỔ KẾT HỢP
            decimal remainingQty = qty;
            var selectedInventory = new Dictionary<int, decimal>();

            // ✅ Bước 1: Phân bổ từ kho giữ (lấy hết những gì có)
            foreach (var inv in inventoryProjects)
            {
                if (remainingQty <= 0) break;

                int id = GetIntFromDynamic(inv, "ID");
                decimal totalRemain = GetDecimalFromDynamic(inv, "TotalQuantityRemain");
                decimal used = usedQuantityByInventoryID.ContainsKey(id) ? usedQuantityByInventoryID[id] : 0;
                decimal available = Math.Max(0, totalRemain - used);

                if (available > 0)
                {
                    decimal allocateQty = Math.Min(available, remainingQty);
                    selectedInventory[id] = allocateQty;
                    remainingQty -= allocateQty;
                }
            }

            // ✅ Bước 2: Kiểm tra phần còn lại
            if (remainingQty > 0)
            {
                // Còn thiếu → Kiểm tra tồn kho có đủ bù không
                if (totalStockAvailable >= remainingQty)
                {
                    // ✅ Tồn kho đủ bù phần thiếu
                    // Trả về phần đã phân bổ từ kho giữ + phần còn lại lấy từ tồn kho

                    if (selectedInventory.Any())
                    {
                        // Format result: "id1-qty1;id2-qty2"
                        string result = string.Join(";", selectedInventory.Select(kv => $"{kv.Key}-{kv.Value}"));

                        // Get ProductCodes
                        string codes = string.Join(";",
                            inventoryProjects
                                .Where(inv => selectedInventory.ContainsKey(GetIntFromDynamic(inv, "ID")))
                                .Select(inv => GetStringFromDynamic(inv, "ProductCode"))
                                .Where(code => !string.IsNullOrEmpty(code))
                        );

                        // Note: Phần remainingQty sẽ được lấy tự động từ tồn kho
                        // Backend sẽ xử lý: totalFromKeep + totalFromStock = qty
                        return (result, codes);
                    }
                    else
                    {
                        // Không phân bổ được từ kho giữ, lấy toàn bộ từ tồn kho
                        return ("", "");
                    }
                }
                else
                {
                    // ❌ Tồn kho không đủ bù phần thiếu
                    // Không đủ tổng cộng, để validation bắt lỗi
                    return ("", "");
                }
            }
            else
            {
                // ✅ Kho giữ đã đủ, không cần lấy từ tồn kho
                if (selectedInventory.Any())
                {
                    string result = string.Join(";", selectedInventory.Select(kv => $"{kv.Key}-{kv.Value}"));

                    string codes = string.Join(";",
                        inventoryProjects
                            .Where(inv => selectedInventory.ContainsKey(GetIntFromDynamic(inv, "ID")))
                            .Select(inv => GetStringFromDynamic(inv, "ProductCode"))
                            .Where(code => !string.IsNullOrEmpty(code))
                    );

                    return (result, codes);
                }
                else
                {
                    return ("", "");
                }
            }
        }
        #region Helper Methods for Dynamic Object

        /// <summary>
        /// Safely get decimal value from dynamic object
        /// </summary>
        private decimal GetDecimalFromDynamic(dynamic obj, string fieldName, decimal defaultValue = 0)
        {
            if (obj is not IDictionary<string, object> dict)
                return defaultValue;

            if (!dict.ContainsKey(fieldName))
                return defaultValue;

            var value = dict[fieldName];
            if (value == null || value is DBNull)
                return defaultValue;

            try
            {
                return Convert.ToDecimal(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Safely get int value from dynamic object
        /// </summary>
        private int GetIntFromDynamic(dynamic obj, string fieldName, int defaultValue = 0)
        {
            if (obj is not IDictionary<string, object> dict)
                return defaultValue;

            if (!dict.ContainsKey(fieldName))
                return defaultValue;

            var value = dict[fieldName];
            if (value == null || value is DBNull)
                return defaultValue;

            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Safely get string value from dynamic object
        /// </summary>
        private string GetStringFromDynamic(dynamic obj, string fieldName, string defaultValue = "")
        {
            if (obj is not IDictionary<string, object> dict)
                return defaultValue;

            if (!dict.ContainsKey(fieldName))
                return defaultValue;

            var value = dict[fieldName];
            if (value == null || value is DBNull)
                return defaultValue;

            return value.ToString() ?? defaultValue;
        }

        /// <summary>
        /// Safely get DateTime value from dynamic object
        /// </summary>
        private DateTime GetDateTimeFromDynamic(dynamic obj, string fieldName, DateTime? defaultValue = null)
        {
            if (obj is not IDictionary<string, object> dict)
                return defaultValue ?? DateTime.MinValue;

            if (!dict.ContainsKey(fieldName))
                return defaultValue ?? DateTime.MinValue;

            var value = dict[fieldName];
            if (value == null || value is DBNull)
                return defaultValue ?? DateTime.MinValue;

            try
            {
                return Convert.ToDateTime(value);
            }
            catch
            {
                return defaultValue ?? DateTime.MinValue;
            }
        }

        /// <summary>
        /// Safely get bool value from dynamic object
        /// </summary>
        private bool GetBoolFromDynamic(dynamic obj, string fieldName, bool defaultValue = false)
        {
            if (obj is not IDictionary<string, object> dict)
                return defaultValue;

            if (!dict.ContainsKey(fieldName))
                return defaultValue;

            var value = dict[fieldName];
            if (value == null || value is DBNull)
                return defaultValue;

            try
            {
                return Convert.ToBoolean(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        #endregion
        //private Dictionary<int, decimal> CalculateUsedInventoryQuantities(
        //    List<BillExportDetailExtendedDTO> allDetails,
        //    int currentDetailId,
        //    int productId,
        //    int projectId,
        //    int pokhDetailId)
        //{
        //    var usedQuantities = new Dictionary<int, decimal>();

        //    var relatedDetails = allDetails.Where(d => d.ID == currentDetailId && d.ProductID == productId && d.POKHDetailID == pokhDetailId);

        //    foreach (var detail in relatedDetails)
        //    {
        //        if (string.IsNullOrWhiteSpace(detail.ChosenInventoryProject))
        //            continue;

        //        foreach (var item in detail.ChosenInventoryProject.Split(';'))
        //        {
        //            if (string.IsNullOrWhiteSpace(item)) continue;

        //            var parts = item.Split('-');
        //            if (parts.Length < 2) continue;

        //            if (int.TryParse(parts[0], out int id) && decimal.TryParse(parts[1], out decimal qty))
        //            {
        //                if (!usedQuantities.ContainsKey(id))
        //                    usedQuantities[id] = 0;
        //                usedQuantities[id] += qty;
        //            }
        //        }
        //    }

        //    return usedQuantities;
        //}
        private Dictionary<int, decimal> CalculateUsedInventoryQuantities(
    List<BillExportDetailExtendedDTO> allDetails,
    int currentDetailId,
    int productId,
    int projectId,
    int pokhDetailId)
        {
            var usedQuantities = new Dictionary<int, decimal>();

            // ✅ FIX: Lấy các detail KHÁC (loại trừ currentDetailId)
            var relatedDetails = allDetails.Where(d =>
                (d.ChildID ?? d.ID) != currentDetailId &&  // ✅ LOẠI TRỪ detail hiện tại
                d.ProductID == productId &&
                (pokhDetailId > 0
                    ? d.POKHDetailID == pokhDetailId
                    : d.ProjectID == projectId)  // ✅ Thêm điều kiện ProjectID
            );

            foreach (var detail in relatedDetails)
            {
                if (string.IsNullOrWhiteSpace(detail.ChosenInventoryProject))
                    continue;

                foreach (var item in detail.ChosenInventoryProject.Split(';'))
                {
                    if (string.IsNullOrWhiteSpace(item)) continue;

                    var parts = item.Split('-');
                    if (parts.Length < 2) continue;

                    if (int.TryParse(parts[0], out int id) &&
                        decimal.TryParse(parts[1], out decimal qty))
                    {
                        if (!usedQuantities.ContainsKey(id))
                            usedQuantities[id] = 0;
                        usedQuantities[id] += qty;
                    }
                }
            }

            return usedQuantities;
        }
        #endregion

        #region Save Operations
        private async Task<int> SaveBillExport(BillExportDTO dto)
        {
            dto.billExport.UpdatedDate = DateTime.Now;
            dto.billExport.UpdatedBy = _currentUser.LoginName;

            if (dto.billExport.ID <= 0)
            {
                dto.billExport.IsMerge = false;
                dto.billExport.UnApprove = 0;
                dto.billExport.IsDeleted = false;
                dto.billExport.BillDocumentExportType = 2;
                dto.billExport.CreatedDate = DateTime.Now;
                dto.billExport.CreatedBy = _currentUser.LoginName;

                await CreateAsync(dto.billExport);
            }
            else
            {
                await UpdateAsync(dto.billExport);
            }
            if (dto.billExport.IsTransfer == true)
            {
                await HandleTransferWarehouse(dto, dto.billExport.ID);

            }
            return dto.billExport.ID;
        }

        private async Task HandleDeletedDetails(List<int> deletedDetailIDs)
        {
            foreach (var detailId in deletedDetailIDs)
            {
                var detail = await _billExportDetailRepo.GetByIDAsync(detailId);
                if (detail != null)
                {
                    detail.IsDeleted = true;
                    detail.UpdatedBy = _currentUser.LoginName;
                    detail.UpdatedDate = DateTime.Now;
                    await _billExportDetailRepo.UpdateAsync(detail);
                }

                // Xóa SerialNumber
                var serialNumbers = _serialNumberRepo.GetAll(x => x.BillExportDetailID == detailId);
                foreach (var sn in serialNumbers)
                {
                    await _serialNumberRepo.DeleteAsync(sn.ID);
                }
            }
        }

        private async Task SaveBillExportDetails(BillExportDTO dto, int billExportId)
        {
            foreach (var detail in dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>())
            {
                detail.BillID = billExportId;

                if (detail.ID <= 0)
                {
                    detail.IsDeleted = false;
                    detail.CreatedDate = DateTime.Now;
                    detail.CreatedBy = _currentUser.LoginName;

                    var newDetail = MapToEntity(detail);
                    await _billExportDetailRepo.CreateAsync(newDetail);
                    detail.ID = newDetail.ID;
                }
                else
                {
                    detail.UpdatedDate = DateTime.Now;
                    detail.UpdatedBy = _currentUser.LoginName;

                    var existingDetail = await _billExportDetailRepo.GetByIDAsync(detail.ID);
                    if (existingDetail != null)
                    {
                        MapToExistingEntity(detail, existingDetail);
                        await _billExportDetailRepo.UpdateAsync(existingDetail);
                    }
                }

                // Kiểm tra và tạo Inventory
                await EnsureInventoryExists(dto.billExport.WarehouseID ?? 0, detail.ProductID ?? 0);

                // Lưu InventoryProjectExport
                if (dto.billExport.Status == 2 || dto.billExport.Status == 6)
                {
                    await SaveInventoryProjectExport(detail);
                }
            }
        }

        private BillExportDetail MapToEntity(BillExportDetailExtendedDTO dto)
        {
            return new BillExportDetail
            {
                BillID = dto.BillID,
                ProductID = dto.ProductID,
                ProductFullName = dto.ProductFullName,
                Qty = dto.Qty,
                ProjectName = dto.ProjectName,
                Note = dto.Note,
                STT = dto.STT,
                TotalQty = dto.TotalQty,
                ProjectID = dto.ProjectID,
                ProductType = dto.ProductType,
                POKHID = dto.POKHID,
                GroupExport = dto.GroupExport,
                SerialNumber = dto.SerialNumber,
                TradePriceDetailID = dto.TradePriceDetailID,
                POKHDetailID = dto.POKHDetailID,
                Specifications = dto.Specifications,
                BillImportDetailID = dto.BillImportDetailID,
                TotalInventory = dto.TotalInventory,
                ExpectReturnDate = dto.ExpectReturnDate,
                ProjectPartListID = dto.ProjectPartListID,
                IsDeleted = dto.IsDeleted,
                CreatedDate = dto.CreatedDate,
                CreatedBy = dto.CreatedBy
            };
        }

        private void MapToExistingEntity(BillExportDetailExtendedDTO dto, BillExportDetail entity)
        {
            entity.BillID = dto.BillID;
            entity.ProductID = dto.ProductID;
            entity.ProductFullName = dto.ProductFullName;
            entity.Qty = dto.Qty;
            entity.ProjectName = dto.ProjectName;
            entity.Note = dto.Note;
            entity.STT = dto.STT;
            entity.TotalQty = dto.TotalQty;
            entity.ProjectID = dto.ProjectID;
            entity.ProductType = dto.ProductType;
            entity.POKHID = dto.POKHID;
            entity.GroupExport = dto.GroupExport;
            entity.SerialNumber = dto.SerialNumber;
            entity.TradePriceDetailID = dto.TradePriceDetailID;
            entity.POKHDetailID = dto.POKHDetailID;
            entity.Specifications = dto.Specifications;
            entity.BillImportDetailID = dto.BillImportDetailID;
            entity.TotalInventory = dto.TotalInventory;
            entity.ExpectReturnDate = dto.ExpectReturnDate;
            entity.ProjectPartListID = dto.ProjectPartListID;
            entity.UpdatedDate = dto.UpdatedDate;
            entity.UpdatedBy = dto.UpdatedBy;
        }

        private async Task EnsureInventoryExists(int warehouseId, int productId)
        {
            var existingInventory = _inventoryRepo.GetAll(x =>
                x.WarehouseID == warehouseId &&
                x.ProductSaleID == productId
            ).FirstOrDefault();

            if (existingInventory == null)
            {
                var newInventory = new Inventory
                {
                    WarehouseID = warehouseId,
                    ProductSaleID = productId,
                    TotalQuantityFirst = 0,
                    TotalQuantityLast = 0,
                    Import = 0,
                    Export = 0
                };
                await _inventoryRepo.CreateAsync(newInventory);
            }
        }

        private async Task SaveInventoryProjectExport(BillExportDetailExtendedDTO detail)
        {
            string chosenInventoryProject = detail.ChosenInventoryProject ?? "";
            if (string.IsNullOrWhiteSpace(chosenInventoryProject))
                return;

            // Xóa soft các record cũ
            var existingRecords = _inventoryProjectExportRepo.GetAll(x =>
                x.BillExportDetailID == detail.ID && x.IsDeleted != true);

            foreach (var existing in existingRecords)
            {
                existing.IsDeleted = true;
                existing.UpdatedBy = _currentUser.LoginName;
                existing.UpdatedDate = DateTime.Now;
                await _inventoryProjectExportRepo.UpdateAsync(existing);
            }

            // Tạo mới
            string[] chosenInventoryProjects = chosenInventoryProject.Split(';');
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
                    IsDeleted = false,
                    CreatedBy = _currentUser.LoginName,
                    CreatedDate = DateTime.Now
                };

                await _inventoryProjectExportRepo.CreateAsync(projectExport);
            }
        }

        private async Task CreateDocumentExports(int billExportId)
        {
            var documentExports = _documentExportRepo.GetAll(x => x.IsDeleted != true);

            foreach (var doc in documentExports)
            {
                var billDocument = new BillDocumentExport
                {
                    BillExportID = billExportId,
                    DocumentExportID = doc.ID
                };
                await _billDocumentExportRepo.CreateAsync(billDocument);
            }
        }

        private async Task HandleTransferWarehouse(BillExportDTO dto, int billExportId)
        {
            var billExport = await GetByIDAsync(billExportId);
            BillImport existingImport = null;

            // Kiểm tra xem phiếu xuất đã có BillImportID chưa
            if (billExport?.BillImportID != null && billExport.BillImportID > 0)
            {
                existingImport = _billImportRepo.GetByID(billExport.BillImportID.Value);
                if (existingImport?.IsDeleted == true)
                {
                    existingImport = null;
                }
            }

            if (existingImport == null)
            {
                existingImport = _billImportRepo.GetAll(x =>
                    x.BillExportID == billExportId &&
                    x.IsDeleted != true
                ).FirstOrDefault();
            }

            BillImport billImport;

            //  Lấy thông tin người dùng và nhà cung cấp
            var deliver = _userRepo.GetByID(dto.billExport.SenderID ?? 0);
            var reciver = _userRepo.GetByID(dto.billExport.UserID ?? 0);
            var supplier = _supplierRepo.GetByID(dto.billExport.SupplierID ?? 0);
            var productGroup = _productGroupRepo.GetByID(dto.billExport.KhoTypeID ?? 0);

            //  Chưa có phiếu nhập → Tạo mới
            if (existingImport == null)
            {
                billImport = new BillImport
                {
                    BillExportID = billExportId,
                    DeliverID = dto.billExport.SenderID,
                    Deliver = deliver?.FullName,
                    ReciverID = dto.billExport.UserID,
                    Reciver = reciver?.FullName,
                    KhoTypeID = dto.billExport.KhoTypeID,
                    SupplierID = dto.billExport.SupplierID,
                    Suplier = supplier?.SupplierName,
                    GroupID = dto.billExport.GroupID,
                    DateRequestImport = DateTime.Now,
                    BillTypeNew = 4,
                    BillImportCode = _billImportRepo.GetBillCode(4),
                    WarehouseID = dto.billExport.WareHouseTranferID ?? 0,
                    CreatDate = dto.billExport.CreatDate,
                    Status = false,
                    IsDeleted = false,
                    KhoType = productGroup?.ProductGroupName
                };
                await _billImportRepo.CreateAsync(billImport);

                SQLHelper<object>.ExcuteProcedure("spCreateDocumentImport",
                    new string[] { "@BillImportID", "@CreatedBy" },
                    new object[] { billImport.ID, _currentUser.LoginName });
            }
            else
            {
                // Đã có phiếu nhập → Update
                existingImport.DeliverID = dto.billExport.SenderID;
                existingImport.Deliver = deliver?.FullName;
                existingImport.Reciver = reciver?.FullName;
                existingImport.ReciverID = dto.billExport.UserID;
                existingImport.KhoTypeID = dto.billExport.KhoTypeID;
                existingImport.SupplierID = dto.billExport.SupplierID;
                existingImport.GroupID = dto.billExport.GroupID;
                existingImport.WarehouseID = dto.billExport.WareHouseTranferID ?? 0;
                existingImport.CreatDate = dto.billExport.CreatDate;
                existingImport.UpdatedDate = DateTime.Now;
                existingImport.Suplier = supplier?.SupplierName;
                existingImport.KhoType = productGroup?.ProductGroupName;

                await _billImportRepo.UpdateAsync(existingImport);
                billImport = existingImport;
            }

            //  chi tiết phiếu nhập
            await SyncBillImportDetails(billImport.ID, dto);

            // Update lại BillImportID cho phiếu xuất (nếu chưa có)
            if (billExport != null && billExport.BillImportID != billImport.ID)
            {
                billExport.BillImportID = billImport.ID;
                await UpdateAsync(billExport);
            }
        }
        //private async Task HandleTransferWarehouse(BillExportDTO dto, int billExportId)
        //{
        //    // ✅ 1. Tìm phiếu nhập liên kết (nếu có)
        //    var existingImport = _billImportRepo.GetAll(x =>
        //        x.BillExportID == billExportId &&
        //        x.IsDeleted != true
        //    ).FirstOrDefault();

        //    BillImport billImport;

        //    // ✅ 2. TH1: Chưa có phiếu nhập → Tạo mới
        //    var deliver = _userRepo.GetByID(dto.billExport.SenderID ?? 0);
        //    var reciver = _userRepo.GetByID(dto.billExport.UserID ?? 0);
        //    var supplier = _supplierRepo.GetByID(dto.billExport.SupplierID ?? 0);
        //    if (existingImport == null)
        //    {
        //        billImport = new BillImport
        //        {
        //            BillExportID = billExportId,
        //            DeliverID = dto.billExport.SenderID,
        //            Deliver = deliver.FullName,
        //            ReciverID = dto.billExport.UserID,
        //            Reciver = reciver.FullName,
        //            KhoTypeID = dto.billExport.KhoTypeID,
        //            SupplierID = dto.billExport.SupplierID,
        //            Suplier = supplier.SupplierName,
        //            GroupID = dto.billExport.GroupID,
        //            DateRequestImport = DateTime.Now,
        //            BillTypeNew = 4,
        //            BillImportCode = _billImportRepo.GetBillCode(4),
        //            WarehouseID = dto.billExport.WareHouseTranferID ?? 0,
        //            CreatDate = dto.billExport.CreatDate,
        //            Status = false,
        //            IsDeleted = false,
        //            CreatedDate = DateTime.Now,
        //            CreatedBy = _currentUser.LoginName
        //        };

        //        await _billImportRepo.CreateAsync(billImport);

        //        SQLHelper<object>.ExcuteProcedure("spCreateDocumentImport",
        //            new string[] { "@BillImportID", "@CreatedBy" },
        //            new object[] { billImport.ID, _currentUser.LoginName });
        //    }
        //    else
        //    {

        //        existingImport.DeliverID = dto.billExport.SenderID;
        //        existingImport.Deliver = deliver.FullName;
        //        existingImport.Reciver = reciver.FullName;
        //        existingImport.ReciverID = dto.billExport.UserID;
        //        existingImport.KhoTypeID = dto.billExport.KhoTypeID;
        //        existingImport.SupplierID = dto.billExport.SupplierID;
        //        existingImport.GroupID = dto.billExport.GroupID;
        //        existingImport.WarehouseID = dto.billExport.WareHouseTranferID ?? 0;
        //        existingImport.CreatDate = dto.billExport.CreatDate;
        //        existingImport.UpdatedDate = DateTime.Now;
        //        existingImport.Suplier = supplier.SupplierName;
        //        existingImport.UpdatedBy = _currentUser.LoginName;

        //        await _billImportRepo.UpdateAsync(existingImport);

        //        billImport = existingImport;
        //    }

        //    await SyncBillImportDetails(billImport.ID, dto);

        //    var billExport = await GetByIDAsync(billExportId);
        //    if (billExport != null)
        //    {
        //        billExport.BillImportID = billImport.ID;
        //        await UpdateAsync(billExport);
        //    }
        //}
        private async Task SyncBillImportDetails(int billImportId, BillExportDTO dto)
        {
            var existingDetails = _billImportDetailRepo
                .GetAll(x => x.BillImportID == billImportId && x.IsDeleted != true);

            if (existingDetails.Any())
            {
                // 1. Xóa mềm SerialNumber
                foreach (var detail in existingDetails)
                {
                    var serials = _billImportDetailSerialNumberRepo
                        .GetAll(x => x.BillImportDetailID == detail.ID)
                        .ToList();

                    foreach (var serial in serials)
                    {
                        serial.IsDeleted = true;

                        await _billImportDetailSerialNumberRepo.UpdateAsync(serial);
                    }
                }

                // 2. Xóa mềm BillImportDetail
                foreach (var detail in existingDetails)
                {
                    detail.IsDeleted = true;

                    await _billImportDetailRepo.UpdateAsync(detail);
                }
            }

            // 3. Insert lại dữ liệu mới
            foreach (var exportDetail in dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>())
            {
                if ((exportDetail.ProductID ?? 0) <= 0) continue;

                var importDetail = new BillImportDetail
                {
                    BillImportID = billImportId,
                    ProductID = exportDetail.ProductID,
                    Qty = exportDetail.Qty,
                    Price = 0,
                    TotalPrice = 0,
                    ProjectID = exportDetail.ProjectID,
                    ProjectName = exportDetail.ProjectName,
                    ProjectCode = exportDetail.ProjectName,
                    SomeBill = "",
                    Note = dto.billExport.Code,
                    STT = exportDetail.STT,
                    TotalQty = exportDetail.TotalQty,
                    SerialNumber = exportDetail.SerialNumber,
                    BillExportDetailID = exportDetail.ID,
                    QtyRequest = exportDetail.Qty,
                    ReturnedStatus = false,
                    IsNotKeep = true,
                    IsDeleted = false,
                    CreatedDate = DateTime.Now,
                    CreatedBy = _currentUser.LoginName
                };

                await _billImportDetailRepo.CreateAsync(importDetail);

                await EnsureInventoryExists(
                    dto.billExport.WareHouseTranferID ?? 0,
                    exportDetail.ProductID ?? 0
                );
            }

            // 4. Update trạng thái trả hàng
            SQLHelper<object>.ExcuteProcedure(
                "spUpdateReturnedStatusForBillExportDetail",
                new[] { "@BillImportID", "@Approved" },
                new object[] { billImportId, 0 }
            );
        }

        /// <summary>
        /// Hủy chuyển kho - xóa phiếu nhập liên kết
        /// (Đã được validate ở ValidateBillExport)
        /// </summary>
        private async Task CancelTransferWarehouse(int billExportId)
        {
            var existingImport = _billImportRepo.GetAll(x =>
                x.BillExportID == billExportId &&
                x.IsDeleted != true
            ).FirstOrDefault();

            if (existingImport != null)
            {
                existingImport.IsDeleted = true;
                existingImport.UpdatedDate = DateTime.Now;
                existingImport.UpdatedBy = _currentUser.LoginName;

                await _billImportRepo.UpdateAsync(existingImport);

                var billExport = await GetByIDAsync(billExportId);
                if (billExport != null)
                {
                    billExport.BillImportID = null;
                    await UpdateAsync(billExport);
                }
            }
        }
        #endregion
        //#region POKH Handling
        //private async Task HandlePOKH(BillExportDTO dto)
        //{
        //    if (dto.POKHDetailIDs == null || !dto.POKHDetailIDs.Any())
        //        return;

        //    DateTime? creatDate = dto.billExport.CreatDate;

        //    foreach (var pokhDetailId in dto.POKHDetailIDs)
        //    {
        //        var pokhDetail = await _pokhDetailRepo.GetByIDAsync(pokhDetailId);
        //        if (pokhDetail != null)
        //        {
        //            pokhDetail.IsExport = true;
        //            pokhDetail.ActualDeliveryDate = creatDate;
        //            await _pokhDetailRepo.UpdateAsync(pokhDetail);
        //        }
        //    }

        //    if (dto.POKHID > 0)
        //    {
        //        await CheckAndUpdatePOKHStatus(dto.POKHID);
        //    }
        //}

        //private async Task CheckAndUpdatePOKHStatus(int pokhId)
        //{
        //    var pokhDetails = _pokhDetailRepo.GetAll(x => x.POKHID == pokhId);
        //    int totalDetails = pokhDetails.Count();
        //    int exportedDetails = pokhDetails.Where(x => x.IsExport == true).Count();

        //    var pokh = await _pokhRepo.GetByIDAsync(pokhId);
        //    if (pokh != null)
        //    {
        //        if (exportedDetails > 0 && exportedDetails < totalDetails)
        //        {
        //            pokh.IsExport = true;
        //        }
        //        else if (exportedDetails == totalDetails)
        //        {
        //            pokh.IsExport = true;
        //        }
        //        await _pokhRepo.UpdateAsync(pokh);
        //    }
        //}
        //#endregion


        #region Load Existing Inventory Project Data
        /// <summary>
        /// Load ChosenInventoryProject từ DB cho các detail đang sửa
        /// Chỉ load nếu frontend KHÔNG gửi ChosenInventoryProject và ForceReallocate = false
        /// </summary>
        private async Task LoadExistingInventoryProjectData(BillExportDTO dto)
        {
            foreach (var detail in dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>())
            {
                //Chỉ xử lý detail có ID (đang sửa)
                if (detail.ID <= 0)
                    continue;

                // Nếu frontend đã gửi ChosenInventoryProject → giữ nguyên
                if (!string.IsNullOrWhiteSpace(detail.ChosenInventoryProject))
                    continue;

                //  Nếu frontend yêu cầu phân bổ lại (ForceReallocate = true) → skip load từ DB
                if (detail.ForceReallocate == true)
                {
                    // Để trống để AutoAllocateInventoryProject xử lý
                    continue;
                }

                // Load từ DB chỉ khi frontend gửi rỗng VÀ KHÔNG có thay đổi
                var existingExports = _inventoryProjectExportRepo.GetAll(x =>
                    x.BillExportDetailID == detail.ID &&
                    x.IsDeleted != true
                ).ToList();

                if (existingExports.Any())
                {
                    // Format: "123-5;456-10"
                    detail.ChosenInventoryProject = string.Join(";",
                        existingExports.Select(x => $"{x.InventoryProjectID}-{x.Quantity}")
                    );

                    // Load ProductCode
                    var ds = GetInventoryProjectImportExport(
                        dto.billExport.WarehouseID ?? 0,
                        detail.ProductID ?? 0,
                        (detail.POKHDetailID ?? 0) > 0 ? 0 : detail.ProjectID ?? 0,
                        detail.POKHDetailID ?? 0,
                        detail.ID
                    );

                    if (ds.Count > 0)
                    {
                        var inventoryProjects = ds[0];
                        var productCodes = new List<string>();

                        foreach (var export in existingExports)
                        {
                            var inv = inventoryProjects.FirstOrDefault(x =>
                                GetIntFromDynamic(x, "ID") == export.InventoryProjectID
                            );
                            if (inv != null)
                            {
                                productCodes.Add(GetStringFromDynamic(inv, "ProductCode"));
                            }
                        }

                        detail.ProductCodeExport = string.Join(";",
                            productCodes.Where(x => !string.IsNullOrEmpty(x)));
                    }
                }
            }
        }
        #endregion

        public async Task<(bool Success, string Message)> HandleDeleteBill(BillExport billExport)
        {
            if (billExport.IsApproved == true)
            {
                return (false, $"Không thể xóa {billExport.Code} do đã nhận chứng từ!");
            }

            if (billExport.IsTransfer == true)
            {
                BillImport billImports = _billImportRepo.GetAll(x => x.BillExportID == billExport.ID && x.IsDeleted == false).FirstOrDefault();
                if (billImports != null)
                {
                    if (billImports.BillTypeNew != 4)
                    {
                        return (false, $"Không thể xóa {billExport.Code} do phiếu là phiếu xuất chuyển kho và trạng thái phiếu nhập đã thay đổi!");
                    }
                    if (billImports.Status == true)
                    {
                        return (false, $"Không thể xóa {billExport.Code} do phiếu là phiếu xuất chuyển kho và phiếu nhập đã được duyệt!");
                    }
                    billImports.IsDeleted = true;
                    await _billImportRepo.UpdateAsync(billImports);
                }
            }

            billExport.IsDeleted = true;
            await UpdateAsync(billExport);

            HistoryDeleteBill historyDeleteBill = new HistoryDeleteBill
            {
                BillID = billExport.ID,
                UserID = billExport.UserID,
                DeleteDate = DateTime.Now,
                TypeBill = billExport.Code,
            };
            await _historyDeleteBillRepo.CreateAsync(historyDeleteBill);

            return (true, $"Xóa phiếu xuất {billExport.Code} thành công!");
        }
    }
}