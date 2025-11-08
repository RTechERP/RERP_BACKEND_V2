using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Asset
{
    public class TSTypeAssetPersonalRepo : GenericRepo<TSTypeAssetPersonal>
    {
        public TSTypeAssetPersonalRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
