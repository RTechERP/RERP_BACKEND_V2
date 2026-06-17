using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Nguồn thông tin tuyển dụng mà ứng viên biết đến vị trí tuyển dụng
/// </summary>
public partial class HRHiringCandidateInformationFormRecruitmentInfo
{
    /// <summary>
    /// ID thông tin nguồn tuyển dụng ứng viên (Primary Key)
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID bảng HRRecruitmentApplicationForm
    /// </summary>
    public int? HRRecruitmentApplicationFormID { get; set; }

    /// <summary>
    /// Ứng viên biết thông tin tuyển dụng qua website tuyển dụng (VietnamWorks, TopCV, v.v.)
    /// </summary>
    public bool? JobWebsites { get; set; }

    /// <summary>
    /// Ứng viên biết thông tin tuyển dụng qua báo chí
    /// </summary>
    public bool? Newspapers { get; set; }

    /// <summary>
    /// Ứng viên biết thông tin tuyển dụng qua mạng xã hội (Facebook, LinkedIn, v.v.)
    /// </summary>
    public bool? SocialNetwork { get; set; }

    /// <summary>
    /// Ứng viên được giới thiệu qua headhunter / công ty tuyển dụng
    /// </summary>
    public bool? Headhunters { get; set; }

    /// <summary>
    /// Ứng viên biết thông tin tuyển dụng qua người thân / bạn bè
    /// </summary>
    public bool? Relatives { get; set; }

    /// <summary>
    /// Nguồn tuyển dụng khác
    /// </summary>
    public bool? Others { get; set; }

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
