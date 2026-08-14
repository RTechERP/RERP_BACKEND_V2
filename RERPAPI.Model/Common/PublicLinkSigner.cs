using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using RERPAPI.Model.DTO;

namespace RERPAPI.Model.Common
{
    /// <summary>
    /// Ký và xác thực token cho link xem công khai.
    ///
    /// Mục đích: endpoint đọc dữ liệu ẩn danh KHÔNG được nhận thẳng ID từ URL,
    /// vì như vậy ai cũng lặp ID để lấy sạch dữ liệu. Thay vào đó URL mang một
    /// token đã ký bằng secret của server — không có secret thì không tự tạo
    /// được token hợp lệ.
    ///
    /// Token là stateless (không cần bảng trong DB). Đổi lại: không thu hồi được
    /// từng link riêng lẻ; muốn vô hiệu hoá toàn bộ link đã phát thì đổi secret.
    /// </summary>
    public static class PublicLinkSigner
    {
        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static string Sign(PublicLinkPayload payload, string secret)
        {
            if (string.IsNullOrWhiteSpace(secret))
                throw new InvalidOperationException("Chưa cấu hình PublicLinkSettings:SecretKey.");

            string json = JsonSerializer.Serialize(payload, JsonOpts);
            string body = ToBase64Url(Encoding.UTF8.GetBytes(json));

            return body + "." + ToBase64Url(ComputeHash(body, secret));
        }

        /// <summary>
        /// Xác thực chữ ký và hạn dùng. Trả về false cho mọi trường hợp token
        /// sai định dạng, sai chữ ký hoặc đã hết hạn.
        /// </summary>
        public static bool TryVerify(string token, string secret, out PublicLinkPayload? payload)
        {
            payload = null;

            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(secret))
                return false;

            string[] parts = token.Split('.');
            if (parts.Length != 2) return false;

            byte[] expected;
            byte[] actual;
            try
            {
                expected = ComputeHash(parts[0], secret);
                actual = FromBase64Url(parts[1]);
            }
            catch
            {
                return false;
            }

            // So sánh thời gian cố định để không lộ thông tin qua thời gian phản hồi.
            if (!CryptographicOperations.FixedTimeEquals(expected, actual)) return false;

            try
            {
                string json = Encoding.UTF8.GetString(FromBase64Url(parts[0]));
                payload = JsonSerializer.Deserialize<PublicLinkPayload>(json, JsonOpts);
            }
            catch
            {
                payload = null;
                return false;
            }

            if (payload == null) return false;

            if (payload.Exp.HasValue && DateTimeOffset.UtcNow.ToUnixTimeSeconds() > payload.Exp.Value)
            {
                payload = null;
                return false;
            }

            return true;
        }

        private static byte[] ComputeHash(string body, string secret)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            return hmac.ComputeHash(Encoding.UTF8.GetBytes(body));
        }

        private static string ToBase64Url(byte[] bytes)
            => Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');

        private static byte[] FromBase64Url(string text)
        {
            string b64 = text.Replace('-', '+').Replace('_', '/');
            switch (b64.Length % 4)
            {
                case 2: b64 += "=="; break;
                case 3: b64 += "="; break;
            }
            return Convert.FromBase64String(b64);
        }
    }
}
