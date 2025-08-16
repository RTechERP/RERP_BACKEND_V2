using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class HistoryBillDeleteParamRequest
    {
       /* int billType,int? billImportID,int? billExportID*/
        public int billType { get; set; }
        public int? billImportID { get; set; }
        public int? billExportID { get; set; }


    }
}
