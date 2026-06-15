using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class SaveViewPOKHDTO
    {
        public List<POKHDetail> pokhDetails { get; set; }
        public List<RequestInvoiceDetail> requestInvoiceDetails { get; set; }
    }
}