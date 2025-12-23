using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class EmployeeContactDTO: Employee
    {
        public Int64 STT { get; set; }
        public string DepartmentName { get; set; }
        public string ChucVuHD { get; set; }
        public string ChucVu { get; set; }
        public string EmployeeTeamName { get; set; }
    }
}
