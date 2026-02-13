using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM;
using RERPAPI.SendService;

namespace RERPAPI.Controllers.GeneralCategory
{
    [Route("api/[controller]")]
    [ApiController]
 
    public class UpdateVersionController : ControllerBase
    {
        private readonly UpdateVersionRepo _updateVersionRepo;
        private readonly SseService _sseService;
        private CurrentUser _currentUser;
        public UpdateVersionController(UpdateVersionRepo updateVersionRepo, SseService sseService, CurrentUser currentUser)
        {
            _updateVersionRepo = updateVersionRepo;
            _sseService = sseService;
            _currentUser = currentUser;
        }
        //lấy danh sách update phiên bản
        //       [RequiresPermission("N1,N34")]
        [HttpGet("get-update-version")]
        [Authorize]
        public IActionResult GetUpdateVersion()
        {
            try
            {
                var year = DateTime.Now.Year;
                var data = _updateVersionRepo.GetAll(x => x.IsDeleted != true).OrderByDescending(x=>x.CreatedDate);
                var dataNextCode = _updateVersionRepo.GetAll().OrderByDescending(x => x.CreatedDate);
                var nextCode = $"{year}.{dataNextCode.Count(x => x.CreatedDate?.Year == year) + 1}";
                return Ok(ApiResponseFactory.Success(new { data, nextCode }, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-current-version")]
        [Authorize]
        public IActionResult GetLastVersion()
        {
            try
            {
                var year = DateTime.Now.Year;
                var data = _updateVersionRepo.GetAll(x => x.IsDeleted != true&& x.Status==1).OrderByDescending(x => x.CreatedDate).FirstOrDefault()?? new UpdateVersion();
                var currentVersion = data.Code;
              
                return Ok(ApiResponseFactory.Success(currentVersion, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //lưu phiên bản

        [Authorize]
        [HttpPost("save-version")]
        public async Task<IActionResult> SaveVersion([FromBody] UpdateVersion item)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                _currentUser = ObjectMapper.GetCurrentUser(claims);
                bool isAdmin = _currentUser.IsAdmin && _currentUser.EmployeeID <= 0;
                if (!isAdmin) return BadRequest(ApiResponseFactory.Fail(null, "Bạn không có quyền update phiên bản!"));
                if (item == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu"));
                }
                else
                {
                    //if (term != null && term.IsDeleted != true)
                    //{
                    //    var validate = _economicContractTermRepo.Validate(term);
                    //    if (validate.status == 0) return BadRequest(validate);
                    //}
                    int result = 0;
                    if (item != null)
                    {
                        if (item.ID > 0)
                        {
                            if(item.Status==1)
                            {
                                item.PublicDate = DateTime.Now;
                            }    
                            result = await _updateVersionRepo.UpdateAsync(item);
                           
                        }
                        else
                        {
                            result = await _updateVersionRepo.CreateAsync(item);
                        }
                        if(result>0)
                        {
                            if (item.Status == 1)
                            {
                                await _sseService.SendEventAsync(
                                    "contract-updated",
                                    new
                                    {
                                        id = item.ID,
                                        code = item.Code,
                                        content=item.Content,
                                        status = item.Status,
                                        message = "Phiên bản đã được publish",
                                        time = DateTime.Now
                                    }
                                );
                            }

                            return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
                            
                        }  
                        else
                        {   
                            return BadRequest(ApiResponseFactory.Fail(null, "Lưu dữ liệu không thành công"));
                        }    
                      
                    }
                    return BadRequest(ApiResponseFactory.Fail(null, "Lưu dữ liệu không thành công"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("sse/update-version")]
        public async Task GetContractSse()
        {
            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            await _sseService.AddClientAsync(Response);
        }
    }
}
