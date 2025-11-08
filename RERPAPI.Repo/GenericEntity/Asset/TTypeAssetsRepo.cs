using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Asset
{
    public class TTypeAssetsRepo : GenericRepo<TSAsset>
    {
        public TTypeAssetsRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public bool Validate(TSAsset item, out string message)
        {
            message = "";
            bool exists = GetAll().Any(x => x.AssetCode == item.AssetCode && x.ID != item.ID && x.IsDeleted != true);
            if (exists)
            {
                message = "Loại tài sản đã tồn tại";
                return false;
            }
            return true;
        }
    }
}
