using System.ComponentModel.DataAnnotations;

namespace RERPAPI.Model.Param.HRM.Course
{
    public class CopyCourseRequest
    {
        [Range(1, int.MaxValue)]
        public int SourceCourseId { get; set; }

        [Required, MaxLength(50)]
        public string NewCode { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string NewName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int TargetCatalogId { get; set; }
    }

    public class CopyCoursePreview
    {
        public CopyCourseSource SourceCourse { get; set; } = new();
        public CopyCourseCounts Counts { get; set; } = new();
    }

    public class CopyCourseSource
    {
        public int ID { get; set; }
        public string? Code { get; set; }
        public string? NameCourse { get; set; }
        public int? CourseCatalogID { get; set; }
        public string? CourseCatalogName { get; set; }
        public int? CatalogType { get; set; }
    }

    public class CopyCourseResult
    {
        public int NewCourseId { get; set; }
        public CopyCourseCounts Counts { get; set; } = new();
    }

    public class CopyCourseCounts
    {
        public int Lessons { get; set; }
        public int CourseFiles { get; set; }
        public int Exams { get; set; }
        public int Questions { get; set; }
        public int Answers { get; set; }
        public int RightAnswers { get; set; }
    }
}
