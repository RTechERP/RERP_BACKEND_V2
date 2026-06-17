using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Chứng chỉ khác của ứng viên
/// </summary>
public partial class HRHiringCandidateInformationFormOtherCertificate
{
    /// <summary>
    /// ID chứng chỉ khác của ứng viên (Primary Key)
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID bảng HRRecruitmentApplicationForm
    /// </summary>
    public int? HRRecruitmentApplicationFormID { get; set; }

    /// <summary>
    /// Ngày cấp chứng chỉ
    /// </summary>
    public DateTime? DateOfIssue { get; set; }

    /// <summary>
    /// Tên chứng chỉ
    /// </summary>
    public string? Certificates { get; set; }

    /// <summary>
    /// Đơn vị cấp chứng chỉ
    /// </summary>
    public string? IssuedBy { get; set; }

    /// <summary>
    /// Mức độ / xếp loại chứng chỉ (1:Yếu; 2:Trung bình; 3:Khá; 4:Giỏi; 5:Xuất sắc)
    /// </summary>
    public int? QualificationLevel { get; set; }

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
