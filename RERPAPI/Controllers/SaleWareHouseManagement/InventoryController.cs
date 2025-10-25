using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        InventoryRepo _inventoryRepo = new InventoryRepo();
        [HttpPost("get-inventory")]
        public IActionResult getInventory(InventoryPram filter)
        {
            try
            {

                if (filter.checkAll == true) filter.productGroupID = 0;
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                       "spGetInventory", new string[] { "@ID", "@Find", "@WarehouseCode", "@IsStock" },
                    new object[] { filter.productGroupID, filter.Find, filter.WarehouseCode, filter.IsStock == false ? 0 : 1 }
                   );
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(result, 0), "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("{id}")]
        public IActionResult getDataByID(int id)
        {
            try
            {
                Inventory result = _inventoryRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-productgroup-warehouse")]
        public IActionResult getProductSale(int productGroupID, string warehouseCode)
        {
            try
            {
                int WarehouseID = warehouseCode.ToUpper() == "HN" ? 1 :
                             warehouseCode.ToUpper() == "HCM" ? 2 : 3;
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                       "spGetProductGroupWarehouse", new string[] { "@WarehouseID", "@ProductGroupID" },
                    new object[] { WarehouseID, productGroupID }
                   );
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(result, 0), "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("get-inventory-borrow-ncc")]

        public IActionResult getInventoryBorrowNCC(InventoryBorrowNCCParam filter)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                    "spGetInventoryBorrowSupllier",
                    new string[] { "@PageNumber", "@PageSize", "@FilterText", "@DateStart", "@DateEnd", "@WarehouseID", "SupplierSaleID" },
                    new object[] { filter.PageNumber, filter.PageSize, filter.FilterText, filter.DateStart, filter.DateEnd, filter.WareHouseID, filter.SupplierSaleID }
                    );
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(result, 0), "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
