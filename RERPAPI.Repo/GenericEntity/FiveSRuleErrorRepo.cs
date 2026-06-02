using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class FiveSRuleErrorRepo : GenericRepo<FiveSRuleError>
    {
        public FiveSRuleErrorRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}