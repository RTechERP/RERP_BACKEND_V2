using System.ComponentModel.DataAnnotations;

namespace RERPAPI.Model.Param.HRM.Course
{
    public class CopyCourseCatalogRequest
    {
        [Range(1, int.MaxValue)]
        public int SourceCatalogId { get; set; }

        [Required, MaxLength(50)]
        public string NewCode { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string NewName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int DepartmentId { get; set; }

        [Range(1, 2)]
        public int CatalogType { get; set; }

        public List<int> ProjectTypeIds { get; set; } = new();
    }

    public class CopyCourseCatalogPreview
    {
        public CopyCourseCatalogSource SourceCatalog { get; set; } = new();
        public CopyCourseCatalogCounts Counts { get; set; } = new();
    }

    public class CopyCourseCatalogSource
    {
        public int ID { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public int? DepartmentID { get; set; }
        public int? CatalogType { get; set; }
        public List<int> ProjectTypeIDs { get; set; } = new();
    }

    public class CopyCourseCatalogResult
    {
        public int NewCatalogId { get; set; }
        public CopyCourseCatalogCounts Counts { get; set; } = new();
    }

    public class CopyCourseCatalogCounts
    {
        public int CatalogProjectTypes { get; set; }
        public int Courses { get; set; }
        public int CourseKpiMaps { get; set; }
        public int Lessons { get; set; }
        public int CourseFiles { get; set; }
        public int Exams { get; set; }
        public int Questions { get; set; }
        public int Answers { get; set; }
        public int RightAnswers { get; set; }
    }
}
