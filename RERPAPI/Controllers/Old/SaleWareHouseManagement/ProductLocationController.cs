using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductLocationController : ControllerBase
    {
        ProductLocationRepo _productLocationRepo = new ProductLocationRepo();

        [HttpPost("get-product-locations")]
        public IActionResult getProductLocations()
        {
            try
            {
                List<ProductLocation> dataProductLocation = _productLocationRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(dataProductLocation, ""));
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
                var productLocation = _productLocationRepo.GetByID(id);
                if (productLocation == null || productLocation.IsDeleted == true)
                {
                    return Ok(ApiResponseFactory.Success(null, "Không tìm thấy vị trí sản phẩm"));
                }

                return Ok(ApiResponseFactory.Success(productLocation, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("check-location-code")]
        public IActionResult checkLocationCodeExists(string locationCode, int? id = null)
        {
            try
            {
                bool exists = _productLocationRepo.CheckLocationCodeExists(locationCode, id);
                return Ok(ApiResponseFactory.Success(exists, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> saveData([FromBody] ProductLocation productLocation)
        {
            try
            {
                if (productLocation.ID <= 0)
                    await _productLocationRepo.CreateAsync(productLocation);
                else
                    await _productLocationRepo.UpdateAsync(productLocation);

                return Ok(ApiResponseFactory.Success("", "Lưu vị trí sản phẩm thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete-data")]
        public async Task<IActionResult> deleteData([FromBody] int id)
        {
            try
            {
                var productLocation = _productLocationRepo.GetByID(id);
                if (productLocation == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy vị trí sản phẩm"));
                }

                // Soft delete: cập nhật trường IsDelete thành true
                productLocation.IsDeleted = true;
                await _productLocationRepo.UpdateAsync(productLocation);

                return Ok(ApiResponseFactory.Success("", "Xóa vị trí sản phẩm thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}