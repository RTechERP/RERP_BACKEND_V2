using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class RequestBuy
{
    public int ID { get; set; }

    /// <summary>
    /// Mã yêu cầu hỏi giá
    /// </summary>
    public string? RequestBuyCode { get; set; }

    /// <summary>
    /// Khách hàng
    /// </summary>
    public int? CustomerID { get; set; }

    /// <summary>
    /// Dự án
    /// </summary>
    public int? ProjectID { get; set; }

    /// <summary>
    /// Phòng ban
    /// </summary>
    public int? DepartmentID { get; set; }

    /// <summary>
    /// Người yêu cầu
    /// </summary>
    public int? RequestPersonID { get; set; }

    /// <summary>
    /// Trạng thái YC: 0:Chưa thực hiện, 1: Đang thực hiện, 2: Đã hoàn thành
    /// </summary>
    public int? RequestBuyStatus { get; set; }

    /// <summary>
    /// Loại hỏi giá: 1: thương mại, 2: sản xuất
    /// </summary>
    public int? RequestType { get; set; }

    /// <summary>
    /// Hạn hỏi giá
    /// </summary>
    public DateTime? DeadLine { get; set; }

    /// <summary>
    /// Mục đích hỏi giá chung cho dự án, khách hàng nào, ai
    /// </summary>
    public string? Purpose { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

    public int? YearCreate { get; set; }

    public int? SortOrder { get; set; }

    /// <summary>
    /// Chi phí vận chuyển
    /// </summary>
    public decimal? DeliveryCost { get; set; }

    /// <summary>
    /// Chi phí hải quan
    /// </summary>
    public decimal? CustomsCost { get; set; }

    /// <summary>
    /// Chi phí ngân hàng
    /// </summary>
    public decimal? BankCost { get; set; }

    /// <summary>
    /// Là hàng nhập khẩu(vd: cognex)
    /// </summary>
    public bool? IsImport { get; set; }

    public decimal? QtySet { get; set; }

    public decimal? Price { get; set; }

    public decimal? TotalPrice { get; set; }

    /// <summary>
    /// Đã được duyệt hay chưa
    /// </summary>
    public bool? IsApproved { get; set; }

    /// <summary>
    /// Người duyệt
    /// </summary>
    public int? ApprovedID { get; set; }

    /// <summary>
    /// Ngày duyệt
    /// </summary>
    public DateTime? ApprovedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? SupplierID { get; set; }
}
