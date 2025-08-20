using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param.TB.Inventory
{
    public class ProductExportAndBorrowRequestParam
    {
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int Page { get; set; } = 10000;
        public int Size { get; set; } = 1;
        public string? Filtertext { get; set; }  
        public int? WarehouseID { get; set; }
    }
}
