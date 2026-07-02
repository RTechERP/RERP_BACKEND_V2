using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.KPITech;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.KPITech;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Technical.KPI;
using System.Data;

// --- QUAN TRỌNG: THÊM CÁC DÒNG NÀY ---

namespace RERPAPI.Controllers.KPITechnical
{
    [Route("api/[controller]")]
    [ApiController]
    public class KPIEvaluationFactorScoringController : ControllerBase
    {
        private KPIEvaluationPointRepo _kpiEvaluationPointRepo;
        private KPISessionRepo _kpiSessionRepo;
        private KPIEmployeePointRepo _kpiEmployeePointRepo;
        private KPIPositionRepo _kpiPositionRepo;
        private KPIPositionEmployeeRepo _kpiPositionEmployeeRepo;
        private KPIEvaluationRuleRepo _kpiEvaluationRuleRepo;
        private KPIEmployeePointDetailRepo _kpiEmployeePointDetailRepo;
        private KPIExamRepo _kpiExamRepo;
        private KPIEvaluationLogRepo _kpiEvaluationLogRepo;
        private EmployeeRepo _employeeRepo;
        private KPIEvaluationRuleDetailRepo _kpiEvaluationRuleDetailRepo;
        private KPIEvaluationFactorRepo _kpiEvaluationFactorRepo;
        private KPIExamPositionRepo _kpiExamPositionRepo;

        public KPIEvaluationFactorScoringController(
            KPIEvaluationPointRepo kpiEvaluationPointRepo,
            KPISessionRepo kpiSessionRepo,
            KPIEmployeePointRepo kpiEmployeePointRepo,
            KPIPositionRepo kpiPositionRepo,
            KPIPositionEmployeeRepo kpiPositionEmployeeRepo,
            KPIEvaluationRuleRepo kpiEvaluationRuleRepo,
            KPIEmployeePointDetailRepo kPIEmployeePointDetailRepo,
            KPIExamRepo kpiExamRepo,
            KPIEvaluationLogRepo kpiEvaluationLogRepo,
            EmployeeRepo employeeRepo,
            KPIEvaluationRuleDetailRepo kpiEvaluationRuleDetailRepo,
            KPIEvaluationFactorRepo kpiEvaluationFactorRepo,
            KPIExamPositionRepo kpiExamPositionRepo)
        {
            _kpiEvaluationPointRepo = kpiEvaluationPointRepo;
            _kpiSessionRepo = kpiSessionRepo;
            _kpiEmployeePointRepo = kpiEmployeePointRepo;
            _kpiPositionRepo = kpiPositionRepo;
            _kpiPositionEmployeeRepo = kpiPositionEmployeeRepo;
            _kpiEvaluationRuleRepo = kpiEvaluationRuleRepo;
            _kpiEmployeePointDetailRepo = kPIEmployeePointDetailRepo;
            _kpiExamRepo = kpiExamRepo;
            _kpiEvaluationLogRepo = kpiEvaluationLogRepo;
            _employeeRepo = employeeRepo;
            _kpiEvaluationRuleDetailRepo = kpiEvaluationRuleDetailRepo;
            _kpiEvaluationFactorRepo = kpiEvaluationFactorRepo;
            _kpiExamPositionRepo = kpiExamPositionRepo;
        }

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
                List<spGetKpiRuleSumarizeTeamNewResultDTO> data1 = await SqlDapper<spGetKpiRuleSumarizeTeamNewResultDTO>.ProcedureToListTAsync("spGetKpiRuleSumarizeTeamNew_TNB", param);

                //var data1 = SQLHelper<object>.ProcedureToList("spGetKpiRuleSumarizeTeamNew_TNB"
                // , new string[] { "@KPIEmployeePointID" }
                // , new object[] { empPointId });

                var param2 = new
                {
                    KPIEmployeePointID = empPointId,
                    IsPublic = 1
                };
                var data2 = await SqlDapper<spGetEmployeeRulePointByKPIEmpPointIDNewResultDTO>.ProcedureToListTAsync("spGetEmployeeRulePointByKPIEmpPointIDNew_TNB", param2);
                //var data2 = SQLHelper<object>.ProcedureToList("spGetEmployeeRulePointByKPIEmpPointIDNew_TNB"
                //  , new string[] { "@KPIEmployeePointID", "@IsPublic" }
                //  , new object[] { empPointId, 1 });

                var dtTeam = data1;
                var dtKpiRule = data2;

                List<KPIEmployeePointDetail> lst = _kpiEmployeePointDetailRepo.GetAll(x => x.KPIEmployeePointID == empPointId);

                return Ok(ApiResponseFactory.Success(new { dtTeam, dtKpiRule, lst, empPointID = empPointId }, "Lấy dữ liệu thành công"));
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

        #region action duyệt/hủy

