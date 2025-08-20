using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Param.TB;
using RERPAPI.Model.Param.TB.Inventory;

namespace RERPAPI.Controllers.TB.Inventory
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductExportAndBorrowController : ControllerBase
    {
        [HttpPost("get-product-export-and-borrow")]
        public IActionResult GetProductExportEndBorrow([FromBody] ProductExportAndBorrowRequestParam request)
        {
            try
            {
                var products = SQLHelper<dynamic>.ProcedureToList("spGetProductByExportAndBorrowDate",
                new string[] { "@DateStart", "@DateEnd", "@PageNumber", "@PageSize", "@FilterText", "@WarehouseID" },
                new object[] { request.DateStart, request.DateEnd, request.Page, request.Size, request.Filtertext, request.WarehouseID });
 
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
    }
}
