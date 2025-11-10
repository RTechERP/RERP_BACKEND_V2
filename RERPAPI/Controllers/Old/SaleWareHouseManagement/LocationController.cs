using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.DTO;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly LocationRepo _locationRepo;

        public LocationController(LocationRepo locationRepo)
        {
            _locationRepo = locationRepo;
        }

        // hàm lấy vị trí cho productSale combobox nếu productGroupID=70 thì tìm theeo ID70 nếu khác thì tìm ID0
        [HttpGet("get-location-by-product-group")]
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
        public async Task<IActionResult> saveLocation([FromBody] List<LocationDTO> dtos)
        {
            try
            {
                //TN.Binh update 19/10/25
                foreach (var dto in dtos)
                {
                    if (!CheckLocationCode(dto))
                    {
                        return Ok(new { status = 0, message = $"Mã vị trí [{dto.LocationCode}] đã tồn tại!" });
                    }
                }
                    foreach (var dto in dtos)
                {
                    if (dto.ID <= 0) await _locationRepo.CreateAsync(dto);
                    else await _locationRepo.UpdateAsync(dto);
                }

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

        //TN.Binh update 19/10/25
        #region check trùng mã sản phẩm khi thêm, sửa vị trí
        private bool CheckLocationCode(LocationDTO dto)
        {
            bool check = true;
            var exists = _locationRepo.GetAll()
                .Where(x => x.LocationCode == dto.LocationCode
                            && x.ID != dto.ID).ToList();
            if (exists.Count > 0) check = false;
            return check;
        }
        #endregion
        //end update
    }
}
