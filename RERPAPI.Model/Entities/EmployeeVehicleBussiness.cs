using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeVehicleBussiness
{
    public int ID { get; set; }

    public string? VehicleCode { get; set; }

    public string? VehicleName { get; set; }

    public decimal? Cost { get; set; }

    /// <summary>
    /// True: Cho phép người khai báo công tác sửa chi phí đi lại, False: Không cho phép
    /// </summary>
    public bool? EditCost { get; set; }

    public bool? IsDeleted { get; set; }
}
