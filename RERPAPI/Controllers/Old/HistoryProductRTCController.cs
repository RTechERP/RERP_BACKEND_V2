using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Technical;
using ZXing.QrCode.Internal;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HistoryProductRTCController : ControllerBase
    {
        private const int WAREHOUSE_ID = 1;
        private readonly HistoryProductRTCRepo _historyRepo;

        public HistoryProductRTCController(HistoryProductRTCRepo historyRepo)
        {
            _historyRepo = historyRepo;
        }

        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
                //currentUser.ID = 1533;

                DateTime dateStart = new DateTime(1900, 01, 01);
                DateTime dateEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                string status = "4,7";
                var historys = SQLHelper<object>.ProcedureToList("spGetHistoryProduct_New",
                                new string[] { "@DateStart", "@DateEnd", "@Status", "@WarehouseID", "@UserID" },
                                new object[] { dateStart, dateEnd, status, WAREHOUSE_ID, currentUser.ID });

                var data = SQLHelper<object>.GetListData(historys, 0);
                var borrows = data.Where(x => x.Status == 7 && x.ModulaLocationDetailID > 0).ToList();
                var returns = data.Where(x => x.Status == 4 && x.ModulaLocationDetailID > 0).ToList();
                //var a = data.Where(x => x.Status == 1 && x.ModulaLocationDetailID > 0).ToList();
                return Ok(ApiResponseFactory.Success(new { borrows, returns }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] List<HistoryProductRTC> historyProducts)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
                //_historyRepo.SetClaim(claims);


                foreach (var item in historyProducts)
                {
                    if (item.ID <= 0)
                    {
                        var historys = _historyRepo.GetAll(x => x.Status != 0 &&
                                                                x.ProductRTCQRCodeID == item.ProductRTCQRCodeID &&
                                                                x.IsDelete == false);
                        if (historys.Count > 0)
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, $"Sản phẩm có QR code [{item.ProductRTCQRCode}] đang được mượn.\n" +
                                                                            $"Bạn không thể đăng ký mượn.", historys));
                        }
                    }
                }

                foreach (var item in historyProducts)
                {
                    if (item.ID > 0)
                    {
                        if (currentUser.ID != item.PeopleID) continue;

                        item.UpdatedBy = currentUser.LoginName;
                        await _historyRepo.UpdateAsync(item);
                    }
                    else
                    {
                        item.PeopleID = currentUser.ID;
                        item.WarehouseID = WAREHOUSE_ID;
                        item.Status = 7;
                        item.AdminConfirm = false;
                        item.CreatedBy = item.UpdatedBy = currentUser.LoginName;

                        await _historyRepo.CreateAsync(item);
                    }
                }

                return Ok(ApiResponseFactory.Success(historyProducts, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #region Mượn trả phiếu bằng qrcode
        [HttpGet("user-product-qr")]
        public IActionResult getUserProductQR(int userId)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetUsersHistoryProductRTC",
                                new string[] { "@UsersID" },
                                new object[] { userId });
                var dt = SQLHelper<dynamic>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(dt, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("product-qr")]
        public IActionResult ProductQR()
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetProductQrCode",
                                new string[] { },
                                new object[] { });
                var dt = SQLHelper<dynamic>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(dt, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("product-rtc-by-qr")]
        public IActionResult ProductRTCByQR(string qrCode, int warehouseID)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetProductQrCode",
                                new string[] { "@ProductRTCQRCode", "@WarehouseID" },
                                new object[] { qrCode, warehouseID });
                var dt = SQLHelper<dynamic>.GetListData(data, 0);
                var dt1 = SQLHelper<dynamic>.GetListData(data, 1);

                var result = new
                {
                    data = dt,
                    data1 = dt1
                };
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("load-qr-code")]
        public IActionResult LoadQRCode(string qrCode, int warehouseID)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetProductRTCByQrCode",
                                new string[] { "@ProductRTCQRCode", "@WarehouseID" },
                                new object[] { qrCode, warehouseID });
                var dt = SQLHelper<dynamic>.GetListData(data, 0);
                var dt1 = SQLHelper<dynamic>.GetListData(data, 1);
                if (dt.Count() > 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Sản phẩm đã được mượn.\nVui lòng báo với admin."));
                }

                if (dt1.Count() <= 0) return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy sản phẩm cần mượn.\nVui lòng báo với admin."));

                return Ok(ApiResponseFactory.Success(dt1, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("load-qr-code-return")]
        public IActionResult LoadQRCodeReturn(string qrCode, int userId, int warehouseID)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetProductRTCByQrCode_Return",
                                new string[] { "@ProductRTCQRCode", "@PeopleID", "@WarehouseID" },
                                new object[] { qrCode, userId, warehouseID });
                var dt = SQLHelper<dynamic>.GetListData(data, 0);
                if (dt.Count() <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy mã {qrCode} đang mượn!"));
                }

                return Ok(ApiResponseFactory.Success(dt, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data-product-qr")]
        public async Task<IActionResult> SaveDataProductRTC([FromBody] List<HistoryProductRTC> historyProducts)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
                if (historyProducts.Count() <= 0) return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu rỗng"));

                foreach (var model in historyProducts)
                {
                    if (model.ID > 0)
                    {
                        await _historyRepo.UpdateAsync(model);
                    }
                    else
                    {
                        await _historyRepo.CreateAsync(model);
                        var exec = SQLHelper<object>.ProcedureToList("spUpdateStatusProductRTCQRCode",
                                    new string[] { "@ProductRTCQRCodeID", "@Status" },
                                    new object[] { model.ProductRTCQRCodeID, 2 });
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

    }
}