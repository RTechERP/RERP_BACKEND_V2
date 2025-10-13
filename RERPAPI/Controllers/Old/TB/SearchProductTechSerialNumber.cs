using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;

namespace RERPAPI.Controllers.Old.TB
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchProductTechSerialNumber : ControllerBase
    {
      
            [HttpGet("get-search-product-tech-serial")]
            public IActionResult GetSearchProductTechSerial([FromQuery] int WarehouseID=1, string serialNumber="")
            {
                try
                {
                    var serials = SQLHelper<dynamic>.ProcedureToList("spGetSearchProductTechSerial",
                    new string[] { "@SerialNumber", "@WarehouseID" },
                    new object[] { serialNumber, WarehouseID });

                    return Ok(new
                    {
                        status = 1,
                        import = SQLHelper<dynamic>.GetListData(serials, 1),
                        export = SQLHelper<dynamic>.GetListData(serials, 0)
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
