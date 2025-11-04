using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class FollowProjectBaseFilterParam
    {
        public int? page { get; set; }               // @PageNumber
        public int? size { get; set; }               // @PageSize
        public DateTime? dateStart { get; set; }    // @DateStart
        public DateTime? dateEnd { get; set; }      // @DateEnd
        public string? filterText { get; set; }      // @FilterText
        public string? user { get; set; }            // @User
        public int? customerID { get; set; }        // @CustomerID
        public string? pm { get; set; }              // @PM
        public int? warehouseID { get; set; }       // @WarehouseID
        public int? groupSaleID { get; set; }       // @GroupSaleID

    }
}
