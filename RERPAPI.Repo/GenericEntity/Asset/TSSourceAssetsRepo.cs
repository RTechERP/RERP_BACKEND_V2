using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Asset
{
    public class
        TSSourceAssetsRepo : GenericRepo<TSSourceAsset>
    {
        public TSSourceAssetsRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public bool Validate(TSSourceAsset item, out string message)
        {
            message = "";
            bool exists = GetAll().Any(x => x.SourceCode == item.SourceCode && x.ID != item.ID && x.IsDeleted != true);
            if (exists)
            {
                message = "Mã nguồn gốc tài sản đã tồn tại";
                return false;
            }
            return true;
        }
    }
}
