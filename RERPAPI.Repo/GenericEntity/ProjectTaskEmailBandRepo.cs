using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTaskEmailBandRepo : GenericRepo<ProjectTaskEmailBand>
    {
        public ProjectTaskEmailBandRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}