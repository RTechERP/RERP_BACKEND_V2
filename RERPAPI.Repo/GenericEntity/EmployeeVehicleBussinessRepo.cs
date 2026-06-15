using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeVehicleBussinessRepo : GenericRepo<EmployeeVehicleBussiness>
    {
        public EmployeeVehicleBussinessRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}