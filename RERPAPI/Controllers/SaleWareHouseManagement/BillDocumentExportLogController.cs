using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillDocumentExportLogController : ControllerBase
    {   
        [HttpGet("get-by-BdeID/{bdeID}")]
        public IActionResult getDataByBillDocumentExportID(int bdeID)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                      "spGetBillDocumentExportLog", new string[] { "@BillDocumentExportID" },
                   new object[] { bdeID }
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
