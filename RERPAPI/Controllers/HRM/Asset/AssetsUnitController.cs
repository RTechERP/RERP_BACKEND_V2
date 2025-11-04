using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.Asset;

namespace RERPAPI.Controllers.Old.Asset
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsUnitController : ControllerBase
    {
        UnitRepo _unitRepo = new UnitRepo();

        [HttpGet("get-unit")]
        public IActionResult GetAll()
        {
            try
            {
                var units = _unitRepo.GetAll();

                return Ok(new
                {
                    status = 1,
                    data = units
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

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] List<UnitCount> units)
        {
            try
            {
                if (units != null && units.Any())
                {
                    foreach (var item in units)
                    {
                        if (item.ID > 0) await _unitRepo.UpdateAsync(item);
                        else await _unitRepo.CreateAsync(item);

                    }
                }
                return Ok(new { status = 1 });
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
