using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Ocsp;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.KPITech;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.HRM;
using RERPAPI.Model.Param.KPITech;
using RERPAPI.Repo.GenericEntity.Technical.KPI;
using System.Data;
using System.IO;

namespace RERPAPI.Controllers.KPITechnical
{
    [Route("api/[controller]")]
    [ApiController]
    public class KPIEvaluationFactorScoringController : ControllerBase
    {
        KPIEvaluationPointRepo _kpiEvaluationPointRepo;
        KPISessionRepo _kpiSessionRepo;
        KPIEmployeePointRepo _kpiEmployeePointRepo;
        KPIPositionRepo _kpiPositionRepo;
        KPIPositionEmployeeRepo _kpiPositionEmployeeRepo;
        KPIEvaluationRuleRepo _kpiEvaluationRuleRepo;
        KPIEmployeePointDetailRepo _kpiEmployeePointDetailRepo;
        public KPIEvaluationFactorScoringController(KPIEvaluationPointRepo kpiEvaluationPointRepo, KPISessionRepo kpiSessionRepo, KPIEmployeePointRepo kpiEmployeePointRepo, KPIPositionRepo kpiPositionRepo, KPIPositionEmployeeRepo kpiPositionEmployeeRepo, KPIEvaluationRuleRepo kpiEvaluationRuleRepo, KPIEmployeePointDetailRepo kPIEmployeePointDetailRepo)
        {
            _kpiEvaluationPointRepo = kpiEvaluationPointRepo;
            _kpiSessionRepo = kpiSessionRepo;
            _kpiEmployeePointRepo = kpiEmployeePointRepo;
            _kpiPositionRepo = kpiPositionRepo;
            _kpiPositionEmployeeRepo = kpiPositionEmployeeRepo;
            _kpiEvaluationRuleRepo = kpiEvaluationRuleRepo;
            _kpiEmployeePointDetailRepo = kPIEmployeePointDetailRepo;
        }
        #region load dữ liệu KPI kỹ năng , chuyên môn , chung , rule
        [HttpGet("load-kpi-kynang")]
        public async Task<IActionResult> LoadKPIKyNang(int kpiExamID, bool isPublic, int employeeID)
        {
            try
            {
                var param = new
                {
                    EmployeeID = employeeID,
                    EvaluationType = 1,
                    KPIExamID = kpiExamID,
                    IsPulbic = isPublic,
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetAllKPIEvaluationPoint", param);

                //var data = SQLHelper<object>.ProcedureToList("spGetAllKPIEvaluationPoint"
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
        public async Task<IActionResult> LoadKPIChung(int kpiExamID, bool isPublic, int employeeID)
        {
            try
            {
                var param = new
                {
                    EmployeeID = employeeID,
                    EvaluationType = 3,
                    KPIExamID = kpiExamID,
                    IsPulbic = isPublic,
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetAllKPIEvaluationPoint", param);
                //var data = SQLHelper<object>.ProcedureToList("spGetAllKPIEvaluationPoint"
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
        public async Task<IActionResult> LoadKPIChuyenMon(int kpiExamID, bool isPublic, int employeeID)
        {
            try
            {
                var param = new
                {
                    EmployeeID = employeeID,
                    EvaluationType = 2,
                    KPIExamID = kpiExamID,
                    IsPulbic = isPublic,
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetAllKPIEvaluationPoint", param);
                //var data = SQLHelper<object>.ProcedureToList("spGetAllKPIEvaluationPoint"
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
                var data1 = await SqlDapper<object>.ProcedureToListAsync("spGetKpiRuleSumarizeTeamNew", param);
            //var data1 = SQLHelper<object>.ProcedureToList("spGetKpiRuleSumarizeTeamNew"
            // , new string[] { "@KPIEmployeePointID" }
            // , new object[] { empPointId });

            var param2 = new
            {
                KPIEmployeePointID = empPointId,
                IsPublic = 1
            };
            var data2 = await SqlDapper<object>.ProcedureToListAsync("spGetEmployeeRulePointByKPIEmpPointIDNew", param2);
            //var data2 = SQLHelper<object>.ProcedureToList("spGetEmployeeRulePointByKPIEmpPointIDNew"
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
                return Ok(ApiResponseFactory.Success(null, $"{statusText} đánh giá thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion

        #region admin xác nhận
        [HttpPost("admin-confirm-kpi")]
        public async Task<IActionResult> AdminConfirmKPI(int kpiExamID, int empID)
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
                    item.IsAdminConfirm = true;
                    await _kpiEvaluationPointRepo.UpdateAsync(item);
                }
                return Ok(ApiResponseFactory.Success(null, $"Xác nhận đánh giá thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
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

                //câp nhật trang thai của KPIEmployeePoint status = 2; totalPercent = percentRemaining
                KPIEmployeePoint master = _kpiEmployeePointRepo.GetByID(empPointID);
                master.Status = 2;
                master.TotalPercent = request.PercentRemaining;
                //await _kpiEmployeePointRepo.UpdateAsync(master);

                foreach (var item in request.lstKPIEmployeePointDetail)
                {
                    int detailID = item.EmpPointDetailID ?? 0;
                    KPIEmployeePointDetail detail = _kpiEmployeePointDetailRepo.GetByID(detailID);
                    detail.KPIEmployeePointID = empPointID;
                    detail.KPIEvaluationRuleDetailID = item.ID;
                    detail.FirstMonth = item.FirstMonth;
                    detail.SecondMonth =item.SecondMonth;
                    detail.ThirdMonth = item.ThirdMonth;
                    detail.PercentBonus = item.PercentBonus;
                    detail.PercentRemaining = item.PercentRemaining;
                    //if (detail.ID > 0)
                    //{
                    //    await _kpiEmployeePointDetailRepo.UpdateAsync(detail);
                    //}
                    //else
                    //{
                    //    await _kpiEmployeePointDetailRepo.CreateAsync(detail);
                    //}
                }
                return Ok(ApiResponseFactory.Success(null, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
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
                    List<KPIRuleDetailDTO> dtKpiRule = await SqlDapper<KPIRuleDetailDTO>.ProcedureToListTAsync("spGetEmployeeRulePointByKPIEmpPointIDNew", param);
                    //List<KPIRuleDetailDTO> dtKpiRule = SQLHelper<KPIRuleDetailDTO>.ProcedureToListModel("spGetEmployeeRulePointByKPIEmpPointIDNew", new string[] { "@KPIEmployeePointID" }, new object[] { empPoint.ID });
                    if (dtKpiRule.Count <= 0) continue;


                    #region hàm LoadDataView trong winform

                    List<KPISumarizeDTO> lstResult = await SqlDapper<KPISumarizeDTO>.ProcedureToListTAsync("spGetSumarizebyKPIEmpPointIDNew", param2);
                    //List<KPISumarizeDTO> lstResult = SQLHelper<KPISumarizeDTO>.ProcedureToListModel("spGetSumarizebyKPIEmpPointIDNew",
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
                    #endregion
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
                KPIEvaluationRule rule = _kpiEvaluationRuleRepo.GetAll( x=> x.KPIPositionID == position.KPIPosiotionID  && x.KPISessionID == request.kpiSessionID)
                    .FirstOrDefault() ?? new KPIEvaluationRule();
                int empPointMaster = await GetKPIEmployeePointID(rule.ID, request.employeeID);

                return Ok(ApiResponseFactory.Success(empPointMaster, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion

    }
}
