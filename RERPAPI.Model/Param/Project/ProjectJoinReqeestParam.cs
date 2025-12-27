using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param.Project
{
    public class ProjectJoinReqeestParam
    {
        public DateTime? dateStart { get; set; }
        public DateTime? dateEnd { get; set; }
        public int? employeeID { get; set; }
    }
}
