using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Course
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseTypeController : ControllerBase
    {
        CoureTypeRepo _coureTypeRepo;
        public CourseTypeController(CoureTypeRepo coureTypeRepo)
        {
            _coureTypeRepo = coureTypeRepo;
        }

        //get data 
        [HttpGet("get-all")]
        public async Task<IActionResult> getAllData()
        {
            try
            {
                List<CourseType> data = _coureTypeRepo.GetAll(x => x.IsDeleted == false);
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
                        var dataDel = _coureTypeRepo.GetByID(item.ID);
                        dataDel.IsDeleted = true;
                        await _coureTypeRepo.UpdateAsync(dataDel);
                    }
                    else
                    {
                        if (!_coureTypeRepo.ValidateCourseType(item, out string message))
                        {
                            return BadRequest(ApiResponseFactory.Fail(null,message));
                        }
                        if (item.ID <= 0)
                        {
                            await _coureTypeRepo.CreateAsync(item);
                        }
                        else
                        {
                            await _coureTypeRepo.UpdateAsync(item);
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
