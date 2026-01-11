using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class JobRequirementParam
    {
        public DateTime DateStart { get; set; } = DateTime.MinValue;
        public DateTime DateEnd { get; set; } = DateTime.MaxValue;
        public string Request { get; set; } = string.Empty;
        public int EmployeeID { get; set; } = 0;
        public int Step { get; set; } = 0;
        public int DepartmentID { get; set; } = 0;
        public int ApprovedTBPID { get; set; } = 0;
    }
}
