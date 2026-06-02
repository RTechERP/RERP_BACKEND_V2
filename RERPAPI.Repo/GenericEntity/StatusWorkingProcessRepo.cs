using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class StatusWorkingProcessRepo : GenericRepo<EmployeeStatus>
    {
        public StatusWorkingProcessRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}