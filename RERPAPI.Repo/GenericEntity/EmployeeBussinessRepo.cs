using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeBussinessRepo : GenericRepo<EmployeeBussiness>
    {
        public EmployeeBussinessRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}