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
using System.Collections.Generic;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsAllocationController : ControllerBase
    {
        TSAssetManagementRepo tsAssetManagementRepo = new TSAssetManagementRepo();
        TSAssetAllocationRepo tSAssetAllocationRepo = new TSAssetAllocationRepo();
        TSAssetAllocationDetailRepo tSAssetAllocationDetailRepo = new TSAssetAllocationDetailRepo();
        [HttpGet("getAssetsAllocationDetail")]
        public IActionResult getAssetAllocationDetail(string? id)
        {
            try
            {
                var assetsAllocationDetail = SQLHelper<dynamic>.ProcedureToList(
           "spGetTSAssetAllocationDetail",
           new string[] { "@TSAssetAllocationID" },
           new object[] { id }
       );
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
        [HttpPost("getAllocation")]
        public async Task<ActionResult> getAssetAllocationnn([FromBody] AssetAllocationRequestDTO request)
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
        [HttpGet("generateAllocationCode")]
        public async Task<IActionResult> generateAllocationCode([FromQuery] DateTime? allocationDate)
        {
            if (allocationDate == null)
                return BadRequest("allocationDate is required.");

            string newCode = tSAssetAllocationRepo.generateAllocationCode(allocationDate); 
            return Ok(new
            {
                status = 1,
                data = newCode
            });
        }
        [HttpPost("saveAllocation")]
        public async Task<IActionResult> saveAllocationn([FromBody] TSAssetAllocationFullDTO dto)
        {
            try
            {
                var allocationModel = new TSAssetAllocation
                {
                    ID = dto.ID,
                    Code = dto.Code,
                    DateAllocation = dto.DateAllocation,
                    EmployeeID = dto.EmployeeID,
                    Note = dto.Note,
                    Status = 1
                };

                if (allocationModel.ID > 0)
                {
                    await tSAssetAllocationRepo.UpdateAsync(allocationModel);
                }
                else
                {
                    allocationModel.ID = await tSAssetAllocationRepo.CreateAsync(allocationModel);
                }
                foreach (var detail in dto.AssetDetails)
                {
                    var detailModel = new TSAssetAllocationDetail
                    {
                        ID = detail.ID,
                        STT = detail.STT,
                        AssetManagementID = detail.AssetManagementID,
                        TSAssetAllocationID = allocationModel.ID,
                        Quantity = detail.Quantity,
                        Note = detail.Note,
                        EmployeeID = detail.EmployeeID
                    };
                    if (detailModel.ID > 0)
                    {
                        await tSAssetAllocationDetailRepo.UpdateAsync(detailModel);
                    }
                    else
                    {
                        await tSAssetAllocationDetailRepo.CreateAsync(detailModel);
                    }
                    var existingAsset = tsAssetManagementRepo.GetByID(detail.AssetManagementID);

                    if (existingAsset != null)
                    {
                        existingAsset.EmployeeID = detail.EmployeeID;
                        existingAsset.DepartmentID = detail.DepartmentID;
                        existingAsset.UpdatedDate = DateTime.Now;
                        existingAsset.UpdatedBy = detail.UpdatedBy;
                        existingAsset.StatusID = 2;
                        existingAsset.Status = "Đang sử dụng";
                    }
                    await tsAssetManagementRepo.UpdateAsync(existingAsset);
                }
                return Ok(new
                {
                    status = 1,
                    message = "Lưu thành công",
                    data = allocationModel
                });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 0, message = ex.Message, error = ex.ToString() });
            }
        }
        [HttpPost("saveData")]
        public async Task<IActionResult> saveData([FromBody] List<TSAssetAllocation> allocations)
        {
            try
            {
                if (allocations != null && allocations.Any())
                {
                    foreach (var items in allocations)
                    {
                        if (items.ID > 0)
                        {
                            tSAssetAllocationRepo.UpdateFieldsByID(items.ID, items);
                        }
                        else
                        {
                            tSAssetAllocationRepo.CreateAsync(items);
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
