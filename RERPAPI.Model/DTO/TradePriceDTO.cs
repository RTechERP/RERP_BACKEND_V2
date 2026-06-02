using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class TradePriceDTO
    {
        public TradePrice tradePrices { get; set; }
        public List<TradePriceDetail> tradePriceDetails { get; set; }
        public List<int> deletedTradePriceDetails { get; set; }
    }
}