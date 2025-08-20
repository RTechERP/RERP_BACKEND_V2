using System;
using System.Linq;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Asset
{
    public class TSRepairAssetRepo : GenericRepo<TSRepairAsset>
    {
        public object? GetLatestRepairByAssetManagementID(int assetManagementID)
        {
            return table
                .Where(x => x.AssetManagementID == assetManagementID)
                .OrderByDescending(x => x.ID)
                .Select(x => new
                {
                    x.ID,
                    x.AssetManagementID,
                    x.Name,
                    x.DateRepair,
                    x.ExpectedCost,
                    x.Reason
                })
                .FirstOrDefault();
        }
    }
}
