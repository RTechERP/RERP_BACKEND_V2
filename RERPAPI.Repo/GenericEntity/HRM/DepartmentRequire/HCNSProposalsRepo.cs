using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM.DepartmentRequire
{
    public class HCNSProposalsRepo : GenericRepo<HCNSProposal>
    {
        public HCNSProposalsRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}