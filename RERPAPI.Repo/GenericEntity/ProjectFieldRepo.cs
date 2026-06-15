using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectFieldRepo : GenericRepo<BusinessField>
    {
        public ProjectFieldRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}