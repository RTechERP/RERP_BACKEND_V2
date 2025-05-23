using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectEmployee
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? ProjectID { get; set; }

    public int? EmployeeID { get; set; }

    public int? ProjectTypeID { get; set; }

    /// <summary>
    /// Người nhận bàn giao
    /// </summary>
    public int? ReceiverID { get; set; }

    public bool? IsLeader { get; set; }

    public string? Note { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
