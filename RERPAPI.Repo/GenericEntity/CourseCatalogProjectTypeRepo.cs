using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CourseCatalogProjectTypeRepo : GenericRepo<CourseCatalogProjectType>
    {
        public CourseCatalogProjectTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}