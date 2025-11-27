using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
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
using System.Diagnostics.Metrics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ZXing;
using ZXing.OneD.RSS;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
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
        UnitCountKTRepo _unitCountKTRepo;
        public ProjectPartlistController(ProjectPartListRepo projectPartlistRepo, ProductSaleRepo productSaleRepo, FirmRepo firmRepo, UnitCountKTRepo unitCountRepo, ProductRTCRepo productRTCRepo, ProjectPartlistPriceRequestRepo priceRequestRepo, ProjectPartlistVersionRepo partlistVersionRepo, ProjectPartlistPurchaseRequestRepo partlistPurchaseRequestRepo, UnitCountKTRepo unitCountKTRepo, WarehouseRepo warehouseRepo, BillExportRepo billExportRepo, ProductGroupRepo productGroupRepo)
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
                        return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy danh mục vật tư"));
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
                        return Ok(new { status = 2, message = $"Vật tư Stt [{item.STT}] đã được Y/c báo giá.\nVui lòng kiểm tra lại!" });
                    }
                }

                // ===== LOOP 2: XỬ LÝ =====
                foreach (var item in request)
                {
                    if (item.ID <= 0) continue;
                    if (item.StatusPriceRequest > 0) continue;

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
                        StatusRequest = item.StatusPriceRequest + 1,
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
                        return Ok(new { status = 2, message = $"Phòng mua đàng check giá sản phẩm Stt [{item.STT}].\nBạn không thể hủy y/c báo giá " });
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
                return Ok(ApiResponseFactory.Success(null, "hủy duyệt thành công!"));
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
                        if (!_projectPartlistRepo.CheckValidate(data, out messageError)) { return Ok(new { status = 2, message = messageError }); }
                    }
                    if (!item.IsLeaf) continue;


                    if (item.IsDeleted == true)
                    {
                        return Ok(new { status = 2, message = $"Không thể {approvedText} vì vật tư thứ tự [{item.TT}] đã bị xóa!" });
                    }
                    //validate duyệt
                    if (item.IsApprovedTBP == false && isApproved == true)
                    {
                        return Ok(new { status = 2, message = $"Không thể {approvedText} vì vật tư thứ tự [{item.TT}] chưa được TBP duyệt!" });
                    }
                    if (isApproved == true && item.IsApprovedTBPNewCode == false && item.IsNewCode == true)
                    {
                        return Ok(new { status = 2, message = $"Không thể {approvedText} vì vật tư thứ tự [{item.TT}] chưa được TBP duyệt mới!" });
                    }
                    if (isApproved == true && item.IsApprovedPurchase == true)
                    {
                        return Ok(new { status = 2, message = $"Vật tư thứ tự [{item.TT}] đã được Y/c mua.\nVui lòng kiểm tra lại!" });
                    }
                }
                List<ProjectPartList> listPartLists = new List<ProjectPartList>();

                if (isApproved)
                {
                    var version = _partlistVersionRepo.GetAll(x => x.ProjectTypeID == projectTypeID && x.ProjectSolutionID == projectSolutionID && x.StatusVersion == 1).FirstOrDefault();
                    if (version == null) version = new ProjectPartListVersion();

                    var dt = SQLHelper<dynamic>.ProcedureToList("spGetProjectPartList_Khanh", new string[] { "@ProjectID", "@PartListTypeID", "@IsDeleted", "@Keyword", "@IsApprovedTBP", "@IsApprovedPurchase", "@ProjectPartListVersionID" }, new object[] { projectID, projectTypeID, 0, " ", -1, -1, version.ID });
                    var s = SQLHelper<object>.GetListData(dt, 0);

                    foreach (var item in request)
                    {
                        if (!item.IsLeaf) continue;

                        ProjectPartList model = _projectPartlistRepo.GetByID(item.ID);
                        model.IsApprovedPurchase = isApproved;
                        model.RequestDate = DateTime.Now;
                        model.Status = 1;
                        model.ExpectedReturnDate = item.DeadlinePur;
                        //
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
                        var purchaseRequest = _partlistPurchaseRequestRepo.GetAll(x => x.ProjectPartListID == item.ID && x.IsDeleted == false && x.EmployeeIDRequestApproved > 0);
                        if (purchaseRequest.Count > 0)
                        {
                            return Ok(new { status = 2, message = $"Vật tư thứ tự [{item.TT}] đang được check đặt hàng.Bạn không thể hủy.\nVui lòng liên hệ nhân viên Pur để hủy!" });
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
                _projectPartlistRepo.UpdatePurchaseRequest(listPartLists);
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
                string approvedText = isApprovedNew ? "duyệt" : "hủy duyệt";
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

                    if (productSales?.IsFix == true && isApprovedNew) continue;

                    // Update ProjectPartList
                    ProjectPartList model = _projectPartlistRepo.GetByID(item.ID);
                    model.IsApprovedTBPNewCode = isApprovedNew;
                    model.DateApprovedNewCode = DateTime.Now;
                    model.EmployeeApprovedNewCode = currentUser.EmployeeID;
                    await _projectPartlistRepo.UpdateAsync(model);

                    // Update ProductSale
                    Firm firm = firms.FirstOrDefault(x => x.FirmName.ToUpper().Trim() == item.Manufacturer
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
                string approvedText = isFix ? "duyệt" : "hủy duyệt";
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
                    if (!_projectPartlistRepo.ValidateProduct(item, out messageErr))
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

                    var productSale = _productSaleRepo.GetAll().FirstOrDefault(x => x.ProductCode == item.ProductCode);
                    if (productSale == null) continue;

                    // Tìm Firm theo WinForm
                    var firm = firmDemos
                        .FirstOrDefault(x => x.FirmName.ToUpper().Trim() == (item.Manufacturer ?? "").ToUpper().Trim())
                        ?? new Firm();

                    if (isFix)
                    {
                        // Giống WinForm UpdateProductSale
                        productSale.ProductName = item.GroupMaterial;
                        productSale.Maker = item.Manufacturer;
                        productSale.Unit = item.Unit;
                        productSale.IsFix = true;
                        productSale.FirmID = firm.ID;
                    }
                    else
                    {
                        productSale.IsFix = false;
                    }
                    await _productSaleRepo.UpdateAsync(productSale);
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
        public async Task<IActionResult> SaveDataAsync(ProjectPartList partList)
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
                return BadRequest(ApiResponseFactory.Fail(ex, "Lỗi khi lưu phiên bản danh sách phần"));
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
        private ProjectPartList BuildProjectPartListEntity(
    ProjectPartlistImportRowDto item, PartlistImportRequestDTO request, int parentID)
        {
            return new ProjectPartList
            {
                TT = item.TT,
                GroupMaterial = item.GroupMaterial,
                ProductCode = item.ProductCode,
                OrderCode = item.OrderCode,
                Manufacturer = item.Manufacturer,
                Model = item.Model,
                QtyMin = item.QtyMin,
                QtyFull = item.QtyFull,
                Unit = item.Unit,
                Price = item.Price,
                Amount = item.Amount,
                LeadTime = item.LeadTime,
                NCC = item.NCC,
                RequestDate = item.RequestDate,
                LeadTimeRequest = item.LeadTimeRequest,
                QuantityReturn = item.QuantityReturn,
                NCCFinal = item.NCCFinal,
                PriceOrder = item.PriceOrder,
                OrderDate = item.OrderDate,
                ExpectedReturnDate = item.ExpectedReturnDate,
                Status = item.Status,
                Quality = item.Quality,
                Note = item.Note,
                ReasonProblem = item.ReasonProblem,

                ProjectID = request.ProjectID,
                ProjectPartListVersionID = request.ProjectPartListVersionID,
                ProjectTypeID = request.ProjectTypeID,
                ParentID = parentID,

                IsDeleted = false,
                IsApprovedTBP = false,
                IsApprovedPurchase = false
            };
        }
        [HttpPost("import-check")]
        public IActionResult ImportCheck([FromBody] PartlistImportRequestDTO request)
        {
            try
            {
                // Gọi Validate2 trong repo
                var result = _projectPartlistRepo.Validate2(request);

                if (!result.IsValid)
                {
                    return Ok(new
                    {
                        success = false,
                        message = result.Message,
                        //diffs = result.Diffs,
                        //needConfirm = result.Diffs.Any()
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Dữ liệu hợp lệ!",
                    needConfirm = false,
                    //diffs = new List<PartlistDiffDTO>()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("apply-diff")]
        public async Task<IActionResult> ApplyDiff([FromBody] PartlistImportRequestDTO request)
        {
            try
            {
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

                //-------------------------------------------------------------------
                // 2. UPDATE STOCK (NẾU CHECKISSTOCK = TRUE)
                //-------------------------------------------------------------------
                if (request.CheckIsStock == true)
                {
                    foreach (var item in request.Items)
                    {
                        var fixedProduct = _productSaleRepo
                            .GetAll(x => x.ProductCode == item.ProductCode && x.IsFix == true && x.IsDeleted == false)
                            .FirstOrDefault();

                        // CHỈ UPDATE KHI KHÔNG PHẢI FIXED
                        if (fixedProduct == null) continue;

                        // Update ProductSale (Kho chính)
                        var dictProduct = new Dictionary<Expression<Func<ProductSale, object>>, object>
                {
                    { x => x.ProductName, item.GroupMaterial },
                    { x => x.Maker, item.Manufacturer },
                    { x => x.Unit, item.Unit }
                };
                        await _productSaleRepo.UpdateFieldByAttributeAsync(
                            x => x.ProductCode == item.ProductCode, dictProduct);

                        // Update ProductRTC (kho demo)
                        var dictRTC = new Dictionary<Expression<Func<ProductRTC, object>>, object>
                {
                    { x => x.ProductName, item.GroupMaterial },
                    { x => x.Maker, item.Manufacturer }
                };
                        await _productRTCRepo.UpdateFieldByAttributeAsync(
                            x => x.ProductCode == item.ProductCode, dictRTC);

                        // Update PriceRequest
                        var dictPriceReq = new Dictionary<Expression<Func<ProjectPartlistPriceRequest, object>>, object>
                {
                    { x => x.ProductName, item.GroupMaterial },
                    { x => x.Maker, item.Manufacturer }
                };
                        await _priceRequestRepo.UpdateFieldByAttributeAsync(
                            x => x.ProductCode == item.ProductCode, dictPriceReq);

                        // Update tất cả Partlist trước đó có cùng ProductCode
                        var dictPartlist = new Dictionary<Expression<Func<ProjectPartList, object>>, object>
                {
                    { x => x.Unit, item.Unit },
                    { x => x.GroupMaterial, item.GroupMaterial },
                    { x => x.Manufacturer, item.Manufacturer }
                };
                        await _projectPartlistRepo.UpdateFieldByAttributeAsync(
                            x => x.ProductCode == item.ProductCode, dictPartlist);
                    }
                }

                //-------------------------------------------------------------------
                // 3. INSERT/UPDATE PARTLIST
                //-------------------------------------------------------------------
                foreach (var item in request.Items)
                {
                    // Tính ParentID
                    int parentID = _projectPartlistRepo.GetParentID(
                        item.TT, request.ProjectTypeID, request.ProjectPartListVersionID);

                    // Kiểm tra xem dòng có tồn tại chưa
                    var existing = _projectPartlistRepo
                        .GetAll(x =>
                            x.ProjectID == request.ProjectID &&
                            x.ProjectPartListVersionID == request.ProjectPartListVersionID &&
                            x.ProductCode == item.ProductCode &&
                            x.TT == item.TT &&
                            x.IsDeleted == false)
                        .FirstOrDefault();

                    //-------------------------------------------------------------------
                    // 3.1 TRƯỜNG HỢP PHÁT SINH (IsProblem = true)
                    // WinForms: nếu phát sinh → tạo bản ghi mới
                    //-------------------------------------------------------------------
                    if (request.IsProblem == true)
                    {
                        var entityNew = BuildProjectPartListEntity(item, request, parentID);
                        entityNew.ID = 0;
                        entityNew.IsProblem = true;
                        entityNew.Status = 2;
                        entityNew.IsApprovedPurchase = false;
                        entityNew.IsApprovedTBP = false;

                        await _projectPartlistRepo.CreateAsync(entityNew);
                        continue;
                    }

                    //-------------------------------------------------------------------
                    // 3.2 UPDATE NẾU TỒN TẠI
                    //-------------------------------------------------------------------
                    if (existing != null)
                    {
                        var updateEntity = BuildProjectPartListEntity(item, request, parentID);
                        updateEntity.ID = existing.ID;

                        await _projectPartlistRepo.UpdateAsync(updateEntity);
                    }
                    else
                    {
                        //-------------------------------------------------------------------
                        // 3.3 INSERT MỚI
                        //-------------------------------------------------------------------
                        var newEntity = BuildProjectPartListEntity(item, request, parentID);
                        await _projectPartlistRepo.CreateAsync(newEntity);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Import & Áp dụng Diff thành công!"));
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
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có sản phẩm hợp lệ để xuất kho!"));
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
                    var productGroup =  _productGroupRepo.GetByID(productGroupID);
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
    }

}