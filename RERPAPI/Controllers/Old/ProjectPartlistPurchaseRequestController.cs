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

        private ProjectPartlistPurchaseRequestRepo _repo;
        private InventoryProjectRepo _inventoryProjectRepo;
        private ProjectPartlistPurchaseRequestTypeRepo _typeRepo;
        private ProjectRepo _projectRepo;
        private POKHRepo _pokhRepo;
        private FirmRepo _firmRepo;
        private ProductSaleRepo _productSaleRepo;
        private RequestInvoiceRepo _requestInvoiceRepo;
        private POKHDetailRepo _pOKHDetailRepo;
        private WarehouseRepo _warehouseRepo;
        private ProductGroupRTCRepo _productGroupRTCRepo;
        private ProjectPartListPurchaseRequestApproveLogRepo _projectPartListPurchaseRequestApproveLogRepo;
        private List<PathStaticFile> _pathStaticFiles;
        private ProductGroupRepo _productGroupRepo;
        private ProductRTCRepo _productRTCRepo;
        private UnitCountKTRepo _unitCountKTRepo;
        private ProjectPartlistPurchaseRequestNoteRepo _projectPartlistPurchaseRequestNoteRepo;
        private ProjectPartlistPriceRequestRepo _projectPartlistPriceRequestRepo;

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
            ProjectPartListPurchaseRequestApproveLogRepo projectPartListPurchaseRequestApproveLogRepo,
            ProductGroupRepo productGroupRepo,
            ProductRTCRepo productRTCRepo,
            UnitCountKTRepo unitCountKTRepo,
            ProjectPartlistPurchaseRequestNoteRepo projectPartlistPurchaseRequestNoteRepo,
            ProjectPartlistPriceRequestRepo projectPartlistPriceRequestRepo
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
            _productGroupRepo = productGroupRepo;
            _productRTCRepo = productRTCRepo;
            _unitCountKTRepo = unitCountKTRepo;
            _projectPartlistPurchaseRequestNoteRepo = projectPartlistPurchaseRequestNoteRepo;
            _projectPartlistPriceRequestRepo = projectPartlistPriceRequestRepo;
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

        [HttpGet("product-group-rtc")]
        public IActionResult getProductGrouprtc(int warehouseType)
        {
            try
            {
                var data = _productGroupRTCRepo.GetAll(x => x.WarehouseID == 1
                && x.ProductGroupNo.Trim().ToLower() != "dbh"
                && x.ProductGroupNo.Trim().ToLower() != "dbh" && x.WarehouseType == warehouseType);

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
                //var dt = SQLHelper<dynamic>.ProcedureToList("spGetProjectPartlistPurchaseRequest_New_Nhat",
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
        [RequiresPermission("N58,N1,N32")]
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

        [HttpPost("save-data-rtc-excel")]
        public async Task<IActionResult> SaveDataRTCExcel(List<ImportExcelRtcDto> models)
        {
            try
            {
                if (!_repo.ValidateExcel(models, out string message))
                    return BadRequest(ApiResponseFactory.Fail(null, message));

                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                // =========================
                // LOAD DATA 1 LẦN
                // =========================

                var allUnits = _unitCountKTRepo.GetAll();
                var allFirms = _firmRepo.GetAll();
                var allProducts = _productRTCRepo.GetAll();

                // =========================
                // DICTIONARY CACHE
                // =========================

                var unitDict = allUnits
                    .GroupBy(x => (x.UnitCountName ?? "").Trim().ToUpper())
                    .ToDictionary(x => x.Key, x => x.First());

                var firmDict = allFirms
                    .GroupBy(x => (x.FirmName ?? "").Trim().ToUpper())
                    .ToDictionary(x => x.Key, x => x.First());

                // =========================
                // BATCH INSERT LIST
                // =========================

                List<UnitCountKT> newUnits = new();
                List<ProductRTC> newProducts = new();

                // =========================
                // PHASE 1: UNIT + PRODUCT
                // =========================

                foreach (var model in models)
                {
                    var unitName = (model.UnitName ?? "").Trim();
                    var unitKey = unitName.ToUpper();

                    // UNIT
                    if (!unitDict.TryGetValue(unitKey, out var unit))
                    {
                        unit = new UnitCountKT
                        {
                            UnitCountName = unitName,
                            UnitCountCode = unitName,
                            IsDeleted = false
                        };

                        newUnits.Add(unit);
                        unitDict[unitKey] = unit;
                    }

                    // FIRM
                    var maker = (model.Maker ?? "").Trim();
                    var makerKey = maker.ToUpper();

                    firmDict.TryGetValue(makerKey, out var firm);

                    // PRODUCT
                    var product = allProducts.FirstOrDefault(x =>
                        (x.ProductCode ?? "").Trim().ToUpper() == (model.ProductCode ?? "").Trim().ToUpper()
                        && (x.ProductName ?? "").Trim().ToUpper() == (model.ProductName ?? "").Trim().ToUpper()
                        && x.ProductGroupRTCID == model.ProductGroupRTCID
                        && x.UnitCountID == unit.ID
                        &&
                        (
                            // ưu tiên FirmID
                            (firm != null && x.FirmID == firm.ID)

                            ||

                            // fallback Maker
                            (
                                firm == null
                                && (x.Maker ?? "").Trim().ToUpper() == makerKey
                            )
                        )
                    );

                    if (product == null)
                    {
                        product = new ProductRTC
                        {
                            ProductCode = model.ProductCode,
                            ProductName = model.ProductName,
                            Maker = model.Maker,
                            UnitCountID = unit.ID,
                            ProductGroupRTCID = model.ProductGroupRTCID,
                            ProductCodeRTC = _productRTCRepo.generateProductCode(model.ProductGroupRTCID),
                            FirmID = firm?.ID
                        };

                        newProducts.Add(product);
                        allProducts.Add(product);
                    }
                }

                // =========================
                // SAVE UNIT
                // =========================

                if (newUnits.Any())
                {
                    await _unitCountKTRepo.CreateRangeAsync(newUnits);
                }

                // reload units
                allUnits = _unitCountKTRepo.GetAll();

                unitDict = allUnits
                    .GroupBy(x => (x.UnitCountName ?? "").Trim().ToUpper())
                    .ToDictionary(x => x.Key, x => x.First());

                // =========================
                // UPDATE UNIT ID CHO PRODUCT
                // =========================

                foreach (var product in newProducts)
                {
                    var unit = unitDict[
                        (allUnits.First(x => x.ID == product.UnitCountID).UnitCountName ?? "")
                        .Trim()
                        .ToUpper()
                    ];

                    product.UnitCountID = unit.ID;
                }

                // =========================
                // SAVE PRODUCT
                // =========================

                if (newProducts.Any())
                {
                    await _productRTCRepo.CreateRangeAsync(newProducts);
                }

                // reload products
                allProducts = _productRTCRepo.GetAll();

                // =========================
                // CREATE PURCHASE REQUEST
                // =========================

                List<ProjectPartlistPurchaseRequest> requests = new();

                foreach (var model in models)
                {
                    var unitKey = (model.UnitName ?? "").Trim().ToUpper();

                    var unit = unitDict[unitKey];

                    var makerKey = (model.Maker ?? "").Trim().ToUpper();

                    firmDict.TryGetValue(makerKey, out var firm);

                    var product = allProducts.FirstOrDefault(x =>
                        (x.ProductCode ?? "").Trim().ToUpper() == (model.ProductCode ?? "").Trim().ToUpper()
                        && (x.ProductName ?? "").Trim().ToUpper() == (model.ProductName ?? "").Trim().ToUpper()
                        && x.ProductGroupRTCID == model.ProductGroupRTCID
                        && x.UnitCountID == unit.ID
                        &&
                        (
                            (firm != null && x.FirmID == firm.ID)

                            ||

                            (
                                firm == null
                                && (x.Maker ?? "").Trim().ToUpper() == makerKey
                            )
                        )
                    );

                    requests.Add(new ProjectPartlistPurchaseRequest
                    {
                        ProductCode = model.ProductCode,
                        ProductName = model.ProductName,
                        Quantity = model.Quantity,
                        UnitName = model.UnitName,
                        Maker = model.Maker,
                        //Note = model.Note,
                        ProductGroupRTCID = model.ProductGroupRTCID,
                        SupplierSaleID = model.SupplierSaleID,
                        DateReturnExpected = model.DateReturnExpected,
                        TicketType = model.TicketType,
                        IsTechBought = false,
                        ProjectPartListID = model.ProjectPartListID,
                        ProductRTCID = product?.ID ?? 0,
                        WarehouseID = model.WarehouseID,
                        StatusRequest = 1,
                        ProjectPartlistPurchaseRequestTypeID = model.TicketType == 1 ? 4 : 3,
                        EmployeeApproveID = 87,//Phạm Văn Quyền
                        ApprovedTBP = 87,//Phạm Văn Quyền
                        EmployeeID = currentUser.EmployeeID,
                        DateRequest = DateTime.Now,
                    });
                }

                // =========================
                // SAVE REQUEST
                // =========================

                await _repo.CreateRangeAsync(requests);
                // =========================
                // SAVE NOTE
                // =========================

                List<ProjectPartlistPurchaseRequestNote> requestNotes = new();

                foreach (var request in requests)
                {
                    // nếu không có note thì bỏ qua
                    if (string.IsNullOrWhiteSpace(request.Note))
                        continue;

                    requestNotes.Add(new ProjectPartlistPurchaseRequestNote
                    {
                        ProjectPartlistPurchaseRequestID = request.ID,
                        Note = request.Note,
                    });
                }

                if (requestNotes.Any())
                {
                    await _projectPartlistPurchaseRequestNoteRepo
                        .CreateRangeAsync(requestNotes);
                }

                return Ok(ApiResponseFactory.Success(
                    requests,
                    $"Lưu thành công {requests.Count} dòng."
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //[HttpPost("save-data-rtc-excel")]
        //public async Task<IActionResult> SaveDataRTCExcel(List<ImportExcelRtcDto> models)
        //{
        //	try
        //	{
        //		List<int> lstID = new List<int>();
        //		if (!_repo.ValidateExcel(models, out string message))
        //			return BadRequest(ApiResponseFactory.Fail(null, message));
        //		var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
        //		CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
        //		foreach (var model in models)
        //		{
        //			var unitcount = _unitCountKTRepo.GetSingleNoTracking(x => (x.UnitCountName ?? "").Trim().ToUpper() == model.UnitName.Trim().ToUpper());
        //			if (unitcount.ID <= 0)
        //			{
        //				unitcount.UnitCountCode = unitcount.UnitCountName;
        //				unitcount.IsDeleted = false;
        //				await _unitCountKTRepo.CreateAsync(unitcount);
        //			}
        //			var maker = (model.Maker ?? "").Trim();
        //			var makerUpper = maker.ToUpper();

        //			var firm = _firmRepo.GetSingleNoTracking(x => (x.FirmName ?? "").Trim().ToUpper() == makerUpper);
        //			int? firmID = firm?.ID;
        //			var productrtc = _productRTCRepo.GetSingleNoTracking(x => (x.ProductCode ?? "").Trim().ToUpper() == model.ProductCode.Trim().ToUpper() && (x.ProductName ?? "").Trim().ToUpper() == model.ProductName.Trim().ToUpper() && x.ProductGroupRTCID == model.ProductGroupRTCID && x.Maker == model.Maker && x.UnitCountID == unitcount.ID &&
        //			 (
        //				// Ưu tiên FirmID nếu tìm được Firm
        //				(firmID.HasValue && x.FirmID == firmID)

        //				||

        //				// Không có Firm thì fallback Maker
        //				(!firmID.HasValue &&
        //					(x.Maker ?? "").Trim().ToUpper() == makerUpper)
        //			));
        //			if (productrtc.ID <= 0)
        //			{
        //				productrtc.ProductCode = model.ProductCode;
        //				productrtc.ProductName = model.ProductName;
        //				productrtc.Maker = model.Maker;
        //				productrtc.UnitCountID = unitcount.ID;
        //				productrtc.ProductGroupRTCID = model.ProductGroupRTCID;
        //				productrtc.ProductCodeRTC = _productRTCRepo.generateProductCode(model.ProductGroupRTCID);
        //				await _productRTCRepo.CreateAsync(productrtc);
        //			}
        //			ProjectPartlistPurchaseRequest purchaseRequest = new ProjectPartlistPurchaseRequest
        //			{
        //				ProductCode = model.ProductCode,
        //				ProductName = model.ProductName,
        //				Quantity = model.Quantity,
        //				UnitName = model.UnitName,
        //				Maker = model.Maker,
        //				Note = model.Note,
        //				ProductGroupRTCID = model.ProductGroupRTCID,
        //				SupplierSaleID = model.SupplierSaleID,
        //				DateReturnExpected = model.DateReturnExpected,
        //				TicketType = model.TicketType,
        //				IsTechBought = model.IsTechBought,
        //				ProjectPartListID = model.ProjectPartListID,
        //				ProductRTCID = productrtc.ID,
        //				WarehouseID = model.WarehouseID,
        //				StatusRequest = 1,
        //				ProjectPartlistPurchaseRequestTypeID = model.TicketType == 1 ? 4 : 3,
        //				EmployeeApproveID = 87, //Phạm văn Quyền,
        //				EmployeeID = currentUser.EmployeeID
        //			};
        //			await _repo.CreateAsync(purchaseRequest);
        //			lstID.Add(purchaseRequest.ID);
        //		}
        //		return Ok(ApiResponseFactory.Success(lstID, $"Lưu thành công {lstID.Count} dòng."));
        //	}
        //	catch (Exception ex)
        //	{
        //		return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //	}
        //}

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
                        else
                        {
                            item.UpdatedDate = DateTime.Now;
                            await _repo.UpdateAsync(item);
                        }
                        continue;
                    }
                    if (item.ProjectPartlistPurchaseRequestTypeID != 3 && item.ProjectPartlistPurchaseRequestTypeID != 4)
                    {
                        List<int> productGroupIDs = _productGroupRepo.GetAll(
                        x => x.ID == item.ProductGroupID || x.ParentID == item.ProductGroupID)
                        .Select(x => x.ID).ToList();
                        var purchaseReq = _repo.GetSingleNoTracking(x => x.ID == item.ID);
                        if (purchaseReq.ProductSaleID <= 0)
                        {
                            var productSales = _productSaleRepo.GetAll(x =>
                            x.ProductCode.ToLower() == item.ProductCode.ToLower() &&
                            x.IsDeleted != true
                        ).ToList();

                            ProductSale productSale = productSales
                                .FirstOrDefault(x => productGroupIDs.Contains((int)x.ProductGroupID))
                                ?? new ProductSale();

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
                        }
                    }
                    else
                    {
                        await _repo.CreateProduct(data);
                    }
                    //item.ProductNewCode = productSale.ProductNewCode;

                    if (item.ProjectPartListID > 0)
                    {
                        var pokhDetails = _pOKHDetailRepo.GetAll(x => x.ProjectPartListID == item.ProjectPartListID);
                        if (pokhDetails.Count() > 0)
                        {
                            foreach (var detail in pokhDetails)
                            {
                                detail.ProductID = item.ProductSaleID;
                                await _pOKHDetailRepo.UpdateAsync(detail);
                            }
                        }
                    }

                    _repo.UpdateData(item);
                    if (item.ID <= 0) await _repo.CreateAsync(item);
                    else
                    {
                        item.UpdatedDate = DateTime.Now;
                        await _repo.UpdateAsync(item);
                    }

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
        public async Task<IActionResult> DeletedRequest([FromBody] List<ProjectPartlistPurchaseRequestDTO> data, bool isPurchaseRequestDemo)
        {
            try
            {
                if (data == null || data.Count == 0)
                {
                    return BadRequest(new { status = 0, message = "Dữ liệu không hợp lệ" });
                }
                List<ProjectPartlistPurchaseRequestDTO> successData = new List<ProjectPartlistPurchaseRequestDTO>();
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

                    if (existingRequest.EmployeeID != currentUser.EmployeeID
                        && !currentUser.IsAdmin) continue;

                    if (item.ID <= 0) continue;
                    item.IsDeleted = true;
                    await _repo.UpdateAsync(item);
                    successData.Add(item);

                    int inventoryProjectID = Convert.ToInt32(item.InventoryProjectID);
                    if (inventoryProjectID > 0 && !inventoryProjects.Contains(inventoryProjectID))
                        inventoryProjects.Add(inventoryProjectID);

                    if (item.ProjectPartlistPurchaseRequestTypeID == 8)
                    {
                        var purchaseRequest = _repo.GetByID(item.ID);
                        if (purchaseRequest.ProjectPartlistPriceRequestID > 0)
                        {
                            var priceRequest = _projectPartlistPriceRequestRepo.GetByID((int)purchaseRequest.ProjectPartlistPriceRequestID);
                            if (priceRequest != null)
                            {
                                priceRequest.IsDeleted = true;
                                await _projectPartlistPriceRequestRepo.UpdateAsync(priceRequest);
                            }
                        }
                    }
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

                return Ok(ApiResponseFactory.Success(data, "Xóa yêu cầu mua hàng thành công!"));
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

                if (existingRequest.EmployeeID != currentUser.EmployeeID
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

                    //var dt = SQLHelper<dynamic>.ProcedureToList("spGetInventory", new[] { "@ProductSaleID" }, new object[] { item.ProductSaleID });
                    var dt = SQLHelper<dynamic>.ProcedureToList("spGetInventory_Test", new[] { "@ProductSaleID" }, new object[] { item.ProductSaleID });
                    var inventoryData = SQLHelper<dynamic>.GetListData(dt, 0);
                    if (inventoryData.Count < 0) return BadRequest(ApiResponseFactory.Fail(null, $"Sản phẩm [{item.ProductName}] không có trong tồn kho!"));
                    var quantity = inventoryData[0]?.TotalQuantityLast;
                    if (quantity == null || Convert.ToDecimal(quantity) <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, $"Số lượng tồn trong kho sản phẩm [{item.ProductName}] không đủ để giữ!"));
                    }
                    if (item.Quantity > Convert.ToDecimal(quantity))
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, $"Số lượng giữ hàng [{item.Quantity}] vượt quá số lượng tồn kho [{quantity}] của sản phẩm [{item.ProductName}]!"));
                    }
                    var inventoryProject = item.InventoryProjectID > 0
                        ? _inventoryProjectRepo.GetByID(item.InventoryProjectID ?? 0)
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

                return Ok(ApiResponseFactory.Success(data, "Đã cập nhật giữ hàng thành công."));
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

        #region Lấy dữ liệu tổng hợp mua hàng dự án

        [HttpGet("purchase-quote-summary")]
        [RequiresPermission("N35,N1,N33")]
        public async Task<IActionResult> getPurchaseQuoteSummary(
            DateTime? DateStart,
            DateTime? DateEnd,
            int? DepartmentID = -1,
            int? EmployeeRequestID = -1,
            string? Keyword = ""
            )
        {
            try
            {
                var param = new
                {
                    DateStart = DateStart,
                    DateEnd = DateEnd,
                    DepartmentID = DepartmentID,
                    EmployeeRequestID = EmployeeRequestID,
                    Keyword = Keyword,
                };

                var data = await SqlDapper<object>.ProcedureToListAsync("spGetPurchaseQuoteSummary", param);

                return Ok(ApiResponseFactory.Success(data, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("purchase-quote-summary-new")]
        [RequiresPermission("N35,N1,N33")]
        public async Task<IActionResult> getPurchaseQuoteSummaryNew(
            DateTime? DateStart,
            DateTime? DateEnd,
            int? DepartmentID = -1,
            int? EmployeeRequestID = -1,
            string? Keyword = ""
            )
        {
            try
            {
                var param = new
                {
                    DateStart = DateStart,
                    DateEnd = DateEnd,
                    DepartmentID = DepartmentID,
                    EmployeeRequestID = EmployeeRequestID,
                    Keyword = Keyword,
                };

                var data = await SqlDapper<object>.ProcedureToListAsync("spGetPurchaseQuoteSummary_New", param);

                return Ok(ApiResponseFactory.Success(data, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion Lấy dữ liệu tổng hợp mua hàng dự án
    }
}