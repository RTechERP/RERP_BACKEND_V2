using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO.Asset
{
    public class TSAssetManagementPersonalFullDTO
    {
        public TSAllocationAssetPersonal? tSAllocationAssetPersonal { get; set; }
        public TSTypeAssetPersonal? tSTypeAssetPersonal { get; set; }
        public List<TSAllocationAssetPersonalDetail>? tSAllocationAssetPersonalDetails { set; get; }
        public TSAssetManagementPersonal? tSAssetManagementPersonal { set; get; }
        public TSRecoveryAssetPersonal? tSRecoveryAssetPersonal { set; get; }
        public List<TSRecoveryAssetPersonalDetail>? tSRecoveryAssetPersonalDetails { set; get; } 
    }
}
    