using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class InventoryPram
    {
        public int productGroupID { get; set; } = -1;
        public bool checkAll { get; set; }
        public string Find { get; set; }
        public string WarehouseCode { get; set; }
        public bool IsStock { get; set; }
    }
}
