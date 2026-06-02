using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class PhasedAllocationPersonDetailRepo : GenericRepo<PhasedAllocationPersonDetail>
    {
        public PhasedAllocationPersonDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}