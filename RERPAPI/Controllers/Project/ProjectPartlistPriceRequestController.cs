using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    //[ApiKeyAuthorize]
    [Authorize]
    public class ProjectPartlistPriceRequestController : ControllerBase
    {
        #region Khai báo repository
        ProjectRepo projectRepo;
        POKHRepo pOKHRepo;
        ProjectPartlistPriceRequestRepo requestRepo;
        ProductSaleRepo productSaleRepo;
        CurrencyRepo currencyRepo;
        SupplierSaleRepo supplierSaleRepo;
        ProjectSolutionRepo projectSolutionRepo;
        EmployeeSendEmailRepo employeeSendEmailRepo;
        ProjectPartlistPriceRequestTypeRepo projectPartlistPriceRequestTypeRepo;
        ProjectPartlistPriceRequestNoteRepo projectPartlistPriceRequestNoteRepo;
        ProjectPartlistPurchaseRequestRepo _projectPartlistPurchaseRequestRepo;
        UnitCountRepo _unitCountRepo;
        ProductRTCRepo _productRTCRepo;


        public ProjectPartlistPriceRequestController(ProjectRepo projectRepo, POKHRepo pOKHRepo, ProjectPartlistPriceRequestRepo requestRepo, ProductSaleRepo productSaleRepo, CurrencyRepo currencyRepo, SupplierSaleRepo supplierSaleRepo, ProjectSolutionRepo projectSolutionRepo, EmployeeSendEmailRepo employeeSendEmailRepo, ProjectPartlistPriceRequestTypeRepo projectPartlistPriceRequestTypeRepo, ProjectPartlistPriceRequestNoteRepo projectPartlistPriceRequestNoteRepo, ProjectPartlistPurchaseRequestRepo projectPartlistPurchaseRequestRepo, UnitCountRepo unitCountRepo, ProductRTCRepo productRTCRepo)
        {
            this.projectRepo = projectRepo;
            this.pOKHRepo = pOKHRepo;
            this.requestRepo = requestRepo;
            this.productSaleRepo = productSaleRepo;
            this.currencyRepo = currencyRepo;
            this.supplierSaleRepo = supplierSaleRepo;
            this.projectSolutionRepo = projectSolutionRepo;
            this.employeeSendEmailRepo = employeeSendEmailRepo;
            this.projectPartlistPriceRequestTypeRepo = projectPartlistPriceRequestTypeRepo;
            this.projectPartlistPriceRequestNoteRepo = projectPartlistPriceRequestNoteRepo;
            _projectPartlistPurchaseRequestRepo = projectPartlistPurchaseRequestRepo;
            _unitCountRepo = unitCountRepo;
            _productRTCRepo = productRTCRepo;
        }

        #endregion
        #region Lấy tất cả yêu cầu báo giá
        /// <summary>
        /// lấy tất cả yêu cầu báo giá
        /// </summary>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <param name="statusRequest"> 0:tất cả;1:Yêu cầu báo giá;2:Đã báo giá;3:Đã hoàn thành</param>
        /// <param name="projectId"></param>
        /// <param name="keyword"></param>
        /// <param name="isDeleted"></param>
        /// <param name="projectTypeID"></param>
        /// <param name="poKHID"></param>
        /// <param name="isCommercialProduct"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet("get-all-project-parList-price-request")]
        [RequiresPermission("N35,N33,N1,N36,N27,N69,N80")]
        public async Task<IActionResult> GetAll(DateTime dateStart, DateTime dateEnd, int statusRequest, int projectId, string? keyword, int employeeID,
            int isDeleted, int projectTypeID, int poKHID, int isJobRequirement = -1, int projectPartlistPriceRequestTypeID = -1, int isCommercialProduct = -1, int page = 1, int size = 25)
        {
            keyword = string.IsNullOrWhiteSpace(keyword) ? null : keyword.Trim();
            dateEnd = dateEnd.Date.AddDays(1).AddSeconds(-1);
            dateStart = dateStart.Date;

            List<List<dynamic>> dtPriceRequest = SQLHelper<dynamic>.ProcedureToList(
                "spGetProjectPartlistPriceRequest_New_Nhat",
                //"spGetProjectPartlistPriceRequest_New",
                new string[] {
                    "@DateStart", "@DateEnd", "@StatusRequest", "@ProjectID", "@Keyword", "@IsDeleted",
                    "@ProjectTypeID", "@IsCommercialProduct", "@IsJobRequirement",
                    "@ProjectPartlistPriceRequestTypeID", "@POKHID",
                    "@PageNumber", "@PageSize", "@EmployeeID"
                },
                new object[] {
                    dateStart, dateEnd, statusRequest, projectId, keyword, isDeleted,
                    projectTypeID, isCommercialProduct, isJobRequirement,
                    projectPartlistPriceRequestTypeID, poKHID,
                    page, size, employeeID
                }
            );

            var data = SQLHelper<dynamic>.GetListData(dtPriceRequest, 0);
            int totalPages = 1;
            if (data.Count > 0)
            {
                totalPages = data[0].TotalPage;

            }
            return Ok(new
            {
                status = 1,
                data = new
                {
                    dtData = data,
                    totalPages
                }
            });
        }
        [HttpGet("get-partlist")]
        public IActionResult GetProjectPartlists(
           DateTime dateStart, DateTime dateEnd, int statusRequest, int projectId, string? keyword, int employeeID,
            int isDeleted, int projectTypeID, int poKHID, int isJobRequirement = -1, int projectPartlistPriceRequestTypeID = -1, int isCommercialProduct = -1)
        {
            try
            {
                dateStart = dateStart.Date;
                dateEnd = dateEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                var dtPriceRequestResults = SQLHelper<dynamic>.ProcedureToList(
                    "spGetProjectPartlistPriceRequest_New",
                    new string[] {
                    "@DateStart", "@DateEnd", "@StatusRequest", "@ProjectID", "@Keyword",
                    "@IsDeleted", "@ProjectTypeID", "@IsCommercialProduct", "@POKHID",
                    "@IsJobRequirement", "@ProjectPartlistPriceRequestTypeID", "@EmployeeID"
                    },
                    new object[] {
                    dateStart, dateEnd, statusRequest, projectId, keyword ?? "", isDeleted,
                    projectTypeID, isCommercialProduct, poKHID,
                    isJobRequirement, projectPartlistPriceRequestTypeID, employeeID
                    }
                );

                var dt = SQLHelper<dynamic>.GetListData(dtPriceRequestResults, 0);
                return Ok(ApiResponseFactory.Success(dt, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
        [HttpGet("get-type")]
        public async Task<IActionResult> GetAllTypebyEmployeeID(int employeeID, int projectTypeID)
        {

            List<List<dynamic>> dtType = SQLHelper<dynamic>.ProcedureToList("spGetProjectTypeAssignByEmployeeID",
                                                        new string[] { "@EmployeeID", "@ProjectTypeID" },
                                                        new object[] { employeeID, projectTypeID });
            var data = SQLHelper<dynamic>.GetListData(dtType, 0);

            return Ok(new
            {
                status = 0,
                data = new
                {
                    dtType = data
                }
            });
        }
        [HttpGet("get-all-projects")]
        public async Task<IActionResult> GetAllProjects()
        {
            List<Model.Entities.Project> lstProjects = projectRepo.GetAll();
            return Ok(new { status = 1, data = lstProjects });
        }
        [HttpGet("get-all-employee")]
        public async Task<IActionResult> GetAllEmployee()
        {

            var employees = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployee",
                                            new string[] { "@Status" },
                                            new object[] { 0 });
            return Ok(new { status = 1, data = new { dtEmployee = employees } });
        }
        [HttpGet("get-po-code")]
        public async Task<IActionResult> GetPoCode()
        {
            var dtPOKH = pOKHRepo.GetAll(x => x.IsDeleted == false);
            return Ok(new { status = 1, data = dtPOKH });
        }
        [HttpGet("get-product-sale")]
        public async Task<IActionResult> GetProductSale()
        {
            var data = productSaleRepo.GetAll(x => x.IsDeleted == false);

            return Ok(new
            {
                status = 1,
                data,
            });
        }
        [HttpGet("get-currency")]
        public async Task<IActionResult> GetCurrency()
        {
            List<Currency> currencies = currencyRepo.GetAll(x => x.IsDeleted == false);

            return Ok(new { status = 1, data = currencies });
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] List<ProjectPartlistPriceRequest> projectPartlistPriceRequest)
        {
            try
            {
                List<ProjectPartlistPriceRequest> data = new List<ProjectPartlistPriceRequest>();
                if (projectPartlistPriceRequest != null && projectPartlistPriceRequest.Any())
                {
                    foreach (var item in projectPartlistPriceRequest)
                    {
                        if (item.ID > 0)
                        {
                            requestRepo.Update(item);
                        }
                        else
                        {
                            requestRepo.Create(item);
                        }

                        data.Add(item);
                    }
                }

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpGet("get-price-request-type")]
        public IActionResult GetPriceRequestType()
        {
            List<ProjectPartlistPriceRequestType> requestType = projectPartlistPriceRequestTypeRepo.GetAll();
            return Ok(ApiResponseFactory.Success(requestType, ""));
        }
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                var dt = SQLHelper<dynamic>.ProcedureToList("spGetProjectPartlistPriceRequest_New", new string[] { "@DateStart", "@DateEnd" },
                                                new object[] { new DateTime(2000, 1, 1), new DateTime(2000, 1, 1) });
                var data = SQLHelper<dynamic>.GetListData(dt, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Lấy nhà cung cấp
        [HttpGet("get-supplier-sale")]
        public async Task<IActionResult> GetSupplierSale()
        {
            try
            {
                var purchaseRequest = supplierSaleRepo.GetAll(x => x.IsDeleted == false)
                    .OrderBy(x => x.NgayUpdate)
                    .Select(x => new
                    {
                        x.ID,
                        x.CodeNCC,
                        x.NameNCC
                    })
                    .ToList();
                return Ok(ApiResponseFactory.Success(purchaseRequest, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-product-sale-by-id")]
        public IActionResult GetProductSaleByID(int id)
        {
            try
            {
                var productSale = productSaleRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(productSale, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-price-history-partlist")]
        [RequiresPermission("N38,N1,N79,N13,N82")]
        public async Task<IActionResult> GetPriceHistoryPartlist(int pageNumber, int pageSize, int projectId, int supplierSaleId, int employeeRequestId, string? keyword)
        {
            try
            {
                var priceHistoryPartlist = SQLHelper<object>.ProcedureToList("spGetHistoryPricePartlist",
                new string[] { "@Keyword", "@ProjectID", "@SupplierSaleID", "@EmployeeRequestID", "@PageSize", "@PageNumber" },
                new object[] { keyword ?? "", projectId, supplierSaleId, employeeRequestId, pageSize, pageNumber });
                var dt = SQLHelper<object>.GetListData(priceHistoryPartlist, 0);
                var totalpage = SQLHelper<object>.GetListData(priceHistoryPartlist, 1);
                return Ok(ApiResponseFactory.Success(new { dt, totalpage }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //[HttpPost("download")]
        //public async Task<IActionResult> DownloadFile([FromBody] DownloadProjectPartlistPriceRequestDTO request)
        //{
        //    var project = projectRepo.GetByID(request.ProjectId);
        //    if (project == null || project.CreatedDate == null)
        //        return BadRequest(new
        //        {
        //            status = 0,
        //            message = "Không tim thấy dự án!"
        //        });

        //    var solutions = SQLHelper<ProjectSolution>.ProcedureToList("spGetProjectSolutionByProjectPartListID",
        //                                                    new string[] { "@ProjectPartListID" }, new object[] { request.PartListId });

        //    if (solutions.Count <= 0)
        //        return BadRequest(new
        //        {
        //            status = 0,
        //            message = "Không tìm thấy giải pháp dự án!"
        //        });
        //    var solution = SQLHelper<dynamic>.GetListData(solutions, 0).FirstOrDefault();
        //    var year = project.CreatedDate.Value.Year;
        //    var pathPattern = $"{year}/{project.ProjectCode.Trim()}/THIETKE.Co/{solution?.CodeSolution.Trim()}/2D/GC/DH";
        //    var fileName = $"{request.ProductCode}.pdf";
        //    //sửa
        //    var fileUrl = $"http://113.190.234.64/:8081/api/project/{pathPattern}/{fileName}";

        //    try
        //    {
        //        using var httpClient = new HttpClient();
        //        var response = await httpClient.GetAsync(fileUrl);

        //        if (!response.IsSuccessStatusCode)
        //            return NotFound($"Không tìm thấy file: {fileUrl}");

        //        var stream = await response.Content.ReadAsStreamAsync();
        //        return File(stream, "application/pdf", fileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new
        //        {
        //            status = 0,
        //            message = ex.Message,
        //            error = ex.ToString()
        //        });
        //    }
        //}
        [HttpPost("download")]
        [RequiresPermission("N35,N1")]
        public async Task<IActionResult> DownloadFile([FromBody] DownloadProjectPartlistPriceRequestDTO request)
        {
            try
            {
                var project = projectRepo.GetByID(request.ProjectId);
                if (project == null || project.CreatedDate == null)
                    return BadRequest(new { status = 0, message = "Không tìm thấy dự án!" });

                var solutions = SQLHelper<ProjectSolution>.ProcedureToList(
                    "spGetProjectSolutionByProjectPartListID",
                    new[] { "@ProjectPartListID" }, new object[] { request.PartListId });

                if (solutions.Count <= 0)
                    return BadRequest(new { status = 0, message = "Không tìm thấy giải pháp dự án!" });

                var solution = SQLHelper<dynamic>.GetListData(solutions, 0).FirstOrDefault();
                var year = project.CreatedDate.Value.Year;

                var pathPattern = $"{year}/{project.ProjectCode.Trim()}/THIETKE.Co/{solution?.CodeSolution.Trim()}/2D/GC/DH";
                var fileName = $"{request.ProductCode}.pdf";

                // Lấy base URL real-time
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var fileUrl = $"{baseUrl}/api/api/share/duan/Projects/{pathPattern}/{fileName}";
                //var fileUrl = $"{baseUrl}/api/share/software/duan/Projects/{pathPattern}/{fileName}";

                return Ok(ApiResponseFactory.Success(fileUrl, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("check-price")]
        [RequiresPermission("N35,N1")]
        public async Task<IActionResult> CheckPrice(List<ProjectPartlistPriceRequest> lst)
        {
            try
            {
                if (lst == null || lst.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có yêu cầu nào để check giá!"));
                }
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                foreach (var item in lst)
                {
                    var exist = requestRepo.GetByID(item.ID);
                    if (currentUser.EmployeeID != item.QuoteEmployeeID && item.QuoteEmployeeID > 0) continue;
                    item.QuoteEmployeeID = item.IsCheckPrice == false ? 0 : item.QuoteEmployeeID;
                    item.UpdatedDate = exist.UpdatedDate;
                    await requestRepo.SaveData(item);

                }
                return Ok(ApiResponseFactory.Success(lst, "Cập nhật dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("cancel-price-request")]
        [RequiresPermission("N35,N1")]
        public async Task<IActionResult> CancelPriceRequest(List<ProjectPartlistPriceRequest> lst)
        {
            try
            {
                if (lst == null || lst.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có yêu cầu nào để hủy!"));
                }
                foreach (var item in lst)
                {
                    await requestRepo.SaveData(item);

                }
                return Ok(ApiResponseFactory.Success(lst, "Hủy yêu cầu báo giá thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("send-mail")]
        [RequiresPermission("N35,N1")]
        public async Task<IActionResult> SendMail(List<MailItemPriceRequestDTO> data)
        {
            try
            {
                await requestRepo.SendMail(data);
                return Ok(ApiResponseFactory.Success(data));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

        #region Update Price Request Status (Từ chối / Hủy từ chối báo giá)

        [HttpPost("update-price-request-status")]
        [RequiresPermission("N35,N1")]
        public async Task<IActionResult> UpdatePriceRequestStatus([FromBody] UpdatePriceRequestStatusRequestDTO request)
        {
            try
            {
                if (request == null || request.ListModel == null || !request.ListModel.Any())
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Danh sách yêu cầu báo giá không được để trống!"
                    });
                }

                var firstItem = request.ListModel.First();
                var newStatus = firstItem.StatusRequest ?? 0;
                var reasonUnPrice = firstItem.ReasonUnPrice ?? string.Empty;

                // --- VALIDATE TRẠNG THÁI ---
                // Chỉ chấp nhận 1 = Hủy từ chối, 5 = Từ chối
                if (newStatus != 1 && newStatus != 5)
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Trạng thái không hợp lệ! (1: Hủy từ chối, 5: Từ chối)"
                    });
                }

                // Nếu là từ chối (status = 5) → bắt buộc phải nhập lý do
                if (newStatus == 5 && string.IsNullOrWhiteSpace(reasonUnPrice))
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Vui lòng nhập lý do từ chối!"
                    });
                }

                var validIDs = new List<int>();
                var invalidProducts = new List<string>();

                foreach (var item in request.ListModel)
                {
                    var id = item.ID;
                    if (id <= 0)
                    {
                        invalidProducts.Add($"ID không hợp lệ");
                        continue;
                    }

                    var priceRequest = requestRepo.GetByID(id);

                    if (priceRequest == null)
                    {
                        invalidProducts.Add($"ID {id} không tồn tại");
                        continue;
                    }

                    // --- VALIDATE NGHIỆP VỤ---

                    if (newStatus == 5) // TỪ CHỐI
                    {
                        // Không cho từ chối lại khi đã từ chối
                        if (priceRequest.StatusRequest == 5)
                        {
                            invalidProducts.Add($"[{priceRequest.ProductCode}] đã bị từ chối trước đó");
                            continue;
                        }

                        // Không thể từ chối nếu đã báo giá
                        if (priceRequest.StatusRequest == 2)
                        {
                            invalidProducts.Add($"[{priceRequest.ProductCode}] đã ở trạng thái Đã báo giá, không thể từ chối");
                            continue;
                        }

                        // Không thể từ chối nếu đã hoàn thành
                        if (priceRequest.StatusRequest == 3)
                        {
                            invalidProducts.Add($"[{priceRequest.ProductCode}] đã hoàn thành, không thể từ chối");
                            continue;
                        }
                    }
                    else if (newStatus == 1) // HỦY TỪ CHỐI
                    {
                        if (priceRequest.StatusRequest != 5)
                        {
                            invalidProducts.Add($"[{priceRequest.ProductCode}] chưa bị từ chối");
                            continue;
                        }
                    }

                    validIDs.Add(id);
                }

                if (!validIDs.Any())
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Không có sản phẩm hợp lệ để cập nhật!",
                        invalidProducts
                    });
                }

                var updateCount = 0;
                foreach (var item in request.ListModel)
                {
                    if (!validIDs.Contains(item.ID)) continue;

                    var priceRequest = requestRepo.GetByID(item.ID);
                    if (priceRequest == null) continue;

                    priceRequest.StatusRequest = item.StatusRequest;
                    priceRequest.UpdatedBy = item.UpdatedBy;
                    priceRequest.UpdatedDate = DateTime.Now;
                    priceRequest.EmployeeIDUnPrice = item.EmployeeIDUnPrice;
                    priceRequest.ReasonUnPrice = item.ReasonUnPrice;

                    requestRepo.Update(priceRequest);
                    updateCount++;
                }

                // Gửi email nếu là từ chối
                if (newStatus == 5 && request.ListDataMail != null && request.ListDataMail.Any())
                {
                    try
                    {
                        await SendUnPriceEmail(
                            request.ListDataMail,
                            reasonUnPrice,
                            firstItem.EmployeeIDUnPrice ?? 0
                        );
                    }
                    catch (Exception emailEx)
                    {
                        Console.WriteLine("Lỗi gửi email: " + emailEx.Message);
                    }
                }

                var actionName = (newStatus == 5) ? "Từ chối báo giá" : "Hủy từ chối báo giá";

                return Ok(new
                {
                    status = 1,
                    message = $"{actionName} thành công {updateCount} sản phẩm!",
                    validCount = validIDs.Count,
                    invalidCount = invalidProducts.Count,
                    invalidProducts
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = "Có lỗi xảy ra khi cập nhật trạng thái!",
                    error = ex.Message
                });
            }
        }

        [HttpPost("request-buy")]
        [RequiresPermission("N35,N1")]
        public async Task<IActionResult> RequestBuy([FromBody] RequestBuyDTO request)
        {
            try
            {
                if (request == null || request.Products == null || !request.Products.Any())
                    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách sản phẩm không được để trống."));

                // Validate deadline
                DateTime deadline = request.Deadline.Date;
                DateTime now = DateTime.Now;
                double timeSpan = (deadline - now.Date).TotalDays + 1;

                if (now.Hour < 15 && timeSpan < 2)
                    return BadRequest(ApiResponseFactory.Fail(null, "Deadline tối thiểu là 2 ngày từ ngày hiện tại!"));
                if (now.Hour >= 15 && timeSpan < 3)
                    return BadRequest(ApiResponseFactory.Fail(null, "Yêu cầu từ sau 15h nên ngày Deadline tối thiểu là 2 ngày tính từ ngày hôm sau!"));

                if (deadline.DayOfWeek == DayOfWeek.Saturday || deadline.DayOfWeek == DayOfWeek.Sunday)
                    return BadRequest(ApiResponseFactory.Fail(null, "Deadline phải là ngày làm việc (T2 - T6)!"));

                int weekendCount = 0;
                for (int i = 0; i < timeSpan; i++)
                {
                    var d = now.Date.AddDays(i);
                    if (d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday)
                        weekendCount++;
                }

                var confirmWeekend = weekendCount > 0 ? true : false; // bạn có thể thêm cảnh báo nếu cần

                var resultSuccess = new List<string>();
                var resultFail = new List<string>();

                foreach (var item in request.Products)
                {
                    try
                    {
                        // Kiểm tra xem sản phẩm đã tồn tại trong DB chưa
                        var existingRequests = _projectPartlistPurchaseRequestRepo
                            .GetAll(r => r.JobRequirementID == request.JobRequirementID
                                            && r.ProductCode == item.ProductCode
                                            && r.IsDeleted == false);

                        ProjectPartlistPurchaseRequest requestModel = existingRequests.FirstOrDefault() ?? new ProjectPartlistPurchaseRequest();

                        // Nếu đã approved thì bỏ qua
                        if (requestModel.EmployeeApproveID > 0)
                        {
                            resultFail.Add($"{item.ProductCode} đã được duyệt, không thể yêu cầu mua.");
                            continue;
                        }
                        if (request.ProjectPartlistPriceRequestTypeID == 6)
                        {
                            requestModel.SupplierSaleID = item.SupplierSaleID ?? 0;
                        }
                        // Map thông tin
                        requestModel.JobRequirementID = request.IsVPP ? 999999 : request.JobRequirementID;
                        requestModel.EmployeeID = request.EmployeeID;
                        requestModel.ProductCode = item.ProductCode;
                        requestModel.ProductName = item.ProductName;
                        requestModel.StatusRequest = 1;
                        requestModel.DateRequest = DateTime.Now;
                        requestModel.DateReturnExpected = deadline;
                        requestModel.Quantity = item.Quantity;
                        requestModel.NoteHR = item.NoteHR;
                        if (request.ProjectPartlistPriceRequestTypeID == 4) requestModel.ProjectPartlistPurchaseRequestTypeID = 7;//mkt
                        else if (request.ProjectPartlistPriceRequestTypeID == 3) requestModel.ProjectPartlistPurchaseRequestTypeID = 6;//hr
                        else if (request.ProjectPartlistPriceRequestTypeID == 6) requestModel.ProjectPartlistPurchaseRequestTypeID = 3;//demo
                        if (requestModel.ProjectPartlistPurchaseRequestTypeID != 3)
                        {
                            // Gán ProductSale & Unit
                            var productSale = productSaleRepo.GetAll(p =>
                                p.ProductCode == item.ProductCode &&
                                p.ProductGroupID == (request.IsVPP ? 80 : (request.JobRequirementID > 0 ? 77 : 0)) &&
                                p.IsDeleted == false);
                            var productSaleModel = productSale.FirstOrDefault() ?? new ProductSale();
                            requestModel.ProductSaleID = productSaleModel.ID;
                            requestModel.ProductGroupID = productSaleModel.ProductGroupID;

                        }
                        else
                        {
                            var productRTC = _productRTCRepo.GetAll(x => x.ProductCode == item.ProductCode).FirstOrDefault();
                            if (productRTC == null)
                            {
                                return BadRequest(ApiResponseFactory.Fail(null, "Sản phẩm không có trong kho demo!"));
                            }
                            requestModel.ProductRTCID = productRTC.ID;
                            requestModel.ProductGroupRTCID = productRTC.ProductGroupRTCID;
                        }

                        var unit = _unitCountRepo.GetAll(u => u.UnitName == item.UnitName.Trim());
                        requestModel.UnitCountID = unit.FirstOrDefault()?.ID;
                        requestModel.UnitName = item.UnitName;

                        // Insert hoặc Update
                        if (requestModel.ID <= 0 || request.ProjectPartlistPriceRequestTypeID == 7)
                        {
                            var inserted = await _projectPartlistPurchaseRequestRepo.CreateAsync(requestModel);
                            if (inserted != null)
                                resultSuccess.Add(item.ProductCode);
                            else
                                resultFail.Add(item.ProductCode);
                        }
                        else
                        {
                            await _projectPartlistPurchaseRequestRepo.UpdateAsync(requestModel);
                            resultSuccess.Add(item.ProductCode);
                        }
                    }
                    catch (Exception ex)
                    {
                        resultFail.Add($"{item.ProductCode}: {ex.Message}");
                    }
                }

                return Ok(ApiResponseFactory.Success(new
                {
                    Success = resultSuccess,
                    Fail = resultFail
                }, "Yêu cầu mua đã xử lý xong."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Gửi email thông báo từ chối báo giá
        /// </summary>
        private async Task SendUnPriceEmail(List<ProductMailInfo> listDataMail, string reason, int employeeIDUnPrice)
        {
            try
            {
                // Nhóm theo người yêu cầu (FullName hoặc EmployeeID)
                var grouped = listDataMail
                    .GroupBy(d => d.EmployeeID)
                    .Where(g => g.Key > 0)
                    .ToList();

                foreach (var group in grouped)
                {
                    var employeeID = group.Key;

                    // Lấy thông tin người nhận từ stored procedure
                    var receiverData = SQLHelper<dynamic>.ProcedureToList("spGetEmployee",
                        new string[] { "@Status", "@ID" },
                        new object[] { 0, employeeID });

                    if (receiverData == null || receiverData.Count == 0) continue;

                    var receiver = SQLHelper<dynamic>.GetListData(receiverData, 0).FirstOrDefault();
                    if (receiver == null) continue;

                    // Tạo bảng HTML
                    var tableBuilder = new System.Text.StringBuilder();
                    tableBuilder.Append(@"
                        <table border='1' cellspacing='0' cellpadding='5' style='border-collapse:collapse; font-family:Arial; font-size:13px; text-align:center;'>
                            <thead style='background-color:#f2f2f2; font-weight:bold;'>
                                <tr>
                                    <th>Mã dự án</th>
                                    <th>Mã SP</th>
                                    <th>Tên SP</th>
                                    <th>Hãng</th>
                                    <th>ĐVí</th>
                                    <th>Số lượng</th>
                                    <th>Ngày YC</th>
                                    <th>Deadline</th>
                                </tr>
                            </thead>
                            <tbody>");

                    foreach (var item in group)
                    {
                        tableBuilder.Append($@"
                            <tr>
                                <td>{item.ProjectCode ?? ""}</td>
                                <td>{item.ProductCode ?? ""}</td>
                                <td>{item.ProductName ?? ""}</td>
                                <td>{item.Manufacturer ?? ""}</td>
                                <td>{item.UnitCount ?? ""}</td>
                                <td>{item.Quantity}</td>
                                <td>{(item.DateRequest != null ? Convert.ToDateTime(item.DateRequest).ToString("dd/MM/yyyy") : "")}</td>
                                <td>{(item.Deadline != null ? Convert.ToDateTime(item.Deadline).ToString("dd/MM/yyyy") : "")}</td>
                            </tr>");
                    }

                    tableBuilder.Append("</tbody></table>");

                    // Tạo email body
                    var emailBody = $@"
                        <div>
                            <p style='font-weight: bold; color: red;'>[NO REPLY]</p>
                            <p>Dear anh/chị {receiver.FullName ?? "phụ trách"},</p>
                        </div>
                        <div style='margin-top: 20px;'>
                            <p><b>Lý do từ chối:</b> {reason}</p>
                            <br/>
                            <p>Danh sách sản phẩm yêu cầu báo giá đã bị từ chối:</p>
                            {tableBuilder}
                        </div>
                        <div style='margin-top: 20px;'>
                            <p>Trân trọng,</p>
                        </div>";

                    // Insert vào bảng EmployeeSendEmail
                    EmployeeSendEmail sendEmail = new EmployeeSendEmail();
                    sendEmail.Subject = $"[THÔNG BÁO] TỪ CHỐI BÁO GIÁ - {DateTime.Now:dd/MM/yyyy}";
                    sendEmail.EmailTo = receiver.EmailCongTy ?? receiver.EmailCaNhan ?? "";
                    sendEmail.EmailCC = "";
                    sendEmail.Body = emailBody;
                    sendEmail.StatusSend = 1;
                    sendEmail.EmployeeID = employeeIDUnPrice;
                    sendEmail.Receiver = employeeID;
                    sendEmail.DateSend = DateTime.Now;
                    employeeSendEmailRepo.Create(sendEmail);

                }
            }
            catch (Exception ex)
            {
                // Log error nhưng không throw để không làm fail toàn bộ transaction
                Console.WriteLine($"SendUnPriceEmail Error: {ex.Message}");
                throw; // Re-throw để caller biết có lỗi
            }
        }

        #endregion
        [HttpPost("quote-price")]
        [RequiresPermission("N35,N1")]
        public async Task<IActionResult> QuotePriceRequest(List<ProjectPartlistPriceRequest> lst)
        {
            try
            {
                if (lst == null || lst.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có yêu cầu nào để cập nhật!"));
                }

                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                bool isAdmin = currentUser.IsAdmin;

                foreach (var item in lst)
                {
                    ProjectPartlistPriceRequest request = requestRepo.GetByID(item.ID);
                    if (request == null) continue;

                    if (request.QuoteEmployeeID != currentUser.EmployeeID && !isAdmin)
                        continue;

                    int oldStatus = request.StatusRequest ?? 0;
                    int newStatus = item.StatusRequest ?? oldStatus;

                    if (oldStatus == newStatus)
                        continue;

                    request.StatusRequest = newStatus;

                    if (!isAdmin)
                        request.QuoteEmployeeID = currentUser.EmployeeID;

                    if (newStatus == 1) // Hủy báo giá
                    {
                        request.DatePriceQuote = null;
                    }
                    else if (newStatus == 2) // Báo giá
                    {
                        request.DatePriceQuote = DateTime.Now;
                    }
                    item.StatusRequest = request.StatusRequest;
                    item.DatePriceQuote = request.DatePriceQuote;
                    item.ID = request.ID;
                    item.UpdatedDate = request.UpdatedDate;
                    item.UpdatedBy = request.UpdatedBy;
                    item.QuoteEmployeeID = request.QuoteEmployeeID;
                    await requestRepo.SaveData(item);
                }

                return Ok(ApiResponseFactory.Success(lst, "Cập nhật trạng thái báo giá thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-request-note")]
        [RequiresPermission("N35,N1")]
        public async Task<IActionResult> SaveDataPriceRequestNote([FromBody] List<ProjectPartlistPriceRequestNote> notes)
        {
            try
            {
                foreach (var note in notes)
                {
                    if (note.ID > 0)
                    {
                        await projectPartlistPriceRequestNoteRepo.UpdateAsync(note);
                    }
                    else
                    {
                        await projectPartlistPriceRequestNoteRepo.CreateAsync(note);
                    }
                }
                return Ok(ApiResponseFactory.Success(notes, "Lưu ghi chú thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}
