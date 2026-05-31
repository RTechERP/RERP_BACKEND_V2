using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace RERPAPI.Repo.GenericEntity.Asset
{
    public class AssetAllocationLogRepo: GenericRepo<AssetAllocationLog>
    {
        public AssetAllocationLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        #region Các trường hệ thống/nội bộ không cần ghi log thay đổi

        private static readonly HashSet<string> _ignoredProperties = new(StringComparer.OrdinalIgnoreCase)
        {
            // Audit & system fields
            "ID", "CreatedDate", "UpdatedDate",
            "CreatedBy", "UpdatedBy", "IsDeleted", "STT", "StatusID",
            // Internal tracking fields
            "Code", 

        };
        #endregion

        #region  Tên hiển thị tiếng Việt cho các property (thay vì tên cột DB)

        private static readonly Dictionary<string, string> _propertyLabels = new()
        {
            // ── TSAssetAllocation ──
            ["DateAllocation"] = "Ngày cấp phát",
            ["EmployeeID"] = "Nhân viên",
        };
        #endregion

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
                //nhan 
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
                    var sourceIds = new HashSet<int>();
                    var employeeIds = new HashSet<int>();
                    var unitIds = new HashSet<int>();
                    var assetIds = new HashSet<int>();

                    foreach (var (name, oldVal, newVal) in fkChanges)
                    {
                        CollectFkId(name, oldVal, sourceIds, employeeIds, unitIds, assetIds);
                        CollectFkId(name, newVal, sourceIds, employeeIds, unitIds, assetIds);
                    }

                    // Batch load 1 query mỗi bảng (thay vì N+1)
                    var sourceAsset = sourceIds.Count > 0
                        ? db.TSSourceAssets.Where(x => sourceIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.SourceName ?? "")
                        : new Dictionary<int, string>();
                    var employees = employeeIds.Count > 0
                        ? db.Employees.Where(x => employeeIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.FullName ?? "")
                        : new Dictionary<int, string>();
                    var units = unitIds.Count > 0
                        ? db.UnitCounts.Where(x => unitIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.UnitName ?? "")
                        : new Dictionary<int, string>();
                    var assets = assetIds.Count > 0
                        ? db.TSAssets.Where(x => assetIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.AssetType ?? "")
                        : new Dictionary<int, string>();

                    foreach (var (name, oldVal, newVal) in fkChanges)
                    {
                        var label = _propertyLabels.GetValueOrDefault(name, name);
                        var oldText = ResolveFkDisplayValue(name, oldVal, sourceAsset, employees, units, assets);
                        var newText = ResolveFkDisplayValue(name, newVal, sourceAsset, employees, units, assets);

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
        #endregion

        #region Phân loại ID vào đúng HashSet theo tên property (để batch query)

        private static void CollectFkId(string propertyName, object? value,
            HashSet<int> sourceIds, HashSet<int> userIds, HashSet<int> unitIds,
            HashSet<int> assetIds)
        {
            if (value == null || !int.TryParse(value.ToString(), out int id) || id <= 0) return;

            switch (propertyName)
            {
                case "SourceID": sourceIds.Add(id); break;
                case "EmployeeID": userIds.Add(id); break;
                case "UnitID": unitIds.Add(id); break;
                case "TSAssetID": assetIds.Add(id); break;
            }
        }
        #endregion

        #region Resolve FK ID thành tên hiển thị từ dictionary đã batch-load

        private static string ResolveFkDisplayValue(string propertyName, object? value,
            Dictionary<int, string> sourceIds, Dictionary<int, string> employees,
            Dictionary<int, string> units, Dictionary<int, string> assets)
        {
            if (value == null) return "Trống";
            var text = value.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(text)) return "Trống";
            if (!int.TryParse(text, out int id) || id <= 0) return text;

            return propertyName switch
            {
                "SourceID" => sourceIds.GetValueOrDefault(id, text),
                "EmployeeID" => employees.GetValueOrDefault(id, text),
                "UnitID" => units.GetValueOrDefault(id, text),
                "TSAssetID" => assets.GetValueOrDefault(id, text),
                _ => text
            };
        }
        #endregion

    }
}

