using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Technical.KPI
{
    public class KPISumaryEvaluationRepo : GenericRepo<KPISumaryEvaluation>
    {
        public KPISumaryEvaluationRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}