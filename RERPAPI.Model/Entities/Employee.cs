using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class Employee
{
    public int ID { get; set; }

    public int? UserID { get; set; }

    public string? Code { get; set; }

    public string? FullName { get; set; }

    public DateTime? BirthOfDate { get; set; }

    public int? Sex { get; set; }

    public string? Qualifications { get; set; }

    public string? BankAccount { get; set; }

    public string? BHYT { get; set; }

    public string? MST { get; set; }

    public string? BHXH { get; set; }

    public string? CMTND { get; set; }

    public string? JobDescription { get; set; }

    public bool? NhanVienTuVan { get; set; }

    public string? Telephone { get; set; }

    public string? HandPhone { get; set; }

    public string? HomeAddress { get; set; }

    public string? Resident { get; set; }

    public string? PostalCode { get; set; }

    public int? DepartmentID { get; set; }

    public int? Status { get; set; }

    public string? Communication { get; set; }

    public DateTime? PassExpireDate { get; set; }

    public bool? IsCashier { get; set; }

    public int? CashierNo { get; set; }

    public string? EmailCom { get; set; }

    public string? Email { get; set; }

    public DateTime? StartWorking { get; set; }

    public int? UserGroupID { get; set; }

    public int? UserGroupSXID { get; set; }

    public int? MainViewID { get; set; }

    public string? Position { get; set; }

    public bool? IsSetupFunction { get; set; }

    public string? ImagePath { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? isAdminSale { get; set; }

    public int? RoleID { get; set; }

    public int? TeamID { get; set; }

    public int? Leader { get; set; }

    public string? IDChamCongCu { get; set; }

    public string? IDChamCongMoi { get; set; }

    public string? AnhCBNV { get; set; }

    public int? ChucVuHDID { get; set; }

    public int? ChuVuID { get; set; }

    public string? DvBHXH { get; set; }

    public string? DiaDiemLamViec { get; set; }

    public string? NoiSinh { get; set; }

    public int? GioiTinh { get; set; }

    public string? DanToc { get; set; }

    public string? TonGiao { get; set; }

    public string? QuocTich { get; set; }

    public int? TinhTrangHonNhanID { get; set; }

    public string? SoCMTND { get; set; }

    public DateTime? NgayCap { get; set; }

    public string? NoiCap { get; set; }

    public string? DcThuongTru { get; set; }

    public string? DcTamTru { get; set; }

    public string? SDTCaNhan { get; set; }

    public string? EmailCaNhan { get; set; }

    public string? SDTCongTy { get; set; }

    public string? EmailCongTy { get; set; }

    public string? NguoiLienHeKhiCan { get; set; }

    public string? MoiQuanHe { get; set; }

    public string? SDTNguoiThan { get; set; }

    public int? LoaiHDLDID { get; set; }

    /// <summary>
    /// 1: 
    /// </summary>
    public string? TinhTrangKyHD { get; set; }

    public DateTime? NgayBatDauHD { get; set; }

    public DateTime? NgayKetThucHD { get; set; }

    public string? SoHD { get; set; }

    public string? SoSoBHXH { get; set; }

    public int? NguoiGiuSoBHXH { get; set; }

    public DateTime? NgayBatDauBHXHCty { get; set; }

    public decimal? MucDongBHXHHienTai { get; set; }

    public decimal? LuongThuViec { get; set; }

    public decimal? LuongCoBan { get; set; }

    public decimal? AnCa { get; set; }

    public decimal? XangXe { get; set; }

    public decimal? DienThoai { get; set; }

    public decimal? NhaO { get; set; }

    public decimal? TrangPhuc { get; set; }

    public decimal? ChuyenCan { get; set; }

    public decimal? Khac { get; set; }

    public decimal? TongPhuCap { get; set; }

    public decimal? TongLuong { get; set; }

    public decimal? GiamTruBanThan { get; set; }

    public int? SoNguoiPT { get; set; }

    public decimal? TongTien { get; set; }

    public string? STKChuyenLuong { get; set; }

    public bool? SYLL { get; set; }

    public bool? GiayKS { get; set; }

    public bool? CMNDorCCCD { get; set; }

    public bool? SoHK { get; set; }

    public bool? XNNS { get; set; }

    public bool? BangCap { get; set; }

    public bool? CV { get; set; }

    public bool? DXV { get; set; }

    public bool? CamKetTs { get; set; }

    public bool? ToTrinhTD { get; set; }

    public bool? ThuMoiNhanViec { get; set; }

    public bool? QDTD { get; set; }

    public bool? HDTV { get; set; }

    public bool? DGTV { get; set; }

    public bool? HDLDXDTH { get; set; }

    public bool? DGChuyenHD { get; set; }

    public bool? HDLDKXDTH { get; set; }

    public bool? TinhTrangCapDongPhuc { get; set; }

    public bool? GiayKSK { get; set; }

    public DateTime? NgayBatDauThuViec { get; set; }

    public DateTime? NgayKetThucThuViec { get; set; }

    public string? SoHDTV { get; set; }

    public DateTime? NgayBatDauHDXDTH { get; set; }

    public DateTime? NgayKetThucHDXDTH { get; set; }

    public string? SoHDXDTH { get; set; }

    public DateTime? NgayHieuLucHDKXDTH { get; set; }

    public string? SoHDKXDTH { get; set; }

    public DateTime? NgayBatDauBHXH { get; set; }

    public DateTime? NgayKetThucBHXH { get; set; }

    public string? SoNhaDcThuongTru { get; set; }

    public string? DuongDcThuongTru { get; set; }

    public string? PhuongDcThuongTru { get; set; }

    public string? QuanDcThuongTru { get; set; }

    public string? TinhDcThuongTru { get; set; }

    public string? SoNhaDcTamTru { get; set; }

    public string? DuongDcTamTru { get; set; }

    public string? PhuongDcTamTru { get; set; }

    public string? QuanDcTamTru { get; set; }

    public string? TinhDcTamTru { get; set; }

    public bool? HDLDXDTHYear { get; set; }

    public bool? DGChuyenHDYear { get; set; }

    public DateTime? EndWorking { get; set; }

    public string? ReasonDeleted { get; set; }

    public string? CodeOld { get; set; }

    public int? ProjectTypeID { get; set; }

    public int? EmployeeTeamID { get; set; }

    public bool? GiayXacNhanCuTru { get; set; }

    public string? UserZaloID { get; set; }

    public int? TaxCompanyID { get; set; }
}
