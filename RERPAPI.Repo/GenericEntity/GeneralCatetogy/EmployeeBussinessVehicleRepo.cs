using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.GeneralCatetogy
{
    public class EmployeeBussinessVehicleRepo : GenericRepo<EmployeeBussinessVehicle>
    {
        public EmployeeBussinessVehicleRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}