using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
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
                var employeeTypeOverTimes = _employeeTypeOverTimeRepo.GetAll(x=> x.IsDeleted != true);
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
        [RequiresPermission("N2,N1")]
        public async Task<IActionResult> SaveEmployeeTypeOverTime([FromBody] EmployeeTypeOvertime employeeTypeOverTime)
        {
            try
            {
                List<EmployeeTypeOvertime> existingEmployeeTypeOverTimes = _employeeTypeOverTimeRepo.GetAll();

                if (existingEmployeeTypeOverTimes.Any(x => (x.Type == employeeTypeOverTime.Type || x.TypeCode == employeeTypeOverTime.TypeCode) && x.ID != employeeTypeOverTime.ID
                && x.IsDeleted != true
                ))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Tên hoặc mã kiểu làm thêm đã tồn tại"));
                }
                employeeTypeOverTime.Type = employeeTypeOverTime.Type.Trim();
                employeeTypeOverTime.TypeCode = employeeTypeOverTime.TypeCode.Trim();
                if (employeeTypeOverTime.ID <= 0)
                {
                    await _employeeTypeOverTimeRepo.CreateAsync(employeeTypeOverTime);
                }
                else
                {
                    await _employeeTypeOverTimeRepo.UpdateAsync(employeeTypeOverTime);
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
