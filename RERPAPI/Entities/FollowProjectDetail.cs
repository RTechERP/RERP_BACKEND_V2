using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class FollowProjectDetail
{
    public int ID { get; set; }

    /// <summary>
    /// 1: Finish, 0: Pending
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// Ngày PO
    /// </summary>
    public DateTime? PODate { get; set; }

    /// <summary>
    /// ngày yêu cầu giao hàng
    /// </summary>
    public DateTime? DeliveryRequestedDate { get; set; }

    /// <summary>
    /// ngày đặt hàng
    /// </summary>
    public DateTime? OderDate { get; set; }

    /// <summary>
    /// ngày ship hàng
    /// </summary>
    public DateTime? ShipmentDate { get; set; }

    /// <summary>
    /// ngày hàng về
    /// </summary>
    public DateTime? ArrivalDate { get; set; }

    public string? Partner { get; set; }

    /// <summary>
    /// số lượng
    /// </summary>
    public int? Qty { get; set; }

    public int? FollowProjectID { get; set; }

    /// <summary>
    /// tên sản phẩm
    /// </summary>
    public int? ProductID { get; set; }

    /// <summary>
    /// model dự án
    /// </summary>
    public string? ProjectModel { get; set; }

    /// <summary>
    /// model chuẩn
    /// </summary>
    public string? StandardModel { get; set; }

    public decimal? UnitPriceUSD { get; set; }

    public decimal? UnitPriceVND { get; set; }

    public decimal? TotalPriceUSD { get; set; }

    public decimal? TotalPriceVND { get; set; }

    /// <summary>
    /// thuế nhập khẩu (%)
    /// </summary>
    public decimal? ImportTax { get; set; }

    /// <summary>
    /// Thuế nhập khẩu 1/pcs (vnd)
    /// </summary>
    public decimal? ImportTaxVND { get; set; }

    /// <summary>
    /// tổng thuế nhập khẩu
    /// </summary>
    public decimal? TotalImportTax { get; set; }

    /// <summary>
    /// chí phí hải quan
    /// </summary>
    public decimal? CustomFees { get; set; }

    /// <summary>
    /// Số tờ khai
    /// </summary>
    public decimal? Declaration { get; set; }

    /// <summary>
    /// phí ngân hàng /1 tờ điện
    /// </summary>
    public decimal? BankCharges { get; set; }

    /// <summary>
    /// Chi phí bảo hiểm
    /// </summary>
    public decimal? InsuranceFees { get; set; }

    /// <summary>
    /// Phí vận chuyển
    /// </summary>
    public decimal? TransportFee { get; set; }

    /// <summary>
    /// Tổng chi phí hải quan
    /// </summary>
    public decimal? TotalCustomfees { get; set; }

    /// <summary>
    /// Số lần điện
    /// </summary>
    public decimal? NumberOfTransactions { get; set; }

    /// <summary>
    /// Tổng chi phí ngân hàng
    /// </summary>
    public decimal? TotalBankCharges { get; set; }

    /// <summary>
    /// 1: Đã nhận hàng, 2: đã nhận hóa đơn, , 3:đã thanh toán, 4: đã nhập kho
    /// </summary>
    public int? Progress { get; set; }

    public decimal? Exchange { get; set; }

    /// <summary>
    /// Công nợ Nhà cung cấp
    /// </summary>
    public int? Debt { get; set; }

    public string? Bill { get; set; }

    public bool? IsPay { get; set; }

    public bool? IsAddWarehouse { get; set; }

    public bool? IsItemReceived { get; set; }

    public bool? IsBillStatus { get; set; }

    /// <summary>
    /// Ngày thanh toán
    /// </summary>
    public DateTime? PayDate { get; set; }

    /// <summary>
    /// Số lượng khách hàng đặt
    /// </summary>
    public int? QtyCustomer { get; set; }

    public string? PONo { get; set; }

    public int? LeadTime { get; set; }

    public bool? StatusShip { get; set; }

    public bool? IsAlreadyDelivered { get; set; }

    /// <summary>
    /// 0:Pending;1:Finish; 2:Chưa giao đủ
    /// </summary>
    public string? Note { get; set; }

    public int? ParentID { get; set; }

    public decimal? CostIncludingVATDetail { get; set; }

    public decimal? CostWithoutVATDetail { get; set; }

    public decimal? TaxDetail { get; set; }

    public int? NewRow { get; set; }

    public int? POKHDetailID { get; set; }

    public int? STT { get; set; }

    public decimal? OldPrice { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }

    public int? SupplierID { get; set; }

    public bool? SlowDelivery { get; set; }

    public string? NoteDelivery { get; set; }
}
