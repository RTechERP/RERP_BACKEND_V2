using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities.RTCCourse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericCourseEntity
{
    public class EmployeeRepo : GenericCourseRepo<Employee>
    {
        public EmployeeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}