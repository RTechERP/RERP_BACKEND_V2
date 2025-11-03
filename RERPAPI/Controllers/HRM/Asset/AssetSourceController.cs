
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.Asset;
using RERPAPI.Repo.GenericEntity.HRM.Vehicle;

namespace RERPAPI.Controllers.Old.Asset
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
                List<TSSourceAsset> tsSources = _tsSourceAssetRepo.GetAll(x=>x.IsDeleted!=true).OrderByDescending(x => x.CreatedDate).ToList()  ;
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
                if (sourceasset!= null && sourceasset.IsDeleted != true)
                {
                    if (!_tsSourceAssetRepo.Validate(sourceasset, out string message))
                        return BadRequest(ApiResponseFactory.Fail(null, message));
                }

                if (sourceasset.ID <= 0)
                {

                    await _tsSourceAssetRepo.CreateAsync(sourceasset);
                }
                else
                {
                    await _tsSourceAssetRepo.UpdateAsync(sourceasset);
                }
                return Ok(new
                {
                    status = 1,
                    message = "Lưu thành công.",
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

