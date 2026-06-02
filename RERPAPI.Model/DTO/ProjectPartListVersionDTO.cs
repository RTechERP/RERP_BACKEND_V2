using RERPAPI.Model.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace RERPAPI.Model.DTO
{
    public class ProjectPartListVersionDTO : ProjectPartListVersion
    {
        [NotMapped]
        public bool? IsApproveAction { get; set; }// true:duyệt; false:hủy duyệt
    }
}