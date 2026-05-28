using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class HandoverSubordinate
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? HandoverID { get; set; }

    public int? PositionID { get; set; }

    public int? SubordinateID { get; set; }

    public int? AssigneeID { get; set; }

    public int? ReceiverID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
