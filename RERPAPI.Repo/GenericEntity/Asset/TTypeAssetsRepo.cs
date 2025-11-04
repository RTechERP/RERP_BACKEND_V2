using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RERPAPI.Model.Entities;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.Asset
{
    public class TTypeAssetsRepo:GenericRepo<TSAsset>
    {
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
