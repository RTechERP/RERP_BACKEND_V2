using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using System.Diagnostics;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryRepo _inventoryRepo;
        private readonly WarehouseRepo _warehouseRepo;
        private readonly ProductSaleRepo _productSaleRepo;
        public InventoryController(InventoryRepo inventoryRepo, WarehouseRepo warehouseRepo, ProductSaleRepo productSaleRepo)
        {
            _inventoryRepo = inventoryRepo;
            _warehouseRepo = warehouseRepo;
            _productSaleRepo = productSaleRepo;
        }
        [HttpPost("get-inventory")]
        public IActionResult getInventory(InventoryPram filter)
        {
            try
            {
                //Stopwatch stopwatch = new Stopwatch();
                if (filter.checkAll == true) filter.productGroupID = 0;
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                       //"spGetInventory", new string[] { "@ID", "@Find", "@WarehouseCode", "@IsStock" },
                       "spGetInventory_Test", new string[] { "@ID", "@Find", "@WarehouseCode", "@IsStock" },
                    new object[] { filter.productGroupID, filter.Find, filter.WarehouseCode, filter.IsStock == false ? 0 : 1 }
                   );
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(result, 0)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        //[HttpPost("get-inventory-pagination")]
        //public IActionResult GetInventory(InventoryPram filter)
        //{
        //    try
        //    {

        //        if (filter.checkAll == true) filter.productGroupID = 0;
        //        List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
        //               "spGetInventory_Pagination", new string[] { "@ID", "@Find", "@WarehouseCode", "@IsStock", "@PageSize", "@PageNumber" },
        //            new object[] { filter.productGroupID, filter.Find, filter.WarehouseCode, filter.IsStock == false ? 0 : 1, filter.PageSize, filter.PageNumber }
        //           );

        //        //List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
        //        //       "spGetInventory", new string[] { "@ID", "@Find", "@WarehouseCode", "@IsStock", "@PageSize", "@PageNumber" },
        //        //    new object[] { filter.productGroupID, filter.Find, filter.WarehouseCode, filter.IsStock == false ? 0 : 1, filter.PageSize, filter.PageNumber }
        //        //   );
        //        return Ok(new
        //        {
        //            status = 1,
        //            data = SQLHelper<object>.GetListData(result, 0)
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new
        //        {
        //            status = 0,
        //            message = ex.Message,
        //            error = ex.ToString()
        //        });
        //    }
        //}
        [HttpGet("{id}")]
        public IActionResult getDataByID(int id)
        {
            try
            {
                Inventory result = _inventoryRepo.GetByID(id);
                return Ok(new
                {
                    status = 1,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                });
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
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(result, 0)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
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
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(result, 0)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    error = ex.Message
                });
            }
        }
        [HttpGet("get-chi-tiet-san-pham-sale")]
        public IActionResult GetHistoryImportExportProductSale(int productSaleID, string warehouseCode)
        {
            try
            {
                RERPAPI.Model.Entities.Warehouse warehouse = _warehouseRepo.GetAll(x => x.WarehouseCode.Trim().ToUpper() == warehouseCode).FirstOrDefault() ?? new RERPAPI.Model.Entities.Warehouse();


                var data = SQLHelper<dynamic>.ProcedureToList("spGetHistoryImportExportInventory", ["@ProductSaleID", "@WarehouseCode"], [productSaleID, warehouseCode]);

                var dataHold = SQLHelper<dynamic>.ProcedureToList("spGetInventoryProject", ["@ProjectID", "@EmployeeID", "@ProductSaleID", "@Keyword", "@WarehouseID"], [0, 0, productSaleID, "", warehouse.ID]);
                return Ok(ApiResponseFactory.Success(new
                {
                    dtProduct = SQLHelper<dynamic>.GetListData(data, 0),
                    dtImport = SQLHelper<dynamic>.GetListData(data, 1),
                    dtExport = SQLHelper<dynamic>.GetListData(data, 2),
                    dtRequestImport = SQLHelper<dynamic>.GetListData(data, 4),
                    dtRequestExport = SQLHelper<dynamic>.GetListData(data, 5),
                    dtHold = SQLHelper<dynamic>.GetListData(dataHold, 0),
                    dtCbProduct = SQLHelper<dynamic>.GetListData(data, 3),
                }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("set-location")]
        public IActionResult SetLocation(int productSaleID, int locationID)
        {
            try
            {
                bool rs = _productSaleRepo.SetLocation(productSaleID, locationID);
                if (rs)
                {
                    return Ok(ApiResponseFactory.Success(productSaleID, "Cập nhật vị trí thành công"));
                }
                else
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Cập nhật vị trí thất bại"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("set-location-list")]
        public async Task<IActionResult> SetLocation([FromBody] SetLocationRequestDTO request)
        {
            try
            {

                bool rs = await _productSaleRepo.SetLocationList(request);
                if (rs)
                {
                    return Ok(ApiResponseFactory.Success(request.LstIDs, "Cập nhật vị trí thành công"));
                }
                else
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Cập nhật vị trí thất bại"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
