using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.TB;
using System.Linq;
using System.Linq.Expressions;
namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProtectiveGearController : Controller
    {
        [HttpGet("protective-gears")]
        public IActionResult GetProtectiveGears(int productGroupID , string? keyword = "", int allProduct = 1, int warehouseID = 5)
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
                var productGroupRTCRepo = new ProductGroupRTCRepo();
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
