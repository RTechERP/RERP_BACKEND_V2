using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeStatusRepo : GenericRepo<EmployeeStatus>
    {
        public EmployeeStatusRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}