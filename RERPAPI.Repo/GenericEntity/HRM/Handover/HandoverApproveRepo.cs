using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.BBNV
{
    public class HandoverApproveRepo : GenericRepo<HandoverApprove>
    {
        public HandoverApproveRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
