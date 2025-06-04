using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class AssetAllocationDTO 
    {
        public TSAssetAllocation tSAssetAllocation { get; set; }
        public List<TSAllocationEvictionAsset> tSAllocationEvictionAssets { get; set; }
       

    }
}
