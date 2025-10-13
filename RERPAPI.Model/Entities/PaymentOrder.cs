using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class PaymentOrder
{
    public int ID { get; set; }

    public string? Code { get; set; }

    /// <summary>
    /// Loại đề nghị (1:Đề nghị tạm ứng; 2:Đề nghị thanh toán/quyết toán)
    /// </summary>
    public int? TypeOrder { get; set; }

    public int? PaymentOrderTypeID { get; set; }

    /// <summary>
    /// Ngày làm đề nghị
    /// </summary>
    public DateTime? DateOrder { get; set; }

    public int? EmployeeID { get; set; }

    public string? ReasonOrder { get; set; }

    /// <summary>
    /// Ngày quyết toán đối với đề nghị tạm ứng
    /// </summary>
    public DateTime? DatePayment { get; set; }

    public string? ReceiverInfo { get; set; }

    /// <summary>
    /// Loại thanh toán(1:Chuyển khoản; 2:Tiền mặt)
    /// </summary>
    public int? TypePayment { get; set; }

    public string? AccountNumber { get; set; }

    public string? Bank { get; set; }

    public decimal? TotalMoney { get; set; }

    public string? TotalMoneyText { get; set; }

    public string? Unit { get; set; }

    public bool? IsDelete { get; set; }

    public string? Note { get; set; }

    /// <summary>
    /// 1:Chuyển khoản RTC; 2:Chuyển khoản MVI;3:Chuyển khoản APR;4:Chuyển khoản Yonko;5:Chuyển khoản cá nhân
    /// </summary>
    public int? TypeBankTransfer { get; set; }

    public string? ContentBankTransfer { get; set; }

    public string? AccountingNote { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public DateTime? DeadlinePayment { get; set; }

    public bool? IsUrgent { get; set; }

    public int? PONCCID { get; set; }

    public int? SupplierSaleID { get; set; }

    public int? CustomerID { get; set; }

    public int? TypeDocument { get; set; }

    public string? NumberDocument { get; set; }

    public bool? IsSpecialOrder { get; set; }

    public int? ProjectID { get; set; }

    /// <summary>
    /// Có hóa đơn
    /// </summary>
    public bool? IsBill { get; set; }

    /// <summary>
    /// Điểm đi
    /// </summary>
    public string? StartLocation { get; set; }

    /// <summary>
    /// Điểm đến
    /// </summary>
    public string? EndLocation { get; set; }
}
