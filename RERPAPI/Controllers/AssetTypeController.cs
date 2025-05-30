using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetTypeController : ControllerBase
    {
      
        TTypeAssetsRepo typeAssetRepo = new TTypeAssetsRepo();

        [HttpGet("getAssetType")]
        public IActionResult getAssetType()
        {
            try
            {
                List<TSAsset> tsAssets = typeAssetRepo.GetAll();
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
        

    }
}
