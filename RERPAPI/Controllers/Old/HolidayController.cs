using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [ApiController]
    [Route("api/[controller]")]
    public class HolidayController : ControllerBase
    {
        private readonly HolidayRepo _holidayRepo;

        public HolidayController(HolidayRepo holidayRepo)
        {
            _holidayRepo = holidayRepo;
        }


        [HttpGet]
        public IActionResult GetHoliday(int month, int year)
        {
            try
            {
                var holidays = SQLHelper<object>.ProcedureToList("spGetHoliday",
                                                                new string[] { "@Month", "@Year" },
                                                                new object[] { month, year });

                var data = new
                {
                    holidays = SQLHelper<object>.GetListData(holidays, 0),
                    scheduleWorkSaturdays = SQLHelper<object>.GetListData(holidays, 1),
                };

                return Ok(ApiResponseFactory.Success(data, ""));


            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveHoliday([FromBody] Holiday holiday)
        {
            try
            {
                var existingHoliday = _holidayRepo.GetAll(
                    x => x.HolidayYear == holiday.HolidayYear && x.HolidayDay == holiday.HolidayDay && x.HolidayMonth == holiday.HolidayMonth && x.ID != holiday.ID);

                if (existingHoliday.Any()) return BadRequest(ApiResponseFactory.Fail(null, "Ngày nghỉ này đã có. Vui lòng chọn ngày khác!"));
                holiday.HolidayName = holiday.HolidayName.Trim();
                if (holiday.ID <= 0) await _holidayRepo.CreateAsync(holiday);
                else await _holidayRepo.UpdateAsync(holiday);
                return Ok(ApiResponseFactory.Success(holiday, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete-holiday")]
        public async Task<IActionResult> DeleteHoliday([FromBody] int holidayId)
        {
            try
            {
                if(holidayId <= 0) return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy ID xóa phòng ban!"));
                await _holidayRepo.DeleteAsync(holidayId);
                return Ok(ApiResponseFactory.Success(null, "Đã xóa ngày nghỉ!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}
