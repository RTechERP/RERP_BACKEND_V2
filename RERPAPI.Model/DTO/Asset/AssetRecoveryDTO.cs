using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO.Asset
{
    public class AssetRecoveryDTO
    {
        public TSAssetRecovery? tSAssetRecovery { get; set; }
        public List<TSAssetRecoveryDetail>? TSAssetRecoveryDetails { get; set; }
        public List<TSAssetManagement>? tSAssetManagements { get; set; }
        public List<TSAllocationEvictionAsset>? tSAllocationEvictionAssets { get; set; }
    }
}