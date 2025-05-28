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
        TSAssetManagementRepo tasset = new TSAssetManagementRepo();
        TSAssetAllocationRepo tSAssetAllocationRepo = new TSAssetAllocationRepo();
        TSAssetAllocationDetailRepo tSAssetAllocationDetailRepo = new TSAssetAllocationDetailRepo();
        [HttpGet("getassetsallocationdetail")]
        public IActionResult GetAllocationDetail(string? ID)
        {
            try
            {
                var assetsallocationdetail = SQLHelper<dynamic>.ProcedureToList(
           "spGetTSAssetAllocationDetail",
           new string[] { "@TSAssetAllocationID" },
           new object[] { ID }
       );
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        assetsallocationdetail = SQLHelper<dynamic>.GetListData(assetsallocationdetail, 0)
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
        [HttpGet("getTSAssestAllocation")]
        public async Task<IActionResult> GetTSAssetAllocation(
                           DateTime? dateStart = null,
                           DateTime? dateEnd = null,
                           int? employeeID = null,
                           string? status = null,
                           string? filterText = null,
                           int pageSize = 100000,
                           int pageNumber = 1)
        {
            try
            {
                var assetallocation = SQLHelper<dynamic>.ProcedureToList(
                    "spGetTSAssetAllocation",
                    new string[] { "@DateStart", "@DateEnd", "@EmployeeID", "@Status", "@FilterText", "@PageSize", "@PageNumber" },
                    new object[] {
                dateStart,
                dateEnd,
                employeeID,
                status,
                filterText ?? string.Empty,
                pageSize,
                pageNumber
                    });
                var assetDate = dateStart ?? DateTime.Now;
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        assetallocation = SQLHelper<dynamic>.GetListData(assetallocation, 0)
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
        [HttpGet("generate-allocation-code")]
        public async Task<IActionResult> GenerateAllocationCode([FromQuery] DateTime? allocationDate)
        {
            if (allocationDate == null)
                return BadRequest("allocationDate is required.");

            string newcode = tSAssetAllocationRepo.GenerateAllocationCode(allocationDate);

            return Ok(new
            {
                status = 1,
                data = newcode
            });
        }
        [HttpPost("SaveAllocation")]
        public async Task<IActionResult> SaveAllocationn([FromBody] TSAssetAllocationFullDTO dto)
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
                    var existingAsset = tasset.GetByID(detail.AssetManagementID);

                    if (existingAsset != null)
                    {
                        existingAsset.EmployeeID = detail.EmployeeID;
                        existingAsset.DepartmentID = detail.DepartmentID;
                        existingAsset.UpdatedDate = DateTime.Now;
                        existingAsset.UpdatedBy = detail.UpdatedBy;
                        existingAsset.StatusID = 2;
                        existingAsset.Status = "Đang sử dụng";
                    }
                    await tasset.UpdateAsync(existingAsset);
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
                return Ok(new { status = 0, message = ex.Message });
            }
        }
        [HttpPost("savedata")]
        public async Task<IActionResult> SaveData([FromBody] List<TSAssetAllocation> allocation)
        {
            try
            {
                if (allocation != null && allocation.Any())
                {
                    foreach (var item in allocation)
                    {
                        if (item.ID > 0)
                        {                          
                                tSAssetAllocationRepo.UpdateFieldsByID(item.ID, item);
                        }
                        else
                        {
                            tSAssetAllocationRepo.Create(item);
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
