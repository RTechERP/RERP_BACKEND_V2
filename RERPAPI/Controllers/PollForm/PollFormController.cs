using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.Poll;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Globalization;
using System.Reflection;
using System.Security.Claims;
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
            new() { FieldKey = nameof(Employee.ID), Label = "Employee ID", DataType = "number" },
            new() { FieldKey = nameof(Employee.Code), Label = "Ma nhan vien", DataType = "string" },
            new() { FieldKey = nameof(Employee.FullName), Label = "Ho ten", DataType = "string" },
            new() { FieldKey = nameof(Employee.DepartmentID), Label = "Phong ban", DataType = "number" },
            new() { FieldKey = nameof(Employee.Position), Label = "Chuc danh", DataType = "string" },
            new() { FieldKey = nameof(Employee.ChucVuHDID), Label = "Chuc vu HDLD", DataType = "number" },
            new() { FieldKey = nameof(Employee.ChuVuID), Label = "Chuc vu noi bo", DataType = "number" },
            new() { FieldKey = nameof(Employee.TeamID), Label = "Team", DataType = "number" },
            new() { FieldKey = nameof(Employee.EmployeeTeamID), Label = "Nhom nhan vien", DataType = "number" },
            new() { FieldKey = nameof(Employee.ProjectTypeID), Label = "Loai du an", DataType = "number" },
            new() { FieldKey = nameof(Employee.TaxCompanyID), Label = "Cong ty tinh thue", DataType = "number" },
            new() { FieldKey = nameof(Employee.Leader), Label = "Quan ly truc tiep", DataType = "number" },
            new() { FieldKey = nameof(Employee.StartWorking), Label = "Ngay vao lam", DataType = "date", SuggestedQuestionType = "Date" },
            new() { FieldKey = nameof(Employee.EndWorking), Label = "Ngay nghi viec", DataType = "date", SuggestedQuestionType = "Date" },
            new() { FieldKey = nameof(Employee.Status), Label = "Trang thai nhan vien", DataType = "number" },
            new() { FieldKey = nameof(Employee.GioiTinh), Label = "Gioi tinh", DataType = "number" },
            new() { FieldKey = nameof(Employee.Sex), Label = "Gioi tinh cu", DataType = "number" },
            new() { FieldKey = nameof(Employee.DiaDiemLamViec), Label = "Dia diem lam viec", DataType = "string" },
            new() { FieldKey = nameof(Employee.EmailCom), Label = "Email cong ty", DataType = "string" },
            new() { FieldKey = nameof(Employee.EmailCongTy), Label = "Email cong ty HR", DataType = "string" },
            new() { FieldKey = nameof(Employee.SDTCongTy), Label = "So dien thoai cong ty", DataType = "string" },
            new() { FieldKey = nameof(Employee.NoiSinh), Label = "Noi sinh", DataType = "string", IsSensitive = true },
            new() { FieldKey = nameof(Employee.BirthOfDate), Label = "Ngay sinh", DataType = "date", SuggestedQuestionType = "Date", IsSensitive = true },
            new() { FieldKey = nameof(Employee.SDTCaNhan), Label = "So dien thoai ca nhan", DataType = "string", IsSensitive = true },
            new() { FieldKey = nameof(Employee.EmailCaNhan), Label = "Email ca nhan", DataType = "string", IsSensitive = true },
            new() { FieldKey = nameof(Employee.DanToc), Label = "Dan toc", DataType = "string", IsSensitive = true },
            new() { FieldKey = nameof(Employee.TonGiao), Label = "Ton giao", DataType = "string", IsSensitive = true },
            new() { FieldKey = nameof(Employee.QuocTich), Label = "Quoc tich", DataType = "string", IsSensitive = true },
            new() { FieldKey = nameof(Employee.TinhTrangHonNhanID), Label = "Tinh trang hon nhan", DataType = "number", IsSensitive = true }
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
                var pollForms = _pollFormRepo.GetAll(x => x.IsDeleted != true)
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
                var pollForm = _pollFormRepo.GetByID(id);
                if (pollForm == null || pollForm.ID <= 0 || pollForm.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Poll form not found"));

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
                    IsPublic = dto.IsPublic ?? true,
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
                if (pollForm == null || pollForm.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Poll form not found"));

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
                var pollForm = _pollFormRepo.GetByID(id);
                if (pollForm == null || pollForm.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Poll form not found"));

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
                var section = _pollSectionRepo.GetByID(id);
                if (section == null || section.ID <= 0 || section.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Section not found"));

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
                var question = _pollQuestionRepo.GetByID(id);
                if (question == null)
                    return NotFound(ApiResponseFactory.Fail(null, "Question not found"));

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
                if (question == null)
                    return NotFound(ApiResponseFactory.Fail(null, "Question not found"));

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
                if (option == null)
                    return NotFound(ApiResponseFactory.Fail(null, "Option not found"));

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
                var option = _pollQuestionOptionRepo.GetByID(id);
                if (option == null)
                    return NotFound(ApiResponseFactory.Fail(null, "Option not found"));

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
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                if (dto.SectionID == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "SectionID is required"));

                var pollForm = _pollFormRepo.GetByID(pollFormId);
                if (pollForm == null || pollForm.ID <= 0 || pollForm.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Poll form not found"));

                var now = DateTime.Now;
                if (pollForm.StartDate.HasValue && now < pollForm.StartDate)
                    return BadRequest(ApiResponseFactory.Fail(null, "Poll has not started yet"));

                if (pollForm.EndDate.HasValue && now > pollForm.EndDate)
                    return BadRequest(ApiResponseFactory.Fail(null, "Poll has ended"));

                var section = _pollSectionRepo.GetByID(dto.SectionID.Value);
                if (section == null || section.ID <= 0 || section.PollFormID != pollFormId || section.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Section not found"));

                PollResponse response;
                if (dto.PollResponseID.HasValue)
                {
                    response = _pollResponseRepo.GetByID(dto.PollResponseID.Value);
                    if (response == null || response.ID <= 0 || response.PollFormID != pollFormId)
                        return BadRequest(ApiResponseFactory.Fail(null, "Poll response is invalid for this poll form"));
                }
                else
                {
                    response = new PollResponse
                    {
                        PollFormID = pollFormId,
                        EmployeeID = dto.EmployeeID,
                        CreatedBy = currentUser.LoginName,
                        CreatedDate = DateTime.Now
                    };

                    var createResponseResult = _pollResponseRepo.Create(response);
                    if (createResponseResult <= 0 || response.ID <= 0)
                        return BadRequest(ApiResponseFactory.Fail(null, "Failed to create poll response"));
                }

                var sectionQuestions = _pollQuestionRepo.GetAll(x => x.PollFormID == pollFormId && x.PollSectionID == section.ID)
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.ID)
                    .ToList();
                var sectionQuestionIds = sectionQuestions.Select(x => x.ID).ToHashSet();

                var currentEmployee = GetCurrentEmployee(dto.EmployeeID);
                var answers = AddEmployeeMappedAnswers(dto.Answers, sectionQuestions, currentEmployee);
                var invalidQuestionIds = answers
                    .Where(x => x.QuestionID.HasValue && !sectionQuestionIds.Contains(x.QuestionID.Value))
                    .Select(x => x.QuestionID)
                    .Distinct()
                    .ToList();

                if (invalidQuestionIds.Count > 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Answers contain questions outside this section", invalidQuestionIds));

                var savedAnswerCount = 0;
                foreach (var answer in answers)
                {
                    if (!answer.QuestionID.HasValue)
                        continue;

                    var existingAnswer = _pollResponseAnswerRepo
                        .GetAll(x => x.PollResponseID == response.ID && x.PollQuestionID == answer.QuestionID)
                        .FirstOrDefault();

                    if (existingAnswer != null && existingAnswer.ID > 0)
                    {
                        existingAnswer.AnswerText = answer.AnswerText;
                        existingAnswer.AnswerJson = answer.AnswerJson;
                        existingAnswer.UpdatedBy = currentUser.LoginName;
                        existingAnswer.UpdatedDate = DateTime.Now;

                        if (_pollResponseAnswerRepo.Update(existingAnswer) > 0)
                            savedAnswerCount++;
                    }
                    else
                    {
                        var pollAnswer = new PollResponseAnswer
                        {
                            PollResponseID = response.ID,
                            PollQuestionID = answer.QuestionID,
                            AnswerText = answer.AnswerText,
                            AnswerJson = answer.AnswerJson,
                            CreatedBy = currentUser.LoginName,
                            CreatedDate = DateTime.Now
                        };

                        if (_pollResponseAnswerRepo.Create(pollAnswer) > 0)
                            savedAnswerCount++;
                    }
                }

                var allSections = _pollSectionRepo.GetAll(x => x.PollFormID == pollFormId && x.IsDeleted != true)
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.ID)
                    .ToList();
                var allQuestions = _pollQuestionRepo.GetAll(x => x.PollFormID == pollFormId).ToList();
                var allResponseAnswers = _pollResponseAnswerRepo.GetAll(x => x.PollResponseID == response.ID).ToList();
                var answerMap = BuildAnswerMap(allQuestions, allResponseAnswers);
                var nextSectionId = _pollBranchingRuleEvaluator.ResolveNextSectionId(section, allSections, answerMap);

                var result = new SubmitPollSectionResultDTO
                {
                    PollResponseID = response.ID,
                    PollFormID = pollFormId,
                    SectionID = section.ID,
                    NextSectionID = nextSectionId,
                    IsCompleted = nextSectionId == null,
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
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var pollForm = _pollFormRepo.GetByID(dto.PollFormID ?? 0);
                if (pollForm == null || pollForm.ID <= 0 || pollForm.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Poll form not found"));

                // Check if poll is still open
                var now = DateTime.Now;
                if (pollForm.StartDate.HasValue && now < pollForm.StartDate)
                    return BadRequest(ApiResponseFactory.Fail(null, "Poll has not started yet"));

                if (pollForm.EndDate.HasValue && now > pollForm.EndDate)
                    return BadRequest(ApiResponseFactory.Fail(null, "Poll has ended"));

                var response = new PollResponse
                {
                    PollFormID = dto.PollFormID,
                    EmployeeID = dto.EmployeeID,
                    CreatedBy = currentUser.LoginName,
                    CreatedDate = DateTime.Now
                };

                var responseResult = _pollResponseRepo.Create(response);
                if (responseResult <= 0 || response.ID <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Failed to submit poll response"));

                var responseId = response.ID;
                var pollQuestions = _pollQuestionRepo.GetAll(x => x.PollFormID == dto.PollFormID).ToList();
                var currentEmployee = GetCurrentEmployee(dto.EmployeeID);
                var answers = AddEmployeeMappedAnswers(dto.Answers, pollQuestions, currentEmployee);

                // Add answers
                if (answers.Count > 0)
                {
                    foreach (var answer in answers)
                    {
                        var pollAnswer = new PollResponseAnswer
                        {
                            PollResponseID = responseId,
                            PollQuestionID = answer.QuestionID,
                            AnswerText = answer.AnswerText,
                            AnswerJson = answer.AnswerJson,
                            CreatedBy = currentUser.LoginName,
                            CreatedDate = DateTime.Now
                        };

                        _pollResponseAnswerRepo.Create(pollAnswer);
                    }
                }

                return Ok(ApiResponseFactory.Success(response, "Poll response submitted successfully"));
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
                var responses = _pollResponseRepo.GetAll(x => x.PollFormID == pollFormId)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToList();

                var responseList = responses.Select(r => new PollResponseDetailDTO
                {
                    ID = r.ID,
                    PollFormID = r.PollFormID,
                    EmployeeID = r.EmployeeID,
                    CreatedBy = r.CreatedBy,
                    CreatedDate = r.CreatedDate,
                    UpdatedBy = r.UpdatedBy,
                    UpdatedDate = r.UpdatedDate,
                    Answers = _pollResponseAnswerRepo.GetAll(x => x.PollResponseID == r.ID)
                        .Select(a => new PollResponseAnswerDTO
                        {
                            ID = a.ID,
                            PollResponseID = a.PollResponseID,
                            PollQuestionID = a.PollQuestionID,
                            AnswerText = a.AnswerText,
                            AnswerJson = a.AnswerJson
                        })
                        .ToList()
                }).ToList();

                return Ok(ApiResponseFactory.Success(responseList, ""));
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
                var response = _pollResponseRepo.GetByID(id);
                if (response == null)
                    return NotFound(ApiResponseFactory.Fail(null, "Response not found"));

                var responseDetail = new PollResponseDetailDTO
                {
                    ID = response.ID,
                    PollFormID = response.PollFormID,
                    EmployeeID = response.EmployeeID,
                    CreatedBy = response.CreatedBy,
                    CreatedDate = response.CreatedDate,
                    UpdatedBy = response.UpdatedBy,
                    UpdatedDate = response.UpdatedDate,
                    Answers = _pollResponseAnswerRepo.GetAll(x => x.PollResponseID == id)
                        .Select(a => new PollResponseAnswerDTO
                        {
                            ID = a.ID,
                            PollResponseID = a.PollResponseID,
                            PollQuestionID = a.PollQuestionID,
                            AnswerText = a.AnswerText,
                            AnswerJson = a.AnswerJson
                        })
                        .ToList()
                };

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
                var pollForm = _pollFormRepo.GetByID(pollFormId);
                if (pollForm == null || pollForm.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Poll form not found"));

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
                DataSourceValue = isEmployeeMapped ? GetEmployeeFieldValue(employee, q.DataSourceField) : null,
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
            var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
            var currentUser = ObjectMapper.GetCurrentUser(claims);
            var employeeId = currentUser.EmployeeID > 0 ? currentUser.EmployeeID : fallbackEmployeeId.GetValueOrDefault();

            if (employeeId <= 0)
                return null;

            var employee = _employeeRepo.GetByID(employeeId);
            return employee != null && employee.ID > 0 ? employee : null;
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

        private static List<AnswerItemDTO> AddEmployeeMappedAnswers(
            List<AnswerItemDTO>? submittedAnswers,
            List<PollQuestion> questions,
            Employee? employee)
        {
            var answers = submittedAnswers?.ToList() ?? new List<AnswerItemDTO>();
            if (employee == null)
                return answers;

            var mappedValues = questions
                .Where(IsEmployeeDataSource)
                .Select(question => new
                {
                    Question = question,
                    Value = GetEmployeeFieldValue(employee, question.DataSourceField)
                })
                .ToDictionary(x => x.Question.ID, x => x);

            foreach (var answer in answers.Where(x => x.QuestionID.HasValue))
            {
                if (!mappedValues.TryGetValue(answer.QuestionID!.Value, out var mappedValue))
                    continue;

                answer.AnswerText = mappedValue.Value;
                answer.AnswerJson = null;
            }

            var answeredQuestionIds = answers
                .Where(x => x.QuestionID.HasValue)
                .Select(x => x.QuestionID!.Value)
                .ToHashSet();

            foreach (var mappedValue in mappedValues.Values)
            {
                if (answeredQuestionIds.Contains(mappedValue.Question.ID))
                    continue;

                if (string.IsNullOrWhiteSpace(mappedValue.Value))
                    continue;

                answers.Add(new AnswerItemDTO
                {
                    QuestionID = mappedValue.Question.ID,
                    AnswerText = mappedValue.Value
                });
            }

            return answers;
        }

        private static string? GetEmployeeFieldValue(Employee? employee, string? fieldKey)
        {
            if (employee == null || string.IsNullOrWhiteSpace(fieldKey) || !EmployeeFieldOptionMap.ContainsKey(fieldKey))
                return null;

            var property = typeof(Employee).GetProperty(fieldKey, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property == null)
                return null;

            return FormatEmployeeFieldValue(property.GetValue(employee));
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
