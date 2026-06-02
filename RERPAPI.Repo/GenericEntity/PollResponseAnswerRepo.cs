using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class PollResponseAnswerRepo : GenericRepo<PollResponseAnswer>
    {
        public PollResponseAnswerRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}