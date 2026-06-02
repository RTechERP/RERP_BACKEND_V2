using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class PollSectionRepo : GenericRepo<PollSection>
    {
        public PollSectionRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}