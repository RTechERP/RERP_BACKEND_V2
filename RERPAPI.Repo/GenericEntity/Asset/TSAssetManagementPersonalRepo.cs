using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Asset
{
    public class TSAssetManagementPersonalRepo:GenericRepo<TSAssetManagementPersonal>
    {
        public TSAssetManagementPersonalRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
