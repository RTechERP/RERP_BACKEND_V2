using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.GeneralCategory
{
    [Route("api/[controller]")]
    [ApiController]
    public class FiveSRatingController : ControllerBase
    {
        private readonly FiveSRatingRepo _fiveSRatingRepo;
        private CurrentUser _currentUser;

        public FiveSRatingController(FiveSRatingRepo fiveSRatingRepo, CurrentUser currentUser)
        {
            _fiveSRatingRepo = fiveSRatingRepo;
            _currentUser = currentUser;
        }

        [HttpGet("get-all")]
        [Authorize]
        public IActionResult GetAll(int? yearValue, int? monthValue, string? keyword)
        {
            try
            {
                var arrParamName = new string[] { "@YearValue", "@MonthValue", "@Keyword" };
                var arrParamValue = new object[] { 
                    yearValue ?? (object)DBNull.Value, 
                    monthValue ?? (object)DBNull.Value, 
                    keyword ?? (object)DBNull.Value 
                };
                
                var dataSet = SQLHelper<object>.ProcedureToList("spGetFiveSRatingList", arrParamName, arrParamValue);
                var result = SQLHelper<object>.GetListData(dataSet, 0);

                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [Authorize]
        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] FiveSRating item)
        {
            try
            {
                if (item == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu"));
                int result = 0;
                if (item.ID > 0)
                {
                    result = await _fiveSRatingRepo.UpdateAsync(item);
                }
                else
                {
                    // Kiểm tra xem ngày này đã có đợt đánh giá nào chưa
                    if (item.RatingDate.HasValue)
                    {
                        var dateOnly = item.RatingDate.Value.Date;
                        bool exists = await _fiveSRatingRepo.ExistsAsync(x => x.RatingDate.Value.Date == dateOnly && x.IsDeleted != true);
                        if (exists)
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, $"Bạn đã tạo đợt chấm điểm cho ngày {dateOnly:dd/MM/yyyy}!"));
                        }
                    }

                    // Tự sinh mã Code khi thêm mới
                    int year = item.YearValue ?? DateTime.Now.Year;
                    int month = item.MonthValue ?? DateTime.Now.Month;
                    item.Code = await _fiveSRatingRepo.GenerateCodeAsync(year, month);

                    result = await _fiveSRatingRepo.CreateAsync(item);
                }
                if (result > 0)
                    return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
                else
                    return BadRequest(ApiResponseFactory.Fail(null, "Lưu dữ liệu không thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [Authorize]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] FiveSRating item)
        {
            try
            {
                if (item == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu"));
                item.IsDeleted = true;
                int result = await _fiveSRatingRepo.UpdateAsync(item);

                if (result > 0)
                    return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
                else
                    return BadRequest(ApiResponseFactory.Fail(null, "Xóa dữ liệu không thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
