using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static NPOI.HSSF.Util.HSSFColor;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class AppMobileVersionController : ControllerBase
    {
        private readonly AppMobileVersionRepo _appMobileVersionRepo;

        public AppMobileVersionController(AppMobileVersionRepo appMobileVersionRepo)
        {
            _appMobileVersionRepo = appMobileVersionRepo;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var versions = _appMobileVersionRepo.GetAll();
            return Ok(ApiResponseFactory.Success(new { versions }, "Lấy dữ liệu version thành công"));
        }

        [HttpPut]
        public IActionResult Update([FromBody] AppMobileVersion version)
        {
            var existingVersion = _appMobileVersionRepo.GetByID(version.ID);
            if (existingVersion == null)
            {
                return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy version"));
            }
            _appMobileVersionRepo.Update(version);
            return Ok(ApiResponseFactory.Success(new { version }, "Cập nhật phiên bản ứng dụng thành công"));
        }
    }
}
