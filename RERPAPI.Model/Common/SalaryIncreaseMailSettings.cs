namespace RERPAPI.Model.Common
{
    /// <summary>Danh sách email cố định được Cc khi gửi mail quyết định điều chỉnh lương. Giá trị thật cấu hình trong appsettings.json.</summary>
    public class SalaryIncreaseMailSettings
    {
        public string BGDEmail { get; set; } = string.Empty;
        public string HRMEmail { get; set; } = string.Empty;
        public string KTTEmail { get; set; } = string.Empty;

        /// <summary>Giai đoạn test: nếu có giá trị, mọi mail quyết định lương gửi về đây thay vì email thật của nhân viên. Xóa giá trị này khi go-live.</summary>
        public string TestRecipientEmail { get; set; } = string.Empty;
    }
}
