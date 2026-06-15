using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class PONCCRulePayRepo : GenericRepo<PONCCRulePay>
    {
        public PONCCRulePayRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}