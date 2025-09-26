using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillDocumentImportLogController : ControllerBase
    {
        [HttpGet("get-by-BdiID/")]
        public IActionResult getDataByBillDocumentExportID(int bdiID, int dcocumentImportID )
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                      "spGetBillDocumentImportLog", new string[] { "@billDocumentImportID", "@DocumentImportID" },
                   new object[] { bdiID, dcocumentImportID }
                  );
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(result, 0)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new
                    {
                        status = 0,
                        error = ex.Message
                    });
            }
        }
    }
}
