using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class HandoverPersonalAssetRepo : GenericRepo<HandoverPersonalAsset>
    {
        public HandoverPersonalAssetRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}