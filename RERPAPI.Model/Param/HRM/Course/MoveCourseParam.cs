using System.ComponentModel.DataAnnotations;

namespace RERPAPI.Model.Param.HRM.Course;

public class MoveCourseRequest
{
    [Range(1, int.MaxValue)]
    public int SourceCourseId { get; set; }

    [Range(1, int.MaxValue)]
    public int TargetCatalogId { get; set; }

    public int? TargetDepartmentId { get; set; }

    public int? TargetCatalogType { get; set; }

    public List<int> ProjectTypeIds { get; set; } = new();
}

public class MoveCoursePreview
{
    public MoveCourseSource SourceCourse { get; set; } = new();
    public MoveCourseCounts Counts { get; set; } = new();
}

public class MoveCourseSource
{
    public int ID { get; set; }
    public string? Code { get; set; }
    public string? NameCourse { get; set; }
    public int? CourseCatalogID { get; set; }
    public int? DepartmentID { get; set; }
    public int? CatalogType { get; set; }
}

public class MoveCourseResult
{
    public int MovedCourseId { get; set; }
    public MoveCourseCounts Counts { get; set; } = new();
}

public class MoveCourseCounts
{
    public int CourseKpiMaps { get; set; }
    public int Lessons { get; set; }
    public int CourseFiles { get; set; }
    public int Exams { get; set; }
    public int Questions { get; set; }
    public int Answers { get; set; }
    public int RightAnswers { get; set; }
}
