using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class FirmController : ControllerBase
    {
        private readonly FirmRepo _firmRepo;

        public FirmController(FirmRepo firmRepo)
        {
            _firmRepo = firmRepo;
        }
        [HttpGet("")]
        public IActionResult getDataFirm(int firmType)
        {
            try
            {
                List<Firm> dataFirm = _firmRepo.GetAll(x => x.IsDelete != true && (x.FirmType == firmType || firmType == 0));
                return Ok(ApiResponseFactory.Success(dataFirm, "Lấy dữ liệu hãng thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-firm-code")]
        public IActionResult GetFirmCode(int firmType = 2)
        {
            string firmcode = _firmRepo.GenerateCode(firmType);
            return Ok(ApiResponseFactory.Success(firmcode, ""));
        }
        [HttpGet("check-code")]
        public IActionResult CheckFirmCodeExists([FromQuery] string firmCode, [FromQuery] int? id = null)
        {
            try
            {
                // Kiểm tra xem mã hãng đã tồn tại chưa
                if (firmCode == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Mã hãng không được để trống!"));
                }
                var existingFirm = _firmRepo.GetAll().FirstOrDefault(f =>
                    f.FirmCode.ToLower() == firmCode.ToLower() &&
                    (id == null || f.ID != id));

                return Ok(new { exists = existingFirm != null });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteMultipleFirms([FromBody] List<int> firmIds)
        {
            try
            {
                if (firmIds == null || firmIds.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có ID hãng nào được cung cấp!"));
                }

                int successCount = 0;
                List<string> failedIds = new List<string>();

                foreach (var id in firmIds)
                {
                    try
                    {
                        var firm = _firmRepo.GetByID(id);
                        if (firm != null)
                        {
                            firm.IsDelete = true;
                            await _firmRepo.UpdateAsync(firm);
                            successCount++;
                        }
                        else
                        {
                            failedIds.Add(id.ToString());
                        }
                    }
                    catch
                    {
                        failedIds.Add(id.ToString());
                    }
                }

                string message = $"Đã xóa thành công {successCount}/{firmIds.Count} hãng.";
                if (failedIds.Count > 0)
                {
                    message += $" Không thể xóa các hãng có ID: {string.Join(", ", failedIds)}";
                }

                return Ok(ApiResponseFactory.Success(new { status = 1, successCount, failedIds }, message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("")]
        public async Task<IActionResult> saveDataFirm([FromBody] Firm firmData)
        {
            try
            {
                // Kiểm tra xem mã hãng đã tồn tại chưa
                var existingFirm = _firmRepo.GetAll().FirstOrDefault(f =>
                    f.FirmCode.ToLower() == firmData.FirmCode.ToLower() &&
                    f.ID != firmData.ID);

                if (existingFirm != null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Mã hãng đã tồn tại. Vui lòng nhập mã khác!"));
                }

                if (firmData.ID <= 0)
                {

                    firmData.FirmCode = _firmRepo.GenerateCode(firmData.FirmType ?? 0);
                    await _firmRepo.CreateAsync(firmData);
                }
                else
                    await _firmRepo.UpdateAsync(firmData);

                return Ok(ApiResponseFactory.Success(new { status = 1 }, "Lưu dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //[HttpPost("save-data")]
        //public async Task<IActionResult> saveDataFirm([FromBody] List<FirmDTO> dtos)
        //{
        //    try
        //    {
        //        foreach (var dto in dtos)
        //        {
        //            if (dto.ID <= 0) await _firmRepo.CreateAsync(dto);
        //            else await _firmRepo.UpdateAsync(dto);

        //        }
        //        return Ok(ApiResponseFactory.Success(null, "Xử lý dữ liệu thành công!"));

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}

    }
}
