using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeWorkingProcessRepo : GenericRepo<EmployeeWorkingProcess>
    {
        public EmployeeWorkingProcessRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}