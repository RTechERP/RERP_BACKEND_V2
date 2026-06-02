using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class FiveSBonusMinusRepo : GenericRepo<FiveSBonusMinu>
    {
        public FiveSBonusMinusRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}