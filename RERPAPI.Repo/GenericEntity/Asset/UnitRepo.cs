using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Asset
{
    public class UnitRepo : GenericRepo<UnitCount>
    {
        public UnitRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
