using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class ConvertPartListPODTO : ProjectPartList
    {
        public decimal UnitPriceQuote { get; set; }
    }
}