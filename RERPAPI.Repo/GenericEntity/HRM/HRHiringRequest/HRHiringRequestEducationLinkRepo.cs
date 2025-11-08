using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class HRHiringRequestEducationLinkRepo : GenericRepo<HRHiringRequestEducationLink>
    {
        public HRHiringRequestEducationLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
