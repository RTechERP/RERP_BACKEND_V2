using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeOnLeaveRepo : GenericRepo<EmployeeOnLeave>
    {
        public EmployeeOnLeaveRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}