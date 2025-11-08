using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class HRHiringRequestHealthLinkRepo : GenericRepo<HRHiringRequestHealthLink>
    {
        public HRHiringRequestHealthLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
