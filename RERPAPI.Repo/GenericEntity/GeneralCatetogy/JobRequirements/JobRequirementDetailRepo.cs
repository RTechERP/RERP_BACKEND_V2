using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.GeneralCatetogy.JobRequirements
{
    public class JobRequirementDetailRepo : GenericRepo<JobRequirementDetail>
    {
        public JobRequirementDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}