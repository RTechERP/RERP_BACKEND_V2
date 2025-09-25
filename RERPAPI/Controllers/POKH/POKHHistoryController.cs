using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
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
    public class POKHHistoryController : ControllerBase
    {
        POKHHistoryRepo _pokhHistoryRepo = new POKHHistoryRepo();
        // GET: api/<POKHistoryController>
        [HttpGet]
        public IActionResult Get( DateTime startDate, DateTime endDate, string cusCode,string keywords = "")
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetPOKHHistory",
                    new string[] { "@Keywords", "@PODateStart", "@PODateEnd", "@CustomerCode" },
                    new object[] { keywords, startDate, endDate, cusCode });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }   
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> Save([FromBody] List<POKHHistory> pokhHistorys)
        {
            try
            {   
                foreach(var item in pokhHistorys)
                {
                    await _pokhHistoryRepo.CreateAsync(item);

                }
                return Ok(ApiResponseFactory.Success(null, "POKH history added successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }
    }
}
