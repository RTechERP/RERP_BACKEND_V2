using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using System.Data;
using System.Data.Entity.Migrations.Model;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.Project
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
            WarehouseRepo warehouseRepo
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
                        t.RequestTypeCode
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
                var warehouses = _warehouseRepo.GetAll();
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

        [HttpPost("get-all")]
        [RequiresPermission("N58,N35,N1")]
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
                "@POKHID", "@ProductRTCID", "@IsDeleted", "@IsTechBought", "@IsJobRequirement"

                    },
                    new object[] {
                filter.DateStart, filter.DateEnd, filter.StatusRequest, filter.ProjectID, filter.Keyword,
                filter.SupplierSaleID, filter.IsApprovedTBP, filter.IsApprovedBGD, filter.IsCommercialProduct,
                filter.POKHID, filter.ProductRTCID, filter.IsDeleted, filter.IsTechBought, filter.IsJobRequirement

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

                List<ProjectPartlistPurchaseRequestDTO> projectPartlistPurchaseRequests = new List<ProjectPartlistPurchaseRequestDTO>();
                foreach (ProjectPartlistPurchaseRequestDTO item in data)
                {
                    if (item.ID <= 0) continue;
                    var existingRequest = _repo.GetByID(item.ID);
                    if (existingRequest == null) continue;

                    if (existingRequest.EmployeeIDRequestApproved != currentUser.EmployeeID
                        && !currentUser.IsAdmin) continue;
                    projectPartlistPurchaseRequests.Add(item);
                }
                if (projectPartlistPurchaseRequests.Count() <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));

                foreach (ProjectPartlistPurchaseRequestDTO item in data)
                {
                    _repo.UpdateData(item);
                    item.IsRequestApproved = status;
                    item.EmployeeIDRequestApproved = currentUser.EmployeeID;

                    await _repo.UpdateAsync(item);
                }


                return Ok(ApiResponseFactory.Success(null, $"Đã cập nhật trạng thái {textStatus} thành công."));
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

                List<ProjectPartlistPurchaseRequestDTO> projectPartlistPurchaseRequests = new List<ProjectPartlistPurchaseRequestDTO>();
                foreach (ProjectPartlistPurchaseRequestDTO item in data)
                {
                    if (item.ID <= 0) continue;
                    var existingRequest = _repo.GetByID(item.ID);
                    if (existingRequest == null) continue;

                    if (existingRequest.EmployeeIDRequestApproved != currentUser.EmployeeID
                        && !currentUser.IsAdmin) continue;
                    projectPartlistPurchaseRequests.Add(item);
                }

                foreach (ProjectPartlistPurchaseRequestDTO item in data)
                {
                    _repo.UpdateData(item);
                    item.StatusRequest = status;
                    item.EmployeeIDRequestApproved = currentUser.EmployeeID;

                    await _repo.UpdateAsync(item);
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
                string textStatus = status ? "duyệt" : "hủy duyệt";
                if (data.Count() <= 0) return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));

                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                List<ProjectPartlistPurchaseRequestDTO> projectPartlistPurchaseRequests = new List<ProjectPartlistPurchaseRequestDTO>();
                foreach (ProjectPartlistPurchaseRequestDTO item in data)
                {
                    if (item.ID <= 0) continue;
                    if (item.ProductSaleID <= 0 && status && item.ProductNewCode == null)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, $"Vui lòng tạo Mã nội bộ cho sản phẩm [{item.ProductCode}].\nChọn Loại kho sau đó chọn Lưu thay đổi để tạo Mã nội bộ!"));
                    }
                    projectPartlistPurchaseRequests.Add(item);
                }

                foreach (ProjectPartlistPurchaseRequestDTO item in data)
                {
                    if (item.ProjectPartlistPurchaseRequestTypeID == 3 || item.ProjectPartlistPurchaseRequestTypeID == 7)
                    {
                        _repo.UpdateData(item);
                    }

                    if (type)
                    {
                        item.IsApprovedTBP = status;
                        item.DateApprovedTBP = DateTime.Now;
                    }
                    else
                    {
                        item.IsApprovedBGD = status;
                        item.DateApprovedBGD = DateTime.Now;
                        item.ApprovedBGD = currentUser.EmployeeID;
                    }

                    await _repo.UpdateAsync(item);
                }

                return Ok(ApiResponseFactory.Success(null, $"Đã cập nhật trạng thái {textStatus} thành công."));
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

                var firms = _firmRepo.GetAll(x => x.FirmType == 1 && x.IsDelete != true);

                foreach (var item in data)
                {
                    if ((item.EmployeeIDRequestApproved != currentUser.EmployeeID && !currentUser.IsAdmin)
                        || (item.ProductGroupID <= 0 && item.ProductGroupRTCID <= 0)
                    )
                    {
                        _repo.UpdateData(item);
                        await _repo.UpdateAsync(item);
                        continue;
                    }

                    int productSaleId = 0;

                    ProductSale productSale = _productSaleRepo.GetAll(x =>
                    x.ProductGroupID == item.ProductGroupID &&
                    x.ProductCode.ToLower() == item.ProductCode.ToLower() &&
                    x.IsDeleted != true
                    ).FirstOrDefault();

                    productSale = productSale ?? new ProductSale();
                    productSaleId = productSale.ID;
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
                        productSaleId = await _productSaleRepo.CreateAsync(productSale);
                    }

                    item.ProductSaleID = productSaleId;
                    item.ProductNewCode = productSale.ProductNewCode;



                    if (item.ProjectPartListID > 0)
                    {
                        var pokhDetails = _pOKHDetailRepo.GetAll(x => x.ProjectPartListID == item.ProjectPartListID);
                        if (pokhDetails.Count() > 0)
                        {
                            foreach (var detail in pokhDetails)
                            {
                                detail.ProductID = productSaleId;
                                await _pOKHDetailRepo.UpdateAsync(detail);
                            }
                        }

                    }

                    _repo.UpdateData(item);
                    await _repo.UpdateAsync(item);
                }

                return Ok(ApiResponseFactory.Success(null, $"Đã lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("deleted-request")]
        [RequiresPermission("N35,N1")]
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
                if (!_repo.ValidateSaveDataDetail(requestBought, out string message))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }

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
                foreach (var item in data) await _repo.UpdateAsync(item);
                return Ok(ApiResponseFactory.Success(null, ""));
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

            try
            {
                foreach (var item in data)
                {
                    if (item.ID <= 0) continue;
                    ProjectPartlistPurchaseRequest duplicate = item;
                    duplicate.ID = 0;
                    duplicate.Quantity = 0;
                    if (item.DuplicateID > 0)
                    {
                        duplicate.DuplicateID = item.DuplicateID;
                        duplicate.OriginQuantity = item.OriginQuantity;
                    }
                    else
                    {
                        duplicate.DuplicateID = item.ID;
                        duplicate.OriginQuantity = item.Quantity;
                    }
                    await _repo.CreateAsync(duplicate);
                    var newId = duplicate.ID;

                    if (item.DuplicateID <= 0)
                    {
                        item.DuplicateID = newId;
                        item.OriginQuantity = duplicate.OriginQuantity;
                        await _repo.UpdateAsync(item);
                    }
                }
                return Ok(ApiResponseFactory.Success(null, ""));
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
                    if (item.ID <= 0 || item.ProductSaleID <= 0
                        || (item.EmployeeIDRequestApproved != currentUser.EmployeeID && !currentUser.IsAdmin))
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


    }
}