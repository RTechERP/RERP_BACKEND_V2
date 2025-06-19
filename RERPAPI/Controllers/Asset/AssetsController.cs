using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Common;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Context;
using Microsoft.AspNetCore.Http.HttpResults;
using RERPAPI.Model.DTO.Asset;
using RERPAPI.Model.Param.Asset;
using RERPAPI.Repo.GenericEntity.Asset;
namespace RERPAPI.Controllers.Asset
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        TSLostReportAssetRepo _tsLostReportRepo = new TSLostReportAssetRepo();
        TSAllocationEvictionAssetRepo _tSAllocationEvictionRepo = new TSAllocationEvictionAssetRepo();
        TSReportBrokenAssetRepo _tsReportBrokenAssetRepo = new TSReportBrokenAssetRepo();
        TSAssetManagementRepo _tsAssetManagementRepo = new TSAssetManagementRepo();
       
        [HttpPost("get-asset")]
        public IActionResult GetListAssets([FromBody] AssetmanagementRequestParam request)
        {
            try
            {
                var assets = SQLHelper<dynamic>.ProcedureToList("spLoadTSAssetManagement",
                    new string[] { "@FilterText", "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@Status", "@Department" },
                    new object[] { request.FilterText, request.PageNumber, request.PageSize, request.DateStart, request.DateEnd, request.Status, request.Department });
                
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        assets = SQLHelper<dynamic>.GetListData(assets, 0),
                        total = SQLHelper<dynamic>.GetListData(assets, 1)
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

        [HttpGet("get-allocation-detail")]
        public IActionResult GetAllocation(string? id)
        {
            try
            {
                var assetsAllocation = SQLHelper<dynamic>.ProcedureToList(
            "spLoadTSAllocationEvictionAsset",
            new string[] { "@ID" },
            new object[] { id });
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        assetsAllocation = SQLHelper<dynamic>.GetListData(assetsAllocation, 0)
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
        [HttpGet("get-asset-code")]
        public async Task<IActionResult> GenerateAssetCode([FromQuery] DateTime? assetDate)
        {
            if (assetDate == null)
                return BadRequest("AssetDate is required.");

            var newcode = _tsAssetManagementRepo.GenerateAssetCode(assetDate);

            return Ok(new
            {
                status = 1,
                data = newcode
            });
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] AssetmanagementFullDTO asset)
        {
            try
            {
                if (asset == null)
                {
                    return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." });
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
                            await _tSAllocationEvictionRepo.CreateAsync(item);
                        else
                            _tSAllocationEvictionRepo.UpdateFieldsByID(item.ID, item);
                    }
                }
                if (asset.tSLostReportAsset != null)
                {
                    if (asset.tSLostReportAsset.ID <= 0)
                        await _tsLostReportRepo.CreateAsync(asset.tSLostReportAsset);
                    else
                        _tsLostReportRepo.UpdateFieldsByID(asset.tSLostReportAsset.ID, asset.tSLostReportAsset);
                }
                if (asset.tSReportBrokenAsset != null)
                {
                    if (asset.tSReportBrokenAsset.ID <= 0)
                        await _tsReportBrokenAssetRepo.CreateAsync(asset.tSReportBrokenAsset);
                    else
                        _tsReportBrokenAssetRepo.UpdateFieldsByID(asset.tSReportBrokenAsset.ID, asset.tSReportBrokenAsset);
                }

                return Ok(new { status = 1, message = "Lưu dữ liệu thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 0,
                    message = "Lỗi xảy ra khi lưu dữ liệu.",
                    detail = ex.Message.ToString(),
                });
            }
        }
    }
}
