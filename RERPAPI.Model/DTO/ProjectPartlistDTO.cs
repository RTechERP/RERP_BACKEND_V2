using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ProjectPartlistDTO:ProjectPartList
    {
        /*public int? Mode { get; set; }//1:ApproveTBP; 2:ApproveNewCode*/
        public bool IsLeaf { get; set; }
        public bool? HasChildren { get; set; }
        public bool? IsCheckPrice { get; set; }
        public int? EmployeeIDRequestPrice { get; set; }

        // yc mua
        public DateTime? DeadlinePur { get; set; }
        public int? SupplierSaleQuoteID { get; set; }
        public int? UnitPriceQuote { get; set; }
        public int? UnitPriceHistory { get; set; }

    }
}
