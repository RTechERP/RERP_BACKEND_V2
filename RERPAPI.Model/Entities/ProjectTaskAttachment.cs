using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectTaskAttachment
{
    public int ID { get; set; }

    public int? ProjectTaskID { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public int? EmployeeUploadID { get; set; }

    public DateTime? UploadedDate { get; set; }
}
