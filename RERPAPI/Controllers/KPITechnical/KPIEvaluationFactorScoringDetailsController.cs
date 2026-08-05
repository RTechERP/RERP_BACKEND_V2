using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.KPITech;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.KPITech;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Technical.KPI;

namespace RERPAPI.Controllers.KPITechnical
{
    [Route("api/[controller]")]
    [ApiController]
    public class KPIEvaluationFactorScoringDetailsController : ControllerBase
    {
        private KPIEvaluationPointRepo _kpiEvaluationPointRepo;
        private KPISessionRepo _kpiSessionRepo;
        private KPIEmployeePointRepo _kpiEmployeePointRepo;
        private KPIPositionRepo _kpiPositionRepo;
        private KPIPositionEmployeeRepo _kpiPositionEmployeeRepo;
        private KPIEvaluationRuleRepo _kpiEvaluationRuleRepo;
        private KPIExamRepo _kpiExamRepo;
        private KPIExamPositionRepo _kpiExamPositionRepo;
        private KPICriterionRepo _kpiCriterionRepo;
        private KPIEmployeePointDetailRepo _kpiEmployeePointDetailRepo;
        private KPISumaryEvaluationRepo _kpiSumaryEvaluationRepo;
        private UserTeamRepo _userTeamRepo;
        private EmployeeRepo _employeeRepo;
        private KPIEmployeeTeamRepo _kPIEmployeeTeamRepo;
        private KPIEmployeeTeamLinkRepo _kPIEmployeeTeamLinkRepo;
        private KPIEvaluationLogRepo _kpiEvaluationLogRepo;
        private KPIEvaluationRuleDetailRepo _kpiEvaluationRuleDetailRepo;
        private KPIEvaluationFactorRepo _kpiEvaluationFactorRepo;

        public KPIEvaluationFactorScoringDetailsController(
            KPIEvaluationPointRepo kpiEvaluationPointRepo,
            KPISessionRepo kpiSessionRepo,
            KPIEmployeePointRepo kpiEmployeePointRepo,
            KPIPositionRepo kpiPositionRepo,
            KPIPositionEmployeeRepo kpiPositionEmployeeRepo,
            KPIEvaluationRuleRepo kpiEvaluationRuleRepo,
            KPIExamRepo kpiExamRepo,
            KPIExamPositionRepo kpiExamPositionRepo,
            KPICriterionRepo kpiCriterionRepo,
            KPIEmployeePointDetailRepo kpiEmployeePointDetailRepo,
            KPISumaryEvaluationRepo kpiSumaryEvaluationRepo,
            UserTeamRepo userTeamRepo,
            EmployeeRepo employeeRepo,
            KPIEmployeeTeamRepo kPIEmployeeTeamRepo,
            KPIEmployeeTeamLinkRepo kPIEmployeeTeamLinkRepo,
            KPIEvaluationLogRepo kpiEvaluationLogRepo,
            KPIEvaluationRuleDetailRepo kpiEvaluationRuleDetailRepo,
            KPIEvaluationFactorRepo kpiEvaluationFactorRepo)
        {
            _kpiEvaluationPointRepo = kpiEvaluationPointRepo;
            _kpiSessionRepo = kpiSessionRepo;
            _kpiEmployeePointRepo = kpiEmployeePointRepo;
            _kpiPositionRepo = kpiPositionRepo;
            _kpiPositionEmployeeRepo = kpiPositionEmployeeRepo;
            _kpiEvaluationRuleRepo = kpiEvaluationRuleRepo;
            _kpiExamRepo = kpiExamRepo;
            _kpiExamPositionRepo = kpiExamPositionRepo;
            _kpiCriterionRepo = kpiCriterionRepo;
            _kpiEmployeePointDetailRepo = kpiEmployeePointDetailRepo;
            _kpiSumaryEvaluationRepo = kpiSumaryEvaluationRepo;
            _userTeamRepo = userTeamRepo;
            _employeeRepo = employeeRepo;
            _kPIEmployeeTeamLinkRepo = kPIEmployeeTeamLinkRepo;
            _kPIEmployeeTeamRepo = kPIEmployeeTeamRepo;
            _kpiEvaluationLogRepo = kpiEvaluationLogRepo;
            _kpiEvaluationRuleDetailRepo = kpiEvaluationRuleDetailRepo;
            _kpiEvaluationFactorRepo = kpiEvaluationFactorRepo;
        }

        #region lấy dữ liệu combobox bài đánh giá

