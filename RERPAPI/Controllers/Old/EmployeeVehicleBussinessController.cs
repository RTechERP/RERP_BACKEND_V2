using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeVehicleBussinessController : Controller
    {
        private readonly EmployeeVehicleBussinessRepo _employeeVehicleBussinessRepo;

        public EmployeeVehicleBussinessController(EmployeeVehicleBussinessRepo employeeVehicleBussinessRepo)
        {
            _employeeVehicleBussinessRepo = employeeVehicleBussinessRepo;
        }
        [HttpGet]
        public IActionResult GetEmployeeVehicleBussiness()
        {
            try
            {
                var result = _employeeVehicleBussinessRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }

        }




        [HttpPost]
        public async Task<IActionResult> SaveEmployeeVehicleBussiness([FromBody] EmployeeVehicleBussiness employeeVehicleBussiness)
        {
            try
            {
                List<EmployeeVehicleBussiness> listData = _employeeVehicleBussinessRepo.GetAll();

                if (employeeVehicleBussiness.ID <= 0)
                {
                    await _employeeVehicleBussinessRepo.CreateAsync(employeeVehicleBussiness);
                }
                else
                {
                    await _employeeVehicleBussinessRepo.UpdateAsync(employeeVehicleBussiness);
                }
                return Ok(new
                {
                    status = 1,
                    data = employeeVehicleBussiness,
                    message = "Lưu thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
    }
}
