using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.Record.Chart;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Finance.Implementations;
using Org.BouncyCastle.Asn1.Pkcs;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZXing;
using ZXing.OneD.RSS;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectPartlistController : ControllerBase
    {
        ProductSaleRepo _productSaleRepo;
        FirmRepo _firmRepo;
        UnitCountKTRepo _unitCountRepo;
        ProductRTCRepo _productRTCRepo;
        ProjectPartlistPriceRequestRepo _priceRequestRepo;
        ProjectPartlistVersionRepo _partlistVersionRepo;
        ProjectPartlistPurchaseRequestRepo _partlistPurchaseRequestRepo;
        private readonly ProjectPartListRepo _projectPartlistRepo;
        private readonly WarehouseRepo _warehouseRepo;
        private readonly BillExportRepo _billExportRepo;
        private readonly ProductGroupRepo _productGroupRepo;
        private readonly InventoryStockRepo _inventoryStockRepo;
        UnitCountKTRepo _unitCountKTRepo;
        public ProjectPartlistController(ProjectPartListRepo projectPartlistRepo, ProductSaleRepo productSaleRepo, FirmRepo firmRepo, UnitCountKTRepo unitCountRepo, ProductRTCRepo productRTCRepo, ProjectPartlistPriceRequestRepo priceRequestRepo, ProjectPartlistVersionRepo partlistVersionRepo, ProjectPartlistPurchaseRequestRepo partlistPurchaseRequestRepo, UnitCountKTRepo unitCountKTRepo, WarehouseRepo warehouseRepo, BillExportRepo billExportRepo, ProductGroupRepo productGroupRepo, InventoryStockRepo inventoryStockRepo)
        {
            _projectPartlistRepo = projectPartlistRepo;
            _productSaleRepo = productSaleRepo;
            _firmRepo = firmRepo;
            _unitCountRepo = unitCountRepo;
            _productRTCRepo = productRTCRepo;
            _priceRequestRepo = priceRequestRepo;
            _partlistVersionRepo = partlistVersionRepo;
            _partlistPurchaseRequestRepo = partlistPurchaseRequestRepo;
            _unitCountKTRepo = unitCountKTRepo;
            _warehouseRepo = warehouseRepo;
            _billExportRepo = billExportRepo;
            _productGroupRepo = productGroupRepo;
            _inventoryStockRepo = inventoryStockRepo;
        }
        [HttpPost("get-all")]
        public IActionResult GetAll(ProjectPartlistParam param)
        {
            try
            {
                var dt = SQLHelper<dynamic>.ProcedureToList("spGetProjectPartList_Khanh", new string[] { "@ProjectID", "@PartListTypeID", "@IsDeleted", "@Keyword", "@IsApprovedTBP", "@IsApprovedPurchase", "@ProjectPartListVersionID" }, new object[] { param.ProjectID, param.PartlistTypeID, param.IsDeleted, param.Keywords, param.IsApprovedTBP, param.IsApprovedPurchase, param.ProjectPartListVersionID });

                return Ok(ApiResponseFactory.Success(
                    SQLHelper<object>.GetListData(dt, 0),
                    "Lấy dữ liệu danh mục vật tư thành công!"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(
                    ex,
                    ex.Message
                ));
            }
        }
        [HttpGet("get-suggestion")]
        public IActionResult GetSuggestion()
        {
            var dt = SQLHelper<dynamic>.ProcedureToList("spGetHistoryPricePartlist_Khanh",
                                                    new string[] { "@Keyword", "@ProjectID", "@SupplierSaleID" },
                                                    new object[] { "", 0, 0 });
            var data = SQLHelper<dynamic>.GetListData(dt, 0);
            return Ok(ApiResponseFactory.Success(data, ""));
        }
        [HttpGet("get-stt")]
        public IActionResult getSTT(int versionID)
        {
            int stt = _projectPartlistRepo.getSTT(versionID);
            return Ok(ApiResponseFactory.Success(stt, ""));
        }
        [HttpPost("approvedTBP")]
        public async Task<IActionResult> AppprovedTBP([FromBody] ApprovedTBPRequest request)
        {
            try
            {
                string messageError;

                foreach (var item in request.ProjectPartListID)
                {
                    var pjPL = _projectPartlistRepo.GetByID(item);
                    if (pjPL == null)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Không thể duyệt vì vật tư thứ TT []"));
                    }


                    var productSales = _productSaleRepo.GetAll(x => x.IsDeleted == false
                       && x.ProductCode == pjPL.ProductCode && x.IsFix == true).FirstOrDefault();

                    if (productSales?.IsFix == true)
                    {
                        if (!_projectPartlistRepo.ValidateProduct(pjPL, out string messageErr, true))
                        {
                            return Ok(new { status = 2, message = messageErr });
                        }
                    }


                    if (!_projectPartlistRepo.ValidateApproveTBP(pjPL, request.Approved, out messageError))
                    {
                        return Ok(new
                        {
                            status = 2,
                            message = messageError
                        });
                    }

                    pjPL.IsApprovedTBP = request.Approved;
                    await _projectPartlistRepo.UpdateAsync(pjPL);
                }

                return Ok(ApiResponseFactory.Success(null, "Duyệt thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi:{ex.Message}"));
            }
        }
        [HttpPost("price-request")]
        public async Task<IActionResult> PriceRequest([FromBody] List<ProjectPartlistDTO> request)
        {
            try
            {
                bool checkThreeMonth = false;
                string messageError;
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                // ===== LOOP 1: VALIDATION =====
                foreach (var item in request)
                {
                    if (item.ID <= 0) continue;
                    if (!item.IsLeaf) continue; // Bỏ qua TẤT CẢ node cha (mọi cấp)
                    if (!_projectPartlistRepo.CheckValidate(item, out messageError))
                    {
                        return Ok(new { status = 2, message = messageError });
                    }
                    if (item.IsApprovedTBPNewCode == false && item.IsNewCode == true)
                    {
                        return Ok(new { status = 2, message = $"Vật tư Stt [{item.STT}] chưa được TBP duyệt mới.\nVui lòng kiểm tra lại!" });
                    }

                    var existingRequest = _priceRequestRepo.GetAll(x => x.ProjectPartListID == item.ID && x.IsDeleted == false)
                                                           .OrderByDescending(x => x.StatusRequest)
                                                           .FirstOrDefault();

                    if (existingRequest != null && existingRequest.StatusRequest > 0)
                    {
                        // Tạo mốc thời gian 3 tháng trước
                        var threeMonthsAgo = DateTime.Now.AddMonths(-3);

                        // Nếu chưa có ngày báo giá hoặc ngày báo giá đã quá 3 tháng
                        if (existingRequest.DatePriceQuote == null || existingRequest.DatePriceQuote > threeMonthsAgo)
                        {
                            checkThreeMonth = true;
                            return Ok(new
                            {
                                status = 2,
                                message = $"Vật tư Stt [{item.STT}] đã được yêu cầu báo giá.\nVui lòng kiểm tra lại!"
                            });
                        }
                    }
                }

                // ===== LOOP 2: XỬ LÝ =====
                foreach (var item in request)
                {
                    // Tạo mốc thời gian 3 tháng trước
                    var threeMonthsAgo = DateTime.Now.AddMonths(-3);
                    if (item.ID <= 0) continue;
                    if (item.StatusPriceRequest > 0 && (item.DatePriceQuote == null || item.DatePriceQuote > threeMonthsAgo)) continue;

                    // Cập nhật ProjectPartList (cả cha và con)
                    var partList = _projectPartlistRepo.GetByID(item.ID);
                    if (partList == null || partList.ID <= 0) continue;

                    partList.StatusPriceRequest = 1;
                    partList.DeadlinePriceRequest = item.DeadlinePriceRequest;
                    partList.DatePriceRequest = DateTime.Now;
                    await _projectPartlistRepo.UpdateAsync(partList);
                    // CHỈ TẠO PRICEREQUEST CHO NODE LÁ
                    if (!item.IsLeaf) continue;

                    var priceRequest = new ProjectPartlistPriceRequest
                    {
                        ProjectPartListID = item.ID,
                        EmployeeID = currentUser.EmployeeID,
                        ProductCode = item.ProductCode,
                        ProductName = item.GroupMaterial,
                        StatusRequest = checkThreeMonth ? item.StatusPriceRequest + 1 : 1,
                        DateRequest = DateTime.Now,
                        Deadline = item.DeadlinePriceRequest,
                        Quantity = item.QtyFull,
                        IsDeleted = false
                    };

                    await _priceRequestRepo.CreateAsync(priceRequest);
                }

                return Ok(ApiResponseFactory.Success(null, "Yêu cầu báo giá thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi: {ex.Message}"));
            }
        }

        [HttpPost("cancel-price-request")]
        public async Task<IActionResult> CancelPriceRequest([FromBody] List<ProjectPartlistDTO> request)
        {
            try
            {
                string messageError;
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                foreach (var item in request)
                {
                    if (item.ID <= 0) continue;
                    if (!item.IsLeaf) continue; // Bỏ qua TẤT CẢ node cha (mọi cấp)

                    if (item.IsCheckPrice == true)
                    {
                        return Ok(new { status = 2, message = $"Phòng mua đang check giá sản phẩm Stt [{item.STT}].\nBạn không thể hủy y/c báo giá " });
                    }
                    if (item.EmployeeIDRequestPrice != currentUser.EmployeeID && !currentUser.IsAdmin)
                    {
                        return Ok(new { status = 2, message = $"Bạn không thể hủy yêu cầu báo giá của người khác!" });
                    }
                    List<ProjectPartlistPriceRequest> priceRequests = _priceRequestRepo.GetAll(x => x.ProjectPartListID == item.ID && x.EmployeeID == item.EmployeeIDRequestPrice);
                    if (priceRequests.Count > 0)
                    {
                        foreach (var rs in priceRequests)
                        {
                            rs.IsDeleted = true;
                            rs.StatusRequest = 0;
                            await _priceRequestRepo.UpdateAsync(rs);
                        }
                    }
                    ProjectPartList partList = _projectPartlistRepo.GetByID(item.ID);
                    if (partList == null || partList.ID <= 0) continue;
                    partList.StatusPriceRequest = 0;
                    partList.DatePriceRequest = null;
                    partList.DeadlinePriceRequest = null;
                    await _projectPartlistRepo.UpdateAsync(partList);
                }
                return Ok(ApiResponseFactory.Success(null, "Hủy yêu cầu báo giá thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi:{ex.Message}"));
            }
        }
        //lấy giá lịch sử
        [HttpPost("get-price-history")]
        public async Task<IActionResult> GetPriceHistory([FromBody] List<ProjectPartlistDTO> request)
        {
            try
            {
                string messageError;
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                foreach (var item in request)
                {
                    if (item.ID <= 0) continue;
                    if (!item.IsLeaf) continue; // Bỏ qua TẤT CẢ node cha (mọi cấp)

                    decimal unitPriceHistory = Convert.ToDecimal(item.UnitPriceHistory);
                    decimal qtyFull = Convert.ToDecimal(item.QtyFull);
                    decimal amount = unitPriceHistory * qtyFull;

                    ProjectPartList data = _projectPartlistRepo.GetByID(item.ID);
                    data.Price = unitPriceHistory;
                    data.Amount = amount;
                    await _projectPartlistRepo.UpdateAsync(data);

                }
                return Ok(ApiResponseFactory.Success(null, "Lấy giá lịch sử thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi:{ex.Message}"));
            }
        }
        //end
        //khôi phục partlist 
        [HttpPost("restore-delete")]
        public async Task<IActionResult> RestoreDelete([FromBody] List<ProjectPartlistDTO> request)
        {
            try
            {
                foreach (var item in request)
                {
                    ProjectPartList data = _projectPartlistRepo.GetByID(item.ID);
                    data.IsDeleted = false;
                    await _projectPartlistRepo.UpdateAsync(data);

                }
                return Ok(ApiResponseFactory.Success(null, "Khôi phục thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi:{ex.Message}"));
            }
        }

        [HttpPost("approve-purchase-request")]
        public async Task<IActionResult> ApprovePurchaseRequest([FromBody] List<ProjectPartlistDTO> request, bool isApproved, int projectTypeID, int projectSolutionID, int projectID)
        {
            try
            {
                string messageError;
                string approvedText = isApproved ? "yêu cầu mua hàng" : "hủy yêu cầu mua hàng";
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                foreach (var item in request)
                {
                    var data = _projectPartlistRepo.GetByID(item.ID);

                    if (isApproved)
                    {
                        if (!_projectPartlistRepo.CheckValidate(data, out messageError))
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, messageError));
                        }
                    }

                    if (!item.IsLeaf) continue;

                    if (item.IsDeleted == true)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, $"Không thể {approvedText} vì vật tư thứ tự [{item.TT}] đã bị xóa!"));
                    }

                    //validate duyệt
                    if (item.IsApprovedTBP == false && isApproved == true)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, $"Không thể {approvedText} vì vật tư thứ tự [{item.TT}] chưa được TBP duyệt!"));
                    }

                    if (isApproved == true && item.IsApprovedTBPNewCode == false && item.IsNewCode == true)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, $"Không thể {approvedText} vì vật tư thứ tự [{item.TT}] chưa được TBP duyệt mới!"));
                    }

                    if (isApproved == true && item.IsApprovedPurchase == true)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, $"Vật tư thứ tự [{item.TT}] đã được Y/c mua.\nVui lòng kiểm tra lại!"));
                    }
                    if (isApproved == true && item.DatePriceQuote == null)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, $"Vật tư thứ tự [{item.TT}] chưa được báo giá.\nVui lòng kiểm tra lại!"));
                    }
                    // Tạo mốc thời gian 3 tháng trước
                    var threeMonthsAgo = DateTime.Now.AddMonths(-3);
                    if (isApproved == true && item.DatePriceQuote < threeMonthsAgo)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, $"Vật tư thứ tự [{item.TT}] đã được báo giá từ hơn 3 tháng trước!\nVui lòng yêu cầu báo giá lại!"));
                    }
                }

                List<ProjectPartList> listPartLists = new List<ProjectPartList>();

                if (isApproved)
                {
                    var version = _partlistVersionRepo.GetAll(x => x.ProjectTypeID == projectTypeID && x.ProjectSolutionID == projectSolutionID && x.StatusVersion == 1).FirstOrDefault();
                    if (version == null) version = new ProjectPartListVersion();

                    var dt = SQLHelper<dynamic>.ProcedureToList(
                        "spGetProjectPartList_Khanh",
                        new string[] { "@ProjectID", "@PartListTypeID", "@IsDeleted", "@Keyword", "@IsApprovedTBP", "@IsApprovedPurchase", "@ProjectPartListVersionID" },
                        new object[] { projectID, projectTypeID, 0, " ", -1, -1, version.ID }
                    );
                    var s = SQLHelper<object>.GetListData(dt, 0);

                    foreach (var item in request)
                    {
                        if (!item.IsLeaf) continue;

                        ProjectPartList model = _projectPartlistRepo.GetByID(item.ID);
                        model.IsApprovedPurchase = isApproved;
                        model.RequestDate = DateTime.Now;
                        model.Status = 1;
                        model.ExpectedReturnDate = item.DeadlinePur;
                        model.SupplierSaleID = item.SupplierSaleQuoteID;
                        model.UnitMoney = item.UnitMoney;
                        model.PriceOrder = item.UnitPriceQuote;
                        model.TotalPriceOrder = item.TotalPriceOrder;
                        model.QtyFull = item.QtyFull;
                        model.LeadTime = item.LeadTime;

                        await _projectPartlistRepo.UpdateAsync(model);
                        listPartLists.Add(model);
                    }
                }
                else
                {
                    foreach (var item in request)
                    {
                        var purchaseRequest = _partlistPurchaseRequestRepo.GetAll(x =>
                            x.ProjectPartListID == item.ID &&
                            x.IsDeleted == false &&
                            x.EmployeeIDRequestApproved > 0
                        );

                        if (purchaseRequest.Count > 0)
                        {
                            return BadRequest(ApiResponseFactory.Fail(null,
                                $"Vật tư thứ tự [{item.TT}] đang được check đặt hàng. Bạn không thể hủy.\nVui lòng liên hệ nhân viên Pur để hủy!"
                            ));
                        }

                        ProjectPartList model = _projectPartlistRepo.GetByID(item.ID);
                        model.IsApprovedPurchase = isApproved;
                        model.RequestDate = null;
                        model.ExpectedReturnDate = null;
                        model.Status = 2;

                        await _projectPartlistRepo.UpdateAsync(model);
                        listPartLists.Add(model);
                    }
                }

                _projectPartlistRepo.UpdatePurchaseRequest(listPartLists, currentUser.EmployeeID);
                return Ok(ApiResponseFactory.Success(null, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi: {ex.Message}"));
            }
        }


        [HttpPost("approved-newcode")]
        public async Task<IActionResult> ApprovedNewCode(bool isApprovedNew, [FromBody] List<ProjectPartlistDTO> request)
        {
            try
            {
                string approvedText = isApprovedNew ? "Duyệt" : "Hủy duyệt";
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                if (request.Count <= 0) // Sửa: < 0 → <= 0
                {
                    return Ok(new { status = 2, message = "Vui lòng chọn sản phẩm cần duyệt!" });
                }

                List<Firm> firms = _firmRepo.GetAll(x => x.FirmType == 1);
                List<Firm> firmDemos = _firmRepo.GetAll(x => x.FirmType == 2);

                foreach (var item in request)
                {
                    if (!item.IsLeaf) continue;
                    if (item.IsNewCode == false && isApprovedNew) continue;
                    if (string.IsNullOrWhiteSpace(item.ProductCode)) continue;

                    var productSales = _productSaleRepo.GetAll(x => x.IsDeleted == false
                        && x.ProductCode == item.ProductCode && x.IsFix == true).FirstOrDefault();

                    if (productSales?.IsFix == true && isApprovedNew)
                    {
                        // Kiểm tra ValidateProduct (logic đã chuẩn)
                        if (!_projectPartlistRepo.ValidateProduct(item, out string messageErr, true))
                        {
                            return Ok(new { status = 2, message = messageErr });
                        }
                    }
                    if (productSales?.IsFix == true && isApprovedNew) continue;

                    // Update ProjectPartList
                    ProjectPartList model = _projectPartlistRepo.GetByID(item.ID);
                    model.IsApprovedTBPNewCode = isApprovedNew;
                    model.DateApprovedNewCode = DateTime.Now;
                    model.EmployeeApprovedNewCode = currentUser.EmployeeID;
                    await _projectPartlistRepo.UpdateAsync(model);

                    // Update ProductSale
                    Firm firm = firms.FirstOrDefault(x => x.FirmName.ToUpper().Trim() == item.Manufacturer.ToUpper().Trim()
                        && x.FirmType == 1) ?? new Firm(); // Sửa: == 1 thay vì == 2

                    var products = _productSaleRepo.GetAll(x => x.ProductCode == item.ProductCode).ToList();
                    if (products != null && products.Any()) // Sửa: thêm Any()
                    {
                        foreach (var prod in products)
                        {
                            prod.FirmID = firm.ID;
                            prod.Unit = item.Unit;
                            prod.ProductName = item.GroupMaterial;
                            prod.Maker = item.Manufacturer;
                            await _productSaleRepo.UpdateAsync(prod);
                        }
                    }

                    // Update ProductRTC (kho demo)
                    Firm firmDemo = firmDemos.FirstOrDefault(x => x.FirmName.ToUpper().Trim() == item.Manufacturer
                        && x.FirmType == 2) ?? new Firm();

                    var productRTCs = _productRTCRepo.GetAll(x => x.ProductCode == item.ProductCode).ToList();
                    if (productRTCs != null && productRTCs.Any())
                    {
                        foreach (var prod in productRTCs)
                        {
                            prod.FirmID = firmDemo.ID;
                            prod.ProductName = item.GroupMaterial;
                            prod.Maker = item.Manufacturer;
                            await _productRTCRepo.UpdateAsync(prod);
                        }
                    }

                    // Update PriceRequest
                    var priceRequests = _priceRequestRepo.GetAll(x => x.ProductCode == item.ProductCode).ToList();
                    if (priceRequests != null && priceRequests.Any())
                    {
                        foreach (var pr in priceRequests)
                        {
                            pr.ProductName = item.GroupMaterial;
                            pr.Maker = item.Manufacturer;
                            await _priceRequestRepo.UpdateAsync(pr);
                        }
                    }

                    // Update Partlist
                    var partlists = _projectPartlistRepo.GetAll(x => x.ProductCode == item.ProductCode).ToList();
                    if (partlists != null && partlists.Any())
                    {
                        foreach (var pl in partlists)
                        {
                            pl.Unit = item.Unit;
                            pl.GroupMaterial = item.GroupMaterial;
                            pl.Manufacturer = item.Manufacturer; // Sửa: items thay vì item
                            await _projectPartlistRepo.UpdateAsync(pl);
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(null, $"{approvedText} thành công!")); // Sửa message động
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi: {ex.Message}"));
            }
        }
        //duyệt tích xanh 
        [HttpPost("approved-fix")]
        public async Task<IActionResult> ApprovedIsFix([FromBody] List<ProjectPartlistDTO> request, bool isFix)
        {
            try
            {
                string approvedText = isFix ? "Duyệt" : "Hủy duyệt";
                string messageErr;

                // ===============================
                // 1. Validate: giống WinForm
                // ===============================

                foreach (var item in request)
                {
                    if (!item.IsLeaf) continue;

                    // Kiểm tra bị xóa
                    if (item.IsDeleted == true)
                    {
                        return Ok(new { status = 2, message = "Không thể duyệt vì vật tư đã bị xóa!" });
                    }

                    // Kiểm tra ValidateProduct (logic đã chuẩn)
                    if (!_projectPartlistRepo.ValidateProduct(item, out messageErr, false))
                    {
                        return Ok(new { status = 2, message = messageErr });
                    }
                }
                // preload Firm
                var firms = _firmRepo.GetAll(x => x.FirmType == 1).ToList();
                var firmDemos = _firmRepo.GetAll(x => x.FirmType == 2).ToList();


                foreach (var item in request)
                {
                    if (!item.IsLeaf) continue;

                    // Skip mã mới khi DUYỆT
                    if (item.IsNewCode == true && isFix == true)
                        continue;

                    var productSale = _productSaleRepo.GetAll(x => x.ProductCode == item.ProductCode);
                    if (productSale == null) continue;

                    // Tìm Firm theo WinForm
                    var firm = firms
                        .FirstOrDefault(x => x.FirmName.ToUpper().Trim() == (item.Manufacturer ?? "").ToUpper().Trim() && x.FirmType == 2)
                        ?? new Firm();
                    foreach (var product in productSale)
                    {

                        if (isFix)
                        {
                            // Giống WinForm UpdateProductSale
                            product.ProductName = item.GroupMaterial;
                            product.Maker = item.Manufacturer;
                            product.Unit = item.Unit;
                            product.IsFix = true;
                            product.FirmID = firm.ID;
                        }
                        else
                        {
                            product.IsFix = false;
                        }
                        await _productSaleRepo.UpdateAsync(product);
                    }
                }
                return Ok(ApiResponseFactory.Success(null,
                    $"{approvedText} tích xanh thành công!"));
            }
            catch (Exception ex)
            {

                return BadRequest(ApiResponseFactory.Fail(ex,
                    $"Lỗi: {ex.Message}"));
            }

        }
        //end
        //get gợi ý cho tên thiết bị và hãng sản xuất 
        [HttpGet("get-suggestion-name-maker")]
        public async Task<IActionResult> GetSuggestionNameMaker()
        {
            try
            {
                var dt = SQLHelper<dynamic>.ProcedureToList("spGetProjectPartlistSuggest",
                                                   new string[] { },
                                                   new object[] { });
                var data = SQLHelper<dynamic>.GetListData(dt, 0);

                var result = new
                {
                    ProductNames = data.Select(x => x.ProductName).Distinct().ToList(),
                    Makers = data.Select(x => x.Maker).Distinct().ToList(),
                };

                return Ok(ApiResponseFactory.Success(result, " lấy dữ liệu thành công!")); // Sửa message động
            }
            catch (Exception ex) { return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi: {ex.Message}")); }

        }
        //end
        [HttpGet("get-partlist-by-id")]
        public async Task<IActionResult> GetPartListByID(int partlistID)
        {
            try
            {
                var result = _projectPartlistRepo.GetByID(partlistID);
                return Ok(ApiResponseFactory.Success(result, " lấy dữ liệu thành công!")); // Sửa message động
            }
            catch (Exception ex) { return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi: {ex.Message}")); }

        }
        [HttpPost("save-projectpartlist")]
        public async Task<IActionResult> SaveDataAsync(ProjectPartList partList, bool overrideFix = false)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                // 1. Trim string fields
                partList.TT = partList.TT?.Trim();
                partList.ProductCode = partList.ProductCode?.Trim();
                partList.SpecialCode = partList.SpecialCode?.Trim();
                partList.GroupMaterial = partList.GroupMaterial?.Trim();
                partList.Manufacturer = partList.Manufacturer?.Trim();
                partList.Model = partList.Model?.Trim();
                partList.Unit = partList.Unit?.Trim();
                partList.ReasonProblem = partList.ReasonProblem?.Trim();
                partList.Note = partList.Note?.Trim();

                // 2. Validate
                if (!_projectPartlistRepo.Validate(partList, out string message))
                    return BadRequest(ApiResponseFactory.Fail(null, message));

                // ===== 3. Validate TÍCH XANH nếu KHÔNG override =====
                if (!overrideFix)
                {
                    if (!_projectPartlistRepo.ValidateFixProduct(partList, out string fixMessage))
                    {
                        var fixedProduct = _productSaleRepo.GetAll(x =>
                                            x.IsDeleted != true &&
                                            x.ProductCode == partList.ProductCode &&
                                            x.IsFix == true
                                            ).FirstOrDefault() ;
                        return Ok(ApiResponseFactory.Fail(null, fixMessage,fixedProduct));
                    }
                }

                // 3. Get ProjectTypeID from Version
                var version = _partlistVersionRepo.GetByID(partList.ProjectPartListVersionID ?? 0);
                if (version == null || version.ID <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy phiên bản!"));
                partList.ProjectTypeID = version.ProjectTypeID;

                // 4. Calculate ParentID & STT
                partList.ParentID = _projectPartlistRepo.GetParentID(partList.TT, partList.ProjectTypeID ?? 0, partList.ProjectPartListVersionID ?? 0);
                if (partList.STT == null || partList.STT <= 0)
                    partList.STT = _projectPartlistRepo.getSTT(partList.ProjectPartListVersionID ?? 0);

                // 5. INSERT or UPDATE
                if (partList.ID <= 0)
                {
                    // INSERT
                    partList.IsApprovedPurchase = false;
                    partList.IsApprovedTBP = false;
                    await _projectPartlistRepo.CreateAsync(partList);
                }
                else
                {
                    // UPDATE
                    var partlistOld = _projectPartlistRepo.GetByID(partList.ID);
                    if (partlistOld == null) return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy dữ liệu!"));

                    // Nếu chuyển sang IsProblem = true → INSERT new record
                    if (partList.IsProblem == true && partlistOld.IsProblem != true)
                    {
                        partList.ID = 0;
                        partList.IsApprovedPurchase = false;
                        partList.IsApprovedTBP = false;
                        partList.StatusPriceRequest = 0;
                        partList.DatePriceRequest = null;
                        partList.DeadlinePriceRequest = null;
                        partList.RequestDate = null;
                        partList.ExpectedReturnDate = null;
                        partList.Status = 2;
                        await _projectPartlistRepo.CreateAsync(partList);
                    }
                    else
                    {
                        await _projectPartlistRepo.UpdateAsync(partList);
                        await UpdateRequestQuoteAsync(partList, partlistOld, currentUser);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        private async Task UpdateRequestQuoteAsync(
     ProjectPartList partListNew,
     ProjectPartList partListOld,
     CurrentUser currentUser)
        {
            // Check if important fields changed
            bool hasChanges = partListNew.ProductCode != partListOld.ProductCode ||
                            partListNew.GroupMaterial != partListOld.GroupMaterial ||
                            partListNew.Manufacturer != partListOld.Manufacturer ||
                            partListNew.Model != partListOld.Model ||
                            partListNew.QtyMin != partListOld.QtyMin ||
                            partListNew.QtyFull != partListOld.QtyFull ||
                            partListNew.Unit != partListOld.Unit ||
                            partListNew.SpecialCode != partListOld.SpecialCode ||
                            partListNew.ReasonProblem != partListOld.ReasonProblem ||
                            partListNew.Note != partListOld.Note;

            if (!hasChanges) return;

            // Get existing price requests
            var listQuotes = _priceRequestRepo.GetAll(x =>
                x.ProjectPartListID == partListNew.ID &&
                x.IsDeleted != true
            );

            if (!listQuotes.Any()) return;
            if (!partListNew.DeadlinePriceRequest.HasValue || !partListNew.DatePriceRequest.HasValue) return;

            // Mark old quotes as deleted
            foreach (var quote in listQuotes)
            {
                quote.IsDeleted = true;
                await _priceRequestRepo.UpdateAsync(quote);
            }

            // Calculate new deadline
            DateTime now = DateTime.Now;
            partListNew.StatusPriceRequest = 1;

            // Lưu DatePriceRequest cũ để tính toán
            DateTime oldDateRequest = partListNew.DatePriceRequest.Value;
            DateTime oldDeadline = partListNew.DeadlinePriceRequest.Value;

            // Set DatePriceRequest mới
            partListNew.DatePriceRequest = now;

            // Tính số ngày đã trôi qua từ deadline cũ đến hiện tại
            double totalDay = (now.Date - oldDeadline.Date).TotalDays;

            // Tính số ngày ban đầu giữa DateRequest và Deadline
            double totalDayOld = (oldDeadline.Date - oldDateRequest.Date).TotalDays;

            // Nếu đã quá deadline, cộng thêm số ngày quá hạn + số ngày ban đầu
            if (totalDay > 0)
            {
                partListNew.DeadlinePriceRequest = oldDeadline.AddDays(totalDay + totalDayOld);
            }
            else
            {
                // Chưa quá hạn, giữ nguyên khoảng cách ban đầu từ ngày hiện tại
                partListNew.DeadlinePriceRequest = now.Date.AddDays(totalDayOld);
            }

            // Add 1 day if after 15:30
            if (now.TimeOfDay > new TimeSpan(15, 30, 0))
            {
                partListNew.DeadlinePriceRequest = partListNew.DeadlinePriceRequest.Value.AddDays(1);
            }

            // Handle weekend
            if (partListNew.DeadlinePriceRequest.Value.DayOfWeek == DayOfWeek.Saturday)
            {
                partListNew.DeadlinePriceRequest = partListNew.DeadlinePriceRequest.Value.AddDays(2);
            }
            else if (partListNew.DeadlinePriceRequest.Value.DayOfWeek == DayOfWeek.Sunday)
            {
                partListNew.DeadlinePriceRequest = partListNew.DeadlinePriceRequest.Value.AddDays(1);
            }
            await _projectPartlistRepo.UpdateAsync(partListNew);

            // Create new price request
            await _priceRequestRepo.CreateAsync(new ProjectPartlistPriceRequest
            {
                ProjectPartListID = partListNew.ID,
                EmployeeID = currentUser.EmployeeID,
                ProductCode = partListNew.ProductCode,
                ProductName = partListNew.GroupMaterial,
                StatusRequest = 1,
                DateRequest = now,
                Deadline = partListNew.DeadlinePriceRequest,
                Quantity = partListNew.QtyFull,
            });
        }

        //end

        [HttpPost("import-check")]
        public IActionResult ImportCheck([FromBody] PartlistImportRequestDTO request)
        {
            try
            {
                // --- 1. Validate dữ liệu bắt buộc ---
                if (request.Items == null || request.Items.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu!"));

                if (request.ProjectID <= 0 || request.ProjectPartListVersionID <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Thiếu thông tin ProjectID hoặc VersionID!"));

                // --- 2. Gọi Validate2 y như WinForms ---
                var result = _projectPartlistRepo.Validate2(request);

                // Validate fail nhưng không có diff → lỗi thực sự
                if (!result.IsValid && !result.Diffs.Any())
                    return BadRequest(ApiResponseFactory.Fail(null, result.Message));

                // Có diff → yêu cầu người dùng chọn Excel hoặc Kho
                return Ok(new
                {
                    success = true,
                    message = result.Diffs.Any()
                        ? "Phát hiện khác biệt với kho, vui lòng xác nhận!"
                        : "Dữ liệu hợp lệ!",
                    needConfirm = result.Diffs.Any(),
                    diffs = result.Diffs
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("check-tontai")]
        public async Task<IActionResult> checkTonTai([FromBody] PartlistImportRequestDTO request)
        {
            try
            {
                var partlists = _projectPartlistRepo.GetAll(x => x.ProjectID == request.ProjectID && x.ProjectPartListVersionID == request.ProjectPartListVersionID && x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(partlists, "Check du lieu thanh cong "));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("over-write-data")]
        public async Task<IActionResult> OverWriteData([FromBody] PartlistImportRequestDTO request)
        {
            try
            {
                var partlists = _projectPartlistRepo.GetAll(x => x.ProjectID == request.ProjectID && x.ProjectPartListVersionID == request.ProjectPartListVersionID && x.IsDeleted != true);
                if (partlists.Count > 0)
                {
                    foreach (var item in partlists)
                    {
                        item.IsDeleted = true;
                        await _projectPartlistRepo.UpdateAsync(item);
                    }
                }
                return Ok(ApiResponseFactory.Success(null, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("apply-diff2")]
        public async Task<IActionResult> ApplyDiff([FromBody] PartlistImportRequestDTO request)
        {
            try
            {
                // --- 1. Áp dụng diff vào dữ liệu Excel ---
                if (request.Diffs != null && request.Diffs.Any())
                {
                    foreach (var diff in request.Diffs)
                    {
                        var row = request.Items.FirstOrDefault(x => x.ProductCode == diff.ProductCode);
                        if (row == null) continue;

                        if (diff.Choose == "Stock")
                        {
                            row.GroupMaterial = diff.GroupMaterialStock;
                            row.Manufacturer = diff.ManufacturerStock;
                            row.Unit = diff.UnitStock;
                        }
                    }
                }

                // --- 2. Lấy TT cha theo version & project ---
                var oldItems = _projectPartlistRepo.GetAll(x =>
                    x.ProjectID == request.ProjectID &&
                    x.ProjectPartListVersionID == request.ProjectPartListVersionID &&
                    x.IsDeleted != true
                );

                List<string> listParentTT = oldItems
                    .Where(x => x.TT.Contains("."))
                    .Select(x => x.TT.Substring(0, x.TT.LastIndexOf(".")))
                    .Distinct()
                    .ToList();

                Regex regex = new Regex(@"^-?[\d\.]+$");

                // --- 3. Lấy suggest từ WinForms ---
                var dt = SQLHelper<dynamic>.ProcedureToList("spGetProjectPartlistSuggest", new string[] { }, new object[] { });
                var suggest = SQLHelper<object>.GetListData(dt, 0);

                var newCodes = new List<ProjectPartList>();

                // --- 4. Insert/update từng dòng Excel ---
                foreach (var item in request.Items)
                {
                    if (string.IsNullOrEmpty(item.TT) || !regex.IsMatch(item.TT))
                        continue;

                    // Tìm record cũ theo TT
                    var exist = oldItems.FirstOrDefault(x => x.TT == item.TT);

                    var entity = exist ?? new ProjectPartList();
                    bool isNew = exist == null;

                    entity.ProjectID = request.ProjectID;
                    entity.ProjectTypeID = request.ProjectTypeID;
                    entity.ProjectPartListVersionID = request.ProjectPartListVersionID;
                    entity.ProjectPartListTypeID = request.ProjectTypeID;

                    entity.TT = item.TT;

                    // --- 4.1 ParentID ---
                    entity.ParentID = _projectPartlistRepo.GetParentIdImport(
                        item.TT,
                        request.ProjectPartListVersionID,
                        request.IsProblem,
                        request.ProjectTypeID
                    );

                    // --- 4.2 Gán dữ liệu ---
                    entity.GroupMaterial = item.GroupMaterial;
                    entity.ProductCode = item.ProductCode;
                    entity.OrderCode = item.OrderCode;
                    entity.Manufacturer = item.Manufacturer;
                    entity.SpecialCode = item.SpecialCode; //TN.Binh update
                    entity.Model = item.Model;
                    entity.QtyMin = item.QtyMin;
                    entity.QtyFull = item.QtyFull;
                    entity.Unit = item.Unit;
                    entity.Price = item.Price;
                    entity.Amount = item.Amount;
                    entity.LeadTime = item.LeadTime;
                    entity.NCC = item.NCC;
                    entity.RequestDate = item.RequestDate;
                    entity.LeadTimeRequest = item.LeadTimeRequest;
                    entity.QuantityReturn = item.QuantityReturn;
                    entity.NCCFinal = item.NCCFinal;
                    entity.PriceOrder = item.PriceOrder;
                    entity.OrderDate = item.OrderDate;
                    entity.ExpectedReturnDate = item.ExpectedReturnDate;
                    entity.Status = item.Status;
                    entity.Quality = item.Quality;
                    entity.Note = item.Note;
                    entity.ReasonProblem = item.ReasonProblem;

                    entity.IsProblem = request.IsProblem;

                    // --- 4.3 Kiểm tra new-code WinForms ---
                    entity.IsNewCode = true;

                    var code = (item.ProductCode ?? "").ToUpper();
                    var rowSuggest = suggest.FirstOrDefault(x => (x.ProductCode ?? "").ToUpper() == code);

                    if (rowSuggest != null)
                    {
                        if (((rowSuggest.Maker ?? "").ToUpper() == (item.Manufacturer ?? "").ToUpper()) &&
                            ((rowSuggest.Unit ?? "").ToUpper() == (item.Unit ?? "").ToUpper()) &&
                            ((rowSuggest.ProductName ?? "").ToUpper() == (item.GroupMaterial ?? "").ToUpper()))
                        {
                            entity.IsNewCode = false;
                        }
                    }

                    // --- 4.4 Save ---
                    if (isNew)
                        await _projectPartlistRepo.CreateAsync(entity);
                    else
                        await _projectPartlistRepo.UpdateAsync(entity);

                    if (entity.IsNewCode == true)
                        newCodes.Add(entity);
                }

                // --- 5. Lấy diff mismatch giống WinForms ---
                object diffData = null;

                if (newCodes.Count > 0)
                {
                    string listIds = string.Join(",", newCodes.Select(x => x.ID));
                    var dt2 = SQLHelper<object>.ProcedureToList("spGetProjectParlistNotSame",
                        new string[] { "@ProjectParlistID" },
                        new object[] { listIds });

                    diffData = SQLHelper<object>.GetListData(dt2, 0);
                }

                return Ok(ApiResponseFactory.Success(new
                {
                    DiffData = diffData
                }, "Nhập dữ liệu excel thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="status =2 theo kho, =1 theo partlist"></param>
        /// <returns></returns>
        [HttpPost("save-import")]
        public async Task<IActionResult> SaveImport([FromBody] List<DataDiffSaveDTO> request, int status)
        {
            try
            {
                if (request == null || request.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu để cập nhật"));
                }

                foreach (var item in request)
                {
                    var partList = _projectPartlistRepo.GetByID(item.ID);
                    if (partList == null) continue;

                    // status = 1 → update theo partlist (dữ liệu gốc FE gửi lên)
                    if (status == 1)
                    {
                        partList.Manufacturer = item.Maker;
                        partList.Unit = item.Unit;
                        partList.GroupMaterial = item.ProductName;
                    }

                    // status = 2 → update theo kho (stock)
                    if (status == 2)
                    {
                        partList.Manufacturer = item.MakerStock;
                        partList.Unit = item.UnitStock;
                        partList.GroupMaterial = item.ProductNameStock;
                    }
                    await _projectPartlistRepo.UpdateAsync(partList);
                }

                return Ok(ApiResponseFactory.Success(null, "Cập nhật thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete-partlist")]
        public async Task<IActionResult> DeletePartList([FromBody] List<ProjectPartList> listItem)
        {
            try
            {
                foreach (var item in listItem)
                {
                    if (item.IsApprovedPurchase == true)
                    {
                        return Ok(new { status = 2, message = $"Vật tư TT {item.TT} đã được yêu cầu mua hàng. Vui lòng hủy yêu cầu mua trước" });
                    }
                    if (item.IsApprovedTBP == true)
                    {
                        return Ok(new { status = 2, message = $"Vật tư TT {item.TT} đã được TBP duyệt. Vui lòng hủy duyệt trước" });
                    }
                    item.IsDeleted = true;
                    await _projectPartlistRepo.UpdateAsync(item);
                }
                return Ok(ApiResponseFactory.Success(null, "Đã xóa thành công! "));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // yêu cầu xuất kho
        [HttpPost("request-export")]
        public async Task<IActionResult> RequestExport([FromBody] RequestExportRequestDTO request)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                // Validate warehouse
                var warehouse = _warehouseRepo.GetAll(x => x.WarehouseCode == request.WarehouseCode).FirstOrDefault();
                if (warehouse == null || warehouse.ID <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy kho!"));
                }

                // Validate list items
                if (request.ListItem == null || request.ListItem.Count <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn sản phẩm muốn yêu cầu xuất kho!"));
                }

                // Validate lần 1: Thu thập sản phẩm không đủ số lượng (productNewCodes)
                List<string> productNewCodes = new List<string>();
                foreach (var item in request.ListItem)
                {
                    if (!_projectPartlistRepo.ValidateKeep(item, warehouse.ID, out string productNewCode))
                    {
                        if (!string.IsNullOrWhiteSpace(productNewCode) && !productNewCodes.Contains(productNewCode))
                        {
                            productNewCodes.Add(productNewCode);
                        }
                    }
                }

                // Validate lần 2: Lấy danh sách ID hợp lệ
                List<int> validIds = new List<int>();
                foreach (var item in request.ListItem)
                {
                    if (!_projectPartlistRepo.ValidateKeep(item, warehouse.ID, out string productNewCode))
                    {
                        continue;
                    }

                    if (item.ID > 0)
                    {
                        validIds.Add(item.ID);
                    }
                }

                if (validIds.Count <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Các sản phẩm TT [{string.Join(";  ", productNewCodes)}] sẽ không được yêu cầu xuất kho vì không đủ số lượng!"));
                }

                // Warning message nếu có sản phẩm không đủ số lượng
                string warningMessage = "";
                if (productNewCodes.Count > 0)
                {
                    warningMessage = $"Các sản phẩm có mã nội bộ [{string.Join(";", productNewCodes)}] sẽ không được yêu cầu xuất kho vì không đủ số lượng!";
                }

                // Gọi stored procedure
                string idText = string.Join(",", validIds);
                var ds = SQLHelper<dynamic>.ProcedureToList("spGetProjectPartListByID_RequestExport",
                    new[] { "@ID" }, new object[] { idText });

                // Lấy data từ table đầu tiên
                var data = SQLHelper<dynamic>.GetListData(ds, 0);

                if (data == null || data.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy dữ liệu sản phẩm!"));
                }

                // Group theo ProductGroupID (giống distinctValues trong winform)
                var distinctGroups = data
                    .Select(x => (IDictionary<string, object>)x)
                    .Where(x => x.ContainsKey("ProductGroupID"))
                    .GroupBy(x => Convert.ToInt32(x["ProductGroupID"]))
                    .ToList();

                List<BillExportDataDTO> billsData = new List<BillExportDataDTO>();

                // Loop qua từng ProductGroupID
                for (int j = 0; j < distinctGroups.Count; j++)
                {
                    var group = distinctGroups[j];
                    int productGroupID = group.Key;
                    var groupDetails = group.ToList();

                    if (groupDetails.Count == 0) continue;

                    // Lấy row đầu tiên để lấy thông tin chung
                    var firstDetail = (IDictionary<string, object>)groupDetails[0];

                    // Lấy product group warehouse info
                    var dtGroupWarehouse = SQLHelper<dynamic>.ProcedureToList("spGetProductGroupWarehouse",
                        new[] { "@WarehouseID", "@ProductGroupID" },
                        new object[] { warehouse.ID, productGroupID });
                    var groupWarehouseList = SQLHelper<dynamic>.GetListData(dtGroupWarehouse, 0);

                    int senderID = 0;
                    int senderEmployeeID = 0;
                    if (groupWarehouseList != null && groupWarehouseList.Count > 0)
                    {
                        var groupWarehouseDict = (IDictionary<string, object>)groupWarehouseList[0];
                        senderID = groupWarehouseDict.ContainsKey("UserID") ? Convert.ToInt32(groupWarehouseDict["UserID"]) : 0;
                        senderEmployeeID = groupWarehouseDict.ContainsKey("EmployeeID") ? Convert.ToInt32(groupWarehouseDict["EmployeeID"]) : 0;
                    }

                    // Tạo bill object
                    BillExportRQPDTO billDTO = new BillExportRQPDTO
                    {
                        Code = _billExportRepo.GetBillCode(1),
                        Status = 6,
                        SenderID = senderID,
                        SenderEmployeeID = senderEmployeeID,
                        UserID = currentUser.ID,
                        WarehouseID = warehouse.ID,
                        RequestDate = DateTime.Now,
                        CustomerID = firstDetail.ContainsKey("CustomerID") ? Convert.ToInt32(firstDetail["CustomerID"]) : 0,
                        Address = firstDetail.ContainsKey("Address") ? firstDetail["Address"]?.ToString() : "",
                        KhoTypeID = productGroupID,
                        GroupID = productGroupID.ToString(),
                        WarehouseCode = warehouse.WarehouseCode,
                        IsPOKH = true
                    };

                    // Lấy tên ProductGroup
                    var productGroup = _productGroupRepo.GetByID(productGroupID);
                    if (productGroup != null)
                    {
                        billDTO.WarehouseType = productGroup.ProductGroupName;
                    }

                    // Tạo detail list
                    List<BillExportDetailRQPDTO> detailList = new List<BillExportDetailRQPDTO>();

                    for (int i = 0; i < groupDetails.Count; i++)
                    {
                        var detailDict = (IDictionary<string, object>)groupDetails[i];
                        int detailID = Convert.ToInt32(detailDict["ID"]);

                        // Tìm item tương ứng trong request.ListItem (giống TreeListNode)
                        var itemNode = request.ListItem.FirstOrDefault(x => x.ID == detailID);
                        if (itemNode == null) continue;

                        // Lấy các giá trị số lượng
                        decimal remainQuantity = itemNode.RemainQuantity;
                        decimal quantityReturn = itemNode.QuantityReturn;
                        decimal qtyFull = itemNode.QtyFull;

                        // Nếu đã xuất hết rồi thì bỏ qua
                        if (remainQuantity <= 0)
                            continue;

                        // Nếu số lượng trả <= 0 thì bỏ qua luôn
                        if (quantityReturn <= 0)
                            continue;

                        // Tính số lượng cần xuất (logic mới từ winform)
                        decimal qtyToExport = (quantityReturn >= qtyFull)
                            ? remainQuantity
                            : Math.Min(remainQuantity, quantityReturn);

                        BillExportDetailRQPDTO detailDTO = new BillExportDetailRQPDTO
                        {
                            STT = i + 1,
                            ChildID = i + 1,
                            ParentID = 0,

                            // Từ SP (dataRowDetail)
                            ProductID = detailDict.ContainsKey("ProductSaleID") ? Convert.ToInt32(detailDict["ProductSaleID"]) : 0,
                            ProductCode = detailDict.ContainsKey("ProductCode") ? detailDict["ProductCode"]?.ToString() : "",
                            ProductFullName = detailDict.ContainsKey("GroupMaterial") ? detailDict["GroupMaterial"]?.ToString() : "",
                            ProjectID = detailDict.ContainsKey("ProjectID") ? Convert.ToInt32(detailDict["ProjectID"]) : 0,
                            ProjectName = detailDict.ContainsKey("ProjectName") ? detailDict["ProjectName"]?.ToString() : "",
                            Note = detailDict.ContainsKey("Note") ? detailDict["Note"]?.ToString() : "",
                            TotalQty = detailDict.ContainsKey("QtyFull") ? Convert.ToDecimal(detailDict["QtyFull"]) : 0,
                            ProjectPartListID = detailID,

                            // Từ itemNode (giống node.GetValue)
                            ProductNewCode = itemNode.ProductNewCode,
                            ProductName = itemNode.GroupMaterial,
                            Unit = itemNode.Unit,
                            ProjectCodeText = itemNode.ProjectCode,
                            ProjectCodeExport = itemNode.ProjectCode,

                            // Số lượng - SỬ DỤNG qtyToExport thay vì remainQuantity
                            Qty = qtyToExport,

                            SerialNumber = ""
                        };

                        detailList.Add(detailDTO);
                    }

                    // Chỉ thêm nếu có detail (giống if (dtDetail.Rows.Count <= 0) continue)
                    if (detailList.Count > 0)
                    {
                        billsData.Add(new BillExportDataDTO
                        {
                            Bill = billDTO,
                            Details = detailList
                        });
                    }
                }

                // Trả về data cho FE
                return Ok(ApiResponseFactory.Success(new
                {
                    Bills = billsData,
                    Warning = warningMessage,
                    TotalBills = billsData.Count,
                }, "Đã lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [NonAction]
        public async Task UpdateAdditionPartListPO(
            int newVersionID,
            int oldVersionID,
            List<ProjectPartlistDTO> lstItem,
            int projectID,
            string reasonProblem)
        {

            try
            {
                // Lấy danh sách Partlist version cũ
                var data = SQLHelper<object>.ProcedureToList(
                    "spGetProjectPartList_Khanh",
                    new[] { "@ProjectID", "@PartListTypeID", "@IsDeleted", "@Keyword",
                        "@IsApprovedTBP", "@IsApprovedPurchase", "@ProjectPartListVersionID" },
                    new object[] { projectID, 0, -1, "", -1, -1, oldVersionID }
                );

                var oldPartlists = SQLHelper<ProjectPartlistDTO>.GetListData(data, 0);

                // ----------------------------------------------------
                // 1. SORT THEO TT để đảm bảo cha được insert trước con
                // ----------------------------------------------------
                lstItem = lstItem
                    .OrderBy(x => x.ID)
                    .ToList();

                bool hasInsert = false;

                // ----------------------------------------------------
                // 2. BẮT ĐẦU XỬ LÝ TỪNG NODE
                // ----------------------------------------------------
                foreach (var item in lstItem)
                {
                    var oldItem = oldPartlists.FirstOrDefault(x => x.ID == item.ID);
                    if (oldItem == null) continue;

                    string stt = oldItem.TT?.Trim();
                    if (string.IsNullOrEmpty(stt)) continue;

                    // Check tồn tại trong version PO
                    var existed = _projectPartlistRepo.GetAll(x =>
                        x.ProjectPartListVersionID == newVersionID &&
                        x.TT == stt &&
                        x.IsDeleted == false
                    ).FirstOrDefault();

                    if (existed != null) continue; // đã tồn tại → bỏ qua

                    // --------------------------------------------
                    // 3. Tạo mới bản ghi
                    // --------------------------------------------
                    ProjectPartList newPart = new ProjectPartList();

                    newPart.ProjectID = oldItem.ProjectID;
                    newPart.ProjectPartListVersionID = newVersionID;
                    newPart.TT = oldItem.TT.Trim();

                    // Lấy ParentID trong version mới dựa trên TT cha
                    newPart.ParentID = _projectPartlistRepo.GetParentIDAdditionalPO(stt, newVersionID, false);

                    // Copy thông tin còn lại
                    newPart.ProjectTypeID = oldItem.ProjectTypeID;
                    newPart.GroupMaterial = oldItem.GroupMaterial;
                    newPart.ProductCode = oldItem.ProductCode;
                    newPart.OrderCode = oldItem.OrderCode;
                    newPart.Manufacturer = oldItem.Manufacturer;
                    newPart.Model = oldItem.Model;
                    newPart.QtyMin = oldItem.QtyMin;
                    newPart.QtyFull = oldItem.QtyFull;
                    newPart.Unit = oldItem.Unit;
                    newPart.Price = oldItem.Price;
                    newPart.Amount = oldItem.Amount;
                    newPart.LeadTime = oldItem.LeadTime;
                    newPart.NCC = oldItem.NCC;
                    newPart.RequestDate = oldItem.RequestDate;
                    newPart.LeadTimeRequest = oldItem.LeadTimeRequest;
                    newPart.QuantityReturn = oldItem.QuantityReturn;
                    newPart.NCCFinal = oldItem.NCCFinal;
                    newPart.PriceOrder = oldItem.PriceOrder;
                    newPart.OrderDate = oldItem.OrderDate;
                    newPart.ExpectedReturnDate = oldItem.ExpectedReturnDate;
                    newPart.Status = oldItem.Status;
                    newPart.Quality = oldItem.Quality;
                    newPart.Note = oldItem.Note;
                    newPart.SpecialCode = oldItem.SpecialCode;
                    newPart.IsDeleted = oldItem.IsDeleted;

                    // --------------------------------------------
                    // 4. Gắn ReasonProblem nếu là node con
                    // --------------------------------------------
                    if (item.IsLeaf)
                    {
                        newPart.ReasonProblem = reasonProblem;
                        newPart.IsProblem = true;
                    }
                    else
                    {
                        newPart.ReasonProblem = "";
                        newPart.IsProblem = false;
                    }

                    await _projectPartlistRepo.CreateAsync(newPart);
                    hasInsert = true;
                }

                if (!hasInsert)
                    throw new Exception("Không có vật tư mới nào được bổ sung. Tất cả đều đã tồn tại trong PO.");

            }
            catch (Exception ex)
            {

                throw new Exception("Lỗi khi bổ sung PO: " + ex.Message);
            }
        }





        //lịch sử giá + sản phẩm trong kho 
        [HttpPost("history-partlist")]
        public async Task<IActionResult> HistoryPartList([FromBody] HistoryPartListRequestParam request)
        {
            try
            {
                var data = SQLHelper<dynamic>.ProcedureToList(
                    "spGetProductInventoryByKeyword",
                    ["@Keyword", "@ProductCode"],
                    [request.keyword ?? "", request.productCode]
                );

                var dt = SQLHelper<dynamic>.ProcedureToList(
                    "spGetHistoryPricePartlist_Khanh",
                    ["@Keyword", "@ProductCode"],
                    [request.keyword ?? "", request.productCode]
                );

                return Ok(ApiResponseFactory.Success(new { data, dt }, "Đã xử lý thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("additional-partlist-po")]
        public async Task<IActionResult> AdditionalPartListPO([FromBody] AdditionPartlistPoDTO request)
        {
            try
            {

                if (request.ListItem.Count <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn parartlist cần bổ sung"));
                }
                var version = _partlistVersionRepo.GetAll(x => x.ProjectSolutionID == request.ProjectSolutionID && x.StatusVersion == 2 && x.ProjectTypeID == request.ProjectTypeID).FirstOrDefault() ?? new ProjectPartListVersion();
                if (version.ID > 0)
                {
                    ProjectPartListVersion versionModel = _partlistVersionRepo.GetByID(request.VersionID);
                    await UpdateAdditionPartListPO(version.ID, versionModel.ID, request.ListItem, request.projectID, request.ReasonProblem);
                }
                else
                {
                    ProjectPartListVersion versionModel = _partlistVersionRepo.GetByID(request.VersionID);

                    ProjectPartListVersion newVersion = new ProjectPartListVersion();
                    newVersion.IsApproved = false;
                    newVersion.IsActive = false;
                    newVersion.StatusVersion = 2;
                    newVersion.ProjectID = versionModel.ProjectID;
                    newVersion.STT = versionModel.STT;
                    newVersion.Code = versionModel.Code;
                    newVersion.DescriptionVersion = versionModel.DescriptionVersion;
                    newVersion.ProjectSolutionID = versionModel.ProjectSolutionID;

                    newVersion.ProjectTypeID = versionModel.ProjectTypeID;
                    newVersion.ApprovedID = versionModel.ApprovedID;

                    await _partlistVersionRepo.CreateAsync(newVersion);
                    await UpdateAdditionPartListPO(newVersion.ID, versionModel.ID, request.ListItem, request.projectID, request.ReasonProblem);
                }
                return Ok(ApiResponseFactory.Success(null, "Đã xử lý thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("update-special-code")]
        public async Task<IActionResult> UpdateSpecialCode(int partlistId, string? specialCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(specialCode))
                {
                    /*  return BadRequest(ApiResponseFactory.Fail(null, "Mã đặc biệt không được để trống!"));*/
                    specialCode = "";
                }

                specialCode = specialCode.Trim();

                var partlistData = _projectPartlistRepo.GetByID(partlistId);
                if (partlistData == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy partList. Vui lòng kiểm tra lại!"));
                }

                var existsData = _projectPartlistRepo.GetAll(
                    x => x.SpecialCode.Trim().ToLower() == specialCode.ToLower()
                         && x.ID != partlistId);

                if (existsData.Count > 0 && !string.IsNullOrWhiteSpace(specialCode))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Mã đặc biệt [{specialCode}] đã tồn tại."));
                }

                partlistData.SpecialCode = specialCode;
                partlistData.SpecialCode = specialCode;
                await _projectPartlistRepo.UpdateAsync(partlistData);

                return Ok(ApiResponseFactory.Success(null, "Cập nhật mã đặc biệt thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("convert-versionPO")]
        public async Task<IActionResult> ConvertVersionPo([FromBody] ConvertVersionPODTO request)
        {
            try
            {
                var versions = _partlistVersionRepo.GetAll(x => x.ProjectTypeID == request.ProjectTypeID && x.StatusVersion == 2 && x.ProjectSolutionID == request.ProjectSolutionID && x.IsDeleted == false);
                if (versions.Count > 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Danh mục [{request.ProjectTypeName}] đã có phiên bản PO!"));
                }
                else
                {
                    ProjectPartListVersion versionModel = _partlistVersionRepo.GetByID(request.ID);
                    ProjectPartListVersion newVersion = new ProjectPartListVersion();
                    newVersion.IsApproved = false;
                    newVersion.IsActive = false;
                    newVersion.StatusVersion = 2;
                    newVersion.ProjectID = versionModel.ProjectID;
                    newVersion.STT = versionModel.STT;
                    newVersion.Code = versionModel.Code;
                    newVersion.DescriptionVersion = versionModel.DescriptionVersion;
                    newVersion.ProjectSolutionID = versionModel.ProjectSolutionID;
                    newVersion.ProjectTypeID = versionModel.ProjectTypeID;
                    newVersion.ApprovedID = versionModel.ApprovedID;

                    await _partlistVersionRepo.CreateAsync(newVersion);

                    await UpdatePartList(newVersion.ID, versionModel.ID, request.ProjectID ?? 0);


                }
                return Ok(ApiResponseFactory.Success(null, "Đã xử lý thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [NonAction]
        public async Task UpdatePartList(int newVersionID, int oldVersionID, int projectID)
        {
            try
            {
                List<ConvertPartListPODTO> listPartlists = SQLHelper<ConvertPartListPODTO>.ProcedureToListModel(
                        "spGetProjectPartList_Khanh",
                        new string[] { "@ProjectID", "@PartListTypeID", "@IsDeleted", "@Keyword", "@IsApprovedTBP", "@IsApprovedPurchase", "@ProjectPartListVersionID" },
                        new object[] { projectID, 0, -1, " ", -1, -1, oldVersionID }
                    );
                Regex regex = new Regex(@"^-?[\d\.]+$");
                foreach (ConvertPartListPODTO item in listPartlists)
                {
                    string stt = item.TT;

                    if (string.IsNullOrEmpty(stt)) continue;
                    if (!regex.IsMatch(stt)) continue;
                    //if (isUpdatePartList && partlists.Any(x => x.TT == stt)) continue;

                    ProjectPartList partList = new ProjectPartList();
                    partList.ProjectID = item.ProjectID;
                    partList.TT = stt;

                    partList.ParentID = _projectPartlistRepo.GetParentIDAdditionalPO(stt, newVersionID, false);


                    partList.ProjectTypeID = item.ProjectTypeID;
                    partList.ProjectPartListVersionID = newVersionID;

                    partList.GroupMaterial = item.GroupMaterial;
                    partList.ProductCode = item.ProductCode;
                    partList.OrderCode = item.OrderCode;
                    partList.Manufacturer = item.Manufacturer;
                    partList.Model = item.Model;
                    partList.QtyMin = item.QtyMin;
                    partList.QtyFull = item.QtyFull;
                    partList.Unit = item.Unit;
                    partList.Price = item.Price.HasValue? Convert.ToDecimal(item.Price.Value): 0m; ;//<= 0 ? item.UnitPriceQuote : item.Price;
                    partList.Amount = item.Amount;// partList.Price * partList.QtyFull;
                    partList.LeadTime = item.LeadTime; // tien do
                    partList.NCC = item.NCC;
                    partList.RequestDate = item.RequestDate;
                    partList.LeadTimeRequest = item.LeadTimeRequest;
                    partList.QuantityReturn = item.QuantityReturn;
                    partList.NCCFinal = item.NCCFinal;
                    partList.PriceOrder = item.PriceOrder;
                    partList.OrderDate = item.OrderDate;
                    partList.ExpectedReturnDate = item.ExpectedReturnDate;
                    partList.Status = item.Status;
                    partList.Quality = item.Quality;
                    partList.Note = item.Note;
                    partList.ReasonProblem = item.ReasonProblem;
                    partList.IsProblem = item.IsProblem;
                    partList.IsDeleted = item.IsDeleted;

                    partList.SpecialCode = item.SpecialCode;
                    if (partList.ID > 0)
                    {
                        await _projectPartlistRepo.UpdateAsync(partList);

                    }
                    else
                    {
                        await _projectPartlistRepo.CreateAsync(partList);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi update dữ liệu PO: " + ex.Message);
            }

        }

    }

}