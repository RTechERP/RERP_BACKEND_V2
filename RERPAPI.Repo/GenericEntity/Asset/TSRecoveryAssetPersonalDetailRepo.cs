using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Asset
{
    public class TSRecoveryAssetPersonalDetailRepo : GenericRepo<TSRecoveryAssetPersonalDetail>
    {
        public TSRecoveryAssetPersonalDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
