using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeEarlyLateRepo : GenericRepo<EmployeeEarlyLate>
    {
        public EmployeeEarlyLateRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}