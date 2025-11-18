using Microsoft.AspNetCore.Mvc;
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

namespace RERPAPI.Controllers.Old.ProjectManager
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
        private readonly ProjectPartListRepo _projectPartlistRepo;
        public ProjectPartlistController(ProjectPartListRepo projectPartlistRepo, ProductSaleRepo productSaleRepo, FirmRepo firmRepo, UnitCountKTRepo unitCountRepo, ProductRTCRepo productRTCRepo, ProjectPartlistPriceRequestRepo priceRequestRepo)
        {
            _projectPartlistRepo = projectPartlistRepo;
            _productSaleRepo = productSaleRepo;
            _firmRepo = firmRepo;
            _unitCountRepo = unitCountRepo;
            _productRTCRepo = productRTCRepo;
            _priceRequestRepo = priceRequestRepo;
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
        [HttpPost("save-data")]
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

                    request.ParentID = _projectPartlistRepo.getParentID(request.TT ??"", request.ProjectTypeID ?? 0, request.ProjectPartListVersionID ?? 0);

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
    }
}