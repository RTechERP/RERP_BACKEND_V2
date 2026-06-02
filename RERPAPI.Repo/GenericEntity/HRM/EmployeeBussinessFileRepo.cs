using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class EmployeeBussinessFileRepo : GenericRepo<EmployeeBussinessFile>
    {
        public EmployeeBussinessFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}