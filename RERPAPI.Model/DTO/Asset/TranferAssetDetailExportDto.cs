using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.Asset
{
    public class TranferAssetDetailExportDto
    {
        public string TSCodeNCC { get; set; }
        public string TSAssetName { get; set; }
        public string UnitName { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
    }

}
