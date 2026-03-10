using RERPAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param.HRM.DepartmentRequired
{
    public class DepartmentRequiredRequestParam
    {
        public string Keyword { get; set; } = "";
        public int JobRequirementID { get; set; }
        public int DepartmentRequiredID { get; set; }
        public int EmployeeID { get; set; }
        public int DepartmentID { get; set; }
        public DateTime DateStart { get; set; } = TextUtils.MinDate;
        public DateTime DateEnd { get; set; } = TextUtils.MaxDate;

    }
}
