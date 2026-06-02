using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeePayrollBonusDeuctionRepo : GenericRepo<EmployeePayrollBonusDeuction>
    {
        public EmployeePayrollBonusDeuctionRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}