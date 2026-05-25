using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class PollQuestionRepo : GenericRepo<PollQuestion>
    {
        public PollQuestionRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
