using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Technical;
using System.Threading.Tasks;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static RERPAPI.Controllers.BorrowController;

namespace RERPAPI.Controllers.HRM.ProductProtectiveGear
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryProductRtcProtectiveGearController : ControllerBase
    {
        private readonly HistoryProductRTCRepo _historyProductRTCRepo;
        private readonly vUserGroupLinksRepo _vUserGroupLinksRepo;
        private readonly HistoryProductRTCLogRepo _historyProductRTCLogRepo;
        private readonly ProductRTCRepo _productRTCRepo;
        private readonly ProductLocationRepo _productLocationRepo;
        public HistoryProductRtcProtectiveGearController(HistoryProductRTCRepo historyProductRTCRepo, vUserGroupLinksRepo vUserGroupLinksRepo, HistoryProductRTCLogRepo historyProductRTCLogRepo, ProductRTCRepo productRTCRepo, ProductLocationRepo productLocationRepo)
        {
            _historyProductRTCRepo = historyProductRTCRepo;
            _vUserGroupLinksRepo = vUserGroupLinksRepo;
            _historyProductRTCLogRepo = historyProductRTCLogRepo;
            _productRTCRepo = productRTCRepo;
            _productLocationRepo = productLocationRepo;
        }
        [HttpGet("get-product-history")]
        public async Task<IActionResult> GetProductHistory(DateTime dateStart, DateTime dateEnd, string? keyWords, int warehouseID, int userID, string status, int page, int size, int isDeleted)
        {
            try
            {
                DateTime ds = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 0).AddSeconds(-1);
                DateTime de = new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 23, 59, 59).AddSeconds(+1);

                var data = SQLHelper<object>.ProcedureToList("spGetHistoryProduct_New",
                    new string[] { "@DateStart", "@DateEnd", "@Keyword", "@Status", "@WarehouseID", "@PageNumber", "@PageSize", "@UserID", "@IsDeleted" },
                    new object[] { ds, de, keyWords, status, warehouseID, page, size, userID, isDeleted });
                var dt = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(dt, ""));
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
        [HttpPost("save-extend-product")]
        public async Task<IActionResult> SaveExtend(HistoryProductRTC item)
        {
            try
            {

                // if (item.ID <= 0)
                //return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy lịch sử mượn trong database!"));
                var hcnsLst = _vUserGroupLinksRepo.GetAll(c => c.Code == "N34");
                var hcnsIDs = hcnsLst.Select(p => p.UserID);
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var isAdmin = hcnsIDs.Contains(currentUser.ID) || currentUser.IsAdmin;
                if (isAdmin)
                {
                    item.Status = 1;

                    item.AdminConfirm = true;
                }
                else
                {
                    item.Status = 8;
                }
                _historyProductRTCRepo.Update(item);
                // lưu log 
                HistoryProductRTCLog logModel = new HistoryProductRTCLog();
                logModel.HistoryProductRTCID = item.ID;
                logModel.DateReturnExpected = item.DateReturnExpected;
                var result = await _historyProductRTCLogRepo.CreateAsync(logModel);

                if (result <= 0)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Không thể lưu History Product RTC Log!"));
                }
                return Ok(ApiResponseFactory.Success(item, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("approve-borrowing")]
        public IActionResult ApproveBorrowing([FromBody] ReturnProductRtcRequest req)
        {
            var hcnsLst = _vUserGroupLinksRepo.GetAll(c => c.Code == "N34");
            var hcnsIDs = hcnsLst.Select(p => p.UserID);
            var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
            var currentUser = ObjectMapper.GetCurrentUser(claims);
            var isAdmin = hcnsIDs.Contains(currentUser.ID) || currentUser.IsAdmin;
            if (req == null || isAdmin != true)
                return BadRequest(new { status = 0, message = "Chỉ có admin mới có quyền truy cập vào chức năng này!" });

            if (req.HistoryId <= 0)
                return BadRequest(new { status = 0, message = "Dữ liệu truyền vào không đúng, vui lòng thử lại!" });

            var history = _historyProductRTCRepo.GetByID(req.HistoryId);
            if (history == null)
                return BadRequest(new { status = 0, message = "Không tìm thấy lịch sử mượn!" });

            try
            {
                if (isAdmin && (history.Status == 7 || history.Status == 8))
                {
                    history.Status = 1;
                    _historyProductRTCRepo.Update(history);
                    return Ok(new { status = 1, data = new { id = history.ID } });
                }

                return Ok(new { status = 1, data = new { id = 0 } });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("return-productrtc")]
        public IActionResult ReturnProductRtc([FromBody] ReturnProductRtcRequest req)
        {
            var hcnsLst = _vUserGroupLinksRepo.GetAll(c => c.Code == "N34");
            var hcnsIDs = hcnsLst.Select(p => p.UserID);
            var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
            var currentUser = ObjectMapper.GetCurrentUser(claims);
            var isAdmin = hcnsIDs.Contains(currentUser.ID) || currentUser.IsAdmin;

            if (req == null)
                return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu truyền vào!"));

            if (req.HistoryId <= 0)
                return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu truyền vào!"));

            var history = _historyProductRTCRepo.GetByID(req.HistoryId);
            if (history == null)
                return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy lịch sử mượn!"));

            // Status phải được phép trả
            if (history.Status != 1 && history.Status != 4 && history.Status != 7)
                return BadRequest(ApiResponseFactory.Fail(null, "Trạng thái hiện tại không cho phép trả sản phẩm!"));

            // Nếu là admin → cần check thêm điều kiện như WinForms
            if (isAdmin)
            {
                if (req.ModulaLocationDetailID > 0 && history.StatusPerson <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null,
                        "Nhân viên chưa hoàn thành thao tác trả hàng. Bạn không thể duyệt trả!"));
                }
            }
            try
            {
                if (isAdmin)
                {
                    // Admin duyệt trả
                    history.Status = 0;
                    history.DateReturn = DateTime.Now;
                    history.AdminConfirm = true;
                    _historyProductRTCRepo.Update(history);

                    // Update QR
                    SQLHelper<object>.ExcuteProcedure(
                        "spUpdateStatusProductRTCQRCode",
                        new[] { "@ProductRTCQRCodeID", "@Status" },
                        new object[] { history.ProductRTCQRCodeID, 1 }
                    );
                }
                else
                {
                    // USER TRẢ → Status = 4
                    history.Status = 4;
                    _historyProductRTCRepo.Update(history);
                }

                return Ok(new { status = 1, data = new { id = history.ID } });
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

                    if (await _historyProductRTCRepo.CreateAsync(item) > 0)
                    {
                        return Ok(ApiResponseFactory.Success(item, "Thêm mới thành công"));
                    }
                }
                else
                {

                    if (await _historyProductRTCRepo.UpdateAsync(item) > 0)
                    {
                        return Ok(ApiResponseFactory.Success(item, "Cập nhật thành công"));
                    }

                }
                return Ok(ApiResponseFactory.Fail(null, "Cập nhật thất bại"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("save-update-status-product-rtc")]
        public async Task<IActionResult> SaveUpdatsStatusProductRTC(int id, int status)
        {
            try
            {
                var product = _productRTCRepo.GetByID(id);
                if (product.ID > 0)
                {
                    product.Status = status;
                    if (_productRTCRepo.Update(product) > 0)
                    {
                        return Ok(ApiResponseFactory.Success(product, "Cập nhật thành công"));
                    }
                }

                return Ok(ApiResponseFactory.Fail(null, "Cập nhật thất bại"));
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

                var data = _historyProductRTCRepo.GetByID(productHistoryID);
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
        [HttpGet("get-user-history-product")]
        public async Task<IActionResult> GetUserHistoryProduct(int userId, int? status)
        {
            try
            {
                var hcnsLst = _vUserGroupLinksRepo.GetAll(c => c.Code == "N34");
                var hcnsIDs = hcnsLst.Select(p => p.UserID);
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var isAdmin = hcnsIDs.Contains(currentUser.ID) || currentUser.IsAdmin;
                if (isAdmin)
                {
                    userId = 0;
                }
                else
                {
                    userId = currentUser.ID;
                }
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

        [HttpGet("get-productrtc-detail")]
        public async Task<IActionResult> GetProductrtcDetail(int ProductGroupID, string? Keyword, string? Filter, int WarehouseID)
        {
            try
            {
                DateTime date = new DateTime(2024, 09, 01);
                DateTime dateStart = date;
                DateTime dateEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

                string keyword = "";
                string status = "1,2,3,4,5,6,7,8";
                int employeeID = 0;
                int isDeleted = 0;
                int productGroupRTCID = 140;
                int warehouseID = 5;
                var data = SQLHelper<object>.ProcedureToList("spGetHistoryProductRTCProtectiveGear",
                           new string[] { "@DateStart", "@DateEnd", "@EmployeeID", "@Status", "@IsDeleted", "@WarehouseID", "@ProductGroupRTCID", "@Keyword" },
                           new object[] { dateStart, dateEnd, employeeID, status, isDeleted, warehouseID, productGroupRTCID, keyword });
                var dt = SQLHelper<object>.GetListData(data, 1);

                var data0 = dt.FindAll(c => c.ProductGroupRTCID != 140);
                return Ok(ApiResponseFactory.Success(data0, ""))
                ;

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-history-productrtc")]
        public async Task<IActionResult> SaveHistoryProductrtc(HistoryProductRTC item)
        {
            try
            {
                var hcnsLst = _vUserGroupLinksRepo.GetAll(c => c.Code == "N34");
                var hcnsIDs = hcnsLst.Select(p => p.UserID);
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var isAdmin = hcnsIDs.Contains(currentUser.ID) || currentUser.IsAdmin;

                if (item.ID <= 0)
                {

                    if (isAdmin)
                    {
                        item.Status = 1;
                    }

                    var result = await _historyProductRTCRepo.CreateAsync(item);
                    if (result <= 0)
                    {
                        return Ok(ApiResponseFactory.Fail(null, "Có lỗi xảy ra khi thêm mới HistoryProductRTC"));
                    }
                }
                else
                {
                    if (isAdmin)
                    {
                        item.Status = 1;
                    }
                    var result = await _historyProductRTCRepo.UpdateAsync(item);
                    if (result <= 0)
                    {
                        return Ok(ApiResponseFactory.Fail(null, "Có lỗi xảy ra khi cập nhật HistoryProductRTC"));
                    }

                }
                return Ok(ApiResponseFactory.Success(item, ""));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteHistoryProduct(List<int> items)
        {
            try
            {
                foreach (var item in items)
                {
                    var history = _historyProductRTCRepo.GetByID(item);
                    if (history.ID <= 0) return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy lịch sử mượn"));
                    history.IsDelete = true;
                    await _historyProductRTCRepo.UpdateAsync(history);
                }
                return Ok(ApiResponseFactory.Success(items, ""));
            }

            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-coordinates")]
        public async Task<IActionResult> SaveCoordinates([FromBody] List<ProductLocationUpdateDTO> items)
        {
            try
            {
                if (items == null)
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Không có dữ liệu để lưu"
                    });
                }
                foreach (var item in items)
                {
                   
                    int ProductLocationID = item.ProductLocationID;
                    int LocationType = item.LocationType;
                    int CoordinatesX = item.CoordinatesX;
                    int CoordinatesY = item.CoordinatesY;


                    // Tìm ProductLocation theo ProductRTCID và LocationType
                    var entity = _productLocationRepo.GetByID(ProductLocationID);
                    if (entity != null)
                    {
                        // Update coordinates
                        entity.CoordinatesX = CoordinatesX;
                        entity.CoordinatesY = CoordinatesY;
                      await _productLocationRepo.UpdateAsync(entity);

                    }
                }
                return Ok(ApiResponseFactory.Success(null, "Cập nhật vị trí thành công!"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 0,
                    message = "Lỗi khi lưu vị trí: " + ex.Message
                });
            }
        }

    }
    // DTO
    public class ProductLocationUpdateDTO
    {
        public int ProductLocationID { get; set; }  // ← Primary key!
        public int LocationType { get; set; }
        public int CoordinatesX { get; set; }
        public int CoordinatesY { get; set; }
    }
}
