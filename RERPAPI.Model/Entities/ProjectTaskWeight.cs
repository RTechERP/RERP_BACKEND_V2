namespace RERPAPI.Model.Entities;

public partial class ProjectTaskWeight
{
    /// <summary>
    /// ID tự tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Trọng số của công việc
    /// </summary>
    public int? Weight { get; set; }

    /// <summary>
    /// Tên trọng số
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Yếu tố trọng số
    /// </summary>
    public decimal? Factor { get; set; }

    /// <summary>
    /// Mô tả trọng số
    /// </summary>
    public string? Description { get; set; }

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