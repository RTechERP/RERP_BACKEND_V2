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
        TSAssetRecoveryRepo tSAssetRecoveryRepo = new TSAssetRecoveryRepo();
        TSLostReportAssetRepo tslostreport = new TSLostReportAssetRepo();
        TSAllocationEvictionAssetRepo tSAllocationEvictionrepo = new TSAllocationEvictionAssetRepo();
        TSReportBrokenAssetRepo reportrepo = new TSReportBrokenAssetRepo();
        TSStatusAssetRepo tSStatusAssetRepo = new TSStatusAssetRepo();
        TTypeAssetsRepo typerepo = new TTypeAssetsRepo();

        TSAssetManagementRepo tasset = new TSAssetManagementRepo();
        TSSourceAssetsRepo tssourcerepo = new TSSourceAssetsRepo();
        TSAssetAllocationRepo tSAssetAllocationRepo = new TSAssetAllocationRepo();

        TSAssetAllocationDetailRepo tSAssetAllocationDetailRepo = new TSAssetAllocationDetailRepo();
        [HttpGet("getsourceassets")]
        public IActionResult GetSourceAssets()
        {
            try
            {
                List<TSSourceAsset> tSSources = tssourcerepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = tSSources
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
