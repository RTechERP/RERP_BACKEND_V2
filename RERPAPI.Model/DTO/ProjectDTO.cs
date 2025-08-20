using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ProjectDTO 
    {
        public int projectStatusOld { get; set; }
       public Entities.Project project { get; set; } 
        public ProjectStatusLog projectStatusLog { get; set; } 
        public FollowProjectBase followProjectBase { get; set; }
        public List<prjTypeLinkDTO> projectTypeLinks { get; set; }
        public List<ProjectUser> projectUsers { get; set; } 
        public List<ProjectPriorityLink> listPriorities { get; set; } 

    }

    public class prjTypeLinkDTO : ProjectTypeLink
    {
        public int EmployeeID { get; set; }
        public int ProjectTypeLinkID { get; set; }
    }
}
