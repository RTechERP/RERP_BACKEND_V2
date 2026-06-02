using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System.Reflection;

namespace RERPAPI.Repo.GenericEntity
{
    public class BillExportSaleLogRepo : GenericRepo<BillExportSaleLog>
    {
        public BillExportSaleLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        #region Các trường hệ thống/nội bộ không cần ghi log thay đổi

        private static readonly HashSet<string> _ignoredProperties = new(StringComparer.OrdinalIgnoreCase)
        {
            // Audit & system fields
            "ID", "BillExportID", "CreatedDate", "UpdatedDate",
            "CreatedBy", "UpdatedBy", "IsDeleted", "STT",
            // Internal tracking fields
            "SupplierID", "StockID", "GroupID", "KhoTypeID",
            "AddressStockID", "IsMerge", "UnApprove", "IsPrepared", "IsReceived",
            "PreparedDate", "BillDocumentExportType", "BillImportID", "IsTranfer", "IsTransfer", "Deadline",
            "BillID", "ExportID", "TotalQty", "POKHID", "POKHDetailID",
            "TradePriceDetailID", "BillImportDetailID", "ProjectPartListID",
            "ReturnedStatus", "TemQty", "IsTemVerify", "IsHeavy",
            "ProductFullName", "ProjectName", "TotalInventory", "IsInvoice"
        };

        #endregion Các trường hệ thống/nội bộ không cần ghi log thay đổi

        #region Tên hiển thị tiếng Việt cho các property (thay vì tên cột DB)

        private static readonly Dictionary<string, string> _propertyLabels = new()
        {
            // ── BillExport ──
            ["Code"] = "Mã phiếu",
            ["CreatDate"] = "Ngày xuất",
            ["TypeBill"] = "Loại phiếu",
            ["Description"] = "Mô tả",
            ["Address"] = "Địa chỉ khách hàng",
            ["IsApproved"] = "Duyệt",
            ["Status"] = "Trạng thái",
            ["ProductType"] = "Loại hàng",
            ["WarehouseType"] = "Loại kho",
            ["WarehouseID"] = "Kho",
            ["RequestDate"] = "Ngày YC xuất",
            ["WareHouseTranferID"] = "Kho chuyển",
            ["UserID"] = "Người nhận",
            ["SenderID"] = "Người giao",
            ["CustomerID"] = "Khách hàng",
            // ── BillExportDetail ──
            ["ProductID"] = "Sản phẩm",
            ["ProductFullName"] = "Tên sản phẩm",
            ["Qty"] = "Số lượng",
            ["ProjectName"] = "Tên dự án",
            ["ProjectID"] = "Dự án",
            ["Note"] = "Ghi chú",
            ["GroupExport"] = "Nhóm xuất",
            ["InvoiceNumber"] = "Số hóa đơn",
            ["SerialNumber"] = "Số serial",
            ["Specifications"] = "Thông số/Model",
            ["TotalInventory"] = "Tồn kho",
            ["ExpectReturnDate"] = "Ngày dự kiến trả",
            ["CustomerResponse"] = "Phản hồi KH",
            ["OtherInfo"] = "Thông tin khác",
        };

        #endregion Tên hiển thị tiếng Việt cho các property (thay vì tên cột DB)

        #region So sánh 2 entity cùng loại, trả về danh sách mô tả thay đổi (tiếng Việt, resolve FK)

