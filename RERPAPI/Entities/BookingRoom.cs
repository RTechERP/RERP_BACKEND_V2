using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class BookingRoom
{
    public int ID { get; set; }

    public int? MeetingRoomID { get; set; }

    public DateTime? DateRegister { get; set; }

    public int? EmployeeID { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public string? Content { get; set; }

    /// <summary>
    /// 0:Chưa duyệt; 1:đã duyệt
    /// </summary>
    public int? IsApproved { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? DepartmentID { get; set; }
}
