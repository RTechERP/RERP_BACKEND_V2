using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RERPAPI.Controllers.HRM.ProductProtectiveGear
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductLocationTechController : ControllerBase
    {
        private readonly ProductLocationRepo _productLocationRepo;
        public ProductLocationTechController(ProductLocationRepo productLocationRepo)
        {
            _productLocationRepo = productLocationRepo;
        }
        [HttpGet("get-product-location-tech")]
        public IActionResult GetProductLocation(int warehouseID)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetProductLocation", new string[] { "@WarehouseID" }, new object[] { warehouseID });
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-stt")]
        public IActionResult GetSTT(int warehouseID)
        {
            int stt = _productLocationRepo.GetSTT(warehouseID);
            return Ok(ApiResponseFactory.Success(stt, ""));
        }
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var data = await _productLocationRepo.GetByIDAsync(id);
                if (data == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy dữ liệu"));
                }
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> PostSaveDataAsync([FromBody] ProductLocation productLocation)
        {
            try
            {
                if (_productLocationRepo.CheckLocationCodeExists(productLocation.LocationCode, productLocation.ID) == true)
                {
                    string message = $"Mã [{productLocation.LocationCode}] đã tồn tại!";
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                } 
                if (productLocation.ID <= 0)
                {
                    productLocation.CreatedDate = DateTime.Now;
                    await _productLocationRepo.CreateAsync(productLocation);
                }
                else await _productLocationRepo.UpdateAsync(productLocation);

                return Ok(ApiResponseFactory.Success(null, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
