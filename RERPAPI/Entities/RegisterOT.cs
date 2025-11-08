using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class RegisterOT
{
    public int ID { get; set; }

    public int? UserOT { get; set; }

    public bool? TypeOT { get; set; }

    public bool? TypeGOB { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public double? SumTime { get; set; }

    public double? CostsOT { get; set; }

    public int? CustomerID { get; set; }

    public bool? NotCheckInOffice { get; set; }

    public int? Vehicle { get; set; }

    public double? CostVehicle { get; set; }

    public bool? Overnight { get; set; }

    public double? CostON { get; set; }

    public bool? Confirm { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual User? UserOTNavigation { get; set; }
}
