using RERPAPI.Model.DTO;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTaskApproveRepo : GenericRepo<Model.Entities.ProjectTaskApprove>
    {
        public ProjectTaskApproveRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}