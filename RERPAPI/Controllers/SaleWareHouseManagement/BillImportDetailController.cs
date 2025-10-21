using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillImportDetailController : ControllerBase
    {
        [HttpGet("BillImportID/{billImportID}")]
        public IActionResult getBillExportDetailByBillID(int billImportID)
        {
            try
            {

                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                       "spGetBillImportDetail", new string[] { "@ID" },
                    new object[] { billImportID }
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
