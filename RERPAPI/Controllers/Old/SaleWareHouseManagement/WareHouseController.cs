using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
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
                List<RERPAPI.Model.Entities.Warehouse> warehouse = _warehouseRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = warehouse
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
    }
}
