using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO.HRM
{
    public class HandoverDTO
    {
        public Handover? Handover { get; set; }
        public List<HandoverReceiver>? Receivers { get; set; }
        public List<HandoverWork>? Works { get; set; }
        public List<HandoverWarehouseAsset>? WarehouseAssets { get; set; }
        public List<HandoverAssetManagement>? AssetManagements { get; set; }
        public List<HandoverFinance>? Finances { get; set; }
        public List<HandoverSubordinate>? Subordinates { get; set; }
        public List<HandoverApprove>? Approves { get; set; }
        public List<HandoverPersonalAsset>? PersonalAssets { get; set; }
        public List<int>? DeletedReceivers { get; set; }
        public List<int>? DeletedWorks { get; set; }
        public List<int>? DeletedAssets { get; set; }
        public List<int>? DeletedWarehouseAssets { get; set; }
        public List<int>? DeletedFinances { get; set; }
        public List<int>? DeletedPersonalAssets { get; set; }
    }
}