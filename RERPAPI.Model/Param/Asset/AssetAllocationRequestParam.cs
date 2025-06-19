using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public  class AssetAllocationRequestParam
    {
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int? EmployeeID { get; set; }
        public int? Status { get; set; }
        public string? FilterText { get; set; }
        public int PageSize { get; set; } 
        public int PageNumber { get; set; }
    }
}
