using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class QuotationDTO
    {
        public QuotationKH quotationKHs { get; set; } = new QuotationKH();
        public List<QuotationKHDetail> quotationKHDetails { get; set; } = new List<QuotationKHDetail>();
        public List<int> DeletedDetailIds { get; set; } = new List<int>();
    }
}