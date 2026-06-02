using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class KPIErrorFineAmountRepo : GenericRepo<KPIErrorFineAmount>
    {
        public KPIErrorFineAmountRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}