using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.Asset
{
    public class AssetRecoveryDetailExportDto
    {
        public int ID { get; set; }
        public int STT { get; set; }
        public int TSAssetRecoveryID { get; set; }
        public int AssetManagementID { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
        public string? TSAssetName { get; set; }
        public string? TSCodeNCC { get; set; }
        public string? UnitName { get; set; }
        public string? Status { get; set; }
    }

}
