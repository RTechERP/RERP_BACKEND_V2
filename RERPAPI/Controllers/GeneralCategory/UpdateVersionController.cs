using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM;
using RERPAPI.SendService;

namespace RERPAPI.Controllers.GeneralCategory
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class UpdateVersionController : ControllerBase
    {
        private readonly UpdateVersionRepo _updateVersionRepo;
        private readonly SseService _sseService;
        public UpdateVersionController(UpdateVersionRepo updateVersionRepo, SseService sseService)
        {
            _updateVersionRepo = updateVersionRepo;
            _sseService = sseService;
        }
        //lấy danh sách update phiên bản
        //       [RequiresPermission("N1,N34")]
        [HttpGet("get-update-version")]
        public IActionResult GetEconomicContractTerms()
        {
            try
            {
                var year = DateTime.Now.Year;
                var data = _updateVersionRepo.GetAll(x => x.IsDeleted != true).OrderByDescending(x=>x.CreatedDate);
                var nextCode = $"{year}.{data.Count(x => x.CreatedDate?.Year == year) + 1}";
                return Ok(ApiResponseFactory.Success(new { data, nextCode }, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-current-version")]
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
        //lưu hợp đồng

        [HttpPost("save-version")]
        public async Task<IActionResult> SaveContract([FromBody] UpdateVersion item)
        {
            try
            {
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
                                        message = "Hợp đồng đã được duyệt",
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
        [HttpGet("sse/contracts")]
        public async Task GetContractSse()
        {
            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            await _sseService.AddClientAsync(Response);
        }
    }
}
