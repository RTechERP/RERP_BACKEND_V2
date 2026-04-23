using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.HRM.ProductProtectiveGear;
using RERPAPI.Model.Param.Technical;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM.ProductProtectiveGear;
using RERPAPI.Repo.GenericEntity.Technical;
using RTCApi.Repo.GenericRepo;
using System.Drawing.Drawing2D;
using System.Net;

namespace RERPAPI.Controllers.HRM.ProductProtectiveGear
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillImportTechnicalProtectiveGearController : ControllerBase
    {
        private readonly BillImportTechnicalRepo _billImportTechnicalRepo;
        private readonly BillImportDetailTechnicalRepo _billImportDetailTechnicalRepo;
        private readonly SupplierSaleRepo _supplierSaleRepo;
        private readonly CustomerRepo _customerRepo;
        private readonly RulePayRepo _rulePayRepo;
        private readonly WarehouseRepo _warehouseRepo;
        private readonly HistoryDeleteBillRepo _historyDeleteBillRepo;


        public BillImportTechnicalProtectiveGearController
            (BillImportTechnicalRepo billImportTechnicalRepo,
            SupplierSaleRepo supplierSaleRepo,
            CustomerRepo customerRepo,
            RulePayRepo rulePayRepo,
            WarehouseRepo warehouseRepo,
            BillImportDetailTechnicalRepo billImportDetailTechnicalRepo,
            HistoryDeleteBillRepo historyDeleteBillRepo


            )
        {
            _billImportTechnicalRepo = billImportTechnicalRepo;
            _billImportDetailTechnicalRepo = billImportDetailTechnicalRepo;
            _supplierSaleRepo = supplierSaleRepo;
            _customerRepo = customerRepo;
            _rulePayRepo = rulePayRepo;
            _warehouseRepo = warehouseRepo;
            _historyDeleteBillRepo = historyDeleteBillRepo;
        }
        [HttpGet("get-all")]
        public IActionResult GetInventoryDemo([FromQuery] BillImportTechnicalRequestParam param)
        {
            try
            {
                int allProduct = 1;
                string keywords = "";
                if (!string.IsNullOrEmpty(param.FilterText))
                {
                    keywords = param.FilterText;
                }
                var data = SQLHelper<object>.ProcedureToList("spGetBillImportTechnical"
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
        [HttpGet("billimport/{id}")]
        public IActionResult GetBillImportByID(int id)
        {
            try
            {
                var result = _billImportTechnicalRepo.GetByID(id);
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
        [HttpGet("BillImportDetail/BillImportID/{billImportID}")]
        public IActionResult GetBillExportDetailByBillID(int billImportID)
        {
            try
            {

                var data = SQLHelper<dynamic>.ProcedureToList("spGetBillImportDetailTechnical", new string[] { "@ID" }, new object[] { billImportID });
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

                var data = _customerRepo.GetAll().OrderByDescending(c => c.ID);
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
                string productGroupNo = "DBH";
                int productGroupID = 140;
                productGroupID = 0;
                string keyword = "";
                var data = SQLHelper<object>.ProcedureToList("spGetProductRTC",
                                new string[] { "@ProductGroupID", "@Keyword", "@CheckAll", "@WarehouseID", "@ProductGroupNo" },
                                new object[] { productGroupID, keyword, 1, warehouseID, productGroupNo });
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
        //[HttpPost("save-data-bill-import")]
        //public async Task<IActionResult> PostSaveDataBillImportAsync([FromBody] BillImportTechnical billImportTechnical)
        //{
        //    try
        //    {
        //        if (billImportTechnical.ID <= 0)
        //        {
        //            billImportTechnical.CreatedDate = DateTime.Now;
        //            await _billImportTechnicalRepo.CreateAsync(billImportTechnical);
        //        }
        //        else await _billImportTechnicalRepo.UpdateAsync(billImportTechnical);

        //        return Ok(ApiResponseFactory.Success(billImportTechnical, ""));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}
        [HttpGet("get-bill-code")]
        public async Task<IActionResult> GenerateBillCode(string billCode, int id)
        {
            var exp1 = new Expression("BillCode", billCode);
            var exp2 = new Expression("ID", id, "<>");

            var listBillImports = _billImportTechnicalRepo.GetAll(c => c.BillCode == billCode && c.ID != id);
            //var listBillImports = SQLHelper<BillImportTechnical>.FindByExpression(exp1.And(exp2));
            bool status = false;
            if (listBillImports.Count() >= 0)
            {
                status = true;


            }
            return Ok(new
            {
                status = 1,
                data = status
            });
        }
        [HttpGet("validate-bill-code")]
        public async Task<IActionResult> ValidateBillCode([FromQuery] int billtype)
        {
            string billCode = _billImportTechnicalRepo.GetBillCode(billtype);
            return Ok(new
            {
                status = 1,
                data = billCode
            });
        }


        [HttpPost("save-data")]
        public async Task<IActionResult> PostSaveDataBillImportDetailAsync([FromBody] List<BillImportTechnicalProtectiveGearDTO> dtos)
        {
            try
            {
                foreach (var dto in dtos)
                {
                    var billImportTechnical = dto.BillImportTechnical;
                    var billImportDetailTechnical = dto.BillImportDetailTechnical;
                    var deletedDetailIds = dto.DeletedDetailIds;

                    // ============================================
                    // 1. XỬ LÝ MASTER (BillImportTechnical)
                    // ============================================
                    int masterID = 0;

                    if (billImportTechnical.ID > 0)
                    {
                        // UPDATE - Bản ghi đã tồn tại
                        var result = await _billImportTechnicalRepo.UpdateAsync(billImportTechnical);
                        if (result <= 0)
                        {
                            return Ok(ApiResponseFactory.Fail(null, "Lỗi cập nhật BillImportTechnical"));
                        }
                        masterID = billImportTechnical.ID;
                    }
                    else
                    {
                        // INSERT - Bản ghi mới
                        var insertedId = await _billImportTechnicalRepo.CreateAsync(billImportTechnical);
                        if (insertedId <= 0)
                        {
                            return Ok(ApiResponseFactory.Fail(null, "Lỗi thêm mới BillImportTechnical"));
                        }
                        masterID = billImportTechnical.ID;
                    }

                    // ============================================
                    // 2. XỬ LÝ DELETED DETAILS
                    // ============================================
                    List<BillImportDetailTechnical> deletedBillDetail = new List<BillImportDetailTechnical>();
                    if (deletedDetailIds != null && deletedDetailIds.Count > 0)
                    {
                        foreach (var detailId in deletedDetailIds)
                        {

                            var billDetailByID = _billImportDetailTechnicalRepo.GetByID(detailId);
                            deletedBillDetail.Add(billDetailByID);

                            //// xóa mềm
                            //billDetailByID.IsDeleted = true;
                            //await _billImportDetailTechnicalRepo.UpdateAsync(billDetailByID);
                        }
                        var result1 = await _billImportDetailTechnicalRepo.DeleteRangeAsync(deletedBillDetail); // xóa cứng
                    }

                    // ============================================
                    // 3. XỬ LÝ DETAILS (Insert/Update)
                    // ============================================
                    if (billImportDetailTechnical != null && billImportDetailTechnical.Count > 0)
                    {
                        foreach (var detail in billImportDetailTechnical)
                        {
                            // Gán BillImportTechID từ master
                            detail.BillImportTechID = masterID;

                            if (detail.ID > 0)
                            {
                                // UPDATE - Bản ghi đã tồn tại detail
                                var updateResult = await _billImportDetailTechnicalRepo.UpdateAsync(detail);
                                if (updateResult <= 0)
                                {
                                    return Ok(ApiResponseFactory.Fail(null, $"Lỗi cập nhật detail ID: {detail.ID}"));
                                }
                            }
                            else
                            {
                                // INSERT - Bản ghi mới
                                var insertResult = await _billImportDetailTechnicalRepo.CreateAsync(detail);
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

                var data = _billImportTechnicalRepo.GetByID(id);
                if (data.ID <= 0)
                {
                    return Ok(ApiResponseFactory.Fail(null, $"Không tìm thấy bill có id = {id}!"));
                }
                if (data.Status == true)
                {
                    return Ok(ApiResponseFactory.Fail(null, $"Phiếu nhập [{data.BillCode}] đã được duyệt.\nBạn không thể xóa phiếu này!)", "Thông báo"));
                }
                data.IsDeleted = true;
                var result = await _billImportTechnicalRepo.UpdateAsync(data);
                if (result <= 0)
                {
                    return Ok(ApiResponseFactory.Fail(null, $"Xóa bill import không thành công!", "Thông báo"));
                }
                // xóa bill detail
                List<BillImportDetailTechnical> lstDetail = _billImportDetailTechnicalRepo.GetAll(c => c.BillImportTechID == data.ID);
                if (lstDetail != null)
                {
                    foreach (var item in lstDetail)
                    {
                        item.IsDeleted = true;
                        await _billImportDetailTechnicalRepo.UpdateAsync(item);
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
                    TypeBill = data.BillCode
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

    }
}
