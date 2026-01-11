using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;
using RERPAPI.Repo.GenericEntity.Technical.KPI;

namespace RERPAPI.Controllers.KPITechnical
{
    [Route("api/[controller]")]
    [ApiController]
    public class KPIEvaluationEmployeeController : ControllerBase
    {
        KPIEvaluationPointRepo _kpiEvaluationPointRepo;
        KPISessionRepo _kpiSessionRepo;
        public KPIEvaluationEmployeeController(KPIEvaluationPointRepo kpiEvaluationPointRepo, KPISessionRepo kpiSessionRepo)
        {
            _kpiEvaluationPointRepo = kpiEvaluationPointRepo;
            _kpiSessionRepo = kpiSessionRepo;
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
                 , new object[] { 1, departmentID, keyword ?? "", status, userTeamID,kpiExamID });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
    }
}
