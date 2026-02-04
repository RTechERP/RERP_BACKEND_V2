using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NPOI.SS.Formula.Functions;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using SkiaSharp;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RERPAPI.Controllers.KHOAHOC
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseExamController : ControllerBase
    {
        private readonly CourseExamRepo _courseExamRepo;
        private readonly CourseAnswerRepo _courseAnswerRepo;
        private readonly CourseRightAnswerRepo _courseRightAnswerRepo;
        private readonly CourseQuestionRepo _courseQuestionRepo;
        private readonly CourseLessonRepo _courseeLessonRepo;
        public CourseExamController(CourseExamRepo courseExamRepo,
            CourseAnswerRepo courseAnswerRepo,
            CourseRightAnswerRepo courseRightAnswerRepo,
            CourseQuestionRepo courseQuestionRepo,
            CourseLessonRepo courseLessonRepo
            )
        {
            _courseExamRepo = courseExamRepo;
            _courseAnswerRepo = courseAnswerRepo;
            _courseRightAnswerRepo = courseRightAnswerRepo;
            _courseQuestionRepo = courseQuestionRepo;
            _courseeLessonRepo = courseLessonRepo;
        }

        // load khóa học
        [HttpGet("get-course-data")]
        public IActionResult GetCourseData()
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetDataCourse",
                                                new string[] { },
                                                new object[] { });

                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // load đề thi
        [HttpGet("get-course-exam")]
        public IActionResult GetCourseExam(int? courseID)
        {
            try
            {
                courseID = courseID ?? 0;
                var data = SQLHelper<object>.ProcedureToList("spGetCourseExam",
                                                new string[] { "@CourseID" },
                                                new object[] { courseID });

                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // load câu hỏi
        [HttpGet("get-course-question")]
        public IActionResult GetCourseQuestion(int? examID)
        {
            try
            {
                examID = examID ?? 0;
                var data = SQLHelper<object>.ProcedureToList("spGetCourseQuestion",
                                                new string[] { "@ExamID" },
                                                new object[] { examID });

                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // load đáp án trắc nhiệu
        [HttpGet("get-right-answer")]
        public IActionResult GetRightAnswer(int? questionID)
        {
            try
            {
                questionID = questionID ?? 0;
                var data = SQLHelper<object>.ProcedureToList("spGetCourseAnswersByCourseQuestionID",
                                                new string[] { "@CourseQuestionID" },
                                                new object[] { questionID });

                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // load lấy dữ liệu bài kiểm tra của bài học
        [HttpGet("get-lesson-exam")]
        public IActionResult GetLessonExam(int? courseID)
        {
            try
            {
                courseID = courseID ?? 0;
                var data = SQLHelper<object>.ProcedureToList("spGetLessonExamByLessonID",
                                                new string[] { "@CourseID" },
                                                new object[] { courseID });

                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //
        [HttpGet("get-course-new")]
        public IActionResult GetCourseNew()
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                var data = SQLHelper<object>.ProcedureToList("spGetCourseNew",
                                                new string[] { "@DepartmentID", "@Status" },
                                                new object[] { currentUser.DepartmentID, -1,
                });

                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // load lấy dữ liệu bài kiểm tra của bài học
        [HttpGet("get-course-lesson")]
        public IActionResult GetCaurseLesson(int? courseID)
        {
            try
            {
                courseID = courseID ?? 0;
                var data = _courseeLessonRepo.GetAll(x => x.CourseID == courseID).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Thêm sửa xóa đề thi
        [HttpPost("save-data-exam")]
        public async Task<IActionResult> SaveDataExam([FromBody] CourseExam model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu gửi lên không hợp lệ!"));
                }
                if (model.CourseId <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn khóa học!"));
                }
                if (model.ExamType <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn Loại!"));
                }
                if (model.ExamType < 2 && model.TestTime <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Thời gian!"));
                }
                if (string.IsNullOrEmpty(model.NameExam))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Tên đề thi!"));
                }
                if (model.Goal <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Số điểm cần đạt!"));
                }
                if (string.IsNullOrEmpty(model.CodeExam))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Mã đề thi!"));
                }
                var validateData = _courseExamRepo.GetAll(x => x.CourseId == model.CourseId && x.ExamType == model.ExamType && x.ID != model.ID).FirstOrDefault();
                if (validateData != null && validateData.ID > 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Bạn đã tạo đề thi cho khóa học này!"));
                }
                var validateData2 = _courseExamRepo.GetAll(x => x.CourseId == model.CourseId && x.CodeExam == model.CodeExam && x.ID != model.ID).FirstOrDefault();
                if (validateData2 != null && validateData2.ID > 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Mã đề thi [{model.CodeExam}] đã tồn tại!"));
                }

                if (model.ID <= 0)
                {
                    model.ID = await _courseExamRepo.CreateAsync(model);
                }
                else
                {
                    await _courseExamRepo.UpdateAsync(model);
                }

                return Ok(ApiResponseFactory.Success(model, "Lưu đề thi thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Thêm sửa xóa đề thi - bài học
        [HttpPost("save-data-lesson")]
        public async Task<IActionResult> SaveDataLesson([FromBody] CourseExam model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu gửi lên không hợp lệ."));
                }
                if (model.LessonID <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Bài học!"));
                }
                if (model.ExamType <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn Loại!"));
                }
                if (model.ExamType < 2 && model.TestTime <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Thời gian!"));
                }
                if (string.IsNullOrEmpty(model.NameExam))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Tên đề thi!"));
                }
                if (model.Goal <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Số điểm cần đạt!"));
                }
                if (string.IsNullOrEmpty(model.CodeExam))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Mã đề thi!"));
                }
                var validateData = _courseExamRepo.GetAll(x => x.LessonID == model.LessonID && x.ExamType == model.ExamType && x.ID != model.ID).FirstOrDefault();
                if (validateData != null && validateData.ID > 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Bạn đã tạo đề thi cho bài học này!"));
                }
                var validateData2 = _courseExamRepo.GetAll(x => x.LessonID == model.LessonID && (x.CodeExam.ToUpper().Trim() == model.CodeExam.ToUpper().Trim()) && x.ID != model.ID).FirstOrDefault();
                if (validateData2 != null && validateData2.ID > 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Mã đề thi [{model.CodeExam}] đã tồn tại!"));
                }

                if (model.ID <= 0)
                {
                    await _courseExamRepo.CreateAsync(model);
                }
                else
                {
                    await _courseExamRepo.UpdateAsync(model);
                }

                return Ok(ApiResponseFactory.Success(model, "Lưu đề thi thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // xóa đề thi
        [HttpPost("delete-course-exam")]
        public async Task<IActionResult> DeleteCourseExam(int id)
        {
            try
            {
                await _courseExamRepo.DeleteAsync(id);

                return Ok(ApiResponseFactory.Success(null, "Xóa đề thi thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi xóa đề thi: {ex.Message}"));
            }
        }

        // Thêm sửa đề thi - bài học
        [HttpPost("save-course-exam-lesson")]
        public async Task<IActionResult> SaveCourseExamLesson([FromBody] CourseExam model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu gửi lên không hợp lệ."));
                }
                if (model.LessonID <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Bài học!"));
                }
                if (model.ExamType <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn Loại!"));
                }
                if (model.ExamType < 2 && model.TestTime <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Thời gian!"));
                }
                if (string.IsNullOrEmpty(model.NameExam))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Tên đề thi!"));
                }
                if (model.Goal <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Số điểm cần đạt!"));
                }
                if (string.IsNullOrEmpty(model.CodeExam))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Mã đề thi!"));
                }
                var validateData = _courseExamRepo.GetAll(x => x.LessonID == model.LessonID && x.ExamType == model.ExamType && x.ID != model.ID).FirstOrDefault();
                if (validateData != null && validateData.ID > 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Bạn đã tạo đề thi cho bài học này!"));
                }
                var validateData2 = _courseExamRepo.GetAll(x => x.LessonID == model.LessonID && (x.CodeExam.ToUpper().Trim() == model.CodeExam.ToUpper().Trim()) && x.ID != model.ID).FirstOrDefault();
                if (validateData2 != null && validateData2.ID > 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Mã đề thi [{model.CodeExam}] đã tồn tại!"));
                }

                if (model.ID <= 0)
                {
                    await _courseExamRepo.CreateAsync(model);
                }
                else
                {
                    await _courseExamRepo.UpdateAsync(model);
                }

                return Ok(ApiResponseFactory.Success(model, "Lưu đề thi thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // lấy số thứ tự của câu hỏi
        [HttpGet("get-course-question-no")]
        public IActionResult GetCourseQuestionNo(int? examID)
        {
            try
            {
                int maxSTT = _courseQuestionRepo.GetAll(x => x.CourseExamId == examID).Select(x => (int?)x.STT).Max() ?? 0;
                int nextSTT = maxSTT + 1;
                return Ok(ApiResponseFactory.Success(nextSTT, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // lấy danh sách đáp án
        [HttpGet("get-course-answers")]
        public IActionResult GetCourseAnswers(int? questionID)
        {
            try
            {
                questionID = questionID ?? 0;
                var data = SQLHelper<object>.ProcedureToList("spGetCourseAnswers",
                                                new string[] { "@QuestionID" },
                                                new object[] { questionID });

                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Controller
        [HttpPost("save-course-question")]
        public async Task<IActionResult> SaveCourseQuestion([FromBody] SaveCourseQuestionParam data)
        {
            try
            {
                if (data == null || data.Question == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));

                if (string.IsNullOrWhiteSpace(data.Question.QuestionText))
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Nội dung câu hỏi !"));

                if (data.ExamType == 1)
                {
                    if (data.Answers.Count <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Nội dung đáp án!"));
                    }
                    else if (data.Answers.Count > 4)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Số lượng đáp án không được lớn hơn 4!"));
                    }
                    else
                    {
                        int countAnswer = 1;
                        bool checkRightAnswer = false;
                        foreach (var item in data.Answers)
                        {
                            string code = countAnswer == 1 ? "A" : countAnswer == 2 ? "B" : countAnswer == 3 ? "C" : "D";
                            if (string.IsNullOrEmpty(item.AnswerText))
                                return BadRequest(ApiResponseFactory.Fail(null, $"Vui lòng nhập nội dung đáp án [{code}]!"));

                            if (item.IsRightAnswer)
                                checkRightAnswer = item.IsRightAnswer;
                            countAnswer++;
                        }
                        if (!checkRightAnswer)
                            return BadRequest(ApiResponseFactory.Fail(null, $"Vui lòng chọn ít nhất một đáp án đúng!"));
                    }
                }

                // insert / update question
                if (data.Question.ID <= 0)
                    await _courseQuestionRepo.CreateAsync(data.Question);
                else
                    await _courseQuestionRepo.UpdateAsync(data.Question);

                if (data.Question.ID <= 0) return BadRequest(ApiResponseFactory.Fail(null, "Đã có lỗi sảy ra khi tạo câu hỏi!"));

                // delete right answers
                await _courseRightAnswerRepo.DeleteByAttributeAsync("CourseQuestionID", data.Question.ID);

                // delete answers
                if (data.DeleteAnswerIds != null && data.DeleteAnswerIds.Count > 0)
                {
                    foreach (var id in data.DeleteAnswerIds)
                        await _courseAnswerRepo.DeleteAsync(id);
                }

                // save answers
                int index = 1;
                foreach (var ans in data.Answers)
                {
                    var entity = new CourseAnswer
                    {
                        ID = ans.ID,
                        CourseQuestionId = data.Question.ID,
                        AnswerText = ans.AnswerText,
                        AnswerNumber = index
                    };
                    if (entity.ID <= 0)
                        await _courseAnswerRepo.CreateAsync(entity);
                    else
                        await _courseAnswerRepo.UpdateAsync(entity);

                    string code = index == 1 ? "A" : index == 2 ? "B" : index == 3 ? "C" : "D";
                    if (entity.ID <= 0) return BadRequest(ApiResponseFactory.Fail(null, $"Đã có lỗi sảy ra khi tạo đáp án [{code}]!"));

                    if (ans.IsRightAnswer)
                    {
                        await _courseRightAnswerRepo.CreateAsync(new CourseRightAnswer
                        {
                            CourseQuestionID = data.Question.ID,
                            CourseAnswerID = entity.ID
                        });
                    }
                    index++;
                }

                return Ok(ApiResponseFactory.Success(data.Question, "Lưu câu hỏi thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // xóa list câu hỏi
        [HttpPost("delete-course-question")]
        public async Task<IActionResult> DeleteCourseQuestion(string ids)
        {
            try
            {
                if (string.IsNullOrEmpty(ids))
                    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách ID rỗng"));

                List<int> idList = ids.Split(',')
                      .Select(id => int.Parse(id.Trim()))
                      .ToList();
                foreach (var id in idList)
                {
                    await _courseAnswerRepo.DeleteByAttributeAsync("CourseQuestionID", id);
                    await _courseRightAnswerRepo.DeleteByAttributeAsync("CourseQuestionID", id);
                    await _courseQuestionRepo.DeleteAsync(id);
                }

                return Ok(ApiResponseFactory.Success(null, "Xóa danh sách câu hỏi thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi xóa danh sách câu hỏi: {ex.Message}"));
            }
        }

    }
}
