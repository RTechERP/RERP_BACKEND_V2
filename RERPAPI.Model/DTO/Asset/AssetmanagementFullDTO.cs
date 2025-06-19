using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO.Asset
{
    public class AssetmanagementFullDTO
    {
        public List<TSAssetManagement>? tSAssetManagements { get; set; }
        public TSLostReportAsset? tSLostReportAsset { get; set; }
        public TSReportBrokenAsset? tSReportBrokenAsset { get; set; }
        public List<TSAllocationEvictionAsset>? tSAllocationEvictionAssets { get; set; }
    }
}
