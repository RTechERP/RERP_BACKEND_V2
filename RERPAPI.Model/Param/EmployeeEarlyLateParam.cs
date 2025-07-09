using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class EmployeeEarlyLateParam
    {
        public string filterText { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public int departmentId { get; set; }
        public int idApprovedTp { get; set; }
        public int status { get; set; }
    }
}
