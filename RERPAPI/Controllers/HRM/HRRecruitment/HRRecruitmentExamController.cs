using DocumentFormat.OpenXml.Office.SpreadSheetML.Y2023.MsForms;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRRecruitmentExamRepo;
using RERPAPI.Repo.GenericEntity.Technical.KPI;

namespace RERPAPI.Controllers.HRM.HRRecruitment
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HRRecruitmentExamController : ControllerBase
    {
        HRRecruitmentExamRepo _hrRecruitmentExamRepo;
        HRRecruitmentQuestionRepo _hrRecruitmentQuestionRepo;
        HRRecruitmentAnswersRepo _hrRecruitmentAnswersRepo;
        HRRecruitmentRightAnswearsRepo _hrRecruitmentRightAnswearsRepo;
        HRRecruitmentExamResultRepo _hrRecruitmentExamResultRepo;
        HRRecruitmentExamResultDetailRepo _hrRecruitmentExamResultDetailRepo;
        HRRecruitmentExamResultImageRepo _hrRecruitmentExamResultImageRepo;
        public HRRecruitmentExamController(HRRecruitmentQuestionRepo hrRecruitmentQuestionRepo, HRRecruitmentAnswersRepo hrRecruitmentAnswersRepo, HRRecruitmentRightAnswearsRepo hrRecruitmentRightAnswearsRepo, HRRecruitmentExamRepo hRRecruitmentExamRepo, HRRecruitmentExamResultRepo hrRecruitmentExamResultRepo, HRRecruitmentExamResultDetailRepo hrRecruitmentExamResultDetailRepo, HRRecruitmentExamResultImageRepo hrRecruitmentExamResultImageRepo)
        {
            _hrRecruitmentQuestionRepo = hrRecruitmentQuestionRepo;
            _hrRecruitmentAnswersRepo = hrRecruitmentAnswersRepo;
            _hrRecruitmentRightAnswearsRepo = hrRecruitmentRightAnswearsRepo;
            _hrRecruitmentExamRepo = hRRecruitmentExamRepo;
            _hrRecruitmentExamResultRepo = hrRecruitmentExamResultRepo;
            _hrRecruitmentExamResultDetailRepo = hrRecruitmentExamResultDetailRepo;
            _hrRecruitmentExamResultImageRepo = hrRecruitmentExamResultImageRepo;
        }
        #region load dữ liệu exam
        [RequiresPermission("N1,N2,N20,N32,N33,N38,N51,N52,N56,N61,N78,N79,N81,N86")]
        [HttpGet("get-data-exam")]
        public async Task<IActionResult> getDataExam(int departmentID, string? filter)
        {
            try
            {
                var param = new
                {
                    DepartmentID = departmentID,
                    FilterText = filter ?? ""
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetHRRecruitmentExam", param);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
        #region load dữ liêu chi tiết exam
        [HttpGet("get-data-exam-by-id")]
        public async Task<IActionResult> getDataExamByID(int examID)
        {
            try
            {
                var data = _hrRecruitmentExamRepo.GetByID(examID);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
       
        #region save exam
        //load data cbb đợt tuyển dụng cho trường hợp thêm mới ( chỉ lấy những vị trí chưa có đề thi) 
        [HttpGet("get-data-cbb-hiring-request-insert")]
        public async Task<IActionResult> GetDataCbbHiringRequestInsert(int departmentID)
        {
            try
            {
                var param = new
                {
                    DepartmentID = departmentID
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetHiringRequest_ComboBoxInsert", param);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2,N20,N32,N33,N38,N51,N52,N56,N61,N78,N79,N81,N86")]
        [HttpPost("save-data-exam")]
        public async Task<IActionResult> SaveDataExam([FromBody] HRRecruitmentExam request)
        {
            try
            {
                string messageError = string.Empty;
                if (!_hrRecruitmentExamRepo.CheckValidate(request, out messageError))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, messageError));
                }
                if (request.ID > 0)
                {
                    await _hrRecruitmentExamRepo.UpdateAsync(request);
                }
                else
                {
                    await _hrRecruitmentExamRepo.CreateAsync(request);
                }
                return Ok(ApiResponseFactory.Success(request, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
        #region đóng/mở hoạt động cho bài thi
        //[HttpPost("active-exam")]
        //public async Task<IActionResult> ActiveExam(int examID, bool isActive)
        //{
        //    try
        //    {
        //        if (examID <= 0)
        //        {
        //            return BadRequest(ApiResponseFactory.Fail(null, "Mã đề thi không hợp lệ"));
        //        }
        //        HRRecruitmentExam exam = _hrRecruitmentExamRepo.GetByID(examID);
        //        if (exam == null)
        //        {
        //            return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy đề thi"));
        //        }

        //        var exist = _hrRecruitmentExamRepo.GetAll(x => x.IsActive == true
        //                                                    && x.IsDeleted != true
        //                                                    && x.ID != exam.ID
        //                                                    && x.HiringRequestID == exam.HiringRequestID);

        //        if (exist.Count > 0 && isActive == true)
        //            {
        //                return BadRequest(ApiResponseFactory.Fail(null, $"Yêu cầu tuyển dụng đã có bài thi hoạt động. Vui lòng kiểm tra lại!"));
        //        }
        //        exam.IsActive = isActive;
        //        await _hrRecruitmentExamRepo.UpdateAsync(exam);
        //        return Ok(ApiResponseFactory.Success(exam, "Cập nhật trạng thái thành công!"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}
        #endregion

        #region delete exam
        [RequiresPermission("N1,N2,N20,N32,N33,N38,N51,N52,N56,N61,N78,N79,N81,N86")]
        [HttpPost("delete-data-exam")]
        public async Task<IActionResult> DeleteDataExam(int examID)
        {
            try
            {
                var dataDelete = _hrRecruitmentExamRepo.GetByID(examID);
                if (dataDelete == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Đề thi không tồn tại hoặc đã bị xóa!"));
                }
                if (_hrRecruitmentQuestionRepo.GetAll(x => x.RecruitmentExamID == examID && x.IsDeleted == false).Count() > 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không thể xóa đề thi [{dataDelete.NameExam}] vì đang có câu hỏi liên quan.\n Vui lòng xóa câu hỏi trước!"));
                }
                dataDelete.IsDeleted = true;
                await _hrRecruitmentExamRepo.UpdateAsync(dataDelete);
                return Ok(ApiResponseFactory.Success(dataDelete, "Xóa đề thi thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion

        #region các api liên quan đến chức năng copy 
        //load data cbb đợt tuyển dụng
        [HttpGet("get-data-cbb-hiring-request")]
        public async Task<IActionResult> GetDataCbbHiringRequest(int departmentID)
        {
            try
            {
                var param = new
                {
                    DepartmentID = departmentID
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("r ", param);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //load danh sách câu hỏi để copy
        [HttpPost("get-data-questionAnswers")]
        public async Task<IActionResult> GetDataQuestionAnswers([FromBody] spGetHRRecruitmentExamQuestionContentParam requestpr)
        {
            try
            {
                var param = new
                {
                    requestpr.ExamType,
                    requestpr.HRRecruitmentExamID,
                    requestpr.FilterText
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetHRRecruitmentExamQuestionContent", param);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // copy câu hỏi 

        [RequiresPermission("N1,N2,N20,N32,N33,N38,N51,N52,N56,N61,N78,N79,N81,N86")]
        [HttpPost("copy-question-answers")]
        public async Task<IActionResult> CopyQuestionAnswers([FromBody] CopyQuestionAnswersParam request)
        {
            try
            {
                var exam = _hrRecruitmentExamRepo.GetByID(request.HRRecruitmentExamID ?? 0);
                if (exam == null)
                    return BadRequest("Exam không tồn tại");

                int examType = exam.ExamType ?? 0;
                int examID = exam.ID;

                var listQuestions = _hrRecruitmentQuestionRepo
                    .GetAll(x => x.RecruitmentExamID == examID && x.IsDeleted == false)
                    .ToList();

                int maxSttQuestion = listQuestions.Any() ? listQuestions.Max(x => x.STT ?? 0) : 0;

                foreach (var questionID in request.ListQuestionID)
                {
                    if (questionID <= 0) continue;

                    var existed = _hrRecruitmentQuestionRepo
                        .GetAll(x => x.RecruitmentExamID == examID && x.ID == questionID && x.IsDeleted == false)
                        .FirstOrDefault();

                    if (existed != null) continue;

                    var oldQuestion = _hrRecruitmentQuestionRepo.GetByID(questionID);
                    if (oldQuestion == null) continue;

                    oldQuestion.ID = 0;
                    oldQuestion.RecruitmentExamID = examID;
                    oldQuestion.STT = ++maxSttQuestion;

                    await _hrRecruitmentQuestionRepo.CreateAsync(oldQuestion);

                    int newQuestionID = oldQuestion.ID;

                    if (examType == 1 || examType == 3)
                    {
                        var answers = _hrRecruitmentAnswersRepo
                            .GetAll(x => x.RecruitmentQuestionID == questionID && x.IsDeleted == false)
                            .ToList();

                        if (!answers.Any()) continue;

                        // map oldAnswerID -> newAnswerID
                        var answerMap = new Dictionary<int, int>();

                        foreach (var ans in answers)
                        {
                            int oldAnswerID = ans.ID;

                            ans.ID = 0;
                            ans.RecruitmentQuestionID = newQuestionID;

                            await _hrRecruitmentAnswersRepo.CreateAsync(ans);

                            int newAnswerID = ans.ID;

                            answerMap.Add(oldAnswerID, newAnswerID);
                        }

                        var correctAnswers = _hrRecruitmentRightAnswearsRepo
                            .GetAll(x => x.RecruitmentQuestionID == questionID && x.IsDeleted == false)
                            .ToList();

                        foreach (var ra in correctAnswers)
                        {
                            int oldAnswerID = ra.RecruitmentAnswerID ?? 0;

                            ra.ID = 0;
                            ra.RecruitmentQuestionID = newQuestionID;

                            if (answerMap.ContainsKey(oldAnswerID))
                            {
                                ra.RecruitmentAnswerID = answerMap[oldAnswerID];
                            }

                            await _hrRecruitmentRightAnswearsRepo.CreateAsync(ra);
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(request, "Copy câu hỏi thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion

        #region liên quan đến làm bài thi tuyển dụng 
        // API cho phần thi Trắc nghiệm
        [HttpPost("create-exam-Recruitment-result")]
        public async Task<IActionResult> CreateExamRecruitmentResult(int recruitmentExamID)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var hRRecruitmentExamResult = new HRRecruitmentExamResult();
                hRRecruitmentExamResult.RecruitmentExamID = recruitmentExamID;
                hRRecruitmentExamResult.EmployeeID = currentUser.EmployeeID;
                hRRecruitmentExamResult.TotalCorrect = 0;
                hRRecruitmentExamResult.TotalIncorrect = 0;
                hRRecruitmentExamResult.PercentageCorrect = 0;
                //hRRecruitmentExamResult. = 0;
                await _hrRecruitmentExamResultRepo.CreateAsync(hRRecruitmentExamResult);

                if (hRRecruitmentExamResult.ID > 0)
                {
                    return Ok(ApiResponseFactory.Success(hRRecruitmentExamResult, "Tạo kết quả thi ứng tuyển thành công!"));
                }
                return BadRequest(ApiResponseFactory.Fail(null, "Tạo mới kết quả thi ứng tuyển thất bại"));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //api lấy thông tin bài thi theo ứng viên 
        [HttpGet("get-data-exam-by-employee")]
        public async Task<IActionResult> GetDataExamByEmployee()
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentCandidate(claims);
                var param = new
                {
                    HRRecruitmentCandidateID = currentUser.ID,
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetExamByCandidate", param);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // lấy thông tin câu hỏi - đáp án cho thi 
        [HttpPost("get-data-question-answers-by-exam")]
        public async Task<IActionResult> GetDataQuestionAnswersByExam(int examID)
        {
            try
            {
                var param = new
                {
                    ExamID = examID
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetRecruitmentExamQuestionAnswer", param);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // tạo mới kết quả 
        [HttpPost("create-exam-hr-recruitment-result")]
        public async Task<IActionResult> CreateExamRecruitmentResult([FromBody] HRRecruitmentExamResult request)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentCandidate(claims);
                
                // Kiểm tra xem đã có bản ghi nào "Đang làm" hay chưa để tránh thừa bản ghi
                var existingResult = _hrRecruitmentExamResultRepo.GetAll(x => 
                    x.EmployeeID == currentUser.ID && 
                    x.RecruitmentExamID == request.RecruitmentExamID && 
                    x.StatusResult == 0 && 
                    x.IsDeleted == false).FirstOrDefault();

                if (existingResult != null)
                {
                    // Reset dữ liệu cũ để ứng viên làm lại từ đầu (No Resume)
                    var oldDetails = _hrRecruitmentExamResultDetailRepo.GetAll(x => x.RecruitmentExamResultID == existingResult.ID);
                    foreach (var detail in oldDetails)
                    {
                        await _hrRecruitmentExamResultDetailRepo.DeleteAsync(detail.ID);
                    }

                    // Reset điểm số
                    existingResult.TotalCorrect = 0;
                    existingResult.TotalIncorrect = 0;
                    existingResult.PercentageCorrect = 0;
                    existingResult.TotalScore = 0;
                    existingResult.MaxPossibleScore = 0;
                    await _hrRecruitmentExamResultRepo.UpdateAsync(existingResult);

                    return Ok(ApiResponseFactory.Success(existingResult, "Khởi động lại bài thi cũ!"));
                }

                request.EmployeeID = currentUser.ID;
                request.StatusResult = 0; // cực kỳ quan trọng: 0 là đang làm
                await _hrRecruitmentExamResultRepo.CreateAsync(request);
                if (request.ID > 0)
                {
                    return Ok(ApiResponseFactory.Success(request, "Tạo kết quả thi ứng tuyển thành công!"));
                }
                return BadRequest(ApiResponseFactory.Fail(null, "Tạo mới kết quả thi ứng tuyển thất bại"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-question-progress")]
        public async Task<IActionResult> SaveQuestionProgress([FromBody] SaveMultiAnswerRequestDTO request)
        {
            var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
            var currentUser = ObjectMapper.GetCurrentCandidate(claims);

            try
            {
                await InternalSaveAnswer(request, currentUser);
                return Ok(ApiResponseFactory.Success(null, "Lưu câu trả lời thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        private async Task InternalSaveAnswer(SaveMultiAnswerRequestDTO request, HRRecruitmentCandidate currentUser)
        {
            // 1. Lấy tất cả bản ghi cũ của câu hỏi này trong bài thi hiện tại (để xử lý ghi đè)
            var existingDetails = _hrRecruitmentExamResultDetailRepo.GetAll(x =>
                x.RecruitmentExamResultID == request.RecruitmentExamResultID &&
                x.RecruitmentQuestionID == request.RecruitmentQuestionID &&
                x.IsDeleted == false).ToList();

            int? targetDetailID = null;

            // TRƯỜNG HỢP 1: CÂU HỎI TRẮC NGHIỆM (Có danh sách AnswerIDs)
            if (request.RecruitmentAnswerIDs != null && request.RecruitmentAnswerIDs.Count > 0)
            {
                // Xóa tất cả đáp án cũ
                foreach (var item in existingDetails)
                {
                    item.IsDeleted = true;
                    item.UpdatedBy = currentUser.UserName;
                    item.UpdatedDate = DateTime.Now;
                    await _hrRecruitmentExamResultDetailRepo.UpdateAsync(item);
                }

                // Thêm mới các đáp án ứng viên vừa chọn
                foreach (var answerId in request.RecruitmentAnswerIDs)
                {
                    var newDetail = new HRRecruitmentExamResultDetail
                    {
                        RecruitmentExamResultID = request.RecruitmentExamResultID,
                        RecruitmentQuestionID = request.RecruitmentQuestionID,
                        RecruitmentAnswerID = answerId,
                        IsGraded = true,
                        CreatedBy = currentUser.UserName,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    await _hrRecruitmentExamResultDetailRepo.CreateAsync(newDetail);
                    targetDetailID = newDetail.ID; // Lưu ID của bản ghi detail cuối cùng (đối với trắc nghiệm thường là 1 đáp án, nếu nhiều đáp án thì lấy cái cuối)
                }
            }
            // TRƯỜNG HỢP 2: CÂU HỎI TỰ LUẬN (Có nội dung AnswerText HOẶC có đính kèm)
            else if (!string.IsNullOrEmpty(request.AnswerText) || request.litsAnswerImage != null && request.litsAnswerImage.Count > 0)
            {
                var essayDetail = existingDetails.FirstOrDefault();
                if (essayDetail != null)
                {
                    // Cập nhật nội dung mới
                    essayDetail.AnswerText = request.AnswerText;
                    essayDetail.UpdatedDate = DateTime.Now;
                    essayDetail.UpdatedBy = currentUser.UserName;
                    essayDetail.IsGraded = false;
                    await _hrRecruitmentExamResultDetailRepo.UpdateAsync(essayDetail);
                    targetDetailID = essayDetail.ID;
                }
                else
                {
                    // Tạo mới bản ghi tự luận
                    var newEssay = new HRRecruitmentExamResultDetail
                    {
                        RecruitmentExamResultID = request.RecruitmentExamResultID,
                        RecruitmentQuestionID = request.RecruitmentQuestionID,
                        AnswerText = request.AnswerText,
                        IsGraded = false,
                        CreatedDate = DateTime.Now,
                        CreatedBy = currentUser.UserName,
                        IsDeleted = false
                    };
                    await _hrRecruitmentExamResultDetailRepo.CreateAsync(newEssay);
                    targetDetailID = newEssay.ID;
                }
            }
            // TRƯỜNG HỢP 3: ỨNG VIÊN BỎ CHỌN/XÓA HẾT ĐÁP ÁN
            else
            {
                foreach (var item in existingDetails)
                {
                    item.IsDeleted = true;
                    item.UpdatedDate = DateTime.Now;
                    item.UpdatedBy = currentUser.UserName;
                    await _hrRecruitmentExamResultDetailRepo.UpdateAsync(item);
                }
            }

            //LƯU ĐÍNH KÈM(Nếu có)
            if (request.litsAnswerImage != null && request.litsAnswerImage.Count > 0 && targetDetailID.HasValue)
            {
                foreach (var img in request.litsAnswerImage)
                {
                    if (img.ID > 0)
                    {
                        img.RecruitmentExamResultDetailID = targetDetailID;
                        img.UpdatedDate = DateTime.Now;
                        img.UpdatedBy = currentUser.UserName;
                        await _hrRecruitmentExamResultImageRepo.UpdateAsync(img);
                    }
                    else
                    {
                        img.RecruitmentExamResultDetailID = targetDetailID;
                        img.CreatedDate = DateTime.Now;
                        img.CreatedBy = currentUser.UserName;
                        img.IsDeleted = false;
                        await _hrRecruitmentExamResultImageRepo.CreateAsync(img);
                    }
                }
            }
        }
        // nộp bài và chấm điểm
        [HttpPost("submit-exam-result")]
        public async Task<IActionResult> SubmitExamResult([FromBody] SubmitExamRequestDTO request)
        {
            var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
            var currentUser = ObjectMapper.GetCurrentCandidate(claims);

            try
            {
                // 1. Lấy kết quả tổng quan
                var result = _hrRecruitmentExamResultRepo.GetByID(request.ExamResultID);
                if (result == null || result.EmployeeID != currentUser.ID)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy kết quả bài thi hợp lệ."));
                }

                if (result.StatusResult == 1 || result.StatusResult == 2)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Bài thi này đã được nộp hoặc đã có kết quả."));
                }

                // 2. Lưu toàn bộ danh sách câu trả lời nếu được gửi kèm từ Frontend
                if (request.Answers != null && request.Answers.Count > 0)
                {
                    foreach (var ans in request.Answers)
                    {
                        // Gán lại ID kết quả bài thi từ request cha để đảm bảo tính nhất quán
                        ans.RecruitmentExamResultID = request.ExamResultID;
                        await InternalSaveAnswer(ans, currentUser);
                    }
                }

                // 3. Lấy danh sách chi tiết câu trả lời của ứng viên (sau khi đã cập nhật ở bước 2)
                var candidateDetails = _hrRecruitmentExamResultDetailRepo.GetAll(x =>
                    x.RecruitmentExamResultID == request.ExamResultID && x.IsDeleted == false).ToList();

                // 4. Lấy danh sách câu hỏi của bài thi
                var questions = _hrRecruitmentQuestionRepo.GetAll(x =>
                    x.RecruitmentExamID == result.RecruitmentExamID && x.IsDeleted == false).ToList();

                int totalCorrect = 0;
                int totalIncorrect = 0;
                decimal currentScore = 0;
                decimal maxPossibleScore = questions.Sum(q => q.Point ?? 0);
                bool hasManualGrading = false; 

                foreach (var q in questions)
                {
                    // 1. Chấm điểm Trắc nghiệm (Single/Multi choice)
                    if (q.QuestionType == 1 || q.QuestionType == 3)
                    {
                        var candidateAnswers = candidateDetails
                            .Where(x => x.RecruitmentQuestionID == q.ID)
                            .Select(x => x.RecruitmentAnswerID ?? 0)
                            .Where(id => id > 0)
                            .ToList();

                        var correctAnswers = _hrRecruitmentRightAnswearsRepo
                            .GetAll(x => x.RecruitmentQuestionID == q.ID && x.IsDeleted == false)
                            .Select(x => x.RecruitmentAnswerID ?? 0)
                            .ToList();

                        bool isCorrect = false;
                        if (correctAnswers.Count > 0)
                        {
                            isCorrect = candidateAnswers.Count == correctAnswers.Count && !candidateAnswers.Except(correctAnswers).Any();
                        }

                        if (isCorrect)
                        {
                            totalCorrect++;
                            currentScore += q.Point ?? 0;
                        }
                        else totalIncorrect++;

                        var detailsToUpdate = candidateDetails.Where(x => x.RecruitmentQuestionID == q.ID).ToList();
                        foreach (var detail in detailsToUpdate)
                        {
                            detail.IsGraded = true;
                            detail.Score = isCorrect ? q.Point ?? 0 : 0;
                            await _hrRecruitmentExamResultDetailRepo.UpdateAsync(detail);
                        }
                    }
                    // 2. Chấm điểm Tự luận Số
                    else if (q.IsAnswerNumberValue == true && !string.IsNullOrEmpty(q.EssayGuidance))
                    {
                        var answerDetail = candidateDetails.FirstOrDefault(x => x.RecruitmentQuestionID == q.ID);
                        bool isCorrect = false;

                        if (answerDetail != null && !string.IsNullOrEmpty(answerDetail.AnswerText))
                        {
                            if (double.TryParse(answerDetail.AnswerText, out double candidateVal) && 
                                double.TryParse(q.EssayGuidance, out double correctVal))
                            {
                                isCorrect = Math.Abs(candidateVal - correctVal) < 0.0001; 
                            }
                        }

                        if (isCorrect)
                        {
                            totalCorrect++;
                            currentScore += q.Point ?? 0;
                        }
                        else totalIncorrect++;

                        if (answerDetail != null)
                        {
                            answerDetail.IsGraded = true;
                            answerDetail.Score = isCorrect ? q.Point ?? 0 : 0;
                            await _hrRecruitmentExamResultDetailRepo.UpdateAsync(answerDetail);
                        }
                    }
                    // 3. Câu hỏi tự luận thường (Cần HR chấm điểm)
                    else 
                    {
                        hasManualGrading = true;
                    }
                }

                // 5. Cập nhật kết quả tổng quan
                result.TotalCorrect = totalCorrect;
                result.TotalIncorrect = totalIncorrect;
                result.TotalScore = currentScore;
                result.MaxPossibleScore = maxPossibleScore;
                result.PercentageCorrect = maxPossibleScore > 0 ? currentScore / maxPossibleScore * 100 : 0;
                result.StatusResult = hasManualGrading ? 1 : 2; 

                result.UpdatedBy = currentUser.UserName;
                result.UpdatedDate = DateTime.Now;

                await _hrRecruitmentExamResultRepo.UpdateAsync(result);

                return Ok(ApiResponseFactory.Success(result, result.StatusResult == 2 ? "Bài thi đã được chấm điểm tự động." : "Nộp bài thành công, vui lòng chờ HR chấm điểm."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
    }
}
