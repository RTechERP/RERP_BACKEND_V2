using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.Asset;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.Asset;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Asset;

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
        private readonly TSAssetAllocationRepo _tSAssetAllocationRepo;
        private readonly TSAssetRecoveryRepo _tSAssetRecoveryRepo;
        private readonly TSAssetTransferRepo _tSAssetTransferRepo;
        private readonly AssetLogRepo _assetLogRepo;

        public AssetsController(TSLostReportAssetRepo tsLostReportRepo, TSAllocationEvictionAssetRepo tSAllocationEvictionRepo, TSReportBrokenAssetRepo tsReportBrokenAssetRepo, TSAssetManagementRepo tsAssetManagementRepo, TSRepairAssetRepo tSRepairAssetRepo, TSLiQuidationAssetRepo tsLiQuidationAssetRepo, ProductGroupRepo productgroupRepo, ProductSaleRepo productsaleRepo, UnitCountRepo unitCountRepo, TSAssetAllocationRepo tSAssetAllocationRepo, TSAssetRecoveryRepo tSAssetRecoveryRepo, TSAssetTransferRepo tSAssetTransferRepo, AssetLogRepo assetLogRepo)
        {
            _tsLostReportRepo = tsLostReportRepo;
            _tSAllocationEvictionRepo = tSAllocationEvictionRepo;
            _tsReportBrokenAssetRepo = tsReportBrokenAssetRepo;
            _tsAssetManagementRepo = tsAssetManagementRepo;
            _tSRepairAssetRepo = tSRepairAssetRepo;
            _tsLiQuidationAssetRepo = tsLiQuidationAssetRepo;
            _productgroupRepo = productgroupRepo;
            _productsaleRepo = productsaleRepo;
            _unitCountRepo = unitCountRepo;
            _tSAssetAllocationRepo = tSAssetAllocationRepo;
            _tSAssetRecoveryRepo = tSAssetRecoveryRepo;
            _tSAssetTransferRepo = tSAssetTransferRepo;
            _assetLogRepo = assetLogRepo;
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
        [HttpPost("get-asset-person")]
        public IActionResult GetPersonalPropertiesByPerson([FromBody] AssetmanagementRequestParam request)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
                request.DateStart = TextUtils.MinDate;
                request.DateEnd = TextUtils.MaxDate;
                var assets = SQLHelper<dynamic>.ProcedureToList("spGetTSAssetManagement",
                         new string[] { "@FilterText", "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@EmployeeID" },
                    new object[] { request.FilterText, request.PageNumber, request.PageSize, request.DateStart, request.DateEnd, currentUser.EmployeeID });

                return Ok(ApiResponseFactory.Success(assets));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-personal-properties")]
        public IActionResult GetPersonalProperties([FromQuery] DateTime dateStart,
            [FromQuery] DateTime dateEnd,
            [FromQuery] int receiverID = 0,
            [FromQuery] int assetCategory = 0)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
                var assets = SQLHelper<dynamic>.ProcedureToList("spGetPersonalProperty",
                       new string[] { "@DateStart", "@DateEnd", "@ReceiverID", "@AssetCategory" },
                    new object[] { dateStart, dateEnd, currentUser.EmployeeID, assetCategory });
                return Ok(ApiResponseFactory.Success(assets));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-personal-property-details")]
        public IActionResult GetPersonalPropertyDetails(int assetID, int assetCategory)
        {
            try
            {
                var data = SQLHelper<dynamic>.ProcedureToList(
                          "spGetPersonalPropertyDetail",
                          new string[] { "@AssetID", "@AssetCategory" },
                          new object[] { assetID, assetCategory });
                return Ok(ApiResponseFactory.Success(data));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
                var existingAssets = _tsAssetManagementRepo.GetExistingAssets(TSAssetCode, TSCodeNCC);

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

        [HttpGet("get-asset-log/{assetId}")]
        public IActionResult GetAssetLog(int assetId)
        {
            try
            {
                var assetLogs = _assetLogRepo.GetAll(x => x.AssetID == assetId).OrderByDescending(x => x.CreatedDate).ToList();
                return Ok(ApiResponseFactory.Success(assetLogs));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] AssetmanagementFullDTO asset)
        {
            try
            {
                int assetId;
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                if (asset == null)
                {
                    return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." });
                }
                if (asset.tSAssetManagements != null && asset.tSAssetManagements.Any())
                {
                    var productgroupModel = _productgroupRepo.GetSingleNoTracking(x => x.ProductGroupID == "C");
                    int groupID = productgroupModel?.ID ?? 0;

                    foreach (var item in asset.tSAssetManagements)
                    {
                        if (item.IsDeleted != true)
                        {
                            if (item.ID <= 0 && !string.IsNullOrWhiteSpace(item.TSCodeNCC))
                            {
                                var existingAsset = _tsAssetManagementRepo.GetSingleNoTracking(x => x.TSCodeNCC == item.TSCodeNCC && x.IsDeleted != true);
                                if (existingAsset != null && existingAsset.ID > 0)
                                {
                                    item.ID = existingAsset.ID;
                                }
                            }

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
                        {
                            var existingMaster = _tsAssetManagementRepo.GetSingleNoTracking(x => x.ID == item.ID);
                            await _tsAssetManagementRepo.UpdateAsync(item);
                            assetId = item.ID;

                            List<string> changeDetails = _assetLogRepo.GetEntityChanges(existingMaster, item);

                            bool isSpecialAction =
                                (asset.tSLostReportAsset != null && asset.tSLostReportAsset.ID <= 0) ||
                                (asset.tSReportBrokenAsset != null && asset.tSReportBrokenAsset.ID <= 0) ||
                                (asset.tSLiQuidationAsset != null && asset.tSLiQuidationAsset.ID <= 0) ||
                                (asset.tSAllocationEvictionAssets != null && asset.tSAllocationEvictionAssets.Any(a => a.ID <= 0)) ||
                                (asset.tSRepairAssets != null && asset.tSRepairAssets.Any(a => a.ID <= 0));

                            if (changeDetails.Any() && !isSpecialAction)
                            {
                                await _assetLogRepo.CreateAsync(new AssetLog
                                {
                                    AssetID = assetId,
                                    EmployeeID = currentUser.EmployeeID,
                                    TypeLog = "CẬP NHẬT TÀI SẢN",
                                    LogContent = $"Cập nhật tài sản: {string.Join(", ", changeDetails)}",
                                    CreatedBy = currentUser.LoginName,
                                    CreatedDate = DateTime.Now,
                                    DateLog = DateTime.Now
                                });
                            }
                        }

                        string productCode = item.Model ?? "";
                        var codeExist = _productsaleRepo.GetSingleNoTracking(x => x.ProductCode == productCode && x.ProductGroupID == groupID);

                        if (codeExist != null && codeExist.ID > 0)
                        {
                            codeExist.ProductName = item.TSAssetName;
                            codeExist.Unit = _unitCountRepo.GetByID(item.UnitID ?? 0)?.UnitName ?? "";
                            await _productsaleRepo.UpdateAsync(codeExist);
                        }
                        else
                        {
                            codeExist = new ProductSale();
                            codeExist.ProductNewCode = GenerateProductNewCode(groupID);
                            codeExist.SupplierName = "";
                            codeExist.ProductGroupID = groupID;
                            codeExist.ProductCode = item.Model;
                            codeExist.ProductName = item.TSAssetName;
                            codeExist.Unit = _unitCountRepo.GetByID(item.UnitID ?? 0)?.UnitName ?? "";
                            await _productsaleRepo.CreateAsync(codeExist);
                        }
                    }
                }
                if (asset.tSAllocationEvictionAssets != null && asset.tSAllocationEvictionAssets.Any())
                {
                    foreach (var item in asset.tSAllocationEvictionAssets)
                    {
                        bool isNew = item.ID <= 0;
                        if (isNew)
                            await _tSAllocationEvictionRepo.CreateAsync(item);
                        else
                            await _tSAllocationEvictionRepo.UpdateAsync(item);

                        if (isNew)
                        {
                            var master = _tsAssetManagementRepo.GetSingleNoTracking(x => x.ID == item.AssetManagementID);
                            string code = master?.TSCodeNCC ?? "Trống";
                            string action = item.Status == "Đang sử dụng" ? "Cấp phát" : "Thu hồi";

                            bool isSideEffectOfOtherReport =
                                (asset.tSLostReportAsset != null && asset.tSLostReportAsset.ID <= 0) ||
                                (asset.tSReportBrokenAsset != null && asset.tSReportBrokenAsset.ID <= 0) ||
                                (asset.tSLiQuidationAsset != null && asset.tSLiQuidationAsset.ID <= 0) ||
                                (asset.tSRepairAssets != null && asset.tSRepairAssets.Any(a => a.ID <= 0));

                            if (!isSideEffectOfOtherReport)
                            {
                                await _assetLogRepo.CreateAsync(new AssetLog
                                {
                                    AssetID = item.AssetManagementID ?? 0,
                                    EmployeeID = currentUser.EmployeeID,
                                    TypeLog = $"{action.ToUpper()} TÀI SẢN",
                                    LogContent = $"{action} tài sản. Mã: {code}. Ghi chú: {item.Note}",
                                    CreatedBy = currentUser.LoginName,
                                    CreatedDate = DateTime.Now,
                                    DateLog = DateTime.Now
                                });
                            }
                        }
                    }
                }
                if (asset.tSLostReportAsset != null)
                {
                    bool isNew = asset.tSLostReportAsset.ID <= 0;
                    if (isNew)
                        await _tsLostReportRepo.CreateAsync(asset.tSLostReportAsset);
                    else
                        await _tsLostReportRepo.UpdateAsync(asset.tSLostReportAsset);

                    if (isNew)
                    {
                        var master = _tsAssetManagementRepo.GetSingleNoTracking(x => x.ID == asset.tSLostReportAsset.AssetManagementID);
                        string code = master?.TSCodeNCC ?? "Trống";
                        await _assetLogRepo.CreateAsync(new AssetLog
                        {
                            AssetID = asset.tSLostReportAsset.AssetManagementID ?? 0,
                            EmployeeID = currentUser.EmployeeID,
                            TypeLog = "BÁO MẤT TÀI SẢN",
                            LogContent = $"Báo mất tài sản. Mã: {code}. Lý do: {asset.tSLostReportAsset.Reason}",
                            CreatedBy = currentUser.LoginName,
                            CreatedDate = DateTime.Now,
                            DateLog = DateTime.Now
                        });
                    }
                }
                if (asset.tSReportBrokenAsset != null)
                {
                    bool isNew = asset.tSReportBrokenAsset.ID <= 0;
                    if (isNew)
                        await _tsReportBrokenAssetRepo.CreateAsync(asset.tSReportBrokenAsset);
                    else
                        await _tsReportBrokenAssetRepo.UpdateAsync(asset.tSReportBrokenAsset);

                    if (isNew)
                    {
                        var master = _tsAssetManagementRepo.GetSingleNoTracking(x => x.ID == asset.tSReportBrokenAsset.AssetManagementID);
                        string code = master?.TSCodeNCC ?? "Trống";
                        await _assetLogRepo.CreateAsync(new AssetLog
                        {
                            AssetID = asset.tSReportBrokenAsset.AssetManagementID ?? 0,
                            EmployeeID = currentUser.EmployeeID,
                            TypeLog = "BÁO HỎNG TÀI SẢN",
                            LogContent = $"Báo hỏng tài sản. Mã: {code}. Lý do: {asset.tSReportBrokenAsset.Reason}",
                            CreatedBy = currentUser.LoginName,
                            CreatedDate = DateTime.Now,
                            DateLog = DateTime.Now
                        });
                    }
                }
                if (asset.tSLiQuidationAsset != null)
                {
                    bool isNew = asset.tSLiQuidationAsset.ID <= 0;
                    if (isNew)
                        await _tsLiQuidationAssetRepo.CreateAsync(asset.tSLiQuidationAsset);
                    else
                        await _tsLiQuidationAssetRepo.UpdateAsync(asset.tSLiQuidationAsset);

                    if (isNew)
                    {
                        var master = _tsAssetManagementRepo.GetSingleNoTracking(x => x.ID == asset.tSLiQuidationAsset.AssetManagementID);
                        string code = master?.TSCodeNCC ?? "Trống";
                        await _assetLogRepo.CreateAsync(new AssetLog
                        {
                            AssetID = asset.tSLiQuidationAsset.AssetManagementID ?? 0,
                            EmployeeID = currentUser.EmployeeID,
                            TypeLog = "ĐỀ NGHỊ THANH LÝ",
                            LogContent = $"Đề nghị thanh lý tài sản. Mã: {code}. Lý do: {asset.tSLiQuidationAsset.Reason}",
                            CreatedBy = currentUser.LoginName,
                            CreatedDate = DateTime.Now,
                            DateLog = DateTime.Now
                        });
                    }
                }
                if (asset.tSRepairAssets != null && asset.tSRepairAssets.Any())
                {
                    foreach (var item in asset.tSRepairAssets)
                    {
                        bool isNew = item.ID <= 0;
                        if (isNew)
                            await _tSRepairAssetRepo.CreateAsync(item);
                        else
                            await _tSRepairAssetRepo.UpdateAsync(item);

                        if (isNew)
                        {
                            var master = _tsAssetManagementRepo.GetSingleNoTracking(x => x.ID == item.AssetManagementID);
                            string code = master?.TSCodeNCC ?? "Trống";
                            await _assetLogRepo.CreateAsync(new AssetLog
                            {
                                AssetID = item.AssetManagementID ?? 0,
                                EmployeeID = currentUser.EmployeeID,
                                TypeLog = "SỬA CHỮA / BẢO DƯỠNG",
                                LogContent = $"Sửa chữa / bảo dưỡng tài sản. Mã: {code}. Đơn vị sửa chữa: {item.Name}. Chi phí dự kiến: {item.ExpectedCost}. Lý do: {item.Reason}",
                                CreatedBy = currentUser.LoginName,
                                CreatedDate = DateTime.Now,
                                DateLog = DateTime.Now
                            });
                        }
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

            // Bước 2: Lấy mã lớn nhất hiện tại
            var maxProductCode = _productsaleRepo.GetMaxProductNewCode(productGroupId, productGroupCode);

            // Bước 3: Tính STT tiếp theo
            int nextSTT = 1;
            if (!string.IsNullOrEmpty(maxProductCode) && maxProductCode.Length > productGroupCode.Length)
            {
                string numberPart = maxProductCode.Substring(productGroupCode.Length);
                if (int.TryParse(numberPart, out int num))
                {
                    nextSTT = num + 1;
                }
            }

            string numberCodeText = nextSTT.ToString().PadLeft(9 - productGroupCode.Length, '0');

            return productGroupCode + numberCodeText;
        }

        [HttpPost("change-status-asset")]
        public IActionResult ChangeStatusAsset([FromBody] PersonalPropertyDTO asset)
        {
            try
            {
                var repoDictionary = new Dictionary<int, dynamic>
                    {
                      { 0,_tSAssetTransferRepo },
                      { 1,_tSAssetAllocationRepo },
                      { 2,_tSAssetRecoveryRepo  }
                     };
                if (!repoDictionary.ContainsKey(asset.AssetCategory))
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));
                var repo = repoDictionary[asset.AssetCategory];
                var model = repo.GetByID(asset.AssetID);
                if (model == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Phiếu không tồn tại!"));
                }
                model.IsApprovedPersonalProperty = asset.IsApprove;
                model.DateApprovedPersonalProperty = DateTime.Now;
                repo.Update(model);

                return Ok(ApiResponseFactory.Success("Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}