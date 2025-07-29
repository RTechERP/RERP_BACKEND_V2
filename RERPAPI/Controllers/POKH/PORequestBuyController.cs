using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers.PO
{
    [Route("api/[controller]")]
    [ApiController]
    public class PORequestBuyController : ControllerBase
    {
        ProjectPartlistPurchaseRequestRepo _PPPRRepo = new ProjectPartlistPurchaseRequestRepo();
        UnitCountRepo _unitCountRepo = new UnitCountRepo();
        [HttpPost("save-data")]
        public async Task<IActionResult> Save([FromBody] List<ProjectPartlistPurchaseRequest> request)
        {
            try
            {
                var results = new List<object>();

                foreach (var item in request)
                {
                    var model = new ProjectPartlistPurchaseRequest
                    {
                        ProjectPartListID = 0, // Chưa có thông tin về ProjectPartListID, cần xác định cách lấy giá trị này

                        EmployeeID = item.EmployeeID,
                        ProductCode = item.ProductCode,
                        ProductName = item.ProductName,
                        StatusRequest = 1, // yêu cầu mua hàng
                        DateRequest = item.DateRequest,
                        DateReturnExpected = item.DateReturnExpected,
                        DateReceive = item.DateReceive,
                        Quantity = item.Quantity,
                        CurrencyRate = item.CurrencyRate,
                        //UnitPrice = item.UnitPrice,
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
                    };

                    // Lấy UnitCountID từ UnitName
                    var unitCount = GetUnitCountByName(item.UnitName);
                    if (unitCount != null)
                    {
                        model.UnitCountID = unitCount.ID;
                    }

                    // Tính tiền quy đổi Việt Nam
                    model.TotalPriceExchange = item.CurrencyRate <= 0 ? 0 : item.TotalPrice * item.CurrencyRate;

                    // Tính thành tiền có VAT
                    decimal? totalMoneyVAT_New = item.TotalPrice + item.TotalPrice * item.VAT / 100;
                    model.TotaMoneyVAT = item.TotaMoneyVAT == totalMoneyVAT_New ?
                        item.TotaMoneyVAT : totalMoneyVAT_New;

                    // Tính tổng số ngày lead time
                    if (item.DateReturnExpected.HasValue && item.DateRequest.HasValue)
                    {
                        int totalDays = (item.DateReturnExpected.Value - item.DateRequest.Value).Days;
                        model.TotalDayLeadTime = totalDays;
                    }

                    // Lưu vào database
                    var insertResult = await _PPPRRepo.CreateAsync(model);
                    results.Add(new { item.ProductCode, Success = insertResult > 0 });
                }

                return Ok(new { status = 1, success = true, message = "Purchase requests saved successfully", data = results });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = "Có lỗi xảy ra: " + ex.Message,
                    error = ex.ToString()
                });
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
