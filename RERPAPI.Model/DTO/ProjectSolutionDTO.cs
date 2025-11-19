using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ProjectSolutionDTO : ProjectSolution
    {
        [NotMapped]
        public int? ApproveStatus { get; set; } // 1: Báo giá, 2: PO
        [NotMapped]
        public bool? IsApproveAction { get; set; } // true: Duyệt, false: Hủy duyệt

        public List<ProjectSolutionFile>? projectSolutionFile { get; set; }
        public List<int>? deletedFileID { get; set; }
    }
}
