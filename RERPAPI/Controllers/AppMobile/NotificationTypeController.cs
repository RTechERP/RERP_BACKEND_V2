using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.Record.Chart;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Project;

namespace RERPAPI.Controllers.AppMobile
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class NotificationTypeController : ControllerBase
    {
        private readonly NotificationTypeRepo _notificationTypeRepo;
        private readonly NotificationTypeLinkRepo _notificationTypeLinkRepo;

        public NotificationTypeController(
            NotificationTypeRepo notificationTypeRepo,
            NotificationTypeLinkRepo notificationTypeLinkRepo
        )
        {
            _notificationTypeRepo = notificationTypeRepo;
            _notificationTypeLinkRepo = notificationTypeLinkRepo;
        }

        //lấy dữ liệu theo người dùng
        [HttpGet("get-data-by-userid")]
        public async Task<IActionResult> GetNotificationTypesByUser()
        {
            try
            {

                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
                var param = new
                {
                    @UserID = currentUser?.EmployeeID ?? 0
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetNotificationTypesByUser", param);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] List<NotificationTypeLink> notificationTypeLink)
        {
            try
            {
                foreach( var item in notificationTypeLink)
                { 
                    if (item.ID <= 0)
                    {
                        await _notificationTypeLinkRepo.CreateAsync(item);
                    }
                    else
                    {
                        var data = _notificationTypeLinkRepo.GetByID(item.ID);
                        if (item.IsSelected == data.IsSelected) continue;
                        await _notificationTypeLinkRepo.UpdateAsync(item);
                    }
                }
                return Ok(ApiResponseFactory.Success(notificationTypeLink, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }   

    }
}
