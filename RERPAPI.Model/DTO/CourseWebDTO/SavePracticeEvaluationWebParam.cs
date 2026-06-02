using RERPAPI.Model.Entities.RTCCourse;

namespace RERPAPI.Model.DTO.CourseWebDTO
{
    public partial class SavePracticeEvaluationWebParam
    {
        public int CourseId { get; set; }
        public int ExamType { get; set; } // 1: Trắc nghiệm, 2: Thực hành, 3: Bài tập
        public CourseExamResult CourseExamResult { get; set; } = new CourseExamResult();
        public List<CourseExamEvaluate> Evaluations { get; set; } = new List<CourseExamEvaluate>();
    }
}