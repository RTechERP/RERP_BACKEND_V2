using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeBussinessVehicle
{
    public int ID { get; set; }

    public int? EmployeeBussinesID { get; set; }

    public int? EmployeeVehicleBussinessID { get; set; }

    public decimal? Cost { get; set; }

    public string? BillImage { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public string? Note { get; set; }

    public string? VehicleName { get; set; }

    public bool? IsDeleted { get; set; }
}
