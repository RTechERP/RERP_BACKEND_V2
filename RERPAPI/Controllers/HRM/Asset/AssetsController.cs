using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO.Asset;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.Asset;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Asset;
using RERPAPI.Repo.GenericEntity.HRM.Vehicle;
using System;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace RERPAPI.Controllers.Old.Asset
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        private readonly TSLostReportAssetRepo _tsLostReportRepo;
        private readonly TSAllocationEvictionAssetRepo _tSAllocationEvictionRepo;
        private readonly TSReportBrokenAssetRepo _tsReportBrokenAssetRepo;
        private readonly TSAssetManagementRepo _tsAssetManagementRepo;
        private readonly TSRepairAssetRepo _tSRepairAssetRepo;
        private readonly TSLiQuidationAssetRepo _tsLiQuidationAssetRepo;
        private readonly ProductGroupRepo _productgroupRepo;
        private readonly ProductSaleRepo _productsaleRepo;
        private readonly UnitCountRepo _unitCountRepo;

        public AssetsController(
            TSLostReportAssetRepo tsLostReportRepo,
            TSAllocationEvictionAssetRepo tSAllocationEvictionRepo,
            TSReportBrokenAssetRepo tsReportBrokenAssetRepo,
            TSAssetManagementRepo tsAssetManagementRepo,
            TSRepairAssetRepo tSRepairAssetRepo,
            TSLiQuidationAssetRepo tsLiQuidationAssetRepo,
          ProductSaleRepo productSaleRepo,
            ProductGroupRepo productgroupRepo,
            UnitCountRepo unitCountRepo
        )
        {
            _tsLostReportRepo = tsLostReportRepo;
            _tSAllocationEvictionRepo = tSAllocationEvictionRepo;
            _tsReportBrokenAssetRepo = tsReportBrokenAssetRepo;
            _tsAssetManagementRepo = tsAssetManagementRepo;
            _tSRepairAssetRepo = tSRepairAssetRepo;
            _tsLiQuidationAssetRepo = tsLiQuidationAssetRepo;
            _productgroupRepo = productgroupRepo;
            _productsaleRepo = productSaleRepo;
            _unitCountRepo = unitCountRepo;
        }
        [RequiresPermission("N2,N23,N1,N67")]
        [HttpPost("get-asset")]
        public IActionResult GetListAssets([FromBody] AssetmanagementRequestParam request)
        {
            try
            {
                var assets = SQLHelper<dynamic>.ProcedureToList("spLoadTSAssetManagement",
                    new string[] { "@FilterText", "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@Status", "@Department" },
                    new object[] { request.FilterText, request.PageNumber, request.PageSize, request.DateStart, request.DateEnd, request.Status, request.Department });
                int maxSTT = _tsAssetManagementRepo.GetMaxSTT();
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        assets = SQLHelper<dynamic>.GetListData(assets, 0),
                        total = SQLHelper<dynamic>.GetListData(assets, 1),
                        maxSTT
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

        [HttpGet("get-repair")]
        public IActionResult GetLatestRepairByAssetManagementID([FromQuery] int assetManagementID)
        {
            try
            {
                var result = _tSRepairAssetRepo.GetLatestRepairByAssetManagementID(assetManagementID);

                if (result == null)
                {
                    return Ok(new
                    {
                        status = 0,
                        message = "Không tìm thấy thông tin sửa chữa cho tài sản này"
                    });
                }
                return Ok(new
                {
                    status = 1,
                    data = result
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
        public IActionResult GenerateAssetCode([FromQuery] DateTime? assetDate)
        {

            if (assetDate == null)
                return BadRequest("AssetDate is required.");

            var newcode = _tsAssetManagementRepo.GenerateAssetCode(assetDate);
            int maxSTT = _tsAssetManagementRepo.GetMaxSTT();
            return Ok(new
            {
                status = 1,
                data = newcode,
                maxSTT
            });
        }

        //check-productsale trong excel
        [HttpPost("check-asset-exist")]
        public IActionResult CheckAssetExist([FromBody] List<TSAssetManagement> asset)
        {
            try
            {
                var TSAssetCode = asset.Select(x => x.TSAssetCode).ToList();
                var TSCodeNCC = asset.Select(x => x.TSCodeNCC).ToList();
                var existingAssets = _tsAssetManagementRepo.GetAll()
                    .Where(x => TSAssetCode.Contains(x.TSAssetCode) && TSCodeNCC.Contains(x.TSCodeNCC))
                    .Select(x => new
                    {
                        x.ID,
                        x.TSAssetCode,
                        x.TSCodeNCC

                    })
                    .ToList();

                return Ok(new
                {
                    data = new
                    {
                        existingAssets
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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
                        if (item.IsDeleted != true)
                        {
                            if (!_tsAssetManagementRepo.Validate(item, out string message))
                            {
                                return BadRequest(ApiResponseFactory.Fail(null, message));
                            }
                        }

                        if (item.ID <= 0)
                        {

                            item.StatusID = 1;
                            item.Status = "Chưa sử dụng";

                            await _tsAssetManagementRepo.CreateAsync(item);
                        }

                        else
                            await _tsAssetManagementRepo.UpdateAsync(item);
                        var productgroupModel = _productgroupRepo.GetAll(x => x.ProductGroupID == "C").FirstOrDefault();
                        int groupID = 0;
                        if (productgroupModel != null)
                        {
                            groupID = productgroupModel.ID;
                        }
                        string productCode = item.Model ?? "";
                        var codeExist = _productsaleRepo.GetAll(x => x.ProductCode == productCode&&x.ProductGroupID==groupID).FirstOrDefault();
                        codeExist.ProductNewCode = GenerateProductNewCode(groupID);
                        codeExist = codeExist == null ? new ProductSale() : codeExist;
                        codeExist.SupplierName = "";
                        codeExist.ProductGroupID = groupID;
                        codeExist.ProductCode = item.Model;
                        codeExist.ProductName = item.TSAssetName;
                        codeExist.Unit = _unitCountRepo.GetByID(item.UnitID??0).UnitName;
                        if(codeExist.ID>0)
                        {
                            await _productsaleRepo.UpdateAsync(codeExist);
                        }    
                        else
                        {
                            await _productsaleRepo.CreateAsync(codeExist);
                        }    
                    }
                }
                if (asset.tSAllocationEvictionAssets != null && asset.tSAllocationEvictionAssets.Any())
                {
                    foreach (var item in asset.tSAllocationEvictionAssets)
                    {
                        if (item.ID <= 0)
                            await _tSAllocationEvictionRepo.CreateAsync(item);
                        else
                            await _tSAllocationEvictionRepo.UpdateAsync(item);
                    }
                }
                if (asset.tSLostReportAsset != null)
                {
                    if (asset.tSLostReportAsset.ID <= 0)
                        await _tsLostReportRepo.CreateAsync(asset.tSLostReportAsset);
                    else
                        await _tsLostReportRepo.UpdateAsync(asset.tSLostReportAsset);
                }
                if (asset.tSReportBrokenAsset != null)
                {
                    if (asset.tSReportBrokenAsset.ID <= 0)
                        await _tsReportBrokenAssetRepo.CreateAsync(asset.tSReportBrokenAsset);
                    else
                        await _tsReportBrokenAssetRepo.UpdateAsync(asset.tSReportBrokenAsset);
                }
                if (asset.tSLiQuidationAsset != null)
                {
                    if (asset.tSLiQuidationAsset.ID <= 0)
                        await _tsLiQuidationAssetRepo.CreateAsync(asset.tSLiQuidationAsset);
                    else
                        await _tsLiQuidationAssetRepo.UpdateAsync(asset.tSLiQuidationAsset);
                }
                if (asset.tSRepairAssets != null && asset.tSRepairAssets.Any())
                {
                    foreach (var item in asset.tSRepairAssets)
                    {
                        if (item.ID <= 0)
                            await _tSRepairAssetRepo.CreateAsync(item);
                        else
                            await _tSRepairAssetRepo.UpdateAsync(item);
                    }
                }
                return Ok(ApiResponseFactory.Success(asset, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));

            }
        }
        private string GenerateProductNewCode(int productGroupId)
        {
            // Bước 1: Lấy mã nhóm sản phẩm từ ID
            var productGroup = _productgroupRepo.GetByID(productGroupId);
            if (productGroup == null || string.IsNullOrWhiteSpace(productGroup.ProductGroupID))
                return string.Empty;

            string productGroupCode = productGroup.ProductGroupID.Trim();

            // Bước 2: Lấy danh sách sản phẩm thuộc nhóm này
            var listProducts = _productsaleRepo.GetAll()
                .Where(x => x.ProductGroupID == productGroupId &&
                            !string.IsNullOrWhiteSpace(x.ProductNewCode) &&
                            x.ProductNewCode.StartsWith(productGroupCode))
                .ToList();

            // Bước 3: Tính STT cao nhất đang dùng
            var listNewCodes = listProducts.Select(x => new
            {
                STT = int.TryParse(x.ProductNewCode.Substring(productGroupCode.Length), out int num) ? num : 0
            });

            int nextSTT = listNewCodes.Any() ? listNewCodes.Max(x => x.STT) + 1 : 1;
            string numberCodeText = nextSTT.ToString().PadLeft(9 - productGroupCode.Length, '0');

            return productGroupCode + numberCodeText;
        }
    }
}
