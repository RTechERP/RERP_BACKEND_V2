using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class AssetTranferFullDTO
    {
        public List<TSAssetManagement>? tSAssetManagements { get; set; }
        public List<TSTranferAssetDetail>? tSTranferAssetDetails { get; set; }
        public TSTranferAsset? tSTranferAsset{ get; set; }
        public List<TSAllocationEvictionAsset>? tSAllocationEvictionAssets { get; set; }
    }
}
