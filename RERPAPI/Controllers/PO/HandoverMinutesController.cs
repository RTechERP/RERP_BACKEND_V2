using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers.PO
{
    [Route("api/[controller]")]
    [ApiController]
    public class HandoverMinutesController : ControllerBase
    {
        HandoverMinutesRepo _handoverMinutesRepo = new HandoverMinutesRepo();
        HandoverMinutesDetailRepo _handoverMinutesDetailRepo = new HandoverMinutesDetailRepo();
        [HttpGet]
        public IActionResult Get(DateTime dateStart, DateTime dateEnd, string keyWords = "")
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToDynamicLists("spGetAllHandoverMinutes", new string[] { "@DateStart", "@DateEnd", "@KeyWords" }, new object[] { dateStart, dateEnd, keyWords });
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<dynamic>.GetListData(list, 0)
                });
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
        [HttpGet("get-details")]
        public IActionResult GetDetail(int id)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToDynamicLists("spGetHanoverMinutesDetail", new string[] { "@HandoverMinutesID" }, new object[] { id });
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<dynamic>.GetListData(list, 0)
                });
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
        [HttpDelete("{id}")]
        public IActionResult Delete(int id) 
        {
            try
            {
                _handoverMinutesRepo.UpdateFieldsByID(id, new HandoverMinute { IsDeleted = true }); 
                return Ok(new
                {
                    status = 1,
                });
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
    }
}
