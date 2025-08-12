using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ProjectWorkReportDTO
    {
        public int ProjectIDOld { get; set; }
        public int ProjectIDNew { get; set; }

        public List<int> reportIDs { get; set; }
    }

}
