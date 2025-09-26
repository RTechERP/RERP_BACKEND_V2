using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusWorkingProcessController : Controller
    {
        StatusWorkingProcessRepo statusWorkingProcessRepo = new StatusWorkingProcessRepo();
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var statusWorkingProcesses = statusWorkingProcessRepo.GetAll().Where(x => x.IsDeleted == false);
                return Ok(new
                {
                    status = 1,
                    data = statusWorkingProcesses
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
        public async Task<IActionResult> SaveStatusWorkingProcess([FromBody] EmployeeStatus statusWorkingProcess)
        {
            try
            {
                List<EmployeeStatus> statusWorkingProcesses = statusWorkingProcessRepo.GetAll().Where(x => x.IsDeleted == false).ToList();
                if(statusWorkingProcesses.Any(x => (x.StatusName == statusWorkingProcess.StatusName || x.StatusCode == statusWorkingProcess.StatusCode) && x.ID != statusWorkingProcess.ID))
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Trạng thái làm việc đã tồn tại"
                    });
                }

                if(statusWorkingProcess.ID <= 0)
                {
                    await statusWorkingProcessRepo.CreateAsync(statusWorkingProcess);
                } else
                {
                    await statusWorkingProcessRepo.UpdateAsync(statusWorkingProcess);
                }
                return Ok(new
                {
                    status = 1,
                    data = statusWorkingProcess,
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
