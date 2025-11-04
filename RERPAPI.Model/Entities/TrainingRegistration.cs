using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class TrainingRegistration
{
    public int ID { get; set; }

    /// <summary>
    /// Người đăng ký lấy từ bảng Employee
    /// </summary>
    public int? EmployeeID { get; set; }

    public string? Purpose { get; set; }

    /// <summary>
    /// 1: Đào tạo nội bộ; 2: Đào tạo ngoài
    /// </summary>
    public int? TrainingType { get; set; }

    /// <summary>
    /// 1: Có cấp chứng chỉ; 0: Không cấp
    /// </summary>
    public bool? IsCertification { get; set; }

    /// <summary>
    /// Đánh giá mức độ hoàn thành
    /// </summary>
    public string? CompletionAssessment { get; set; }

    public DateTime? DateRegister { get; set; }

    public DateTime? DateStart { get; set; }

    public DateTime? DateEnd { get; set; }

    /// <summary>
    /// Số buổi/khóa
    /// </summary>
    public int? SessionsPerCourse { get; set; }

    /// <summary>
    /// Thời lượng 1 buổi (Phút)
    /// </summary>
    public int? SessionDuration { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
