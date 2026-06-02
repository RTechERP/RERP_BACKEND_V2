using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System.Reflection;

namespace RERPAPI.Repo.GenericEntity.Technical
{
    public class BillImportTechnicalAuditLogRepo : GenericRepo<BillImportTechnicalAuditLog>
    {
        public BillImportTechnicalAuditLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        #region Các trường hệ thống/nội bộ không cần ghi log thay đổi

        private static readonly HashSet<string> _ignoredProperties = new(StringComparer.OrdinalIgnoreCase)
        {
            // Audit & system fields
            "ID", "WarehouseType", "CreatedDate", "UpdatedDate",
            "CreatedBy", "UpdatedBy", "IsDeleted", "STT", "BIllExportSaleID", "IsBorrowSupplier", "Image", "BillCode",
            "WarehouseID", "TotalQuantity", "UnitName", "UnitID", "ReceiverID", "SuplierID",
            // Internal tracking fields
            "BillExportDetailSaleID", "DPO", "DueDate", "TaxReduction", "COFormE",
            "DeadlineReturnNCC", "EmployeIDBorrow", "BillCodePO", "PONCCDetailID", "QtyRequest", "IsBorrowSupplier", "InternalCode",
             "ProjectName", "ProjectCode",
        };

        #endregion Các trường hệ thống/nội bộ không cần ghi log thay đổi

        #region Tên hiển thị tiếng Việt cho các property (thay vì tên cột DB)

