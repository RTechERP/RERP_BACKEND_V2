using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.HRM
{
    public class HandoverDTO
    {
        public Handover Handover { get; set; }
        public List<HandoverReceiver>? HandoverReceiver { get; set; }
        public List<HandoverWork>? HandoverWork { get; set; }
        public List<HandoverWarehouseAsset>? HandoverWarehouseAsset { get; set; }
        public List<HandoverAssetManagement>? HandoverAssetManagement { get; set; }
        public List<HandoverFinance>? HandoverFinance { get; set; }
        public List<HandoverSubordinate>? HandoverSubordinate { get; set; }
        public List<HandoverApprove> HandoverApprove { get; set; }
        public List<int>? DeletedHandoverReceiver { get; set; }
        public List<int>? DeletedWork { get; set; }
        public List<int>? DeletedAsset { get; set; }
        public List<int>? DeletedWarehouseAsset { get; set; }
        public List<int>? DeletedFinance { get; set; }



    }
}
