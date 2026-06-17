using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class Notify
{
    public int ID { get; set; }

    public string? Title { get; set; }

    public string? Text { get; set; }

    /// <summary>
    /// Người nhận thông báo này
    /// </summary>
    public int? EmployeeID { get; set; }

    /// <summary>
    /// Phòng ban nhận thông báo này
    /// </summary>
    public int? DepartmentID { get; set; }

    /// <summary>
    /// 1:Chưa gửi;2:Đã gửi
    /// </summary>
    public int? NotifyStatus { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
