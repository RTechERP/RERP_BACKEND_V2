using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class CourseAnswerDTO : CourseAnswer
    {
        public bool IsRightAnswer { get; set; }
    }
}