using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ExamResultAnswerDetailRepo : GenericRepo<ExamResultAnswerDetail>
    {
        public ExamResultAnswerDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
