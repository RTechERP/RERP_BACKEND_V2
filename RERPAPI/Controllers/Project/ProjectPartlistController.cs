using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.Record.Chart;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Finance.Implementations;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo;
using RERPAPI.Repo.GenericEntity;
using System.Diagnostics.Metrics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ZXing.OneD.RSS;

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
        UnitCountKTRepo _unitCountKTRepo;
        public ProjectPartlistController(ProjectPartListRepo projectPartlistRepo, ProductSaleRepo productSaleRepo, FirmRepo firmRepo, UnitCountKTRepo unitCountRepo, ProductRTCRepo productRTCRepo, ProjectPartlistPriceRequestRepo priceRequestRepo, ProjectPartlistVersionRepo partlistVersionRepo, ProjectPartlistPurchaseRequestRepo partlistPurchaseRequestRepo, UnitCountKTRepo unitCountKTRepo)
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
        /*[HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] List<ProjectPartList> lstRequest)
        {
            try
            {
                foreach (var request in lstRequest)
                {
                    string message = string.Empty;

                    // validate cơ bản
                    if (!_projectPartlistRepo.Validate(request, out message))
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, message));
                    }
                    //ProjectPartList oldPartlist = _projectPartlistRepo.GetByID(request.ID);
                    //if (request.is == false && oldPartlist.IsApprovedPurchase==true)
                    //{
                    //    var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                    //    var currentUser = ObjectMapper.GetCurrentUser(claims);
                    //    ProjectPartlistPriceRequest priceRequestExist = _priceRequestRepo.GetAll(x=>x.ProjectPartListID == request.ID && x.IsDeleted == false).FirstOrDefault() ?? new ProjectPartlistPriceRequest();
                    //    if(priceRequestExist.EmployeeID != currentUser.EmployeeID)
                    //    {
                    //        return BadRequest(ApiResponseFactory.Fail(null, "Bạn không thể huỷ Y/c mua hafb của người khác!"));
                    //    }
                    //}
                    // Trường hợp duyệt TBP
                    //if (request.Mode == 1)
                    //{
                    //    if (!_projectPartlistRepo.ValidateApproveTBP(request, request.IsApprovedTBP ?? false, out message))
                    //    {
                    //        return BadRequest(ApiResponseFactory.Fail(null, message));
                    //    }

                    //    // Cập nhật duyệt TBP
                    //    ProjectPartList partlist = _projectPartlistRepo.GetByID(request.ID);
                    //    partlist.IsApprovedTBP = request.IsApprovedTBP;
                    //    await _projectPartlistRepo.UpdateAsync(partlist);

                    //    return Ok(ApiResponseFactory.Success(partlist, $"Đã {(request.IsApprovedTBP == true ? "duyệt" : "hủy duyệt")} thành công!"));
                    //}

                    //// Trường hợp duyệt NewCode
                    //if (request.Mode == 2)
                    //{
                    //    List<Firm> firms = _firmRepo.GetAll(x => x.FirmType == 1 && x.IsDelete == false);
                    //    List<Firm> firmDemos = _firmRepo.GetAll(x => x.FirmType == 2 && x.IsDelete == false);
                    //    if (!_projectPartlistRepo.ValidateProduct(request, out message))
                    //    {
                    //        return BadRequest(ApiResponseFactory.Fail(null, message));
                    //    }
                    //    var productSales = _productSaleRepo.GetAll(x => x.IsDeleted == false
                    //                                                && x.ProductCode == request.ProductCode
                    //                                                && x.IsFix == true)
                    //                                        .FirstOrDefault() ?? new ProductSale();
                    //    if (productSales.IsFix == true) continue;//Nếu mã hàng đã được tích xanh
                    //    ProjectPartList partlist = _projectPartlistRepo.GetByID(request.ID);
                    //    partlist.IsApprovedTBPNewCode = request.IsApprovedTBPNewCode;
                    //    partlist.DateApprovedNewCode = DateTime.Now;
                    //    await _projectPartlistRepo.UpdateAsync(partlist);
                    //    Firm firm = firms.FirstOrDefault(x => x.FirmName.ToUpper().Trim() == request.Manufacturer.ToUpper().Trim() && x.FirmType == 2) ?? new Firm();
                    //    Firm firmDemo = firmDemos.FirstOrDefault(x => x.FirmName.ToUpper().Trim() == request.Manufacturer.ToUpper().Trim() && x.FirmType == 2) ?? new Firm();
                    //    var myDict = new Dictionary<Expression<Func<ProductSale, object>>, object>
                    //    {
                    //        { x => x.FirmID, firm.ID },
                    //        { x => x.Unit, request.Unit},
                    //        { x => x.ProductName, request.GroupMaterial},
                    //        { x => x.Maker, request.Manufacturer},
                    //    };
                    //    await _productSaleRepo.UpdateFieldByAttributeAsync(x => x.ProductCode == request.ProductCode, myDict);
                    //    UnitCountKT unitcount = _unitCountRepo.GetAll(x => x.UnitCountName.ToUpper().Trim() == request.Unit.ToUpper().Trim()).FirstOrDefault() ?? new UnitCountKT();
                    //    var myDicDemo = new Dictionary<Expression<Func<ProductRTC, object>>, object>
                    //    {
                    //        { x => x.FirmID, firmDemo.ID },
                    //        //{ x => x.Unit, unitcount.UnitCountName},
                    //        { x => x.ProductName, request.GroupMaterial},
                    //        { x => x.Maker, request.Manufacturer},
                    //    };
                    //    await _productRTCRepo.UpdateFieldByAttributeAsync(x => x.ProductCode == request.ProductCode, myDicDemo);
                    //    var myDictPriceRequest = new Dictionary<Expression<Func<ProjectPartlistPriceRequest, object>>, object>
                    //    {
                    //        { x => x.ProductName, request.GroupMaterial },
                    //        { x => x.Maker, request.Manufacturer },
                    //    };
                    //    await _priceRequestRepo.UpdateFieldByAttributeAsync(x => x.ProductCode == request.ProductCode, myDictPriceRequest);
                    //    var myDictPartlist = new Dictionary<Expression<Func<ProjectPartList, object>>, object>
                    //    {
                    //        { x => x.Unit, request.Unit },
                    //        { x => x.GroupMaterial, request.GroupMaterial },
                    //        { x => x.Manufacturer, request.Manufacturer },
                    //    };
                    //    await _projectPartlistRepo.UpdateFieldByAttributeAsync(x => x.ProductCode == request.ProductCode, myDictPartlist);
                    //    return Ok(ApiResponseFactory.Success(partlist, $"Đã {(request.IsApprovedTBPNewCode == true ? "duyệt mã mới" : "hủy duyệt mã mới")} thành công!"));
                    //}

                    request.ParentID = _projectPartlistRepo.getParentID(request.TT ?? "", request.ProjectTypeID ?? 0, request.ProjectPartListVersionID ?? 0);

                    if (request.ID <= 0)
                    {
                        request.IsApprovedPurchase = false;
                        request.IsApprovedTBP = false;
                        await _projectPartlistRepo.CreateAsync(request);
                    }
                    else
                    {
                        ProjectPartList partlistProblem = _projectPartlistRepo.GetByID(request.ID);
                        if (request.IsProblem == true && partlistProblem.IsProblem == false)
                        {
                            request.IsApprovedPurchase = false;
                            request.IsApprovedTBP = false;
                            request.StatusPriceRequest = 0;
                            request.DatePriceRequest = null;
                            request.DeadlinePriceRequest = null;
                            request.IsApprovedPurchase = false;
                            request.RequestDate = null;
                            request.ExpectedReturnDate = null;
                            request.Status = 2;
                            request.ID = 0;
                            await _projectPartlistRepo.CreateAsync(request);
                        }
                        else
                        {
                            await _projectPartlistRepo.UpdateAsync(request);
                        }
                    }
                }
                return Ok(ApiResponseFactory.Success(lstRequest, "Lưu dữ liệu thành công!"));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi:{ex.Message}"));
            }
        }
*/
        //public async Task<IActionResult> UpdateRequestQuote(ProjectPartList oldPartlist, ProjectPartList newPartlist)
        //{
        //    var resultCompare = DeepEquals.DeepEqual(oldPartlist, newPartlist);
        //    bool equal = (bool)resultCompare.GetType().GetProperty("equal").GetValue(resultCompare);
        //    if (!equal)
        //    {
        //        List<string> propertys = (List<string>)resultCompare.GetType().GetProperty("property").GetValue(resultCompare);

        //        propertys = propertys.Where(p => !p.Contains("TT")).ToList();

        //        if (propertys.Count <= 0) return BadRequest(ApiResponseFactory.Fail(null, "không tìm thấy property của class này!"));
        //    }
        //    List<ProjectPartlistPriceRequest> listQuotes = _priceRequestRepo.GetAll(x => x.ProjectPartListID == oldPartlist.ID && x.IsDeleted == false).ToList();
        //    if (listQuotes.Count > 0)
        //    {
        //        var myDictPriceRequest = new Dictionary<Expression<Func<ProjectPartlistPriceRequest, object>>, object>
        //            {
        //                { x => x.IsDeleted, 1},
        //            };
        //        await _priceRequestRepo.UpdateFieldByAttributeAsync(x => x.ProjectPartListID == oldPartlist.ID, myDictPriceRequest);
        //    }

        //}
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
                //-------------------------------------------------------------------
                // 1. ÁP DỤNG DIFF
                //-------------------------------------------------------------------
                //if (request.Diffs != null && request.Diffs.Any())
                //{
                //    foreach (var diff in request.Diffs)
                //    {
                //        var row = request.Items.FirstOrDefault(x => x.ProductCode == diff.ProductCode);
                //        if (row == null) continue;

                //        if (diff.Choose == "Stock")
                //        {
                //            row.GroupMaterial = diff.GroupMaterialStock;
                //            row.Manufacturer = diff.ManufacturerStock;
                //            row.Unit = diff.UnitStock;
                //        }
                //    }
                //}

                //-------------------------------------------------------------------
                // 2. UPDATE STOCK (NẾU CHECKISSTOCK = TRUE)
                //-------------------------------------------------------------------
                if (request.CheckIsStock == true)
                {
                    foreach (var item in request.Items)
                    {
                        var fixedProduct = _productSaleRepo
                            .GetAll(x => x.ProductCode == item.ProductCode && x.IsFix==true && x.IsDeleted==false)
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
                            x.IsDeleted==false)
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

    }

}