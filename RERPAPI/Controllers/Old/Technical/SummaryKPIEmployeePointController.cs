using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Technical.KPI;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RERPAPI.Controllers.Old.Technical
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SummaryKPIEmployeePointController : ControllerBase
    {
        private readonly DepartmentRepo _departmentRepo;
        private readonly EmployeeRepo _employeeRepo;
        private readonly KPIEmployeePointRepo _kpiEmployeePointRepo;
        private readonly KPIEmployeePointDetailRepo _kpiEmployeePointDetailRepo;
        private readonly KPISessionRepo _kpiSessionRepo;
        private readonly KPIEvaluationRuleRepo _kpiEvaluationRuleRepo;
        private readonly KPIPositionEmployeeRepo _kpiPositionEmployeeRepo;
        private readonly KPIExamRepo _kpiExamRepo;



        public SummaryKPIEmployeePointController(
            DepartmentRepo departmentRepo,
            EmployeeRepo employeeRepo,
            KPIEmployeePointRepo kpiEmployeePointRepo,
            KPIEmployeePointDetailRepo kpiEmployeePointDetailRepo,
            KPISessionRepo kpiSessionRepo,
            KPIEvaluationRuleRepo kpiEvaluationRuleRepo,
            KPIPositionEmployeeRepo kpiPositionEmployeeRepo,
            KPIExamRepo kpiExamRepo)
        {
            _departmentRepo = departmentRepo;
            _employeeRepo = employeeRepo;
            _kpiEmployeePointRepo = kpiEmployeePointRepo;
            _kpiEmployeePointDetailRepo = kpiEmployeePointDetailRepo;
            _kpiSessionRepo = kpiSessionRepo;
            _kpiEvaluationRuleRepo = kpiEvaluationRuleRepo;
            _kpiPositionEmployeeRepo = kpiPositionEmployeeRepo;
            _kpiExamRepo = kpiExamRepo;
        }

        [HttpGet("get-department")]
        public IActionResult GetDepartment()
        {
            try
            {
                var data = _departmentRepo
                    .GetAll(x => x.IsDeleted != true)
                    .OrderBy(x => x.STT);

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-employees")]
        public IActionResult LoadUser()
        {
            try
            {
                var list = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployee",
                                 new string[] { "@Status" },
                                 new object[] { 0 });
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("load-data")]
        public async Task<IActionResult> LoadData([FromBody] SummaryKPIEmployeePointRequest request)
        {
            try
            {
                int[] _departmentCoKhiLRs = new int[] { 10, 23 };
                string spName = _departmentCoKhiLRs.Contains(request.DepartmentID)
                    ? "spGetKPISumaryEvaluation"
                    : "spGetSummaryKPIEmployeePoint";
                var param = new
                {
                    Year = request.Year,
                    Quarter = request.Quarter,
                    DepartmentID = request.DepartmentID,
                    EmployeeID = request.EmployeeID,
                    Keyword = request.Keyword
                };
                var dataStore = await SqlDapper<object>.ProcedureToListTAsync(spName, param);

                return Ok(ApiResponseFactory.Success(dataStore, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-kpi-rule-detail")]
        public async Task<IActionResult> GetKPIRuleDetail(int kpiEmployeePointID)
        {
            try
            {
                var param = new
                {
                    KPIEmployeePointID = kpiEmployeePointID,
                };
                var dataStore = await SqlDapper<object>.ProcedureToListAsync("spGetEmployeeRulePointByKPIEmpPointIDNew", param);


                return Ok(ApiResponseFactory.Success(dataStore, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-kpi-exam-by-id")]
        public IActionResult GetKPIExamByID(int id)
        {
            try
            {
                var data = _kpiExamRepo
                    .GetByID(id);

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-kpi-team-summary")]
        public async Task<IActionResult> GetKPITeamSummary(int kpiEmployeePointID)
        {
            try
            {
                var param = new
                {
                    KPIEmployeePointID = kpiEmployeePointID,
                };
                var dataStore = await SqlDapper<object>.ProcedureToListAsync("spGetKpiRuleSumarizeTeamNew", param);

                return Ok(ApiResponseFactory.Success(dataStore, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-kpi-sumarize")]
        public async Task<IActionResult> GetKPISumarize(int kpiEmployeePointID)
        {
            try
            {
                //var data = SQLHelper<object>.ProcedureToList(
                //    "spGetSumarizebyKPIEmpPointIDNew",
                //    new[] { "@KPIEmployeePointID" },
                //    new object[] { kpiEmployeePointID }
                //);

                var param = new
                {
                    KPIEmployeePointID = kpiEmployeePointID,
                };
                var dataStore = await SqlDapper<object>.ProcedureToListAsync("spGetSumarizebyKPIEmpPointIDNew", param);

                return Ok(ApiResponseFactory.Success(dataStore, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        //[HttpPost("publish")]
        //public IActionResult Publish([FromBody] List<int> ids)
        //{
        //    try
        //    {
        //        if (ids == null || !ids.Any())
        //            return BadRequest(ApiResponseFactory.Fail(null, "Danh sách ID không hợp lệ"));

        //        foreach (var id in ids)
        //        {
        //            var entity = _kpiEmployeePointRepo.GetByID(id);
        //            if (entity == null) continue;

        //            entity.IsPublish = true;

        //            _kpiEmployeePointRepo.Update(entity);
        //        }

        //        return Ok(ApiResponseFactory.Success(null, "Duyệt thành công"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}

        //[HttpPost("unpublish")]
        //public IActionResult UnPublish([FromBody] List<int> ids)
        //{
        //    try
        //    {
        //        if (ids == null || !ids.Any())
        //            return BadRequest(ApiResponseFactory.Fail(null, "Danh sách ID không hợp lệ"));

        //        foreach (var id in ids)
        //        {
        //            var entity = _kpiEmployeePointRepo.GetByID(id);
        //            if (entity == null) continue;

        //            entity.IsPublish = false;

        //            _kpiEmployeePointRepo.Update(entity);
        //        }

        //        return Ok(ApiResponseFactory.Success(null, "Hủy duyệt thành công"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}

        //[HttpPost("save-actual")]
        //public IActionResult SaveActual([FromBody] Dictionary<int, decimal> data)
        //{
        //    try
        //    {
        //        foreach (var item in data)
        //        {
        //            var entity = _kpiEmployeePointRepo.GetByID(item.Key);
        //            if (entity == null) continue;

        //            entity.TotalPercentActual = item.Value;

        //            _kpiEmployeePointRepo.Update(entity);
        //        }

        //        return Ok(ApiResponseFactory.Success(null, "Lưu điểm thành công"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}

        [HttpPost("save-actual-new")]
        public IActionResult SaveActual([FromBody] List<SaveActualRequest> data)
        {
            try
            {
                if (data == null || !data.Any())
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));

                foreach (var item in data)
                {
                    var entity = _kpiEmployeePointRepo.GetByID(item.Id);
                    if (entity == null) continue;

                    entity.TotalPercentActual = item.TotalPercentActual;

                    if (item.IsPublish.HasValue)
                        entity.IsPublish = item.IsPublish.Value;

                    _kpiEmployeePointRepo.Update(entity);
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //[HttpPost("save-kpi-point-detail")]
        //public async Task<IActionResult> SaveKPIEmployeePointDetail([FromBody] List<KPIEmployeePointDetailSaveRequest> data)
        //{
        //    try
        //    {
        //        if (data == null || !data.Any())
        //            return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));

        //        foreach (var item in data)
        //        {
        //            var entity = new KPIEmployeePointDetail
        //            {
        //                KPIEmployeePointID = item.KPIEmployeePointID,
        //                KPIEvaluationRuleDetailID = item.KPIEvaluationRuleDetailID,
        //                FirstMonth = item.FirstMonth,
        //                SecondMonth = item.SecondMonth,
        //                ThirdMonth = item.ThirdMonth,
        //                PercentBonus = item.PercentBonus,
        //                PercentRemaining = item.PercentRemaining
        //            };

        //            await _kpiEmployeePointDetailRepo.CreateAsync(entity);
        //        }

        //        return Ok(ApiResponseFactory.Success(null, "Lưu chi tiết KPI thành công"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}

        [HttpPost("save-kpi-point-detail")]
        public async Task<IActionResult> SaveKPIEmployeePointDetail([FromBody] List<KPIEmployeePointDetailSaveRequest> data)
        {
            try
            {
                if (data == null || !data.Any())
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));

                var entities = data.Select(item => new KPIEmployeePointDetail
                {
                    KPIEmployeePointID = item.KPIEmployeePointID,
                    KPIEvaluationRuleDetailID = item.KPIEvaluationRuleDetailID,
                    FirstMonth = item.FirstMonth,
                    SecondMonth = item.SecondMonth,
                    ThirdMonth = item.ThirdMonth,
                    PercentBonus = item.PercentBonus,
                    PercentRemaining = item.PercentRemaining
                }).ToList();

                var result = await _kpiEmployeePointDetailRepo.CreateRangeAsync(entities);

                if (result > 0)
                {
                    return Ok(ApiResponseFactory.Success(entities,
                        $"Lưu thành công {result} bản ghi"));
                }

                return BadRequest(ApiResponseFactory.Fail(null, "Không có bản ghi nào được lưu"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpGet("get-kpi-ranking")]
        public async Task<IActionResult> GetKPIRanking(int year, int quarter, int departmentID)
        {
            try
            {
                var param = new
                {
                    Year = year,
                    Quarter = quarter,
                    DepartmentID = departmentID
                };
                var dataStore = await SqlDapper<object>.ProcedureToListAsync("spGetKPIRankingSummary", param);

                return Ok(ApiResponseFactory.Success(dataStore, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-kpi-employee-point-by-employee")]
        public async Task<IActionResult> GetKPIEmployeePoint(int employeeID, int year, int quarter)
        {
            try
            {
                var param = new
                {
                    EmployeeID = employeeID,
                    Year = year,
                    Quarter = quarter
                };
                var dataStore = await SqlDapper<object>.ProcedureToListAsync("spGetKPIEmployeePointByEmployee", param);

                return Ok(ApiResponseFactory.Success(dataStore, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-kpi-session")]
        public IActionResult GetKPISession(int year, int quarter)
        {
            try
            {
                var session = _kpiSessionRepo
                    .GetAll(x => x.YearEvaluation == year 
                              && x.QuarterEvaluation == quarter
                              && x.IsDeleted != true)
                    .FirstOrDefault();

                if (session == null)
                {
                    return Ok(ApiResponseFactory.Success(null, "Chưa tạo kỳ đánh giá"));
                }

                return Ok(ApiResponseFactory.Success(session, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-kpi-evaluation-rule")]
        public IActionResult GetKPIEvaluationRule(int kpiSessionID, int kpiPositionID)
        {
            try
            {
                var rule = _kpiEvaluationRuleRepo
                    .GetAll(x => x.KPISessionID == kpiSessionID
                              && x.KPIPositionID == kpiPositionID
                              && x.IsDeleted != true)
                    .FirstOrDefault();

                return Ok(ApiResponseFactory.Success(rule, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-kpi-evaluation-rule-by-id")]
        public IActionResult GetKPIEvaluationRuleById(int id)
        {
            try
            {
                var rule = _kpiEvaluationRuleRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(rule, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-kpi-position-by-employee")]
        public IActionResult GetKPIPositionByEmployee(int employeeID)
        {
            try
            {
                var position = _kpiPositionEmployeeRepo
                    .GetAll(x => x.EmployeeID == employeeID && x.IsDeleted != true)
                    .FirstOrDefault();

                return Ok(ApiResponseFactory.Success(position, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-kpi-employee-point-by-id")]
        public IActionResult GetKPIEmployeePointById(int id)
        {
            try
            {
                var entity = _kpiEmployeePointRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(entity, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-kpi-employee-point-detail")]
        public IActionResult GetKPIEmployeePointDetail(int kpiEmployeePointID)
        {
            try
            {
                var data = _kpiEmployeePointDetailRepo
                    .GetAll(x => x.KPIEmployeePointID == kpiEmployeePointID)
                    .ToList();

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("calculate-kpi-rule")]
        public async Task<IActionResult> CalculateKPIRule([FromBody] CalculateKPIRuleRequest request)
        {
            try
            {
                var session = _kpiSessionRepo
                    .GetAll(x => x.YearEvaluation == request.Year
                              && x.QuarterEvaluation == request.Quarter
                              && x.IsDeleted != true)
                    .FirstOrDefault();

                if (session == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy kỳ đánh giá"));

                var empPoint = _kpiEmployeePointRepo.GetByID(request.KPIEmployeePointID);
                if (empPoint == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy bản ghi KPI"));

                var ruleModel = _kpiEvaluationRuleRepo.GetByID(empPoint.KPIEvaluationRuleID ?? 0);
                if (ruleModel == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy quy tắc đánh giá"));

                bool isTBP = ruleModel.KPIPositionID == 5;

                // Lấy dữ liệu chi tiết
                var param = new { KPIEmployeePointID = request.KPIEmployeePointID };
                var dtKpiRule = await SqlDapper<KPIRuleDetailDTO>
                    .ProcedureToListTAsync("spGetEmployeeRulePointByKPIEmpPointIDNew", param);

                if (!dtKpiRule.Any())
                    return Ok(ApiResponseFactory.Success(null, "Không có dữ liệu chi tiết"));

                // Kiểm tra xem đã có detail chưa
                var lstDetails = _kpiEmployeePointDetailRepo
                    .GetAll(x => x.KPIEmployeePointID == request.KPIEmployeePointID)
                    .ToList();

                bool isAdminConfirm = false;
                try
                {
                    var summaryParam = new
                    {
                        Year = request.Year,
                        Quarter = request.Quarter,
                        DepartmentID = 0,
                        EmployeeID = empPoint.EmployeeID ?? 0,
                        Keyword = ""
                    };
                    var summaryData = await SqlDapper<dynamic>
                        .ProcedureToListTAsync("spGetSummaryKPIEmployeePoint", summaryParam);
                    var currentEmp = summaryData.FirstOrDefault(x => x.KPIEmployeePointID == request.KPIEmployeePointID);
                    if (currentEmp != null && currentEmp.IsAdminConfirm != null)
                    {
                        isAdminConfirm = currentEmp.IsAdminConfirm;
                    }
                }
                catch { }


                if (!lstDetails.Any())
                {
                    // Load lần đầu
                    await LoadPointRule(request.KPIEmployeePointID, dtKpiRule);
                }
                else if (!isAdminConfirm)
                {
                    // Load lại nếu chưa confirm
                    await LoadPointRuleLastMonth(request.KPIEmployeePointID, dtKpiRule, empPoint.EmployeeID ?? 0);
                }

                // Tính toán
                CalculatorNoError(dtKpiRule, empPoint.EmployeeID ?? 0);
                CalculatorPoint(dtKpiRule, isTBP);

                // Tính tổng điểm
                decimal totalPercent = dtKpiRule
                    .Where(x => x.ParentID == 0)
                    .Sum(x => x.PercentRemaining);

                string level = GetLevel(totalPercent);

                var result = new
                {
                    Details = dtKpiRule,
                    TotalPercent = totalPercent,
                    Level = level
                };

                return Ok(ApiResponseFactory.Success(result, "Tính toán thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-calculated-details")]
        public async Task<IActionResult> SaveCalculatedDetails([FromBody] SaveCalculatedDetailsRequest request)
        {
            try
            {
                if (request.Details == null || !request.Details.Any())
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));

                // Xóa chi tiết cũ nếu có
                var oldDetails = _kpiEmployeePointDetailRepo
                    .GetAll(x => x.KPIEmployeePointID == request.KPIEmployeePointID)
                    .ToList();

                // Chỉ xóa nếu có data cũ
                if (oldDetails.Any())
                {
                    foreach (var old in oldDetails)
                    {
                        await _kpiEmployeePointDetailRepo.DeleteAsync(old.ID);
                    }
                }


                // Lưu chi tiết mới
                foreach (var item in request.Details)
                {
                    var entity = new KPIEmployeePointDetail
                    {
                        KPIEmployeePointID = request.KPIEmployeePointID,
                        KPIEvaluationRuleDetailID = item.ID,
                        FirstMonth = item.FirstMonth,
                        SecondMonth = item.SecondMonth,
                        ThirdMonth = item.ThirdMonth,
                        PercentBonus = item.PercentBonus,
                        PercentRemaining = item.PercentRemaining
                    };

                    await _kpiEmployeePointDetailRepo.CreateAsync(entity);
                }

                // Cập nhật TotalPercent vào KPIEmployeePoint
                var empPoint = _kpiEmployeePointRepo.GetByID(request.KPIEmployeePointID);
                if (empPoint != null)
                {
                    empPoint.TotalPercent = request.TotalPercent;
                    //empPoint.TotalPercentText = GetLevel(request.TotalPercent);
                    _kpiEmployeePointRepo.Update(empPoint);
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu chi tiết thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        private void CalculatorNoError(List<KPIRuleDetailDTO> list, int employeeID)
        {
            string[] listCodes = { "MA01", "MA02", "MA03", "MA04", "MA05", "MA06", "MA07", "WorkLate", "NotWorking" };
            string[] listAdminCodes = { "AMA01", "AMA02", "AMA03", "AMA04", "AMA05", "AMA06", "AMA07", "AMA08", "AMA09", "AMA10", "AMA11", "AMA12", "AMA13", "AMA14", "AMA15", "AMA16", "AMA17", "AMA18", "AMA19", "WorkLate", "NotWorking" };

            var codesToUse = employeeID == 548 ? listAdminCodes : listCodes;

            var errorItems = list
                .Where(x => codesToUse.Contains(x.EvaluationCode))
                .ToList();

            decimal first = errorItems.Sum(x => x.FirstMonth);
            decimal second = errorItems.Sum(x => x.SecondMonth);
            decimal third = errorItems.Sum(x => x.ThirdMonth);

            var ma09 = list.FirstOrDefault(x => x.EvaluationCode == "MA09");
            if (ma09 != null)
            {
                ma09.FirstMonth = first;
                ma09.SecondMonth = second;
                ma09.ThirdMonth = third;
            }
        }

        private void CalculatorPoint(List<KPIRuleDetailDTO> dt, bool isTBP)
        {
            string[] lstTeamTBP = { "TEAM01", "TEAM02", "TEAM03" };

            for (int i = dt.Count - 1; i >= 0; i--)
            {
                var row = dt[i];
                string ruleCode = row.EvaluationCode?.ToUpper() ?? "";
                bool isDiemThuong = ruleCode == "THUONG";

                decimal maxPercentBonus = row.MaxPercent;
                decimal percentageAdjustment = row.PercentageAdjustment;
                decimal maxPercentageAdjustment = row.MaxPercentageAdjustment;

                int id = row.ID;
                var childRows = dt.Where(x => x.ParentID == id).ToList();

                if (childRows.Any())
                {
                    decimal totalPercentBonus = 0;
                    decimal totalPercentRemaining = 0;
                    bool isKPI = false;

                    foreach (var childRow in childRows)
                    {
                        string childRuleCode = childRow.EvaluationCode ?? "";
                        isKPI = childRuleCode.ToUpper().StartsWith("KPI");
                        totalPercentBonus += childRow.PercentBonus;
                        totalPercentRemaining += childRow.PercentRemaining;
                    }

                    if (lstTeamTBP.Contains(ruleCode) && isTBP)
                    {
                        row.TotalError = row.ThirdMonth;
                    }
                    else if (isKPI)
                    {
                        row.PercentRemaining = totalPercentRemaining;
                    }
                    else if (isDiemThuong)
                    {
                        row.PercentRemaining = maxPercentBonus > totalPercentBonus
                            ? totalPercentBonus
                            : maxPercentBonus;
                    }
                    else if (maxPercentBonus > 0)
                    {
                        row.PercentRemaining = maxPercentBonus > totalPercentBonus
                            ? maxPercentBonus - totalPercentBonus
                            : 0;
                    }
                    else
                    {
                        row.PercentBonus = totalPercentBonus;
                        row.PercentRemaining = totalPercentRemaining;
                    }

                    if (lstTeamTBP.Contains(ruleCode) && isTBP)
                    {
                        decimal bonus = row.ThirdMonth * percentageAdjustment;
                        row.PercentBonus = bonus > maxPercentageAdjustment
                            ? maxPercentageAdjustment
                            : bonus;
                    }
                    else if (maxPercentageAdjustment > 0)
                    {
                        row.PercentBonus = maxPercentageAdjustment > totalPercentBonus
                            ? totalPercentBonus
                            : maxPercentageAdjustment;
                    }
                }
                else
                {
                    decimal totalError = row.FirstMonth + row.SecondMonth + row.ThirdMonth;
                    row.TotalError = totalError;

                    if (ruleCode == "OT")
                        row.TotalError = (totalError / 3) >= 20 ? 1 : 0;

                    decimal totalPercentDeduction = percentageAdjustment * row.TotalError;
                    row.PercentBonus = maxPercentageAdjustment > 0
                        ? Math.Min(totalPercentDeduction, maxPercentageAdjustment)
                        : totalPercentDeduction;

                    if (ruleCode.StartsWith("KPI") && ruleCode != "KPINL" && ruleCode != "KPINQ")
                    {
                        row.TotalError = row.ThirdMonth;
                        row.PercentRemaining = row.TotalError * maxPercentBonus / 5;
                    }
                    else if (ruleCode.StartsWith("TEAMKPI"))
                    {
                        row.PercentBonus = row.TotalError * maxPercentageAdjustment / 5;
                    }
                    else if (ruleCode == "MA09")
                    {
                        row.PercentBonus = totalPercentDeduction > maxPercentageAdjustment
                            ? 0
                            : maxPercentageAdjustment - totalPercentDeduction;
                    }
                    else
                    {
                        row.PercentRemaining = row.TotalError * maxPercentBonus;
                    }
                }
            }
        }

        private async Task LoadPointRule(int empPointID, List<KPIRuleDetailDTO> dtKpiRule)
        {
            try
            {
                // Lấy dữ liệu tổng hợp
                var paramSumarize = new { KPIEmployeePointID = empPointID };
                var lstResult = await SqlDapper<KPISumarizeDTO>
                    .ProcedureToListTAsync("spGetSumarizebyKPIEmpPointIDNew", paramSumarize);

                // Lấy dữ liệu team
                var paramTeam = new { KPIEmployeePointID = empPointID };
                var dtTeam = await SqlDapper<TeamSummaryDTO>
                    .ProcedureToListTAsync("spGetKpiRuleSumarizeTeamNew", paramTeam);

                decimal timeWork = 0;
                decimal fiveS = 0;
                decimal reportWork = 0;
                decimal customerComplaint = 0;
                decimal deadlineDelay = 0;
                decimal teamKPIKyNang = 0;
                decimal teanKPIChung = 0;
                decimal teamKPIPLC = 0;
                decimal teamKPIVISION = 0;
                decimal teamKPISOFTWARE = 0;
                decimal missingTool = 0;

                if (dtTeam != null && dtTeam.Any())
                {
                    var timeWorks = dtTeam.Select(x => x.TimeWork).ToList();
                    var fiveSs = dtTeam.Select(x => x.FiveS).ToList();
                    var reportWorks = dtTeam.Select(x => x.ReportWork).ToList();
                    var customerComplaints = dtTeam.Select(x => x.ComplaneAndMissing).ToList(); 
                    var deadlineDelays = dtTeam.Select(x => x.DeadlineDelay).ToList();
                    var teamKPIKyNangs = dtTeam.Select(x => x.KPIKyNang).ToList();
                    var teanKPIChungs = dtTeam.Select(x => x.KPIChung).ToList();
                    var teamKPIPLCs = dtTeam.Select(x => x.KPIPLC).ToList();
                    var teamKPIVISIONs = dtTeam.Select(x => x.KPIVision).ToList();
                    var teamKPISOFTWAREs = dtTeam.Select(x => x.KPISoftware).ToList();
                    var missingTools = dtTeam.Select(x => x.MissingTool).ToList();

                    timeWork = timeWorks.Average() ?? 0;
                    fiveS = fiveSs.Average() ?? 0;
                    reportWork = reportWorks.Average() ?? 0;
                    customerComplaint = customerComplaints.Average() ?? 0;
                    deadlineDelay = deadlineDelays.Average() ?? 0;
                    teamKPIKyNang = teamKPIKyNangs.Average() ?? 0;
                    teanKPIChung = teanKPIChungs.Average() ?? 0;
                    teamKPIPLC = teamKPIPLCs.Average() ?? 0;
                    teamKPIVISION = teamKPIVISIONs.Average() ?? 0;
                    teamKPISOFTWARE = teamKPISOFTWAREs.Average() ?? 0;
                    missingTool = missingTools.Average() ?? 0;
                }

                // Tính MA11
                string[] lstCodeTBP = { "MA03", "MA04", "NotWorking", "WorkLate" };
                var ltsMA11 = lstResult.Where(p => lstCodeTBP.Contains(p.EvaluationCode?.Trim())).ToList();
                decimal totalErrorTBP = ltsMA11.Sum(p => p.FirstMonth + p.SecondMonth + p.ThirdMonth);

                lstResult.AddRange(new List<KPISumarizeDTO>
                {
                    new KPISumarizeDTO { EvaluationCode = "TEAM01", ThirdMonth = timeWork },
                    new KPISumarizeDTO { EvaluationCode = "TEAM02", ThirdMonth = fiveS },
                    new KPISumarizeDTO { EvaluationCode = "TEAM03", ThirdMonth = reportWork },
                    new KPISumarizeDTO { EvaluationCode = "TEAM04", ThirdMonth = customerComplaint + missingTool + deadlineDelay },
                    new KPISumarizeDTO { EvaluationCode = "TEAM05", ThirdMonth = customerComplaint },
                    new KPISumarizeDTO { EvaluationCode = "TEAM06", ThirdMonth = missingTool },
                    new KPISumarizeDTO { EvaluationCode = "TEAMKPIKYNANG", ThirdMonth = teamKPIKyNang },
                    new KPISumarizeDTO { EvaluationCode = "TEAMKPIChung", ThirdMonth = teanKPIChung },
                    new KPISumarizeDTO { EvaluationCode = "TEAMKPIPLC", ThirdMonth = teamKPIPLC },
                    new KPISumarizeDTO { EvaluationCode = "TEAMKPIVISION", ThirdMonth = teamKPIVISION },
                    new KPISumarizeDTO { EvaluationCode = "TEAMKPISOFTWARE", ThirdMonth = teamKPISOFTWARE },
                    new KPISumarizeDTO { EvaluationCode = "MA11", ThirdMonth = totalErrorTBP }
                    //new KPISumarizeDTO { EvaluationCode = "TEAMKPICHUYENMON", ThirdMonth = teamKPIChuyenMon }
                });

                // Map dữ liệu vào dtKpiRule
                foreach (var item in lstResult)
                {
                    var row = dtKpiRule.FirstOrDefault(x => x.EvaluationCode == item.EvaluationCode);
                    if (row == null) continue;

                    row.FirstMonth = item.FirstMonth;
                    row.SecondMonth = item.SecondMonth;
                    row.ThirdMonth = item.ThirdMonth;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"LoadPointRule Error: {ex.Message}", ex);
            }
        }

        private async Task LoadPointRuleLastMonth(int empPointID, List<KPIRuleDetailDTO> dtKpiRule, int employeeID)
        {
            try
            {
                // Lấy dữ liệu tổng hợp
                var paramSumarize = new { KPIEmployeePointID = empPointID };
                var lstResult = await SqlDapper<KPISumarizeDTO>
                    .ProcedureToListTAsync("spGetSumarizebyKPIEmpPointIDNew", paramSumarize);

                // Lấy dữ liệu team
                var paramTeam = new { KPIEmployeePointID = empPointID };
                var dtTeam = await SqlDapper<TeamSummaryDTO>
                    .ProcedureToListTAsync("spGetKpiRuleSumarizeTeamNew", paramTeam);

                // Tính trung bình các chỉ số team
                decimal timeWork = dtTeam.Any() ? dtTeam.Average(x => x.TimeWork ?? 0) : 0;
                decimal fiveS = dtTeam.Any() ? dtTeam.Average(x => x.FiveS ?? 0) : 0;
                decimal reportWork = dtTeam.Any() ? dtTeam.Average(x => x.ReportWork ?? 0) : 0;
                decimal customerComplaint = dtTeam.Any() ? dtTeam.Average(x => x.ComplaneAndMissing ?? 0) : 0;
                decimal deadlineDelay = dtTeam.Any() ? dtTeam.Average(x => x.DeadlineDelay ?? 0) : 0;
                decimal teamKPIKyNang = dtTeam.Any() ? dtTeam.Average(x => x.KPIKyNang ?? 0) : 0;
                decimal teanKPIChung = dtTeam.Any() ? dtTeam.Average(x => x.KPIChung ?? 0) : 0;
                decimal teamKPIPLC = dtTeam.Any() ? dtTeam.Average(x => x.KPIPLC ?? 0) : 0;
                decimal teamKPIVISION = dtTeam.Any() ? dtTeam.Average(x => x.KPIVision ?? 0) : 0;
                decimal teamKPISOFTWARE = dtTeam.Any() ? dtTeam.Average(x => x.KPISoftware ?? 0) : 0;
                decimal missingTool = dtTeam.Any() ? dtTeam.Average(x => x.MissingTool ?? 0) : 0;
                decimal teamKPIChuyenMon = dtTeam.Any() ? dtTeam.Average(x => x.KPIChuyenMon ?? 0) : 0;

                // Tính MA11
                string[] lstCodeTBP = { "MA03", "MA04", "NotWorking", "WorkLate" };
                var ltsMA11 = lstResult.Where(p => lstCodeTBP.Contains(p.EvaluationCode?.Trim())).ToList();
                decimal totalErrorTBP = ltsMA11.Sum(p => p.FirstMonth + p.SecondMonth + p.ThirdMonth);

                // Thêm dữ liệu team vào lstResult
                lstResult.AddRange(new List<KPISumarizeDTO>
                {
                    new KPISumarizeDTO { EvaluationCode = "TEAM01", ThirdMonth = timeWork },
                    new KPISumarizeDTO { EvaluationCode = "TEAM02", ThirdMonth = fiveS },
                    new KPISumarizeDTO { EvaluationCode = "TEAM03", ThirdMonth = reportWork },
                    new KPISumarizeDTO { EvaluationCode = "TEAM04", ThirdMonth = customerComplaint + missingTool + deadlineDelay },
                    new KPISumarizeDTO { EvaluationCode = "TEAM05", ThirdMonth = customerComplaint },
                    new KPISumarizeDTO { EvaluationCode = "TEAM06", ThirdMonth = deadlineDelay }, // Khác với LoadPointRule (deadlineDelay thay vì missingTool)
                    new KPISumarizeDTO { EvaluationCode = "TEAMKPIKYNANG", ThirdMonth = teamKPIKyNang },
                    new KPISumarizeDTO { EvaluationCode = "TEAMKPIChung", ThirdMonth = teanKPIChung },
                    new KPISumarizeDTO { EvaluationCode = "TEAMKPIPLC", ThirdMonth = teamKPIPLC },
                    new KPISumarizeDTO { EvaluationCode = "TEAMKPIVISION", ThirdMonth = teamKPIVISION },
                    new KPISumarizeDTO { EvaluationCode = "TEAMKPISOFTWARE", ThirdMonth = teamKPISOFTWARE },
                    new KPISumarizeDTO { EvaluationCode = "MA11", ThirdMonth = totalErrorTBP },
                    new KPISumarizeDTO { EvaluationCode = "TEAMKPICHUYENMON", ThirdMonth = teamKPIChuyenMon }
                });

                // Chỉ cập nhật ThirdMonth
                foreach (var item in lstResult)
                {
                    var row = dtKpiRule.FirstOrDefault(x => x.EvaluationCode == item.EvaluationCode);
                    if (row == null) continue;
                    row.ThirdMonth = item.ThirdMonth;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"LoadPointRuleLastMonth Error: {ex.Message}", ex);
            }
        }

        private decimal CalculateTotalPercent(List<KPIRuleDetailDTO> list)
        {
            return list
                .Where(x => x.ParentID == 0)
                .Sum(x => x.PercentRemaining);
        }

        private string GetLevel(decimal totalPercent)
        {
            if (totalPercent < 60) return "D";
            if (totalPercent < 65) return "C-";
            if (totalPercent < 70) return "C";
            if (totalPercent < 75) return "C+";
            if (totalPercent < 80) return "B-";
            if (totalPercent < 85) return "B";
            if (totalPercent < 90) return "B+";
            if (totalPercent < 95) return "A-";
            if (totalPercent < 100) return "A";
            return "A+";
        }

        public class SaveActualRequest
        {
            public int Id { get; set; }
            public decimal TotalPercentActual { get; set; }
            public bool? IsPublish { get; set; } // null = không thay đổi
        }

        public class SummaryKPIEmployeePointRequest
        {
            public int Year { get; set; }
            public int Quarter { get; set; }
            public int DepartmentID { get; set; }
            public int EmployeeID { get; set; }
            public string Keyword { get; set; } = "";
        }

        public class KPIEmployeePointDetailSaveRequest
        {
            public int KPIEmployeePointID { get; set; }
            public int KPIEvaluationRuleDetailID { get; set; }
            public decimal FirstMonth { get; set; }
            public decimal SecondMonth { get; set; }
            public decimal ThirdMonth { get; set; }
            public decimal PercentBonus { get; set; }
            public decimal PercentRemaining { get; set; }
        }

        public class KPIRuleDetailDTO
        {
            public int ID { get; set; }
            public string? STT { get; set; }
            public int ParentID { get; set; }

            public string? RuleContent { get; set; }
            public string? FormulaCode { get; set; }

            public decimal MaxPercentageAdjustment { get; set; }
            public decimal MaxPercent { get; set; }
            public decimal PercentageAdjustment { get; set; }

            public string? RuleNote { get; set; }
            public string? Note { get; set; }

            public int EmpPointDetailID { get; set; }
            public int KPIEmployeePointID { get; set; }
            public int KPIEvaluationRuleDetailID { get; set; }

            public decimal PercentBonus { get; set; }
            public decimal PercentRemaining { get; set; }

            public string? EvaluationCode { get; set; }

            public decimal FirstMonth { get; set; }
            public decimal SecondMonth { get; set; }
            public decimal ThirdMonth { get; set; }

            public decimal TotalError { get; set; }
        }

        public class KPISumarizeDTO
        {
            public string? EvaluationCode { get; set; }
            public decimal FirstMonth { get; set; }
            public decimal SecondMonth { get; set; }
            public decimal ThirdMonth { get; set; }
        }

        public class TeamSummaryDTO
        {
            public decimal? TimeWork { get; set; }
            public decimal? FiveS { get; set; }
            public decimal? ReportWork { get; set; }
            public decimal? ComplaneAndMissing { get; set; }
            public decimal? DeadlineDelay { get; set; }
            public decimal? KPIKyNang { get; set; }
            public decimal? KPIChung { get; set; }
            public decimal? KPIPLC { get; set; }
            public decimal? KPIVision { get; set; }
            public decimal? KPISoftware { get; set; }
            public decimal? MissingTool { get; set; }
            public decimal? KPIChuyenMon { get; set; }
        }

        public class CalculateKPIRuleRequest
        {
            public int KPIEmployeePointID { get; set; }
            public int Year { get; set; }
            public int Quarter { get; set; }
        }

        public class SaveCalculatedDetailsRequest
        {
            public int KPIEmployeePointID { get; set; }
            public decimal TotalPercent { get; set; }
            public List<KPIRuleDetailDTO> Details { get; set; } = new();
        }


    }
}
