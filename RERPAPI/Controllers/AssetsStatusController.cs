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
        [HttpPost("saveData")]
        public async Task<IActionResult> SaveData([FromBody] TSStatusAsset statusasset)
        {
            try
            {
                if (statusasset.ID <= 0) await tsStatusAssetRepo.CreateAsync(statusasset);
                else tsStatusAssetRepo.UpdateFieldsByID(statusasset.ID, statusasset);

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
