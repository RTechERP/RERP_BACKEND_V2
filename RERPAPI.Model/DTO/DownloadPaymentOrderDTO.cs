namespace RERPAPI.Model.DTO
{
    public class DownloadPaymentOrderDTO
    {
        public int PaymentOrderId { get; set; }
        public string PaymentOrderCode { get; set; }
        public List<string> FilePath { get; set; }
    }
    public class DownloadPaymentOrderFileDTO
    {
        public int PaymentOrderId { get; set; }

        public string PaymentOrderCode { get; set; } = string.Empty;

        /// <summary>
        /// Đường dẫn đầy đủ đến file trên server
        /// Ví dụ: \\192.168.1.190\Accountant\...\File.xlsx
        /// </summary>
        public string FilePath { get; set; } = string.Empty;
    }
}