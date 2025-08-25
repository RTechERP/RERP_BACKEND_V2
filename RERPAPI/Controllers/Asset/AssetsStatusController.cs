using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.Asset;

namespace RERPAPI.Controllers.Asset
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsStatusController : ControllerBase
    {

        TSStatusAssetRepo _tsStatusAssetRepo = new TSStatusAssetRepo();

        [HttpGet("get-asset-status")]
        public IActionResult GetStatus()
        {
            try
            {
                var tsStatusAssets = _tsStatusAssetRepo.GetAll().Where(x => x.IsDeleted == false).ToList();

                return Ok(new
                {
                    status = 1,
                    data = tsStatusAssets
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
        public async Task<IActionResult> SaveData([FromBody] TSStatusAsset statusasset)
        {
            try
            {
                if (statusasset.ID <= 0) await _tsStatusAssetRepo.CreateAsync(statusasset);
                else await _tsStatusAssetRepo.UpdateAsync(statusasset);

                return Ok(new
                {
                    status = 1,
                    message = "Lưu thành công.",
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
