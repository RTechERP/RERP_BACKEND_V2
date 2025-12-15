using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;

namespace RERPAPI.Controllers.Old.KETOAN
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HistoryExportAccountantController : ControllerBase
    {
        [HttpGet("load-data")]
        public IActionResult GetData(int page, int size, DateTime dateStart, DateTime dateEnd, int status, string filterText = "" )
        {
            try
            {
                var result = SQLHelper<dynamic>.ProcedureToList("spGetHistoryExportAcountant",
                                new string[] { "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@FilterText", "@Status" },
                                new object[] { page, size, dateStart, dateEnd, filterText, status });

                var data = SQLHelper<dynamic>.GetListData(result, 0);
                var totalPage = SQLHelper<dynamic>.GetListData(result, 1);
                return Ok(ApiResponseFactory.Success(new { data, totalPage }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
