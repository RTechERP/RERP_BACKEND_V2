using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NPOI.OpenXmlFormats.Dml.WordProcessing;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Model.Param.HRM.Course;
using RERPAPI.Repo.GenericEntity;
using System.Linq;
using System.Security.Claims;

namespace RERPAPI.Controllers.KHOAHOC
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class CourseController : ControllerBase
    {
        private readonly CourseCatalogRepo _courseCatalogRepo;
        private readonly CourseCatalogProjectTypeRepo _courseCatalogProjectTypeRepo;
        private readonly DepartmentRepo _departmentRepo;
        private readonly KPIPositionTypeRepo _kpiPositionTypeRepo;
        private readonly RegisterIdeaRepo _RegisterIdeaRepo;
        private readonly CourseRepo _courseRepo;
        private readonly RegisterIdeaRepo _registerIdeaRepo;
        private readonly CourseRegisterIdeaRepo _courseRegisterIdeaRepo;
        private readonly CourseLessonRepo _courseLessonRepo;
        private readonly CourseFilesRepo _courseFilesRepo;
        public CourseController(CourseCatalogRepo courseCatalogRepo, 
            CourseCatalogProjectTypeRepo courseCatalogProjectTypeRepo,
            DepartmentRepo departmentRepo,
            KPIPositionTypeRepo kpiPositionTypeRepo,
            CourseRepo courseRepo,
            RegisterIdeaRepo registerIdeaRepo,
            CourseRegisterIdeaRepo courseRegisterIdeaRepo,
            CourseLessonRepo courseLessonRepo,
            CourseFilesRepo courseFilesRepo)
        {
            _courseCatalogRepo = courseCatalogRepo;
            _courseCatalogProjectTypeRepo = courseCatalogProjectTypeRepo;
            _departmentRepo = departmentRepo;
            _kpiPositionTypeRepo = kpiPositionTypeRepo;
            _courseRepo = courseRepo;
            _registerIdeaRepo = registerIdeaRepo;
            _courseRegisterIdeaRepo = courseRegisterIdeaRepo;
            _courseLessonRepo = courseLessonRepo;
            _courseFilesRepo = courseFilesRepo;
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
        public async Task<IActionResult> GetDanhMuc()
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetCourseCatalog",
                                              new string[] { },
                                              new object[] { });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), ""));
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
                var data = SQLHelper<object>.ProcedureToList("spGetCourseNew",
                                              new string[] { "@CourseCatalogID", "@UserID", "@Status" },
                                              new object[] { courseCatalogID, currentUser.ID, -1 });
                    return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), ""));
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
                                              new object[] { courseID});
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data,0), ""));
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
                                              new string[] {},
                                              new object[] {});
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
                                              new object[] { 0, 0, 0, TextUtils.MinDate, dateEnd, "", 1, 999999999, courseCategoryID });
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
        }

        //lấy danh sách KPI Position Type
        [HttpGet("load-kpipositiontype")]
        public async Task<IActionResult> LoadKPIPositionType()
        {
            try
            {
                var data = _kpiPositionTypeRepo.GetAll(x => x.IsDeleted == false);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Thêm sửa xóa danh mục khóa học
        [HttpPost("save-data-category")]
        public async Task<IActionResult> SaveDataCategory([FromBody] SaveCoureCategoryParam model)
        {
            try
            {
                var exitCategory = _courseCatalogRepo.GetAll(x => ( x.Code.ToUpper().Trim() == model.Code.ToUpper().Trim()) && x.ID != model.ID && (!x.IsDeleted ?? true) ).FirstOrDefault();

                if (exitCategory != null && exitCategory.ID > 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Mã danh mục đã tồn tại! Vui lòng kiểm tra lại."));
                }

                

                if (model.ID <= 0)
                {
                    var dept = _departmentRepo.GetAll(x => x.STT == model.DepartmentSTT).FirstOrDefault();
                    var courseCatalog = new CourseCatalog
                    {
                        ID = 0,
                        Name = model.Name,
                        CatalogType = model.CatalogType,
                        DeleteFlag = model.DeleteFlag,
                        DepartmentID = dept.ID,
                        STT = model.STT,
                        Code = model.Code,
                        IsDeleted = model.IsDeleted,
                    };



                     await _courseCatalogRepo.CreateAsync(courseCatalog);
                    
                    if(model.ProjectTypeIDs.Count > 0)
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
                    var dept = _departmentRepo.GetAll(x => x.STT == model.DepartmentSTT).FirstOrDefault();

                    var courseCatalog = _courseCatalogRepo.GetByID(model.ID);
                    courseCatalog.Name = model.Name;
                    courseCatalog.CatalogType = model.CatalogType;
                    courseCatalog.DepartmentID = dept.ID;
                    courseCatalog.STT = model.STT;
                    courseCatalog.IsDeleted = model.IsDeleted;
                    courseCatalog.DeleteFlag = model.DeleteFlag;
                    courseCatalog.Code = model.Code;
                    await _courseCatalogRepo.UpdateAsync(courseCatalog);

                    var existingProjectTypes = _courseCatalogProjectTypeRepo.GetAll(x => x.CourseCatalogID == model.ID && (!x.IsDeleted ?? true)).ToList();
                    if(existingProjectTypes.Count > 0 && !(existingProjectTypes.IsNullOrEmpty()))
                    {
                        if(model.ProjectTypeIDs.Count <= 0 || model.ProjectTypeIDs.IsNullOrEmpty())
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

                            // ❌ cần xóa
                            var idsToRemove = existingIds
                                .Except(model.ProjectTypeIDs)
                                .ToList();

                            // ➕ cần thêm
                            var idsToAdd = model.ProjectTypeIDs
                                .Except(existingIds)
                                .ToList();

                            // DELETE
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
                            foreach (var item in existingProjectTypes)
                            {
                                var entity = _courseCatalogProjectTypeRepo.GetByID(item.ID);
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
                                        ProjectTypeID = item.ProjectTypeID
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
                    return BadRequest(ApiResponseFactory.Fail(null, "Mã khóa học đã tồn tại! Vui lòng kiểm tra lại."));
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
                        DeleteFlag = model.DeleteFlag,
                        QuestionCount = model.QuestionCount,
                        QuestionDuration = model.QuestionDuration,
                        LeadTime = model.LeadTime,
                        CourseCopyID = model.CourseCopyID,
                        CourseTypeID = model.CourseTypeID,
                        EmployeeID = currentUser.ID,
                    };
                    await _courseRepo.CreateAsync(courseNew);

                    if(model.IdeaIDs != null && model.IdeaIDs.Count > 0)
                    {
                        foreach (var item in model.IdeaIDs)
                        {
                            var newCourseIdea = new CourseRegisterIdea
                            {
                                CourseID = courseNew.ID,
                                IdeaID = item
                            };
                            await _courseRegisterIdeaRepo.CreateAsync(newCourseIdea);
                        }
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
                    await _courseRepo.UpdateAsync(courseUpdate);

                    var existingCourseIdeas = _courseRegisterIdeaRepo.GetAll(x => x.CourseID == model.ID && (!x.IsDeleted ?? true)).ToList();
                    if (existingCourseIdeas.Count > 0 && !(existingCourseIdeas.IsNullOrEmpty()))
                    {
                        if (model.IdeaIDs == null || model.IdeaIDs.Count <= 0 || model.IdeaIDs.IsNullOrEmpty())
                        {
                            foreach (var item in existingCourseIdeas)
                            {
                                var courseIdea = existingCourseIdeas.Where(x => x.ID == item.ID).FirstOrDefault();
                                courseIdea.IsDeleted = true;
                                await _courseRegisterIdeaRepo.UpdateAsync(courseIdea);
                            }
                        }
                        else
                        {
                            var existCourseIdeaIDs = existingCourseIdeas
                                .Select(x => x.IdeaID)
                                .ToList();
                            // ❌ cần xóa
                            var idsToRemove = existCourseIdeaIDs
                                .Except(model.IdeaIDs)
                                .ToList();
                            // ➕ cần thêm
                            var idsToAdd = model.IdeaIDs
                                .Except(existCourseIdeaIDs)
                                .ToList();
                            // DELETE
                            foreach (var ideaId in idsToRemove)
                            {
                                var entity = existingCourseIdeas
                                    .FirstOrDefault(x => x.IdeaID == ideaId);
                                if (entity != null && entity.ID > 0)
                                {
                                    entity.IsDeleted = true;
                                    await _courseRegisterIdeaRepo.UpdateAsync(entity);
                                }
                            }
                            // INSERT
                            foreach (var ideaId in idsToAdd)
                            {
                                var entity = _courseRegisterIdeaRepo.GetByID(ideaId);
                                if(entity != null && entity.ID > 0)
                                {
                                    entity.IsDeleted = false;
                                    await _courseRegisterIdeaRepo.UpdateAsync(entity);
                                    continue;
                                }
                                else
                                {
                                    var newEntity = new CourseRegisterIdea
                                    {
                                        CourseID = courseUpdate.ID,
                                        IdeaID = ideaId
                                    };
                                    await _courseRegisterIdeaRepo.CreateAsync(newEntity);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (model.IdeaIDs != null && model.IdeaIDs.Count > 0)
                        {
                            foreach (var item in model.IdeaIDs)
                            {
                                var entity = _courseRegisterIdeaRepo.GetByID(item);
                                if (entity != null && entity.ID > 0)
                                {
                                    entity.IsDeleted = false;
                                    await _courseRegisterIdeaRepo.UpdateAsync(entity);
                                    continue;
                                }
                                var newEntity = new CourseRegisterIdea
                                {
                                    CourseID = courseUpdate.ID,
                                    IdeaID = item
                                };
                                await _courseRegisterIdeaRepo.CreateAsync(newEntity);
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
                if (model == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu gửi lên không hợp lệ."));
                }
                if ( model.CourseLesson.IsDeleted ?? false)
                {
                    if(model.CourseLesson.ID > 0)
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

                if (model.CourseLesson.ID <= 0)
                {
                    if (model.CoursePdf != null &&
                        !string.IsNullOrWhiteSpace(model.CoursePdf.NameFile))
                    {
                        model.CourseLesson.UrlPDF = model.CoursePdf.NameFile;
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

                    if(model.CoursePdf != null && model.CoursePdf.NameFile != null)
                    {
                        model.CourseLesson.UrlPDF = model.CoursePdf.NameFile;
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
                var courseLessonFile = _courseFilesRepo.GetAll(x => x.LessonID == lessonId && (!x.IsDeleted ?? true));

                return Ok(ApiResponseFactory.Success(courseLessonFile, "Success"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi GET lesson file bay lesonId: {ex.Message}"));
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
