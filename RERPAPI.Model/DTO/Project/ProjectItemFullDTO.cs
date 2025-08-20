using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.Project
{
    public class ProjectItemFullDTO
    {
        public ProjectItemProblem? projectItemProblem { get; set; }
        public List<ProjectItem>? projectItems { get; set; }
        public ProjectItemFile? ProjectItemFile { get; set; }
    }
}
