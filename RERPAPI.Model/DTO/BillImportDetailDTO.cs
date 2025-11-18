using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{

    public class BillImportDetailDTO : BillImportDetail
    {

        public int? POKHDetailID { get; set; }
        public int? CustomerID { get; set; }
        public decimal? QuantityRequestBuy { get; set; }
    }
}
