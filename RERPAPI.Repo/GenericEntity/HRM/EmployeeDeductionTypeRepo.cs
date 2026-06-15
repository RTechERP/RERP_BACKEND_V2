using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class EmployeeDeductionTypeRepo : GenericRepo<EmployeeDeductionType>
    {
        public EmployeeDeductionTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}