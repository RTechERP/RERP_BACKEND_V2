using System.ComponentModel.DataAnnotations;

namespace RERPAPI.Model.Param.HRM.Course;

public class MoveLessonRequest
{
    [Range(1, int.MaxValue)]
    public int SourceLessonId { get; set; }

    [Range(1, int.MaxValue)]
    public int TargetCourseId { get; set; }

    public int? TargetCatalogId { get; set; }

    public int? TargetDepartmentId { get; set; }

    public int? TargetCatalogType { get; set; }

    public List<int> ProjectTypeIds { get; set; } = new();
}

public class MoveLessonPreview
{
    public MoveLessonSource SourceLesson { get; set; } = new();
    public MoveLessonCounts Counts { get; set; } = new();
}

public class MoveLessonSource
{
    public int ID { get; set; }
    public string? Code { get; set; }
    public string? LessonTitle { get; set; }
    public int? CourseID { get; set; }
    public int? DepartmentID { get; set; }
    public int? CatalogType { get; set; }
}

public class MoveLessonResult
{
    public int MovedLessonId { get; set; }
    public MoveLessonCounts Counts { get; set; } = new();
}

public class MoveLessonCounts
{
    public int CourseFiles { get; set; }
    public int Exams { get; set; }
    public int Questions { get; set; }
    public int Answers { get; set; }
    public int RightAnswers { get; set; }
}
