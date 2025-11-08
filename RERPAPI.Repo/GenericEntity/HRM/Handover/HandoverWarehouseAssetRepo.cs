using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.BBNV
{
    public class HandoverWarehouseAssetRepo : GenericRepo<HandoverWarehouseAsset>
    {
        public HandoverWarehouseAssetRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
