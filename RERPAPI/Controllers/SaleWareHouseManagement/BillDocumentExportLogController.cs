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
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(result, 0), "Lấy dữ liệu thành công thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
