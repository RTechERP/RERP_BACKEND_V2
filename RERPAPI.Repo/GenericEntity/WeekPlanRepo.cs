using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class WeekPlanRepo : GenericRepo<WeekPlan>
    {
        public WeekPlanRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}