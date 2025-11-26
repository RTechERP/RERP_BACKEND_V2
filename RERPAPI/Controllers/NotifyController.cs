using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RERPAPI.Middleware;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class NotifyController : ControllerBase
    {
        private readonly NotifyRepo _notifyRepo;
        public NotifyController(NotifyRepo notifyRepo)
        {
            _notifyRepo = notifyRepo;
        }

        [HttpPost("add-notify")]
        public async Task<IActionResult> AddNotify([FromBody] NotifyRequestParam request)
        {
            try
            {
                Notify notify = new Notify();
                notify.Title = request.title;
                notify.Text = request.text;
                notify.EmployeeID = request.employeeID;
                notify.DepartmentID = request.departmentID ?? 0;
                notify.NotifyStatus = 1;
                await _notifyRepo.CreateAsync(notify);

                return Ok(ApiResponseFactory.Success(null, "Lấy dữu liệu thành công"));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}
