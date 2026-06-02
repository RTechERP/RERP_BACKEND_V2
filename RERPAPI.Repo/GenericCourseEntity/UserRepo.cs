using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities.RTCCourse;

namespace RERPAPI.Repo.GenericCourseEntity
{
    public class UserRepo : GenericCourseRepo<User>
    {
        public UserRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}