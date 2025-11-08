using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Asset
{
    public class TSAssetAllocationDetailRepo : GenericRepo<TSAssetAllocationDetail>
    {
        public TSAssetAllocationDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
