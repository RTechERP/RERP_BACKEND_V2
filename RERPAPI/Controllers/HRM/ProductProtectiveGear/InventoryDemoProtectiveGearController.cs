using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.HRM.ProductProtectiveGear;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.HRM.ProductProtectiveGear
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryDemoProtectiveGearController : ControllerBase
    {
        private readonly ProductGroupRTCRepo _productGroupRTCRepo;

        public InventoryDemoProtectiveGearController(ProductGroupRTCRepo productGroupRTCRepo)
        {

            _productGroupRTCRepo = productGroupRTCRepo;

        }
        [HttpGet("get-product-group")]
        public IActionResult GetProductGroup(int warehouseID)
        {
            try
            {
                var data = _productGroupRTCRepo.GetAll(c => c.WarehouseID == warehouseID && c.ProductGroupNo == "DBH");
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-inventory-demo")]
        public IActionResult GetInventoryDemo([FromQuery] InventoryDemoParam param)
        {
            try
            {
                int allProduct = 1;
                string keywords = "";
                if (!string.IsNullOrEmpty(param.Keyword))
                {
                    keywords = param.Keyword;
                }
                var data = SQLHelper<object>.ProcedureToList("spGetInventoryDemo",
                                    new string[] { "@ProductGroupID", "@Keyword", "@CheckAll", "@WarehouseID" },
                                    new object[] { param.ProductGroupID, keywords, allProduct, param.WarehouseID }
                );
                var data0 = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(data0, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
