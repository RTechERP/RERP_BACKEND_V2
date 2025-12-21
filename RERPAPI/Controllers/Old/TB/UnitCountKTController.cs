using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.TB
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitCountKTController : ControllerBase
    {
        private readonly UnitCountKTRepo _unitCountKTRepo;
        public UnitCountKTController(UnitCountKTRepo unitCountKTRepo)
        {
            _unitCountKTRepo = unitCountKTRepo;
        }


        [HttpGet("get-all")]
        public IActionResult GetUnitCount()
        {
            try
            {
                var rs = _unitCountKTRepo.GetAll(x => x.IsDeleted == false);
                return Ok(ApiResponseFactory.Success(rs, ""));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData(List<UnitCountKT> unitCounts)
        {
            try
            {
                foreach (var unitCount in unitCounts)
                {
                    if (unitCount == null) return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu trả về để lưu"));
                    if (unitCount.ID <= 0) await _unitCountKTRepo.CreateAsync(unitCount);
                    else await _unitCountKTRepo.UpdateAsync(unitCount);
                }

                return Ok(ApiResponseFactory.Success(unitCounts, ""));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
