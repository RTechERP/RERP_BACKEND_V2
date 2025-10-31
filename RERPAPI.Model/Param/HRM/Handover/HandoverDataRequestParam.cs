using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param.Handover
{
    public class HandoverDataRequestParam
    {
        public int? HandoverID { get; set; }
        public int? EmployeeID { get; set; }
        public int? LeaderID { get; set; }
         public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? DateBegin { get; set; }
        public string? DateEnd { get; set; }
        public int? ProductGroupID { get; set; }
        public int? ReturnStatus { get; set; }
        public string? FilterText { get; set; }
        public int WareHouseID { get; set; }
        //HCNS
        public string? DateStart { get; set; }






    }
}
