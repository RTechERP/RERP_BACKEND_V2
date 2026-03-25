using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class spGetProjectTaskTreeParam
    {
        public int ID { get; set; }

        public int? ProjectID { get; set; }

        public int? ProjectTaskGroupID { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public int? EmployeeID { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? PlanStartDate { get; set; }

        public DateTime? PlanEndDate { get; set; }

        public int? Priority { get; set; }

        public int? OrderIndex { get; set; }

        public int? ParentID { get; set; }

        public int? ProgressPercent { get; set; }

        /// <summary>
        /// 1: Chờ duyệt, 2: Đã hoàn thành, 3: Chưa hoàn thành, default: chưa duyệt
        /// </summary>
        public int? ReviewStatus { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string? UpdatedBy { get; set; }

        public bool? IsDeleted { get; set; }

        /// <summary>
        /// 1: Chưa làm, 2: Đang làm, 3: Hoàn thành, 4: Pending
        /// </summary>
        public int? Status { get; set; }

        public string? Code { get; set; }

        public int? EmployeeCreateID { get; set; }

        /// <summary>
        /// 1: Công việc cá nhân
        /// </summary>
        public bool? IsPersonalProject { get; set; }

        public int? ProjectTaskTypeID { get; set; }

        /// <summary>
        /// 1. Có công việc phát sinh hay không
        /// </summary>
        public bool? IsAdditional { get; set; }

        /// <summary>
        /// 1. Độ phức tạp của công việc
        /// </summary>
        public int? TaskComplexity { get; set; }

        /// <summary>
        /// 1. Phần trăm chênh lệch quá hạn
        /// </summary>
        public decimal? PercentOverTime { get; set; }

        public string? ProjectCode { get; set; }
        public string? ProjectName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? FullName { get; set; }
        public int? DepartmentID { get; set; }

    }
}
