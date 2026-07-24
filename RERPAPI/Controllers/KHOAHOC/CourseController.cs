using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Model.Param.HRM.Course;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.KHOAHOC
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CourseController : ControllerBase
    {
        private readonly CourseCatalogRepo _courseCatalogRepo;
        private readonly CourseCatalogProjectTypeRepo _courseCatalogProjectTypeRepo;
        private readonly DepartmentRepo _departmentRepo;
        private readonly KPIPositionTypeRepo _kpiPositionTypeRepo;
        private readonly RegisterIdeaRepo _registerIdeaRepo;
        private readonly CourseRepo _courseRepo;
        private readonly CourseLessonRepo _courseLessonRepo;
        private readonly CourseFilesRepo _courseFilesRepo;
        private readonly ConfigSystemRepo _configSystemRepo;

        //private readonly Course_KPIPositionTypeRepo _course_KPIPositionTypeRepo;
        private readonly CourseKPIEmployeeTeamMapRepo _course_KPIEmployeeTeamMapRepo;

        private readonly CourseKPIEmployeeTeamRepo _course_KPIEmployeeTeamRepo;
        private readonly CourseExamRepo _courseExamRepo;
        private readonly CourseQuestionRepo _courseQuestionRepo;
        private readonly CourseAnswerRepo _courseAnswerRepo;
        private readonly CourseRightAnswerRepo _courseRightAnswerRepo;
        private readonly CurrentUser _currentUser;

        public CourseController(
            CourseCatalogRepo courseCatalogRepo,
            CourseCatalogProjectTypeRepo courseCatalogProjectTypeRepo,
            DepartmentRepo departmentRepo,
            KPIPositionTypeRepo kpiPositionTypeRepo,
            CourseRepo courseRepo,
            RegisterIdeaRepo registerIdeaRepo,
            CourseLessonRepo courseLessonRepo,
            ConfigSystemRepo configSystemRepo,
            CourseFilesRepo courseFilesRepo,
           CourseKPIEmployeeTeamMapRepo course_KPIEmployeeTeamMapRepo,
           CourseKPIEmployeeTeamRepo course_KPIEmployeeTeamRepo,
           CourseExamRepo courseExamRepo,
           CourseQuestionRepo courseQuestionRepo,
           CourseAnswerRepo courseAnswerRepo,
           CourseRightAnswerRepo courseRightAnswerRepo,
           CurrentUser currentUser
            )
        {
            _courseCatalogRepo = courseCatalogRepo;
            _courseCatalogProjectTypeRepo = courseCatalogProjectTypeRepo;
            _departmentRepo = departmentRepo;
            _kpiPositionTypeRepo = kpiPositionTypeRepo;
            _courseRepo = courseRepo;
            _registerIdeaRepo = registerIdeaRepo;
            _courseLessonRepo = courseLessonRepo;
            _courseFilesRepo = courseFilesRepo;
            _configSystemRepo = configSystemRepo;
            //_course_KPIPositionTypeRepo = course_KPIPositionTypeRepo;
            _course_KPIEmployeeTeamMapRepo = course_KPIEmployeeTeamMapRepo;
            _course_KPIEmployeeTeamRepo = course_KPIEmployeeTeamRepo;
            _courseExamRepo = courseExamRepo;
            _courseQuestionRepo = courseQuestionRepo;
            _courseAnswerRepo = courseAnswerRepo;
            _courseRightAnswerRepo = courseRightAnswerRepo;
            _currentUser = currentUser;
        }

        [HttpGet("get-course-summary")]
        public IActionResult GetCourseSummary(int? departmentid, int? employeeID)
        {
            try
            {
                departmentid = departmentid ?? 0;
                employeeID = employeeID ?? 0;

                var data = SQLHelper<object>.ProcedureToList("spGetCourseNew",
                                                new string[] { "@DepartmentID", "@Status", "@EmployeeID" },
                                                new object[] { departmentid, -1, employeeID,
                });

                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("load-danhmuc")]
        public async Task<IActionResult> GetDanhMuc(int catalogType)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var data = SQLHelper<object>.ProcedureToList("spGetCourseCatalogNew1",
                                              new string[] { "@CatalogType", "@UserID" },
                                              new object[] { catalogType, currentUser.ID });
                var data0 = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(data0, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-stt-course-catalog")]
        public async Task<IActionResult> GetSTTCourseCatalog(int TypeID, int DepartmentID)
        {
            try
            {
                var maxSTT = _courseCatalogRepo.GetAll(c => c.IsDeleted != true && c.CatalogType == TypeID && c.DepartmentID == DepartmentID).Max(c => c.STT);
                maxSTT = maxSTT.HasValue ? maxSTT.Value + 1 : 1;
                return Ok(ApiResponseFactory.Success(maxSTT, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // get stt course
        [HttpGet("get-stt-course")]
        public async Task<IActionResult> GetSTTCourse(int CourseCatalogID, int CourseTypeID)
        {
            try
            {
                var maxSTT = _courseRepo.GetAll(c => c.CourseCatalogID == CourseCatalogID && c.CourseTypeID == CourseTypeID).Max(c => c.STT);
                maxSTT = maxSTT.HasValue ? maxSTT.Value + 1 : 1;
                return Ok(ApiResponseFactory.Success(maxSTT, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // get stt lesson
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
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
                var data = SQLHelper<object>.ProcedureToList("spGetCourseNew1",
                                             new string[] { "@CourseCatalogID", "@EmployeeID", "@Status" },
                                             new object[] { courseCatalogID, currentUser.EmployeeID, -1 });
                var data0 = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(data0, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //lấy danh sách bài học
        [HttpGet("get-kpi-by-courseid")]
        public async Task<IActionResult> getKPIByCourseID(int courseID)
        {
            try
            {
                //var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                //var currentUser = ObjectMapper.GetCurrentUser(claims);
                var data = _course_KPIEmployeeTeamMapRepo.GetAll(c => c.CourseID == courseID && (c.IsDeleted == false || c.IsDeleted == null));
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
                var data = SQLHelper<object>.ProcedureToList("spGetLesson",
                                              new string[] { "@CourseID" },
                                              new object[] { courseID });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //lấy danh sách team của khóa học
        [HttpGet("load-teams")]
        public async Task<IActionResult> LoadDataCourseTeam()
        {
            try
            {
                var lstTeam = SQLHelper<object>.ProcedureToList("spGetallProjectType",
                                              new string[] { },
                                              new object[] { });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(lstTeam, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //lấy danh sách Idea
        [HttpGet("load-ideas")]
        public async Task<IActionResult> LoadDataIdea(int courseCategoryID)
        {
            try
            {
                DateTime dateEnd = DateTime.Today.AddDays(1).AddSeconds(-1);

                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var data = SQLHelper<object>.ProcedureToList("spGetRegisterIdea",
                                              new string[] { "@EmployeeID", "@DepartmentID", "@AuthorID", "@DateStart", "@DateEnd", "@FilterText", "@PageNumber", "@PageSize", "@RegisterTypeID" },
                                              new object[] { 0, 0, currentUser.EmployeeID, TextUtils.MinDate, dateEnd, "", 1, 999999999, courseCategoryID });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // lấy danh sách nhân viên cho bảng trong tổng hợp khóa học
        [HttpGet("get-employees")]
        public IActionResult GetEmployee(int? userTeamID, int? departmentid, int? employeeID)
        {
            try
            {
                departmentid = departmentid ?? 0;
                userTeamID = userTeamID ?? 0;
                employeeID = employeeID ?? 0;

                var data = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployee",
                                                new string[] { "@DepartmentID", "@Status", "@ID" },
                                                new object[] { departmentid, 0, employeeID,
                });
                if (userTeamID > 0 && employeeID <= 0)
                {
                    data = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployeeByTeamID",
                                                new string[] { "@TeamID" },
                                                new object[] { userTeamID });
                }
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //Lấy Idea by Course ID
        [HttpGet("get-idea-by-courseid")]
        public IActionResult GetIdeaByCourseID(int courseID)
        {
            try
            {
                var data = _registerIdeaRepo.GetAll(x => x.CourseID == courseID && (x.IsDeleted == false || x.IsDeleted == null));
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("load-lessons-by-catalog")]
        public IActionResult getLessonsByCatalog(int courseID)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetLessonByCourseCatalogID",
                                              new string[] { "@CourseCatalogID" },
                                              new object[] { courseID });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //lấy danh sách nhân viên phụ trách
        //[HttpGet("load-employee")]
        //public async Task<IActionResult> LoadDataEmployee()
        //{
        //    try
        //    {
        //        var data = SQLHelper<object>.ProcedureToList("spGetEmployee",
        //                                      new string[] { "@DepartmentID", "@Status" },
        //                                      new object[] { 0, 0 });
        //        return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), ""));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}

        // Lấy thông tin bài học theo ID
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
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        } // Lấy videoUrl để lưu

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
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //[HttpGet("stream/{lessonId}")]
        //public IActionResult StreamByLesson(int lessonId)
        //{
        //    var lesson = _courseLessonRepo.GetByID(lessonId);
        //    if (lesson == null || string.IsNullOrEmpty(lesson.VideoURL))
        //        return NotFound();

        //    var path = lesson.VideoURL;
        //    if (!System.IO.File.Exists(path))
        //        return NotFound();

        //    return PhysicalFile(
        //        path,
        //        "video/mp4",
        //        enableRangeProcessing: true
        //    );
        //}
        //lấy danh sách KPI emplpoyee team
        [HttpGet("load-kpi-employee-team")]
        public async Task<IActionResult> LoadKPIEmployeeTeam()
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetALLCourseKPIEmployeeTeam",
                                                                new string[] { "@DepartmentID" },
                                                                new object[] { 0 });
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("copy-course-catalog-preview")]
        public IActionResult GetCopyCourseCatalogPreview(int sourceCatalogId)
        {
            try
            {
                var graph = LoadCopySourceGraph(sourceCatalogId);
                var preview = new CopyCourseCatalogPreview
                {
                    SourceCatalog = new CopyCourseCatalogSource
                    {
                        ID = graph.Catalog.ID,
                        Code = graph.Catalog.Code,
                        Name = graph.Catalog.Name,
                        DepartmentID = graph.Catalog.DepartmentID,
                        CatalogType = graph.Catalog.CatalogType,
                        ProjectTypeIDs = graph.CatalogProjectTypes.Where(x => x.ProjectTypeID.HasValue).Select(x => x.ProjectTypeID!.Value).Distinct().ToList()
                    },
                    Counts = BuildCopyCounts(graph)
                };
                return Ok(ApiResponseFactory.Success(preview, ""));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseFactory.Fail(null, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("copy-course-catalog-full")]
        public IActionResult CopyCourseCatalogFull([FromBody] CopyCourseCatalogRequest model)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var result = CopyCourseCatalog(model, currentUser);
                return Ok(ApiResponseFactory.Success(result, "Sao chép toàn bộ danh mục khóa học thành công"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseFactory.Fail(null, ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponseFactory.Fail(null, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponseFactory.Fail(null, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseFactory.Fail(ex, "Không thể sao chép danh mục khóa học."));
            }
        }

        [HttpGet("copy-course-preview")]
        public IActionResult GetCopyCoursePreview(int sourceCourseId)
        {
            try
            {
                var graph = LoadCopyCourseGraph(sourceCourseId);
                var preview = new CopyCoursePreview
                {
                    SourceCourse = new CopyCourseSource
                    {
                        ID = graph.Course.ID,
                        Code = graph.Course.Code,
                        NameCourse = graph.Course.NameCourse,
                        CourseCatalogID = graph.Course.CourseCatalogID,
                        CourseCatalogName = graph.CatalogName,
                        CatalogType = graph.CatalogType
                    },
                    Counts = new CopyCourseCounts
                    {
                        Lessons = graph.Lessons.Count,
                        CourseFiles = graph.CourseFiles.Count,
                        Exams = graph.Exams.Count,
                        Questions = graph.Questions.Count,
                        Answers = graph.Answers.Count,
                        RightAnswers = graph.RightAnswers.Count
                    }
                };
                return Ok(ApiResponseFactory.Success(preview, ""));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseFactory.Fail(null, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("copy-course")]
        public IActionResult CopyCourse([FromBody] CopyCourseRequest model)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var result = ExecuteCopyCourse(model, currentUser);
                return Ok(ApiResponseFactory.Success(result, "Sao chép khóa học thành công"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseFactory.Fail(null, ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponseFactory.Fail(null, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponseFactory.Fail(null, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseFactory.Fail(ex, "Không thể sao chép khóa học."));
            }
        }

        [HttpGet("copy-lesson-preview")]
        public IActionResult GetCopyLessonPreview(int sourceLessonId)
        {
            try
            {
                var graph = LoadCopyLessonGraph(sourceLessonId);
                var preview = new CopyLessonPreview
                {
                    SourceLesson = new CopyLessonSource
                    {
                        ID = graph.Lesson.ID,
                        Code = graph.Lesson.Code,
                        LessonTitle = graph.Lesson.LessonTitle,
                        LessonContent = graph.Lesson.LessonContent,
                        CourseID = graph.Lesson.CourseID,
                        CourseName = graph.CourseName,
                        Duration = graph.Lesson.Duration,
                        VideoURL = graph.Lesson.VideoURL,
                        UrlPDF = graph.Lesson.UrlPDF
                    },
                    Counts = new CopyLessonCounts
                    {
                        Files = graph.CourseFiles.Count,
                        Exams = graph.Exams.Count,
                        Questions = graph.Questions.Count,
                        Answers = graph.Answers.Count,
                        RightAnswers = graph.RightAnswers.Count
                    }
                };
                return Ok(ApiResponseFactory.Success(preview, ""));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseFactory.Fail(null, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("copy-lesson")]
        public IActionResult CopyLesson([FromBody] CopyLessonRequest model)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var result = ExecuteCopyLesson(model, currentUser);
                return Ok(ApiResponseFactory.Success(result, "Sao chép bài học thành công"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseFactory.Fail(null, ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponseFactory.Fail(null, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponseFactory.Fail(null, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseFactory.Fail(ex, "Không thể sao chép bài học."));
            }
        }

        [HttpGet("move-course-catalog-preview")]
        public IActionResult GetMoveCourseCatalogPreview(int sourceCatalogId)
        {
            try
            {
                var graph = LoadCopySourceGraph(sourceCatalogId);
                var preview = new MoveCourseCatalogPreview
                {
                    SourceCatalog = new MoveCourseCatalogSource
                    {
                        ID = graph.Catalog.ID,
                        Code = graph.Catalog.Code,
                        Name = graph.Catalog.Name,
                        DepartmentID = graph.Catalog.DepartmentID,
                        CatalogType = graph.Catalog.CatalogType,
                        ProjectTypeIDs = graph.CatalogProjectTypes.Where(x => x.ProjectTypeID.HasValue).Select(x => x.ProjectTypeID!.Value).Distinct().ToList()
                    },
                    Counts = new MoveCourseCatalogCounts
                    {
                        CatalogProjectTypes = graph.CatalogProjectTypes.Count,
                        Courses = graph.Courses.Count,
                        CourseKpiMaps = graph.CourseKpiMaps.Count,
                        Lessons = graph.Lessons.Count,
                        CourseFiles = graph.CourseFiles.Count,
                        Exams = graph.Exams.Count,
                        Questions = graph.Questions.Count,
                        Answers = graph.Answers.Count,
                        RightAnswers = graph.RightAnswers.Count
                    }
                };
                return Ok(ApiResponseFactory.Success(preview, ""));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseFactory.Fail(null, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("move-course-catalog")]
        public IActionResult MoveCourseCatalog([FromBody] MoveCourseCatalogRequest model)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var result = ExecuteMoveCourseCatalog(model);
                return Ok(ApiResponseFactory.Success(result, "Di chuyển danh mục khóa học thành công"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseFactory.Fail(null, ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponseFactory.Fail(null, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponseFactory.Fail(null, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseFactory.Fail(ex, "Không thể di chuyển danh mục khóa học."));
            }
        }

        private MoveCourseCatalogResult ExecuteMoveCourseCatalog(MoveCourseCatalogRequest request)
        {
            NormalizeMoveRequest(request);
            var graph = LoadCopySourceGraph(request.SourceCatalogId);

            // Check if same department and catalog type
            if (graph.Catalog.DepartmentID == request.TargetDepartmentId && graph.Catalog.CatalogType == request.TargetCatalogType)
            {
                throw new InvalidOperationException("Không thể di chuyển vào chính danh mục nguồn.");
            }

            // Update Code with MV- prefix
            var newCode = $"MV-{graph.Catalog.Code}";
            var normalizedCode = newCode.ToUpper();
            
            // Handle duplicate code
            var existingCode = _courseCatalogRepo.GetAll(x => 
                x.ID != request.SourceCatalogId && 
                x.IsDeleted != true && 
                x.Code != null && 
                x.Code.ToUpper() == normalizedCode
            ).FirstOrDefault();

            if (existingCode != null)
            {
                var suffix = 1;
                while (_courseCatalogRepo.GetAll(x => 
                    x.Code != null && 
                    x.Code.ToUpper() == $"{normalizedCode}-{suffix}"
                ).Any())
                {
                    suffix++;
                }
                newCode = $"{normalizedCode}-{suffix}";
            }

            // Update CourseCatalog
            graph.Catalog.Code = newCode;
            graph.Catalog.DepartmentID = request.TargetDepartmentId;
            graph.Catalog.CatalogType = request.TargetCatalogType;
            _courseCatalogRepo.Update(graph.Catalog);

            // Update or Create CourseCatalogProjectTypes
            var newProjectTypeIds = request.ProjectTypeIds.Where(x => x > 0).Distinct().ToList();
            
            // Remove existing project types
            foreach (var existingPT in graph.CatalogProjectTypes)
            {
                existingPT.IsDeleted = true;
                _courseCatalogProjectTypeRepo.Update(existingPT);
            }
            
            // Create new project types
            foreach (var projectTypeId in newProjectTypeIds)
            {
                var newPT = new CourseCatalogProjectType
                {
                    CourseCatalogID = graph.Catalog.ID,
                    ProjectTypeID = projectTypeId,
                    IsDeleted = false
                };
                _courseCatalogProjectTypeRepo.Create(newPT);
            }

            // Update all Courses - CourseCatalogID không đổi vì chỉ có 1 danh mục
            // STT sẽ được giữ nguyên

            // Counts
            return new MoveCourseCatalogResult
            {
                MovedCatalogId = graph.Catalog.ID,
                Counts = new MoveCourseCatalogCounts
                {
                    CatalogProjectTypes = newProjectTypeIds.Count,
                    Courses = graph.Courses.Count,
                    CourseKpiMaps = graph.CourseKpiMaps.Count,
                    Lessons = graph.Lessons.Count,
                    CourseFiles = graph.CourseFiles.Count,
                    Exams = graph.Exams.Count,
                    Questions = graph.Questions.Count,
                    Answers = graph.Answers.Count,
                    RightAnswers = graph.RightAnswers.Count
                }
            };
        }

        private static void NormalizeMoveRequest(MoveCourseCatalogRequest request)
        {
            request.ProjectTypeIds ??= new List<int>();
            if (request.SourceCatalogId <= 0) throw new ArgumentException("Danh mục nguồn không hợp lệ.");
            if (request.TargetDepartmentId <= 0 || request.TargetCatalogType is < 1 or > 2) 
                throw new ArgumentException("Phòng ban hoặc loại danh mục không hợp lệ.");
        }

        [HttpGet("move-course-preview")]
        public IActionResult GetMoveCoursePreview(int sourceCourseId)
        {
            try
            {
                var graph = LoadCopyCourseGraph(sourceCourseId);
                var catalog = graph.CatalogId.HasValue ? _courseCatalogRepo.GetAll(x => x.ID == graph.CatalogId).FirstOrDefault() : null;
                var preview = new MoveCoursePreview
                {
                    SourceCourse = new MoveCourseSource
                    {
                        ID = graph.Course.ID,
                        Code = graph.Course.Code,
                        NameCourse = graph.Course.NameCourse,
                        CourseCatalogID = graph.Course.CourseCatalogID,
                        DepartmentID = catalog?.DepartmentID,
                        CatalogType = catalog?.CatalogType
                    },
                    Counts = new MoveCourseCounts
                    {
                        CourseKpiMaps = graph.CourseKpiMaps.Count,
                        Lessons = graph.Lessons.Count,
                        CourseFiles = graph.CourseFiles.Count,
                        Exams = graph.Exams.Count,
                        Questions = graph.Questions.Count,
                        Answers = graph.Answers.Count,
                        RightAnswers = graph.RightAnswers.Count
                    }
                };
                return Ok(ApiResponseFactory.Success(preview, ""));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseFactory.Fail(null, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("move-course")]
        public IActionResult MoveCourse([FromBody] MoveCourseRequest model)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var result = ExecuteMoveCourse(model);
                return Ok(ApiResponseFactory.Success(result, "Di chuyển khóa học thành công"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseFactory.Fail(null, ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponseFactory.Fail(null, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponseFactory.Fail(null, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseFactory.Fail(ex, "Không thể di chuyển khóa học."));
            }
        }

        private MoveCourseResult ExecuteMoveCourse(MoveCourseRequest request)
        {
            request.ProjectTypeIds ??= new List<int>();
            if (request.SourceCourseId <= 0) throw new ArgumentException("Khóa học nguồn không hợp lệ.");
            if (request.TargetCatalogId <= 0) throw new ArgumentException("Danh mục đích không hợp lệ.");

            var graph = LoadCopyCourseGraph(request.SourceCourseId);

            if (graph.Course.CourseCatalogID == request.TargetCatalogId)
            {
                throw new InvalidOperationException("Không thể di chuyển vào chính danh mục nguồn.");
            }

            var targetCatalog = _courseCatalogRepo.GetAll(x => x.ID == request.TargetCatalogId && x.IsDeleted != true).FirstOrDefault()
                ?? throw new KeyNotFoundException("Danh mục đích không tồn tại.");

            graph.Course.CourseCatalogID = request.TargetCatalogId;
            //graph.Course.DepartmentID = targetCatalog.DepartmentID;
            _courseRepo.Update(graph.Course);

            return new MoveCourseResult
            {
                MovedCourseId = graph.Course.ID,
                Counts = new MoveCourseCounts
                {
                    CourseKpiMaps = graph.CourseKpiMaps.Count,
                    Lessons = graph.Lessons.Count,
                    CourseFiles = graph.CourseFiles.Count,
                    Exams = graph.Exams.Count,
                    Questions = graph.Questions.Count,
                    Answers = graph.Answers.Count,
                    RightAnswers = graph.RightAnswers.Count
                }
            };
        }

        [HttpGet("move-lesson-preview")]
        public IActionResult GetMoveLessonPreview(int sourceLessonId)
        {
            try
            {
                var graph = LoadCopyLessonGraph(sourceLessonId);
                var course = graph.CourseId.HasValue ? _courseRepo.GetAll(x => x.ID == graph.CourseId && x.DeleteFlag == true).FirstOrDefault() : null;
                var catalog = course?.CourseCatalogID.HasValue == true ? _courseCatalogRepo.GetAll(x => x.ID == course.CourseCatalogID).FirstOrDefault() : null;
                var preview = new MoveLessonPreview
                {
                    SourceLesson = new MoveLessonSource
                    {
                        ID = graph.Lesson.ID,
                        Code = graph.Lesson.Code,
                        LessonTitle = graph.Lesson.LessonTitle,
                        CourseID = graph.Lesson.CourseID,
                        DepartmentID = catalog?.DepartmentID,
                        CatalogType = catalog?.CatalogType
                    },
                    Counts = new MoveLessonCounts
                    {
                        CourseFiles = graph.CourseFiles.Count,
                        Exams = graph.Exams.Count,
                        Questions = graph.Questions.Count,
                        Answers = graph.Answers.Count,
                        RightAnswers = graph.RightAnswers.Count
                    }
                };
                return Ok(ApiResponseFactory.Success(preview, ""));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseFactory.Fail(null, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("move-lesson")]
        public IActionResult MoveLesson([FromBody] MoveLessonRequest model)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var result = ExecuteMoveLesson(model);
                return Ok(ApiResponseFactory.Success(result, "Di chuyển bài học thành công"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseFactory.Fail(null, ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponseFactory.Fail(null, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponseFactory.Fail(null, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseFactory.Fail(ex, "Không thể di chuyển bài học."));
            }
        }

        private MoveLessonResult ExecuteMoveLesson(MoveLessonRequest request)
        {
            request.ProjectTypeIds ??= new List<int>();
            if (request.SourceLessonId <= 0) throw new ArgumentException("Bài học nguồn không hợp lệ.");
            if (request.TargetCourseId <= 0) throw new ArgumentException("Khóa học đích không hợp lệ.");

            var graph = LoadCopyLessonGraph(request.SourceLessonId);

            if (graph.Lesson.CourseID == request.TargetCourseId)
            {
                throw new InvalidOperationException("Không thể di chuyển vào chính khóa học nguồn.");
            }

            var targetCourse = _courseRepo.GetAll(x => x.ID == request.TargetCourseId && x.DeleteFlag == true).FirstOrDefault()
                ?? throw new KeyNotFoundException("Khóa học đích không tồn tại.");

            graph.Lesson.CourseID = request.TargetCourseId;
            _courseLessonRepo.Update(graph.Lesson);

            return new MoveLessonResult
            {
                MovedLessonId = graph.Lesson.ID,
                Counts = new MoveLessonCounts
                {
                    CourseFiles = graph.CourseFiles.Count,
                    Exams = graph.Exams.Count,
                    Questions = graph.Questions.Count,
                    Answers = graph.Answers.Count,
                    RightAnswers = graph.RightAnswers.Count
                }
            };
        }

        private CopySourceGraph LoadCopySourceGraph(int sourceCatalogId)
        {
            var catalog = _courseCatalogRepo.GetAll(x => x.ID == sourceCatalogId && x.IsDeleted != true).FirstOrDefault()
                ?? throw new KeyNotFoundException("Danh mục nguồn không tồn tại hoặc đã bị xóa.");
            var catalogProjectTypes = _courseCatalogProjectTypeRepo.GetAll(x => x.CourseCatalogID == sourceCatalogId && x.IsDeleted != true);
            var courses = _courseRepo.GetAll(x => x.CourseCatalogID == sourceCatalogId && x.DeleteFlag == true);
            var courseIds = courses.Select(x => x.ID).ToList();
            var courseKpiMaps = _course_KPIEmployeeTeamMapRepo.GetAll(x => courseIds.Contains(x.CourseID) && x.IsDeleted != true);
            var lessons = _courseLessonRepo.GetAll(x => x.CourseID.HasValue && courseIds.Contains(x.CourseID.Value) && x.IsDeleted != true);
            var lessonIds = lessons.Select(x => x.ID).ToList();
            var courseFiles = _courseFilesRepo.GetAll(x => x.IsDeleted != true &&
                ((x.CourseID.HasValue && courseIds.Contains(x.CourseID.Value)) || (x.LessonID.HasValue && lessonIds.Contains(x.LessonID.Value))));
            var exams = _courseExamRepo.GetAll(x =>
                (x.CourseId.HasValue && courseIds.Contains(x.CourseId.Value)) || (x.LessonID.HasValue && lessonIds.Contains(x.LessonID.Value)));
            var examIds = exams.Select(x => x.ID).ToList();
            var questions = _courseQuestionRepo.GetAll(x => x.CourseExamId.HasValue && examIds.Contains(x.CourseExamId.Value));
            var questionIds = questions.Select(x => x.ID).ToList();
            var answers = _courseAnswerRepo.GetAll(x => x.CourseQuestionId.HasValue && questionIds.Contains(x.CourseQuestionId.Value));
            var answerIds = answers.Select(x => x.ID).ToList();
            var rightAnswers = _courseRightAnswerRepo.GetAll(x =>
                x.CourseQuestionID.HasValue && questionIds.Contains(x.CourseQuestionID.Value) &&
                x.CourseAnswerID.HasValue && answerIds.Contains(x.CourseAnswerID.Value));

            return new CopySourceGraph(catalog, catalogProjectTypes, courses, courseKpiMaps, lessons, courseFiles, exams, questions, answers, rightAnswers);
        }

        private CopyCourseCatalogResult CopyCourseCatalog(CopyCourseCatalogRequest request, CurrentUser currentUser)
        {
            NormalizeCopyRequest(request);
            var graph = LoadCopySourceGraph(request.SourceCatalogId);
            var normalizedCode = request.NewCode.ToUpper();
            if (_courseCatalogRepo.GetAll(x => x.ID != request.SourceCatalogId && x.IsDeleted != true && x.Code != null && x.Code.ToUpper() == normalizedCode).Any())
                throw new InvalidOperationException("Mã danh mục đã tồn tại! Vui lòng kiểm tra lại.");

            var nextCatalogOrder = (_courseCatalogRepo.GetAll(x => x.IsDeleted != true && x.DepartmentID == request.DepartmentId && x.CatalogType == request.CatalogType).Max(x => x.STT) ?? 0) + 1;
            var newCatalog = new CourseCatalog
            {
                Code = request.NewCode, Name = request.NewName, DepartmentID = request.DepartmentId,
                CatalogType = request.CatalogType, STT = nextCatalogOrder, IsDeleted = false, DeleteFlag = true
            };
            _courseCatalogRepo.Create(newCatalog);

            var projectTypeIds = request.ProjectTypeIds.Where(x => x > 0).Distinct().ToList();
            var newProjectTypes = projectTypeIds.Select(projectTypeId => new CourseCatalogProjectType
            {
                CourseCatalogID = newCatalog.ID, ProjectTypeID = projectTypeId, IsDeleted = false
            }).ToList();
            if (newProjectTypes.Count > 0) _courseCatalogProjectTypeRepo.CreateRange(newProjectTypes);

            var usedCourseCodes = new HashSet<string>(_courseRepo.GetAll(x => x.Code != null).Select(x => x.Code!), StringComparer.OrdinalIgnoreCase);
            var courseMap = new Dictionary<int, Course>();
            foreach (var sourceCourse in graph.Courses.OrderBy(x => x.STT).ThenBy(x => x.ID))
            {
                var newCourse = new Course
                {
                    STT = sourceCourse.STT, Code = GenerateCopyCode(sourceCourse.Code, request.NewCode, usedCourseCodes),
                    NameCourse = sourceCourse.NameCourse, Instructor = sourceCourse.Instructor, CourseCatalogID = newCatalog.ID,
                    FileCourseID = sourceCourse.FileCourseID, IsPractice = sourceCourse.IsPractice, QuestionCount = sourceCourse.QuestionCount,
                    QuestionDuration = sourceCourse.QuestionDuration, LeadTime = sourceCourse.LeadTime, CourseCopyID = sourceCourse.ID,
                    CourseTypeID = sourceCourse.CourseTypeID, EmployeeID = sourceCourse.EmployeeID, DeleteFlag = true
                };
                _courseRepo.Create(newCourse);
                courseMap[sourceCourse.ID] = newCourse;
            }

            var newKpiMaps = graph.CourseKpiMaps.Where(x => courseMap.ContainsKey(x.CourseID))
                .GroupBy(x => new { x.CourseID, x.KPIEmployeeTeamID }).Select(x => x.First())
                .Select(x => new CourseKPIEmployeeTeamMap { CourseID = courseMap[x.CourseID].ID, KPIEmployeeTeamID = x.KPIEmployeeTeamID, IsDeleted = false }).ToList();
            if (newKpiMaps.Count > 0) _course_KPIEmployeeTeamMapRepo.CreateRange(newKpiMaps);

            var usedLessonCodes = new HashSet<string>(_courseLessonRepo.GetAll(x => x.Code != null).Select(x => x.Code!), StringComparer.OrdinalIgnoreCase);
            var lessonMap = new Dictionary<int, CourseLesson>();
            foreach (var sourceLesson in graph.Lessons.OrderBy(x => x.STT).ThenBy(x => x.ID))
            {
                if (!sourceLesson.CourseID.HasValue || !courseMap.TryGetValue(sourceLesson.CourseID.Value, out var newParentCourse))
                    throw new InvalidOperationException($"Bài học {sourceLesson.ID} không thuộc graph danh mục nguồn.");
                var newLesson = new CourseLesson
                {
                    Code = GenerateCopyCode(sourceLesson.Code, request.NewCode, usedLessonCodes), LessonTitle = sourceLesson.LessonTitle,
                    LessonContent = sourceLesson.LessonContent, Duration = sourceLesson.Duration, VideoURL = sourceLesson.VideoURL,
                    STT = sourceLesson.STT, CourseID = newParentCourse.ID, FileCourseID = sourceLesson.FileCourseID, UrlPDF = sourceLesson.UrlPDF,
                    LessonCopyID = sourceLesson.ID, IsDeleted = false, EmployeeID = sourceLesson.EmployeeID,
                    RequiredWatchedPercent = sourceLesson.RequiredWatchedPercent, Chapters = sourceLesson.Chapters
                };
                _courseLessonRepo.Create(newLesson);
                lessonMap[sourceLesson.ID] = newLesson;
            }

            var newFiles = new List<CourseFile>();
            foreach (var sourceFile in graph.CourseFiles)
            {
                int? newCourseId = sourceFile.CourseID.HasValue && courseMap.TryGetValue(sourceFile.CourseID.Value, out var mappedCourse) ? mappedCourse.ID : null;
                int? newLessonId = sourceFile.LessonID.HasValue && lessonMap.TryGetValue(sourceFile.LessonID.Value, out var mappedLesson) ? mappedLesson.ID : null;
                if (!newCourseId.HasValue && !newLessonId.HasValue) throw new InvalidOperationException($"File khóa học {sourceFile.ID} không thuộc graph danh mục nguồn.");
                newFiles.Add(new CourseFile { NameFile = sourceFile.NameFile, CourseID = newCourseId, LessonID = newLessonId, OriginPath = sourceFile.OriginPath, ServerPath = sourceFile.ServerPath, IsDeleted = false });
            }
            if (newFiles.Count > 0) _courseFilesRepo.CreateRange(newFiles);

            var examMap = new Dictionary<int, CourseExam>();
            foreach (var sourceExam in graph.Exams.OrderBy(x => x.ID))
            {
                int? newCourseId;
                int? newLessonId;
                if (sourceExam.LessonID > 0)
                {
                    if (!lessonMap.TryGetValue(sourceExam.LessonID.Value, out var mappedLesson)) throw new InvalidOperationException($"Đề thi {sourceExam.ID} trỏ tới bài học ngoài graph danh mục nguồn.");
                    newLessonId = mappedLesson.ID;
                    newCourseId = sourceExam.CourseId > 0 && courseMap.TryGetValue(sourceExam.CourseId.Value, out var lessonExamCourse) ? lessonExamCourse.ID : sourceExam.CourseId;
                }
                else if (sourceExam.CourseId > 0 && courseMap.TryGetValue(sourceExam.CourseId.Value, out var mappedCourse))
                {
                    newCourseId = mappedCourse.ID; newLessonId = sourceExam.LessonID;
                }
                else throw new InvalidOperationException($"Đề thi {sourceExam.ID} không thuộc graph danh mục nguồn.");
                var newExam = new CourseExam { CourseId = newCourseId, LessonID = newLessonId, NameExam = sourceExam.NameExam, CodeExam = sourceExam.CodeExam, Goal = sourceExam.Goal, TestTime = sourceExam.TestTime, ExamType = sourceExam.ExamType };
                _courseExamRepo.Create(newExam);
                examMap[sourceExam.ID] = newExam;
            }

            var questionMap = new Dictionary<int, CourseQuestion>();
            foreach (var sourceQuestion in graph.Questions.OrderBy(x => x.ID))
            {
                if (!sourceQuestion.CourseExamId.HasValue || !examMap.TryGetValue(sourceQuestion.CourseExamId.Value, out var newExam)) throw new InvalidOperationException($"Câu hỏi {sourceQuestion.ID} không thuộc graph danh mục nguồn.");
                var newQuestion = new CourseQuestion { QuestionText = sourceQuestion.QuestionText, STT = sourceQuestion.STT, CourseExamId = newExam.ID, CheckInput = sourceQuestion.CheckInput, Marks = sourceQuestion.Marks, Image = sourceQuestion.Image };
                _courseQuestionRepo.Create(newQuestion);
                questionMap[sourceQuestion.ID] = newQuestion;
            }

            var answerMap = new Dictionary<int, CourseAnswer>();
            foreach (var sourceAnswer in graph.Answers.OrderBy(x => x.ID))
            {
                if (!sourceAnswer.CourseQuestionId.HasValue || !questionMap.TryGetValue(sourceAnswer.CourseQuestionId.Value, out var newQuestion)) throw new InvalidOperationException($"Đáp án {sourceAnswer.ID} không thuộc graph danh mục nguồn.");
                var newAnswer = new CourseAnswer { AnswerText = sourceAnswer.AnswerText, CourseQuestionId = newQuestion.ID, AnswerNumber = sourceAnswer.AnswerNumber };
                _courseAnswerRepo.Create(newAnswer);
                answerMap[sourceAnswer.ID] = newAnswer;
            }

            var newRightAnswers = new List<CourseRightAnswer>();
            foreach (var sourceRightAnswer in graph.RightAnswers)
            {
                if (!sourceRightAnswer.CourseQuestionID.HasValue || !sourceRightAnswer.CourseAnswerID.HasValue || !questionMap.TryGetValue(sourceRightAnswer.CourseQuestionID.Value, out var newQuestion) || !answerMap.TryGetValue(sourceRightAnswer.CourseAnswerID.Value, out var newAnswer)) throw new InvalidOperationException($"Đáp án đúng {sourceRightAnswer.ID} có liên kết nguồn không hợp lệ.");
                newRightAnswers.Add(new CourseRightAnswer { CourseQuestionID = newQuestion.ID, CourseAnswerID = newAnswer.ID });
            }
            if (newRightAnswers.Count > 0) _courseRightAnswerRepo.CreateRange(newRightAnswers);

            return new CopyCourseCatalogResult
            {
                NewCatalogId = newCatalog.ID,
                Counts = new CopyCourseCatalogCounts { CatalogProjectTypes = newProjectTypes.Count, Courses = courseMap.Count, CourseKpiMaps = newKpiMaps.Count, Lessons = lessonMap.Count, CourseFiles = newFiles.Count, Exams = examMap.Count, Questions = questionMap.Count, Answers = answerMap.Count, RightAnswers = newRightAnswers.Count }
            };
        }

        private static void NormalizeCopyRequest(CopyCourseCatalogRequest request)
        {
            request.NewCode = request.NewCode.Trim();
            request.NewName = request.NewName.Trim();
            request.ProjectTypeIds ??= new List<int>();
            if (request.SourceCatalogId <= 0) throw new ArgumentException("Danh mục nguồn không hợp lệ.");
            if (string.IsNullOrWhiteSpace(request.NewCode) || request.NewCode.Length > 50) throw new ArgumentException("Mã danh mục mới phải có từ 1 đến 50 ký tự.");
            if (string.IsNullOrWhiteSpace(request.NewName) || request.NewName.Length > 100) throw new ArgumentException("Tên danh mục mới phải có từ 1 đến 100 ký tự.");
            if (request.DepartmentId <= 0 || request.CatalogType is < 1 or > 2) throw new ArgumentException("Phòng ban hoặc loại danh mục không hợp lệ.");
        }

        private static string GenerateCopyCode(string? sourceCode, string catalogCode, ISet<string> usedCodes)
        {
            var baseCode = FitCopyCode($"{(string.IsNullOrWhiteSpace(sourceCode) ? "COPY" : sourceCode.Trim())}-{catalogCode.Trim()}", 20);
            var candidate = baseCode;
            var sequence = 2;
            while (!usedCodes.Add(candidate))
            {
                var suffix = $"-{sequence++}";
                candidate = FitCopyCode(baseCode, 20 - suffix.Length) + suffix;
            }
            return candidate;
        }

        private static string FitCopyCode(string value, int maxLength) => value.Length <= maxLength ? value : value[..maxLength];

        private static CopyCourseCatalogCounts BuildCopyCounts(CopySourceGraph graph) => new()
        {
            CatalogProjectTypes = graph.CatalogProjectTypes.Count, Courses = graph.Courses.Count, CourseKpiMaps = graph.CourseKpiMaps.Count,
            Lessons = graph.Lessons.Count, CourseFiles = graph.CourseFiles.Count, Exams = graph.Exams.Count,
            Questions = graph.Questions.Count, Answers = graph.Answers.Count, RightAnswers = graph.RightAnswers.Count
        };

        private sealed record CopySourceGraph(CourseCatalog Catalog, List<CourseCatalogProjectType> CatalogProjectTypes, List<Course> Courses, List<CourseKPIEmployeeTeamMap> CourseKpiMaps, List<CourseLesson> Lessons, List<CourseFile> CourseFiles, List<CourseExam> Exams, List<CourseQuestion> Questions, List<CourseAnswer> Answers, List<CourseRightAnswer> RightAnswers);
        private sealed record CopyCourseGraph(Course Course, int? CatalogId, string? CatalogName, int? CatalogType, List<CourseLesson> Lessons, List<CourseFile> CourseFiles, List<CourseExam> Exams, List<CourseQuestion> Questions, List<CourseAnswer> Answers, List<CourseRightAnswer> RightAnswers, List<CourseKPIEmployeeTeamMap> CourseKpiMaps);
        private sealed record CopyLessonGraph(CourseLesson Lesson, int? CourseId, string? CourseName, List<CourseFile> CourseFiles, List<CourseExam> Exams, List<CourseQuestion> Questions, List<CourseAnswer> Answers, List<CourseRightAnswer> RightAnswers);

        private CopyCourseGraph LoadCopyCourseGraph(int sourceCourseId)
        {
            var course = _courseRepo.GetAll(x => x.ID == sourceCourseId && x.DeleteFlag == true).FirstOrDefault()
                ?? throw new KeyNotFoundException("Khóa học nguồn không tồn tại hoặc đã bị xóa.");

            var catalog = _courseCatalogRepo.GetAll(x => x.ID == course.CourseCatalogID && x.IsDeleted != true).FirstOrDefault();

            var lessons = _courseLessonRepo.GetAll(x => x.CourseID == sourceCourseId && x.IsDeleted != true);
            var lessonIds = lessons.Select(x => x.ID).ToList();

            var courseFiles = _courseFilesRepo.GetAll(x => x.IsDeleted != true &&
                ((x.CourseID.HasValue && x.CourseID.Value == sourceCourseId) || (x.LessonID.HasValue && lessonIds.Contains(x.LessonID.Value))));

            var exams = _courseExamRepo.GetAll(x =>
                (x.CourseId.HasValue && x.CourseId.Value == sourceCourseId) || (x.LessonID.HasValue && lessonIds.Contains(x.LessonID.Value)));
            var examIds = exams.Select(x => x.ID).ToList();

            var questions = _courseQuestionRepo.GetAll(x => x.CourseExamId.HasValue && examIds.Contains(x.CourseExamId.Value));
            var questionIds = questions.Select(x => x.ID).ToList();

            var answers = _courseAnswerRepo.GetAll(x => x.CourseQuestionId.HasValue && questionIds.Contains(x.CourseQuestionId.Value));
            var answerIds = answers.Select(x => x.ID).ToList();

            var rightAnswers = _courseRightAnswerRepo.GetAll(x =>
                x.CourseQuestionID.HasValue && questionIds.Contains(x.CourseQuestionID.Value) &&
                x.CourseAnswerID.HasValue && answerIds.Contains(x.CourseAnswerID.Value));

            var courseKpiMaps = _course_KPIEmployeeTeamMapRepo.GetAll(x => x.CourseID == sourceCourseId && x.IsDeleted != true);

            return new CopyCourseGraph(course, catalog?.ID, catalog?.Name, catalog?.CatalogType, lessons, courseFiles, exams, questions, answers, rightAnswers, courseKpiMaps);
        }

        private CopyLessonGraph LoadCopyLessonGraph(int sourceLessonId)
        {
            var lesson = _courseLessonRepo.GetAll(x => x.ID == sourceLessonId && x.IsDeleted != true).FirstOrDefault()
                ?? throw new KeyNotFoundException("Bài học nguồn không tồn tại hoặc đã bị xóa.");

            var course = _courseRepo.GetAll(x => x.ID == lesson.CourseID && x.DeleteFlag == true).FirstOrDefault();

            var courseFiles = _courseFilesRepo.GetAll(x => x.IsDeleted != true && x.LessonID == sourceLessonId);

            var exams = _courseExamRepo.GetAll(x => x.LessonID.HasValue && x.LessonID.Value == sourceLessonId);
            var examIds = exams.Select(x => x.ID).ToList();

            var questions = _courseQuestionRepo.GetAll(x => x.CourseExamId.HasValue && examIds.Contains(x.CourseExamId.Value));
            var questionIds = questions.Select(x => x.ID).ToList();

            var answers = _courseAnswerRepo.GetAll(x => x.CourseQuestionId.HasValue && questionIds.Contains(x.CourseQuestionId.Value));
            var answerIds = answers.Select(x => x.ID).ToList();

            var rightAnswers = _courseRightAnswerRepo.GetAll(x =>
                x.CourseQuestionID.HasValue && questionIds.Contains(x.CourseQuestionID.Value) &&
                x.CourseAnswerID.HasValue && answerIds.Contains(x.CourseAnswerID.Value));

            return new CopyLessonGraph(lesson, course?.ID, course?.NameCourse, courseFiles, exams, questions, answers, rightAnswers);
        }

        private CopyCourseResult ExecuteCopyCourse(CopyCourseRequest request, CurrentUser currentUser)
        {
            if (request.SourceCourseId <= 0) throw new ArgumentException("Khóa học nguồn không hợp lệ.");
            if (string.IsNullOrWhiteSpace(request.NewCode)) throw new ArgumentException("Mã khóa học mới không được trống.");
            if (request.TargetCatalogId <= 0) throw new ArgumentException("Danh mục đích không hợp lệ.");

            var targetCatalog = _courseCatalogRepo.GetAll(x => x.ID == request.TargetCatalogId && x.IsDeleted != true).FirstOrDefault()
                ?? throw new KeyNotFoundException("Danh mục đích không tồn tại hoặc đã bị xóa.");

            var graph = LoadCopyCourseGraph(request.SourceCourseId);
            var normalizedCode = request.NewCode.ToUpper();
            if (_courseRepo.GetAll(x => x.ID != request.SourceCourseId && x.DeleteFlag == true && x.Code != null && x.Code.ToUpper() == normalizedCode).Any())
                throw new InvalidOperationException("Mã khóa học đã tồn tại! Vui lòng kiểm tra lại.");

            var nextCourseOrder = (_courseRepo.GetAll(x => x.CourseCatalogID == request.TargetCatalogId && x.DeleteFlag == true).Max(x => x.STT) ?? 0) + 1;
            var newCourse = new Course
            {
                STT = nextCourseOrder,
                Code = request.NewCode,
                NameCourse = request.NewName,
                CourseCatalogID = request.TargetCatalogId,
                CourseCopyID = request.SourceCourseId,
                Instructor = graph.Course.Instructor,
                FileCourseID = graph.Course.FileCourseID,
                IsPractice = graph.Course.IsPractice,
                QuestionCount = graph.Course.QuestionCount,
                QuestionDuration = graph.Course.QuestionDuration,
                LeadTime = graph.Course.LeadTime,
                CourseTypeID = graph.Course.CourseTypeID,
                EmployeeID = graph.Course.EmployeeID,
                DeleteFlag = true
            };
            _courseRepo.Create(newCourse);

            var usedLessonCodes = new HashSet<string>(_courseLessonRepo.GetAll(x => x.Code != null).Select(x => x.Code!), StringComparer.OrdinalIgnoreCase);
            var lessonMap = new Dictionary<int, CourseLesson>();
            foreach (var sourceLesson in graph.Lessons.OrderBy(x => x.STT).ThenBy(x => x.ID))
            {
                var newLesson = new CourseLesson
                {
                    Code = GenerateCopyCode(sourceLesson.Code, request.NewCode, usedLessonCodes),
                    LessonTitle = sourceLesson.LessonTitle,
                    LessonContent = sourceLesson.LessonContent,
                    Duration = sourceLesson.Duration,
                    VideoURL = sourceLesson.VideoURL,
                    STT = sourceLesson.STT,
                    CourseID = newCourse.ID,
                    FileCourseID = sourceLesson.FileCourseID,
                    UrlPDF = sourceLesson.UrlPDF,
                    LessonCopyID = sourceLesson.ID,
                    IsDeleted = false,
                    EmployeeID = sourceLesson.EmployeeID,
                    RequiredWatchedPercent = sourceLesson.RequiredWatchedPercent,
                    Chapters = sourceLesson.Chapters
                };
                _courseLessonRepo.Create(newLesson);
                lessonMap[sourceLesson.ID] = newLesson;
            }

            var newFiles = new List<CourseFile>();
            foreach (var sourceFile in graph.CourseFiles)
            {
                int? newCourseId = sourceFile.CourseID.HasValue && sourceFile.CourseID.Value == request.SourceCourseId ? newCourse.ID : null;
                int? newLessonId = sourceFile.LessonID.HasValue && lessonMap.TryGetValue(sourceFile.LessonID.Value, out var mappedLesson) ? mappedLesson.ID : null;
                if (!newCourseId.HasValue && !newLessonId.HasValue) continue;
                newFiles.Add(new CourseFile { NameFile = sourceFile.NameFile, CourseID = newCourseId, LessonID = newLessonId, OriginPath = sourceFile.OriginPath, ServerPath = sourceFile.ServerPath, IsDeleted = false });
            }
            if (newFiles.Count > 0) _courseFilesRepo.CreateRange(newFiles);

            var examMap = new Dictionary<int, CourseExam>();
            foreach (var sourceExam in graph.Exams.OrderBy(x => x.ID))
            {
                int? newCourseId;
                int? newLessonId;
                if (sourceExam.LessonID > 0)
                {
                    if (!lessonMap.TryGetValue(sourceExam.LessonID.Value, out var mappedLesson)) continue;
                    newLessonId = mappedLesson.ID;
                    newCourseId = sourceExam.CourseId > 0 ? newCourse.ID : null;
                }
                else if (sourceExam.CourseId > 0 && sourceExam.CourseId.Value == request.SourceCourseId)
                {
                    newCourseId = newCourse.ID;
                    newLessonId = sourceExam.LessonID;
                }
                else continue;

                var newExam = new CourseExam { CourseId = newCourseId, LessonID = newLessonId, NameExam = sourceExam.NameExam, CodeExam = sourceExam.CodeExam, Goal = sourceExam.Goal, TestTime = sourceExam.TestTime, ExamType = sourceExam.ExamType };
                _courseExamRepo.Create(newExam);
                examMap[sourceExam.ID] = newExam;
            }

            var questionMap = new Dictionary<int, CourseQuestion>();
            foreach (var sourceQuestion in graph.Questions.OrderBy(x => x.ID))
            {
                if (!sourceQuestion.CourseExamId.HasValue || !examMap.TryGetValue(sourceQuestion.CourseExamId.Value, out var newExam)) continue;
                var newQuestion = new CourseQuestion { QuestionText = sourceQuestion.QuestionText, STT = sourceQuestion.STT, CourseExamId = newExam.ID, CheckInput = sourceQuestion.CheckInput, Marks = sourceQuestion.Marks, Image = sourceQuestion.Image };
                _courseQuestionRepo.Create(newQuestion);
                questionMap[sourceQuestion.ID] = newQuestion;
            }

            var answerMap = new Dictionary<int, CourseAnswer>();
            foreach (var sourceAnswer in graph.Answers.OrderBy(x => x.ID))
            {
                if (!sourceAnswer.CourseQuestionId.HasValue || !questionMap.TryGetValue(sourceAnswer.CourseQuestionId.Value, out var newQuestion)) continue;
                var newAnswer = new CourseAnswer { CourseQuestionId = newQuestion.ID, AnswerText = sourceAnswer.AnswerText, AnswerNumber = sourceAnswer.AnswerNumber };
                _courseAnswerRepo.Create(newAnswer);
                answerMap[sourceAnswer.ID] = newAnswer;
            }

            foreach (var sourceRightAnswer in graph.RightAnswers)
            {
                if (!sourceRightAnswer.CourseQuestionID.HasValue || !questionMap.TryGetValue(sourceRightAnswer.CourseQuestionID.Value, out var newQuestion)) continue;
                if (!sourceRightAnswer.CourseAnswerID.HasValue || !answerMap.TryGetValue(sourceRightAnswer.CourseAnswerID.Value, out var newAnswer)) continue;
                var newRightAnswer = new CourseRightAnswer { CourseQuestionID = newQuestion.ID, CourseAnswerID = newAnswer.ID };
                _courseRightAnswerRepo.Create(newRightAnswer);
            }

            return new CopyCourseResult
            {
                NewCourseId = newCourse.ID,
                Counts = new CopyCourseCounts
                {
                    Lessons = lessonMap.Count,
                    CourseFiles = newFiles.Count,
                    Exams = examMap.Count,
                    Questions = questionMap.Count,
                    Answers = answerMap.Count,
                    RightAnswers = graph.RightAnswers.Count
                }
            };
        }

        private CopyLessonResult ExecuteCopyLesson(CopyLessonRequest request, CurrentUser currentUser)
        {
            if (request.SourceLessonId <= 0) throw new ArgumentException("Bài học nguồn không hợp lệ.");
            if (string.IsNullOrWhiteSpace(request.NewCode)) throw new ArgumentException("Mã bài học mới không được trống.");
            if (request.TargetCourseId <= 0) throw new ArgumentException("Khóa học đích không hợp lệ.");

            var targetCourse = _courseRepo.GetAll(x => x.ID == request.TargetCourseId && x.DeleteFlag == true).FirstOrDefault()
                ?? throw new KeyNotFoundException("Khóa học đích không tồn tại hoặc đã bị xóa.");

            var graph = LoadCopyLessonGraph(request.SourceLessonId);
            var normalizedCode = request.NewCode.ToUpper();
            if (_courseLessonRepo.GetAll(x => x.ID != request.SourceLessonId && x.IsDeleted != true && x.Code != null && x.Code.ToUpper() == normalizedCode).Any())
                throw new InvalidOperationException("Mã bài học đã tồn tại! Vui lòng kiểm tra lại.");

            var nextLessonOrder = (_courseLessonRepo.GetAll(x => x.CourseID == request.TargetCourseId && x.IsDeleted != true).Max(x => x.STT) ?? 0) + 1;
            var newLesson = new CourseLesson
            {
                STT = nextLessonOrder,
                Code = request.NewCode,
                LessonTitle = request.NewName,
                LessonContent = graph.Lesson.LessonContent,
                Duration = graph.Lesson.Duration,
                VideoURL = graph.Lesson.VideoURL,
                CourseID = request.TargetCourseId,
                FileCourseID = graph.Lesson.FileCourseID,
                UrlPDF = graph.Lesson.UrlPDF,
                LessonCopyID = request.SourceLessonId,
                IsDeleted = false,
                EmployeeID = graph.Lesson.EmployeeID,
                RequiredWatchedPercent = graph.Lesson.RequiredWatchedPercent,
                Chapters = graph.Lesson.Chapters
            };
            _courseLessonRepo.Create(newLesson);

            var newFiles = graph.CourseFiles.Select(sourceFile => new CourseFile
            {
                NameFile = sourceFile.NameFile,
                LessonID = newLesson.ID,
                OriginPath = sourceFile.OriginPath,
                ServerPath = sourceFile.ServerPath,
                IsDeleted = false
            }).ToList();
            if (newFiles.Count > 0) _courseFilesRepo.CreateRange(newFiles);

            var examMap = new Dictionary<int, CourseExam>();
            foreach (var sourceExam in graph.Exams)
            {
                var newExam = new CourseExam
                {
                    CourseId = request.TargetCourseId,
                    LessonID = newLesson.ID,
                    NameExam = sourceExam.NameExam,
                    CodeExam = sourceExam.CodeExam,
                    Goal = sourceExam.Goal,
                    TestTime = sourceExam.TestTime,
                    ExamType = sourceExam.ExamType
                };
                _courseExamRepo.Create(newExam);
                examMap[sourceExam.ID] = newExam;
            }

            var questionMap = new Dictionary<int, CourseQuestion>();
            foreach (var sourceQuestion in graph.Questions.OrderBy(x => x.ID))
            {
                if (!sourceQuestion.CourseExamId.HasValue || !examMap.TryGetValue(sourceQuestion.CourseExamId.Value, out var newExam)) continue;
                var newQuestion = new CourseQuestion { QuestionText = sourceQuestion.QuestionText, STT = sourceQuestion.STT, CourseExamId = newExam.ID, CheckInput = sourceQuestion.CheckInput, Marks = sourceQuestion.Marks, Image = sourceQuestion.Image };
                _courseQuestionRepo.Create(newQuestion);
                questionMap[sourceQuestion.ID] = newQuestion;
            }

            var answerMap = new Dictionary<int, CourseAnswer>();
            foreach (var sourceAnswer in graph.Answers.OrderBy(x => x.ID))
            {
                if (!sourceAnswer.CourseQuestionId.HasValue || !questionMap.TryGetValue(sourceAnswer.CourseQuestionId.Value, out var newQuestion)) continue;
                var newAnswer = new CourseAnswer { CourseQuestionId = newQuestion.ID, AnswerText = sourceAnswer.AnswerText, AnswerNumber = sourceAnswer.AnswerNumber };
                _courseAnswerRepo.Create(newAnswer);
                answerMap[sourceAnswer.ID] = newAnswer;
            }

            foreach (var sourceRightAnswer in graph.RightAnswers)
            {
                if (!sourceRightAnswer.CourseQuestionID.HasValue || !questionMap.TryGetValue(sourceRightAnswer.CourseQuestionID.Value, out var newQuestion)) continue;
                if (!sourceRightAnswer.CourseAnswerID.HasValue || !answerMap.TryGetValue(sourceRightAnswer.CourseAnswerID.Value, out var newAnswer)) continue;
                var newRightAnswer = new CourseRightAnswer { CourseQuestionID = newQuestion.ID, CourseAnswerID = newAnswer.ID };
                _courseRightAnswerRepo.Create(newRightAnswer);
            }

            return new CopyLessonResult
            {
                NewLessonId = newLesson.ID,
                Counts = new CopyLessonCounts
                {
                    Files = newFiles.Count,
                    Exams = examMap.Count,
                    Questions = questionMap.Count,
                    Answers = answerMap.Count,
                    RightAnswers = graph.RightAnswers.Count
                }
            };
        }

        // Thêm sửa xóa danh mục khóa học
        [HttpPost("save-data-category")]
        public async Task<IActionResult> SaveDataCategory([FromBody] SaveCoureCategoryParam model)
        {
            try
            {
                var exitCategory = _courseCatalogRepo.GetAll(x => (x.Code.ToUpper().Trim() == model.Code.ToUpper().Trim()) && x.ID != model.ID && (!x.IsDeleted ?? true)).FirstOrDefault();

                if (exitCategory != null && exitCategory.ID > 0)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Mã danh mục đã tồn tại! Vui lòng kiểm tra lại."));
                }

                // Check trùng STT cho cùng DepartmentID + CatalogType
                var exitSTT = _courseCatalogRepo.GetAll(x =>
                    x.STT == model.STT &&
                    x.DepartmentID == model.DepartmentID &&
                    x.ID != model.ID &&
                    (!x.IsDeleted ?? true)
                ).FirstOrDefault();

                if (exitSTT != null && exitSTT.ID > 0)
                {
                    return Ok(ApiResponseFactory.Fail(null, $"STT {model.STT} đã tồn tại cho phòng ban này! Vui lòng nhập STT khác."));
                }

                if (model.ID <= 0)
                {
                    var courseCatalog = new CourseCatalog
                    {
                        ID = 0,
                        Name = model.Name,
                        CatalogType = model.CatalogType,
                        DepartmentID = model.DepartmentID,
                        STT = model.STT,
                        Code = model.Code,
                    };

                    await _courseCatalogRepo.CreateAsync(courseCatalog);
                    if (courseCatalog.ID > 0)
                    {
                        courseCatalog.DeleteFlag = true;
                        await _courseCatalogRepo.UpdateAsync(courseCatalog);
                    }

                    if (model.ProjectTypeIDs.Count > 0)
                    {
                        foreach (var item in model.ProjectTypeIDs)
                        {
                            var newProjectType = new CourseCatalogProjectType
                            {
                                CourseCatalogID = courseCatalog.ID,
                                ProjectTypeID = item
                            };
                            await _courseCatalogProjectTypeRepo.CreateAsync(newProjectType);
                        }
                    }
                }
                else
                {
                    var dept = _departmentRepo.GetAll(x => x.ID == model.DepartmentID).FirstOrDefault();
                    int departmentID = 0;
                    if (dept != null)
                    {
                        departmentID = dept.ID;
                    }
                    var courseCatalog = _courseCatalogRepo.GetByID(model.ID);
                    courseCatalog.Name = model.Name;
                    courseCatalog.CatalogType = model.CatalogType;
                    courseCatalog.DepartmentID = departmentID;
                    courseCatalog.STT = model.STT;
                    courseCatalog.IsDeleted = model.IsDeleted;
                    courseCatalog.DeleteFlag = model.DeleteFlag;
                    courseCatalog.Code = model.Code;
                    await _courseCatalogRepo.UpdateAsync(courseCatalog);

                    var existingProjectTypes = _courseCatalogProjectTypeRepo.GetAll(x => x.CourseCatalogID == model.ID && (x.IsDeleted == false)).ToList();
                    if (existingProjectTypes.Count > 0 && !(existingProjectTypes.IsNullOrEmpty()))
                    {
                        if (model.ProjectTypeIDs.Count <= 0 || model.ProjectTypeIDs.IsNullOrEmpty())
                        {
                            foreach (var item in existingProjectTypes)
                            {
                                await _courseCatalogProjectTypeRepo.DeleteAsync(item.ID);
                            }
                        }
                        else
                        {
                            var existingIds = existingProjectTypes
                                .Where(x => x.ProjectTypeID.HasValue)
                                .Select(x => x.ProjectTypeID.Value)
                                .ToList();

                            var idsToRemove = existingIds
                                .Except(model.ProjectTypeIDs)
                                .ToList();

                            var idsToAdd = model.ProjectTypeIDs
                                .Except(existingIds)
                                .ToList();

                            foreach (var projectTypeId in idsToRemove)
                            {
                                var entity = existingProjectTypes
                                    .FirstOrDefault(x => x.ProjectTypeID == projectTypeId);
                                if (entity != null && entity.ID > 0)
                                {
                                    entity.IsDeleted = true;
                                    await _courseCatalogProjectTypeRepo.UpdateAsync(entity);
                                }
                            }

                            // INSERT
                            foreach (var projectTypeId in idsToAdd)
                            {
                                var exitingEntity = _courseCatalogProjectTypeRepo.GetByID(projectTypeId);

                                if (exitingEntity != null && exitingEntity.ID > 0)
                                {
                                    exitingEntity.IsDeleted = false;
                                    await _courseCatalogProjectTypeRepo.UpdateAsync(exitingEntity);
                                    continue;
                                }
                                var newEntity = new CourseCatalogProjectType
                                {
                                    CourseCatalogID = courseCatalog.ID,
                                    ProjectTypeID = projectTypeId
                                };
                                await _courseCatalogProjectTypeRepo.CreateAsync(newEntity);
                            }
                        }
                    }
                    else
                    {
                        if (model.ProjectTypeIDs.Count > 0)
                        {
                            foreach (var item in model.ProjectTypeIDs)
                            {
                                var entity = _courseCatalogProjectTypeRepo.GetByID(item);
                                if (entity != null && entity.ID > 0)
                                {
                                    entity.IsDeleted = true;
                                    await _courseCatalogProjectTypeRepo.UpdateAsync(entity);
                                }
                                else
                                {
                                    var newEntity = new CourseCatalogProjectType
                                    {
                                        CourseCatalogID = courseCatalog.ID,
                                        ProjectTypeID = item
                                    };
                                    await _courseCatalogProjectTypeRepo.CreateAsync(newEntity);
                                }
                            }
                        }
                    }
                }
                return Ok(ApiResponseFactory.Success(model, "Lưu danh mục khóa học thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
                        if (model.IdeaIDs != null && model.IdeaIDs.Count > 0)
                        {
                            foreach (var id in model.IdeaIDs)
                            {
                                var entity = _registerIdeaRepo.GetByID(id);
                                if (entity != null || entity.ID > 0)
                                {
                                    entity.CourseID = courseNew.ID;
                                    await _registerIdeaRepo.UpdateAsync(entity);
                                }
                            }
                        }
                        // thêm kpi
                        var kpiIds = model.KPIIDs ?? new List<int>();

                        foreach (var kpiId in kpiIds)
                        {
                            var newCourseKpi = new CourseKPIEmployeeTeamMap
                            {
                                CourseID = courseNew.ID,
                                KPIEmployeeTeamID = kpiId,
                                IsDeleted = false
                            };
                            await _course_KPIEmployeeTeamMapRepo.CreateAsync(newCourseKpi);
                        }
                    }
                }
                else
                {
                    //var courseUpdate = _courseRepo.GetByID(model.ID);
                    //courseUpdate.STT = model.STT;
                    //courseUpdate.Code = model.Code;
                    //courseUpdate.NameCourse = model.NameCourse;
                    //courseUpdate.CourseCatalogID = model.CourseCatalogID;
                    //courseUpdate.DeleteFlag = model.DeleteFlag;
                    //courseUpdate.QuestionCount = model.QuestionCount;
                    //courseUpdate.QuestionDuration = model.QuestionDuration;
                    //courseUpdate.LeadTime = model.LeadTime;
                    //courseUpdate.CourseCopyID = model.CourseCopyID;
                    //courseUpdate.CourseTypeID = model.CourseTypeID;
                    //if (await _courseRepo.UpdateAsync(courseUpdate) > 0)
                    //{
                    //    if (model.IdeaIDs != null && model.IdeaIDs.Count > 0)
                    //    {
                    //        foreach (var id in model.IdeaIDs)
                    //        {
                    //            var entity = _registerIdeaRepo.GetByID(id);
                    //            if (entity != null || entity.ID > 0)
                    //            {
                    //                entity.CourseID = courseUpdate.ID;
                    //                await _registerIdeaRepo.UpdateAsync(entity);

                    //            }
                    //        }
                    //    }
                    //}

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

                    if (await _courseRepo.UpdateAsync(courseUpdate) > 0)
                    {
                        //  luôn xử lý Idea (kể cả khi list rỗng)
                        var newIdeaIds = model.IdeaIDs ?? new List<int>();

                        var currentIdeas = _registerIdeaRepo
                            .GetAll(x => x.CourseID == courseUpdate.ID)
                            .ToList();

                        foreach (var idea in currentIdeas.Where(x => !newIdeaIds.Contains(x.ID)))
                        {
                            idea.IsDeleted = true;
                            //idea.CourseID = null;
                            await _registerIdeaRepo.UpdateAsync(idea);
                        }

                        foreach (var id in newIdeaIds.Where(id => !currentIdeas.Any(x => x.ID == id)))
                        {
                            var entity = _registerIdeaRepo.GetByID(id);
                            if (entity != null && entity.ID > 0)
                            {
                                entity.CourseID = courseUpdate.ID;
                                await _registerIdeaRepo.UpdateAsync(entity);
                            }
                        }
                        // KPI
                        //List<Course_KPIPositionType> course_KPIPositionTypes = _course_KPIPositionTypeRepo.GetAll(c=>c.CourseID == courseUpdate.ID);
                        //foreach (var item in course_KPIPositionTypes)
                        //{
                        //    item.IsDeleted = !model.KPIIDs.Contains(item.KPIPositionTypeID);
                        //   await _course_KPIPositionTypeRepo.UpdateAsync(item);
                        //}
                        //foreach (var item in model.KPIIDs)
                        //{
                        //    if (!course_KPIPositionTypes.Any(c=>c.KPIPositionTypeID==item))
                        //    {
                        //        var newCourse_KPIPositionTypeModel = new Course_KPIPositionType();
                        //        newCourse_KPIPositionTypeModel.CourseID = courseUpdate.ID;
                        //        newCourse_KPIPositionTypeModel.KPIPositionTypeID = item;
                        //        newCourse_KPIPositionTypeModel.IsDeleted = false;
                        //       await _course_KPIPositionTypeRepo.CreateAsync(newCourse_KPIPositionTypeModel);
                        //    }
                        //}

                        // === SYNC KPI (UPDATE) ===
                        // luôn xử lý KPI (kể cả khi list rỗng)
                        var newKpiIds = model.KPIIDs ?? new List<int>();

                        var currentKpis = _course_KPIEmployeeTeamMapRepo
                            .GetAll(x => x.CourseID == courseUpdate.ID && x.IsDeleted == false)
                            .ToList();

                        // === REMOVE (soft delete + gỡ liên kết) ===
                        foreach (var kpi in currentKpis.Where(x => !newKpiIds.Contains(TextUtils.ToInt32(x.KPIEmployeeTeamID))))
                        {
                            kpi.IsDeleted = true;
                            // nếu entity của bạn có CourseID nullable thì gỡ ra, còn không thì bỏ dòng này
                            // kpi.CourseID = null;

                            await _course_KPIEmployeeTeamMapRepo.UpdateAsync(kpi);
                        }

                        // === ADD NEW ===
                        foreach (var id in newKpiIds.Where(id => !currentKpis.Any(x => x.KPIEmployeeTeamID == id)))
                        {
                            // kiểm tra đã tồn tại record soft delete chưa
                            var existed = _course_KPIEmployeeTeamMapRepo
                                .GetAll(x => x.CourseID == courseUpdate.ID && x.KPIEmployeeTeamID == id)
                                .FirstOrDefault();

                            if (existed != null)
                            {
                                // restore lại
                                existed.IsDeleted = false;
                                await _course_KPIEmployeeTeamMapRepo.UpdateAsync(existed);
                            }
                            else
                            {
                                // tạo mới
                                var newKpi = new CourseKPIEmployeeTeamMap
                                {
                                    CourseID = courseUpdate.ID,
                                    KPIEmployeeTeamID = id,
                                    IsDeleted = false
                                };

                                await _course_KPIEmployeeTeamMapRepo.CreateAsync(newKpi);
                            }
                        }
                    }
                }
                return Ok(ApiResponseFactory.Success(model, "Lưu khóa học thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //// Thêm sửa xóa bài học
        [HttpPost("save-data-lesson")]
        public async Task<IActionResult> SaveDataLesson([FromBody] SaveCourseLessonParam model)
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
                    return BadRequest(ApiResponseFactory.Fail(null, "Mã bài học đã tồn tại! Vui lòng kiểm tra lại."));
                }
                //if (exitLessonOld.VideoURL!=null)
                //{
                //    exitLessonOld.VideoURL = pathUpload + "/videos/"+
                //}

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
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("get-course-lesson-file-by-lessonid")]
        public IActionResult GetCourseLessonFileByLessonId(int lessonId)
        {
            try
            {
                var courseLessonFile = _courseFilesRepo.GetAll(x => x.LessonID == lessonId && (!x.IsDeleted ?? true) && !string.IsNullOrEmpty(x.NameFile));

                return Ok(ApiResponseFactory.Success(courseLessonFile, "Success"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi GET lesson file by lesonId: {ex.Message}"));
            }
        }

        [HttpPost("delete-course-lesson-file-by-lessonid")]
        public async Task<IActionResult> DeleteCourseLessonFileByLessonId(string ids)
        {
            try
            {
                List<int> idList = ids.Split(',')
                      .Select(id => int.Parse(id.Trim()))
                      .ToList();

                //                var newsletterFile = _newsletterFileRepo.GetAll(x => x.IsDeleted == false).Where(y => idList.Contains(y.ID));

                foreach (var item in idList)
                {
                    var courseFile = _courseFilesRepo.GetByID(item);
                    courseFile.IsDeleted = true;
                    await _courseFilesRepo.UpdateAsync(courseFile);
                }

                return Ok(ApiResponseFactory.Success(null, "Xóa File thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi xóa File: {ex.Message}"));
            }
        }
    }
}