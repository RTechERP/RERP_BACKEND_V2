using Microsoft.AspNetCore.Authorization;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.Poll;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Globalization;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using PollFormEntity = RERPAPI.Model.Entities.PollForm;

namespace RERPAPI.Controllers.PollForm
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PollFormController : ControllerBase
    {
        private readonly PollFormRepo _pollFormRepo;
        private readonly PollSectionRepo _pollSectionRepo;
        private readonly PollQuestionRepo _pollQuestionRepo;
        private readonly PollQuestionOptionRepo _pollQuestionOptionRepo;
        private readonly PollResponseRepo _pollResponseRepo;
        private readonly PollResponseAnswerRepo _pollResponseAnswerRepo;
        private readonly EmployeeRepo _employeeRepo;
        private readonly PollBranchingRuleEvaluator _pollBranchingRuleEvaluator;
        private const string EmployeeDataSourceType = "Employee";

        private static readonly List<PollEmployeeFieldOptionDTO> EmployeeFieldOptions = new()
        {
            new() { FieldKey = nameof(Employee.Code), Label = "Mã nhân viên", DataType = "string" },
            new() { FieldKey = nameof(Employee.FullName), Label = "Họ tên", DataType = "string" },
            new() { FieldKey = nameof(Employee.DepartmentID), Label = "Phòng ban", DataType = "number", DisplayType = "lookup", LookupSource = "Department.Name" },
            new() { FieldKey = nameof(Employee.Position), Label = "Chức danh", DataType = "string" },
            new() { FieldKey = nameof(Employee.ChucVuHDID), Label = "Chức vụ HDLD", DataType = "number", DisplayType = "lookup", LookupSource = "EmployeeChucVuHD.Name" },
            new() { FieldKey = nameof(Employee.ChuVuID), Label = "Chức vụ nội bộ", DataType = "number", DisplayType = "lookup", LookupSource = "EmployeeChucVu.Name" },
            new() { FieldKey = nameof(Employee.TeamID), Label = "Team", DataType = "number", DisplayType = "lookup", LookupSource = "Team.Name" },
            new() { FieldKey = nameof(Employee.EmployeeTeamID), Label = "Nhóm nhân viên", DataType = "number", DisplayType = "lookup", LookupSource = "EmployeeTeam.Name" },
            new() { FieldKey = nameof(Employee.ProjectTypeID), Label = "Loại dự án", DataType = "number", DisplayType = "lookup", LookupSource = "ProjectType.ProjectTypeName" },
            new() { FieldKey = nameof(Employee.TaxCompanyID), Label = "Công ty tính thuế", DataType = "number", DisplayType = "lookup", LookupSource = "TaxCompany.Name" },
            new() { FieldKey = nameof(Employee.Leader), Label = "Quản lý trực tiếp", DataType = "number", DisplayType = "lookup", LookupSource = "Employee.FullName" },
            new() { FieldKey = nameof(Employee.StartWorking), Label = "Ngày vào làm", DataType = "date", SuggestedQuestionType = "Date" },
            new() { FieldKey = nameof(Employee.EndWorking), Label = "Ngày nghỉ việc", DataType = "date", SuggestedQuestionType = "Date" },
            new() { FieldKey = nameof(Employee.Status), Label = "Trạng thái nhân viên", DataType = "number", DisplayType = "lookup", LookupSource = "EmployeeStatus.StatusName" },
            new() { FieldKey = nameof(Employee.GioiTinh), Label = "Giới tính", DataType = "number", DisplayType = "enum", LookupSource = "0:Nữ;1:Nam;3:Khác" },
            new() { FieldKey = nameof(Employee.DiaDiemLamViec), Label = "Địa điểm làm việc", DataType = "string" },
            new() { FieldKey = nameof(Employee.EmailCom), Label = "Email công ty", DataType = "string" },
            new() { FieldKey = nameof(Employee.EmailCongTy), Label = "Email công ty HR", DataType = "string" },
            new() { FieldKey = nameof(Employee.SDTCongTy), Label = "Số điện thoại công ty", DataType = "string" },
            new() { FieldKey = nameof(Employee.NoiSinh), Label = "Nơi sinh", DataType = "string", IsSensitive = true },
            new() { FieldKey = nameof(Employee.BirthOfDate), Label = "Ngày sinh", DataType = "date", SuggestedQuestionType = "Date", IsSensitive = true },
            new() { FieldKey = nameof(Employee.SDTCaNhan), Label = "Số điện thoại cá nhân", DataType = "string", IsSensitive = true },
            new() { FieldKey = nameof(Employee.EmailCaNhan), Label = "Email cá nhân", DataType = "string", IsSensitive = true },
            new() { FieldKey = nameof(Employee.DanToc), Label = "Dân tộc", DataType = "string", IsSensitive = true },
            new() { FieldKey = nameof(Employee.TonGiao), Label = "Tôn giáo", DataType = "string", IsSensitive = true },
            new() { FieldKey = nameof(Employee.QuocTich), Label = "Quốc tịch", DataType = "string", IsSensitive = true },
            new() { FieldKey = nameof(Employee.CMTND), Label = "CCCD", DataType = "string", IsSensitive = true },
            new() { FieldKey = nameof(Employee.NgayCap), Label = "Ngày cấp CCCD", DataType = "string", IsSensitive = true },
            new() { FieldKey = nameof(Employee.NoiCap), Label = "Nơi cấp CCCD", DataType = "string", IsSensitive = true },
            new() { FieldKey = nameof(Employee.DcThuongTru), Label = "Địa chỉ thường trú", DataType = "string", IsSensitive = true },
            new() { FieldKey = nameof(Employee.TinhTrangHonNhanID), Label = "Tình trạng hôn nhân", DataType = "number", DisplayType = "lookup", LookupSource = "EmployeeTinhTrangHonNhan.Name", IsSensitive = true }
        };

        private static readonly Dictionary<string, PollEmployeeFieldOptionDTO> EmployeeFieldOptionMap =
            EmployeeFieldOptions.ToDictionary(x => x.FieldKey, StringComparer.OrdinalIgnoreCase);

        public PollFormController(
            PollFormRepo pollFormRepo,
            PollSectionRepo pollSectionRepo,
            PollQuestionRepo pollQuestionRepo,
            PollQuestionOptionRepo pollQuestionOptionRepo,
            PollResponseRepo pollResponseRepo,
            PollResponseAnswerRepo pollResponseAnswerRepo,
            EmployeeRepo employeeRepo,
            PollBranchingRuleEvaluator pollBranchingRuleEvaluator)
        {
            _pollFormRepo = pollFormRepo;
            _pollSectionRepo = pollSectionRepo;
            _pollQuestionRepo = pollQuestionRepo;
            _pollQuestionOptionRepo = pollQuestionOptionRepo;
            _pollResponseRepo = pollResponseRepo;
            _pollResponseAnswerRepo = pollResponseAnswerRepo;
            _employeeRepo = employeeRepo;
            _pollBranchingRuleEvaluator = pollBranchingRuleEvaluator;
        }

        /// <summary>
        /// Get all poll forms
        /// </summary>
        [HttpGet("all")]
        public IActionResult GetAllPollForms()
        {
            try
            {
                var currentUser = GetCurrentUser();
                var pollForms = _pollFormRepo.GetAll(x => x.IsDeleted != true)
                    .Where(x => x.IsPublic == true || CanManagePoll(x, currentUser))
                    .OrderByDescending(x => x.CreatedDate)
                    .ToList();
                return Ok(ApiResponseFactory.Success(pollForms, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Get employee fields that can be mapped to poll questions
        /// </summary>
        [HttpGet("employee-field-options")]
        [HttpGet("employee-fields")]
        public IActionResult GetEmployeeFieldOptions()
        {
            return Ok(ApiResponseFactory.Success(EmployeeFieldOptions, ""));
        }

        /// <summary>
        /// Get poll form by ID with questions and options
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetPollFormDetail(int id)
        {
            try
            {
                var currentUser = GetCurrentUser();
                var pollForm = _pollFormRepo.GetByID(id);
                if (pollForm == null || pollForm.ID <= 0 || pollForm.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Poll form not found"));

                var viewFailure = EnsureCanViewPollForm(pollForm, currentUser);
                if (viewFailure != null)
                    return viewFailure;

                var sections = _pollSectionRepo.GetAll(x => x.PollFormID == id && x.IsDeleted != true)
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.ID)
                    .ToList();

                var questions = _pollQuestionRepo.GetAll(x => x.PollFormID == id)
                    .OrderBy(x => x.PollSectionID)
                    .ThenBy(x => x.SortOrder)
                    .ThenBy(x => x.ID)
                    .ToList();

                var currentEmployee = GetCurrentEmployee();
                var questionDetails = questions.Select(q => MapQuestionDetail(q, currentEmployee)).ToList();

                var pollFormDetail = new PollFormDetailDTO
                {
                    ID = pollForm.ID,
                    Title = pollForm.Title,
                    Description = pollForm.Description,
                    StartDate = pollForm.StartDate,
                    EndDate = pollForm.EndDate,
                    IsPublic = pollForm.IsPublic,
                    IsDeleted = pollForm.IsDeleted,
                    CreatedBy = pollForm.CreatedBy,
                    CreatedDate = pollForm.CreatedDate,
                    UpdatedBy = pollForm.UpdatedBy,
                    UpdatedDate = pollForm.UpdatedDate,
                    Questions = questionDetails,
                    Sections = sections.Select(s => new PollSectionDetailDTO
                    {
                        ID = s.ID,
                        PollFormID = s.PollFormID,
                        Title = s.Title,
                        Description = s.Description,
                        SortOrder = s.SortOrder,
                        ShowIfJson = s.ShowIfJson,
                        BranchingRulesJson = s.BranchingRulesJson,
                        IsDeleted = s.IsDeleted,
                        CreatedBy = s.CreatedBy,
                        CreatedDate = s.CreatedDate,
                        UpdatedBy = s.UpdatedBy,
                        UpdatedDate = s.UpdatedDate,
                        Questions = questionDetails
                            .Where(q => q.SectionID == s.ID)
                            .OrderBy(q => q.SortOrder)
                            .ThenBy(q => q.ID)
                            .ToList()
                    }).ToList()
                };

                return Ok(ApiResponseFactory.Success(pollFormDetail, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Get the current employee's latest response for a poll form
        /// </summary>
        [HttpGet("{pollFormId}/my-response")]
        public IActionResult GetMyPollResponse(int pollFormId)
        {
            try
            {
                var currentUser = GetCurrentUser();
                var pollForm = _pollFormRepo.GetByID(pollFormId);
                if (pollForm == null || pollForm.ID <= 0 || pollForm.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Poll form not found"));

                var viewFailure = EnsureCanViewPollForm(pollForm, currentUser);
                if (viewFailure != null)
                    return viewFailure;

                var employeeId = GetCurrentEmployeeId();
                if (employeeId <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Current employee could not be determined"));

                var response = GetLatestEmployeeResponse(pollFormId, employeeId);
                var now = DateTime.Now;
                var canEdit = CanEditPoll(pollForm, now, out var closedReason) && pollForm.IsPublic == true;
                if (pollForm.IsPublic != true)
                    closedReason = "Poll form is not public yet";

                var result = new PollEmployeeResponseStatusDTO
                {
                    PollFormID = pollFormId,
                    EmployeeID = employeeId,
                    HasResponse = response != null && response.ID > 0,
                    IsCompleted = response?.IsCompleted == true,
                    CanEdit = canEdit,
                    IsClosed = !canEdit,
                    ClosedReason = closedReason,
                    StartDate = pollForm.StartDate,
                    EndDate = pollForm.EndDate,
                    Response = response != null && response.ID > 0 ? MapResponseDetail(response) : null
                };

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Create a new poll form
        /// </summary>
        [HttpPost("create")]
        public IActionResult CreatePollForm([FromBody] PollFormCreateDTO dto)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var pollForm = new PollFormEntity
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    IsPublic = dto.IsPublic ?? false,
                    IsDeleted = false,
                    CreatedBy = currentUser.LoginName,
                    CreatedDate = DateTime.Now
                };

                var result = _pollFormRepo.Create(pollForm);
                if (result > 0)
                    return Ok(ApiResponseFactory.Success(pollForm, "Poll form created successfully"));

                return BadRequest(ApiResponseFactory.Fail(null, "Failed to create poll form"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Update a poll form
        /// </summary>
        [HttpPut("update")]
        public IActionResult UpdatePollForm([FromBody] PollFormUpdateDTO dto)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var pollForm = _pollFormRepo.GetByID(dto.ID);
                if (pollForm == null || pollForm.ID <= 0 || pollForm.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Poll form not found"));

                var manageFailure = EnsureCanManagePoll(pollForm, currentUser);
                if (manageFailure != null)
                    return manageFailure;

                pollForm.Title = dto.Title ?? pollForm.Title;
                pollForm.Description = dto.Description ?? pollForm.Description;
                pollForm.StartDate = dto.StartDate ?? pollForm.StartDate;
                pollForm.EndDate = dto.EndDate ?? pollForm.EndDate;
                pollForm.IsPublic = dto.IsPublic ?? pollForm.IsPublic;
                pollForm.UpdatedBy = currentUser.LoginName;
                pollForm.UpdatedDate = DateTime.Now;

                var result = _pollFormRepo.Update(pollForm);
                if (result > 0)
                    return Ok(ApiResponseFactory.Success(pollForm, "Poll form updated successfully"));

                return BadRequest(ApiResponseFactory.Fail(null, "Failed to update poll form"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Delete a poll form (soft delete)
        /// </summary>
        [HttpDelete("delete/{id}")]
        public IActionResult DeletePollForm(int id)
        {
            try
            {
                var currentUser = GetCurrentUser();
                var pollForm = _pollFormRepo.GetByID(id);
                if (pollForm == null || pollForm.ID <= 0 || pollForm.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Poll form not found"));

                var manageFailure = EnsureCanManagePoll(pollForm, currentUser);
                if (manageFailure != null)
                    return manageFailure;

                pollForm.IsDeleted = true;
                var result = _pollFormRepo.Update(pollForm);

                if (result > 0)
                    return Ok(ApiResponseFactory.Success(null, "Poll form deleted successfully"));

                return BadRequest(ApiResponseFactory.Fail(null, "Failed to delete poll form"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Create a section for a poll form
        /// </summary>
        [HttpPost("{pollFormId}/section")]
        public IActionResult CreateSection(int pollFormId, [FromBody] PollSectionCreateDTO dto)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var pollForm = _pollFormRepo.GetByID(pollFormId);
                if (pollForm == null || pollForm.ID <= 0 || pollForm.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Poll form not found"));

                var manageFailure = EnsureCanManagePoll(pollForm, currentUser);
                if (manageFailure != null)
                    return manageFailure;

                var nextSortOrder = _pollSectionRepo.GetAll(x => x.PollFormID == pollFormId && x.IsDeleted != true)
                    .Select(x => x.SortOrder ?? 0)
                    .DefaultIfEmpty(0)
                    .Max() + 1;

                var section = new PollSection
                {
                    PollFormID = pollFormId,
                    Title = dto.Title,
                    Description = dto.Description,
                    SortOrder = dto.SortOrder ?? nextSortOrder,
                    ShowIfJson = dto.ShowIfJson,
                    BranchingRulesJson = dto.BranchingRulesJson,
                    IsDeleted = false,
                    CreatedBy = currentUser.LoginName,
                    CreatedDate = DateTime.Now
                };

                var result = _pollSectionRepo.Create(section);
                if (result > 0)
                    return Ok(ApiResponseFactory.Success(section, "Section created successfully"));

                return BadRequest(ApiResponseFactory.Fail(null, "Failed to create section"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Update a section
        /// </summary>
        [HttpPut("section/{id}")]
        public IActionResult UpdateSection(int id, [FromBody] PollSectionUpdateDTO dto)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var section = _pollSectionRepo.GetByID(id);
                if (section == null || section.ID <= 0 || section.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Section not found"));

                var pollForm = _pollFormRepo.GetByID(section.PollFormID ?? 0);
                if (pollForm == null || pollForm.ID <= 0 || pollForm.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Poll form not found"));

                var manageFailure = EnsureCanManagePoll(pollForm, currentUser);
                if (manageFailure != null)
                    return manageFailure;

                section.Title = dto.Title ?? section.Title;
                section.Description = dto.Description ?? section.Description;
                section.SortOrder = dto.SortOrder ?? section.SortOrder;
                section.ShowIfJson = dto.ShowIfJson ?? section.ShowIfJson;
                section.BranchingRulesJson = dto.BranchingRulesJson ?? section.BranchingRulesJson;
                section.IsDeleted = dto.IsDeleted ?? section.IsDeleted;
                section.UpdatedBy = currentUser.LoginName;
                section.UpdatedDate = DateTime.Now;

                var result = _pollSectionRepo.Update(section);
                if (result > 0)
                    return Ok(ApiResponseFactory.Success(section, "Section updated successfully"));

                return BadRequest(ApiResponseFactory.Fail(null, "Failed to update section"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Delete a section (soft delete)
        /// </summary>
        [HttpDelete("section/{id}")]
        public IActionResult DeleteSection(int id)
        {
            try
            {
                var currentUser = GetCurrentUser();
                var section = _pollSectionRepo.GetByID(id);
                if (section == null || section.ID <= 0 || section.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Section not found"));

                var pollForm = _pollFormRepo.GetByID(section.PollFormID ?? 0);
                if (pollForm == null || pollForm.ID <= 0 || pollForm.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Poll form not found"));

                var manageFailure = EnsureCanManagePoll(pollForm, currentUser);
                if (manageFailure != null)
                    return manageFailure;

                section.IsDeleted = true;
                section.UpdatedDate = DateTime.Now;

                var result = _pollSectionRepo.Update(section);
                if (result > 0)
                    return Ok(ApiResponseFactory.Success(null, "Section deleted successfully"));

                return BadRequest(ApiResponseFactory.Fail(null, "Failed to delete section"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Add a question to a poll form
        /// </summary>
        [HttpPost("question/add")]
        public IActionResult AddQuestion([FromBody] PollQuestionCreateDTO dto)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var pollForm = _pollFormRepo.GetByID(dto.PollFormID ?? 0);
                if (pollForm == null || pollForm.ID <= 0 || pollForm.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Poll form not found"));

                var manageFailure = EnsureCanManagePoll(pollForm, currentUser);
                if (manageFailure != null)
                    return manageFailure;

                if (dto.SectionID.HasValue)
                {
                    var section = _pollSectionRepo.GetByID(dto.SectionID.Value);
                    if (section == null || section.ID <= 0 || section.PollFormID != dto.PollFormID || section.IsDeleted == true)
                        return BadRequest(ApiResponseFactory.Fail(null, "Section is invalid for this poll form"));
                }

                if (!TryNormalizeQuestionDataSource(dto.DataSourceType, dto.DataSourceField, out var dataSourceType, out var dataSourceField, out var validationMessage))
                    return BadRequest(ApiResponseFactory.Fail(null, validationMessage));

                var question = new PollQuestion
                {
                    PollFormID = dto.PollFormID,
                    PollSectionID = dto.SectionID,
                    QuestionText = dto.QuestionText,
                    FieldKey = dto.FieldKey,
                    QuestionType = dto.QuestionType ?? "Text",
                    IsRequired = dto.IsRequired,
                    SortOrder = dto.SortOrder ?? 0,
                    ConfigJson = dto.ConfigJson,
                    DataSourceType = dataSourceType,
                    DataSourceField = dataSourceField,
                    CreatedBy = currentUser.LoginName,
                    CreatedDate = DateTime.Now
                };

                var result = _pollQuestionRepo.Create(question);
                if (result > 0)
                    return Ok(ApiResponseFactory.Success(question, "Question added successfully"));

                return BadRequest(ApiResponseFactory.Fail(null, "Failed to add question"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Update a question
        /// </summary>
        [HttpPut("question/update")]
        public IActionResult UpdateQuestion([FromBody] PollQuestionUpdateDTO dto)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var question = _pollQuestionRepo.GetByID(dto.ID);
                if (question == null || question.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Question not found"));

                var pollForm = _pollFormRepo.GetByID(question.PollFormID ?? 0);
                if (pollForm == null || pollForm.ID <= 0 || pollForm.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Poll form not found"));

                var manageFailure = EnsureCanManagePoll(pollForm, currentUser);
                if (manageFailure != null)
                    return manageFailure;

                if (dto.SectionID.HasValue)
                {
                    var section = _pollSectionRepo.GetByID(dto.SectionID.Value);
                    if (section == null || section.ID <= 0 || section.PollFormID != question.PollFormID || section.IsDeleted == true)
                        return BadRequest(ApiResponseFactory.Fail(null, "Section is invalid for this question"));
                }

                question.PollSectionID = dto.SectionID ?? question.PollSectionID;
                question.QuestionText = dto.QuestionText ?? question.QuestionText;
                question.FieldKey = dto.FieldKey ?? question.FieldKey;
                question.QuestionType = dto.QuestionType ?? question.QuestionType;
                question.IsRequired = dto.IsRequired ?? question.IsRequired;
                question.SortOrder = dto.SortOrder ?? question.SortOrder;
                question.ConfigJson = dto.ConfigJson ?? question.ConfigJson;

                if (dto.DataSourceType != null || dto.DataSourceField != null)
                {
                    var requestedDataSourceType = dto.DataSourceType ?? question.DataSourceType;
                    var requestedDataSourceField = dto.DataSourceField ?? question.DataSourceField;

                    if (!TryNormalizeQuestionDataSource(requestedDataSourceType, requestedDataSourceField, out var dataSourceType, out var dataSourceField, out var validationMessage))
                        return BadRequest(ApiResponseFactory.Fail(null, validationMessage));

                    question.DataSourceType = dataSourceType ?? "";
                    question.DataSourceField = dataSourceField ?? "";
                }

                question.UpdatedBy = currentUser.LoginName;
                question.UpdatedDate = DateTime.Now;

                var result = _pollQuestionRepo.Update(question);
                if (result > 0)
                    return Ok(ApiResponseFactory.Success(question, "Question updated successfully"));

                return BadRequest(ApiResponseFactory.Fail(null, "Failed to update question"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Delete a question
        /// </summary>
        [HttpDelete("question/delete/{id}")]
        public IActionResult DeleteQuestion(int id)
        {
            try
            {
                var currentUser = GetCurrentUser();
                var question = _pollQuestionRepo.GetByID(id);
                if (question == null || question.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Question not found"));

                var pollForm = _pollFormRepo.GetByID(question.PollFormID ?? 0);
                if (pollForm == null || pollForm.ID <= 0 || pollForm.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Poll form not found"));

                var manageFailure = EnsureCanManagePoll(pollForm, currentUser);
                if (manageFailure != null)
                    return manageFailure;

                var result = _pollQuestionRepo.Delete(id);
                if (result > 0)
                    return Ok(ApiResponseFactory.Success(null, "Question deleted successfully"));

                return BadRequest(ApiResponseFactory.Fail(null, "Failed to delete question"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Add an option to a question
        /// </summary>
        [HttpPost("option/add")]
        public IActionResult AddOption([FromBody] PollQuestionOptionCreateDTO dto)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var question = _pollQuestionRepo.GetByID(dto.PollQuestionID ?? 0);
                if (question == null || question.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Question not found"));

                var pollForm = _pollFormRepo.GetByID(question.PollFormID ?? 0);
                if (pollForm == null || pollForm.ID <= 0 || pollForm.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Poll form not found"));

                var manageFailure = EnsureCanManagePoll(pollForm, currentUser);
                if (manageFailure != null)
                    return manageFailure;

                var option = new PollQuestionOption
                {
                    PollQuestionID = dto.PollQuestionID,
                    OptionText = dto.OptionText,
                    OptionValue = dto.OptionValue,
                    SortOrder = dto.SortOrder ?? 0,
                    CreatedBy = currentUser.LoginName,
                    CreatedDate = DateTime.Now
                };

                var result = _pollQuestionOptionRepo.Create(option);
                if (result > 0)
                    return Ok(ApiResponseFactory.Success(option, "Option added successfully"));

                return BadRequest(ApiResponseFactory.Fail(null, "Failed to add option"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Update an option
        /// </summary>
        [HttpPut("option/update")]
        public IActionResult UpdateOption([FromBody] PollQuestionOptionUpdateDTO dto)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var option = _pollQuestionOptionRepo.GetByID(dto.ID);
                if (option == null || option.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Option not found"));

                var question = _pollQuestionRepo.GetByID(option.PollQuestionID ?? 0);
                if (question == null || question.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Question not found"));

                var pollForm = _pollFormRepo.GetByID(question.PollFormID ?? 0);
                if (pollForm == null || pollForm.ID <= 0 || pollForm.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Poll form not found"));

                var manageFailure = EnsureCanManagePoll(pollForm, currentUser);
                if (manageFailure != null)
                    return manageFailure;

                option.OptionText = dto.OptionText ?? option.OptionText;
                option.OptionValue = dto.OptionValue ?? option.OptionValue;
                option.SortOrder = dto.SortOrder ?? option.SortOrder;
                option.UpdatedBy = currentUser.LoginName;
                option.UpdatedDate = DateTime.Now;

                var result = _pollQuestionOptionRepo.Update(option);
                if (result > 0)
                    return Ok(ApiResponseFactory.Success(option, "Option updated successfully"));

                return BadRequest(ApiResponseFactory.Fail(null, "Failed to update option"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Delete an option
        /// </summary>
        [HttpDelete("option/delete/{id}")]
        public IActionResult DeleteOption(int id)
        {
            try
            {
                var currentUser = GetCurrentUser();
                var option = _pollQuestionOptionRepo.GetByID(id);
                if (option == null || option.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Option not found"));

                var question = _pollQuestionRepo.GetByID(option.PollQuestionID ?? 0);
                if (question == null || question.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Question not found"));

                var pollForm = _pollFormRepo.GetByID(question.PollFormID ?? 0);
                if (pollForm == null || pollForm.ID <= 0 || pollForm.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Poll form not found"));

                var manageFailure = EnsureCanManagePoll(pollForm, currentUser);
                if (manageFailure != null)
                    return manageFailure;

                var result = _pollQuestionOptionRepo.Delete(id);
                if (result > 0)
                    return Ok(ApiResponseFactory.Success(null, "Option deleted successfully"));

                return BadRequest(ApiResponseFactory.Fail(null, "Failed to delete option"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Submit answers for one section and return the next section
        /// </summary>
        [HttpPost("{pollFormId}/submit-section")]
        public IActionResult SubmitPollSection(int pollFormId, [FromBody] SubmitPollSectionDTO dto)
        {
            try
            {
                var currentUser = GetCurrentUser();

                if (dto.SectionID == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "SectionID is required"));

                var pollForm = _pollFormRepo.GetByID(pollFormId);
                if (pollForm == null || pollForm.ID <= 0 || pollForm.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Poll form not found"));

                var voteFailure = EnsureCanVotePollForm(pollForm);
                if (voteFailure != null)
                    return voteFailure;

                if (!CanEditPoll(pollForm, DateTime.Now, out var closedReason))
                    return BadRequest(ApiResponseFactory.Fail(null, closedReason ?? "Poll is closed"));

                var section = _pollSectionRepo.GetByID(dto.SectionID.Value);
                if (section == null || section.ID <= 0 || section.PollFormID != pollFormId || section.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Section not found"));

                var employeeId = GetCurrentEmployeeId(dto.EmployeeID);
                var sectionQuestions = _pollQuestionRepo.GetAll(x => x.PollFormID == pollFormId && x.PollSectionID == section.ID)
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.ID)
                    .ToList();

                var currentEmployee = GetCurrentEmployee(employeeId);

                using var dbContext = CreateDbContext(currentUser);
                using var transaction = dbContext.Database.BeginTransaction();

                PollResponse response;
                if (dto.PollResponseID.HasValue)
                {
                    response = dbContext.PollResponses
                        .FirstOrDefault(x => x.ID == dto.PollResponseID.Value && x.PollFormID == pollFormId) ?? new PollResponse();
                    if (response.ID <= 0)
                        return BadRequest(ApiResponseFactory.Fail(null, "Poll response is invalid for this poll form"));

                    if (!CanManagePoll(pollForm, currentUser) &&
                        (employeeId <= 0 || response.EmployeeID != employeeId))
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Cannot update another employee's poll response"));
                    }
                }
                else
                {
                    response = employeeId > 0
                        ? GetLatestEmployeeResponse(dbContext, pollFormId, employeeId) ?? new PollResponse()
                        : new PollResponse();

                    if (response.ID <= 0)
                    {
                        response.PollFormID = pollFormId;
                        response.EmployeeID = employeeId > 0 ? employeeId : dto.EmployeeID;
                        response.IsCompleted = false;
                        response.CreatedBy = currentUser.LoginName;
                        response.CreatedDate = DateTime.Now;
                        dbContext.PollResponses.Add(response);
                        dbContext.SaveChanges();
                    }
                }

                var answers = AddEmployeeMappedAnswers(dto.Answers, sectionQuestions, currentEmployee, dbContext);
                if (!TryValidateAnswers(answers, sectionQuestions, dbContext, out var validationMessage, out var validationData))
                    return BadRequest(ApiResponseFactory.Fail(null, validationMessage, validationData));

                var savedAnswerCount = SaveResponseAnswers(dbContext, response.ID, answers, currentUser);
                dbContext.SaveChanges();

                var allSections = dbContext.PollSections
                    .Where(x => x.PollFormID == pollFormId && x.IsDeleted != true)
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.ID)
                    .ToList();
                var allQuestions = dbContext.PollQuestions.Where(x => x.PollFormID == pollFormId).ToList();
                var allResponseAnswers = dbContext.PollResponseAnswers.Where(x => x.PollResponseID == response.ID).ToList();
                var answerMap = BuildAnswerMap(allQuestions, allResponseAnswers);
                var nextSectionId = _pollBranchingRuleEvaluator.ResolveNextSectionId(section, allSections, answerMap);
                var isCompleted = nextSectionId == null;

                response.IsCompleted = response.IsCompleted == true || isCompleted;
                response.CompletedDate = response.IsCompleted == true
                    ? response.CompletedDate ?? DateTime.Now
                    : response.CompletedDate;
                response.UpdatedBy = currentUser.LoginName;
                response.UpdatedDate = DateTime.Now;
                dbContext.SaveChanges();
                transaction.Commit();

                var result = new SubmitPollSectionResultDTO
                {
                    PollResponseID = response.ID,
                    PollFormID = pollFormId,
                    SectionID = section.ID,
                    NextSectionID = nextSectionId,
                    IsCompleted = isCompleted,
                    SavedAnswerCount = savedAnswerCount
                };

                return Ok(ApiResponseFactory.Success(result, "Section submitted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Submit poll response (voting)
        /// </summary>
        [HttpPost("submit")]
        public IActionResult SubmitPollResponse([FromBody] SubmitPollResponseDTO dto)
        {
            try
            {
                var currentUser = GetCurrentUser();

                var pollForm = _pollFormRepo.GetByID(dto.PollFormID ?? 0);
                if (pollForm == null || pollForm.ID <= 0 || pollForm.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Poll form not found"));

                var voteFailure = EnsureCanVotePollForm(pollForm);
                if (voteFailure != null)
                    return voteFailure;

                if (!CanEditPoll(pollForm, DateTime.Now, out var closedReason))
                    return BadRequest(ApiResponseFactory.Fail(null, closedReason ?? "Poll is closed"));

                var employeeId = GetCurrentEmployeeId(dto.EmployeeID);
                var currentEmployee = GetCurrentEmployee(employeeId);

                using var dbContext = CreateDbContext(currentUser);
                using var transaction = dbContext.Database.BeginTransaction();

                var response = employeeId > 0
                    ? GetLatestEmployeeResponse(dbContext, pollForm.ID, employeeId) ?? new PollResponse()
                    : new PollResponse();
                var pollQuestions = dbContext.PollQuestions
                    .Where(x => x.PollFormID == pollForm.ID)
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.ID)
                    .ToList();
                var answers = AddEmployeeMappedAnswers(dto.Answers, pollQuestions, currentEmployee, dbContext);

                if (!TryValidateAnswers(answers, pollQuestions, dbContext, out var validationMessage, out var validationData))
                    return BadRequest(ApiResponseFactory.Fail(null, validationMessage, validationData));

                if (response.ID <= 0)
                {
                    response.PollFormID = pollForm.ID;
                    response.EmployeeID = employeeId > 0 ? employeeId : dto.EmployeeID;
                    response.IsCompleted = true;
                    response.CompletedDate = DateTime.Now;
                    response.CreatedBy = currentUser.LoginName;
                    response.CreatedDate = DateTime.Now;
                    dbContext.PollResponses.Add(response);
                    dbContext.SaveChanges();
                }

                response.IsCompleted = true;
                response.CompletedDate = response.CompletedDate ?? DateTime.Now;
                response.UpdatedBy = currentUser.LoginName;
                response.UpdatedDate = DateTime.Now;

                SaveResponseAnswers(dbContext, response.ID, answers, currentUser);
                dbContext.SaveChanges();
                transaction.Commit();

                return Ok(ApiResponseFactory.Success(MapResponseDetail(response), "Poll response submitted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Get all responses for a poll form
        /// </summary>
        [HttpGet("{pollFormId}/responses")]
        public IActionResult GetPollResponses(int pollFormId)
        {
            try
            {
                var currentUser = GetCurrentUser();
                var pollForm = _pollFormRepo.GetByID(pollFormId);
                if (pollForm == null || pollForm.ID <= 0 || pollForm.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Poll form not found"));

                if (!CanManagePoll(pollForm, currentUser))
                    return ForbiddenResponse("You do not have permission to view poll responses");

                var responses = _pollResponseRepo.GetAll(x => x.PollFormID == pollFormId)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToList();

                var responseList = responses.Select(MapResponseDetail).ToList();

                return Ok(ApiResponseFactory.Success(responseList, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Export poll responses to Excel. Each employee response is one row.
        /// </summary>
        [HttpGet("{pollFormId}/responses/export-excel")]
        public IActionResult ExportPollResponsesExcel(int pollFormId, [FromQuery] bool includeIncomplete = false)
        {
            try
            {
                var currentUser = GetCurrentUser();
                var pollForm = _pollFormRepo.GetByID(pollFormId);
                if (pollForm == null || pollForm.ID <= 0 || pollForm.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Poll form not found"));

                if (!CanManagePoll(pollForm, currentUser))
                    return ForbiddenResponse("You do not have permission to export poll responses");

                var sections = _pollSectionRepo.GetAll(x => x.PollFormID == pollFormId && x.IsDeleted != true)
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.ID)
                    .ToList();
                var sectionSortOrders = sections.ToDictionary(x => x.ID, x => x.SortOrder ?? 0);
                var sectionsById = sections.ToDictionary(x => x.ID);

                var questions = _pollQuestionRepo.GetAll(x => x.PollFormID == pollFormId)
                    .OrderBy(x => x.PollSectionID.HasValue && sectionSortOrders.ContainsKey(x.PollSectionID.Value)
                        ? sectionSortOrders[x.PollSectionID.Value]
                        : int.MaxValue)
                    .ThenBy(x => x.PollSectionID ?? int.MaxValue)
                    .ThenBy(x => x.SortOrder ?? 0)
                    .ThenBy(x => x.ID)
                    .ToList();

                var responses = _pollResponseRepo.GetAll(x => x.PollFormID == pollFormId)
                    .Where(x => includeIncomplete || x.IsCompleted == true)
                    .OrderBy(x => x.EmployeeID ?? 0)
                    .ThenBy(x => x.CreatedDate)
                    .ThenBy(x => x.ID)
                    .ToList();

                if (responses.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu bình chọn để xuất Excel"));

                var responseIds = responses.Select(x => x.ID).ToHashSet();
                var questionIds = questions.Select(x => x.ID).ToHashSet();

                var answers = _pollResponseAnswerRepo.GetAll(x =>
                        x.PollResponseID.HasValue &&
                        responseIds.Contains(x.PollResponseID.Value) &&
                        x.PollQuestionID.HasValue &&
                        questionIds.Contains(x.PollQuestionID.Value))
                    .ToList();

                var answersByResponse = answers
                    .GroupBy(x => x.PollResponseID!.Value)
                    .ToDictionary(
                        g => g.Key,
                        g => g.GroupBy(x => x.PollQuestionID!.Value)
                            .ToDictionary(
                                q => q.Key,
                                q => q.OrderByDescending(a => a.UpdatedDate ?? a.CreatedDate).ThenByDescending(a => a.ID).First()));

                var optionsByQuestion = _pollQuestionOptionRepo.GetAll(x =>
                        x.PollQuestionID.HasValue &&
                        questionIds.Contains(x.PollQuestionID.Value))
                    .GroupBy(x => x.PollQuestionID!.Value)
                    .ToDictionary(g => g.Key, g => g.OrderBy(x => x.SortOrder).ThenBy(x => x.ID).ToList());

                var employeeIds = responses
                    .Where(x => x.EmployeeID.HasValue)
                    .Select(x => x.EmployeeID!.Value)
                    .Distinct()
                    .ToHashSet();

                var employees = employeeIds.Count == 0
                    ? new Dictionary<int, Employee>()
                    : _employeeRepo.GetAll(x => employeeIds.Contains(x.ID)).ToDictionary(x => x.ID);

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Responses");
                const int sectionHeaderRow = 1;
                const int questionHeaderRow = 2;
                const int firstDataRow = 3;

                var fixedHeaders = new[]
                {
                    "STT"
                    //"Response ID",
                    //"Employee ID",
                    //"Ma nhan vien",
                    //"Ho ten",
                    //"Trang thai",
                    //"Ngay hoan thanh",
                    //"Ngay tao"
                };

                for (var i = 0; i < fixedHeaders.Length; i++)
                {
                    var columnIndex = i + 1;
                    worksheet.Cell(sectionHeaderRow, columnIndex).Value = fixedHeaders[i];
                    worksheet.Range(sectionHeaderRow, columnIndex, questionHeaderRow, columnIndex).Merge();
                }

                for (var i = 0; i < questions.Count; i++)
                {
                    worksheet.Cell(questionHeaderRow, fixedHeaders.Length + i + 1).Value =
                        string.IsNullOrWhiteSpace(questions[i].QuestionText)
                            ? $"Question {questions[i].ID}"
                            : questions[i].QuestionText;
                }

                WriteSectionHeaders(worksheet, questions, sectionsById, fixedHeaders.Length, sectionHeaderRow);

                var rowIndex = firstDataRow;
                foreach (var response in responses)
                {
                    employees.TryGetValue(response.EmployeeID ?? 0, out var employee);
                    answersByResponse.TryGetValue(response.ID, out var responseAnswers);

                    worksheet.Cell(rowIndex, 1).Value = rowIndex - firstDataRow + 1;
                    worksheet.Cell(rowIndex, 2).Value = response.ID;
                    worksheet.Cell(rowIndex, 3).Value = response.EmployeeID?.ToString() ?? "";
                    worksheet.Cell(rowIndex, 4).Value = employee?.Code ?? "";
                    worksheet.Cell(rowIndex, 5).Value = employee?.FullName ?? "";
                    worksheet.Cell(rowIndex, 6).Value = response.IsCompleted == true ? "Hoan thanh" : "Chua hoan thanh";
                    worksheet.Cell(rowIndex, 7).Value = FormatDateTimeForExport(response.CompletedDate);
                    worksheet.Cell(rowIndex, 8).Value = FormatDateTimeForExport(response.CreatedDate);

                    for (var questionIndex = 0; questionIndex < questions.Count; questionIndex++)
                    {
                        var question = questions[questionIndex];
                        PollResponseAnswer? answer = null;
                        responseAnswers?.TryGetValue(question.ID, out answer);
                        worksheet.Cell(rowIndex, fixedHeaders.Length + questionIndex + 1).Value =
                            FormatAnswerForExport(question, answer, optionsByQuestion);
                    }

                    rowIndex++;
                }

                var usedRange = worksheet.RangeUsed();
                if (usedRange != null)
                {
                    usedRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    usedRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                }

                var totalColumns = fixedHeaders.Length + questions.Count;
                var headerRange = worksheet.Range(sectionHeaderRow, 1, questionHeaderRow, totalColumns);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                if (questions.Count > 0)
                {
                    worksheet.Range(questionHeaderRow, fixedHeaders.Length + 1, rowIndex - 1, totalColumns)
                        .SetAutoFilter();
                }

                worksheet.SheetView.FreezeRows(questionHeaderRow);
                worksheet.Columns().AdjustToContents();
                worksheet.Columns().Style.Alignment.WrapText = true;

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"PollResponses_{pollForm.ID}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(
                    stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Get response detail by ID
        /// </summary>
        [HttpGet("response/{id}")]
        public IActionResult GetResponseDetail(int id)
        {
            try
            {
                var currentUser = GetCurrentUser();
                var response = _pollResponseRepo.GetByID(id);
                if (response == null || response.ID <= 0)
                    return NotFound(ApiResponseFactory.Fail(null, "Response not found"));

                var pollForm = _pollFormRepo.GetByID(response.PollFormID ?? 0);
                if (pollForm == null || pollForm.ID <= 0 || pollForm.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Poll form not found"));

                if (!CanReadResponse(response, pollForm, currentUser))
                    return ForbiddenResponse("You do not have permission to view this poll response");

                var responseDetail = MapResponseDetail(response);

                return Ok(ApiResponseFactory.Success(responseDetail, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Get poll statistics
        /// </summary>
        [HttpGet("{pollFormId}/statistics")]
        public IActionResult GetPollStatistics(int pollFormId)
        {
            try
            {
                var currentUser = GetCurrentUser();
                var pollForm = _pollFormRepo.GetByID(pollFormId);
                if (pollForm == null || pollForm.ID <= 0 || pollForm.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Poll form not found"));

                if (!CanManagePoll(pollForm, currentUser))
                    return ForbiddenResponse("You do not have permission to view poll statistics");

                var totalResponses = _pollResponseRepo.GetAll(x => x.PollFormID == pollFormId).Count;

                var questions = _pollQuestionRepo.GetAll(x => x.PollFormID == pollFormId)
                    .OrderBy(x => x.SortOrder)
                    .ToList();

                var statistics = new
                {
                    PollFormId = pollFormId,
                    PollFormTitle = pollForm.Title,
                    TotalResponses = totalResponses,
                    Questions = questions.Select(q => new
                    {
                        QuestionId = q.ID,
                        QuestionText = q.QuestionText,
                        QuestionType = q.QuestionType,
                        TotalAnswers = _pollResponseAnswerRepo.GetAll(x => x.PollQuestionID == q.ID).Count,
                        Options = _pollQuestionOptionRepo.GetAll(x => x.PollQuestionID == q.ID)
                            .OrderBy(x => x.SortOrder)
                            .Select(o => new
                            {
                                OptionId = o.ID,
                                OptionText = o.OptionText,
                                OptionValue = o.OptionValue,
                                Count = _pollResponseAnswerRepo.GetAll(x => 
                                    x.PollQuestionID == q.ID && 
                                    x.AnswerText == o.OptionValue)
                                    .Count()
                            })
                            .ToList()
                    })
                    .ToList()
                };

                return Ok(ApiResponseFactory.Success(statistics, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        private PollQuestionDetailDTO MapQuestionDetail(PollQuestion q, Employee? employee = null)
        {
            var isEmployeeMapped = IsEmployeeDataSource(q);
            EmployeeFieldOptionMap.TryGetValue(q.DataSourceField ?? "", out var fieldOption);
            var dataSourceValue = isEmployeeMapped
                ? ResolveEmployeeFieldValue(employee, q.DataSourceField)
                : EmployeeFieldResolvedValue.Empty;

            return new PollQuestionDetailDTO
            {
                ID = q.ID,
                PollFormID = q.PollFormID,
                SectionID = q.PollSectionID,
                QuestionText = q.QuestionText,
                FieldKey = q.FieldKey,
                QuestionType = q.QuestionType,
                IsRequired = q.IsRequired,
                SortOrder = q.SortOrder,
                ConfigJson = q.ConfigJson,
                DataSourceType = q.DataSourceType,
                DataSourceField = q.DataSourceField,
                DataSourceLabel = fieldOption?.Label,
                DataSourceValue = dataSourceValue.RawValue,
                DataSourceDisplayValue = dataSourceValue.DisplayValue,
                IsAutoFilled = isEmployeeMapped,
                Options = _pollQuestionOptionRepo.GetAll(x => x.PollQuestionID == q.ID)
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.ID)
                    .Select(o => new PollQuestionOptionDTO
                    {
                        ID = o.ID,
                        PollQuestionID = o.PollQuestionID,
                        OptionText = o.OptionText,
                        OptionValue = o.OptionValue,
                        SortOrder = o.SortOrder
                    })
                    .ToList()
            };
        }

        private Employee? GetCurrentEmployee(int? fallbackEmployeeId = null)
        {
            var employeeId = GetCurrentEmployeeId(fallbackEmployeeId);

            if (employeeId <= 0)
                return null;

            var employee = _employeeRepo.GetByID(employeeId);
            return employee != null && employee.ID > 0 ? employee : null;
        }

        private int GetCurrentEmployeeId(int? fallbackEmployeeId = null)
        {
            var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
            var currentUser = ObjectMapper.GetCurrentUser(claims);
            return currentUser.EmployeeID > 0 ? currentUser.EmployeeID : fallbackEmployeeId.GetValueOrDefault();
        }

        private CurrentUser GetCurrentUser()
        {
            var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
            return ObjectMapper.GetCurrentUser(claims);
        }

        private static RTCContext CreateDbContext(CurrentUser currentUser)
        {
            var dbContext = new RTCContext();
            dbContext.CurrentUser = currentUser;
            return dbContext;
        }

        private ObjectResult ForbiddenResponse(string message)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiResponseFactory.Fail(null, message));
        }

        private IActionResult? EnsureCanManagePoll(PollFormEntity pollForm, CurrentUser currentUser)
        {
            return CanManagePoll(pollForm, currentUser)
                ? null
                : ForbiddenResponse("You do not have permission to manage this poll form");
        }

        private IActionResult? EnsureCanViewPollForm(PollFormEntity pollForm, CurrentUser currentUser)
        {
            return pollForm.IsPublic == true || CanManagePoll(pollForm, currentUser)
                ? null
                : ForbiddenResponse("Poll form is not public yet");
        }

        private IActionResult? EnsureCanVotePollForm(PollFormEntity pollForm)
        {
            return pollForm.IsPublic == true
                ? null
                : ForbiddenResponse("Poll form is not public yet");
        }

        private static bool CanManagePoll(PollFormEntity pollForm, CurrentUser currentUser)
        {
            if (currentUser.IsAdmin || currentUser.Permissions.Contains("N99"))
                return true;

            return !string.IsNullOrWhiteSpace(pollForm.CreatedBy) &&
                !string.IsNullOrWhiteSpace(currentUser.LoginName) &&
                string.Equals(pollForm.CreatedBy, currentUser.LoginName, StringComparison.OrdinalIgnoreCase);
        }

        private static bool CanReadResponse(PollResponse response, PollFormEntity pollForm, CurrentUser currentUser)
        {
            if (CanManagePoll(pollForm, currentUser))
                return true;

            return currentUser.EmployeeID > 0 && response.EmployeeID == currentUser.EmployeeID;
        }

        private static bool CanEditPoll(PollFormEntity pollForm, DateTime now, out string? closedReason)
        {
            closedReason = null;

            if (pollForm.StartDate.HasValue && now < pollForm.StartDate.Value)
            {
                closedReason = "Poll has not started yet";
                return false;
            }

            if (pollForm.EndDate.HasValue && now > pollForm.EndDate.Value)
            {
                closedReason = "Poll has ended";
                return false;
            }

            return true;
        }

        private PollResponse? GetLatestEmployeeResponse(int pollFormId, int employeeId)
        {
            return _pollResponseRepo.GetAll(x => x.PollFormID == pollFormId && x.EmployeeID == employeeId)
                .OrderByDescending(x => x.IsCompleted == true)
                .ThenByDescending(x => x.CompletedDate ?? x.UpdatedDate ?? x.CreatedDate)
                .ThenByDescending(x => x.ID)
                .FirstOrDefault(x => x.ID > 0);
        }

        private static PollResponse? GetLatestEmployeeResponse(RTCContext dbContext, int pollFormId, int employeeId)
        {
            return dbContext.PollResponses
                .Where(x => x.PollFormID == pollFormId && x.EmployeeID == employeeId)
                .OrderByDescending(x => x.IsCompleted == true)
                .ThenByDescending(x => x.CompletedDate ?? x.UpdatedDate ?? x.CreatedDate)
                .ThenByDescending(x => x.ID)
                .FirstOrDefault(x => x.ID > 0);
        }

        private PollResponseDetailDTO MapResponseDetail(PollResponse response)
        {
            return new PollResponseDetailDTO
            {
                ID = response.ID,
                PollFormID = response.PollFormID,
                EmployeeID = response.EmployeeID,
                IsCompleted = response.IsCompleted,
                CompletedDate = response.CompletedDate,
                CreatedBy = response.CreatedBy,
                CreatedDate = response.CreatedDate,
                UpdatedBy = response.UpdatedBy,
                UpdatedDate = response.UpdatedDate,
                Answers = _pollResponseAnswerRepo.GetAll(x => x.PollResponseID == response.ID)
                    .Select(a => new PollResponseAnswerDTO
                    {
                        ID = a.ID,
                        PollResponseID = a.PollResponseID,
                        PollQuestionID = a.PollQuestionID,
                        AnswerText = a.AnswerText,
                        AnswerJson = a.AnswerJson,
                        DisplayText = a.DisplayText
                    })
                    .ToList()
            };
        }

        private int SaveResponseAnswers(RTCContext dbContext, int responseId, List<AnswerItemDTO> answers, CurrentUser currentUser)
        {
            var savedAnswerCount = 0;
            var submittedAnswers = answers
                .Where(x => x.QuestionID.HasValue)
                .GroupBy(x => x.QuestionID!.Value)
                .Select(x => x.Last())
                .ToList();
            var questionIds = submittedAnswers.Select(x => x.QuestionID!.Value).ToHashSet();
            var existingAnswers = dbContext.PollResponseAnswers
                .Where(x => x.PollResponseID == responseId && x.PollQuestionID.HasValue && questionIds.Contains(x.PollQuestionID.Value))
                .ToDictionary(x => x.PollQuestionID!.Value);

            foreach (var answer in submittedAnswers)
            {
                if (existingAnswers.TryGetValue(answer.QuestionID!.Value, out var existingAnswer) && existingAnswer.ID > 0)
                {
                    existingAnswer.AnswerText = answer.AnswerText;
                    existingAnswer.AnswerJson = answer.AnswerJson;
                    existingAnswer.DisplayText = answer.DisplayText;
                    existingAnswer.UpdatedBy = currentUser.LoginName;
                    existingAnswer.UpdatedDate = DateTime.Now;
                    savedAnswerCount++;
                }
                else
                {
                    var pollAnswer = new PollResponseAnswer
                    {
                        PollResponseID = responseId,
                        PollQuestionID = answer.QuestionID,
                        AnswerText = answer.AnswerText,
                        AnswerJson = answer.AnswerJson,
                        DisplayText = answer.DisplayText,
                        CreatedBy = currentUser.LoginName,
                        CreatedDate = DateTime.Now
                    };

                    dbContext.PollResponseAnswers.Add(pollAnswer);
                    savedAnswerCount++;
                }
            }

            return savedAnswerCount;
        }

        private static bool TryValidateAnswers(
            List<AnswerItemDTO> answers,
            List<PollQuestion> allowedQuestions,
            RTCContext dbContext,
            out string validationMessage,
            out object? validationData)
        {
            validationMessage = "";
            validationData = null;

            if (answers.Any(x => !x.QuestionID.HasValue))
            {
                validationMessage = "QuestionID is required for all answers";
                return false;
            }

            var allowedQuestionIds = allowedQuestions.Select(x => x.ID).ToHashSet();
            var invalidQuestionIds = answers
                .Where(x => x.QuestionID.HasValue && !allowedQuestionIds.Contains(x.QuestionID.Value))
                .Select(x => x.QuestionID)
                .Distinct()
                .ToList();

            if (invalidQuestionIds.Count > 0)
            {
                validationMessage = "Answers contain questions outside this poll/section";
                validationData = invalidQuestionIds;
                return false;
            }

            var duplicatedQuestionIds = answers
                .Where(x => x.QuestionID.HasValue)
                .GroupBy(x => x.QuestionID!.Value)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

            if (duplicatedQuestionIds.Count > 0)
            {
                validationMessage = "Answers contain duplicated questions";
                validationData = duplicatedQuestionIds;
                return false;
            }

            var answerByQuestion = answers
                .Where(x => x.QuestionID.HasValue)
                .ToDictionary(x => x.QuestionID!.Value);
            var missingRequiredQuestionIds = allowedQuestions
                .Where(x => x.IsRequired)
                .Where(x => !answerByQuestion.TryGetValue(x.ID, out var answer) || IsEmptyAnswer(answer))
                .Select(x => x.ID)
                .ToList();

            if (missingRequiredQuestionIds.Count > 0)
            {
                validationMessage = "Required questions are missing answers";
                validationData = missingRequiredQuestionIds;
                return false;
            }

            var choiceQuestions = allowedQuestions
                .Where(IsChoiceQuestion)
                .ToList();
            if (choiceQuestions.Count == 0)
                return true;

            var choiceQuestionIds = choiceQuestions.Select(x => x.ID).ToHashSet();
            var optionsByQuestion = dbContext.PollQuestionOptions
                .Where(x => x.PollQuestionID.HasValue && choiceQuestionIds.Contains(x.PollQuestionID.Value))
                .GroupBy(x => x.PollQuestionID!.Value)
                .ToDictionary(x => x.Key, x => x.ToList());

            foreach (var question in choiceQuestions)
            {
                if (!answerByQuestion.TryGetValue(question.ID, out var answer) || IsEmptyAnswer(answer))
                    continue;

                optionsByQuestion.TryGetValue(question.ID, out var options);
                options ??= new List<PollQuestionOption>();
                var values = ExtractAnswerValues(answer);

                if (string.Equals(question.QuestionType, "SingleChoice", StringComparison.OrdinalIgnoreCase) && values.Count > 1)
                {
                    validationMessage = "SingleChoice question accepts only one answer";
                    validationData = question.ID;
                    return false;
                }

                var invalidValues = values
                    .Where(x => !IsValidChoiceValue(x, options))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (invalidValues.Count > 0)
                {
                    validationMessage = "Answer contains invalid option values";
                    validationData = new { questionId = question.ID, invalidValues };
                    return false;
                }
            }

            return true;
        }

        private static bool IsEmptyAnswer(AnswerItemDTO answer)
        {
            return string.IsNullOrWhiteSpace(answer.AnswerText) &&
                string.IsNullOrWhiteSpace(answer.AnswerJson);
        }

        private static bool IsChoiceQuestion(PollQuestion question)
        {
            return string.Equals(question.QuestionType, "SingleChoice", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(question.QuestionType, "MultipleChoice", StringComparison.OrdinalIgnoreCase);
        }

        private static List<string> ExtractAnswerValues(AnswerItemDTO answer)
        {
            if (!string.IsNullOrWhiteSpace(answer.AnswerJson))
            {
                try
                {
                    using var document = JsonDocument.Parse(answer.AnswerJson);
                    var root = document.RootElement;
                    if (root.ValueKind == JsonValueKind.Array)
                    {
                        return root.EnumerateArray()
                            .Select(ExtractJsonAnswerValue)
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .Cast<string>()
                            .ToList();
                    }

                    var value = ExtractJsonAnswerValue(root);
                    return string.IsNullOrWhiteSpace(value) ? new List<string>() : new List<string> { value };
                }
                catch
                {
                    return new List<string> { answer.AnswerJson };
                }
            }

            if (string.IsNullOrWhiteSpace(answer.AnswerText))
                return new List<string>();

            return answer.AnswerText
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }

        private static string? ExtractJsonAnswerValue(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number => element.ToString(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                JsonValueKind.Object when element.TryGetProperty("optionValue", out var optionValue) => optionValue.ToString(),
                JsonValueKind.Object when element.TryGetProperty("value", out var value) => value.ToString(),
                JsonValueKind.Object when element.TryGetProperty("id", out var id) => id.ToString(),
                JsonValueKind.Object when element.TryGetProperty("optionText", out var optionText) => optionText.ToString(),
                JsonValueKind.Object when element.TryGetProperty("label", out var label) => label.ToString(),
                JsonValueKind.Object when element.TryGetProperty("text", out var text) => text.ToString(),
                _ => element.ToString()
            };
        }

        private static bool IsValidChoiceValue(string value, List<PollQuestionOption> options)
        {
            var trimmedValue = value.Trim();
            if (string.IsNullOrWhiteSpace(trimmedValue))
                return false;

            return options.Any(x =>
                string.Equals(x.ID.ToString(CultureInfo.InvariantCulture), trimmedValue, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x.OptionValue, trimmedValue, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x.OptionText, trimmedValue, StringComparison.OrdinalIgnoreCase));
        }

        private static bool TryNormalizeQuestionDataSource(
            string? requestedDataSourceType,
            string? requestedDataSourceField,
            out string? dataSourceType,
            out string? dataSourceField,
            out string validationMessage)
        {
            dataSourceType = null;
            dataSourceField = null;
            validationMessage = "";

            if (string.IsNullOrWhiteSpace(requestedDataSourceType) && string.IsNullOrWhiteSpace(requestedDataSourceField))
                return true;

            if (!string.Equals(requestedDataSourceType?.Trim(), EmployeeDataSourceType, StringComparison.OrdinalIgnoreCase))
            {
                validationMessage = "Only Employee data source is supported";
                return false;
            }

            if (string.IsNullOrWhiteSpace(requestedDataSourceField) ||
                !EmployeeFieldOptionMap.TryGetValue(requestedDataSourceField.Trim(), out var fieldOption))
            {
                validationMessage = "Employee data source field is not allowed";
                return false;
            }

            dataSourceType = EmployeeDataSourceType;
            dataSourceField = fieldOption.FieldKey;
            return true;
        }

        private static bool IsEmployeeDataSource(PollQuestion question)
        {
            return string.Equals(question.DataSourceType, EmployeeDataSourceType, StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(question.DataSourceField) &&
                EmployeeFieldOptionMap.ContainsKey(question.DataSourceField);
        }

        private List<AnswerItemDTO> AddEmployeeMappedAnswers(
            List<AnswerItemDTO>? submittedAnswers,
            List<PollQuestion> questions,
            Employee? employee,
            RTCContext dbContext)
        {
            var answers = submittedAnswers?.ToList() ?? new List<AnswerItemDTO>();
            foreach (var answer in answers)
            {
                answer.DisplayText = null;
            }

            if (employee == null)
                return answers;

            var mappedValues = questions
                .Where(IsEmployeeDataSource)
                .Select(question => new
                {
                    Question = question,
                    Value = ResolveEmployeeFieldValue(employee, question.DataSourceField, dbContext)
                })
                .ToDictionary(x => x.Question.ID, x => x);

            foreach (var answer in answers.Where(x => x.QuestionID.HasValue))
            {
                if (!mappedValues.TryGetValue(answer.QuestionID!.Value, out var mappedValue))
                {
                    answer.DisplayText = null;
                    continue;
                }

                answer.AnswerText = mappedValue.Value.RawValue;
                answer.AnswerJson = null;
                answer.DisplayText = mappedValue.Value.DisplayValue;
            }

            var answeredQuestionIds = answers
                .Where(x => x.QuestionID.HasValue)
                .Select(x => x.QuestionID!.Value)
                .ToHashSet();

            foreach (var mappedValue in mappedValues.Values)
            {
                if (answeredQuestionIds.Contains(mappedValue.Question.ID))
                    continue;

                if (string.IsNullOrWhiteSpace(mappedValue.Value.RawValue))
                    continue;

                answers.Add(new AnswerItemDTO
                {
                    QuestionID = mappedValue.Question.ID,
                    AnswerText = mappedValue.Value.RawValue,
                    DisplayText = mappedValue.Value.DisplayValue
                });
            }

            return answers;
        }

        private EmployeeFieldResolvedValue ResolveEmployeeFieldValue(Employee? employee, string? fieldKey, RTCContext? dbContext = null)
        {
            if (employee == null || string.IsNullOrWhiteSpace(fieldKey) || !EmployeeFieldOptionMap.ContainsKey(fieldKey))
                return EmployeeFieldResolvedValue.Empty;

            var property = typeof(Employee).GetProperty(fieldKey, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property == null)
                return EmployeeFieldResolvedValue.Empty;

            var rawValue = FormatEmployeeFieldValue(property.GetValue(employee));
            var displayValue = ResolveEmployeeLookupDisplayValue(fieldKey, rawValue, dbContext) ?? rawValue;

            return new EmployeeFieldResolvedValue(rawValue, displayValue);
        }

        private string? ResolveEmployeeLookupDisplayValue(string fieldKey, string? rawValue, RTCContext? dbContext = null)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return null;

            if (!EmployeeFieldOptionMap.TryGetValue(fieldKey, out var fieldOption) ||
                string.Equals(fieldOption.DisplayType, "raw", StringComparison.OrdinalIgnoreCase))
            {
                return rawValue;
            }

            if (string.Equals(fieldOption.DisplayType, "enum", StringComparison.OrdinalIgnoreCase) &&
                (fieldKey.Equals(nameof(Employee.GioiTinh), StringComparison.OrdinalIgnoreCase) ||
                 fieldKey.Equals(nameof(Employee.Sex), StringComparison.OrdinalIgnoreCase)))
            {
                return FormatGender(rawValue);
            }

            var ownsContext = dbContext == null;
            dbContext ??= CreateDbContext(GetCurrentUser());

            try
            {
                if (!int.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
                {
                    return fieldKey.Equals(nameof(Employee.GioiTinh), StringComparison.OrdinalIgnoreCase) ||
                        fieldKey.Equals(nameof(Employee.Sex), StringComparison.OrdinalIgnoreCase)
                            ? FormatGender(rawValue)
                            : rawValue;
                }

                if (fieldKey.Equals(nameof(Employee.DepartmentID), StringComparison.OrdinalIgnoreCase))
                    return dbContext.Departments.Find(id)?.Name;
                if (fieldKey.Equals(nameof(Employee.ChucVuHDID), StringComparison.OrdinalIgnoreCase))
                    return dbContext.EmployeeChucVuHDs.Find(id)?.Name;
                if (fieldKey.Equals(nameof(Employee.ChuVuID), StringComparison.OrdinalIgnoreCase))
                    return dbContext.EmployeeChucVus.Find(id)?.Name;
                if (fieldKey.Equals(nameof(Employee.TeamID), StringComparison.OrdinalIgnoreCase))
                    return dbContext.Teams.Find(id)?.Name;
                if (fieldKey.Equals(nameof(Employee.EmployeeTeamID), StringComparison.OrdinalIgnoreCase))
                    return dbContext.EmployeeTeams.Find(id)?.Name;
                if (fieldKey.Equals(nameof(Employee.ProjectTypeID), StringComparison.OrdinalIgnoreCase))
                    return dbContext.ProjectTypes.Find(id)?.ProjectTypeName;
                if (fieldKey.Equals(nameof(Employee.TaxCompanyID), StringComparison.OrdinalIgnoreCase))
                {
                    var taxCompany = dbContext.TaxCompanies.Find(id);
                    return taxCompany?.Name ?? taxCompany?.FullName;
                }
                if (fieldKey.Equals(nameof(Employee.Leader), StringComparison.OrdinalIgnoreCase))
                {
                    var leader = dbContext.Employees.Find(id);
                    return string.IsNullOrWhiteSpace(leader?.FullName)
                        ? leader?.Code
                        : leader.FullName;
                }
                if (fieldKey.Equals(nameof(Employee.Status), StringComparison.OrdinalIgnoreCase))
                    return dbContext.EmployeeStatuses.Find(id)?.StatusName;
                if (fieldKey.Equals(nameof(Employee.TinhTrangHonNhanID), StringComparison.OrdinalIgnoreCase))
                    return dbContext.EmployeeTinhTrangHonNhans.Find(id)?.Name;
                if (fieldKey.Equals(nameof(Employee.GioiTinh), StringComparison.OrdinalIgnoreCase) ||
                    fieldKey.Equals(nameof(Employee.Sex), StringComparison.OrdinalIgnoreCase))
                    return FormatGender(id);

                return rawValue;
            }
            finally
            {
                if (ownsContext)
                    dbContext.Dispose();
            }
        }

        private static string FormatGender(int value)
        {
            return value switch
            {
                0 => "Nữ",
                1 => "Nam",
                3 => "Khác",
                _ => value.ToString(CultureInfo.InvariantCulture)
            };
        }

        private static string FormatGender(string value)
        {
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var gender)
                ? FormatGender(gender)
                : value;
        }

        private sealed record EmployeeFieldResolvedValue(string? RawValue, string? DisplayValue)
        {
            public static readonly EmployeeFieldResolvedValue Empty = new(null, null);
        }

        private static string? FormatEmployeeFieldValue(object? value)
        {
            if (value == null)
                return null;

            return value switch
            {
                DateTime dateValue => dateValue.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                DateTimeOffset dateTimeOffsetValue => dateTimeOffsetValue.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                bool boolValue => boolValue ? "true" : "false",
                IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
                _ => value.ToString()
            };
        }

        private static string FormatDateTimeForExport(DateTime? value)
        {
            return value.HasValue
                ? value.Value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                : "";
        }

        private static void WriteSectionHeaders(
            IXLWorksheet worksheet,
            List<PollQuestion> questions,
            Dictionary<int, PollSection> sectionsById,
            int fixedHeaderCount,
            int sectionHeaderRow)
        {
            var questionIndex = 0;
            while (questionIndex < questions.Count)
            {
                var startIndex = questionIndex;
                var sectionId = questions[questionIndex].PollSectionID;

                while (questionIndex < questions.Count &&
                       questions[questionIndex].PollSectionID == sectionId)
                {
                    questionIndex++;
                }

                var startColumn = fixedHeaderCount + startIndex + 1;
                var endColumn = fixedHeaderCount + questionIndex;
                var sectionHeader = GetSectionHeaderText(sectionId, sectionsById);
                var sectionRange = worksheet.Range(sectionHeaderRow, startColumn, sectionHeaderRow, endColumn);

                if (startColumn < endColumn)
                    sectionRange.Merge();

                sectionRange.FirstCell().Value = sectionHeader;
                sectionRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sectionRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }
        }

        private static string GetSectionHeaderText(int? sectionId, Dictionary<int, PollSection> sectionsById)
        {
            if (sectionId.HasValue && sectionsById.TryGetValue(sectionId.Value, out var section))
            {
                return string.IsNullOrWhiteSpace(section.Title)
                    ? $"Section {section.ID}"
                    : section.Title;
            }

            return "Chua phan section";
        }

        private static string FormatAnswerForExport(
            PollQuestion question,
            PollResponseAnswer? answer,
            Dictionary<int, List<PollQuestionOption>> optionsByQuestion)
        {
            if (answer == null)
                return "";

            optionsByQuestion.TryGetValue(question.ID, out var options);
            options ??= new List<PollQuestionOption>();

            var questionType = question.QuestionType ?? "";
            var isChoiceQuestion =
                string.Equals(questionType, "SingleChoice", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(questionType, "MultipleChoice", StringComparison.OrdinalIgnoreCase);

            if (isChoiceQuestion && !string.IsNullOrWhiteSpace(answer.AnswerJson))
            {
                var formattedJsonAnswer = FormatChoiceJsonForExport(answer.AnswerJson, options);
                if (!string.IsNullOrWhiteSpace(formattedJsonAnswer))
                    return formattedJsonAnswer;
            }

            if (isChoiceQuestion && !string.IsNullOrWhiteSpace(answer.AnswerText))
            {
                var values = answer.AnswerText
                    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(x => FormatChoiceValueForExport(x, options))
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList();

                return values.Count > 1
                    ? string.Join("; ", values)
                    : FormatChoiceValueForExport(answer.AnswerText, options);
            }

            if (!string.IsNullOrWhiteSpace(answer.DisplayText))
                return answer.DisplayText;

            return !string.IsNullOrWhiteSpace(answer.AnswerText)
                ? answer.AnswerText
                : answer.AnswerJson ?? "";
        }

        private static string FormatChoiceJsonForExport(string? answerJson, List<PollQuestionOption> options)
        {
            if (string.IsNullOrWhiteSpace(answerJson))
                return "";

            try
            {
                using var document = JsonDocument.Parse(answerJson);
                var root = document.RootElement;

                if (root.ValueKind == JsonValueKind.Array)
                {
                    var values = root.EnumerateArray()
                        .Select(x => FormatChoiceJsonElementForExport(x, options))
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToList();

                    return string.Join("; ", values);
                }

                return FormatChoiceJsonElementForExport(root, options);
            }
            catch
            {
                return answerJson;
            }
        }

        private static string FormatChoiceJsonElementForExport(JsonElement element, List<PollQuestionOption> options)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    return FormatChoiceValueForExport(element.GetString(), options);
                case JsonValueKind.Number:
                    return FormatChoiceValueForExport(element.ToString(), options);
                case JsonValueKind.Object:
                    if (element.TryGetProperty("optionText", out var optionText))
                        return optionText.ToString();
                    if (element.TryGetProperty("label", out var label))
                        return label.ToString();
                    if (element.TryGetProperty("text", out var text))
                        return text.ToString();
                    if (element.TryGetProperty("optionValue", out var optionValue))
                        return FormatChoiceValueForExport(optionValue.ToString(), options);
                    if (element.TryGetProperty("value", out var value))
                        return FormatChoiceValueForExport(value.ToString(), options);
                    if (element.TryGetProperty("id", out var id))
                        return FormatChoiceValueForExport(id.ToString(), options);
                    return element.ToString();
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return element.GetBoolean() ? "true" : "false";
                default:
                    return element.ToString();
            }
        }

        private static string FormatChoiceValueForExport(string? value, List<PollQuestionOption> options)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "";

            var trimmedValue = value.Trim();

            if (int.TryParse(trimmedValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var optionId))
            {
                var optionById = options.FirstOrDefault(x => x.ID == optionId);
                if (optionById != null)
                    return optionById.OptionText ?? optionById.OptionValue ?? trimmedValue;
            }

            var option = options.FirstOrDefault(x =>
                string.Equals(x.OptionValue, trimmedValue, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x.OptionText, trimmedValue, StringComparison.OrdinalIgnoreCase));

            return option?.OptionText ?? trimmedValue;
        }

        private static Dictionary<string, string?> BuildAnswerMap(
            List<PollQuestion> questions,
            List<PollResponseAnswer> answers)
        {
            var questionMap = questions.ToDictionary(x => x.ID);
            var answerMap = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

            foreach (var answer in answers)
            {
                if (!answer.PollQuestionID.HasValue)
                    continue;

                var value = !string.IsNullOrWhiteSpace(answer.AnswerJson)
                    ? answer.AnswerJson
                    : answer.AnswerText;

                answerMap[answer.PollQuestionID.Value.ToString()] = value;

                if (questionMap.TryGetValue(answer.PollQuestionID.Value, out var question) &&
                    !string.IsNullOrWhiteSpace(question.FieldKey))
                {
                    answerMap[question.FieldKey] = value;
                }
            }

            return answerMap;
        }
    }
}
