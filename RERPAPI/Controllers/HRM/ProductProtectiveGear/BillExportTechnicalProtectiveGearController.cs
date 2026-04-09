using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.Technical;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM.ProductProtectiveGear;
using RERPAPI.Repo.GenericEntity.Technical;
using RTCApi.Repo.GenericRepo;

namespace RERPAPI.Controllers.HRM.ProductProtectiveGear
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillExportTechnicalProtectiveGearController : ControllerBase
    {
        private readonly BillExportTechnicalRepo _billExportTechnicalRepo;
        private readonly BillExportDetailTechnicalRepo _billExportDetailTechnicalRepo;
        private readonly HistoryDeleteBillRepo _historyDeleteBillRepo;
        private readonly SupplierSaleRepo _supplierSaleRepo;
        private readonly CustomerRepo _customerRepo;
        private readonly RulePayRepo _rulePayRepo;
        private readonly WarehouseRepo _warehouseRepo;
        public BillExportTechnicalProtectiveGearController(
            BillExportTechnicalRepo billExportTechnicalRepo,
            BillExportDetailTechnicalRepo billExportDetailTechnicalRepo,
            SupplierSaleRepo supplierSaleRepo,
            CustomerRepo customerRepo,
            RulePayRepo rulePayRepo,
            WarehouseRepo warehouseRepo,
            BillImportDetailTechnicalRepo billImportDetailTechnicalRepo,
            HistoryDeleteBillRepo historyDeleteBillRepo
            )
        {
            _billExportTechnicalRepo = billExportTechnicalRepo;
            _billExportDetailTechnicalRepo = billExportDetailTechnicalRepo;
            _historyDeleteBillRepo = historyDeleteBillRepo;
            _supplierSaleRepo = supplierSaleRepo;
            _customerRepo = customerRepo;
            _rulePayRepo = rulePayRepo;
            _warehouseRepo = warehouseRepo;
        }
        [HttpGet("get-all")]
        public IActionResult GetInventoryDemo([FromQuery] BillExportTechnicalRequestParam param)
        {
            try
            {
                int allProduct = 1;
                string keywords = "";
                if (!string.IsNullOrEmpty(param.FilterText))
                {
                    keywords = param.FilterText;
                }
                var data = SQLHelper<object>.ProcedureToList("spGetBillExportTechnical"
                , new string[] { "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@Status", "@FilterText", "@WarehouseID" }
                , new object[] { 1, 999999, param.DateStart, param.DateEnd, param.Status, keywords, param.WarehouseID }
                );
                var data0 = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(data0, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> PostSaveDataBillImportDetailAsync([FromBody] List<BillExportTechnicalProtectiveGearDTO> dtos)
        {
            try
            {
                foreach (var dto in dtos)
                {
                    var billExportTechnical = dto.BillExportTechnical;
                    var billExportDetailTechnical = dto.BillExportDetailTechnical;
                    var deletedDetailIds = dto.DeletedDetailIds;

                    // ============================================
                    // 1. XỬ LÝ MASTER (BillImportTechnical)
                    // ============================================
                    int masterID = 0;

                    if (billExportTechnical.ID > 0)
                    {
                        // UPDATE - Bản ghi đã tồn tại
                        var result = await _billExportTechnicalRepo.UpdateAsync(billExportTechnical);
                        if (result <= 0)
                        {
                            return Ok(ApiResponseFactory.Fail(null, "Lỗi cập nhật BillImportTechnical"));
                        }
                        masterID = billExportTechnical.ID;
                    }
                    else
                    {
                        // INSERT - Bản ghi mới
                        var insertedId = await _billExportTechnicalRepo.CreateAsync(billExportTechnical);
                        if (insertedId <= 0)
                        {
                            return Ok(ApiResponseFactory.Fail(null, "Lỗi thêm mới Bill Export Technical"));
                        }
                        masterID = billExportTechnical.ID;
                    }

                    // ============================================
                    // 2. XỬ LÝ DELETED DETAILS
                    // ============================================
                    List<BillExportDetailTechnical> deletedBillDetail = new List<BillExportDetailTechnical>();
                    if (deletedDetailIds != null && deletedDetailIds.Count > 0)
                    {
                        foreach (var detailId in deletedDetailIds)
                        {

                            var billDetailByID = _billExportDetailTechnicalRepo.GetByID(detailId);
                            deletedBillDetail.Add(billDetailByID);

                            //// xóa mềm
                            //billDetailByID.IsDeleted = true;
                            //await _billImportDetailTechnicalRepo.UpdateAsync(billDetailByID);
                        }
                        var result1 = await _billExportDetailTechnicalRepo.DeleteRangeAsync(deletedBillDetail); // xóa cứng
                    }

                    // ============================================
                    // 3. XỬ LÝ DETAILS (Insert/Update)
                    // ============================================
                    if (billExportDetailTechnical != null && billExportDetailTechnical.Count > 0)
                    {
                        foreach (var detail in billExportDetailTechnical)
                        {
                            // Gán BillImportTechID từ master
                            detail.BillExportTechID = masterID;

                            if (detail.ID > 0)
                            {
                                // UPDATE - Bản ghi đã tồn tại detail
                                var updateResult = await _billExportDetailTechnicalRepo.UpdateAsync(detail);
                                if (updateResult <= 0)
                                {
                                    return Ok(ApiResponseFactory.Fail(null, $"Lỗi cập nhật detail ID: {detail.ID}"));
                                }
                            }
                            else
                            {
                                // INSERT - Bản ghi mới
                                var insertResult = await _billExportDetailTechnicalRepo.CreateAsync(detail);
                                if (insertResult <= 0)
                                {
                                    return Ok(ApiResponseFactory.Fail(null, "Lỗi thêm mới detail"));
                                }
                            }
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("delete-data")]
        public async Task<IActionResult> PostDeleteDataAsync([FromBody] int id)
        {
            try
            {

                var data = _billExportTechnicalRepo.GetByID(id);
                if (data.ID <= 0)
                {
                    return Ok(ApiResponseFactory.Fail(null, $"Không tìm thấy bill có id = {id}!"));
                }
                if (data.Status == 1)
                {
                    return Ok(ApiResponseFactory.Fail(null, $"Phiếu xuất [{data.Code}] đã được duyệt.\nBạn không thể xóa phiếu này!)", "Thông báo"));
                }
                data.IsDeleted = true;
                var result = await _billExportTechnicalRepo.UpdateAsync(data);
                if (result <= 0)
                {
                    return Ok(ApiResponseFactory.Fail(null, $"Xóa bill import không thành công!", "Thông báo"));
                }
                // xóa bill detail
                List<BillExportDetailTechnical> lstDetail = _billExportDetailTechnicalRepo.GetAll(c => c.BillExportTechID == data.ID);
                if (lstDetail != null)
                {
                    foreach (var item in lstDetail)
                    {
                        item.IsDeleted = true;
                        await _billExportDetailTechnicalRepo.UpdateAsync(item);
                    }
                }

                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                HistoryDeleteBill historyDeleteBill = new HistoryDeleteBill()
                {
                    BillID = data.ID,
                    UserID = currentUser.ID,
                    DeleteDate = DateTime.Now,
                    Name = currentUser.LoginName,
                    TypeBill = data.Code
                };
                var resultSaveHistoryDelete = await _historyDeleteBillRepo.CreateAsync(historyDeleteBill);
                if (resultSaveHistoryDelete <= 0)
                {
                    return Ok(ApiResponseFactory.Fail(null, $"Lưu lịch sử xóa không thành công!", "Thông báo"));
                }
                return Ok(ApiResponseFactory.Success(null, "Xóa thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("billexport/{id}")]
        public IActionResult GetBillImportByID(int id)
        {
            try
            {
                var result = _billExportTechnicalRepo.GetByID(id);
                /*   var newCode = _billexportRepo.GetBillCode()*/
                return Ok(new
                {
                    status = 1,
                    data = result,
                    /*  newCode = newCode,*/
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
        [HttpGet("BillExportDetail/BillExportID/{billExportID}")]
        public IActionResult GetBillExportDetailByBillID(int billExportID)
        {
            try
            {

                var data = SQLHelper<dynamic>.ProcedureToList("spGetBillExportTechDetail_New", new string[] { "@Id" }, new object[] { billExportID });
                var data0 = SQLHelper<object>.GetListData(data, 0);
                return Ok(new
                {
                    status = 1,
                    data = data0,
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

        [HttpGet("get-supplier")]
        public IActionResult GetSupplier()
        {
            try
            {

                var data = _supplierSaleRepo.GetAll().OrderByDescending(c => c.NgayUpdate);
                return Ok(new
                {
                    status = 1,
                    data = data,
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
        [HttpGet("get-customer")]
        public IActionResult GetCustomer()
        {
            try
            {

                var data = _customerRepo.GetAll(c => c.IsDeleted == false).OrderByDescending(c => c.ID);
                return Ok(new
                {
                    status = 1,
                    data = data,
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
        [HttpGet("get-receiver-and-deliver")]
        public IActionResult GetReceiverAndDeliver()
        {
            try
            {

                var data = SQLHelper<object>.ProcedureToList("spGetUsersHistoryProductRTC", new string[] { "@UsersID" }, new object[] { 0 });
                var data0 = SQLHelper<object>.GetListData(data, 0);
                return Ok(new
                {
                    status = 1,
                    data = data0,
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
        [HttpGet("get-rule-pay")]
        public IActionResult GetRulePay()
        {
            try
            {

                var data = _rulePayRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = data,
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
        [HttpGet("get-warehouse")]
        public IActionResult GetWarehouse()
        {
            try
            {

                var data = _warehouseRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = data,
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
        [HttpGet("get-product")]
        public IActionResult GetProduct(int warehouseID)
        {
            try
            {
                int productGroupID = 140;
                productGroupID = 0;
                string keyword = "";
                var data = SQLHelper<object>.ProcedureToList("spGetInventoryDemo",
                                        new string[] { "@ProductGroupID", "@Keyword", "@CheckAll", "@WarehouseID" },
                                        new object[] { productGroupID, keyword, "", -1, warehouseID });
                var data0 = SQLHelper<object>.GetListData(data, 0);
                return Ok(new
                {
                    status = 1,
                    data = data0,
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
    }
}
