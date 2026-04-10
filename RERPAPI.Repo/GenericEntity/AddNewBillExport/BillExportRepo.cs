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

            List<BillExport> billExports = GetAll(x => (x.Code ?? "").Contains(billDate.ToString("yyMMdd")) && x.IsDeleted == false);

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
        #endregion
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

                // Pre-fetch tất cả SP data song song với ValidateBillExport
                // (N spGetInventoryProject + 1 batch SP chạy song song trên thread pool trong khi main thread validate)
                int billStatus = dto.billExport.Status ?? 0;
                Task<(Dictionary<(int, int, int, int), List<dynamic>>, Dictionary<(int, int, int), dynamic>)> preFetchTask = null;
                if (billStatus == 2 || billStatus == 6)
                    preFetchTask = PreFetchAllInventoryData(dto);

                // 1. Validate form (chạy song song với preFetchTask trên thread pool)
                var (isValid, errorMessage) = await ValidateBillExport(dto);
                if (!isValid)
                    return (false, errorMessage, 0);

                // 2. Validate phiếu mượn
                var borrowValidation = await ValidateBorrowBill(dto);
                if (!borrowValidation.Success)
                    return (false, borrowValidation.Message, 0);

                // Lấy kết quả pre-fetch (thường đã xong rồi vì chạy song song)
                var spInventoryProjectListCache = new Dictionary<(int, int, int, int), List<dynamic>>();
                var batchLookup = new Dictionary<(int, int, int), dynamic>();
                if (preFetchTask != null)
                    (spInventoryProjectListCache, batchLookup) = await preFetchTask;

                // 3. Tự động phân bổ kho giữ — thuần logic từ cache, không còn gọi DB
                await AutoAllocateInventoryProjects(dto, spInventoryProjectListCache, batchLookup);

                // 4. Validate Keep — thuần logic từ cache, không còn gọi DB
                var keepValidation = ValidateKeepInventory(dto, batchLookup);
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
        /// <summary>
        /// Lấy tất cả BillExportDetailIds liên quan đến cùng product/project/pokh
        /// </summary>
        private string GetRelatedBillExportDetailIds(
            List<BillExportDetailExtendedDTO> allDetails,
            int productId,
            int projectId,
            int pokhDetailId)
        {
            var relatedIds = allDetails
                .Where(d =>
                    d.ProductID == productId &&
                    //(pokhDetailId > 0
                    //    ? d.POKHDetailID == pokhDetailId
                    //    : d.ProjectID == projectId) &&
                    d.ID > 0) // ✅ Chỉ lấy detail đã có ID (đã lưu vào DB)
                .Select(d => d.ID.ToString())
                .ToList();

            return relatedIds.Any() ? string.Join(",", relatedIds) : "";
        }
        /// <summary>
        /// Validate tồn kho từ pre-fetched batchLookup — không gọi DB.
        /// </summary>
        private (bool Success, string Message) ValidateKeepInventory(
            BillExportDTO dto,
            Dictionary<(int, int, int), dynamic> batchLookup)
        {
            int status = dto.billExport.Status ?? 0;
            if (status != 2 && status != 6)
                return (true, string.Empty);

            var skipUnitNames = new[] { "m", "mét", "met" };
            var allDetails = (dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>()).ToList();

            var groupedQuantities = allDetails
                .GroupBy(d => new
                {
                    d.ProductID,
                    ProjectID = (d.POKHDetailID ?? 0) > 0 ? 0 : d.ProjectID,
                    POKHDetailID = d.POKHDetailID ?? 0
                })
                .ToDictionary(g => g.Key, g => g.Sum(d => d.Qty ?? 0));

            foreach (var detail in allDetails)
                {
                if (!string.IsNullOrWhiteSpace(detail.Unit) && skipUnitNames.Contains(detail.Unit.Trim().ToLower())) continue;
                if (!string.IsNullOrWhiteSpace(detail.UnitName) && skipUnitNames.Contains(detail.UnitName.Trim().ToLower())) continue;

                var grpKey = new
                {
                    detail.ProductID,
                    ProjectID = (detail.POKHDetailID ?? 0) > 0 ? 0 : detail.ProjectID,
                    POKHDetailID = detail.POKHDetailID ?? 0
                };
                if (groupedQuantities.ContainsKey(grpKey))
                    detail.TotalQty = groupedQuantities[grpKey];

                int productId = detail.ProductID ?? 0;
                int projectId = (detail.POKHDetailID ?? 0) > 0 ? 0 : detail.ProjectID ?? 0;
                int pokhDetailId = detail.POKHDetailID ?? 0;
                decimal totalQty = detail.TotalQty ?? 0;

                if (!batchLookup.TryGetValue((productId, projectId, pokhDetailId), out var row))
                    continue;

                decimal totalQuantityKeep = GetDecimalFromDynamic(row, "TotalQuantityKeep");
                decimal totalImport = GetDecimalFromDynamic(row, "TotalImport");
                decimal totalExport = GetDecimalFromDynamic(row, "TotalExport");
                decimal totalQuantityLast = GetDecimalFromDynamic(row, "TotalQuantityLast");

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
        /// <summary>
        /// Pre-fetch toàn bộ inventory data song song:
        /// - N task spGetInventoryProject (per unique key)
        /// - 1 task spGetInventoryProjectImportExportBatch (cho tất cả keys)
        /// Kết quả dùng chung cho cả AutoAllocate và ValidateKeep — không gọi DB thêm nữa.
        /// </summary>
        private async Task<(
            Dictionary<(int, int, int, int), List<dynamic>> inventoryProjectListCache,
            Dictionary<(int, int, int), dynamic> batchLookup
        )> PreFetchAllInventoryData(BillExportDTO dto)
        {
            var allDetails = (dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>()).ToList();
            int warehouseId = dto.billExport.WarehouseID ?? 0;

            // Unique keys cho spGetInventoryProject
            var uniqueKeys = allDetails
                .Where(d => (d.Qty ?? 0) > 0 && (d.ProductID ?? 0) > 0)
                .Select(d =>
                {
                    int pId = d.ProductID ?? 0;
                    int pokhId = d.POKHDetailID ?? 0;
                    int prId = pokhId > 0 ? 0 : d.ProjectID ?? 0;
                    return (warehouseId, pId, prId, pokhId);
                })
                .Distinct().ToList();

            // Params cho batch SP
            string billExportDetailIds = string.Join(",",
                allDetails.Where(d => d.ID > 0).Select(d => d.ID));
            string productParamsJson = "[" + string.Join(",", uniqueKeys
                .Select(k => (k.pId, k.prId, k.pokhId))
                .Distinct()
                .Select(g => $"{{\"ProductID\":{g.pId},\"ProjectID\":{g.prId},\"POKHDetailID\":{g.pokhId}}}")) + "]";

            // Bắn tất cả SP calls song song trong 1 Task.WhenAll
            var listTasks = uniqueKeys.Select(k => Task.Run(() =>
            {
                var data = GetInventoryProjectList(k.warehouseId, k.pId, k.prId, k.pokhId);
                return (k, data);
            })).ToList();

            var batchTask = Task.Run(() =>
                GetInventoryProjectImportExportBatch(warehouseId, billExportDetailIds, productParamsJson));

            await Task.WhenAll(listTasks.Concat<Task>(new[] { batchTask }));

            var inventoryProjectListCache = new Dictionary<(int, int, int, int), List<dynamic>>();
            foreach (var t in listTasks)
            {
                var (k, data) = t.Result;
                inventoryProjectListCache[k] = data;
            }

            var batchLookup = batchTask.Result.ToDictionary(
                r => ((int)GetIntFromDynamic(r, "ProductID"), (int)GetIntFromDynamic(r, "ProjectID"), (int)GetIntFromDynamic(r, "POKHDetailID")),
                r => (dynamic)r);

            return (inventoryProjectListCache, batchLookup);
        }

        private Task AutoAllocateInventoryProjects(
            BillExportDTO dto,
            Dictionary<(int, int, int, int), List<dynamic>> inventoryProjectListCache,
            Dictionary<(int, int, int), dynamic> batchLookup)
        {
            int status = dto.billExport.Status ?? 0;
            if (status != 2 && status != 6)
                return Task.CompletedTask;

            var allDetails = (dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>()).ToList();
            int warehouseId = dto.billExport.WarehouseID ?? 0;

            var detailsToAllocate = allDetails.Where(d =>
            {
                if (!string.IsNullOrWhiteSpace(d.ChosenInventoryProject)) return false;
                int pId = d.ProductID ?? 0;
                int pokhId = d.POKHDetailID ?? 0;
                int prId = pokhId > 0 ? 0 : d.ProjectID ?? 0;
                decimal qty = d.Qty ?? 0;
                return qty > 0 && pId > 0 && (prId > 0 || pokhId > 0);
            }).ToList();

            if (!detailsToAllocate.Any())
                return Task.CompletedTask;

            // Tất cả data đã pre-fetch — chỉ còn allocation loop thuần logic
            foreach (var detail in detailsToAllocate)
            {
                if (!string.IsNullOrWhiteSpace(detail.ChosenInventoryProject))
                    continue;

                var result = AutoAllocateInventoryProject(
                    detail, warehouseId, allDetails, inventoryProjectListCache, batchLookup);

                detail.ChosenInventoryProject = result.ChosenInventoryProject;
                detail.ProductCodeExport = result.ProductCodeExport;
            }

            return Task.CompletedTask;
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
    string billExportDetailIds) // ✅ Đổi từ string sang string để nhận nhiều ID
        {
            var result = SQLHelper<dynamic>.ProcedureToList(
                "spGetInventoryProjectImportExport",
                new string[] { "@WarehouseID", "@ProductID", "@ProjectID", "@POKHDetailID", "@BillExportDetailID" },
                new object[] { warehouseId, productId, projectId, pokhDetailId, billExportDetailIds ?? "" }
            );
            return result;
        }
        /// <summary>
        /// Batch version: gọi 1 lần SP cho toàn bộ danh sách sản phẩm.
        /// Trả về 1 row per (ProductID, ProjectID, POKHDetailID) với đầy đủ thông tin tồn kho.
        /// </summary>
        public List<dynamic> GetInventoryProjectImportExportBatch(
            int warehouseId,
            string billExportDetailIds,
            string productParamsJson)
        {
            var result = SQLHelper<dynamic>.ProcedureToList(
                "spGetInventoryProjectImportExportBatch",
                new[] { "@WarehouseID", "@BillExportDetailIDs", "@ProductParams" },
                new object[] { warehouseId, billExportDetailIds ?? "", productParamsJson ?? "[]" }
            );
            return result.Count > 0 ? result[0] : new List<dynamic>();
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

            return result.Count > 0 ? result[0] : new List<dynamic>();
        }

        private (string ChosenInventoryProject, string ProductCodeExport) AutoAllocateInventoryProject(
    BillExportDetailExtendedDTO currentDetail,
    int warehouseId,
            List<BillExportDetailExtendedDTO> allDetails,
            Dictionary<(int, int, int, int), List<dynamic>> inventoryProjectListCache,
            Dictionary<(int, int, int), dynamic> batchLookup)
        {
            int productId = currentDetail.ProductID ?? 0;
            int projectId = currentDetail.ProjectID ?? 0;
            int pokhDetailId = currentDetail.POKHDetailID ?? 0;
            if (pokhDetailId > 0) projectId = 0;
            decimal qty = currentDetail.Qty ?? 0;

            var spKey = (warehouseId, productId, projectId, pokhDetailId);

            // 1. Lấy danh sách kho giữ từ cache (đã pre-fetch)
            inventoryProjectListCache.TryGetValue(spKey, out var inventoryProjectsRaw);
            inventoryProjectsRaw ??= new List<dynamic>();

            // 2. Filter và sort
            var inventoryProjects = inventoryProjectsRaw
                .Where(x => GetDecimalFromDynamic(x, "TotalQuantityRemain") > 0)
                .OrderBy(x => GetDateTimeFromDynamic(x, "CreatedDate"))
                .ToList();

            // 3. Lấy TotalQuantityLast từ batchLookup (đã pre-fetch, không gọi SP)
            decimal totalStockAvailable = 0;
            if (batchLookup.TryGetValue((productId, projectId, pokhDetailId), out var batchRow))
                totalStockAvailable = Math.Max(0, GetDecimalFromDynamic(batchRow, "TotalQuantityLast"));

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
            // Batch load all details at once
            var details = _billExportDetailRepo.GetAll(x => deletedDetailIDs.Contains(x.ID));
            foreach (var detail in details)
                {
                    detail.IsDeleted = true;
                    detail.UpdatedBy = _currentUser.LoginName;
                    detail.UpdatedDate = DateTime.Now;
                }
            if (details.Any())
                await _billExportDetailRepo.UpdateRangeAsync_Binh(details);

            // Batch load and delete all serial numbers at once
            var serialNumbers = _serialNumberRepo.GetAll(x => deletedDetailIDs.Contains(x.BillExportDetailID ?? 0));
            if (serialNumbers.Any())
                await _serialNumberRepo.DeleteRangeAsync(serialNumbers);
        }

        private async Task SaveBillExportDetails(BillExportDTO dto, int billExportId)
        {
            var details = dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>();

            var newDetailDtos = details.Where(d => d.ID <= 0).ToList();
            var existingDetailDtos = details.Where(d => d.ID > 0).ToList();

            // Batch create new details
            if (newDetailDtos.Any())
            {
                var newEntities = newDetailDtos.Select(detail =>
            {
                detail.BillID = billExportId;
                    detail.IsDeleted = false;
                    detail.CreatedDate = DateTime.Now;
                    detail.CreatedBy = _currentUser.LoginName;
                    return MapToEntity(detail);
                }).ToList();

                await _billExportDetailRepo.CreateRangeAsync(newEntities);

                // Copy generated IDs back to DTOs
                for (int i = 0; i < newDetailDtos.Count; i++)
                    newDetailDtos[i].ID = newEntities[i].ID;
                }

            // Batch update existing details
            if (existingDetailDtos.Any())
            {
                var ids = existingDetailDtos.Select(d => d.ID).ToList();
                var existingEntities = _billExportDetailRepo.GetAll(x => ids.Contains(x.ID));
                var entityMap = existingEntities.ToDictionary(e => e.ID);

                foreach (var detail in existingDetailDtos)
                {
                    detail.BillID = billExportId;
                    detail.UpdatedDate = DateTime.Now;
                    detail.UpdatedBy = _currentUser.LoginName;

                    if (entityMap.TryGetValue(detail.ID, out var entity))
                        MapToExistingEntity(detail, entity);
                    }

                if (existingEntities.Any())
                    await _billExportDetailRepo.UpdateRangeAsync_Binh(existingEntities);
                }

            // Batch EnsureInventory: 1 SELECT query instead of N queries
            int warehouseId = dto.billExport.WarehouseID ?? 0;
            var uniqueProductIds = details
                .Select(d => d.ProductID ?? 0)
                .Where(id => id > 0)
                .Distinct();
            await EnsureInventoryExistsBatch(warehouseId, uniqueProductIds, dto.billExport.KhoTypeID ?? 0);

            // Batch SaveInventoryProjectExport: 3 queries total instead of 3N queries
                if (dto.billExport.Status == 2 || dto.billExport.Status == 6)
                {
                var detailsWithInventory = details
                    .Where(d => !string.IsNullOrWhiteSpace(d.ChosenInventoryProject))
                    .ToList();
                if (detailsWithInventory.Any())
                    await SaveInventoryProjectExportBatch(detailsWithInventory);
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
                CreatedBy = dto.CreatedBy,
                //TemQty = dto.TemQty,
                //IsTemVerify = dto.IsTemVerify,
                //IsHeavy = dto.IsHeavy,
                //OtherInfo = dto.OtherInfo
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
            //entity.TemQty = dto.TemQty;
            //entity.IsHeavy = dto.IsHeavy;
            //entity.IsTemVerify = dto.IsTemVerify;
            //entity.OtherInfo = dto.OtherInfo;
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

        private async Task EnsureInventoryExistsBatch(int warehouseId, IEnumerable<int> productIds, int productGroupID)
        {
            var productIdList = productIds.ToList();
            if (!productIdList.Any()) return;

            var existingProductIds = _inventoryRepo.GetAll(x =>
                x.WarehouseID == warehouseId &&
                productIdList.Contains(x.ProductSaleID ?? 0))
                .Select(x => x.ProductSaleID ?? 0)
                .ToHashSet();

            foreach (var productId in productIdList.Where(id => !existingProductIds.Contains(id)))
            {
                await _inventoryRepo.CreateAsync(new Inventory
                {
                    WarehouseID = warehouseId,
                    ProductSaleID = productId,
                    ProductGroupID = productGroupID,
                    TotalQuantityFirst = 0,
                    TotalQuantityLast = 0,
                    Import = 0,
                    Export = 0
                });
            }
        }

        private async Task SaveInventoryProjectExportBatch(List<BillExportDetailExtendedDTO> details)
        {
            var detailIds = details.Where(d => d.ID > 0).Select(d => d.ID).ToList();

            // 1. Batch soft-delete tất cả existing records trong 1 query
            if (detailIds.Any())
            {
                var allExisting = _inventoryProjectExportRepo.GetAll(x =>
                    detailIds.Contains(x.BillExportDetailID ?? 0) && x.IsDeleted != true);

                if (allExisting.Any())
                {
                    foreach (var existing in allExisting)
                    {
                        existing.IsDeleted = true;
                        existing.UpdatedBy = _currentUser.LoginName;
                        existing.UpdatedDate = DateTime.Now;
                    }
                    await _inventoryProjectExportRepo.UpdateRangeAsync_Binh(allExisting);
                }
            }

            // 2. Batch insert tất cả new records trong 1 query
            var newExports = new List<InventoryProjectExport>();
            foreach (var detail in details)
            {
                foreach (string item in (detail.ChosenInventoryProject ?? "").Split(';'))
                {
                    if (string.IsNullOrWhiteSpace(item)) continue;
                    var parts = item.Split('-');
                    if (parts.Length < 2) continue;
                    if (!int.TryParse(parts[0], out int inventoryProjectID)) continue;
                    if (!decimal.TryParse(parts[1], out decimal quantity)) continue;

                    newExports.Add(new InventoryProjectExport
                    {
                        BillExportDetailID = detail.ID,
                        InventoryProjectID = inventoryProjectID,
                        Quantity = quantity,
                        IsDeleted = false,
                        CreatedBy = _currentUser.LoginName,
                        CreatedDate = DateTime.Now
                    });
                }
            }

            if (newExports.Any())
                await _inventoryProjectExportRepo.CreateRangeAsync(newExports);
        }

        private async Task SaveInventoryProjectExport(BillExportDetailExtendedDTO detail)
        {
            string chosenInventoryProject = detail.ChosenInventoryProject ?? "";
            if (string.IsNullOrWhiteSpace(chosenInventoryProject))
                return;

            // Batch soft-delete các record cũ
            var existingRecords = _inventoryProjectExportRepo.GetAll(x =>
                x.BillExportDetailID == detail.ID && x.IsDeleted != true);

            if (existingRecords.Any())
            {
            foreach (var existing in existingRecords)
            {
                existing.IsDeleted = true;
                existing.UpdatedBy = _currentUser.LoginName;
                existing.UpdatedDate = DateTime.Now;
                }
                await _inventoryProjectExportRepo.UpdateRangeAsync_Binh(existingRecords);
            }

            // Batch create mới
            var newExports = new List<InventoryProjectExport>();
            foreach (string item in chosenInventoryProject.Split(';'))
            {
                if (string.IsNullOrWhiteSpace(item)) continue;

                var parts = item.Split('-');
                if (parts.Length < 2) continue;

                if (!int.TryParse(parts[0], out int inventoryProjectID)) continue;
                if (!decimal.TryParse(parts[1], out decimal quantity)) continue;

                newExports.Add(new InventoryProjectExport
                {
                    BillExportDetailID = detail.ID,
                    InventoryProjectID = inventoryProjectID,
                    Quantity = quantity,
                    IsDeleted = false,
                    CreatedBy = _currentUser.LoginName,
                    CreatedDate = DateTime.Now
                });
            }

            if (newExports.Any())
                await _inventoryProjectExportRepo.CreateRangeAsync(newExports);
        }

        private async Task CreateDocumentExports(int billExportId)
        {
            var documentExports = _documentExportRepo.GetAll(x => x.IsDeleted != true);

            var billDocuments = documentExports.Select(doc => new BillDocumentExport
                {
                    BillExportID = billExportId,
                    DocumentExportID = doc.ID
            }).ToList();

            if (billDocuments.Any())
                await _billDocumentExportRepo.CreateRangeAsync(billDocuments);
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
        private async Task SyncBillImportDetails(int billImportId, BillExportDTO dto)
        {
            var existingDetails = _billImportDetailRepo
                .GetAll(x => x.BillImportID == billImportId && x.IsDeleted != true);

            if (existingDetails.Any())
            {
                var detailIds = existingDetails.Select(d => d.ID).ToList();
                    var serials = _billImportDetailSerialNumberRepo
                    .GetAll(x => detailIds.Contains(x.BillImportDetailID ?? 0));

                if (serials.Any())
                    {
                    foreach (var serial in serials) serial.IsDeleted = true;
                    await _billImportDetailSerialNumberRepo.UpdateRangeAsync_Binh(serials);
                }

                foreach (var detail in existingDetails) detail.IsDeleted = true;
                await _billImportDetailRepo.UpdateRangeAsync_Binh(existingDetails);
            }

            var exportDetails = (dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>())
                .Where(d => (d.ProductID ?? 0) > 0).ToList();

            var newImportDetails = exportDetails.Select(exportDetail => new BillImportDetail
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
            }).ToList();

            if (newImportDetails.Any())
                await _billImportDetailRepo.CreateRangeAsync(newImportDetails);

            // EnsureInventoryExists with cache
            var inventoryCache = new HashSet<(int warehouseId, int productId)>();
            foreach (var exportDetail in exportDetails)
            {
                var cacheKey = (dto.billExport.WareHouseTranferID ?? 0, exportDetail.ProductID ?? 0);
                if (!inventoryCache.Contains(cacheKey))
                {
                    await EnsureInventoryExists(dto.billExport.WareHouseTranferID ?? 0, exportDetail.ProductID ?? 0);
                    inventoryCache.Add(cacheKey);
                }
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



        #region Load Existing Inventory Project Data
        /// <summary>
        /// Load ChosenInventoryProject từ DB cho các detail đang sửa
        /// Chỉ load nếu frontend KHÔNG gửi ChosenInventoryProject và ForceReallocate = false
        /// </summary>
        private async Task LoadExistingInventoryProjectData(BillExportDTO dto)
        {
            var allDetails = dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>();

            var detailIdsToLoad = allDetails
                .Where(d => d.ID > 0 && string.IsNullOrWhiteSpace(d.ChosenInventoryProject) && d.ForceReallocate != true)
                .Select(d => d.ID)
                .ToList();

            var allExistingExports = detailIdsToLoad.Any()
                ? _inventoryProjectExportRepo.GetAll(x => detailIdsToLoad.Contains(x.BillExportDetailID ?? 0) && x.IsDeleted != true)
                : new List<InventoryProjectExport>();

            var exportsByDetailId = allExistingExports
                .GroupBy(x => x.BillExportDetailID ?? 0)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var detail in allDetails)
            {
                if (detail.ID <= 0)
                    continue;

                if (!string.IsNullOrWhiteSpace(detail.ChosenInventoryProject))
                    continue;

                if (detail.ForceReallocate == true)
                {
                    continue;
                }

                exportsByDetailId.TryGetValue(detail.ID, out var existingExports);
                existingExports ??= new List<InventoryProjectExport>();

                if (existingExports.Any())
                {
                    // Format: "123-5;456-10"
                    detail.ChosenInventoryProject = string.Join(";",
                        existingExports.Select(x => $"{x.InventoryProjectID}-{x.Quantity}")
                    );
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