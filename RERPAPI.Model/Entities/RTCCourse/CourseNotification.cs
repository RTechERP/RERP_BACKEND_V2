using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities.RTCCourse;

public partial class CourseNotification
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public int? TriggerEmployeeId { get; set; }

    public string NotificationType { get; set; } = null!;

    public string? Title { get; set; }

    public string Content { get; set; } = null!;

    public string? TargetUrl { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? CommentId { get; set; }
}
