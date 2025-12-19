using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Model.Param.SendEmailRegisterContract;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.DocumentManager;
using System.Text;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RegisterContractController : ControllerBase
    {
        public DocumentTypeRepo _documentTypeRepo;
        public TaxCompanyRepo _taxCompanyRepo;
        public RegisterContractRepo _registerContractRepo;
        public EmployeeSendEmailRepo _employeeSendEmailRepo;
        public EmployeeRepo _employeeRepo;

        public RegisterContractController(
           DocumentTypeRepo documentTypeRepo,
           TaxCompanyRepo taxCompanyRepo,
           RegisterContractRepo registerContractRepo,
           EmployeeSendEmailRepo employeeSendEmailRepo,
           EmployeeRepo employeeRepo)
        {
            _documentTypeRepo = documentTypeRepo;
            _taxCompanyRepo = taxCompanyRepo;
            _registerContractRepo = registerContractRepo;
            _employeeSendEmailRepo = employeeSendEmailRepo;
            _employeeRepo = employeeRepo;
        }

        [HttpPost("get-all-data")]
        public async Task<IActionResult> GetAll([FromBody] RegisterContractRequestParam request)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetallRegisterContract",
               new string[] { "@DateStart", "@DateEnd", "@Status", "@EmployeeID", "@KeyWords", "@DepartmentID" }
               , new object[] { request.dateStart, request.dateEnd, request.status, request.empID, request.keyword, request.departmentID });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // hàm lấy dữ liệu cho combobox
        [HttpGet("get-document-type")]
        public async Task<IActionResult> GetDocumentType()
        {
            try
            {
                var data = _documentTypeRepo.GetAll();
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-tax-company")]
        public async Task<IActionResult> GetTaxCompany()
        {
            try
            {
                var data = _taxCompanyRepo.GetAll();
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //lấy dữ liệu theo id 
        [HttpGet("get-data-by-id")]
        public async Task<IActionResult> GetDataByID(int id)
        {
            try
            {
                var data = _registerContractRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] RegisterContract request)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                int userId = currentUser.ID;

                if (request.ID > 0 && request.IsDeleted == true)
                {
                    RegisterContract data = _registerContractRepo.GetByID(request.ID) ?? new RegisterContract();

                    if (data == null)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy phiếu!"));
                    }
                    if (data.Status > 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Phiếu theo dõi sổ đóng dấu đã được duyệt! Không thể xóa!"));
                    }
                    else if (data.EmployeeID != currentUser.EmployeeID && !currentUser.IsAdmin)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Bạn không thể xóa Phiếu theo dõi đóng dấu của người khác!"));
                    }
                    data.IsDeleted = true;
                    await _registerContractRepo.UpdateAsync(data);

                    return Ok(ApiResponseFactory.Success(null, "Xóa phiếu thành công"));
                }

                if (!_registerContractRepo.Validate(request, out string message))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }
                string status = request.ID > 0 ? "Sửa" : "Thêm";
                if (request.ID > 0)
                {
                    if (request.Status > 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Phiếu theo dõi sổ đóng dấu đã được duyệt! Không thể sửa!"));
                    }
                    else if (request.EmployeeID != currentUser.EmployeeID)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Bạn không thể sửa Phiếu theo dõi đóng dấu của người khác!"));
                    }
                    request.EmployeeID = currentUser.EmployeeID;
                    request.ReasonCancel = "";
                    await _registerContractRepo.UpdateAsync(request);
                }
                else
                {
                    request.Status = 0;
                    request.ReasonCancel = "";
                    request.EmployeeID = currentUser.EmployeeID;
                    await _registerContractRepo.CreateAsync(request);
                }

                // Return ID để frontend có thể gửi email
                var responseData = new { ID = request.ID };
                return Ok(ApiResponseFactory.Success(responseData, $"{status} dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("approve-or-cancel")]
        public async Task<IActionResult> ApproveOrCancel([FromBody] ApproveOrCancelRequest request)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                RegisterContract data = _registerContractRepo.GetByID(request.ID);
                if (data == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy đăng ký hợp đồng!"));
                }

                // Kiểm tra quyền: Người nhận hoặc Admin
                if (data.EmployeeReciveID != currentUser.EmployeeID && !currentUser.IsAdmin)
                {
                    string action = request.Status == 2 ? "Hủy" : "Xác nhận";
                    return BadRequest(ApiResponseFactory.Fail(null, $"Bạn không có quyền [{action}] của người khác!"));
                }

                // Nếu hủy: bắt buộc nhập lý do
                if (request.Status == 2 && string.IsNullOrWhiteSpace(request.ReasonCancel))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Hãy nhập lý do hủy duyệt!"));
                }

                data.Status = request.Status;
                data.ReasonCancel = request.ReasonCancel ?? "";
                data.DateApproved = data.UpdatedDate = DateTime.Now;

                await _registerContractRepo.UpdateAsync(data);

                string message = request.Status == 1 ? "Xác nhận thành công" : "Hủy thành công";
                return Ok(ApiResponseFactory.Success(null, message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("send-email-approval")]
        public async Task<IActionResult> SendEmailApproval([FromBody] SendEmailApprovalRequest request)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                // Validate
                if (request.RegisterContractID <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy thông tin đăng ký hợp đồng!"));
                }

                // Lấy thông tin đăng ký hợp đồng
                RegisterContract contract = _registerContractRepo.GetByID(request.RegisterContractID);
                if (contract == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy đăng ký hợp đồng!"));
                }

                // Lấy thông tin người đăng ký (người nhận email)
                Employee employeeRegister = _employeeRepo.GetByID(contract.EmployeeID ?? 0);
                if (employeeRegister == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy thông tin người đăng ký!"));
                }

                // Lấy thông tin công ty
                string taxCompanyName = "";
                if (contract.TaxCompanyID.HasValue && contract.TaxCompanyID > 0)
                {
                    var taxCompany = _taxCompanyRepo.GetByID(contract.TaxCompanyID.Value);
                    taxCompanyName = taxCompany?.Name ?? "";
                }

                // Lấy loại hồ sơ
                string documentTypeName = "";
                if (contract.DocumentTypeID.HasValue && contract.DocumentTypeID > 0)
                {
                    var docType = _documentTypeRepo.GetByID(contract.DocumentTypeID.Value);
                    documentTypeName = docType?.Name ?? "";
                }

                // Xác định trạng thái
                string statusText = request.Status == 1 ? "XÁC NHẬN HOÀN THÀNH" : "HỦY";
                string statusColor = request.Status == 1 ? "green" : "red";

                // Tạo nội dung email HTML
                var emailBody = _registerContractRepo.BuildEmailApprovalBody(
                    contract,
                    employeeRegister,
                    currentUser.FullName,
                    taxCompanyName,
                    documentTypeName,
                    statusText,
                    statusColor,
                    request.ReasonCancel
                );

                // Tạo subject
                string subject = $"{currentUser.FullName.ToUpper()} - THÔNG BÁO {statusText} ĐĂNG KÝ HỢP ĐỒNG - {contract.DocumentName}";

                // Tạo entity email
                var emailEntity = new EmployeeSendEmail
                {
                    Subject = subject,
                    Body = emailBody,
                    EmailTo = employeeRegister.EmailCongTy ?? employeeRegister.EmailCaNhan ?? "nhubinh2104@gmail.com",
                    EmailCC = "",
                    StatusSend = 1, // 1: Đã gửi
                    DateSend = DateTime.Now,
                    EmployeeID = currentUser.EmployeeID, // Người gửi (người xác nhận/hủy)
                    Receiver = employeeRegister.ID        // Người nhận (người đăng ký)
                };

                await _employeeSendEmailRepo.CreateAsync(emailEntity);

                return Ok(ApiResponseFactory.Success(null, "Gửi email thông báo thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi khi gửi email: {ex.Message}"));
            }
        }

        /// <summary>
        /// API gửi email thông báo khi có đăng ký hợp đồng MỚI
        /// Gửi cho người nhận (EmployeeReciveID)
        /// </summary>
        [HttpPost("send-email-new-contract")]
        public async Task<IActionResult> SendEmailNewContract([FromBody] SendEmailNewContractRequest request)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                // Validate
                if (request.RegisterContractID <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy thông tin đăng ký hợp đồng!"));
                }

                // Lấy thông tin đăng ký hợp đồng
                RegisterContract contract = _registerContractRepo.GetByID(request.RegisterContractID);
                if (contract == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy đăng ký hợp đồng!"));
                }

                // Lấy thông tin người nhận (người được giao xử lý)
                Employee employeeReciver = _employeeRepo.GetByID(contract.EmployeeReciveID ?? 0);
                if (employeeReciver == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy thông tin người nhận!"));
                }

                // Lấy thông tin công ty
                string taxCompanyName = "";
                if (contract.TaxCompanyID.HasValue && contract.TaxCompanyID > 0)
                {
                    var taxCompany = _taxCompanyRepo.GetByID(contract.TaxCompanyID.Value);
                    taxCompanyName = taxCompany?.Name ?? "";
                }

                // Lấy loại hồ sơ
                string documentTypeName = "";
                if (contract.DocumentTypeID.HasValue && contract.DocumentTypeID > 0)
                {
                    var docType = _documentTypeRepo.GetByID(contract.DocumentTypeID.Value);
                    documentTypeName = docType?.Name ?? "";
                }

                // Tạo nội dung email HTML
                var emailBody = _registerContractRepo.BuildEmailNewContractBody(contract, currentUser.FullName, employeeReciver.FullName, taxCompanyName, documentTypeName
                );

                // Tạo subject
                string subject = $"{currentUser.FullName.ToUpper()} - THÔNG BÁO ĐĂNG KÝ HỢP ĐỒNG MỚI - {contract.DocumentName}";

                // Tạo entity email
                var emailEntity = new EmployeeSendEmail
                {
                    Subject = subject,
                    Body = emailBody,
                    EmailTo = employeeReciver.EmailCongTy ?? employeeReciver.EmailCaNhan ?? "nhubinh2104@gmail.com",
                    EmailCC = "",
                    StatusSend = 1,
                    DateSend = DateTime.Now,
                    EmployeeID = currentUser.EmployeeID, // Người gửi (người đăng ký)
                    Receiver = employeeReciver.ID         // Người nhận (người được giao xử lý)
                };

                await _employeeSendEmailRepo.CreateAsync(emailEntity);

                return Ok(ApiResponseFactory.Success(null, "Gửi email thông báo thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi khi gửi email: {ex.Message}"));
            }
        }
    }
}
