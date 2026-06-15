using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ExamResultDetailRepo : GenericRepo<ExamResultDetail>
    {
        public ExamResultDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}