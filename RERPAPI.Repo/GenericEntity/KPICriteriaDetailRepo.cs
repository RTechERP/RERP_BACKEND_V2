using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class KPICriteriaDetailRepo : GenericRepo<KPICriteriaDetail>
    {
        public KPICriteriaDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}