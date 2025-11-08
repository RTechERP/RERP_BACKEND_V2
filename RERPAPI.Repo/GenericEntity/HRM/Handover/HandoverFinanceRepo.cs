using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.BBNV
{
    public class HandoverFinanceRepo : GenericRepo<HandoverFinance>
    {
        public HandoverFinanceRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
