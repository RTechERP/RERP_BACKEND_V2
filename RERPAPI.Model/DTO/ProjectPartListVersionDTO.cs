using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ProjectPartListVersionDTO: ProjectPartListVersion
    {
        [NotMapped]
        public bool? IsApproveAction { get; set; }// true:duyệt; false:hủy duyệt
    }
}
