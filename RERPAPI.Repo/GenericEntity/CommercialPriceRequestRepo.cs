using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CommercialPriceRequestRepo : GenericRepo<CommercialPriceRequest>
    {
        public CommercialPriceRequestRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}