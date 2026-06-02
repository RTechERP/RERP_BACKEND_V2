namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu lịch sử thay đổi/tracking tài sản
/// </summary>
public partial class AssetLog
{
    /// <summary>
    /// Khóa chính, tự tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID tài sản
    /// </summary>
    public int? AssetID { get; set; }

    /// <summary>
    /// ID nhân viên liên quan
    /// </summary>
    public int? EmployeeID { get; set; }

    /// <summary>
    /// Thời gian ghi nhận log
    /// </summary>
    public DateTime? DateLog { get; set; }

    /// <summary>
    /// Nội dung log (mô tả thay đổi)
    /// </summary>
    public string? LogContent { get; set; }

    /// <summary>
    /// Người tạo bản ghi
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Thời gian tạo bản ghi
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người cập nhật
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Thời gian cập nhật
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Loại log (ví dụ: bàn giao, thu hồi, sửa chữa...)
    /// </summary>
    public string? TypeLog { get; set; }
}