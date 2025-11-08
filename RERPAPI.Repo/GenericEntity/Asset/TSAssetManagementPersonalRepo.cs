using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Asset
{
    public class TSAssetManagementPersonalRepo : GenericRepo<TSAssetManagementPersonal>
    {
        public TSAssetManagementPersonalRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
