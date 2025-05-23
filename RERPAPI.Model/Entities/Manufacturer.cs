using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class Manufacturer
{
    public int ID { get; set; }

    public string ManufacturerCode { get; set; } = null!;

    public string ManufacturerName { get; set; } = null!;

    public string? Note { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
