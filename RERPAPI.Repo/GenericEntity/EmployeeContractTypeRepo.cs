using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeContractTypeRepo : GenericRepo<EmployeeLoaiHDLD>
    {
        public EmployeeContractTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}