using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class Province
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public string? ProvinceCode { get; set; }

    public string? ProvinceName { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
