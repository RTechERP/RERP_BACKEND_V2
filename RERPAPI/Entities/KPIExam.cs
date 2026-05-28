using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class KPIExam
{
    public int ID { get; set; }

    public int? KPISessionID { get; set; }

    public string? ExamCode { get; set; }

    public string? ExamName { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? Deadline { get; set; }
}
