using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class HRHiringRequestCommunicationLinkRepo : GenericRepo<HRHiringRequestCommunicationLink>
    {
        public HRHiringRequestCommunicationLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
