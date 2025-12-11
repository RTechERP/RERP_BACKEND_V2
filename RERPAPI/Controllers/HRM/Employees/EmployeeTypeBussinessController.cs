using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.HRM.Employees
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeTypeBussinessController : ControllerBase
    {
        private readonly EmployeeTypeBussinessRepo _employeeTypeBussinessRepo;

        public EmployeeTypeBussinessController(EmployeeTypeBussinessRepo employeeTypeBussinessRepo)
        {
            _employeeTypeBussinessRepo = employeeTypeBussinessRepo;
        }
     
        [HttpGet]
        public IActionResult GetEmployeeTypeBussiness()
        {
            try
            {
                var result = _employeeTypeBussinessRepo.GetAll();
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

        [RequiresPermission("N1,N2")]
        [HttpPost]
        public async Task<IActionResult> SaveEmployeeTypeBussiness([FromBody] EmployeeTypeBussiness employeeTypeBussiness)
        {
            try
            {
                List<EmployeeTypeBussiness> listData = _employeeTypeBussinessRepo.GetAll();

                if (employeeTypeBussiness.ID <= 0)
                {
                    await _employeeTypeBussinessRepo.CreateAsync(employeeTypeBussiness);
                }
                else
                {
                    await _employeeTypeBussinessRepo.UpdateAsync(employeeTypeBussiness);
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
