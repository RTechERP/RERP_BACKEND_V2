using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class PaymentOrderDetail
{
    public int ID { get; set; }

    public int? PaymentOrderID { get; set; }

    public string? STT { get; set; }

    public string? ContentPayment { get; set; }

    public string? Unit { get; set; }

    public decimal? Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? TotalMoney { get; set; }

    public string Note { get; set; } = null!;

    public int? ParentID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// 0:Tiền mặt; 1: Chuyển khoản
    /// </summary>
    public int? PaymentMethods { get; set; }

    public string? PaymentInfor { get; set; }

    public int? EmployeeID { get; set; }

    public decimal? TotalPaymentAmount { get; set; }

    public decimal? PaymentPercentage { get; set; }

    public bool? IsDeleted { get; set; }
}
