using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM;

using RERPAPI.Repo.GenericEntity.HRM.HRRecruitmentInterviewAssessment;
using System.Linq;
using Dapper;
using Microsoft.Data.SqlClient;

namespace RERPAPI.Controllers.HRM
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HRRecruitmentCandidateController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private EmployeeChucVuHDRepo _employeeChucVuHDRepo;
        private ConfigSystemRepo _configSystemRepo;
        private HRRecruitmentCandidateRepo _hrRecruitmentCandidateRepo;
        private HRRecruitmentCandidateLogRepo _hrRecruitmentCandidateLogRepo;
        private EmailHelper _emailHelper;
        private HRHiringRequestRepo _hrHiringRequestRepo;
        private readonly IWebHostEnvironment _environment;
        private readonly HRRecruitmentInterviewAssessmentFormRepo _hrRecruitmentInterviewAssessmentFormRepo;
        private readonly HRRecruitmentApproveRepo _hrRecruitmentApproveRepo;
        private readonly HRRecruitmentApplicationFormRepo _hrRecruitmentApplicationFormRepo;

        public HRRecruitmentCandidateController(
            EmployeeChucVuHDRepo employeeChucVuHDRepo,
            ConfigSystemRepo configSystemRepo,
            HRRecruitmentCandidateRepo hrRecruitmentCandidateRepo,
            HRRecruitmentCandidateLogRepo hrRecruitmentCandidateLogRepo,
            EmailHelper emailHelper,
            IWebHostEnvironment environment,
            IConfiguration configuration,
            HRHiringRequestRepo hrHiringRequestRepo,
            HRRecruitmentInterviewAssessmentFormRepo hrRecruitmentInterviewAssessmentFormRepo,
            HRRecruitmentApproveRepo hrRecruitmentApproveRepo,
            HRRecruitmentApplicationFormRepo hrRecruitmentApplicationFormRepo)
        {
            _employeeChucVuHDRepo = employeeChucVuHDRepo;
            _configSystemRepo = configSystemRepo;
            _hrRecruitmentCandidateRepo = hrRecruitmentCandidateRepo;
            _hrRecruitmentCandidateLogRepo = hrRecruitmentCandidateLogRepo;
            _emailHelper = emailHelper;
            _environment = environment;
            _configuration = configuration;
            _hrHiringRequestRepo = hrHiringRequestRepo;
            _hrRecruitmentInterviewAssessmentFormRepo = hrRecruitmentInterviewAssessmentFormRepo;
            _hrRecruitmentApproveRepo = hrRecruitmentApproveRepo;
            _hrRecruitmentApplicationFormRepo = hrRecruitmentApplicationFormRepo;
        }

        [HttpGet("position-contract")]
        public IActionResult GetPositionContract()
        {
            try
            {
                List<EmployeeChucVuHD> positionContracts = _employeeChucVuHDRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(positionContracts, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [RequiresPermission("N1,N2,N94", permissionFunction: "frmCandidate_View")]
        //Lấy UserName
        [HttpGet("get-username-candidate")]
        public IActionResult GetUserName()
        {
            try
            {
                var userName = _hrRecruitmentCandidateRepo.GenerateUserName();
                return Ok(ApiResponseFactory.Success(userName, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("hiring-request")]
        public async Task<IActionResult> GetHiringRequest()
        {
            try
            {
                var dtMaster = await SqlDapper<object>.ProcedureToListAsync("spGetHrRequestSelect", new { });
                return Ok(ApiResponseFactory.Success(dtMaster, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

            [HttpGet("data")]
            public async Task<IActionResult> GetDataHrRecruitmentCandidates(
                int? id = 0,
                int? status = -1,
                int? employeeRequestId = -1,
                int? departmentId = -1,
                DateTime? dateStart = null,
                DateTime? dateEnd = null,
                string? keyword = ""
             )
            {
                try
                {
                        var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                    CurrentUser _currentUser = ObjectMapper.GetCurrentUser(claims);

                    bool isHr = _currentUser.Permissions
                                .Split(',')
                                .Any(p => p.Trim() == "N1" || p.Trim() == "N2" || p.Trim() == "N94") || _currentUser.IsAdmin;
                    int loginDepartmentID = -1;
                    int interviewID = -1;

                    if (!isHr)
                    {
                        loginDepartmentID = _currentUser.DepartmentID;
                        interviewID = _currentUser.EmployeeID;
                    }

                    var param = new
                    {
                        ID = id,
                        Status = status,
                        EmployeeRequestID = employeeRequestId,
                        DepartmentID = departmentId,
                        DateStart = dateStart,
                        DateEnd = dateEnd,
                        FilterText = keyword?.Trim(),
                        LoginDepartmentID= loginDepartmentID,
                        InterviewID=interviewID
                    };
                    var result = await SqlDapper<dynamic>.ProcedureToListAsync("spGetHrRecruitmentCandidate", param);

                    var dtMaster = ((IEnumerable<dynamic>)result).ToList();

             
                    return Ok(ApiResponseFactory.Success(dtMaster, null));
                }
                catch (Exception ex)
                {
                    return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
                }
            }

        [HttpPost("delete")]
        [RequiresPermission("N1,N2,N94")]
        public async Task<IActionResult> DeleteMultipleHr([FromBody] List<int> listIds)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser _currentUser = ObjectMapper.GetCurrentUser(claims);

                if (listIds == null || listIds.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có bản ghi nào được chọn cung cấp!"));
                }

                if (listIds.Count() > 0)
                {
                    foreach (var id in listIds)
                    {
                        var hrRecruitmentCandidates = _hrRecruitmentCandidateRepo.GetByID(id);
                        if (hrRecruitmentCandidates.ID > 0)
                        {
                            hrRecruitmentCandidates.UpdatedDate = DateTime.Now;
                            hrRecruitmentCandidates.UpdatedBy = _currentUser.Code;
                            hrRecruitmentCandidates.IsDeleted = true;
                            await _hrRecruitmentCandidateRepo.UpdateAsync(hrRecruitmentCandidates);
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Đã xóa ứng viên thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("update-status")]
        [RequiresPermission("N1,N2,N94")]
        public async Task<IActionResult> UpdateStatus([FromForm] HRRecruitmentCandidateDTO data)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser _currentUser = ObjectMapper.GetCurrentUser(claims);

                if (data.listIds == null || data.listIds.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có bản ghi nào được chọn cung cấp!"));
                }

                if (data.listIds.Count() > 0)
                {
                    foreach (var id in data.listIds)
                    {
                        var hrRecruitmentCandidates = _hrRecruitmentCandidateRepo.GetByID(id);

                        if (data.Status == 1 && data.isApproved == false)
                        {
                            hrRecruitmentCandidates.SendMailTime = null;
                            hrRecruitmentCandidates.StatusMail = 0;
                            hrRecruitmentCandidates.DateInterview = null;
                        }

                        if (data.Status > 0)
                        {
                            HRRecruitmentCandidateLog log = _hrRecruitmentCandidateLogRepo.
                                GetAll(x => x.HRRecruitmentCandidateID == hrRecruitmentCandidates.ID && x.ApprovedStep == data.Status)
                                .OrderByDescending(x => x.CreatedDate).FirstOrDefault() ?? new HRRecruitmentCandidateLog();
                            log.Note = data.NoteLog ?? "";
                            log.ApprovedStep = data.Status;
                            log.IsApproved = data.isApproved;
                            log.IsDeleted = false;
                            log.CreatedDate = DateTime.Now;
                            log.CreatedBy = _currentUser.Code;
                            log.UpdatedDate = DateTime.Now;
                            log.UpdatedBy = _currentUser.Code;
                            log.HRRecruitmentCandidateID = hrRecruitmentCandidates.ID;

                            if (log.ID > 0) await _hrRecruitmentCandidateLogRepo.UpdateAsync(log);
                            else await _hrRecruitmentCandidateLogRepo.CreateAsync(log);
                        }

                        if (!(bool)data.isApproved)
                        {
                            HRRecruitmentCandidateLog checkStatusOld = _hrRecruitmentCandidateLogRepo
                                .GetAll(x => x.HRRecruitmentCandidateID == hrRecruitmentCandidates.ID && x.IsApproved == true)
                                .OrderByDescending(x => x.CreatedDate).FirstOrDefault() ?? new HRRecruitmentCandidateLog();

                            hrRecruitmentCandidates.Status = checkStatusOld.ApprovedStep ?? 0;
                        }
                        else
                        {
                            hrRecruitmentCandidates.Status = data.Status;
                        }

                        await _hrRecruitmentCandidateRepo.UpdateAsync(hrRecruitmentCandidates);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Đã lưu ứng viên thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("download-file-cv")]
        [RequiresPermission("N1,N2,N94")]
        public IActionResult DownloadFileCv(int id)
        {
            try
            {
                var hrRecruitmentCandidates = _hrRecruitmentCandidateRepo.GetByID(id);
                if (hrRecruitmentCandidates == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu tải file không hợp lệ!"));
                }

                string filePath = Path.Combine(hrRecruitmentCandidates.ServerPath, hrRecruitmentCandidates.FileCVName);
                if (!System.IO.File.Exists(filePath))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy file CV: {hrRecruitmentCandidates.FileCVName}"));
                }

                // Đọc file và trả về
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                string fileName = hrRecruitmentCandidates.FileCVName;
                string contentType = GetContentType(fileName);

                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = "Lỗi khi tải file CV",
                    error = ex.ToString()
                });
            }
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };
        }

        [HttpPost("save-data")]
        [Consumes("multipart/form-data")]
        [RequiresPermission("N1,N2,N94")]
        public async Task<IActionResult> SaveData([FromForm] HRRecruitmentCandidateDTO data)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser _currentUser = ObjectMapper.GetCurrentUser(claims);
                if (data == null) return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu để lưu!"));

                if (!_hrRecruitmentCandidateRepo.Validate(data, out string mesage))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, mesage));
                }

                if (data.FileCV != null)
                {
                    string deleteFileName = "";
                    HRRecruitmentCandidate hrRecruitmentCandidateOld = null;
                    if (data.ID > 0)
                    {
                        hrRecruitmentCandidateOld = _hrRecruitmentCandidateRepo.GetByID(data.ID);
                        if (hrRecruitmentCandidateOld != null)
                        {
                            deleteFileName = hrRecruitmentCandidateOld.FileCVName ?? "";
                        }
                    }

                    data.FileCVName = data.FileCV.FileName.Trim();

                    var uploadPath = _configSystemRepo.GetUploadPathByKey("HrRecruitmentCandidate");
                    if (string.IsNullOrWhiteSpace(uploadPath))
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy cấu hình đường dẫn cho key: HrRecruitmentCandidate"));
                    }

                    // Dynamic subpath: CvUngVien/Year/PositionName
                    string year = (data.DateApply ?? DateTime.Now).ToString("yyyy");
                    string position = string.IsNullOrWhiteSpace(data.PositionName) ? "NoPosition" : data.PositionName;
                    string pathPattern = Path.Combine(year, position);

                    string pathUpload = data.ServerPath = Path.Combine(uploadPath, pathPattern);

                    if (!Directory.Exists(pathUpload))
                    {
                        Directory.CreateDirectory(pathUpload);
                    }

                    //if (!string.IsNullOrWhiteSpace(deleteFileName) && deleteFileName.ToLower() != data.FileCVName.ToLower())
                    //{
                    //    var oldPath = hrRecruitmentCandidateOld?.ServerPath ?? pathUpload;
                    //    var oldFilePath = Path.Combine(oldPath, deleteFileName);

                    //    if (System.IO.File.Exists(oldFilePath))
                    //    {
                    //        System.IO.File.Delete(oldFilePath);
                    //    }
                    //}

                    var fullPath = Path.Combine(pathUpload, data.FileCVName!);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await data.FileCV.CopyToAsync(stream);
                    }
                }

                //data.Password = MaHoaMD5.EncryptPassword(data.Password);
                data.Note = data.Note?.Trim() ?? "";
                data.FileCVName = data.FileCVName?.Trim() ?? "";
                data.ServerPath = data.ServerPath?.Trim() ?? "";
                data.UserName = data.UserName?.Trim() ?? "";
                data.Password = data.Password?.Trim() ?? "";
                data.Email = data.Email?.Trim() ?? "";

                if (data.ID > 0)
                {
                    data.UpdatedDate = DateTime.Now;
                    data.UpdatedBy = _currentUser.Code;

                    await _hrRecruitmentCandidateRepo.UpdateAsync(data);
                }
                else
                {
                    data.CreatedDate = DateTime.Now;
                    data.CreatedBy = _currentUser.Code;
                    data.UpdatedDate = DateTime.Now;
                    data.UpdatedBy = _currentUser.Code;

                    await _hrRecruitmentCandidateRepo.CreateAsync(data);
                }

                if (data.Status > 0)
                {
                    HRRecruitmentCandidateLog log = _hrRecruitmentCandidateLogRepo.
                        GetAll(x => x.HRRecruitmentCandidateID == data.ID && x.ApprovedStep == data.Status)
                        .OrderByDescending(x => x.CreatedDate).FirstOrDefault() ?? new HRRecruitmentCandidateLog();
                    log.Note = data.NoteLog ?? "";
                    log.ApprovedStep = data.Status;
                    log.IsApproved = true;
                    log.IsDeleted = false;
                    log.CreatedDate = DateTime.Now;
                    log.CreatedBy = _currentUser.Code;
                    log.UpdatedDate = DateTime.Now;
                    log.UpdatedBy = _currentUser.Code;
                    log.HRRecruitmentCandidateID = data.ID;

                    if (log.ID > 0) await _hrRecruitmentCandidateLogRepo.UpdateAsync(log);
                    else await _hrRecruitmentCandidateLogRepo.CreateAsync(log);
                }

                return Ok(ApiResponseFactory.Success(null, "Đã lưu ứng viên thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("send-interview-mail")]
        [RequiresPermission("N1,N2,N94")]
        public async Task<IActionResult> SendEmail([FromBody] List<EmployeeSendEmailDTO> sendEmails)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser _currentUser = ObjectMapper.GetCurrentUser(claims);
                var footer = _configuration["FooterMail:HR:Footer"];
                if (sendEmails.Count() > 0)
                {
                    foreach (var email in sendEmails)
                    {
                        if (email.ID > 0)
                        {
                            var hrRecruitmentCandidate = _hrRecruitmentCandidateRepo.GetByID(email.ID);
                            if (hrRecruitmentCandidate != null)
                            {
                                hrRecruitmentCandidate.StatusMail = email.StatusSend;
                                hrRecruitmentCandidate.DateInterview = email.DateSend;
                                hrRecruitmentCandidate.DeadlineFeedbackMail = email.DeadlineFeedbackMail;
                                hrRecruitmentCandidate.SendMailTime = DateTime.Now;
                                hrRecruitmentCandidate.CreatedDate = DateTime.Now;
                                hrRecruitmentCandidate.CreatedBy = _currentUser.Code;
                                hrRecruitmentCandidate.UpdatedDate = DateTime.Now;
                                hrRecruitmentCandidate.UpdatedBy = _currentUser.Code;
                                await _hrRecruitmentCandidateRepo.UpdateAsync(hrRecruitmentCandidate);
                            }
                        }
                        await _emailHelper.SendAsyncHr(email.EmailTo, email.Subject, email.Body + footer, cc: "dept_manager@rtc.edu.vn");
                    }
                }
                return Ok(ApiResponseFactory.Success(null, "Gửi thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("send-offer-letter-mail")]
        [RequiresPermission("N1,N2,N94")]
        public async Task<IActionResult> SendEmailOferLetter([FromBody] List<EmployeeSendEmail> sendEmails)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser _currentUser = ObjectMapper.GetCurrentUser(claims);
                var footer = _configuration["FooterMail:HR:Footer"];
                string emailCC = "dept_manager@rtc.edu.vn";
                if (sendEmails.Count() > 0)
                {
                    foreach (var email in sendEmails)
                    {
                        if (email.ID > 0)
                        {
                            var hrRecruitmentCandidate = _hrRecruitmentCandidateRepo.GetByID(email.ID);
                            if (hrRecruitmentCandidate != null)
                            {
                                hrRecruitmentCandidate.StatusMail = email.StatusSend;
                                hrRecruitmentCandidate.SendMailTime = DateTime.Now;
                                await _hrRecruitmentCandidateRepo.UpdateAsync(hrRecruitmentCandidate);
                            }
                        }
                        await _emailHelper.SendAsyncHr(email.EmailTo, email.Subject, email.Body + footer, cc: emailCC);
                    }
                }
                return Ok(ApiResponseFactory.Success(null, "Gửi thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("summary-detail")]
        public async Task<IActionResult> GetSummaryDetail(int candidateId)
        {
            try
            {
                var details = SQLHelper<dynamic>.ProcedureToList(
                    "spGetHrRecruitmentCandidateSummaryDetail",
                    new[] { "@HRRecruitmentCandidateID" },
                    new object[] { candidateId });

                var applicationForm = SQLHelper<dynamic>.GetListData(details, 0);
                var workingExperiences = SQLHelper<dynamic>.GetListData(details, 1);
                var otherCertificates = SQLHelper<dynamic>.GetListData(details, 2);
                var educations = SQLHelper<dynamic>.GetListData(details, 3);
                var emergencyContacts = SQLHelper<dynamic>.GetListData(details, 4);
                var foreignLanguageSkills = SQLHelper<dynamic>.GetListData(details, 5);
                var recruitmentInfo = SQLHelper<dynamic>.GetListData(details, 6);

                var applicationFormMaster = ((System.Collections.IEnumerable)applicationForm).Cast<dynamic>().FirstOrDefault();
                
                var assessmentData = SQLHelper<dynamic>.GetListData(details, 7);
                var assessmentMaster = ((System.Collections.IEnumerable)assessmentData).Cast<dynamic>().FirstOrDefault();

                var candidateData = SQLHelper<dynamic>.GetListData(details, 8);

                var approveDataList = SQLHelper<dynamic>.GetListData(details, 9);
                var approveData = ((System.Collections.IEnumerable)approveDataList).Cast<dynamic>().FirstOrDefault();

                var examResults = SQLHelper<dynamic>.GetListData(details, 10);

                return Ok(ApiResponseFactory.Success(new
                {
                    ApplicationForm = new
                    {
                        Master = applicationFormMaster,
                        WorkingExperiences = workingExperiences,
                        OtherCertificates = otherCertificates,
                        Educations = educations,
                        EmergencyContacts = emergencyContacts,
                        ForeignLanguageSkills = foreignLanguageSkills,
                        RecruitmentInfo = recruitmentInfo
                    },
                    InterviewAssessment = new
                    {
                        CandidateInfo = candidateData,
                        AssessmentInfo = assessmentMaster
                    },
                    RecruitmentApprove = approveData,
                    ExamResults = examResults
                }, "Lấy dữ liệu tổng hợp tuyển dụng thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("summary-master")]
        public async Task<IActionResult> GetSummaryMaster(
            int? id = 0,
            int? status = -1,
            int? employeeRequestId = -1,
            int? departmentId = -1,
            DateTime? dateStart = null,
            DateTime? dateEnd = null,
            string? keyword = "")
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser _currentUser = ObjectMapper.GetCurrentUser(claims);

                bool isHr = _currentUser.Permissions
                            .Split(',')
                            .Any(p => p.Trim() == "N1" || p.Trim() == "N2" || p.Trim() == "N94") || _currentUser.IsAdmin;
                int loginDepartmentID = -1;
                int empReqId = isHr ? -1 : _currentUser.EmployeeID;
                if(!isHr)
                {
                    loginDepartmentID = _currentUser.DepartmentID;
                }    
                var param = new
                {
                    ID = id ?? 0,
                    Status = status ?? -1,
                    EmployeeRequestID = empReqId,
                    DepartmentID = departmentId ?? 0,
                    DateStart = dateStart,
                    DateEnd = dateEnd,
                    FilterText = keyword?.Trim() ?? "",
                    LoginDepartmentID = loginDepartmentID
                };

                var data = await SqlDapper<dynamic>.ProcedureToListAsync("spGetHrRecruitmentCandidateSummaryMaster", param);
                return Ok(ApiResponseFactory.Success(data, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}