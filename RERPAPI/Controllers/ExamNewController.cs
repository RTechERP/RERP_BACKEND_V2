using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamNewController : ControllerBase
    {
        private readonly ExamResultRepo _erRepo;
        private readonly ExamResultDetailRepo _erdRepo;
        private readonly ExamResultAnswerDetailRepo _eradRepo;

        public ExamNewController(
            ExamResultRepo erRepo,
            ExamResultDetailRepo erdRepo,
            ExamResultAnswerDetailRepo eradRepo)
        {
            _erRepo = erRepo;
            _erdRepo = erdRepo;
            _eradRepo = eradRepo;
        }

        // Tạo đề
        [HttpPost("tests")]
        public async Task<IActionResult> Tests([FromForm] int YearValue, [FromForm] int Season, [FromForm] int TestType,
            [FromForm] int EmployeeID, [FromForm] string LoginName)
        {
            int rollbackExamID = 0;
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spCheckExamResultStatus",
                            new string[] { "@YearValue", "@Season", "@TestType", "@EmployeeID" },
                            new object[] { YearValue, Season, TestType, EmployeeID });

                var existingExamResult = SQLHelper<object>.GetListData(data, 0).FirstOrDefault();
                if (existingExamResult == null)
                {
                    throw new Exception("Không lấy được thông tin trạng thái bài thi.");
                }

                var row = (IDictionary<string, object>)existingExamResult;
                var existed = TextUtils.ToBoolean(row.ContainsKey("existed") ? row["existed"] : row["Existed"]);
                var total = TextUtils.ToInt32(row.ContainsKey("total") ? row["total"] : row["Total"]);
                var duration = TextUtils.ToDecimal(row.ContainsKey("duration") ? row["duration"] : row["Duration"]);

                if (existed)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Bạn đã làm bài thi này."));
                }
                else
                {
                    ExamResult newExam = new()
                    {
                        YearValue = YearValue,
                        Season = Season,
                        TestType = TestType,
                        EmployeeID = EmployeeID,
                        CreatedBy = LoginName,
                        CreatedDate = DateTime.Now
                    };
                    await _erRepo.CreateAsync(newExam);
                    rollbackExamID = newExam.ID;

                    var newQuestions = SQLHelper<ExamResultDetail>.ProcedureToListModel("spCreateExamQuestions",
                            new string[] { "@ExamResultID", "@LoginName" },
                            new object[] { newExam.ID, LoginName })
                        ?? throw new Exception();

                    List<ExamResultAnswerDetail> answers = new();
                    foreach (var question in newQuestions)
                    {
                        var newAnswers = SQLHelper<ExamResultAnswerDetail>.ProcedureToListModel("spCreateExamAnswers",
                            new string[] { "@ExamResultDetailID", "@CourseQuestionID", "@LoginName" },
                            new object[] { question.ID, question.CourseQuestionID, LoginName });
                        answers.AddRange(newAnswers);
                    }

                    return Ok(ApiResponseFactory.Success(new
                    {
                        examID = newExam.ID,
                        total,
                        duration
                    }, $"Bài thi có {total} câu, thời lượng {duration} phút."));
                }
            }
            catch (Exception ex)
            {
                if (rollbackExamID > 0)
                {
                    _erRepo.Delete(rollbackExamID);
                }
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Lấy dữ liệu đề thi
        [HttpGet("tests")]
        public IActionResult Tests(int YearValue, int Season, int TestType, int EmployeeID)
        {
            try
            {
                var content = SQLHelper<ExamContentDTO>.ProcedureToListModel("spGetExamTestContent",
                    new string[] { "@YearValue", "@Season", "@TestType", "@EmployeeID" },
                    new object[] { YearValue, Season, TestType, EmployeeID });

                if (content == null || content.Count == 0)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Lấy dữ liệu đề thi thất bại!"));
                }

                return Ok(ApiResponseFactory.Success(content, "Lấy dữ liệu đề thi thành công"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // update câu hỏi
        [HttpPost("answers")]
        public async Task<IActionResult> Answers([FromBody] List<ExamContentDTO> answers)
        {
            try
            {
                foreach (var answer in answers)
                {
                    var ans = _eradRepo.GetByID(answer.ID);
                    if (ans == null || ans.ID <= 0)
                    {
                        throw new Exception("Câu trả lời đã bị xóa?");
                    }
                    ans.IsPicked = answer.IsPicked;
                    ans.UpdatedBy = answer.CreatedBy;
                    ans.UpdatedDate = DateTime.Now;
                    await _eradRepo.UpdateAsync(ans);
                }
                return Ok(ApiResponseFactory.Success(null, "Lưu câu trả lời thành công"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Hoàn thành bài thi
        [HttpPost("result")]
        public IActionResult Result([FromForm] int Id, [FromForm] string LoginName, [FromForm] int EmployeeID)
        {
            try
            {
                var exam = _erRepo.GetByID(Id);
                if (exam == null || exam.ID <= 0 || exam.EmployeeID != EmployeeID)
                {
                    throw new Exception("Đã có lỗi xảy ra.");
                }

                SQLHelper<object>.ExcuteProcedure("spCalculateExamConclusion",
                    new string[] { "@ExamID", "@LoginName" },
                    new object[] { Id, LoginName });

                return Ok(ApiResponseFactory.Success(null, "Bài thi hoàn thành"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Trả kết quả thi
        [HttpGet("result")]
        public IActionResult Result(int Id)
        {
            try
            {
                var exam = _erRepo.GetByID(Id);
                if (exam == null || exam.ID <= 0)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Không tìm thấy kết quả thi"));
                }
                return Ok(ApiResponseFactory.Success(exam, "Lấy kết quả thi thành công"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
