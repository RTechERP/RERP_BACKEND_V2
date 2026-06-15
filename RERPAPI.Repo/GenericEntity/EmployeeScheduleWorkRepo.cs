using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeScheduleWorkRepo : GenericRepo<EmployeeScheduleWork>
    {
        public EmployeeScheduleWorkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}