using RERPAPI.Model.DTO.CourseWebDTO;
using RERPAPI.Model.Entities.RTCCourse;

namespace RERPAPI.Model.Param.CourseWeb
{
    public partial class SaveCourseQuestionParamWeb
    {
        public int ExamType { get; set; }
        public CourseQuestion Question { get; set; } = new CourseQuestion();
        public List<CourseAnswerWebDTO> Answers { get; set; } = new List<CourseAnswerWebDTO>();
        public List<int> DeleteAnswerIds { get; set; } = new List<int>();
    }
}