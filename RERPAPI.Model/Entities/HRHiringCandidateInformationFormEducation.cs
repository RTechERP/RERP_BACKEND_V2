using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Trình độ học vấn
/// </summary>
public partial class HRHiringCandidateInformationFormEducation
{
    /// <summary>
    /// ID thông tin học vấn ứng viên (Primary Key)
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID bảng HRHiringCandidateInformationForm
    /// </summary>
    public int? HRHiringCandidateInformationFormID { get; set; }

    /// <summary>
    /// Tên trường học
    /// </summary>
    public string? NameOfSchool { get; set; }

    /// <summary>
    /// Chuyên ngành đào tạo
    /// </summary>
    public string? Major { get; set; }

    /// <summary>
    /// Thời gian tốt nghiệp
    /// </summary>
    public string? GraduatedTime { get; set; }

    /// <summary>
    /// 1:Yếu; 2:Trung bình; 3:Khá; 4:Giỏi; 5:Xuất sắc
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
