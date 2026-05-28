using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class TermCondition
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? Type { get; set; }

    public string? TermCode { get; set; }

    public string? DescriptionVietnamese { get; set; }

    public string? DescriptionEnglish { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}
