using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.Record.Chart;
using RERPAPI.Attributes;
using RERPAPI.Controllers.HRM.JobPerfomanceEvaluation;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.HRM.JobPerfomanceEvaluation;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM;
using RERPAPI.Repo.GenericEntity.HRM.HRRecruitmentInterviewAssessment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.HRM.JobPerfomanceEvaluation
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobPerfomanceEvaluationController : ControllerBase
    {

        private EmployeeApproveRepo _employeeApproveRepo;
        private JobPerfomanceEvaluationApproveRepo _jobPerfomanceEvaluationApproveRepo;
        private readonly IConfiguration _configuration;
        EmailHelper _emailHelper;
        private JobPerfomanceEvaluationNewRepo _jobPerfomanceEvaluationNewRepo;
        private vUserGroupLinksRepo _vUserGroupLinksRepo;

        public JobPerfomanceEvaluationController(EmployeeApproveRepo employeeApproveRepo, EmailHelper emailHelper,
            IConfiguration configuration,
            JobPerfomanceEvaluationApproveRepo jobPerfomanceEvaluationApproveRepo,
            JobPerfomanceEvaluationNewRepo jobPerfomanceEvaluationNewRepo, vUserGroupLinksRepo vUserGroupLinksRepo)
        {
            _employeeApproveRepo = employeeApproveRepo;
            _jobPerfomanceEvaluationApproveRepo = jobPerfomanceEvaluationApproveRepo;
            _jobPerfomanceEvaluationNewRepo = jobPerfomanceEvaluationNewRepo;
            _emailHelper = emailHelper;
            _configuration = configuration;
            _vUserGroupLinksRepo = vUserGroupLinksRepo;
        }

        [HttpGet("get-contact-transfer-review")]
        public async Task<IActionResult> GetContactTransferReview([FromQuery] GetContactTransferReviewParam? param)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(param!.Keyword))
                {
                    param!.Keyword = "";
                }
                if (param!.Role == "tbp")
                {
                    Dictionary<string, string> claims = base.User.Claims.ToDictionary((Claim x) => x.Type, (Claim x) => x.Value);
                    CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
                    param!.EmployeeID = currentUser.EmployeeID;
                }
                //var data = await SqlDapper<object>.ProcedureToListTAsync("spGetContactTransferReview", new
                var data = await SqlDapper<object>.ProcedureToListTAsync("spGetContactTransferReviewNew", new
                {
                    param.Keyword,
                    param.DepartmentID,
                    param.EmployeeID,
                    param.DateFrom,
                    param.DateTo,
                    param.Step,
                    param.Role
                });
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(null, ex.Message));
            }
        }

        [HttpGet("get-employee-approve")]
        public async Task<IActionResult> GetEmployeeApprove()
        {
            try
            {
                List<EmployeeApprove> data = _employeeApproveRepo.GetAll(c => c.IsDeleted == false && c.Type == 1).ToList();
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(null, ex.Message));
            }
        }

        [HttpGet("get-infomation-employee")]
        public async Task<IActionResult> GetInfomationEmployee(int EmployeeID)
        {
            try
            {
                var data = await SqlDapper<object>.ProcedureToListTAsync("spGetInfomationEmployee", new { EmployeeID });
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(null, ex.Message));
            }
        }

        [HttpGet("get-employee-mail-info")]
        public async Task<IActionResult> GetEmployeeMailInfo(int EmployeeID)
        {
            try
            {
                var data = await SqlDapper<object>.ProcedureToListTAsync("spGetInfomationEmployeeCV", new { EmployeeID });
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(null, ex.Message));
            }
        }


        [HttpGet("get-detail")]
        public async Task<IActionResult> GetDetail(int JobPerfomanceEvaluationID)
        {
            try
            {
                return Ok(ApiResponseFactory.Success(await _jobPerfomanceEvaluationNewRepo.GetByIDAsync(JobPerfomanceEvaluationID), "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(null, ex.Message));
            }
        }
        [HttpGet("get-detail-new")]
        public async Task<IActionResult> GetDetailNew(int id)
        {
            try
            {
                var data = await _jobPerfomanceEvaluationNewRepo.GetByIDAsync(id);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(null, ex.Message));
            }
        }

        //[HttpGet("get-criteria")]
        //public async Task<IActionResult> GetCriteria(int JobPerfomanceEvaluationID)
        //{
        //    try
        //    {
        //        List<JobPerfomanceEvaluationCriterion> data = _jobPerfomanceEvaluationCriteriaRepo
        //            .GetAll(c => c.JobPerfomanceEvaluationID == JobPerfomanceEvaluationID && c.IsDeleted == false)
        //            .OrderBy(c => c.STT)
        //            .ToList();
        //        return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công!"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(ApiResponseFactory.Fail(null, ex.Message));
        //    }
        //}

        [HttpGet("get-approve-step")]
        public IActionResult GetApproveStep([FromQuery] int id, [FromQuery] string role)
        {
            // Map tên role sang số thứ tự của bước (Step)
            var roleStepMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        { "employee", 1 },
        { "manager", 2 },
        { "tbp", 2 },
        { "hr", 3 },
        { "bgd", 4 }
    };

            if (!roleStepMap.TryGetValue(role ?? "", out var expectedStep))
            {
                return Ok(ApiResponseFactory.Fail(null, $"Role '{role}' không hợp lệ!"));
            }

            // Lấy bước duyệt mới nhất (ID lớn nhất) của phiếu này mà chưa bị xóa
            var approveRecord = _jobPerfomanceEvaluationApproveRepo.GetAll(x => x.JobPerfomanceEvaluationID == id && !x.IsDeleted.Value)
                                .OrderByDescending(x => x.ID)
                                .FirstOrDefault();

            // Nếu chưa có lịch sử duyệt nào, trả về bước dự kiến theo Role và status = 0 (Chờ duyệt)
            if (approveRecord == null)
            {
                return Ok(ApiResponseFactory.Success(new
                {
                    Step = expectedStep,
                    StatusApprove = 0
                }));
            }

            // Trả về bước và trạng thái thực tế từ database
            return Ok(ApiResponseFactory.Success(new
            {
                approveRecord.Step,
                approveRecord.StatusApprove
            }));
        }


        [HttpPost("save-new")]
        public async Task<IActionResult> SaveDataNew([FromBody] JobPerfomanceEvaluationDTO model)
        {
            try
            {
                if (model == null)
                    return Ok(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));

                RERPAPI.Model.Entities.JobPerfomanceEvaluationNew entity;
                string stepName = "";
                if (model.ID > 0)
                {
                    // ── CẬP NHẬT ──────────────────────────────────────────────────
                    entity = await _jobPerfomanceEvaluationNewRepo.GetByIDAsync(model.ID);
                    if (entity == null)
                        return Ok(ApiResponseFactory.Fail(null, "Không tìm thấy dữ liệu"));

                    // FK người liên quan
                    entity.EmployeeID = model.EmployeeID;
                    entity.EmployeeEvaluationID = model.EmployeeEvaluationID;
                    entity.EvaluationEmployeeLoaiHDID = model.EvaluationEmployeeLoaiHDID;
                    entity.ConclusionEmployeeLoaiHDID = model.ConclusionEmployeeLoaiHDID;

                    // Người duyệt
                    entity.TBPApproveID = model.EmployeeEvaluationID;
                    //entity.TBPApproveName = model.TBPApproveName;
                    //entity.HCNSApproveID = model.HCNSApproveID;
                    //entity.HCNSApproveName = model.HCNSApproveName;
                    //entity.BGDApproveID = model.BGDApproveID;
                    //entity.BGDApproveName = model.BGDApproveName;

                    // Công việc chính
                    entity.MainJobmainResponsibilities = model.MainJobmainResponsibilities;
                    //-------------------------------NLD-----------------------------------------------------------------
                    // Nhóm A: Năng lực chuyên môn
                    entity.ProfessionalCompetency = model.ProfessionalCompetency;
                    entity.ProfessionalKnowledge = model.ProfessionalKnowledge;
                    entity.ToolAndSystemSkills = model.ToolAndSystemSkills;
                    entity.WorkQuality = model.WorkQuality;
                    entity.WorkProgress = model.WorkProgress;
                    entity.ProblemSolvingAbility = model.ProblemSolvingAbility;

                    // Nhóm B: Hiệu quả công việc & phối hợp
                    entity.Proactiveness = model.Proactiveness;
                    entity.CollaborationAndSupport = model.CollaborationAndSupport;
                    entity.CommunicationAndTeamwork = model.CommunicationAndTeamwork;
                    entity.WorkOutputKPI = model.WorkOutputKPI;

                    // Nhóm C: Kỷ luật, tác phong & thái độ
                    entity.DisciplineAndAttitude = model.DisciplineAndAttitude;
                    entity.ComplianceWithRegulations = model.ComplianceWithRegulations;
                    entity.Attendance = model.Attendance;
                    entity.WorkStyle = model.WorkStyle;
                    entity.AttitudeAndResponsibility = model.AttitudeAndResponsibility;

                    // Nhóm D: Văn hóa, phát triển & gắn bó
                    entity.CulturalFitRTC = model.CulturalFitRTC;
                    entity.LearningAndGrowthMindset = model.LearningAndGrowthMindset;
                    entity.CompanyCommitment = model.CompanyCommitment;

                    // Tổng điểm & xếp loại
                    entity.TotalScore = model.TotalScore;
                    entity.EvaluationGrade = model.EvaluationGrade;
                    //--------------------------------------------------------------------------------------------------

                    //-----------------------------------------TBP----------------------------------------------------------
                    /// // Nhóm A: Năng lực chuyên môn
                    entity.TBPProfessionalCompetency = model.TBPProfessionalCompetency;
                    entity.TBPProfessionalKnowledge = model.TBPProfessionalKnowledge;
                    entity.TBPToolAndSystemSkills = model.TBPToolAndSystemSkills;
                    entity.TBPWorkQuality = model.TBPWorkQuality;
                    entity.TBPWorkProgress = model.TBPWorkProgress;
                    entity.TBPProblemSolvingAbility = model.TBPProblemSolvingAbility;
                    // Nhóm B: Hiệu quả công việc & phối hợp
                    entity.TBPProactiveness = model.TBPProactiveness;
                    entity.TBPCollaborationAndSupport = model.TBPCollaborationAndSupport;
                    entity.TBPCommunicationAndTeamwork = model.TBPCommunicationAndTeamwork;
                    entity.TBPWorkOutputKPI = model.TBPWorkOutputKPI;

                    // Nhóm C: Kỷ luật, tác phong & thái độ
                    entity.TBPDisciplineAndAttitude = model.TBPDisciplineAndAttitude;
                    entity.TBPComplianceWithRegulations = model.TBPComplianceWithRegulations;
                    entity.TBPAttendance = model.TBPAttendance;
                    entity.TBPWorkStyle = model.TBPWorkStyle;
                    entity.TBPAttitudeAndResponsibility = model.TBPAttitudeAndResponsibility;

                    // Nhóm D: Văn hóa, phát triển & gắn bó
                    entity.TBPCulturalFitRTC = model.TBPCulturalFitRTC;
                    entity.TBPLearningAndGrowthMindset = model.TBPLearningAndGrowthMindset;
                    entity.TBPCompanyCommitment = model.TBPCompanyCommitment;

                    // Tổng điểm & xếp loại
                    entity.TBPTotalScore = model.TBPTotalScore;
                    entity.TBPEvaluationGrade = model.TBPEvaluationGrade;
                    //--------------------------------------------------------------------------------------------------

                    // Nhận xét / Kết luận
                    entity.Strengths = model.Strengths;
                    entity.AreasForImprovement = model.AreasForImprovement;
                    entity.RecommendationsOrOther = model.RecommendationsOrOther;
                    entity.OtherConclusion = model.OtherConclusion;

                    // Thời gian & địa điểm
                    entity.DateStart = model.DateStart;
                    entity.DateEnd = model.DateEnd;
                    entity.DateEvaluation = model.DateEvaluation;
                    entity.LocationEvaluation = model.LocationEvaluation;

                    entity.TBPStrengths = model.TBPStrengths;
                    entity.TBPAreasForImprovement = model.TBPAreasForImprovement;
                    entity.TBPConclusionEmployeeLoaiHDID = model.TBPConclusionEmployeeLoaiHDID;
                    entity.TBPRecommendationsOrOther = model.TBPRecommendationsOrOther;


                    // Audit
                    entity.UpdatedBy = model.UpdatedBy;
                    entity.UpdatedDate = DateTime.Now;
                    entity.IsDeleted = false;

                    if (await _jobPerfomanceEvaluationNewRepo.UpdateAsync(entity) <= 0)
                        return Ok(ApiResponseFactory.Fail(null, "Cập nhật thất bại!"));

                    // cập nhật trạng thái tờ phiếu
                    var currentUser = ObjectMapper.GetCurrentUser(User.Claims.ToDictionary(x => x.Type, x => x.Value));
                    if (!string.IsNullOrWhiteSpace(model.Role) && model.ID > 0)
                    {
                        if (model.Role.Equals("employee", StringComparison.OrdinalIgnoreCase))
                        {
                            // NLĐ lưu → "Người lao động: Chờ xác nhận"
                            var nldStep = _jobPerfomanceEvaluationApproveRepo
                                .GetAll(x => x.JobPerfomanceEvaluationID == entity.ID
                                          && x.Step == 1
                                          && x.StatusApprove == 0
                                          && x.IsDeleted != true)
                                .OrderByDescending(x => x.ID).FirstOrDefault();
                            if (nldStep != null)
                            {
                                nldStep.StepName = "Người lao động: Chờ xác nhận";
                                stepName = nldStep.StepName;
                                await _jobPerfomanceEvaluationApproveRepo.UpdateAsync(nldStep);
                            }
                        }
                        else if (model.Role.Equals("tbp", StringComparison.OrdinalIgnoreCase)
                              || model.Role.Equals("manager", StringComparison.OrdinalIgnoreCase)
                              || (model.Role.Equals("hr", StringComparison.OrdinalIgnoreCase) && model.TBPApproveID == currentUser.EmployeeID))
                        {
                            // Option B: chỉ chuyển "TBP: Chờ xác nhận" khi TBP đã nhập đủ dữ liệu bắt buộc
                            bool hasEnoughTbpData =
                                model.ConclusionEmployeeLoaiHDID != null &&
                                model.TBPTotalScore != null &&
                                model.TBPProfessionalKnowledge != null &&
                                model.TBPToolAndSystemSkills != null &&
                                model.TBPWorkQuality != null &&
                                model.TBPWorkProgress != null &&
                                model.TBPProblemSolvingAbility != null &&
                                model.TBPProactiveness != null &&
                                model.TBPCollaborationAndSupport != null &&
                                model.TBPCommunicationAndTeamwork != null &&
                                model.TBPWorkOutputKPI != null &&
                                model.TBPComplianceWithRegulations != null &&
                                model.TBPAttendance != null &&
                                model.TBPWorkStyle != null &&
                                model.TBPAttitudeAndResponsibility != null &&
                                model.TBPCulturalFitRTC != null &&
                                model.TBPLearningAndGrowthMindset != null &&
                                model.TBPCompanyCommitment != null &&
                                !string.IsNullOrWhiteSpace(model.Strengths) &&
                                !string.IsNullOrWhiteSpace(model.AreasForImprovement);

                            var tbpStep = _jobPerfomanceEvaluationApproveRepo
                                .GetAll(x => x.JobPerfomanceEvaluationID == entity.ID
                                          && x.Step == 2
                                          && x.StatusApprove == 0
                                          && x.IsDeleted != true)
                                .OrderByDescending(x => x.ID).FirstOrDefault();
                            if (tbpStep != null)
                            {
                                tbpStep.StepName = hasEnoughTbpData
                                    ? "TBP: Chờ xác nhận"
                                    : "TBP: Chờ đánh giá";
                                stepName = tbpStep.StepName;
                                await _jobPerfomanceEvaluationApproveRepo.UpdateAsync(tbpStep);
                            }
                        }
                        else if (model.Role.Equals("hr", StringComparison.OrdinalIgnoreCase))
                        {
                            // HR lưu khi phiếu còn ở bước "chờ gửi mail" -> trả StepName cho frontend quyết định mở lại modal gửi mail
                            var hrWaitingStep = _jobPerfomanceEvaluationApproveRepo
                                .GetAll(x => x.JobPerfomanceEvaluationID == entity.ID
                                          && x.Step == 1
                                          && x.StatusApprove == 0
                                          && x.IsDeleted != true)
                                .OrderByDescending(x => x.ID).FirstOrDefault();
                            if (hrWaitingStep != null)
                            {
                                stepName = hrWaitingStep.StepName ?? "";
                            }
                        }
                    }
                }
                else
                {
                    // ── TẠO MỚI ───────────────────────────────────────────────────
                    entity = new RERPAPI.Model.Entities.JobPerfomanceEvaluationNew
                    {
                        // FK người liên quan
                        EmployeeID = model.EmployeeID,
                        EmployeeEvaluationID = model.EmployeeEvaluationID,
                        EvaluationEmployeeLoaiHDID = model.EvaluationEmployeeLoaiHDID,
                        ConclusionEmployeeLoaiHDID = model.ConclusionEmployeeLoaiHDID,

                        // Người duyệt
                        TBPApproveID = model.EmployeeEvaluationID,
                        //TBPApproveName = model.TBPApproveName,
                        //HCNSApproveID = model.HCNSApproveID,
                        //HCNSApproveName = model.HCNSApproveName,
                        //BGDApproveID = model.BGDApproveID,
                        //BGDApproveName = model.BGDApproveName,

                        // Công việc chính
                        MainJobmainResponsibilities = model.MainJobmainResponsibilities,

                        // Nhóm A
                        ProfessionalCompetency = model.ProfessionalCompetency,
                        ProfessionalKnowledge = model.ProfessionalKnowledge,
                        ToolAndSystemSkills = model.ToolAndSystemSkills,
                        WorkQuality = model.WorkQuality,
                        WorkProgress = model.WorkProgress,
                        ProblemSolvingAbility = model.ProblemSolvingAbility,

                        // Nhóm B
                        Proactiveness = model.Proactiveness,
                        CollaborationAndSupport = model.CollaborationAndSupport,
                        CommunicationAndTeamwork = model.CommunicationAndTeamwork,
                        WorkOutputKPI = model.WorkOutputKPI,

                        // Nhóm C
                        DisciplineAndAttitude = model.DisciplineAndAttitude,
                        ComplianceWithRegulations = model.ComplianceWithRegulations,
                        Attendance = model.Attendance,
                        WorkStyle = model.WorkStyle,
                        AttitudeAndResponsibility = model.AttitudeAndResponsibility,

                        // Nhóm D
                        CulturalFitRTC = model.CulturalFitRTC,
                        LearningAndGrowthMindset = model.LearningAndGrowthMindset,
                        CompanyCommitment = model.CompanyCommitment,

                        // Tổng điểm & xếp loại
                        TotalScore = model.TotalScore,
                        EvaluationGrade = model.EvaluationGrade,
                        //-----------------------------------------TBP----------------------------------------------------------
                        /// // Nhóm A: Năng lực chuyên môn
                        TBPProfessionalCompetency = model.TBPProfessionalCompetency,
                        TBPProfessionalKnowledge = model.TBPProfessionalKnowledge,
                        TBPToolAndSystemSkills = model.TBPToolAndSystemSkills,
                        TBPWorkQuality = model.TBPWorkQuality,
                        TBPWorkProgress = model.TBPWorkProgress,
                        TBPProblemSolvingAbility = model.TBPProblemSolvingAbility,
                        // Nhóm B: Hiệu quả công việc & phối hợp
                        TBPProactiveness = model.TBPProactiveness,
                        TBPCollaborationAndSupport = model.TBPCollaborationAndSupport,
                        TBPCommunicationAndTeamwork = model.TBPCommunicationAndTeamwork,
                        TBPWorkOutputKPI = model.TBPWorkOutputKPI,

                        // Nhóm C: Kỷ luật, tác phong & thái độ
                        TBPDisciplineAndAttitude = model.TBPDisciplineAndAttitude,
                        TBPComplianceWithRegulations = model.TBPComplianceWithRegulations,
                        TBPAttendance = model.TBPAttendance,
                        TBPWorkStyle = model.TBPWorkStyle,
                        TBPAttitudeAndResponsibility = model.TBPAttitudeAndResponsibility,

                        // Nhóm D: Văn hóa, phát triển & gắn bó
                        TBPCulturalFitRTC = model.TBPCulturalFitRTC,
                        TBPLearningAndGrowthMindset = model.TBPLearningAndGrowthMindset,
                        TBPCompanyCommitment = model.TBPCompanyCommitment,

                        // Tổng điểm & xếp loại
                        TBPTotalScore = model.TBPTotalScore,
                        TBPEvaluationGrade = model.TBPEvaluationGrade,
                        //--------------------------------------------------------------------------------------------------


                        // Nhận xét / Kết luận
                        Strengths = model.Strengths,
                        AreasForImprovement = model.AreasForImprovement,
                        RecommendationsOrOther = model.RecommendationsOrOther,
                        OtherConclusion = model.OtherConclusion,

                        // Thời gian & địa điểm
                        DateStart = model.DateStart,
                        DateEnd = model.DateEnd,
                        DateEvaluation = null,
                        LocationEvaluation = model.LocationEvaluation,

                        TBPStrengths = model.TBPStrengths,
                        TBPAreasForImprovement = model.TBPAreasForImprovement,
                        TBPConclusionEmployeeLoaiHDID = model.TBPConclusionEmployeeLoaiHDID,
                        TBPRecommendationsOrOther = model.TBPRecommendationsOrOther,

                        // Audit
                        CreatedBy = model.CreatedBy,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false,
                    };

                    if (await _jobPerfomanceEvaluationNewRepo.CreateAsync(entity) <= 0)
                        return Ok(ApiResponseFactory.Fail(null, "Thêm mới thất bại!"));

                    // Tạo bước duyệt ban đầu: HR chờ gửi mail
                    await _jobPerfomanceEvaluationApproveRepo.CreateAsync(new JobPerfomanceEvaluationApprove
                    {
                        JobPerfomanceEvaluationID = entity.ID,
                        Step = 1,
                        StepName = "HR: Chờ gửi mail",
                        StatusApprove = 0,
                        DateApproved = DateTime.Now,
                        IsDeleted = false
                    });
                    stepName = "HR: Chờ gửi mail";
                }
                var data = new { ID = entity.ID, StepName = stepName };

                return Ok(ApiResponseFactory.Success(data, "Lưu dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(null, ex.Message));
            }
        }

        //[HttpPost("save-data")]
        //public async Task<IActionResult> SaveData([FromBody] JobPerfomanceEvaluationDTO model)
        //{
        //    try
        //    {
        //        if (model == null)
        //        {
        //            return Ok(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));
        //        }
        //        RERPAPI.Model.Entities.JobPerfomanceEvaluation entity;
        //        if (model.ID > 0)
        //        {
        //            entity = await _jobPerfomanceEvaluationRepo.GetByIDAsync(model.ID);
        //            if (entity == null)
        //            {
        //                return Ok(ApiResponseFactory.Fail(null, "Không tìm thấy dữ liệu"));
        //            }
        //            entity.ID = model.ID;
        //            entity.EmployeeID = model.EmployeeID;
        //            entity.LeaderID = model.LeaderID;
        //            entity.EmployeeEvaluationID = model.EmployeeEvaluationID;
        //            entity.DateStart = model.DateStart;
        //            entity.DateEnd = model.DateEnd;
        //            entity.ProfessionalCompetence = model.ProfessionalCompetence;
        //            entity.JobComplete = model.JobComplete;
        //            entity.Consciousness = model.Consciousness;
        //            entity.Regulations = model.Regulations;
        //            entity.OtherPossiblities = model.OtherPossiblities;
        //            entity.PesonalWishes = model.PesonalWishes;
        //            entity.AssignTask = model.AssignTask;
        //            entity.ResultOfImplementation = model.ResultOfImplementation;
        //            entity.OtherComment = model.OtherComment;
        //            entity.Recomment = model.Recomment;
        //            entity.EmployeeLoaiHDID = model.EmployeeLoaiHDID;
        //            entity.SalaryPropose = model.SalaryPropose;
        //            entity.DateEvaluation = model.DateEvaluation;
        //            entity.LocationEvaluation = model.LocationEvaluation;
        //            entity.EmployeeName = model.EmployeeName;
        //            entity.LeaderName = model.LeaderName;
        //            entity.EmployeeEvaluationName = model.EmployeeEvaluationName;
        //            entity.ResultOfImplementationConsciousness = model.ResultOfImplementationConsciousness;
        //            entity.TBPApproveID = model.TBPApproveID;
        //            entity.IsDeleted = false;
        //            if (await _jobPerfomanceEvaluationRepo.UpdateAsync(entity) <= 0)
        //            {
        //                return Ok(ApiResponseFactory.Fail(null, "Cập nhật thất bại!"));
        //            }
        //        }
        //        else
        //        {
        //            entity = new RERPAPI.Model.Entities.JobPerfomanceEvaluation
        //            {
        //                EmployeeID = model.EmployeeID,
        //                LeaderID = model.LeaderID,
        //                EmployeeEvaluationID = model.EmployeeEvaluationID,
        //                DateStart = model.DateStart,
        //                DateEnd = model.DateEnd,
        //                ProfessionalCompetence = model.ProfessionalCompetence,
        //                JobComplete = model.JobComplete,
        //                Consciousness = model.Consciousness,
        //                Regulations = model.Regulations,
        //                OtherPossiblities = model.OtherPossiblities,
        //                PesonalWishes = model.PesonalWishes,
        //                AssignTask = model.AssignTask,
        //                ResultOfImplementation = model.ResultOfImplementation,
        //                OtherComment = model.OtherComment,
        //                Recomment = model.Recomment,
        //                EmployeeLoaiHDID = model.EmployeeLoaiHDID,
        //                SalaryPropose = model.SalaryPropose,
        //                DateEvaluation = null,
        //                LocationEvaluation = model.LocationEvaluation,
        //                EmployeeName = model.EmployeeName,
        //                LeaderName = model.LeaderName,
        //                EmployeeEvaluationName = model.EmployeeEvaluationName,
        //                ResultOfImplementationConsciousness = model.ResultOfImplementationConsciousness,
        //                TBPApproveID = model.TBPApproveID,
        //                IsDeleted = false
        //            };
        //            if (await _jobPerfomanceEvaluationRepo.CreateAsync(entity) <= 0)
        //            {
        //                return Ok(ApiResponseFactory.Fail(null, "Thêm mới thất bại!"));
        //            }
        //        }
        //        List<JobPerfomanceEvaluationCriterion> dbCriteria = _jobPerfomanceEvaluationCriteriaRepo.GetAll(x => x.JobPerfomanceEvaluationID == entity.ID && !x.IsDeleted.Value).ToList();
        //        var requestIds = model.ListJobPerfomanceEvaluationCriterion?.Where(x => x.ID > 0).Select(x => x.ID).ToList() ?? new List<int>();
        //        foreach (JobPerfomanceEvaluationCriterion oldItem in dbCriteria.Where((JobPerfomanceEvaluationCriterion x) => !requestIds.Contains(x.ID)))
        //        {
        //            oldItem.IsDeleted = true;
        //            await _jobPerfomanceEvaluationCriteriaRepo.UpdateAsync(oldItem);
        //        }
        //        if (model.ListJobPerfomanceEvaluationCriterion?.Any() ?? false)
        //        {
        //            foreach (JobPerfomanceEvaluationCriterion item in model.ListJobPerfomanceEvaluationCriterion)
        //            {
        //                if (item.ID > 0)
        //                {
        //                    JobPerfomanceEvaluationCriterion existItem = dbCriteria.FirstOrDefault((JobPerfomanceEvaluationCriterion x) => x.ID == item.ID);
        //                    if (existItem != null)
        //                    {
        //                        existItem.STT = item.STT;
        //                        existItem.CodeCriteria = item.CodeCriteria;
        //                        existItem.NameCriteria = item.NameCriteria;
        //                        existItem.ResultEvaluation = item.ResultEvaluation;
        //                        existItem.Note = item.Note;
        //                        existItem.IsDeleted = false;
        //                        await _jobPerfomanceEvaluationCriteriaRepo.UpdateAsync(existItem);
        //                    }
        //                }
        //                else
        //                {
        //                    JobPerfomanceEvaluationCriterion newItem = new JobPerfomanceEvaluationCriterion
        //                    {
        //                        JobPerfomanceEvaluationID = entity.ID,
        //                        STT = item.STT,
        //                        CodeCriteria = item.CodeCriteria,
        //                        NameCriteria = item.NameCriteria,
        //                        ResultEvaluation = item.ResultEvaluation,
        //                        Note = item.Note,
        //                        IsDeleted = false
        //                    };
        //                    await _jobPerfomanceEvaluationCriteriaRepo.CreateAsync(newItem);
        //                }
        //            }
        //        }
        //        if (model.ID > 0)
        //        {
        //            var step2 = _jobPerfomanceEvaluationApproveRepo.GetAll(x => x.JobPerfomanceEvaluationID == entity.ID && x.Step == 2 && x.StatusApprove == 0 && !x.IsDeleted.Value)
        //                        .OrderByDescending(x => x.ID).FirstOrDefault();
        //            if (step2 != null)
        //            {
        //                bool hasLeaderEval = !string.IsNullOrWhiteSpace(entity.AssignTask) || !string.IsNullOrWhiteSpace(entity.ResultOfImplementation) || !string.IsNullOrWhiteSpace(entity.Recomment);
        //                step2.StepName = hasLeaderEval ? "TBP: Đã đánh giá, chờ xác nhận" : "TBP: Chờ đánh giá";
        //                await _jobPerfomanceEvaluationApproveRepo.UpdateAsync(step2);
        //            }
        //        }
        //        else
        //        {
        //            //await _jobPerfomanceEvaluationApproveRepo.CreateAsync(new JobPerfomanceEvaluationApprove
        //            //{
        //            //    JobPerfomanceEvaluationID = entity.ID,
        //            //    Step = 1,
        //            //    StepName = "Người lao động: Chờ xác nhận",
        //            //    StatusApprove = 0,
        //            //    DateApproved = DateTime.Now,
        //            //    IsDeleted = false
        //            //});
        //            await _jobPerfomanceEvaluationApproveRepo.CreateAsync(new JobPerfomanceEvaluationApprove
        //            {
        //                JobPerfomanceEvaluationID = entity.ID,
        //                Step = 1,
        //                StepName = "HR: Chờ gửi mail",
        //                StatusApprove = -2,
        //                DateApproved = DateTime.Now,
        //                IsDeleted = false
        //            });
        //        }

        //        return Ok(ApiResponseFactory.Success(entity.ID, "Lưu dữ liệu thành công!"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(ApiResponseFactory.Fail(null, ex.Message));
        //    }
        //}

        //[HttpPost("confirm")]
        //public async Task<IActionResult> Confirm([FromBody] ConfirmRequest req)
        //{
        //    try
        //    {
        //        List<int> ids = new List<int>();
        //        if (req.Id.ValueKind == JsonValueKind.Array)
        //        {
        //            ids = (from e in req.Id.EnumerateArray()
        //                   select e.GetInt32()).ToList();
        //        }
        //        else if (req.Id.ValueKind == JsonValueKind.Number)
        //        {
        //            ids.Add(req.Id.GetInt32());
        //        }
        //        if (!ids.Any())
        //        {
        //            return Ok(ApiResponseFactory.Fail(null, "Danh sách ID không hợp lệ!"));
        //        }
        //        if (req.IsApprove == 2 && string.IsNullOrWhiteSpace(req.Reason))
        //        {
        //            return Ok(ApiResponseFactory.Fail(null, "Bạn phải nhập lý do khi không duyệt!"));
        //        }
        //        Dictionary<string, (int cur, string curName, int next, string nextName)> roleMap =
        //      new Dictionary<string, (int, string, int, string)>(StringComparer.OrdinalIgnoreCase)
        //          {
        //            { "employee", (1, "Người lao động", 2, "TBP") },
        //            { "manager",  (2, "TBP", 3, "HR") },
        //            { "tbp",      (2, "TBP", 3, "HR") },
        //            { "hr",       (3, "Phòng HR", 4, "BGĐ") },
        //            { "bgd",      (4, "BGĐ", 0, "") }
        //          };
        //        if (!roleMap.TryGetValue(req.Role ?? "", out var info))
        //        {
        //            return Ok(ApiResponseFactory.Fail(null, "Role '" + req.Role + "' không hợp lệ!"));
        //        }
        //        int successCount = 0;
        //        foreach (int id in ids)
        //        {
        //            if (req.IsApprove == 0 && info.next > 0)
        //            {
        //                var nextStep = _jobPerfomanceEvaluationApproveRepo.GetAll(x =>
        //                    x.JobPerfomanceEvaluationID == id && x.Step == info.next && x.IsDeleted != true)
        //                    .OrderByDescending(x => x.ID).FirstOrDefault();

        //                if (nextStep != null && nextStep.StatusApprove != 0) continue; // Đã duyệt bước sau, không cho hủy bước trước
        //            }
        //            //// Lấy bản ghi phê duyệt hiện tại
        //            //var record = (req.IsApprove != 0)
        //            //    ? _jobPerfomanceEvaluationApproveRepo.GetAll(x => x.JobPerfomanceEvaluationID == id && x.Step == info.cur && x.StatusApprove == 0 && x.IsDeleted != true).FirstOrDefault()
        //            //    : _jobPerfomanceEvaluationApproveRepo.GetAll(x => x.JobPerfomanceEvaluationID == id && x.Step == info.cur && x.StatusApprove == 1 && x.IsDeleted != true).FirstOrDefault();
        //            // Lấy bản ghi phê duyệt hiện tại
        //            var record = (req.IsApprove != 0)
        //                ? _jobPerfomanceEvaluationApproveRepo.GetAll(x =>
        //                      x.JobPerfomanceEvaluationID == id
        //                      && x.Step == info.cur
        //                      && (x.StatusApprove == 0 || (info.cur == 1 && x.StatusApprove == -1))
        //                      && x.IsDeleted != true).FirstOrDefault()
        //                : _jobPerfomanceEvaluationApproveRepo.GetAll(x =>
        //                      x.JobPerfomanceEvaluationID == id
        //                      && x.Step == info.cur
        //                      && x.StatusApprove == 1
        //                      && x.IsDeleted != true).FirstOrDefault();

        //            if (record == null) continue;
        //            if (req.IsApprove == 1)
        //            {
        //                record.StatusApprove = 1;
        //                record.StepName = info.curName + ": Xác nhận";
        //                record.DateApproved = DateTime.Now;
        //                record.ReasonUnApproved = null;
        //                await _jobPerfomanceEvaluationApproveRepo.UpdateAsync(record);
        //                if (info.next > 0)
        //                {
        //                    var nextRec = _jobPerfomanceEvaluationApproveRepo.GetAll(x => x.JobPerfomanceEvaluationID == id && x.Step == info.next && x.IsDeleted != true)
        //                             .OrderByDescending(x => x.ID).FirstOrDefault();

        //                    string nextStepName = (info.cur == 1) ? (info.nextName + ": Chờ đánh giá") : (info.nextName + ": Chờ xác nhận");

        //                    if (nextRec == null)
        //                    {
        //                        await _jobPerfomanceEvaluationApproveRepo.CreateAsync(new JobPerfomanceEvaluationApprove
        //                        {
        //                            JobPerfomanceEvaluationID = id,
        //                            Step = info.next,
        //                            StepName = nextStepName,
        //                            StatusApprove = 0,
        //                            IsDeleted = false
        //                        });
        //                    }
        //                    else
        //                    {
        //                        nextRec.StatusApprove = 0;
        //                        nextRec.DateApproved = null;
        //                        nextRec.StepName = nextStepName;
        //                        nextRec.IsDeleted = false;
        //                        await _jobPerfomanceEvaluationApproveRepo.UpdateAsync(nextRec);
        //                    }
        //                }
        //                successCount++;
        //            }
        //            else if (req.IsApprove == 2)// KHÔNG DUYỆT (REJECT)
        //            {
        //                record.StatusApprove = 2;
        //                record.DateApproved = DateTime.Now;
        //                record.ReasonUnApproved = req.Reason;
        //                record.StepName = info.curName + ": Không xác nhận";
        //                await _jobPerfomanceEvaluationApproveRepo.UpdateAsync(record);
        //                successCount++;
        //            }
        //            else // HỦY XÁC NHẬN (REVERT / UNDO)
        //            {
        //                record.StatusApprove = 0;
        //                record.DateApproved = null;
        //                record.ReasonUnApproved = null;
        //                record.StepName = info.curName + ": Chờ xác nhận";
        //                await _jobPerfomanceEvaluationApproveRepo.UpdateAsync(record);
        //                // Xóa bước chờ của cấp trên (nếu có)
        //                if (info.next > 0)
        //                {
        //                    var next = _jobPerfomanceEvaluationApproveRepo.GetAll(x => x.JobPerfomanceEvaluationID == id && x.Step == info.next && x.IsDeleted != true).FirstOrDefault();
        //                    if (next != null)
        //                    {
        //                        next.IsDeleted = true;
        //                        await _jobPerfomanceEvaluationApproveRepo.UpdateAsync(next);
        //                    }
        //                }
        //                successCount++;
        //            }
        //            var masterEntity = await _jobPerfomanceEvaluationRepo.GetByIDAsync(id);
        //            if (masterEntity == null)
        //            {
        //                continue;
        //            }
        //            var currentUser = ObjectMapper.GetCurrentUser(User.Claims.ToDictionary(x => x.Type, x => x.Value));


        //            int currentUserId = currentUser.EmployeeID;
        //            string currentFullName = currentUser.FullName;
        //            if (req.IsApprove == 1)
        //            {
        //                switch (record.Step)
        //                {
        //                    case 2:
        //                        //masterEntity.TBPApproveID = currentUserId;
        //                        masterEntity.TBPApproveName = currentFullName;
        //                        break;
        //                    case 3:
        //                        masterEntity.HCNSApproveID = currentUserId;
        //                        masterEntity.HCNSApproveName = currentFullName;
        //                        break;
        //                    case 4:
        //                        masterEntity.BGDApproveID = currentUserId;
        //                        masterEntity.BGDApproveName = currentFullName;
        //                        masterEntity.DateEvaluation = DateTime.Now;
        //                        break;
        //                }
        //            }
        //            else if (req.IsApprove == 0)
        //            {
        //                switch (record.Step)
        //                {
        //                    case 2:
        //                        //masterEntity.TBPApproveID = 0;
        //                        masterEntity.TBPApproveName = "";
        //                        break;
        //                    case 3:
        //                        masterEntity.HCNSApproveID = 0;
        //                        masterEntity.HCNSApproveName = "";
        //                        break;
        //                    case 4:
        //                        masterEntity.BGDApproveID = 0;
        //                        masterEntity.BGDApproveName = "";
        //                        break;
        //                }
        //            }
        //            await _jobPerfomanceEvaluationRepo.UpdateAsync(masterEntity);
        //        }
        //        string label = (req.IsApprove == 1) ? "Xác nhận" : (req.IsApprove == 2 ? "Không xác nhận" : "Huỷ xác nhận");
        //        return Ok(ApiResponseFactory.Success(successCount, $"{label} thành công {successCount}/{ids.Count} bản ghi!"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(ApiResponseFactory.Fail(null, ex.Message));
        //    }
        //}
        [HttpPost("confirm")]
        public async Task<IActionResult> Confirm([FromBody] ConfirmRequest req)
        {
            try
            {
                List<int> ids = new List<int>();
                if (req.Id.ValueKind == JsonValueKind.Array)
                    ids = req.Id.EnumerateArray().Select(e => e.GetInt32()).ToList();
                else if (req.Id.ValueKind == JsonValueKind.Number)
                    ids.Add(req.Id.GetInt32());

                if (!ids.Any())
                    return Ok(ApiResponseFactory.Fail(null, "Danh sách ID không hợp lệ!"));

                if (req.IsApprove == 2 && string.IsNullOrWhiteSpace(req.Reason))
                    return Ok(ApiResponseFactory.Fail(null, "Bạn phải nhập lý do khi không duyệt!"));

                var roleMap = new Dictionary<string, (int cur, string curName, int next, string nextName)>(
                    StringComparer.OrdinalIgnoreCase)
        {
            { "employee", (1, "Người lao động", 2, "TBP") },
            { "manager",  (2, "TBP",            3, "HR")  },
            { "tbp",      (2, "TBP",            3, "HR")  },
            { "hr",       (3, "HR",       4, "BGĐ") },
            { "bgd",      (4, "BGĐ",            0, "")    },
        };

                if (!roleMap.TryGetValue(req.Role ?? "", out var info))
                    return Ok(ApiResponseFactory.Fail(null, $"Role '{req.Role}' không hợp lệ!"));

                // Lấy current user 1 lần ngoài vòng lặp
                var currentUser = ObjectMapper.GetCurrentUser(
                    User.Claims.ToDictionary(x => x.Type, x => x.Value));
                int currentUserId = currentUser.EmployeeID;
                string currentFullName = currentUser.FullName;

                int successCount = 0;

                foreach (int id in ids)
                {
                    // ── Kiểm tra có thể hủy xác nhận không ─────────────────────────
                    if (req.IsApprove == 0 && info.next > 0)
                    {
                        var nextStep = _jobPerfomanceEvaluationApproveRepo
                            .GetAll(x => x.JobPerfomanceEvaluationID == id
                                      && x.Step == info.next
                                      && x.IsDeleted != true)
                            .OrderByDescending(x => x.ID).FirstOrDefault();

                        if (nextStep != null && nextStep.StatusApprove != 0) continue;
                    }

                    // ── Lấy bản ghi approve hiện tại ────────────────────────────────
                    var record = (req.IsApprove != 0)
                        ? _jobPerfomanceEvaluationApproveRepo.GetAll(x =>
                              x.JobPerfomanceEvaluationID == id
                              && x.Step == info.cur
                              && (x.StatusApprove == 0 || (info.cur == 1 && x.StatusApprove == -1))
                              && x.IsDeleted != true).FirstOrDefault()
                        : _jobPerfomanceEvaluationApproveRepo.GetAll(x =>
                              x.JobPerfomanceEvaluationID == id
                              && x.Step == info.cur
                              && x.StatusApprove == 1
                              && x.IsDeleted != true).FirstOrDefault();

                    if (record == null) continue;

                    // ── Xử lý theo isApprove ────────────────────────────────────────
                    if (req.IsApprove == 1) // DUYỆT
                    {
                        record.StatusApprove = 1;
                        record.StepName = info.curName + ": Xác nhận";
                        record.DateApproved = DateTime.Now;
                        record.ReasonUnApproved = null;
                        await _jobPerfomanceEvaluationApproveRepo.UpdateAsync(record);

                        if (info.next > 0)
                        {
                            var nextRec = _jobPerfomanceEvaluationApproveRepo
                                .GetAll(x => x.JobPerfomanceEvaluationID == id
                                          && x.Step == info.next
                                          && x.IsDeleted != true)
                                .OrderByDescending(x => x.ID).FirstOrDefault();

                            string nextStepName = info.cur == 1
                                ? info.nextName + ": Chờ đánh giá"
                                : info.nextName + ": Chờ xác nhận";

                            if (nextRec == null)
                            {
                                await _jobPerfomanceEvaluationApproveRepo.CreateAsync(
                                    new JobPerfomanceEvaluationApprove
                                    {
                                        JobPerfomanceEvaluationID = id,
                                        Step = info.next,
                                        StepName = nextStepName,
                                        StatusApprove = 0,
                                        IsDeleted = false,
                                    });
                            }
                            else
                            {
                                nextRec.StatusApprove = 0;
                                nextRec.DateApproved = null;
                                nextRec.StepName = nextStepName;
                                nextRec.IsDeleted = false;
                                await _jobPerfomanceEvaluationApproveRepo.UpdateAsync(nextRec);
                            }
                        }
                        successCount++;
                    }
                    else if (req.IsApprove == 2) // KHÔNG DUYỆT
                    {
                        record.StatusApprove = 2;
                        record.DateApproved = DateTime.Now;
                        record.ReasonUnApproved = $"{info.curName} - {currentUser.FullName} không xác nhận: {req.Reason}";
                        record.StepName = info.curName + $": Không xác nhận";
                        await _jobPerfomanceEvaluationApproveRepo.UpdateAsync(record);
                        successCount++;
                    }
                    else // HỦY XÁC NHẬN
                    {
                        record.StatusApprove = 0;
                        record.DateApproved = null;
                        record.ReasonUnApproved = null;
                        record.StepName = info.curName + ": Chờ xác nhận";
                        await _jobPerfomanceEvaluationApproveRepo.UpdateAsync(record);

                        if (info.next > 0)
                        {
                            var next = _jobPerfomanceEvaluationApproveRepo
                                .GetAll(x => x.JobPerfomanceEvaluationID == id
                                          && x.Step == info.next
                                          && x.IsDeleted != true).FirstOrDefault();
                            if (next != null)
                            {
                                next.IsDeleted = true;
                                await _jobPerfomanceEvaluationApproveRepo.UpdateAsync(next);
                            }
                        }
                        successCount++;

                    }

                    // ── Cập nhật approve names vào master entity ─────────────────────
                    // Thử bảng cũ trước
                    var masterOld = await _jobPerfomanceEvaluationNewRepo.GetByIDAsync(id);
                    if (masterOld != null)
                    {
                        ApplyApproveNames(masterOld, record.Step ?? 0, req.IsApprove, currentUserId, currentFullName);
                        await _jobPerfomanceEvaluationNewRepo.UpdateAsync(masterOld);
                    }
                    else
                    {
                        // Fallback: bảng mới (JobPerfomanceEvaluationNew)
                        var masterNew = await _jobPerfomanceEvaluationNewRepo.GetByIDAsync(id);
                        if (masterNew != null)
                        {
                            ApplyApproveNamesNew(masterNew, record.Step ?? 0, req.IsApprove, currentUserId, currentFullName);
                            await _jobPerfomanceEvaluationNewRepo.UpdateAsync(masterNew);
                        }
                    }
                }

                string label = req.IsApprove == 1 ? "Xác nhận"
                             : req.IsApprove == 2 ? "Không xác nhận"
                             : "Huỷ xác nhận";
                return Ok(ApiResponseFactory.Success(successCount,
                    $"{label} thành công {successCount}/{ids.Count} bản ghi!"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(null, ex.Message));
            }
        }

        // ── Helper: cập nhật approve names bảng cũ ──────────────────────────────────
        private void ApplyApproveNames(
            RERPAPI.Model.Entities.JobPerfomanceEvaluationNew entity,
            int step, int isApprove, int userId, string fullName)
        {
            if (isApprove == 1)
            {
                switch (step)
                {
                    case 2: entity.TBPApproveName = fullName; break;
                    case 3: entity.HCNSApproveID = userId; entity.HCNSApproveName = fullName; break;
                    case 4:
                        entity.BGDApproveID = userId; entity.BGDApproveName = fullName;
                        entity.DateEvaluation = DateTime.Now; break;
                }
            }
            else if (isApprove == 0)
            {
                switch (step)
                {
                    case 2: entity.TBPApproveName = ""; break;
                    case 3: entity.HCNSApproveID = 0; entity.HCNSApproveName = ""; break;
                    case 4: entity.BGDApproveID = 0; entity.BGDApproveName = ""; break;
                }
            }
        }

        // ── Helper: cập nhật approve names bảng mới ─────────────────────────────────
        private void ApplyApproveNamesNew(
            RERPAPI.Model.Entities.JobPerfomanceEvaluationNew entity,
            int step, int isApprove, int userId, string fullName)
        {
            if (isApprove == 1)
            {
                switch (step)
                {
                    case 3: entity.HCNSApproveID = userId; entity.HCNSApproveName = fullName; break;
                    case 4:
                        entity.BGDApproveID = userId; entity.BGDApproveName = fullName;
                        entity.DateEvaluation = DateTime.Now; break;
                }
            }
            else if (isApprove == 0)
            {
                switch (step)
                {
                    case 3: entity.HCNSApproveID = 0; entity.HCNSApproveName = ""; break;
                    case 4: entity.BGDApproveID = 0; entity.BGDApproveName = ""; break;
                }
            }
        }


        [HttpPost("cancel-confirm")]
        public async Task<IActionResult> CancelConfirm([FromBody] CancelConfirmRequest req)
        {
            ConfirmRequest fakeReq = new ConfirmRequest
            {
                Id = JsonSerializer.SerializeToElement(req.Id),
                Role = req.Role,
                IsApprove = 0,
                Reason = null
            };
            return await Confirm(fakeReq);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                //var entity = await _jobPerfomanceEvaluationRepo.GetByIDAsync(id);
                //if (entity == null || entity.IsDeleted.GetValueOrDefault())
                //{
                //    return Ok(ApiResponseFactory.Fail(null, "Không tìm thấy phiếu!"));
                //}
                //entity.IsDeleted = true;
                //await _jobPerfomanceEvaluationRepo.UpdateAsync(entity);
                //return Ok(ApiResponseFactory.Success(id, "Xóa phiếu thành công!"));


                var newEntity = await _jobPerfomanceEvaluationNewRepo.GetByIDAsync(id);
                if (newEntity != null && !newEntity.IsDeleted.GetValueOrDefault())
                {
                    newEntity.IsDeleted = true;
                    await _jobPerfomanceEvaluationNewRepo.UpdateAsync(newEntity);
                    return Ok(ApiResponseFactory.Success(id, "Xóa phiếu thành công!"));
                }
                return Ok(ApiResponseFactory.Fail(null, "Không tìm thấy phiếu!"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(null, ex.Message));
            }
        }
        //[HttpPost("send-mail")]
        //[RequiresPermission("N1,N2")]
        //public async Task<IActionResult> SendEmailOferLetter([FromBody] List<RERPAPI.Model.Entities.EmployeeSendEmail> sendEmails)
        //{
        //    try
        //    {
        //        var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
        //        CurrentUser _currentUser = ObjectMapper.GetCurrentUser(claims);
        //        var footer = _configuration["FooterMail:HR:Footer"];
        //        //string emailCC = "";
        //        string emailCC = "dept_manager@rtc.edu.vn";
        //        if (sendEmails.Count() > 0)
        //        {
        //            foreach (var email in sendEmails)
        //            {
        //                if (email.ID > 0)
        //                {
        //                    // cập nhật trạng thái ở bảng duyệt tờ trình
        //                    // cập nhật là đã gửi mail
        //                    var jobPerfomanceEvaluationApprove = _jobPerfomanceEvaluationApproveRepo.GetAll(c => c.JobPerfomanceEvaluationID == email.ID && c.IsDeleted != true).OrderByDescending(c => c.Step).FirstOrDefault();
        //                    if (jobPerfomanceEvaluationApprove.Step != 0)
        //                        continue; // nếu step =0 thì là trạng thái hr tạo
        //                    if (jobPerfomanceEvaluation != null)
        //                    {
        //                        jobPerfomanceEvaluation. = email.StatusSend;
        //                        await _jobPerfomanceEvaluationRepo.UpdateAsync(jobPerfomanceEvaluation);
        //                    }
        //                }
        //                await _emailHelper.SendAsyncHr(email.EmailTo, email.Subject, email.Body + footer, cc: emailCC);
        //            }
        //        }
        //        return Ok(ApiResponseFactory.Success(null, "Gửi thành công!"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}
        [HttpPost("send-mail")]
        [RequiresPermission("N1,N2")]
        public async Task<IActionResult> SendEmailOferLetter([FromBody] List<EmployeeSendEmail> sendEmails)
        {
            try
            {
                var footer = _configuration["FooterMail:HR:FooterContract"];
                ////string emailCC = "dept_manager@rtc.edu.vn";
                //string emailCC = "";

                foreach (var email in sendEmails)
                {
                    if (email.ID <= 0) continue;

                    // Tìm record Step=1, StatusApprove=-2 (HR: Chờ gửi mail)
                    var waitingRecord = _jobPerfomanceEvaluationApproveRepo
                        .GetAll(c => c.JobPerfomanceEvaluationID == email.ID
                                  && c.Step == 1
                                  && c.StatusApprove == 0 && c.StepName == "HR: Chờ gửi mail"
                                  && c.IsDeleted != true)
                        .FirstOrDefault();


                    await _emailHelper.SendAsyncHr(
                        email.EmailTo, email.Subject,
                        email.Body + footer, cc: email.EmailCC);

                    if (waitingRecord == null) continue;  // Đã gửi rồi, gửi lại thì không cập nhật trạng thái

                    waitingRecord.StatusApprove = 0;
                    waitingRecord.StepName = "HR: Đã gửi mail";
                    waitingRecord.DateApproved = DateTime.Now;
                    if (await _jobPerfomanceEvaluationApproveRepo.UpdateAsync(waitingRecord) > 0)
                    {
                        await _jobPerfomanceEvaluationApproveRepo.CreateAsync(new JobPerfomanceEvaluationApprove
                        {
                            JobPerfomanceEvaluationID = waitingRecord.JobPerfomanceEvaluationID,
                            Step = 1,
                            StepName = "Người lao động: Chờ đánh giá",
                            StatusApprove = 0,
                            DateApproved = DateTime.Now,
                            IsDeleted = false
                        });
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Gửi thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-pending-count")]
        public IActionResult GetPendingCount()
        {
            var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
            var currentUser = ObjectMapper.GetCurrentUser(claims);
            int empId = currentUser.EmployeeID;

            //// NV cần tự đánh giá: phiếu có EmployeeID=mình, Step=1, SA=-1
            //int asEmployee = _jobPerfomanceEvaluationApproveRepo
            //    .GetAll(x => x.Step == 1 && x.StatusApprove == -1 && x.IsDeleted != true)
            //    .Join(_jobPerfomanceEvaluationNewRepo.GetAll(j => j.EmployeeID == empId && j.IsDeleted != true),
            //          a => a.JobPerfomanceEvaluationID, j => j.ID, (a, j) => a)
            //    .Count();

            //// Người đánh giá: EmployeeEvaluationID=mình, Step=2, SA=0 
            //int asEvaluator = _jobPerfomanceEvaluationApproveRepo
            //    .GetAll(x => x.Step == 2 && x.StatusApprove == 0 && x.IsDeleted != true && x.StepName == "TBP: Chờ đánh giá")
            //    .Join(_jobPerfomanceEvaluationNewRepo.GetAll(j => j.EmployeeEvaluationID == empId && j.IsDeleted != true),
            //          a => a.JobPerfomanceEvaluationID, j => j.ID, (a, j) => a)
            //    .Count();

            //// TBP: TBPApproveID=mình, Step=2, SA=0
            //int asTBP = _jobPerfomanceEvaluationApproveRepo
            //    .GetAll(x => x.Step == 2 && x.StatusApprove == 0 && x.IsDeleted != true && x.StepName == "TBP: Chờ xác nhận")
            //    .Join(_jobPerfomanceEvaluationNewRepo.GetAll(j => j.TBPApproveID == empId && j.IsDeleted != true),
            //          a => a.JobPerfomanceEvaluationID, j => j.ID, (a, j) => a)
            //    .Count();

            var vUserGroup = _vUserGroupLinksRepo.GetAll();
            var vUserGroupHR = vUserGroup.FirstOrDefault(x => (
                                                     x.Code == "N56") &&
                                                     x.UserID == currentUser.ID);
            var vUserGroupBGD = vUserGroup.FirstOrDefault(x => (
                                                     x.Code == "N1") &&
                                                     x.UserID == currentUser.ID);


            var approveQuery = _jobPerfomanceEvaluationApproveRepo
            .GetAll(x => x.IsDeleted != true);

            var evalQuery = _jobPerfomanceEvaluationNewRepo
                .GetAll(x => x.IsDeleted != true);
            var latestApproveQuery = _jobPerfomanceEvaluationApproveRepo
    .GetAll(x => x.IsDeleted != true)
    .GroupBy(x => x.JobPerfomanceEvaluationID)
    .Select(g => g.OrderByDescending(x => x.Step).FirstOrDefault());
            // NV tự đánh giá
            int asEmployee = latestApproveQuery.Count(a =>
                a.Step == 1 &&
                a.StatusApprove == 0 &&
                evalQuery.Any(j => j.ID == a.JobPerfomanceEvaluationID && j.EmployeeID == empId)
            );

            // Người đánh giá
            int asEvaluator = approveQuery.Count(a =>
                a.Step == 2 &&
                a.StatusApprove == 0 &&
                a.StepName == "TBP: Chờ đánh giá" &&
                evalQuery.Any(j => j.ID == a.JobPerfomanceEvaluationID && j.EmployeeEvaluationID == empId)
            );

            // TBP
            int asTBP = approveQuery.Count(a =>
                a.Step == 2 &&
                a.StatusApprove == 0 &&
                a.StepName == "TBP: Chờ xác nhận" &&
                evalQuery.Any(j => j.ID == a.JobPerfomanceEvaluationID && j.TBPApproveID == empId)
            );
            // HR
            int asHR = approveQuery.Count(a =>
                a.Step == 3 &&
                a.StatusApprove == 0 &&
                a.StepName == "HR: Chờ xác nhận" &&
                evalQuery.Any(j => j.ID == a.JobPerfomanceEvaluationID && vUserGroupHR != null)
            );
            // BGD
            int asBGD = approveQuery.Count(a =>
                a.Step == 4 &&
                a.StatusApprove == 0 &&
                a.StepName == "BGĐ: Chờ xác nhận" &&
                evalQuery.Any(j => j.ID == a.JobPerfomanceEvaluationID && vUserGroupBGD != null)
            );

            return Ok(ApiResponseFactory.Success(new
            {
                AsEmployee = asEmployee,
                AsEvaluator = asEvaluator,
                AsTBP = asTBP,
                AsHR = asHR,
                AsBGD = asBGD
            }, "OK"));
        }


    }
}