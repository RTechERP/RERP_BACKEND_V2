using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductRTCController : ControllerBase
    {
        ProductRTCRepo _productRTCRepo = new ProductRTCRepo();

        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                List<ProductRTC> result = _productRTCRepo.GetAll();
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
