using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class KPIEvaluationRuleDetailRepo : GenericRepo<KPIEvaluationRuleDetail>
    {
        public KPIEvaluationRuleDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}