        public List<string> GetEntityChanges<T>(T oldObj, T newObj)
        {
            var changes = new List<string>();
            if (oldObj == null || newObj == null) return changes;

            // Thu thập các property thay đổi, tách riêng FK cần resolve qua DB
            var fkChanges = new List<(string Name, object? OldVal, object? NewVal)>();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                if (_ignoredProperties.Contains(prop.Name)) continue;

                var oldVal = prop.GetValue(oldObj);
                var newVal = prop.GetValue(newObj);

                if (oldVal == null && newVal == null) continue;

                var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                var label = _propertyLabels.GetValueOrDefault(prop.Name, prop.Name);

                // ── String: coi null và "" là giống nhau ──
                if (propType == typeof(string))
                {
                    var oldStr = (oldVal as string ?? "").Trim();
                    var newStr = (newVal as string ?? "").Trim();
                    if (oldStr == newStr) continue;

                    changes.Add($"{label}: '{oldStr}' → '{newStr}'");
                    continue;
                }

                // ── DateTime: so sánh mức ngày (tránh lệch múi giờ API) ──
                if (propType == typeof(DateTime))
                {
                    var oldDate = oldVal as DateTime?;
                    var newDate = newVal as DateTime?;
                    if (oldDate?.Date == newDate?.Date) continue;

                    changes.Add($"{label}: {oldDate?.ToString("dd/MM/yyyy") ?? "Trống"} → {newDate?.ToString("dd/MM/yyyy") ?? "Trống"}");
                    continue;
                }

                // ── Decimal: format số ──
                if (propType == typeof(decimal))
                {
                    var oldDec = oldVal as decimal? ?? 0m;
                    var newDec = newVal as decimal? ?? 0m;
                    if (oldDec == newDec) continue;

                    changes.Add($"{label}: {oldDec:N2} → {newDec:N2}");
                    continue;
                }

                // ── Bool: hiển thị Có/Không ──
                if (propType == typeof(bool))
                {
                    if (Equals(oldVal, newVal)) continue;

                    changes.Add($"{label}: {((oldVal as bool?) == true ? "Có" : "Không")} → {((newVal as bool?) == true ? "Có" : "Không")}");
                    continue;
                }

                // ── Status: enum nội bộ, map số → text (case 6 phụ thuộc IsTransfer) ──
                if (prop.Name == "Status")
                {
                    if (Equals(oldVal, newVal)) continue;
                    var isTransferProp = typeof(T).GetProperty("IsTransfer", BindingFlags.Public | BindingFlags.Instance);
                    var isTransfer = isTransferProp != null && (isTransferProp.GetValue(newObj) as bool?) == true;
                    changes.Add($"{label}: {StatusText(oldVal, isTransfer)} → {StatusText(newVal, isTransfer)}");
                    continue;
                }

                // ── ProductType: enum nội bộ, map số → text ──
                if (prop.Name == "ProductType")
                {
                    if (Equals(oldVal, newVal)) continue;
                    changes.Add($"{label}: {ProductTypeText(oldVal)} → {ProductTypeText(newVal)}");
                    continue;
                }

                // ── Int: coi 0 và null là giống nhau (thường dùng cho FK) ──
                if (propType == typeof(int))
                {
                    var oldInt = oldVal as int? ?? 0;
                    var newInt = newVal as int? ?? 0;
                    if (oldInt == newInt) continue;

                    fkChanges.Add((prop.Name, oldVal, newVal));
                    continue;
                }

                // ── FK (int) hoặc các kiểu khác: kiểm tra thay đổi, lưu lại để batch resolve ──
                if (Equals(oldVal, newVal)) continue;
                fkChanges.Add((prop.Name, oldVal, newVal));
            }

            // Batch resolve FK display values (chỉ tạo DbContext khi thực sự cần)
            if (fkChanges.Count > 0)
            {
                try
                {
                    using var db = new RTCContext();

                    // Thu thập tất cả ID cần lookup theo từng bảng
                    var productIds = new HashSet<int>();
                    var employeeIds = new HashSet<int>();
                    var supplierIds = new HashSet<int>();
                    var customerIds = new HashSet<int>();
                    var warehouseIds = new HashSet<int>();
                    var projectIds = new HashSet<int>();

                    foreach (var (name, oldVal, newVal) in fkChanges)
                    {
                        CollectFkId(name, oldVal, productIds, employeeIds, supplierIds, customerIds, warehouseIds, projectIds);
                        CollectFkId(name, newVal, productIds, employeeIds, supplierIds, customerIds, warehouseIds, projectIds);
                    }

                    // Batch load 1 query mỗi bảng (thay vì N+1)
                    var products = productIds.Count > 0
                        ? db.ProductSales.Where(x => productIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.ProductName ?? "")
                        : new Dictionary<int, string>();
                    var employees = employeeIds.Count > 0
                        ? db.Employees.Where(x => x.UserID.HasValue && employeeIds.Contains(x.UserID.Value))
                                      .ToList()
                                      .GroupBy(x => x.UserID!.Value)
                                      .ToDictionary(g => g.Key, g => g.First().FullName ?? "")
                        : new Dictionary<int, string>();
                    var suppliers = supplierIds.Count > 0
                        ? db.Suppliers.Where(x => supplierIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.SupplierName ?? "")
                        : new Dictionary<int, string>();
                    var customers = customerIds.Count > 0
                        ? db.Customers.Where(x => customerIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.CustomerName ?? "")
                        : new Dictionary<int, string>();
                    var warehouses = warehouseIds.Count > 0
                        ? db.Warehouses.Where(x => warehouseIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.WarehouseName ?? "")
                        : new Dictionary<int, string>();
                    var projects = projectIds.Count > 0
                        ? db.Projects.Where(x => projectIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.ProjectName ?? "")
                        : new Dictionary<int, string>();

                    foreach (var (name, oldVal, newVal) in fkChanges)
                    {
                        var label = _propertyLabels.GetValueOrDefault(name, name);
                        var oldText = ResolveFkDisplayValue(name, oldVal, products, employees, suppliers, customers, warehouses, projects);
                        var newText = ResolveFkDisplayValue(name, newVal, products, employees, suppliers, customers, warehouses, projects);

                        if (oldText != newText)
                            changes.Add($"{label}: {oldText} → {newText}");
                    }
                }
                catch
                {
                    // Fallback: ghi raw value nếu lookup thất bại
                    foreach (var (name, oldVal, newVal) in fkChanges)
                    {
                        var label = _propertyLabels.GetValueOrDefault(name, name);
                        changes.Add($"{label}: {oldVal ?? "Trống"} → {newVal ?? "Trống"}");
                    }
                }
            }

