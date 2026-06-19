using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu lịch sử các thao tác đối với từng vật tư (ProjectPartList)
/// </summary>
public partial class ProjectPartListLog
{
    /// <summary>
    /// Khóa chính tự tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Mã định danh vật tư (ProjectPartList.ID)
    /// </summary>
    public int? ProjectPartListID { get; set; }

    /// <summary>
    /// Loại thao tác (Thêm mới, Cập nhật, Xóa mềm, Duyệt TBP, YC báo giá, YC mua hàng,...)
    /// </summary>
    public string? ActionType { get; set; }

    /// <summary>
    /// Nội dung chi tiết log (chứa diff giá trị cũ -&gt; giá trị mới)
    /// </summary>
    public string? ContentLog { get; set; }

    /// <summary>
    /// Tên tài khoản đăng nhập của người thực hiện
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Mã nhân viên (EmployeeID) thực hiện thao tác
    /// </summary>
    public int? CreatedByEmployeeID { get; set; }

    /// <summary>
    /// Thời gian tạo bản ghi log
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Trạng thái xóa mềm (0: Đang hoạt động, 1: Đã xóa)
    /// </summary>
    public bool? IsDeleted { get; set; }
}