        private static readonly Dictionary<string, string> _propertyLabels = new()
        {
            // ── BillImportTechnical ──
            ["BillTypeNew"] = "Loại phiếu",
            ["RulePayID"] = "Điều khoản TT",
            ["IsNormalize"] = "Chuẩn hóa",
            ["Receiver"] = "Người nhận",
            ["DeliverID"] = "Người giao",
            ["Deliver"] = "Tên người giao",
            ["CustomerID"] = "Khách hàng",
            ["CreatDate"] = "Ngày nhập",
            ["ApproverID"] = "Người duyệt",
            ["SupplierSaleID"] = "Nhà cung cấp",

            // ── BillImportDetailTechnical ──
            ["ProductID"] = "Sản phẩm",
            ["ProductCodeRTC"] = "Số lượng",
            ["Quantity"] = "Số lượng",
            ["Price"] = "Đơn giá",
            ["TotalPrice"] = "Thành tiền",
            ["SomeBill"] = "Số hóa đơn",
            ["Note"] = "Ghi chú",
            ["DateSomeBill"] = "Ngày hóa đơn",
            ["TaxReduction"] = "Giảm thuế",
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

                // ── BillTypeNew: enum nội bộ, map số → text ──
                if (prop.Name == "BillTypeNew")
                {
                    if (Equals(oldVal, newVal)) continue;
                    changes.Add($"{label}: {BillTypeText(oldVal)} → {BillTypeText(newVal)}");
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
                    var approverIds = new HashSet<int>();
                    var supplierIds = new HashSet<int>();
                    var warehouseIds = new HashSet<int>();
                    var projectIds = new HashSet<int>();
                    var rulePayIds = new HashSet<int>();
                    var customerIds = new HashSet<int>();

                    foreach (var (name, oldVal, newVal) in fkChanges)
                    {
                        CollectFkId(name, oldVal, productIds, employeeIds, supplierIds, warehouseIds, projectIds, rulePayIds, approverIds, customerIds);
                        CollectFkId(name, newVal, productIds, employeeIds, supplierIds, warehouseIds, projectIds, rulePayIds, approverIds, customerIds);
                    }

                    // Batch load 1 query mỗi bảng (thay vì N+1)
                    var products = productIds.Count > 0
                        ? db.ProductRTCs
                            .Where(p => productIds.Contains(p.ID))
                            .Select(p => new
                            {
                                p.ID,
                                p.ProductCode,
                                p.ProductName,
                                p.ProductCodeRTC,
                                p.Maker,
                                UnitName = db.UnitCountKTs.Where(u => u.ID == p.UnitCountID).Select(u => u.UnitCountName).FirstOrDefault()
                            })
                            .AsEnumerable()
                            .ToDictionary(
                                x => x.ID,
                                x => string.Join(" - ", new[] {
                                    $"[Mã sản phẩm: {x.ProductCode}] - Tên sản phẩm: {x.ProductName}".Trim(),
                                    string.IsNullOrWhiteSpace(x.ProductCodeRTC) ? null : $"Mã nội bộ: {x.ProductCodeRTC}",
                                    string.IsNullOrWhiteSpace(x.UnitName) ? null : $"ĐVT: {x.UnitName}",
                                    string.IsNullOrWhiteSpace(x.Maker) ? null : $"Hãng: {x.Maker}"
                                }.Where(s => !string.IsNullOrWhiteSpace(s)))
                            )
                        : new Dictionary<int, string>();
                    //var employees = employeeIds.Count > 0
                    //    ? db.Employees.Where(x => employeeIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.FullName ?? "")
                    //    : new Dictionary<int, string>();
                    var employees = employeeIds.Count > 0
                       ? db.Employees.Where(x => x.UserID.HasValue && employeeIds.Contains(x.UserID.Value))
                                     .ToList()
                                     .GroupBy(x => x.UserID!.Value)
                                     .ToDictionary(g => g.Key, g => g.First().FullName ?? "")
                       : new Dictionary<int, string>();
                    var approvers = approverIds.Count > 0
                       ? db.Employees.Where(x => approverIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.FullName ?? "")
                       : new Dictionary<int, string>();
                    var suppliers = supplierIds.Count > 0
                        ? db.SupplierSales.Where(x => supplierIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.NameNCC ?? "")
                        : new Dictionary<int, string>();
                    var warehouses = warehouseIds.Count > 0
                        ? db.Warehouses.Where(x => warehouseIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.WarehouseName ?? "")
                        : new Dictionary<int, string>();
                    var projects = projectIds.Count > 0
                        ? db.Projects.Where(x => projectIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.ProjectName ?? "")
                        : new Dictionary<int, string>();
                    var rulePays = rulePayIds.Count > 0
                        ? db.RulePays.Where(x => rulePayIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.Note ?? "")
                        : new Dictionary<int, string>();
                    var customers = customerIds.Count > 0
                      ? db.Customers.Where(x => customerIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.CustomerName ?? "")
                      : new Dictionary<int, string>();

                    foreach (var (name, oldVal, newVal) in fkChanges)
                    {
                        var label = _propertyLabels.GetValueOrDefault(name, name);
                        var oldText = ResolveFkDisplayValue(name, oldVal, products, employees, suppliers, warehouses, projects, rulePays, approvers, customers);
                        var newText = ResolveFkDisplayValue(name, newVal, products, employees, suppliers, warehouses, projects, rulePays, approvers, customers);

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
            HashSet<int> productIds, HashSet<int> employeeIds, HashSet<int> supplierIds,
            HashSet<int> warehouseIds, HashSet<int> projectIds, HashSet<int> rulePayIds, HashSet<int> approverIds, HashSet<int> customerIds)
        {
            if (value == null || !int.TryParse(value.ToString(), out int id) || id <= 0) return;

            switch (propertyName)
            {
                case "ProductID": productIds.Add(id); break;
                case "DeliverID": case "ReceiverID": employeeIds.Add(id); break;
                case "ApproverID": approverIds.Add(id); break;
                case "SupplierSaleID": supplierIds.Add(id); break;
                case "WarehouseID": warehouseIds.Add(id); break;
                case "ProjectID": projectIds.Add(id); break;
                case "RulePayID": rulePayIds.Add(id); break;
                case "CustomerID": customerIds.Add(id); break;
            }
        }

        #endregion Phân loại ID vào đúng HashSet theo tên property (để batch query)

        #region Resolve FK ID thành tên hiển thị từ dictionary đã batch-load

        private static string ResolveFkDisplayValue(string propertyName, object? value,
            Dictionary<int, string> products, Dictionary<int, string> employees,
            Dictionary<int, string> suppliers, Dictionary<int, string> warehouses,
            Dictionary<int, string> projects, Dictionary<int, string> rulePays,
            Dictionary<int, string> approvers, Dictionary<int, string> customers)
        {
            if (value == null) return "Trống";
            var text = value.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(text)) return "Trống";
            if (!int.TryParse(text, out int id) || id <= 0) return text;

            return propertyName switch
            {
                "ProductID" => products.GetValueOrDefault(id, text),
                "DeliverID" or "ReciverID" => employees.GetValueOrDefault(id, text),
                "SupplierSaleID" => suppliers.GetValueOrDefault(id, text),
                "ApproverID" => approvers.GetValueOrDefault(id, text),
                "WarehouseID" => warehouses.GetValueOrDefault(id, text),
                "ProjectID" => projects.GetValueOrDefault(id, text),
                "RulePayID" => rulePays.GetValueOrDefault(id, text),
                "CustomerID" => customers.GetValueOrDefault(id, text),
                _ => text
            };
        }

        #endregion Resolve FK ID thành tên hiển thị từ dictionary đã batch-load

        #region Map BillTypeNew (int) → tên loại phiếu tiếng Việt

        private static string BillTypeText(object? value)
        {
            if (value == null || !int.TryParse(value.ToString(), out int v)) return "Trống";
            return v switch
            {
                0 => "Chọn loại",
                1 => "Mượn NCC",
                2 => "Mua NCC",
                3 => "Trả",
                4 => "Nhập nội bộ",
                5 => "Y/c nhập kho",
                6 => "Nhập hàng bảo hành",
                7 => "NCC tặng/cho",
                _ => v.ToString()
            };
        }

        #endregion Map BillTypeNew (int) → tên loại phiếu tiếng Việt
    }
}