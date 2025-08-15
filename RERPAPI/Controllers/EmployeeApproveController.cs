using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeApproveController : ControllerBase
    {
        EmployeeApproveRepo _employeeApproveRepo = new EmployeeApproveRepo();
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                List<EmployeeApprove> employeeApprovals = _employeeApproveRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = employeeApprovals
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
