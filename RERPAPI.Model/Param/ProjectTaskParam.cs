using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class ProjectTaskParam
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

        public int? Status { get; set; }

        public int? OrderIndex { get; set; }

        public int? ParentID { get; set; }

        public bool? IsDeleted { get; set; }

        public bool? IsPersonalProject { get; set; }
        public int? ProjectTaskTypeID { get; set; }

        public bool? IsAdditional { get; set; }
        public int? TaskComplexity { get; set; }


        public List<int>? Employee { get; set; }
        public List<int>? EmployeeRelate { get; set; }
        public List<int>? Links { get; set; }
        public List<int>? Files { get; set; }
        public List<int>? ProjectTaskChecklists { get; set; } 
        public List<int>? ProjectTaskAdditionals { get; set; } 
    }
}
