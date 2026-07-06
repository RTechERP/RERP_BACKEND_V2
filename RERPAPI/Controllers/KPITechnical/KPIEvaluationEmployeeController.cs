using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.KPITech;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Technical.KPI;

namespace RERPAPI.Controllers.KPITechnical
{
    [Route("api/[controller]")]
    [ApiController]
    public class KPIEvaluationEmployeeController : ControllerBase
    {
        private KPIEvaluationPointRepo _kpiEvaluationPointRepo;
        private KPISessionRepo _kpiSessionRepo;
        private KPIEmployeePointRepo _kpiEmployeePointRepo;
        private KPIPositionRepo _kpiPositionRepo;
        private KPIPositionEmployeeRepo _kpiPositionEmployeeRepo;
        private KPIEvaluationRuleRepo _kpiEvaluationRuleRepo;
        private KPIEmployeePointDetailRepo _kpiEmployeePointDetailRepo;
        private KPIEvaluationLogRepo _kpiEvaluationLogRepo;
        private KPIExamRepo _kpiExamRepo;
        private KPIExamPositionRepo _kpiExamPositionRepo;
        private KPIEvaluationRuleDetailRepo _kpiEvaluationRuleDetailRepo;
        private KPIEvaluationRepo _kpiEvaluationRepo;

        public KPIEvaluationEmployeeController(
            KPIEvaluationPointRepo kpiEvaluationPointRepo,
            KPISessionRepo kpiSessionRepo,
            KPIEmployeePointRepo kpiEmployeePointRepo,
            KPIPositionRepo kpiPositionRepo,
            KPIPositionEmployeeRepo kpiPositionEmployeeRepo,
            KPIEvaluationRuleRepo kpiEvaluationRuleRepo,
            KPIEmployeePointDetailRepo kpiEmployeePointDetailRepo,
            KPIEvaluationLogRepo kpiEvaluationLogRepo,
            KPIExamRepo kpiExamRepo,
            KPIExamPositionRepo kpiExamPositionRepo,
            KPIEvaluationRuleDetailRepo kpiEvaluationRuleDetailRepo,
            KPIEvaluationRepo kpiEvaluationRepo)
        {
            _kpiEvaluationPointRepo = kpiEvaluationPointRepo;
            _kpiSessionRepo = kpiSessionRepo;
            _kpiEmployeePointRepo = kpiEmployeePointRepo;
            _kpiPositionRepo = kpiPositionRepo;
            _kpiPositionEmployeeRepo = kpiPositionEmployeeRepo;
            _kpiEvaluationRuleRepo = kpiEvaluationRuleRepo;
            _kpiEmployeePointDetailRepo = kpiEmployeePointDetailRepo;
            _kpiEvaluationLogRepo = kpiEvaluationLogRepo;
            _kpiExamRepo = kpiExamRepo;
            _kpiExamPositionRepo = kpiExamPositionRepo;
            _kpiEvaluationRuleDetailRepo = kpiEvaluationRuleDetailRepo;
            _kpiEvaluationRepo = kpiEvaluationRepo;
        }

        #region load dữ liệu combobox team

        [HttpGet("get-combobox-team")]
        public async Task<IActionResult> getComboboxTeam(int kpiSession)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var data = SQLHelper<object>.ProcedureToList("spGetPoistionByEmployeeID"
                   , new string[] { "@KPISessionID", "@EmployeeID" }
                   , new object[] { kpiSession, currentUser.EmployeeID });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion load dữ liệu combobox team

        #region load dữ liệu kpi session

