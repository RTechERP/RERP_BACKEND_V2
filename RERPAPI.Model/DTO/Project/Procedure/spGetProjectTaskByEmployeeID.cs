using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.Project.Procedure
{
    public class spGetProjectTaskByEmployeeID
    {
        public int ID { get; set; }

        public int? Status { get; set; }

        public string? STT { get; set; }

        /// <summary>
        /// Người phụ trách 
        /// </summary>
        public int? UserID { get; set; }

        public int? ProjectID { get; set; }

        /// <summary>
        /// Tương tự với Title
        /// </summary>
        public string? Mission { get; set; }

        public DateTime? PlanStartDate { get; set; }

        public DateTime? PlanEndDate { get; set; }

        public DateTime? ActualStartDate { get; set; }

        public DateTime? ActualEndDate { get; set; }

        public string? Note { get; set; }

        public decimal? TotalDayPlan { get; set; }

        public decimal? PercentItem { get; set; }

        public int? ParentID { get; set; }

        public decimal? TotalDayActual { get; set; }

        /// <summary>
        /// 1:Hạng mục quá hạn,
        /// 0: Hạng mục đúng hạn
        /// </summary>
        public int? ItemLate { get; set; }

        public decimal? TimeSpan { get; set; }

        /// <summary>
        /// Loại công việc
        /// </summary>
        public int? TypeProjectItem { get; set; }

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
        /// 0: Chờ duyệt kế hoạch; 1:Leader duyệt kế hoạch; 2:Chờ duyệt thực tế; 3:Leader Duyệt thực tế // 1: Chờ duyệt, 2: Đã hoàn thành, 3: Chưa hoàn thành, default: chưa duyệt
        /// </summary>
        public int? IsApproved { get; set; }

        public string? Code { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

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

        public bool? IsDeleted { get; set; }

        public string? Location { get; set; }

        public int? EmployeeCreateID { get; set; }

        public string? Description { get; set; }

        public bool? IsPersonalProject { get; set; }

        public bool? IsAdditional { get; set; }

        /// <summary>
        /// 1. Độ phức tạp của công việc (1 - 5)
        /// </summary>
        public int? TaskComplexity { get; set; }

        /// <summary>
        /// 1. Phần trăm chênh lệch quá hạn
        /// </summary>
        public decimal? PercentOverTime { get; set; }


        public string? ProjectCode { get; set; }
        public string? ProjectName { get; set; }

        public string? FullName { get; set; }


        public string? ParentCode { get; set; }
        public string? ParentTitle { get; set; }

        public int? SecondEmployeeID { get; set; }
        public string? SecondEmployeeFullName { get; set; }
        public int? SecondEmployeeType { get; set; }
        public string? ReviewDiscription { get; set; }
        public string? ProjectTaskTypeName { get; set; }
        public int? DepartmentAssignerID { get; set; }
        public int? DepartmentAssigneeID { get; set; }
        public string? DepartmentAssignerName { get; set; }
        public string? DepartmentAssigneeName { get; set; }
        public int? AsigneeEmployeeID { get; set; }
        public string? AsigneeEmployeeFullName { get; set; }
        /// <summary>
        /// Mã màu
        /// </summary>
        public string? ProjectTaskColor { get; set; }
        public decimal? TotalActualHours { get; set; }

        /// <summary>
        /// đánh giá mức độ hoàn thành công việc từ 1 -&gt; 5
        /// </summary>
        public int? ReviewCompletionRating { get; set; }
        /// <summary>
        /// Trạng thái làm công việc
        /// </summary>
        public bool? IsCheck { get; set; }
        /// <summary>
        /// Thời gian công việc phải hoàn thành
        /// </summary>
        public DateTime? Deadline { get; set; }
        public int? ProjectTaskTypeID { get; set; }
        public bool? ApprovalStatus { get; set; }

    }
}
