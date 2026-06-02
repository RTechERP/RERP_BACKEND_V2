using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO.Asset
{
    public class AssetAllocationDTO
    {
        public TSAssetAllocation tSAssetAllocation { get; set; }
        public List<TSAllocationEvictionAsset> tSAllocationEvictionAssets { get; set; }
    }
}