using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param.HRM.VehicleManagement
{
    public  class VehicleRepairRequestParam
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 99999;
        public DateTime? @DateStart { get; set; } 
        public DateTime? DateEnd { get; set; }
        public string FilterText { get; set; } = "";
        public int EmployeeID { get; set; } = 0;
        public int VehicleID { get; set; } = 0;
        public int TypeID { get; set; } = 0;
    }
}
