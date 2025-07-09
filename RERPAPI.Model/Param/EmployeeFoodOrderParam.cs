using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class EmployeeFoodOrderParam
    {
        public DateTime dateStart { get; set; }
        public DateTime dateEnd { get; set; }
        public string keyWord { get; set; } = "";
        public int employeeId { get; set; } = 0;
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 1000000;

    }
}
