﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductGroupWareHouseController : ControllerBase
    {
        ProductGroupWareHouseRepo _productgroupwarehouseRepo = new ProductGroupWareHouseRepo();

        [HttpGet("")]
        public IActionResult getDataProductGroupWareHours(int? warehouseID, int? productgroupID)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToDynamicLists(
                    "spGetProductGroupWarehouse",
                    new string[] { "@WarehouseID", "@ProductGroupID" },
                    new object[] { warehouseID ?? 0, productgroupID ?? 0 }
                );

                if (result == null || result.Count == 0 || result[0] == null)
                {
                    return Ok(new
                    {
                        status = 1,
                        data = new List<dynamic>()
                    });
                }

                return Ok(new
                {
                    status = 1,
                    data = result[0]
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
    }
}
