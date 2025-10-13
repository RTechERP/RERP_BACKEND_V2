using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.TB;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductGroupRTCController : ControllerBase
    {
        ProductGroupRTCRepo _productGroupRTCRepo = new ProductGroupRTCRepo();
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                List<ProductGroupRTC> productGroups = _productGroupRTCRepo.GetAll(x => x.WarehouseID == 1 && !x.ProductGroupNo.Contains("DBH") && x.ProductGroupNo != "CCDC");
                return Ok(new
                {
                    status = 1,
                    data = productGroups
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
