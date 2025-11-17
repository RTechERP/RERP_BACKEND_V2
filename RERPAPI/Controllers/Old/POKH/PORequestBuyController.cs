using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers.Old.POKH
{
    [Route("api/[controller]")]
    [ApiController]
    public class PORequestBuyController : ControllerBase
    {
        private readonly ProjectPartlistPurchaseRequestRepo _PPPRRepo;
        private readonly UnitCountRepo _unitCountRepo;

        public PORequestBuyController(ProjectPartlistPurchaseRequestRepo pPPRRepo, UnitCountRepo unitCountRepo)
        {
            _PPPRRepo = pPPRRepo;
            _unitCountRepo = unitCountRepo;
        }

        [HttpGet("get-pokh-product")]
        public IActionResult getPOKHProduct(int id, int idDetail)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetPOKHDetail", new string[] { "@ID", "@IDDetail" }, new object[] { id, idDetail });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //[HttpPost("save-data")]
        //public async Task<IActionResult> Save([FromBody] List<ProjectPartlistPurchaseRequest> request)
        //{
        //    try
        //    {
        //        var results = new List<object>();

        //        foreach (var item in request)
        //        {
        //            var model = new ProjectPartlistPurchaseRequest
        //            {
        //                ProjectPartListID = 0, // Chưa có thông tin về ProjectPartListID, cần xác định cách lấy giá trị này

        //                EmployeeID = item.EmployeeID,
        //                ProductCode = item.ProductCode,
        //                ProductName = item.ProductName,
        //                StatusRequest = 1, // yêu cầu mua hàng
        //                DateRequest = item.DateRequest,
        //                DateReturnExpected = item.DateReturnExpected,
        //                DateReceive = item.DateReceive,
        //                Quantity = item.Quantity,
        //                CurrencyRate = item.CurrencyRate,
        //                //UnitPrice = item.UnitPrice,
        //                TotalPrice = item.TotalPrice,
        //                VAT = item.VAT,
        //                Note = item.Note,
        //                ProductSaleID = item.ProductSaleID,
        //                ProductGroupID = item.ProductGroupID,
        //                CurrencyID = item.CurrencyID,
        //                IsCommercialProduct = true,
        //                POKHDetailID = item.POKHDetailID,
        //                IsApprovedTBP = false,
        //                ApprovedTBP = 0,
        //                IsApprovedBGD = false,
        //                ApprovedBGD = 0,
        //            };

        //            // Lấy UnitCountID từ UnitName
        //            var unitCount = GetUnitCountByName(item.UnitName);
        //            if (unitCount != null)
        //            {
        //                model.UnitCountID = unitCount.ID;
        //            }

        //            // Tính tiền quy đổi Việt Nam
        //            model.TotalPriceExchange = item.CurrencyRate <= 0 ? 0 : item.TotalPrice * item.CurrencyRate;

        //            // Tính thành tiền có VAT
        //            decimal? totalMoneyVAT_New = item.TotalPrice + item.TotalPrice * item.VAT / 100;
        //            model.TotaMoneyVAT = item.TotaMoneyVAT == totalMoneyVAT_New ?
        //                item.TotaMoneyVAT : totalMoneyVAT_New;

        //            // Tính tổng số ngày lead time
        //            if (item.DateReturnExpected.HasValue && item.DateRequest.HasValue)
        //            {
        //                int totalDays = (item.DateReturnExpected.Value - item.DateRequest.Value).Days;
        //                model.TotalDayLeadTime = totalDays;
        //            }

        //            // Lưu vào database
        //            var insertResult = await _PPPRRepo.CreateAsync(model);
        //            results.Add(new { item.ProductCode, Success = insertResult > 0 });
        //        }

        //        return Ok(ApiResponseFactory.Success(results, "Purchase requests saved successfully"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}
        [HttpPost("save-data")]
        public async Task<IActionResult> Save([FromBody] List<ProjectPartlistPurchaseRequest> request)
        {
            try
            {
                if (request == null || request.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null,"Dữ liệu gửi lên rỗng"));

                if (!_PPPRRepo.Validate(request, out string validateMessage))
                    return BadRequest(ApiResponseFactory.Fail(null,validateMessage));

                var results = new List<object>();

                foreach (var item in request)
                {
                    // desktop lấy quantityRequest từ cột colQuantityRequestRemain
                    decimal quantityRequest = item.Quantity ?? 0m;
                    if (quantityRequest <= 0m) continue; // desktop bỏ qua khi <= 0

                    var model = new ProjectPartlistPurchaseRequest
                    {
                        ProjectPartListID = 0,
                        EmployeeID = item.EmployeeID,
                        ProductCode = item.ProductCode,
                        ProductName = item.ProductName,
                        StatusRequest = 1, // yêu cầu mua hàng
                        DateRequest = item.DateRequest,
                        DateReturnExpected = item.DateReturnExpected,
                        DateReceive = item.DateReceive,
                        Quantity = quantityRequest,
                        CurrencyRate = item.CurrencyRate,
                        TotalPrice = item.TotalPrice,
                        VAT = item.VAT,
                        Note = item.Note,
                        ProductSaleID = item.ProductSaleID,
                        ProductGroupID = item.ProductGroupID,
                        CurrencyID = item.CurrencyID,
                        IsCommercialProduct = true,
                        POKHDetailID = item.POKHDetailID,
                        IsApprovedTBP = false,
                        ApprovedTBP = 0,
                        IsApprovedBGD = false,
                        ApprovedBGD = 0,
                        ProjectPartlistPurchaseRequestTypeID = 5, // yc mua thương mại
                        ParentProductCode = item.ParentProductCode,
                        CreatedDate = DateTime.Now
                    };

                    // Lấy UnitCountID từ UnitName (desktop tra bằng tên)
                    var unitCount = GetUnitCountByName(item.UnitName);
                    if (unitCount != null)
                    {
                        model.UnitCountID = unitCount.ID;
                    }

                    // Tính tiền quy đổi VN
                    decimal currencyRate = item.CurrencyRate ?? 0m;
                    decimal totalPrice = item.TotalPrice ?? 0m;
                    model.TotalPriceExchange = currencyRate <= 0m ? 0m : (totalPrice * currencyRate);

                    // Tính thành tiền có VAT (so sánh giống desktop)
                    decimal vat = item.VAT ?? 0m;
                    decimal totalMoneyVAT_New = totalPrice + ((totalPrice * vat) / 100m);
                    if (item.TotaMoneyVAT.HasValue && item.TotaMoneyVAT.Value == totalMoneyVAT_New)
                        model.TotaMoneyVAT = item.TotaMoneyVAT;
                    else
                        model.TotaMoneyVAT = totalMoneyVAT_New;

                    // Tính tổng số ngày lead time
                    if (item.DateReturnExpected.HasValue && item.DateRequest.HasValue)
                    {
                        int totalDays = (item.DateReturnExpected.Value.Date - item.DateRequest.Value.Date).Days;
                        model.TotalDayLeadTime = totalDays;
                    }

                    // Lưu vào DB
                    var insertResult = await _PPPRRepo.CreateAsync(model);
                    results.Add(new { ProductCode = item.ProductCode, Success = insertResult > 0 });
                }

                return Ok(ApiResponseFactory.Success(results, "Lưu yêu cầu mua hàng thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        private UnitCount GetUnitCountByName(string unitName)
        {
            try
            {
                UnitCount? unitCount = _unitCountRepo.GetAll().FirstOrDefault(x => x.UnitName == unitName.Trim());
                return unitCount;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
