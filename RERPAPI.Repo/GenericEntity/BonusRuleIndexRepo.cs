using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class BonusRuleIndexRepo : GenericRepo<BonusRuleIndex>
    {
        public BonusRuleIndexRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}