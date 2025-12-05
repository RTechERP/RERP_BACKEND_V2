using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.ProjectAGV;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.HRM.VehicleManagement;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Asset;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryProjectController : ControllerBase
    {
        ProjectRepo _projectRepo;
        InventoryProjectRepo _inventoryProjecRepo;

        public InventoryProjectController(ProjectRepo projectRepo, InventoryProjectRepo inventoryProjecRepo)
        {
            _projectRepo = projectRepo;
            _inventoryProjecRepo = inventoryProjecRepo;
        }

        [HttpGet("get-inventory-by-product")]
        public IActionResult GetInventoryByProduct(string keyword="")
        {
            try
            {
                string procedureName = "spGetProductInventoryByKeyword";
                string[] paramNames = new string[] { "@Keyword" };
                object[] paramValues = new object[] { keyword??"" };
                var data = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);
                var inventory = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(inventory, "Lấy dữ liệu thành công")); 
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //Lấy danh sách hàng nhả giữ
        [HttpPost("get-inventory-project")]
        public IActionResult GetInventoryProject([FromBody] InventoryProjectRequestParam request)
        {
            try
            {
            
                string procedureName = "spGetInventoryProject";
                string[] paramNames = new string[] { "@ProjectID", "@EmployeeID", "@ProductSaleID", "@Keyword"};
                object[] paramValues = new object[] { request.ProjectID??0, request.EmployeeID??0, 0, request.KeyWord ?? ""};
                var data = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);
                var inventoryProject = SQLHelper<object>.GetListData(data, 0);
               
                return Ok(ApiResponseFactory.Success(inventoryProject, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-POKH")]
        public IActionResult GetPOKH(int productSaleID)
        {
            try
            {
                string procedureName = "spGetPOKHDetailForInventoryProject";
                string[] paramNames = new string[] {"@ProductID"};
                object[] paramValues = new object[] { productSaleID };
                var data = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);
                var POKH = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(POKH, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-project")]
        public IActionResult GetProject()
        {
            try
            {
              var projects = _projectRepo.GetAll().ToList();
                return Ok(ApiResponseFactory.Success(projects, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
      
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] InventoryProject inventory)
        {
            try
            {
                //if (inventory != null && inventory.IsDeleted != true)
                //{
                //    if (!_tsSourceAssetRepo.Validate(inventory, out string message))
                //        return BadRequest(ApiResponseFactory.Fail(null, message));
                //}

                if (inventory.ID <= 0)
                {

                    await _inventoryProjecRepo.CreateAsync(inventory);
                }
                else
                {
                    await _inventoryProjecRepo.UpdateAsync(inventory);
                }
                return Ok(new
                {
                    status = 1,
                    message = "Lưu thành công.",
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}
