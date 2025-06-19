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
using RERPAPI.Model.Param;
using Azure.Core;
using RERPAPI.Model.DTO.Asset;
using RERPAPI.Model.Param.Asset;
using RERPAPI.Repo.GenericEntity.Asset;

namespace RERPAPI.Controllers.Asset
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetManagementPersonalController : ControllerBase
    {
        TSAllocationAssetPersonalRepo _tSAllocationAssetPersonalRepo = new TSAllocationAssetPersonalRepo();
        TSAllocationAssetPersonalDetailRepo _tSAssetAllocationDetailRepo = new TSAllocationAssetPersonalDetailRepo();
        TSRecoveryAssetPersonalDetailRepo _tSRecoveryAssetPersonalDetailRepo = new TSRecoveryAssetPersonalDetailRepo();
        TSRecoveryAssetPersonalRepo _tSRecoveryAssetPersonalRepo = new TSRecoveryAssetPersonalRepo();
        TSAssetManagementPersonalRepo _tSAssetManagementPersonalRepo = new TSAssetManagementPersonalRepo();
        TSTypeAssetPersonalRepo _typeAssetPersonalRepo = new TSTypeAssetPersonalRepo();

        [HttpGet("get-all-asset-management-personal")]
        public IActionResult GetAllTSAssetManagementPersonal()
        {
            try
            {
                var tSAssetManagmentPersonal = SQLHelper<dynamic>.ProcedureToList(
                     "spGetTSAssetManagmentPersonal", new string[] { }, new object[] { });
                return Ok(new
                {
                    status = 1,
                    tSAssetManagmentPersonal = SQLHelper<dynamic>.GetListData(tSAssetManagmentPersonal, 0)
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
        [HttpPost("get-asset-allocation-personal")]
        public async Task<ActionResult> GetAssetAllocationPersonal([FromBody] AssetAllocationRequestParam request)
        {
            try
            {
                var assetAllocationPersonal = SQLHelper<dynamic>.ProcedureToList(
                "spGetTSAssetAllocationPersonal",
                new string[] { "@DateStart", "@DateEnd", "@EmployeeID", "@Status", "@FilterText", "@PageSize", "@PageNumber" },
                    new object[] { request.DateStart, request.DateEnd, request.EmployeeID, request.Status, request.FilterText ?? string.Empty, request.PageSize, request.PageNumber });
                var dataList = SQLHelper<dynamic>.GetListData(assetAllocationPersonal, 0);
                var maxSTT = dataList.Max(x => (int)x.STT);
                return Ok(new
                {
                    status = 1,
                    assetAllocationPersonal = SQLHelper<dynamic>.GetListData(assetAllocationPersonal, 0),
                    MaxSTT=maxSTT,
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

        [HttpGet("get-asset-allocation-personal-detail")]
        public IActionResult GetAssetAllocationPersonalDetail(int? TSAllocationAssetPersonalID, int? EmployeeID)
        {
            try
            {
                var assetsAllocationPersonalDetail = SQLHelper
                    <dynamic>.ProcedureToList("spGetTSAssetAllocationPersonalDetail", new string[] { "@TSAllocationAssetPersonalID", "@EmployeeID" }, new object[] { TSAllocationAssetPersonalID, EmployeeID });
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        assetsAllocationPersonalDetail = SQLHelper<dynamic>.GetListData(assetsAllocationPersonalDetail, 0)
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
        [HttpPost("get-asset-recovery-personal")]
        public async Task<ActionResult> GetAssetsRecoveryPerson(AssetRecoveryRequestParam request)
        {
            try
            {
                var assetsrecoveryPersonal = SQLHelper<dynamic>.ProcedureToList(
                    "spGetTSAssetRecoveryPersonal",
                    new string[] {"@DateStart", "@DateEnd", "@EmployeeReturnID","@EmployeeRecoveryID", "@Status", "@FilterText","@PageSize", "@PageNumber"},
                    new object[] {request.DateStart,request.DateEnd,request.EmployeeRecoveryID,request.EmployeeReturnID,request.Status,request.Filtertext,request.PageSize, request.PageNumber });
                var dataList = SQLHelper<dynamic>.GetListData(assetsrecoveryPersonal, 0);

                var maxSTT = dataList.Max(x => (int)x.STT);
                return Ok(new
                {
                    status = 1,
                    assetsrecoveryPersonal = SQLHelper<dynamic>.GetListData(assetsrecoveryPersonal, 0),
                    maxSTT
                   
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
        [HttpGet("get-asset-recovery-personal-detail")]
        public IActionResult GetAssetsRecoveryPersonalDetail(int? TSAssetRecoveryPersonID, int? EmployeeID)
        {
            try
            {
                var result = SQLHelper<dynamic>.ProcedureToList(
             "spGetTSAssetRecoveryPersonalDetail",
             new string[] { "@TSAssetRecoveryPersonID", "@EmployeeID" },
             new object[] { TSAssetRecoveryPersonID, EmployeeID }
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
        [HttpGet("get-allocation-personal-code")]
        public async Task<IActionResult> GenerateAllocationPersonalCode([FromQuery] DateTime? allocationDate)
        {
            if (allocationDate == null)
                return BadRequest("allocationDate is required.");

            string newCode = _tSAllocationAssetPersonalRepo.generateAllocationPersonalCode(allocationDate);
            return Ok(new
            {
                status = 1,
                data = newCode
            });
        }
        [HttpGet("get-recovery-personal-code")]
        public async Task<IActionResult> GenerateRecoveryPersonalCode([FromQuery] DateTime? recoveryDate)
        {
            if (recoveryDate == null)
                return BadRequest("recoveryDate is required.");

            string newCode = _tSRecoveryAssetPersonalRepo.generateRecoveryPersonalCode(recoveryDate);
            return Ok(new
            {
                status = 1,
                data = newCode
            });
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] TSAssetManagementPersonalFullDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." });
                }
                if (dto.tSAssetManagementPersonal != null)
                {
                    if (dto.tSAssetManagementPersonal.ID <= 0)
                    {
                        await _tSAssetManagementPersonalRepo.CreateAsync(dto.tSAssetManagementPersonal);
                    }
                    else
                    {
                        _tSAssetManagementPersonalRepo.UpdateFieldsByID(dto.tSAssetManagementPersonal.ID, dto.tSAssetManagementPersonal);
                    }
                }
                if (dto.tSTypeAssetPersonal != null)
                {
                    if (dto.tSTypeAssetPersonal.ID <= 0)
                    {
                        await _typeAssetPersonalRepo.CreateAsync(dto.tSTypeAssetPersonal);
                    }
                    else
                    {
                        _typeAssetPersonalRepo.UpdateFieldsByID(dto.tSTypeAssetPersonal.ID, dto.tSTypeAssetPersonal);
                    }
                }
                if (dto.tSAllocationAssetPersonal != null)
                {
                    if (dto.tSAllocationAssetPersonal.ID <= 0)
                    {
                        await _tSAllocationAssetPersonalRepo.CreateAsync(dto.tSAllocationAssetPersonal);
                    }
                    else
                    {
                        _tSAllocationAssetPersonalRepo.UpdateFieldsByID(dto.tSAllocationAssetPersonal.ID, dto.tSAllocationAssetPersonal);
                    }
                }
                if (dto.tSRecoveryAssetPersonal != null)
                {

                    if (dto.tSRecoveryAssetPersonal.ID <= 0)
                    {
                        await _tSRecoveryAssetPersonalRepo.CreateAsync(dto.tSRecoveryAssetPersonal);
                    }
                    else
                    {
                        _tSRecoveryAssetPersonalRepo.UpdateFieldsByID(dto.tSRecoveryAssetPersonal.ID, dto.tSRecoveryAssetPersonal);
                    }
                }
                if (dto.tSRecoveryAssetPersonalDetails != null && dto.tSRecoveryAssetPersonalDetails.Any())
                {
                    foreach (var item in dto.tSRecoveryAssetPersonalDetails)
                    {
                        item.TSRecoveryAssetPersonalID = dto.tSRecoveryAssetPersonal.ID;
                        if (item.ID <= 0)
                            await _tSRecoveryAssetPersonalDetailRepo.CreateAsync(item);
                        else
                            _tSRecoveryAssetPersonalDetailRepo.UpdateFieldsByID(item.ID, item);
                    }
                }
                if (dto.tSAllocationAssetPersonalDetails != null && dto.tSAllocationAssetPersonalDetails.Any())
                {

                    foreach (var item in dto.tSAllocationAssetPersonalDetails)
                    {
                        item.TSAllocationAssetPersonalID = dto.tSAllocationAssetPersonal.ID;
                        if (item.ID <= 0)
                            await _tSAssetAllocationDetailRepo.CreateAsync(item);
                        else
                            _tSAssetAllocationDetailRepo.UpdateFieldsByID(item.ID, item);
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
