using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class VehicleCategory
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public string? CategoryCode { get; set; }

    public string? CategoryName { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
