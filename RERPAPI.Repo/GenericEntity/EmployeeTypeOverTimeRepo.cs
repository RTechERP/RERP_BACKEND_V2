using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeTypeOverTimeRepo : GenericRepo<EmployeeTypeOvertime>
    {
        public EmployeeTypeOverTimeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}