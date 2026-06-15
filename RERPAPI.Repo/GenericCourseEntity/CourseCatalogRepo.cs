using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities.RTCCourse;

namespace RERPAPI.Repo.GenericCourseEntity
{
    public class CourseCatalogRepo : GenericCourseRepo<CourseCatalog>
    {
        public CourseCatalogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}