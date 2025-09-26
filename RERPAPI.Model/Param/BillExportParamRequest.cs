using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class BillExportParamRequest
    {
        public string KhoType { get; set; } = "";
        public int Status { get; set; } = 0;
        public DateTime? @DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string FilterText { get; set; } = "";
        public int PageNumber { get; set; } = 0;
        public int PageSize { get; set; } = 0;
        public string WarehouseCode { get; set; } = "";
        public bool checkedAll { get; set; }
    }
}
