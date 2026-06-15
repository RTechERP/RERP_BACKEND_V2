using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class RulePayRepo : GenericRepo<RulePay>
    {
        public RulePayRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}