using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO.Asset
{
    public class TSAssetAllocationFullDTO
    {
        public TSAssetAllocation? tSAssetAllocation { get; set; }
        public List<TSAssetAllocationDetail>? tSAssetAllocationDetails { get; set; }
        public List<TSAssetManagement>? tSAssetManagements { get; set; }
        public List<TSAllocationEvictionAsset>? tSAllocationEvictionAssets { get; set; }
    }
}