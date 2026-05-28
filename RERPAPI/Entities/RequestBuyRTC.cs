using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class RequestBuyRTC
{
    public int ID { get; set; }

    public int? ParentID { get; set; }

    public int? POKHDetailID { get; set; }

    public int? NguoiYeuCauID { get; set; }

    public int? PhongBanID { get; set; }

    /// <summary>
    /// Ngày nhận yêu cầu
    /// </summary>
    public DateTime? NgayNhanYeuCau { get; set; }

    public int? ProjectID { get; set; }

    public int? ProductID { get; set; }

    public int? PONCCID { get; set; }

    /// <summary>
    /// Ngày yêu cầu giao
    /// </summary>
    public DateTime? NgayYeuCauGiao { get; set; }

    public string? Unit { get; set; }

    public int? Qty { get; set; }

    public int? QtyReal { get; set; }

    /// <summary>
    /// Đơn Giá nhập
    /// </summary>
    public decimal? DonGiaNhap { get; set; }

    public string? ProductCode_ { get; set; }

    public string? ProductName_ { get; set; }

    public string? GuestCode_ { get; set; }

    public decimal? Vat { get; set; }

    public string? MaSPMua { get; set; }

    public string? TenSPMua { get; set; }

    /// <summary>
    /// Thành tiền
    /// </summary>
    public decimal? ThanhTien { get; set; }

    /// <summary>
    /// Nhà cung cấp
    /// </summary>
    public int? SupplierID { get; set; }

    /// <summary>
    /// Công nợ
    /// </summary>
    public string? CongNo { get; set; }

    /// <summary>
    /// hạn tt
    /// </summary>
    public DateTime? HanTT { get; set; }

    public bool? TinhTrangTT { get; set; }

    /// <summary>
    /// Ngày đặt hàng
    /// </summary>
    public DateTime? NgayDatHang { get; set; }

    /// <summary>
    /// Ngày dự kiến hàng về
    /// </summary>
    public DateTime? NgayDuKienVe { get; set; }

    /// <summary>
    /// Ngày về thực tế
    /// </summary>
    public DateTime? NgayVeThucTe { get; set; }

    /// <summary>
    /// values (0,chưa về) (1,đã về)
    /// </summary>
    public int? TinhTrangDonHang { get; set; }

    /// <summary>
    /// Hoá đơn/Số tờ khai
    /// </summary>
    public string? HoaDon { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? GhiChu { get; set; }

    public bool? IsApproved_Level1 { get; set; }

    public bool? IsApproved_Level2 { get; set; }

    public bool? IsApproved_Level3 { get; set; }

    public int? IsApproved_Level1_PeopleID { get; set; }

    public int? IsApproved_Level2_PeopleID { get; set; }

    public int? IsApproved_Level3_PeopleID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public decimal? PriceSale { get; set; }
}
