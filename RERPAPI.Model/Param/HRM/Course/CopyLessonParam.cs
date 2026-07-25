using System.ComponentModel.DataAnnotations;

namespace RERPAPI.Model.Param.HRM.Course
{
    public class CopyLessonRequest
    {
        [Range(1, int.MaxValue)]
        public int SourceLessonId { get; set; }

        [Required, MaxLength(50)]
        public string NewCode { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string NewName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int TargetCourseId { get; set; }
    }

    public class CopyLessonPreview
    {
        public CopyLessonSource SourceLesson { get; set; } = new();
        public CopyLessonCounts Counts { get; set; } = new();
    }

    public class CopyLessonSource
    {
        public int ID { get; set; }
        public string? Code { get; set; }
        public string? LessonTitle { get; set; }
        public string? LessonContent { get; set; }
        public int? CourseID { get; set; }
        public string? CourseName { get; set; }
        public int? Duration { get; set; }
        public string? VideoURL { get; set; }
        public string? UrlPDF { get; set; }
    }

    public class CopyLessonResult
    {
        public int NewLessonId { get; set; }
        public CopyLessonCounts Counts { get; set; } = new();
    }

    public class CopyLessonCounts
    {
        public int Files { get; set; }
        public int Exams { get; set; }
        public int Questions { get; set; }
        public int Answers { get; set; }
        public int RightAnswers { get; set; }
    }
}
