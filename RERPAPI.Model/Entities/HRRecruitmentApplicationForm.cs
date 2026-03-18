using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Tờ khai thông tin ứng viên
/// </summary>
public partial class HRRecruitmentApplicationForm
{
    public int ID { get; set; }

    /// <summary>
    /// ID ứng viên trong hệ thống tuyển dụng
    /// </summary>
    public int? HRRecruitmentCandidateID { get; set; }

    /// <summary>
    /// Tên file ảnh 3x4 của ứng viên
    /// </summary>
    public string? Image3x4 { get; set; }

    /// <summary>
    /// Các hoạt động, thành tích xã hội khác
    /// </summary>
    public string? OtherActivities { get; set; }

    /// <summary>
    /// Đặc điểm cá nhân và kinh nghiệm phù hợp với vị trí ứng tuyển
    /// </summary>
    public string? Experiences { get; set; }

    /// <summary>
    /// Lý do nộp đơn dự tuyển vào Công ty
    /// </summary>
    public string? ReasonApplication { get; set; }

    /// <summary>
    /// Mức lương tối thiểu mong muốn
    /// </summary>
    public decimal? AcceptedSalary { get; set; }

    /// <summary>
    /// Ngày có thể bắt đầu làm việc
    /// </summary>
    public DateTime? DateOfStart { get; set; }

    /// <summary>
    /// 0: Chưa xác nhận, 1: Đã xác nhận
    /// </summary>
    public bool? IsSignature { get; set; }

    /// <summary>
    /// Ngày khai báo / ký xác nhận
    /// </summary>
    public DateTime? DateSign { get; set; }

    /// <summary>
    /// Người tạo bản ghi
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày tạo bản ghi
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người cập nhật gần nhất
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật gần nhất
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// 0: Chưa xóa, 1: Đã xóa
    /// </summary>
    public bool? IsDeleted { get; set; }

    /// <summary>
    /// Họ và tên ứng viên
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// 1: Nam, 2: Nữ
    /// </summary>
    public int? Gender { get; set; }

    /// <summary>
    /// Ngày sinh
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Nơi sinh
    /// </summary>
    public string? PlaceOfBirth { get; set; }

    /// <summary>
    /// Dân tộc
    /// </summary>
    public string? Ethnic { get; set; }

    /// <summary>
    /// Tôn giáo
    /// </summary>
    public string? Religion { get; set; }

    /// <summary>
    /// Địa chỉ thường trú
    /// </summary>
    public string? PermanentResidence { get; set; }

    /// <summary>
    /// Địa chỉ hiện nay
    /// </summary>
    public string? CurrentAddress { get; set; }

    /// <summary>
    /// Số CMND/CCCD/ID
    /// </summary>
    public string? NumberCCCD { get; set; }

    /// <summary>
    /// Ngày cấp CMND/CCCD
    /// </summary>
    public DateTime? IssuedOn { get; set; }

    /// <summary>
    /// Nơi cấp CMND/CCCD
    /// </summary>
    public string? IssuedBy { get; set; }

    /// <summary>
    /// Số điện thoại bàn
    /// </summary>
    public string? Tel { get; set; }

    /// <summary>
    /// Số điện thoại di động
    /// </summary>
    public string? Mobile { get; set; }

    /// <summary>
    /// Email liên hệ
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Sở thích cá nhân
    /// </summary>
    public string? Hobbies { get; set; }

    /// <summary>
    /// Chiều cao (cm)
    /// </summary>
    public decimal? Height { get; set; }

    /// <summary>
    /// Cân nặng (kg)
    /// </summary>
    public decimal? Weight { get; set; }

    /// <summary>
    /// 1: Độc thân, 2: Đã lập gia đình, 3: Ly hôn
    /// </summary>
    public int? MaritalStatus { get; set; }

    /// <summary>
    /// 0: Không, 1: Có - Bị thương tật hoặc bệnh nặng
    /// </summary>
    public bool? InjuriesOrSeriousIll { get; set; }

    /// <summary>
    /// Nếu có thương tật/bệnh nặng thì mô tả chi tiết
    /// </summary>
    public string? IfYesSpecify { get; set; }

    /// <summary>
    /// 0: Không, 1: Có - Hiện tại có mang thai (Nữ)
    /// </summary>
    public bool? CurrentlyPregnant { get; set; }

    /// <summary>
    /// 0: Không, 1: Có - Dự kiến mang thai trong 6 tháng tới (Nữ)
    /// </summary>
    public bool? IsPlanPregnant { get; set; }

    public int? ChucVuHDID { get; set; }

    public string? FileName { get; set; }

    public bool? HasRelativeOrFriendInCompany { get; set; }

    public string? RelativeInfo { get; set; }

    public bool? HasSocialInsurance { get; set; }

    public string? BHXH { get; set; }

    public bool? HasTaxCode { get; set; }

    public string? TaxCode { get; set; }

    /// <summary>
    /// 0.Chưa hoàn thành 1.Hoàn thành
    /// </summary>
    public bool? IsComplete { get; set; }
}
