using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class DrawingLogRepo : GenericRepo<DrawingLog>
    {
        public DrawingLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        private static readonly Dictionary<string, string> _map = new()
        {
            { "ID", "ID bản vẽ" },
            { "DrawingName", "tên bản vẽ" },
            { "Version", "phiên bản" },
            { "ServerPath", "đường dẫn file" },
            { "ProjectID", "dự án" },
            { "ProjectTypeID", "loại dự án" },

            { "DesignByID", "người thiết kế" },
            { "DesignDate", "ngày thiết kế" },

            { "CheckedByID", "người check" },
            { "CheckedDate", "ngày check" },

            { "ApprovedByID", "người duyệt" },
            { "ApprovedDate", "ngày duyệt" },

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

        public string GenerateLog(Drawing oldObj, Drawing newObj)
        {
            if (oldObj == null || newObj == null) return string.Empty;

            var changes = new List<string>();
            var props = typeof(Drawing).GetProperties();

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

        private static string FormatValue(string fieldName, object? value)
        {
            if (value == null) return "trống";

            if (value is DateTime dt)
                return dt.ToString("dd/MM/yyyy HH:mm");

            if (value is bool b)
                return b ? "Có" : "Không";

            if (value is decimal dec)
                return dec.ToString("N0");

            if (fieldName == "IsDeleted")
                return FormatDeleted(value);

            return value.ToString() ?? "";
        }

        private static string FormatDeleted(object value)
        {
            if (value == null) return "Chưa xóa";

            bool? deleted = null;

            if (value is bool b)
                deleted = b;
            else if (bool.TryParse(value.ToString(), out bool result))
                deleted = result;

            if (deleted == true) return "Đã xóa";
            if (deleted == false) return "Chưa xóa";

            return "Không xác định";
        }

        public async Task AddLog(int drawingID, string logContent, string typeLog)
        {
            if (drawingID <= 0) return;

            if (string.IsNullOrWhiteSpace(logContent)) return;

            DrawingLog log = new DrawingLog
            {
                DrawingID = drawingID,
                TypeLog = typeLog,
                ContentLog = logContent,
                IsDeleted = false
            };

            await CreateAsync(log);
        }
    }
}