using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeePayrollRepo : GenericRepo<EmployeePayroll>
    {
        public EmployeePayrollRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}