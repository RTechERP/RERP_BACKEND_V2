using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CurrencyConfigRepo : GenericRepo<CurrencyConfig>
    {
        public CurrencyConfigRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}