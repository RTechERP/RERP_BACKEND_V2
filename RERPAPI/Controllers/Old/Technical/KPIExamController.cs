using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Technical.KPI;
using static RERPAPI.Controllers.Old.KPIEmployeeTeamController;

namespace RERPAPI.Controllers.Old.Technical
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KPIExamController : ControllerBase
    {
        private readonly KPISessionRepo _kpiSessionRepo;
        private readonly KPIExamRepo _kpiExamRepo;
        private readonly KPIExamPositionRepo _kpiExamPositionRepo;
        private readonly KPIEvaluationFactorRepo _kpiEvaluationFactorRepo;
        public KPIExamController(KPISessionRepo kPISessionRepo, KPIExamRepo kPIExamRepo, KPIExamPositionRepo kPIExamPositionRepo, KPIEvaluationFactorRepo kpiEvaluationFactorRepo)
        {
            _kpiSessionRepo = kPISessionRepo;
            _kpiExamRepo = kPIExamRepo;
            _kpiExamPositionRepo = kPIExamPositionRepo;
            _kpiEvaluationFactorRepo = kpiEvaluationFactorRepo;
        }


        [HttpGet("get-data-position")]
        public async Task<IActionResult> LoadDataPosition(int kpiExamId, int kpiSessionID)
        {
            try
            {
                var param = new
                {
                    KPIExamID = kpiExamId,
                    KPISessionID = kpiSessionID
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetKPIPositionByExamID", param);
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
                var data = _kpiSessionRepo.GetAll(x => x.IsDeleted == false);

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] SaveDataRequest dto)
        {
            try
            {
                //Validate
                if(dto.KPIExam.ExamCode == null || dto.KPIExam.ExamCode.Trim() == "")
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Mã bài đánh giá không được để trống"));
                }
                if(dto.KPIExam.ExamName == null || dto.KPIExam.ExamName.Trim() == "")
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Tên bài đánh giá không được để trống"));
                }
                if(dto.KPIExam.KPISessionID <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn kỳ đánh giá"));
                }
                if(dto.positionIds.Count <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng ít nhất 1 vị trí"));
                }    
                List<KPIExam> list = _kpiExamRepo.GetAll(x => x.IsDeleted != true && x.ExamCode == dto.KPIExam.ExamCode && x.ID != dto.KPIExam.ID && x.KPISessionID == dto.KPIExam.KPISessionID);
                if(list.Count > 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Mã bài đánh giá [{dto.KPIExam.ExamCode}] đã được sử dụng!"));
                }
                //End validate

                if(dto.KPIExam.ID > 0)
                {
                    await _kpiExamRepo.UpdateAsync(dto.KPIExam);
                }
                else
                {
                    await _kpiExamRepo.CreateAsync(dto.KPIExam);
                }

                var dataDel = _kpiExamPositionRepo.GetAll(x => x.KPIExamID == dto.KPIExam.ID);
                if(dataDel.Count > 0)
                {
                    await _kpiExamPositionRepo.DeleteRangeAsync(dataDel);
                }
                foreach(var positionId in dto.positionIds)
                {
                    KPIExamPosition kPIExamPosition = new KPIExamPosition
                    {
                        KPIExamID = dto.KPIExam.ID,
                        KPIPositionID = positionId
                    };
                    await _kpiExamPositionRepo.CreateAsync(kPIExamPosition);
                }    
                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //COPY

        [HttpGet("get-kpi-session")]
        public IActionResult GetKPISessionForCopy(int departmentId)
        {
            try
            {
                var data = _kpiSessionRepo.GetAll(x => x.IsDeleted == false && x.DepartmentID == departmentId);

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-kpi-exam-copy")]
        public async Task<IActionResult> GetKPIExamForCopy(int departmentId, int kpiSessionId)
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

        [HttpPost("copy-exam")]
        public async Task<IActionResult> CopyExam([FromBody] CopyKPIExamRequest dto)
        {
            try
            {
                //if (dto.SourceExamId <= 0 || dto.TargetExamId <= 0)
                //    return BadRequest(ApiResponseFactory.Fail(null, "Exam không hợp lệ"));
                if(dto.SourceExamId <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn bài thi muốn sao chép"));
                if (dto.TargetExamId <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn bài thi muốn sao chép tới"));


                if (dto.SourceExamId == dto.TargetExamId)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không thể copy trùng bài thi"));

                // Lấy tiêu chí bài nguồn
                var source = _kpiEvaluationFactorRepo
                    .GetAll(x => x.KPIExamID == dto.SourceExamId && x.IsDeleted == false)
                    .OrderBy(x => x.STT)
                    .ToList();

                // Lấy tiêu chí bài đích
                var target = _kpiEvaluationFactorRepo
                    .GetAll(x => x.KPIExamID == dto.TargetExamId && x.IsDeleted == false)
                    .ToList();

                // Nếu có dữ liệu và không cho overwrite
                if (target.Count > 0 && dto.Overwrite == false)
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu đã tồn tại"));

                // Ghi đè → IsDeleted = true
                if (target.Count > 0)
                {
                    foreach (var item in target)
                    {
                        item.IsDeleted = true;
                    }
                    await _kpiEvaluationFactorRepo.UpdateRangeAsync(target);
                }

                // Copy
                //foreach (var detail in source)
                //{
                //    var model = new KPIEvaluationFactor
                //    {
                //        KPIExamID = dto.TargetExamId,
                //        STT = detail.STT,
                //        EvaluationContent = detail.EvaluationContent,
                //        VerificationToolsContent = detail.VerificationToolsContent,
                //        StandardPoint = detail.StandardPoint,
                //        Coefficient = detail.Coefficient,
                //        Unit = detail.Unit,
                //        EvaluationType = detail.EvaluationType,
                //        SpecializationType = detail.SpecializationType,
                //        IsDeleted = false
                //    };

                //    await _kpiEvaluationFactorRepo.CreateAsync(model);

                //    // Gán Parent sau khi có ID
                //    model.ParentID = GetParentId(model.STT, dto.TargetExamId, model.EvaluationType ?? 0);
                //    await _kpiEvaluationFactorRepo.UpdateAsync(model);
                //}

                var map = new Dictionary<string, int>();

                foreach (var detail in source)
                {
                    var model = new KPIEvaluationFactor
                    {
                        KPIExamID = dto.TargetExamId,
                        STT = detail.STT,
                        EvaluationContent = detail.EvaluationContent,
                        VerificationToolsContent = detail.VerificationToolsContent,
                        StandardPoint = detail.StandardPoint,
                        Coefficient = detail.Coefficient,
                        Unit = detail.Unit,
                        EvaluationType = detail.EvaluationType,
                        SpecializationType = detail.SpecializationType,
                        IsDeleted = false
                    };

                    // Set ParentID bằng map trước
                    model.ParentID = GetParentIdFromMap(model.STT, map);

                    await _kpiEvaluationFactorRepo.CreateAsync(model);

                    // Lưu lại ID vừa insert
                    map[model.STT] = model.ID;
                }


                return Ok(ApiResponseFactory.Success(null, "Copy thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        private int GetParentId(string stt, int examId, int evaluationType)
        {
            if (string.IsNullOrEmpty(stt) || !stt.Contains(".")) return 0;

            var parentSTT = stt.Substring(0, stt.LastIndexOf(".")).Trim();

            var parent = _kpiEvaluationFactorRepo.GetAll(x =>
                    x.KPIExamID == examId
                    && x.EvaluationType == evaluationType
                    && x.STT == parentSTT
                    && x.IsDeleted == false
                ).FirstOrDefault();

            return parent?.ID ?? 0;
        }

        private int GetParentIdFromMap(string stt, Dictionary<string, int> map)
        {
            if (string.IsNullOrEmpty(stt) || !stt.Contains(".")) return 0;

            var parentSTT = stt.Substring(0, stt.LastIndexOf(".")).Trim();

            return map.ContainsKey(parentSTT) ? map[parentSTT] : 0;
        }

        public class SaveDataRequest
        {
            public KPIExam KPIExam { get; set; }
            public List<int> positionIds { get; set; }
        }

        public class CopyKPIExamRequest
        {
            public int SourceExamId { get; set; }   // cboExamCopyValue
            public int TargetExamId { get; set; }   // cboExamValue
            public bool Overwrite { get; set; }     // true = ghi đè
        }

    }
}
