using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsRecoveryController : ControllerBase
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
        [HttpGet("getTSAssetRecovery")]
        public async Task<ActionResult> GetTSAssetsRecovery(
     DateTime? Datestart = null, DateTime? dateEnd = null,
     int? employeereturnID = null, int? employeeRecoveryID = null,
     int? status = null, string? Filtertext = null,
     int pagesize = 100000, int pagenumber = 1)
        {
            try
            {
                var assetsrecovery = SQLHelper<dynamic>.ProcedureToList(
                    "spGetTSAssetRecovery",
                    new string[] {
                "@DateStart", "@DateEnd", "@EmployeeReturnID",
                "@EmployeeRecoveryID", "@Status", "@FilterText",
                "@PageSize", "@PageNumber"
                    },
                    new object[] {
                Datestart ?? DateTime.MinValue,
                dateEnd ?? DateTime.MaxValue,
                employeereturnID ?? 0,
                employeeRecoveryID ?? 0,
                status ?? -1,
                Filtertext ?? "",
                pagesize,
                pagenumber
                    });


                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        assetsrecovery = SQLHelper<dynamic>.GetListData(assetsrecovery, 0)
                    }
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
        [HttpGet("getassetsrecoveryDetail")]
        public IActionResult GetAssetsRecoveryDetail(int? ID)
        {
            try
            {
                var result = SQLHelper<dynamic>.ProcedureToList(
             "spGetTSAssetRecoveryDetail",
             new string[] { "@TSAssetRecoveryID" },
             new object[] { ID }
         );
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        assetsrecoverydetail = SQLHelper<dynamic>.GetListData(result, 0)
                    }
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
        [HttpGet("generaterecoverycode")]
        public async Task<IActionResult> GenerateRecoveryCode([FromQuery] DateTime? recoveryDate)
        {
            if (recoveryDate == null)
                return BadRequest("recoveryDate is required.");

            string newcode = tSAssetRecoveryRepo.GenCodeRecovery(recoveryDate);

            return Ok(new
            {
                status = 1,
                data = newcode
            });
        }
        [HttpGet("getrecoverybyemployee")]
        public IActionResult GetRecoveryByEmployee(int? ID, int? RecoveID, int? employeeID)
        {
            try
            {
                var assetsrecoverybyemployee = SQLHelper<dynamic>.ProcedureToList(
                  "spGetTSAssetByEmployee",
                  new string[] { "@EmployeeID", "@StatusID" },
                  new object[] { employeeID, 0 });
                if (RecoveID > 0)
                {
                    assetsrecoverybyemployee = SQLHelper<dynamic>.ProcedureToList(
                  "spGetTSAssetByEmployeeMerge",
                  new string[] { "@TSAssetID", "@TSAssetType" },
                  new object[] { RecoveID, 2 });
                }
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        assetsrecoverybyemployee = SQLHelper<dynamic>.GetListData(assetsrecoverybyemployee, 0)
                    }
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
