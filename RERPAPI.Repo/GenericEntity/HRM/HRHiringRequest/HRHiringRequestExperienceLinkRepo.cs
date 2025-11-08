using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class HRHiringRequestExperienceLinkRepo : GenericRepo<HRHiringRequestExperienceLink>
    {
        public HRHiringRequestExperienceLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
