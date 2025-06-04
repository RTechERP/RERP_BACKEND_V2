using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetSourceController : ControllerBase
    {

        TSSourceAssetsRepo tsSourceAssetRepo = new TSSourceAssetsRepo();
        [HttpGet("getsourceassets")]
        public IActionResult GetSourceAssets()
        {
            try
            {
                List<TSSourceAsset> tsSources = tsSourceAssetRepo.GetAll();
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
        [HttpPost("saveData")]
        public async Task<IActionResult> SaveData([FromBody] TSSourceAsset sourceasset)
        {
            try
            {
                if (sourceasset.ID <= 0) await tsSourceAssetRepo.CreateAsync(sourceasset);
                else tsSourceAssetRepo.UpdateFieldsByID(sourceasset.ID, sourceasset);

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
