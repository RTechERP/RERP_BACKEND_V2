using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RERPAPI.Model.Entities;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.Asset
{
    public class
        TSSourceAssetsRepo : GenericRepo<TSSourceAsset>
    {
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
