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
        [HttpGet("getAllAssetsManagement")]
        public IActionResult getAllAsset()
        {
            try
            {
                List<TSAssetManagement> tSAssetManagements = tsAssetManagementRepo.GetAll();
                var maxSTT = tSAssetManagements
                          .Where(x => x.STT.HasValue)
                          .Max(x => x.STT);
                return Ok(new
                {
                    status = 1,
                    data = maxSTT
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

        [HttpGet("getAllReportBroken")]
        public IActionResult getAllReportBroken()
        {
            try
            {
                List<TSReportBrokenAsset> reportBroken = tsReportBrokenAssetRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = reportBroken
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



        [HttpPost("getAssets")]
        public IActionResult getListAssets([FromBody] AssetmanagementRequestDTO request)
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
        public IActionResult getAllocation(string? id)
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
     
        public async Task<IActionResult> saveData([FromBody] TSAssetManagement asset)
        {
            try
            {
                if (asset.ID <= 0) await tsAssetManagementRepo.CreateAsync(asset);
                else await tsAssetManagementRepo.UpdateAsync(asset);

                return Ok(new
                {
                    status = 1,
                    data = asset
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
        [HttpPost("saveDataReportBroken")]
        public async Task<IActionResult> saveReportBroken([FromBody] ReportBrokenFullDto dto)
        {
            try
            {

                int? chucVuId = 30;
                var assetManagementData = tsAssetManagementRepo.GetByID(dto.AssetID) ?? new TSAssetManagement();
                assetManagementData.ID = dto.AssetID;
                assetManagementData.Note = dto.Note;
                assetManagementData.Status = dto.Status;
                assetManagementData.StatusID = dto.StatusID;
                if (assetManagementData.ID > 0)
                    await tsAssetManagementRepo.UpdateAsync(assetManagementData);
                else
                    await tsAssetManagementRepo.CreateAsync(assetManagementData);


                var reportBrokenData = new TSReportBrokenAsset

                {
                    AssetManagementID = dto.AssetID,
                    DateReportBroken = dto.DateReportBroken,
                    Reason = dto.Reason,
                    CreatedDate = dto.DateReportBroken,
                    UpdatedDate = dto.DateReportBroken

                };
                await tsReportBrokenAssetRepo.CreateAsync(reportBrokenData);

                // 3. Thêm  
                var allocationEvictionAsset = new TSAllocationEvictionAsset
                {
                    AssetManagementID = dto.AssetID,
                    EmployeeID = dto.EmployeeID,
                    DepartmentID = dto.DepartmentID,
                    ChucVuID = chucVuId ?? 0,
                    DateAllocation = dto.DateReportBroken,
                    Note = dto.Reason,
                    Status = "Hỏng"
                };
                await tSAllocationEvictionRepo.CreateAsync(allocationEvictionAsset);

                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        assetManagementData,
                        reportBrokenData,
                        allocationEvictionAsset
                    }
                });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 0, message = ex.Message });
            }
        }
        [HttpPost("saveDataReportLost")]
        public async Task<IActionResult> saveReportLost([FromBody] ReportLostFullDto dto)
        {
            try
            {
                int? chucVuId = 30;
                var assetManagementData = tsAssetManagementRepo.GetByID(dto.AssetManagementID) ?? new TSAssetManagement();
                assetManagementData.ID = dto.AssetManagementID;
                assetManagementData.Note = dto.Note;
                assetManagementData.Status = dto.AssetStatus;
                assetManagementData.StatusID = dto.AssetStatusID;
                assetManagementData.UpdatedDate = dto.UpdatedDate;
                assetManagementData.UpdatedBy = dto.UpdatedBy;
                if (assetManagementData.ID > 0)
                    await tsAssetManagementRepo.UpdateAsync(assetManagementData);
                else
                    await tsAssetManagementRepo.CreateAsync(assetManagementData);
                var reportLostData = new TSLostReportAsset
                {
                    AssetManagementID = dto.AssetManagementID,
                    DateLostReport = dto.DateLostReport,
                    Reason = dto.Reason,
                    CreatedDate = dto.CreatedDate,
                    CreatedBy = dto.CreatedBy,
                    UpdatedDate = dto.UpdatedDate,
                    UpdatedBy = dto.UpdatedBy
                };
                await tsLostReportRepo.CreateAsync(reportLostData);
                var allocationEvictionAsset = new TSAllocationEvictionAsset
                {
                    AssetManagementID = dto.AssetManagementID,
                    EmployeeID = dto.EmployeeID,
                    DepartmentID = dto.DepartmentID,
                    ChucVuID = chucVuId ?? 0,
                    DateAllocation = dto.DateAllocation,
                    Note = dto.Reason,
                    Status = "Hỏng",
                    CreatedDate = dto.CreatedDate,
                    CreatedBy = dto.CreatedBy,
                    UpdatedDate = dto.UpdatedDate,
                    UpdatedBy = dto.UpdatedBy
                };
                await tSAllocationEvictionRepo.CreateAsync(allocationEvictionAsset);
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        assetManagementData,
                        reportLostData,
                        allocationEvictionAsset
                    }
                });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 0, message = ex.Message });
            }
        }
        [HttpGet("generateAllocationCodeAsset")]
        public async Task<IActionResult> generateAllocationCodeAsset([FromQuery] DateTime? assetDate)
        {
            if (assetDate == null)
                return BadRequest("allocationDate is required.");

            string newCode = tsAssetManagementRepo.GenerateAllocationCodeAsset(assetDate);

            return Ok(new
            {
                status = 1,
                data = newCode
            });
        }
        [HttpPost("deleteAssetManagement")]
        public async Task<IActionResult> deleteAssetManagement([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return BadRequest("Danh sách ID không hợp lệ.");
            foreach (var ID in ids)
            {
                var item = tsAssetManagementRepo.GetByID(ID);
                if (item != null)
                {
                    item.IsDeleted = true;
                    tsAssetManagementRepo.UpdateAsync(item);
                }
                await tsAssetManagementRepo.UpdateAsync(item);
            }
            return Ok(new { message = "Đã xóa thành công." });
        }

    }
}
