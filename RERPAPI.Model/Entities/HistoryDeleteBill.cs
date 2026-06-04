using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class HistoryDeleteBill
{
    /// <summary>
    /// ID lịch sử xóa phiếu
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID phiếu
    /// </summary>
    public int? BillID { get; set; }

    /// <summary>
    /// ID người xóa
    /// </summary>
    public int? UserID { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Ngày xóa
    /// </summary>
    public DateTime? DeleteDate { get; set; }

    /// <summary>
    /// Tên
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Loại phiếu
    /// </summary>
    public string? TypeBill { get; set; }
}
