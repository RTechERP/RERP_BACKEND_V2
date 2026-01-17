using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.Record.Chart;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;
using RERPAPI.Repo.GenericEntity.Technical.KPI;
using System.IO;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.KPITechnical
{
    [Route("api/[controller]")]
    [ApiController]
    public class KPIEvaluationEmployeeController : ControllerBase
    {
        KPIEvaluationPointRepo _kpiEvaluationPointRepo;
        KPISessionRepo _kpiSessionRepo;
        KPIEmployeePointRepo _kpiEmployeePointRepo;
        KPIPositionRepo _kpiPositionRepo;
        KPIPositionEmployeeRepo _kpiPositionEmployeeRepo;
        KPIEvaluationRuleRepo _kpiEvaluationRuleRepo;
        public KPIEvaluationEmployeeController(KPIEvaluationPointRepo kpiEvaluationPointRepo, KPISessionRepo kpiSessionRepo, KPIEmployeePointRepo kpiEmployeePointRepo, KPIPositionRepo kpiPositionRepo, KPIPositionEmployeeRepo kpiPositionEmployeeRepo, KPIEvaluationRuleRepo kpiEvaluationRuleRepo   )
        {
            _kpiEvaluationPointRepo = kpiEvaluationPointRepo;
            _kpiSessionRepo = kpiSessionRepo;
            _kpiEmployeePointRepo = kpiEmployeePointRepo;
            _kpiPositionRepo = kpiPositionRepo;
            _kpiPositionEmployeeRepo = kpiPositionEmployeeRepo;
            _kpiEvaluationRuleRepo = kpiEvaluationRuleRepo;
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
        #endregion
        #region load dữ liệu kpi session
        [HttpGet("get-data-kpi-session")]
        public async Task<IActionResult> getDataKPISession(int year, int departmentID, string? keyword)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetKPISession"
                   , new string[] { "@Year", "@Keywords", "@DepartmentID" }
                   , new object[] { year, keyword ?? "", departmentID });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
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
        #endregion
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

                List<KPIEvaluationPoint> lst = SQLHelper<KPIEvaluationPoint>.ProcedureToListModel("spGetKPIEvaluationPoint"
                   , new string[] { "@KPIExamID", "@EmployeeID" }
                   , new object[] { kpiExamID, currentUser.EmployeeID });
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

                List<KPIEvaluationPoint> lst = SQLHelper<KPIEvaluationPoint>.ProcedureToListModel("spGetKPIEvaluationPoint"
                  , new string[] { "@KPIExamID", "@EmployeeID" }
                  , new object[] { kpiExamID, currentUser.EmployeeID });
                foreach (KPIEvaluationPoint item in lst)
                {
                    item.Status = 1;
                    item.DateEmployeeConfirm = DateTime.Now;
                    await _kpiEvaluationPointRepo.UpdateAsync(item);
                }
                return Ok(ApiResponseFactory.Success(null, "Xác nhận thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion

        #region load dữ liệu combobox kỳ đánh giá 
        [HttpGet("get-combobox-kpi-session")]
        public async Task<IActionResult> getComboboxKPISession(int year, int departmentID)
        {
            try
            {
                var data = _kpiSessionRepo.GetAll().Where(x => x.IsDeleted == false && x.DepartmentID == departmentID).ToList();
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
        #region load dữ liệu vị trí theo kỳ 
        [HttpGet("get-position-employee")]
        public async Task<IActionResult> GetPostionEmployee(int kpiSessionID)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                List<KPIPositionEmployee> employee = SQLHelper<KPIPositionEmployee>.ProcedureToListModel("spGetEmployeeInKPISession", new string[] { "@KPISessionID", "@EmployeeID" },
                                                                                                  new object[] { kpiSessionID, currentUser.EmployeeID });
                return Ok(ApiResponseFactory.Success(employee, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion


        #region load dữ liệu cobobox team (kpi)
        [HttpGet("get-combobox-team-kpi")]
        public async Task<IActionResult> getComboboxTeamKPI(int kpiSessionId, int departmentID)
        {
            try
            {
                KPISession kpiSession = _kpiSessionRepo.GetByID(kpiSessionId);

                var data = SQLHelper<object>.ProcedureToList("spGetALLKPIEmployeeTeam"
                 , new string[] { "@YearValue", "@QuarterValue", "@DepartmentID" }
                 , new object[] { kpiSession.YearEvaluation, kpiSession.QuarterEvaluation, departmentID });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion

        #region load dữ liệu bài đánh giá theo KPISESSIONID
        [HttpGet("get-kpi-exam-by-kpisessionid")]
        public async Task<IActionResult> getKpiExamByKsID(int kpiSessionId, int departmentID)
        {
            try
            {
                KPISession kpiSession = _kpiSessionRepo.GetByID(kpiSessionId);

                var data = SQLHelper<object>.ProcedureToList("spGetKPIExamByKPISessionID"
                 , new string[] { "@KPISessionID", "@DepartmentID" }
                 , new object[] { kpiSessionId, departmentID });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
        #region load danh sách nhân viên đánh giá KPI
        [HttpGet("get-list-employee-kpi-evaluation")]
        public async Task<IActionResult> getListEmployeeKPIEvaluation(int kpiExamID, int status, int departmentID, int userTeamID, string? keyword)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetAllEmployeeKPIEvaluated"
                 , new string[] { "@EvaluationType", "@DepartmentID", "@Keywords", "@Status", "@UserTeamID", "@KPIExamID" }
                 , new object[] { 1, departmentID, keyword ?? "", status, userTeamID, kpiExamID });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion


        #region load dữ liệu KPI kỹ năng , chuyên môn , chung , rule
        [HttpGet("load-kpi-kynang")]
        public async Task<IActionResult> LoadKPIKyNang(int kpiExamID, bool isPublic, int employeeID)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetAllKPIEvaluationPoint"
                 , new string[] { "@EmployeeID", "@EvaluationType", "@KPIExamID", "@IsPulbic" }
                 , new object[] { employeeID, 1, kpiExamID, isPublic });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("load-kpi-chung")]
        public async Task<IActionResult> LoadKPIChung(int kpiExamID, bool isPublic, int employeeID)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetAllKPIEvaluationPoint"
                 , new string[] { "@EmployeeID", "@EvaluationType", "@KPIExamID", "@IsPulbic" }
                 , new object[] { employeeID, 3, kpiExamID, isPublic });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("load-kpi-chuyenmon")]
        public async Task<IActionResult> LoadKPIChuyenMon(int kpiExamID, bool isPublic, int employeeID)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetAllKPIEvaluationPoint"
                 , new string[] { "@EmployeeID", "@EvaluationType", "@KPIExamID", "@IsPulbic" }
                 , new object[] { employeeID, 2, kpiExamID, isPublic });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), "Lấy dữ liệu thành công"));
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
                var data1 = SQLHelper<object>.ProcedureToList("spGetKpiRuleSumarizeTeamNew"
                 , new string[] { "@KPIEmployeePointID" }
                 , new object[] { kpiEmpPoint.ID });

                var data2 = SQLHelper<object>.ProcedureToList("spGetEmployeeRulePointByKPIEmpPointIDNew"
              , new string[] { "@KPIEmployeePointID", "@IsPublic" }
              , new object[] { kpiEmpPoint.ID, isPublic });

                var dtTeam = SQLHelper<object>.GetListData(data1, 0);
                var dtKpiRule = SQLHelper<object>.GetListData(data2, 0);
               
                return Ok(ApiResponseFactory.Success(new { dtTeam , dtKpiRule }, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
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
                KPIEmployeePoint model = _kpiEmployeePointRepo.GetAll().FirstOrDefault(x => x.EmployeeID == empID && x.KPIEvaluationRuleID == ruleID && x.IsDelete == false) ?? new KPIEmployeePoint();
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
        #endregion
    }
}
