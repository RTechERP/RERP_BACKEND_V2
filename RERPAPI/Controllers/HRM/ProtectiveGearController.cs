using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Repo.GenericEntity;
namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProtectiveGearController : Controller
    {
        ProductGroupRTCRepo productGroupRTCRepo;
        public ProtectiveGearController(ProductGroupRTCRepo productGroupRTCRepo)
        {
            this.productGroupRTCRepo = productGroupRTCRepo;
        }
        [HttpGet("protective-gears")]
        public IActionResult GetProtectiveGears(int productGroupID, string? keyword = "", int allProduct = 1, int warehouseID = 5)
        {
            try
            {
                var protectiveGears = SQLHelper<object>.ProcedureToList("spGetInventoryDemo",
                                    new string[] { "@ProductGroupID", "@Keyword", "@CheckAll", "@WarehouseID" },
                                    new object[] { productGroupID, keyword ?? "", allProduct, warehouseID });

                return Ok(new
                {
                    status = 1,
                    data = protectiveGears,

                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-product-group-rtc")]
        public IActionResult GetProductGroupRTC()
        {
            try
            {

                var productGroups = productGroupRTCRepo.GetAll(p => p.WarehouseID == 1 && p.ProductGroupNo == "DBH");


                return Ok(new
                {
                    status = 1,
                    data = productGroups
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
