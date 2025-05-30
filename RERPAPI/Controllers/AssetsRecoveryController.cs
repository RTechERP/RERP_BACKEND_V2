using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsRecoveryController : ControllerBase
    {
        TSAssetRecoveryRepo tSAssetRecoveryRepo = new TSAssetRecoveryRepo();
        [HttpGet("getAssetRecovery")]
        public async Task<ActionResult> GetAssetsRecovery(
     DateTime? dateStart = null, DateTime? dateEnd = null,
     int? employeeReturnID = null, int? employeeRecoveryID = null,
     int? status = null, string? filterText = null,
     int pageSize = 100000, int pageNumber = 1)
        {
            try
            {
                var assetRecovery = SQLHelper<dynamic>.ProcedureToList(
                    "spGetTSAssetRecovery",
                    new string[] {
                "@DateStart", "@DateEnd", "@EmployeeReturnID",
                "@EmployeeRecoveryID", "@Status", "@FilterText",
                "@PageSize", "@PageNumber"
                    },
                    new object[] {
                dateStart ?? DateTime.MinValue,
                dateEnd ?? DateTime.MaxValue,
                employeeReturnID ?? 0,
                employeeRecoveryID ?? 0,
                status ?? -1,
                filterText ?? "",
                pageSize,
                pageNumber
                    });


                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        assetsrecovery = SQLHelper<dynamic>.GetListData(assetRecovery, 0)
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
        [HttpGet("getAssetsRecoveryDetail")]
        public IActionResult getAssetsRecoveryDetail(int? id)
        {
            try
            {
                var result = SQLHelper<dynamic>.ProcedureToList(
             "spGetTSAssetRecoveryDetail",
             new string[] { "@TSAssetRecoveryID" },
             new object[] { id}
         );
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        assetsRecoveryDetail = SQLHelper<dynamic>.GetListData(result, 0)
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
        [HttpGet("generateRecoveryCode")]
        public async Task<IActionResult> generateRecoveryCode([FromQuery] DateTime? recoveryDate)
        {
            if (recoveryDate == null)
                return BadRequest("recoveryDate is required.");

            string newCode = tSAssetRecoveryRepo.genCodeRecovery(recoveryDate);

            return Ok(new
            {
                status = 1,
                data = newCode
            });
        }
        [HttpGet("getRecoveryByEmployee")]
        public IActionResult getRecoveryByEmployee( int? recoveID, int? employeeID)
        {
            try
            {
                var assetsRecoveryByEmployee = SQLHelper<dynamic>.ProcedureToList(
                    "spGetTSAssetByEmployee",
                    new string[] { "@EmployeeID", "@StatusID" },
                    new object[] { employeeID, 0 });
                if (recoveID > 0)
                {
                    var mergedAssets = SQLHelper<dynamic>.ProcedureToList(
                        "spGetTSAssetByEmployeeMerge",
                        new string[] { "@TSAssetID", "@TSAssetType" },
                        new object[] { recoveID, 2 });
                    assetsRecoveryByEmployee.AddRange(mergedAssets);
                }
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        assetsRecoveryByEmployee = SQLHelper<dynamic>.GetListData(assetsRecoveryByEmployee, 0)
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
        [HttpPost("saveData")]
        public async Task<IActionResult> saveData([FromBody] List<TSAssetRecovery> assetRecoverys)
        {
            try
            {
                if (assetRecoverys != null && assetRecoverys.Any())
                {
                    foreach (var items in assetRecoverys)
                    {
                        if (items.ID > 0)
                        {
                            tSAssetRecoveryRepo.UpdateFieldsByID(items.ID, items);
                        }
                        else
                        {
                            tSAssetRecoveryRepo.CreateAsync(items);
                        }
                    }
                }
                return Ok(new { status = 1 });
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
