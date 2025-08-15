using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using System.Data;
using System.Text.Json;


namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectPartlistPurchaseRequestController : ControllerBase
    {
        #region Khai báo repository
        private ProjectPartlistPurchaseRequestRepo _repo = new ProjectPartlistPurchaseRequestRepo();
        private InventoryProjectRepo _inventoryProjectRepo = new InventoryProjectRepo();
        //private InventoryRepo _invRepo = new InventoryRepo();

        #endregion

        [HttpPost("get-all")]
        public IActionResult GetAll([FromBody] ProjectPartlistPurchaseRequestParam filter, int employeeID)
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

                var purchaseRequests = allData.Where(x => x.ProductRTCID == null || x.ProductRTCID <= 0).ToList();// Lọc PurchaseRequest

                var dataRTC = allData.Where(x => x.ProductRTCID > 0 && x.TicketType == 0).ToList();// Lọc ProductRTC
                var techBought = allData.Where(x => x.IsTechBought == true).ToList();// Lọc kt đã mua

                var productRTCBorrow = allData
                    .Where(x => x.TicketType == 1 && (filter.IsApprovedTBP == 0 || x.ApprovedTBP == employeeID)).ToList();//lọc ProductRTC mượn

                var productCommercial = allData.Where(p => p.IsCommercialProduct == true).ToList();
                var productHr = allData.Where(p => p.JobRequirementID > 0).ToList();
                int a = 1 * 15;
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        purchaseRequests,
                        dataRTC,
                        techBought,
                        productRTCBorrow,
                        productCommercial,
                        productHr
                    }
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
        //[HttpPost("save-data")]
        //public async Task<IActionResult> SaveData([FromBody] List<ProjectPartlistPurchaseRequest> data)
        //{
        //    try
        //    {
        //        foreach (var item in data)
        //        {
        //            if (item.ID <= 0)
        //            {
        //                await _repo.CreateAsync(item);
        //            }
        //            else
        //            {
        //                _repo.UpdateFieldsByID(item.ID, item);
        //            }
        //        }

        //        return Ok(new { status = 1, message = "Lưu dữ liệu thành công" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { status = 0, message = ex.Message });
        //    }
        //}
        [HttpGet("get-by-id/{id}")]
        public IActionResult GetByID(int id)
        {
            ProjectPartlistPurchaseRequest purchaseRequest = _repo.GetByID(id);
            return Ok(new
            {
                status = 1,
                data = purchaseRequest
            });
        }
        [HttpPost("check-order")]
        public IActionResult CheckOrder([FromBody] List<ProjectPartlistPurchaseRequest> data)
        {
            try
            {
                if (data == null || data.Count == 0)
                {
                    return BadRequest(new { status = 0, message = "Dữ liệu không hợp lệ" });
                }

                foreach (var item in data)
                {
                    try
                    {
                        int id = item.ID;
                        int employeeId = item.EmployeeIDRequestApproved ?? 0;

                        if (id <= 0 || employeeId <= 0) continue;

                        var existingRequest = _repo.GetByID(id);
                        if (existingRequest == null) continue;

                        // Chỉ cho phép cập nhật nếu chưa ai check hoặc mình là người đã check
                        if (existingRequest.EmployeeIDRequestApproved != 0 &&
                            existingRequest.EmployeeIDRequestApproved != employeeId)
                        {
                            continue;
                        }

                        // Toggle trạng thái: nếu đang là 0 thì gán employeeId, nếu đang là mình thì gán lại 0
                        existingRequest.EmployeeIDRequestApproved =
                            existingRequest.EmployeeIDRequestApproved == 0 ? employeeId : 0;

                        _repo.UpdateFieldsByID(id, existingRequest);
                    }
                    catch
                    {
                        continue;
                    }
                }

                return Ok(new { status = 1, message = "Đã xử lý xong danh sách check." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 0, message = ex.Message });
            }
        }
        [HttpPost("cancel-request")]
        public IActionResult CancelRequest([FromBody] List<ProjectPartlistPurchaseRequest> data)
        {
            try
            {
                if (data == null || data.Count == 0)
                {
                    return BadRequest(new { status = 0, message = "Dữ liệu không hợp lệ" });
                }

                foreach (var item in data)
                {
                    try
                    {
                        int id = item.ID;

                        if (id <= 0) continue;

                        var existingRequest = _repo.GetByID(id);
                        if (existingRequest == null) continue;

                        existingRequest.IsDeleted = item.IsDeleted;

                        _repo.UpdateFieldsByID(id, existingRequest);
                        if (item.InventoryProjectID > 0)
                        {
                            InventoryProject inven = new InventoryProject
                            {
                                ID = item.InventoryProjectID ?? 0,
                                IsDeleted = true
                            };
                            _inventoryProjectRepo.UpdateFieldsByID(inven.ID, inven);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                return Ok(new { status = 1, message = "Đã xử lý xong danh sách xoá." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 0, message = ex.Message });
            }
        }
        [HttpPost("request-approved")]
        public IActionResult RequestApproved([FromBody] List<ProjectPartlistPurchaseRequest> data)
        {
            try
            {
                if (data == null || data.Count == 0)
                {
                    return BadRequest(new { status = 0, message = "Dữ liệu không hợp lệ." });
                }

                foreach (var item in data)
                {
                    try
                    {
                        if (item.ID <= 0) continue;

                        //item.UpdatedDate = DateTime.Now;
                        //item.UpdatedBy = Global.LoginName;

                        _repo.UpdateFieldsByID(item.ID, item);
                    }
                    catch
                    {
                        continue; // Bỏ qua lỗi từng dòng, không dừng toàn bộ
                        //return BadRequest(new { status = 0, message = "Lỗi khi cập nhật yêu cầu duyệt." });
                    }
                }

                return Ok(new { status = 1, message = "Đã cập nhật trạng thái yêu cầu duyệt thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 0, message = $"Lỗi xử lý: {ex.Message}" });
            }
        }
        [HttpPost("complete-request-buy")]
        public IActionResult CompleteRequest([FromBody] List<ProjectPartlistPurchaseRequest> data)
        {
            if (data == null || data.Count == 0)
                return BadRequest(new { status = 0, message = "Danh sách không hợp lệ!" });

            foreach (var item in data)
            {
                try
                {
                    if (item.ID <= 0) continue;

                    //item.UpdatedDate = DateTime.Now;
                    //item.UpdatedBy = Global.LoginName;

                    _repo.UpdateFieldsByID(item.ID, item);
                }
                catch
                {
                    continue;
                }
            }

            return Ok(new { status = 1, message = "Cập nhật trạng thái hoàn thành thành công!" });
        }
        [HttpPost("approve")]
        public IActionResult Approve([FromBody] List<ProjectPartlistPurchaseRequest> requests, int employeeID)
        {
            if (requests == null || requests.Count == 0)
                return BadRequest(new { status = 0, message = "Danh sách yêu cầu không hợp lệ!" });

            foreach (var item in requests)
            {
                try
                {
                    if (item.ID <= 0) continue;

                    var now = DateTime.Now;

                    // Cập nhật trực tiếp các trường
                    //item.UpdatedDate = now;
                    //item.UpdatedBy = Global.LoginName;

                    // Nếu có BGĐ duyệt thì gán luôn người duyệt
                    if (item.IsApprovedBGD == true || item.IsApprovedBGD == false)
                    {
                        item.ApprovedBGD = item.ApprovedBGD ?? employeeID;
                        item.DateApprovedBGD = now;
                    }

                    // Nếu có TBP duyệt thì gán ngày duyệt
                    if (item.IsApprovedTBP == true || item.IsApprovedTBP == false)
                    {
                        item.DateApprovedTBP = now;
                    }

                    _repo.UpdateFieldsByID(item.ID, item);
                }
                catch
                {
                    continue; // Bỏ qua dòng lỗi
                }
            }

            return Ok(new { status = 1, message = "Cập nhật duyệt thành công!" });
        }

        [HttpPost("save-data")]
        public IActionResult SaveData([FromBody] List<ProjectPartlistPurchaseRequest> data)
        {
            if (data == null || data.Count == 0)
                return BadRequest(new { status = 0, message = "Dữ liệu không hợp lệ." });

            foreach (var item in data)
            {
                try
                {
                    if (item.ID <= 0) continue;

                    //item.UpdatedDate = DateTime.Now;
                    //item.UpdatedBy = item.UpdatedBy ?? "System";

                    // Cập nhật nhiều field cùng lúc
                    _repo.UpdateFieldsByID(item.ID, item);
                }
                catch
                {
                    continue;
                }
            }
            return Ok(new { status = 1, message = "Cập nhật thành công." });
        }
        [HttpPost("keep-product")]
        public IActionResult KeepProduct([FromBody] List<ProductHoldDTO> data)
        {
            if (data == null || data.Count == 0)
                return BadRequest(new { status = 0, message = "Danh sách sản phẩm không hợp lệ!" });

            foreach (var item in data)
            {
                try
                {
                    if (item.ProjectParlistPurchaseRequestID.Count <= 0 || item.ProductSaleID <= 0 || item.ProjectID <= 0) continue;

                    // Check tồn kho

                    var dt = SQLHelper<dynamic>.ProcedureToList("spGetInventory", new[] { "@ProductSaleID" }, new object[] { item.ProductSaleID });
                    var inventoryData = SQLHelper<dynamic>.GetListData(dt, 0);
                    var quantity = inventoryData[0]?.TotalQuantityLast;
                    if (quantity == null || Convert.ToDecimal(quantity) <= 0)
                    {
                        return BadRequest(new
                        {
                            status = 0,
                            message = "Số lượng tồn cuối kì phải lớn hơn 0!"
                        });
                    }
                    ;


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
                        _repo.UpdateFieldsByID(item.ID, itemPR);

                    }
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
