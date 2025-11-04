using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class FirmController : ControllerBase
    {
        FirmRepo _firmRepo = new FirmRepo();

        [HttpGet("")]
        public IActionResult GetFirms()
        {
            try
            {
                List<Firm> dataFirm = _firmRepo.GetAll();
                return Ok(ApiResponseFactory.Success(dataFirm, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetFirmById(int id)
        {
            try
            {
                var firm = _firmRepo.GetByID(id);
                if (firm == null)
                {
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy hãng"));
                }

                return Ok(ApiResponseFactory.Success(firm, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("check-code")]
        public IActionResult CheckFirmCodeExists(string firmCode, int? id = null)
        {
            try
            {
                bool exists = _firmRepo.CheckFirmCodeExists(firmCode, id);
                return Ok(ApiResponseFactory.Success(exists, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("")]
        public async Task<IActionResult> SaveFirm([FromBody] Firm firm)
        {
            try
            {
                if (firm.ID <= 0)
                    await _firmRepo.CreateAsync(firm);
                else
                    await _firmRepo.UpdateAsync(firm);

                return Ok(ApiResponseFactory.Success(null, "Lưu hãng thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFirm(int id)
        {
            try
            {
                var firm = _firmRepo.GetByID(id);
                if (firm == null)
                {
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy hãng"));
                }

                // Soft delete: cập nhật trường IsDelete thành true
                firm.IsDelete = true;
                await _firmRepo.UpdateAsync(firm);

                return Ok(ApiResponseFactory.Success(null, "Xóa hãng thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
