using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class KPICriterionRepo : GenericRepo<KPICriterion>
    {
        public KPICriterionRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}