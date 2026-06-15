using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class EmployeeOvertimeFileRepo : GenericRepo<EmployeeOvertimeFile>
    {
        public EmployeeOvertimeFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}