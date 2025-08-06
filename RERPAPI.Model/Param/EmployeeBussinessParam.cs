using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class EmployeeBussinessParam
    {
        public string keyWord { get; set; }
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 1000000;
        public DateTime dateStart { get; set; }
        public DateTime dateEnd { get; set; }
        public int departmentId { get; set; }
        public int idApprovedTp { get; set; }
        public int status { get; set; }
    }
}
