using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.Asset;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.Asset;
using RERPAPI.Repo.GenericEntity.Asset;

namespace RERPAPI.Controllers.Asset
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsRecoveryController : ControllerBase
    {
        TSAssetRecoveryRepo _tSAssetRecoveryRepo = new TSAssetRecoveryRepo();
        TSAssetRecoveryDetailRepo _tSAssetRecoveryDetailRepo = new TSAssetRecoveryDetailRepo();
        TSAssetManagementRepo _tsAssetManagementRepo = new TSAssetManagementRepo();
        TSAllocationEvictionAssetRepo _tSAllocationEvictionAssetRepo = new TSAllocationEvictionAssetRepo();

        [HttpPost("get-asset-recovery")]
        public async Task<ActionResult> GetAssetsRecovery(AssetRecoveryRequestParam request)
        {
            try
            {
                var assetRecovery = SQLHelper<dynamic>.ProcedureToList("spGetTSAssetRecovery",
                    new string[] { "@DateStart", "@DateEnd", "@EmployeeReturnID", "@EmployeeRecoveryID", "@Status", "@FilterText", "@PageSize", "@PageNumber" },
                    new object[] { request.DateStart, request.DateEnd, request.EmployeeReturnID ?? 0, request.EmployeeRecoveryID ?? 0, request.Status ?? -1, request.Filtertext ?? "", request.PageSize, request.PageNumber, });
                return Ok(new
                {
                    status = 1,
                   
                        assetsrecovery = SQLHelper<dynamic>.GetListData(assetRecovery, 0)
                    
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
        [HttpGet("get-asset-recovery-detail")]
        public IActionResult GetAssetsRecoveryDetail(int? id)
        {
            try
            {
                var result = SQLHelper<dynamic>.ProcedureToList(
             "spGetTSAssetRecoveryDetail",
             new string[] { "@TSAssetRecoveryID" },
             new object[] { id }
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
        [HttpGet("get-recovery-code")]
        public async Task<IActionResult> GenerateRecoveryCode([FromQuery] DateTime? recoveryDate)
        {
            if (recoveryDate == null)
                return BadRequest("recoveryDate is required.");

            string newCode = _tSAssetRecoveryRepo.genCodeRecovery(recoveryDate);

            return Ok(new
            {
                status = 1,
                data = newCode
            });
        }
        [HttpGet("get-recovery-by-employee")]
        public IActionResult GetRecoveryByEmployee(int? recoveID, int? employeeID)
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
                   
                        assetsRecoveryByEmployee = SQLHelper<dynamic>.GetListData(assetsRecoveryByEmployee, 0)
                   
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
        public async Task<IActionResult> SaveData2([FromBody] AssetRecoveryDTO asset)
        {
            try
            {
                if (asset == null)
                {
                    return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." });
                }
                if (asset.tSAssetRecovery != null)
                {
                    if (asset.tSAssetRecovery.ID <= 0)
                    {
                        await _tSAssetRecoveryRepo.CreateAsync(asset.tSAssetRecovery);
                    }
                    else
                    {
                        _tSAssetRecoveryRepo.UpdateFieldsByID(asset.tSAssetRecovery.ID, asset.tSAssetRecovery);
                    }
                }
                if (asset.TSAssetRecoveryDetails != null && asset.TSAssetRecoveryDetails.Any())
                {
                    foreach (var item in asset.TSAssetRecoveryDetails)
                    {
                        item.TSAssetRecoveryID = asset.tSAssetRecovery.ID;
                        if (item.ID <= 0)
                            await _tSAssetRecoveryDetailRepo.CreateAsync(item);
                        else
                            _tSAssetRecoveryDetailRepo.UpdateFieldsByID(item.ID, item);
                    }
                }
                if (asset.tSAssetManagements != null && asset.tSAssetManagements.Any())
                {
                    foreach (var item in asset.tSAssetManagements)
                    {

                        if (item.ID <= 0)
                            await _tsAssetManagementRepo.CreateAsync(item);
                        else
                            _tsAssetManagementRepo.UpdateFieldsByID(item.ID, item);
                    }
                }
                if (asset.tSAllocationEvictionAssets != null && asset.tSAllocationEvictionAssets.Any())
                {
                    foreach (var item in asset.tSAllocationEvictionAssets)
                    {

                        if (item.ID <= 0)
                            await _tSAllocationEvictionAssetRepo.CreateAsync(item);
                        else
                            _tSAllocationEvictionAssetRepo.UpdateFieldsByID(item.ID, item);
                    }
                }
                return Ok(new
                {
                    status = 1,
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
