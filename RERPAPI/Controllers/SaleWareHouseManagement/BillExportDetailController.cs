using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Param;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillExportDetailController : ControllerBase
    {
        [HttpGet("BillExportID/{billExportID}")]
        public IActionResult getBillExportDetailByBillID(int billExportID )
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                       "spGetBillExportDetail", new string[] { "@BillID"},
                    new object[] {billExportID}
                   );
                int totalPage = 0;
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(result, 0), "Lấy dữ liệu chi tiết hóa đơn thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
