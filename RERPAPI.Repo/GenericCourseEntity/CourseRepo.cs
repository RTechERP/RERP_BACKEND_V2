using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities.RTCCourse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericCourseEntity
{
    public class CourseRepo : GenericCourseRepo<Course>
    {
        public CourseRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}