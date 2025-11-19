using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeTypeOverTimeController : Controller
    {
        private readonly EmployeeTypeOverTimeRepo _employeeTypeOverTimeRepo;

        public EmployeeTypeOverTimeController(EmployeeTypeOverTimeRepo employeeTypeOverTimeRepo)
        {
            _employeeTypeOverTimeRepo = employeeTypeOverTimeRepo;
        }
        [HttpGet]
        public IActionResult GetAllEmployeeTypeOverTime()
        {
            try
            {
                var employeeTypeOverTimes = _employeeTypeOverTimeRepo.GetAll();
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
                List<EmployeeTypeOvertime> existingEmployeeTypeOverTimes = _employeeTypeOverTimeRepo.GetAll();

                if (existingEmployeeTypeOverTimes.Any(x => (x.Type == employeeTypeOverTime.Type || x.TypeCode == employeeTypeOverTime.TypeCode) && x.ID != employeeTypeOverTime.ID
                //&& x.IsDeleted == false
                ))
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Tên hoặc mã kiểu làm thêm đã tồn tại"
                    });
                }

                if (employeeTypeOverTime.ID <= 0)
                {
                    await _employeeTypeOverTimeRepo.CreateAsync(employeeTypeOverTime);
                }
                else
                {
                    await _employeeTypeOverTimeRepo.UpdateAsync(employeeTypeOverTime);
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
