using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class HistoryProductRTC
{
    /// <summary>
    /// ID lịch sử mượn
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID sản phẩm kho demo
    /// </summary>
    public int? ProductRTCID { get; set; }

    /// <summary>
    /// Ngày mượn thiết bị
    /// </summary>
    public DateTime? DateBorrow { get; set; }

    /// <summary>
    /// Ngày trả dự kiến
    /// </summary>
    public DateTime? DateReturnExpected { get; set; }

    /// <summary>
    /// ID người mượn
    /// </summary>
    public int? PeopleID { get; set; }

    /// <summary>
    /// Dự án sử dụng thiết bị
    /// </summary>
    public string? Project { get; set; }

    /// <summary>
    /// Ngày trả đồ
    /// </summary>
    public DateTime? DateReturn { get; set; }

    /// <summary>
    /// chú thích
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Trạng thái (0: Đã trả; 1: Đang mượn; 2: Thiết bị đã mất;3: Thiết bị hỏng;4: Đăng ký trả;5: Quá hạn;6: Sắp hết hạn;7: Đăng kí mượn; 8: Đăng ký gia hạn)
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// Số lượng mượn
    /// </summary>
    public decimal? NumberBorrow { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Xác nhận của admin
    /// </summary>
    public bool AdminConfirm { get; set; }

    /// <summary>
    /// ID phiếu xuất 
    /// </summary>
    public int? BillExportTechnicalID { get; set; }

    /// <summary>
    /// ID mã QR
    /// </summary>
    public int? ProductRTCQRCodeID { get; set; }

    /// <summary>
    /// ID kho
    /// </summary>
    public int? WarehouseID { get; set; }

    /// <summary>
    /// Mã QR
    /// </summary>
    public string? ProductRTCQRCode { get; set; }

    public bool? IsDelete { get; set; }

    /// <summary>
    /// ID vị trí sản phẩm
    /// </summary>
    public int? ProductLocationID { get; set; }

    /// <summary>
    /// Trạng thái người dùng modula (1: Hoàn thành thao tác lấy hàng; 2: Hoàn thành thao tác trả hàng)
    /// </summary>
    public int? StatusPerson { get; set; }

    /// <summary>
    /// ID nhóm cha
    /// </summary>
    public int? ParentID { get; set; }
}
