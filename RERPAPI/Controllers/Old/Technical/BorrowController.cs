using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Technical;
using RTCApi.Repo.GenericRepo;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowController : ControllerBase
    {
        HistoryProductRTCRepo historyProductRTCRepo;
        HistoryErrorRepo historyErrorRepo;
        HistoryProductRTCLogRepo historyProductRTCLogRepo;
        BillExportDetailTechnicalRepo billExportDetailTechnicalRepo;
        BillExportTechnicalRepo billExportTechnicalRepo;
        public BorrowController(HistoryProductRTCRepo historyProductRTCRepo, HistoryErrorRepo historyErrorRepo, HistoryProductRTCLogRepo historyProductRTCLogRepo, BillExportDetailTechnicalRepo billExportDetailTechnicalRepo, BillExportTechnicalRepo billExportTechnicalRepo)
        {
            this.historyProductRTCRepo = historyProductRTCRepo;
            this.historyErrorRepo = historyErrorRepo;
            this.billExportTechnicalRepo = billExportTechnicalRepo;
            this.billExportDetailTechnicalRepo = billExportDetailTechnicalRepo;
            this.historyProductRTCLogRepo = historyProductRTCLogRepo;
        }

        [HttpGet("get-product-history")]
        public async Task<IActionResult> GetProductHistory(DateTime dateStart, DateTime dateEnd, string? keyWords, int warehouseID, int userID, string status, int page, int size, int isDeleted, int warehouseType)
        {
            try
            {

                var productHistory = SQLHelper<object>.ProcedureToList("spGetHistoryProduct_New", 
                    new string[] { "@DateStart", "@DateEnd", "@Keyword", "@WarehouseID", "@UserID", "@Status", "@PageNumber", "@PageSize", "@IsDeleted", "@WarehouseType" }, 
                    new object[] { dateStart, dateEnd, keyWords ?? "", warehouseID, userID, status, page, size, isDeleted, warehouseType });
                var data = SQLHelper<object>.GetListData(productHistory, 0);

                return Ok(ApiResponseFactory.Success(data, ""));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-user-history-product")]
        public async Task<IActionResult> GetUserHistoryProduct(int userId, int? status)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetUsersHistoryProductRTC", new string[] { "@UsersID", "@Status" }, new object[] { userId, status });
                var dt = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(dt, ""))
                ;

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-history-product-borrow-detail")]
        public async Task<IActionResult> GetHistoryProductBorrowDetail(int historyId)
        {
            try
            {
                if (historyId <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có tìm thấy lịch sử mượn có ID: " + historyId));
                }

                var historyProductBorrowDetail = SQLHelper<object>.ProcedureToList(
                    "spGetHistoryProductBorrowDetail",
                    new string[] { "@HistoryId", "@WarehouseId" },
                    new object[] { historyId, 1 }
                );

                return Ok(ApiResponseFactory.Success(historyProductBorrowDetail, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpGet("get-productrtc-detail")]
        public async Task<IActionResult> GetProductrtcDetail(int ProductGroupID, string? Keyword, int CheckAll, string? Filter, int WarehouseID, int WarehouseType)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetProductRTC_Detail", 
                    new string[] { "@ProductGroupID", "@Keyword", "@CheckAll", "@Filter", "@WarehouseID", "@WarehouseType" }, 
                    new object[] { ProductGroupID, Keyword ?? "", CheckAll, Filter ?? "", WarehouseID, WarehouseType });
                var dt = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(dt, ""))
                ;

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpGet("get-employee-team-and-department")]
        public async Task<IActionResult> GetEmployeeTeamAndDepartment()
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetEmployeeTeamAndDepartment", new string[] { }, new object[] { });
                var dt = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(dt, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-personal-history-error")]
        public IActionResult GetPersonalHistoryError(int Id)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetHistoryErrorByID", new string[] { @"UserID" }, new object[] { Id });
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(data, 0)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-history-error")]
        public IActionResult GetHistoryError(int productHistoryID)
        {
            try
            {
                var data = historyErrorRepo.GetAll(c => c.ProductHistoryID == productHistoryID);
                if (data.Count != 0)
                {
                    return Ok(new
                    {
                        status = 1,
                        data = data.FirstOrDefault()
                    });
                }
                return Ok(new
                {
                    status = 1,
                    data = new HistoryError()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-history-product-rtc-by-id")]
        public async Task<IActionResult> GetHistoryProductrtcById(int productHistoryID)
        {
            try
            {
                var data = historyProductRTCRepo.GetByID(productHistoryID) ?? new HistoryProductRTC();
                return Ok(new
                {
                    status = 1,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-history-productrtc-log")]
        public IActionResult GetHistoryProductrtcLog(int historyID)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetHistoryProductRTCLog", new string[] { "@HistoryProductRTCID" }, new object[] { historyID });
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(data, 0)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpGet("get-bill-number")]
        public IActionResult GetBillNumber(int billType)
        {
            try
            {
                var billNumber = billExportTechnicalRepo.GetBillCode(billType);

                return Ok(new
                {
                    status = 1,
                    data = billNumber
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("save-history-productrtc")]
        public async Task<IActionResult> SaveHistoryProductrtc(ModulaLocationDTO.SerialNumberModulaLocation item)
        {
            try
            {
                await historyProductRTCRepo.SaveDataAsync(item);
                return Ok(ApiResponseFactory.Success(item, ""));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-history-product")]
        public async Task<IActionResult> SaveHistoryProduct(HistoryProductRTC item)
        {
            try
            {
                if (item.ID <= 0)
                {
                    await historyProductRTCRepo.CreateAsync(item);
                }
                else
                {
                    historyProductRTCRepo.Update(item);

                }
                return Ok(ApiResponseFactory.Success(item, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-bill-export-technical")]
        public async Task<IActionResult> SaveBillExportTechnical(BillExportTechnical item)
        {
            try
            {
                if (item.ID <= 0)
                {
                    await billExportTechnicalRepo.CreateAsync(item);
                }
                else
                {
                    billExportTechnicalRepo.Update(item);

                }
                return Ok(ApiResponseFactory.Success(item, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-bill-export-detail-technical")]
        public async Task<IActionResult> SaveBillExportTechnical(BillExportDetailTechnical item)
        {
            try
            {
                if (item.ID <= 0)
                {
                    await billExportDetailTechnicalRepo.CreateAsync(item);
                }
                else
                {
                    billExportDetailTechnicalRepo.Update(item);

                }
                return Ok(ApiResponseFactory.Success(item, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-history-product-log")]
        public async Task<IActionResult> SaveHistoryProductLog(HistoryProductRTCLog item)
        {
            try
            {
                if (item.ID <= 0)
                {
                    await historyProductRTCLogRepo.CreateAsync(item);
                }
                else
                {
                    historyProductRTCLogRepo.Update(item);

                }
                return Ok(new
                {
                    status = 1,
                    data = ""
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        public sealed class ReturnProductRtcRequest
        {
            public int HistoryId { get; set; }
            public bool IsAdmin { get; set; }
        }

        [HttpPost("return-productrtc")]
        public IActionResult ReturnProductRtc([FromBody] ReturnProductRtcRequest req)
        {
            if (req == null || req.IsAdmin != true)
                return BadRequest(ApiResponseFactory.Fail(null, "Chỉ có admin mới có quyền truy cập vào chức năng này!"));

            if (req.HistoryId <= 0)
                return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu truyền vào không đúng, vui lòng thử lại!"));

            var history = historyProductRTCRepo.GetByID(req.HistoryId);
            if (history == null)
                return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy lịch sử mượn!"));

            try
            {
                if (req.IsAdmin)
                {
                    history.Status = 0;
                    history.DateReturn = DateTime.Now;
                    history.AdminConfirm = true;
                    historyProductRTCRepo.Update(history);

                    // Cập nhật trạng thái QR
                    SQLHelper<object>.ExcuteProcedure(
                        "spUpdateStatusProductRTCQRCode",
                        new[] { "@ProductRTCQRCodeID", "@Status" },
                        new object[] { history.ProductRTCQRCodeID, 1 }
                    );
                }
                else
                {
                    history.Status = 4;
                    historyProductRTCRepo.Update(history);
                }

                return Ok(new { status = 1, data = new { id = history.ID } });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("approve-borrowing")]
        public IActionResult ApproveBorrowing([FromBody] ReturnProductRtcRequest req)
        {
            if (req == null || req.IsAdmin != true)
                return BadRequest(new { status = 0, message = "Chỉ có admin mới có quyền truy cập vào chức năng này!" });

            if (req.HistoryId <= 0)
                return BadRequest(new { status = 0, message = "Dữ liệu truyền vào không đúng, vui lòng thử lại!" });

            var history = historyProductRTCRepo.GetByID(req.HistoryId);
            if (history == null)
                return BadRequest(new { status = 0, message = "Không tìm thấy lịch sử mượn!" });

            try
            {
                if (req.IsAdmin && history.Status == 7)
                {
                    history.Status = 1;
                    historyProductRTCRepo.Update(history);
                    return Ok(new { status = 1, data = new { id = history.ID } });
                }

                return Ok(new { status = 1, data = new { id = 0 } });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-history-error")]
        public async Task<IActionResult> SaveHistoryError(HistoryError historyError)
        {
            try
            {
                if (historyError.ID <= 0)
                {
                    await historyErrorRepo.CreateAsync(historyError);
                }
                else
                {
                    await historyErrorRepo.UpdateAsync(historyError);
                }
                return Ok(new
                {
                    status = 1,
                    data = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
