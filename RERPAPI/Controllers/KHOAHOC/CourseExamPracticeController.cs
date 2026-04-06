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
    public class CourseExamPracticeController : ControllerBase
    {
        private readonly CourseExamRepo _courseExamRepo;
        private readonly CourseAnswerRepo _courseAnswerRepo;
        private readonly CourseRightAnswerRepo _courseRightAnswerRepo;
        private readonly CourseQuestionRepo _courseQuestionRepo;
        private readonly CourseLessonRepo _courseeLessonRepo;
        //
        private readonly CourseExamPracticeRepo _courseExamPracticeRepo;
        private readonly CourseExamResultRepo _courseExamResultRepo;
        private readonly CourseExamEvaluateRepo _courseExamEvaluateRepo;
        public CourseExamPracticeController(CourseExamRepo courseExamRepo,
            CourseAnswerRepo courseAnswerRepo,
            CourseRightAnswerRepo courseRightAnswerRepo,
            CourseQuestionRepo courseQuestionRepo,
            CourseLessonRepo courseLessonRepo,
            CourseExamPracticeRepo courseExamPracticeRepo,
            CourseExamResultRepo courseExamResultRepo,
            CourseExamEvaluateRepo courseExamEvaluateRepo
            )
        {
            _courseExamRepo = courseExamRepo;
            _courseAnswerRepo = courseAnswerRepo;
            _courseRightAnswerRepo = courseRightAnswerRepo;
            _courseQuestionRepo = courseQuestionRepo;
            _courseeLessonRepo = courseLessonRepo;
            _courseExamPracticeRepo = courseExamPracticeRepo;
            _courseExamResultRepo = courseExamResultRepo;
            _courseExamEvaluateRepo = courseExamEvaluateRepo;
        }


        // load khóa học
        [HttpGet("get-course-data")]
        public IActionResult GetCourseData(int? employeeID)
        {
            try
            {
                employeeID = employeeID ?? 0;
                var data = SQLHelper<object>.ProcedureToList("spGetDataCourse",
                                                new string[] { "@EmployeeID" },
                                                new object[] { employeeID });

                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // load nhân viên
        [HttpGet("get-employee-data")]
        public IActionResult GetEmployeeData()
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetEmployee",
                                                new string[] { "@Status" },
                                                new object[] { 0 });

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

        // Hàm này kiểm tra trong cơ sở dữ liệu xem khóa học có các loại bài thi 1, 2, 3 hay không, rồi bật/tắt hiển thị từng tab tương ứng trên giao diện WinForms.
        [HttpGet("get-check-course-exam")]
        public IActionResult GetCheckCourseExam(int? courseID)
        {
            try
            {
                courseID ??= 0;

                var data = SQLHelper<object>.ProcedureToList(
                    "spGetCourseExam",
                    new string[] { "@CourseID" },
                    new object[] { courseID }
                );

                var list = SQLHelper<object>.GetListData(data, 0);

                var result = new
                {
                    HasExamType1 = list.Any(x => Convert.ToInt32(x.ExamType) == 1),
                    HasExamType2 = list.Any(x => Convert.ToInt32(x.ExamType) == 2),
                    HasExamType3 = list.Any(x => Convert.ToInt32(x.ExamType) == 3)
                };

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-course-exam-practice")]
        public IActionResult GetCourseExamPractice(int? courseId, int? employeeId)
        {
            courseId = courseId ?? 0;
            employeeId = employeeId ?? 0;
            var exams = _courseExamRepo.GetAll(x => x.CourseId == courseId).ToList();

            object? TracNhiem = null;
            object? ThucHanh = null;
            object? BaiTap = null;

            var examTN = exams.FirstOrDefault(x => x.ExamType == 1);
            if (examTN != null && examTN?.ID > 0)
            {
                var data1 = SQLHelper<object>.ProcedureToList(
                    "spGetCourseExamResult",
                    new[] { "@CourseExamID", "@EmployeeID", "@OrderNumber" },
                    new object[] { examTN.ID, employeeId, 1 });
                TracNhiem = SQLHelper<object>.GetListData(data1, 0);
            }

            var examTH = exams.FirstOrDefault(x => x.ExamType == 2);
            if (examTH != null && examTH?.ID > 0)
            {
                var data2 = SQLHelper<object>.ProcedureToList(
                    "spGetCourseExamPracticeResult",
                    new[] { "@CourseExamID" },
                    new object[] { examTH.ID });
                ThucHanh = SQLHelper<object>.GetListData(data2, 0);
            }

            var examBT = exams.FirstOrDefault(x => x.ExamType == 3);
            if (examBT != null && examBT?.ID > 0)
            {
                var data3 = SQLHelper<object>.ProcedureToList(
                    "spGetCourseExamPracticeResult",
                    new[] { "@CourseExamID" },
                    new object[] { examBT.ID });
                BaiTap = SQLHelper<object>.GetListData(data3, 0);
            }

            return Ok(ApiResponseFactory.Success(new { TracNhiem, ThucHanh, BaiTap }, ""));
        }

        // xóa kết quả thi của nhân viên
        [HttpPost("delete-course-exam-practice")]
        public async Task<IActionResult> DeleteCourseExamPractice(int id)
        {
            try
            {
                await _courseExamResultRepo.DeleteAsync(id);

                return Ok(ApiResponseFactory.Success(null, "Xóa kết quả thi của nhân viên này thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi xóa kết quả thi: {ex.Message}"));
            }
        }

        //Đánh giá đạt / không đạt
        [HttpPost("course-exam-results-evaluate")]
        public IActionResult CourseRxamResultsEvaluate(string lstId, bool evaluate)
        {

            SQLHelper<object>.ExcuteProcedure(
                "spUpdateEvaluate",
                new[] { "@LstID", "@Evaluate" },
                new object[] { lstId, evaluate });

            string status = evaluate ? "đạt" : "không đạt";
            return Ok(ApiResponseFactory.Success(true, $"Đánh giá {status} thành công!"));
        }

        // load bài học
        [HttpGet("get-course-lessons")]
        public IActionResult GetCourselessons(int? courseID)
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

        /// load kết quả thi bài học
        [HttpGet("get-lesson-exam-result")]
        public IActionResult GetLessonExamResult(int? lessonId, int? employeeId)
        {
            try
            {
                lessonId = lessonId ?? 0;
                employeeId = employeeId ?? 0;
                var exams = _courseExamRepo.GetAll(x => x.LessonID == lessonId).ToList();

                object? TracNhiem = null, ThucHanh = null, BaiTap = null;

                var examTN = exams.FirstOrDefault(x => x.ExamType == 1);
                if (examTN != null)
                {
                    var data = SQLHelper<object>.ProcedureToList(
                        "spGetCourseExamResult",
                        new[] { "@CourseExamID", "@EmployeeID", "@OrderNumber" },
                        new object[] { examTN.ID, employeeId, 1 });
                    TracNhiem = SQLHelper<object>.GetListData(data, 0);
                }

                var examTH = exams.FirstOrDefault(x => x.ExamType == 2);
                if (examTH != null)
                {
                    var data = SQLHelper<object>.ProcedureToList(
                        "spGetCourseExamPracticeResult",
                        new[] { "@CourseExamID" },
                        new object[] { examTH.ID });
                    ThucHanh = SQLHelper<object>.GetListData(data, 0);
                }

                var examBT = exams.FirstOrDefault(x => x.ExamType == 3);
                if (examBT != null)
                {
                    var data = SQLHelper<object>.ProcedureToList(
                        "spGetCourseExamPracticeResult",
                        new[] { "@CourseExamID" },
                        new object[] { examBT.ID });
                    BaiTap = SQLHelper<object>.GetListData(data, 0);
                }

                return Ok(ApiResponseFactory.Success(new { TracNhiem, ThucHanh, BaiTap }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // 
        [HttpGet("get-check-lesson-exam")]
        public IActionResult GetCheckLessonExam(int? lessonID)
        {
            try
            {
                lessonID ??= 0;
                var data = _courseExamRepo.GetAll(x => x.LessonID == lessonID).ToList();
                var result = new
                {
                    HasExamType1 = data.Any(x => TextUtils.ToInt32(x.ExamType) == 1),
                    HasExamType2 = data.Any(x => TextUtils.ToInt32(x.ExamType) == 2),
                    HasExamType3 = data.Any(x => TextUtils.ToInt32(x.ExamType) == 3)
                };
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        // curd kết quả thị khóa học: TH, BT

        [HttpGet("get-course-new")]
        public IActionResult GetCourseNew()
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                int departmentID = currentUser.EmployeeID == 54 ? 2 : currentUser.DepartmentID;

                var data = SQLHelper<object>.ProcedureToList("spGetCourseNew",
                                                new string[] { "@UserID", "@Status", "@DepartmentID" },
                                                new object[] { currentUser.ID, -1, departmentID });

                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        // Thêm sửa kết quả thi: TH, BT
        [HttpPost("save-course-exam-practice")]
        public async Task<IActionResult> SaveCourseExamPractice([FromBody] SaveCourseExamResultParam paramData)
        {
            try
            {
                if (paramData == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu gửi lên không hợp lệ."));
                }
                if (paramData.CourseId <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Đề thi!"));
                }
                if (paramData.CourseExamResult.EmployeeId <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Nhân viên!"));
                }
                string examTypeName = paramData.ExamType == 1 ? "Trắc nghiệm" :
                                          paramData.ExamType == 2 ? "Thực hành" :
                                          paramData.ExamType == 3 ? "Bài tập" : "Không xác định";
                var validateData2 = _courseExamRepo.GetAll(x => x.CourseId == paramData.CourseId && x.ExamType == paramData.ExamType);
                if (validateData2 == null || validateData2?.Count <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Dề thi loại [{examTypeName}] không tồn tại!"));
                }

                var courseExamResult = await _courseExamResultRepo.GetByIDAsync(paramData.CourseExamResult.ID) ?? new CourseExamResult();
                courseExamResult.CourseExamId = paramData.CourseExamResult.CourseExamId;
                courseExamResult.EmployeeId = paramData.CourseExamResult.EmployeeId;
                courseExamResult.PracticePoints = paramData.CourseExamResult.PracticePoints;
                courseExamResult.Evaluate = paramData.CourseExamResult.Evaluate;
                courseExamResult.Note = paramData.CourseExamResult.Note;

                if (paramData.CourseExamResult.ID <= 0)
                {
                    courseExamResult.PercentageCorrect = 0;
                    courseExamResult.Status = 0;
                    courseExamResult.TotalCorrect = 0;
                    courseExamResult.TotalIncorrect = 0;
                    await _courseExamResultRepo.CreateAsync(courseExamResult);
                }
                else
                {
                    await _courseExamResultRepo.UpdateAsync(courseExamResult);
                }

                if (courseExamResult.ID > 0)
                    return Ok(ApiResponseFactory.Success(courseExamResult, $"Lưu kết quả [{examTypeName}] thành công!"));
                else
                    return BadRequest(ApiResponseFactory.Fail(null, $"Lưu kết quả [{examTypeName}] không thành công!", courseExamResult));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // load chi tiêt kết quả thi
        [HttpGet("get-exam-result-detail")]
        public IActionResult GetExamResultDetail(int? courseID, int? courseResultID, int? employeeID, int? courseExamID)
        {
            try
            {
                courseID ??= 0;
                courseResultID ??= 0;
                employeeID ??= 0;
                courseExamID ??= 0;

                var data = SQLHelper<object>.ProcedureToList("spGetCourseExamResultDetail",
                            new string[] { "@CourseID", "@CourseExamResultID", "@EmployeeID", "@CourseExamID" },
                            new object[] { courseID, courseResultID, employeeID, courseExamID });

                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-course-exam-result-by-id")]
        public async Task<IActionResult> getCourseExamResultById(int? id)
        {
            try
            {
                var data = await _courseExamResultRepo.GetByIDAsync(id ?? 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //
        [HttpGet("get-practice-result-history")]
        public IActionResult GetPracticeResultHistory(int? employeeId, int? courseExamId)
        {
            try
            {
                employeeId = employeeId ?? 0;
                courseExamId = courseExamId ?? 0;
                var data = SQLHelper<object>.ProcedureToList("spGetResultHistoryPractice",
                                                   new string[] { "@EmployeeId", "@CourseExamId" },
                                                   new object[] { employeeId, courseExamId });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-practice-evaluation-details")]
        public IActionResult GetPracticeEvaluationDetails(int? courseExamId, int? employeeId, int? courseResultId)
        {
            try
            {
                courseExamId = courseExamId ?? 0;
                employeeId = employeeId ?? 0;
                courseResultId = courseResultId ?? 0;
                var data = SQLHelper<object>.ProcedureToList("spGetResultHistoryByPractice",
                                                 new string[] { "@CourseExamId", "@EmployeeId", "@CourseResultId" },
                                                 new object[] { courseExamId, employeeId, courseResultId });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //
        [HttpPost("save-practice-evaluation")]
        public async Task<IActionResult> SavePracticeEvaluation([FromBody] SavePracticeEvaluationParam paramData)
        {
            try
            {
                if (paramData == null) return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ!"));

                string examTypeName = paramData.ExamType == 1 ? "Trắc nghiệm" :
                                      paramData.ExamType == 2 ? "Thực hành" :
                                      paramData.ExamType == 3 ? "Bài tập" : "Không xác định";

                // Logic tính toán Pass/Fail trong API
                decimal point = 0;
                decimal totalPoint = 0;

                if (paramData.CourseExamResult == null || paramData.CourseExamResult.ID <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu lịch sử thi! - [0]"));

                foreach (var eval in paramData.Evaluations ?? new List<CourseExamEvaluate>())
                {
                    if (eval.ID <= 0) continue;
                    var detail = await _courseExamEvaluateRepo.GetByIDAsync(eval.ID);
                    if (detail != null)
                    {
                        detail.Point = eval.Point ?? 0;
                        detail.Note = eval.Note;
                        await _courseExamEvaluateRepo.UpdateAsync(detail);
                        point += detail.Point ?? 0;
                        totalPoint += 10; // Mỗi câu tối đa 10 điểm
                    }
                }

                var courseExamResult = await _courseExamResultRepo.GetByIDAsync(paramData.CourseExamResult.ID) ?? new CourseExamResult();
                if (courseExamResult.ID <= 0) return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy dữ liệu lịch sử thi! - [{paramData.CourseExamResult.ID}]"));

                int courseExamId = paramData?.CourseExamResult?.CourseExamId ?? 0;
                if (courseExamId <= 0) return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy dữ liệu điểm thi! - [{courseExamId}]"));
                var courseExam = await _courseExamRepo.GetByIDAsync(courseExamId) ?? new CourseExam();
                if (courseExam.ID <= 0) return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy dữ liệu điểm thi! - [{courseExamId}]"));

                courseExamResult.Status = 3;
                courseExamResult.PracticePoints = point;

                decimal goalPoint = courseExam?.Goal ?? 0;
                if (totalPoint == 0)
                {
                    courseExamResult.Evaluate = false;
                }
                else
                {
                    courseExamResult.Evaluate = ((point / totalPoint) * 100) >= goalPoint ? true : false;
                }

                await _courseExamResultRepo.UpdateAsync(courseExamResult);

                return Ok(ApiResponseFactory.Success(courseExamResult, $"Lưu kết quả điểm thi [{examTypeName}] thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


    }
}
