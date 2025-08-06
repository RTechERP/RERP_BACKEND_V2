using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeTypeOverTimeController : Controller
    {
        EmployeeTypeOverTimeRepo employeeTypeOverTimeRepo = new EmployeeTypeOverTimeRepo();
        [HttpGet]
        public IActionResult GetAllEmployeeTypeOverTime()
        {
            try
            {
                var employeeTypeOverTimes = employeeTypeOverTimeRepo.GetAll().Where(x => x.IsDeleted == false);
                return Ok(new
                {
                    status = 1,
                    data = employeeTypeOverTimes.ToList()
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
        public async Task<IActionResult> SaveEmployeeTypeOverTime([FromBody] EmployeeTypeOvertime employeeTypeOverTime)
        {
            try
            {
                List<EmployeeTypeOvertime> existingEmployeeTypeOverTimes = employeeTypeOverTimeRepo.GetAll();

                if(existingEmployeeTypeOverTimes.Any(x => (x.Type == employeeTypeOverTime.Type || x.TypeCode == employeeTypeOverTime.TypeCode) && x.ID != employeeTypeOverTime.ID && x.IsDeleted == false))
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Tên hoặc mã kiểu làm thêm đã tồn tại"
                    });
                }

                if (employeeTypeOverTime.ID <= 0)
                {
                    await employeeTypeOverTimeRepo.CreateAsync(employeeTypeOverTime);
                }
                else
                {
                    employeeTypeOverTimeRepo.UpdateFieldsByID(employeeTypeOverTime.ID, employeeTypeOverTime);
                }

                return Ok(new
                {
                    status = 1,
                    message = "Lưu loại làm thêm thành công"
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
