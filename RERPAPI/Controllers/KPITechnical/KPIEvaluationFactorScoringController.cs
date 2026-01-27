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
// --- QUAN TRỌNG: THÊM CÁC DÒNG NÀY ---
using OfficeOpenXml; // Thư viện EPPlus
using OfficeOpenXml.Style; // Thư viện Style Excel
using System.Drawing; // Cần cài NuGet System.Drawing.Common

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
                    detail.SecondMonth = item.SecondMonth;
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
                KPIEvaluationRule rule = _kpiEvaluationRuleRepo.GetAll(x => x.KPIPositionID == position.KPIPosiotionID && x.KPISessionID == request.kpiSessionID)
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

        #region LoadPointRuleNew
        [HttpGet("load-point-rule-new")]
        public async Task<IActionResult> LoadPointRuleNew(int empPointMaster)
        {
            try
            {
                var param = new
                {
                    KPIEmployeePointID = empPointMaster,
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetSumarizebyKPIEmpPointIDNew", param);
                //var data = SQLHelper<object>.ProcedureToList("spGetEmployeeRulePointByKPIEmpPointIDNew"
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

        //#region Xuất Excel theo Team
        ///// <summary>
        ///// Xuất file Excel đánh giá KPI theo nhóm (Team), nén thành file ZIP
        ///// </summary>
        ///// <param name="kpiSessionId">ID của kỳ đánh giá</param>
        ///// <param name="departmentId">ID phòng ban</param>
        ///// <returns>File ZIP chứa các file Excel theo Team</returns>
        //[HttpGet("export-excel-by-team")]
        //public async Task<IActionResult> ExportExcelByTeam(int kpiSessionId, int departmentId)
        //{
        //    try
        //    {
        //        // 1. Lấy thông tin Kỳ đánh giá
        //        var paramSession = new { ID = kpiSessionId };
        //        var dtSession = await SqlDapper<dynamic>.ProcedureToListAsync("spGetKPISessionByID", paramSession);

        //        if (dtSession == null || dtSession.Count == 0)
        //        {
        //            return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy kỳ đánh giá!"));
        //        }

        //        var sessionRow = dtSession.First();
        //        string year = sessionRow.YearEvaluation?.ToString() ?? DateTime.Now.Year.ToString();
        //        string quarter = "Q" + (sessionRow.QuarterEvaluation?.ToString() ?? "1");

        //        // 2. Chuẩn bị stream để nén file Zip
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            using (var archive = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create, true))
        //            {
        //                // 3. Lấy danh sách bài thi (Exam)
        //                var paramExam = new { KPISessionID = kpiSessionId, DepartmentID = departmentId };
        //                var exams = await SqlDapper<dynamic>.ProcedureToListAsync("spGetKPIExamByKPISessionID", paramExam);

        //                if (exams != null && exams.Count > 0)
        //                {
        //                    foreach (var examRow in exams)
        //                    {
        //                        int kpiExamID = Convert.ToInt32(examRow.ID);
        //                        string examCode = examRow.ExamCode?.ToString() ?? "Unknown";

        //                        // 4. Lấy danh sách nhân viên đã được đánh giá
        //                        var paramEmp = new
        //                        {
        //                            EvaluationType = 1,
        //                            DepartmentID = departmentId,
        //                            Keywords = "",
        //                            Status = -1,
        //                            UserTeamID = 0,
        //                            KPIExamID = kpiExamID
        //                        };
        //                        var employees = await SqlDapper<dynamic>.ProcedureToListAsync("spGetAllEmployeeKPIEvaluated", paramEmp);

        //                        if (employees == null || employees.Count == 0) continue;

        //                        // 5. Nhóm nhân viên theo ProjectTypeName (Tên Team)
        //                        var projectTypeGroups = employees
        //                            .GroupBy(e => SanitizeFileName(e.ProjectTypeName?.ToString() ?? "ChuaXacDinh"));

        //                        foreach (var group in projectTypeGroups)
        //                        {
        //                            string teamName = group.Key;

        //                            foreach (var empRow in group)
        //                            {
        //                                int employeeID = Convert.ToInt32(empRow.ID);
        //                                string employeeName = SanitizeFileName(empRow.FullName?.ToString() ?? "Unknown");
        //                                string employeeCode = empRow.Code?.ToString() ?? "";

        //                                // 6. Tạo đường dẫn file trong ZIP
        //                                // Cấu trúc: Year/Quarter/TeamName/DanhGiaKPI_MaNV_TenNV.xlsx
        //                                string zipEntryPath = $"{year}/{quarter}/{teamName}/DanhGiaKPI_{examCode}_{employeeCode}_{employeeName}.xlsx";

        //                                // 7. Tạo file Excel cho nhân viên
        //                                byte[] excelBytes = await GenerateKPIExcelBytesAsync(employeeID, kpiExamID, kpiSessionId, departmentId);

        //                                if (excelBytes != null && excelBytes.Length > 0)
        //                                {
        //                                    // Thêm file Excel vào ZIP
        //                                    var zipEntry = archive.CreateEntry(zipEntryPath);
        //                                    using (var entryStream = zipEntry.Open())
        //                                    {
        //                                        await entryStream.WriteAsync(excelBytes, 0, excelBytes.Length);
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            // 8. Trả về file ZIP cho client
        //            memoryStream.Position = 0;
        //            string zipFileName = $"KPI_Export_{year}_{quarter}.zip";
        //            return File(memoryStream.ToArray(), "application/zip", zipFileName);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}

        ///// <summary>
        ///// Loại bỏ các ký tự không hợp lệ trong tên file/thư mục
        ///// </summary>
        ///// <param name="name">Tên gốc</param>
        ///// <returns>Tên đã được làm sạch</returns>
        //[NonAction]
        //private string SanitizeFileName(string name)
        //{
        //    if (string.IsNullOrEmpty(name)) return "Unknown";

        //    // Loại bỏ các ký tự không hợp lệ trong tên file
        //    foreach (char c in Path.GetInvalidFileNameChars())
        //    {
        //        name = name.Replace(c, '_');
        //    }

        //    // Loại bỏ khoảng trắng thừa
        //    return name.Trim();
        //}

        ///// <summary>
        ///// Tạo file Excel chứa thông tin đánh giá KPI cho một nhân viên
        ///// </summary>
        ///// <param name="employeeID">ID nhân viên</param>
        ///// <param name="kpiExamID">ID bài thi KPI</param>
        ///// <param name="kpiSessionId">ID kỳ đánh giá</param>
        ///// <param name="departmentId">ID phòng ban</param>
        ///// <returns>Mảng byte chứa nội dung file Excel</returns>
        //[NonAction]
        //private async Task<byte[]> GenerateKPIExcelBytesAsync(int employeeID, int kpiExamID, int kpiSessionId, int departmentId)
        //{
        //    try
        //    {
        //        // Khai báo License cho EPPlus (bắt buộc với phiên bản mới)
        //        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

        //        using (var package = new OfficeOpenXml.ExcelPackage())
        //        {
        //            #region Sheet 1: Thông tin nhân viên

        //            var infoSheet = package.Workbook.Worksheets.Add("Thông tin");

        //            // Lấy thông tin nhân viên từ database
        //            var paramEmp = new { ID = employeeID };
        //            var employee = await SqlDapper<dynamic>.ProcedureToListAsync("spGetEmployeeByID", paramEmp);

        //            if (employee != null && employee.Count > 0)
        //            {
        //                var empRow = employee.First();

        //                // Tiêu đề
        //                infoSheet.Cells["A1"].Value = "THÔNG TIN NHÂN VIÊN";
        //                infoSheet.Cells["A1"].Style.Font.Bold = true;
        //                infoSheet.Cells["A1"].Style.Font.Size = 14;
        //                infoSheet.Cells["A1:B1"].Merge = true;

        //                // Thông tin chi tiết
        //                infoSheet.Cells["A3"].Value = "Họ và tên:";
        //                infoSheet.Cells["B3"].Value = empRow.FullName?.ToString();
        //                infoSheet.Cells["A4"].Value = "Mã nhân viên:";
        //                infoSheet.Cells["B4"].Value = empRow.Code?.ToString();
        //                infoSheet.Cells["A5"].Value = "Phòng ban:";
        //                infoSheet.Cells["B5"].Value = empRow.DepartmentName?.ToString();
        //                infoSheet.Cells["A6"].Value = "Chức vụ:";
        //                infoSheet.Cells["B6"].Value = empRow.PositionName?.ToString();
        //                infoSheet.Cells["A7"].Value = "Email:";
        //                infoSheet.Cells["B7"].Value = empRow.Email?.ToString();

        //                // Định dạng cột A (label)
        //                infoSheet.Cells["A3:A7"].Style.Font.Bold = true;
        //                infoSheet.Cells["A3:A7"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        //                infoSheet.Cells["A3:A7"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

        //                // Tự động điều chỉnh độ rộng cột
        //                infoSheet.Cells["A:B"].AutoFitColumns();
        //            }

        //            #endregion

        //            #region Sheet 2: KPI Đánh giá kỹ năng

        //            var kpiSkillSheet = package.Workbook.Worksheets.Add("KPI Kỹ năng");

        //            // Lấy dữ liệu KPI Kỹ năng
        //            var paramSkill = new { EmployeeID = employeeID, EvaluationType = 1, KPIExamID = kpiExamID, IsPulbic = true };
        //            var dtSkill = await SqlDapper<dynamic>.ProcedureToListAsync("spGetAllKPIEvaluationPoint", paramSkill);

        //            if (dtSkill != null && dtSkill.Count > 0)
        //            {
        //                FillKPISheet(kpiSkillSheet, dtSkill, "ĐÁNH GIÁ KPI KỸ NĂNG");
        //            }
        //            else
        //            {
        //                kpiSkillSheet.Cells["A1"].Value = "Không có dữ liệu";
        //            }

        //            #endregion

        //            #region Sheet 3: KPI Đánh giá chuyên môn

        //            var kpiProfSheet = package.Workbook.Worksheets.Add("KPI Chuyên môn");

        //            // Lấy dữ liệu KPI Chuyên môn
        //            var paramProf = new { EmployeeID = employeeID, EvaluationType = 2, KPIExamID = kpiExamID, IsPulbic = true };
        //            var dtProf = await SqlDapper<dynamic>.ProcedureToListAsync("spGetAllKPIEvaluationPoint", paramProf);

        //            if (dtProf != null && dtProf.Count > 0)
        //            {
        //                FillKPISheet(kpiProfSheet, dtProf, "ĐÁNH GIÁ KPI CHUYÊN MÔN");
        //            }
        //            else
        //            {
        //                kpiProfSheet.Cells["A1"].Value = "Không có dữ liệu";
        //            }

        //            #endregion

        //            #region Sheet 4: KPI Đánh giá chung

        //            var kpiGenSheet = package.Workbook.Worksheets.Add("KPI Chung");

        //            // Lấy dữ liệu KPI Chung
        //            var paramGen = new { EmployeeID = employeeID, EvaluationType = 3, KPIExamID = kpiExamID, IsPulbic = true };
        //            var dtGen = await SqlDapper<dynamic>.ProcedureToListAsync("spGetAllKPIEvaluationPoint", paramGen);

        //            if (dtGen != null && dtGen.Count > 0)
        //            {
        //                FillKPISheet(kpiGenSheet, dtGen, "ĐÁNH GIÁ KPI CHUNG");
        //            }
        //            else
        //            {
        //                kpiGenSheet.Cells["A1"].Value = "Không có dữ liệu";
        //            }

        //            #endregion

        //            #region Sheet 5: Tổng hợp điểm

        //            var summarySheet = package.Workbook.Worksheets.Add("Tổng hợp");

        //            // Lấy dữ liệu tổng hợp từ Rule
        //            var kpiPositions = _kpiPositionRepo.GetAll(x => x.KPISessionID == kpiSessionId && x.IsDeleted == false);
        //            var kpiPositionEmployees = _kpiPositionEmployeeRepo.GetAll(x => x.EmployeeID == employeeID && x.IsDeleted == false);

        //            var empPosition = (from p in kpiPositions
        //                               join pe in kpiPositionEmployees on p.ID equals pe.KPIPosiotionID
        //                               select pe).FirstOrDefault() ?? new KPIPositionEmployee();

        //            KPIEvaluationRule rule = _kpiEvaluationRuleRepo.GetAll(x => x.KPISessionID == kpiSessionId && x.KPIPositionID == (empPosition.KPIPosiotionID > 0 ? empPosition.KPIPosiotionID : 1) && x.IsDeleted == false)
        //                .FirstOrDefault() ?? new KPIEvaluationRule();

        //            int empPointId = await GetKPIEmployeePointID(rule.ID, employeeID);

        //            var paramSummary = new { KPIEmployeePointID = empPointId, IsPublic = 1 };
        //            var dtSummary = await SqlDapper<dynamic>.ProcedureToListAsync("spGetEmployeeRulePointByKPIEmpPointIDNew", paramSummary);

        //            if (dtSummary != null && dtSummary.Count > 0)
        //            {
        //                FillSummarySheet(summarySheet, dtSummary);
        //            }
        //            else
        //            {
        //                summarySheet.Cells["A1"].Value = "Không có dữ liệu tổng hợp";
        //            }

        //            #endregion

        //            // Trả về mảng byte của file Excel
        //            return package.GetAsByteArray();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Ghi log lỗi
        //        Console.WriteLine($"Lỗi khi tạo Excel cho nhân viên {employeeID}: {ex.Message}");
        //        return null;
        //    }
        //}

        ///// <summary>
        ///// Điền dữ liệu KPI vào sheet Excel
        ///// </summary>
        ///// <param name="sheet">ExcelWorksheet cần điền dữ liệu</param>
        ///// <param name="data">Danh sách dữ liệu KPI</param>
        ///// <param name="title">Tiêu đề của sheet</param>
        //[NonAction]
        //private void FillKPISheet(OfficeOpenXml.ExcelWorksheet sheet, List<dynamic> data, string title)
        //{
        //    int row = 1;

        //    // Tiêu đề sheet
        //    sheet.Cells[row, 1].Value = title;
        //    sheet.Cells[row, 1].Style.Font.Bold = true;
        //    sheet.Cells[row, 1].Style.Font.Size = 14;
        //    sheet.Cells[row, 1, row, 7].Merge = true;
        //    sheet.Cells[row, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //    row += 2;

        //    // Header của bảng
        //    string[] headers = new[]
        //    {
        //        "STT",
        //        "Nội dung đánh giá",
        //        "Hệ số",
        //        "Điểm chuẩn",
        //        "Tự đánh giá",
        //        "TBP đánh giá",
        //        "BGĐ đánh giá"
        //    };

        //    for (int i = 0; i < headers.Length; i++)
        //    {
        //        var cell = sheet.Cells[row, i + 1];
        //        cell.Value = headers[i];
        //        cell.Style.Font.Bold = true;
        //        cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        //        cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(68, 114, 196)); // Màu xanh dương
        //        cell.Style.Font.Color.SetColor(System.Drawing.Color.White);
        //        cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //        cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        //        cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    }
        //    row++;

        //    // Điền dữ liệu
        //    int stt = 1;
        //    foreach (var dataRow in data)
        //    {
        //        sheet.Cells[row, 1].Value = stt++;
        //        sheet.Cells[row, 2].Value = dataRow.VerificationToolsContent?.ToString();
        //        sheet.Cells[row, 3].Value = ConvertToDouble(dataRow.Coefficient);
        //        sheet.Cells[row, 4].Value = ConvertToDouble(dataRow.StandardPoint);
        //        sheet.Cells[row, 5].Value = ConvertToDouble(dataRow.EmployeePoint);
        //        sheet.Cells[row, 6].Value = ConvertToDouble(dataRow.TBPPoint);
        //        sheet.Cells[row, 7].Value = ConvertToDouble(dataRow.BGDPoint);

        //        // Định dạng border cho từng ô
        //        for (int col = 1; col <= 7; col++)
        //        {
        //            sheet.Cells[row, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

        //            // Căn giữa cho các cột số
        //            if (col != 2)
        //            {
        //                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //            }
        //        }

        //        row++;
        //    }

        //    // Dòng tổng kết
        //    sheet.Cells[row, 1].Value = "";
        //    sheet.Cells[row, 2].Value = "TỔNG CỘNG";
        //    sheet.Cells[row, 2].Style.Font.Bold = true;
        //    sheet.Cells[row, 3].Value = "";
        //    sheet.Cells[row, 4].Value = "";

        //    // Tính tổng điểm
        //    sheet.Cells[row, 5].Formula = $"SUM(E4:E{row - 1})";
        //    sheet.Cells[row, 6].Formula = $"SUM(F4:F{row - 1})";
        //    sheet.Cells[row, 7].Formula = $"SUM(G4:G{row - 1})";

        //    // Định dạng dòng tổng
        //    for (int col = 1; col <= 7; col++)
        //    {
        //        var cell = sheet.Cells[row, col];
        //        cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //        cell.Style.Font.Bold = true;
        //        cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        //        cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
        //    }

        //    // Tự động điều chỉnh độ rộng cột
        //    sheet.Cells.AutoFitColumns();

        //    // Đặt độ rộng cố định cho cột nội dung
        //    sheet.Column(2).Width = 50;
        //}

        ///// <summary>
        ///// Điền dữ liệu tổng hợp vào sheet Excel
        ///// </summary>
        ///// <param name="sheet">ExcelWorksheet cần điền dữ liệu</param>
        ///// <param name="data">Danh sách dữ liệu tổng hợp</param>
        //[NonAction]
        //private void FillSummarySheet(OfficeOpenXml.ExcelWorksheet sheet, List<dynamic> data)
        //{
        //    int row = 1;

        //    // Tiêu đề
        //    sheet.Cells[row, 1].Value = "TỔNG HỢP ĐIỂM ĐÁNH GIÁ KPI";
        //    sheet.Cells[row, 1].Style.Font.Bold = true;
        //    sheet.Cells[row, 1].Style.Font.Size = 14;
        //    sheet.Cells[row, 1, row, 7].Merge = true;
        //    sheet.Cells[row, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //    row += 2;

        //    // Header của bảng
        //    string[] headers = new[]
        //    {
        //        "Nội dung đánh giá",
        //        "Tháng 1",
        //        "Tháng 2",
        //        "Tháng 3",
        //        "% Thưởng",
        //        "% Còn lại"
        //    };

        //    for (int i = 0; i < headers.Length; i++)
        //    {
        //        var cell = sheet.Cells[row, i + 1];
        //        cell.Value = headers[i];
        //        cell.Style.Font.Bold = true;
        //        cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        //        cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 176, 80)); // Màu xanh lá
        //        cell.Style.Font.Color.SetColor(System.Drawing.Color.White);
        //        cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //        cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        //        cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //    }
        //    row++;

        //    // Điền dữ liệu
        //    foreach (var dataRow in data)
        //    {
        //        sheet.Cells[row, 1].Value = dataRow.EvaluationContent?.ToString();
        //        sheet.Cells[row, 2].Value = ConvertToDouble(dataRow.FirstMonth);
        //        sheet.Cells[row, 3].Value = ConvertToDouble(dataRow.SecondMonth);
        //        sheet.Cells[row, 4].Value = ConvertToDouble(dataRow.ThirdMonth);
        //        sheet.Cells[row, 5].Value = ConvertToDouble(dataRow.PercentBonus);
        //        sheet.Cells[row, 6].Value = ConvertToDouble(dataRow.PercentRemaining);

        //        // Định dạng border và căn giữa
        //        for (int col = 1; col <= 6; col++)
        //        {
        //            sheet.Cells[row, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //            if (col > 1)
        //            {
        //                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //            }
        //        }

        //        row++;
        //    }

        //    // Tự động điều chỉnh độ rộng cột
        //    sheet.Cells.AutoFitColumns();
        //}

        ///// <summary>
        ///// Chuyển đổi giá trị object sang double, trả về 0 nếu không hợp lệ
        ///// </summary>
        ///// <param name="value">Giá trị cần chuyển đổi</param>
        ///// <returns>Giá trị double</returns>
        //[NonAction]
        //private double ConvertToDouble(object value)
        //{
        //    if (value == null)
        //        return 0;

        //    if (double.TryParse(value.ToString(), out double result))
        //        return result;

        //    return 0;
        //}
        //#endregion
    }
}
