using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Entities
{
    [Table("ProjectIssues")] // Đảm bảo trùng với tên bảng
    public class ProjectIssues
    {
        public int ID { get; set; }
        public int? STT { get; set; }    
        public int ProjectID { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public byte? Probability { get; set; }
        public byte? Impact { get; set; }
        public byte? Status { get; set; }

        public string? Solution { get; set; }
        public int? DepartmentID { get; set; }
        public string? MitigationPlan { get; set; }
        public string? FilePath { get; set; }

        public bool? IsApprove { get; set; }
        public string? Note { get; set; }

        public int? EmployeeID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
