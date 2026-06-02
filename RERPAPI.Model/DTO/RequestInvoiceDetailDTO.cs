using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class RequestInvoiceDetailDTO
    {
        public RequestInvoice RequestInvoices { get; set; } = new RequestInvoice();
        public List<RequestInvoiceDetail>? RequestInvoiceDetails { get; set; } = new List<RequestInvoiceDetail>();
        public List<int>? DeletedDetailIds { get; set; }
    }
}