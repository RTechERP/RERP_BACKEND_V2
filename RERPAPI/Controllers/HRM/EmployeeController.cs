using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;
using System.Net.Mime;
using ZXing;

namespace RERPAPI.Controllers.HRM
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly EmployeeRepo _employeeRepo;
        private vUserGroupLinksRepo _vUserGroupLinksRepo;
        private CurrentUser _currentUser;
        private readonly EmployeeApprovedRepo _approvedRepo;
        private readonly TaxCompanyRepo _taxCompanyRepo;
        private readonly EmployeeSignatureFileRepo _employeeSignatureFileRepo;
        private readonly ConfigSystemRepo _configSystemRepo;

        public EmployeeController(
            IConfiguration configuration,
            EmployeeRepo employeeRepo,
            vUserGroupLinksRepo vUserGroupLinksRepo,
            CurrentUser currentUser,
            EmployeeApprovedRepo approvedRepo,
            TaxCompanyRepo taxCompanyRepo,
            EmployeeSignatureFileRepo employeeSignatureFileRepo,
            ConfigSystemRepo configSystemRepo
            )
        {
            _configuration = configuration;
            _employeeRepo = employeeRepo;
            _vUserGroupLinksRepo = vUserGroupLinksRepo;
            _currentUser = currentUser;
            _approvedRepo = approvedRepo;
            _taxCompanyRepo = taxCompanyRepo;
            _employeeSignatureFileRepo = employeeSignatureFileRepo;
            _configSystemRepo = configSystemRepo;
        }

        [HttpGet("employees")]
        public IActionResult GetEmployee(int? status, int? departmentid, string? keyword)
        {
            try
            {
                departmentid = departmentid ?? 0;
                keyword = string.IsNullOrWhiteSpace(keyword) ? "" : keyword;
                status = status ?? 0;
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                object data;
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
                var vUserHR = _vUserGroupLinksRepo.GetAll().FirstOrDefault(x => (x.Code == "N1" || x.Code == "N2") && x.UserID == currentUser.ID);
                var vUserHRN60 = _vUserGroupLinksRepo.GetAll().FirstOrDefault(x => (x.Code == "N60") && x.UserID == currentUser.ID);
                if (vUserHR == null && vUserHRN60 == null)
                {
                    data = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployee",
                                                new string[] { "@Status", "@DepartmentID", "@Keyword" },
                                                new object[] { status, departmentid, keyword ?? "" });
                }
                else if (vUserHRN60 != null)
                {
                    data = SQLHelper<EmployeeDTON60>.ProcedureToListModel("spGetEmployee",
                                                new string[] { "@Status", "@DepartmentID", "@Keyword" },
                                                new object[] { status, departmentid, keyword ?? "" });
                }
                else
                {
                    var employee = SQLHelper<object>.ProcedureToList("spGetEmployee",
                                               new string[] { "@Status", "@DepartmentID", "@Keyword" },
                                               new object[] { status, departmentid, keyword });
                    data = SQLHelper<object>.GetListData(employee, 0);
                }
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("")]
        public IActionResult GetEmployees(int? status, int? departmentid, string? keyword)
        {
            try
            {
                departmentid = departmentid ?? 0;
                keyword = string.IsNullOrWhiteSpace(keyword) ? "" : keyword;
                status = status ?? 0;
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                object data;
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
                var vUserHR = _vUserGroupLinksRepo.GetAll().FirstOrDefault(x => (x.Code == "N1" || x.Code == "N2") && x.UserID == currentUser.ID);
                var vUserHRN60 = _vUserGroupLinksRepo.GetAll().FirstOrDefault(x => (x.Code == "N60") && x.UserID == currentUser.ID);
                if (vUserHR == null && vUserHRN60 == null && currentUser.IsAdmin != true)
                {
                    data = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployee",
                                                new string[] { "@Status", "@DepartmentID", "@Keyword" },
                                                new object[] { status, departmentid, keyword ?? "" });
                }
                else if (vUserHRN60 != null)
                {
                    data = SQLHelper<EmployeeDTON60>.ProcedureToListModel("spGetEmployee",
                                                new string[] { "@Status", "@DepartmentID", "@Keyword" },
                                                new object[] { status, departmentid, keyword ?? "" });
                }
                else
                {
                    var employee = SQLHelper<object>.ProcedureToList("spGetEmployee",
                                               new string[] { "@Status", "@DepartmentID", "@Keyword" },
                                               new object[] { status, departmentid, keyword });
                    data = SQLHelper<object>.GetListData(employee, 0);
                }
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("employees/{id}")]
        [RequiresPermission("N42")]
        public IActionResult GetByID(int id)
        {
            try
            {
                RERPAPI.Model.Entities.Employee employee = _employeeRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(employee, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveEmployee([FromBody] RERPAPI.Model.Entities.Employee employee)
        {
            try
            {
                if (!_employeeRepo.Validate(employee, out string message) && employee.Status != 1)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }
                if (employee.ID <= 0)
                {
                    employee.IsExcludedFromSalary = true;
                    await _employeeRepo.CreateAsync(employee);
                } 
                else await _employeeRepo.UpdateAsync(employee);

                return Ok(ApiResponseFactory.Success(employee, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-approve")]
        public IActionResult GetApprove()
        {
            try
            {
                var approvedTBPs = _approvedRepo.GetAll(x => x.Type == 1 && x.IsDeleted != true);

                return Ok(ApiResponseFactory.Success(approvedTBPs, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-tax-company")]
        public IActionResult GetTaxCompany()
        {
            try
            {
                var data = _taxCompanyRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("update-count-salary")]
        public async Task<IActionResult> UpdateCountSalary([FromQuery] int employeeID, [FromQuery] bool isExcludedFromSalary)
        {
            try
            {
                int result = 0;
                var employee = _employeeRepo.GetByID(employeeID);
                if (employee == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy nhân viên"));
                employee.IsExcludedFromSalary = isExcludedFromSalary;
                result = await _employeeRepo.UpdateAsync(employee);
                if (result > 0)
                {
                    return Ok(ApiResponseFactory.Success(null, "Cập nhật trạng thái thành công"));
                }
                else
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Cập nhật trạng thái thất bại"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //API thêm nhân viên vào phòng ban
        [HttpGet("update-employee-department")]
        public async Task<IActionResult> UpdateEmployeeDepartment([FromQuery] int employeeID, [FromQuery] int departmentID)
        {
            try
            {
                var employee = _employeeRepo.GetByID(employeeID);
                if (employee == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy nhân viên"));

                employee.DepartmentID = departmentID;
                int result = await _employeeRepo.UpdateAsync(employee);
                if (result > 0)
                {
                    return Ok(ApiResponseFactory.Success(null, "Cập nhật phòng ban thành công"));
                }
                else
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Cập nhật phòng ban thất bại"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //API thêm nhân viên vào phòng ban
        [HttpGet("get-employee-by-department-string")]
        public async Task<IActionResult> GetEmployeeByDepartmentString(string departmentString = "", int teamID = -1)
        {
            try
            {
                var param = new
                {
                    DepartmentString = departmentString,
                    TeamID = teamID
                };
                var lstEmp = await SqlDapper<object>.ProcedureToListAsync("spGetEmployeeByDepartmentString", param);
                return Ok(ApiResponseFactory.Success(lstEmp));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #region API xử lý ảnh chữ ký cá nhân
        [HttpGet("signature")]
        public IActionResult GetSignature()
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var signature = _employeeSignatureFileRepo.GetAll(
                    x => x.EmployeeID == currentUser.EmployeeID &&
                         x.IsDeleted == false)
                    .FirstOrDefault();

                string path = signature != null ? Path.Combine(signature.ServerPath, signature.FileName) : null;

                if (string.IsNullOrEmpty(path))
                    return BadRequest("File path is required.");

                if (!System.IO.File.Exists(path))
                    return NotFound("File not found.");

                var fileName = Path.GetFileName(path);

                var fileBytes = System.IO.File.ReadAllBytes(path);

                var contentType = GetContentType(path);

                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete-signature")]
        public async Task<IActionResult> DeleteBillExport()
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var rs = _employeeSignatureFileRepo
                    .GetAll(x => x.EmployeeID == currentUser.EmployeeID && x.IsDeleted == false)
                    .FirstOrDefault();

                if (rs == null) return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy chữ ký"));
                else
                {
                    rs.IsDeleted = true;
                    await _employeeSignatureFileRepo.UpdateAsync(rs);
                }

                return Ok(ApiResponseFactory.Success(null, "Xóa file thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("upload-signature")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadMultiple([FromForm] UploadSignatureRequest fileSignature)
        {
            try
            {
                if (fileSignature == null)
                {
                    return Ok(new { status = 0, message = "Không có file nào để tải lên." });
                }

                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var file = fileSignature.File;

                var uploadPath = _configSystemRepo.GetUploadPathByKey("SIGNATURE_PATH");
                if (string.IsNullOrWhiteSpace(uploadPath))
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy cấu hình đường dẫn cho key: EmployeeSignature"));

                var fileExtension = Path.GetExtension(file.FileName);
                var uniqueFileName = $"{currentUser.Code}{fileExtension}";
                var fullPath = Path.Combine(uploadPath, uniqueFileName);

                var checkExistsFile = _employeeSignatureFileRepo.GetAll(x => x.EmployeeID == currentUser.EmployeeID && x.IsDeleted == true).FirstOrDefault();
                if (checkExistsFile != null)
                {
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    checkExistsFile.FileName = uniqueFileName;
                    checkExistsFile.ServerPath = uploadPath;
                    checkExistsFile.OriginPath = uploadPath;
                    checkExistsFile.IsDeleted = false;
                    checkExistsFile.EmployeeID = currentUser.EmployeeID;

                    await _employeeSignatureFileRepo.UpdateAsync(checkExistsFile);
                    return Ok(ApiResponseFactory.Success(null, "Tải file lên thành công"));
                }

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var fileUpload = new EmployeeSignatureFile
                {
                    FileName = uniqueFileName,
                    OriginPath = uploadPath,
                    ServerPath = uploadPath,
                    IsDeleted = false,
                    EmployeeID = currentUser.EmployeeID
                };

                await _employeeSignatureFileRepo.CreateAsync(fileUpload);

                return Ok(ApiResponseFactory.Success(null, "Tải file lên thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        private string GetContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".txt" => MediaTypeNames.Text.Plain,
                ".pdf" => MediaTypeNames.Application.Pdf,
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".xls" => "application/vnd.ms-excel",
                ".csv" => "text/csv",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => MediaTypeNames.Application.Octet
            };
        }

        public class UploadSignatureRequest
        {
            public IFormFile File { get; set; }
        }
        #endregion
    }
}