        [HttpPost("check-update-status-kpi")]
        public async Task<IActionResult> CheckUpdateStatusKPI(int status, int kpiExamID, int empID)
        {
            try
            {
                if (kpiExamID <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn bài đánh giá!"));
                }
                var param = new
                {
                    KPIExamID = kpiExamID,
                    EmployeeID = empID
                };
                List<KPIEvaluationPoint> lst = await SqlDapper<KPIEvaluationPoint>.ProcedureToListTAsync("spGetKPIEvaluationPoint", param);
                //List<KPIEvaluationPoint> lst = SQLHelper<KPIEvaluationPoint>.ProcedureToListModel("spGetKPIEvaluationPoint", new string[] { "@KPIExamID", "@EmployeeID" }, new object[] { kpiExamID, empID });
                if (lst.Count <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng Đánh giá KPI trước khi hoàn thành!"));
                }
                return Ok(ApiResponseFactory.Success(null, "check dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("update-status-kpi")]
        public async Task<IActionResult> UpdateStatusKPI(int status, int kpiExamID, int empID)
        {
            var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
            var currentUser = ObjectMapper.GetCurrentUser(claims);
            try
            {
                int[] statusCancel = new int[] { 0, 4, 5 };
                string statusText = statusCancel.Contains(status) ? "Hủy" : "Hoàn thành";
                var param = new
                {
                    KPIExamID = kpiExamID,
                    EmployeeID = empID
                };
                List<KPIEvaluationPoint> lst = await SqlDapper<KPIEvaluationPoint>.ProcedureToListTAsync("spGetKPIEvaluationPoint", param);
                //List<KPIEvaluationPoint> lst = SQLHelper<KPIEvaluationPoint>.ProcedureToListModel("spGetKPIEvaluationPoint", new string[] { "@KPIExamID", "@EmployeeID" }, new object[] { kpiExamID, empID });
                foreach (KPIEvaluationPoint item in lst)
                {
                    item.Status = status;
                    if (status == 2)
                    {
                        item.DateTBPConfirm = DateTime.Now;
                    }
                    else
                    {
                        item.DateBGDConfirm = DateTime.Now;
                    }
                    await _kpiEvaluationPointRepo.UpdateAsync(item);
                }

                // Cập nhật trạng thái cho KPIEmployeePoint master
                KPIExam kpiExam = _kpiExamRepo.GetByID(kpiExamID);
                if (kpiExam != null)
                {
                    int sessionID = kpiExam.KPISessionID ?? 0;
                    List<KPIPosition> kpiPositions = _kpiPositionRepo.GetAll(x => x.KPISessionID == sessionID && x.IsDeleted == false);
                    List<KPIPositionEmployee> kpiPositionEmployees = _kpiPositionEmployeeRepo.GetAll(x => x.EmployeeID == empID && x.IsDeleted == false);

                    var empPosition = (from p in kpiPositions
                                       join pe in kpiPositionEmployees on p.ID equals pe.KPIPosiotionID
                                       select pe)
                                       .FirstOrDefault() ?? new KPIPositionEmployee();

                    KPIEvaluationRule rule = _kpiEvaluationRuleRepo.GetAll(x => x.KPISessionID == sessionID && x.KPIPositionID == (empPosition.KPIPosiotionID > 0 ? empPosition.KPIPosiotionID : 1) && x.IsDeleted == false)
                        .FirstOrDefault() ?? new KPIEvaluationRule();

                    int empPointId = await GetKPIEmployeePointID(rule.ID, empID);
                    KPIEmployeePoint empPoint = _kpiEmployeePointRepo.GetByID(empPointId);
                    if (empPoint != null)
                    {
                        empPoint.Status = status;
                        await _kpiEmployeePointRepo.UpdateAsync(empPoint);
                    }
                }

                try
                {
                    var employee = _employeeRepo.GetByID(empID);
                    string employeeName = employee?.FullName ?? "";

                    string actionType = "Cập nhật trạng thái";
                    string contentLog = $"{currentUser.FullName} đã cập nhật trạng thái {status} của nhân viên {employeeName}";

                    switch (status)
                    {
                        case 0:
                            actionType = "Hủy đánh giá";
                            contentLog = $"{currentUser.FullName} đã hủy đánh giá của nhân viên: {employeeName}";
                            break;
                        case 2:
                            actionType = "TBP xác nhận";
                            contentLog = $"{currentUser.FullName} đã xác nhận đánh giá của nhân viên: {employeeName}";
                            break;
                        case 3:
                            actionType = "BGĐ xác nhận";
                            contentLog = $"{currentUser.FullName} đã xác nhận đánh giá của nhân viên: {employeeName}";
                            break;
                        case 4:
                            actionType = "BGĐ hủy xác nhận";
                            contentLog = $"{currentUser.FullName} đã hủy xác nhận đánh giá của nhân viên: {employeeName}";
                            break;
                        case 5:
                            actionType = "TBP hủy xác nhận";
                            contentLog = $"{currentUser.FullName} đã hủy xác nhận đánh giá của nhân viên: {employeeName}";
                            break;
                    }

                    var log = new KPIEvaluationLog
                    {
                        KPIExamID = kpiExamID,
                        EmployeeID = empID,
                        ActionType = actionType,
                        ContentLog = contentLog,
                        CreatedBy = currentUser.LoginName,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    await _kpiEvaluationLogRepo.CreateAsync(log);
                }
                catch (Exception) { }

                return Ok(ApiResponseFactory.Success(null, $"{statusText} đánh giá thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("test-db-query")]
        public async Task<IActionResult> TestDbQuery(int empID, int examID)
        {
            try
            {
                var kpiExam = _kpiExamRepo.GetByID(examID);
                if (kpiExam == null) return Ok("Exam null");
                int sessionID = kpiExam.KPISessionID ?? 0;
                var kpiPositions = _kpiPositionRepo.GetAll(x => x.KPISessionID == sessionID && x.IsDeleted == false);
                var kpiPositionEmployees = _kpiPositionEmployeeRepo.GetAll(x => x.EmployeeID == empID && x.IsDeleted == false);
                var empPosition = (from p in kpiPositions
                                   join pe in kpiPositionEmployees on p.ID equals pe.KPIPosiotionID
                                   select pe).FirstOrDefault();

                int positionID = empPosition?.KPIPosiotionID ?? -999;
                var rule = _kpiEvaluationRuleRepo.GetAll(x => x.KPISessionID == sessionID && x.KPIPositionID == (positionID > 0 ? positionID : 1) && x.IsDeleted == false).FirstOrDefault();

                int empPointId = -999;
                KPIEmployeePoint empPoint = null;
                if (rule != null)
                {
                    empPointId = await GetKPIEmployeePointID(rule.ID, empID);
                    empPoint = _kpiEmployeePointRepo.GetByID(empPointId);
                }

                return Ok(new
                {
                    SessionID = sessionID,
                    PositionCount = kpiPositions.Count,
                    PositionEmployeeCount = kpiPositionEmployees.Count,
                    ResolvedPositionID = positionID,
                    RuleFound = rule != null,
                    RuleID = rule?.ID,
                    EmpPointId = empPointId,
                    EmpPointFound = empPoint != null,
                    EmpPointStatus = empPoint?.Status
                });
            }
            catch (Exception ex)
            {
                return Ok(ex.ToString());
            }
        }

        #endregion action duyệt/hủy

        #region admin xác nhận

        [HttpPost("admin-confirm-kpi")]
        public async Task<IActionResult> AdminConfirmKPI(int kpiExamID, int empID, bool isConfirm = true)
        {
            try
            {
                if (kpiExamID <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn bài đánh giá!"));
                }
                var param = new
                {
                    KPIExamID = kpiExamID,
                    EmployeeID = empID
                };
                List<KPIEvaluationPoint> lst = await SqlDapper<KPIEvaluationPoint>.ProcedureToListTAsync("spGetKPIEvaluationPoint", param);
                //List<KPIEvaluationPoint> lst = SQLHelper<KPIEvaluationPoint>.ProcedureToListModel("spGetKPIEvaluationPoint", new string[] { "@KPIExamID", "@EmployeeID" }, new object[] { kpiExamID, empID });
                foreach (KPIEvaluationPoint item in lst)
                {
                    item.IsAdminConfirm = isConfirm;
                    await _kpiEvaluationPointRepo.UpdateAsync(item);
                }
                try
                {
                    var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                    var currentUser = ObjectMapper.GetCurrentUser(claims);

                    var employee = _employeeRepo.GetByID(empID);
                    string employeeName = employee?.FullName ?? "";

                    var log = new KPIEvaluationLog
                    {
                        KPIExamID = kpiExamID,
                        EmployeeID = empID,
                        ActionType = isConfirm ? "ADMIN xác nhận" : "ADMIN hủy xác nhận",
                        ContentLog = isConfirm
                            ? $"{currentUser.FullName} đã xác nhận đánh giá của nhân viên: {employeeName}"
                            : $"{currentUser.FullName} đã hủy xác nhận đánh giá của nhân viên: {employeeName}",
                        CreatedBy = currentUser.LoginName,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    await _kpiEvaluationLogRepo.CreateAsync(log);
                }
                catch (Exception) { }
                return Ok(ApiResponseFactory.Success(null, isConfirm ? "Xác nhận đánh giá thành công" : "Hủy xác nhận đánh giá thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion admin xác nhận

        #region save data rule old

        [HttpPost("save-data-rule-old")]
        public async Task<IActionResult> SaveKPIEmployeePointDetailOld([FromBody] SaveKPIEmployeePointDetailRequest request)
        {
            try
            {
                //Get possition của nhân viên
                var kpiPositions = _kpiPositionRepo.GetAll(x => x.KPISessionID == request.KPISessionID && x.IsDeleted == false);
                var kpiPositionEmployees = _kpiPositionEmployeeRepo.GetAll(x => x.EmployeeID == request.EmployeeID && x.IsDeleted == false);
                var positionEmp = (from p in kpiPositions
                                   join pe in kpiPositionEmployees on p.ID equals pe.KPIPosiotionID
                                   select pe)
                        .FirstOrDefault() ?? new KPIPositionEmployee();
                KPIEvaluationRule kpiRule = _kpiEvaluationRuleRepo.GetAll(x => x.KPISessionID == request.KPISessionID && x.KPIPositionID == (positionEmp.KPIPosiotionID > 0 ? positionEmp.KPIPosiotionID : 1) && x.IsDeleted == false)
                    .FirstOrDefault() ?? new KPIEvaluationRule(); // 1 là kỹ thuật
                int empPointID = await GetKPIEmployeePointID(kpiRule.ID, request.EmployeeID);

                var trackedDetails = _kpiEmployeePointDetailRepo.GetAll(x => x.KPIEmployeePointID == empPointID);

                var oldDetails = trackedDetails
                    .Select(x => new
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

                //câp nhật trang thai của KPIEmployeePoint status = 2; totalPercent = percentRemaining
                KPIEmployeePoint master = _kpiEmployeePointRepo.GetByID(empPointID);
                decimal oldTotalPercent = master.TotalPercent ?? 0;
                master.Status = 2;
                master.TotalPercent = request.PercentRemaining;
                await _kpiEmployeePointRepo.UpdateAsync(master);

                var toUpdate = new List<KPIEmployeePointDetail>();
                var toCreate = new List<KPIEmployeePointDetail>();

                foreach (var item in request.lstKPIEmployeePointDetail)
                {
                    int detailID = item.EmpPointDetailID ?? 0;
                    if (detailID > 0)
                    {
                        var detail = trackedDetails.FirstOrDefault(x => x.ID == detailID);
                        if (detail != null)
                        {
                            detail.KPIEmployeePointID = empPointID;
                            detail.KPIEvaluationRuleDetailID = item.ID;
                            detail.FirstMonth = item.FirstMonth;
                            detail.SecondMonth = item.SecondMonth;
                            detail.ThirdMonth = item.ThirdMonth;
                            detail.PercentBonus = item.PercentBonus;
                            detail.PercentRemaining = item.PercentRemaining;
                            toUpdate.Add(detail);
                        }
                    }
                    else
                    {
                        var detail = new KPIEmployeePointDetail
                        {
                            KPIEmployeePointID = empPointID,
                            KPIEvaluationRuleDetailID = item.ID,
                            FirstMonth = item.FirstMonth,
                            SecondMonth = item.SecondMonth,
                            ThirdMonth = item.ThirdMonth,
                            PercentBonus = item.PercentBonus,
                            PercentRemaining = item.PercentRemaining
                        };
                        toCreate.Add(detail);
                    }
                }

                if (toUpdate.Count > 0)
                {
                    await _kpiEmployeePointDetailRepo.UpdateRangeAsync_Binh(toUpdate);
                }
                if (toCreate.Count > 0)
                {
                    await _kpiEmployeePointDetailRepo.CreateRangeAsync(toCreate);
                }
                try
                {
                    var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                    var currentUser = ObjectMapper.GetCurrentUser(claims);

                    var employee = _employeeRepo.GetByID(request.EmployeeID);
                    string employeeName = employee?.FullName ?? "";

                    var changes = new List<string>();

                    var allRuleDetails = _kpiEvaluationRuleDetailRepo.GetAll(x => x.KPIEvaluationRuleID == kpiRule.ID && x.IsDeleted == false).ToList();
                    var parentIds = allRuleDetails.Where(x => x.ParentID.HasValue && x.ParentID.Value > 0)
                        .Select(x => x.ParentID.Value)
                        .Distinct()
                        .ToHashSet();

                    foreach (var item in request.lstKPIEmployeePointDetail)
                    {
                        // Chỉ ghi log cho các node con (leaf nodes)
                        if (parentIds.Contains(item.ID))
                        {
                            continue;
                        }

                        var oldDetail = oldDetails.FirstOrDefault(x => x.KPIEvaluationRuleDetailID == item.ID);
                        var ruleDetail = allRuleDetails.FirstOrDefault(x => x.ID == item.ID);
                        string ruleName = ruleDetail?.RuleContent ?? ruleDetail?.FormulaCode ?? $"ID {item.ID}";

                        var itemChanges = new List<string>();
                        if (oldDetail == null)
                        {
                            if (item.FirstMonth != null && item.FirstMonth != 0) itemChanges.Add($"T1: {item.FirstMonth}");
                            if (item.SecondMonth != null && item.SecondMonth != 0) itemChanges.Add($"T2: {item.SecondMonth}");
                            if (item.ThirdMonth != null && item.ThirdMonth != 0) itemChanges.Add($"T3: {item.ThirdMonth}");
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

                            if (itemChanges.Count > 0)
                            {
                                changes.Add($"Dòng [{ruleName}]: {string.Join(", ", itemChanges)}");
                            }
                        }
                    }

                    // Check if totalPercent changed
                    if (oldTotalPercent != request.PercentRemaining)
                    {
                        changes.Add($"Tổng % còn lại: {oldTotalPercent} -> {request.PercentRemaining}");
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

                    var employeeExam = (from exam in _kpiExamRepo.GetAll(x => x.KPISessionID == request.KPISessionID && x.IsDeleted == false)
                                        join ep in _kpiExamPositionRepo.GetAll(x => x.IsDeleted == false) on exam.ID equals ep.KPIExamID
                                        where ep.KPIPositionID == (positionEmp.KPIPosiotionID > 0 ? positionEmp.KPIPosiotionID : 1)
                                        select exam).FirstOrDefault();
                    int? resolvedExamID = employeeExam?.ID;
                    if (resolvedExamID == null)
                    {
                        var fallbackExam = _kpiExamRepo.GetAll(x => x.KPISessionID == request.KPISessionID && x.IsDeleted != true).FirstOrDefault();
                        resolvedExamID = fallbackExam?.ID;
                    }

                    var log = new KPIEvaluationLog
                    {
                        KPIExamID = resolvedExamID,
                        EmployeeID = request.EmployeeID,
                        ActionType = "Lưu điểm KPI Rule",
                        ContentLog = contentLog,
                        CreatedBy = currentUser.LoginName,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    await _kpiEvaluationLogRepo.CreateAsync(log);
                }
                catch (Exception) { }
                return Ok(ApiResponseFactory.Success(new { master, request.lstKPIEmployeePointDetail }, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion save data rule old

        #region save data rule 
        [HttpPost("save-data-rule")]
        public async Task<IActionResult> SaveKPIEmployeePointDetail([FromBody] SaveKPIEmployeePointDetailRequest request)
        {
            try
            {
                //Get possition của nhân viên
                var kpiPositions = _kpiPositionRepo.GetAll(x => x.KPISessionID == request.KPISessionID && x.IsDeleted == false);
                var kpiPositionEmployees = _kpiPositionEmployeeRepo.GetAll(x => x.EmployeeID == request.EmployeeID && x.IsDeleted == false);
                var positionEmp = (from p in kpiPositions
                                   join pe in kpiPositionEmployees on p.ID equals pe.KPIPosiotionID
                                   select pe)
                        .FirstOrDefault() ?? new KPIPositionEmployee();
                KPIEvaluationRule kpiRule = _kpiEvaluationRuleRepo.GetAll(x => x.KPISessionID == request.KPISessionID && x.KPIPositionID == (positionEmp.KPIPosiotionID > 0 ? positionEmp.KPIPosiotionID : 1) && x.IsDeleted == false)
                    .FirstOrDefault() ?? new KPIEvaluationRule(); // 1 là kỹ thuật
                int empPointID = await GetKPIEmployeePointID(kpiRule.ID, request.EmployeeID);

                // Snapshot dữ liệu cũ để ghi log thay đổi
                var oldDetails = _kpiEmployeePointDetailRepo.GetAll(x => x.KPIEmployeePointID == empPointID)
                    .Select(x => new
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

                //câp nhật trang thai của KPIEmployeePoint status = 2; totalPercent = percentRemaining
                KPIEmployeePoint master = _kpiEmployeePointRepo.GetByID(empPointID);
                decimal oldTotalPercent = master.TotalPercent ?? 0;
                master.Status = 2;
                master.TotalPercent = request.PercentRemaining;
                await _kpiEmployeePointRepo.UpdateAsync(master);

                // ===== DELETE ALL + INSERT ALL (theo chuẩn WinForm SaveDataRule) =====
                // WinForm: SQLHelper<KPIEmployeePointDetailModel>.DeleteByAttribute("KPIEmployeePointID", empPointID);
                // Sau đó INSERT lại tất cả node — tránh hoàn toàn việc sinh ra bản ghi duplicate
                await _kpiEmployeePointDetailRepo.DeleteByAttributeAsync("KPIEmployeePointID", empPointID);

                var toCreate = request.lstKPIEmployeePointDetail
                    .Select(item => new KPIEmployeePointDetail
                    {
                        KPIEmployeePointID = empPointID,
                        KPIEvaluationRuleDetailID = item.ID,
                        FirstMonth = item.FirstMonth,
                        SecondMonth = item.SecondMonth,
                        ThirdMonth = item.ThirdMonth,
                        PercentBonus = item.PercentBonus,
                        PercentRemaining = item.PercentRemaining
                    }).ToList();

                if (toCreate.Count > 0)
                {
                    await _kpiEmployeePointDetailRepo.CreateRangeAsync(toCreate);
                }
                try
                {
                    var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                    var currentUser = ObjectMapper.GetCurrentUser(claims);

                    var employee = _employeeRepo.GetByID(request.EmployeeID);
                    string employeeName = employee?.FullName ?? "";

                    var changes = new List<string>();

                    var allRuleDetails = _kpiEvaluationRuleDetailRepo.GetAll(x => x.KPIEvaluationRuleID == kpiRule.ID && x.IsDeleted == false).ToList();
                    var parentIds = allRuleDetails.Where(x => x.ParentID.HasValue && x.ParentID.Value > 0)
                        .Select(x => x.ParentID.Value)
                        .Distinct()
                        .ToHashSet();

                    foreach (var item in request.lstKPIEmployeePointDetail)
                    {
                        // Chỉ ghi log cho các node con (leaf nodes)
                        if (parentIds.Contains(item.ID))
                        {
                            continue;
                        }

                        var oldDetail = oldDetails.FirstOrDefault(x => x.KPIEvaluationRuleDetailID == item.ID);
                        var ruleDetail = allRuleDetails.FirstOrDefault(x => x.ID == item.ID);
                        string ruleName = ruleDetail?.RuleContent ?? ruleDetail?.FormulaCode ?? $"ID {item.ID}";

                        var itemChanges = new List<string>();
                        if (oldDetail == null)
                        {
                            if (item.FirstMonth != null && item.FirstMonth != 0) itemChanges.Add($"T1: {item.FirstMonth}");
                            if (item.SecondMonth != null && item.SecondMonth != 0) itemChanges.Add($"T2: {item.SecondMonth}");
                            if (item.ThirdMonth != null && item.ThirdMonth != 0) itemChanges.Add($"T3: {item.ThirdMonth}");
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

                            if (itemChanges.Count > 0)
                            {
                                changes.Add($"Dòng [{ruleName}]: {string.Join(", ", itemChanges)}");
                            }
                        }
                    }

                    // Check if totalPercent changed
                    if (oldTotalPercent != request.PercentRemaining)
                    {
                        changes.Add($"Tổng % còn lại: {oldTotalPercent} -> {request.PercentRemaining}");
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

                    var employeeExam = (from exam in _kpiExamRepo.GetAll(x => x.KPISessionID == request.KPISessionID && x.IsDeleted == false)
                                        join ep in _kpiExamPositionRepo.GetAll(x => x.IsDeleted == false) on exam.ID equals ep.KPIExamID
                                        where ep.KPIPositionID == (positionEmp.KPIPosiotionID > 0 ? positionEmp.KPIPosiotionID : 1)
                                        select exam).FirstOrDefault();
                    int? resolvedExamID = employeeExam?.ID;
                    if (resolvedExamID == null)
                    {
                        var fallbackExam = _kpiExamRepo.GetAll(x => x.KPISessionID == request.KPISessionID && x.IsDeleted != true).FirstOrDefault();
                        resolvedExamID = fallbackExam?.ID;
                    }

                    var log = new KPIEvaluationLog
                    {
                        KPIExamID = resolvedExamID,
                        EmployeeID = request.EmployeeID,
                        ActionType = "Lưu điểm KPI Rule",
                        ContentLog = contentLog,
                        CreatedBy = currentUser.LoginName,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    await _kpiEvaluationLogRepo.CreateAsync(log);
                }
                catch (Exception) { }
                return Ok(ApiResponseFactory.Success(new { master, request.lstKPIEmployeePointDetail }, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion save data rule

        #region chức năng LOADDATATEAM

        [HttpGet("get-all-team-by-empID")]
        public async Task<IActionResult> GetAllTeamByEmployeeID(int employeeID, int kpiSessionID)
        {
            try
            {
                if (kpiSessionID <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn Kỳ đánh giá!"));
                }
                var param = new
                {
                    EmployeeID = employeeID
                };
                List<Employee> lstTeam = await SqlDapper<Employee>.ProcedureToListTAsync("spGetAllTeamByEmployeeID", param);
                //List<Employee> lstTeam = SQLHelper<Employee>.ProcedureToListModel("spGetAllTeamByEmployeeID", new string[] { "@EmployeeID" }, new object[] { employeeID });

                return Ok(ApiResponseFactory.Success(lstTeam, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-all-team-by-empID-new")]
        public async Task<IActionResult> GetAllTeamByEmployeeIDNew(int employeeID, int kpiSessionID)
        {
            try
            {
                if (kpiSessionID <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn Kỳ đánh giá!"));
                }

                var session = _kpiSessionRepo.GetByID(kpiSessionID);
                if (session == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy Kỳ đánh giá!"));
                }

                int year = session.YearEvaluation ?? DateTime.Now.Year;
                int quarter = session.QuarterEvaluation ?? 1;

                var param = new
                {
                    EmployeeID = employeeID,
                    Year = year,
                    Quarter = quarter
                };

                var lstTeam = await SqlDapper<dynamic>.ProcedureToListAsync("spGetTeamMembersByEmployeeID", param);

                return Ok(ApiResponseFactory.Success(lstTeam, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("load-data-team")]
        public async Task<IActionResult> LoadDataTeam([FromBody] LoadDataTeamRequest request)
        {
            try
            {
                if (request.kpiSessionID <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn Kỳ đánh giá!"));
                }
                var param = new
                {
                    EmployeeID = request.employeeID
                };
                List<Employee> lstTeam = await SqlDapper<Employee>.ProcedureToListTAsync("spGetAllTeamByEmployeeID", param);
                //List<Employee> lstTeam = SQLHelper<Employee>.ProcedureToListModel("spGetAllTeamByEmployeeID", new string[] { "@EmployeeID" }, new object[] { request.employeeID });

                var kpiPositions = _kpiPositionRepo.GetAll(x => x.KPISessionID == request.kpiSessionID && x.IsDeleted == false);
                foreach (Employee emp in lstTeam)
                {
                    var kpiPositionEmployees1 = _kpiPositionEmployeeRepo.GetAll(x => x.EmployeeID == emp.ID && x.IsDeleted == false);
                    var position1 = (from p in kpiPositions
                                     join pe in kpiPositionEmployees1 on p.ID equals pe.KPIPosiotionID
                                     select pe)
                                 .FirstOrDefault() ?? new KPIPositionEmployee();
                    KPIEvaluationRule ruleModel = _kpiEvaluationRuleRepo.GetAll(x => x.KPISessionID == request.kpiSessionID && x.KPIPositionID == (position1.KPIPosiotionID > 0 ? position1.KPIPosiotionID : 1) && x.IsDeleted == false)
                        .FirstOrDefault() ?? new KPIEvaluationRule(); // 1 là kỹ thuật
                    if (ruleModel.ID <= 0) continue;

                    bool targetIsDelete = request.lstEmpChose.Any(x => x.ID == emp.ID) ? true : false;
                    KPIEmployeePoint empPoint = _kpiEmployeePointRepo.GetAll(x => x.EmployeeID == emp.ID && x.KPIEvaluationRuleID == ruleModel.ID && x.IsDelete == targetIsDelete).OrderByDescending(x => x.ID).FirstOrDefault() ?? new KPIEmployeePoint();
                    if (empPoint.ID <= 0) continue;
                    // 1. Kiểm tra: Nếu nhân viên KHÔNG có trong danh sách chọn (So sánh bằng ID)
                    if (!request.lstEmpChose.Any(x => x.ID == emp.ID))
                    {
                        // 2. Chỉ thực hiện Update nếu bản ghi điểm đã tồn tại trong DB (ID > 0)
                        if (empPoint.ID > 0)
                        {
                            // 3. Gán trạng thái xóa (Kiểm tra Model của bạn là bool hay int để gán true hoặc 1)
                            empPoint.IsDelete = true; // hoặc = 1;

                            // 4. Gọi Repository để update
                            await _kpiEmployeePointRepo.UpdateAsync(empPoint);
                        }
                        // 5. Bỏ qua các bước bên dưới, chuyển sang nhân viên tiếp theo
                        continue;
                    }
                    empPoint.IsDelete = false;
                    await _kpiEmployeePointRepo.UpdateAsync(empPoint);
                    var param2 = new
                    {
                        KPIEmployeePointID = empPoint.ID
                    };
                    List<KPIRuleDetailDTO> dtKpiRule = await SqlDapper<KPIRuleDetailDTO>.ProcedureToListTAsync("spGetEmployeeRulePointByKPIEmpPointIDNew_TNB", param2);
                    //List<KPIRuleDetailDTO> dtKpiRule = SQLHelper<KPIRuleDetailDTO>.ProcedureToListModel("spGetEmployeeRulePointByKPIEmpPointIDNew_TNB", new string[] { "@KPIEmployeePointID" }, new object[] { empPoint.ID });
                    if (dtKpiRule.Count <= 0) continue;

                    #region hàm LoadDataView trong winform

                    List<KPISumarizeDTO> lstResult = await SqlDapper<KPISumarizeDTO>.ProcedureToListTAsync("spGetSumarizebyKPIEmpPointIDNew_TNB", param2);
                    //List<KPISumarizeDTO> lstResult = SQLHelper<KPISumarizeDTO>.ProcedureToListModel("spGetSumarizebyKPIEmpPointIDNew_TNB",
                    //                                                   new string[] { "@KPIEmployeePointID" },
                    //                                                   new object[] { empPoint.ID });
                    // 2. TỐI ƯU HIỆU NĂNG: Chuyển List kết quả sang Dictionary
                    // Lý do: Dùng Dictionary giúp việc tìm kiếm theo EvaluationCode tốn O(1) thay vì O(N) như vòng lặp lồng nhau.
                    // Key là EvaluationCode, Value là object KPISumarizeDTO
                    // 3. Map dữ liệu (Thay thế hàm LoadDataView)
                    var summaryDict = lstResult
                        .GroupBy(x => x.EvaluationCode)
                        .ToDictionary(g => g.Key.Trim(), g => g.First());

                    foreach (var detail in dtKpiRule)
                    {
                        if (!string.IsNullOrEmpty(detail.EvaluationCode) &&
                            summaryDict.TryGetValue(detail.EvaluationCode.Trim(), out KPISumarizeDTO summaryVal))
                        {
                            detail.FirstMonth = summaryVal.FirstMonth;
                            detail.SecondMonth = summaryVal.SecondMonth;
                            detail.ThirdMonth = summaryVal.ThirdMonth;
                        }
                    }

                    #endregion hàm LoadDataView trong winform

                    // --- ĐOẠN NÀY CHÍNH LÀ SaveDataDetails ---

                    // 1. Kiểm tra xem đã có dữ liệu chi tiết trong DB chưa (Giống dòng: if (lstDetails.Count > 0) return;)
                    var existingDetails = _kpiEmployeePointDetailRepo.GetAll(x => x.KPIEmployeePointID == empPoint.ID).ToList();

                    // 2. Nếu chưa có (Count == 0) thì mới thực hiện Insert
                    if (existingDetails.Count == 0)
                    {
                        // Thay vì foreach(DataRow row in dt.Rows)
                        foreach (var item in dtKpiRule)
                        {
                            var newDetail = new KPIEmployeePointDetail
                            {
                                KPIEmployeePointID = empPoint.ID,

                                // row["ID"] cũ chính là ID của rule detail từ SP khung
                                KPIEvaluationRuleDetailID = item.ID,

                                // Các trường dữ liệu đã được map từ bước 3
                                FirstMonth = item.FirstMonth,
                                SecondMonth = item.SecondMonth,
                                ThirdMonth = item.ThirdMonth,
                                PercentBonus = item.PercentBonus,
                                PercentRemaining = item.PercentRemaining
                            };
                            await _kpiEmployeePointDetailRepo.CreateAsync(newDetail);
                        }
                    }
                }
                //Get possition của nhân viên
                var kpiPositionEmployees = _kpiPositionEmployeeRepo.GetAll(x => x.EmployeeID == request.employeeID && x.IsDeleted == false);
                var position = (from p in kpiPositions
                                join pe in kpiPositionEmployees on p.ID equals pe.KPIPosiotionID
                                select pe)
                            .FirstOrDefault() ?? new KPIPositionEmployee();
                KPIEvaluationRule rule = _kpiEvaluationRuleRepo.GetAll(x => x.KPIPositionID == position.KPIPosiotionID && x.KPISessionID == request.kpiSessionID)
                    .FirstOrDefault() ?? new KPIEvaluationRule();
                int empPointMaster = await GetKPIEmployeePointID(rule.ID, request.employeeID);

                try
                {
                    var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                    var currentUser = ObjectMapper.GetCurrentUser(claims);
                    var employeeExam = (from exam in _kpiExamRepo.GetAll(x => x.KPISessionID == request.kpiSessionID && x.IsDeleted == false)
                                        join ep in _kpiExamPositionRepo.GetAll(x => x.IsDeleted == false) on exam.ID equals ep.KPIExamID
                                        where ep.KPIPositionID == (position.KPIPosiotionID > 0 ? position.KPIPosiotionID : 1)
                                        select exam).FirstOrDefault();
                    int? resolvedExamID = employeeExam?.ID;
                    if (resolvedExamID == null)
                    {
                        var fallbackExam = _kpiExamRepo.GetAll(x => x.KPISessionID == request.kpiSessionID && x.IsDeleted != true).FirstOrDefault();
                        resolvedExamID = fallbackExam?.ID;
                    }

                    var log = new KPIEvaluationLog
                    {
                        KPIExamID = resolvedExamID,
                        EmployeeID = request.employeeID,
                        ActionType = "UPDATE_TEAM",
                        ContentLog = System.Text.Json.JsonSerializer.Serialize(request),
                        CreatedBy = currentUser.LoginName,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    await _kpiEvaluationLogRepo.CreateAsync(log);
                }
                catch (Exception) { }

                return Ok(ApiResponseFactory.Success(empPointMaster, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion chức năng LOADDATATEAM

        #region LoadPointRuleNew

        [HttpGet("load-point-rule-new")]
        public async Task<IActionResult> LoadPointRuleNew(int kpiExamID, int employeeID, int sessionID)
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

        #region Xuất Excel theo Team

        /// <summary>
        /// Xuất file Excel đánh giá KPI theo nhóm (Team), nén thành file ZIP
        /// </summary>
        /// <param name="kpiSessionId">ID của kỳ đánh giá</param>
        /// <param name="departmentId">ID phòng ban</param>
        /// <returns>File ZIP chứa các file Excel theo Team</returns>
        [HttpGet("export-excel-by-team")]
        public async Task<IActionResult> ExportExcelByTeam(int kpiSessionId, int departmentId, int userTeamId = 0)
        {
            try
            {
                // 1. Lấy thông tin Kỳ đánh giá
                var session = _kpiSessionRepo.GetByID(kpiSessionId);
                if (session == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy kỳ đánh giá!"));
                }

                string year = session.YearEvaluation?.ToString() ?? DateTime.Now.Year.ToString();
                string quarter = "Q" + (session.QuarterEvaluation?.ToString() ?? "1");

                // 2. Chuẩn bị stream để nén file Zip
                using (var memoryStream = new MemoryStream())
                {
                    using (var archive = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create, true))
                    {
                        // 3. Lấy danh sách bài thi (Exam)
                        var paramExam = new { KPISessionID = kpiSessionId, DepartmentID = departmentId };
                        var exams = await SqlDapper<object>.ProcedureToListAsync("spGetKPIExamByKPISessionID", paramExam) as List<dynamic>;

                        if (exams != null && exams.Count > 0)
                        {
                            foreach (dynamic examRow in exams)
                            {
                                int kpiExamID = Convert.ToInt32(((IDictionary<string, object>)examRow)["ID"]);
                                string examCode = ((IDictionary<string, object>)examRow)["ExamCode"]?.ToString() ?? "Unknown";

                                // 4. Lấy danh sách nhân viên đã được đánh giá
                                var paramEmp = new
                                {
                                    EvaluationType = 1,
                                    DepartmentID = departmentId,
                                    Keywords = "",
                                    Status = -1,
                                    UserTeamID = userTeamId,
                                    KPIExamID = kpiExamID
                                };
                                var employees = await SqlDapper<object>.ProcedureToListAsync("spGetAllEmployeeKPIEvaluated", paramEmp) as List<dynamic>;

                                if (employees == null || employees.Count == 0) continue;

                                // 5. Nhóm nhân viên theo ProjectTypeName (Tên Team)
                                var projectTypeGroups = employees
                                    .Select(e => (IDictionary<string, object>)e)
                                    .GroupBy(e => SanitizeFileName(e.ContainsKey("ProjectTypeName") ? e["ProjectTypeName"]?.ToString() ?? "ChuaXacDinh" : "ChuaXacDinh"));

                                var tasks = new List<Task<(string zipPath, byte[] bytes)>>();

                                foreach (var group in projectTypeGroups)
                                {
                                    string teamName = group.Key;

                                    foreach (var empRow in group)
                                    {
                                        int employeeID = Convert.ToInt32(empRow["ID"]);
                                        string employeeName = SanitizeFileName(empRow.ContainsKey("FullName") ? empRow["FullName"]?.ToString() ?? "Unknown" : "Unknown");
                                        string employeeCode = empRow.ContainsKey("Code") ? empRow["Code"]?.ToString() ?? "" : "";

                                        // 6. Tạo đường dẫn file trong ZIP
                                        string zipEntryPath = $"{year}/{quarter}/{teamName}/DanhGiaKPI_{examCode}_{employeeCode}_{employeeName}.xlsx";

                                        // 7. Tạo file Excel cho nhân viên (chạy đồng thời)
                                        tasks.Add(Task.Run(async () =>
                                        {
                                            var bytes = await GenerateKPIExcelBytesAsync(employeeID, kpiExamID, kpiSessionId, departmentId);
                                            return (zipEntryPath, bytes);
                                        }));
                                    }
                                }

                                var results = await Task.WhenAll(tasks);
                                foreach (var result in results)
                                {
                                    if (result.bytes != null && result.bytes.Length > 0)
                                    {
                                        var zipEntry = archive.CreateEntry(result.zipPath);
                                        using (var entryStream = zipEntry.Open())
                                        {
                                            await entryStream.WriteAsync(result.bytes, 0, result.bytes.Length);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // 8. Trả về file ZIP cho client
                    memoryStream.Position = 0;
                    string zipFileName = $"KPI_Export_{year}_{quarter}.zip";
                    return File(memoryStream.ToArray(), "application/zip", zipFileName);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Loại bỏ các ký tự không hợp lệ trong tên file/thư mục
        /// </summary>
        /// <param name="name">Tên gốc</param>
        /// <returns>Tên đã được làm sạch</returns>
        [NonAction]
        private string SanitizeFileName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "Unknown";

            // Loại bỏ các ký tự không hợp lệ trong tên file
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }

            // Loại bỏ khoảng trắng thừa
            return name.Trim();
        }

        /// <summary>
        /// Tạo file Excel chứa thông tin đánh giá KPI cho một nhân viên
        /// </summary>
        /// <param name="employeeID">ID nhân viên</param>
        /// <param name="kpiExamID">ID bài thi KPI</param>
        /// <param name="kpiSessionId">ID kỳ đánh giá</param>
        /// <param name="departmentId">ID phòng ban</param>
        /// <returns>Mảng byte chứa nội dung file Excel</returns>
        [NonAction]
        private async Task<byte[]> GenerateKPIExcelBytesAsync(int employeeID, int kpiExamID, int kpiSessionId, int departmentId)
        {
            try
            {
                // Khai báo License cho EPPlus (bắt buộc với phiên bản mới)
                OfficeOpenXml.ExcelPackage.License.SetNonCommercialOrganization("RTC");

                using (var package = new OfficeOpenXml.ExcelPackage())
                {
                    // Sheet 1 removed

                    #region Sheet 2: KPI Đánh giá kỹ năng

                    var kpiSkillSheet = package.Workbook.Worksheets.Add("ĐÁNH GIÁ KỸ NĂNG");

                    // Lấy dữ liệu KPI Kỹ năng
                    var paramSkill = new { EmployeeID = employeeID, EvaluationType = 1, KPIExamID = kpiExamID, IsPulbic = true };
                    var dtSkill = await SqlDapper<object>.ProcedureToListAsync("spGetAllKPIEvaluationPoint_TNB", paramSkill) as List<dynamic>;
                    dtSkill = CalculateKPIPoints(dtSkill);

                    if (dtSkill != null && dtSkill.Count > 0)
                    {
                        FillKPISheet(kpiSkillSheet, dtSkill, "ĐÁNH GIÁ KPI KỸ NĂNG");
                    }
                    else
                    {
                        kpiSkillSheet.Cells["A1"].Value = "Không có dữ liệu";
                    }

                    #endregion Sheet 2: KPI Đánh giá kỹ năng

                    #region Sheet 3: KPI Đánh giá chuyên môn

                    var kpiProfSheet = package.Workbook.Worksheets.Add("ĐÁNH GIÁ CHUYÊN MÔN");

                    // Lấy dữ liệu KPI Chuyên môn
                    var paramProf = new { EmployeeID = employeeID, EvaluationType = 2, KPIExamID = kpiExamID, IsPulbic = true };
                    var dtProf = await SqlDapper<object>.ProcedureToListAsync("spGetAllKPIEvaluationPoint_TNB", paramProf) as List<dynamic>;
                    dtProf = CalculateKPIPoints(dtProf);

                    if (dtProf != null && dtProf.Count > 0)
                    {
                        FillKPISheet(kpiProfSheet, dtProf, "ĐÁNH GIÁ KPI CHUYÊN MÔN");
                    }
                    else
                    {
                        kpiProfSheet.Cells["A1"].Value = "Không có dữ liệu";
                    }

                    #endregion Sheet 3: KPI Đánh giá chuyên môn

                    #region Sheet 4: KPI Đánh giá chung

                    var kpiGenSheet = package.Workbook.Worksheets.Add("ĐÁNH GIÁ CHUNG");

                    // Lấy dữ liệu KPI Chung
                    var paramGen = new { EmployeeID = employeeID, EvaluationType = 3, KPIExamID = kpiExamID, IsPulbic = true };
                    var dtGen = await SqlDapper<object>.ProcedureToListAsync("spGetAllKPIEvaluationPoint_TNB", paramGen) as List<dynamic>;
                    dtGen = CalculateKPIPoints(dtGen);

                    if (dtGen != null && dtGen.Count > 0)
                    {
                        FillKPISheet(kpiGenSheet, dtGen, "ĐÁNH GIÁ KPI CHUNG");
                    }
                    else
                    {
                        kpiGenSheet.Cells["A1"].Value = "Không có dữ liệu";
                    }

                    #endregion Sheet 4: KPI Đánh giá chung

                    #region Sheet 5: Tổng hợp đánh giá

                    var summarySheet = package.Workbook.Worksheets.Add("TỔNG HỢP ĐÁNH GIÁ");
                    FillTotalAVGSheet(summarySheet, dtSkill, dtProf, dtGen);

                    #endregion Sheet 5: Tổng hợp đánh giá

                    #region Sheet 6: KPI RULE

                    var kpiRuleSheet = package.Workbook.Worksheets.Add("KPI RULE");

                    // Lấy dữ liệu tổng hợp từ Rule
                    var kpiPositions = _kpiPositionRepo.GetAll(x => x.KPISessionID == kpiSessionId && x.IsDeleted == false);
                    var kpiPositionEmployees = _kpiPositionEmployeeRepo.GetAll(x => x.EmployeeID == employeeID && x.IsDeleted == false);

                    var empPosition = (from p in kpiPositions
                                       join pe in kpiPositionEmployees on p.ID equals pe.KPIPosiotionID
                                       select pe).FirstOrDefault() ?? new KPIPositionEmployee();

                    KPIEvaluationRule rule = _kpiEvaluationRuleRepo.GetAll(x => x.KPISessionID == kpiSessionId && x.KPIPositionID == (empPosition.KPIPosiotionID > 0 ? empPosition.KPIPosiotionID : 1) && x.IsDeleted == false)
                        .FirstOrDefault() ?? new KPIEvaluationRule();

                    int empPointId = await GetKPIEmployeePointID(rule.ID, employeeID);

                    // Lấy điểm tổng hợp
                    var paramSummary = new { KPIEmployeePointID = empPointId, IsPublic = 1 };
                    var dtSummary = await SqlDapper<object>.ProcedureToListAsync("spGetEmployeeRulePointByKPIEmpPointIDNew_TNB", paramSummary) as List<dynamic>;

                    if (dtSummary != null && dtSummary.Count > 0)
                    {
                        // Lấy TotalPercentActual từ KPIEmployeePoint (khớp với FE: getFinalPoint -> TotalPercentActual)
                        KPIEmployeePoint kpiEmpPoint = _kpiEmployeePointRepo.GetByID(empPointId);
                        double totalPercentActual = (double)(kpiEmpPoint?.TotalPercentActual ?? 0);
                        FillSummarySheet(kpiRuleSheet, dtSummary, totalPercentActual);
                    }
                    else
                    {
                        kpiRuleSheet.Cells["A1"].Value = "Không có dữ liệu KPI RULE";
                    }

                    #endregion Sheet 6: KPI RULE

                    #region Sheet 7: TEAM RULE

                    var teamRuleSheet = package.Workbook.Worksheets.Add("TEAM RULE");
                    var paramTeam = new { KPIEmployeePointID = empPointId };
                    var dtTeam = await SqlDapper<object>.ProcedureToListAsync("spGetKpiRuleSumarizeTeamNew_TNB", paramTeam) as List<dynamic>;
                    // Luôn điền header cho sheet Team Rule kể cả khi không có dữ liệu
                    FillTeamRuleSheet(teamRuleSheet, dtTeam ?? new List<dynamic>());

                    #endregion Sheet 7: TEAM RULE

                    // Trả về mảng byte của file Excel
                    return package.GetAsByteArray();
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                Console.WriteLine($"Lỗi khi tạo Excel cho nhân viên {employeeID}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Điền dữ liệu KPI vào sheet Excel
        /// </summary>
        /// <param name="sheet">ExcelWorksheet cần điền dữ liệu</param>
        /// <param name="data">Danh sách dữ liệu KPI</param>
        /// <param name="title">Tiêu đề của sheet</param>
        [NonAction]
        private void FillKPISheet(OfficeOpenXml.ExcelWorksheet sheet, List<dynamic> data, string title)
        {
            // Cài đặt font mặc định cho sheet
            sheet.Cells.Style.Font.Name = "Tahoma";
            sheet.Cells.Style.Font.Size = 8.5f;
            sheet.DefaultRowHeight = 20;

            int row = 1;

            //// Tiêu đề sheet
            //sheet.Cells[row, 1].Value = title;
            //sheet.Cells[row, 1].Style.Font.Bold = true;
            //sheet.Cells[row, 1].Style.Font.Size = 14;
            //sheet.Cells[row, 1, row, 15].Merge = true;
            //sheet.Cells[row, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            //row += 2;

            // Header của bảng (Dòng 1: Group Headers)

            int headerRow1 = row;
            int headerRow2 = row + 1;

            Action<int, int, int, int, string> setHeader = (r1, c1, r2, c2, text) =>
            {
                var range = sheet.Cells[r1, c1, r2, c2];
                range.Merge = true;
                range.Value = text;
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                range.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                range.Style.WrapText = true;
            };

            // Dòng 1: Main Headers (Vertical Merge)
            setHeader(headerRow1, 1, headerRow2, 1, "STT");
            setHeader(headerRow1, 2, headerRow2, 2, "Yếu tố đánh giá");
            setHeader(headerRow1, 3, headerRow2, 3, "Điểm chuẩn");
            setHeader(headerRow1, 4, headerRow2, 4, "Hệ số điểm");
            setHeader(headerRow1, 5, headerRow2, 5, "Mức tự đánh giá");
            setHeader(headerRow1, 6, headerRow2, 6, "TBP/PBP đánh giá");
            setHeader(headerRow1, 7, headerRow2, 7, "Đánh giá của BGĐ");
            setHeader(headerRow1, 8, headerRow2, 8, "Phương tiện xác minh tiêu chí");
            setHeader(headerRow1, 9, headerRow2, 9, "ĐVT");

            // Dòng 1: Group Headers
            setHeader(headerRow1, 10, headerRow1, 11, "NV đánh giá");
            setHeader(headerRow1, 12, headerRow1, 13, "TBP đánh giá");
            setHeader(headerRow1, 14, headerRow1, 15, "GĐ đánh giá");

            // Dòng 2: Sub Headers for Groups
            string[] subHeaders = new[]
            {
             "Điểm đánh giá", "Điểm theo hệ số",
             "Điểm đánh giá", "Điểm theo hệ số",
             "Điểm đánh giá", "Điểm theo hệ số"
         };

            for (int i = 0; i < subHeaders.Length; i++)
            {
                var cell = sheet.Cells[headerRow2, i + 10];
                cell.Value = subHeaders[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                cell.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                cell.Style.WrapText = true;
            }

            sheet.Row(headerRow1).Height = 30;
            sheet.Row(headerRow2).Height = 35;
            row = headerRow2 + 1;

            // Thêm dòng TỔNG HỆ SỐ ngay dưới header
            sheet.Cells[row, 1].Value = "";
            sheet.Cells[row, 2].Value = "TỔNG HỆ SỐ";
            sheet.Cells[row, 2].Style.Font.Bold = true;
            sheet.Row(row).Height = 25; // Tăng height cho dòng tổng hệ số ở trên
            sheet.Cells[row, 3].Value = "";

            // Lấy danh sách các node root (ParentID == 0 hoặc -1)
            var roots = data.Where(x =>
            {
                var dr = (IDictionary<string, object>)x;
                int pId = -2; // Default to something that won't match
                if (dr.ContainsKey("ParentID") && dr["ParentID"] != null)
                {
                    int.TryParse(dr["ParentID"].ToString(), out pId);
                }
                return pId == 0 || pId == -1;
            }).ToList();

            // Tổng hệ số = tổng Coefficient của tất cả con trực tiếp của các root (để khớp với FE)
            // Vì CalculateKPIPoints ghi đè Coefficient của root = tổng Coefficient con
            // nên dùng trực tiếp Coefficient của root sau khi calculate là đúng
            double sumRootCoef = roots.Sum(x => ConvertToDouble(((IDictionary<string, object>)x).ContainsKey("Coefficient") ? ((IDictionary<string, object>)x)["Coefficient"] : 0));
            double sumEmpCoef = roots.Sum(x => ConvertToDouble(((IDictionary<string, object>)x).ContainsKey("EmployeeCoefficient") ? ((IDictionary<string, object>)x)["EmployeeCoefficient"] : 0));
            double sumTBPCoef = roots.Sum(x => ConvertToDouble(((IDictionary<string, object>)x).ContainsKey("TBPCoefficient") ? ((IDictionary<string, object>)x)["TBPCoefficient"] : 0));
            double sumBGDCoef = roots.Sum(x => ConvertToDouble(((IDictionary<string, object>)x).ContainsKey("BGDCoefficient") ? ((IDictionary<string, object>)x)["BGDCoefficient"] : 0));

            // Điểm trung bình = Tổng hệ số / Tổng hệ số gốc (khớp với FE: EmployeeCoefficient / Coefficient)
            double avgEmpEval = sumRootCoef > 0 ? Math.Round(sumEmpCoef / sumRootCoef, 2) : 0;
            double avgTBPEval = sumRootCoef > 0 ? Math.Round(sumTBPCoef / sumRootCoef, 2) : 0;
            double avgBGDEval = sumRootCoef > 0 ? Math.Round(sumBGDCoef / sumRootCoef, 2) : 0;

            sheet.Cells[row, 4].Value = sumRootCoef;
            sheet.Cells[row, 4].Style.Font.Bold = true;
            sheet.Cells[row, 8].Value = "TỔNG ĐIỂM TRUNG BÌNH";
            sheet.Cells[row, 8].Style.Font.Bold = true;

            // Điểm đánh giá trung bình (cột 10, 12, 14) - khớp FE: EmployeeEvaluation của root
            sheet.Cells[row, 10].Value = avgEmpEval;
            sheet.Cells[row, 10].Style.Font.Bold = true;
            sheet.Cells[row, 12].Value = avgTBPEval;
            sheet.Cells[row, 12].Style.Font.Bold = true;
            sheet.Cells[row, 14].Value = avgBGDEval;
            sheet.Cells[row, 14].Style.Font.Bold = true;

            // Điểm theo hệ số (cột 11, 13, 15)
            sheet.Cells[row, 11].Value = sumEmpCoef;
            sheet.Cells[row, 11].Style.Font.Bold = true;
            sheet.Cells[row, 13].Value = sumTBPCoef;
            sheet.Cells[row, 13].Style.Font.Bold = true;
            sheet.Cells[row, 15].Value = sumBGDCoef;
            sheet.Cells[row, 15].Style.Font.Bold = true;

            for (int col = 1; col <= 15; col++)
            {
                var cell = sheet.Cells[row, col];
                cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
                cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                if (col == 4 || col >= 10) cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                // Định dạng số cho dòng tổng (trừ cột 4 là hệ số tổng)
                if (col >= 10 || col == 11 || col == 13 || col == 15)
                {
                    cell.Style.Numberformat.Format = "#,##0.00";
                }
            }
            row++;

            int dataStartRow = row;

            // Điền dữ liệu
            foreach (var item in data)
            {
                var dataRow = (IDictionary<string, object>)item;
                string[] keys = new[] { "STT", "EvaluationContent", "StandardPoint", "Coefficient", "EmployeePoint", "TBPPoint", "BGDPoint", "VerificationToolsContent", "Unit", "EmployeeEvaluation", "EmployeeCoefficient", "TBPEvaluation", "TBPCoefficient", "BGDEvaluation", "BGDCoefficient" };
                for (int i = 0; i < keys.Length; i++)
                {
                    object val = dataRow.ContainsKey(keys[i]) ? dataRow[keys[i]] : null;
                    if (i == 0 || i == 1 || i == 7 || i == 8) // Text columns
                        sheet.Cells[row, i + 1].Value = val?.ToString();
                    else // Numeric columns
                        sheet.Cells[row, i + 1].Value = ConvertToDouble(val);
                }

                // Định dạng border cho từng ô
                int pId = -2;
                if (dataRow.ContainsKey("ParentID") && dataRow["ParentID"] != null)
                {
                    int.TryParse(dataRow["ParentID"].ToString(), out pId);
                }
                bool isParentRow = pId == 0 || pId == -1;

                for (int col = 1; col <= 15; col++)
                {
                    var cell = sheet.Cells[row, col];
                    cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    if (isParentRow)
                    {
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }

                    // Căn giữa cho các cột số
                    if (col != 2 && col != 8)
                    {
                        cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    }

                    if (col == 2 || col == 8)
                    {
                        cell.Style.WrapText = true;
                    }

                    // Định dạng số cho các cột kết quả (trừ cột 3-điểm chuẩn và 4-hệ số)
                    if (col >= 5 && col != 8 && col != 9)
                    {
                        cell.Style.Numberformat.Format = "#,##0.00";
                    }

                    cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                }

                // Tự động điều chỉnh chiều cao dòng nếu text quá dài ở cột 2 hoặc 8 (WrapText)
                string evalContent = dataRow.ContainsKey("EvaluationContent") ? dataRow["EvaluationContent"]?.ToString() : "";
                string verifyTools = dataRow.ContainsKey("VerificationToolsContent") ? dataRow["VerificationToolsContent"]?.ToString() : "";

                // Ước tính số dòng dựa trên độ dài text (Cột 2 Width=50 ~65 ký tự, Cột 8 Width=100 ~130 ký tự)
                int estimatedLinesContent = (evalContent?.Length ?? 0) / 65 + 1;
                int estimatedLinesVerify = (verifyTools?.Length ?? 0) / 130 + 1;
                int actualLines = Math.Max(evalContent?.Split('\n').Length ?? 1, verifyTools?.Split('\n').Length ?? 1);
                int totalLines = Math.Max(actualLines, Math.Max(estimatedLinesContent, estimatedLinesVerify));

                isParentRow = isParentRow || evalContent.Contains("(Hệ số:");
                bool isTotalRow = evalContent.IndexOf("TỔNG", StringComparison.OrdinalIgnoreCase) >= 0;

                if (totalLines > 1)
                {
                    sheet.Row(row).Height = totalLines * 15 + 10;
                }
                else if (isParentRow || isTotalRow)
                {
                    sheet.Row(row).Height = 30; // Tăng height cho các dòng tiêu đề 1.1, 1.2 và dòng tổng
                }
                else
                {
                    sheet.Row(row).Height = 22; // Default height
                }

                row++;
            }

            // Dòng tổng kết
            sheet.Cells[row, 1].Value = "";
            sheet.Cells[row, 2].Value = "";
            sheet.Cells[row, 2].Style.Font.Bold = true;
            sheet.Cells[row, 3].Value = "";
            sheet.Cells[row, 4].Value = "";
            sheet.Cells[row, 5].Value = "";
            sheet.Cells[row, 6].Value = "";
            sheet.Cells[row, 7].Value = "";
            sheet.Cells[row, 8].Value = "";
            sheet.Cells[row, 9].Value = "";

            int summaryRow = dataStartRow - 1;
            // Tính tổng điểm - Lấy trực tiếp từ dòng TỔNG HỆ SỐ ở trên để tránh bị x2 do tính cả node cha và con
            sheet.Cells[row, 4].Formula = $"={OfficeOpenXml.ExcelAddress.GetAddress(summaryRow, 4)}";
            sheet.Cells[row, 10].Formula = $"={OfficeOpenXml.ExcelAddress.GetAddress(summaryRow, 10)}";
            sheet.Cells[row, 11].Formula = $"={OfficeOpenXml.ExcelAddress.GetAddress(summaryRow, 11)}";
            sheet.Cells[row, 12].Formula = $"={OfficeOpenXml.ExcelAddress.GetAddress(summaryRow, 12)}";
            sheet.Cells[row, 13].Formula = $"={OfficeOpenXml.ExcelAddress.GetAddress(summaryRow, 13)}";
            sheet.Cells[row, 14].Formula = $"={OfficeOpenXml.ExcelAddress.GetAddress(summaryRow, 14)}";
            sheet.Cells[row, 15].Formula = $"={OfficeOpenXml.ExcelAddress.GetAddress(summaryRow, 15)}";

            // Định dạng dòng tổng
            for (int col = 1; col <= 15; col++)
            {
                var cell = sheet.Cells[row, col];
                cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                if (col >= 10 || (col >= 5 && col <= 7))
                {
                    cell.Style.Numberformat.Format = "#,##0.00";
                }
            }
            sheet.Row(row).Height = 25;

            // Tự động điều chỉnh độ rộng cột
            sheet.Cells.AutoFitColumns();

            // Đặt độ rộng cố định cho cột nội dung để WrapText hiệu quả
            sheet.Column(2).Width = 50;
            sheet.Column(8).Width = 100; // Tăng width cột H
        }

        /// <summary>
        /// Điền dữ liệu tổng hợp vào sheet Excel
        /// </summary>
        /// <param name="sheet">ExcelWorksheet cần điền dữ liệu</param>
        /// <param name="data">Danh sách dữ liệu tổng hợp</param>
        [NonAction]
        private void FillSummarySheet(OfficeOpenXml.ExcelWorksheet sheet, List<dynamic> data, double totalPercentActual = 0)
        {
            sheet.Cells.Style.Font.Name = "Tahoma";
            sheet.Cells.Style.Font.Size = 8.5f;

            int row = 1;

            //// Tiêu đề
            //sheet.Cells[row, 1].Value = "TỔNG HỢP ĐIỂM ĐÁNH GIÁ KPI";
            //sheet.Cells[row, 1].Style.Font.Bold = true;
            //sheet.Cells[row, 1].Style.Font.Size = 14;
            //sheet.Cells[row, 1, row, 7].Merge = true;
            //sheet.Cells[row, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            //row += 2;

            //// Header của bảng (Dòng 1: Group Headers)
            //row++;
            int headerRow1 = row;
            int headerRow2 = row + 1;

            Action<int, int, int, int, string, System.Drawing.Color> setHeader = (r1, c1, r2, c2, text, color) =>
            {
                var range = sheet.Cells[r1, c1, r2, c2];
                range.Merge = true;
                range.Value = text;
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                range.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                range.Style.WrapText = true;
            };

            var headerColor = System.Drawing.Color.LightGray;

            // Dòng 1: Group Headers
            setHeader(headerRow1, 1, headerRow1, 2, "Nội dung đánh giá", headerColor);
            setHeader(headerRow1, 3, headerRow1, 6, "Số lần vi phạm", headerColor);
            setHeader(headerRow1, 7, headerRow1, 11, "Tổng % thưởng", headerColor);
            setHeader(headerRow1, 12, headerRow2, 12, "Rule", headerColor);
            setHeader(headerRow1, 13, headerRow2, 13, "Ghi chú", headerColor);

            // Dòng 2: Sub Headers
            string[] subHeaders = new[]
            {
             "STT", "Nội dung đánh giá",
             "Tháng 1", "Tháng 2", "Tháng 3", "Tổng",
             "Tổng % thưởng tối đa", "Số % trừ (cộng) 1 lần", "Số % trừ (cộng) lớn nhất", "Tổng số % trừ(cộng)", "% thưởng còn lại"
         };

            for (int i = 0; i < subHeaders.Length; i++)
            {
                var cell = sheet.Cells[headerRow2, i + 1];
                cell.Value = subHeaders[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(headerColor);
                cell.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                cell.Style.WrapText = true;
            }

            sheet.Row(headerRow1).Height = 30;
            sheet.Row(headerRow2).Height = 45;
            row = headerRow2 + 1;

            string[] keys = new[]
            {
             "STT",
             "RuleContent",
             "FirstMonth",
             "SecondMonth",
             "ThirdMonth",
             "TotalError",
             "MaxPercent",
             "PercentageAdjustment",
             "MaxPercentageAdjustment",
             "PercentBonus",
             "PercentRemaining",
             "EvaluationCode",
             "Note"
         };

            // Điền dữ liệu
            foreach (var item in data)
            {
                var dataRow = (IDictionary<string, object>)item;
                int pId = -2;
                if (dataRow.ContainsKey("ParentID") && dataRow["ParentID"] != null)
                {
                    int.TryParse(dataRow["ParentID"].ToString(), out pId);
                }
                // Tô màu xám cho cả dòng cha cấp 1 (ParentID=0/-1) VÀ dòng cha trung gian (có node con trỏ vào ID của nó)
                int rowId = -1;
                if (dataRow.ContainsKey("ID") && dataRow["ID"] != null)
                    int.TryParse(dataRow["ID"].ToString(), out rowId);
                bool isParent = pId == 0 || pId == -1
                    || (rowId > 0 && data.Any(c =>
                    {
                        var cr = (IDictionary<string, object>)c;
                        int cPid = -2;
                        if (cr.ContainsKey("ParentID") && cr["ParentID"] != null)
                            int.TryParse(cr["ParentID"].ToString(), out cPid);
                        return cPid == rowId;
                    }));

                for (int i = 0; i < keys.Length; i++)
                {
                    var cell = sheet.Cells[row, i + 1];
                    object val = dataRow.ContainsKey(keys[i]) ? dataRow[keys[i]] : null;

                    if (i == 0 || i == 1 || i == 11 || i == 12) // STT, RuleContent, EvaluationCode, Note are Text
                        cell.Value = val?.ToString();
                    else // Numeric columns
                    {
                        cell.Value = ConvertToDouble(val);
                        cell.Style.Numberformat.Format = "#,##0.00";
                    }

                    cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    if (isParent)
                    {
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }

                    if (i != 1) // Center everything except content
                        cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }
                string content = dataRow.ContainsKey("EvaluationContent") ? dataRow["EvaluationContent"]?.ToString() : "";
                bool isTotalSummary = content.IndexOf("TỔNG", StringComparison.OrdinalIgnoreCase) >= 0 || content.IndexOf("HỆ SỐ", StringComparison.OrdinalIgnoreCase) >= 0;
                if (isTotalSummary)
                {
                    sheet.Row(row).Height = 35;
                }
                else
                {
                    sheet.Row(row).Height = 22;
                }
                row++;
            }

            // Dòng tổng kết cho KPI Rule - Khớp đúng với logic FE Angular (updateRuleFooter)
            sheet.Cells[row, 1].Value = "";
            sheet.Cells[row, 2].Value = "TỔNG CỘNG";
            sheet.Cells[row, 2].Style.Font.Bold = true;

            // Chuẩn bị helper để đọc giá trị từ data rows
            Func<IDictionary<string, object>, string, double> getVal = (dr, key) =>
                ConvertToDouble(dr.ContainsKey(key) ? dr[key] : null);

            Func<IDictionary<string, object>, int> getPId = (dr) =>
            {
                int pid = -2;
                if (dr.ContainsKey("ParentID") && dr["ParentID"] != null)
                    int.TryParse(dr["ParentID"].ToString(), out pid);
                return pid;
            };

            Func<IDictionary<string, object>, int> getId = (dr) =>
            {
                int id = 0;
                if (dr.ContainsKey("ID") && dr["ID"] != null)
                    int.TryParse(dr["ID"].ToString(), out id);
                return id;
            };

            var allRows = data.Select(x => (IDictionary<string, object>)x).ToList();

            // MaxPercent: SUM từ TẤT CẢ node (AllNodesSummary = true, khớp FE)
            double totalMaxPercent = allRows.Sum(dr => getVal(dr, "MaxPercent"));

            // Root nodes: ParentID == 0 hoặc -1 (khớp FE: item.ParentID === 0 || null || undefined)
            var rootRows = allRows.Where(dr => { int p = getPId(dr); return p == 0 || p == -1; }).ToList();
            double totalPercentRemaining = rootRows.Sum(dr => getVal(dr, "PercentRemaining"));
            double totalPercentBonus = rootRows.Sum(dr => getVal(dr, "PercentBonus"));

            // Leaf KPI nodes: không có node nào trỏ ID của nó làm ParentID, và EvaluationCode bắt đầu bằng KPI (khớp FE isLeaf && isKPI)
            bool IsKpi(string code) => !string.IsNullOrEmpty(code)
                && code.ToUpper().StartsWith("KPI")
                && !code.Equals("KPINL", StringComparison.OrdinalIgnoreCase)
                && !code.Equals("KPINQ", StringComparison.OrdinalIgnoreCase);

            double empPoint = 0, tbpPoint = 0, bgdPoint = 0;
            foreach (var dr in allRows)
            {
                int nodeId = getId(dr);
                bool isLeaf = !allRows.Any(c => getPId(c) == nodeId);
                string evalCode = dr.ContainsKey("EvaluationCode") ? dr["EvaluationCode"]?.ToString() ?? "" : "";
                if (isLeaf && IsKpi(evalCode))
                {
                    double maxPct = getVal(dr, "MaxPercent");
                    double fm = getVal(dr, "FirstMonth");
                    double sm = getVal(dr, "SecondMonth");
                    double tm = getVal(dr, "ThirdMonth");
                    empPoint += Math.Round((fm * maxPct) / 5, 2);
                    tbpPoint += Math.Round((sm * maxPct) / 5, 2);
                    bgdPoint += Math.Round((tm * maxPct) / 5, 2);
                }
            }

            // FirstMonth/SecondMonth/ThirdMonth: khớp FE (firstMonthFooter = totalPercentRemaining - bgdPoint + empPoint)
            double firstMonthFooter = totalPercentRemaining - bgdPoint + empPoint;
            double secondMonthFooter = totalPercentRemaining - bgdPoint + tbpPoint;
            double thirdMonthFooter = totalPercentRemaining;

            // Gán giá trị vào dòng tổng theo đúng vị trí cột
            // col 3=FirstMonth, 4=SecondMonth, 5=ThirdMonth, 6=TotalError(bỏ qua), 7=MaxPercent, 8-9=adj(bỏ qua), 10=PercentBonus, 11=PercentRemaining
            double[] totalValues = new double[]
            {
                0,                     // i=2 → col 3: FirstMonth (calculated below)
                0,                     // i=3 → col 4: SecondMonth
                0,                     // i=4 → col 5: ThirdMonth
                0,                     // i=5 → col 6: TotalError (không tính)
                totalMaxPercent,       // i=6 → col 7: MaxPercent (sum ALL)
                0,                     // i=7 → col 8: PercentageAdjustment (không tính)
                0,                     // i=8 → col 9: MaxPercentageAdjustment (không tính)
                totalPercentBonus,     // i=9 → col 10: PercentBonus (root only)
                totalPercentRemaining, // i=10 → col 11: PercentRemaining (root only)
                0,                     // i=11 → col 12: EvaluationCode (text, bỏ qua)
                0                      // i=12 → col 13: Note (text, bỏ qua)
            };

            for (int i = 2; i < 13; i++)
            {
                var cell = sheet.Cells[row, i + 1];
                int idx = i - 2;
                double val = totalValues[idx];

                // FirstMonth/SecondMonth/ThirdMonth dùng công thức đặc biệt
                if (i == 2) val = firstMonthFooter;
                else if (i == 3) val = secondMonthFooter;
                else if (i == 4) val = thirdMonthFooter;

                // PercentRemaining (i=10, col 11): Hiển thị 2 dòng như FE
                // Dòng 1: "Điểm xếp loại: {totalPercentRemaining}" (từ root nodes)
                // Dòng 2: "Điểm cuối cùng: {totalPercentActual}" (từ KPIEmployeePoint.TotalPercentActual)
                if (i == 10)
                {
                    string line1 = $"Điểm xếp loại: {totalPercentRemaining:F2}";
                    string line2 = totalPercentActual > 0 ? $"Điểm cuối cùng: {totalPercentActual:F2}" : "";
                    cell.Value = string.IsNullOrEmpty(line2) ? line1 : $"{line1}\n{line2}";
                    cell.Style.WrapText = true;
                    cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                }
                else
                {
                    cell.Value = val;
                    cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    cell.Style.Numberformat.Format = "#,##0.00";
                }

                cell.Style.Font.Bold = true;
                cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
            }

            // Border và style cho cột nhãn
            sheet.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            sheet.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            sheet.Cells[row, 2].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            sheet.Cells[row, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
            // Tăng chiều cao dòng tổng nếu có 2 dòng text (có Điểm cuối cùng)
            sheet.Row(row).Height = totalPercentActual > 0 ? 40 : 25;
            row++;

            // Tự động điều chỉnh độ rộng cột
            sheet.Cells.AutoFitColumns();
            sheet.Column(2).Width = 60; // Nội dung đánh giá
        }

        [NonAction]
        private void FillTotalAVGSheet(OfficeOpenXml.ExcelWorksheet sheet, List<dynamic> dtSkill, List<dynamic> dtProf, List<dynamic> dtGen)
        {
            sheet.Cells.Style.Font.Name = "Tahoma";
            sheet.Cells.Style.Font.Size = 8.5f;

            int row = 1;
            sheet.Cells[row, 1].Value = "TỔNG HỢP ĐÁNH GIÁ";
            sheet.Cells[row, 1].Style.Font.Bold = true;
            sheet.Cells[row, 1].Style.Font.Size = 14;
            sheet.Cells[row, 1, row, 4].Merge = true;
            sheet.Cells[row, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            row += 2;

            string[] headers = new[] { "Người đánh giá", "Kỹ năng", "Chuyên môn", "Chung" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = sheet.Cells[row, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                cell.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            }
            row++;

            Func<List<dynamic>, string, double> getAvg = (data, field) =>
            {
                if (data == null || data.Count == 0) return 0;
                var list = data.Select(x => (IDictionary<string, object>)x).Where(x =>
                {
                    int pId = Convert.ToInt32(x.ContainsKey("ParentID") ? x["ParentID"] : -2);
                    return pId == 0 || pId == -1;
                }).ToList();
                if (list.Count == 0) return 0;
                double totalCoef = list.Sum(x => x.ContainsKey("Coefficient") ? ConvertToDouble(x["Coefficient"]) : 0);
                if (totalCoef == 0) return 0;
                double totalWeighted = list.Sum(x => (x.ContainsKey(field) ? ConvertToDouble(x[field]) : 0) * (x.ContainsKey("Coefficient") ? ConvertToDouble(x["Coefficient"]) : 0));
                return Math.Round(totalWeighted / totalCoef, 2);
            };

            string[] evalTypes = new[] { "Tự đánh giá", "Đánh giá của Trưởng/Phó BP", "Đánh giá của GĐ" };
            string[] fields = new[] { "EmployeeEvaluation", "TBPEvaluation", "BGDEvaluation" };

            for (int i = 0; i < evalTypes.Length; i++)
            {
                sheet.Cells[row, 1].Value = evalTypes[i];
                sheet.Cells[row, 2].Value = getAvg(dtSkill, fields[i]);
                sheet.Cells[row, 2].Style.Numberformat.Format = "#,##0.00";
                sheet.Cells[row, 3].Value = getAvg(dtProf, fields[i]);
                sheet.Cells[row, 3].Style.Numberformat.Format = "#,##0.00";
                sheet.Cells[row, 4].Value = getAvg(dtGen, fields[i]);
                sheet.Cells[row, 4].Style.Numberformat.Format = "#,##0.00";

                for (int col = 1; col <= 4; col++)
                {
                    sheet.Cells[row, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheet.Cells[row, col].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    if (col > 1) sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }
                row++;
            }
            sheet.Cells.AutoFitColumns();
        }

        [NonAction]
        private void FillTeamRuleSheet(OfficeOpenXml.ExcelWorksheet sheet, List<dynamic> data)
        {
            sheet.Cells.Style.Font.Name = "Tahoma";
            sheet.Cells.Style.Font.Size = 8.5f;

            int row = 1;
            //sheet.Cells[row, 1].Value = "TEAM RULE";
            //sheet.Cells[row, 1].Style.Font.Bold = true;
            //sheet.Cells[row, 1].Style.Font.Size = 14;
            //sheet.Cells[row, 1, row, 17].Merge = true; // Cột 1 -> 17
            //sheet.Cells[row, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            //row += 2;

            //// Header của bảng (Dòng 1: Group Headers)
            //row++;
            int headerRow1 = row;
            int headerRow2 = row + 1;

            Action<int, int, int, int, string> setHeader = (r1, c1, r2, c2, text) =>
            {
                var range = sheet.Cells[r1, c1, r2, c2];
                range.Merge = true;
                range.Value = text;
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                range.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                range.Style.WrapText = true;
            };

            // Dòng 1: Main Headers (Vertical Merge)
            setHeader(headerRow1, 1, headerRow2, 1, "STT");
            setHeader(headerRow1, 2, headerRow2, 2, "Thành viên");
            setHeader(headerRow1, 3, headerRow2, 3, "Vị trí");
            setHeader(headerRow1, 4, headerRow2, 4, "Nhóm");

            // Dòng 1: Group Headers
            setHeader(headerRow1, 5, headerRow1, 7, "Tuân thủ quy định");
            setHeader(headerRow1, 8, headerRow1, 9, "Tinh thần làm việc");

            // Dòng 1: Individual Columns (Vertical Merge)
            setHeader(headerRow1, 10, headerRow2, 10, "Làm mất, hỏng thiết bị");
            setHeader(headerRow1, 11, headerRow2, 11, "Chậm tiến độ...");
            setHeader(headerRow1, 12, headerRow2, 12, "Kỹ năng");
            setHeader(headerRow1, 13, headerRow2, 13, "Chuyên môn");
            setHeader(headerRow1, 14, headerRow2, 14, "PLC");
            setHeader(headerRow1, 15, headerRow2, 15, "Vision");
            setHeader(headerRow1, 16, headerRow2, 16, "Software");
            setHeader(headerRow1, 17, headerRow2, 17, "Đánh giá chung");

            // Dòng 2: Sub Headers
            string[] subHeaders = new[]
            {
             "Thời gian, giờ giấc", "5s, Quy trình quy định", "Chuẩn bị hàng, report",
             "Có thái độ không tốt...", "Thái độ với KH"
         };

            int[] subHeaderCols = new[] { 5, 6, 7, 8, 9 };
            for (int i = 0; i < subHeaders.Length; i++)
            {
                var cell = sheet.Cells[headerRow2, subHeaderCols[i]];
                cell.Value = subHeaders[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                cell.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                cell.Style.WrapText = true;
            }

            sheet.Row(headerRow1).Height = 30;
            sheet.Row(headerRow2).Height = 50;
            row = headerRow2 + 1;

            string[] keys = new[]
            {
             "STTTeam", "FullName", "Position", "Group",
             "TimeWork", "FiveS", "ReportWork",
             "ComplaneAndMissing", "CustomerComplaint", "Error4",
             "DeadlineDelay", "KPIKyNang", "KPIChuyenMon", "KPIPLC_Robot", "KPIVision", "KPISoftware", "KPIChung"
         };

            foreach (var item in data)
            {
                var dataRow = (IDictionary<string, object>)item;
                for (int i = 0; i < keys.Length; i++)
                {
                    sheet.Cells[row, i + 1].Value = dataRow.ContainsKey(keys[i]) ? dataRow[keys[i]]?.ToString() : "";
                    sheet.Cells[row, i + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheet.Cells[row, i + 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                }
                row++;
            }

            // Tự động dãn cột và căn chỉnh thủ công một số cột quan trọng
            sheet.Cells.AutoFitColumns();
            sheet.Column(1).Width = 5;     // STT
            sheet.Column(2).Width = 25;    // Họ tên
            sheet.Column(3).Width = 20;    // Chức vụ
            sheet.Column(4).Width = 15;    // Nhóm

            // Các cột nội dung đánh giá và KPI (Cột 5 -> 17)
            for (int i = 5; i <= 17; i++)
            {
                if (sheet.Column(i).Width < 12)
                    sheet.Column(i).Width = 12;
            }
        }

        [NonAction]
        private List<dynamic> CalculateKPIPoints(List<dynamic> data)
        {
            if (data == null || data.Count == 0) return data;

            // Chuyển sang Dictionary để có thể thêm/sửa cột
            var rows = data.Select(x => new Dictionary<string, object>((IDictionary<string, object>)x)).ToList();

            // Sắp xếp lại theo STT phân cấp (VD: 1 -> 1.1 -> 1.2 -> 1.10 -> 2)
            rows = rows.OrderBy(x =>
            {
                string stt = x.ContainsKey("STT") ? x["STT"]?.ToString()?.Trim() ?? "" : "";
                if (string.IsNullOrEmpty(stt)) return "99999";

                // Tách theo dấu chấm và pad từng phần để sort chính xác
                var parts = stt.Split('.').Select(p =>
                {
                    if (int.TryParse(p, out int val)) return val.ToString("D5");
                    return p.PadLeft(5, '0');
                });
                return string.Join(".", parts);
            }, StringComparer.Ordinal).ToList();

            // Lấy ra tất cả các ID
            var allIds = rows.Select(x => Convert.ToInt32(x.ContainsKey("ID") ? x["ID"] : 0)).ToHashSet();

            // Tính cho các node lá trước
            foreach (var row in rows)
            {
                int id = Convert.ToInt32(row.ContainsKey("ID") ? row["ID"] : 0);
                bool hasChild = rows.Any(x => Convert.ToInt32(x.ContainsKey("ParentID") ? x["ParentID"] : -1) == id);
                int parentId = Convert.ToInt32(row.ContainsKey("ParentID") ? row["ParentID"] : -1);

                if (parentId != 0 && parentId != -1 && !hasChild)
                {
                    double coef = ConvertToDouble(row.ContainsKey("Coefficient") ? row["Coefficient"] : 0);

                    double empPoint = ConvertToDouble(row.ContainsKey("EmployeePoint") ? row["EmployeePoint"] : 0);
                    double tbpPoint = ConvertToDouble(row.ContainsKey("TBPPoint") ? row["TBPPoint"] : 0);
                    double bgdPoint = ConvertToDouble(row.ContainsKey("BGDPoint") ? row["BGDPoint"] : 0);

                    row["EmployeeEvaluation"] = empPoint;
                    row["TBPEvaluation"] = tbpPoint;
                    row["BGDEvaluation"] = bgdPoint;

                    row["EmployeeCoefficient"] = Math.Round(empPoint * coef, 2);
                    row["TBPCoefficient"] = Math.Round(tbpPoint * coef, 2);
                    row["BGDCoefficient"] = Math.Round(bgdPoint * coef, 2);
                }
            }

            // Tính các node cha trung gian (ParentID != 0 && ParentID != -1) từ dưới lên
            var parentIds = rows.Where(x =>
            {
                int pId = Convert.ToInt32(x.ContainsKey("ParentID") ? x["ParentID"] : -1);
                return pId != 0 && pId != -1 && rows.Any(c => Convert.ToInt32(c.ContainsKey("ParentID") ? c["ParentID"] : -1) == Convert.ToInt32(x["ID"]));
            })
                                .OrderByDescending(x => (x.ContainsKey("STT") ? x["STT"]?.ToString() ?? "" : "").Length)
                                .Select(x => Convert.ToInt32(x["ID"]))
                                .Distinct()
                                .ToList();

            foreach (int pId in parentIds)
            {
                var parentRow = rows.FirstOrDefault(x => Convert.ToInt32(x["ID"]) == pId);
                if (parentRow == null) continue;

                var children = rows.Where(x => Convert.ToInt32(x.ContainsKey("ParentID") ? x["ParentID"] : -1) == pId).ToList();

                double totalCoef = children.Sum(x => ConvertToDouble(x.ContainsKey("Coefficient") ? x["Coefficient"] : 0));
                double totalEmpCoef = children.Sum(x => ConvertToDouble(x.ContainsKey("EmployeeCoefficient") ? x["EmployeeCoefficient"] : 0));
                double totalTBPCoef = children.Sum(x => ConvertToDouble(x.ContainsKey("TBPCoefficient") ? x["TBPCoefficient"] : 0));
                double totalBGDCoef = children.Sum(x => ConvertToDouble(x.ContainsKey("BGDCoefficient") ? x["BGDCoefficient"] : 0));

                if (totalCoef > 0)
                {
                    parentRow["EmployeeEvaluation"] = Math.Round(totalEmpCoef / totalCoef, 2);
                    parentRow["TBPEvaluation"] = Math.Round(totalTBPCoef / totalCoef, 2);
                    parentRow["BGDEvaluation"] = Math.Round(totalBGDCoef / totalCoef, 2);
                }
                else
                {
                    parentRow["EmployeeEvaluation"] = children.Count > 0 ? Math.Round(totalEmpCoef / children.Count, 2) : 0;
                    parentRow["TBPEvaluation"] = children.Count > 0 ? Math.Round(totalTBPCoef / children.Count, 2) : 0;
                    parentRow["BGDEvaluation"] = children.Count > 0 ? Math.Round(totalBGDCoef / children.Count, 2) : 0;
                }

                double parentCoef = ConvertToDouble(parentRow.ContainsKey("Coefficient") ? parentRow["Coefficient"] : 0);
                parentRow["EmployeeCoefficient"] = Math.Round(ConvertToDouble(parentRow["EmployeeEvaluation"]) * parentCoef, 2);
                parentRow["TBPCoefficient"] = Math.Round(ConvertToDouble(parentRow["TBPEvaluation"]) * parentCoef, 2);
                parentRow["BGDCoefficient"] = Math.Round(ConvertToDouble(parentRow["BGDEvaluation"]) * parentCoef, 2);
            }

            // Tính cho root node (ParentID == 0 hoặc -1)
            var rootRows = rows.Where(x =>
            {
                int pId = Convert.ToInt32(x.ContainsKey("ParentID") ? x["ParentID"] : -1);
                return pId == 0 || pId == -1;
            }).ToList();
            foreach (var rootRow in rootRows)
            {
                int rootId = Convert.ToInt32(rootRow["ID"]);
                var rootChildren = rows.Where(x => Convert.ToInt32(x.ContainsKey("ParentID") ? x["ParentID"] : -1) == rootId).ToList();

                double rootTotalCoef = rootChildren.Sum(x => ConvertToDouble(x.ContainsKey("Coefficient") ? x["Coefficient"] : 0));
                double rootTotalEmpCoef = rootChildren.Sum(x => ConvertToDouble(x.ContainsKey("EmployeeCoefficient") ? x["EmployeeCoefficient"] : 0));
                double rootTotalTBPCoef = rootChildren.Sum(x => ConvertToDouble(x.ContainsKey("TBPCoefficient") ? x["TBPCoefficient"] : 0));
                double rootTotalBGDCoef = rootChildren.Sum(x => ConvertToDouble(x.ContainsKey("BGDCoefficient") ? x["BGDCoefficient"] : 0));

                rootRow["Coefficient"] = rootTotalCoef > 0 ? rootTotalCoef : 1;
                rootRow["EmployeeCoefficient"] = Math.Round(rootTotalEmpCoef, 2);
                rootRow["TBPCoefficient"] = Math.Round(rootTotalTBPCoef, 2);
                rootRow["BGDCoefficient"] = Math.Round(rootTotalBGDCoef, 2);

                double divisor = rootTotalCoef > 0 ? rootTotalCoef : 1;
                rootRow["EmployeeEvaluation"] = Math.Round(rootTotalEmpCoef / divisor, 2);
                rootRow["TBPEvaluation"] = Math.Round(rootTotalTBPCoef / divisor, 2);
                rootRow["BGDEvaluation"] = Math.Round(rootTotalBGDCoef / divisor, 2);
            }

            return rows.Cast<dynamic>().ToList();
        }

        /// <summary>
        /// Chuyển đổi giá trị object sang double, trả về 0 nếu không hợp lệ
        /// </summary>
        /// <param name="value">Giá trị cần chuyển đổi</param>
        /// <returns>Giá trị double</returns>
        [NonAction]
        private double ConvertToDouble(object value)
        {
            if (value == null)
                return 0;

            if (double.TryParse(value.ToString(), out double result))
                return result;

            return 0;
        }

        #endregion Xuất Excel theo Team

        #region Lịch sử thao tác KPI

        /// <summary>
        /// Lấy lịch sử thao tác KPI của nhân viên theo kỳ đánh giá
        /// </summary>
        /// <param name="employeeID">ID nhân viên</param>
        /// <param name="kpiSessionID">ID kỳ đánh giá KPI (0 = tất cả kỳ)</param>
        [HttpGet("get-log-activity-kpi")]
        public async Task<IActionResult> GetLogActivityKpi(int employeeID, int kpiSessionID = 0)
        {
            try
            {
                // Lấy danh sách KPIExam theo kỳ đánh giá (nếu có)
                var examIds = new List<int>();
                if (kpiSessionID > 0)
                {
                    var exams = _kpiExamRepo.GetAll(x => x.KPISessionID == kpiSessionID && x.IsDeleted != true);
                    examIds = exams.Select(x => x.ID).ToList();
                }

                var session = kpiSessionID > 0 ? _kpiSessionRepo.GetByID(kpiSessionID) : null;
                var allLogs = _kpiEvaluationLogRepo.GetAll(x =>
                    x.EmployeeID == employeeID &&
                    x.IsDeleted != true).ToList();

                // Lấy log theo employeeID và lọc theo examIds nếu có
                IEnumerable<KPIEvaluationLog> logs;
                if (kpiSessionID > 0 && session != null)
                {
                    logs = allLogs.Where(x =>
                        examIds.Contains(x.KPIExamID ?? 0) ||
                        (x.KPIExamID == null && IsLogDateInSession(x.CreatedDate, session.YearEvaluation ?? 0, session.QuarterEvaluation ?? 0))
                    ).ToList();
                }
                else
                {
                    logs = allLogs;
                }

                // Map sang DTO để trả về frontend (dùng ActionType làm TypeLog cho đồng nhất với PONCC)
                var result = logs
                    .OrderByDescending(x => x.CreatedDate)
                    .Select(x => new
                    {
                        x.ID,
                        TypeLog = x.ActionType,
                        x.ContentLog,
                        x.CreatedBy,
                        x.CreatedDate
                    });

                return Ok(new { Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [NonAction]
        private bool IsLogDateInSession(DateTime? createdDate, int year, int quarter)
        {
            if (!createdDate.HasValue) return false;
            var date = createdDate.Value;
            int logYear = date.Year;
            int logMonth = date.Month;

            if (quarter == 1)
            {
                return logYear == year && (logMonth >= 1 && logMonth <= 4);
            }
            else if (quarter == 2)
            {
                return logYear == year && (logMonth >= 4 && logMonth <= 7);
            }
            else if (quarter == 3)
            {
                return logYear == year && (logMonth >= 7 && logMonth <= 10);
            }
            else if (quarter == 4)
            {
                return (logYear == year && logMonth >= 10) || (logYear == year + 1 && logMonth == 1);
            }
            return false;
        }

        #endregion Lịch sử thao tác KPI
    }
}