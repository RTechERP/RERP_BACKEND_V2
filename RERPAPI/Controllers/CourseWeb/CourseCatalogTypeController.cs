using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities.RTCCourse;
using RERPAPI.Repo.GenericCourseEntity;

namespace RERPAPI.Controllers.CourseWeb
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CourseCatalogTypeController : ControllerBase
    {
        private CourseCatalogTypeRepo _courseCatalogTypeRepo;

        public CourseCatalogTypeController(CourseCatalogTypeRepo courseCatalogTypeRepo)
        {
            _courseCatalogTypeRepo = courseCatalogTypeRepo;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> getAllData()
        {
            try
            {
                List<CourseCatalogType> data = _courseCatalogTypeRepo.GetAll(x => x.IsDeleted == false);
                return Ok(ApiResponseFactory.Success(data,
                   "Lấy dữ liệu loại khóa học thành công!"
               ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(
                    ex,
                    ex.Message
                ));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> saveData([FromBody] List<CourseCatalogType> request)
        {
            try
            {
                foreach (var item in request)
                {
                    if (item.IsDeleted == true && item.ID > 0)
                    {
                        var dataDel = _courseCatalogTypeRepo.GetByID(item.ID);
                        dataDel.IsDeleted = true;
                        await _courseCatalogTypeRepo.UpdateAsync(dataDel);
                    }
                    else
                    {
                        if (!_courseCatalogTypeRepo.ValidateCourseType(item, out string message))
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, message));
                        }
                        if (item.ID <= 0)
                        {
                            await _courseCatalogTypeRepo.CreateAsync(item);
                        }
                        else
                        {
                            await _courseCatalogTypeRepo.UpdateAsync(item);
                        }
                    }
                }
                return Ok(ApiResponseFactory.Success(null,
                    "Lưu dữ liệu thành công!"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}