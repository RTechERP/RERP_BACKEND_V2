using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using System.Data;
using System.Linq.Expressions;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyAuthorize]
    public class ProjectPartlistPurchaseRequestController : ControllerBase
    {
        #region Khai báo repository

        private ProjectPartlistPurchaseRequestRepo _repo;
        private InventoryProjectRepo _inventoryProjectRepo;
        public ProjectPartlistPurchaseRequestController(ProjectPartlistPurchaseRequestRepo repo, InventoryProjectRepo inventoryProjectRepo)
        {
            _repo = repo;
            _inventoryProjectRepo = inventoryProjectRepo;
        }
        //private InventoryRepo _invRepo = new InventoryRepo();

        #endregion Khai báo repository

        [HttpPost("get-all")]
        public IActionResult GetAll([FromBody] ProjectPartlistPurchaseRequestParam filter)
        {
            try
            {
                var dt = SQLHelper<dynamic>.ProcedureToList("spGetProjectPartlistPurchaseRequest_New_Khanh",
                    new string[] {
                "@DateStart", "@DateEnd", "@StatusRequest", "@ProjectID", "@Keyword",
                "@SupplierSaleID", "@IsApprovedTBP", "@IsApprovedBGD", "@IsCommercialProduct",
                "@POKHID", "@ProductRTCID", "@IsDeleted", "@IsTechBought", "@IsJobRequirement","@PageSize","@PageNumber"
                    },
                    new object[] {
                filter.DateStart, filter.DateEnd, filter.StatusRequest, filter.ProjectID, filter.Keyword,
                filter.SupplierSaleID, filter.IsApprovedTBP , filter.IsApprovedBGD, filter.IsCommercialProduct,
                filter.POKHID, filter.ProductRTCID, filter.IsDeleted, filter.IsTechBought, filter.IsJobRequirement, filter.Size, filter.Page
                    });

                var allData = SQLHelper<dynamic>.GetListData(dt, 0);
                int totalPage = SQLHelper<dynamic>.GetListData(dt, 1)[0].TotalPage;

                var purchaseRequests = allData.Where(x => x.ProductRTCID == null || x.ProductRTCID <= 0).ToList();// Lọc PurchaseRequest

                var dataRTC = allData.Where(x => x.ProductRTCID > 0 && x.TicketType == 0).ToList();// Lọc ProductRTC
                var techBought = allData.Where(x => x.IsTechBought == true).ToList();// Lọc kt đã mua

                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var productRTCBorrow = allData
                    .Where(x => x.TicketType == 1 && (filter.IsApprovedTBP == 0 || x.ApprovedTBP == currentUser.EmployeeID)).ToList();//lọc ProductRTC mượn

                var productCommercial = allData.Where(p => p.IsCommercialProduct == true).ToList();
                var productHr = allData.Where(p => p.JobRequirementID > 0).ToList();
                return Ok(ApiResponseFactory.Success(new
                {
                    purchaseRequests,
                    dataRTC,
                    techBought,
                    productRTCBorrow,
                    productCommercial,
                    productHr,
                    totalPage

                }, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Lỗi khi lấy dữ liệu: " + ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] List<ProjectPartlistPurchaseRequest> data)
        {
            try
            {
                string message = string.Empty;
                if (!_repo.Validate(data, out message))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }
                foreach (var item in data)
                {
                    if (item.ID <= 0)
                    {
                        await _repo.CreateAsync(item);
                    }
                    else
                    {
                        if (item.IsRequestApproved == true)
                        {
                            var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                            var currentUser = ObjectMapper.GetCurrentUser(claims);
                            item.EmployeeIDRequestApproved = currentUser.EmployeeID;
                        }
                        else if (item.IsRequestApproved == false)
                        {
                            item.EmployeeIDRequestApproved = null;
                        }
                        if (item.IsApprovedBGD == true)
                        {
                            var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                            var currentUser = ObjectMapper.GetCurrentUser(claims);
                            item.ApprovedBGD = currentUser.EmployeeID;
                        }
                        else if (item.IsApprovedBGD == false)
                        {
                            item.ApprovedBGD = null;
                        }
                        await _repo.UpdateAsync(item);
                    }
                }
                return Ok(ApiResponseFactory.Success(data, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-by-id/{id}")]
        public IActionResult GetByID(int id)
        {
            ProjectPartlistPurchaseRequest purchaseRequest = _repo.GetByID(id);
            return Ok(ApiResponseFactory.Success(purchaseRequest, "Lưu dữ liệu thành công"));
        }
        [HttpPost("keep-product")]
        public IActionResult KeepProduct([FromBody] List<ProductHoldDTO> data)
        {
            string message = string.Empty;
            if (!_repo.ValidateKeepProduct(data, out message))
            {
                return BadRequest(ApiResponseFactory.Fail(null, message));
            }

            foreach (var item in data)
            {
                try
                {
                    if (item.ProjectParlistPurchaseRequestID.Count <= 0 || item.ProductSaleID <= 0 || item.ProjectID <= 0) continue;

                    // Tạo hoặc cập nhật InventoryProject
                    var inventoryProject = item.ID > 0
                        ? _inventoryProjectRepo.GetByID(item.ID)
                        : new InventoryProject();

                    inventoryProject.ProjectID = item.ProjectID;
                    inventoryProject.ProductSaleID = item.ProductSaleID;
                    inventoryProject.EmployeeID = item.EmployeeID;
                    inventoryProject.WarehouseID = 1;
                    inventoryProject.Quantity = item.Quantity;

                    if (inventoryProject.ID > 0)
                        _inventoryProjectRepo.Update(inventoryProject);
                    else
                        _inventoryProjectRepo.Create(inventoryProject);

                    // Gán lại ID giữ hàng và update trực tiếp
                    foreach (var purchaseRequestID in item.ProjectParlistPurchaseRequestID)
                    {
                        if (purchaseRequestID <= 0) continue;
                        ProjectPartlistPurchaseRequest itemPR = _repo.GetByID(purchaseRequestID);
                        itemPR.InventoryProjectID = inventoryProject.ID;
                        _repo.Update(itemPR);
                    }
                    return Ok(ApiResponseFactory.Success(data, "Giữ hàng thành công"));
                    //var dictKeep = new Dictionary<Expression<Func<ProjectPartlistPurchaseRequest, object>>, object>
                    //{
                    //    { x => x.InventoryProjectID, inventoryProject.ID }
                    //};
                    //await _repo.UpdateFieldByAttributeAsync(x => x.ID == request.ProductCode, myDict);
                    //itemPR.UpdatedDate = DateTime.Now;
                    //itemPR.UpdatedBy = item.UpdatedBy ?? "system";
                }
                catch (Exception ex)
                {
                    return BadRequest(new { status = 0, message = $"Lỗi khi cập nhật giữ hàng:{ex.Message}", error = ex.ToString() });
                }
            }

            return Ok(new { status = 1, message = "Đã cập nhật giữ hàng thành công." });
        }
    }
}