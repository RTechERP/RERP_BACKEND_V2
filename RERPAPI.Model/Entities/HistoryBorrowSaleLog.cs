using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class HistoryBorrowSaleLog
{
    /// <summary>
    /// ID lịch sử mượn sale
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID chi tiết phiếu xuất
    /// </summary>
    public int? BillExportDetailID { get; set; }

    /// <summary>
    /// Ngày gia hạn
    /// </summary>
    public DateTime? ExtendDate { get; set; }

    /// <summary>
    /// Ngày đăng ký gia hạn
    /// </summary>
    public DateTime? ExpectedReturnDate { get; set; }

    /// <summary>
    /// Trạng thái duyệt
    /// </summary>
    public bool? IsApproved { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người tạo
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật 
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Người cập nhật
    /// </summary>
    public string? UpdatedBy { get; set; }
}
