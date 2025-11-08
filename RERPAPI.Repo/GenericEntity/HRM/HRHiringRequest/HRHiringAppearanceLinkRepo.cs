using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;


namespace RERPAPI.Repo.GenericEntity
{
    public class HRHiringAppearanceLinkRepo : GenericRepo<HRHiringAppearanceLink>

    {
        public HRHiringAppearanceLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
