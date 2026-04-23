using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class ProjectTaskChildParam
    {
        public int ID { get; set; }

        public int? ProjectID { get; set; }

        public string? Mission { get; set; }

        public DateTime? PlanStartDate { get; set; }

        public DateTime? PlanEndDate { get; set; }
        public int? ParentID { get; set; }
        /// <summary>
        /// Loại hạng mục công việc
        /// </summary>
        public int? TypeProjectItem { get; set; }

        /// <summary>
        /// Người giao công việc
        /// </summary>
        public int? EmployeeIDRequest { get; set; }

        /// <summary>
        /// ID của nhân viên tạo bản ghi
        /// </summary>
        public int? EmployeeAssigneeID { get; set; }


        /// <summary>
        /// Độ phức tạp của công việc (1 - 5)
        /// </summary>
        public int? TaskComplexity { get; set; }

        /// <summary>
        /// Loại công việc
        /// </summary>
        public int? ProjectTaskTypeID { get; set; }

        public bool? IsDeletedFromParent { get; set; }

    }
}
