using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.Warehouses.AGV;
using System.Threading.Tasks;

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
                var products = _agvProductRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(products, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] AGVProduct product)
        {
            try
            {

                var validate = _agvProductRepo.Validate(product);
                if (validate.status == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, validate.message));
                }

                if (product.ID <= 0) await _agvProductRepo.CreateAsync(product);
                else await _agvProductRepo.UpdateAsync(product);

                return Ok(ApiResponseFactory.Success(product, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex,ex.Message));
            }
        }
    }
}
