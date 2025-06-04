using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;


namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetTranferController : ControllerBase
    {
        TSAssetTransferRepo tSAssetTransferRepo = new TSAssetTransferRepo();
        TSAllocationEvictionAssetRepo tSAllocationEvictionRepo = new TSAllocationEvictionAssetRepo();
        TSAssetManagementRepo tsAssetManagementRepo = new TSAssetManagementRepo();
        TSAssetTransferDetailRepo tSAssetTransferDetailRepo = new TSAssetTransferDetailRepo();

        [HttpPost("getAssetTransfer")]
        public IActionResult GetListAssetTransfer([FromBody] AssetTransferRequestParam request)
        {
            try
            {
                var assetTranfer = SQLHelper<dynamic>.ProcedureToList("spGetTranferAssetMaster",
                   new string[] { "@DateStart", "@DateEnd", "@IsApproved", "@DeliverID", "@ReceiverID", "TextFilter", "@PageSize", "@PageNumber" },
                                            new object[] { request.DateStart, request.DateEnd, request.IsApproved, request.DeliverID, request.ReceiverID, request.TextFilter, request.PageSize, request.PageNumber });
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        assets = SQLHelper<dynamic>.GetListData(assetTranfer, 0)
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
        [HttpGet("getAssetTranferDetail")]
        public IActionResult GetAssetTranferDetail(string? id)
        {
            try
            {
                var assetTranferDetail = SQLHelper<dynamic>.ProcedureToList(
            "spGetTranferAssetDetail",
            new string[] { "@TranferAssetID" },
            new object[] { id });
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        assetTransferDetail = SQLHelper<dynamic>.GetListData(assetTranferDetail, 0)
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
        [HttpGet("generateTransferCode")]
        public async Task<IActionResult> GenerateTransferCode([FromQuery] DateTime? transferDate)
        {
            if (transferDate == null)
                return BadRequest("allocationDate is required.");

            string newCode = tSAssetTransferRepo.GenCodeRecovery(transferDate);
            return Ok(new
            {
                status = 1,
                data = newCode
            });
        }
        [HttpPost("saveData")]
        public async Task<IActionResult> SaveData([FromBody] AssetTranferFullDTO assetTransfer)
        {
            try
            {
                if (assetTransfer == null)
                {
                    return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." });
                }
                if (assetTransfer.tSAssetManagements != null && assetTransfer.tSAssetManagements.Any())
                {
                    foreach (var item in assetTransfer.tSAssetManagements)
                    {
                        if (item.ID <= 0)
                            await tsAssetManagementRepo.CreateAsync(item);
                        else
                            tsAssetManagementRepo.UpdateFieldsByID(item.ID, item);
                    }
                }
                if (assetTransfer.tSAllocationEvictionAssets != null && assetTransfer.tSAllocationEvictionAssets.Any())
                {
                    foreach (var item in assetTransfer.tSAllocationEvictionAssets)
                    {
                        if (item.ID <= 0)
                            await tSAllocationEvictionRepo.CreateAsync(item);
                        else
                            tSAllocationEvictionRepo.UpdateFieldsByID(item.ID, item);
                    }
                }
                if (assetTransfer.tSTranferAssetDetails != null && assetTransfer.tSTranferAssetDetails.Any())
                {
                    foreach (var item in assetTransfer.tSTranferAssetDetails)
                    {
                        if (item.ID <= 0)
                            await tSAssetTransferDetailRepo.CreateAsync(item);
                        else
                            tSAssetTransferDetailRepo.UpdateFieldsByID(item.ID, item);
                    }
                }
                if (assetTransfer.tSTranferAsset != null)
                {
                    if (assetTransfer.tSTranferAsset.ID <= 0)
                        await tSAssetTransferRepo.CreateAsync(assetTransfer.tSTranferAsset);
                    else
                        tSAssetTransferRepo.UpdateFieldsByID(assetTransfer.tSTranferAsset.ID, assetTransfer.tSTranferAsset);
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
