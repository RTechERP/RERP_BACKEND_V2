using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.Util;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Enum;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using System.Data;

namespace RERPAPI.Controllers.Old
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectPartlistPurchaseRequestController : ControllerBase
    {
        #region Khai báo repository

        ProjectPartlistPurchaseRequestRepo _repo;
        InventoryProjectRepo _inventoryProjectRepo;
        ProjectPartlistPurchaseRequestTypeRepo _typeRepo;
        ProjectRepo _projectRepo;
        POKHRepo _pokhRepo;
        FirmRepo _firmRepo;
        ProductSaleRepo _productSaleRepo;
        RequestInvoiceRepo _requestInvoiceRepo;
        POKHDetailRepo _pOKHDetailRepo;
        WarehouseRepo _warehouseRepo;
        ProductGroupRTCRepo _productGroupRTCRepo;
        ProjectPartListPurchaseRequestApproveLogRepo _projectPartListPurchaseRequestApproveLogRepo;
        List<PathStaticFile> _pathStaticFiles;

        public ProjectPartlistPurchaseRequestController(
            ProjectPartlistPurchaseRequestRepo projectPartlistPurchaseRequestRepo,
            InventoryProjectRepo inventoryProjectRepo,
            ProjectRepo projectRepo,
            POKHRepo pOKHRepo,
            FirmRepo firmRepo,
            ProjectPartlistPurchaseRequestTypeRepo projectPartlistPurchaseRequestTypeRepo,
            ProductSaleRepo productSaleRepo,
            RequestInvoiceRepo requestInvoiceRepo,
            POKHDetailRepo pOKHDetailRepo,
            WarehouseRepo warehouseRepo,
            ProductGroupRTCRepo productGroupRTCRepo,
            IConfiguration configuration,
            ProjectPartListPurchaseRequestApproveLogRepo projectPartListPurchaseRequestApproveLogRepo
            )
        {
            _repo = projectPartlistPurchaseRequestRepo;
            _inventoryProjectRepo = inventoryProjectRepo;
            _typeRepo = projectPartlistPurchaseRequestTypeRepo;
            _projectRepo = projectRepo;
            _pokhRepo = pOKHRepo;
            _firmRepo = firmRepo;
            _productSaleRepo = productSaleRepo;
            _requestInvoiceRepo = requestInvoiceRepo;
            _pOKHDetailRepo = pOKHDetailRepo;
            _warehouseRepo = warehouseRepo;
            _productGroupRTCRepo = productGroupRTCRepo;
            _pathStaticFiles = configuration.GetSection("PathStaticFiles").Get<List<PathStaticFile>>() ?? new List<PathStaticFile>();
            _projectPartListPurchaseRequestApproveLogRepo = projectPartListPurchaseRequestApproveLogRepo;
        }

        #endregion Khai báo repository

        [HttpGet("get-all-project")]
        public IActionResult GetAllProject()
        {
            try
            {
                var lstProjects = _projectRepo.GetAll();
                return Ok(ApiResponseFactory.Success(lstProjects, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

        [HttpGet("request-types")]
        public IActionResult GetRequestTypes()
        {
            try
            {
                var types = _typeRepo.GetAll()
                    .Select(t => new
                    {
                        t.ID,
                        t.RequestTypeName,
                        t.RequestTypeCode,
                        t.IsIgnoreBGD
                    })
                    .OrderBy(t => t.ID)
                    .ToList();

                return Ok(ApiResponseFactory.Success(types, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-po-code")]
        public IActionResult GetPOCode()
        {
            try
            {
                var lstPos = _pokhRepo.GetAll();
                return Ok(ApiResponseFactory.Success(lstPos, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("warehouse")]
        public IActionResult GetWarehouse()
        {
            try
            {
                var warehouses = _warehouseRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(warehouses, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-product-sale")]
        public IActionResult getProductPartlist(int productSaleId)
        {
            try
            {
                decimal historyPrice = 0;
                var productSale = _productSaleRepo.GetByID(productSaleId);
                if (!string.IsNullOrWhiteSpace(productSale.ProductCode))
                {
                    var dt = SQLHelper<dynamic>.ProcedureToList("spGetHistoryPricePartlist",
                        new string[] { "@ProductSaleID" },
                        new object[] { productSaleId });

                    var data = SQLHelper<dynamic>.GetListData(dt, 0);

                    var dataHistoryPrice = data.Where(x => x.ProductCode == productSale.ProductCode).FirstOrDefault();

                    decimal lastHistoryPrice = dataHistoryPrice != null ? Convert.ToDecimal(dataHistoryPrice.UnitPrice) : 0;
                    historyPrice = lastHistoryPrice;
                }

                var result = new
                {
                    ProductName = productSale.ProductName,
                    Maker = productSale.Maker,
                    Unit = productSale.Unit,
                    ProductGroupID = productSale.ProductGroupID,
                    HistoryPrice = historyPrice
                };

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("product-group_rtc")]
        public IActionResult getProductGrouprtc(int productSaleId)
        {
            try
            {
                var data = _productGroupRTCRepo.GetAll(x => x.WarehouseID == 1
                && x.ProductGroupNo.Trim().ToLower() != "dbh"
                && x.ProductGroupNo.Trim().ToLower() != "dbh");

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("history-product-partlist")]
        public IActionResult historyProductPartlist(string? keyword)
        {
            try
            {
                var dt = SQLHelper<dynamic>.ProcedureToList("spGetHistoryPricePartlist"
                    , new string[] { "@Keyword" }, new object[] { keyword });

                var data = SQLHelper<dynamic>.GetListData(dt, 0);

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-by-id")]
        [RequiresPermission("N58,N35,N1")]
        public IActionResult GetByID(int id)
        {
            try
            {
                ProjectPartlistPurchaseRequest purchaseRequest = _repo.GetByID(id);
                return Ok(ApiResponseFactory.Success(purchaseRequest, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }
        [HttpPost("get-all-demo")]
        //[RequiresPermission("N58,N35,N1")]
        public IActionResult GetAllDemo([FromBody] ProjectPartlistPurchaseRequestParam filter)
        {
            try
            {
                DateTime dateStart = filter.DateStart.Date;
                DateTime dateEnd = filter.DateEnd.Date
                                    .AddHours(23)
                                    .AddMinutes(59)
                                    .AddSeconds(59);
                var dt = SQLHelper<dynamic>.ProcedureToList("spGetProjectPartlistPurchaseRequest_New_Khanh",
                    new string[] {
                "@DateStart", "@DateEnd", "@StatusRequest", "@ProjectID", "@Keyword",
                "@SupplierSaleID", "@IsApprovedTBP", "@IsApprovedBGD", "@IsCommercialProduct",
                "@POKHID", "@ProductRTCID", "@IsDeleted", "@IsTechBought", "@IsJobRequirement","@EmployeeID"

                    },
                    new object[] {
                filter.DateStart, filter.DateEnd, filter.StatusRequest, filter.ProjectID, filter.Keyword,
                filter.SupplierSaleID, filter.IsApprovedTBP, filter.IsApprovedBGD, filter.IsCommercialProduct,
                filter.POKHID, filter.ProductRTCID, filter.IsDeleted, filter.IsTechBought, filter.IsJobRequirement,
                filter.EmployeeID
                    });

                var data = SQLHelper<dynamic>.GetListData(dt, 0);

                return Ok(ApiResponseFactory.Success(data, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("get-all")]
        //[RequiresPermission("N58,N35,N1")]
        public IActionResult GetAll([FromBody] ProjectPartlistPurchaseRequestParam filter)
        {
            try
            {
                DateTime dateStart = filter.DateStart.Date;
                DateTime dateEnd = filter.DateEnd.Date
                                    .AddHours(23)
                                    .AddMinutes(59)
                                    .AddSeconds(59);
                var dt = SQLHelper<dynamic>.ProcedureToList("spGetProjectPartlistPurchaseRequest_New_Khanh",
                    new string[] {
                "@DateStart", "@DateEnd", "@StatusRequest", "@ProjectID", "@Keyword",
                "@SupplierSaleID", "@IsApprovedTBP", "@IsApprovedBGD", "@IsCommercialProduct",
                "@POKHID", "@ProductRTCID", "@IsDeleted", "@IsTechBought", "@IsJobRequirement", "@IsRequestApproved"

                    },
                    new object[] {
                filter.DateStart, filter.DateEnd, filter.StatusRequest, filter.ProjectID, filter.Keyword,
                filter.SupplierSaleID, filter.IsApprovedTBP, filter.IsApprovedBGD, filter.IsCommercialProduct,
                filter.POKHID, filter.ProductRTCID, filter.IsDeleted, filter.IsTechBought, filter.IsJobRequirement, filter.IsRequestApproved

                    });

                var data = SQLHelper<dynamic>.GetListData(dt, 0);

                return Ok(ApiResponseFactory.Success(data, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("check-order")]
        [RequiresPermission("N35,N1")]
        public async Task<IActionResult> CheckOrder([FromBody] List<int> listIds, bool status)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                if (listIds == null || listIds.Count == 0) return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));
                string statusText = status ? "check" : "hủy check";
                PurchaseRequestApproveStatus logStatus = status
                                                        ? PurchaseRequestApproveStatus.CheckOrder
                                                        : PurchaseRequestApproveStatus.CancelCheckOrder;

                List<ProjectPartlistPurchaseRequest> projectPartlistPurchaseRequests = new List<ProjectPartlistPurchaseRequest>();
                foreach (var id in listIds)
                {
                    try
                    {
                        if (id <= 0) continue;

                        var existingRequest = _repo.GetByID(id);
                        if (existingRequest == null) continue;

                        if (existingRequest.EmployeeIDRequestApproved > 0
                            && existingRequest.EmployeeIDRequestApproved != currentUser.EmployeeID
                            && !currentUser.IsAdmin) continue;
                        projectPartlistPurchaseRequests.Add(existingRequest);
                    }
                    catch
                    {
                        continue;
                    }
                }

                if (projectPartlistPurchaseRequests.Count() <= 0) return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));
                int employeeId = status ? currentUser.EmployeeID : 0;

                foreach (var item in projectPartlistPurchaseRequests)
                {
                    item.EmployeeIDRequestApproved = employeeId;
                    await _repo.UpdateAsync(item);
                    await _projectPartListPurchaseRequestApproveLogRepo.CreateLogAsync(item.ID, logStatus, currentUser.EmployeeID, currentUser.LoginName);
                }

                return Ok(ApiResponseFactory.Success(null, $"Đã xử lý xong danh sách {statusText}"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("request-approved")]
        [RequiresPermission("N35,N1")]
        public async Task<IActionResult> RequestApproved([FromBody] List<ProjectPartlistPurchaseRequestDTO> data, bool status)
        {
            try
            {
                string textStatus = status ? "Y/C duyệt" : "hủy Y/C duyệt";
                if (!_repo.ValidateRequestApproved(data, out string message))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                PurchaseRequestApproveStatus logStatus = status
                                                           ? PurchaseRequestApproveStatus.RequestApprove
                                                           : PurchaseRequestApproveStatus.CancelRequestApprove;
                foreach (ProjectPartlistPurchaseRequestDTO item in data)
                {
                    if (item.ID <= 0) continue;
                    var existingRequest = _repo.GetByID(item.ID);
                    if (existingRequest == null) continue;

                    if (existingRequest.EmployeeIDRequestApproved != currentUser.EmployeeID
                        && !currentUser.IsAdmin) continue;

                    _repo.UpdateData(item);
                    item.IsRequestApproved = status;
                    item.EmployeeIDRequestApproved = currentUser.EmployeeID;
                    await _repo.UpdateAsync(item);
                    await _projectPartListPurchaseRequestApproveLogRepo.CreateLogAsync(item.ID, logStatus, currentUser.EmployeeID, currentUser.LoginName);
                }
                return Ok(ApiResponseFactory.Success(data, $"Đã cập nhật trạng thái {textStatus} thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("complete-request-buy")]
        [RequiresPermission("N35,N1")]
        public async Task<IActionResult> CompleteRequest([FromBody] List<ProjectPartlistPurchaseRequestDTO> data, int status)
        {
            try
            {
                string textStatus = status == 7 ? "hoàn thành" : "hủy hoàn thành";
                if (!_repo.ValidateRequestApproved(data, out string message))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                PurchaseRequestApproveStatus logStatus = status == 7
                                                            ? PurchaseRequestApproveStatus.Completed
                                                            : PurchaseRequestApproveStatus.CancelCompleted;
                foreach (ProjectPartlistPurchaseRequestDTO item in data)
                {
                    if (item.ID <= 0) continue;
                    var existingRequest = _repo.GetByID(item.ID);
                    if (existingRequest == null) continue;

                    if (existingRequest.EmployeeIDRequestApproved != currentUser.EmployeeID
                        && !currentUser.IsAdmin) continue;

                    _repo.UpdateData(item);
                    item.StatusRequest = status;
                    item.EmployeeIDRequestApproved = currentUser.EmployeeID;

                    await _repo.UpdateAsync(item);
                    await _projectPartListPurchaseRequestApproveLogRepo.CreateLogAsync(item.ID, logStatus, currentUser.EmployeeID, currentUser.LoginName);

                }


                return Ok(ApiResponseFactory.Success(null, $"Đã cập nhật trạng thái {textStatus} thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("approved")]
        [RequiresPermission("N58,N1")]
        public async Task<IActionResult> Approved([FromBody] List<ProjectPartlistPurchaseRequestDTO> data, bool status, bool type)
        {
            try
            {
                if (data == null || !data.Any())
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));

                string textStatus = status ? "duyệt" : "hủy duyệt";

                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                foreach (var item in data)
                {
                    if (item.ID <= 0) continue;

                    if (status && item.ProductSaleID <= 0 && string.IsNullOrEmpty(item.ProductNewCode))
                    {
                        return BadRequest(ApiResponseFactory.Fail(
                            null,
                            $"Vui lòng tạo Mã nội bộ cho sản phẩm [{item.ProductCode}].\n" +
                            $"Chọn Loại kho sau đó chọn Lưu thay đổi để tạo Mã nội bộ!"
                        ));
                    }

                    var existingRequest = _repo.GetByID(item.ID);
                    if (existingRequest == null) continue;

                    PurchaseRequestApproveStatus logStatus;

                    if (type) // TBP
                    {
                        existingRequest.IsApprovedTBP = status;
                        existingRequest.DateApprovedTBP = DateTime.Now;

                        logStatus = status
                                    ? PurchaseRequestApproveStatus.TBPApprove
                                    : PurchaseRequestApproveStatus.TBPCancelApprove;
                    }
                    else // BGĐ
                    {
                        existingRequest.IsApprovedBGD = status;
                        existingRequest.DateApprovedBGD = DateTime.Now;
                        existingRequest.ApprovedBGD = currentUser.EmployeeID;

                        logStatus = status
                                    ? PurchaseRequestApproveStatus.BGDApprove
                                    : PurchaseRequestApproveStatus.BGDCancelApprove;
                    }
                    await _projectPartListPurchaseRequestApproveLogRepo.CreateLogAsync(item.ID, logStatus, currentUser.EmployeeID, currentUser.LoginName);

                    await _repo.UpdateAsync(existingRequest);
                }

                return Ok(ApiResponseFactory.Success(data, $"Đã cập nhật trạng thái {data.Count} yêu cầu {textStatus} thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data-rtc")]
        public async Task<IActionResult> SaveDataRTC(ProjectPartlistPurchaseRequest model)
        {
            try
            {
                if (!_repo.Validate(model, out string message))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }
                if (model.IsTechBought == true)
                {
                    var requestBoughts = _repo.GetAll(x => x.ProjectPartListID == model.ProjectPartListID);
                    if (requestBoughts.Count > 0)
                    {
                        foreach (var item in requestBoughts)
                        {
                            item.IsDeleted = model.IsTechBought;
                            await _repo.UpdateAsync(item);
                        }
                    }
                    //model.IsTechBought = requestBought.IsTechBought;
                    //model.ProjectPartListID = requestBought.ProjectPartListID;
                    //model.Note = txtNote.Text;
                    //model.ProductCode = requestBought.ProductCode;
                }
                model.StatusRequest = 1;
                model.ProjectPartlistPurchaseRequestTypeID = model.TicketType == 1 ? 4 : 3;
                if (model.ID > 0) await _repo.UpdateAsync(model);
                else await _repo.CreateAsync(model);
                return Ok(ApiResponseFactory.Success(model));


            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data")]
        [RequiresPermission("N35,N1")]
        public async Task<IActionResult> SaveData([FromBody] List<ProjectPartlistPurchaseRequestDTO> data)
        {
            try
            {
                if (data.Count() <= 0) return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));

                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                if (!_repo.validateManufacturer(data, out string message))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }

                if (!_repo.ValidateUpdateData(data, out string mes))
                    return BadRequest(ApiResponseFactory.Fail(null, mes));
                //foreach (var item in data)
                //{
                //// ===== HANDLE DUPLICATE =====
                //if (item.ID <= 0 && item.DuplicateID > 0)
                //{
                //    var source = _repo.GetByID(item.DuplicateID ?? 0);
                //    if (source.ID <= 0) continue;
                //    item.EmployeeID = source.EmployeeID;
                //    item.ProjectPartListID = source.ProjectPartListID;
                //    item.UnitCountID = source.UnitCountID;
                //    item.StatusRequest = source.StatusRequest;
                //    item.SupplierSaleID = source.SupplierSaleID;

                //    item.IsApprovedTBP = source.IsApprovedTBP;
                //    item.ApprovedTBP = source.ApprovedTBP;
                //    item.IsApprovedBGD = source.IsApprovedBGD;
                //    item.ApprovedBGD = source.ApprovedBGD;

                //    item.ProductSaleID = source.ProductSaleID;
                //    item.ProductGroupID = source.ProductGroupID;
                //    item.CurrencyID = source.CurrencyID;

                //    item.IsImport = source.IsImport;
                //    item.IsRequestApproved = source.IsRequestApproved;
                //    item.POKHDetailID = source.POKHDetailID;
                //    item.JobRequirementID = source.JobRequirementID;

                //    item.IsDeleted = source.IsDeleted;
                //    item.InventoryProjectID = source.InventoryProjectID;
                //    item.IsTechBought = source.IsTechBought;

                //    item.ProductGroupRTCID = source.ProductGroupRTCID;
                //    item.ProductRTCID = source.ProductRTCID;
                //    item.TicketType = source.TicketType;

                //    item.DateReturnEstimated = source.DateReturnEstimated;

                //    item.EmployeeApproveID = source.EmployeeApproveID;
                //    item.EmployeeIDRequestApproved = source.EmployeeIDRequestApproved;

                //    item.UnitName = source.UnitName;

                //}
                //// ===== END HANDLE DUPLICATE =====
                //}

                var firms = _firmRepo.GetAll(x => x.FirmType == 1 && x.IsDelete != true);
                foreach (var item in data)
                {
                    ProjectPartlistPurchaseRequest prjPartList = _repo.GetByID(item.ID);

                    if ((prjPartList.ID > 0 && prjPartList.EmployeeIDRequestApproved != currentUser.EmployeeID && !currentUser.IsAdmin)
                    )
                    {
                        continue;
                    }

                    if ((item.ProductGroupID <= 0 && item.ProductGroupRTCID <= 0))
                    {
                        _repo.UpdateData(item);
                        if (item.ID <= 0) await _repo.CreateAsync(item);
                        else await _repo.UpdateAsync(item);
                        continue;
                    }


                    ProductSale productSale = _productSaleRepo.GetAll(x =>
                    x.ProductGroupID == item.ProductGroupID &&
                    x.ProductCode.ToLower() == item.ProductCode.ToLower() &&
                    x.IsDeleted != true
                    ).FirstOrDefault() ?? new ProductSale();
                    //productSale = productSale ?? new ProductSale();
                    if (productSale.ID <= 0)
                    {
                        productSale.ProductCode = item.ProductCode;
                        productSale.ProductName = item.ProductName;
                        productSale.Unit = item.UnitName;
                        productSale.ProductGroupID = item.ProductGroupID;
                        productSale.ProductNewCode = _repo.GenerateProductNewCode(Convert.ToInt32(item.ProductGroupID));
                        string maker = item.Manufacturer;

                        Firm firm = _firmRepo.GetAll(x =>
                                    x.FirmName.Trim().ToLower() == maker.Trim().ToLower())
                                    .FirstOrDefault() ?? new Firm();

                        productSale.Maker = maker;
                        productSale.FirmID = firm.ID;
                        await _productSaleRepo.CreateAsync(productSale);
                    }

                    item.ProductSaleID = productSale.ID;
                    //item.ProductNewCode = productSale.ProductNewCode;

                    if (item.ProjectPartListID > 0)
                    {
                        var pokhDetails = _pOKHDetailRepo.GetAll(x => x.ProjectPartListID == item.ProjectPartListID);
                        if (pokhDetails.Count() > 0)
                        {
                            foreach (var detail in pokhDetails)
                            {
                                detail.ProductID = productSale.ID;
                                await _pOKHDetailRepo.UpdateAsync(detail);
                            }
                        }

                    }

                    _repo.UpdateData(item);
                    if (item.ID <= 0) await _repo.CreateAsync(item);
                    else await _repo.UpdateAsync(item);

                    await _projectPartListPurchaseRequestApproveLogRepo.CreateLogAsync(item.ID, PurchaseRequestApproveStatus.SaveData, currentUser.EmployeeID, currentUser.LoginName);

                }

                return Ok(ApiResponseFactory.Success(null, $"Đã lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("deleted-request")]
        //[RequiresPermission("N35,N1")]
        public async Task<IActionResult> DeletedRequest([FromBody] List<ProjectPartlistPurchaseRequestDTO> data, bool isPurchaseRequestDemo)
        {
            try
            {
                if (data == null || data.Count == 0)
                {
                    return BadRequest(new { status = 0, message = "Dữ liệu không hợp lệ" });
                }
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                if (!currentUser.IsAdmin)
                {
                    if (!_repo.validateDeleted(data, isPurchaseRequestDemo, out string message))
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, message));
                    }
                }

                List<int> inventoryProjects = new List<int>();
                foreach (var item in data)
                {
                    if (item.ID <= 0) continue;
                    var existingRequest = _repo.GetByID(item.ID);
                    if (existingRequest == null) continue;

                    if (existingRequest.EmployeeIDRequestApproved != currentUser.EmployeeID
                        && !currentUser.IsAdmin) continue;

                    if (item.ID <= 0) continue;
                    item.IsDeleted = true;
                    await _repo.UpdateAsync(item);


                    int inventoryProjectID = Convert.ToInt32(item.InventoryProjectID);
                    if (inventoryProjectID > 0 && !inventoryProjects.Contains(inventoryProjectID))
                        inventoryProjects.Add(inventoryProjectID);
                }

                if (inventoryProjects.Count > 0)
                {
                    foreach (int item in inventoryProjects)
                    {
                        var inventoryProject = _inventoryProjectRepo.GetByID(item);
                        inventoryProject.IsDeleted = true;
                        await _inventoryProjectRepo.UpdateAsync(inventoryProject);
                    }
                }

                return Ok(new { status = 1, message = "Đã xử lý xong danh sách xoá." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 0, message = ex.Message });
            }
        }

        [HttpPost("save-data-detail")]
        [RequiresPermission("N35,N1")]
        public async Task<IActionResult> SaveDataDetail([FromBody] ProjectPartlistPurchaseRequestDTO requestBought)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                if (!_repo.ValidateSaveDataDetail(requestBought, out string message))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }

                if (requestBought.ID <= 0) return BadRequest(ApiResponseFactory.Fail(null, "Lỗi dữ liệu không có ID"));
                var existingRequest = _repo.GetByID(requestBought.ID);
                if (existingRequest == null) return BadRequest(ApiResponseFactory.Fail(null, "Lỗi dữ liệu không tìm thấy")); ;

                if (existingRequest.EmployeeIDRequestApproved != currentUser.EmployeeID
                    && !currentUser.IsAdmin) return BadRequest(ApiResponseFactory.Fail(null, "Bạn không có quyền sửa của nhân viên khác!")); ;

                if ((bool)requestBought.IsTechBought)
                {
                    var requestBoughts = _repo.GetAll(x => x.ProjectPartListID == requestBought.ProjectPartListID);
                    if (requestBoughts.Count > 0)
                    {
                        foreach (var item in requestBoughts)
                        {
                            item.IsDeleted = true;
                            await _repo.UpdateAsync(item);
                        }
                    }
                }

                if (requestBought.ID <= 0)
                {
                    await _repo.CreateAsync(requestBought);
                    await _repo.SendMail(requestBought);
                }
                else
                {
                    await _repo.UpdateAsync(requestBought);
                }

                return Ok(ApiResponseFactory.Success(null, $"Đã lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("update-product-import")]
        [RequiresPermission("N35,N1")]
        public async Task<IActionResult> UpdateProductImport([FromBody] List<ProjectPartlistPurchaseRequestDTO> data)
        {
            try
            {
                if (data.Count() <= 0) return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));
                string import = data[0].IsImport == true
                    ? "cập nhật hàng nhập khẩu"
                    : "hủy hàng nhập khẩu";
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                foreach (var item in data)
                {
                    if (item.ID <= 0) continue;
                    var existingRequest = _repo.GetByID(item.ID);
                    if (existingRequest == null) continue;

                    if (existingRequest.EmployeeIDRequestApproved != currentUser.EmployeeID
                        && !currentUser.IsAdmin) continue;
                    await _repo.UpdateAsync(item);
                }
                return Ok(ApiResponseFactory.Success(null, $"Đã {import} "));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("duplicate")]
        [RequiresPermission("N35,N1")]
        public async Task<IActionResult> Duplicate(List<ProjectPartlistPurchaseRequestDTO> data)
        {
            if (data == null || data.Count() <= 0)
                return BadRequest(new { status = 0, message = "Dữ liệu không hợp lệ." });

            var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
            var currentUser = ObjectMapper.GetCurrentUser(claims);
            try
            {
                foreach (var item in data)
                {
                    if (item.ID <= 0) continue;
                    var existingRequest = _repo.GetByID(item.ID);
                    if (existingRequest == null) continue;

                    if (existingRequest.EmployeeIDRequestApproved != currentUser.EmployeeID
                        && !currentUser.IsAdmin) continue;

                    ProjectPartlistPurchaseRequest duplicate = existingRequest.Copy();
                    duplicate.ID = 0;
                    duplicate.Quantity = 0;
                    if (existingRequest.DuplicateID > 0)
                    {
                        duplicate.DuplicateID = existingRequest.DuplicateID;
                        duplicate.OriginQuantity = existingRequest.OriginQuantity;
                    }
                    else
                    {
                        duplicate.DuplicateID = existingRequest.ID;
                        duplicate.OriginQuantity = existingRequest.Quantity;
                    }
                    await _repo.CreateAsync(duplicate);
                    var newId = duplicate.DuplicateID;

                    if (existingRequest.DuplicateID <= 0)
                    {
                        existingRequest.DuplicateID = newId;
                        existingRequest.OriginQuantity = duplicate.OriginQuantity;
                        await _repo.UpdateAsync(existingRequest);
                    }
                }
                return Ok(ApiResponseFactory.Success(null, "Đã tạo bản ghi mới cho sản phẩm được chọn!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("keep-product")]
        [RequiresPermission("N35,N1")]
        public async Task<IActionResult> KeepProduct([FromBody] List<ProjectPartlistPurchaseRequestDTO> data)
        {
            try
            {
                if (data == null || data.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách sản phẩm không hợp lệ!"));
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                foreach (var item in data)
                {
                    ProjectPartlistPurchaseRequest prjPartList = _repo.GetByID(item.ID);
                    if (item.ID <= 0 || item.ProductSaleID <= 0
                        || (prjPartList.EmployeeIDRequestApproved != currentUser.EmployeeID && !currentUser.IsAdmin))
                        continue;

                    var dt = SQLHelper<dynamic>.ProcedureToList("spGetInventory", new[] { "@ProductSaleID" }, new object[] { item.ProductSaleID });
                    var inventoryData = SQLHelper<dynamic>.GetListData(dt, 0);
                    var quantity = inventoryData[0]?.TotalQuantityLast;
                    if (quantity == null || Convert.ToDecimal(quantity) <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, $"Số lượng giữ sản phẩm [{item.ProductName}] cần lớn hơn 0!"));
                    }

                    var inventoryProject = item.ID > 0
                        ? _inventoryProjectRepo.GetByID(item.ID)
                        : new InventoryProject();

                    inventoryProject.ProjectID = item.ProjectID;
                    inventoryProject.ProductSaleID = item.ProductSaleID;
                    inventoryProject.EmployeeID = item.EmployeeIDRequestApproved;
                    inventoryProject.WarehouseID = 1;
                    inventoryProject.Quantity = item.Quantity;
                    inventoryProject.CustomerID = item.CustomerID;
                    inventoryProject.POKHDetailID = item.POKHDetailID;

                    if (inventoryProject.ID > 0)
                        await _inventoryProjectRepo.UpdateAsync(inventoryProject);
                    else
                        await _inventoryProjectRepo.CreateAsync(inventoryProject);

                    item.InventoryProjectID = inventoryProject.ID;
                    await _repo.UpdateAsync(item);
                }

                return Ok(ApiResponseFactory.Success(null, "Đã cập nhật giữ hàng thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

        [HttpPost("download-file-list")]
        [RequiresPermission("N35,N1")]
        public IActionResult DownloadFileList([FromBody] List<ProjectPartlistPurchaseRequestDTO> data)
        {
            try
            {
                var existingFiles = new List<(string FilePath, string FileName)>();

                foreach (var item in data)
                {
                    // Lấy project
                    var project = _projectRepo.GetByID((int)item.ProjectID);
                    if (project == null || !project.CreatedDate.HasValue)
                        continue;

                    // Lấy solution
                    var solution = SQLHelper<object>
                        .ProcedureToList("spGetProjectSolutionByProjectPartListID",
                                         new[] { "@ProjectPartListID" },
                                         new object[] { item.ProjectPartListID });
                    if (solution.Count() > 0) continue;
                    var dt = SQLHelper<object>.GetListData(solution, 0)[0];
                    if (dt == null || string.IsNullOrEmpty(dt.CodeSolution))
                        continue;

                    // Build path
                    string pathPattern =
                        $"{project.CreatedDate.Value.Year}/{project.ProjectCode.Trim()}/" +
                        $"THIETKE.Co/{dt.CodeSolution.Trim()}/2D/GC/DH";

                    var pathStaticFile = _pathStaticFiles
                        .FirstOrDefault(p => p.PathName.ToLower() == "software");

                    string fullPath = pathStaticFile != null
                        ? Path.Combine(pathStaticFile.PathFull, pathPattern)
                        : pathPattern;

                    string filePath = Path.Combine(fullPath, $"{item.ProductCode}.pdf");

                    if (System.IO.File.Exists(filePath))
                    {
                        existingFiles.Add((filePath, $"{item.ProductCode}.pdf"));
                    }
                }

                if (existingFiles.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có file nào tồn tại để tải xuống."));
                }

                using (var ms = new MemoryStream())
                {
                    using (var zip = new System.IO.Compression.ZipArchive(ms,
                        System.IO.Compression.ZipArchiveMode.Create, true))
                    {
                        foreach (var file in existingFiles)
                        {
                            var entry = zip.CreateEntry(file.FileName,
                                System.IO.Compression.CompressionLevel.Fastest);

                            using (var entryStream = entry.Open())
                            using (var fs = System.IO.File.OpenRead(file.FilePath))
                            {
                                fs.CopyTo(entryStream);
                            }
                        }
                    }

                    ms.Position = 0;

                    return File(ms.ToArray(), "application/zip", "DownloadFiles.zip");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("validate-add-poncc")]
        [RequiresPermission("N35,N1")]
        public async Task<IActionResult> ValidateAddPoncc([FromBody] List<ProjectPartlistPurchaseRequestDTO> data)
        {
            try
            {
                if (!_repo.ValidateAddPoncc(data, out string message))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }

                return Ok(ApiResponseFactory.Success(null, $""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("create-product")]
        public async Task<IActionResult> CreateProduct(List<ProjectPartlistPurchaseRequestDTO> data)
        {
            if (data == null || data.Count <= 0) return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu truyền về để cập nhật!"));
            try
            {
                await _repo.CreateProduct(data);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}