using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Repo.GenericEntity.GeneralCatetogy;

namespace RERPAPI.Controllers
{
    // NDNhat Update 03/08/2026: đọc cấu hình chung từ dbo.BusinessConfig — ban đầu dùng để lấy
    // danh sách DepartmentID thuộc Phòng Sale (ConfigType = 1), thay cho việc hardcode mảng
    // DepartmentID rải rác ở nhiều nơi FE (Đăng ký công tác, Đặt xe, trang master).
    [ApiController]
    [Route("api/[controller]")]
    public class BusinessConfigController : ControllerBase
    {
        private BusinessConfigRepo _businessConfigRepo;

        public BusinessConfigController(BusinessConfigRepo businessConfigRepo)
        {
            _businessConfigRepo = businessConfigRepo;
        }

        [HttpGet("get-department-ids")]
        public IActionResult GetDepartmentIds(int configType)
        {
            try
            {
                var departmentIDs = _businessConfigRepo.GetDepartmentIDsByConfigType(configType);
                return Ok(ApiResponseFactory.Success(departmentIDs, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
