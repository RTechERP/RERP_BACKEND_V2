using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class MeetingMinutesDetail
{
    public int ID { get; set; }

    public int? MeetingMinutesID { get; set; }

    public int? ProjectHistoryProblemID { get; set; }

    public int? EmployeeID { get; set; }

    public string? CustomerName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? DetailContent { get; set; }

    public string? DetailResult { get; set; }

    public string? Note { get; set; }

    public string? DetailPlan { get; set; }

    public bool? IsEmployee { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? PlanDate { get; set; }
}
