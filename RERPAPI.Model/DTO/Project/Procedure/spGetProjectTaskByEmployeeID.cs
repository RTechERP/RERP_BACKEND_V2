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

        public int? ProjectID { get; set; }
        public string? ProjectCode { get; set; }
        public string? ProjectName { get; set; }

        public int? ProjectTaskGroupID { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public int? EmployeeID { get; set; }
        public string? FullName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? DueDate { get; set; }

        public int? Priority { get; set; }
        public int? ReviewStatus { get; set; }

        public int? Status { get; set; }

        public int? OrderIndex { get; set; }

        public int? ParentID { get; set; }
        public string? ParentCode { get; set; }
        public string? ParentTitle { get; set; }

        public int? ProgressPercent { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public DateTime? PlanStartDate { get; set; }

        public DateTime? PlanEndDate { get; set; }
        public string? Code { get; set; }

        public bool? IsDeleted { get; set; }
        public int? SecondEmployeeID { get; set; }
        public string? SecondEmployeeFullName { get; set; }
        public int? SecondEmployeeType { get; set; }
        public bool? ApprovalStatus { get; set; } // 1: Pending, 2: Approved, 3: Rejected
        public string? ReviewDiscription { get; set; }
        public int? ProjectTaskTypeID { get; set; }
        public string? ProjectTaskTypeName { get; set; }
        public int MyProperty { get; set; }
        public int? DepartmentAssignerID { get; set; }
        public int? DepartmentAssigneeID { get; set; }
        public string? DepartmentAssignerName { get; set; }
        public string? DepartmentAssigneeName { get; set; }
        public bool? IsPersonalProject { get; set; }

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

    }
}
