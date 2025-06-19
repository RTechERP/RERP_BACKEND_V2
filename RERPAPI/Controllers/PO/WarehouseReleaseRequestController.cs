using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers.PO
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseReleaseRequestController : ControllerBase
    {
        ProductGroupRepo _productGroupRepo = new ProductGroupRepo();
        WarehouseRepo _warehouseRepo = new WarehouseRepo();
        [HttpGet("get-pokh-export-request")]
        public IActionResult GetPOKHExportRequest( int warehouseId, int customerId, int projectId, int productGroupId, string keyword = "")
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToDynamicLists("spGetPOKHRequestExport",
                    new string[] { "@WarehouseID", "@CustomerID", "@ProjectID", "@ProductGroupID", "@Keyword" },
                    new object[] { warehouseId, customerId, projectId, productGroupId, keyword});
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<dynamic>.GetListData(list,0)
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
        [HttpGet("get-productgroup")]
        public IActionResult GetProductGroup()
        {
            try
            {
                var data = _productGroupRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data
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
        [HttpGet("get-warehouse")]
        public IActionResult GetWarehouse()
        {
            try
            {
                var data = _warehouseRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data
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
