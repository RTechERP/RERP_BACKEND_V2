using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class FlightBookingPassenger
{
    public int ID { get; set; }

    public int? Type { get; set; }

    public int? EmployeeID { get; set; }

    public string? FullName { get; set; }

    public int? FlightBookingManagementID { get; set; }
}
