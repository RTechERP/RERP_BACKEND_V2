using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class JobRequirementRecommendDetailRepo : GenericRepo<JobRequirementRecommendDetail>
    {
        public JobRequirementRecommendDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}