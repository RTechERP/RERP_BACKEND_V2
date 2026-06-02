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
		private readonly BillExportSaleLogRepo _billExportSaleLogRepo;
		private readonly InventoryProjectRepo _inventoryProjectRepo;

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
			ProductGroupRepo productGroupRepo,
			BillExportSaleLogRepo billExportSaleLogRepo,
			InventoryProjectRepo inventoryProjectRepo
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
			_billExportSaleLogRepo = billExportSaleLogRepo;
			_inventoryProjectRepo = inventoryProjectRepo;
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
					await HandleDeletedDetails(dto.DeletedDetailIDs, billExportId);

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
			// IDs từ DTO hiện tại (đúng project)
			var relatedIds = allDetails
				.Where(d =>
					d.ProductID == productId &&
					(pokhDetailId > 0
						? d.POKHDetailID == pokhDetailId
						: d.ProjectID == projectId) &&
					d.ID > 0)
				.Select(d => d.ID)
				.ToHashSet();

			// Thêm: IDs của các detail trong DTO có cùng ProductID nhưng đã đổi project
			// → cần loại trừ khỏi SP để tránh double-count
			var changedProjectIds = allDetails
				.Where(d =>
					d.ProductID == productId &&
					d.ID > 0 &&
					!relatedIds.Contains(d.ID)
				)
				.Select(d => d.ID)
				.ToList();

			if (changedProjectIds.Any())
			{
				var dbDetails = _billExportDetailRepo.GetAll(x =>
					changedProjectIds.Contains(x.ID) &&
					x.IsDeleted != true &&
					(pokhDetailId > 0
						? x.POKHDetailID == pokhDetailId
						: x.ProjectID == projectId)
				);

				foreach (var d in dbDetails)
					relatedIds.Add(d.ID);
			}

			return relatedIds.Any() ? string.Join(",", relatedIds) : "";
		}
		private async Task<(bool Success, string Message)> ValidateKeepInventory(BillExportDTO dto)
		{
			int status = dto.billExport.Status ?? 0;
			if (status != 2 && status != 6)
				return (true, string.Empty);

			var skipUnitNames = new[] { "m", "mét", "met" };
			var allDetails = (dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>()).ToList();

			var groupedQuantities = allDetails
				.GroupBy(d => new
				{
					ProductID = d.ProductID ?? 0,
					ProjectID = (d.POKHDetailID ?? 0) > 0 ? 0 : d.ProjectID ?? 0,
					POKHDetailID = d.POKHDetailID ?? 0
				})
				.ToDictionary(g => g.Key, g => g.Sum(d => d.Qty ?? 0));

			var toValidate = allDetails.Where(d =>
				(string.IsNullOrWhiteSpace(d.Unit) || !skipUnitNames.Contains(d.Unit.Trim().ToLower())) &&
				(string.IsNullOrWhiteSpace(d.UnitName) || !skipUnitNames.Contains(d.UnitName.Trim().ToLower()))
			).ToList();

			if (!toValidate.Any())
				return (true, string.Empty);

			// Các key riêng biệt để gọi batch SP 1 lần
			var distinctKeys = toValidate
				.Select(d => (
					ProductID: d.ProductID ?? 0,
					ProjectID: (d.POKHDetailID ?? 0) > 0 ? 0 : d.ProjectID ?? 0,
					POKHDetailID: d.POKHDetailID ?? 0
				))
				.Distinct()
				.ToList();

			// Gom tất cả BillExportDetailIds cần loại trừ
			var allExcludeIds = new HashSet<int>();
			foreach (var key in distinctKeys)
			{
				string ids = GetRelatedBillExportDetailIds(allDetails, key.ProductID, key.ProjectID, key.POKHDetailID);
				foreach (var part in ids.Split(',', StringSplitOptions.RemoveEmptyEntries))
					if (int.TryParse(part.Trim(), out int pid)) allExcludeIds.Add(pid);
			}

			string productParamsJson = System.Text.Json.JsonSerializer.Serialize(
				distinctKeys.Select(k => new { k.ProductID, k.ProjectID, k.POKHDetailID })
			);
			string combinedExcludeIds = allExcludeIds.Any() ? string.Join(",", allExcludeIds) : "";

			// Gọi batch SP 1 lần thay vì N lần
			var batchRows = GetInventoryProjectImportExportBatch(
				dto.billExport?.WarehouseID ?? 0,
				productParamsJson,
				combinedExcludeIds
			);

			var stockLookup = new Dictionary<(int, int, int), dynamic>();
			foreach (var row in batchRows)
			{
				var r = (dynamic)row;
				int pid = Convert.ToInt32(r.ProductID);
				int pjid = Convert.ToInt32(r.ProjectID);
				int pokhid = Convert.ToInt32(r.POKHDetailID);
				var k = (pid, pjid, pokhid);
				if (!stockLookup.ContainsKey(k))
					stockLookup[k] = row; // Giữ row đầu tiên nếu SP trả về duplicate
			}

			foreach (var detail in toValidate)
			{
				var groupKey = new
				{
					ProductID = detail.ProductID ?? 0,
					ProjectID = (detail.POKHDetailID ?? 0) > 0 ? 0 : detail.ProjectID ?? 0,
					POKHDetailID = detail.POKHDetailID ?? 0
				};

				if (groupedQuantities.TryGetValue(groupKey, out var grouped))
					detail.TotalQty = grouped;

				decimal totalQty = detail.TotalQty ?? 0;
				var lookupKey = (groupKey.ProductID, groupKey.ProjectID, groupKey.POKHDetailID);

				decimal totalQuantityKeep = 0, totalQuantityLast = 0, totalImport = 0, totalExport = 0;
				if (stockLookup.TryGetValue(lookupKey, out var stockRow))
				{
					var r = (dynamic)stockRow;
					totalQuantityKeep = Convert.ToDecimal(r.TotalQuantityKeep);
					totalQuantityLast = Convert.ToDecimal(r.TotalQuantityLast);
					totalImport = Convert.ToDecimal(r.TotalImport);
					totalExport = Convert.ToDecimal(r.TotalExport);
				}

				decimal totalQuantityRemain = Math.Max(totalImport - totalExport, 0);
				decimal totalStock = Math.Max(totalQuantityKeep, 0) + totalQuantityRemain + Math.Max(totalQuantityLast, 0);

				if (totalStock < totalQty)
				{
					string productDisplay = detail.ProductNewCode ?? detail.ProductCode ?? $"ID:{detail.ProductID}";
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
			if (status != 2 && status != 6) return;

			int warehouseId = dto.billExport.WarehouseID ?? 0;
			var allDetails = (dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>()).ToList();

			var toAllocate = allDetails.Where(d =>
				string.IsNullOrWhiteSpace(d.ChosenInventoryProject) &&
				(d.ProductID ?? 0) > 0 &&
				(d.Qty ?? 0) > 0 &&
				((d.POKHDetailID ?? 0) > 0 || (d.ProjectID ?? 0) > 0)
			).ToList();

			if (!toAllocate.Any()) return;

			// Build distinct keys
			var distinctKeys = toAllocate
				.Select(d => (
					ProductID: d.ProductID ?? 0,
					ProjectID: (d.POKHDetailID ?? 0) > 0 ? 0 : d.ProjectID ?? 0,
					POKHDetailID: d.POKHDetailID ?? 0
				))
				.Distinct()
				.ToList();

			string productParamsJson = System.Text.Json.JsonSerializer.Serialize(
				distinctKeys.Select(k => new { k.ProductID, k.ProjectID, k.POKHDetailID })
			);

			// Batch 1: lấy stock tổng (spGetInventoryProjectImportExportBatch)
			var allExcludeIds = new HashSet<int>();
			foreach (var key in distinctKeys)
			{
				string ids = GetRelatedBillExportDetailIds(allDetails, key.ProductID, key.ProjectID, key.POKHDetailID);
				foreach (var part in ids.Split(',', StringSplitOptions.RemoveEmptyEntries))
					if (int.TryParse(part.Trim(), out int xid)) allExcludeIds.Add(xid);
			}
			string combinedExcludeIds = allExcludeIds.Any() ? string.Join(",", allExcludeIds) : "";
			var batchStockRows = GetInventoryProjectImportExportBatch(warehouseId, productParamsJson, combinedExcludeIds);

			var stockLookup = new Dictionary<(int, int, int), decimal>();
			foreach (var row in batchStockRows)
			{
				var r = (IDictionary<string, object>)row;
				int pid = Convert.ToInt32(r["ProductID"]);
				int pjid = Convert.ToInt32(r["ProjectID"]);
				int pokhid = Convert.ToInt32(r["POKHDetailID"]);
				var k = (pid, pjid, pokhid);
				if (!stockLookup.ContainsKey(k))
					stockLookup[k] = Convert.ToDecimal(r["TotalQuantityLast"]);
			}

			// Batch 2: lấy danh sách kho giữ (spGetInventoryProjectBatch) — 1 lần thay vì N lần
			var inventoryByKey = GetInventoryProjectBatch(warehouseId, productParamsJson);

			foreach (var detail in toAllocate)
			{
				int productId = detail.ProductID ?? 0;
				int pokhDetailId = detail.POKHDetailID ?? 0;
				int projectId = pokhDetailId > 0 ? 0 : detail.ProjectID ?? 0;
				var cacheKey = (productId, projectId, pokhDetailId);

				var inventoryProjectsRaw = inventoryByKey.TryGetValue(cacheKey, out var list) ? list : new List<dynamic>();
				decimal totalStockAvailable = stockLookup.TryGetValue(cacheKey, out var stock) ? Math.Max(0, stock) : 0;

				var result = await AutoAllocateInventoryProject(detail, warehouseId, allDetails, inventoryProjectsRaw, totalStockAvailable);
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
	string billExportDetailIds) // ✅ Đổi từ string sang string để nhận nhiều ID
		{
			var result = SQLHelper<dynamic>.ProcedureToList(
				"spGetInventoryProjectImportExport",
				new string[] { "@WarehouseID", "@ProductID", "@ProjectID", "@POKHDetailID", "@BillExportDetailID" },
				new object[] { warehouseId, productId, projectId, pokhDetailId, billExportDetailIds ?? "" }
			);
			return result;
		}
		public List<dynamic> GetInventoryProjectImportExportBatch(
			int warehouseId,
			string productParamsJson,
			string billExportDetailIds)
		{
			var result = SQLHelper<dynamic>.ProcedureToList(
				"spGetInventoryProjectImportExportBatch",
				new string[] { "@WarehouseID", "@BillExportDetailIDs", "@ProductParams" },
				new object[] { warehouseId, billExportDetailIds ?? "", productParamsJson }
			);
			return result.Count > 0 ? result[0] : new List<dynamic>();
		}

		/// <summary>
		/// Lấy danh sách kho giữ có thể phân bổ (từ spGetInventoryProject) — single key
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
				new object[] { pokhDetailId > 0 ? 0 : projectId, 0, productId, "", warehouseId, pokhDetailId }
			);

			return result.Count > 0 ? result[0] : new List<dynamic>();
		}

		/// <summary>
		/// Batch version — nhận nhiều (ProductID, ProjectID, POKHDetailID) qua JSON, 1 lần gọi SP
		/// Trả về Dictionary keyed by (ProductID, ProjectID, POKHDetailID) → List rows
		/// </summary>
		public Dictionary<(int, int, int), List<dynamic>> GetInventoryProjectBatch(int warehouseId, string productParamsJson)
		{
			var result = SQLHelper<dynamic>.ProcedureToList(
				"spGetInventoryProjectBatch",
				new string[] { "@WarehouseID", "@ProductParams" },
				new object[] { warehouseId, productParamsJson }
			);

			var rows = result.Count > 0 ? result[0] : new List<dynamic>();
			var lookup = new Dictionary<(int, int, int), List<dynamic>>();

			foreach (var row in rows)
			{
				var r = (IDictionary<string, object>)row;
				int pid = Convert.ToInt32(r["ProductSaleID"]);
				int pjid = Convert.ToInt32(r["InputProjectID"]);
				int pokhid = Convert.ToInt32(r["InputPOKHDetailID"]);
				var k = (pid, pjid, pokhid);
				if (!lookup.ContainsKey(k)) lookup[k] = new List<dynamic>();
				lookup[k].Add(row);
			}

			return lookup;
		}
		private async Task<(string ChosenInventoryProject, string ProductCodeExport)> AutoAllocateInventoryProject(
			BillExportDetailExtendedDTO currentDetail,
			int warehouseId,
			List<BillExportDetailExtendedDTO> allDetails,
			List<dynamic> inventoryProjectsRaw,
			decimal totalStockAvailable)
		{
			int productId = currentDetail.ProductID ?? 0;
			int projectId = currentDetail.ProjectID ?? 0;
			int pokhDetailId = currentDetail.POKHDetailID ?? 0;
			if (pokhDetailId > 0) projectId = 0;
			decimal qty = currentDetail.Qty ?? 0;

			// 1. Filter và sort từ data pre-fetched (không gọi SP)
			var inventoryProjects = inventoryProjectsRaw
				.Where(x => GetDecimalFromDynamic(x, "TotalQuantityRemain") > 0)
				.OrderBy(x => GetDateTimeFromDynamic(x, "CreatedDate"))
				.ToList();

			// 2. Nếu không có kho giữ
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

			// 3. Tính toán số lượng đã sử dụng
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

				_billExportSaleLogRepo.Create(new BillExportSaleLog
				{
					BillExportID = dto.billExport.ID,
					TypeLog = "TẠO MỚI PHIẾU",
					ContentLog = $"Tạo mới phiếu xuất",
					CreatedBy = _currentUser.LoginName,
					CreatedDate = DateTime.Now
				});
			}
			else
			{
				var existingMaster = GetSingleNoTracking(x => x.ID == dto.billExport.ID);
				await UpdateAsync(dto.billExport);

				List<string> changeDetails = _billExportSaleLogRepo.GetEntityChanges(existingMaster, dto.billExport);
				if (changeDetails.Any())
				{
					_billExportSaleLogRepo.Create(new BillExportSaleLog
					{
						BillExportID = dto.billExport.ID,
						TypeLog = "CẬP NHẬT PHIẾU",
						ContentLog = $"Cập nhật phiếu xuất: {string.Join(", ", changeDetails)}",
						CreatedBy = _currentUser.LoginName,
						CreatedDate = DateTime.Now
					});
				}
			}
			if (dto.billExport.IsTransfer == true)
			{
				await HandleTransferWarehouse(dto, dto.billExport.ID);

			}
			return dto.billExport.ID;
		}

		private async Task HandleDeletedDetails(List<int> deletedDetailIDs, int billExportId)
		{
			if (!deletedDetailIDs.Any()) return;

			// Batch fetch thay vì N lần query riêng lẻ
			var details = _billExportDetailRepo
				.GetAll(x => deletedDetailIDs.Contains(x.ID) && x.IsDeleted != true)
				.ToList();
			var allSerials = _serialNumberRepo
				.GetAll(x => deletedDetailIDs.Contains(x.BillExportDetailID ?? 0))
				.ToList();

			foreach (var sn in allSerials)
				await _serialNumberRepo.DeleteAsync(sn.ID);

			foreach (var detail in details)
			{
				detail.IsDeleted = true;
				detail.UpdatedBy = _currentUser.LoginName;
				detail.UpdatedDate = DateTime.Now;
				await _billExportDetailRepo.UpdateAsync(detail);

				var deletedProductName = detail.ProductFullName ?? $"ID:{detail.ProductID}";
				_billExportSaleLogRepo.Create(new BillExportSaleLog
				{
					BillExportID = billExportId,
					TypeLog = "XOÁ CHI TIẾT",
					ContentLog = $"Xoá chi tiết phiếu xuất: {deletedProductName}",
					CreatedBy = _currentUser.LoginName,
					CreatedDate = DateTime.Now
				});
			}
		}

		private async Task SaveBillExportDetails(BillExportDTO dto, int billExportId)
		{
			int warehouseId = dto.billExport.WarehouseID ?? 0;
			var allProductIds = (dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>())
				.Where(d => (d.ProductID ?? 0) > 0)
				.Select(d => d.ProductID!.Value)
				.Distinct()
				.ToList();

			// Pre-fetch toàn bộ inventory 1 lần thay vì N lần trong loop
			var existingInventorySet = _inventoryRepo
				.GetAll(x => x.WarehouseID == warehouseId && allProductIds.Contains(x.ProductSaleID ?? 0))
				.Select(x => x.ProductSaleID ?? 0)
				.ToHashSet();

			var existingDetailDtos = new List<BillExportDetailExtendedDTO>();

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

					var addedProductName = detail.ProductName ?? $"ID:{detail.ProductID}";
					var addedParts = new List<string>();

					if (detail.Qty != null) addedParts.Add($"SL xuất: {detail.Qty}");
					if (!string.IsNullOrWhiteSpace(detail.ProjectName)) addedParts.Add($"Dự án: {detail.ProjectName}");
					if (!string.IsNullOrWhiteSpace(detail.Note)) addedParts.Add($"Ghi chú: {detail.Note}");
					if (!string.IsNullOrWhiteSpace(detail.GroupExport)) addedParts.Add($"Nhóm xuất: {detail.GroupExport}");
					if (!string.IsNullOrWhiteSpace(detail.InvoiceNumber)) addedParts.Add($"Số HĐ: {detail.InvoiceNumber}");
					if (!string.IsNullOrWhiteSpace(detail.SerialNumber)) addedParts.Add($"Serial: {detail.SerialNumber}");
					if (!string.IsNullOrWhiteSpace(detail.Specifications)) addedParts.Add($"Thông số/Model: {detail.Specifications}");

					_billExportSaleLogRepo.Create(new BillExportSaleLog
					{
						BillExportID = billExportId,
						TypeLog = "THÊM CHI TIẾT",
						ContentLog = $"Thêm chi tiết phiếu xuất: [{addedProductName}] {string.Join(", ", addedParts)}",
						CreatedBy = _currentUser.LoginName,
						CreatedDate = DateTime.Now
					});

					// Tạo Inventory nếu chưa có (dùng pre-fetched set)
					int productId = detail.ProductID ?? 0;
					if (productId > 0 && !existingInventorySet.Contains(productId))
					{
						await _inventoryRepo.CreateAsync(new Inventory
						{
							WarehouseID = warehouseId,
							ProductSaleID = productId,
							TotalQuantityFirst = 0,
							TotalQuantityLast = 0,
							Import = 0,
							Export = 0
						});
						existingInventorySet.Add(productId);
					}

					if (dto.billExport.Status == 2 || dto.billExport.Status == 6)
						await SaveInventoryProjectExport(detail);
				}
				else
				{
					existingDetailDtos.Add(detail);
				}
			}

			// Xử lý cập nhật các chi tiết đã có
			if (existingDetailDtos.Any())
			{
				var ids = existingDetailDtos.Select(d => d.ID).ToList();
				var existingEntities = _billExportDetailRepo.GetAll(x => ids.Contains(x.ID)).ToList();
				var entityMap = existingEntities.ToDictionary(e => e.ID);

				foreach (var detail in existingDetailDtos)
				{
					detail.UpdatedDate = DateTime.Now;
					detail.UpdatedBy = _currentUser.LoginName;
					detail.CreatedBy = _currentUser.LoginName;
					detail.CreatedDate = DateTime.Now;

					if (entityMap.TryGetValue(detail.ID, out var existingEntity))
					{
						List<string> changeDetails = _billExportSaleLogRepo.GetEntityChanges<BillExportDetail>(existingEntity, detail);

						if (changeDetails.Any())
						{
							bool isProductChanged = existingEntity.ProductID != detail.ProductID;
							var updatedProductName = detail.ProductName ?? $"ID:{detail.ProductID}";
							string contentLogPrefix = isProductChanged
								? "Cập nhật chi tiết"
								: $"Cập nhật chi tiết [{updatedProductName}]";

							_billExportSaleLogRepo.Create(new BillExportSaleLog
							{
								BillExportID = billExportId,
								TypeLog = "CẬP NHẬT CHI TIẾT",
								ContentLog = $"{contentLogPrefix}: {string.Join(", ", changeDetails)}",
								CreatedBy = _currentUser.LoginName,
								CreatedDate = DateTime.Now
							});
						}

						// Dùng entity từ entityMap, bỏ GetByIDAsync thừa
						MapToExistingEntity(detail, existingEntity);
						await _billExportDetailRepo.UpdateAsync(existingEntity);
					}

					// Tạo Inventory nếu chưa có (dùng pre-fetched set)
					int productId = detail.ProductID ?? 0;
					if (productId > 0 && !existingInventorySet.Contains(productId))
					{
						await _inventoryRepo.CreateAsync(new Inventory
						{
							WarehouseID = warehouseId,
							ProductSaleID = productId,
							TotalQuantityFirst = 0,
							TotalQuantityLast = 0,
							Import = 0,
							Export = 0
						});
						existingInventorySet.Add(productId);
					}

					if (dto.billExport.Status == 2 || dto.billExport.Status == 6)
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
				x.ProductSaleID == productId) //&& x.ProductGroupID == productGroupId)
				.FirstOrDefault();

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
					SupplierID = 1175,
					Suplier = "NHẬP NỘI BỘ",
					GroupID = dto.billExport.GroupID,
					DateRequestImport = DateTime.Now,
					BillTypeNew = 4,
					BillImportCode = _billImportRepo.GetBillCode(4),
					WarehouseID = dto.billExport.WareHouseTranferID ?? 0,
					CreatDate = dto.billExport.CreatDate,
					Status = false,
					IsDeleted = false,
					KhoType = productGroup?.ProductGroupName,
					RulePayID = 34
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

				// 1. Xóa mềm SerialNumber (batch fetch thay vì N lần query)
				var allSerials = _billImportDetailSerialNumberRepo
					.GetAll(x => detailIds.Contains(x.BillImportDetailID ?? 0))
					.ToList();
				foreach (var serial in allSerials)
				{
					serial.IsDeleted = true;
					await _billImportDetailSerialNumberRepo.UpdateAsync(serial);
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
					ProjectCode = exportDetail.ProductFullName,
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
			int warehouseId = dto.billExport.WarehouseID ?? 0;

			var detailsToProcess = (dto.billExportDetail ?? new List<BillExportDetailExtendedDTO>())
				.Where(d => d.ID > 0 && string.IsNullOrWhiteSpace(d.ChosenInventoryProject) && d.ForceReallocate != true)
				.ToList();

			if (!detailsToProcess.Any()) return;

			// Batch 1: toàn bộ InventoryProjectExport của tất cả detail
			var detailIds = detailsToProcess.Select(d => d.ID).ToList();
			var exportsByDetailId = _inventoryProjectExportRepo
				.GetAll(x => detailIds.Contains(x.BillExportDetailID ?? 0) && x.IsDeleted != true)
				.GroupBy(x => x.BillExportDetailID)
				.ToDictionary(g => g.Key, g => g.ToList());

			var allInventoryProjectIds = exportsByDetailId.Values
				.SelectMany(e => e)
				.Select(x => x.InventoryProjectID ?? 0)
				.Where(id => id > 0)
				.Distinct()
				.ToList();

			if (!allInventoryProjectIds.Any()) return;

			// Batch 2: truy vấn trực tiếp bảng InventoryProject (thay vì N lần gọi spGetInventoryProject)
			var inventoryProjectMap = _inventoryProjectRepo
				.GetAll(x => allInventoryProjectIds.Contains(x.ID) && x.IsDeleted != true && x.WarehouseID == warehouseId)
				.ToDictionary(x => x.ID);

			// Batch 3: lấy ProductCode từ bảng ProductSale
			var productSaleIds = inventoryProjectMap.Values
				.Select(x => x.ProductSaleID ?? 0).Where(id => id > 0).Distinct().ToList();
			var productCodeMap = _productSaleRepo
				.GetAll(x => productSaleIds.Contains(x.ID))
				.ToDictionary(x => x.ID, x => x.ProductCode ?? "");

			foreach (var detail in detailsToProcess)
			{
				if (!exportsByDetailId.TryGetValue(detail.ID, out var existingExports) || !existingExports.Any())
					continue;

				int productId = detail.ProductID ?? 0;
				int pokhDetailId = detail.POKHDetailID ?? 0;
				int projectId = pokhDetailId > 0 ? 0 : detail.ProjectID ?? 0;

				var validExports = existingExports
					.Where(x =>
					{
						int ipId = x.InventoryProjectID ?? 0;
						if (!inventoryProjectMap.TryGetValue(ipId, out var ip)) return false;
						if ((ip.ProductSaleID ?? 0) != productId) return false;
						if (pokhDetailId > 0) return (ip.POKHDetailID ?? 0) == pokhDetailId;
						return (ip.ProjectID ?? 0) == projectId;
					})
					.ToList();

				if (!validExports.Any())
				{
					detail.ChosenInventoryProject = "";
					continue;
				}

				detail.ChosenInventoryProject = string.Join(";",
					validExports.Select(x => $"{x.InventoryProjectID}-{x.Quantity}")
				);

				detail.ProductCodeExport = string.Join(";",
					validExports
						.Select(x =>
						{
							if (!inventoryProjectMap.TryGetValue(x.InventoryProjectID ?? 0, out var ip)) return "";
							return productCodeMap.TryGetValue(ip.ProductSaleID ?? 0, out var code) ? code : "";
						})
						.Where(code => !string.IsNullOrEmpty(code))
				);
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