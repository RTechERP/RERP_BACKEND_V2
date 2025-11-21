using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Project
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProjectFieldController : ControllerBase
    {
        private readonly ProjectFieldRepo _projectFieldRepo;
        public ProjectFieldController(
          ProjectFieldRepo projectFieldRepo)
        {
            _projectFieldRepo = projectFieldRepo;
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllProjectFields()
        {
            try
            {
                var projectFields = _projectFieldRepo.GetAll(x => x.IsDeleted == false);
                return Ok(ApiResponseFactory.Success(projectFields, "Lấy dữu liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveProjectField([FromBody] List<BusinessField> dto)
        {
            try
            {

                if (dto == null || dto.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu gửi lên không hợp lệ"));
                }
                foreach (var item in dto)
                {
                    if (item.IsDeleted == false)
                    {
                        var checkcode = _projectFieldRepo.GetAll(x => x.Code == item.Code && x.ID != item.ID && x.IsDeleted == false);
                        if (checkcode.Count > 0)
                        {
                            return Ok(new
                            {
                                status = 2,
                                message = "Mã lĩnh vực đã tồn tại!"
                            });

                        }
                    }

                    if (item.ID <= 0)
                    {
                        await _projectFieldRepo.CreateAsync(item);
                    }
                    else
                    {
                        await _projectFieldRepo.UpdateAsync(item);
                    }
                }
                return Ok(ApiResponseFactory.Success(null, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
