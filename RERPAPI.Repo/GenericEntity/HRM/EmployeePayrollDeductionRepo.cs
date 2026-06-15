using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class EmployeePayrollDeductionRepo : GenericRepo<EmployeePayrollDeduction>
    {
        public EmployeePayrollDeductionRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}