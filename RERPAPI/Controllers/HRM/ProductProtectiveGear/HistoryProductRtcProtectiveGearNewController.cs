using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.HRM;

namespace RERPAPI.Controllers.HRM.ProductProtectiveGear
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryProductRtcProtectiveGearNewController : ControllerBase
    {
        public HistoryProductRtcProtectiveGearNewController()
        {
                
        }
        [HttpGet("get-productrtc")]
        public async Task<IActionResult> GetProductrtc(string? Keyword)
        {
            try
            {
                DateTime date = new DateTime(2024, 09, 01);
                DateTime dateStart = date;
                DateTime dateEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

                string keyword = Keyword??"";
                string status = "1,2,3,4,5,6,7,8";
                int employeeID = 0;
                int isDeleted = 0;
                int productGroupRTCID = 140;
                int warehouseID = 5;
                var data = SQLHelper<ProductRTCDTO>.ProcedureToList("spGetHistoryProductRTCProtectiveGear",
                           new string[] { "@DateStart", "@DateEnd", "@EmployeeID", "@Status", "@IsDeleted", "@WarehouseID", "@ProductGroupRTCID", "@Keyword" },
                           new object[] { dateStart, dateEnd, employeeID, status, isDeleted, warehouseID, productGroupRTCID, keyword });
                var dt = SQLHelper<object>.GetListData(data, 0);

                var data0 = dt.FindAll(c => c.ProductGroupRTCID == 140);
                return Ok(ApiResponseFactory.Success(data0, ""))
                ;

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
