using RERPAPI.Model.DTO.Project;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ProjectItemDTO
    {
        public List<ProjectItem> projectItem { get; set; }
        public int ProjectID { get; set; }
        public List<int>? DeletedIdsprojectItem {  get; set; }
    }
}
