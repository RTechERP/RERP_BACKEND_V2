using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class KPISaleApprovalLogRepo : GenericRepo<KPISaleApprovalLog>
    {
        public KPISaleApprovalLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}