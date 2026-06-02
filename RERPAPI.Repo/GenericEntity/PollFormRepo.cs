using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class PollFormRepo : GenericRepo<PollForm>
    {
        public PollFormRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}