        [HttpGet("get-combobox-exam")]
        public async Task<IActionResult> GetComboboxExam(int kpiSession)
        {
            try
            {
                List<KPIExam> data = _kpiExamRepo.GetAll(x => x.KPISessionID == kpiSession && x.IsDeleted == false);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion lấy dữ liệu combobox bài đánh giá

        #region lấy dữ liệu combobox kỳ đánh giá

        [HttpGet("get-combobox-session")]
        public async Task<IActionResult> GetComboboxKPISession(int kpiSession)
        {
            try
            {
                List<KPISession> data = _kpiSessionRepo.GetAll(x => x.IsDeleted == false).OrderByDescending(x => x.ID).ToList();
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion lấy dữ liệu combobox kỳ đánh giá

        #region lấy dữ liệu combobox nhân viên kỳ đánh giá

        [HttpGet("get-combobox-employee")]
        public async Task<IActionResult> GetEmployee(int kpiSession)
        {
            try
            {
                var param = new
                {
                    Status = 0,
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetEmployee", param);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion lấy dữ liệu combobox nhân viên kỳ đánh giá

        #region lấy dữ liệu bảng tiêu chí

        [HttpGet("kpi-criteria")]
        public async Task<IActionResult> GetKPICriteria(int criteriaYear, int criteriaQuarter)
        {
            try
            {
                List<KPICriterion> lstCol = _kpiCriterionRepo.GetAll(x => x.IsDeleted == false && x.KPICriteriaYear == criteriaYear && x.KPICriteriaQuater == criteriaQuarter);

                return Ok(ApiResponseFactory.Success(lstCol, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("kpi-criteria-pivot")]
        public async Task<IActionResult> GetKPICriteriaPivot(int criteriaYear, int criteriaQuarter)
        {
            try
            {
                var param = new
                {
                    Year = criteriaYear,
                    Quater = criteriaQuarter,
                };
                var data = await SqlDapper<object>.ProcedureToListTAsync("spGetKpiCriteriaPivot", param);

                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion lấy dữ liệu bảng tiêu chí

        #region load dữ liệu KPI kỹ năng , chuyên môn , chung , rule

        [HttpGet("load-kpi-kynang")]
        public async Task<IActionResult> LoadKPIKyNang(int kpiExamID, bool isPublicTBP, bool isPublicBGD, int employeeID)
        {
            try
            {
                var param = new
                {
                    EmployeeID = employeeID,
                    EvaluationType = 1,
                    KPIExamID = kpiExamID,
                    IsPublicTBP = isPublicTBP,
                    IsPublicBGD = isPublicBGD,
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetAllKPIEvaluationPoint_TNB", param);

                //var data = SQLHelper<object>.ProcedureToList("spGetAllKPIEvaluationPoint_TNB"
                // , new string[] { "@EmployeeID", "@EvaluationType", "@KPIExamID", "@IsPulbic" }
                // , new object[] { employeeID, 1, kpiExamID, isPublic });
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("load-kpi-chung")]
        public async Task<IActionResult> LoadKPIChung(int kpiExamID, bool isPublicTBP, bool isPublicBGD, int employeeID)
        {
            try
            {
                var param = new
                {
                    EmployeeID = employeeID,
                    EvaluationType = 3,
                    KPIExamID = kpiExamID,
                    IsPublicTBP = isPublicTBP,
                    IsPublicBGD = isPublicBGD,
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetAllKPIEvaluationPoint_TNB", param);
                //var data = SQLHelper<object>.ProcedureToList("spGetAllKPIEvaluationPoint_TNB"
                // , new string[] { "@EmployeeID", "@EvaluationType", "@KPIExamID", "@IsPulbic" }
                // , new object[] { employeeID, 3, kpiExamID, isPublic });
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("load-kpi-chuyenmon")]
        public async Task<IActionResult> LoadKPIChuyenMon(int kpiExamID, bool isPublicTBP, bool isPublicBGD, int employeeID)
        {
            try
            {
                var param = new
                {
                    EmployeeID = employeeID,
                    EvaluationType = 2,
                    KPIExamID = kpiExamID,
                    IsPublicTBP = isPublicTBP,
                    IsPublicBGD = isPublicBGD,
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetAllKPIEvaluationPoint_TNB", param);
                //var data = SQLHelper<object>.ProcedureToList("spGetAllKPIEvaluationPoint_TNB"
                // , new string[] { "@EmployeeID", "@EvaluationType", "@KPIExamID", "@IsPulbic" }
                // , new object[] { employeeID, 2, kpiExamID, isPublic });
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("load-kpi-rule-and-team")]
        public async Task<IActionResult> LoadKPIRule(int kpiExamID, bool isAmdinConfirm, int employeeID, int sessionID)
        {
            try
            {
                //Get possition của nhân viên
                List<KPIPosition> kpiPositions = _kpiPositionRepo.GetAll(x => x.KPISessionID == sessionID && x.IsDeleted == false);
                List<KPIPositionEmployee> kpiPositionEmployees = _kpiPositionEmployeeRepo.GetAll(x => x.EmployeeID == employeeID && x.IsDeleted == false);

                var empPosition = (from p in kpiPositions
                                   join pe in kpiPositionEmployees on p.ID equals pe.KPIPosiotionID
                                   select pe)

                     .FirstOrDefault() ?? new KPIPositionEmployee();

                KPIEvaluationRule rule = _kpiEvaluationRuleRepo.GetAll(x => x.KPISessionID == sessionID && x.KPIPositionID == (empPosition.KPIPosiotionID > 0 ? empPosition.KPIPosiotionID : 1) && x.IsDeleted == false)
                    .FirstOrDefault() ?? new KPIEvaluationRule(); // 1 là kỹ thuật

                int empPointId = await GetKPIEmployeePointID(rule.ID, employeeID);

                var param = new
                {
                    KPIEmployeePointID = empPointId
                };
                var data1 = await SqlDapper<object>.ProcedureToListAsync("spGetKpiRuleSumarizeTeamNew_TNB", param);
                //var data1 = SQLHelper<object>.ProcedureToList("spGetKpiRuleSumarizeTeamNew_TNB"
                // , new string[] { "@KPIEmployeePointID" }
                // , new object[] { empPointId });

                var param2 = new
                {
                    KPIEmployeePointID = empPointId,
                    IsPublic = 1
                };
                var data2 = await SqlDapper<object>.ProcedureToListAsync("spGetEmployeeRulePointByKPIEmpPointIDNew_TNB", param2);
                //var data2 = SQLHelper<object>.ProcedureToList("spGetEmployeeRulePointByKPIEmpPointIDNew_TNB"
                //  , new string[] { "@KPIEmployeePointID", "@IsPublic" }
                //  , new object[] { empPointId, 1 });

                var dtTeam = data1;
                var dtKpiRule = data2;

                List<KPIEmployeePointDetail> lst = _kpiEmployeePointDetailRepo.GetAll(x => x.KPIEmployeePointID == empPointId);

                return Ok(ApiResponseFactory.Success(new { dtTeam, dtKpiRule, lst }, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion load dữ liệu KPI kỹ năng , chuyên môn , chung , rule

        #region lấy dữ liệu KPIEmployeePointID

        [NonAction]
        public async Task<int> GetKPIEmployeePointID(int ruleID, int employeeID)
        {
            try
            {
                int empID = employeeID;
                if (empID <= 0)
                {
                    return -1;
                }
                if (ruleID <= 0)
                {
                    return -1;
                }
                KPIEmployeePoint model = _kpiEmployeePointRepo.GetAll(x => x.EmployeeID == empID && x.KPIEvaluationRuleID == ruleID && x.IsDelete == false).FirstOrDefault() ?? new KPIEmployeePoint();
                model.EmployeeID = empID;
                model.KPIEvaluationRuleID = ruleID;
                model.Status = 1;
                if (model.ID > 0)
                {
                    return model.ID;
                }
                else
                {
                    await _kpiEmployeePointRepo.CreateAsync(model);
                    return model.ID;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi: " + ex.Message);
            }
        }

        #endregion lấy dữ liệu KPIEmployeePointID

        #region save Datarule

        [HttpPost("save-data-rule")]
        public async Task<IActionResult> SaveDataRule([FromBody] SaveDataRuleRequestParam request)
        {
            try
            {
                KPIEmployeePoint master = _kpiEmployeePointRepo.GetByID(request.employeeID);
                var oldDetails = _kpiEmployeePointDetailRepo.GetAll(x => x.KPIEmployeePointID == request.employeeID)
                    .Select(x => new KPIEmployeePointDetail
                    {
                        ID = x.ID,
                        KPIEmployeePointID = x.KPIEmployeePointID,
                        KPIEvaluationRuleDetailID = x.KPIEvaluationRuleDetailID,
                        FirstMonth = x.FirstMonth,
                        SecondMonth = x.SecondMonth,
                        ThirdMonth = x.ThirdMonth,
                        PercentBonus = x.PercentBonus,
                        PercentRemaining = x.PercentRemaining
                    }).ToList();

                await _kpiEmployeePointDetailRepo.DeleteByAttributeAsync("KPIEmployeePointID", request.employeeID);

                master.TotalPercent = request.totalPercentRemaining;
                master.Status = 2;
                await _kpiEmployeePointRepo.UpdateAsync(master);

                var toCreateRuleDetails = new List<KPIEmployeePointDetail>();
                foreach (var item in request.lstKPIEmployeePointDetail)
                {
                    KPIEmployeePointDetail detail = new KPIEmployeePointDetail();
                    detail.KPIEmployeePointID = request.employeeID;
                    detail.KPIEvaluationRuleDetailID = item.ID;
                    detail.FirstMonth = item.FirstMonth;
                    detail.SecondMonth = item.SecondMonth;
                    detail.ThirdMonth = item.ThirdMonth;
                    detail.PercentBonus = item.PercentBonus;
                    detail.PercentRemaining = item.PercentRemaining;
                    toCreateRuleDetails.Add(detail);
                }
                if (toCreateRuleDetails.Count > 0)
                {
                    await _kpiEmployeePointDetailRepo.CreateRangeAsync(toCreateRuleDetails);
                }
                try
                {
                    var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                    var currentUser = ObjectMapper.GetCurrentUser(claims);

                    var employee = _employeeRepo.GetByID(master?.EmployeeID ?? 0);
                    string employeeName = employee?.FullName ?? "";

                    var changes = new List<string>();

                    var ruleDetailIds = oldDetails.Select(x => x.KPIEvaluationRuleDetailID)
                        .Union(request.lstKPIEmployeePointDetail.Select(x => (int?)x.ID))
                        .Where(id => id.HasValue)
                        .Select(id => id.Value)
                        .Distinct()
                        .ToList();
                    var ruleDetails = _kpiEvaluationRuleDetailRepo.GetAll(x => ruleDetailIds.Contains(x.ID)).ToList();

                    foreach (var item in request.lstKPIEmployeePointDetail)
                    {
                        var oldDetail = oldDetails.FirstOrDefault(x => x.KPIEvaluationRuleDetailID == item.ID);
                        var ruleDetail = ruleDetails.FirstOrDefault(x => x.ID == item.ID);
                        string ruleName = ruleDetail?.RuleContent ?? ruleDetail?.FormulaCode ?? $"ID {item.ID}";

                        var itemChanges = new List<string>();
                        if (oldDetail == null)
                        {
                            if (item.FirstMonth != null && item.FirstMonth != 0) itemChanges.Add($"T1: {item.FirstMonth}");
                            if (item.SecondMonth != null && item.SecondMonth != 0) itemChanges.Add($"T2: {item.SecondMonth}");
                            if (item.ThirdMonth != null && item.ThirdMonth != 0) itemChanges.Add($"T3: {item.ThirdMonth}");
                            if (item.PercentBonus != null && item.PercentBonus != 0) itemChanges.Add($"% Thưởng: {item.PercentBonus}");
                            if (item.PercentRemaining != null && item.PercentRemaining != 0) itemChanges.Add($"% Còn lại: {item.PercentRemaining}");
                            if (itemChanges.Count > 0)
                            {
                                changes.Add($"Thêm mới dòng [{ruleName}]: {string.Join(", ", itemChanges)}");
                            }
                        }
                        else
                        {
                            if ((oldDetail.FirstMonth ?? 0) != (item.FirstMonth ?? 0))
                                itemChanges.Add($"T1 ({oldDetail.FirstMonth ?? 0} -> {item.FirstMonth ?? 0})");
                            if ((oldDetail.SecondMonth ?? 0) != (item.SecondMonth ?? 0))
                                itemChanges.Add($"T2 ({oldDetail.SecondMonth ?? 0} -> {item.SecondMonth ?? 0})");
                            if ((oldDetail.ThirdMonth ?? 0) != (item.ThirdMonth ?? 0))
                                itemChanges.Add($"T3 ({oldDetail.ThirdMonth ?? 0} -> {item.ThirdMonth ?? 0})");
                            if ((oldDetail.PercentBonus ?? 0) != (item.PercentBonus ?? 0))
                                itemChanges.Add($"% Thưởng ({oldDetail.PercentBonus ?? 0} -> {item.PercentBonus ?? 0})");
                            if ((oldDetail.PercentRemaining ?? 0) != (item.PercentRemaining ?? 0))
                                itemChanges.Add($"% Còn lại ({oldDetail.PercentRemaining ?? 0} -> {item.PercentRemaining ?? 0})");

                            if (itemChanges.Count > 0)
                            {
                                changes.Add($"Dòng [{ruleName}]: {string.Join(", ", itemChanges)}");
                            }
                        }
                    }

                    // Check if totalPercent changed
                    if ((master.TotalPercent ?? 0) != request.totalPercentRemaining)
                    {
                        changes.Add($"Tổng % còn lại: {master.TotalPercent ?? 0} -> {request.totalPercentRemaining}");
                    }

                    string contentLog = $"{currentUser.FullName} đã lưu điểm KPI Rule của nhân viên: {employeeName}";
                    if (changes.Count > 0)
                    {
                        contentLog += "\nChi tiết thay đổi:\n" + string.Join("\n", changes);
                    }
                    else
                    {
                        contentLog += "\nKhông có thay đổi về điểm số.";
                    }

                    int? resolvedExamID = null;
                    var rule = _kpiEvaluationRuleRepo.GetByID(master?.KPIEvaluationRuleID ?? 0);
                    int? kpiSessionID = rule?.KPISessionID;
                    if (kpiSessionID.HasValue && kpiSessionID > 0)
                    {
                        var kpiPositions = _kpiPositionRepo.GetAll(x => x.KPISessionID == kpiSessionID && x.IsDeleted == false);
                        var kpiPositionEmployees = _kpiPositionEmployeeRepo.GetAll(x => x.EmployeeID == master.EmployeeID && x.IsDeleted == false);
                        var positionEmp = (from p in kpiPositions
                                           join pe in kpiPositionEmployees on p.ID equals pe.KPIPosiotionID
                                           select pe).FirstOrDefault();

                        int currentPositionID = positionEmp?.KPIPosiotionID ?? 1;

                        var employeeExam = (from exam in _kpiExamRepo.GetAll(x => x.KPISessionID == kpiSessionID && x.IsDeleted == false)
                                            join ep in _kpiExamPositionRepo.GetAll(x => x.IsDeleted == false) on exam.ID equals ep.KPIExamID
                                            where ep.KPIPositionID == currentPositionID
                                            select exam).FirstOrDefault();

                        resolvedExamID = employeeExam?.ID;
                        if (resolvedExamID == null)
                        {
                            var fallbackExam = _kpiExamRepo.GetAll(x => x.KPISessionID == kpiSessionID && x.IsDeleted != true).FirstOrDefault();
                            resolvedExamID = fallbackExam?.ID;
                        }
                    }

                    var log = new KPIEvaluationLog
                    {
                        KPIExamID = resolvedExamID,
                        EmployeeID = master?.EmployeeID,
                        ActionType = "Lưu điểm KPI Rule",
                        ContentLog = contentLog,
                        CreatedBy = currentUser.LoginName,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    await _kpiEvaluationLogRepo.CreateAsync(log);
                }
                catch (Exception) { }
                return Ok(ApiResponseFactory.Success(true, "Lưu dữ liệu rule thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion save Datarule

        #region save Data kpi

        [HttpPost("save-data-kpi")]
        public async Task<IActionResult> SaveDataKPI([FromBody] SaveDataKPIRequestParam request)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var changes = new List<string>();

                //validate
                if (request.KPISessionID == 0 || request.KPISessionID == null)
                { return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn kỳ đánh giá")); }
                if (request.KPIExamID == 0 || request.KPIExamID == null)
                { return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn bài đánh giá")); }
                if (request.employeeID == 0 || request.employeeID == null)
                { return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn nhân viên")); }

                // 1. Tải trước toàn bộ KPIEvaluationFactors liên quan đến các danh sách trong request
                var factorIds = (request.kpiKyNang ?? new List<KPIEvaluationPoint>()).Select(x => x.KPIEvaluationFactorsID)
                    .Union((request.kpiChuyenMon ?? new List<KPIEvaluationPoint>()).Select(x => x.KPIEvaluationFactorsID))
                    .Union((request.kpiChung ?? new List<KPIEvaluationPoint>()).Select(x => x.KPIEvaluationFactorsID))
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .Distinct()
                    .ToList();
                var factors = _kpiEvaluationFactorRepo.GetAll(x => factorIds.Contains(x.ID)).ToList();
                var factorsDict = factors.ToDictionary(x => x.ID);

                // 2. Tải trước toàn bộ KPIEvaluationPoint hiện có liên quan đến các ID trong request
                var pointIds = (request.kpiKyNang ?? new List<KPIEvaluationPoint>()).Select(x => x.ID)
                    .Union((request.kpiChuyenMon ?? new List<KPIEvaluationPoint>()).Select(x => x.ID))
                    .Union((request.kpiChung ?? new List<KPIEvaluationPoint>()).Select(x => x.ID))
                    .Where(id => id > 0)
                    .Distinct()
                    .ToList();
                var existingPoints = _kpiEvaluationPointRepo.GetAll(x => pointIds.Contains(x.ID)).ToList();
                var pointsDict = existingPoints.ToDictionary(x => x.ID);

                // 3. Tải trước toàn bộ KPISumaryEvaluation liên quan đến nhân viên và bài đánh giá
                var existingSummaries = _kpiSumaryEvaluationRepo.GetAll(x => x.EmployeeID == request.employeeID && x.KPIExamID == request.KPIExamID).ToList();

                var pointsToUpdate = new List<KPIEvaluationPoint>();
                var pointsToCreate = new List<KPIEvaluationPoint>();

                // lưu dữ liệu kpi kỹ năng
                // Vòng lặp 1: Xử lý kpiKyNang (tương ứng treeData)
                foreach (var item in request.kpiKyNang ?? new List<KPIEvaluationPoint>())
                {
                    if (item.ID < 0) continue;
                    pointsDict.TryGetValue(item.ID, out var model);
                    if (model == null)
                    {
                        model = new KPIEvaluationPoint();
                    }

                    factorsDict.TryGetValue(item.KPIEvaluationFactorsID ?? 0, out var factor);
                    string factorName = factor?.EvaluationContent ?? $"ID {item.KPIEvaluationFactorsID}";
                    var itemChanges = new List<string>();

                    if (request.typePoint == 1)
                    {
                        if (model.ID == 0 || (model.EmployeePoint ?? 0) != (item.EmployeePoint ?? 0))
                            itemChanges.Add($"Điểm ({model.EmployeePoint ?? 0} -> {item.EmployeePoint ?? 0})");
                        if (model.ID == 0 || (model.EmployeeEvaluation ?? 0) != (item.EmployeeEvaluation ?? 0))
                            itemChanges.Add($"Tự đánh giá ({model.EmployeeEvaluation ?? 0} -> {item.EmployeeEvaluation ?? 0})");
                    }
                    else if (request.typePoint == 2)
                    {
                        if (model.ID == 0 || (model.TBPPoint ?? 0) != (item.TBPPoint ?? 0))
                            itemChanges.Add($"Điểm TBP ({model.TBPPoint ?? 0} -> {item.TBPPoint ?? 0})");
                        if (model.ID == 0 || (model.TBPEvaluation ?? 0) != (item.TBPEvaluation ?? 0))
                            itemChanges.Add($"TBP đánh giá ({model.TBPEvaluation ?? 0} -> {item.TBPEvaluation ?? 0})");
                    }
                    else if (request.typePoint == 3)
                    {
                        if (model.ID == 0 || (model.BGDPoint ?? 0) != (item.BGDPoint ?? 0))
                            itemChanges.Add($"Điểm BGĐ ({model.BGDPoint ?? 0} -> {item.BGDPoint ?? 0})");
                        if (model.ID == 0 || (model.BGDEvaluation ?? 0) != (item.BGDEvaluation ?? 0))
                            itemChanges.Add($"BGĐ đánh giá ({model.BGDEvaluation ?? 0} -> {item.BGDEvaluation ?? 0})");
                    }

                    if (itemChanges.Count > 0)
                    {
                        changes.Add($"[Kỹ năng] {factorName}: {string.Join(", ", itemChanges)}");
                    }

                    model.EmployeeID = request.employeeID;
                    if (request.typePoint == 1)
                    {
                        if (!currentUser.IsAdmin && request.employeeID != 0 && request.employeeID != currentUser.EmployeeID)
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, "Bạn không thể đánh giá KPI của người khác!"));
                        }
                        model.EmployeePoint = item.EmployeePoint;
                        model.EmployeeEvaluation = item.EmployeeEvaluation;
                        model.Status = 0;
                    }
                    else if (request.typePoint == 2)
                    {
                        model.TBPPoint = item.TBPPoint; // Đã sửa từ item.EmployeePoint
                        model.TBPID = currentUser.EmployeeID;
                        model.TBPEvaluation = item.TBPEvaluation;
                        // dành cho cơ khí ( lưu điểm tbp = bgđ )
                        if (request.departmentID == 10) model.BGDEvaluation = item.TBPEvaluation;
                    }
                    else if (request.typePoint == 3)
                    {
                        model.BGDPoint = item.BGDPoint;
                        model.BGDID = currentUser.EmployeeID;
                        model.BGDEvaluation = item.BGDEvaluation;
                    }

                    model.EmployeeCoefficient = item.EmployeeCoefficient;
                    model.TBPCoefficient = item.TBPCoefficient;
                    model.BGDCoefficient = item.BGDCoefficient;

                    model.KPIEvaluationFactorsID = item.KPIEvaluationFactorsID;

                    model.TBPPointInput = item.TBPPointInput; // Lấy từ item của request
                    model.BGDPointInput = item.BGDPointInput; // Lấy từ item của request
                    
                    if (model.ID > 0)
                    {
                        if (!pointsToUpdate.Contains(model)) pointsToUpdate.Add(model);
                    }
                    else
                    {
                        if (!pointsToCreate.Contains(model)) pointsToCreate.Add(model);
                    }
                }

                // Vòng lặp 2: Xử lý kpichuyenmon (tương ứng treeData2)
                foreach (var item in request.kpiChuyenMon ?? new List<KPIEvaluationPoint>())
                {
                    if (item.ID < 0) continue;
                    pointsDict.TryGetValue(item.ID, out var model);
                    if (model == null)
                    {
                        model = new KPIEvaluationPoint();
                    }

                    factorsDict.TryGetValue(item.KPIEvaluationFactorsID ?? 0, out var factor);
                    string factorName = factor?.EvaluationContent ?? $"ID {item.KPIEvaluationFactorsID}";
                    var itemChanges = new List<string>();

                    if (request.typePoint == 1)
                    {
                        if (model.ID == 0 || (model.EmployeePoint ?? 0) != (item.EmployeePoint ?? 0))
                            itemChanges.Add($"Điểm ({model.EmployeePoint ?? 0} -> {item.EmployeePoint ?? 0})");
                        if (model.ID == 0 || (model.EmployeeEvaluation ?? 0) != (item.EmployeeEvaluation ?? 0))
                            itemChanges.Add($"Tự đánh giá ({model.EmployeeEvaluation ?? 0} -> {item.EmployeeEvaluation ?? 0})");
                    }
                    else if (request.typePoint == 2)
                    {
                        if (model.ID == 0 || (model.TBPPoint ?? 0) != (item.TBPPoint ?? 0))
                            itemChanges.Add($"Điểm TBP ({model.TBPPoint ?? 0} -> {item.TBPPoint ?? 0})");
                        if (model.ID == 0 || (model.TBPEvaluation ?? 0) != (item.TBPEvaluation ?? 0))
                            itemChanges.Add($"TBP đánh giá ({model.TBPEvaluation ?? 0} -> {item.TBPEvaluation ?? 0})");
                    }
                    else if (request.typePoint == 3)
                    {
                        if (model.ID == 0 || (model.BGDPoint ?? 0) != (item.BGDPoint ?? 0))
                            itemChanges.Add($"Điểm BGĐ ({model.BGDPoint ?? 0} -> {item.BGDPoint ?? 0})");
                        if (model.ID == 0 || (model.BGDEvaluation ?? 0) != (item.BGDEvaluation ?? 0))
                            itemChanges.Add($"BGĐ đánh giá ({model.BGDEvaluation ?? 0} -> {item.BGDEvaluation ?? 0})");
                    }

                    if (itemChanges.Count > 0)
                    {
                        changes.Add($"[Chuyên môn] {factorName}: {string.Join(", ", itemChanges)}");
                    }

                    model.EmployeeID = request.employeeID;
                    if (request.typePoint == 1)
                    {
                        // Không có kiểm tra quyền như vòng lặp 1
                        model.EmployeeEvaluation = item.EmployeeEvaluation;
                        model.EmployeePoint = item.EmployeePoint;
                        model.Status = 0;
                    }
                    else if (request.typePoint == 2)
                    {
                        model.TBPPoint = item.TBPPoint;
                        model.TBPID = currentUser.EmployeeID;
                        model.TBPEvaluation = item.TBPEvaluation;
                        // Dựa trên phân tích, lỗi tiềm ẩn: WinForms dùng treeData.GetRowCellValue(item, colTBPEvaluation)
                        // Giả định ý định là dùng item từ kpiKyNang2
                        if (request.departmentID == 10) model.BGDEvaluation = item.TBPEvaluation;
                    }
                    else if (request.typePoint == 3)
                    {
                        model.BGDPoint = item.BGDPoint;
                        model.BGDID = currentUser.EmployeeID;
                        model.BGDEvaluation = item.BGDEvaluation;
                    }
                    model.EmployeeCoefficient = item.EmployeeCoefficient;
                    model.TBPCoefficient = item.TBPCoefficient;
                    model.BGDCoefficient = item.BGDCoefficient;

                    model.KPIEvaluationFactorsID = item.KPIEvaluationFactorsID;

                    // Dựa trên phân tích, lỗi tiềm ẩn: WinForms dùng treeData.GetRowCellValue(item, colTBPPointInput)
                    // Giả định ý định là dùng item từ kpiKyNang2
                    model.TBPPointInput = item.TBPPointInput;
                    model.BGDPointInput = item.BGDPointInput;

                    if (model.ID > 0)
                    {
                        if (!pointsToUpdate.Contains(model)) pointsToUpdate.Add(model);
                    }
                    else
                    {
                        if (!pointsToCreate.Contains(model)) pointsToCreate.Add(model);
                    }
                }

                // Vòng lặp 3: Xử lý kpichung (tương ứng treeData3)
                foreach (var item in request.kpiChung ?? new List<KPIEvaluationPoint>())
                {
                    if (item.ID < 0) continue;
                    pointsDict.TryGetValue(item.ID, out var model);
                    if (model == null)
                    {
                        model = new KPIEvaluationPoint();
                    }

                    factorsDict.TryGetValue(item.KPIEvaluationFactorsID ?? 0, out var factor);
                    string factorName = factor?.EvaluationContent ?? $"ID {item.KPIEvaluationFactorsID}";
                    var itemChanges = new List<string>();

                    if (request.typePoint == 1)
                    {
                        if (model.ID == 0 || (model.EmployeePoint ?? 0) != (item.EmployeePoint ?? 0))
                            itemChanges.Add($"Điểm ({model.EmployeePoint ?? 0} -> {item.EmployeePoint ?? 0})");
                        if (model.ID == 0 || (model.EmployeeEvaluation ?? 0) != (item.EmployeeEvaluation ?? 0))
                            itemChanges.Add($"Tự đánh giá ({model.EmployeeEvaluation ?? 0} -> {item.EmployeeEvaluation ?? 0})");
                    }
                    else if (request.typePoint == 2)
                    {
                        if (model.ID == 0 || (model.TBPPoint ?? 0) != (item.TBPPoint ?? 0))
                            itemChanges.Add($"Điểm TBP ({model.TBPPoint ?? 0} -> {item.TBPPoint ?? 0})");
                        if (model.ID == 0 || (model.TBPEvaluation ?? 0) != (item.TBPEvaluation ?? 0))
                            itemChanges.Add($"TBP đánh giá ({model.TBPEvaluation ?? 0} -> {item.TBPEvaluation ?? 0})");
                    }
                    else if (request.typePoint == 3)
                    {
                        if (model.ID == 0 || (model.BGDPoint ?? 0) != (item.BGDPoint ?? 0))
                            itemChanges.Add($"Điểm BGĐ ({model.BGDPoint ?? 0} -> {item.BGDPoint ?? 0})");
                        if (model.ID == 0 || (model.BGDEvaluation ?? 0) != (item.BGDEvaluation ?? 0))
                            itemChanges.Add($"BGĐ đánh giá ({model.BGDEvaluation ?? 0} -> {item.BGDEvaluation ?? 0})");
                    }

                    if (itemChanges.Count > 0)
                    {
                        changes.Add($"[Chung] {factorName}: {string.Join(", ", itemChanges)}");
                    }

                    model.EmployeeID = request.employeeID;
                    if (request.typePoint == 1)
                    {
                        // Không có kiểm tra quyền như vòng lặp 1
                        model.EmployeeEvaluation = item.EmployeeEvaluation;
                        model.EmployeePoint = item.EmployeePoint;
                        model.Status = 0;
                    }
                    else if (request.typePoint == 2)
                    {
                        model.TBPPoint = item.TBPPoint;
                        model.TBPID = currentUser.EmployeeID;
                        model.TBPEvaluation = item.TBPEvaluation;
                        // Không có logic đặc biệt cho BGDEvaluation như vòng lặp 1 và 2
                    }
                    else if (request.typePoint == 3)
                    {
                        model.BGDPoint = item.BGDPoint;
                        model.BGDID = currentUser.EmployeeID;
                        model.BGDEvaluation = item.BGDEvaluation;
                    }

                    model.EmployeeCoefficient = item.EmployeeCoefficient;
                    model.TBPCoefficient = item.TBPCoefficient;
                    model.BGDCoefficient = item.BGDCoefficient;

                    model.KPIEvaluationFactorsID = item.KPIEvaluationFactorsID;

                    model.TBPPointInput = item.TBPPointInput;
                    model.BGDPointInput = item.BGDPointInput;
                    
                    if (model.ID > 0)
                    {
                        if (!pointsToUpdate.Contains(model)) pointsToUpdate.Add(model);
                    }
                    else
                    {
                        if (!pointsToCreate.Contains(model)) pointsToCreate.Add(model);
                    }
                }

                if (pointsToUpdate.Count > 0)
                {
                    await _kpiEvaluationPointRepo.UpdateRangeWithNullAsync(pointsToUpdate);
                }
                if (pointsToCreate.Count > 0)
                {
                    await _kpiEvaluationPointRepo.CreateRangeAsync(pointsToCreate);
                }

                var summariesToUpdate = new List<KPISumaryEvaluation>();
                var summariesToCreate = new List<KPISumaryEvaluation>();

                // Lưu thông tin tổng hợp đánh giá
                foreach (var item in request.kpiSumaryEvaluation ?? new List<KPISumaryEvaluation>())
                {
                    KPISumaryEvaluation sumaryModel = existingSummaries.FirstOrDefault(x => x.SpecializationType == item.SpecializationType) ?? new KPISumaryEvaluation();

                    var specType = item.SpecializationType;
                    string specName = specType == 1 ? "Kỹ năng" : specType == 2 ? "PLC, Robot" : specType == 3 ? "Vision" : specType == 4 ? "Software" : $"Loại {specType}";
                    var sumChanges = new List<string>();

                    if (sumaryModel.ID == 0)
                    {
                        if (item.EmployeePoint != null && item.EmployeePoint != 0) sumChanges.Add($"Điểm NV: {item.EmployeePoint}");
                        if (item.TBPPoint != null && item.TBPPoint != 0) sumChanges.Add($"Điểm TBP: {item.TBPPoint}");
                        if (item.BGDPoint != null && item.BGDPoint != 0) sumChanges.Add($"Điểm BGĐ: {item.BGDPoint}");
                    }
                    else
                    {
                        if ((sumaryModel.EmployeePoint ?? 0) != (item.EmployeePoint ?? 0))
                            sumChanges.Add($"Điểm NV ({sumaryModel.EmployeePoint ?? 0} -> {item.EmployeePoint ?? 0})");
                        if ((sumaryModel.TBPPoint ?? 0) != (item.TBPPoint ?? 0))
                            sumChanges.Add($"Điểm TBP ({sumaryModel.TBPPoint ?? 0} -> {item.TBPPoint ?? 0})");
                        if ((sumaryModel.BGDPoint ?? 0) != (item.BGDPoint ?? 0))
                            sumChanges.Add($"Điểm BGĐ ({sumaryModel.BGDPoint ?? 0} -> {item.BGDPoint ?? 0})");
                    }

                    if (sumChanges.Count > 0)
                    {
                        changes.Add($"[Tổng hợp] {specName}: {string.Join(", ", sumChanges)}");
                    }

                    sumaryModel.SpecializationType = item.SpecializationType;
                    sumaryModel.EmployeeID = request.employeeID;
                    sumaryModel.KPIExamID = request.KPIExamID;
                    sumaryModel.EmployeePoint = item.EmployeePoint;
                    sumaryModel.TBPPoint = item.TBPPoint;
                    sumaryModel.BGDPoint = item.BGDPoint;
                    
                    if (sumaryModel.ID > 0)
                    {
                        summariesToUpdate.Add(sumaryModel);
                    }
                    else
                    {
                        summariesToCreate.Add(sumaryModel);
                    }
                }

                if (summariesToUpdate.Count > 0)
                {
                    await _kpiSumaryEvaluationRepo.UpdateRangeAsync_Binh(summariesToUpdate);
                }
                if (summariesToCreate.Count > 0)
                {
                    await _kpiSumaryEvaluationRepo.CreateRangeAsync(summariesToCreate);
                }
                try
                {
                    var employee = _employeeRepo.GetByID(request.employeeID);
                    string employeeName = employee?.FullName ?? "";

                    string actionType = "Lưu điểm KPI";
                    string contentLog = $"{currentUser.FullName} đã lưu điểm KPI của nhân viên: {employeeName}";

                    if (request.typePoint == 1)
                    {
                        actionType = "NV tự đánh giá";
                        contentLog = $"{currentUser.FullName} đã tự đánh giá điểm KPI";
                    }
                    else if (request.typePoint == 2)
                    {
                        actionType = "TBP đánh giá";
                        contentLog = $"{currentUser.FullName} đã đánh giá điểm KPI cho nhân viên: {employeeName}";
                    }
                    else if (request.typePoint == 3)
                    {
                        actionType = "BGĐ đánh giá";
                        contentLog = $"{currentUser.FullName} đã đánh giá điểm KPI cho nhân viên: {employeeName}";
                    }

                    if (changes.Count > 0)
                    {
                        contentLog += "\nChi tiết thay đổi:\n" + string.Join("\n", changes);
                    }
                    else
                    {
                        contentLog += "\nKhông có thay đổi về điểm số.";
                    }

                    var log = new KPIEvaluationLog
                    {
                        KPIExamID = request.KPIExamID,
                        EmployeeID = request.employeeID,
                        ActionType = actionType,
                        ContentLog = contentLog,
                        CreatedBy = currentUser.LoginName,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    await _kpiEvaluationLogRepo.CreateAsync(log);
                }
                catch (Exception) { }
                return Ok(ApiResponseFactory.Success(true, "Lưu dữ liệu KPI thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion save Data kpi

        #region lấy ispublic

        [HttpGet("get-ispublic")]
        public async Task<IActionResult> GetIsPublic(int empPointID)
        {
            try
            {
                KPIEmployeePoint empPoint = _kpiEmployeePointRepo.GetByID(empPointID);
                return Ok(ApiResponseFactory.Success(empPoint, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion lấy ispublic

        #region Lấy dữ liệu training cho THUONG02 (tham gia training) và THUONG03 (tổ chức training)

        /// <summary>
        /// Gọi spGetCourseTraining để lấy số liệu 3 tháng cho THUONG02 và THUONG03.
        /// - ResultSet[0] → Raw training data (bỏ qua)
        /// - ResultSet[1] → THUONG03 (Tổ chức training)
        /// - ResultSet[2] → THUONG02 (Tích cực tham gia training)
        /// </summary>
        [HttpGet("get-course-training")]
        public async Task<IActionResult> GetCourseTraining(int year, int quarter, int employeeID)
        {
            try
            {
                // spGetCourseTraining trả về 3 result sets:
                // ResultSet[0] → Raw training data (bỏ qua)
                // ResultSet[1] → THUONG03 (Tổ chức training)
                // ResultSet[2] → THUONG02 (Tích cực tham gia training)
                var (rawList, thuong03List, thuong02List) = await SqlDapper<KPISumarizeDTO>.QueryMultipleAsync<dynamic, KPISumarizeDTO, KPISumarizeDTO>(
                    "spGetCourseTraining",
                    new { Year = year, Quarter = quarter, EmployeeID = employeeID });

                var thuong03Data = thuong03List.FirstOrDefault() ?? new KPISumarizeDTO();
                var thuong02Data = thuong02List.FirstOrDefault() ?? new KPISumarizeDTO();

                var result = new
                {
                    THUONG02 = new { thuong02Data.FirstMonth, thuong02Data.SecondMonth, thuong02Data.ThirdMonth },
                    THUONG03 = new { thuong03Data.FirstMonth, thuong03Data.SecondMonth, thuong03Data.ThirdMonth }
                };

                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu training thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion Lấy dữ liệu training cho THUONG02 (tham gia training) và THUONG03 (tổ chức training)

        #region update điểm row kpi rule

        [HttpGet("update-row-rule")]
        public async Task<IActionResult> UpdateRowRule(int kpiExamID, bool isAmdinConfirm, int employeeID, int sessionID)
        {
            try
            {
                //Get possition của nhân viên
                List<KPIPosition> kpiPositions = _kpiPositionRepo.GetAll(x => x.KPISessionID == sessionID && x.IsDeleted == false);
                List<KPIPositionEmployee> kpiPositionEmployees = _kpiPositionEmployeeRepo.GetAll(x => x.EmployeeID == employeeID && x.IsDeleted == false);

                var empPosition = (from p in kpiPositions
                                   join pe in kpiPositionEmployees on p.ID equals pe.KPIPosiotionID
                                   select pe)

                     .FirstOrDefault() ?? new KPIPositionEmployee();

                KPIEvaluationRule rule = _kpiEvaluationRuleRepo.GetAll(x => x.KPISessionID == sessionID && x.KPIPositionID == (empPosition.KPIPosiotionID > 0 ? empPosition.KPIPosiotionID : 1) && x.IsDeleted == false)
                    .FirstOrDefault() ?? new KPIEvaluationRule(); // 1 là kỹ thuật

                int empPointId = await GetKPIEmployeePointID(rule.ID, employeeID);

                var param = new
                {
                    KPIEmployeePointID = empPointId
                };
                List<KPISumarizeDTO> lstResult = await SqlDapper<KPISumarizeDTO>.ProcedureToListTAsync("spGetSumarizebyKPIEmpPointIDNew_TNB", param);
                //var data1 = await SqlDapper<object>.ProcedureToListAsync("spGetKpiRuleSumarizeTeamNew_TNB", param);
                return Ok(ApiResponseFactory.Success(lstResult, "Cập nhật dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion update điểm row kpi rule

        #region LoadPointRuleNew

        [HttpGet("load-point-rule-new-detail")]
        public async Task<IActionResult> LoadPointRuleNew(int kpiExamID, bool isAmdinConfirm, int employeeID, int sessionID)
        {
            try
            {
                //Get possition của nhân viên
                List<KPIPosition> kpiPositions = _kpiPositionRepo.GetAll(x => x.KPISessionID == sessionID && x.IsDeleted == false);
                List<KPIPositionEmployee> kpiPositionEmployees = _kpiPositionEmployeeRepo.GetAll(x => x.EmployeeID == employeeID && x.IsDeleted == false);

                var empPosition = (from p in kpiPositions
                                   join pe in kpiPositionEmployees on p.ID equals pe.KPIPosiotionID
                                   select pe)

                     .FirstOrDefault() ?? new KPIPositionEmployee();

                KPIEvaluationRule rule = _kpiEvaluationRuleRepo.GetAll(x => x.KPISessionID == sessionID && x.KPIPositionID == (empPosition.KPIPosiotionID > 0 ? empPosition.KPIPosiotionID : 1) && x.IsDeleted == false)
                    .FirstOrDefault() ?? new KPIEvaluationRule(); // 1 là kỹ thuật

                int empPointId = await GetKPIEmployeePointID(rule.ID, employeeID);
                var param = new
                {
                    KPIEmployeePointID = empPointId,
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetSumarizebyKPIEmpPointIDNew_TNB", param);
                //var data = SQLHelper<object>.ProcedureToList("spGetEmployeeRulePointByKPIEmpPointIDNew_TNB"
                //  , new string[] { "@KPIEmployeePointID", "@IsPublic" }
                //  , new object[] { kpiEmployeePointID, isPublic });
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion LoadPointRuleNew

        #region LẤY USER TEAM

        [HttpGet("get-team")]
        public async Task<IActionResult> GetUserTeam()
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var data = _userTeamRepo.GetAll(x => x.LeaderID == currentUser.EmployeeID).FirstOrDefault();
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion LẤY USER TEAM

        #region BATCH RECALCULATE KPI

        [HttpPost("batch-recalculate-kpi")]
        public async Task<IActionResult> BatchRecalculateKPI([FromBody] RecalcKPIBatchRequestParam request)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var employeeLogs = new List<KPIEvaluationLog>();

                // 1. Lấy thông tin bài đánh giá (KPIExam) theo phong cách hiện tại
                var kpiExam = _kpiExamRepo.GetAll(x => x.KPISessionID == request.KpiSessionID && x.IsDeleted == false).FirstOrDefault();
                if (kpiExam == null) return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy bài đánh giá cho kỳ này."));

                // 2. Lấy danh sách nhân viên theo bộ lọc (Phòng ban/Team)
                // Tham khảo logic lấy nhân viên tương tự như các API hiện có

                var employees = await SqlDapper<sp_GetEmployeeByDepartmentAndTeamDTO>.ProcedureToListTAsync("sp_GetEmployeeByDepartmentAndTeam", new
                {
                    DepartmentID = request.DepartmentID,
                    TeamID = request.TeamID,
                    KPISessionID = request.KpiSessionID,
                });

                // 3. Cache danh sách Position để tối ưu hóa vòng lặp
                List<KPIPosition> kpiPositions = _kpiPositionRepo.GetAll(x => x.KPISessionID == request.KpiSessionID && x.IsDeleted == false);
                int updatedCount = 0;
                int skippedCount = 0;

                foreach (var emp in employees)
                {
                    if (emp.EmployeeID == 0) continue;
                    // 4. Tìm Rule của nhân viên (Tham khảo logic L548-L559)
                    List<KPIPositionEmployee> kpiPositionEmployees = _kpiPositionEmployeeRepo.GetAll(x => x.EmployeeID == emp.EmployeeID && x.IsDeleted == false);

                    var empPosition = (from p in kpiPositions
                                       join pe in kpiPositionEmployees on p.ID equals pe.KPIPosiotionID
                                       select pe).FirstOrDefault() ?? new KPIPositionEmployee();

                    KPIEvaluationRule rule = _kpiEvaluationRuleRepo.GetAll(x => x.KPISessionID == request.KpiSessionID
                                                                          && x.KPIPositionID == (empPosition.KPIPosiotionID > 0 ? empPosition.KPIPosiotionID : 1)
                                                                          && x.IsDeleted == false)
                                                                   .FirstOrDefault() ?? new KPIEvaluationRule();
                    //return Ok(ApiResponseFactory.Success(new { updatedCount, skippedCount },$"Đã cập nhật thành công {updatedCount} nhân viên. Bỏ qua {skippedCount} nhân viên đã chốt."));

                    // 5. Lấy hoặc Tạo mới KPIEmployeePointID (Sử dụng helper của bạn tại L256)
                    int empPointId = await GetKPIEmployeePointID(rule.ID, emp.EmployeeID ?? 0);
                    if (empPointId <= 0) continue;

                    // Kiểm tra bảo vệ kỳ đã chốt
                    var masterPoint = _kpiEmployeePointRepo.GetByID(empPointId);
                    //if (masterPoint != null && masterPoint.IsPublish == true)
                    //{
                    //    skippedCount++;
                    //    continue;
                    //}

                    // 6. Xử lý KPI Factor (Chuyên môn, Kỹ năng, Chung)
                    int currentPositionID = empPosition.KPIPosiotionID > 0 ? empPosition.KPIPosiotionID.Value : 1;

                    // Tìm bài đánh giá (KPIExam) tương ứng với Chức vụ của nhân viên trong kỳ đánh giá
                    var employeeExam = (from exam in _kpiExamRepo.GetAll(x => x.KPISessionID == request.KpiSessionID && x.IsDeleted == false)
                                        join ep in _kpiExamPositionRepo.GetAll(x => x.IsDeleted == false) on exam.ID equals ep.KPIExamID
                                        where ep.KPIPositionID == currentPositionID
                                        select exam).FirstOrDefault();

                    int examID = employeeExam?.ID ?? kpiExam.ID;

                    var factorScores = await SqlDapper<KPISumaryEvaluation>.ProcedureToListTAsync("spCalcAndSyncKPIFactorScore_TNB", new
                    {
                        KPIExamID = examID,
                        EmployeeID = emp.EmployeeID
                    });

                    foreach (var score in factorScores)
                    {
                        var existingSummary = _kpiSumaryEvaluationRepo.GetAll(x => x.EmployeeID == emp.EmployeeID
                                                                                && x.KPIExamID == examID
                                                                                && x.SpecializationType == score.SpecializationType)
                                                                       .FirstOrDefault();
                        if (existingSummary != null)
                        {
                            existingSummary.EmployeePoint = score.EmployeePoint;
                            existingSummary.TBPPoint = score.TBPPoint ?? 0;
                            existingSummary.BGDPoint = score.BGDPoint ?? 0;
                            await _kpiSumaryEvaluationRepo.UpdateAsync(existingSummary);
                        }
                        else
                        {
                            score.EmployeeID = emp.EmployeeID;
                            score.KPIExamID = examID;
                            score.SpecializationType = score.SpecializationType;
                            score.EmployeePoint = score.EmployeePoint;
                            score.TBPPoint = score.TBPPoint ?? 0;
                            score.BGDPoint = score.BGDPoint ?? 0;
                            await _kpiSumaryEvaluationRepo.CreateAsync(score);
                        }
                    }

                    // 7. Xử lý KPI Rule (Lỗi)
                    var ruleDetailsCalculated = await SqlDapper<KPIEmployeePointDetail>.ProcedureToListTAsync("spCalcAndSyncKPIRuleScore_TNB", new
                    {
                        KPIEmployeePointID = empPointId
                    });

                    decimal totalPercent = 0;
                    foreach (var detail in ruleDetailsCalculated)
                    {
                        var dbDetail = _kpiEmployeePointDetailRepo.GetByID(detail.ID);
                        if (dbDetail != null && dbDetail.KPIEmployeePointID != 0 && dbDetail.KPIEmployeePointID != null)
                        {
                            dbDetail.PercentBonus = detail.PercentBonus;
                            dbDetail.PercentRemaining = detail.PercentRemaining;
                            await _kpiEmployeePointDetailRepo.UpdateAsync(dbDetail);
                            totalPercent += (detail.PercentRemaining ?? 0);
                        }
                    }

                    // Cập nhật điểm tổng Master
                    if (masterPoint != null)
                    {
                        masterPoint.TotalPercent = totalPercent;
                        masterPoint.UpdatedDate = DateTime.Now;
                        await _kpiEmployeePointRepo.UpdateAsync(masterPoint);
                    }

                    employeeLogs.Add(new KPIEvaluationLog
                    {
                        KPIExamID = examID,
                        EmployeeID = emp.EmployeeID,
                        ActionType = "Cập nhập điểm KPI",
                        ContentLog = $"{currentUser.FullName} đã cập nhật điểm KPI cho bạn",
                        CreatedBy = currentUser.LoginName,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    });

                    updatedCount++;
                }

                try
                {
                    var teamName = _kPIEmployeeTeamRepo.GetByID(request.TeamID).Name;
                    var kpiSessionName = _kpiSessionRepo.GetByID(request.KpiSessionID).Name;
                    var log = new KPIEvaluationLog
                    {
                        KPIExamID = null,
                        EmployeeID = null,
                        ActionType = "Cập nhập điểm KPI",
                        ContentLog = $"{currentUser.FullName} đã cập nhật điểm KPI của team {teamName} trong kỳ đánh giá {kpiSessionName}",
                        CreatedBy = currentUser.LoginName,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    await _kpiEvaluationLogRepo.CreateAsync(log);

                    if (employeeLogs.Any())
                    {
                        await _kpiEvaluationLogRepo.CreateRangeAsync(employeeLogs);
                    }
                }
                catch (Exception) { }

                return Ok(ApiResponseFactory.Success(new { updatedCount, skippedCount },
                    $"Đã cập nhật thành công {updatedCount} nhân viên."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion BATCH RECALCULATE KPI
    }
}