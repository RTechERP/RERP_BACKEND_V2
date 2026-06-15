using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectHistoryProblemLogRepo : GenericRepo<ProjectHistoryProblemLog>
    {
        public ProjectHistoryProblemLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        #region Log chính ProjectHistoryProblem

        private static readonly Dictionary<string, string> _map = new()
        {
            { "ProjectID", "dự án" },
            { "STT", "số thứ tự" },
            { "TypeProblem", "loại vấn đề" },
            { "ContentError", "nội dung lỗi" },
            { "Reason", "nguyên nhân" },
            { "Remedies", "biện pháp khắc phục" },
            { "TestMethod", "phương pháp kiểm tra" },
            { "Image", "hình ảnh" },
            { "DateProblem", "ngày phát sinh" },
            { "DateImplementation", "ngày thực hiện" },
            { "PIC", "PIC" },
            { "EmployeeID", "nhân viên tạo" },
            { "IssueLogType", "loại phát sinh" },
            { "CreatorID", "người tạo" },
            { "PriorityLevel", "mức độ ưu tiên" },
            { "PerformerID", "người thực hiện" },
            { "StatusProblem", "trạng thái xử lý" },
            { "IssueConclusion", "kết luận phát sinh" },
            { "IsApproved_PM", "duyệt PM" },
            { "DateApproved_PM", "ngày duyệt PM" },
            { "IsApproved_TP", "duyệt TP" },
            { "DateApproved_TP", "ngày duyệt TP" },
            { "IsApproved_PP", "duyệt PP" },
            { "DateApproved_PP", "ngày duyệt PP" },
            { "IsDeleted", "trạng thái xóa" }
        };

        public static string GetDisplayName(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                return string.Empty;

            return _map.TryGetValue(fieldName, out var value)
                ? value
                : fieldName;
        }

        public string GenerateLog(ProjectHistoryProblem oldObj, ProjectHistoryProblem newObj)
        {
            if (oldObj == null || newObj == null) return string.Empty;

            var changes = new List<string>();
            var props = typeof(ProjectHistoryProblem).GetProperties();

            var ignoreFields = new HashSet<string>
            {
                "CreatedDate",
                "CreatedBy",
                "UpdatedDate",
                "UpdatedBy"
            };

            foreach (var prop in props)
            {
                if (ignoreFields.Contains(prop.Name)) continue;

                var oldVal = prop.GetValue(oldObj);
                var newVal = prop.GetValue(newObj);

                if (Equals(oldVal, newVal)) continue;

                string fieldName = GetDisplayName(prop.Name);

                string oldStr = FormatValue(prop.Name, oldVal);
                string newStr = FormatValue(prop.Name, newVal);

                changes.Add($"- thay đổi {fieldName} từ {oldStr} thành {newStr}.");
            }

            return string.Join("\n", changes);
        }

        #endregion Log chính ProjectHistoryProblem

        #region Log chi tiết ProjectHistoryProblemDetail

        private static readonly Dictionary<string, string> _mapDetail = new()
        {
            { "ProjectHistoryProblemID", "phát sinh dự án" },
            { "STT", "số thứ tự" },
            { "Description", "mô tả" },
            { "Status", "trạng thái" },
            { "Note", "ghi chú" },
            { "IsDeleted", "trạng thái xóa" }
        };

        public static string GetDisplayNameDetail(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                return string.Empty;

            return _mapDetail.TryGetValue(fieldName, out var value)
                ? value
                : fieldName;
        }

        public string GenerateLogDetail(ProjectHistoryProblemDetail oldObj, ProjectHistoryProblemDetail newObj)
        {
            if (oldObj == null || newObj == null) return string.Empty;

            var changes = new List<string>();
            var props = typeof(ProjectHistoryProblemDetail).GetProperties();

            var ignoreFields = new HashSet<string>
            {
                "CreatedDate",
                "CreatedBy",
                "UpdatedDate",
                "UpdatedBy"
            };

            foreach (var prop in props)
            {
                if (ignoreFields.Contains(prop.Name)) continue;

                var oldVal = prop.GetValue(oldObj);
                var newVal = prop.GetValue(newObj);

                if (Equals(oldVal, newVal)) continue;

                string fieldName = GetDisplayNameDetail(prop.Name);

                string oldStr = FormatValue(prop.Name, oldVal);
                string newStr = FormatValue(prop.Name, newVal);

                changes.Add($"- thay đổi {fieldName} từ {oldStr} thành {newStr}.");
            }

            return string.Join("\n", changes);
        }

        #endregion Log chi tiết ProjectHistoryProblemDetail

        #region Log danh sách link

        public string GenerateListLog(string fieldDisplayName, List<int> oldIds, List<int> newIds)
        {
            oldIds = oldIds != null ? oldIds.Distinct().OrderBy(x => x).ToList() : new List<int>();
            newIds = newIds != null ? newIds.Distinct().OrderBy(x => x).ToList() : new List<int>();

            if (oldIds.SequenceEqual(newIds))
                return string.Empty;

            string oldText = oldIds.Count > 0 ? string.Join(", ", oldIds) : "trống";
            string newText = newIds.Count > 0 ? string.Join(", ", newIds) : "trống";

            return $"- thay đổi {fieldDisplayName} từ {oldText} thành {newText}.";
        }

        #endregion Log danh sách link

        #region Khác

        private static string FormatValue(string fieldName, object? value)
        {
            if (value == null) return "null";

            if (value is DateTime dt)
                return dt.ToString("dd/MM/yyyy HH:mm");

            if (value is bool b)
                return b ? "Có" : "Không";

            if (value is decimal dec)
                return dec.ToString("N0");

            if (fieldName == "IssueLogType")
                return FormatIssueLogType(value);

            if (fieldName == "PriorityLevel")
                return FormatPriorityLevel(value);

            if (fieldName == "StatusProblem")
                return FormatStatusProblem(value);

            if (fieldName == "IsApproved_PM" ||
                fieldName == "IsApproved_TP" ||
                fieldName == "IsApproved_PP")
                return FormatApproved(value);

            return value.ToString() ?? "";
        }

        private static string FormatIssueLogType(object value)
        {
            int? type = ConvertToNullableInt(value);

            return type switch
            {
                1 => "Khách hàng",
                2 => "Nội bộ",
                _ => "Không xác định"
            };
        }

        private static string FormatPriorityLevel(object value)
        {
            int? priority = ConvertToNullableInt(value);

            return priority switch
            {
                1 => "Thấp",
                2 => "Trung bình",
                3 => "Cao",
                4 => "Rất cao",
                _ => "Không xác định"
            };
        }

        private static string FormatStatusProblem(object value)
        {
            int? status = ConvertToNullableInt(value);

            return status switch
            {
                1 => "Chờ xử lý",
                2 => "Đang xử lý",
                3 => "Đã xử lý",
                _ => "Không xác định"
            };
        }

        private static string FormatApproved(object value)
        {
            if (value == null) return "Chưa duyệt";

            bool? approved = value as bool?;

            if (approved == true) return "Đã duyệt";
            if (approved == false) return "Chưa duyệt";

            return "Không xác định";
        }

        private static int? ConvertToNullableInt(object value)
        {
            if (value == null) return null;

            if (value is int i) return i;

            if (int.TryParse(value.ToString(), out int result))
                return result;

            return null;
        }

        public async Task AddLog(int projectHistoryProblemID, string logContent, string typeLog)
        {
            if (projectHistoryProblemID <= 0) return;

            if (string.IsNullOrWhiteSpace(logContent)) return;

            ProjectHistoryProblemLog log = new ProjectHistoryProblemLog
            {
                ProjectHistoryProblemID = projectHistoryProblemID,
                TypeLog = typeLog,
                ContentLog = logContent
            };

            await CreateAsync(log);
        }

        #endregion Khác
    }
}