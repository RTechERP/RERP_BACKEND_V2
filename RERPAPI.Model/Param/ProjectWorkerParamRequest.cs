using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class ProjectWorkerParamRequest
    {
        public int projectID { get; set; }
        public int projectWorkerTypeID { get; set; }
        public int IsApprovedTBP { get; set; }
        public int IsDeleted { get; set; }
        public string? KeyWord { get; set; }
        public int versionID { get; set; }
    }
}
