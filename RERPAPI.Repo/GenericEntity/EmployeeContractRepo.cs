using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeContractRepo : GenericRepo<EmployeeContract>
    {
        public EmployeeContractRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}