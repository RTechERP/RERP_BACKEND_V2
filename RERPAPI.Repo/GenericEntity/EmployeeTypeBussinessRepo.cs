using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeTypeBussinessRepo : GenericRepo<EmployeeTypeBussiness>
    {
        public EmployeeTypeBussinessRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}