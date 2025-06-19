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
using System.Collections.Generic;
using RERPAPI.Model.Param;
using RERPAPI.Model.DTO.Asset;
using RERPAPI.Repo.GenericEntity.Asset;

namespace RERPAPI.Controllers.Asset
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsAllocationController : ControllerBase
    {
        TSAssetManagementRepo _tsAssetManagementRepo = new TSAssetManagementRepo();
        TSAssetAllocationRepo _tSAssetAllocationRepo = new TSAssetAllocationRepo();
        TSAssetAllocationDetailRepo _tSAssetAllocationDetailRepo = new TSAssetAllocationDetailRepo();
        TSAllocationEvictionAssetRepo tSAllocationEvictionAssetRepo = new TSAllocationEvictionAssetRepo();
        [HttpPost("get-allocation")]
        public async Task<ActionResult> GetAssetAllocationnn([FromBody] AssetAllocationRequestParam request)
        {
            try
            {
                var assetAllocation = SQLHelper<dynamic>.ProcedureToList(
                    "spGetTSAssetAllocation",
                    new string[] { "@DateStart", "@DateEnd", "@EmployeeID", "@Status", "@FilterText", "@PageSize", "@PageNumber" },
                    new object[] { request.DateStart, request.DateEnd, request.EmployeeID, request.Status, request.FilterText ?? string.Empty, request.PageSize, request.PageNumber });

                return Ok(new
                {
                    status = 1,
                    assetAllocation = SQLHelper<dynamic>.GetListData(assetAllocation, 0)

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
        [HttpGet("get-asset-allocation-detail")]
        public IActionResult GetAssetAllocationDetail(string? id)
        {
            try
            {
                var assetsAllocationDetail = SQLHelper<dynamic>.ProcedureToList("spGetTSAssetAllocationDetail", new string[] { "@TSAssetAllocationID" }, new object[] { id });
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        assetsAllocationDetail = SQLHelper<dynamic>.GetListData(assetsAllocationDetail, 0)
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
        [HttpGet("get-allocation-code")]
        public async Task<IActionResult> GenerateAllocationCode([FromQuery] DateTime? allocationDate)
        {
            if (allocationDate == null)
                return BadRequest("allocationDate is required.");

            string newCode = _tSAssetAllocationRepo.generateAllocationCode(allocationDate);
            return Ok(new
            {
                status = 1,
                data = newCode
            });
        }
       
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] TSAssetAllocationFullDTO allocations)
        {
            try
            {
                if(allocations == null){ return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." }); }
                    if (allocations.tSAssetAllocation != null)
                    {
                        if (allocations.tSAssetAllocation.ID <= 0)
                            await _tSAssetAllocationRepo.CreateAsync(allocations.tSAssetAllocation);
                        else
                            _tSAssetAllocationRepo.UpdateFieldsByID(allocations.tSAssetAllocation.ID, allocations.tSAssetAllocation);
                    }
                
                if (allocations.tSAssetAllocationDetails != null && allocations.tSAssetAllocationDetails.Any())
                {
                    foreach (var item in allocations.tSAssetAllocationDetails)
                    {
                        item.TSAssetAllocationID = allocations.tSAssetAllocation.ID;
                        if (item.ID <= 0)
                            await _tSAssetAllocationDetailRepo.CreateAsync(item);
                        else
                            _tSAssetAllocationDetailRepo.UpdateFieldsByID(item.ID, item);
                    }
                }
                if (allocations.tSAssetManagements != null && allocations.tSAssetManagements.Any())
                {
                    foreach (var item in allocations.tSAssetManagements)
                    {
                      
                        if (item.ID <= 0)
                            await _tsAssetManagementRepo.CreateAsync(item);
                        else
                            _tsAssetManagementRepo.UpdateFieldsByID(item.ID, item);
                    }
                }
                if (allocations.tSAllocationEvictionAssets != null && allocations.tSAllocationEvictionAssets.Any())
                {
                    foreach (var item in allocations.tSAllocationEvictionAssets)
                    {

                        if (item.ID <= 0)
                            await tSAllocationEvictionAssetRepo.CreateAsync(item);
                        else
                            tSAllocationEvictionAssetRepo.UpdateFieldsByID(item.ID, item);
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
