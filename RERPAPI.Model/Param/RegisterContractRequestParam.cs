using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class RegisterContractRequestParam
    {
        /*request.dateStart, request.dateEnd, request.status, request.empID, request.keyword, request.departmentID*/
        public DateTime dateStart { get; set; }
        public DateTime dateEnd { get; set; }
        public int status { get; set; }
        public int empID { get; set; }
        public string keyword { get; set; }
        public int departmentID { get; set; }

    }
}
