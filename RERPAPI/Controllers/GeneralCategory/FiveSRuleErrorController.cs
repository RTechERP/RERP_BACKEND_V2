using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.GeneralCategory
{
    [Route("api/[controller]")]
    [ApiController]

    public class FiveSRuleErrorController : ControllerBase
    {
        private readonly FiveSRuleErrorRepo _fiveSRuleErrorRepo;
        private CurrentUser _currentUser;

        public FiveSRuleErrorController(FiveSRuleErrorRepo fiveSRuleErrorRepo, CurrentUser currentUser)
        {
            _fiveSRuleErrorRepo = fiveSRuleErrorRepo;
            _currentUser = currentUser;
        }

        [HttpGet("get-all")]
        [Authorize]
        public IActionResult GetAll()
        {
            try
            {
                var data = _fiveSRuleErrorRepo.GetAll(x => x.IsDeleted != true).OrderBy(x => x.ID);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-by-error-id/{fiveSErrorId}")]
        [Authorize]
        public IActionResult GetByErrorId(int fiveSErrorId)
        {
            try
            {
                var data = _fiveSRuleErrorRepo.GetAll(x => x.FiveSErrorID == fiveSErrorId && x.IsDeleted != true).OrderBy(x => x.ID);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [Authorize]
        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] FiveSRuleError item)
        {
            try
            {
                if (item == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu"));
                }

                int result = 0;
                if (item.ID > 0)
                {
                    result = await _fiveSRuleErrorRepo.UpdateAsync(item);
                }
                else
                {
                    result = await _fiveSRuleErrorRepo.CreateAsync(item);
                }

                if (result > 0)
                {
                    return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
                }
                else
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Lưu dữ liệu không thành công"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [Authorize]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] FiveSRuleError item)
        {
            try
            {
                if (item == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu"));
                }

                item.IsDeleted = true;
                int result = await _fiveSRuleErrorRepo.UpdateAsync(item);

                if (result > 0)
                {
                    return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
                }
                else
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Xóa dữ liệu không thành công"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
