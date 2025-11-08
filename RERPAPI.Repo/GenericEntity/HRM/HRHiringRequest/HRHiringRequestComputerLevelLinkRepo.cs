using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class HRHiringRequestComputerLevelLinkRepo : GenericRepo<HRHiringRequestComputerLevelLink>
    {
        public HRHiringRequestComputerLevelLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
