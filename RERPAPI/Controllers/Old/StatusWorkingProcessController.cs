using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusWorkingProcessController : Controller
    {
        private StatusWorkingProcessRepo _statusWorkingProcessRepo;
        public StatusWorkingProcessController(StatusWorkingProcessRepo statusWorkingProcessRepo)
        {
            _statusWorkingProcessRepo = statusWorkingProcessRepo;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var statusWorkingProcesses = _statusWorkingProcessRepo.GetAll();
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
                List<EmployeeStatus> statusWorkingProcesses = _statusWorkingProcessRepo.GetAll();
                if (statusWorkingProcesses.Any(x => (x.StatusName == statusWorkingProcess.StatusName || x.StatusCode == statusWorkingProcess.StatusCode) && x.ID != statusWorkingProcess.ID))
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Trạng thái làm việc đã tồn tại"
                    });
                }

                if (statusWorkingProcess.ID <= 0)
                {
                    await _statusWorkingProcessRepo.CreateAsync(statusWorkingProcess);
                }
                else
                {
                    await _statusWorkingProcessRepo.UpdateAsync(statusWorkingProcess);
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
