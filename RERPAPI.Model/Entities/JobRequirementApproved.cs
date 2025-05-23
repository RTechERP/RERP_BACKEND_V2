using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class JobRequirementApproved
{
    public int ID { get; set; }

    public int? JobRequirementID { get; set; }

    public int? Step { get; set; }

    public string? StepName { get; set; }

    /// <summary>
    /// 1:Duyệt,2:huỷ duyệt;
    /// </summary>
    public int? IsApproved { get; set; }

    public DateTime? DateApproved { get; set; }

    public int? ApprovedID { get; set; }

    public int? ApprovedActualID { get; set; }

    public string? ReasonCancel { get; set; }

    public string? ContentLog { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
