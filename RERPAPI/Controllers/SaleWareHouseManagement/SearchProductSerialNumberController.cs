using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchProductSerialNumberController : ControllerBase
    {
        [HttpGet("")]
        public IActionResult getAll(string? keyword )
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                    "spGetSearchProductSerialNumber",
                    new string[] {"@FilterText" },
                    new object[] {keyword ?? ""}
                    );
                return Ok(new
                {
                    status = 1,
                    dataImport = SQLHelper<object>.GetListData(result, 0),
                    dataExport = SQLHelper<object>.GetListData(result, 1),
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status=0,
                    message=ex.Message
                });
            }
        }
    }
}
