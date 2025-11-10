using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class POKH
{
    public int ID { get; set; }

    /// <summary>
    /// 0:Chưa giao , chưa thanh toán - 1:Đã giao, đã thanh toán -  2: Chưa giao,đã thanh toán - 3: Đã giao, nhưng thanh toán - 4:Đã thanh toán, GH chưa xuất hóa đơn
    /// </summary>
    public int? Status { get; set; }

    public string? POCode { get; set; }

    /// <summary>
    /// người phụ trách
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// dự án
    /// </summary>
    public int? ProjectID { get; set; }

    /// <summary>
    /// mã hóa đơn
    /// </summary>
    public string? BillCode { get; set; }

    public DateTime? ReceivedDatePO { get; set; }

    /// <summary>
    /// tổng tiền nhận PO
    /// </summary>
    public decimal? TotalMoneyPO { get; set; }

    /// <summary>
    /// ngày bắt đầu
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// ngày kết thúc giao hàng
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// tình trạng tiến độ giao hàng: 1 Chưa giao, 2 : Giao 1 phần, 3: Đã giao
    /// </summary>
    public int? DeliveryStatus { get; set; }

    /// <summary>
    /// tình trạng nhập kho
    /// </summary>
    public int? ImportStatus { get; set; }

    /// <summary>
    /// tình trạng xuất kho
    /// </summary>
    public int? ExportStatus { get; set; }

    public string? Note { get; set; }

    public string? GroupID { get; set; }

    public int? CustomerID { get; set; }

    public int? EndUserID { get; set; }

    public int? DealerID { get; set; }

    public bool? IsApproved { get; set; }

    /// <summary>
    /// 0: Prescale 1: PCB 2: VISION 3: Other
    /// </summary>
    public int? POType { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }

    public bool? NewAccount { get; set; }

    public int? UserID { get; set; }

    public string? EndUser { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public decimal? ReceiveMoney { get; set; }

    public bool? IsPay { get; set; }

    public bool? IsShip { get; set; }

    public bool? IsExport { get; set; }

    /// <summary>
    ///  Tình trạng hoá đơn: 0: Chưa có hoá đơn, 1: Đã có hoá đơn
    /// </summary>
    public bool? IsBill { get; set; }

    public decimal? TotalMoneyKoVAT { get; set; }

    public int? FollowProjectID { get; set; }

    public int? PartID { get; set; }

    public int? UserType { get; set; }

    public int? QuotationID { get; set; }

    public string? PONumber { get; set; }

    public bool? IsMerge { get; set; }

    public int? WarehouseID { get; set; }

    public int? CurrencyID { get; set; }

    /// <summary>
    /// Tình trạng thanh toán: 1: Chưa thanh toán, 2: Thanh toán 1 phần, 3: Đã thanh toán
    /// </summary>
    public int? PaymentStatus { get; set; }

    public int? AccountType { get; set; }

    public decimal? Discount { get; set; }

    public decimal? TotalMoneyDiscount { get; set; }

    public bool? IsDeleted { get; set; }
}
