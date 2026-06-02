using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeAttendanceNewRepo : GenericRepo<EmployeeAttendanceNew>
    {
        public EmployeeAttendanceNewRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}