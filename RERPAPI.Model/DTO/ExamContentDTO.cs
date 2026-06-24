using System;

namespace RERPAPI.Model.DTO
{
    public class ExamContentDTO
    {
        public int ID { get; set; }
        public int STT { get; set; }
        public int ExamResultDetailID { get; set; }
        public int CourseQuestionID { get; set; }
        public string QuestionText { get; set; }
        public int CourseAnswerID { get; set; }
        public string AnswerText { get; set; }
        public bool IsPicked { get; set; }
        public int ExamResultID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string ImageName { get; set; }
    }
}
