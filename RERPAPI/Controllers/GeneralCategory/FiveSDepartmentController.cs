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
    public class FiveSDepartmentController : ControllerBase
    {
        private readonly FiveSDepartmentRepo _fiveSDepartmentRepo;
        private CurrentUser _currentUser;

        public FiveSDepartmentController(FiveSDepartmentRepo fiveSDepartmentRepo, CurrentUser currentUser)
        {
            _fiveSDepartmentRepo = fiveSDepartmentRepo;
            _currentUser = currentUser;
        }

        [HttpGet("get-all")]
        [Authorize]
        public IActionResult GetAll()
        {
            try
            {
                var data = _fiveSDepartmentRepo.GetAll(x => x.IsDeleted != true).OrderBy(x => x.STT).ThenBy(x => x.ID);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-next-stt")]
        [Authorize]
        public IActionResult GetNextSTT()
        {
            try
            {
                var maxSTT = _fiveSDepartmentRepo.GetAll(x => x.IsDeleted != true)
                    .Select(x => (int?)x.STT)
                    .Max() ?? 0;
                return Ok(ApiResponseFactory.Success(maxSTT + 1, "Lấy STT tiếp theo thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [Authorize]
        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] FiveSDepartment item)
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
                    item.UpdatedBy = _currentUser.LoginName;
                    item.UpdatedDate = DateTime.Now;
                    result = await _fiveSDepartmentRepo.UpdateAsync(item);
                }
                else
                {
                    item.CreatedBy = _currentUser.LoginName;
                    item.CreatedDate = DateTime.Now;
                    result = await _fiveSDepartmentRepo.CreateAsync(item);
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
        public async Task<IActionResult> Delete([FromBody] FiveSDepartment item)
        {
            try
            {
                if (item == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu"));
                }

                item.IsDeleted = true;
                int result = await _fiveSDepartmentRepo.UpdateAsync(item);

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
