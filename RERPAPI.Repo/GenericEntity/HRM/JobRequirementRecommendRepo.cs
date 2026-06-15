using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class JobRequirementRecommendRepo : GenericRepo<JobRequirementRecommend>
    {
        public JobRequirementRecommendRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}