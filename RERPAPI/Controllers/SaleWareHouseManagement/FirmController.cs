using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using ZXing;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class FirmController : ControllerBase
    {
        FirmRepo _firmRepo = new FirmRepo();
        [HttpGet("")]
        public IActionResult getDataFirm()
        {
            try
            {
                List<Firm> dataFirm = _firmRepo.GetAll();
                return Ok(ApiResponseFactory.Success(dataFirm, "Lấy dữ liệu hãng thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> saveDataFirm([FromBody] List<FirmDTO> dtos)
        {
            try
            {
                foreach (var dto in dtos)
                {
                    if (dto.ID <= 0) await _firmRepo.CreateAsync(dto);
                    else await _firmRepo.UpdateAsync(dto);

                }
                return Ok(ApiResponseFactory.Success(null, "Xử lý dữ liệu thành công!"));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
