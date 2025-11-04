using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitCountController : ControllerBase
    {
        UnitCountRepo _unitcountRepo = new UnitCountRepo();


        [HttpGet("")]
        public IActionResult getUnitCount()
        {

            try
            {
                List<UnitCount> dataUnit = _unitcountRepo.GetAll(x => x.IsDeleted == false);
                return Ok(new
                {
                    status = 1,
                    data = dataUnit,
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
        public async Task<IActionResult> saveUnitCount([FromBody] List<UnitCountDTO> dtos)
        {
            try
            {
                foreach (var dto in dtos)
                {
                    if (!_unitcountRepo.ValidateCode(dto))
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Mã đơn vị tính đã có!"));
                    }
                    if (dto.ID <= 0) await _unitcountRepo.CreateAsync(dto);
                    else await _unitcountRepo.UpdateAsync(dto);
                }
                return Ok(new
                {
                    status = 1,
                    message = "Thêm đơn vị tính thành công!",

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
