using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng chi tiết chấm điểm 5S theo từng lỗi - phòng ban - kỳ đánh giá
/// </summary>
public partial class FiveSRatingDetail
{
    /// <summary>
    /// ID bản ghi
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// FK tới FiveSError
    /// </summary>
    public int? FiveSErrorID { get; set; }

    /// <summary>
    /// FK tới FiveSRating (kỳ chấm)
    /// </summary>
    public int? Rating5SID { get; set; }

    /// <summary>
    /// FK tới FiveSDepartment
    /// </summary>
    public int? FiveSDepartmentID { get; set; }

    /// <summary>
    /// ID người chấm 1
    /// </summary>
    public int? EmployeeRating1ID { get; set; }

    /// <summary>
    /// ID người chấm 2
    /// </summary>
    public int? EmployeeRating2ID { get; set; }

    /// <summary>
    /// ID loại cộng/trừ điểm
    /// </summary>
    public int? FiveSBonusMinusID { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Người cập nhật
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Người tạo
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Cờ xóa mềm
    /// </summary>
    public bool? IsDeleted { get; set; }

    public int? FiveSRatingDetailID { get; set; }

    public string? Note { get; set; }

    public int? FiveSRatingTicketID { get; set; }
}
