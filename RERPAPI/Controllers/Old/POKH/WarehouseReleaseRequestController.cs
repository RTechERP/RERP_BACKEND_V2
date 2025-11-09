using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers.Old.POKH
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseReleaseRequestController : ControllerBase
    {
        private readonly ProductGroupRepo _productGroupRepo;
        private readonly WarehouseRepo _warehouseRepo;

        public WarehouseReleaseRequestController(ProductGroupRepo productGroupRepo, WarehouseRepo warehouseRepo)
        {
            _productGroupRepo = productGroupRepo;
            _warehouseRepo = warehouseRepo;
        }
        [HttpGet("get-pokh-export-request")]
        public IActionResult GetPOKHExportRequest(int warehouseId, int customerId, int projectId, int productGroupId, string keyword = "")
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetPOKHRequestExport",
                    new string[] { "@WarehouseID", "@CustomerID", "@ProjectID", "@ProductGroupID", "@Keyword" },
                    new object[] { warehouseId, customerId, projectId, productGroupId, keyword});
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-productgroup")]
        public IActionResult GetProductGroup()
        {
            try
            {
                var data = _productGroupRepo.GetAll();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-warehouse")]
        public IActionResult GetWarehouse()
        {
            try
            {
                var data = _warehouseRepo.GetAll();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
