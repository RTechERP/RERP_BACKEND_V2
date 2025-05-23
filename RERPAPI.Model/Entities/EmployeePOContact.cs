using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeePOContact
{
    public int ID { get; set; }

    public int? EmployeeID { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public int? ComCode { get; set; }
}
