namespace RERPAPI.Model.DTO.CourseWebDTO
{
    public partial class SaveCourseExamResultWebParam
    {
        public int CourseId { get; set; }
        public int ExamType { get; set; }
        public RERPAPI.Model.Entities.RTCCourse.CourseExamResult CourseExamResult { get; set; } = new RERPAPI.Model.Entities.RTCCourse.CourseExamResult();
    }
}