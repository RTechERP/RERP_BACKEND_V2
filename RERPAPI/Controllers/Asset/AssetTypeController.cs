using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.Asset;
namespace RERPAPI.Controllers.Asset
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetTypeController : ControllerBase
    {
      
        TTypeAssetsRepo _typeAssetRepo = new TTypeAssetsRepo();

        [HttpGet("get-asset-type")]
        public IActionResult GetAssetType()
        {
            try
            {

                var tsAssets = _typeAssetRepo.GetAll().Where(x => x.IsDeleted == false).ToList();
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
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] TSAsset typeasset)
        {
            try
            {
                if (typeasset.ID <= 0) await _typeAssetRepo.CreateAsync(typeasset);
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
