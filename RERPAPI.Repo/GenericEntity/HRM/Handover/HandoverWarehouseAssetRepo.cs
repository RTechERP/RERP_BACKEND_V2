using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.BBNV
{
    public class HandoverWarehouseAssetRepo : GenericRepo<HandoverWarehouseAsset>
    {
        public HandoverWarehouseAssetRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
