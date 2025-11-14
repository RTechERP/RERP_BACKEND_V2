using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillDocumentImportLogController : ControllerBase
    {
        [HttpGet("get-by-BdiID/")]
        public IActionResult getDataByBillDocumentExportID(int bdiID, int dcocumentImportID)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                      "spGetBillDocumentImportLog", new string[] { "@billDocumentImportID", "@DocumentImportID" },
                   new object[] { bdiID, dcocumentImportID }
                  );
                var data = SQLHelper<object>.GetListData(result, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
