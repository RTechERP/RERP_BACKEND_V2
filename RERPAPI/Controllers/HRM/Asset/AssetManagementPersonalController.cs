using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO.Asset;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Model.Param.Asset;
using RERPAPI.Repo.GenericEntity.Asset;
using RERPAPI.Repo.GenericEntity.HRM.Vehicle;
using System;
using ZXing;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RERPAPI.Controllers.Old.Asset
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

        //Lấy danh sách loại tài sản
        [HttpGet("get-type-asset-personal")]
        public IActionResult GetVehicleRepairType()
        {
            try
            {
                var typeAsset = _typeAssetPersonalRepo.GetAll().FindAll(x => x.IsDeleted != true).ToList();
                return Ok(ApiResponseFactory.Success(typeAsset, "Lấy loại tài sản thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-all-asset-management-personal")]
        public IActionResult GetAllTSAssetManagementPersonal()
        {
            try
            {
                var tSAssetManagmentPersonal = SQLHelper<dynamic>.ProcedureToList(
                     "spGetTSAssetManagmentPersonal", new string[] { }, new object[] { });
                var dataList = SQLHelper<dynamic>.GetListData(tSAssetManagmentPersonal, 0);
                return Ok(ApiResponseFactory.Success(dataList, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
                //var maxSTT = dataList.Max(x => (int)x.STT);
                return Ok(ApiResponseFactory.Success(dataList, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-asset-allocation-personal-detail")]
        public IActionResult GetAssetAllocationPersonalDetail(int? TSAllocationAssetPersonalID, int? EmployeeID)
        {
            try
            {
                var assetsAllocationPersonalDetail = SQLHelper
                    <dynamic>.ProcedureToList("spGetTSAssetAllocationPersonalDetail", new string[] { "@TSAllocationAssetPersonalID", "@EmployeeID" }, new object[] { TSAllocationAssetPersonalID, EmployeeID });
                //return Ok(new
                //{
                //    status = 1,
                //    data = new
                //    {
                //        assetsAllocationPersonalDetail = SQLHelper<dynamic>.GetListData(assetsAllocationPersonalDetail, 0)
                //    }
                //});
                var dataList = SQLHelper<dynamic>.GetListData(assetsAllocationPersonalDetail, 0);
                return Ok(ApiResponseFactory.Success(dataList, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("get-asset-recovery-personal")]
        public async Task<ActionResult> GetAssetsRecoveryPerson(AssetRecoveryRequestParam request)
        {
            try
            {
                var assetsrecoveryPersonal = SQLHelper<dynamic>.ProcedureToList(
                    "spGetTSAssetRecoveryyPersonal",
                    new string[] { "@DateStart", "@DateEnd", "@EmployeeReturnID", "@EmployeeRecoveryID", "@Status", "@FilterText", "@PageSize", "@PageNumber" },
                    new object[] { request.DateStart, request.DateEnd, request.EmployeeRecoveryID, request.EmployeeReturnID, request.Status, request.Filtertext, request.PageSize, request.PageNumber });
                var dataList = SQLHelper<dynamic>.GetListData(assetsrecoveryPersonal, 0);
                var TotalPage = SQLHelper<object>.GetListData(assetsrecoveryPersonal, 1);
                var maxSTT = _tSRecoveryAssetPersonalRepo.GetAll().Max(x => (int?)x.STT) + 1 ?? 0;
                return Ok(ApiResponseFactory.Success(new { dataList, maxSTT, TotalPage }, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-asset-recovery-personal-detail")]
        public IActionResult GetAssetsRecoveryPersonalDetail(int? TSAssetRecoveryPersonID)
        {
            try
            {
                var result = SQLHelper<dynamic>.ProcedureToList(
             "spGetTSAssetRecoveryPersonalDetail",
             new string[] { "@TSAssetRecoveryPersonID" },
             new object[] { TSAssetRecoveryPersonID }
         );
                var dataList = SQLHelper<dynamic>.GetListData(result, 0);
                //return Ok(new
                //{
                //    status = 1,
                //    data = new
                //    {
                //        assetsRecoveryDetail = SQLHelper<dynamic>.GetListData(result, 0)
                //    }s
                //});
                return Ok(ApiResponseFactory.Success(dataList, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-allocation-personal-code")]
        public IActionResult GenerateAllocationPersonalCode([FromQuery] DateTime? allocationDate)
        {
            try
            {
                if (allocationDate == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Chưa chọn ngày cấp phát"));

                string newCode = _tSAllocationAssetPersonalRepo.generateAllocationPersonalCode(allocationDate);
                return Ok(ApiResponseFactory.Success(newCode, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }
        [HttpGet("get-recovery-personal-code")]
        public IActionResult GenerateRecoveryPersonalCode([FromQuery] DateTime? recoveryDate)
        {
            try
            {
                if (recoveryDate == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Chưa chọn ngày thu hồi"));

                string newCode = _tSRecoveryAssetPersonalRepo.generateRecoveryPersonalCode(recoveryDate);
                return Ok(ApiResponseFactory.Success(newCode, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

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
                        var maxSTT = _tSAssetManagementPersonalRepo.GetAll().Max(x => x.STT) + 1 ?? 0;
                        dto.tSAssetManagementPersonal.STT = maxSTT;
                        await _tSAssetManagementPersonalRepo.CreateAsync(dto.tSAssetManagementPersonal);
                    }
                    else
                    {
                        await _tSAssetManagementPersonalRepo.UpdateAsync(dto.tSAssetManagementPersonal);
                    }
                }
                if (dto.tSTypeAssetPersonal != null)
                {
                    if (dto.tSTypeAssetPersonal.ID <= 0)
                    {
                        var maxSTT = _typeAssetPersonalRepo.GetAll().Max(x => x.STT) + 1 ?? 0;
                        dto.tSTypeAssetPersonal.STT = maxSTT;
                        await _typeAssetPersonalRepo.CreateAsync(dto.tSTypeAssetPersonal);
                    }
                    else
                    {
                        await _typeAssetPersonalRepo.UpdateAsync(dto.tSTypeAssetPersonal);
                    }
                }
                if (dto.tSAllocationAssetPersonal != null)
                {

                    if (dto.tSAllocationAssetPersonal.ID <= 0)
                    {
                        var maxSTT = _tSAllocationAssetPersonalRepo.GetAll().Max(x => x.STT) + 1 ?? 0;
                        dto.tSAllocationAssetPersonal.STT = maxSTT;

                        await _tSAllocationAssetPersonalRepo.CreateAsync(dto.tSAllocationAssetPersonal);
                    }
                    else
                    {
                        await _tSAllocationAssetPersonalRepo.UpdateAsync(dto.tSAllocationAssetPersonal);
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
                        await _tSRecoveryAssetPersonalRepo.UpdateAsync(dto.tSRecoveryAssetPersonal);
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
                            await _tSRecoveryAssetPersonalDetailRepo.UpdateAsync(item);
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
                            await _tSAssetAllocationDetailRepo.UpdateAsync(item);
                    }
                }
                return Ok(ApiResponseFactory.Success(null, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
