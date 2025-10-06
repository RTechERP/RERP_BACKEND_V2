using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
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
