using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Asset
{
    public class TSAssetTransferDetailRepo:GenericRepo<TSTranferAssetDetail>
    {
        public TSAssetTransferDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
