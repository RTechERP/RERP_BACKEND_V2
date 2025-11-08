using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Asset
{
    public class TSStatusAssetRepo : GenericRepo<TSStatusAsset>
    {
        public TSStatusAssetRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
