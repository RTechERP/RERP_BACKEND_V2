using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.DTO;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        LocationRepo _locationRepo = new LocationRepo();

        // hàm lấy vị trí cho productSale combobox nếu productGroupID=70 thì tìm theeo ID70 nếu khác thì tìm ID0
        [HttpGet("get-by-product-group{productgroupID}")]
        public IActionResult getDataLocation(int productgroupID = 70)
        {
            try
            {
                var dataLocation = _locationRepo.GetAll()
           .Where(x => productgroupID == 70 ? x.ProductGroupID == 70 : true)
           .ToList();
                return Ok(new
                {
                    status = 1,
                    data = dataLocation
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
      

        [HttpPost("save-data")]
        public async Task<IActionResult> saveLocation([FromBody] LocationDTO dto)
        {
            try
            {
                if (dto.ID <= 0) await _locationRepo.CreateAsync(dto);
                else await _locationRepo.UpdateAsync(dto);

                return Ok(new
                {
                    status = 1,
                    message = "Thêm vị trí mới thành công!",

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
