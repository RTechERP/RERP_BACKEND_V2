using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Repo.GenericEntity.BBNV;

namespace RERPAPI.Controllers.HRM
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HandoverController : ControllerBase
    {
        private readonly HandoverRepo _handoverRepo;
        private readonly HandoverReceiverRepo _handoverReceiverRepo;
        private readonly HandoverWorkRepo _handoverWorkRepo;
        private readonly HandoverWarehouseAssetRepo _handoverWarehouseAssetRepo;
        private readonly HandoverAssetManagementRepo _handoverAssetManagementRepo;
        private readonly HandoverFinanceRepo _handoverFinanceRepo;
        private readonly HandoverApproveRepo _approveRepo;
        private readonly HandoverPersonalAssetRepo _handoverPersonalAssetRepo;

        public HandoverController(
            HandoverRepo handoverRepo,
            HandoverReceiverRepo handoverReceiverRepo,
            HandoverWorkRepo handoverWorkRepo,
            HandoverWarehouseAssetRepo handoverWarehouseAssetRepo,
            HandoverAssetManagementRepo handoverAssetManagementRepo,
            HandoverFinanceRepo handoverFinanceRepo,
            HandoverApproveRepo approveRepo,
            HandoverPersonalAssetRepo handoverPersonalAssetRepo)
        {
            _handoverRepo = handoverRepo;
            _handoverReceiverRepo = handoverReceiverRepo;
            _handoverWorkRepo = handoverWorkRepo;
            _handoverWarehouseAssetRepo = handoverWarehouseAssetRepo;
            _handoverAssetManagementRepo = handoverAssetManagementRepo;
            _handoverFinanceRepo = handoverFinanceRepo;
            _approveRepo = approveRepo;
            _handoverPersonalAssetRepo = handoverPersonalAssetRepo;
        }

        public class HandoverSearchRequest
        {
            public int DepartmentID { get; set; } = 0;
            public int EmployeeID { get; set; } = 0;
            public string DateStart { get; set; } = "";
            public string DateEnd { get; set; } = "";
            public string Keyword { get; set; } = "";
            public int ApproverID { get; set; } = 0;
        }

        // Lấy danh sách biên bản bàn giao
        [RequiresPermission("N34")]
        [HttpPost("getall")]
        public IActionResult GetAll([FromBody] HandoverSearchRequest req)
        {
            try
            {
                object start = string.IsNullOrEmpty(req.DateStart) ? DBNull.Value : DateTime.Parse(req.DateStart);
                object end = string.IsNullOrEmpty(req.DateEnd) ? DBNull.Value : DateTime.Parse(req.DateEnd);

                var datas = SQLHelper<object>.ProcedureToList("spGetHandover",
                    new string[] { "@DepartmentID", "@EmployeeID", "@DateStart", "@DateEnd", "@Keyword", "@ApproverID" },
                    new object[] { req.DepartmentID, req.EmployeeID, start, end, req.Keyword ?? "", req.ApproverID });

                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(datas, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        // Lấy chi tiết toàn bộ dữ liệu của một biên bản bàn giao (Unified Detail API)
        [RequiresPermission("N34")]
        [HttpGet("get-detail")]
        public async Task<IActionResult> GetDetail(int handoverId)
        {
            try
            {
                var master = _handoverRepo.GetByID(handoverId);
                var datas = SQLHelper<object>.ProcedureToList(" ",
                    new string[] { "@HandoverID" }, new object[] { handoverId });

                var result = new
                {
                    handover = master,
                    receivers = SQLHelper<object>.GetListData(datas, 0),
                    works = SQLHelper<object>.GetListData(datas, 1),
                    warehouseAssets = SQLHelper<object>.GetListData(datas, 2),
                    assetManagements = SQLHelper<object>.GetListData(datas, 3),
                    finances = SQLHelper<object>.GetListData(datas, 4),
                    personalAssets = SQLHelper<object>.GetListData(datas, 5),
                    approves = SQLHelper<object>.GetListData(datas, 6)
                };

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Lấy dữ liệu liên quan của nhân viên để lập biên bản mới
        [RequiresPermission("N34")]
        [HttpGet("get-related-assets")]
        public IActionResult GetRelatedAssets(int employeeId)
        {
            try
            {
                var datas = SQLHelper<object>.ProcedureToList("spGetHandoverRelatedDataByEmployee",
                    new string[] { "@EmployeeID" }, new object[] { employeeId });

                var result = new
                {
                    profile = SQLHelper<object>.GetListData(datas, 0),
                    works = SQLHelper<object>.GetListData(datas, 1),
                    warehouseAssets = SQLHelper<object>.GetListData(datas, 2),
                    assetManagements = SQLHelper<object>.GetListData(datas, 3),
                    finances = SQLHelper<object>.GetListData(datas, 4),

                    approves = SQLHelper<object>.GetListData(datas, 5)
                };

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [RequiresPermission("N34")]
        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] HandoverDTO dto)
        {
            try
            {
                if (dto == null || dto.Handover == null) return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));

                var handover = dto.Handover;
                if (handover.ID > 0)
                {
                    await _handoverRepo.UpdateAsync(handover);
                }
                else
                {
                    handover.Code = await _handoverRepo.GenerateCodeAsync();
                    await _handoverRepo.CreateAsync(handover);
                }

                int handoverID = handover.ID;

                // Handle Deletions
                if (dto.DeletedReceivers?.Count > 0) foreach (var id in dto.DeletedReceivers) await _handoverReceiverRepo.DeleteAsync(id);
                if (dto.DeletedWorks?.Count > 0) foreach (var id in dto.DeletedWorks) await _handoverWorkRepo.DeleteAsync(id);
                if (dto.DeletedWarehouseAssets?.Count > 0) foreach (var id in dto.DeletedWarehouseAssets) await _handoverWarehouseAssetRepo.DeleteAsync(id);
                if (dto.DeletedAssets?.Count > 0) foreach (var id in dto.DeletedAssets) await _handoverAssetManagementRepo.DeleteAsync(id);
                if (dto.DeletedFinances?.Count > 0) foreach (var id in dto.DeletedFinances) await _handoverFinanceRepo.DeleteAsync(id);
                if (dto.DeletedPersonalAssets?.Count > 0) foreach (var id in dto.DeletedPersonalAssets) await _handoverPersonalAssetRepo.DeleteAsync(id);

                // Handle Details
                if (dto.Receivers != null)
                {
                    foreach (var d in dto.Receivers)
                    {
                        d.HandoverID = handoverID;
                        if (d.ID > 0) await _handoverReceiverRepo.UpdateAsync(d);
                        else await _handoverReceiverRepo.CreateAsync(d);
                    }
                }

                if (dto.Works != null)
                {
                    foreach (var d in dto.Works)
                    {
                        d.HandoverID = handoverID;
                        if (d.ID > 0) await _handoverWorkRepo.UpdateAsync(d);
                        else await _handoverWorkRepo.CreateAsync(d);
                    }
                }

                if (dto.WarehouseAssets != null)
                {
                    foreach (var d in dto.WarehouseAssets)
                    {
                        d.HandoverID = handoverID;
                        if (d.ID > 0) await _handoverWarehouseAssetRepo.UpdateAsync(d);
                        else await _handoverWarehouseAssetRepo.CreateAsync(d);
                    }
                }

                if (dto.AssetManagements != null)
                {
                    foreach (var d in dto.AssetManagements)
                    {
                        d.HandoverID = handoverID;
                        if (d.ID > 0) await _handoverAssetManagementRepo.UpdateAsync(d);
                        else await _handoverAssetManagementRepo.CreateAsync(d);
                    }
                }

                if (dto.Finances != null)
                {
                    foreach (var d in dto.Finances)
                    {
                        d.HandoverID = handoverID;
                        if (d.ID > 0) await _handoverFinanceRepo.UpdateAsync(d);
                        else await _handoverFinanceRepo.CreateAsync(d);
                    }
                }

                if (dto.PersonalAssets != null)
                {
                    foreach (var d in dto.PersonalAssets)
                    {
                        d.HandoverID = handoverID;
                        if (d.ID > 0) await _handoverPersonalAssetRepo.UpdateAsync(d);
                        else await _handoverPersonalAssetRepo.CreateAsync(d);
                    }
                }

                if (dto.Approves != null)
                {
                    foreach (var d in dto.Approves)
                    {
                        d.HandoverID = handoverID;
                        if (d.ID > 0) await _approveRepo.UpdateAsync(d);
                        else await _approveRepo.CreateAsync(d);
                    }
                }

                return Ok(ApiResponseFactory.Success(handover, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        public class DeleteRequest { public int Id { get; set; } }

    
        [RequiresPermission("N34")]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteRequest req)
        {
            try
            {
                if (req == null || req.Id <= 0) return BadRequest(ApiResponseFactory.Fail(null, "ID không hợp lệ"));

                var item = await _handoverRepo.GetByIDAsync(req.Id);
                if (item == null) return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy dữ liệu"));

                item.IsDeleted = true;
                await _handoverRepo.UpdateAsync(item);
                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }



    
        [RequiresPermission("N34")]
        [HttpPost("save-approves")]
        public async Task<IActionResult> SaveApproves([FromBody] List<HandoverApprove> items)
        {
            try
            {
                foreach (var item in items)
                {
                    if (item.ID > 0) await _approveRepo.UpdateAsync(item);
                    else await _approveRepo.CreateAsync(item);
                }
                return Ok(ApiResponseFactory.Success(null, "Lưu lộ trình duyệt thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

      
        [RequiresPermission("N34")]
        [HttpPost("approve")]
        public async Task<IActionResult> Approve([FromBody] HandoverApprove item)
        {
            try
            {
                if (item.ID > 0) await _approveRepo.UpdateAsync(item);
                else await _approveRepo.CreateAsync(item);
                return Ok(ApiResponseFactory.Success(null, "Phê duyệt thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

      
        [RequiresPermission("N34")]
        [HttpGet("export-excel")]
        public IActionResult ExportExcel(int id)
        {
            try
            {
             
                return Ok(ApiResponseFactory.Success(null, "Chức năng xuất Excel đang được xử lý"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}