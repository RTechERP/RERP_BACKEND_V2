using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductGroupRTCController : ControllerBase
    {
        private readonly ProductGroupRTCRepo _productGroupRTCRepo;
        public ProductGroupRTCController(ProductGroupRTCRepo productGroupRTCRepo)
        {
            _productGroupRTCRepo = productGroupRTCRepo;
        }
        [HttpGet("get-all")]
        public IActionResult GetAll(int warehouseType)
        {
            try
            {
                List<ProductGroupRTC> productGroups = _productGroupRTCRepo.GetAll(x => x.WarehouseID == 1 && !x.ProductGroupNo.Contains("DBH") && x.ProductGroupNo != "CCDC" && x.WarehouseType == warehouseType);
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
