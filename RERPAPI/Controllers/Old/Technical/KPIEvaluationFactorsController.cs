using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.POIFS.Properties;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Technical.KPI;
using System.Text.RegularExpressions;

namespace RERPAPI.Controllers.Old.Technical
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KPIEvaluationFactorsController : ControllerBase
    {
        private readonly DepartmentRepo _departmentRepo;
        private readonly KPISessionRepo _kpiSessionRepo;
        private readonly KPIEvaluationFactorRepo _kpiEvaluationFactorRepo;
        private readonly KPIExamRepo _kpiExamRepo;
        private readonly KPIPositionRepo _kpiPositionRepo;
        private readonly KPIExamPositionRepo _kpiExamPositionRepo;
        private readonly KPIEvaluationRuleRepo _kpiEvaluationRuleRepo;
        private readonly KPISpecializationTypeRepo _kpiSpecializationTypeRepo;
        public KPIEvaluationFactorsController(DepartmentRepo departmentRepo, KPISessionRepo kPISessionRepo, KPIEvaluationFactorRepo kPIEvaluationFactorRepo, KPIExamRepo kpiExamRepo, KPIPositionRepo kpiPositionRepo, KPIExamPositionRepo kpiExamPositionRepo, KPIEvaluationRuleRepo kpiEvaluationRuleRepo, KPISpecializationTypeRepo kpiSpecializationTypeRepo)
        {
            _departmentRepo = departmentRepo;
            _kpiSessionRepo = kPISessionRepo;
            _kpiEvaluationFactorRepo = kPIEvaluationFactorRepo;
            _kpiExamRepo = kpiExamRepo;
            _kpiPositionRepo = kpiPositionRepo;
            _kpiExamPositionRepo = kpiExamPositionRepo;
            _kpiEvaluationRuleRepo = kpiEvaluationRuleRepo;
            _kpiSpecializationTypeRepo = kpiSpecializationTypeRepo;
        }

        [HttpGet("get-department")]
        public IActionResult GetDepartment()
        {
            try
            {
                var data = _departmentRepo.GetAll(x => x.IsDeleted != true).OrderBy(x => x.STT);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-data")]
        public IActionResult GetData(int year, int departmentId)
        {
            try
            {
                var data = _kpiSessionRepo.GetAll(x => x.IsDeleted != true && x.YearEvaluation == year && x.DepartmentID == departmentId).OrderByDescending(x => x.ID).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("load-detail")]
        public async Task<IActionResult> LoadDetail(int kpiSessionId, int departmentId)
        {
            try
            {
                var param = new
                {
                    KPISessionID = kpiSessionId,
                    DepartmentID = departmentId
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetKPIExamByKPISessionID", param);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

        [HttpGet("load-kpi-evaluation")]
        public async Task<IActionResult> LoadKPIEvaluation(int kpiExamID)
        {
            try
            {
                var param1 = new
                {
                    KPIExamID = kpiExamID,
                    EvaluationType = 1
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetAllKPIEvaluationByYearAndQuarter", param1);

                var param2 = new
                {
                    KPIExamID = kpiExamID,
                    EvaluationType = 2
                };
                var data2 = await SqlDapper<object>.ProcedureToListAsync("spGetAllKPIEvaluationByYearAndQuarter", param2);

                var param3 = new
                {
                    KPIExamID = kpiExamID,
                    EvaluationType = 3
                };
                var data3 = await SqlDapper<object>.ProcedureToListAsync("spGetAllKPIEvaluationByYearAndQuarter", param3);
                return Ok(ApiResponseFactory.Success(new { data, data2, data3}, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

        [HttpPost("delete-evaluation-factors")]
        public async Task<IActionResult> DeleteKPIEvaluationFactors(int id)
        {
            try
            {
                if(id <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Bản ghi xóa không hợp lệ"));
                }
                await _kpiEvaluationFactorRepo.DeleteAsync(id);
                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

        [HttpPost("delete-session")]
        public async Task<IActionResult> DeleteSession(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Bản ghi xóa không hợp lệ"));
                }
                var model = await _kpiSessionRepo.GetByIDAsync(id);
                model.IsDeleted = true;
                await _kpiSessionRepo.UpdateAsync(model);
                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

        [HttpPost("delete-exam")]
        public async Task<IActionResult> DeleteExam(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Bản ghi xóa không hợp lệ"));
                }
                var model = await _kpiExamRepo.GetByIDAsync(id);
                model.IsDeleted = true;
                await _kpiExamRepo.UpdateAsync(model);
                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

        [HttpGet("get-evaluation-factor-by-id")]
        public async Task<IActionResult> GetEvaluationFactorById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "ID không hợp lệ"));

                var model = await _kpiEvaluationFactorRepo.GetByIDAsync(id);
                if (model == null)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy dữ liệu"));

                return Ok(ApiResponseFactory.Success(model, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-next-stt")]
        public IActionResult GetNextSTT(
            int kpiExamId,
            int evaluationType,
            int parentId
        )
        {
            try
            {
                if (parentId > 0)
                {
                    var parent = _kpiEvaluationFactorRepo.GetByID(parentId);
                    int count = _kpiEvaluationFactorRepo.GetAll(x =>
                        x.ParentID == parentId &&
                        x.IsDeleted == false
                    ).Count();

                    return Ok(ApiResponseFactory.Success($"{parent.STT}.{count + 1}", ""));
                }

                int rootCount = _kpiEvaluationFactorRepo.GetAll(x =>
                    x.KPIExamID == kpiExamId &&
                    x.EvaluationType == evaluationType &&
                    x.ParentID == 0 &&
                    x.IsDeleted == false
                ).Count();

                return Ok(ApiResponseFactory.Success($"{rootCount + 1}", ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("create-auto-kpi-exam")]
        public async Task<IActionResult> CreateAutoKPIExam(int kpiSessionId)
        {
            try
            {
                if (kpiSessionId <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Kỳ đánh giá không hợp lệ"));

                // 1. Lấy session
                var session = await _kpiSessionRepo.GetByIDAsync(kpiSessionId);
                if (session == null)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy kỳ đánh giá"));

                // 2. Check đã có vị trí chưa
                var positions = _kpiPositionRepo.GetAll(x => x.KPISessionID == kpiSessionId && x.IsDeleted == false).ToList();

                if (positions.Count <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(
                        null,
                        "Kỳ đánh giá chưa được thêm vị trí"
                    ));
                }

                CreateAutoKPIExamNew(session);
                CreateAutoKPIRuleNew(session);

                return Ok(ApiResponseFactory.Success(null, "Tạo bài đánh giá & rule thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        private async Task CreateAutoKPIExamNew(KPISession sessionKPI)
        {
            // 1. LẤY POSITION BẰNG SP (GIỐNG WINFORM)
            var param = new
            {
                KPIExamID = 0,
                KPISessionID = sessionKPI.ID
            };

            var posObj = await SqlDapper<object>
                .ProcedureToListAsync("spGetKPIPositionByExamID", param);

            var positions = posObj as IEnumerable<dynamic>;
            if (positions == null) return;

            // 2. XÓA MỀM EXAM + EXAM POSITION CŨ
            var oldExams = _kpiExamRepo
                .GetAll(x => x.KPISessionID == sessionKPI.ID && x.IsDeleted == false)
                .ToList();

            foreach (var exam in oldExams)
            {
                exam.IsDeleted = true;
                await _kpiExamRepo.UpdateAsync(exam);

                var oldExamPositions = _kpiExamPositionRepo
                    .GetAll(x => x.KPIExamID == exam.ID && x.IsDeleted == false)
                    .ToList();

                foreach (var ep in oldExamPositions)
                {
                    ep.IsDeleted = true;
                    await _kpiExamPositionRepo.UpdateAsync(ep);
                }
            }

            // 3. TẠO MỚI EXAM + EXAM POSITION
            foreach (var row in positions)
            {
                int positionID = Convert.ToInt32(row.ID);
                if (positionID <= 0) continue;

                string positionCode = Convert.ToString(row.PositionCode).ToUpper();
                string positionName = Convert.ToString(row.PositionName).ToUpper();

                KPIExam exam = new KPIExam
                {
                    KPISessionID = sessionKPI.ID,
                    ExamCode = $"KPI_{positionCode}_{sessionKPI.YearEvaluation}_Q{sessionKPI.QuarterEvaluation}",
                    ExamName = $"KPI {positionName} Q{sessionKPI.QuarterEvaluation}-{sessionKPI.YearEvaluation}",
                    IsDeleted = false,
                    IsActive = true,
                    Deadline = DateTime.Now.AddMonths(1)
                };

                await _kpiExamRepo.CreateAsync(exam);

                KPIExamPosition examPosition = new KPIExamPosition
                {
                    KPIExamID = exam.ID,
                    KPIPositionID = positionID,
                    IsDeleted = false
                };

                await _kpiExamPositionRepo.CreateAsync(examPosition);
            }
        }

        private async Task CreateAutoKPIRuleNew(KPISession sessionKPI)
        {
            // 1. XÓA MỀM RULE CŨ
            var oldRules = _kpiEvaluationRuleRepo
                .GetAll(x => x.KPISessionID == sessionKPI.ID && x.IsDeleted == false)
                .ToList();

            foreach (var rule in oldRules)
            {
                rule.IsDeleted = true;
                await _kpiEvaluationRuleRepo.UpdateAsync(rule);
            }

            // 2. LẤY POSITION BẰNG SP
            var param = new
            {
                KPIExamID = 0,
                KPISessionID = sessionKPI.ID
            };

            var posObj = await SqlDapper<object>
                .ProcedureToListAsync("spGetKPIPositionByExamID", param);

            var positions = posObj as IEnumerable<dynamic>;
            if (positions == null) return;

            // 3. TẠO RULE MỚI
            foreach (var row in positions)
            {
                int positionID = Convert.ToInt32(row.ID);
                if (positionID <= 0) continue;

                string positionCode = Convert.ToString(row.PositionCode).ToUpper();
                string positionName = Convert.ToString(row.PositionName).ToUpper();

                KPIEvaluationRule rule = new KPIEvaluationRule
                {
                    KPISessionID = sessionKPI.ID,
                    KPIPositionID = positionID,
                    RuleCode = $"KPIRule_{positionCode}_{sessionKPI.YearEvaluation}_Q{sessionKPI.QuarterEvaluation}",
                    RuleName = $"Đánh giá KPI Rule {positionName} Q{sessionKPI.QuarterEvaluation}-{sessionKPI.YearEvaluation}",
                    IsDeleted = false
                };

                await _kpiEvaluationRuleRepo.CreateAsync(rule);
            }
        }

        [HttpGet("get-parent-group")]
        public async Task<IActionResult> GetParentGroup(
            int kpiExamId,
            int evaluationType,
            int currentId = 0
        )
        {
            try
            {
                var param = new
                {
                    KPIExamID = kpiExamId,
                    EvaluationType = evaluationType,
                    ID = currentId
                };

                var data = await SqlDapper<object>
                    .ProcedureToListAsync("spGetAllKPIEvaluationByYearAndQuarter", param);

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-specialization-types")]
        public IActionResult GetSpecializationType(int departmentId)
        {
            try
            {
                var data = _kpiSpecializationTypeRepo.GetAll(x => x.DepartmentID == departmentId && x.IsDeleted == false).OrderBy(x => x.STT);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveKPIEvaluationFactor(
            [FromBody] SaveKPIEvaluationFactorRequest req
        )
        {
            try
            {
                // 1. VALIDATE CƠ BẢN
                if (string.IsNullOrWhiteSpace(req.STT))
                    return BadRequest("Vui lòng nhập STT");

                if (!Regex.IsMatch(req.STT.Trim(), @"^\d+(\.\d+)*$"))
                    return BadRequest("STT chỉ được nhập số và dấu chấm");

                if (req.EvaluationType <= 0)
                    return BadRequest("Vui lòng chọn Loại yếu tố");

                if (req.SpecializationType <= 0)
                    return BadRequest("Vui lòng chọn Loại chuyên môn");

                if (string.IsNullOrWhiteSpace(req.EvaluationContent))
                    return BadRequest("Vui lòng nhập Yếu tố đánh giá");

                // 2. VALIDATE NHÓM CHA
                KPIEvaluationFactor parent = null;

                if (req.ParentID > 0)
                {
                    parent = await _kpiEvaluationFactorRepo.GetByIDAsync(req.ParentID);
                    if (parent == null)
                        return BadRequest("Nhóm cha không tồn tại");

                    // STT không được trùng trong cùng Parent
                    bool isDuplicateSTT = _kpiEvaluationFactorRepo.GetAll(x =>
                        x.ParentID == req.ParentID &&
                        x.IsDeleted == false &&
                        x.ID != req.ID &&
                        x.STT == req.STT
                    ).Any();

                    if (isDuplicateSTT)
                        return BadRequest($"STT đã tồn tại trong nhóm cha [{parent.STT}]");

                    // Điểm không được vượt nhóm cha (trừ phòng 9)
                    if (req.DepartmentID != 9 && req.StandardPoint > (parent.StandardPoint ?? 0))
                        return BadRequest($"Điểm chuẩn phải <= nhóm cha [{parent.STT}]");
                }
                else
                {
                    if (req.StandardPoint <= 0)
                        return BadRequest("Điểm chuẩn phải > 0");
                }

                // 3. LOAD HOẶC TẠO ENTITY
                KPIEvaluationFactor model;

                if (req.ID > 0)
                {
                    model = await _kpiEvaluationFactorRepo.GetByIDAsync(req.ID);
                    if (model == null)
                        return NotFound("Không tìm thấy KPI");
                }
                else
                {
                    model = new KPIEvaluationFactor
                    {
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                }

                // 4. GÁN DỮ LIỆU
                model.KPIExamID = req.KPIExamID;
                model.ParentID = req.ParentID;
                model.EvaluationType = req.EvaluationType;
                model.SpecializationType = req.SpecializationType;
                model.STT = req.STT.Trim();
                model.EvaluationContent = req.EvaluationContent.Trim();
                model.VerificationToolsContent = req.VerificationToolsContent?.Trim();
                model.StandardPoint = req.StandardPoint;
                model.Coefficient = req.Coefficient;
                model.Unit = req.Unit?.Trim();
                model.UpdatedDate = DateTime.Now;

                // 5. SAVE
                if (req.ID > 0)
                    await _kpiEvaluationFactorRepo.UpdateAsync(model);
                else
                    await _kpiEvaluationFactorRepo.CreateAsync(model);

                return Ok(ApiResponseFactory.Success(model, "Lưu KPI thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        public class SaveKPIEvaluationFactorRequest
        {
            public int ID { get; set; }              
            public int KPIExamID { get; set; }
            public int ParentID { get; set; }
            public int EvaluationType { get; set; }
            public int SpecializationType { get; set; }

            public string STT { get; set; }
            public string EvaluationContent { get; set; }
            public string VerificationToolsContent { get; set; }
            public decimal StandardPoint { get; set; }
            public int Coefficient { get; set; }
            public string Unit { get; set; }

            public int DepartmentID { get; set; }  
        }

    }
}
