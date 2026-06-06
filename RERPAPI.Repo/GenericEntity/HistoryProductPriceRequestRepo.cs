
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class HistoryProductPriceRequestRepo : GenericRepo<HistoryProductPriceRequest>
    {
        public HistoryProductPriceRequestRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
