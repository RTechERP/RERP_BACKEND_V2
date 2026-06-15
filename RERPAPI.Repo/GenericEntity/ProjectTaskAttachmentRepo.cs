using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTaskAttachmentRepo : GenericRepo<ProjectTaskAttachment>
    {
        public ProjectTaskAttachmentRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}