namespace RERPAPI.Model.DTO
{
    public class DownloadPaymentOrderDTO
    {
        public int PaymentOrderId { get; set; }
        public string PaymentOrderCode { get; set; }
        public List<string> FilePath { get; set; }
    }
}