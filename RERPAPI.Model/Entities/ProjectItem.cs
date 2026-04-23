using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectItem
{
    /// <summary>
    /// ID tự tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Trạng thái công việc : 0: Chưa làm , 1: Đang làm, 2: Hoàn thành, 3: Pending
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// Số thứ tự hạng mục công việc
    /// </summary>
    public string? STT { get; set; }

    /// <summary>
    /// userID của người nhận việc
    /// </summary>
    public int? UserID { get; set; }

    /// <summary>
    /// Dự án của công việc
    /// </summary>
    public int? ProjectID { get; set; }

    /// <summary>
    /// Title của công việc
    /// </summary>
    public string? Mission { get; set; }

    /// <summary>
    /// Ngày bắt đầu kế hoạch 
    /// </summary>
    public DateTime? PlanStartDate { get; set; }

    /// <summary>
    /// Ngày kết thúc kế hoạch
    /// </summary>
    public DateTime? PlanEndDate { get; set; }

    /// <summary>
    /// Ngày bắt đầu thực tế
    /// </summary>
    public DateTime? ActualStartDate { get; set; }

    /// <summary>
    /// Ngày kết thúc thực tế
    /// </summary>
    public DateTime? ActualEndDate { get; set; }

    public string? Note { get; set; }

    /// <summary>
    /// Tổng ngày kế hoạch
    /// </summary>
    public decimal? TotalDayPlan { get; set; }

    public decimal? PercentItem { get; set; }

    /// <summary>
    /// ID của công việc cha
    /// </summary>
    public int? ParentID { get; set; }

    /// <summary>
    /// Tổng số ngày hoàn thành thực tế
    /// </summary>
    public decimal? TotalDayActual { get; set; }

    /// <summary>
    /// 1:Hạng mục quá hạn,
    /// 0: Hạng mục đúng hạn
    /// </summary>
    public int? ItemLate { get; set; }

    public decimal? TimeSpan { get; set; }

    /// <summary>
    /// Loại hạng mục công việc
    /// </summary>
    public int? TypeProjectItem { get; set; }

    /// <summary>
    /// Phần trăm hoàn thành 
    /// </summary>
    public decimal? PercentageActual { get; set; }

    /// <summary>
    /// Người giao công việc
    /// </summary>
    public int? EmployeeIDRequest { get; set; }

    /// <summary>
    /// Ngày update kết thúc thực tế
    /// </summary>
    public DateTime? UpdatedDateActual { get; set; }

    /// <summary>
    /// 0: Chờ duyệt kế hoạch; 1:Leader duyệt kế hoạch; 2:Chờ duyệt thực tế; 3:Leader Duyệt thực tế
    /// </summary>
    public int? IsApproved { get; set; }

    /// <summary>
    /// Mã công việc
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Thời gian tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người tạo
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Người cập nhật
    /// </summary>
    public string? UpdatedBy { get; set; }

    public bool? IsUpdateLate { get; set; }

    public string? ReasonLate { get; set; }

    public DateTime? UpdatedDateReasonLate { get; set; }

    public bool? IsApprovedLate { get; set; }

    /// <summary>
    /// lưu ID người yêu cầu lấy từ bảng Employee, nếu  = 0 thì là tên KH
    /// </summary>
    public int? EmployeeRequestID { get; set; }

    /// <summary>
    /// lưu tên người yêu cầu lấy từ bảng Employee, nếu  = 0 thì là tên KH
    /// </summary>
    public string? EmployeeRequestName { get; set; }

    /// <summary>
    /// Trạng thái xóa
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Địa điểm làm việc
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// ID của nhân viên tạo bản ghi
    /// </summary>
    public int? EmployeeCreateID { get; set; }

    /// <summary>
    /// Mô tả chi tiết công việc
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Trạng thái đánh giá xem đây có phải là công việc cá nhân hay không
    /// </summary>
    public bool? IsPersonalProject { get; set; }

    /// <summary>
    /// Trạng thái đánh giá xem có phát sinh không 
    /// </summary>
    public bool? IsAdditional { get; set; }

    /// <summary>
    /// Độ phức tạp của công việc (1 - 5)
    /// </summary>
    public int? TaskComplexity { get; set; }

    /// <summary>
    /// Phần trăm chênh lệch quá hạn
    /// </summary>
    public decimal? PercentOverTime { get; set; }

    /// <summary>
    /// Nguyên nhân/Phương án sử lý (sử dụng riêng với loại công việc là BUG)
    /// </summary>
    public string? DescriptionSolution { get; set; }

    /// <summary>
    /// Thời gian công việc phải hoàn thành
    /// </summary>
    public DateTime? Deadline { get; set; }

    /// <summary>
    /// Loại công việc
    /// </summary>
    public int? ProjectTaskTypeID { get; set; }

    /// <summary>
    /// Kết quả công việc
    /// </summary>
    public string? ProjectTaskResult { get; set; }

    /// <summary>
    /// Độ ưu tiên dự án 1: Thấp, 2. Trung bình, 3. Cao, 4. Khẩn cấp
    /// </summary>
    public int? Priority { get; set; }

    /// <summary>
    /// Thời gian dự kiến (h)
    /// </summary>
    public decimal? EstimatedTime { get; set; }

    /// <summary>
    /// 1: Cần phê duyệt, 0: Không cần phê duyệt
    /// </summary>
    public bool? NeedApprove { get; set; }
}
