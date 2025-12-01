using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.DocumentManager;
using System.Threading.Tasks;
using ZXing;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PONCCController : ControllerBase
    {
        PONCCRepo _pONCCRepo;
        ProductGroupRepo _productGroupRepo;
        ProjectRepo _projectRepo;
        PONCCDetailRepo _pONCCDetailRepo;
        PONCCRulePayRepo _pONCCRulePayRepo;
        BillImportDetailRepo _billImportDetailRepo;
        ProjectPartlistPurchaseRequestRepo _projectPartlistPurchaseRequestRepo;
        PONCCDetailRequestBuyRepo _pONCCDetailRequestBuyRepo;
        public PONCCController(
            PONCCRepo pONCCRepo
            , ProductGroupRepo productGroupRepo
            , ProjectRepo projectRepo
            , PONCCRulePayRepo pONCCRulePayRepo
            , PONCCDetailRepo pONCCDetailRepo
            , BillImportDetailRepo billImportDetailRepo
            , ProjectPartlistPurchaseRequestRepo projectPartlistPurchaseRequestRepo
            , PONCCDetailRequestBuyRepo pONCCDetailRequestBuyRepo
            )
        {
            _pONCCRepo = pONCCRepo;
            _productGroupRepo = productGroupRepo;
            _projectRepo = projectRepo;
            _pONCCRulePayRepo = pONCCRulePayRepo;
            _pONCCDetailRepo = pONCCDetailRepo;
            _billImportDetailRepo = billImportDetailRepo;
            _projectPartlistPurchaseRequestRepo = projectPartlistPurchaseRequestRepo;
            _pONCCDetailRequestBuyRepo = pONCCDetailRequestBuyRepo;
        }

        #region Lấy data master/ detail
        [HttpGet("get-all")]
        [RequiresPermission("N35,N33,N1")]
        public IActionResult GetAll(
                string? keyword = "",
                int pageNumber = 1,
                int pageSize = 20,
                DateTime? dateStart = null,
                DateTime? dateEnd = null,
                int? supplierId = null,
                int? status = null,
                int? employeeId = null,
                int? poType = 0
            )
        {
            try
            {
                DateTime _dateStart = dateStart?.Date ?? DateTime.MinValue;
                DateTime _dateEnd = (dateEnd?.Date ?? DateTime.MaxValue)
                                    .AddHours(23).AddMinutes(59).AddSeconds(59);

                // Gọi stored procedure
                var dt = SQLHelper<dynamic>.ProcedureToList(
                    "spGetPONCC_Khanh",
                    new string[]
                    {
                        "@FilterText",
                        "@PageNumber",
                        "@PageSize",
                        "@DateStart",
                        "@DateEnd",
                        "@SupplierID",
                        "@Status",
                        "@EmployeeID"
                    },
                    new object[]
                    {
                        keyword ?? "",
                        pageNumber,
                        pageSize,
                        _dateStart,
                        _dateEnd,
                        supplierId,
                        status,
                        employeeId
                    }
                );

                var data = SQLHelper<dynamic>.GetListData(dt, 0);
                var data1 = SQLHelper<dynamic>.GetListData(dt, 1);

                if (data != null)
                {
                    if (poType == 0)
                    {
                        data = data.Where(x => Convert.ToInt32(x.POType) == 0).ToList();
                    }
                    else if (poType == 1)
                    {
                        data = data.Where(x => Convert.ToInt32(x.POType) == 1).ToList();
                    }
                }
                int totalPage = 0;
                if (data1 != null && data1.Count > 0)
                {
                    totalPage = data1[0].TotalPage ?? 0;
                }

                var response = new
                {
                    data = data,
                    totalPage = totalPage
                };

                return Ok(ApiResponseFactory.Success(response, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpGet("data-detail")]
        [RequiresPermission("N35,N33,N1")]
        public IActionResult GetDetail(int ponccId)
        {
            try
            {
                var dt = SQLHelper<dynamic>
                    .ProcedureToList("spGetPONCCDetail_Khanh", new string[] { "@PONCCID" }, new object[] { ponccId });
                var data = SQLHelper<dynamic>.GetListData(dt, 0);
                var dtRef = SQLHelper<dynamic>.GetListData(dt, 1);

                var result = new
                {
                    data = data,
                    dtRef = dtRef ?? []
                };

                return Ok(ApiResponseFactory.Success(result, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }


        [HttpGet("deleted-poncc-detail")]
        [RequiresPermission("N35,N33,N1")]
        public async Task<IActionResult> deletedPonccDetail(int poDetailId)
        {
            try
            {
                if (poDetailId > 0) await _pONCCDetailRepo.DeleteAsync(poDetailId);
                return Ok(ApiResponseFactory.Success(null, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }
        #endregion

        #region Lấy dữ liệu các select cho bảng
        [HttpGet("load-bill-code")]
        [RequiresPermission("N35,N33,N1")]
        public IActionResult LoadBillCode(int poTypeId)
        {
            try
            {
                string prefix = poTypeId == 0 ? "DMH" : "DEMO";

                var listPO = _pONCCRepo.GetAll(x => x.BillCode != null && x.BillCode.StartsWith(prefix))
                    .Select(x => new
                    {
                        ID = x.ID,
                        BillCode = x.BillCode,
                        STT = Convert.ToInt32(x.BillCode.Substring(prefix.Length))
                    })
                    .ToList();

                int stt = listPO.Count == 0 ? 1 : listPO.Max(x => x.STT) + 1;

                string sttText = stt.ToString().PadLeft(5, '0');

                string newBillCode = prefix + sttText;

                return Ok(ApiResponseFactory.Success(newBillCode, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("product-sale")]
        public IActionResult getProductSale()
        {
            try
            {
                var listGroup = _productGroupRepo.GetAll().Select(x => x.ID).ToList();
                var idGroup = string.Join(",", listGroup);
                var dt = SQLHelper<object>.ProcedureToList("spGetProductSale", new string[] { "@IDgroup" }, new object[] { idGroup });
                var data = SQLHelper<object>.GetListData(dt, 0);
                return Ok(ApiResponseFactory.Success(data, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("product-rtc")]
        public IActionResult getProductRTC()
        {
            try
            {
                var dt = SQLHelper<object>.ProcedureToList("spGetProductRTC", new string[] { }, new object[] { });
                var data = SQLHelper<object>.GetListData(dt, 0);
                return Ok(ApiResponseFactory.Success(data, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("projects")]
        public IActionResult getProjects()
        {
            try
            {
                var dt = _projectRepo.GetAll().OrderByDescending(x => x.ID);
                return Ok(ApiResponseFactory.Success(dt, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("history-price")]
        [RequiresPermission("N35,N33,N1")]
        public IActionResult getHistoryPrice(int productId, string productCode)
        {
            try
            {
                decimal historyPrice = 0;
                var dt = SQLHelper<object>.ProcedureToList("spGetHistoryPricePartlistForProduct",
                    new string[] { "@ProductSaleID", "@ProductCode" }, new object[] { productId, productCode });
                var data = SQLHelper<object>.GetListData(dt, 0);
                if (data.Count() > 0) historyPrice = (decimal)data[0].UnitPrice;

                return Ok(ApiResponseFactory.Success(historyPrice, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("check-po-code")]
        public IActionResult checkPoCode(int id, string pOCode, string billCode)
        {
            try
            {
                var data = _pONCCRepo.GetAll(x =>
                x.ID != id &&
                x.POCode.Trim().ToLower() == pOCode.Trim().ToLower() &&
                x.BillCode.Trim().ToLower() == billCode.Trim().ToLower() &&
                x.IsDeleted != true
                ).ToList();

                return Ok(ApiResponseFactory.Success(data?.Count() ?? 0, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("po-code")]
        public IActionResult getPoCode(string productCode)
        {
            try
            {
                string prefix = $"{DateTime.Now.ToString("MMyyyy")}-{productCode}-";

                var dt = SQLHelper<object>.ProcedureToList("spGetPOCodeInPONCC",
                        new string[] { "@Value" },
                        new object[] { prefix }
                    );

                // Lấy bảng 0
                var data = SQLHelper<object>.GetListData(dt, 0);

                string currentCode = "";

                if (data.Count > 0)
                {
                    currentCode = Convert.ToString(data[0].POCode);
                }

                int stt = 1;

                if (!string.IsNullOrWhiteSpace(currentCode))
                {
                    stt = Convert.ToInt32(currentCode.Substring(prefix.Length)) + 1;
                }

                // Format STT 3 số
                string sttText = stt.ToString().PadLeft(3, '0');

                string newPOCode = prefix + sttText;

                return Ok(ApiResponseFactory.Success(newPOCode, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("poncc")]
        public IActionResult getPoncc(int ponccId)
        {
            try
            {
                var data = _pONCCRepo.GetByID(ponccId);
                return Ok(ApiResponseFactory.Success(data, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion

        #region Lưu dữ liệu
        [HttpPost("save-data")]
        [RequiresPermission("N35,N33,N1")]
        public async Task<IActionResult> SaveData([FromBody] PONCCDTO data)
        {
            try
            {
                if (!_pONCCRepo.Validate(data, out string message))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }
                if (data.poncc.ID > 0) await _pONCCRepo.UpdateAsync(data.poncc);
                else await _pONCCRepo.CreateAsync(data.poncc);

                #region Xử lý rulePay
                if (data.RulePayID > 0)
                {
                    var rulePay = _pONCCRulePayRepo.GetAll(x => x.PONCCID == data.poncc.ID);

                    foreach (var item in rulePay)
                    {
                        if (item.ID > 0) await _pONCCRulePayRepo.DeleteAsync(item.ID);
                    }
                    PONCCRulePay rulepay = new PONCCRulePay();
                    rulepay.PONCCID = data.poncc.ID;
                    rulepay.RulePayID = data.RulePayID;
                    await _pONCCRulePayRepo.CreateAsync(rulepay);
                }

                foreach (var item in data.lstPONCCDetail)
                {
                    item.PONCCID = data.poncc.ID;
                    string totalPriceFormat = String.Format("{0:0.00}",
                        item.ThanhTien + item.VATMoney + item.FeeShip - item.Discount);
                    item.TotalPrice = Convert.ToDecimal(totalPriceFormat);
                    string currencyExchangeFormat = String.Format("{0:0.00}", item.TotalPrice * data.poncc.CurrencyRate);
                    item.CurrencyExchange = Convert.ToDecimal(currencyExchangeFormat);

                    if (item.ID > 0)
                    {
                        await _pONCCDetailRepo.UpdateAsync(item);
                        UpdateBillImportDetail(item, data.lstBillImportId);
                    }
                    else await _pONCCDetailRepo.CreateAsync(item);

                    if (item.ProjectPartlistPurchaseRequestID == null) continue;
                    ProjectPartlistPurchaseRequest request = _projectPartlistPurchaseRequestRepo.
                        GetByID((int)item.ProjectPartlistPurchaseRequestID);

                    if (request == null) continue;
                    if (data.poncc.SupplierSaleID == request.SupplierSaleID) continue;

                    request.SupplierSaleID = data.poncc.SupplierSaleID;
                    if (request.ID > 0)
                    {
                        await _projectPartlistPurchaseRequestRepo.UpdateAsync(request);
                    }

                    string ponccDetailRequestBuyId = item.PONCCDetailRequestBuyID;
                    if (string.IsNullOrWhiteSpace(ponccDetailRequestBuyId)) continue;
                    string[] idRequestBuys = ponccDetailRequestBuyId.Split(';');

                    foreach (string idRequestBuy in idRequestBuys)
                    {
                        var id = Convert.ToInt32(idRequestBuy);
                        PONCCDetailRequestBuy poRequestBuy = _pONCCDetailRequestBuyRepo.GetAll(x =>
                        x.PONCCDetailID == item.ID && x.PONCCDetailID == id).FirstOrDefault();

                        poRequestBuy = poRequestBuy ?? new PONCCDetailRequestBuy();
                        poRequestBuy.PONCCDetailID = item.ID;
                        poRequestBuy.ProjectPartlistPurchaseRequestID = id;
                        if (poRequestBuy.ID <= 0)
                        {
                            await _pONCCDetailRequestBuyRepo.CreateAsync(poRequestBuy);
                        }
                        else
                        {
                            await _pONCCDetailRequestBuyRepo.UpdateAsync(poRequestBuy);
                        }
                    }
                }
                #endregion
                return Ok(ApiResponseFactory.Success(null, "Đã cập nhật đặt hàng thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

        [HttpPost("update-poncc")]
        [RequiresPermission("N35,N33,N1")]
        public async Task<IActionResult> updatePoncc([FromBody] List<PONCC> data)
        {
            try
            {
                if (data.Count() > 0)
                {
                    foreach (var item in data)
                    {
                        if (item.ID > 0) await _pONCCRepo.UpdateAsync(item);
                    }
                }
                return Ok(ApiResponseFactory.Success(null, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

        private async Task UpdateBillImportDetail(PONCCDetail detail, List<int> lstBillImportId)
        {
            try
            {
                if (lstBillImportId.Count <= 0 || detail.ID == 0) return;
                foreach (var i in lstBillImportId)
                {
                    Expression ep1 = new Expression("BillImportID", lstBillImportId[i]);
                    Expression ep2 = new Expression("PONCCDetailID", detail.ID);
                    List<BillImportDetail> lst = _billImportDetailRepo.GetAll(x => x.BillImportID == i && x.PONCCDetailID == detail.ID);
                    foreach (BillImportDetail item in lst)
                    {
                        item.ProductID = detail.ProductSaleID;
                        item.Price = detail.UnitPrice;
                        item.QtyRequest = detail.QtyRequest;
                        item.ProjectCode = detail.ProductCodeOfSupplier;
                        item.ProjectID = detail.ProjectID;
                        if (item.ID <= 0)
                        {
                            await _billImportDetailRepo.CreateAsync(item);
                        }
                        else
                        {
                            await _billImportDetailRepo.UpdateAsync(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
    }
}