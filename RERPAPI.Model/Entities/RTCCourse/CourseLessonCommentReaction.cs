using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities.RTCCourse;

public partial class CourseLessonCommentReaction
{
    public int ID { get; set; }

    public int CommentID { get; set; }

    public int EmployeeID { get; set; }

    public string ReactionType { get; set; } = null!;

    public DateTime CreatedDate { get; set; }
}
