using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Common;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Context;
using Microsoft.AspNetCore.Http.HttpResults;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo;
using RERPAPI.Model.Param.Asset;
using RERPAPI.Model.Param.TB;
using RERPAPI.Repo.GenericEntity.TB;
using RERPAPI.Model.DTO.Asset;
using RERPAPI.Repo.GenericEntity.Asset;
using RERPAPI.Model.DTO.TB;
using System.Data;
using RERPAPI.Model.Param.Technical;
using RERPAPI.Repo.GenericEntity.Technical;

namespace RERPAPI.Controllers.Technical.InventoryDemo
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryDemoController : ControllerBase
    {
        ProductRTCQRCodeRepo _productRTCQRCodeRepo = new ProductRTCQRCodeRepo();
        [HttpPost("get-inventoryDemo")]
        public IActionResult GetInventoryDemo([FromBody] InventoryDemoRequestParam request)
        {
            try
            {
                var products = SQLHelper<dynamic>.ProcedureToList("spGetInventoryDemo",
                    new string[] { "@ProductGroupID", "@Keyword", "@CheckAll", "@WarehouseID", "@ProductRTCID" },
                    new object[] { request.ProductGroupID, request.Keyword, request.CheckAll, request.WarehouseID , request.ProductRTCID });

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
                    new string[] { "@SupplierDemoID", "@WarehouseID", "@DateStart", "@DateEnd", "@FilterText", "@PageNumber", "@PageSize" },
                    new object[] { request.SupplierDemoID, request.WarehouseID, request.DateStart, request.DateEnd, request.FilterText, request.Page,request.Size });

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
                    _productRTCQRCodeRepo.UpdateFieldsByID(productRTCQRCode.ID, productRTCQRCode);

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



    }
}
