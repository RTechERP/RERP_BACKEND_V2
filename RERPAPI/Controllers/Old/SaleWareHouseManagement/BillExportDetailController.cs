using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Param;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
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
                List<dynamic> billDetail = result[0]; // dữ liệu chi tiết hóa đơn
                int totalPage = 0;
                return Ok(new
                {
                    status = 1,
                    data = billDetail, 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
    }
}
