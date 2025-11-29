using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using System.Data;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectPartlistPurchaseRequestController : ControllerBase
    {
        #region Khai báo repository

        private ProjectPartlistPurchaseRequestRepo _repo;
        private InventoryProjectRepo _inventoryProjectRepo;
        ProjectPartlistPurchaseRequestTypeRepo _typeRepo;
        ProjectRepo _projectRepo;
        POKHRepo _pokhRepo;
        //private InventoryRepo _invRepo = new InventoryRepo();

        public ProjectPartlistPurchaseRequestController(ProjectPartlistPurchaseRequestRepo projectPartlistPurchaseRequestRepo, InventoryProjectRepo inventoryProjectRepo, ProjectPartlistPurchaseRequestTypeRepo projectPartlistPurchaseRequestTypeRepo, ProjectRepo projectRepo, POKHRepo pOKHRepo)
        {
            _repo = projectPartlistPurchaseRequestRepo;
            _inventoryProjectRepo = inventoryProjectRepo;
            _typeRepo = projectPartlistPurchaseRequestTypeRepo;
            _projectRepo = projectRepo;
            _pokhRepo = pOKHRepo;
        }

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
                "@POKHID", "@ProductRTCID", "@IsDeleted", "@IsTechBought", "@IsJobRequirement"

                    },
                    new object[] {
                filter.DateStart, filter.DateEnd, filter.StatusRequest, filter.ProjectID, filter.Keyword,
                filter.SupplierSaleID, filter.IsApprovedTBP, filter.IsApprovedBGD, filter.IsCommercialProduct,
                filter.POKHID, filter.ProductRTCID, filter.IsDeleted, filter.IsTechBought, filter.IsJobRequirement

                    });

                var data = SQLHelper<dynamic>.GetListData(dt, 0);

                return Ok(new
                {
                    data,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message
                });
            }
        }
        [HttpGet("get-all-project")]
        public IActionResult GetAllProject()
        {
            List<Model.Entities.Project> lstProjects = _projectRepo.GetAll();
            return Ok(ApiResponseFactory.Success(lstProjects, ""));
        }
        [HttpGet("get-po-code")]
        public IActionResult GetPOCode()
        {
            List<POKH> lstPos = _pokhRepo.GetAll();
            return Ok(ApiResponseFactory.Success(lstPos, ""));

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
        //          return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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

                        _repo.Update(existingRequest);
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
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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

                        _repo.Update(existingRequest);
                        if (item.InventoryProjectID > 0)
                        {
                            InventoryProject inven = new InventoryProject
                            {
                                ID = item.InventoryProjectID ?? 0,
                                IsDeleted = true
                            };
                            _inventoryProjectRepo.Update(inven);
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
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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

                        _repo.Update(item);
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
                    _repo.Update(item);
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

                    _repo.Update(item);
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
                    _repo.Update(item);
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
                        _repo.Update(itemPR);
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
        [HttpPost("duplicate")]
        public IActionResult Duplicate([FromBody] ProjectPartlistPurchaseRequest row)
        {
            if (row == null || row.ID <= 0)
                return BadRequest(new { status = 0, message = "Dữ liệu không hợp lệ." });

            try
            {
                var original = _repo.GetByID(row.ID);
                if (original == null)
                    return BadRequest(new { status = 0, message = "Không tìm thấy bản gốc." });

                var duplicate = new ProjectPartlistPurchaseRequest
                {
                    // Copy tất cả field
                    ProjectPartListID = original.ProjectPartListID,
                    ProductCode = original.ProductCode,
                    ProductName = original.ProductName,
                    Quantity = 0,
                    UnitName = original.UnitName,
                    ProductGroupID = original.ProductGroupID,
                    StatusRequest = original.StatusRequest,
                    DateRequest = original.DateRequest,
                    DateReturnExpected = original.DateReturnExpected,
                    UnitMoney = original.UnitMoney,
                    CurrencyID = original.CurrencyID,
                    CurrencyRate = original.CurrencyRate,
                    UnitPrice = original.UnitPrice,
                    HistoryPrice = original.HistoryPrice,
                    TotalPrice = 0,
                    TotalPriceExchange = 0,
                    TotaMoneyVAT = 0,
                    LeadTime = original.LeadTime,
                    Note = original.Note,
                    ReasonCancel = original.ReasonCancel,
                    DateReturnActual = original.DateReturnActual,
                    DateReceive = original.DateReceive,
                    IsImport = original.IsImport,
                    UnitFactoryExportPrice = original.UnitFactoryExportPrice,
                    ProjectPartlistPurchaseRequestTypeID = original.ProjectPartlistPurchaseRequestTypeID,
                    SupplierSaleID = original.SupplierSaleID,
                    IsApprovedTBP = original.IsApprovedTBP,
                    ApprovedTBP = original.ApprovedTBP,
                    DateApprovedTBP = original.DateApprovedTBP,
                    ApprovedBGD = original.ApprovedBGD,
                    DateApprovedBGD = original.DateApprovedBGD,
                    ProductSaleID = original.ProductSaleID,
                    UnitImportPrice = original.UnitImportPrice,
                    TotalImportPrice = original.TotalImportPrice,
                    IsCommercialProduct = original.IsCommercialProduct,
                    IsDeleted = original.IsDeleted,
                    JobRequirementID = original.JobRequirementID,
                    InventoryProjectID = original.InventoryProjectID,
                };

                // Xử lý DuplicateID
                if (row.DuplicateID > 0)
                {
                    duplicate.DuplicateID = row.DuplicateID;
                    duplicate.OriginQuantity = row.OriginQuantity;
                }
                else
                {
                    duplicate.DuplicateID = original.ID;
                    duplicate.OriginQuantity = original.Quantity;
                }
                _repo.Create(duplicate);
                var newId = duplicate.ID;

                // Cập nhật bản gốc nếu cần
                if (row.DuplicateID <= 0)
                {
                    original.DuplicateID = newId;
                    original.OriginQuantity = original.Quantity;
                    _repo.Update(original);
                }

                return Ok(new { status = 1, newId });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //[HttpGet("request-types")]
        //public IActionResult GetRequestTypes()
        //{
        //    try
        //    {
        //        var types = _typeRepo.GetAll()
        //            .Select(t => new
        //            {
        //                t.ID,
        //                t.RequestTypeName,
        //                t.RequestTypeCode
        //            })
        //            .OrderBy(t => t.ID)
        //            .ToList();

        //        return Ok(ApiResponseFactory.Success(types, null));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}

        [HttpPost("validate-duplicate")]
        public IActionResult ValidateDuplicate([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return Ok(new { status = 1, message = "Không có dữ liệu để kiểm tra." });

            try
            {
                // Dùng Dictionary để gom nhóm nhanh
                var duplicateGroups = new Dictionary<int, List<ProjectPartlistPurchaseRequest>>();

                // Lặp từng ID → Lấy từng dòng
                foreach (var id in ids)
                {
                    var request = _repo.GetByID(id);
                    if (request == null || request.DuplicateID <= 0) continue;

                    if (!duplicateGroups.ContainsKey(request.DuplicateID ?? 0))
                        duplicateGroups[request.DuplicateID ?? 0] = new List<ProjectPartlistPurchaseRequest>();

                    duplicateGroups[request.DuplicateID ?? 0].Add(request);
                }

                // Kiểm tra từng nhóm
                var errors = new List<object>();

                foreach (var group in duplicateGroups)
                {
                    var duplicateID = group.Key;
                    var items = group.Value;

                    // Lấy OriginQuantity từ bản gốc (ID == DuplicateID)
                    var original = items.FirstOrDefault(x => x.ID == duplicateID);
                    var originQuantity = original?.OriginQuantity ?? 0;

                    // Tính tổng Quantity
                    var totalQuantity = items.Sum(x => x.Quantity ?? 0);

                    if (Math.Abs(totalQuantity - originQuantity) > 0.0001m) // Tránh lỗi float
                    {
                        errors.Add(new
                        {
                            DuplicateID = duplicateID,
                            OriginQuantity = originQuantity,
                            TotalQuantity = totalQuantity,
                            Message = $"DuplicateID {duplicateID}: Tổng SL = {totalQuantity}, phải = {originQuantity}",
                            ProductCodes = string.Join(", ", items.Select(x => x.ProductCode).Where(pc => !string.IsNullOrEmpty(pc)))
                        });
                    }
                }

                if (!errors.Any())
                    return Ok(new { status = 1, message = "Tất cả DuplicateID đều hợp lệ." });

                return BadRequest(new
                {
                    status = 0,
                    message = "Có lỗi DuplicateID: Tổng số lượng không khớp với OriginQuantity.",
                    errors
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("unTech-bought")]
        public async Task<IActionResult> UnTechBought(int id)
        {
            try
            {
                var data = _repo.GetAll(x=>x.ProjectPartListID == id).FirstOrDefault();
                data.IsDeleted = true;
                await _repo.UpdateAsync(data);
                return Ok(ApiResponseFactory.Success(null, "Hủy mua thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}