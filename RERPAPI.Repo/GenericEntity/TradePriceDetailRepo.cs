using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class TradePriceDetailRepo : GenericRepo<TradePriceDetail>
    {
        public TradePriceDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}