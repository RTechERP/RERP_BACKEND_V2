using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class CustomerPart
{
    public int ID { get; set; }

    public string? PartName { get; set; }

    public string? PartCode { get; set; }

    public int? CustomerID { get; set; }

    public int? STT { get; set; }
}
