using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class MeetingMinutesAttendance
{
    public int ID { get; set; }

    public int? MeetingMinutesID { get; set; }

    public int? EmployeeID { get; set; }

    public string? Section { get; set; }

    public string? CustomerName { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public bool? IsEmployee { get; set; }

    public int? UserTeamID { get; set; }

    public string? EmailCustomer { get; set; }

    public string? AddressCustomer { get; set; }

    public string? FullName { get; set; }
}
