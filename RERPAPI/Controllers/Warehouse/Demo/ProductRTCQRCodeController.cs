using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Repo.GenericEntity.Technical;

namespace RERPAPI.Controllers.Warehouse.Demo
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductRTCQRCodeController : ControllerBase
    {
        const int WAREHOUSEID = 1;
        ProductRTCQRCodeRepo qrCodeRepo = new ProductRTCQRCodeRepo();

        [HttpGet("get-qrcodes")]
        public IActionResult GetAll(string? keyword)
        {
            try
            {
                keyword = keyword ?? "";
                var datas = SQLHelper<object>.ProcedureToList("spGetProductAndQrCode", 
                                                                new string[] { "@WarehouseID", "@FilterText" }, 
                                                                new object[] { WAREHOUSEID, keyword.Trim() });

                var qrCodes = SQLHelper<object>.GetListData(datas, 0);
                return Ok(ApiResponseFactory.Success(qrCodes, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
