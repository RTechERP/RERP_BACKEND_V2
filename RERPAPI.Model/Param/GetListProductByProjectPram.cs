using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class GetListProductByProjectPram
    {
        public int projectID { get; set; } = 0;
        public string projectCode { get; set; } = "";
        public string WarehouseCode { get; set; } = "";
    }
}
