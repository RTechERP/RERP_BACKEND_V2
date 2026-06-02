using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO.Asset
{
    public class TSAssetPersonalApproveDTO
    {
        public TSAllocationAssetPersonal? tSAllocationAssetPersonal { get; set; }
        public TSRecoveryAssetPersonal? tSRecoveryAssetPersonal { set; get; }
    }
}