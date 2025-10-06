using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HRHiringRequestController : ControllerBase
    {
        private readonly HRHiringRequestRepo _hrHiringRequestRepo = new HRHiringRequestRepo();
        private readonly HRHiringRequestEducationLinkRepo _hiringRequestEducationLinkRepo = new HRHiringRequestEducationLinkRepo();
        private readonly HRHiringRequestExperienceLinkRepo _hiringRequestExperienceLinkRepo = new HRHiringRequestExperienceLinkRepo();
        private readonly HRHiringRequestGenderLinkRepo _hiringRequestGenderLinkRepo = new HRHiringRequestGenderLinkRepo();
        private readonly HRHiringRequestHealthLinkRepo _hiringRequestHealthLinkRepo = new HRHiringRequestHealthLinkRepo();
        private readonly HRHiringRequestLanguageLinkRepo _hiringRequestLanguageLinkRepo = new HRHiringRequestLanguageLinkRepo();
        private readonly HRHiringRequestComputerLevelLinkRepo _hiringRequestComputerLevelLinkRepo = new HRHiringRequestComputerLevelLinkRepo();
        private readonly HRHiringRequestCommunicationLinkRepo _hiringRequestCommunicationLinkRepo = new HRHiringRequestCommunicationLinkRepo();
        private readonly HRHiringRequestApproveLinkRepo _hiringRequestApproveLinkRepo = new HRHiringRequestApproveLinkRepo();
        private readonly HRHiringAppearanceLinkRepo _hiringAppearanceLinkRepo = new HRHiringAppearanceLinkRepo();

        private readonly DepartmentRepo _departmentRepo = new DepartmentRepo();
        private readonly EmployeeRepo _employeeRepo = new EmployeeRepo();
        private readonly EmployeeChucVuHDRepo _employeeChucVuHDRepo = new EmployeeChucVuHDRepo();

        //CurrentUser _currentUser = new CurrentUser();

        //public HRHiringRequestController()
        //{
        //    var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
        //    _currentUser = ObjectMapper.GetCurrentUser(claims);
        //}


        [HttpPost("savedata")]
        public async Task<IActionResult> SaveData([FromBody] HRHiringRequestDTO model)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser _currentUser = ObjectMapper.GetCurrentUser(claims);

                var hiringRequestId = model.HiringRequests.ID;
                bool isUpdate = hiringRequestId > 0;
                bool isSoftDelete = model.HiringRequests.IsDeleted == true;

                // 1. Lưu/Cập nhật HiringRequest chính
                if (!isUpdate)
                {
                    // CREATE - Tạo mới
                    model.HiringRequests.STT = _hrHiringRequestRepo.GetRequestCode().STT;
                    model.HiringRequests.EmployeeRequestID = _currentUser.EmployeeID;
                    model.HiringRequests.HiringRequestCode = _hrHiringRequestRepo.GetRequestCode().HiringRequestCode;
                    model.HiringRequests.CreatedDate = DateTime.Now;
                    model.HiringRequests.IsDeleted = false;
                    await _hrHiringRequestRepo.CreateAsync(model.HiringRequests);
                    hiringRequestId = model.HiringRequests.ID;
                }
                else
                {
                    // UPDATE hoặc SOFT DELETE
                    model.HiringRequests.UpdatedDate = DateTime.Now;
                    await _hrHiringRequestRepo.UpdateAsync(model.HiringRequests);

                    // Nếu là soft delete, chỉ cần update main record và return
                    if (isSoftDelete)
                    {
                        return Ok(ApiResponseFactory.Success("", "Xóa yêu cầu tuyển dụng thành công!"));
                    }

                    // Nếu là update thường, xóa tất cả link records cũ trước khi tạo mới
                    await DeleteExistingLinkRecords(hiringRequestId);
                }

                // 2. Chỉ tạo link records khi không phải soft delete
                if (!isSoftDelete)
                {
                    await CreateLinkRecords(model, hiringRequestId);
                }

                string message = isUpdate ? "Cập nhật thành công!" : "Thêm mới thành công!";
                return Ok(ApiResponseFactory.Success(model, message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Helper method để xóa các link records cũ khi update - SỬA DÙNG GetAll
        private async Task DeleteExistingLinkRecords(int hiringRequestId)
        {
            // Education - SỬA: Dùng GetAll và filter
            var allEducations = _hiringRequestEducationLinkRepo.GetAll();
            var existingEducations = allEducations.Where(x => x.HRHiringRequestID == hiringRequestId && !x.IsDeleted);
            foreach (var item in existingEducations)
            {
                item.IsDeleted = true;
                item.UpdatedDate = DateTime.Now;
                await _hiringRequestEducationLinkRepo.UpdateAsync(item);
            }

            // Experience - SỬA: Dùng GetAll và filter
            var allExperiences = _hiringRequestExperienceLinkRepo.GetAll();
            var existingExperiences = allExperiences.Where(x => x.HRHiringRequestID == hiringRequestId && !x.IsDeleted);
            foreach (var item in existingExperiences)
            {
                item.IsDeleted = true;
                item.UpdatedDate = DateTime.Now;
                await _hiringRequestExperienceLinkRepo.UpdateAsync(item);
            }

            // Gender - SỬA: Dùng GetAll và filter
            var allGenders = _hiringRequestGenderLinkRepo.GetAll();
            var existingGenders = allGenders.Where(x => x.HRHiringRequestID == hiringRequestId && !x.IsDeleted);
            foreach (var item in existingGenders)
            {
                item.IsDeleted = true;
                item.UpdatedDate = DateTime.Now;
                await _hiringRequestGenderLinkRepo.UpdateAsync(item);
            }

            // Appearance - SỬA: Dùng GetAll và filter
            var allAppearances = _hiringAppearanceLinkRepo.GetAll();
            var existingAppearances = allAppearances.Where(x => x.HRHiringRequestID == hiringRequestId && !x.IsDeleted);
            foreach (var item in existingAppearances)
            {
                item.IsDeleted = true;
                item.UpdatedDate = DateTime.Now;
                await _hiringAppearanceLinkRepo.UpdateAsync(item);
            }

            // Health - SỬA: Dùng GetAll và filter
            var allHealth = _hiringRequestHealthLinkRepo.GetAll();
            var existingHealth = allHealth.Where(x => x.HRHiringRequestID == hiringRequestId && !x.IsDeleted);
            foreach (var item in existingHealth)
            {
                item.IsDeleted = true;
                item.UpdatedDate = DateTime.Now;
                await _hiringRequestHealthLinkRepo.UpdateAsync(item);
            }

            // Language - SỬA: Dùng GetAll và filter
            var allLanguages = _hiringRequestLanguageLinkRepo.GetAll();
            var existingLanguages = allLanguages.Where(x => x.HRHiringRequestID == hiringRequestId && !x.IsDeleted);
            foreach (var item in existingLanguages)
            {
                item.IsDeleted = true;
                item.UpdatedDate = DateTime.Now;
                await _hiringRequestLanguageLinkRepo.UpdateAsync(item);
            }

            // Computer - SỬA: Dùng GetAll và filter
            var allComputers = _hiringRequestComputerLevelLinkRepo.GetAll();
            var existingComputers = allComputers.Where(x => x.HRHiringRequestID == hiringRequestId && !x.IsDeleted);
            foreach (var item in existingComputers)
            {
                item.IsDeleted = true;
                item.UpdatedDate = DateTime.Now;
                await _hiringRequestComputerLevelLinkRepo.UpdateAsync(item);
            }

            // Communication - SỬA: Dùng GetAll và filter
            var allCommunications = _hiringRequestCommunicationLinkRepo.GetAll();
            var existingCommunications = allCommunications.Where(x => x.HRHiringRequestID == hiringRequestId && !x.IsDeleted);
            foreach (var item in existingCommunications)
            {
                item.IsDeleted = true;
                item.UpdatedDate = DateTime.Now;
                _hiringRequestCommunicationLinkRepo.UpdateAsync(item);
            }

            // Approval - SỬA: Dùng GetAll và filter
            var allApprovals = _hiringRequestApproveLinkRepo.GetAll();
            var existingApprovals = allApprovals.Where(x => x.HRHiringRequestID == hiringRequestId && !x.IsDeleted);
            foreach (var item in existingApprovals)
            {
                item.IsDeleted = true;
                item.UpdatedDate = DateTime.Now;
                await _hiringRequestApproveLinkRepo.UpdateAsync(item);
            }
        }

        // Helper method để tạo các link records - GIỮ NGUYÊN
        private async Task CreateLinkRecords(HRHiringRequestDTO model, int hiringRequestId)
        {
            // 1. Education Levels
            if (model.EducationLevels != null && model.EducationLevels.Count > 0)
            {
                foreach (int eduItem in model.EducationLevels)
                {
                    var edumodel = new HRHiringRequestEducationLink
                    {
                        HRHiringRequestID = hiringRequestId,
                        EducationLevel = eduItem,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    await _hiringRequestEducationLinkRepo.CreateAsync(edumodel);
                }
            }

            // 2. Experience Levels
            if (model.Experiences != null && model.Experiences.Count > 0)
            {
                foreach (int expItem in model.Experiences)
                {
                    var expmodel = new HRHiringRequestExperienceLink
                    {
                        HRHiringRequestID = hiringRequestId,
                        Experience = expItem,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    await _hiringRequestExperienceLinkRepo.CreateAsync(expmodel);
                }
            }

            // 3. Gender Requirements
            if (model.Genders != null && model.Genders.Count > 0)
            {
                foreach (int genderItem in model.Genders)
                {
                    var gendermodel = new HRHiringRequestGenderLink
                    {
                        HRHiringRequestID = hiringRequestId,
                        Gender = genderItem,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    await _hiringRequestGenderLinkRepo.CreateAsync(gendermodel);
                }
            }

            // 4. Appearance Requirements
            if (model.Appearances != null && model.Appearances.Count > 0)
            {
                foreach (int appearanceItem in model.Appearances)
                {
                    var appearancemodel = new HRHiringAppearanceLink
                    {
                        HRHiringRequestID = hiringRequestId,
                        Appearance = appearanceItem,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    await _hiringAppearanceLinkRepo.CreateAsync(appearancemodel);
                }
            }

            // 5. Health Requirements
            if (model.HealthRequirements != null && model.HealthRequirements.Count > 0)
            {
                foreach (var healthItem in model.HealthRequirements)
                {
                    var healthmodel = new HRHiringRequestHealthLink
                    {
                        HRHiringRequestID = hiringRequestId,
                        HealthType = healthItem.HealthType,
                        HealthDecription = healthItem.HealthDescription,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    await _hiringRequestHealthLinkRepo.CreateAsync(healthmodel);
                }
            }

            // 6. Language Requirements
            if (model.Languages != null && model.Languages.Count > 0)
            {
                foreach (var langItem in model.Languages)
                {
                    var langmodel = new HRHiringRequestLanguageLink
                    {
                        HRHiringRequestID = hiringRequestId,
                        LanguageType = langItem.LanguageType,
                        LanguageTypeName = langItem.LanguageTypeName,
                        LanguageLevel = langItem.LanguageLevel,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    await _hiringRequestLanguageLinkRepo.CreateAsync(langmodel);
                }
            }

            // 7. Computer Skills
            if (model.ComputerSkills != null && model.ComputerSkills.Count > 0)
            {
                foreach (var skillItem in model.ComputerSkills)
                {
                    var skillmodel = new HRHiringRequestComputerLevelLink
                    {
                        HRHiringRequestID = hiringRequestId,
                        ComputerType = skillItem.ComputerType,
                        ComputerName = skillItem.ComputerName,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    await _hiringRequestComputerLevelLinkRepo.CreateAsync(skillmodel);
                }
            }

            // 8. Communication Requirements
            if (model.Communications != null && model.Communications.Count > 0)
            {
                foreach (var commItem in model.Communications)
                {
                    var commmodel = new HRHiringRequestCommunicationLink
                    {
                        HRHiringRequestID = hiringRequestId,
                        CommunicationType = commItem.CommunicationType,
                        CommunicationDecription = commItem.CommunicationDecription,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    await _hiringRequestCommunicationLinkRepo.CreateAsync(commmodel);
                }
            }

            // 9. Approval Requirements
            if (model.Approvals != null && model.Approvals.Count > 0)
            {
                foreach (var approvalItem in model.Approvals)
                {
                    var approvalmodel = new HRHiringRequestApproveLink
                    {
                        HRHiringRequestID = hiringRequestId,
                        ApproveID = approvalItem.ApproveID,
                        Step = approvalItem.Step,
                        StepName = approvalItem.StepName,
                        Note = approvalItem.Note,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    await _hiringRequestApproveLinkRepo.CreateAsync(approvalmodel);
                }
            }
        }



        [HttpGet("get-department")]
        public IActionResult GetDepartment()
        {
            try
            {
                //var departmentRepo = new DepartmentRepo();

                var departments = _departmentRepo.GetAll()
                    .Select(x => new
                    {
                        x.ID,
                        x.Code,
                        x.Name
                    }).ToList();

                return Ok(new
                {
                    status = 1,
                    data = departments
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-hrhiring-request")]
        public IActionResult GetHRHiringRequest(
          [FromQuery] int departmentID = 0,
          [FromQuery] string? findText = null,
          [FromQuery] DateTime? dateStart = null,
          [FromQuery] DateTime? dateEnd = null,
          [FromQuery] int id = 0,
          [FromQuery] int chucVuHDID = 0
      )
        {
            try
            {
                // Chuẩn hóa thời gian
                var ds = dateStart.HasValue ? new DateTime(dateStart.Value.Year, dateStart.Value.Month, dateStart.Value.Day, 0, 0, 0) : (DateTime?)null;
                var de = dateEnd.HasValue ? dateEnd.Value.Date.AddDays(1) : (DateTime?)null; // Đầu ngày tiếp theo

                var dt = SQLHelper<object>.ProcedureToList(
                    "spGetHRHiringRequest",
                    new string[] { "Keyword", "DepartmentID", "ChucVuHDID", "DateStart", "DateEnd", "ID" },
                    new object[] { findText ?? "", departmentID, chucVuHDID, ds, de, id }
                );
                var data = SQLHelper<object>.GetListData(dt, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-employee-chuc-vu-hd")]
        public IActionResult GetEmployeeChucVuHD()
        {
            try
            {

                var dataSet = SQLHelper<object>.ProcedureToList("spGetEmployeeChucVu",new string[] { },new object[] { });


                var chucVuList = SQLHelper<object>.GetListData(dataSet, 1);


                if (chucVuList == null || chucVuList.Count == 0)
                {
                    return Ok(ApiResponseFactory.Success(new List<object>(), "Không có dữ liệu chức vụ"));
                }


                return Ok(ApiResponseFactory.Success(chucVuList, "Lấy danh sách chức vụ thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Thêm DTO class
        public class GetDataRequest
        {
            public string? DateStart { get; set; }
            public string? DateEnd { get; set; }
            public int DepartmentID { get; set; } = 0;
            public string? Keyword { get; set; } = "";
            public int Id { get; set; } = 0;
            public int ChucVuHDID { get; set; } = 0;
        }

        [HttpPost("getdata")]
        public async Task<IActionResult> GetData([FromBody] GetDataRequest request)
        {
            try
            {
                //Console.WriteLine($"GetData called with request: {System.Text.Json.JsonSerializer.Serialize(request)}");

                if (request == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Request body is empty"));
                }

                // Parse parameters với safe conversion
                DateTime? dateStart = null;
                DateTime? dateEnd = null;
                int departmentID = request.DepartmentID;
                string keyword = request.Keyword ?? "";
                int id = request.Id;
                int chucVuHDID = request.ChucVuHDID;

                // Parse dates
                if (!string.IsNullOrEmpty(request.DateStart) && DateTime.TryParse(request.DateStart, out DateTime ds))
                {
                    dateStart = ds;
                }

                if (!string.IsNullOrEmpty(request.DateEnd) && DateTime.TryParse(request.DateEnd, out DateTime de))
                {
                    dateEnd = de;
                }

                //Console.WriteLine($"Parsed parameters: dateStart={dateStart}, dateEnd={dateEnd}, departmentID={departmentID}, keyword={keyword}, id={id}, chucVuHDID={chucVuHDID}");

                // Business logic
                if (id > 0)
                {
                    //Console.WriteLine($"Getting detail data for ID: {id}");
                    var detailData = GetHiringRequestDetailData(id);
                    return Ok(ApiResponseFactory.Success(detailData, "Lấy chi tiết thành công!"));
                }
                else
                {
                    Console.WriteLine("Getting list data using stored procedure");
                    var ds_formatted = dateStart.HasValue ? new DateTime(dateStart.Value.Year, dateStart.Value.Month, dateStart.Value.Day, 0, 0, 0) : (DateTime?)null;
                    var de_formatted = dateEnd.HasValue ? dateEnd.Value.Date.AddDays(1) : (DateTime?)null;

                    var dt = SQLHelper<object>.ProcedureToList(
                        "spGetHRHiringRequest",
                        new string[] { "Keyword", "DepartmentID", "ChucVuHDID", "DateStart", "DateEnd", "ID" },
                        new object[] { keyword ?? "", departmentID, chucVuHDID, ds_formatted, de_formatted, 0 }
                    );
                    var data = SQLHelper<object>.GetListData(dt, 0);
                    return Ok(ApiResponseFactory.Success(new List<object> { data }, "Lấy danh sách thành công!"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetData: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Helper method để lấy chi tiết cho edit với tất cả dữ liệu từ bảng link
        private List<object> GetHiringRequestDetailData(int id)
        {
            var resultSets = new List<object>();

            try
            {
                Console.WriteLine($"Getting main record for ID: {id}");

                // 1. Main record
                var mainRecord = _hrHiringRequestRepo.GetByID(id);
                if (mainRecord == null)
                {
                    throw new Exception($"Không tìm thấy yêu cầu tuyển dụng với ID: {id}");
                }

                // Join với các bảng liên quan để lấy tên hiển thị
                var department = mainRecord.DepartmentID.HasValue ? _departmentRepo.GetByID((int)mainRecord.DepartmentID.Value) : null;
                var employee = mainRecord.EmployeeRequestID.HasValue ? _employeeRepo.GetByID((int)mainRecord.EmployeeRequestID.Value) : null;
                var chucVuHD = mainRecord.EmployeeChucVuHDID.HasValue ? _employeeChucVuHDRepo.GetByID((int)mainRecord.EmployeeChucVuHDID.Value) : null;

                var mainData = new
                {
                    mainRecord.ID,
                    mainRecord.HiringRequestCode,
                    mainRecord.DepartmentID,
                    mainRecord.EmployeeChucVuHDID,
                    mainRecord.EmployeeRequestID,
                    mainRecord.QuantityHiring,
                    mainRecord.SalaryMin,
                    mainRecord.SalaryMax,
                    mainRecord.AgeMin,
                    mainRecord.AgeMax,
                    mainRecord.WorkAddress,
                    mainRecord.ProfessionalRequirement,
                    mainRecord.JobDescription,
                    mainRecord.Note,
                    mainRecord.DateRequest,
                    DepartmentName = department?.Name ?? "",
                    EmployeeChucVuHDName = chucVuHD?.Name ?? "",
                    EmployeeRequestCode = employee?.Code ?? "",
                    EmployeeRequestName = employee?.FullName ?? ""
                };

                resultSets.Add(new List<object> { mainData });
                Console.WriteLine($"Main data: {mainData}");

                // 2. Education selections
                var educations = _hiringRequestEducationLinkRepo.GetAll()
                    .Where(x => x.HRHiringRequestID == id && x.IsDeleted != true)
                    .Select(x => new { value = x.EducationLevel, EducationLevel = x.EducationLevel })
                    .ToList();
                resultSets.Add(educations);
                Console.WriteLine($"Education data: {educations.Count} items");

                // 3. Experience selections
                var experiences = _hiringRequestExperienceLinkRepo.GetAll()
                    .Where(x => x.HRHiringRequestID == id && x.IsDeleted != true)
                    .Select(x => new { value = x.Experience, Experience = x.Experience })
                    .ToList();
                resultSets.Add(experiences);
                Console.WriteLine($"Experience data: {experiences.Count} items");

                // 4. Gender selections
                var genders = _hiringRequestGenderLinkRepo.GetAll()
                    .Where(x => x.HRHiringRequestID == id && x.IsDeleted != true)
                    .Select(x => new { value = x.Gender, Gender = x.Gender })
                    .ToList();
                resultSets.Add(genders);
                Console.WriteLine($"Gender data: {genders.Count} items");



                // 5. Appearance selections - SỬA: Đảm bảo trả về đúng structure
                var appearances = _hiringAppearanceLinkRepo.GetAll()
                    .Where(x => x.HRHiringRequestID == id && x.IsDeleted != true)
                    .Select(x => new { value = x.Appearance, Appearance = x.Appearance })
                    .ToList();
                resultSets.Add(appearances);
                Console.WriteLine($"Appearance data: {appearances.Count} items - {string.Join(", ", appearances.Select(a => a.value))}");

                // ... language, computer code ...

                // 6. Language data
                var languages = _hiringRequestLanguageLinkRepo.GetAll()
                    .Where(x => x.HRHiringRequestID == id && x.IsDeleted != true)
                    .ToList();
                resultSets.Add(languages);
                Console.WriteLine($"Language data: {languages.Count} items");

                // 7. Computer skills
                var computers = _hiringRequestComputerLevelLinkRepo.GetAll()
                    .Where(x => x.HRHiringRequestID == id && x.IsDeleted != true)
                    .ToList();
                resultSets.Add(computers);
                Console.WriteLine($"Computer data: {computers.Count} items");

                // 8. Health requirements
                var healths = _hiringRequestHealthLinkRepo.GetAll()
                    .Where(x => x.HRHiringRequestID == id && x.IsDeleted != true)
                    .ToList();
                resultSets.Add(healths);
                Console.WriteLine($"Health data: {healths.Count} items");

                // 9. Communication requirements
                var communications = _hiringRequestCommunicationLinkRepo.GetAll()
                    .Where(x => x.HRHiringRequestID == id && x.IsDeleted != true)
                    .ToList();
                resultSets.Add(communications);
                Console.WriteLine($"Communication data: {communications.Count} items");

                Console.WriteLine($"Total result sets: {resultSets.Count}");
                return resultSets;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetHiringRequestDetailData: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return new List<object> { new List<object>() };
            }
        }

        [HttpPost("approve")]
        public async Task<IActionResult> ApproveRequest([FromBody] ApproveRequestDTO request)
        {
            try
            {
                Console.WriteLine($"Approve request: {System.Text.Json.JsonSerializer.Serialize(request)}");

                if (request == null || request.HiringRequestID <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));
                }

                // Kiểm tra hiring request có tồn tại không
                var hiringRequest = _hrHiringRequestRepo.GetByID(request.HiringRequestID);
                if (hiringRequest == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy yêu cầu tuyển dụng"));
                }

                // Lấy danh sách approval hiện tại
                var currentApprovals = _hiringRequestApproveLinkRepo.GetAll()
                    .Where(x => x.HRHiringRequestID == request.HiringRequestID && x.IsDeleted != true)
                    .OrderBy(x => x.Step)
                    .ToList();

                // Kiểm tra logic approve theo thứ tự
                var approveResult = await ProcessApproval(request, currentApprovals);

                if (!approveResult.Success)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, approveResult.Message));
                }

                return Ok(ApiResponseFactory.Success("", approveResult.Message));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ApproveRequest: {ex.Message}");
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // In the ProcessApproval method, rename the inner variable 'stepName' to avoid CS0136

        private async Task<ApprovalResult> ProcessApproval(ApproveRequestDTO request, List<HRHiringRequestApproveLink> currentApprovals)
        {
            try
            {
                // Kiểm tra step hiện tại
                var currentStep = GetCurrentApprovalStep(currentApprovals);

                // Validate step logic
                if (!ValidateApprovalStep(request.Step, currentStep, request.IsApprove))
                {
                    var stepNameForError = GetStepName(request.Step); // Renamed variable to avoid CS0136
                    if (request.IsApprove)
                    {
                        return new ApprovalResult
                        {
                            Success = false,
                            Message = $"Không thể duyệt {stepNameForError}. Vui lòng kiểm tra thứ tự duyệt."
                        };
                    }
                    else
                    {
                        return new ApprovalResult
                        {
                            Success = false,
                            Message = $"Không thể hủy duyệt {stepNameForError}."
                        };
                    }
                }

                // Tạo hoặc cập nhật approval record
                await CreateOrUpdateApproval(request, currentApprovals);

                var stepName = GetStepName(request.Step);
                var action = request.IsApprove ? "duyệt" : "hủy duyệt";

                return new ApprovalResult
                {
                    Success = true,
                    Message = $"{stepName} {action} thành công!"
                };
            }
            catch (Exception ex)
            {
                return new ApprovalResult
                {
                    Success = false,
                    Message = $"Lỗi xử lý duyệt: {ex.Message}"
                };
            }
        }

        // Lấy step hiện tại đã được duyệt
        private int GetCurrentApprovalStep(List<HRHiringRequestApproveLink> approvals)
        {
            var approvedSteps = approvals
                .Where(x => x.IsApprove == true)
                .Select(x => x.Step)
                .ToList();

            if (!approvedSteps.Any()) return 0; // Chưa có step nào được duyệt
            return approvedSteps.Max(); // Step cao nhất đã được duyệt
        }

        // Validate logic duyệt theo thứ tự
        private bool ValidateApprovalStep(int requestStep, int currentStep, bool isApprove)
        {
            if (isApprove)
            {
                // Duyệt: phải theo thứ tự 1 → 2 → 3
                return requestStep == currentStep + 1;
            }
            else
            {
                // Hủy duyệt: chỉ có thể hủy step cao nhất đã duyệt
                return requestStep == currentStep && currentStep > 0;
            }
        }

        // Tạo hoặc cập nhật approval record
        private async Task CreateOrUpdateApproval(ApproveRequestDTO request, List<HRHiringRequestApproveLink> currentApprovals)
        {
            var existingApproval = currentApprovals.FirstOrDefault(x => x.Step == request.Step);

            if (existingApproval != null)
            {
                // Cập nhật existing record
                existingApproval.IsApprove = request.IsApprove;
                existingApproval.DateApprove = request.IsApprove ? DateTime.Now : (DateTime?)null;
                existingApproval.ReasonUnApprove = request.IsApprove ? "" : (request.ReasonUnApprove ?? "");
                existingApproval.Note = request.Note ?? "";
                existingApproval.UpdatedDate = DateTime.Now;
                existingApproval.UpdatedBy = request.ApproverName ?? "SYSTEM";

                _hiringRequestApproveLinkRepo.UpdateAsync(existingApproval);
            }
            else
            {
                // Tạo mới record
                var newApproval = new HRHiringRequestApproveLink
                {
                    HRHiringRequestID = request.HiringRequestID,
                    ApproveID = request.ApproveID,
                    Step = request.Step,
                    StepName = GetStepName(request.Step),
                    IsApprove = request.IsApprove,
                    DateApprove = request.IsApprove ? DateTime.Now : (DateTime?)null,
                    ReasonUnApprove = request.IsApprove ? "" : (request.ReasonUnApprove ?? ""),
                    Note = request.Note ?? "",
                    //CreatedDate = DateTime.Now,
                    //CreatedBy = request.ApproverName ?? "SYSTEM",
                    //IsDeleted = false
                };

                await _hiringRequestApproveLinkRepo.CreateAsync(newApproval);
            }

            // Nếu là hủy duyệt, xóa các step cao hơn
            if (!request.IsApprove)
            {
                var higherSteps = currentApprovals.Where(x => x.Step > request.Step && x.IsApprove == true);
                foreach (var step in higherSteps)
                {
                    step.IsDeleted = true;
                    step.UpdatedDate = DateTime.Now;
                    step.UpdatedBy = request.ApproverName ?? "SYSTEM";
                    _hiringRequestApproveLinkRepo.UpdateAsync( step);
                }
            }
        }

        private string GetStepName(int step)
        {
            return step switch
            {
                1 => "HCNS",
                2 => "TBP",
                3 => "BGĐ",
                _ => $"Step {step}"
            };
        }

        // Helper classes
        public class ApproveRequestDTO
        {
            public int HiringRequestID { get; set; }
            public int ApproveID { get; set; }
            public int Step { get; set; }
            public bool IsApprove { get; set; }
            public string? ReasonUnApprove { get; set; }
            public string? Note { get; set; }
            public string? ApproverName { get; set; }
        }

        public class ApprovalResult
        {
            public bool Success { get; set; }
            public string Message { get; set; } = "";
        }

        // API lấy trạng thái duyệt
        [HttpGet("get-approval-status/{hiringRequestId}")]
        public IActionResult GetApprovalStatus(int hiringRequestId)
        {
            try
            {
                var approvals = _hiringRequestApproveLinkRepo.GetAll()
                    .Where(x => x.HRHiringRequestID == hiringRequestId && x.IsDeleted != true)
                    .OrderBy(x => x.Step)
                    .Select(x => new
                    {
                        x.Step,
                        x.StepName,
                        x.IsApprove,
                        x.DateApprove,
                        x.ReasonUnApprove,
                        x.Note,
                        x.CreatedBy,
                        x.CreatedDate
                    })
                    .ToList();

                var currentStep = GetCurrentApprovalStep(_hiringRequestApproveLinkRepo.GetAll()
                    .Where(x => x.HRHiringRequestID == hiringRequestId && x.IsDeleted != true)
                    .ToList());

                return Ok(ApiResponseFactory.Success(new
                {
                    approvals,
                    currentStep,
                    canApproveHCNS = currentStep == 0,
                    canApproveTBP = currentStep == 1,
                    canApproveBGD = currentStep == 2,
                    canCancelHCNS = currentStep >= 1,
                    canCancelTBP = currentStep >= 2,
                    canCancelBGD = currentStep == 3
                }, "Lấy trạng thái duyệt thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
