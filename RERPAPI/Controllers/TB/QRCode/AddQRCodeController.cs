using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Param.TB;


namespace RERPAPI.Controllers.TB.QRCode
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddQRCodeController : ControllerBase
    {
        [HttpPost("get-productRTC")]
        public IActionResult GetListAssets([FromBody] int WarehouseID, string serialNumber)
        {
            try
            {
                var products = SQLHelper<dynamic>.ProcedureToList("spGetSearchProductTechSerial",
                new string[] { "@SerialNumber", "@WarehouseID" },
                new object[] { serialNumber , WarehouseID });

                return Ok(new
                {
                    status = 1,

                    products = SQLHelper<dynamic>.GetListData(products, 0),
                    TotalPage = SQLHelper<dynamic>.GetListData(products, 1)
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {

                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
    }
}
