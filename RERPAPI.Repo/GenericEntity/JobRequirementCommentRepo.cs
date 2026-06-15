using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class JobRequirementCommentRepo : GenericRepo<JobRequirementComment>
    {
        public JobRequirementCommentRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}