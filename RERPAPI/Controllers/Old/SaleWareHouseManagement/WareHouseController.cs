using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;
using RERPAPI.Repo.GenericEntity.Asset;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WareHouseController : ControllerBase
    {
        private readonly WarehouseRepo _warehouseRepo;
        public WareHouseController(WarehouseRepo warehouseRepo)
        {
            _warehouseRepo = warehouseRepo;
        }
        [HttpGet("")]
        public IActionResult GetWareHouse()
        {
            try
            {
                List<RERPAPI.Model.Entities.Warehouse> warehouse = _warehouseRepo.GetAll(x=>x.IsDeleted!=true);
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
        [RequiresPermission("N1")]
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] Model.Entities.Warehouse warehouse)
        {
            try
            {
                if (warehouse != null && warehouse.IsDeleted != true)
                {
                    var validate = _warehouseRepo.Validate(warehouse);
                    if (validate.status == 0) return BadRequest(validate);
                }
               
                if (warehouse.ID > 0)
                {
                    await _warehouseRepo.UpdateAsync(warehouse);
                }
                else
                {
                    await _warehouseRepo.CreateAsync(warehouse);
                }
                return Ok(ApiResponseFactory.Success(warehouse, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}
