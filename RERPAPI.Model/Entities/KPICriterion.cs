using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPICriterion
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public string? CriteriaCode { get; set; }

    public string? CriteriaName { get; set; }

    public int? KPICriteriaQuater { get; set; }

    public int? KPICriteriaYear { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
