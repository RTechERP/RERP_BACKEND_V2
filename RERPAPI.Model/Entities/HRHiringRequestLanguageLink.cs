using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class HRHiringRequestLanguageLink
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? HRHiringRequestID { get; set; }

    public int? LanguageType { get; set; }

    public string? LanguageTypeName { get; set; }

    public int? LanguageLevel { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }
}
