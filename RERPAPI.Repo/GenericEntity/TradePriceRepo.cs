using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class TradePriceRepo : GenericRepo<TradePrice>
    {
        public TradePriceRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}