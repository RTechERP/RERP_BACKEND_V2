using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsStatusController : ControllerBase
    {
        
        TSStatusAssetRepo tsStatusAssetRepo = new TSStatusAssetRepo();
       
        [HttpGet("getAssetStatus")]
        public IActionResult GetStatus()
        {
            try
            {
                List<TSStatusAsset> tsStatusAssets = tsStatusAssetRepo.GetAll();
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
    }
}
