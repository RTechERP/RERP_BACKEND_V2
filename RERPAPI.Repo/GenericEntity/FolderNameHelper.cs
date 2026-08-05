using System.Globalization;
using System.Text;

namespace RERPAPI.Repo.GenericEntity
{
    /// <summary>
    /// Helper chuẩn hoá chuỗi tiếng Việt/có dấu thành tên folder PascalCase không dấu, không khoảng trắng.
    /// Dùng cho các folder theo pattern THIETKE.{TenFolder} trong module Drawing.
    /// </summary>
    public static class FolderNameHelper
    {
        /// <summary>
        /// Chuyển chuỗi đầu vào (vd: "Thiết kế điện") thành tên folder PascalCase không dấu, viết liền.
        /// Quy tắc:
        ///   1. Bỏ dấu tiếng Việt.
        ///   2. Thay tất cả ký tự KHÔNG phải chữ/số bằng khoảng trắng (dấu phân cách từ).
        ///   3. Tách theo khoảng trắng, bỏ rỗng.
        ///   4. PascalCase mỗi từ (chữ đầu viết hoa, các chữ còn lại viết thường).
        ///   5. Nối liền không khoảng trắng.
        ///   6. Nếu rỗng / null thì trả về "Khac".
        /// </summary>
        public static string ToPascalCaseNoDiacritics(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "Khac";

            // 1) Bỏ dấu
            var normalized = value.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(normalized.Length);
            foreach (var ch in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }
            var noDiacritics = sb.ToString().Normalize(NormalizationForm.FormC);

            // 2) Thay mọi ký tự không phải chữ/số thành space
            var buffer = new StringBuilder(noDiacritics.Length);
            foreach (var ch in noDiacritics)
            {
                if (char.IsLetterOrDigit(ch))
                    buffer.Append(ch);
                else
                    buffer.Append(' ');
            }

            // 3) PascalCase từng từ, nối liền
            var words = buffer.ToString()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var result = new StringBuilder();
            foreach (var w in words)
            {
                if (string.IsNullOrWhiteSpace(w)) continue;
                // Bỏ ký tự không phải chữ/số lọt sót (giữ an toàn)
                var cleaned = new string(w.Where(char.IsLetterOrDigit).ToArray());
                if (string.IsNullOrEmpty(cleaned)) continue;

                result.Append(char.ToUpper(cleaned[0]));
                if (cleaned.Length > 1)
                    result.Append(cleaned.Substring(1).ToLower());
            }

            return result.Length == 0 ? "Khac" : result.ToString();
        }
    }
}
