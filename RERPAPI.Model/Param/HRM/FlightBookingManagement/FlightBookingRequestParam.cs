using System;

namespace RERPAPI.Model.Param.HRM.FlightBookingManagement;

/// <summary>
/// Tham số tìm kiếm quản lý vé máy bay
/// </summary>
public class FlightBookingRequestParam
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Keyword { get; set; }
    public int? EmployeeID { get; set; }
    public int? ProjectID { get; set; }
    public System.Collections.Generic.List<int>? SelectedIDs { get; set; }
}
