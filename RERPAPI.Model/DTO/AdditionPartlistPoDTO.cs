using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class AdditionPartlistPoDTO
    {
        public List<ProjectPartlistDTO> ListItem { get; set; }
        public int VersionID { get; set; }
        public int ProjectTypeID { get; set; }
        public string ProjectTypeName { get; set; }
        public int ProjectSolutionID { get; set; }
        public int projectID { get; set; }
        public string ReasonProblem { get; set; }


    }
}
