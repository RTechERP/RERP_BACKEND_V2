using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeOnLeaveMasterRepo : GenericRepo<EmployeeOnLeaveMaster>
    {
        public EmployeeOnLeaveMasterRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}