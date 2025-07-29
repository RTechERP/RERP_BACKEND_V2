using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class TradePriceDTO
    {
        public TradePrice tradePrices { get; set; }
        public List<TradePriceDetail> tradePriceDetails { get; set; }
    }
}
