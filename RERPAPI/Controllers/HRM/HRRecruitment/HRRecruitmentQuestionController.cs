using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.HRRecruitmentExamRepo;

namespace RERPAPI.Controllers.HRM.HRRecruitment
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HRRecruitmentQuestionController : ControllerBase
    {
        HRRecruitmentExamRepo _hrRecruitmentExamRepo;
        HRRecruitmentQuestionRepo _hrRecruitmentQuestionRepo;
        HRRecruitmentAnswersRepo _hrRecruitmentAnswersRepo;
        HRRecruitmentRightAnswearsRepo _hrRecruitmentRightAnswearsRepo;
        HRRecruitmentQuestionImageRepo _hrRecruitmentQuestionImageRepo;
        public HRRecruitmentQuestionController(HRRecruitmentQuestionRepo hrRecruitmentQuestionRepo, HRRecruitmentAnswersRepo hrRecruitmentAnswersRepo, HRRecruitmentRightAnswearsRepo hrRecruitmentRightAnswearsRepo, HRRecruitmentExamRepo hRRecruitmentExamRepo, HRRecruitmentQuestionImageRepo hrRecruitmentQuestionImageRepo)
        {
            _hrRecruitmentQuestionRepo = hrRecruitmentQuestionRepo;
            _hrRecruitmentAnswersRepo = hrRecruitmentAnswersRepo;
            _hrRecruitmentRightAnswearsRepo = hrRecruitmentRightAnswearsRepo;
            _hrRecruitmentExamRepo = hRRecruitmentExamRepo;
            _hrRecruitmentQuestionImageRepo = hrRecruitmentQuestionImageRepo;
        }
        #region lấy dữ liệu câu hỏi - đáp án
        [RequiresPermission("N1,N2,N32,N33,N38,N51,N52,N56,N61,N79,N81,N86")]
        [HttpGet("get-data-question-answers")]
        public async Task<IActionResult> getDataQuestionAnswers(int examID)
        {
            try
            {
                var param = new
                {
                    ExamID = examID
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetHRRecruitmentQuestion", param);
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
        #endregion
        #region lấy đáp án đúng theo mã câu hỏi 
        [RequiresPermission("N1,N2,N32,N33,N38,N51,N52,N56,N61,N79,N81,N86")]
        [HttpGet("get-data-right-answers")]
        public async Task<IActionResult> getDataRightAnswers(int questionID)
        {
            try
            {
                var param = new
                {
                    recruitmentQuestionID = questionID
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetRecruitmentAnswersByCourseQuestionID", param);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion

        #region save data câu hỏi - đáp án
        [RequiresPermission("N1,N2,N32,N33,N38,N51,N52,N56,N61,N79,N81,N86")]
        [HttpPost("save-data-question-answers")]
        public async Task<IActionResult> saveDataQuestionAnswers([FromBody] HRRecruitmentQuestionAnswersDTO item)
        {
            try
            {
                string messageError = string.Empty;
                if (!_hrRecruitmentQuestionRepo.CheckValidate(item, out messageError))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, messageError));
                }
                HRRecruitmentQuestion question = new HRRecruitmentQuestion();
                if (item.question.ID != null && item.question.ID > 0)
                {
                    question = _hrRecruitmentQuestionRepo.GetByID(item.question.ID);
                }
                question.QuestionText = item.question.QuestionText;
                question.STT = item.question.STT;
                question.RecruitmentExamID = item.question.RecruitmentExamID;
                question.Point = item.question.Point;
                //question.Image = item.question.Image;
                question.QuestionType = item.question.QuestionType;
                question.EssayGuidance = item.question.EssayGuidance;
                question.IsAnswerNumberValue = item.question.IsAnswerNumberValue;
                if (question.ID == null || question.ID <= 0)
                {
                    await _hrRecruitmentQuestionRepo.CreateAsync(question);
                }
                else
                {
                    await _hrRecruitmentQuestionRepo.UpdateAsync(question);
                }
                // lưu ảnh câu hỏi
                if (item.litsQuestionImage.Count > 0)
                {
                    foreach (var i in item.litsQuestionImage)
                    {
                        if (i.ID > 0)
                        {
                            i.RecruitmentQuestionID = question.ID;
                            await _hrRecruitmentQuestionImageRepo.UpdateAsync(i);
                        }
                        else
                        {
                            i.RecruitmentQuestionID = question.ID;
                            await _hrRecruitmentQuestionImageRepo.CreateAsync(i);
                        }
                    }
                }
                //xóa ảnh câu hỏi nếu có 
                if(item.listImageIDDelete != null && item.listImageIDDelete.Any())
                {
                    foreach( var i in item.listImageIDDelete)
                    {
                        var ImageDelete = _hrRecruitmentQuestionImageRepo.GetByID(i);
                        ImageDelete.IsDeleted = true;
                        await _hrRecruitmentQuestionImageRepo.UpdateAsync(ImageDelete);
                    }
                    //var listImageDelete = _hrRecruitmentQuestionImageRepo.GetAll(x => item.listImageIDDelete.Contains(x.ID)).ToList();
                    //foreach (var i in listImageDelete)
                    //{
                    //    i.IsDeleted = true;
                    //}
                    //await _hrRecruitmentQuestionImageRepo.UpdateRangeAsync_Binh(listImageDelete);
                }
                //xóa các đáp án đúng cũ của câu hỏi
                var rightAnswers = _hrRecruitmentRightAnswearsRepo.GetAll(x => x.RecruitmentQuestionID == question.ID && x.IsDeleted == false).ToList();
                await _hrRecruitmentRightAnswearsRepo.DeleteRangeAsync(rightAnswers);
                foreach (var a in item.answers)
                {
                    HRRecruitmentAnswer answer = new HRRecruitmentAnswer();
                    answer.AnswersText = a.AnswersText;
                    answer.RecruitmentQuestionID = question.ID;
                   answer.ImageLink = a.ImageLink;
                    answer.AnswersNumber = a.AnswersNumber;
                    if (a.ID > 0)
                    {
                        answer.ID = a.ID;
                        await _hrRecruitmentAnswersRepo.UpdateAsync(answer);
                    }
                    else
                    {
                        await _hrRecruitmentAnswersRepo.CreateAsync(answer);
                    }
                    if (a.IsRightAnswer)
                    {
                        HRRecruitmentRightAnswer rightAnswear = new HRRecruitmentRightAnswer();
                        rightAnswear.RecruitmentQuestionID = question.ID;
                        rightAnswear.RecruitmentAnswerID = answer.ID;
                        await _hrRecruitmentRightAnswearsRepo.CreateAsync(rightAnswear);
                    }
                }
                if (item.listAnswerIDDelete != null && item.listAnswerIDDelete.Count > 0)
                {
                    var listAnswerDelete = _hrRecruitmentAnswersRepo.GetAll(x => item.listAnswerIDDelete.Contains(x.ID)).ToList();
                    foreach (var i in listAnswerDelete)
                    {
                        i.IsDeleted = true;
                    }
                    await _hrRecruitmentAnswersRepo.UpdateRangeAsync_Binh(listAnswerDelete);
                }
                return Ok(ApiResponseFactory.Success(null, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
  
        #region danh cho fill dữ liệu sửa  
        //lấy STT lớn nhất của câu hỏi trong 1 đề thi
        [HttpGet("get-max-stt-question")]
        public int getMaxSTTQuestion(int examID)
        {
            var data = _hrRecruitmentQuestionRepo.GetAll(x => x.RecruitmentExamID == examID && x.IsDeleted != true);
            if (data != null && data.Count() > 0)
            {
                return data.Max(x => x.STT) ?? 0;
            }
            return 0;
        }
        //load dữ liệu câu hỏi 
        [HttpGet("get-data-question-by-id")]
        public async Task<IActionResult> getDataQuestionByID(int questionID)
        {
            try
            {
                var data = _hrRecruitmentQuestionRepo.GetByID(questionID);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //load dữ liệu đáp án của câu hỏi
        [HttpGet("get-data-answers-by-question-id")]
        public async Task<IActionResult> getDataAnswersByQuestionID(int questionID)
        {
            try
            {
                var param = new
                {
                    QuestionID = questionID
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetRecruitmentAnswers", param);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
        #region delete câu hỏi
        [RequiresPermission("N1,N2,N32,N33,N38,N51,N52,N56,N61,N79,N81,N86")]
        [HttpPost("delete-question")]
        public async Task<IActionResult> deleteQuestion(List<int> listQuestionID)
        {
            try
            {
                foreach (var item in listQuestionID)
                {
                    var question = _hrRecruitmentQuestionRepo.GetByID(item);
                    var answers = _hrRecruitmentAnswersRepo.GetAll(x => x.RecruitmentQuestionID == item && x.IsDeleted == false).ToList();
                    var answerRight = _hrRecruitmentRightAnswearsRepo.GetAll(x => x.RecruitmentQuestionID == item && x.IsDeleted == false).ToList();
                    if (question != null)
                    {
                        question.IsDeleted = true;
                        await _hrRecruitmentQuestionRepo.UpdateAsync(question);
                    }
                    if (answerRight != null && answerRight.Count > 0)
                    {
                        foreach (var i in answerRight)
                        {
                            i.IsDeleted = true;
                        }
                        await _hrRecruitmentRightAnswearsRepo.UpdateRangeAsync_Binh(answerRight);
                    }
                    if (answers != null && answers.Count > 0)
                    {
                        foreach (var i in answers)
                        {
                            i.IsDeleted = true;
                        }
                        await _hrRecruitmentAnswersRepo.UpdateRangeAsync_Binh(answers);
                    }
                }
                return Ok(ApiResponseFactory.Success(listQuestionID, "Xóa câu hỏi thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
        #region lấy danh sách ảnh của câu hỏi
        [RequiresPermission("N1,N2,N32,N33,N38,N51,N52,N56,N61,N79,N81,N86")]
        [HttpGet("get-question-images")]
        public async Task<IActionResult> getListImageByQuestionID(int questionID)
        {
            try
            {
                var data = _hrRecruitmentQuestionImageRepo.GetAll(x => x.RecruitmentQuestionID == questionID && x.IsDeleted != true).ToList();
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
