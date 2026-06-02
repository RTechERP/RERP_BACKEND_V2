using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ExamResultRepo : GenericRepo<ExamResult>
    {
        public ExamResultRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}