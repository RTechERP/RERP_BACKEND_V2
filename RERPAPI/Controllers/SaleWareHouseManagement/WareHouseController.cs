using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using ZXing;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class WareHouseController : ControllerBase
    {
        WarehouseRepo _warehouseRepo = new WarehouseRepo();
        [HttpGet("")]
        public IActionResult getDataWH()
        {
            try
            {
                List<Warehouse> warehouse = _warehouseRepo.GetAll();
                return Ok(ApiResponseFactory.Success(warehouse, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
