using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class EmployeeTeamSaleLink
{
    public int ID { get; set; }

    public int EmployeeID { get; set; }

    public int EmployeeTeamSaleID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
