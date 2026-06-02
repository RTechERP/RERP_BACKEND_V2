using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Technical.KPI
{
    public class KPIEvaluationRuleRepo : GenericRepo<KPIEvaluationRule>
    {
        public KPIEvaluationRuleRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}