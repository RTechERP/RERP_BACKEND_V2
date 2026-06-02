using RERPAPI.Model.Entities.RTCCourse;

namespace RERPAPI.Model.DTO.CourseWebDTO
{
    public class CourseAnswerWebDTO : CourseAnswer
    {
        public bool IsRightAnswer { get; set; }
    }
}