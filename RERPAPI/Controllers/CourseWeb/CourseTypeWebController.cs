using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities.RTCCourse;
using RERPAPI.Repo.GenericCourseEntity;

namespace RERPAPI.Controllers.KHOAHOC
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CourseTypeWebController : ControllerBase
    {
        CourseTypeRepo _courseTypeRepo;
        public CourseTypeWebController(CourseTypeRepo courseTypeRepo)
        {
            _courseTypeRepo = courseTypeRepo;
        }

        //get data 
        [HttpGet("get-all")]
        public async Task<IActionResult> getAllData()
        {
            try
            {
                List<CourseType> data = _courseTypeRepo.GetAll(x => x.IsDeleted == false);
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
        public async Task<IActionResult> saveData([FromBody] List<CourseType> request)
        {
            try 
            {
                foreach (var item in request)
                {
                    if (item.IsDeleted == true && item.ID > 0)
                    {
                        var dataDel = _courseTypeRepo.GetByID(item.ID);
                        dataDel.IsDeleted = true;
                        await _courseTypeRepo.UpdateAsync(dataDel);
                    }
                    else
                    {
                        if (!_courseTypeRepo.ValidateCourseType(item, out string message))
                        {
                            return BadRequest(ApiResponseFactory.Fail(null,message));
                        }
                        if (item.ID <= 0)
                        {
                            await _courseTypeRepo.CreateAsync(item);
                        }
                        else
                        {
                            await _courseTypeRepo.UpdateAsync(item);
                        }
                    }
                }
                return Ok(ApiResponseFactory.Success(null,
                    "Lưu dữ liệu thành công!"
                ));
            }
            catch (Exception ex) 
            {
                return BadRequest(ApiResponseFactory.Fail(ex,ex.Message));
            }
            

        }
    }
}
