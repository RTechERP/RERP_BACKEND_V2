using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Asset
{
    public class TSAllocationEvictionAssetRepo : GenericRepo<TSAllocationEvictionAsset>
    {
        public TSAllocationEvictionAssetRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
