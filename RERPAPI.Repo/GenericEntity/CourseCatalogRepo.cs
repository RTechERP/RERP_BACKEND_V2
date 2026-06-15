using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CourseCatalogRepo : GenericRepo<CourseCatalog>
    {
        public CourseCatalogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}