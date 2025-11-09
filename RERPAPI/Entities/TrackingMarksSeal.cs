using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class TrackingMarksSeal
{
    public int ID { get; set; }

    public int? TrackingMartkID { get; set; }

    public int? SealID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}
