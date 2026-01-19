using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param.KPITech
{
    public class LoadDataTeamRequest
    {
        public int employeeID { get; set; }
        public int kpiSessionID { get; set; }
        public List<Employee> lstEmpChose { get; set; } = new List<Employee>();
    }
}
