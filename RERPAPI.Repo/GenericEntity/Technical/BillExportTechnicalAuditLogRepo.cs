using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System.Reflection;

namespace RERPAPI.Repo.GenericEntity.Technical
{
    public class BillExportTechnicalAuditLogRepo : GenericRepo<BillExportTechnicalAuditLog>
    {
        public BillExportTechnicalAuditLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        #region Các trường hệ thống/nội bộ không cần ghi log thay đổi

        private static readonly HashSet<string> _ignoredProperties = new(StringComparer.OrdinalIgnoreCase)
        {
            // Audit & system fields
            "ID", "BillImportID", "ExpectedDate", "UpdatedDate",
            "CreatedBy", "UpdatedBy", "IsDeleted", "STT", "UnitID",
            // Internal tracking fields
            "CustomerID", "Image",
            "WarehouseID", "SupplierSaleID", "BillDocumentExportType",
            "WarehouseTypeBill", "BillExportTechID", "TotalQuantity","internalcode",
            "HistoryProductRTCID", "ProductRTCQRCodeID", "BillImportDetailTechnicalID","Deliver"
        };

        #endregion Các trường hệ thống/nội bộ không cần ghi log thay đổi

        #region Tên hiển thị tiếng Việt cho các property (thay vì tên cột DB)

        private static readonly Dictionary<string, string> _propertyLabels = new()
        {
            // ── BillExport ──
            ["Code"] = "Mã phiếu",
            ["SupplierSaleID"] = "Nhà cung cấp",
            ["DeliverID"] = "Liên hệ",
            ["BillType"] = "Loại",
            ["CreatedDate"] = "Ngày xuất",
            ["ProjectName"] = "Dự án",
            ["ReceiverID"] = "Người nhận",
            ["Receiver"] = "Tên người nhận",
            ["CustomerID"] = "Khách hàng",
            ["ApproverID"] = "Người duyệt",

            // ── BillExportDetail ──
            ["ProductID"] = "Sản phẩm",
            ["Quantity"] = "Số lượng xuất",
            ["ProjectID"] = "Dự án",
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

                // Bỏ qua CreatedDate nếu đây là entity chi tiết (chỉ theo dõi thay đổi CreatedDate ở master)
                if (prop.Name == "CreatedDate" && typeof(T).Name.Contains("Detail")) continue;

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
                if (prop.Name == "BillType")
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

                    foreach (var (name, oldVal, newVal) in fkChanges)
                    {
                        CollectFkId(name, oldVal, productIds, employeeIds, supplierIds, warehouseIds, projectIds, rulePayIds, approverIds);
                        CollectFkId(name, newVal, productIds, employeeIds, supplierIds, warehouseIds, projectIds, rulePayIds, approverIds);
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
                        ? db.Suppliers.Where(x => supplierIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.SupplierName ?? "")
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

                    foreach (var (name, oldVal, newVal) in fkChanges)
                    {
                        var label = _propertyLabels.GetValueOrDefault(name, name);
                        var oldText = ResolveFkDisplayValue(name, oldVal, products, employees, suppliers, warehouses, projects, rulePays, approvers);
                        var newText = ResolveFkDisplayValue(name, newVal, products, employees, suppliers, warehouses, projects, rulePays, approvers);

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
            HashSet<int> warehouseIds, HashSet<int> projectIds, HashSet<int> rulePayIds, HashSet<int> approverIds)
        {
            if (value == null || !int.TryParse(value.ToString(), out int id) || id <= 0) return;

            switch (propertyName)
            {
                case "ProductID": productIds.Add(id); break;
                case "DeliverID": case "ReceiverID": employeeIds.Add(id); break;
                case "ApproverID": approverIds.Add(id); break;
                case "SupplierID": supplierIds.Add(id); break;
                case "WarehouseID": warehouseIds.Add(id); break;
                case "ProjectID": projectIds.Add(id); break;
                case "RulePayID": rulePayIds.Add(id); break;
            }
        }

        #endregion Phân loại ID vào đúng HashSet theo tên property (để batch query)

        #region Resolve FK ID thành tên hiển thị từ dictionary đã batch-load

        private static string ResolveFkDisplayValue(string propertyName, object? value,
            Dictionary<int, string> products, Dictionary<int, string> employees,
            Dictionary<int, string> suppliers, Dictionary<int, string> warehouses,
            Dictionary<int, string> projects, Dictionary<int, string> rulePays,
            Dictionary<int, string> approvers)
        {
            if (value == null) return "Trống";
            var text = value.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(text)) return "Trống";
            if (!int.TryParse(text, out int id) || id <= 0) return text;

            return propertyName switch
            {
                "ProductID" => products.GetValueOrDefault(id, text),
                "DeliverID" or "ReceiverID" => employees.GetValueOrDefault(id, text),
                "SupplierID" => suppliers.GetValueOrDefault(id, text),
                "ApproverID" => approvers.GetValueOrDefault(id, text),
                "WarehouseID" => warehouses.GetValueOrDefault(id, text),
                "ProjectID" => projects.GetValueOrDefault(id, text),
                "RulePayID" => rulePays.GetValueOrDefault(id, text),
                _ => text
            };
        }

        #endregion Resolve FK ID thành tên hiển thị từ dictionary đã batch-load

        #region Map BillType (int) → tên loại phiếu tiếng Việt

        private static string BillTypeText(object? value)
        {
            if (value == null || !int.TryParse(value.ToString(), out int v)) return "Trống";
            return v switch
            {
                0 => "Trả",
                1 => "Cho mượn",
                2 => "Tặng/Bán",
                3 => "Mất",
                4 => "Bảo hành",
                5 => "Xuất dự án",
                6 => "Hỏng",
                7 => "Xuất kho",
                _ => v.ToString()
            };
        }

        #endregion Map BillType (int) → tên loại phiếu tiếng Việt
    }
}