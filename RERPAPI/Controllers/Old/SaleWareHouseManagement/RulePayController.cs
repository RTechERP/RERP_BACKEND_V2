using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.PurchasingManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class RulePayController : ControllerBase
    {

        RulePayRepo _rulePayRepo = new RulePayRepo();

        [HttpGet("")]
        public async Task<IActionResult> getAllRulePay()
        {
            try
            {
                List<RulePay> data = _rulePayRepo.GetAll().Where(r => r.IsDeleted != true).ToList();
                return Ok(ApiResponseFactory.Success(data, "Lấy danh sách RulePay thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Có lỗi xảy ra khi lấy danh sách RulePay."));

            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getAllRulePayByID(int id)
        {
            try
            {
                RulePay data = _rulePayRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(data, "Thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Có lỗi xảy ra."));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveDataRulePay([FromBody] RulePayDTO request)
        {
            if (request == null)
            {
                return BadRequest(ApiResponseFactory.Fail(null, "Request không hợp lệ."));
            }

            try
            {
                // Trường hợp xóa
                if (request.DeleteIds != null && request.DeleteIds.Any())
                {
                    try
                    {
                        foreach (var id in request.DeleteIds)
                        {
                            var item = _rulePayRepo.GetByID(id);
                            if (item != null)
                            {
                                item.IsDeleted = true;
                                await _rulePayRepo.UpdateAsync(item);
                            }
                        }

                        return Ok(ApiResponseFactory.Success(null, "Xóa dữ liệu thành công."));
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ApiResponseFactory.Fail(ex, "Lỗi khi xóa dữ liệu."));
                    }
                }

                // Trường hợp thêm/sửa
                if (request.Data == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ."));
                }

                try
                {
                    var dst = request.Data;
                    var rulePayCheck = _rulePayRepo.GetAll(x => x.Code.ToLower() == dst.Code.ToLower())
                        .FirstOrDefault();

                    if (rulePayCheck == null)
                    {
                        await _rulePayRepo.CreateAsync(dst);
                    }
                    else if (rulePayCheck.ID == dst.ID)
                    {
                        await _rulePayRepo.UpdateAsync(dst);
                    }
                    else
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Mã đã tồn tại."));
                    }

                    return Ok(ApiResponseFactory.Success(dst, "Lưu dữ liệu thành công."));
                }
                catch (Exception ex)
                {
                    return BadRequest(ApiResponseFactory.Fail(ex, "Lỗi khi lưu dữ liệu."));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Lỗi không xác định."));
            }
        }


    }
}