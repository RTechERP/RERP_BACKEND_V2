using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class VehicleBookingFile
{
    public int ID { get; set; }

    public int? VehicleBookingID { get; set; }

    public string? FileName { get; set; }

    public string? OriginPath { get; set; }

    public string? ServerPath { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? STT { get; set; }
}
