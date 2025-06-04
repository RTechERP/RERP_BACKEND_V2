using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Repo.GenericEntity;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Context;
using Microsoft.AspNetCore.Http.HttpResults;
using RERPAPI.Model.Param;
namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        TSLostReportAssetRepo tsLostReportRepo = new TSLostReportAssetRepo();
        TSAllocationEvictionAssetRepo tSAllocationEvictionRepo = new TSAllocationEvictionAssetRepo();
        TSReportBrokenAssetRepo tsReportBrokenAssetRepo = new TSReportBrokenAssetRepo();
        TSAssetManagementRepo tsAssetManagementRepo = new TSAssetManagementRepo();

        [HttpPost("getAssets")]
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
                        assets = SQLHelper<dynamic>.GetListData(assets, 0)
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
        [HttpGet("getAllocationDetail")]
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
        [HttpPost("saveData")]
        public async Task<IActionResult> SaveData([FromBody] TSAssetManagement asset)
        {
            try
            {
                if (asset.ID <= 0) await tsAssetManagementRepo.CreateAsync(asset);
                else await tsAssetManagementRepo.UpdateAsync(asset);

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
        [HttpGet("generateAllocationCodeAsset")]
        public async Task<IActionResult> GenerateAllocationCodeAsset([FromQuery] DateTime? assetDate)
        {
            if (assetDate == null)
                return BadRequest("allocationDate is required.");

            var newcode = tsAssetManagementRepo.GenerateAllocationCodeAsset(assetDate);

            return Ok(new
            {
                status = 1,
                data = newcode
            });
        }
        [HttpPost("saveData2")]
        public async Task<IActionResult> SaveData2([FromBody] AssetmanagementFullDTO asset)
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
                            await tsAssetManagementRepo.CreateAsync(item);
                        else
                            tsAssetManagementRepo.UpdateFieldsByID(item.ID, item);
                    }
                }
                if (asset.tSAllocationEvictionAssets != null && asset.tSAllocationEvictionAssets.Any())
                {
                    foreach (var item in asset.tSAllocationEvictionAssets)
                    {
                        if (item.ID <= 0)
                            await tSAllocationEvictionRepo.CreateAsync(item);
                        else
                            tSAllocationEvictionRepo.UpdateFieldsByID(item.ID, item);
                    }
                }
                if (asset.tSLostReportAsset != null)
                {
                    if (asset.tSLostReportAsset.ID <= 0)
                        await tsLostReportRepo.CreateAsync(asset.tSLostReportAsset);
                    else
                        tsLostReportRepo.UpdateFieldsByID(asset.tSLostReportAsset.ID, asset.tSLostReportAsset);
                }
                if (asset.tSReportBrokenAsset != null)
                {
                    if (asset.tSReportBrokenAsset.ID <= 0)
                        await tsReportBrokenAssetRepo.CreateAsync(asset.tSReportBrokenAsset);
                    else
                        tsReportBrokenAssetRepo.UpdateFieldsByID(asset.tSReportBrokenAsset.ID, asset.tSReportBrokenAsset);
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
