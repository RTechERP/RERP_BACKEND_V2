namespace RERPAPI.Model.DTO
{
    public class RequestInvoiceSummaryFilesDownloadDTO
    {
        public int RequestInvoiceID { get; set; }
        public int? POKHId { get; set; }
        public string CompanyText { get; set; }
        public string InvoiceNumber { get; set; }

        public string InvoiceDate { get; set; }
    }
}