using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu nhóm công việc của dự án
/// </summary>
public partial class ProjectTaskGroup
{
    public int ID { get; set; }

    public int? ProjectID { get; set; }

    public string? TaskGroupName { get; set; }

    public int? OrderIndex { get; set; }

    public string? Color { get; set; }
}