            return changes;
        }

        #endregion So sánh 2 entity cùng loại, trả về danh sách mô tả thay đổi (tiếng Việt, resolve FK)

        #region Phân loại ID vào đúng HashSet theo tên property (để batch query)

        private static void CollectFkId(string propertyName, object? value,
            HashSet<int> productIds, HashSet<int> userIds, HashSet<int> supplierIds,
            HashSet<int> customerIds, HashSet<int> warehouseIds, HashSet<int> projectIds)
        {
            if (value == null || !int.TryParse(value.ToString(), out int id) || id <= 0) return;

            switch (propertyName)
            {
                case "ProductID": productIds.Add(id); break;
                case "UserID": case "SenderID": userIds.Add(id); break;
                case "SupplierID": supplierIds.Add(id); break;
                case "CustomerID": customerIds.Add(id); break;
                case "WarehouseID": case "WareHouseTranferID": warehouseIds.Add(id); break;
                case "ProjectID": projectIds.Add(id); break;
            }
        }

        #endregion Phân loại ID vào đúng HashSet theo tên property (để batch query)

        #region Resolve FK ID thành tên hiển thị từ dictionary đã batch-load

        private static string ResolveFkDisplayValue(string propertyName, object? value,
            Dictionary<int, string> products, Dictionary<int, string> employees,
            Dictionary<int, string> suppliers, Dictionary<int, string> customers,
            Dictionary<int, string> warehouses, Dictionary<int, string> projects)
        {
            if (value == null) return "Trống";
            var text = value.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(text)) return "Trống";
            if (!int.TryParse(text, out int id) || id <= 0) return text;

            return propertyName switch
            {
                "ProductID" => products.GetValueOrDefault(id, text),
                "UserID" or "SenderID" => employees.GetValueOrDefault(id, text),
                "SupplierID" => suppliers.GetValueOrDefault(id, text),
                "CustomerID" => customers.GetValueOrDefault(id, text),
                "WarehouseID" or "WareHouseTranferID" => warehouses.GetValueOrDefault(id, text),
                "ProjectID" => projects.GetValueOrDefault(id, text),
                _ => text
            };
        }

        #endregion Resolve FK ID thành tên hiển thị từ dictionary đã batch-load

        #region Map Status (int) → tên trạng thái phiếu xuất tiếng Việt

        private static string StatusText(object? value, bool isTransfer = false)
        {
            if (value == null || !int.TryParse(value.ToString(), out int v)) return "Trống";
            return v switch
            {
                0 => "Mượn",
                1 => "Tồn kho",
                2 => "Đã xuất kho",
                3 => "Chia trước",
                4 => "Phiếu mượn nội bộ",
                5 => "Xuất trả NCC",
                6 => isTransfer ? "Yêu cầu chuyển kho" : "Yêu cầu xuất kho",
                7 => "Yêu cầu mượn",
                _ => v.ToString()
            };
        }

        #endregion Map Status (int) → tên trạng thái phiếu xuất tiếng Việt

        #region Map ProductType (int) → tên loại hàng tiếng Việt

        private static string ProductTypeText(object? value)
        {
            if (value == null || !int.TryParse(value.ToString(), out int v)) return "Trống";
            return v switch
            {
                1 => "Hàng thương mại",
                2 => "Hàng dự án",
                _ => v.ToString()
            };
        }

        #endregion Map ProductType (int) → tên loại hàng tiếng Việt
    }
}