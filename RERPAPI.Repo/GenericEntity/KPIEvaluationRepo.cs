using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class KPIEvaluationRepo : GenericRepo<KPIEvaluation>
    {
        public KPIEvaluationRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}