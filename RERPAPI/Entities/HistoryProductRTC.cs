using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class HistoryProductRTC
{
    public int ID { get; set; }

    public int? ProductRTCID { get; set; }

    /// <summary>
    /// Ngày mượn thiết bị
    /// </summary>
    public DateTime? DateBorrow { get; set; }

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
    /// 0: Đã trả; 1: Đang mượn; 2: Thiết bị đã mất;3: Thiết bị hỏng;4: Đăng ký trả;5: Quá hạn;6: Sắp hết hạn;7: Đăng kí mượn; 8: Đăng ký gia hạn
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

    public bool AdminConfirm { get; set; }

    public int? BillExportTechnicalID { get; set; }

    public int? ProductRTCQRCodeID { get; set; }

    public int? WarehouseID { get; set; }

    public string? ProductRTCQRCode { get; set; }

    public bool? IsDelete { get; set; }

    public int? ProductLocationID { get; set; }

    /// <summary>
    /// 1: Hoàn thành thao tác lấy hàng; 2: Hoàn thành thao tác trả hàng
    /// </summary>
    public int? StatusPerson { get; set; }
}
