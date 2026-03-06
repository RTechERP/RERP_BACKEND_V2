using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Người liên hệ khi cần
/// </summary>
public partial class HRHiringCandidateInformationEmergencyContact
{
    public int ID { get; set; }

    /// <summary>
    /// ID hồ sơ ứng tuyển
    /// </summary>
    public int? HRRecruitmentApplicationFormID { get; set; }

    /// <summary>
    /// Số thứ tự hiển thị
    /// </summary>
    public int? STT { get; set; }

    /// <summary>
    /// Họ tên người liên hệ
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// Mối quan hệ với ứng viên
    /// </summary>
    public string? Relation { get; set; }

    /// <summary>
    /// Số điện thoại liên hệ
    /// </summary>
    public string? Tel { get; set; }

    /// <summary>
    /// Địa chỉ người liên hệ
    /// </summary>
    public string? Address { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
