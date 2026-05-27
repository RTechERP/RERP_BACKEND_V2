using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param.KPITech
{
    public class RecalcKPIBatchRequestParam
    {
        public int KpiSessionID { get; set; }
        public int DepartmentID { get; set; } // 0 = All
        public int TeamID { get; set; }       // 0 = All
    }
}
