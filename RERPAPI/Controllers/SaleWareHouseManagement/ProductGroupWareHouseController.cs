using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductGroupWareHouseController : ControllerBase
    {
        private readonly ProductGroupWareHouseRepo _productgroupwarehouseRepo;

        public ProductGroupWareHouseController(ProductGroupWareHouseRepo productgroupwarehouseRepo)
        {
            _productgroupwarehouseRepo = productgroupwarehouseRepo;
        }

        [HttpGet("")]
        public IActionResult getDataProductGroupWareHours(int? warehouseID, int? productgroupID)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                    "spGetProductGroupWarehouse",
                    new string[] { "@WarehouseID", "@ProductGroupID" },
                    new object[] { warehouseID ?? 0, productgroupID ?? 0 }
                );

                if (result == null || result.Count == 0 || result[0] == null)
                {
                    return Ok(ApiResponseFactory.Success(new List<dynamic>(), "Lấy dữ liệu thành công!"));         
                }

                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(result, 0), "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
