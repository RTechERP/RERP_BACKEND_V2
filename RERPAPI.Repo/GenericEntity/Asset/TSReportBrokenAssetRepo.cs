using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Asset
{
    public class TSReportBrokenAssetRepo : GenericRepo<TSReportBrokenAsset>
    {
        public TSReportBrokenAssetRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
