using System.ComponentModel.DataAnnotations;

namespace RERPAPI.Model.Param.HRM.Course
{
    public class MoveCourseCatalogRequest
    {
        [Range(1, int.MaxValue)]
        public int SourceCatalogId { get; set; }

        [Range(1, int.MaxValue)]
        public int TargetDepartmentId { get; set; }

        [Range(1, 2)]
        public int TargetCatalogType { get; set; }

        public List<int> ProjectTypeIds { get; set; } = new();
    }

    public class MoveCourseCatalogPreview
    {
        public MoveCourseCatalogSource SourceCatalog { get; set; } = new();
        public MoveCourseCatalogCounts Counts { get; set; } = new();
    }

    public class MoveCourseCatalogSource
    {
        public int ID { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public int? DepartmentID { get; set; }
        public int? CatalogType { get; set; }
        public List<int> ProjectTypeIDs { get; set; } = new();
    }

    public class MoveCourseCatalogResult
    {
        public int MovedCatalogId { get; set; }
        public MoveCourseCatalogCounts Counts { get; set; } = new();
    }

    public class MoveCourseCatalogCounts
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
