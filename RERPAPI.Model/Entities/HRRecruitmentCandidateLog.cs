using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class HRRecruitmentCandidateLog
{
    public int ID { get; set; }

    /// <summary>
    /// 1 Gửi thư mời;
    /// 2 Xác nhận phỏng vấn;
    /// 3 Đã phỏng vấn;
    /// 4 Kết quả phỏng vấn;
    /// 5 Trình phê duyệt;
    /// 6 Gửi thư mời nhận việc;
    /// 7 Xác nhận thư mời;
    /// 8 Nhận việc;
    /// </summary>
    public int? ApprovedStep { get; set; }

    public bool? IsApproved { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? Note { get; set; }

    public int? HRRecruitmentCandidateID { get; set; }
}
