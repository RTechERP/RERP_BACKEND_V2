using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.Technical.KPI;

namespace RERPAPI.Controllers.KPITechnical
{
    [Route("api/[controller]")]
    [ApiController]

    public class KPIEvaluationFactorScoringDetailsController : ControllerBase
    {
        KPIEvaluationPointRepo _kpiEvaluationPointRepo;
        KPISessionRepo _kpiSessionRepo;
        KPIEmployeePointRepo _kpiEmployeePointRepo;
        KPIPositionRepo _kpiPositionRepo;
        KPIPositionEmployeeRepo _kpiPositionEmployeeRepo;
        KPIEvaluationRuleRepo _kpiEvaluationRuleRepo;
        KPIExamRepo _kpiExamRepo;
        public KPIEvaluationFactorScoringDetailsController(KPIEvaluationPointRepo kpiEvaluationPointRepo, KPISessionRepo kpiSessionRepo, KPIEmployeePointRepo kpiEmployeePointRepo, KPIPositionRepo kpiPositionRepo, KPIPositionEmployeeRepo kpiPositionEmployeeRepo, KPIEvaluationRuleRepo kpiEvaluationRuleRepo, KPIExamRepo kpiExamRepo)
        {
            _kpiEvaluationPointRepo = kpiEvaluationPointRepo;
            _kpiSessionRepo = kpiSessionRepo;
            _kpiEmployeePointRepo = kpiEmployeePointRepo;
            _kpiPositionRepo = kpiPositionRepo;
            _kpiPositionEmployeeRepo = kpiPositionEmployeeRepo;
            _kpiEvaluationRuleRepo = kpiEvaluationRuleRepo;
            _kpiExamRepo = kpiExamRepo;
        }

        #region lấy dữ liệu combobox bài đánh giá 
        [HttpGet("get-combobox-exam")]
        public async Task<IActionResult> GetComboboxExam(int kpiSession)
        {
            try {
                List<KPIExam> data = _kpiExamRepo.GetAll(x => x.KPISessionID == kpiSession && x.IsDeleted == false);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
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
        #endregion
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
        #endregion

    }
}
