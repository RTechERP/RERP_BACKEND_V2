using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class RequestInvoice
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public string? Code { get; set; }

    public DateTime? DateRequest { get; set; }

    public int? CustomerID { get; set; }

    /// <summary>
    /// Công ty bán
    /// </summary>
    public int? TaxCompanyID { get; set; }

    /// <summary>
    /// ID của người gửi yêu cầu (Lấy từ bảng employee)
    /// </summary>
    public int? EmployeeRequestID { get; set; }

    /// <summary>
    /// ID của người nhận yêu cầu (Lấy từ bảng employee)
    /// </summary>
    public int? ReceriverID { get; set; }

    /// <summary>
    /// 1: YC xuất HĐ, 2: Đã xuất nháp, 3: Đã phát hành hóa đơn
    /// </summary>
    public int? Status { get; set; }

    public string? Note { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsUrgency { get; set; }

    public DateTime? DealineUrgency { get; set; }

    public string? AmendReason { get; set; }

    public bool? IsCustomsDeclared { get; set; }
}