        [HttpGet("get-data-kpi-session")]
        public async Task<IActionResult> getDataKPISession(int year, int departmentID, string? keyword)
        {
            try
            {
                var param = new
                {
                    Year = year,
                    Keywords = keyword ?? "",
                    DepartmentID = departmentID,
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetKPISession", param);

                //var data = SQLHelper<object>.ProcedureToList("spGetKPISession"
                //   , new string[] { "@Year", "@Keywords", "@DepartmentID" }
                //   , new object[] { year, keyword ?? "", departmentID });
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion load dữ liệu kpi session

        #region load dữ liệu kpi exam

        [HttpGet("get-data-kpi-exam")]
        public async Task<IActionResult> getDataKPIEvaluationEmployee(int employeeID, int kpiSessionID)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetKPIExam"
                   , new string[] { "@EmployeeID", "@KPISessionID" }
                   , new object[] { employeeID, kpiSessionID });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion load dữ liệu kpi exam

        #region Xác nhận hoàn thành

        [HttpGet("check-complete")]
        public async Task<IActionResult> CheckCompelete(int kpiExamID)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                if (kpiExamID <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn bài đánh giá!"));
                }
                var param = new
                {
                    KPIExamID = kpiExamID,
                    EmployeeID = currentUser.EmployeeID,
                };
                List<KPIEvaluationPoint> lst = await SqlDapper<KPIEvaluationPoint>.ProcedureToListTAsync("spGetKPIEvaluationPoint", param);

                //List<KPIEvaluationPoint> lst = SQLHelper<KPIEvaluationPoint>.ProcedureToListModel("spGetKPIEvaluationPoint"
                //   , new string[] { "@KPIExamID", "@EmployeeID" }
                //   , new object[] { kpiExamID, currentUser.EmployeeID });
                if (lst.Count <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng Đánh giá KPI trước khi hoàn thành!"));
                }
                return Ok(ApiResponseFactory.Success(lst, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("confirm-success-kpi")]
        public async Task<IActionResult> ConfrimSuccessKPI(int kpiExamID)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var param = new
                {
                    KPIExamID = kpiExamID,
                    EmployeeID = currentUser.EmployeeID,
                };
                List<KPIEvaluationPoint> lst = await SqlDapper<KPIEvaluationPoint>.ProcedureToListTAsync("spGetKPIEvaluationPoint", param);
                //List<KPIEvaluationPoint> lst = SQLHelper<KPIEvaluationPoint>.ProcedureToListModel("spGetKPIEvaluationPoint"
                //  , new string[] { "@KPIExamID", "@EmployeeID" }
                //  , new object[] { kpiExamID, currentUser.EmployeeID });
                foreach (KPIEvaluationPoint item in lst)
                {
                    item.Status = 1;
                    item.DateEmployeeConfirm = DateTime.Now;
                    await _kpiEvaluationPointRepo.UpdateAsync(item);
                }
                try
                {
                    var log = new KPIEvaluationLog
                    {
                        KPIExamID = kpiExamID,
                        EmployeeID = currentUser.EmployeeID,
                        ActionType = "NV xác nhận hoàn thành",
                        ContentLog = $"{currentUser.FullName} đã xác nhận hoàn thành đánh giá",
                        CreatedBy = currentUser.LoginName,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    await _kpiEvaluationLogRepo.CreateAsync(log);
                }
                catch (Exception) { }
                return Ok(ApiResponseFactory.Success(null, "Xác nhận thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion Xác nhận hoàn thành

        #region load dữ liệu combobox kỳ đánh giá

        [HttpGet("get-combobox-kpi-session")]
        public async Task<IActionResult> getComboboxKPISession(int year, int departmentID)
        {
            try
            {
                var data = _kpiSessionRepo.GetAll(x => x.IsDeleted == false && x.DepartmentID == departmentID).OrderByDescending(x => x.CreatedDate).ToList();
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion load dữ liệu combobox kỳ đánh giá

        #region load dữ liệu vị trí theo kỳ

        [HttpGet("get-position-employee")]
        public async Task<IActionResult> GetPostionEmployee(int kpiSessionID)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var param = new
                {
                    KPISessionID = kpiSessionID,
                    EmployeeID = currentUser.EmployeeID,
                };
                List<KPIPositionEmployee> employee = await SqlDapper<KPIPositionEmployee>.ProcedureToListTAsync("spGetEmployeeInKPISession", param);
                //List<KPIPositionEmployee> employee = SQLHelper<KPIPositionEmployee>.ProcedureToListModel("spGetEmployeeInKPISession", new string[] { "@KPISessionID", "@EmployeeID" },
                //                                                                                  new object[] { kpiSessionID, currentUser.EmployeeID });
                return Ok(ApiResponseFactory.Success(employee, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion load dữ liệu vị trí theo kỳ

        #region chọn vị trí trong kỳ đánh giá

        [HttpPost("choice-position")]
        public async Task<IActionResult> ChoicePosition(int positionID)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                if (positionID == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn vị trí của bạn!"));
                }
                KPIPositionEmployee newModel = new KPIPositionEmployee()
                {
                    KPIPosiotionID = positionID,
                    EmployeeID = currentUser.EmployeeID,
                };
                await _kpiPositionEmployeeRepo.CreateAsync(newModel);
                try
                {
                    var position = _kpiPositionRepo.GetByID(positionID);
                    int? resolvedExamID = null;
                    if (position != null && position.KPISessionID.HasValue)
                    {
                        var employeeExam = (from exam in _kpiExamRepo.GetAll(x => x.KPISessionID == position.KPISessionID.Value && x.IsDeleted == false)
                                            join ep in _kpiExamPositionRepo.GetAll(x => x.IsDeleted == false) on exam.ID equals ep.KPIExamID
                                            where ep.KPIPositionID == positionID
                                            select exam).FirstOrDefault();
                        resolvedExamID = employeeExam?.ID;
                        if (resolvedExamID == null)
                        {
                            var fallbackExam = _kpiExamRepo.GetAll(x => x.KPISessionID == position.KPISessionID.Value && x.IsDeleted != true).FirstOrDefault();
                            resolvedExamID = fallbackExam?.ID;
                        }
                    }

                    var positionName = position?.PositionName ?? "";
                    var log = new KPIEvaluationLog
                    {
                        KPIExamID = resolvedExamID,
                        EmployeeID = currentUser.EmployeeID,
                        ActionType = "NV xác nhận vị trí",
                        ContentLog = $"{currentUser.FullName} đã xác nhận vị trí: {positionName}",
                        CreatedBy = currentUser.LoginName,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    await _kpiEvaluationLogRepo.CreateAsync(log);
                }
                catch (Exception) { }
                return Ok(ApiResponseFactory.Success(newModel, "Chọn vị trí thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion chọn vị trí trong kỳ đánh giá

        #region load dữ liệu cobobox team (kpi)

        [HttpGet("get-combobox-team-kpi")]
        public async Task<IActionResult> getComboboxTeamKPI(int kpiSessionId, int departmentID)
        {
            try
            {
                KPISession kpiSession = _kpiSessionRepo.GetByID(kpiSessionId);
                var param = new
                {
                    YearValue = kpiSession.YearEvaluation,
                    QuarterValue = kpiSession.QuarterEvaluation,
                    DepartmentID = departmentID,
                };

                var data = await SqlDapper<object>.ProcedureToListAsync("spGetALLKPIEmployeeTeam", param);
                //var data = SQLHelper<object>.ProcedureToList("spGetALLKPIEmployeeTeam"
                // , new string[] { "@YearValue", "@QuarterValue", "@DepartmentID" }
                // , new object[] { kpiSession.YearEvaluation, kpiSession.QuarterEvaluation, departmentID });
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion load dữ liệu cobobox team (kpi)

        #region load dữ liệu bài đánh giá theo KPISESSIONID

        [HttpGet("get-kpi-exam-by-kpisessionid")]
        public async Task<IActionResult> getKpiExamByKsID(int kpiSessionId, int departmentID)
        {
            try
            {
                KPISession kpiSession = _kpiSessionRepo.GetByID(kpiSessionId);
                var param = new
                {
                    KPISessionID = kpiSessionId,
                    DepartmentID = departmentID,
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetKPIExamByKPISessionID", param);
                //var data = SQLHelper<object>.ProcedureToList("spGetKPIExamByKPISessionID"
                // , new string[] { "@KPISessionID", "@DepartmentID" }
                // , new object[] { kpiSessionId, departmentID });
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion load dữ liệu bài đánh giá theo KPISESSIONID

        #region load danh sách nhân viên đánh giá KPI

        [HttpGet("get-list-employee-kpi-evaluation")]
        public async Task<IActionResult> getListEmployeeKPIEvaluation(int kpiExamID, int status, int departmentID, int userTeamID, string? keyword)
        {
            try
            {
                var param = new
                {
                    EvaluationType = 1,
                    DepartmentID = departmentID,
                    Keywords = keyword ?? "",
                    Status = status,
                    UserTeamID = userTeamID,
                    KPIExamID = kpiExamID
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetAllEmployeeKPIEvaluated", param);
                //var data = SQLHelper<object>.ProcedureToList("spGetAllEmployeeKPIEvaluated"
                // , new string[] { "@EvaluationType", "@DepartmentID", "@Keywords", "@Status", "@UserTeamID", "@KPIExamID" }
                // , new object[] { 1, departmentID, keyword ?? "", status, userTeamID, kpiExamID });
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion load danh sách nhân viên đánh giá KPI

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
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-ispublish")]
        public async Task<IActionResult> GetISPublish(int kpiExamID, bool isPublic, int employeeID, int sessionID)
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
                KPIEmployeePoint empPoint = _kpiEmployeePointRepo.GetByID(empPointId);

                return Ok(ApiResponseFactory.Success(empPoint, "Lấy dữ liệu thành công"));
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
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("load-kpi-rule-and-team")]
        public async Task<IActionResult> LoadKPIRule(int kpiExamID, bool isPublic, int employeeID, int sessionID)
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
                KPIEmployeePoint empPoint = _kpiEmployeePointRepo.GetByID(empPointId);

                if (rule.ID <= 0)
                {
                    isPublic = false;
                }

                KPIEmployeePoint kpiEmpPoint = _kpiEmployeePointRepo.GetByID(empPoint.ID);
                var param = new
                {
                    KPIEmployeePointID = kpiEmpPoint.ID
                };
                var data1 = await SqlDapper<object>.ProcedureToListAsync("spGetKpiRuleSumarizeTeamNew_TNB", param);
                //var data1 = SQLHelper<object>.ProcedureToList("spGetKpiRuleSumarizeTeamNew"
                // , new string[] { "@KPIEmployeePointID" }
                // , new object[] { kpiEmpPoint.ID });
                var param2 = new
                {
                    KPIEmployeePointID = kpiEmpPoint.ID,
                    IsPublic = isPublic,
                };
                var data2 = await SqlDapper<spGetEmployeeRulePointByKPIEmpPointIDNewResultDTO>.ProcedureToListTAsync("spGetEmployeeRulePointByKPIEmpPointIDNew_TNB", param2);
                //  var data2 = SQLHelper<object>.ProcedureToList("spGetEmployeeRulePointByKPIEmpPointIDNew"
                //, new string[] { "@KPIEmployeePointID", "@IsPublic" }
                //, new object[] { kpiEmpPoint.ID, isPublic });
                List<KPIEmployeePointDetail> lst = _kpiEmployeePointDetailRepo.GetAll(x => x.KPIEmployeePointID == empPoint.ID);

                var dtTeam = data1;
                var dtKpiRule = data2;

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

        #region lấy điểm cuối cùng

        [HttpGet("get-final-point")]
        public async Task<IActionResult> GetFinalPoint(int employeeID, int sessionID)
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
                KPIEmployeePoint empPoint = _kpiEmployeePointRepo.GetByID(empPointId);
                //var data = SQLHelper<object>.ProcedureToList("spGetFinalKPIEmployeePoint"
                // , new string[] { "@KPIExamID", "@EmployeeID" }
                // , new object[] { kpiExamID, employeeID });
                return Ok(ApiResponseFactory.Success(empPoint, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion lấy điểm cuối cùng

        #region LoadPointRuleNew

        [HttpGet("load-point-rule-new2")]
        public async Task<IActionResult> LoadPointRuleNew(int kpiExamID, bool isPublic, int employeeID, int sessionID)
        {
            try
            {
                /*
                // Check if cache exists and is locked
                var session = _kpiSessionRepo.GetByID(sessionID);
                bool isCacheLocked = false;
                if (session != null)
                {
                    isCacheLocked = _kpiEvaluationSummaryCacheRepo.GetAll(x =>
                        x.EmployeeID == employeeID &&
                        x.QuarterValue == session.QuarterEvaluation &&
                        x.YearValue == session.YearEvaluation &&
                        x.IsLocked == true
                    ).Any();
                }

                if (isCacheLocked && session != null)
                {
                    var cacheParam = new
                    {
                        EmployeeID = employeeID,
                        QuarterValue = session.QuarterEvaluation,
                        YearValue = session.YearEvaluation
                    };
                    var cachedData = await SqlDapper<object>.ProcedureToListAsync("spGetKPIEvaluationSummaryFromCache", cacheParam);
                    return Ok(ApiResponseFactory.Success(cachedData, "Lấy dữ liệu từ Cache thành công"));
                }
                */

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
                KPIEmployeePoint empPoint = _kpiEmployeePointRepo.GetByID(empPointId);
                var param = new
                {
                    KPIEmployeePointID = empPoint.ID,
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetSumarizebyKPIEmpPointIDNew_TNB", param);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion LoadPointRuleNew

        #region load point rule last month

        [HttpGet("load-point-rule-last-month")]
        public async Task<IActionResult> LoadPointRuleLastMonthNew(int kpiExamID, bool isPublic, int employeeID, int sessionID)
        {
            try
            {
                /*
                // Check if cache exists and is locked
                var session = _kpiSessionRepo.GetByID(sessionID);
                bool isCacheLocked = false;
                if (session != null)
                {
                    isCacheLocked = _kpiEvaluationSummaryCacheRepo.GetAll(x =>
                        x.EmployeeID == employeeID &&
                        x.QuarterValue == session.QuarterEvaluation &&
                        x.YearValue == session.YearEvaluation &&
                        x.IsLocked == true
                    ).Any();
                }

                if (isCacheLocked && session != null)
                {
                    var cacheParam = new
                    {
                        EmployeeID = employeeID,
                        QuarterValue = session.QuarterEvaluation,
                        YearValue = session.YearEvaluation
                    };
                    var cachedData = await SqlDapper<object>.ProcedureToListAsync("spGetKPIEvaluationSummaryFromCache", cacheParam);
                    return Ok(ApiResponseFactory.Success(cachedData, "Lấy dữ liệu từ Cache thành công last month"));
                }
                */

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
                KPIEmployeePoint empPoint = _kpiEmployeePointRepo.GetByID(empPointId);
                var param = new
                {
                    KPIEmployeePointID = empPoint.ID,
                    IsPublic = isPublic
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetSumarizebyKPIEmpPointIDNew_TNB", param);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công last month thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion load point rule last month


        [HttpGet("get-summary-cache")]
        public async Task<IActionResult> GetSummaryCache(int employeeID, int quarter, int year)
        {
            try
            {
                // 1. Tìm vị trí nhân viên và Session tương ứng theo quý/năm đã chọn
                var kpiPositionEmployees = _kpiPositionEmployeeRepo.GetAll(x => x.EmployeeID == employeeID && x.IsDeleted == false).ToList();
                var positionIds = kpiPositionEmployees.Select(pe => pe.KPIPosiotionID).ToList();

                var kpiPositions = _kpiPositionRepo.GetAll(p => positionIds.Contains(p.ID) && p.IsDeleted == false).ToList();
                var sessionIds = kpiPositions.Select(p => p.KPISessionID).ToList();

                var session = _kpiSessionRepo.GetAll(s => sessionIds.Contains(s.ID) && s.QuarterEvaluation == quarter && s.YearEvaluation == year && s.IsDeleted == false)
                    .FirstOrDefault();

                // Fallback nếu nhân viên chưa được gán vị trí cụ thể trong quý/năm
                if (session == null)
                {
                    session = _kpiSessionRepo.GetAll(x => x.QuarterEvaluation == quarter && x.YearEvaluation == year && x.IsDeleted == false)
                        .FirstOrDefault();
                }

                if (session == null)
                {
                    return Ok(ApiResponseFactory.Success(new List<object>(), "Không tìm thấy session cho quý " + quarter + " năm " + year));
                }

                // 2. Tìm vị trí chính xác của nhân viên trong Session đã xác định
                var sessionPositions = _kpiPositionRepo.GetAll(x => x.KPISessionID == session.ID && x.IsDeleted == false).ToList();
                var empPosition = (from p in sessionPositions
                                   join pe in kpiPositionEmployees on p.ID equals pe.KPIPosiotionID
                                   select pe).FirstOrDefault() ?? new KPIPositionEmployee();

                // 3. Tìm Rule tương ứng
                var rule = _kpiEvaluationRuleRepo.GetAll(x => x.KPISessionID == session.ID && x.KPIPositionID == (empPosition.KPIPosiotionID > 0 ? empPosition.KPIPosiotionID : 1) && x.IsDeleted == false)
                    .FirstOrDefault() ?? new KPIEvaluationRule();

                // 4. Lấy hoặc tự động khởi tạo KPIEmployeePointID
                int empPointId = await GetKPIEmployeePointID(rule.ID, employeeID);
                if (empPointId <= 0)
                {
                    return Ok(ApiResponseFactory.Success(new List<object>(), "Không tìm thấy hoặc tạo được KPI Employee Point"));
                }

                // 5. Gọi store gốc để tính toán trực tiếp số lượng lỗi
                var calcParam = new { KPIEmployeePointID = empPointId, IsPublic = 1 };
                List<KPISumarizeDTO> lstResult = await SqlDapper<KPISumarizeDTO>.ProcedureToListTAsync("spGetSumarizebyKPIEmpPointIDNew_TNB", calcParam);

                // 6. Nhóm theo EvaluationCode để lọc trùng lặp
                var distinctResult = lstResult
                    .Where(x => !string.IsNullOrEmpty(x.EvaluationCode))
                    .GroupBy(x => x.EvaluationCode)
                    .Select(g => g.First())
                    .ToList();

                // 7. Lấy map RuleContent theo EvaluationCode trong Rule của nhân viên
                var ruleDetails = _kpiEvaluationRuleDetailRepo.GetAll(rd => rd.KPIEvaluationRuleID == rule.ID && rd.IsDeleted == false).ToList();
                var evaluations = _kpiEvaluationRepo.GetAll(e => e.IsDeleted == false).ToList();

                var ruleContentMap = (from rd in ruleDetails
                                      join e in evaluations on rd.KPIEvaluationID equals e.ID
                                      group rd by e.EvaluationCode into g
                                      select new
                                      {
                                          EvaluationCode = g.Key,
                                          RuleContent = g.Max(x => x.RuleContent)
                                      }).ToDictionary(x => x.EvaluationCode, x => x.RuleContent);

                // 8. Lọc các EvaluationCode chỉ thuộc Rule của nhân viên và gán tên tiêu chí hiển thị
                var result = distinctResult
                    .Where(x => ruleContentMap.ContainsKey(x.EvaluationCode))
                    .Select(x => new
                    {
                        EvaluationCode = x.EvaluationCode,
                        EvaluationName = ruleContentMap[x.EvaluationCode] ?? "",
                        FirstMonth = x.FirstMonth,
                        SecondMonth = x.SecondMonth,
                        ThirdMonth = x.ThirdMonth
                    })
                    .ToList();

                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu tính toán trực tiếp thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-error-details-by-evaluation")]
        public async Task<IActionResult> GetErrorDetailsByEvaluation(int employeeID, int quarter, int year, string evaluationCode)
        {
            try
            {
                var param = new
                {
                    EmployeeID = employeeID,
                    QuarterValue = quarter,
                    YearValue = year,
                    EvaluationCode = evaluationCode
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetKPIErrorEmployeeDetailsByEvaluation", param);
                return Ok(ApiResponseFactory.Success(data, "Lấy chi tiết lỗi thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}