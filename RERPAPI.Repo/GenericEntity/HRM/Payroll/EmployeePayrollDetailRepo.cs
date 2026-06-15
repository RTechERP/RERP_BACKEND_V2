using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeePayrollDetailRepo : GenericRepo<EmployeePayrollDetail>
    {
        public EmployeePayrollDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}