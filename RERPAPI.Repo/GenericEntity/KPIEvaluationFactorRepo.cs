using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class KPIEvaluationFactorRepo : GenericRepo<KPIEvaluationFactor>
    {
        public KPIEvaluationFactorRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}