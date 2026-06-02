using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class InvoiceDTO
    {
        public int IdMapping { get; set; }
        public List<InvoiceLink> Details { get; set; }
    }
}