using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

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
                List<Firm> dataFirm = _firmRepo.GetAll(x => x.IsDelete != true);
                return Ok(ApiResponseFactory.Success(dataFirm, ""));
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> saveDataFirm([FromBody] List<Firm> dtos)
        {
            try
            {
                foreach (var dto in dtos)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy hãng"));
                }
                return Ok(new
                {
                    status = 1,
                    message = "Thêm hãng thành công!",

                });

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteFirm([FromBody] List<int> ids)
        {
            try
            {
                foreach (var item in ids)
                {
                    var firm = await _firmRepo.GetByIDAsync(item);
                    if (firm == null)
                        return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy hãng có ID = {item}"));

                    firm.IsDelete = true;
                    await _firmRepo.UpdateAsync(firm);
                }

                return Ok(ApiResponseFactory.Success(null, "Xóa hãng thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}
