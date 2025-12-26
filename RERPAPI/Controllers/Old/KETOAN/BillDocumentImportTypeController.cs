using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;

namespace RERPAPI.Controllers.Old.KETOAN
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BillDocumentImportTypeController : ControllerBase
    {

        [HttpGet("get-data")]
        public IActionResult Get(int page, int size, DateTime dateStart, DateTime dateEnd, int billDocumentImportType, string keyword = "")
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetBillImportType",
                    new string[] { "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@FilterText", "@BillDocumentImportType" },
                    new object[] { page, size, dateStart, dateEnd, keyword, billDocumentImportType });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
