using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class AGVProductGroup
{
    public int ID { get; set; }

    public string? AGVProductGroupNo { get; set; }

    public string? AGVProductGroupName { get; set; }

    public int? NumberOrder { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
