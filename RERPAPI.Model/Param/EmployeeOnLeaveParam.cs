using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class EmployeeOnLeaveParam
    {
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 1000000;
        public string keyWord { get; set; } = "";
        public int employeeId { get; set; } = 0;
        public int departmentId { get; set; } = 0;
        public int month { get; set; }
        public int year { get; set; }
        public int IDApprovedTP { get; set; }
        public int status { get; set; } = -1;
        
    }
}
