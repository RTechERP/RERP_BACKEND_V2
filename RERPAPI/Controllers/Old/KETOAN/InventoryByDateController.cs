using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.KETOAN
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryByDateController : ControllerBase
    {

        private readonly InventoryRepo _inventoryRepo;
        public InventoryByDateController(InventoryRepo inventoryRepo)
        {
            _inventoryRepo = inventoryRepo;
        }

        [HttpGet("get-data")]
        public IActionResult Get(DateTime dateTime)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetInventoryByDate",
                     new string[] { "@DateValues", "@WarehouseCode" },
                     new object[] { dateTime, "HN" });

                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-import-export-inventory")]
        public IActionResult GetImportExportInventory(int productSaleId, string warehouseCode, DateTime dateValues)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetHistoryImportExportInventory_ByDate",
                     new string[] { "@ProductSaleID", "@WarehouseCode", "@DateValues" },
                     new object[] { productSaleId, warehouseCode, dateValues });

                var dataImport = SQLHelper<dynamic>.GetListData(list, 0);
                var dataExport = SQLHelper<dynamic>.GetListData(list, 1);
                var dataHistory = SQLHelper<dynamic>.GetListData(list, 2);
                return Ok(ApiResponseFactory.Success(new { dataImport, dataExport, dataHistory}, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-inventory-by-productid")]
        public IActionResult GetInventoryByProductID(int productSaleId)
        {
            try
            {
                Inventory model = _inventoryRepo.GetAll(x => x.ProductSaleID == productSaleId).FirstOrDefault() ?? new Inventory();
                return Ok(ApiResponseFactory.Success(model, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
