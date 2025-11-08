using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class EmployeeChamCongDetailRepo : GenericRepo<EmployeeChamCongDetail>
    {
        public EmployeeChamCongDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
