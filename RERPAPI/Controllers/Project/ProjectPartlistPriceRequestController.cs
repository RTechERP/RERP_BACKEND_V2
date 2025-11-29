using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    //[ApiKeyAuthorize]
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

        public ProjectPartlistPriceRequestController(ProjectRepo projectRepo, POKHRepo pOKHRepo, ProjectPartlistPriceRequestRepo requestRepo, ProductSaleRepo productSaleRepo, CurrencyRepo currencyRepo, SupplierSaleRepo supplierSaleRepo, ProjectSolutionRepo projectSolutionRepo, EmployeeSendEmailRepo employeeSendEmailRepo, ProjectPartlistPriceRequestTypeRepo projectPartlistPriceRequestTypeRepo, ProjectPartlistPriceRequestNoteRepo projectPartlistPriceRequestNoteRepo)
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
        public async Task<IActionResult> GetAll(DateTime dateStart, DateTime dateEnd, int statusRequest, int projectId, string? keyword,
            int isDeleted, int projectTypeID, int poKHID, int isCommercialProduct = -1, int page = 1, int size = 25)
        {
            if (projectTypeID < 0) isCommercialProduct = 1;
            else poKHID = 0;
            int isJobRequirement = -1;
            int projectPartlistPriceRequestTypeID = -1;


            List<List<dynamic>> dtPriceRequest = SQLHelper<dynamic>.ProcedureToList("spGetProjectPartlistPriceRequest_New_Nhat",
                                                                          new string[] {
                                                                  "@DateStart", "@DateEnd", "@StatusRequest", "@ProjectID", "@Keyword", "@IsDeleted",
                                                                  "@ProjectTypeID", "@IsCommercialProduct","@IsJobRequirement","@ProjectPartlistPriceRequestTypeID", "@POKHID", "@PageNumber", "@PageSize"
                                                                          },
                                                                          new object[] {
                                                                  dateStart, dateEnd, statusRequest, projectId, keyword, isDeleted,
                                                                  projectTypeID, isCommercialProduct,isJobRequirement,projectPartlistPriceRequestTypeID, poKHID, page, size
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
            var dtEmployee = SQLHelper<dynamic>.ProcedureToList("spGetEmployee", new string[] { "@Status" }, new object[] { 0 });
            return Ok(new { status = 1, data = new { dtEmployee = dtEmployee[0] } });
        }
        [HttpGet("get-po-code")]
        public async Task<IActionResult> GetPoCode()
        {
            var dtPOKH = pOKHRepo.GetAll();
            return Ok(new { status = 1, data = dtPOKH });
        }
        [HttpGet("get-product-sale")]
        public async Task<IActionResult> GetProductSale()
        {
            var data = productSaleRepo.GetAll().ToList();

            return Ok(new
            {
                status = 1,
                data,
            });
        }
        [HttpGet("get-currency")]
        public async Task<IActionResult> GetCurrency()
        {
            List<Currency> currencies = currencyRepo.GetAll();
            return Ok(new { status = 1, data = currencies });
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] List<ProjectPartlistPriceRequest> projectPartlistPriceRequest)
        {
            try
            {
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
                    }
                }

                return Ok(new { status = 1 });
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
                var purchaseRequest = supplierSaleRepo.GetAll()
                    .OrderBy(x => x.NgayUpdate)
                    .Select(x => new
                    {
                        x.ID,
                        x.CodeNCC
                        ,
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
        public async Task<IActionResult> GetPriceHistoryPartlist(int projectId, int supplierSaleId, int employeeRequestId, string? keyword)
        {
            try
            {
                var priceHistoryPartlist = SQLHelper<object>.ProcedureToList("spGetHistoryPricePartlist",
                new string[] { "@Keyword", "@ProjectID", "@SupplierSaleID", "@EmployeeRequestID" },
                new object[] { keyword ?? "", projectId, supplierSaleId, employeeRequestId });
                var data = SQLHelper<object>.GetListData(priceHistoryPartlist, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("download")]
        public async Task<IActionResult> DownloadFile([FromBody] DownloadProjectPartlistPriceRequestDTO request)
        {
            var project = projectRepo.GetByID(request.ProjectId);
            if (project == null || project.CreatedDate == null)
                return BadRequest(new
                {
                    status = 0,
                    message = "Không tim thấy dự án!"
                });

            var solutions = SQLHelper<ProjectSolution>.ProcedureToList("spGetProjectSolutionByProjectPartListID",
                                                            new string[] { "@ProjectPartListID" }, new object[] { request.PartListId });

            if (solutions.Count <= 0)
                return BadRequest(new
                {
                    status = 0,
                    message = "Không tìm thấy giải pháp dự án!"
                });
            var solution = SQLHelper<dynamic>.GetListData(solutions, 0).FirstOrDefault();
            var year = project.CreatedDate.Value.Year;
            var pathPattern = $"{year}/{project.ProjectCode.Trim()}/THIETKE.Co/{solution?.CodeSolution.Trim()}/2D/GC/DH";
            var fileName = $"{request.ProductCode}.pdf";
            //sửa
            var fileUrl = $"http://113.190.234.64/:8081/api/project/{pathPattern}/{fileName}";

            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(fileUrl);

                if (!response.IsSuccessStatusCode)
                    return NotFound($"Không tìm thấy file: {fileUrl}");

                var stream = await response.Content.ReadAsStreamAsync();
                return File(stream, "application/pdf", fileName);
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
        [HttpPost("check-price")]
        public async Task<IActionResult> CheckPrice(List<ProjectPartlistPriceRequest> lst)
        {
            try
            {
                if (lst == null || lst.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có yêu cầu nào để check giá!"));
                }
                await requestRepo.SaveData(lst);



                return Ok(ApiResponseFactory.Success(lst, "Check giá thành công!"));


            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("complete-price-request")]
        public async Task<IActionResult> CompletePriceRequest(List<ProjectPartlistPriceRequest> lst)
        {
            try
            {
                if (lst == null || lst.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có yêu cầu nào để hoàn thành!"));
                }
                await requestRepo.SaveData(lst);
                return Ok(ApiResponseFactory.Success(lst, "Hoàn thành yêu cầu báo giá thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("cancel-price-request")]
        public async Task<IActionResult> CancelPriceRequest(List<ProjectPartlistPriceRequest> lst)
        {
            try
            {
                if (lst == null || lst.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có yêu cầu nào để hủy!"));
                }
                await requestRepo.SaveData(lst);
                return Ok(ApiResponseFactory.Success(lst, "Hủy yêu cầu báo giá thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #region Update Price Request Status (Từ chối / Hủy từ chối báo giá)

        /// <summary>
        /// Cập nhật trạng thái yêu cầu báo giá (Từ chối hoặc Hủy từ chối)
        /// </summary>
        /// <param name="request">Danh sách model yêu cầu báo giá cần cập nhật kèm dữ liệu gửi mail</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPost("update-price-request-status")]
        public async Task<IActionResult> UpdatePriceRequestStatus([FromBody] UpdatePriceRequestStatusRequestDTO request)
        {
            try
            {
                // Validate input
                if (request == null || request.ListModel == null || !request.ListModel.Any())
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Danh sách yêu cầu báo giá không được để trống!"
                    });
                }

                // Lấy thông tin từ item đầu tiên để validate
                var firstItem = request.ListModel.First();
                var newStatus = firstItem.StatusRequest ?? 0;
                var reasonUnPrice = firstItem.ReasonUnPrice ?? string.Empty;

                // Validate StatusRequest (chỉ cho phép 1: Hủy từ chối hoặc 3: Từ chối, 5: Từ chối)
                if (newStatus != 1 && newStatus != 3 && newStatus != 5)
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Trạng thái không hợp lệ! (1: Hủy từ chối, 3/5: Từ chối)"
                    });
                }

                // Nếu là từ chối (status = 3 hoặc 5), bắt buộc phải có lý do
                if ((newStatus == 3 || newStatus == 5) && string.IsNullOrWhiteSpace(reasonUnPrice))
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Vui lòng nhập lý do từ chối!"
                    });
                }

                var validIDs = new List<int>();
                var invalidProducts = new List<string>();

                // Validate từng record trong database
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

                    // Validate cho trường hợp Từ chối (status = 3 hoặc 5)
                    if (newStatus == 3 || newStatus == 5)
                    {
                        // Không cho phép từ chối lại nếu đã bị từ chối
                        if (priceRequest.StatusRequest == 3 || priceRequest.StatusRequest == 5)
                        {
                            invalidProducts.Add($"[{priceRequest.ProductCode}] đã bị từ chối trước đó");
                            continue;
                        }

                        // Không cho phép từ chối nếu đã báo giá (status = 2)
                        if (priceRequest.StatusRequest == 2)
                        {
                            invalidProducts.Add($"[{priceRequest.ProductCode}] đã ở trạng thái Đã báo giá, không thể từ chối");
                            continue;
                        }
                    }
                    // Validate cho trường hợp Hủy từ chối (status = 1)
                    else if (newStatus == 1)
                    {
                        // Chỉ cho phép hủy nếu đang ở trạng thái Từ chối (status = 3 hoặc 5)
                        if (priceRequest.StatusRequest != 3 && priceRequest.StatusRequest != 5)
                        {
                            invalidProducts.Add($"[{priceRequest.ProductCode}] chưa bị từ chối");
                            continue;
                        }
                    }

                    validIDs.Add(id);
                }

                // Nếu không có ID hợp lệ nào
                if (!validIDs.Any())
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Không có sản phẩm hợp lệ để cập nhật!",
                        invalidProducts
                    });
                }

                // Thực hiện cập nhật
                var updateCount = 0;
                foreach (var item in request.ListModel)
                {
                    var id = item.ID;
                    if (!validIDs.Contains(id)) continue;

                    var priceRequest = requestRepo.GetByID(id);
                    if (priceRequest == null) continue;

                    // Cập nhật các trường từ model
                    priceRequest.StatusRequest = item.StatusRequest;
                    priceRequest.UpdatedBy = item.UpdatedBy;
                    priceRequest.UpdatedDate = DateTime.Now;
                    priceRequest.EmployeeIDUnPrice = item.EmployeeIDUnPrice;
                    priceRequest.ReasonUnPrice = item.ReasonUnPrice;

                    requestRepo.Update(priceRequest);
                    updateCount++;
                }

                // Gửi email nếu là trường hợp Từ chối (status = 3 hoặc 5)
                if ((newStatus == 3 || newStatus == 5) && request.ListDataMail != null && request.ListDataMail.Any())
                {
                    try
                    {
                        await SendUnPriceEmail(request.ListDataMail, reasonUnPrice, firstItem.EmployeeIDUnPrice ?? 0);
                    }
                    catch (Exception emailEx)
                    {
                        // Log lỗi gửi email nhưng vẫn trả về success
                        Console.WriteLine($"Lỗi gửi email: {emailEx.Message}");
                    }
                }

                var actionName = (newStatus == 3 || newStatus == 5) ? "Từ chối báo giá" : "Hủy từ chối báo giá";

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

        /// <summary>
        /// Gửi email thông báo từ chối báo giá
        /// </summary>
        private async Task SendUnPriceEmail(List<dynamic> listDataMail, string reason, int employeeIDUnPrice)
        {
            try
            {
                // Nhóm theo người yêu cầu (FullName hoặc EmployeeID)
                var grouped = listDataMail
                    .GroupBy(d => d.EmployeeID ?? 0)
                    .Where(g => g.Key > 0)
                    .ToList();

                foreach (var group in grouped)
                {
                    var employeeID = group.Key;

                    // Lấy thông tin người nhận từ stored procedure
                    var receiverData = SQLHelper<dynamic>.ProcedureToList("spGetEmployee",
                        new string[] { "@ID" },
                        new object[] { employeeID });

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
                                <td>{item.Quantity ?? 0}</td>
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
                    // Hoặc sử dụng SQL trực tiếp


                    //SQLHelper<dynamic>.ExcuteSQL(
                    //    @"INSERT INTO EmployeeSendEmail (Subject, EmailTo, EmailCC, Body, StatusSend, EmployeeID, Receiver, DateSend)
                    //      VALUES (@Subject, @EmailTo, @EmailCC, @Body, @StatusSend, @EmployeeID, @Receiver, @DateSend)",
                    //    new Dictionary<string, object>
                    //    {
                    //        { "@Subject", $"[THÔNG BÁO] TỪ CHỐI BÁO GIÁ - {DateTime.Now:dd/MM/yyyy}" },
                    //        { "@EmailTo", receiver.EmailCongTy ?? receiver.EmailCaNhan ?? "" },
                    //        { "@EmailCC", "" },
                    //        { "@Body", emailBody },
                    //        { "@StatusSend", 1 },
                    //        { "@EmployeeID", employeeIDUnPrice },
                    //        { "@Receiver", employeeID },
                    //        { "@DateSend", DateTime.Now }
                    //    });
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
        [HttpPost("reject-price-request")]
        public async Task<IActionResult> RejectPriceRequest(List<ProjectPartlistPriceRequest> lst)
        {
            try
            {
                if (lst == null || lst.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có yêu cầu nào để từ chối!"));
                }
                await requestRepo.SaveData(lst);
                return Ok(ApiResponseFactory.Success(lst, "Từ chối báo giá thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("quote-price")]
        public async Task<IActionResult> QuotePriceRequest(List<ProjectPartlistPriceRequest> lst)
        {
            try
            {
                if (lst == null || lst.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có yêu cầu nào để từ chối!"));
                }
                await requestRepo.SaveData(lst);
                return Ok(ApiResponseFactory.Success(lst, "Từ chối báo giá thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-request-note")]
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
