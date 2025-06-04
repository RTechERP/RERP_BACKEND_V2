using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class AssetRecoveryDTO
    {
        public TSAssetRecovery tSAssetRecovery { get; set; }
        public List<TSAssetRecoveryDetail> TSAssetRecoveryDetails { get; set; }
    }
}
