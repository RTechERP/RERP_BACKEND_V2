using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class HRHiringRequestGenderLinkRepo : GenericRepo<HRHiringRequestGenderLink>
    {
        public HRHiringRequestGenderLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
