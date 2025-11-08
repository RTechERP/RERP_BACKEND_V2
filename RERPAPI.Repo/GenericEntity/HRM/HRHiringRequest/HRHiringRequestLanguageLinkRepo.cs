using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;


namespace RERPAPI.Repo.GenericEntity
{
    public class HRHiringRequestLanguageLinkRepo : GenericRepo<HRHiringRequestLanguageLink>
    {
        public HRHiringRequestLanguageLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
