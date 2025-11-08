using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.BBNV
{
    public class HandoverAssetManagementRepo : GenericRepo<HandoverAssetManagement>
    {
        public HandoverAssetManagementRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
