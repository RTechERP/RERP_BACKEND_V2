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
        public int ProjectID { get; set; }//3
        public string? Title { get; set; } //5
        public string? Description { get; set; }//5

        public byte? Probability { get; set; }//Khả năng xảy ra
        public byte? Impact { get; set; }
        public byte? Status { get; set; }//Trạng thái: Tiếp nhận đang xử lí, đã xử lí

        public string? Solution { get; set; }//8--Giair phap
        public int? DepartmentID { get; set; }//7
        public string? MitigationPlan { get; set; }// Kế hoạch thực hiện
        public string? FilePath { get; set; }//Đường dẫn file mô tả

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
