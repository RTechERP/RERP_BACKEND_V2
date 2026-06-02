namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng khai báo các key cấu hình nhận email
/// </summary>
public partial class ConfigNotificationKey
{
    /// <summary>
    /// ID bản ghi
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Tên key
    /// </summary>
    public string? KeyName { get; set; }

    /// <summary>
    /// Mô tả key
    /// </summary>
    public string? KeyContent { get; set; }

    /// <summary>
    /// Trạng thái xóa mềm: 0-Chưa xóa, 1-Đã xóa
    /// </summary>
    public bool? IsDeleted { get; set; }

    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Người cập nhật
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người tạo
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Mã Key
    /// </summary>
    public string? KeyCode { get; set; }
}