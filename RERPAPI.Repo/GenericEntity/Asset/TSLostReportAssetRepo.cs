using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Asset
{
    public class TSLostReportAssetRepo : GenericRepo<TSLostReportAsset>
    {
        public TSLostReportAssetRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
