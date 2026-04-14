using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.GeneralCategory
{
    [Route("api/[controller]")]
    [ApiController]

    public class FiveSErrorController : ControllerBase
    {
        private readonly FiveSErrorRepo _fiveSErrorRepo;
        private readonly FiveSRuleErrorRepo _fiveSRuleErrorRepo;
        private CurrentUser _currentUser;

        public FiveSErrorController(FiveSErrorRepo fiveSErrorRepo, FiveSRuleErrorRepo fiveSRuleErrorRepo, CurrentUser currentUser)
        {
            _fiveSErrorRepo = fiveSErrorRepo;
            _fiveSRuleErrorRepo = fiveSRuleErrorRepo;
            _currentUser = currentUser;
        }

        [HttpGet("get-all")]
        [Authorize]
        public IActionResult GetAll()
        {
            try
            {
                var data = _fiveSErrorRepo.GetAll(x => x.IsDeleted != true).OrderBy(x => x.STT).ThenBy(x => x.ID);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-next-stt/{typeError}")]
        [Authorize]
        public IActionResult GetNextSTT(int typeError)
        {
            try
            {
                var maxSTT = _fiveSErrorRepo.GetAll(x => x.TypeError == typeError && x.IsDeleted != true)
                    .Select(x => (int?)x.STT)
                    .Max() ?? 0;
                return Ok(ApiResponseFactory.Success(maxSTT + 1, "Lấy STT tiếp theo thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [Authorize]
        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] FiveSErrorSaveDTO dto)
        {
            try
            {
                if (dto == null || dto.FiveSError == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu"));
                }

                var item = dto.FiveSError;
                int result = 0;
                if (item.ID > 0)
                {
                    result = await _fiveSErrorRepo.UpdateAsync(item);
                }
                else
                {
                    result = await _fiveSErrorRepo.CreateAsync(item);
                }

                if (result > 0)
                {
                    // Xóa mềm rules cũ liên quan
                    var oldRules = _fiveSRuleErrorRepo.GetAll(x => x.FiveSErrorID == item.ID && x.IsDeleted != true);
                    foreach (var oldRule in oldRules)
                    {
                        oldRule.IsDeleted = true;
                        await _fiveSRuleErrorRepo.UpdateAsync(oldRule);
                    }

                    // Tạo rules mới theo danh sách mức độ được gửi từ Frontend
                    if (dto.FiveSRuleErrors != null && dto.FiveSRuleErrors.Count > 0)
                    {
                        foreach (var rule in dto.FiveSRuleErrors)
                        {
                            var newRule = new FiveSRuleError
                            {
                                FiveSErrorID = item.ID,
                                RatingLevels = rule.RatingLevels,
                                Description = rule.Description,
                                Point = rule.Point,
                                TypePoint = rule.TypePoint,
                                BonusPoint = rule.BonusPoint,
                                MinusPoint = rule.MinusPoint,
                                Name = rule.RatingLevels                            
                            };

                            await _fiveSRuleErrorRepo.CreateAsync(newRule);
                        }
                    }

                    return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
                }
                else
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Lưu dữ liệu không thành công"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [Authorize]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] FiveSError item)
        {
            try
            {
                if (item == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu"));
                }

                item.IsDeleted = true;
                int result = await _fiveSErrorRepo.UpdateAsync(item);

                if (result > 0)
                {
                    return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
                }
                else
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Xóa dữ liệu không thành công"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

public class FiveSErrorSaveDTO
{
    public FiveSError FiveSError { get; set; }
    public List<FiveSRuleError>? FiveSRuleErrors { get; set; }
}
