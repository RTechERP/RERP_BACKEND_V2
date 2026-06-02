using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CourseExamRepo : GenericRepo<CourseExam>
    {
        public CourseExamRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}