using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.Asset;
namespace RERPAPI.Controllers.Old.Asset
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetTypeController : ControllerBase
    {

        TTypeAssetsRepo _typeAssetRepo = new TTypeAssetsRepo();
        [RequiresPermission("N23,N1")]
        [HttpGet("get-asset-type")]
        public IActionResult GetAssetType()
        {
            try
            {

                var tsAssets = _typeAssetRepo.GetAll().Where(x => x.IsDeleted !=true).OrderByDescending(x => x.CreatedDate).ToList();
                return Ok(new
                {
                    status = 1,
                    data = tsAssets
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
        [RequiresPermission("N23,N1")]
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] TSAsset typeasset)
        {
            try
            {
                if (typeasset != null && typeasset.IsDeleted != true)
                {
                    if (!_typeAssetRepo.Validate(typeasset, out string message))
                        return BadRequest(ApiResponseFactory.Fail(null, message));
                }
                if (typeasset.ID <= 0)
                {
                    await _typeAssetRepo.CreateAsync(typeasset);
                }
               
                else await _typeAssetRepo.UpdateAsync( typeasset);

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
