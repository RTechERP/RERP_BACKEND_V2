using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class PollQuestionOptionRepo : GenericRepo<PollQuestionOption>
    {
        public PollQuestionOptionRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
