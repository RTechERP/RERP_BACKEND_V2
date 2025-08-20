
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.Asset;

namespace RERPAPI.Controllers.Asset
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetSourceController : ControllerBase
    {

        TSSourceAssetsRepo _tsSourceAssetRepo = new TSSourceAssetsRepo();
        [HttpGet("get-source-asset")]
        public IActionResult GetSourceAssets()
        {            try
            {
                List<TSSourceAsset> tsSources = _tsSourceAssetRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = tsSources
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
        public async Task<IActionResult> SaveData([FromBody] TSSourceAsset sourceasset)
        {
            try
            {
                if (sourceasset.ID <= 0) await _tsSourceAssetRepo.CreateAsync(sourceasset);
                else _tsSourceAssetRepo.UpdateAsync( sourceasset);

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

