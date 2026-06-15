using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class EconomicContractRepo : GenericRepo<EconomicContract>
    {
        public EconomicContractRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}