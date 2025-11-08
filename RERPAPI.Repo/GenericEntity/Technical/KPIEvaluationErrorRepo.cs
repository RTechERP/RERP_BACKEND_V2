using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Technical
{
    public class KPIEvaluationErrorRepo : GenericRepo<KPIEvaluationError>
    {
        public KPIEvaluationErrorRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
