using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Technical.KPI;

namespace RERPAPI.Controllers.Old.Technical
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KPIEvaluationRuleController : ControllerBase
    {
        private readonly DepartmentRepo _departmentRepo;
        private readonly KPIPositionRepo _kpiPositionRepo;
        private readonly KPISessionRepo _kpiSessionRepo;
        private readonly KPIEvaluationRuleRepo _kpiEvaluationRuleRepo;
        private readonly KPIEvaluationRuleDetailRepo _kpiEvaluationRuleDetailRepo;
        private readonly KPIExamRepo _kpiExamRepo;
        private readonly KPIExamPositionRepo _kpiExamPositionRepo;
        private readonly KPIEvaluationFactorRepo _kpiEvaluationFactorRepo;
        private readonly KPICriterionRepo _kpiCriteriaRepo;
        private readonly KPICriteriaDetailRepo _kpiCriteriaDetailRepo;

        public KPIEvaluationRuleController(DepartmentRepo departmentRepo, 
                                           KPIPositionRepo kPIPositionRepo, 
                                           KPISessionRepo kpiSessionRepo, 
                                           KPIEvaluationRuleRepo kpiEvaluationRuleRepo, 
                                           KPIEvaluationRuleDetailRepo kPIEvaluationRuleDetailRepo, 
                                           KPIExamRepo kpiExamRepo,
                                           KPIExamPositionRepo kpiExamPositionRepo,
                                           KPIEvaluationFactorRepo kpiEvaluationFactorRepo,
                                           KPICriterionRepo kpiCriteriaRepo,
                                           KPICriteriaDetailRepo kpiCriteriaDetailRepo
        )
        {
            _departmentRepo = departmentRepo;
            _kpiPositionRepo = kPIPositionRepo;
            _kpiSessionRepo = kpiSessionRepo;
            _kpiEvaluationRuleRepo = kpiEvaluationRuleRepo;
            _kpiEvaluationRuleDetailRepo = kPIEvaluationRuleDetailRepo;
            _kpiExamRepo = kpiExamRepo;
            _kpiExamPositionRepo = kpiExamPositionRepo;
            _kpiEvaluationFactorRepo = kpiEvaluationFactorRepo;
            _kpiCriteriaRepo = kpiCriteriaRepo;
            _kpiCriteriaDetailRepo = kpiCriteriaDetailRepo;
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

        [HttpGet("get-kpi-rule-by-session-copy")]
        public IActionResult GetKPIRule(int sessionCopyId)
        {
            try
            {
                var data = _kpiEvaluationRuleRepo.GetAll(x => x.IsDeleted != true && x.KPISessionID == sessionCopyId);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-data-kpi-session")]
        public IActionResult LoadKPISession()
        {
            try
            {
                var data = _kpiSessionRepo.GetAll(x => x.IsDeleted == false).OrderByDescending(x => x.ID).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-position-by-session")]
        public IActionResult GetPositionBySession(int kpiSessionId)
        {
            try
            {
                var data = _kpiPositionRepo.GetAll(x => x.IsDeleted != true && x.KPISessionID == kpiSessionId);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpGet("get-position")]
        public IActionResult GetPosition()
        {
            try
            {
                var data = _kpiPositionRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-session")]
        public IActionResult GetSession(int year, int departmentId)
        {
            try
            {
                var data = _kpiSessionRepo.GetAll(x => x.YearEvaluation == year && x.IsDeleted == false && x.DepartmentID == departmentId);

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete-session")]
        public IActionResult DeleteSession(int sessionId)
        {
            try
            {
                KPISession model = _kpiSessionRepo.GetByID(sessionId);
                model.IsDeleted = true;
                _kpiSessionRepo.Update(model);
                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete-exam")]
        public IActionResult DeleteExam(int examId)
        {
            try
            {
                KPIEvaluationRule model = _kpiEvaluationRuleRepo.GetByID(examId);
                model.IsDeleted = true;
                _kpiEvaluationRuleRepo.Update(model);
                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("load-data-rule")]
        public async Task<IActionResult> LoadDataRule(int ruleID)
        {
            try
            {
                var param = new
                {
                    KPIEvaluationRuleID = ruleID
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetKPIEvaluationRuleDetail", param);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

        [HttpPost("delete-rule")]
        public IActionResult DeleteRule(int ruleDetailID)
        {
            try
            {
                KPIEvaluationRuleDetail model = _kpiEvaluationRuleDetailRepo.GetByID(ruleDetailID);
                model.IsDeleted = true;
                _kpiEvaluationRuleDetailRepo.Update(model);
                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-data-details")]
        public async Task<IActionResult> LoadDataDetails(int kpiSessionID)
        {
            try
            {
                var param = new
                {
                    KPISession = kpiSessionID
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetKPIEvaluationRule", param);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

        [HttpPost("save-kpi-session")]
        public async Task<IActionResult> SaveKPISession([FromBody] SaveKPISessionRequest dto)
        {
            try
            {
                // ===== Validate cơ bản =====
                if (dto.model.YearEvaluation <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Năm!"));

                if (dto.model.QuarterEvaluation <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Quý!"));

                if (string.IsNullOrWhiteSpace(dto.model.Code))
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Mã kỳ thi!"));

                if (string.IsNullOrWhiteSpace(dto.model.Name))
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Tên kỳ thi!"));

                // ===== Check trùng =====
                var duplicate = _kpiSessionRepo.GetAll(x =>
                    x.YearEvaluation == dto.model.YearEvaluation &&
                    x.ID != dto.model.ID &&
                    x.DepartmentID == dto.model.DepartmentID &&
                    x.IsDeleted != true
                ).ToList();

                // ===== COPY MODE =====
                if (dto.IsCopy)
                {
                    bool isDuplicate =
                        duplicate.Any(x =>
                            x.QuarterEvaluation == dto.model.QuarterEvaluation ||
                            x.Code == dto.model.Code);

                    if (isDuplicate && !dto.IsForce)
                    {
                        return Ok(ApiResponseFactory.Success(
                            new
                            {
                                NeedConfirm = true
                            },
                            $"Yếu tố đánh giá tại quý {dto.model.QuarterEvaluation} năm {dto.model.YearEvaluation} đã tồn tại! Bạn có muốn ghi đè dữ liệu không?"
                        ));
                    }
                }
                // ===== NORMAL MODE =====
                else
                {
                    if (duplicate.Any(x => x.QuarterEvaluation == dto.model.QuarterEvaluation))
                    {
                        return BadRequest(ApiResponseFactory.Fail(
                            null,
                            $"Quý [{dto.model.QuarterEvaluation}] trong năm [{dto.model.YearEvaluation}] đã có kỳ thi!"
                        ));
                    }

                    if (duplicate.Any(x => x.Code == dto.model.Code))
                    {
                        return BadRequest(ApiResponseFactory.Fail(
                            null,
                            $"Mã kỳ thi đã được sử dụng trong năm [{dto.model.YearEvaluation}]!"
                        ));
                    }
                }

                // ===== Save =====
                KPISession entity;

                if (dto.model.ID > 0)
                {
                    entity = await _kpiSessionRepo.GetByIDAsync(dto.model.ID);
                    if (entity == null)
                        return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy kỳ đánh giá"));

                    entity.YearEvaluation = dto.model.YearEvaluation;
                    entity.QuarterEvaluation = dto.model.QuarterEvaluation;
                    entity.DepartmentID = dto.model.DepartmentID;
                    entity.Code = dto.model.Code.Trim();
                    entity.Name = dto.model.Name.Trim();

                    await _kpiSessionRepo.UpdateAsync(entity);
                }
                else
                {
                    entity = new KPISession
                    {
                        YearEvaluation = dto.model.YearEvaluation,
                        QuarterEvaluation = dto.model.QuarterEvaluation,
                        DepartmentID = dto.model.DepartmentID,
                        Code = dto.model.Code.Trim(),
                        Name = dto.model.Name.Trim()
                    };

                    await _kpiSessionRepo.CreateAsync(entity);

                    await CreateAutoKPIExam(entity);
                    await CreateAutoKPIRule(entity);
                }

                return Ok(ApiResponseFactory.Success(entity, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        private async Task CreateAutoKPIExam(KPISession sessionKPI)
        {
            var param = new
            {
                KPIExamID = 0
            };

            var positions = await SqlDapper<dynamic>
                .ProcedureToListAsync("spGetKPIPositionByExamID", param) as List<dynamic>;

            if (positions == null || positions.Count == 0) return;

            foreach (var row in positions)
            {
                int positionID = Convert.ToInt32(row.ID);
                if (positionID <= 0) continue;

                string positionCode = row.PositionCode?.ToString()?.ToUpper() ?? string.Empty;
                string positionName = row.PositionName?.ToString()?.ToUpper() ?? string.Empty;

                // Insert KPIExam
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

                // Insert KPIExamPosition
                KPIExamPosition examPosition = new KPIExamPosition
                {
                    KPIExamID = exam.ID,
                    KPIPositionID = positionID
                };
                await _kpiExamPositionRepo.CreateAsync(examPosition);
            }
        }

        private async Task CreateAutoKPIRule(KPISession sessionKPI)
        {
            var param = new { KPIExamID = 0 };

            var positions = await SqlDapper<dynamic>
                .ProcedureToListAsync("spGetKPIPositionByExamID", param) as List<dynamic>;

            if (positions == null || positions.Count == 0) return;

            foreach (var row in positions)
            {
                int positionID = Convert.ToInt32(row.ID);
                if (positionID <= 0) continue;

                string positionCode = row.PositionCode?.ToString()?.ToUpper() ?? string.Empty;
                string positionName = row.PositionName?.ToString()?.ToUpper() ?? string.Empty;

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

        [HttpPost("copy-kpi-session")]
        public async Task<IActionResult> CopyKPISession([FromBody] CopyKPISessionRequest dto)
        {
            try
            {
                // ===== 1. Validate =====
                if (dto.FromSessionID <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn kỳ đánh giá nguồn"));

                if (dto.Year <= 0 || dto.Quarter <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Năm / Quý không hợp lệ"));

                // ===== 2. Kiểm tra trùng session =====
                var oldSessions = _kpiSessionRepo.GetAll(x =>
                    x.DepartmentID == dto.DepartmentID &&
                    x.YearEvaluation == dto.Year &&
                    x.QuarterEvaluation == dto.Quarter &&
                    x.IsDeleted == false
                ).ToList();

                if (oldSessions.Any() && !dto.IsForce)
                {
                    return Ok(ApiResponseFactory.Success(
                        new { NeedConfirm = true },
                        "Dữ liệu kỳ đánh giá đã tồn tại, bạn có muốn ghi đè không?"
                    ));
                }

                // ===== 3. Xóa session cũ =====
                foreach (var old in oldSessions)
                {
                    old.IsDeleted = true;
                    await _kpiSessionRepo.UpdateAsync(old);
                }

                // ===== 4. Tạo session mới =====
                KPISession newSession = new KPISession
                {
                    YearEvaluation = dto.Year,
                    QuarterEvaluation = dto.Quarter,
                    DepartmentID = dto.DepartmentID,
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim()
                };

                await _kpiSessionRepo.CreateAsync(newSession);

                // ===== 5. Copy KPIExam =====
                var oldExams = _kpiExamRepo.GetAll(x =>
                    x.KPISessionID == dto.FromSessionID &&
                    x.IsDeleted == false
                ).ToList();

                KPISession oldSession = _kpiSessionRepo.GetByID(dto.FromSessionID);

                string oldCode = $"{oldSession.YearEvaluation}_Q{oldSession.QuarterEvaluation}";
                string oldName = $"Q{oldSession.QuarterEvaluation}-{oldSession.YearEvaluation}";

                string newCode = $"{dto.Year}_Q{dto.Quarter}";
                string newName = $"Q{dto.Quarter}-{dto.Year}";

                foreach (var exam in oldExams)
                {
                    int oldExamId = exam.ID;

                    exam.ID = 0;
                    exam.KPISessionID = newSession.ID;
                    exam.ExamCode = exam.ExamCode.Replace(oldCode, newCode);
                    exam.ExamName = exam.ExamName.Replace(oldName, newName);

                    await _kpiExamRepo.CreateAsync(exam);

                    // ===== Copy KPIExamPosition =====
                    var positions = _kpiExamPositionRepo.GetAll(x => x.KPIExamID == oldExamId);
                    foreach (var pos in positions)
                    {
                        pos.ID = 0;
                        pos.KPIExamID = exam.ID;
                        await _kpiExamPositionRepo.CreateAsync(pos);
                    }

                    // ===== Copy KPIEvaluationFactors (SP) =====
                    var param = new
                    {
                        KPIExamID = oldExamId,
                        EvaluationType = 0
                    };

                    var factors = await SqlDapper<KPIEvaluationFactor>
                        .ProcedureToListTAsync("spGetAllKPIEvaluationByYearAndQuarter", param);

                    //foreach (var factor in factors.OrderBy(x => x.ParentID))
                    //{
                    //    factor.ID = 0;
                    //    factor.KPIExamID = exam.ID;

                    //    if (factor.ParentID > 0)
                    //    {
                    //        //var parent = factors.FirstOrDefault(x =>
                    //        //x.STT == factor.STT.Remove(factor.STT.LastIndexOf(".")));
                    //        string parentSTT = factor.STT.Remove(factor.STT.LastIndexOf("."));

                    //        KPIEvaluationFactor? parent = _kpiEvaluationFactorRepo
                    //            .GetAll(x =>
                    //                x.STT == parentSTT &&
                    //                x.KPIExamID == exam.ID &&
                    //                x.EvaluationType == factor.EvaluationType &&
                    //                x.IsDeleted == false
                    //            )
                    //            .FirstOrDefault();


                    //        factor.ParentID = parent?.ID ?? 0;
                    //    }

                    //    await _kpiEvaluationFactorRepo.CreateAsync(factor);
                    //}

                    foreach (var factor in factors.OrderBy(x => x.ParentID))
                    {
                        int oldParentId = factor.ParentID ?? 0;

                        factor.ID = 0;
                        factor.KPIExamID = exam.ID;
                        factor.ParentID = 0;

                        await _kpiEvaluationFactorRepo.CreateAsync(factor);

                        if (oldParentId > 0)
                        {
                            string parentSTT = factor.STT.Remove(factor.STT.LastIndexOf("."));

                            KPIEvaluationFactor? parent = _kpiEvaluationFactorRepo
                                .GetAll(x =>
                                    x.STT == parentSTT &&
                                    x.KPIExamID == exam.ID &&
                                    x.EvaluationType == factor.EvaluationType &&
                                    x.IsDeleted == false
                                )
                                .FirstOrDefault();

                            if (parent != null)
                            {
                                factor.ParentID = parent.ID;
                                await _kpiEvaluationFactorRepo.UpdateAsync(factor);
                            }
                        }
                    }

                }

                // ===== 6. Copy Criteria =====
                await CopyCriteriaInternal(dto.FromSessionID, dto.Year, dto.Quarter);

                return Ok(ApiResponseFactory.Success(newSession, "Sao chép dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        private async Task CopyCriteriaInternal(int fromSessionId, int toYear, int toQuarter)
        {
            KPISession oldSession = _kpiSessionRepo.GetByID(fromSessionId);
            if (oldSession == null) return;

            // ===== 1. Xóa criteria cũ =====
            var oldCriteria = _kpiCriteriaRepo.GetAll(x => x.KPICriteriaYear == toYear && x.KPICriteriaQuater == toQuarter && x.IsDeleted == false).ToList();

            foreach (var item in oldCriteria)
            {
                var details = _kpiCriteriaDetailRepo.GetAll(x => x.KPICriteriaID == item.ID).ToList();

                await _kpiCriteriaDetailRepo.DeleteRangeAsync(details);

                item.IsDeleted = true;
                await _kpiCriteriaRepo.UpdateAsync(item);
            }


            // ===== 2. Copy criteria từ session cũ =====
            var criteria = _kpiCriteriaRepo.GetAll(x =>
                    x.IsDeleted == false &&
                    x.KPICriteriaYear == oldSession.YearEvaluation &&
                    x.KPICriteriaQuater == oldSession.QuarterEvaluation
                ).ToList();

            foreach (var c in criteria)
            {
                var details = _kpiCriteriaDetailRepo.GetAll(x => x.KPICriteriaID == c.ID)
                    .ToList();

                c.ID = 0;
                c.KPICriteriaYear = toYear;
                c.KPICriteriaQuater = toQuarter;

                await _kpiCriteriaRepo.CreateAsync(c);

                foreach (var d in details)
                {
                    d.ID = 0;
                    d.KPICriteriaID = c.ID;
                    await _kpiCriteriaDetailRepo.CreateAsync(d);
                }

            }
        }

        [HttpPost("save-kpi-rule")]
        public async Task<IActionResult> SaveKPIRule([FromBody] SaveKPIRuleRequest dto)
        {
            try
            {
                // ===== 1. Validate cơ bản =====
                if (dto.Model == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));

                if (dto.Model.KPISessionID <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn Kỳ đánh giá"));

                if (dto.Model.KPIPositionID <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn Vị trí"));

                if (string.IsNullOrWhiteSpace(dto.Model.RuleCode))
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Mã Rule"));

                if (string.IsNullOrWhiteSpace(dto.Model.RuleName))
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Tên Rule"));

                // ===== 2. Check trùng theo Session + Position =====
                var duplicate = _kpiEvaluationRuleRepo.GetAll(x =>
                    x.KPISessionID == dto.Model.KPISessionID &&
                    x.KPIPositionID == dto.Model.KPIPositionID &&
                    x.ID != dto.Model.ID &&
                    x.IsDeleted == false
                ).FirstOrDefault();

                // ===== 3. COPY MODE =====
                if (dto.IsCopy)
                {
                    if (dto.FromRuleID <= 0)
                        return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn Rule nguồn để copy"));

                    if (duplicate != null && !dto.IsForce)
                    {
                        return Ok(ApiResponseFactory.Success(
                            new { NeedConfirm = true },
                            $"Trong kỳ đã có Rule cho vị trí này. Bạn có muốn ghi đè không?"
                        ));
                    }

                    return await CopyKPIRuleInternal(dto);
                }

                // ===== 4. NORMAL MODE =====
                if (duplicate != null)
                {
                    return BadRequest(ApiResponseFactory.Fail(
                        null,
                        $"Trong kỳ đã tồn tại Rule cho vị trí này"
                    ));
                }

                // ===== 5. Save thường =====
                KPIEvaluationRule entity;

                if (dto.Model.ID > 0)
                {
                    entity = await _kpiEvaluationRuleRepo.GetByIDAsync(dto.Model.ID);
                    if (entity == null)
                        return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy Rule"));

                    entity.RuleCode = dto.Model.RuleCode.Trim();
                    entity.RuleName = dto.Model.RuleName.Trim();
                    entity.KPISessionID = dto.Model.KPISessionID;
                    entity.KPIPositionID = dto.Model.KPIPositionID;

                    await _kpiEvaluationRuleRepo.UpdateAsync(entity);
                }
                else
                {
                    entity = new KPIEvaluationRule
                    {
                        KPISessionID = dto.Model.KPISessionID,
                        KPIPositionID = dto.Model.KPIPositionID,
                        RuleCode = dto.Model.RuleCode.Trim(),
                        RuleName = dto.Model.RuleName.Trim(),
                        IsDeleted = false
                    };

                    await _kpiEvaluationRuleRepo.CreateAsync(entity);
                }

                return Ok(ApiResponseFactory.Success(entity, "Lưu Rule thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //private async Task<IActionResult> CopyKPIRuleInternal(SaveKPIRuleRequest dto)
        //{
        //    // ===== 1. Xóa Rule cũ (nếu có) =====
        //    var oldRule = _kpiEvaluationRuleRepo.GetAll(x =>
        //        x.KPISessionID == dto.Model.KPISessionID &&
        //        x.KPIPositionID == dto.Model.KPIPositionID &&
        //        x.IsDeleted == false
        //    ).FirstOrDefault();

        //    if (oldRule != null)
        //    {
        //        oldRule.IsDeleted = true;
        //        await _kpiEvaluationRuleRepo.UpdateAsync(oldRule);
        //    }

        //    // ===== 2. Tạo Rule mới =====
        //    KPIEvaluationRule newRule = new KPIEvaluationRule
        //    {
        //        KPISessionID = dto.Model.KPISessionID,
        //        KPIPositionID = dto.Model.KPIPositionID,
        //        RuleCode = dto.Model.RuleCode.Trim(),
        //        RuleName = dto.Model.RuleName.Trim(),
        //        IsDeleted = false
        //    };

        //    await _kpiEvaluationRuleRepo.CreateAsync(newRule);

        //    // ===== 3. Copy Rule Detail =====
        //    var details = _kpiEvaluationRuleDetailRepo.GetAll(x =>
        //        x.KPIEvaluationRuleID == dto.FromRuleID &&
        //        x.IsDeleted == false
        //    ).OrderBy(x => x.STT).ToList();

        //    Dictionary<int, int> mapOldNew = new();

        //    foreach (var d in details)
        //    {
        //        int oldId = d.ID;

        //        d.ID = 0;
        //        d.KPIEvaluationRuleID = newRule.ID;
        //        d.ParentID = 0;

        //        await _kpiEvaluationRuleDetailRepo.CreateAsync(d);
        //        mapOldNew[oldId] = d.ID;
        //    }

        //    foreach (var d in details.Where(x => x.ParentID > 0))
        //    {
        //        d.ParentID = mapOldNew[d.ParentID.Value];
        //        await _kpiEvaluationRuleDetailRepo.UpdateAsync(d);
        //    }

        //    return Ok(ApiResponseFactory.Success(newRule, "Sao chép Rule thành công"));
        //}

        private async Task<IActionResult> CopyKPIRuleInternal(SaveKPIRuleRequest dto)
        {
            // ===== 1. Ghi đè Rule cũ (nếu có) =====
            var oldRule = _kpiEvaluationRuleRepo.GetAll(x =>
                x.KPISessionID == dto.Model.KPISessionID &&
                x.KPIPositionID == dto.Model.KPIPositionID &&
                x.IsDeleted == false
            ).FirstOrDefault();

            if (oldRule != null)
            {
                oldRule.IsDeleted = true;
                await _kpiEvaluationRuleRepo.UpdateAsync(oldRule);
            }

            // ===== 2. Tạo Rule mới =====
            KPIEvaluationRule newRule = new KPIEvaluationRule
            {
                KPISessionID = dto.Model.KPISessionID,
                KPIPositionID = dto.Model.KPIPositionID,
                RuleCode = dto.Model.RuleCode.Trim(),
                RuleName = dto.Model.RuleName.Trim(),
                IsDeleted = false
            };

            await _kpiEvaluationRuleRepo.CreateAsync(newRule);

            // ===== 3. Lấy RuleDetail nguồn (sort theo STT) =====
            var lstDetails = _kpiEvaluationRuleDetailRepo.GetAll(x =>
                x.KPIEvaluationRuleID == dto.FromRuleID &&
                x.IsDeleted == false
            ).OrderBy(x => x.STT).ToList();

            // ===== 4. Copy từng RuleDetail =====
            foreach (var item in lstDetails)
            {
                KPIEvaluationRuleDetail newDetail = new KPIEvaluationRuleDetail
                {
                    KPIEvaluationRuleID = newRule.ID,
                    KPIEvaluationID = item.KPIEvaluationID,

                    STT = item.STT,
                    RuleContent = item.RuleContent,
                    FormulaCode = item.FormulaCode,
                    MaxPercent = item.MaxPercent,
                    PercentageAdjustment = item.PercentageAdjustment,
                    MaxPercentageAdjustment = item.MaxPercentageAdjustment,
                    RuleNote = item.RuleNote,
                    Note = item.Note,
                    IsDeleted = false,
                    ParentID = 0
                };

                // Nếu có cha → tìm theo STT
                if (!string.IsNullOrEmpty(item.STT) && item.STT.Contains("."))
                {
                    string parentSTT = item.STT.Substring(0, item.STT.LastIndexOf("."));

                    var parent = _kpiEvaluationRuleDetailRepo.GetAll(x =>
                        x.KPIEvaluationRuleID == newRule.ID &&
                        x.STT == parentSTT &&
                        x.IsDeleted == false
                    ).FirstOrDefault();

                    if (parent != null)
                    {
                        newDetail.ParentID = parent.ID;
                    }
                }

                await _kpiEvaluationRuleDetailRepo.CreateAsync(newDetail);
            }

            return Ok(ApiResponseFactory.Success(newRule, "Sao chép Rule đánh giá thành công"));
        }




        public class SaveKPISessionRequest
        {
            public KPISession model { get; set; }
            public bool IsCopy { get; set; }    
            public bool IsForce { get; set; }    // user đã xác nhận ghi đè hay chưa
        }
        public class SaveKPIRuleRequest
        {
            public KPIEvaluationRule Model { get; set; }
            public bool IsCopy { get; set; }  
            public bool IsForce { get; set; }  // user đã confirm ghi đè
            public int FromRuleID { get; set; } // rule nguồn khi copy
        }



        public class CopyKPISessionRequest
        {
            public int FromSessionID { get; set; }   // cboKPISession
            public int Year { get; set; }
            public int Quarter { get; set; }
            public int DepartmentID { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public bool IsForce { get; set; }        // confirm ghi đè
        }


    }
}
