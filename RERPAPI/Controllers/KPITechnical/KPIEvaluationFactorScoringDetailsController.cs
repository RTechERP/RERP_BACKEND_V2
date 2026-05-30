using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.KPITech;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.KPITech;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Technical.KPI;
using System.IO;

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
        KPIExamPositionRepo _kpiExamPositionRepo;
        KPICriterionRepo _kpiCriterionRepo;
        KPIEmployeePointDetailRepo _kpiEmployeePointDetailRepo;
        KPISumaryEvaluationRepo _kpiSumaryEvaluationRepo;
        UserTeamRepo _userTeamRepo;
        EmployeeRepo _employeeRepo;
        KPIEmployeeTeamRepo _kPIEmployeeTeamRepo;
        KPIEmployeeTeamLinkRepo _kPIEmployeeTeamLinkRepo;
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
            KPIEmployeeTeamLinkRepo kPIEmployeeTeamLinkRepo)
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
        #endregion
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
        #region save Datarule
        [HttpPost("save-data-rule")]
        public async Task<IActionResult> SaveDataRule([FromBody] SaveDataRuleRequestParam request)
        {
            try
            {
                KPIEmployeePoint master = _kpiEmployeePointRepo.GetByID(request.employeeID);
                await _kpiEmployeePointDetailRepo.DeleteByAttributeAsync("KPIEmployeePointID", request.employeeID);

                master.TotalPercent = request.totalPercentRemaining;
                master.Status = 2;
                await _kpiEmployeePointRepo.UpdateAsync(master);

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
                    await _kpiEmployeePointDetailRepo.CreateAsync(detail);
                }
                return Ok(ApiResponseFactory.Success(true, "Lưu dữ liệu rule thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
        #region save Data kpi 
        [HttpPost("save-data-kpi")]
        public async Task<IActionResult> SaveDataKPI([FromBody] SaveDataKPIRequestParam request)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                //validate
                if (request.KPISessionID == 0 || request.KPISessionID == null)
                { return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn kỳ đánh giá")); }
                if (request.KPIExamID == 0 || request.KPIExamID == null)
                { return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn bài đánh giá")); }
                if (request.employeeID == 0 || request.employeeID == null)
                { return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn nhân viên")); }

                //lưu dữ liệu kpi kỹ năng
                // Vòng lặp 1: Xử lý kpiKyNang (tương ứng treeData)
                foreach (var item in request.kpiKyNang)
                {
                    if (item.ID < 0) continue;
                    KPIEvaluationPoint model = _kpiEvaluationPointRepo.GetByID(item.ID);

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
                    if (model.ID > 0) await _kpiEvaluationPointRepo.UpdateWithNullAsync(model);
                    else await _kpiEvaluationPointRepo.CreateAsync(model);
                }

                // Vòng lặp 2: Xử lý kpichuyenmon (tương ứng treeData2)
                foreach (var item in request.kpiChuyenMon)
                {
                    if (item.ID < 0) continue;
                    KPIEvaluationPoint model = _kpiEvaluationPointRepo.GetByID(item.ID);

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


                    if (model.ID > 0) await _kpiEvaluationPointRepo.UpdateWithNullAsync(model);
                    else await _kpiEvaluationPointRepo.CreateAsync(model);
                }

                // Vòng lặp 3: Xử lý kpichung (tương ứng treeData3)
                foreach (var item in request.kpiChung)
                {
                    if (item.ID < 0) continue;
                    KPIEvaluationPoint model = _kpiEvaluationPointRepo.GetByID(item.ID);

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
                    if (model.ID > 0) await _kpiEvaluationPointRepo.UpdateWithNullAsync(model);
                    else
                        await _kpiEvaluationPointRepo.CreateAsync(model);
                }
                // Lưu thông tin tổng hợp đánh giá
                foreach (var item in request.kpiSumaryEvaluation)
                {
                    KPISumaryEvaluation sumaryModel = _kpiSumaryEvaluationRepo.GetAll(x => x.EmployeeID == request.employeeID && x.KPIExamID == request.KPIExamID && x.SpecializationType == item.SpecializationType).FirstOrDefault() ?? new KPISumaryEvaluation();

                    sumaryModel.SpecializationType = item.SpecializationType;
                    sumaryModel.EmployeeID = request.employeeID;
                    sumaryModel.KPIExamID = request.KPIExamID;
                    sumaryModel.EmployeePoint = item.EmployeePoint;
                    sumaryModel.TBPPoint = item.TBPPoint;
                    sumaryModel.BGDPoint = item.BGDPoint;
                    if (sumaryModel.ID > 0) await _kpiSumaryEvaluationRepo.UpdateAsync(sumaryModel);
                    else await _kpiSumaryEvaluationRepo.CreateAsync(sumaryModel);
                }
                return Ok(ApiResponseFactory.Success(true, "Lưu dữ liệu KPI thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
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
        #endregion
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
        #endregion

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
        #endregion
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
        #endregion

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
        #endregion

        #region BATCH RECALCULATE KPI
        [HttpPost("batch-recalculate-kpi")]
        public async Task<IActionResult> BatchRecalculateKPI([FromBody] RecalcKPIBatchRequestParam request)
        {
            try
            {
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
                    if (masterPoint != null && masterPoint.IsPublish == true)
                    {
                        skippedCount++;
                        continue;
                    }

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
                        if (dbDetail != null)
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

                    updatedCount++;
                }

                return Ok(ApiResponseFactory.Success(new { updatedCount, skippedCount },
                    $"Đã cập nhật thành công {updatedCount} nhân viên. Bỏ qua {skippedCount} nhân viên đã chốt."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
    }
}


