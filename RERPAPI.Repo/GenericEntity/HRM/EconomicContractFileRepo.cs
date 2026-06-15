using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class EconomicContractFileRepo : GenericRepo<EconomicContractFile>
    {
        public EconomicContractFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}