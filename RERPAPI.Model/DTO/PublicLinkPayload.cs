namespace RERPAPI.Model.DTO
{
    /// <summary>
    /// Nội dung nằm bên trong token của link xem công khai (không cần đăng nhập).
    /// Token = base64url(JSON của lớp này) + "." + base64url(HMACSHA256).
    /// </summary>
    public class PublicLinkPayload
    {
        /// <summary>Route/alias của trang, khớp với deep-link.config.ts bên FE. Ví dụ "issuelog".</summary>
        public string Route { get; set; } = "";

        /// <summary>
        /// Bộ lọc đã giải xong, ví dụ { "projectId": "245" }.
        /// Dùng ID chứ không dùng mã để endpoint ẩn danh khỏi phải tra cứu thêm.
        /// </summary>
        public Dictionary<string, string> Filters { get; set; } = new();

        /// <summary>Thời điểm hết hạn, unix seconds. null = không hết hạn.</summary>
        public long? Exp { get; set; }
    }
}
