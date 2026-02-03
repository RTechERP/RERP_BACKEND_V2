using DocumentFormat.OpenXml.Office.SpreadSheetML.Y2023.MsForms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using System.Collections.Immutable;

namespace RERPAPI.Controllers.KHOAHOC
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursePracticeController : ControllerBase
    {
        private readonly CourseRepo _courseRepo;
        private readonly CourseExamRepo _courseExamRepo;
        private readonly CourseLessonHistoryRepo _courseLessonHistoryRepo;
        private readonly CourseExamResultRepo _courseExamResultRepo;
        private readonly CourseExamResultDetailRepo _courseExamResultDetailRepo;
        private readonly CourseQuestionRepo _courseQuestionRepo;
        private readonly CourseRightAnswerRepo _courseRightAnswerRepo;
        private readonly CourseExamEvaluateRepo _courseExamEvaluateRepo;
        private readonly CourseCatalogRepo _courseCatalogRepo;
        private readonly DepartmentRepo _departmentRepo;
        private readonly CourseLessonRepo _courseLessonRepo;
        public CoursePracticeController(CourseExamRepo courseExamRepo,
            CourseLessonHistoryRepo courseLessonHistoryRepo,
            CourseExamResultRepo courseExamResultRepo,
            CourseExamResultDetailRepo courseExamResultDetailRepo,
            CourseQuestionRepo courseQuestionRepo,
            CourseRightAnswerRepo courseRightAnswerRepo,
            CourseExamEvaluateRepo courseExamEvaluateRepo,
            CourseRepo courseRepo,
            CourseCatalogRepo courseCatalogRepo,
            DepartmentRepo departmentRepo,
            CourseLessonRepo courseLessonRepo
            )
        {
            _courseExamRepo = courseExamRepo;
            _courseLessonHistoryRepo = courseLessonHistoryRepo;
            _courseExamResultRepo = courseExamResultRepo;
            _courseExamResultDetailRepo = courseExamResultDetailRepo;
            _courseQuestionRepo = courseQuestionRepo;
            _courseRightAnswerRepo = courseRightAnswerRepo;
            _courseExamEvaluateRepo = courseExamEvaluateRepo;
            _courseRepo = courseRepo;
            _courseCatalogRepo = courseCatalogRepo;
            _departmentRepo = departmentRepo;
            _courseLessonRepo = courseLessonRepo;
        }

        [HttpGet("get-all-course-exam")]
        public IActionResult GetAllCourseExam()
        {
            try
            {
                var data = _courseExamRepo.GetAll();
                return Ok(ApiResponseFactory.Success(data, ""));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // API để lấy danh sách bài học cho 

        [HttpGet("get-course-leson-by-course-id")]
        public IActionResult GetCourseLessonByCourseID(int CourseID)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var data = SQLHelper<object>.ProcedureToList("spGetCourseLessonByCourseID",
                                              new string[] { "@CourseID", "@EmployeeID" },
                                              new object[] { CourseID, currentUser.ID });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-lesson-history-by-lesson-id")]
        public async Task<IActionResult> GetLessonHistoryByLessonIdAsync(int lessonId)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var data = _courseLessonHistoryRepo.GetAll(c => c.EmployeeId == currentUser.ID && c.LessonId == lessonId).FirstOrDefault();
                if (data == null || data.ID <= 0)
                {
                    var lesson = _courseLessonRepo.GetByID(lessonId);


                    var lessonHistory = new CourseLessonHistory()
                    {
                        Status = 0,
                        LessonId = lessonId,
                        EmployeeId = currentUser.ID,
                        ViewDate = DateTime.Now,
                        //VideoDuration = lesson.VideoDuration,
                        WatchedPercent = 0,
                        LastWatchedSecond = 0,
                        MaxWatchedSecond = 0
                    };

                    await _courseLessonHistoryRepo.CreateAsync(lessonHistory);
                    data = lessonHistory;
                }
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-lesson-by-lesson-id")]
        public async Task<IActionResult> GetLessonByLessonIdAsync(int lessonId)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var data = _courseLessonRepo.GetByID(lessonId);
                if (data.ID<=0)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Không tìm thấy lesson"));
                }
                
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }




        [HttpPost("save-lesson-history-progress")]
        public async Task<IActionResult> SaveLessonHistoryProgress([FromBody] LessonHistoryProgressParam param)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var exitLessonHistory = _courseLessonHistoryRepo.GetAll(x => x.LessonId == param.LessonID && x.EmployeeId == currentUser.ID).FirstOrDefault();
                exitLessonHistory.WatchedPercent = Math.Round((decimal)param.MaxWatchedSecond * 100 / param.VideoDuration, 4);// tiến độ % video   // chia ép kiểu về decimal
                exitLessonHistory.LastWatchedSecond = param.LastWatchedSecond; // thời gian cuối cùng xem
                exitLessonHistory.MaxWatchedSecond = param.MaxWatchedSecond; // thời gian max có thể xem
                exitLessonHistory.VideoDuration = param.VideoDuration;
                await _courseLessonHistoryRepo.UpdateAsync(exitLessonHistory);
              
                return Ok(ApiResponseFactory.Success(exitLessonHistory, "Đã cập nhật thành công"));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Api cập nhật trạng thái hoàn thành cho lesson

        [HttpPost("change-status-lesson-history")]
        public async Task<IActionResult> ChangeStatusLessonHistory(int lessonId, bool completed)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var exitLessonHistory = _courseLessonHistoryRepo.GetAll(x => x.LessonId == lessonId && x.EmployeeId == currentUser.ID).FirstOrDefault();
                if (exitLessonHistory != null && exitLessonHistory.ID > 0)
                {
                    if (completed)
                    {
                        exitLessonHistory.Status = 1;
                    }
                    else
                    {
                        exitLessonHistory.Status = 0;
                    }
                    await _courseLessonHistoryRepo.UpdateAsync(exitLessonHistory);

                }
                else
                {
                    var lessonHistory = new CourseLessonHistory()
                    {
                        Status = 1,
                        LessonId = lessonId,
                        EmployeeId = currentUser.ID,
                        ViewDate = DateTime.Now,
                    };
                    await _courseLessonHistoryRepo.CreateAsync(lessonHistory);
                }
                return Ok(ApiResponseFactory.Success(exitLessonHistory, "Đã cập nhật trạng thái bài học thành công"));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // API lấy lịch sử các bài thi trắc nghiệm 

        [HttpPost("get-exam-result")]
        public async Task<IActionResult> GetExamResult(int courseID = 0, int lessonID = 0)
        {
            try
            {
                var courseExam = new CourseExam();
                if (lessonID > 0)
                {
                    courseExam = _courseExamRepo.GetAll(x => x.LessonID == lessonID && x.ExamType == 1).FirstOrDefault();

                }
                else
                {
                    courseExam = _courseExamRepo.GetAll(x => x.CourseId == courseID && x.ExamType == 1).FirstOrDefault();
                }

                if (courseExam != null && courseExam.ID > 0)
                {
                    var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                    var currentUser = ObjectMapper.GetCurrentUser(claims);
                    var data = SQLHelper<object>.ProcedureToList("spGetCourseExamResult2",
                                              new string[] { "@CourseExamID", "@EmployeeID", "@OrderNumber" },
                                              new object[] { courseExam.ID, currentUser.ID, 0 });
                    return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), ""));
                }
                return BadRequest(ApiResponseFactory.Fail(null, "Không tồn tại bài kiểm tra trong khóa học!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // API cho phần thi Trắc nghiệm
        [HttpPost("create-exam-result")]
        public async Task<IActionResult> CreateExamResult(int courseExamID)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var courseExamResult = new CourseExamResult();
                courseExamResult.CourseExamId = courseExamID;
                courseExamResult.EmployeeId = currentUser.ID;
                courseExamResult.TotalCorrect = 0;
                courseExamResult.TotalIncorrect = 0;
                courseExamResult.PercentageCorrect = 0;
                courseExamResult.Status = 0;
                await _courseExamResultRepo.CreateAsync(courseExamResult);

                if (courseExamResult.ID > 0)
                {
                    return Ok(ApiResponseFactory.Success(courseExamResult, "Tạo kết quả khóa học thành công!"));
                }
                return BadRequest(ApiResponseFactory.Fail(null, "Tạo mới kết quả khóa học thất bại"));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("list-exam-question")]
        public async Task<IActionResult> ListExamQuestion(int courseId = 0, int courseExamResultID = 0, int examType = 1, int lessonID = 0)
        {
            try
            {
                if (lessonID > 0)
                {


                    var dataLessonExam = SQLHelper<object>.ProcedureToList("spCourseLessonQuestion",
                                                            new string[] { "@LessonID", "@CourseExamResultID", "@ExamType" },
                                                            new object[] { lessonID, courseExamResultID, examType });
                    if (dataLessonExam != null && dataLessonExam.Count > 0)
                    {
                        //return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(dataLessonExam, 0), ""));
                        return Ok(ApiResponseFactory.Success(new
                        {
                            listQuestion = SQLHelper<object>.GetListData(dataLessonExam, 0),
                            listAnswer = SQLHelper<object>.GetListData(dataLessonExam, 1)
                        }, ""));
                    }
                    return BadRequest(ApiResponseFactory.Fail(null, "Lấy danh sách câu hỏi bài học thất bại!"));
                }
                else
                {
                    var dataCourseExam = SQLHelper<object>.ProcedureToList("spCourseQuestion",
                                              new string[] { "@CourseID", "@CourseExamResultID", "@ExamType" },
                                              new object[] { courseId, courseExamResultID, examType });
                    if (dataCourseExam != null && dataCourseExam.Count > 0)
                    {
                        //return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(dataCourseExam, 0), ""));
                        return Ok(ApiResponseFactory.Success(new
                        {
                            listQuestion = SQLHelper<object>.GetListData(dataCourseExam, 0),
                            listAnswer = SQLHelper<object>.GetListData(dataCourseExam, 1)
                        }, ""));
                    }
                    return BadRequest(ApiResponseFactory.Fail(null, "Lấy câu hỏi khóa học thất bại!"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("create-exam-result-detail")]
        public async Task<IActionResult> CreateExamResultDetail([FromBody] List<CourseExamResultDetail> details)
        {
            try
            {
                int questionId = 0;
                List<int> listAnswerId = new List<int>();
                if (details.Count <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "tạo kết quả thất bại!"));
                }
                var existingResultDetails = _courseExamResultDetailRepo.GetAll(p => p.CourseExamResultId == details.First().CourseExamResultId && p.CourseQuestionId == details.First().CourseQuestionId);

                _courseExamResultDetailRepo.DeleteRange(existingResultDetails);
                foreach (CourseExamResultDetail item in details)
                {
                    CourseExamResultDetail detail = item;
                    await _courseExamResultDetailRepo.CreateAsync(detail);
                    if (detail != null && detail.ID > 0)
                    {
                        questionId = (int)detail.CourseQuestionId;
                        listAnswerId.Add((int)detail.CourseAnswerId);
                    }
                }
                return Ok(ApiResponseFactory.Success(new
                {
                    QuestionId = questionId,
                    AnswerIds = listAnswerId
                }, "Thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("submit-exam")]
        public async Task<IActionResult> SubmitExam(int courseExamResultID)
        {
            try
            {
                var examResult = _courseExamResultRepo.GetByID(courseExamResultID);
                var courseExam = _courseExamRepo.GetByID(examResult.CourseExamId ?? 0);

                var courseQuestions = _courseQuestionRepo.GetAll(p => p.CourseExamId == courseExam.ID);

                int numCorrectAnswers = 0;
                int numIncorrectAnswers = 0;

                foreach (var question in courseQuestions)
                {
                    var courseRightAnswers = _courseRightAnswerRepo.GetAll(p => p.CourseQuestionID == question.ID);
                    var examResultDetails = _courseExamResultDetailRepo.GetAll(p => p.CourseExamResultId == courseExamResultID && p.CourseQuestionId == question.ID);

                    // Get the selected answer IDs from examResultDetails
                    var selectedAnswerIDs = examResultDetails.Select(detail => detail.CourseAnswerId).ToList();

                    // Check if the question has multiple correct answers
                    bool hasMultipleCorrectAnswers = courseRightAnswers.Count() > 1;

                    // Check if the selected answer IDs match all the correct answer IDs and there are no additional or missing answers
                    var isCorrect = selectedAnswerIDs.Count() == courseRightAnswers.Count()
                        && selectedAnswerIDs.All(answerId => courseRightAnswers.Any(rightAnswer => rightAnswer.CourseAnswerID == answerId))
                        && courseRightAnswers.All(rightAnswer => selectedAnswerIDs.Contains(rightAnswer.CourseAnswerID));

                    // If the question has multiple correct answers, consider it correct only if all correct answers are selected and there are no additional or missing answers
                    // If the question has only one correct answer, consider it correct if the selected answer is the correct answer
                    if ((hasMultipleCorrectAnswers && isCorrect) || (!hasMultipleCorrectAnswers && selectedAnswerIDs.Count == 1 && isCorrect))
                    {
                        numCorrectAnswers++;
                    }
                    else
                    {
                        numIncorrectAnswers++;
                    }
                }

                examResult.TotalCorrect = numCorrectAnswers;
                examResult.TotalIncorrect = numIncorrectAnswers;
                examResult.PercentageCorrect = numCorrectAnswers != 0 ? ((decimal)numCorrectAnswers / (numCorrectAnswers + numIncorrectAnswers)) * 100 : 0;

                _courseExamResultRepo.Update(examResult);

                return Ok(ApiResponseFactory.Success(new
                {
                    NumCorrectAnswers = numCorrectAnswers,
                    NumIncorrectAnswers = numIncorrectAnswers
                }, "Lấy kết quả thi thành công thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

        [HttpGet("get-question-answer-right")]
        public IActionResult GetQuestionAnswerRight(int courseExamResultId)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                // Gọi stored procedure
                var listQuestionRight = SQLHelper<object>.ProcedureToList(
                    "spGetQuestionAnswerRight",
                    new string[] { "@EmployeeID", "@CourseResultID" },
                    new object[] { currentUser.ID, courseExamResultId }
                );

                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(listQuestionRight, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

        // ---------------------------------------- API Cho phần Thực hành ---------------------------------------- 

        [HttpGet("get-history-result-practice")]
        public IActionResult GetHistoryResultPractice(int courseExamId)
        {

            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var data = SQLHelper<object>.ProcedureToList("spGetResultHistoryPractice",
                                                new string[] { "@EmployeeId", "@CourseExamId" },
                                                new object[] { currentUser.ID, courseExamId });

                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-result-practice")]
        public IActionResult GetResultPractice(int courseResultId)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var courseExam = _courseExamResultRepo.GetByID(courseResultId);
                var data = SQLHelper<object>.ProcedureToList("spGetResultHistoryByPractice",
                    new string[] { "@EmployeeId", "@CourseResultId", "@CourseExamId" },
                    new object[] { currentUser.ID, courseResultId, courseExam.CourseExamId });

                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("create-list-exam-valuate")]
        public async Task<bool> CreateListExamValuate([FromBody] List<CourseExamEvaluate> listData)
        {
            try
            {
                foreach (var item in listData)
                {
                    var newExamValuate = new CourseExamEvaluate();

                    newExamValuate.CourseExamResultID = item.CourseExamResultID;
                    newExamValuate.CourseQuestionID = item.CourseQuestionID;
                    await _courseExamEvaluateRepo.CreateAsync(newExamValuate);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        [HttpPost("confirm-practice")]
        public IActionResult ConfirmPractice([FromBody] CourseExamResult data)
        {
            try
            {
                var examResult = _courseExamResultRepo.GetByID(data.ID);
                examResult.Status = 1; // Đã hoàn thành
                _courseExamResultRepo.Update(examResult);
                return Ok(ApiResponseFactory.Success(null, "Hoàn thành bài học!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-question")]
        public IActionResult GetQuesstion(int courseExamId)
        {
            try
            {
                var data = _courseQuestionRepo.GetAll(x => x.CourseExamId == courseExamId).OrderBy(x => x.STT).ToList();
                return Ok(ApiResponseFactory.Success(data, "Lấy câu hỏi thực hành thành công"));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("question-details")]

        public IActionResult QuestionDetails(int questionId = 0, int courseId = 0)
        {
            try
            {
                var coursePractice = _courseQuestionRepo.GetByID(questionId);
                var course = _courseRepo.GetByID(courseId) ?? new Course();

                if (coursePractice == null || course == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy câu hỏi hoặc khóa học!"));
                }

                return Ok(ApiResponseFactory.Success(new
                {
                    CoursePractice = coursePractice,
                    Course = course,
                }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        //[HttpGet("load-course-category")]
        //public async Task<IActionResult> GetDanhMuc()
        //{
        //    try
        //    {
        //        var listCourseCatalogs = _courseCatalogRepo.GetAll(c => c.DeleteFlag == true).OrderBy(x => x.STT);
        //        var listCourseCatalogIds = listCourseCatalogs.Select(x => x.DepartmentID).Distinct().ToList();
        //        var listDepartments = _departmentRepo.GetAll().Where(x => listCourseCatalogIds.Contains(x.ID));

        //        var data = SQLHelper<object>.ProcedureToList("spGetCourseCatalog",
        //                                      new string[] { },
        //                                      new object[] { });
        //        return Ok(ApiResponseFactory.Success(new
        //        {
        //            Department = listDepartments,
        //            CourseCatalog = SQLHelper<object>.GetListData(data, 0)
        //        }, ""));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}
        ////lấy danh sách khóa học
        [HttpGet("load-data-course")]
        public async Task<IActionResult> LoadDataCourse(int courseCatalogID)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                //var listCourseC

                var data = SQLHelper<object>.ProcedureToList("spGetCourseNew",
                                              new string[] { "@CourseCatalogID", "@EmployeeID", "@Status" },
                                              new object[] { courseCatalogID, currentUser.ID, -1 });
                var listCourseParent = SQLHelper<object>.GetListData(data, 0);


                for (int i = 1; i < listCourseParent.Count; i++)
                {
                    var course = listCourseParent[i - 1];

                    if (course.IsLearnInTurn ?? false)
                    {
                        if ((course.NumberLesson == course.TotalHistoryLession && course.Evaluate == 1) || currentUser.IsLeader > 0 || currentUser.IsAdmin || currentUser.ID == 55)
                        {
                            listCourseParent[i].Status = 1;
                        }
                        else
                        {
                            listCourseParent[i].Status = 0;
                        }
                    }
                }
                return Ok(ApiResponseFactory.Success(listCourseParent, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
