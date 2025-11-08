using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class EmployeeChamCongMasterRepo : GenericRepo<EmployeeChamCongMaster>
    {
        public EmployeeChamCongMasterRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
