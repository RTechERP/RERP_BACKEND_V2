using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectPartListPurchaseRequestApproveLog
{
    public int ID { get; set; }

    /// <summary>
    /// 1:Yêu cầu duyệt,2:Hủy yêu cầu duyệt, 3:BGĐ duyệt, 4:BGĐ hủy duyệt,5:TBP duyệt,6:TBP hủy duyệt, 7:hoàn thành, 8:Hủy hoàn thành, 9:Check đặt hàng; 10:Hủy check đặt hàng
    /// </summary>
    public int? Status { get; set; }

    public int? EmployeeID { get; set; }

    public int? ProjectPartlistPurchaseRequestID { get; set; }

    public DateTime? DateStatus { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
