using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HistoryBorrowSaleController : ControllerBase
    {
        private readonly BillExportDetailRepo _billExportDetailRepo;
        private readonly WarehouseRepo _wareHouseRepo;
        private readonly UserRepo _userRepo;
        private readonly HistoryBorrowSaleLogRepo _historyBorrowSaleLogRepo;
        private readonly ProductGroupWareHouseRepo _productgroupwarehouseRepo;

        public HistoryBorrowSaleController(
            BillExportDetailRepo billExportDetailRepo,
            WarehouseRepo wareHouseRepo,
            UserRepo userRepo,
            HistoryBorrowSaleLogRepo historyBorrowSaleLogRepo,
            ProductGroupWareHouseRepo productgroupwarehouseRepo
            )
        {
            _billExportDetailRepo = billExportDetailRepo;
            _wareHouseRepo = wareHouseRepo;
            _userRepo = userRepo;
            _historyBorrowSaleLogRepo = historyBorrowSaleLogRepo;
            _productgroupwarehouseRepo = productgroupwarehouseRepo;
        }

        [HttpPost("")]
        public IActionResult getReport(HistoryBorrowParam filter)
        {
            var rs = _wareHouseRepo.GetAll(x => x.WarehouseCode == "HN").FirstOrDefault();
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                    "spGetHistoryBorrowSale",
                    new string[] { "@PageNumber", "@PageSize", "@DateBegin", "@DateEnd", "@ProductGroupID", "@ReturnStatus", "@FilterText", "@WareHouseID", "@EmployeeID" },
                    new object[] { filter.PageNumber, filter.PageSize, filter.DateStart, filter.DateEnd, filter.ProductGroupID, filter.Status - 1, filter.FilterText, filter.WareHouseID, filter.EmployeeID }
                    );
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(result, 0)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    error = ex.Message
                });
            }
        }

        //đã, hủy trả
        [HttpPost("approved-returned")]

        public async Task<IActionResult> ApprovedReturned([FromBody] List<int> billExportDetailIds, bool isApproved)
        {
            try
            {
                string message = isApproved ? "đã trả" : "hủy trả";
                foreach (var id in billExportDetailIds)
                {
                    if (id <= 0)
                    {
                        return BadRequest(new
                        {
                            status = 0,
                            message = $"ID không hợp lệ: {id}"
                        });
                    }

                    var result = _billExportDetailRepo.GetByID(id);
                    if (result == null)
                    {
                        return BadRequest(new
                        {
                            status = 0,
                            message = $"Không tìm thấy bản ghi với ID: {id}"
                        });
                    }

                    result.ReturnedStatus = isApproved;
                    result.UpdatedDate = DateTime.Now;
                    _billExportDetailRepo.Update(result);
                }

                return Ok(new
                {
                    status = 1,
                    message = $"{(isApproved ? "Đã trả!" : "Đã hủy trả")}"
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
        [HttpGet("get-summary-return")]
        public IActionResult GetSummaryReturn(int exportID)
        {
            try
            {
                var dt = SQLHelper<dynamic>.ProcedureToList(
               "spGetReturnDetailSummaryByExportDetailID",
               new[] { "@ExportDetailID" },
               new object[] { exportID }
           );

                var data = SQLHelper<dynamic>.GetListData(dt, 0);

                int index = 1;
                foreach (var item in data)
                {
                    item.No = index++;
                }

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-user")]
        public IActionResult GetUser()
        {
            var list = _userRepo.GetAll(x => x.Status == 0);
            return Ok(ApiResponseFactory.Success(list, ""));
        }

        [HttpGet("product-group-warehouse")]
        public IActionResult GetProductGroupWarehouse()
        {
            try
            {
                var list = _productgroupwarehouseRepo.GetAll();
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #region Gia hạn mượn
        [HttpPost("extend-product")]
        public async Task<IActionResult> ExtendProduct([FromBody] List<int> billExportDetailIds, DateTime extendDate)
        {
            try
            {
                if (billExportDetailIds.Count() <= 0) return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                foreach (var id in billExportDetailIds)
                {
                    if (id <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, $"ID không hợp lệ: {id}"));
                    }

                    //var checkLog = _historyBorrowSaleLogRepo.GetAll(x => x.BillExportDetailID == id).OrderByDescending(x => x.CreatedDate).FirstOrDefault();

                    HistoryBorrowSaleLog log = new HistoryBorrowSaleLog();
                    log.BillExportDetailID = id;
                    log.ExtendDate = DateTime.Now;
                    log.ExpectedReturnDate = extendDate;
                    log.IsApproved = false;
                    log.CreatedDate = DateTime.Now;
                    log.CreatedBy = currentUser.LoginName;
                    log.UpdatedDate = DateTime.Now;
                    log.UpdatedBy = currentUser.LoginName;
                    await _historyBorrowSaleLogRepo.CreateAsync(log);


                    //BillExportDetail model = _billExportDetailRepo.GetByID(id);
                    //model.ExpectReturnDate = extendDate;
                    //await _billExportDetailRepo.UpdateAsync(model);

                }
                return Ok(ApiResponseFactory.Success(null, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion

        #region Duyệt gia hạn
        [HttpPost("approved-extend-product")]
        public async Task<IActionResult> ApprovedExtendProduct([FromBody] List<int> billExportDetailIds, bool isApproved)
        {
            try
            {
                if (billExportDetailIds.Count() <= 0) return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                if (isApproved)
                {
                    foreach (var id in billExportDetailIds)
                    {
                        if (id <= 0)
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, $"ID không hợp lệ: {id}"));
                        }

                        var log = _historyBorrowSaleLogRepo.GetAll(x => x.BillExportDetailID == id).OrderByDescending(x => x.CreatedDate).FirstOrDefault();

                        if ((bool)log.IsApproved || log == null) continue;

                        log.IsApproved = isApproved;
                        log.UpdatedDate = DateTime.Now;
                        log.UpdatedBy = currentUser.LoginName;
                        await _historyBorrowSaleLogRepo.UpdateAsync(log);


                        BillExportDetail model = _billExportDetailRepo.GetByID(id);
                        model.ExpectReturnDate = log.ExpectedReturnDate;
                        await _billExportDetailRepo.UpdateAsync(model);

                    }
                }

                return Ok(ApiResponseFactory.Success(null, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion

        #region Lấy số lượng đang mượn quá hạn/tới hạn
        [HttpGet("quantity-borrow-sale-persional")]
        public async Task<IActionResult> getQuantityBorrowSalePersional()
        {
            DateTime dateEnd = DateTime.Now.AddDays(7).AddSeconds(-1);
            var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
            var currentUser = ObjectMapper.GetCurrentUser(claims);
            try
            {

                var param = new
                {
                    PageNumber = 1,
                    PageSize = 999999,
                    DateBegin = TextUtils.MinDate,
                    DateEnd = dateEnd,
                    ProductGroupID = 0,
                    ReturnStatus = -1,
                    FilterText = "",
                    WareHouseID = -1,
                    EmployeeID = currentUser.ID
                };

                var result = await SqlDapper<dynamic>.ProcedureToListAsync("spGetHistoryBorrowSale", param);
                var dtMaster = ((IEnumerable<dynamic>)result).ToList();

                int quantityBorrowSale = 0;
                int quantityBorrowExpriedSale = 0;

                if (dtMaster.Count() > 0)
                {
                    quantityBorrowSale = dtMaster.Where(x => x.DualDate == 1).Count();
                    quantityBorrowExpriedSale = dtMaster.Where(x => x.DualDate == 2).Count();
                }


                var dataRs = new
                {
                    quantityBorrowSale = quantityBorrowSale,
                    quantityBorrowExpriedSale = quantityBorrowExpriedSale
                };

                return Ok(ApiResponseFactory.Success(dataRs, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
    }
}
