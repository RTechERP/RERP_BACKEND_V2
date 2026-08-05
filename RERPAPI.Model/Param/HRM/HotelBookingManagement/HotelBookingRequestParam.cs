using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Param.HRM.HotelBookingManagement;

/// <summary>
/// Tham số tìm kiếm quản lý phòng khách sạn
/// </summary>
public class HotelBookingRequestParam
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Keyword { get; set; }
    public int? EmployeeID { get; set; }
    public int? EmployeeBookerID { get; set; }
    public int? ProjectID { get; set; }
    public List<int>? SelectedIDs { get; set; }
}
