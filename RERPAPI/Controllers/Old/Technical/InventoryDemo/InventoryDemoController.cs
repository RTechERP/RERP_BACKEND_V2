using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.Technical;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Technical;

namespace RERPAPI.Controllers.Old.Technical.InventoryDemo
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryDemoController : ControllerBase
    {
        private readonly ProductRTCQRCodeRepo _productRTCQRCodeRepo;
        private readonly ProductGroupRTCRepo _productGroupRTCRepo;
        public InventoryDemoController(ProductRTCQRCodeRepo productRTCQRCodeRepo, ProductGroupRTCRepo productGroupRTCRepo)
        {
            _productRTCQRCodeRepo = productRTCQRCodeRepo;
            _productGroupRTCRepo = productGroupRTCRepo;
        }
        [HttpPost("get-inventoryDemo")]
        public IActionResult GetInventoryDemo([FromBody] InventoryDemoRequestParam request)
        {
            try
            {
                var products = SQLHelper<dynamic>.ProcedureToList("spGetInventoryDemo",
                    new string[] { "@ProductGroupID", "@Keyword", "@CheckAll", "@WarehouseID", "@ProductRTCID", "@WarehouseType" },
                    new object[] { request.ProductGroupID, request.Keyword, request.CheckAll, request.WarehouseID, request.ProductRTCID, request.WarehouseType });

                return Ok(new
                {
                    status = 1,
                    products = SQLHelper<dynamic>.GetListData(products, 0),
                    TotalPage = SQLHelper<dynamic>.GetListData(products, 1)
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
        [HttpPost("get-inventory-borrow-ncc-Demo")]
        public IActionResult GetInventoryBorrowNCCDemo([FromBody] GetInventoryBorrowSupplierDemoRequestParam request)
        {
            try
            {
                var products = SQLHelper<dynamic>.ProcedureToList("spGetInventoryBorrowSupplierDemo",
                    new string[] { "@SupplierDemoID", "@WarehouseID", "@DateStart", "@DateEnd", "@FilterText", "@PageNumber", "@PageSize", "@WarehouseType" },
                    new object[] { request.SupplierDemoID, request.WarehouseID, request.DateStart, request.DateEnd, request.FilterText, request.Page, request.Size, request.WarehouseType });

                return Ok(new
                {
                    status = 1,
                    products = SQLHelper<dynamic>.GetListData(products, 0),
                    TotalPage = SQLHelper<dynamic>.GetListData(products, 1)
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
        [HttpPost("save-data-qrcode")]
        public async Task<IActionResult> SaveDataQRCode([FromBody] ProductRTCQRCode productRTCQRCode)
        {
            try
            {
                if (productRTCQRCode.ID <= 0)
                    await _productRTCQRCodeRepo.CreateAsync(productRTCQRCode);
                else
                    await _productRTCQRCodeRepo.UpdateAsync(productRTCQRCode);

                return Ok(new
                {
                    status = 1,
                    message = "Lưu thành công.",
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
        [HttpGet("get-borrow-import-export-product-rtc")]
        public IActionResult GetBorrowImportExportProductRTC([FromQuery] int? ProductID, [FromQuery] int? WarehouseID)
        {
            try
            {
                var products = SQLHelper<dynamic>.ProcedureToList("spGetBorrowImportExportProductRTC",
                    new string[] { "@ProductID", "@WarehouseID" },
                    new object[] { ProductID, WarehouseID });

                return Ok(new
                {
                    status = 1,
                    listImport = SQLHelper<dynamic>.GetListData(products, 0),
                    listExport = SQLHelper<dynamic>.GetListData(products, 1),
                    listBorrow = SQLHelper<dynamic>.GetListData(products, 2)
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

        [HttpGet("get-productRTC-group")]
        public IActionResult GetAll(int warehouseID, int warehouseType = 1)
        {
            try
            {
                List<ProductGroupRTC> productGroup = _productGroupRTCRepo.GetAll(x => x.WarehouseType == warehouseType && x.WarehouseID == warehouseID
                                                                                    && x.IsDeleted == false && !x.ProductGroupNo.Contains("DBH") && x.ProductGroupNo != "CCDC").OrderBy(x => x.NumberOrder).ToList();

                return Ok(ApiResponseFactory.Success(productGroup, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}
