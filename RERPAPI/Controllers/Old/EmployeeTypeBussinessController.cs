using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeTypeBussinessController : ControllerBase
    {
        EmployeeTypeBussinessRepo employeeTypeBussinessRepo = new EmployeeTypeBussinessRepo();
        [HttpGet]
        public IActionResult GetEmployeeTypeBussiness()
        {
            try
            {
                var result = employeeTypeBussinessRepo.GetAll();
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
        public async Task<IActionResult> SaveEmployeeTypeBussiness([FromBody] EmployeeTypeBussiness employeeTypeBussiness)
        {
            try
            {
                List<EmployeeTypeBussiness> listData = employeeTypeBussinessRepo.GetAll();

                if (employeeTypeBussiness.ID <= 0)
                {
                    await employeeTypeBussinessRepo.CreateAsync(employeeTypeBussiness);
                }
                else
                {
                    await employeeTypeBussinessRepo.UpdateAsync(employeeTypeBussiness);
                }
                return Ok(new
                {
                    status = 1,
                    data = employeeTypeBussiness,
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
