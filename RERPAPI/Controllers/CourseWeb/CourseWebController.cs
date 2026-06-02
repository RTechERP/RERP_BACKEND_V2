using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities.RTCCourse;
using RERPAPI.Model.Param.CourseWeb;
using RERPAPI.Model.Param.HRM.Course;
using RERPAPI.Repo.GenericCourseEntity;

namespace RERPAPI.Controllers.CourseWeb
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseWebController : ControllerBase
    {
        private readonly CourseCatalogRepo _courseCatalogRepo;
        private readonly CourseRepo _courseRepo;
        private readonly CourseLessonRepo _courseLessonRepo;
        private readonly CourseFileRepo _courseFilesRepo;
        private readonly EmployeeRepo _employeeRepo;
        private readonly CourseCatalogTypeRepo _courseCatalogTypeRepo;
        private readonly CourseAnswersRepo _courseAnswersRepo;
        private readonly CourseExamRepo _courseExamRepo;
        private readonly CourseQuestionRepo _courseQuestionRepo;
        private readonly ConfigSystemRepo _configSystemRepo;
        private readonly CourseRightAnswersRepo _courseRightAnswersRepo;
        private readonly RERPAPI.Repo.GenericEntity.CourseCatalogRepo _courseCatalogOldWebRepo;
        private readonly RERPAPI.Repo.GenericEntity.CourseRepo _courseOldWebRepo;
        private readonly RERPAPI.Repo.GenericEntity.CourseLessonRepo _courseLessonOldWebRepo;
        private readonly RERPAPI.Repo.GenericEntity.ConfigSystemRepo _configSystemOldWebRepo;
        private readonly RERPAPI.Repo.GenericEntity.CourseFilesRepo _courseFileOldWebRepo;
        private readonly RERPAPI.Repo.GenericEntity.CourseExamRepo _courseExamOldWebRepo;

        private readonly RERPAPI.Repo.GenericEntity.CourseExamRepo _courseExamOldWdfebRepo;
        private readonly RERPAPI.Repo.GenericEntity.CourseQuestionRepo _courseQuestionOldWebRepo;
        private readonly RERPAPI.Repo.GenericEntity.CourseAnswerRepo _courseAnswersOldWebRepo;
        private readonly RERPAPI.Repo.GenericEntity.CourseRightAnswerRepo _courseRightAnswersOldWebRepo;

        public CourseWebController(
          CourseCatalogRepo courseCatalogRepo,
          CourseRepo courseRepo,
          CourseLessonRepo courseLessonRepo,
          ConfigSystemRepo configSystemRepo,
          CourseFileRepo courseFilesRepo,
            EmployeeRepo employeeRepo,
            CourseCatalogTypeRepo courseCatalogTypeRepo,

        CourseAnswersRepo courseAnswersRepo,
        CourseRightAnswersRepo courseRightAnswersRepo,
            CourseQuestionRepo courseQuestionRepo,
             CourseExamRepo courseExamRepo,
            RERPAPI.Repo.GenericEntity.CourseCatalogRepo courseCatalogOldWebRepo,
            RERPAPI.Repo.GenericEntity.CourseRepo courseOldWebRepo,
             RERPAPI.Repo.GenericEntity.CourseLessonRepo courseLessonOldWebRepo,
             RERPAPI.Repo.GenericEntity.ConfigSystemRepo configSystemOldWebRepo,
             RERPAPI.Repo.GenericEntity.CourseFilesRepo courseFileOldWebRepo,
             RERPAPI.Repo.GenericEntity.CourseExamRepo courseExamOldWebRepo,
             RERPAPI.Repo.GenericEntity.CourseQuestionRepo courseQuestionOldWebRepo,
             RERPAPI.Repo.GenericEntity.CourseAnswerRepo courseAnswersOldWebRepo,
             RERPAPI.Repo.GenericEntity.CourseRightAnswerRepo courseRightAnswersOldWebRepo
          )
        {
            _courseCatalogRepo = courseCatalogRepo;
            _courseRepo = courseRepo;
            _courseLessonRepo = courseLessonRepo;
            _configSystemRepo = configSystemRepo;
            _courseFilesRepo = courseFilesRepo;
            _employeeRepo = employeeRepo;
            _courseCatalogTypeRepo = courseCatalogTypeRepo;
            _courseCatalogOldWebRepo = courseCatalogOldWebRepo;
            _courseOldWebRepo = courseOldWebRepo;
            _courseLessonOldWebRepo = courseLessonOldWebRepo;
            _configSystemOldWebRepo = configSystemOldWebRepo;
            _courseFileOldWebRepo = courseFileOldWebRepo;
            _courseExamOldWebRepo = courseExamOldWebRepo;
            _courseExamRepo = courseExamRepo;
            _courseQuestionOldWebRepo = courseQuestionOldWebRepo;
            _courseQuestionRepo = courseQuestionRepo;
            _courseAnswersOldWebRepo = courseAnswersOldWebRepo;
            _courseRightAnswersRepo = courseRightAnswersRepo;
            _courseRightAnswersOldWebRepo = courseRightAnswersOldWebRepo;
            _courseAnswersRepo = courseAnswersRepo;
        }

        [HttpGet("get-course-summary")]
        public IActionResult GetCourseSummary(int? employeeID)
        {
            try
            {
                employeeID = employeeID ?? 0;

                var data = SQLCourseHelper<object>.ProcedureToList("spGetCourseNew",
                                                new string[] { "@Status", "@EmployeeID" },
                                                new object[] {  -1, employeeID,
                });

                return Ok(ApiResponseFactory.Success(SQLCourseHelper<object>.GetListData(data, 0), ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("load-danhmuc")]
        public async Task<IActionResult> GetDanhMuc(int catalogType)
        {
            try
            {
                //var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                //var currentUser = ObjectMapper.GetCurrentUser(claims);
                var data = SQLCourseHelper<object>.ProcedureToList("spGetCourseCatalog",
                                              new string[] { },
                                              new object[] { });
                var data0 = SQLCourseHelper<object>.GetListData(data, 0);
                //var data = _courseCatalogRepo.GetAll(c=>c.IsDeleted!=true).OrderBy(c=>c.STT);
                return Ok(ApiResponseFactory.Success(data0, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-stt-course-catalog")]
        public async Task<IActionResult> GetSTTCourseCatalog()
        {
            try
            {
                var maxSTT = _courseCatalogRepo.GetAll(c => c.IsDeleted != true).Max(c => c.STT);
                maxSTT = maxSTT.HasValue ? maxSTT.Value + 1 : 1;
                return Ok(ApiResponseFactory.Success(maxSTT, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // get stt course
        [HttpGet("get-stt-course")]
        public async Task<IActionResult> GetSTTCourse(int CourseTypeID, int CourseCatalogID)
        {
            try
            {
                var maxSTT = _courseRepo.GetAll(c => c.CourseCatalogID == CourseCatalogID && c.CourseTypeID == CourseTypeID && c.DeleteFlag == true).Max(c => c.STT);
                maxSTT = maxSTT.HasValue ? maxSTT.Value + 1 : 1;
                return Ok(ApiResponseFactory.Success(maxSTT, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-stt-lesson")]
        public async Task<IActionResult> GetSTTLesson(int CourseID)
        {
            try
            {
                var maxSTT = _courseLessonRepo.GetAll(c => c.CourseID == CourseID && c.IsDeleted != true).Max(c => c.STT);
                maxSTT = maxSTT.HasValue ? (maxSTT.Value + 1) : 1;
                return Ok(ApiResponseFactory.Success(maxSTT, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //lấy danh sách khóa học
        [HttpGet("load-data-course")]
        public async Task<IActionResult> LoadDataCourse(int courseCatalogID)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var data = SQLCourseHelper<object>.ProcedureToList("spGetCourseNew",
                                             new string[] { "@CourseCatalogID", "@EmployeeID", "@Status" },
                                             new object[] { courseCatalogID, currentUser.EmployeeID, -1 });
                var data0 = SQLCourseHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(data0, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //lấy danh sách bài học
        [HttpGet("load-dataLesson")]
        public async Task<IActionResult> LoadDataLesson(int courseID)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var data = SQLCourseHelper<object>.ProcedureToList("spGetLesson",
                                              new string[] { "@CourseID" },
                                              new object[] { courseID });
                return Ok(ApiResponseFactory.Success(SQLCourseHelper<object>.GetListData(data, 0), ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Thêm sửa xóa danh mục khóa học
        [HttpPost("save-data-category")]
        public async Task<IActionResult> SaveDataCategory([FromBody] CourseCatalog model)
        {
            try
            {
                var exitCategory = _courseCatalogRepo.GetAll(x => (x.Code.ToUpper().Trim() == model.Code.ToUpper().Trim()) && x.ID != model.ID && (!x.IsDeleted ?? true)).FirstOrDefault();

                if (exitCategory != null && exitCategory.ID > 0)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Mã danh mục đã tồn tại! Vui lòng kiểm tra lại."));
                }

                if (model.ID <= 0)
                {
                    var courseCatalog = new CourseCatalog
                    {
                        ID = 0,
                        Name = model.Name,
                        STT = model.STT,
                        Code = model.Code,
                    };

                    await _courseCatalogRepo.CreateAsync(courseCatalog);
                    if (courseCatalog.ID > 0)
                    {
                        courseCatalog.DeleteFlag = true;
                        await _courseCatalogRepo.UpdateAsync(courseCatalog);
                    }
                }
                else
                {
                    var courseCatalog = _courseCatalogRepo.GetByID(model.ID);
                    courseCatalog.Name = model.Name;
                    courseCatalog.STT = model.STT;
                    courseCatalog.IsDeleted = model.IsDeleted;
                    courseCatalog.DeleteFlag = model.DeleteFlag;
                    courseCatalog.Code = model.Code;
                    courseCatalog.CatalogType = model.CatalogType;
                    await _courseCatalogRepo.UpdateAsync(courseCatalog);
                }
                return Ok(ApiResponseFactory.Success(model, "Lưu danh mục khóa học thành công"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // lấy danh sách nhân viên cho bảng trong tổng hợp khóa học
        [HttpGet("get-employees")]
        public IActionResult GetEmployee()
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetEmployee",
                                                new string[] { "@Status" },
                                                new object[] { 0 });
                var data0 = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(data0, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-course-catalog-type")]
        public IActionResult GetCatalogType()
        {
            try
            {
                var data = _courseCatalogTypeRepo.GetAll(c => c.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-employees-web")]
        public IActionResult GetEmployeeWeb(int? employeeID)
        {
            try
            {
                var data = SQLCourseHelper<object>.ProcedureToList("spGetEmployee",
                                             new string[] { "@Status", "@ID" },
                                             new object[] { 0, employeeID,
             });
                var data0 = SQLCourseHelper<object>.GetListData(data, 0);
                //var data = _employeeRepo.GetAll(c => c.Status == 0);
                //if (employeeID !=null && employeeID !=0)
                //{
                //     data = data.Where(c=>c.ID == employeeID).ToList();
                //}
                return Ok(ApiResponseFactory.Success(data0, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Thêm sửa xóa khóa học
        [HttpPost("save-data-course")]
        public async Task<IActionResult> SaveDataCourse([FromBody] SaveCourseParam model)
        {
            try
            {
                if ((!(model.DeleteFlag) ?? false) && (model.ID > 0))
                {
                    var existingCourse = _courseRepo.GetAll(x => x.ID == model.ID).FirstOrDefault();
                    if (existingCourse != null)
                    {
                        existingCourse.DeleteFlag = false; // khoá
                        await _courseRepo.UpdateAsync(existingCourse);
                        return Ok(ApiResponseFactory.Success(model, "Khóa học đã được khoá thành công"));
                    }
                }

                var exitCoure = _courseRepo.GetAll(x => (x.Code.ToUpper().Trim() == model.Code.ToUpper().Trim()) && x.ID != model.ID && (x.DeleteFlag ?? true)).FirstOrDefault();

                if (exitCoure != null && exitCoure.ID > 0)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Mã khóa học đã tồn tại! Vui lòng kiểm tra lại."));
                }

                if (model.ID <= 0)
                {
                    var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                    var currentUser = ObjectMapper.GetCurrentUser(claims);

                    var courseNew = new Course
                    {
                        ID = 0,
                        STT = model.STT,
                        Code = model.Code,
                        NameCourse = model.NameCourse,
                        CourseCatalogID = model.CourseCatalogID,
                        QuestionCount = model.QuestionCount,
                        QuestionDuration = model.QuestionDuration,
                        LeadTime = model.LeadTime,
                        CourseCopyID = model.CourseCopyID,
                        CourseTypeID = model.CourseTypeID,
                        EmployeeID = model.EmployeeID,
                        Instructor = currentUser.FullName,
                        FileCourseID = 0,
                        IsPractice = false,
                    };

                    await _courseRepo.CreateAsync(courseNew);
                    if (courseNew.ID > 0)
                    {
                        courseNew.DeleteFlag = true; // trạng thái hiển thị
                        await _courseRepo.UpdateAsync(courseNew);
                    }
                }
                else
                {
                    var courseUpdate = _courseRepo.GetByID(model.ID);
                    courseUpdate.STT = model.STT;
                    courseUpdate.Code = model.Code;
                    courseUpdate.NameCourse = model.NameCourse;
                    courseUpdate.CourseCatalogID = model.CourseCatalogID;
                    courseUpdate.DeleteFlag = model.DeleteFlag;
                    courseUpdate.QuestionCount = model.QuestionCount;
                    courseUpdate.QuestionDuration = model.QuestionDuration;
                    courseUpdate.LeadTime = model.LeadTime;
                    courseUpdate.CourseCopyID = model.CourseCopyID;
                    courseUpdate.CourseTypeID = model.CourseTypeID;
                    courseUpdate.EmployeeID = model.EmployeeID;
                    await _courseRepo.UpdateAsync(courseUpdate);
                }
                return Ok(ApiResponseFactory.Success(model, "Lưu khóa học thành công"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("load-lessons-by-catalog")]
        public IActionResult getLessonsByCatalog(int courseID)
        {
            try
            {
                var data = SQLCourseHelper<object>.ProcedureToList("spGetLessonByCourseCatalogID",
                                              new string[] { "@CourseCatalogID" },
                                              new object[] { courseID });
                return Ok(ApiResponseFactory.Success(SQLCourseHelper<object>.GetListData(data, 0), ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-lesson-by-id")]
        public async Task<IActionResult> getLessonByid(int id)
        {
            try
            {
                var data = _courseLessonRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("get-course-lesson-file-by-lessonid")]
        public IActionResult GetCourseLessonFileByLessonId(int lessonId)
        {
            try
            {
                var courseLessonFile = _courseFilesRepo.GetAll(x => x.LessonID == lessonId && (!x.IsDeleted ?? true));

                return Ok(ApiResponseFactory.Success(courseLessonFile, "Success"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, $"Lỗi GET lesson file by lesonId: {ex.Message}"));
            }
        }

        [HttpGet("get-path-server")]
        public async Task<IActionResult> getPathServer(string subPath)
        {
            try
            {
                var pathUpload = _configSystemRepo.GetUploadPathByKey("CourseLesson");
                subPath = $"/{subPath}/";
                string path = pathUpload + subPath;
                return Ok(ApiResponseFactory.Success(path, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Thêm sửa xóa bài học
        [HttpPost("save-data-lesson")]
        public async Task<IActionResult> SaveDataLesson([FromBody] SaveCourseLessonParamWeb model)
        {
            try
            {
                var pathUpload = _configSystemRepo.GetUploadPathByKey("CourseLesson");
                var subPath = "/pdfs/";
                if (model == null)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Dữ liệu gửi lên không hợp lệ."));
                }
                if (model.CourseLesson.IsDeleted ?? false)
                {
                    if (model.CourseLesson.ID > 0)
                    {
                        var exitLesson = _courseLessonRepo.GetByID(model.CourseLesson.ID);

                        exitLesson.IsDeleted = true;
                        await _courseLessonRepo.UpdateAsync(exitLesson);
                    }
                }

                var exitLessonOld = _courseLessonRepo.GetAll(x => (x.Code.ToUpper().Trim() == model.CourseLesson.Code.ToUpper().Trim()) && x.ID != model.CourseLesson.ID && (!x.IsDeleted ?? true)).FirstOrDefault();

                if (exitLessonOld != null && exitLessonOld.ID > 0)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Mã bài học đã tồn tại! Vui lòng kiểm tra lại."));
                }
                if (!string.IsNullOrEmpty(model.CourseLesson.VideoURL))
                {
                    model.CourseLesson.VideoURL = _courseLessonRepo.ConvertYoutubeToEmbed(model.CourseLesson.VideoURL);
                }
                if (model.CourseLesson.ID <= 0)
                {
                    if (model.CoursePdf != null &&
                        !string.IsNullOrWhiteSpace(model.CoursePdf.NameFile))
                    {
                        model.CourseLesson.UrlPDF = pathUpload + subPath + model.CoursePdf.NameFile;
                    }

                    await _courseLessonRepo.CreateAsync(model.CourseLesson);

                    if (model.CourseFiles != null && model.CourseFiles.Any())
                    {
                        foreach (var file in model.CourseFiles)
                        {
                            file.LessonID = model.CourseLesson.ID;
                            file.ID = 0;
                            await _courseFilesRepo.CreateAsync(file);
                        }
                    }
                }
                else
                {
                    if (model.CoursePdf != null && model.CoursePdf.NameFile != null)
                    {
                        model.CourseLesson.UrlPDF = pathUpload + subPath + model.CoursePdf.NameFile;
                    }
                    await _courseLessonRepo.UpdateAsync(model.CourseLesson);

                    if (model.CourseFiles != null && model.CourseFiles.Any())
                    {
                        foreach (var file in model.CourseFiles)
                        {
                            file.LessonID = model.CourseLesson.ID;
                            if (file.ID <= 0)
                            {
                                await _courseFilesRepo.CreateAsync(file);
                            }
                            else
                            {
                                await _courseFilesRepo.UpdateAsync(file);
                            }
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(model, "Lưu bài học thành công"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-catalog-old-web")]
        public async Task<IActionResult> getCatalogOldWeb()
        {
            try
            {
                var data = _courseCatalogOldWebRepo.GetAll(c => c.IsDeleted != true && c.DeleteFlag == true).OrderBy(c => c.STT);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-catalog-type")]
        public async Task<IActionResult> getCatalogType()
        {
            try
            {
                var data = _courseCatalogTypeRepo.GetAll(c => c.IsDeleted != true).OrderBy(c => c.STT);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //[HttpPost("copy-course-catalog")]
        //public async Task<IActionResult> CopyCatalogCourse(CopyCatalogCourseParam model)
        //{
        //    try
        //    {
        //        // khai báo biến toàn cục để lưu dữ liệu khi copy
        //        var CourseCatalogInsert = new CourseCatalog();
        //        List<Course> lstCouseToInsert = new List<Course>();
        //        List<CourseLesson> lstCouseLessonToInsert = new List<CourseLesson>();
        //        List<CourseExam> lstCourseExam = new List<CourseExam>();

        //        // check exist mã danh mục
        //        var isExistCatalogCode = _courseCatalogRepo.GetAll(c => c.Code.ToLower() == model.Code.Trim().ToLower());
        //        if (isExistCatalogCode.Count() > 0)
        //        {
        //            return Ok(ApiResponseFactory.Fail(null, "Mã danh mục đã tồn tại!"));
        //        }
        //        // thêm stt max của courseCatalog theo ID theo catalogtype
        //        var courseCatalog = _courseCatalogRepo.GetAll(c => c.ID == model.CatalogTypeID && c.IsDeleted != true).Max(c => c.STT);
        //        model.STT = (courseCatalog ?? 0) + 1;

        //        // lưu thông tin danh mục khoá học mới
        //        var couseCatalogNew = new CourseCatalog
        //        {
        //            Name = model.Name,
        //            Code = model.Code,
        //            CatalogType = model.CatalogTypeID,
        //            STT = model.STT,
        //            DeleteFlag = true,
        //            IsDeleted = false
        //        };
        //        if (_courseCatalogRepo.Create(couseCatalogNew) <= 0)
        //        {
        //            return Ok(ApiResponseFactory.Fail(null, "Có lỗi xảy ra khi tạo danh mục khoá học!"));
        //        }
        //        CourseCatalogInsert = couseCatalogNew;

        //        // check is copy khoá học
        //        if (model.CopyCourses)
        //        {
        //            // check danh mục khoá học tồn tại ở web cũ
        //            var couseCatalogOldWeb = _courseCatalogOldWebRepo.GetByID(model.SourceCatalogID);
        //            if (couseCatalogOldWeb.ID <= 0)
        //            {
        //                return Ok(ApiResponseFactory.Success(null, $"Danh mục khoá học không tồn tại!"));

        //            }
        //            var course = _courseOldWebRepo.GetAll(c => c.DeleteFlag == true && c.CourseCatalogID == model.SourceCatalogID).OrderBy(c => c.STT);
        //            // check khoá học trong danh mục
        //            if (course.Count() == 0)
        //            {
        //                return Ok(ApiResponseFactory.Success(null, $"Danh mục khoá học {couseCatalogOldWeb.Name} không có khoá học!"));
        //            }
        //            foreach (var courseItem in course)
        //            {
        //                var newCourse = new Course
        //                {
        //                    STT = courseItem.STT,
        //                    Code = courseItem.Code,
        //                    NameCourse = courseItem.NameCourse,
        //                    Instructor = courseItem.Instructor,

        //                    // Danh mục mới sau khi copy
        //                    CourseCatalogID = CourseCatalogInsert.ID,

        //                    DeleteFlag = courseItem.DeleteFlag,
        //                    FileCourseID = courseItem.FileCourseID,
        //                    //IsPractice = courseItem.IsPractice, // có bài tập

        //                    QuestionCount = courseItem.QuestionCount, // có số câu hỏi
        //                    QuestionDuration = courseItem.QuestionDuration,
        //                    LeadTime = courseItem.LeadTime,

        //                    //CourseTypeID = courseItem.CourseTypeID,
        //                    EmployeeID = courseItem.EmployeeID

        //                };
        //                var resultCreateCourse = await _courseRepo.CreateAsync(newCourse);
        //                if (resultCreateCourse > 0)
        //                {
        //                    if (model.CopyLessons) // check nếu Coppy courseLesson
        //                    {
        //                        var lstCouseLessonOldWeb = _courseLessonOldWebRepo.GetAll(c => c.CourseID == courseItem.ID).OrderBy(c => c.STT);
        //                        foreach (var lessonItem in lstCouseLessonOldWeb)
        //                        {
        //                            var couseLessonNew = new CourseLesson();
        //                            couseLessonNew.Code = lessonItem.Code;
        //                            couseLessonNew.LessonTitle = lessonItem.LessonTitle;
        //                            couseLessonNew.LessonContent = lessonItem.LessonContent;
        //                            couseLessonNew.Duration = lessonItem.Duration;
        //                            //couseLessonNew.VideoURL = lessonItem.VideoURL; // không copy url video
        //                            couseLessonNew.STT = lessonItem.STT;
        //                            couseLessonNew.CourseID = newCourse.ID;
        //                            couseLessonNew.UrlPDF = FileCourseHelper.CopyFile(lessonItem.UrlPDF, Path.Combine(_configSystemRepo.GetUploadPathByKey("CourseLesson"), "pdfs")); // copy file pdf đính kèm
        //                            couseLessonNew.Duration = lessonItem.Duration;
        //                            couseLessonNew.EmployeeID = lessonItem.EmployeeID;
        //                            couseLessonNew.IsDeleted = lessonItem.IsDeleted;

        //                            var resultCreateLesson = await _courseLessonRepo.CreateAsync(couseLessonNew);

        //                            if (resultCreateLesson > 0)
        //                            {
        //                                // copy file đính kèm
        //                                var lstCourseFile = new List<CourseFile>();
        //                                var courseFileOldWeb = _courseFileOldWebRepo.GetAll(c => c.LessonID == lessonItem.ID && c.IsDeleted == false);
        //                                foreach (var courseFileItem in courseFileOldWeb)
        //                                {
        //                                    lstCourseFile.Add(new CourseFile
        //                                    {
        //                                        NameFile = courseFileItem.NameFile,
        //                                        LessonID = couseLessonNew.ID,
        //                                        OriginPath = courseFileItem.OriginPath,
        //                                        IsDeleted = courseFileItem.IsDeleted,
        //                                        ServerPath = FileCourseHelper.CopyFile(courseFileItem.ServerPath, Path.Combine(_configSystemRepo.GetUploadPathByKey("CourseLesson"), "files")) // copy file
        //                                    });
        //                                }
        //                                await _courseFilesRepo.CreateRangeAsync(lstCourseFile);
        //                            }
        //                            // check is copy đề thi/ đáp án
        //                            if (model.CopyExams)
        //                            {
        //                                // kiểm tra copy câu hỏi, đề thi cho bài học
        //                                var listCourseExam = _courseExamOldWebRepo.GetAll(c=>c.LessonID == lessonItem.ID);

        //                                foreach (var examOld in listCourseExam)
        //                                {
        //                                    lstCourseExam.Add(new CourseExam
        //                                    {
        //                                        CodeExam = examOld.CodeExam,
        //                                        NameExam = examOld.NameExam,
        //                                        CourseId = -1,
        //                                        LessonID = couseLessonNew.ID,
        //                                        ExamType = examOld.ExamType,
        //                                        Goal = examOld.Goal,
        //                                        TestTime = examOld.TestTime,
        //                                    });
        //                                }
        //                            }
        //                        }
        //                    }
        //                }

        //            }
        //        }
        //        return Ok(ApiResponseFactory.Success(null, "Success"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(ApiResponseFactory.Fail(ex, $"Lỗi GET lesson file by lesonId: {ex.Message}"));
        //    }
        //}

        [HttpPost("copy-course-catalog")]
        public async Task<IActionResult> CopyCatalogCourse([FromBody] CopyCatalogCourseParam model)
        {
            try
            {
                #region Validate input

                if (model == null)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ!"));
                }

                if (model.SourceCatalogID <= 0)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Vui lòng chọn danh mục cần copy!"));
                }

                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    return Ok(ApiResponseFactory.Fail(null, "Tên danh mục mới không được để trống!"));
                }

                if (string.IsNullOrWhiteSpace(model.Code))
                {
                    return Ok(ApiResponseFactory.Fail(null, "Mã danh mục mới không được để trống!"));
                }

                if (model.CatalogTypeID <= 0)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Vui lòng chọn loại danh mục!"));
                }

                if (!model.CopyCourses && !model.CopyLessons && !model.CopyExams)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Vui lòng chọn ít nhất một dữ liệu cần copy!"));
                }

                var newCatalogCode = model.Code.Trim();

                var isExistCatalogCode = _courseCatalogRepo
                    .GetAll(c => c.Code != null && c.Code.ToLower() == newCatalogCode.ToLower())
                    .Any();

                if (isExistCatalogCode)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Mã danh mục đã tồn tại!"));
                }

                #endregion Validate input

                #region Check source catalog old web

                var oldCatalog = _courseCatalogOldWebRepo.GetByID(model.SourceCatalogID);

                if (oldCatalog == null || oldCatalog.ID <= 0)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Danh mục khóa học nguồn không tồn tại!"));
                }

                #endregion Check source catalog old web

                #region Prepare folder

                var courseLessonRoot = _configSystemRepo.GetUploadPathByKey("CourseLesson");

                if (string.IsNullOrWhiteSpace(courseLessonRoot))
                {
                    return Ok(ApiResponseFactory.Fail(null, "Chưa cấu hình thư mục upload CourseLesson!"));
                }

                var pdfFolder = Path.Combine(courseLessonRoot, "pdfs");
                var fileFolder = Path.Combine(courseLessonRoot, "files");

                #endregion Prepare folder

                #region Create new catalog

                var maxStt = _courseCatalogRepo
                    .GetAll(c => c.CatalogType == model.CatalogTypeID && c.IsDeleted != true)
                    .Select(c => c.STT ?? 0)
                    .DefaultIfEmpty(0)
                    .Max();

                var newCatalog = new CourseCatalog
                {
                    Name = model.Name.Trim(),
                    Code = newCatalogCode,
                    CatalogType = model.CatalogTypeID,
                    STT = maxStt + 1,
                    DeleteFlag = true,
                    IsDeleted = false,
                    CreatedDate = DateTime.Now
                };

                var resultCreateCatalog = _courseCatalogRepo.Create(newCatalog);

                if (resultCreateCatalog <= 0 || newCatalog.ID <= 0)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Có lỗi xảy ra khi tạo danh mục khóa học!"));
                }

                #endregion Create new catalog

                #region Copy courses

                if (!model.CopyCourses)
                {
                    return Ok(ApiResponseFactory.Success(new { NewCatalogID = newCatalog.ID }, "Tạo danh mục khóa học thành công!"));
                }

                var oldCourses = _courseOldWebRepo
                    .GetAll(c => c.DeleteFlag == true && c.CourseCatalogID == model.SourceCatalogID)
                    .OrderBy(c => c.STT)
                    .ToList();

                if (!oldCourses.Any())
                {
                    return Ok(ApiResponseFactory.Fail(null, $"Danh mục khóa học {oldCatalog.Name} không có khóa học!"));
                }

                var courseIdMap = new Dictionary<int, int>();
                var lessonIdMap = new Dictionary<int, int>();
                var examIdMap = new Dictionary<int, int>();
                var questionIdMap = new Dictionary<int, int>();
                var answerIdMap = new Dictionary<int, int>();

                foreach (var oldCourse in oldCourses)
                {
                    var newCourse = new Course
                    {
                        STT = oldCourse.STT,
                        Code = oldCourse.Code,
                        NameCourse = oldCourse.NameCourse,
                        Instructor = oldCourse.Instructor,

                        CourseCatalogID = newCatalog.ID,

                        DeleteFlag = true,
                        FileCourseID = oldCourse.FileCourseID,
                        IsPractice = oldCourse.IsPractice,

                        QuestionCount = oldCourse.QuestionCount,
                        QuestionDuration = oldCourse.QuestionDuration,
                        LeadTime = oldCourse.LeadTime,

                        CourseCopyID = oldCourse.ID,
                        CourseTypeID = oldCourse.CourseTypeID,
                        EmployeeID = oldCourse.EmployeeID,

                        CreatedBy = oldCourse.CreatedBy,
                        CreatedDate = DateTime.Now,
                        UpdatedBy = null,
                        UpdatedDate = null
                    };

                    var resultCreateCourse = await _courseRepo.CreateAsync(newCourse);

                    if (resultCreateCourse <= 0 || newCourse.ID <= 0)
                    {
                        return Ok(ApiResponseFactory.Fail(null, $"Có lỗi khi copy khóa học: {oldCourse.NameCourse}"));
                    }

                    courseIdMap[oldCourse.ID] = newCourse.ID;

                    #region Copy course-level exams

                    if (model.CopyExams)
                    {
                        var oldCourseExams = _courseExamOldWebRepo
                            .GetAll(e =>
                                e.CourseId == oldCourse.ID &&
                                (e.LessonID == null || e.LessonID == 0))
                            .ToList();

                        foreach (var oldExam in oldCourseExams)
                        {
                            var copyExamResult = await CopyExamFullAsync(
                                oldExam: oldExam,
                                newCourseId: newCourse.ID,
                                newLessonId: null,
                                examIdMap: examIdMap,
                                questionIdMap: questionIdMap,
                                answerIdMap: answerIdMap
                            );

                            if (!copyExamResult.Success)
                            {
                                return Ok(ApiResponseFactory.Fail(null, copyExamResult.Message));
                            }
                        }
                    }

                    #endregion Copy course-level exams

                    #region Copy lessons

                    if (!model.CopyLessons)
                    {
                        continue;
                    }

                    var oldLessons = _courseLessonOldWebRepo
                        .GetAll(l => l.CourseID == oldCourse.ID && l.IsDeleted != true)
                        .OrderBy(l => l.STT)
                        .ToList();

                    foreach (var oldLesson in oldLessons)
                    {
                        var newLesson = new CourseLesson
                        {
                            Code = oldLesson.Code,
                            LessonTitle = oldLesson.LessonTitle,
                            LessonContent = oldLesson.LessonContent,
                            Duration = oldLesson.Duration,

                            // Không copy video
                            VideoURL = null,

                            STT = oldLesson.STT,
                            CourseID = newCourse.ID,

                            // Copy file PDF
                            UrlPDF = FileCourseHelper.CopyFile(oldLesson.UrlPDF, pdfFolder),

                            FileCourseID = oldLesson.FileCourseID,
                            LessonCopyID = oldLesson.ID,
                            IsDeleted = oldLesson.IsDeleted,
                            EmployeeID = oldLesson.EmployeeID,
                            RequiredWatchedPercent = oldLesson.RequiredWatchedPercent,
                            Chapters = oldLesson.Chapters,

                            CreatedBy = oldLesson.CreatedBy,
                            CreatedDate = DateTime.Now,
                            UpdatedBy = null,
                            UpdatedDate = null
                        };

                        var resultCreateLesson = await _courseLessonRepo.CreateAsync(newLesson);

                        if (resultCreateLesson <= 0 || newLesson.ID <= 0)
                        {
                            return Ok(ApiResponseFactory.Fail(null, $"Có lỗi khi copy bài học: {oldLesson.LessonTitle}"));
                        }

                        lessonIdMap[oldLesson.ID] = newLesson.ID;

                        #region Copy lesson files

                        var oldFiles = _courseFileOldWebRepo
                            .GetAll(f => f.LessonID == oldLesson.ID && f.IsDeleted == false)
                            .ToList();

                        var newFiles = new List<CourseFile>();

                        foreach (var oldFile in oldFiles)
                        {
                            var oldFilePath = !string.IsNullOrWhiteSpace(oldFile.ServerPath)
                                ? oldFile.ServerPath
                                : oldFile.OriginPath;

                            var newServerPath = FileCourseHelper.CopyFile(oldFilePath, fileFolder);

                            if (string.IsNullOrWhiteSpace(newServerPath))
                            {
                                continue;
                            }

                            newFiles.Add(new CourseFile
                            {
                                NameFile = FileCourseHelper.GetFileNameWithExtension(newServerPath),
                                LessonID = newLesson.ID,
                                CourseID = newCourse.ID,
                                OriginPath = oldFilePath,
                                ServerPath = newServerPath,
                                IsDeleted = oldFile.IsDeleted,
                            });
                        }

                        if (newFiles.Any())
                        {
                            var resultCreateFiles = await _courseFilesRepo.CreateRangeAsync(newFiles);

                            if (resultCreateFiles <= 0)
                            {
                                return Ok(ApiResponseFactory.Fail(null, $"Có lỗi khi copy file đính kèm của bài học: {oldLesson.LessonTitle}"));
                            }
                        }

                        #endregion Copy lesson files

                        #region Copy lesson exams

                        if (model.CopyExams)
                        {
                            var oldLessonExams = _courseExamOldWebRepo
                                .GetAll(e => e.LessonID == oldLesson.ID)
                                .ToList();

                            foreach (var oldExam in oldLessonExams)
                            {
                                var copyExamResult = await CopyExamFullAsync(
                                    oldExam: oldExam,
                                    newCourseId: -1,
                                    newLessonId: newLesson.ID,
                                    examIdMap: examIdMap,
                                    questionIdMap: questionIdMap,
                                    answerIdMap: answerIdMap
                                );

                                if (!copyExamResult.Success)
                                {
                                    return Ok(ApiResponseFactory.Fail(null, copyExamResult.Message));
                                }
                            }
                        }

                        #endregion Copy lesson exams
                    }

                    #endregion Copy lessons
                }

                #endregion Copy courses

                return Ok(ApiResponseFactory.Success(new
                {
                    NewCatalogID = newCatalog.ID,
                    CourseMap = courseIdMap,
                    LessonMap = lessonIdMap,
                    ExamMap = examIdMap,
                    QuestionMap = questionIdMap,
                    AnswerMap = answerIdMap
                }, "Copy danh mục khóa học thành công!"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, $"Có lỗi xảy ra khi copy danh mục khóa học: {ex.Message}"));
            }
        }

        private async Task<(bool Success, string Message)> CopyExamFullAsync(
    RERPAPI.Model.Entities.CourseExam oldExam,
    int newCourseId,
    int? newLessonId,
    Dictionary<int, int> examIdMap,
    Dictionary<int, int> questionIdMap,
    Dictionary<int, int> answerIdMap)
        {
            var newExam = new CourseExam
            {
                CourseId = newCourseId,
                LessonID = newLessonId,

                NameExam = oldExam.NameExam,
                CodeExam = oldExam.CodeExam,
                Goal = oldExam.Goal,
                TestTime = oldExam.TestTime,
                ExamType = oldExam.ExamType,
            };

            var resultCreateExam = await _courseExamRepo.CreateAsync(newExam);

            if (resultCreateExam <= 0 || newExam.ID <= 0)
            {
                return (false, $"Có lỗi khi copy bài thi: {oldExam.NameExam}");
            }

            examIdMap[oldExam.ID] = newExam.ID;

            var oldQuestions = _courseQuestionOldWebRepo
                .GetAll(q => q.CourseExamId == oldExam.ID)
                .OrderBy(q => q.STT)
                .ToList();

            var oldQuestionIds = oldQuestions.Select(q => q.ID).ToList();
            var oldQuestionImageFolder = _configSystemOldWebRepo.GetUploadPathByKey("CourseExamExerciseImages");
            var newQuestionImageFolder = _configSystemRepo.GetUploadPathByKey("CourseExamExerciseImages");
            foreach (var oldQuestion in oldQuestions)
            {
                var newQuestion = new CourseQuestion
                {
                    CourseExamId = newExam.ID,

                    QuestionText = oldQuestion.QuestionText,
                    STT = oldQuestion.STT,
                    CheckInput = oldQuestion.CheckInput,
                    Marks = oldQuestion.Marks,
                    Image = FileCourseHelper.CopyFileFromFolder(
        oldQuestion.Image,
        oldQuestionImageFolder,
        newQuestionImageFolder
    )
                };

                var resultCreateQuestion = await _courseQuestionRepo.CreateAsync(newQuestion);

                if (resultCreateQuestion <= 0 || newQuestion.ID <= 0)
                {
                    return (false, $"Có lỗi khi copy câu hỏi của bài thi: {oldExam.NameExam}");
                }

                questionIdMap[oldQuestion.ID] = newQuestion.ID;

                var oldAnswers = _courseAnswersOldWebRepo
                    .GetAll(a => a.CourseQuestionId == oldQuestion.ID)
                    .OrderBy(a => a.AnswerNumber)
                    .ToList();

                foreach (var oldAnswer in oldAnswers)
                {
                    var newAnswer = new CourseAnswer
                    {
                        CourseQuestionId = newQuestion.ID,

                        AnswerText = oldAnswer.AnswerText,
                        AnswerNumber = oldAnswer.AnswerNumber,
                    };

                    var resultCreateAnswer = await _courseAnswersRepo.CreateAsync(newAnswer);

                    if (resultCreateAnswer <= 0 || newAnswer.ID <= 0)
                    {
                        return (false, $"Có lỗi khi copy đáp án của câu hỏi: {oldQuestion.ID}");
                    }

                    answerIdMap[oldAnswer.ID] = newAnswer.ID;
                }
            }

            var oldRightAnswers = _courseRightAnswersOldWebRepo
                .GetAll(r =>
                    r.CourseQuestionID != null &&
                    oldQuestionIds.Contains(r.CourseQuestionID.Value))
                .ToList();

            var newRightAnswers = new List<CourseRightAnswer>();

            foreach (var oldRightAnswer in oldRightAnswers)
            {
                if (oldRightAnswer.CourseQuestionID == null || oldRightAnswer.CourseAnswerID == null)
                {
                    continue;
                }

                if (!questionIdMap.TryGetValue(oldRightAnswer.CourseQuestionID.Value, out var newQuestionId))
                {
                    continue;
                }

                if (!answerIdMap.TryGetValue(oldRightAnswer.CourseAnswerID.Value, out var newAnswerId))
                {
                    continue;
                }

                newRightAnswers.Add(new CourseRightAnswer
                {
                    CourseQuestionID = newQuestionId,
                    CourseAnswerID = newAnswerId,
                });
            }

            if (newRightAnswers.Any())
            {
                var resultCreateRightAnswers = await _courseRightAnswersRepo.CreateRangeAsync(newRightAnswers);

                if (resultCreateRightAnswers <= 0)
                {
                    return (false, $"Có lỗi khi copy đáp án đúng của bài thi: {oldExam.NameExam}");
                }
            }

            return (true, "OK");
        }
    }

    public class CopyCatalogCourseParam
    {
        public int SourceCatalogID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int CatalogTypeID { get; set; }
        public int STT { get; set; }
        public bool CopyCourses { get; set; }
        public bool CopyLessons { get; set; }
        public bool CopyExams { get; set; }
    }
}