using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class SealRegulation
{
    public int ID { get; set; }

    public string? SealCode { get; set; }

    public string? SealName { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}
