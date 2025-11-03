using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.Asset
{
    public class TSAssetPersonalApproveDTO
    {
        public TSAllocationAssetPersonal? tSAllocationAssetPersonal { get; set; }
        public TSRecoveryAssetPersonal? tSRecoveryAssetPersonal { set; get; }
    }
}
 
 