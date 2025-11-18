using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.TB;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.TB;
using RERPAPI.Repo.GenericEntity;
namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProtectiveGearController : Controller
    {
        ProductGroupRTCRepo productGroupRTCRepo;
        private readonly ProductRTCRepo _productRTCRepo;

        public ProtectiveGearController(ProductGroupRTCRepo productGroupRTCRepo, ProductRTCRepo productRTCRepo)
        {
            this.productGroupRTCRepo = productGroupRTCRepo;
            _productRTCRepo = productRTCRepo;
        }

        [HttpGet("protective-gears")]
        public IActionResult ProtectiveGears(int productGroupID, string? keyword = "", int allProduct = 1, int warehouseID = 5)
        {
            try
            {
                var protectiveGears = SQLHelper<object>.ProcedureToList("spGetInventoryDemo",
                                    new string[] { "@ProductGroupID", "@Keyword", "@CheckAll", "@WarehouseID" },
                                    new object[] { productGroupID, keyword ?? "", allProduct, warehouseID });

                return Ok(new
                {
                    status = 1,
                    data = protectiveGears,

                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-product-group-rtc")]
        public IActionResult GetProductGroupRTC()
        {
            try
            {

                var productGroups = productGroupRTCRepo.GetAll(p => p.WarehouseID == 1 && p.ProductGroupNo.Contains("DBH"));


                return Ok(new
                {
                    status = 1,
                    data = productGroups
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("get-protective-gears")]
        public IActionResult GetProtectiveGears([FromBody] ProductRTCRequetParam request)
            {
            try
            {
                var protectiveGears = SQLHelper<object>.ProcedureToList("spGetProductRTC",
                                    new string[] { "@ProductGroupID", "@Keyword", "@CheckAll", "@WarehouseID" ,"@PageSize","PageNumber"},
                                    new object[] { request.ProductGroupID, request.Keyword ?? "", request.CheckAll, 5,request.Size,request.Page });


                return Ok(new
                {
                    status = 1,
                    data = protectiveGears,
                    TotalPage = SQLHelper<dynamic>.GetListData(protectiveGears, 1)

                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] ProductRTCFullDTO product)
        {
            try
            {
                if (product == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu gửi lên không hợp lệ."));
                }

                if (product.productRTCs != null && product.productRTCs.Any())
                {
                    foreach (var item in product.productRTCs)
                    {
                        if (item.IsDelete == true)
                        {
                            if (item.ID > 0)
                                await _productRTCRepo.UpdateAsync(item);
                        }
                        else
                        {
                            if (item.ID <= 0)
                                await _productRTCRepo.CreateAsync(item);
                            else
                                await _productRTCRepo.UpdateAsync(item);
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu dữ liệu thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
