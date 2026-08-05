using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities.RTCCourse;

public partial class CourseLessonComment
{
    public int ID { get; set; }

    public int LessonID { get; set; }

    public int EmployeeID { get; set; }

    public int? ParentID { get; set; }

    public string Content { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? ReplyToName { get; set; }

    public string? ImageOriginalName { get; set; }

    public string? ImageServerName { get; set; }

    public string? AttachmentOriginalName { get; set; }

    public string? AttachmentServerName { get; set; }
}
