namespace RERPAPI.Model.Entities;

public partial class ProjectTaskStatus
{
    /// <summary>
    /// ID tự tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Vị trí của trạng thái
    /// </summary>
    public int? No { get; set; }

    /// <summary>
    /// Tiêu đề trạng thái
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Mô tả trạng thái
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Loại trạng thái
    /// </summary>
    public int? Type { get; set; }

    /// <summary>
    /// Màu nền của trạng thái
    /// </summary>
    public string? ColorBackground { get; set; }

    /// <summary>
    /// Màu chữ của trạng thái
    /// </summary>
    public string? ColorFont { get; set; }

    /// <summary>
    /// Ngày tạo bản ghi
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người tạo bản ghi
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật bản ghi
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Ngươì cập nhật bản ghi
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Trạng thái khóa mềm
    /// </summary>
    public bool? IsDeleted { get; set; }
}