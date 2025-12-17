using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Net;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RERPAPI.Controllers.HRM
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RegisterIdeaController : ControllerBase
    {

        private readonly RegisterIdeaRepo _registerIdeaRepo;
        private readonly RegisterIdeaTypeRepo _registerIdeaTypeRepo;
        private readonly EmployeeRepo _employeeRepo;
        private readonly DepartmentRepo _departmentRepo;
        private readonly RegisterIdeaDetailRepo _registerIdeaDetailRepo;
        private readonly RegisterIdeaFileRepo _registerIdeaFileRepo;
        private readonly RegisterIdeaScoreRepo _registerIdeaScoreRepo;
        private readonly CourseCatalogRepo _courseCatalogRepo;
        private readonly CurrentUser _currentUser;
        private readonly ConfigSystemRepo _configSystemRepo;
        public RegisterIdeaController(RegisterIdeaRepo registerIdeaRepo, RegisterIdeaTypeRepo registerIdeaTypeRepo, EmployeeRepo employeeRepo, DepartmentRepo departmentRepo, RegisterIdeaDetailRepo registerideadetailRepo, RegisterIdeaFileRepo registerideafileRepo, RegisterIdeaScoreRepo registerideascoreRepo, CourseCatalogRepo courseCatalogRepo, CurrentUser currentUser, ConfigSystemRepo configSystemRepo)
        {
            _registerIdeaRepo = registerIdeaRepo;
            _registerIdeaTypeRepo = registerIdeaTypeRepo;
            _employeeRepo = employeeRepo;
            _departmentRepo = departmentRepo;
            _registerIdeaDetailRepo = registerideadetailRepo;
            _registerIdeaFileRepo = registerideafileRepo;
            _registerIdeaScoreRepo = registerideascoreRepo;
            _courseCatalogRepo = courseCatalogRepo;
            _currentUser = currentUser;
            _configSystemRepo = configSystemRepo;
        }

        [HttpGet("get-course-catalog")]
        public IActionResult GetCourseCatalog()
        {
            try
            {
                var data = _courseCatalogRepo.GetAll(x => x.CatalogType == 2).Select(x => new
                {
                    x.ID,
                    Name = x.Code + " - " + x.Name
                }).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-ideas")]
        public IActionResult GetAllRegisterIdea(int employeeId, DateTime dateStart, DateTime dateEnd, int authorid, int departmentid, int registertypeid, string keyword = "")
        {
            try
            {
                var result = SQLHelper<dynamic>.ProcedureToList("spGetRegisterIdea",
                                new string[] { "@EmployeeID", "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@FilterText", "@DepartmentID", "@AuthorID", "@RegisterTypeID" },
                                new object[] { employeeId, 1, 999999999, dateStart, dateEnd, keyword, departmentid, authorid, registertypeid });

                var data = SQLHelper<dynamic>.GetListData(result, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-idea-detail")]
        public IActionResult GetRegisterIdeaDetail(int id, int currentUserEmployeeId)
        {
            try
            {
                RegisterIdea rgt = _registerIdeaRepo.GetByID(id);
                var em = _employeeRepo.GetAll()
                                        .Select(x => new
                                        {
                                            x.ID,
                                            x.FullName,
                                            x.DepartmentID
                                        }).ToList();
                List<Model.Entities.Department> de = _departmentRepo.GetAll().ToList();
                List<RegisterIdeaDetail> rgtd = _registerIdeaDetailRepo.GetAll().Where(x => x.RegisterIdeaID == id).ToList();
                List<RegisterIdeaFile> rgtf = _registerIdeaFileRepo.GetAll().Where(x => x.RegisterIdeaID == id).ToList();
                List<RegisterIdeaScoreDTO> rgts = SQLHelper<RegisterIdeaScoreDTO>.ProcedureToListModel("spGetRegisterIdeaScore",
                                                            new string[] { "@RegisterIdeaID", "@EmployeeID" },
                new object[] { id, currentUserEmployeeId });
                return Ok(ApiResponseFactory.Success(new { em, de, rgt, rgtd, rgtf, rgts }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete-idea")]
        public IActionResult DeleteIdea(int id)
        {
            try
            {
                var ent = _registerIdeaRepo.GetByID(id);
                if (ent == null) return BadRequest(ApiResponseFactory.Fail(null, "Không tồn tại bản ghi cần xóa"));

                ent.IsDeleted = true;

                _registerIdeaRepo.Update(ent);
                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-idea")]
        public IActionResult SaveRegisterIdea([FromBody] RegisterIdeaDTO model)
        {
            try
            {
                if (model.RegisterIdeaDetails == null || model.RegisterIdeaDetails.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Ý tưởng không có chi tiết"));

                // Validate từng dòng
                foreach (var d in model.RegisterIdeaDetails)
                {
                    if (d.DateStart == null) return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn ngày bắt đầu"));
                    if (d.DateEnd == null) return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn ngày kết thúc"));
                    if (d.DateEnd < d.DateStart) return BadRequest(ApiResponseFactory.Fail(null, "Ngày kết thúc phải lớn hơn ngày bắt đầu"));
                    if (string.IsNullOrEmpty(d.Description)) return BadRequest(ApiResponseFactory.Fail(null, $"Vui lòng nhập diễn giải cho {d.Category}"));
                }

                RegisterIdea entity = model.ID > 0
                    ? _registerIdeaRepo.GetByID(model.ID)
                    : new RegisterIdea();

                entity.DepartmentOrganizationID = model.DepartmentOrganizationID;
                entity.RegisterIdeaTypeID = model.RegisterIdeaTypeID;
                entity.ApprovedTBPID = model.HeadofDepartment;
                entity.EmployeeID = model.EmployeeID;
                entity.DateRegister = DateTime.Now;

                if (model.ID == 0)
                {
                    entity.IsApprovedTBP = entity.IsApproved = false;
                    entity.ApprovedID = 0;
                    _registerIdeaRepo.Create(entity);
                }    
                else
                    _registerIdeaRepo.Update(entity);

                model.ID = entity.ID;

                // Xử lý detail
                foreach (var d in model.RegisterIdeaDetails)
                {
                    var detail = _registerIdeaDetailRepo.GetAll()
                        .FirstOrDefault(x => x.STT == d.STT && x.RegisterIdeaID == model.ID)
                        ?? new RegisterIdeaDetail();

                    detail.RegisterIdeaID = model.ID;
                    detail.STT = d.STT;
                    detail.Category = d.Category;
                    detail.Description = d.Description;
                    detail.Note = d.Note;
                    detail.DateStart = d.DateStart;
                    detail.DateEnd = d.DateEnd;

                    if (detail.ID > 0)
                        _registerIdeaDetailRepo.Update(detail);
                    else
                        _registerIdeaDetailRepo.Create(detail);
                }

                if (model.deletedFileIds != null && model.deletedFileIds.Any())
                {
                    foreach (var fileId in model.deletedFileIds)
                    {
                        var file = _registerIdeaFileRepo.GetByID(fileId);
                        if (file == null)
                            continue;

                        if (file.RegisterIdeaID != model.ID)
                            continue;

                        // xóa file vật lý 
                        if (!string.IsNullOrEmpty(file.ServerPath) && !string.IsNullOrEmpty(file.FileName))
                        {
                            string fullPath = Path.Combine(file.ServerPath, file.FileName);
                            if (System.IO.File.Exists(fullPath))
                            {
                                System.IO.File.Delete(fullPath);
                            }
                        }

                        _registerIdeaFileRepo.Delete(file.ID);
                    }
                }


                return Ok(ApiResponseFactory.Success(new { id = model.ID }, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpGet("get-idea-types")]
        public IActionResult GetAllRegisterIdeaType()
        {
            try
            {
                var data = _registerIdeaTypeRepo.GetAll().Where(x => x.IsDeleted == false).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete-idea-type")]
        public IActionResult DeleteIdeaType(int id)
        {
            try
            {
                var ent = _registerIdeaTypeRepo.GetByID(id);
                if (ent == null) return BadRequest(ApiResponseFactory.Fail(null, "Không tồn tại bản ghi cần xóa"));

                ent.IsDeleted = true;

                _registerIdeaTypeRepo.Update(ent);
                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-idea-type")]
        public async Task<IActionResult> SaveRegisterIdeaType([FromBody] RegisterIdeaType model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.RegisterTypeCode))
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập mã đề tài!"));

                if (string.IsNullOrEmpty(model.RegisterTypeName))
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập tên đề tài!"));

                // Kiểm tra trùng mã
                var exists = _registerIdeaTypeRepo.GetAll()
                    .Any(x => x.RegisterTypeCode == model.RegisterTypeCode && x.ID != model.ID);

                if (exists)
                    return Ok(ApiResponseFactory.Fail(null, "Mã loại đề tài đã tồn tại!"));

                if (model.ID > 0)
                {
                    await _registerIdeaTypeRepo.UpdateAsync(model);
                    return Ok(ApiResponseFactory.Success(null, "Cập nhật loại ý tưởng thành công"));
                }
                else
                {
                    await _registerIdeaTypeRepo.CreateAsync(model);
                    return Ok(ApiResponseFactory.Success(null, "Thêm mới loại ý tưởng thành công"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpGet("get-file")]
        public IActionResult GetRegisterIdeaFile(int fileId)
        {
            try
            {
                var file = _registerIdeaFileRepo.GetByID(fileId);
                if (file == null)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy file"));

                return Ok(ApiResponseFactory.Success(file, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-score")]
        public IActionResult SaveRegisterIdeaScore([FromBody] RegisterIdeaScoreDTO model)
        {
            try
            {

                var registerIdea = _registerIdeaRepo.GetAll()
                    .FirstOrDefault(x => x.ID == model.RegisterIdeaID)
                    ?? new RegisterIdea();

                if (registerIdea.ID <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tồn tại ý tưởng"));

                if (!_currentUser.IsAdmin)
                {
                    int headOfDeptId = 0;
                    int.TryParse(_currentUser.HeadofDepartment, out headOfDeptId);

                    if (registerIdea.EmployeeID == _currentUser.EmployeeID &&
                        _currentUser.EmployeeID != headOfDeptId)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Bạn không có quyền chấm điểm"));
                    }

                }

                if (model.Score <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn điểm"));

                var trashScores = _registerIdeaScoreRepo.GetAll()
                    .Where(x =>
                        x.RegisterIdeaID == model.RegisterIdeaID &&
                        (x.IsTBP == false || x.IsTBP == null) &&
                        (x.IsBGD == false || x.IsBGD == null) &&
                        x.Score <= 0)
                    .ToList();

                if (trashScores.Any())
                {
                    foreach (var ts in trashScores)
                    {
                        _registerIdeaScoreRepo.Delete(ts.ID);
                    }
                } 

                foreach (int deptId in model.LsDepartmentId)
                {
                    var scoreDept = _registerIdeaScoreRepo.GetAll()
                        .FirstOrDefault(x =>
                            x.RegisterIdeaID == model.RegisterIdeaID &&
                            x.DepartmentID == deptId)
                        ?? new RegisterIdeaScore();

                    if (scoreDept.IsTBP == true || scoreDept.IsBGD == true || scoreDept.Score > 0)
                        continue;

                    scoreDept.RegisterIdeaID = model.RegisterIdeaID;
                    scoreDept.DepartmentID = deptId;

                    if (scoreDept.ID > 0)
                        _registerIdeaScoreRepo.Update(scoreDept);
                    else
                        _registerIdeaScoreRepo.Create(scoreDept);
                }

                // xác định department chấm
                int departmentID = _currentUser.EmployeeID == 54
                    ? 2
                    : _currentUser.DepartmentID;

                var registerScore = _registerIdeaScoreRepo.GetAll()
                    .FirstOrDefault(x =>
                        x.RegisterIdeaID == model.RegisterIdeaID &&
                        x.DepartmentID == departmentID)
                    ?? new RegisterIdeaScore();

                // gán điểm
                registerScore.Score = model.Score;
                registerScore.ScoreRating =
                    model.Score == 1 ? "A" :
                    model.Score == 2 ? "B" :
                    model.Score == 3 ? "C" : "D";

                registerScore.RegisterIdeaID = model.RegisterIdeaID;

                
                if (model.tbpCheck == true)
                {
                    // tbp duyệt
                    registerScore.IsApprovedTBP = true;
                    registerScore.DateApprovedTBP = DateTime.Now;
                    registerScore.IsTBP = true;
                    registerScore.DepartmentID = departmentID;

                    registerIdea.IsApprovedTBP = true;
                    registerIdea.DateApprovedTBP = DateTime.Now;
                }
                else if (_currentUser.DepartmentID == 1)
                {
                    // bgd duyệt
                    registerScore.IsApprovedBGD = true;
                    registerScore.DateApprovedBGD = DateTime.Now;
                    registerScore.IsBGD = true;
                    registerScore.DepartmentID = departmentID;

                    registerIdea.IsApproved = true;
                    registerIdea.DateApproved = DateTime.Now;
                    registerIdea.ApprovedID = _currentUser.EmployeeID;
                }
                else
                {
                    // nhân viên
                    registerScore.DepartmentID = _currentUser.DepartmentID;
                    registerScore.IsTBP = false;
                    registerScore.IsBGD = false;
                    registerScore.IsApprovedBGD = false;
                    registerScore.ApprovedBGDID = 0;
                    registerScore.ApprovedID = 0;
                    registerScore.ApprovedTBPID = 0;
                }

                //lưu
                if (registerScore.ID > 0)
                {
                    _registerIdeaScoreRepo.Update(registerScore);
                }
                else
                {
                    _registerIdeaScoreRepo.Create(registerScore);
                }

                // update register idea
                _registerIdeaRepo.Update(registerIdea);

                return Ok(ApiResponseFactory.Success(null, "Đã chấm điểm"));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //[HttpPost("upload-file")]
        //public async Task<IActionResult> UploadFileRegisterIdea(
        //    int registerId,
        //    int employeeId,
        //    [FromForm] List<IFormFile> files)
        //{
        //    try
        //    {
        //        // Check tồn tại ý tưởng
        //        var registerIdea = _registerIdeaRepo.GetByID(registerId);
        //        if (registerIdea == null)
        //            return BadRequest(ApiResponseFactory.Fail(null, "Không tồn tại ý tưởng!"));

        //        // Check quyền upload
        //        if (registerIdea.EmployeeID != employeeId)
        //            return BadRequest(ApiResponseFactory.Fail(null, "Bạn không có quyền upload file cho ý tưởng này"));

        //        // Lấy loại đề tài để tạo đường dẫn
        //        string code = "";
        //        string nameDepartment = "";

        //        var catalog = _courseCatalogRepo.GetByID(registerIdea.RegisterIdeaTypeID ?? 0);
        //        if (catalog != null)
        //        {
        //            code = catalog.Code;
        //            var dept = _courseCatalogRepo.GetByID(catalog.DepartmentID ?? 0);
        //            nameDepartment = dept?.Name ?? "";
        //        }

        //        // Tạo path upload
        //        string pathServer = @"\\192.168.1.190\duan\Tip Trick\";
        //        string pathPattern = $"{registerIdea.DateRegister.Value.Year}\\P {nameDepartment}\\{code}";
        //        string pathUpload = Path.Combine(pathServer, pathPattern);

        //        if (!Directory.Exists(pathUpload))
        //            Directory.CreateDirectory(pathUpload);

        //        foreach (var file in files)
        //        {
        //            if (file.Length <= 0)
        //                continue;

        //            // Check không upload trùng file
        //            var exists = _registerIdeaFileRepo.GetAll()
        //                .Any(x => x.FileName == file.FileName && x.RegisterIdeaID == registerId);

        //            if (exists)
        //                continue;

        //            using var http = new HttpClient();
        //            using var form = new MultipartFormDataContent();

        //            using var stream = file.OpenReadStream();
        //            byte[] bytes = new byte[file.Length];
        //            await stream.ReadAsync(bytes, 0, (int)file.Length);

        //            form.Add(new ByteArrayContent(bytes), "file", file.FileName);

        //            string remoteApi = $"http://113.190.234.64:8083/api/Home/uploadfile?path={pathUpload}";

        //            var result = await http.PostAsync(remoteApi, form);

        //            if (result.StatusCode == HttpStatusCode.OK)
        //            {
        //                var saved = new RegisterIdeaFile
        //                {
        //                    RegisterIdeaID = registerId,
        //                    FileName = file.FileName,
        //                    ServerPath = pathUpload,
        //                    OriginPath = "",
        //                    CreatedDate = DateTime.Now,
        //                    UpdatedDate = DateTime.Now,
        //                };

        //                await _registerIdeaFileRepo.CreateAsync(saved);
        //            }
        //        }

        //        return Ok(ApiResponseFactory.Success(null, "Upload file thành công"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}

        [HttpPost("upload-file")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFileRegisterIdea(int registerId, int employeeId)
        {
            try
            {
                var form = await Request.ReadFormAsync();
                var key = form["key"].ToString();
                var files = form.Files;

                // Validate input
                if (registerId <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "RegisterId không hợp lệ"));

                if (employeeId <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "EmployeeId không hợp lệ"));

                if (string.IsNullOrWhiteSpace(key))
                    return BadRequest(ApiResponseFactory.Fail(null, "Key không được để trống"));

                if (files == null || files.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách file không được để trống"));

                // Check tồn tại ý tưởng
                var registerIdea = _registerIdeaRepo.GetByID(registerId);
                if (registerIdea == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tồn tại ý tưởng"));

                // Check quyền upload
                if (registerIdea.EmployeeID != employeeId)
                    return BadRequest(ApiResponseFactory.Fail(null, "Bạn không có quyền upload file cho ý tưởng này"));

                // Lấy path upload từ config (GIỐNG POKH)
                var uploadPath = _configSystemRepo.GetUploadPathByKey(key);
                if (string.IsNullOrWhiteSpace(uploadPath))
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy cấu hình đường dẫn cho key: {key}"));

                // Xử lý subPath (nếu có)
                var subPathRaw = form["subPath"].ToString()?.Trim() ?? "";
                string targetFolder = uploadPath;

                if (!string.IsNullOrWhiteSpace(subPathRaw))
                {
                    var separator = Path.DirectorySeparatorChar;
                    var segments = subPathRaw
                        .Replace('/', separator)
                        .Replace('\\', separator)
                        .Split(separator, StringSplitOptions.RemoveEmptyEntries)
                        .Select(seg =>
                        {
                            var invalidChars = Path.GetInvalidFileNameChars();
                            var cleaned = new string(seg.Where(c => !invalidChars.Contains(c)).ToArray());
                            cleaned = cleaned.Replace("..", "").Trim();
                            return cleaned;
                        })
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToArray();

                    if (segments.Length > 0)
                        targetFolder = Path.Combine(uploadPath, Path.Combine(segments));
                }
                else
                {
                    // Folder mặc định giống NB{ID} của POKH
                    targetFolder = Path.Combine(uploadPath, $"RI{registerIdea.ID}");
                }

                if (!Directory.Exists(targetFolder))
                    Directory.CreateDirectory(targetFolder);

                var processedFiles = new List<RegisterIdeaFile>();

                foreach (var file in files)
                {
                    if (file.Length <= 0)
                        continue;

                    // Không cho upload trùng file
                    bool exists = _registerIdeaFileRepo.GetAll()
                        .Any(x => x.RegisterIdeaID == registerId && x.FileName == file.FileName);

                    if (exists)
                        continue;

                    var fileExtension = Path.GetExtension(file.FileName);
                    var originalName = Path.GetFileNameWithoutExtension(file.FileName);
                    var uniqueFileName = $"{originalName}{fileExtension}";
                    var fullPath = Path.Combine(targetFolder, uniqueFileName);

                    // Lưu file trực tiếp (GIỐNG POKH)
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var fileEntity = new RegisterIdeaFile
                    {
                        RegisterIdeaID = registerId,
                        FileName = uniqueFileName,
                        OriginPath = targetFolder,
                        ServerPath = targetFolder,
                    };

                    await _registerIdeaFileRepo.CreateAsync(fileEntity);
                    processedFiles.Add(fileEntity);
                }

                return Ok(ApiResponseFactory.Success(
                    processedFiles,
                    $"{processedFiles.Count} file đã được upload thành công"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi upload file: {ex.Message}"));
            }
        }



    }
}
