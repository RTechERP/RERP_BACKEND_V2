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
        HolidayRepo holidayRepo = new HolidayRepo();


        [HttpGet]
        public IActionResult GetHoliday(int month, int year)
        {
            try
            {
                var holidays = SQLHelper<object>.ProcedureToList("spGetHoliday",
                                                           new string[] { "@Month", "@Year"},
                                                                new object[] { month, year});
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(holidays, 0)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        //[HttpGet("schedule-work")]
        //public IActionResult GetEmployeeScheduleWork(int month, int year)
        //{
        //    try
        //    {
        //        var dtScheduleWork = SQLHelper<object>.ProcedureToList("spGetEmployeeScheduleWorkByDate",
        //                                                   new string[] {  "@Year" , "@Month"},
        //                                                        new object[] { year, month });
        //        return Ok(new
        //        {
        //            status = 1,
        //            data = SQLHelper<object>.GetListData(dtScheduleWork, 0)
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new
        //        {
        //            status = 0,
        //            message = ex.Message,
        //            error = ex.ToString()
        //        });
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> SaveHoliday([FromBody] Holiday holiday)
        {
            try
            {
                var existingHoliday = holidayRepo.GetAll()
                    .Where(x => x.HolidayYear == holiday.HolidayYear && x.HolidayDay == holiday.HolidayDay && x.HolidayMonth == holiday.HolidayMonth && x.ID != holiday.ID);

                if (existingHoliday.Any())
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Ngày nghỉ này đã có. Vui lòng chọn ngày khác!"
                    });
                }

                if (holiday.ID <= 0)
                {
                    await holidayRepo.CreateAsync(holiday);
                }
                else
                {
                    await holidayRepo.UpdateAsync(holiday);
                }
                return Ok(new
                {
                    status = 1,
                    data = holiday,
                    message = "Lưu thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

    }
}
