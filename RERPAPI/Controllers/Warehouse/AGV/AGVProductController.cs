using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Repo.GenericEntity.Warehouses.AGV;

namespace RERPAPI.Controllers.Warehouse.AGV
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AGVProductController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AGVProductRepo _agvProductRepo;

        public AGVProductController(AGVProductRepo agvProductRepo,IConfiguration configuration)
        {
            _configuration = configuration;
            _agvProductRepo = agvProductRepo;
        }


        [HttpGet()]
        public IActionResult GetAll()
        {
            try
            {
                var products = _agvProductRepo.GetAll(x => x.IsDeleted == true);
                return Ok(ApiResponseFactory.Success(products, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
