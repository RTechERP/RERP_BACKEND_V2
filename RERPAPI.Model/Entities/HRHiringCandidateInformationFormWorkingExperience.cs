using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Kinh nghiệm làm việc của ứng viên
/// </summary>
public partial class HRHiringCandidateInformationFormWorkingExperience
{
    /// <summary>
    /// ID kinh nghiệm làm việc của ứng viên (Primary Key)
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID bảng HRRecruitmentApplicationForm
    /// </summary>
    public int? HRRecruitmentApplicationFormID { get; set; }

    /// <summary>
    /// Tên công ty đã từng làm việc
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// Tên vị trí / chức danh công việc
    /// </summary>
    public string? PositionName { get; set; }

    /// <summary>
    /// Ngày bắt đầu làm việc
    /// </summary>
    public DateTime? DateStart { get; set; }

    /// <summary>
    /// Ngày kết thúc làm việc
    /// </summary>
    public DateTime? DateEnd { get; set; }

    /// <summary>
    /// Tên người quản lý trực tiếp
    /// </summary>
    public string? Leader { get; set; }

    /// <summary>
    /// Nhiệm vụ / công việc chính
    /// </summary>
    public string? Mission { get; set; }

    /// <summary>
    /// Thành tích đạt được
    /// </summary>
    public string? Achievement { get; set; }

    /// <summary>
    /// Mức lương tại công ty đó
    /// </summary>
    public decimal? Salary { get; set; }

    /// <summary>
    /// 1: Hiện còn làm; 2: Đã nghỉ
    /// </summary>
    public int? WorkingStatus { get; set; }

    /// <summary>
    /// Lý do nghỉ việc
    /// </summary>
    public string? ReasonQuit { get; set; }

    /// <summary>
    /// Người tạo bản ghi
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày tạo bản ghi
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người cập nhật bản ghi
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật bản ghi
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Trạng thái xóa mềm (0: Chưa xóa, 1: Đã xóa)
    /// </summary>
    public bool? IsDeleted { get; set; }
}
