using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class PONCC
{
    public int ID { get; set; }

    /// <summary>
    /// trạng thái duyệt
    /// </summary>
    public bool? IsApproved { get; set; }

    /// <summary>
    /// mã PO
    /// </summary>
    public string? POCode { get; set; }

    /// <summary>
    /// nhà cc
    /// </summary>
    public string? UserNCC { get; set; }

    /// <summary>
    /// mã hóa đơn
    /// </summary>
    public string? BillCode { get; set; }

    /// <summary>
    /// ngày tạo PO
    /// </summary>
    public DateTime? ReceivedDatePO { get; set; }

    /// <summary>
    /// tổng tiền PO
    /// </summary>
    public decimal? TotalMoneyPO { get; set; }

    /// <summary>
    /// ngày yêu cầu giao hàng
    /// </summary>
    public DateTime? RequestDate { get; set; }

    /// <summary>
    /// người liên hệ
    /// </summary>
    public string? UserName { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? GroupID { get; set; }

    public int? SupplierID { get; set; }

    public int? UserID { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public DateTime? ExpectedDate { get; set; }

    /// <summary>
    /// Nhân viên mua hàng
    /// </summary>
    public int? EmployeeID { get; set; }

    /// <summary>
    /// Số ngày giao hàng
    /// </summary>
    public int? DeliveryTime { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Tình trạng đơn hàng(Đã giao ,Chưa giao,....)
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// Số tài khoản
    /// </summary>
    public string? AccountNumber { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? Status_Old { get; set; }

    public bool? NCCNew { get; set; }

    public string? RuleIncoterm { get; set; }

    public string? RulePay { get; set; }

    public string? BankingFee { get; set; }

    public string? AddressDelivery { get; set; }

    public string? SupplierVoucher { get; set; }

    public int? Company { get; set; }

    public string? OriginItem { get; set; }

    public decimal? CurrencyRate { get; set; }

    public int? Currency { get; set; }

    public string? FedexAccount { get; set; }

    public int? SupplierSaleID { get; set; }

    public bool? DeptSupplier { get; set; }

    public string? AccountNumberSupplier { get; set; }

    public string? BankSupplier { get; set; }

    public string? BankCharge { get; set; }

    public string? OtherTerms { get; set; }

    public string? OrderTargets { get; set; }

    public int? CurrencyID { get; set; }

    public bool? IsDeleted { get; set; }

    public int? POType { get; set; }
}
