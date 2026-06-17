using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class VehicleManagement
{
    public int ID { get; set; }

    public int? EmployeeID { get; set; }

    public string? VehicleName { get; set; }

    public string? LicensePlate { get; set; }

    public int? VehicleCategory { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? Slot { get; set; }

    public int? DriverType { get; set; }

    public string? DriverName { get; set; }

    public string? PhoneNumber { get; set; }

    public int? STT { get; set; }

    public int? VehicleCategoryID { get; set; }

    public bool? IsDeleted { get; set; }
}
