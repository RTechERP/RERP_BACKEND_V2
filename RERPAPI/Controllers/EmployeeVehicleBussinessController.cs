using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeVehicleBussinessController : Controller
    {
        EmployeeVehicleBussinessRepo employeeVehicleBussinessRepo = new EmployeeVehicleBussinessRepo();
        [HttpGet]
        public IActionResult GetEmployeeVehicleBussiness()
        {
            try
            {
                var result = employeeVehicleBussinessRepo.GetAll().Where(x => x.IsDeleted == false);
                return Ok(new
                {
                    status = 1,
                    data = result
                });
            } catch (Exception ex)
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
                List<EmployeeVehicleBussiness> listData = employeeVehicleBussinessRepo.GetAll();
       
                if (employeeVehicleBussiness.ID <= 0)
                {
                    await employeeVehicleBussinessRepo.CreateAsync(employeeVehicleBussiness);
                }
                else
                {   
                    await employeeVehicleBussinessRepo.UpdateAsync(employeeVehicleBussiness);
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
