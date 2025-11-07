using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPIPositionType
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public string? TypeCode { get; set; }

    public string? TypeName { get; set; }

    public int? YearValue { get; set; }

    public int? QuaterValue { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public int? ProjectTypeID { get; set; }
}
