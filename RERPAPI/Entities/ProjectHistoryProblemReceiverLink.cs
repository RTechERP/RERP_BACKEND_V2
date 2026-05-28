using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ProjectHistoryProblemReceiverLink
{
    public int ID { get; set; }

    public int? ProjectHistoryProblemID { get; set; }

    public int? ReceiverID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }
}
