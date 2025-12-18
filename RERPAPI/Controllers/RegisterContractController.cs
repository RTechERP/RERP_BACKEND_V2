using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.DocumentManager;
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

        public RegisterContractController(DocumentTypeRepo documentTypeRepo, TaxCompanyRepo taxCompanyRepo, RegisterContractRepo registerContractRepo)
        {
            _documentTypeRepo = documentTypeRepo;
            _taxCompanyRepo = taxCompanyRepo;
            _registerContractRepo = registerContractRepo;

        }

        [HttpGet("get-all-data")]
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

                if (request.ID >0 && request.IsDeleted == true)
                {
                    RegisterContract data = _registerContractRepo.GetByID(request.ID) ?? new RegisterContract();

                    if (request.Status > 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Phiếu theo dõi sổ đóng dấu đã được duyệt! Không thể xóa!"));
                    }
                    else if (request.EmployeeID != currentUser.EmployeeID)
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
                if(request.ID > 0)
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
                return Ok(ApiResponseFactory.Success(null, "Lưu dữ liệu thành công"));
            }
            catch(Exception ex) 
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


    }
}
