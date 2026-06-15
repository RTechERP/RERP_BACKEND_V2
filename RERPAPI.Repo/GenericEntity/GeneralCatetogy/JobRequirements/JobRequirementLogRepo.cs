using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System.Reflection;

namespace RERPAPI.Repo.GenericEntity.GeneralCatetogy.JobRequirements
{
    public class JobRequirementLogRepo : GenericRepo<JobRequirementLog>
    {
        public JobRequirementLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        #region Các trường hệ thống/nội bộ không cần ghi log thay đổi

        private static readonly HashSet<string> _ignoredProperties = new(StringComparer.OrdinalIgnoreCase)
        {
            "ID", "CreatedDate", "UpdatedDate", "CreatedBy", "UpdatedBy", "IsDeleted"
        };

        #endregion Các trường hệ thống/nội bộ không cần ghi log thay đổi

        #region Tên hiển thị tiếng Việt cho các property

        private static readonly Dictionary<string, string> _propertyLabels = new()
        {
            ["NumberRequest"] = "Mã yêu cầu",
            ["DateRequest"] = "Ngày yêu cầu",
            ["DeadlineRequest"] = "Hạn hoàn thành",
            ["EmployeeID"] = "Người yêu cầu",
            ["CoordinationDepartmentID"] = "Phòng ban phối hợp",
            ["RequiredDepartmentID"] = "Phòng ban yêu cầu",
            ["IsApprovedTBP"] = "TBP Duyệt",
            ["DateApprovedTBP"] = "Ngày TBP duyệt",
            ["ApprovedTBPID"] = "Người duyệt TBP",
            ["IsApprovedHR"] = "HCNS Duyệt",
            ["DateApprovedHR"] = "Ngày HCNS duyệt",
            ["ApprovedHRID"] = "Người duyệt HCNS",
            ["IsApprovedBGD"] = "BGĐ Duyệt",
            ["DateApprovedBGD"] = "Ngày BGĐ duyệt",
            ["ApprovedBGDID"] = "Người duyệt BGĐ",
            ["EvaluateCompletion"] = "Đánh giá mức độ hoàn thành",
            ["IsRequestBuy"] = "Yêu cầu mua sắm",
            ["Status"] = "Trạng thái",
            ["Note"] = "Ghi chú",
            ["IsRequestBGDApproved"] = "YC BGĐ duyệt",
            ["IsRequestPriceQuote"] = "YC báo giá"
        };

        #endregion Tên hiển thị tiếng Việt cho các property

        #region So sánh 2 entity cùng loại, trả về danh sách mô tả thay đổi

        public List<string> GetEntityChanges<T>(T oldObj, T newObj)
        {
            var changes = new List<string>();
            if (oldObj == null || newObj == null) return changes;

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

                if (propType == typeof(string))
                {
                    var oldStr = (oldVal as string ?? "").Trim();
                    var newStr = (newVal as string ?? "").Trim();
                    if (oldStr == newStr) continue;

                    changes.Add($"{label}: '{oldStr}' → '{newStr}'");
                    continue;
                }

                if (propType == typeof(DateTime))
                {
                    var oldDate = oldVal as DateTime?;
                    var newDate = newVal as DateTime?;
                    if (oldDate?.Date == newDate?.Date) continue;

                    changes.Add($"{label}: {oldDate?.ToString("dd/MM/yyyy") ?? "Trống"} → {newDate?.ToString("dd/MM/yyyy") ?? "Trống"}");
                    continue;
                }

                if (propType == typeof(bool))
                {
                    if (Equals(oldVal, newVal)) continue;
                    changes.Add($"{label}: {((oldVal as bool?) == true ? "Có" : "Không")} → {((newVal as bool?) == true ? "Có" : "Không")}");
                    continue;
                }

                if (prop.Name == "Status")
                {
                    if (Equals(oldVal, newVal)) continue;
                    changes.Add($"{label}: {GetStatusText(oldVal)} → {GetStatusText(newVal)}");
                    continue;
                }

                if (Equals(oldVal, newVal)) continue;
                fkChanges.Add((prop.Name, oldVal, newVal));
            }

            if (fkChanges.Count > 0)
            {
                try
                {
                    using var db = new RTCContext();

                    var employeeIds = new HashSet<int>();
                    var departmentIds = new HashSet<int>();

                    foreach (var (name, oldVal, newVal) in fkChanges)
                    {
                        CollectFkId(name, oldVal, employeeIds, departmentIds);
                        CollectFkId(name, newVal, employeeIds, departmentIds);
                    }

                    var employees = employeeIds.Count > 0
                        ? db.Employees.Where(x => employeeIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.FullName ?? "")
                        : new Dictionary<int, string>();
                    var departments = departmentIds.Count > 0
                        ? db.Departments.Where(x => departmentIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.Name ?? "")
                        : new Dictionary<int, string>();

                    foreach (var (name, oldVal, newVal) in fkChanges)
                    {
                        var label = _propertyLabels.GetValueOrDefault(name, name);
                        var oldText = ResolveFkDisplayValue(name, oldVal, employees, departments);
                        var newText = ResolveFkDisplayValue(name, newVal, employees, departments);

                        if (oldText != newText)
                            changes.Add($"{label}: {oldText} → {newText}");
                    }
                }
                catch
                {
                    foreach (var (name, oldVal, newVal) in fkChanges)
                    {
                        var label = _propertyLabels.GetValueOrDefault(name, name);
                        changes.Add($"{label}: {oldVal ?? "Trống"} → {newVal ?? "Trống"}");
                    }
                }
            }

            return changes;
        }

        #endregion So sánh 2 entity cùng loại, trả về danh sách mô tả thay đổi

        private static void CollectFkId(string propertyName, object? value, HashSet<int> employeeIds, HashSet<int> departmentIds)
        {
            if (value == null || !int.TryParse(value.ToString(), out int id) || id <= 0) return;

            switch (propertyName)
            {
                case "EmployeeID": case "ApprovedTBPID": case "ApprovedHRID": case "ApprovedBGDID": employeeIds.Add(id); break;
                case "CoordinationDepartmentID": case "RequiredDepartmentID": departmentIds.Add(id); break;
            }
        }

        private static string ResolveFkDisplayValue(string propertyName, object? value, Dictionary<int, string> employees, Dictionary<int, string> departments)
        {
            if (value == null) return "Trống";
            var text = value.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(text)) return "Trống";
            if (!int.TryParse(text, out int id) || id <= 0) return text;

            return propertyName switch
            {
                "EmployeeID" or "ApprovedTBPID" or "ApprovedHRID" or "ApprovedBGDID" => employees.GetValueOrDefault(id, text),
                "CoordinationDepartmentID" or "RequiredDepartmentID" => departments.GetValueOrDefault(id, text),
                _ => text
            };
        }

        private static string GetStatusText(object? value)
        {
            if (value == null || !int.TryParse(value.ToString(), out int v)) return "Trống";
            return v switch
            {
                1 => "Đang thực hiện",
                2 => "Đã hoàn thành",
                3 => "Hủy",
                _ => v.ToString()
            };
        }
    }
}