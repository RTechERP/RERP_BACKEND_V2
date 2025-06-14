using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HolidayController : Controller
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

        [HttpPost]
        public async Task<IActionResult> SaveHoliday([FromBody] Holiday holiday)
        {
            try
            {
                List<Holiday> holidays = holidayRepo.GetAll().Where(x => x.IsDeleted == false).ToList();
                if(holidays.Any(x => x.HolidayDate == holiday.HolidayDate && x.ID != holiday.ID))
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Ngày nghỉ đã tồn tại"
                    });
                }
                if (holiday.ID <= 0)
                {
                    await holidayRepo.CreateAsync(holiday);
                }
                else
                {
                    holidayRepo.UpdateFieldsByID(holiday.ID, holiday);
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
