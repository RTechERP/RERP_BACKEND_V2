using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Technical;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KPIEvaluationController : ControllerBase

    {
        private readonly KPIEvaluationRepo _kpiEvaluationRepo = new KPIEvaluationRepo();
        KPIEvaluationErrorRepo _kpiEvaluationErrorRepo = new KPIEvaluationErrorRepo();
        [HttpGet("get-kpievaluation")]
        public IActionResult GetKPIEvaluation(int departmentID)
        {
            try
            {
                var kpievaluations = SQLHelper<object>.ProcedureToList("spGetKPIEvaluation",
                                                new string[] { "@DepartmentID" },
                                                new object[] { departmentID });
                var data = SQLHelper<object>.GetListData(kpievaluations, 0);

                return Ok(ApiResponseFactory.Success(data, ""));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("savedata")]
        public async Task<IActionResult> SaveData([FromBody] KPIEvaluation kpievaluation)
        {
            try
            {

                if (kpievaluation.ID <= 0) await _kpiEvaluationRepo.CreateAsync(kpievaluation);
                else await _kpiEvaluationRepo.UpdateAsync(kpievaluation);

                return Ok(ApiResponseFactory.Success(kpievaluation, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        public class SetErrorMappingRequest
        {
            public int EvaluationId { get; set; }
            public List<int> ErrorIDs { get; set; } = new(); // danh sách KPIError.ID
        }

        [HttpPost("set-error-mapping")]
        public async Task<IActionResult> SetErrorMapping([FromBody] SetErrorMappingRequest req)
        {
            try
            {
                if (req == null || req.EvaluationId <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "evaluationId không hợp lệ"));

                // --- Xóa mapping cũ ---
                var oldMappings = _kpiEvaluationErrorRepo.GetAll(x => x.KPIEvaluationID == req.EvaluationId);
                if (oldMappings != null && oldMappings.Any())
                {
                    _kpiEvaluationErrorRepo.DeleteRange(oldMappings);
                }

                // --- Thêm mapping mới ---
                if (req.ErrorIDs != null && req.ErrorIDs.Any())
                {
                    var newMappings = req.ErrorIDs
                        .Distinct()
                        .Select(errorId => new KPIEvaluationError
                        {
                            KPIEvaluationID = req.EvaluationId,
                            KPIErrorID = errorId
                        })
                        .ToList();

                    _kpiEvaluationErrorRepo.CreateRange(newMappings);
                }

                return Ok(ApiResponseFactory.Success(
                    new
                    {
                        evaluationId = req.EvaluationId,
                        total = req.ErrorIDs?.Distinct().Count() ?? 0
                    },
                    "Cập nhật danh sách mã lỗi thành công!"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpGet("check-duplicate-kpie/{id}/{EvaluationCode}")]
        public IActionResult CheckDuplicateKPIE(int id, string EvaluationCode)
        {
            try
            {
                bool isDuplicate = false;



                var existKPIE = _kpiEvaluationRepo.GetAll(x => x.ID != id &&
                                x.EvaluationCode == EvaluationCode &&

                                x.IsDeleted == false);

                if (existKPIE.Any())
                {
                    isDuplicate = true;
                }

                return Ok(new
                {
                    status = 1,
                    data = isDuplicate
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = "Lỗi kiểm tra trùng ENF",
                    error = ex.Message
                });
            }
        }

        [HttpGet("get-error-evaluation")]
        public IActionResult GetErroeEvaluation(int evaluationID)
        {
            try
            {
                var errEvaluations = SQLHelper<object>.ProcedureToList("spGetErrorByEvaluation",
                                                new string[] { "@EvaluationID" },
                                                new object[] { evaluationID });
                var data = SQLHelper<object>.GetListData(errEvaluations, 0);

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }



    }
}
