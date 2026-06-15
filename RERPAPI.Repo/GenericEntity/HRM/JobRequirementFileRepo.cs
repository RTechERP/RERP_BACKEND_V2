using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class JobRequirementFileRepo : GenericRepo<JobRequirementFile>
    {
        public JobRequirementFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}