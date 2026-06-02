using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO.HRM
{
    public class EmployeeBussinessDTO
    {
        public EmployeeBussiness? employeeBussiness { get; set; }
        public EmployeeBussinessFile? employeeBussinessFiles { get; set; }
        public EmployeeBussinessVehicle? employeeBussinessVehicle { get; set; }
    }
}