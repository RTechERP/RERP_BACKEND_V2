using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class StatusRequestInvoiceDTO
    {
        public List<RequestInvoiceStatusLink> StatusRequestInvoiceLinks { get; set; }
        public List<int> listIdsStatusDel { get; set; }
        public int requestInvoiceId { get; set; }
    }
}