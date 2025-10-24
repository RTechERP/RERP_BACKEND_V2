using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Repo.GenericEntity;
using ZXing;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        LocationRepo _locationRepo = new LocationRepo();

        // hàm lấy vị trí cho productSale combobox nếu productGroupID=70 thì tìm theeo ID70 nếu khác thì tìm ID0
        [HttpGet("get-location-by-product-group")]
        public IActionResult getDataLocation(int productgroupID = 70)
        {
            try
            {
                var dataLocation = _locationRepo.GetAll()
           .Where(x => productgroupID == 70 ? x.ProductGroupID == 70 : true)
           .ToList();
                return Ok(ApiResponseFactory.Success(dataLocation, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
      

        [HttpPost("save-data")]
        public async Task<IActionResult> saveLocation([FromBody] List<LocationDTO> dtos)
        {
            try
            {
                foreach (var dto in dtos)
                {
                    if (dto.ID <= 0) await _locationRepo.CreateAsync(dto);
                    else await _locationRepo.UpdateAsync(dto);
                }

                return Ok(ApiResponseFactory.Success(null, "Xử lý dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
     
    }
}
