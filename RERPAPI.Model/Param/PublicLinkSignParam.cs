namespace RERPAPI.Model.Param
{
    /// <summary>Body của POST api/PublicLink/sign — yêu cầu tạo link xem công khai.</summary>
    public class PublicLinkSignParam
    {
        /// <summary>Route/alias trang, ví dụ "issuelog".</summary>
        public string Route { get; set; } = "";

        /// <summary>Bộ lọc đã giải xong, ví dụ { "projectId": "245" }.</summary>
        public Dictionary<string, string> Filters { get; set; } = new();

        /// <summary>Số ngày hiệu lực. Bỏ trống thì lấy PublicLinkSettings:ExpireDays.</summary>
        public int? ExpireDays { get; set; }
    }
}
