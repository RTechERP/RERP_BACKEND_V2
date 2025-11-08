using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Asset
{
    public class TSLiQuidationAssetRepo : GenericRepo<TSLiQuidationAsset>
    {
        public TSLiQuidationAssetRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
