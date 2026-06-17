using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Duan.MeetingMinutes;
using RERPAPI.Repo.GenericEntity.Project;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectHistoryProblemController : ControllerBase
    {
        //private readonly ProjectHistoryProblemDetailRepo _historyproblemDetailRepo;
        private readonly ProjectHistoryProblemRepo _historyproblemRepo;

        private readonly ProjectHistoryProblemPartListLinkRepo _projectHistoryProblemPartListLinkRepo;
        private readonly ProjectHistoryProblemProjectItemLinkRepo _projectHistoryProblemProjectItemLinkRepo;
        private readonly ProjectHistoryProblemReceiverLinkRepo _projectHistoryProblemReceiverLinkRepo;
        private readonly ProjectHistoryProblemWorkerLinkRepo _projectHistoryProblemWorkerLinkRepo;
        private readonly DepartmentRepo _departmentRepo;
        private readonly EmployeeRepo _employeeRepo;
        private readonly ProjectRepo _projectRepo;
        private readonly ConfigSystemRepo _configSystemRepo;
        private readonly ProjectHistoryProblemFileRepo _projectHistoryProblemFileRepo;
        private readonly ProjectItemRepo _projectItemRepo;
        private readonly ProjectWorkerVersionRepo _projectWorkerVersionRepo;
        private readonly ProjectPartlistVersionRepo _projectPartListVersionRepo;
        private readonly EmailHelper _emailHelper;
        private readonly ProjectHistoryProblemLogRepo _projectHistoryProblemLogRepo;

        public ProjectHistoryProblemController(
               //ProjectHistoryProblemDetailRepo historyproblemDetailRepo,
               ProjectHistoryProblemRepo historyproblemRepo,
               ProjectHistoryProblemWorkerLinkRepo projectHistoryProblemWorkerLinkRepo,
               ProjectHistoryProblemPartListLinkRepo projectHistoryProblemPartListLinkRepo,
               ProjectHistoryProblemProjectItemLinkRepo projectHistoryProblemProjectItemLinkRepo,
               ProjectHistoryProblemReceiverLinkRepo projectHistoryProblemReceiverLinkRepo,
               DepartmentRepo departmentRepo,
               EmployeeRepo employeeRepo,
               ProjectRepo projectRepo,
               ConfigSystemRepo configSystemRepo,
               ProjectHistoryProblemFileRepo projectHistoryProblemFileRepo,
               ProjectItemRepo projectItemRepo,
               ProjectWorkerVersionRepo projectWorkerVersionRepo,
               ProjectPartlistVersionRepo projectPartlistVersionRepo,
               EmailHelper emailHelper,
               ProjectHistoryProblemLogRepo projectHistoryProblemLogRepo)
        {
            //_historyproblemDetailRepo = historyproblemDetailRepo;
            _historyproblemRepo = historyproblemRepo;
            _projectHistoryProblemWorkerLinkRepo = projectHistoryProblemWorkerLinkRepo;
            _projectHistoryProblemPartListLinkRepo = projectHistoryProblemPartListLinkRepo;
            _projectHistoryProblemProjectItemLinkRepo = projectHistoryProblemProjectItemLinkRepo;
            _projectHistoryProblemReceiverLinkRepo = projectHistoryProblemReceiverLinkRepo;
            _projectHistoryProblemWorkerLinkRepo = projectHistoryProblemWorkerLinkRepo;
            _departmentRepo = departmentRepo;
            _employeeRepo = employeeRepo;
            _projectRepo = projectRepo;
            _configSystemRepo = configSystemRepo;
            _projectHistoryProblemFileRepo = projectHistoryProblemFileRepo;
            _projectItemRepo = projectItemRepo;
            _projectWorkerVersionRepo = projectWorkerVersionRepo;
            _projectPartListVersionRepo = projectPartlistVersionRepo;
            _emailHelper = emailHelper;
            _projectHistoryProblemLogRepo = projectHistoryProblemLogRepo;
        }

        [HttpPost("get-data")]
        public async Task<IActionResult> getDataHistoryProblem(int projectID, int employeeID)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList(
                "spGetProjectHistoryProblemDetail_New",
                new string[] { "@ProjectID", "@EmployeeID" },
                new object[] { projectID, employeeID });

                var dtDetail = SQLHelper<object>.GetListData(data, 0);
                var dtMaster = SQLHelper<object>.GetListData(data, 2);
                return Ok(ApiResponseFactory.Success(new { dtDetail, dtMaster },
                    "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //[HttpPost("get-data-detail")]
        //public async Task<IActionResult> getDataHistoryProblemDetail(int id)
        //{
        //    try
        //    {
        //        var data = _historyproblemDetailRepo.GetAll(x => x.ProjectHistoryProblemID == id && x.IsDeleted == false);

        //        return Ok(ApiResponseFactory.Success(
        //            data,
        //            "Lấy dữ liệu thành công"));

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}

        [HttpGet("get-max-stt-by-project")]
        public IActionResult GetMaxSTTByProject(int projectID)
        {
            try
            {
                if (projectID <= 0)
                {
                    return Ok(ApiResponseFactory.Success(
                        new { maxSTT = 0 },
                        "ProjectID không hợp lệ"));
                }

                var maxSTT = _historyproblemRepo.GetAll(x => x.ProjectID == projectID && x.IsDeleted == false)
                    .Select(x => (int?)x.STT)
                    .Max() ?? 0;

                return Ok(ApiResponseFactory.Success(
                    new { maxSTT = maxSTT },
                    "Lấy STT lớn nhất thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data-problem")]
        public async Task<IActionResult> saveData([FromBody] List<ProjectHistoryProblemDTO> request)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                int lastSavedProblemId = 0;
                if (!_historyproblemRepo.Validate(request, out string message))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }
                //lưu problem
                foreach (var item in request)
                {
                    if (item.projectHistoryProblem != null)
                    {
                        int problemId = 0;
                        if (item.projectHistoryProblem.ID > 0)
                        {
                            //await _historyproblemRepo.UpdateAsync(item.projectHistoryProblem);
                            //problemId = item.projectHistoryProblem.ID;

                            var oldProblemDb = _historyproblemRepo.GetByID(item.projectHistoryProblem.ID);

                            ProjectHistoryProblem oldProblem = null;

                            if (oldProblemDb != null)
                            {
                                oldProblem = new ProjectHistoryProblem
                                {
                                    ID = oldProblemDb.ID,
                                    ProjectID = oldProblemDb.ProjectID,
                                    STT = oldProblemDb.STT,
                                    TypeProblem = oldProblemDb.TypeProblem,
                                    ContentError = oldProblemDb.ContentError,
                                    Reason = oldProblemDb.Reason,
                                    Remedies = oldProblemDb.Remedies,
                                    TestMethod = oldProblemDb.TestMethod,
                                    Image = oldProblemDb.Image,
                                    DateProblem = oldProblemDb.DateProblem,
                                    DateImplementation = oldProblemDb.DateImplementation,
                                    PIC = oldProblemDb.PIC,
                                    Impact = oldProblemDb.Impact,
                                    ErrorLocation = oldProblemDb.ErrorLocation,
                                    Note = oldProblemDb.Note,

                                    CreatedBy = oldProblemDb.CreatedBy,
                                    CreatedDate = oldProblemDb.CreatedDate,
                                    UpdatedBy = oldProblemDb.UpdatedBy,
                                    UpdatedDate = oldProblemDb.UpdatedDate,

                                    EmployeeID = oldProblemDb.EmployeeID,
                                    IsDeleted = oldProblemDb.IsDeleted,
                                    IssueLogType = oldProblemDb.IssueLogType,
                                    CreatorID = oldProblemDb.CreatorID,
                                    ProjectManagerID = oldProblemDb.ProjectManagerID,
                                    PriorityLevel = oldProblemDb.PriorityLevel,
                                    PerformerID = oldProblemDb.PerformerID,
                                    StatusProblem = oldProblemDb.StatusProblem,
                                    IssueConclusion = oldProblemDb.IssueConclusion
                                };
                            }

                            await _historyproblemRepo.UpdateAsync(item.projectHistoryProblem);
                            problemId = item.projectHistoryProblem.ID;

                            if (oldProblem != null)
                            {
                                var contentLog = _projectHistoryProblemLogRepo.GenerateLog(oldProblem, item.projectHistoryProblem);

                                if (!string.IsNullOrWhiteSpace(contentLog))
                                {
                                    await _projectHistoryProblemLogRepo.AddLog(
                                        problemId,
                                        contentLog,
                                        "Cập nhật phát sinh"
                                    );
                                }
                            }
                        }
                        else
                        {
                            item.projectHistoryProblem.EmployeeID = currentUser.EmployeeID;
                            await _historyproblemRepo.CreateAsync(item.projectHistoryProblem);
                            problemId = item.projectHistoryProblem.ID;

                            await _projectHistoryProblemLogRepo.AddLog(
                                problemId,
                                "Tạo mới phát sinh.",
                                "Thêm mới phát sinh"
                            );
                        }

                        lastSavedProblemId = problemId;

                        // Lưu bảng link ProjectHistoryProblemReceiverLink
                        if (problemId > 0)
                        {
                            var oldReceiverIds = _projectHistoryProblemReceiverLinkRepo
                                .GetAll(x => x.ProjectHistoryProblemID == problemId && x.IsDeleted != true)
                                .Where(x => x.ReceiverID.HasValue)
                                .Select(x => x.ReceiverID.Value)
                                .Distinct()
                                .ToList();

                            var newReceiverIds = item.receiverIds != null
                                ? item.receiverIds.Distinct().ToList()
                                : new List<int>();

                            var oldReceiverLinks = _projectHistoryProblemReceiverLinkRepo
                                .GetAll(x => x.ProjectHistoryProblemID == problemId);

                            if (oldReceiverLinks != null && oldReceiverLinks.Count > 0)
                            {
                                foreach (var oldLink in oldReceiverLinks)
                                {
                                    //await _projectHistoryProblemReceiverLinkRepo.DeleteAsync(oldLink.ID);
                                    {
                                        oldLink.IsDeleted = true;
                                        await _projectHistoryProblemReceiverLinkRepo.UpdateAsync(oldLink);
                                    }
                                }
                            }

                            if (item.receiverIds != null && item.receiverIds.Count > 0)
                            {
                                foreach (var receiverId in item.receiverIds)
                                {
                                    var newLink = new ProjectHistoryProblemReceiverLink
                                    {
                                        ProjectHistoryProblemID = problemId,
                                        ReceiverID = receiverId,
                                    };
                                    await _projectHistoryProblemReceiverLinkRepo.CreateAsync(newLink);
                                }
                            }

                            var receiverLog = _projectHistoryProblemLogRepo.GenerateListLog(
                                "người tiếp nhận",
                                oldReceiverIds,
                                newReceiverIds
                            );

                            if (!string.IsNullOrWhiteSpace(receiverLog))
                            {
                                await _projectHistoryProblemLogRepo.AddLog(
                                    problemId,
                                    receiverLog,
                                    "Cập nhật người tiếp nhận"
                                );
                            }
                        }

                        // Lưu bảng link ProjectHistoryProblemProjectItemLink
                        if (problemId > 0)
                        {
                            var oldProjectItemIds = _projectHistoryProblemProjectItemLinkRepo
                                .GetAll(x => x.ProjectHistoryProblemID == problemId && x.IsDeleted == false)
                                .Where(x => x.ProjectItemID.HasValue)
                                .Select(x => x.ProjectItemID.Value)
                                .Distinct()
                                .ToList();

                            var newProjectItemIds = item.projectItemIds != null
                                ? item.projectItemIds.Distinct().ToList()
                                : new List<int>();

                            var oldProjectItemLinks = _projectHistoryProblemProjectItemLinkRepo
                                .GetAll(x => x.ProjectHistoryProblemID == problemId && x.IsDeleted == false);

                            if (oldProjectItemLinks != null && oldProjectItemLinks.Count > 0)
                            {
                                foreach (var oldLink in oldProjectItemLinks)
                                {
                                    oldLink.IsDeleted = true;
                                    await _projectHistoryProblemProjectItemLinkRepo.UpdateAsync(oldLink);
                                }
                            }

                            if (item.projectItemIds != null && item.projectItemIds.Count > 0)
                            {
                                var validProjectItemIds = _projectItemRepo
                                    .GetAll(x => item.projectItemIds.Contains(x.ID) && x.IsDeleted == false)
                                    .Select(x => x.ID)
                                    .Distinct()
                                    .ToList();

                                foreach (var projectItemId in validProjectItemIds)
                                {
                                    var newLink = new ProjectHistoryProblemProjectItemLink
                                    {
                                        ProjectHistoryProblemID = problemId,
                                        ProjectItemID = projectItemId,
                                        IsDeleted = false
                                    };

                                    await _projectHistoryProblemProjectItemLinkRepo.CreateAsync(newLink);
                                }
                            }

                            var projectItemLog = _projectHistoryProblemLogRepo.GenerateListLog(
                                "hạng mục dự án liên quan",
                                oldProjectItemIds,
                                newProjectItemIds
                            );

                            if (!string.IsNullOrWhiteSpace(projectItemLog))
                            {
                                await _projectHistoryProblemLogRepo.AddLog(
                                    problemId,
                                    projectItemLog,
                                    "Cập nhật hạng mục liên quan"
                                );
                            }
                        }
                    }
                }
                //foreach (var item in request)
                //{
                //    if (item.detail != null && item.detail.Count > 0)
                //    {
                //        foreach (var items in item.detail)
                //        {
                //            if (items.ID > 0)
                //            {
                //                await _historyproblemDetailRepo.UpdateAsync(items);
                //            }
                //            else
                //            {
                //                await _historyproblemDetailRepo.CreateAsync(items);
                //            }
                //        }
                //    }
                //}
                //logic xóa dòng ở master, detail
                foreach (var item in request)
                {
                    if (item.deleteIdsMaster != null && item.deleteIdsMaster.Count > 0)
                        foreach (var ids in item.deleteIdsMaster)
                        {
                            ProjectHistoryProblem p = _historyproblemRepo.GetByID(ids);
                            if (p != null)
                            {
                                p.IsDeleted = true;
                                await _historyproblemRepo.UpdateAsync(p);

                                await _projectHistoryProblemLogRepo.AddLog(
                                    p.ID,
                                    "Xóa phát sinh.",
                                    "Xóa phát sinh"
                                );

                                // Xóa receiver link khi xóa master
                                var receiverLinks = _projectHistoryProblemReceiverLinkRepo.GetAll(x => x.ProjectHistoryProblemID == ids);
                                if (receiverLinks != null)
                                    foreach (var link in receiverLinks)
                                    {
                                        link.IsDeleted = true;
                                        await _projectHistoryProblemReceiverLinkRepo.UpdateAsync(link);
                                    }
                            }
                            //List<ProjectHistoryProblemDetail> d = _historyproblemDetailRepo.GetAll(x => x.ProjectHistoryProblemID == ids);
                            //if (d != null && d.Count > 0)
                            //{
                            //    foreach (var idD in d)
                            //    {
                            //        idD.IsDeleted = true;
                            //        await _historyproblemDetailRepo.UpdateAsync(idD);
                            //    }
                            //}
                        }
                    //if (item.deletedIdsDetail != null && item.deletedIdsDetail.Count > 0)
                    //{
                    //    foreach (var ids in item.deletedIdsDetail)
                    //    {
                    //        ProjectHistoryProblemDetail p = _historyproblemDetailRepo.GetByID(ids);
                    //        p.IsDeleted = true;
                    //        await _historyproblemDetailRepo.UpdateAsync(p);
                    //    }
                    //}
                }

                // Xóa file theo danh sách ID
                foreach (var item in request)
                {
                    if (item.deleteFileIds != null && item.deleteFileIds.Count > 0)
                    {
                        foreach (var fileId in item.deleteFileIds)
                        {
                            var file = _projectHistoryProblemFileRepo.GetByID(fileId);
                            if (file != null)
                            {
                                file.IsDeleted = true;
                                await _projectHistoryProblemFileRepo.UpdateAsync(file);

                                if (file.ProjectHistoryProblemID.HasValue)
                                {
                                    await _projectHistoryProblemLogRepo.AddLog(
                                        file.ProjectHistoryProblemID.Value,
                                        $"Xóa file: {file.FileName}.",
                                        "Xóa file"
                                    );
                                }
                            }
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(
                   new { id = lastSavedProblemId }, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("approve")]
        public async Task<IActionResult> Approve([FromBody] ApproveRequest request)
        {
            int id = request.Id;
            string role = request.Role; // PM, PP, TP
            bool approve = request.Approve;

            var problem = _historyproblemRepo.GetByID(id);
            if (problem == null) return NotFound();

            bool? oldApproveValue = null;
            string roleText = role;

            if (role == "PM")
            {
                oldApproveValue = problem.IsApproved_PM;
                problem.IsApproved_PM = approve;
                problem.DateApproved_PM = approve ? DateTime.Now : null;
                roleText = "PM";
            }
            else if (role == "PP")
            {
                oldApproveValue = problem.IsApproved_PP;
                problem.IsApproved_PP = approve;
                problem.DateApproved_PP = approve ? DateTime.Now : null;
                roleText = "PP";
            }
            else if (role == "TP")
            {
                oldApproveValue = problem.IsApproved_TP;
                problem.IsApproved_TP = approve;
                problem.DateApproved_TP = approve ? DateTime.Now : null;
                roleText = "TP";
            }
            else
            {
                return BadRequest(ApiResponseFactory.Fail(null, "Role không hợp lệ"));
            }

            await _historyproblemRepo.UpdateAsync(problem);

            string oldText = oldApproveValue == true ? "Đã duyệt" : "Chưa duyệt";
            string newText = approve ? "Đã duyệt" : "Hủy duyệt";

            await _projectHistoryProblemLogRepo.AddLog(
                problem.ID,
                $"- thay đổi trạng thái duyệt {roleText} từ {oldText} thành {newText}.",
                $"Duyệt {roleText}"
            );

            return Ok(ApiResponseFactory.Success(null, "Thành công"));
        }

        [HttpGet("add-problem")]
        public async Task<IActionResult> GetProjectHistoryProblemByProject(int projectID, DateTime dateProblem)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var fromDate = dateProblem.Date;
                var toDate = fromDate.AddDays(1);

                var data = _historyproblemRepo.GetAll()
                    .Where(p => p.ProjectID == projectID
                        && p.EmployeeID == currentUser.EmployeeID
                        && p.DateProblem.HasValue
                        && p.DateProblem.Value >= fromDate
                        && p.DateProblem.Value < toDate)
                    .Select(p => new
                    {
                        contentError = p.ContentError,
                        remedies = p.Remedies
                    })
                    .ToList();
                // 3. Trả về đúng danh sách data để JS loop
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("get-department-by-employees")]
        public async Task<IActionResult> GetDepartmentByEmployees([FromBody] List<int> employeeIds)
        {
            try
            {
                if (employeeIds == null || !employeeIds.Any())
                    return Ok(ApiResponseFactory.Success("", "Lấy dữ liệu thành công"));

                // 1. Lấy tất cả Employee có ID nằm trong mảng truyền lên
                var employees = _employeeRepo.GetAll(x => employeeIds.Contains(x.ID)).ToList();

                // 2. Lấy ra danh sách các DepartmentID
                var deptIds = employees
                    .Where(x => x.DepartmentID.HasValue)
                    .Select(x => x.DepartmentID.Value)
                    .Distinct()
                    .ToList();

                //// 3. Truy vấn bảng Department để lấy Name
                //var depts = _departmentRepo.GetAll(x => deptIds.Contains(x.ID))
                //    .Select(x => x.Name)
                //    .ToList();

                //var teamDepartmentStr = string.Join(", ", depts);

                return Ok(ApiResponseFactory.Success(deptIds, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-employees")]
        public IActionResult LoadUser()
        {
            try
            {
                var list = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployee",
                                 new string[] { "@Status" },
                                 new object[] { 0 });
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-departments")]
        public IActionResult GetDepartments()
        {
            try
            {
                List<Department> departments = _departmentRepo.GetAll().OrderBy(x => x.STT).ToList();
                return Ok(ApiResponseFactory.Success(departments, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-project")]
        public IActionResult LoadProject()
        {
            try
            {
                var list = _projectRepo.GetAll().Select(x => new { x.ID, x.ProjectCode, x.UserID, x.ContactID, x.CustomerID, x.ProjectName, x.PO });
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-project-items")]
        public IActionResult GetProjectItemsByProject(int projectID)
        {
            try
            {
                var data = _projectItemRepo.GetAll(x => x.ProjectID == projectID && x.IsDeleted == false)
                    .OrderBy(x => x.STT)
                    .ThenBy(x => x.ID)
                    .Select(x => new
                    {
                        x.ID,
                        x.ProjectID,
                        x.STT,
                        x.Code,
                        x.Mission,
                        x.Description,
                        x.ParentID,
                        x.Status,
                        x.TypeProjectItem,
                        x.PlanStartDate,
                        x.PlanEndDate,
                        x.ActualStartDate,
                        x.ActualEndDate,
                        x.Deadline
                    })
                    .ToList();

                return Ok(ApiResponseFactory.Success(data, "Lấy danh sách ProjectItem thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("get-data-detail")]
        public async Task<IActionResult> GetDataDetailHistoryProblem(int id)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList(
                "spGetProjectHistoryProblemLinkedData",
                new string[] { "@ProjectHistoryProblemID" },
                new object[] { id });

                var dtProjectItemLink = SQLHelper<object>.GetListData(data, 0);
                var dtWorkerVersionLink = SQLHelper<object>.GetListData(data, 1);
                var dtPartlistVersionLink = SQLHelper<object>.GetListData(data, 2);
                return Ok(ApiResponseFactory.Success(new { dtProjectItemLink, dtWorkerVersionLink, dtPartlistVersionLink },
                    "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-employee-suggest")]
        public async Task<IActionResult> GetEmployeeSuggest(int projectId)
        {
            try
            {
                var employee = SQLHelper<object>.ProcedureToList("spGetProjectParticipant",
                                            new string[] { "@ProjectID" },
                                            new object[] { projectId });
                var employees = SQLHelper<object>.GetListData(employee, 0);
                return Ok(ApiResponseFactory.Success(employees, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #region Upload và GetFiles

        [HttpPost("upload")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Upload(int requestInvoiceId, int fileType)
        {
            try
            {
                var form = await Request.ReadFormAsync();
                var key = form["key"].ToString();
                var files = form.Files;

                // Kiểm tra input
                if (string.IsNullOrWhiteSpace(key))
                    return BadRequest(ApiResponseFactory.Fail(null, "Key không được để trống!"));

                if (files == null || files.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách file không được để trống!"));

                var hp = _historyproblemRepo.GetByID(requestInvoiceId);
                if (hp == null)
                    throw new Exception("HistoryProblem not found");

                var uploadPath = _configSystemRepo.GetUploadPathByKey(key);
                if (string.IsNullOrWhiteSpace(uploadPath))
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy cấu hình đường dẫn cho key: {key}"));

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
                    targetFolder = Path.Combine(uploadPath, $"{hp.ID}");
                }

                if (!Directory.Exists(targetFolder))
                    Directory.CreateDirectory(targetFolder);

                var processedFile = new List<ProjectHistoryProblemFile>();

                foreach (var file in files)
                {
                    if (file.Length <= 0) continue;

                    // Tạo tên file unique để tránh trùng lặp
                    var fileExtension = Path.GetExtension(file.FileName);
                    var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                    var uniqueFileName = $"{originalFileName}{fileExtension}";
                    var fullPath = Path.Combine(targetFolder, uniqueFileName);

                    // Lưu file trực tiếp vào targetFolder (không tạo file tạm khác)
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var fileUpload = new ProjectHistoryProblemFile
                    {
                        ProjectHistoryProblemID = hp.ID,
                        //FileType = fileType, // Loại file của yêu cầu xuất hóa đơn : 1, loại file tờ khai xuất khẩu: 2
                        FileName = uniqueFileName,
                        OriginPath = targetFolder,
                        ServerPath = targetFolder,
                        IsDeleted = false,
                        //CreatedBy = User.Identity?.Name ?? "System",
                        //CreatedDate = DateTime.Now,
                        //UpdatedBy = User.Identity?.Name ?? "System",
                        //UpdatedDate = DateTime.Now
                    };

                    await _projectHistoryProblemFileRepo.CreateAsync(fileUpload);
                    processedFile.Add(fileUpload);

                    await _projectHistoryProblemLogRepo.AddLog(
                        hp.ID,
                        $"Upload file: {uniqueFileName}.",
                        "Upload file"
                    );
                }

                return Ok(ApiResponseFactory.Success(processedFile, $"{processedFile.Count} tệp đã được tải lên thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi upload file: {ex.Message}"));
            }
        }

        [HttpGet("get-files")]
        public IActionResult GetFiles(int requestInvoiceId)
        {
            try
            {
                var files = _projectHistoryProblemFileRepo
                    .GetAll(x => x.ProjectHistoryProblemID == requestInvoiceId && x.IsDeleted == false)
                    .Select(x => new
                    {
                        x.ID,
                        x.FileName,
                        x.ServerPath,
                        x.OriginPath,
                        x.ProjectHistoryProblemID
                    })
                    .ToList();

                return Ok(ApiResponseFactory.Success(files, "Lấy danh sách file thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion Upload và GetFiles

        #region Dashboard API

        [HttpGet("get-dashboard-department")]
        public IActionResult GetDashboardDepartment(int? projectId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                // 1. Lọc danh sách Phát sinh
                var query = _historyproblemRepo.GetAll().Where(x => x.IsDeleted != true).AsQueryable();

                if (projectId.HasValue && projectId > 0)
                {
                    query = query.Where(x => x.ProjectID == projectId.Value);
                }
                if (fromDate.HasValue)
                {
                    query = query.Where(x => x.DateProblem >= fromDate.Value.Date);
                }
                if (toDate.HasValue)
                {
                    query = query.Where(x => x.DateProblem < toDate.Value.Date.AddDays(1));
                }

                var problems = query.ToList();
                var problemIds = problems.Select(x => x.ID).ToList();

                if (!problemIds.Any())
                {
                    return Ok(ApiResponseFactory.Success(new List<object>(), "Lấy dữ liệu thành công"));
                }

                // 2. Lấy danh sách liên kết người tiếp nhận (receiver) cho các issue trên
                var receiverLinks = _projectHistoryProblemReceiverLinkRepo
                    .GetAll(x => x.ProjectHistoryProblemID != null && problemIds.Contains(x.ProjectHistoryProblemID.Value) && x.IsDeleted != true)
                    .ToList();

                // Lọc lấy các ID nhân viên (tiếp nhận)
                var receiverIds = receiverLinks.Where(x => x.ReceiverID != null).Select(x => x.ReceiverID.Value).Distinct().ToList();

                // 3. Truy xuất thông tin phòng ban từ thông tin nhân viên
                var employees = _employeeRepo.GetAll(x => receiverIds.Contains(x.ID)).ToList();

                var deptIds = employees
                    .Where(x => x.DepartmentID.HasValue)
                    .Select(x => x.DepartmentID.Value)
                    .Distinct()
                    .ToList();

                var departments = _departmentRepo.GetAll(x => deptIds.Contains(x.ID)).ToList();

                // 4. Bắt đầu thống kê tổng hợp gửi ra cho giao diện
                var dashboardData = new List<object>();

                foreach (var dept in departments)
                {
                    // Tìm tất cả nhân viên thuộc phòng ban này trong danh sách receiver
                    var empInDept = employees.Where(x => x.DepartmentID == dept.ID).Select(x => x.ID).ToList();

                    // Đếm các phát sinh CÓ người tiếp nhận nằm trong phòng ban (sử dụng Distinct để không đếm trùng issue)
                    var totalProblems = receiverLinks
                        .Where(x => x.ReceiverID != null && empInDept.Contains(x.ReceiverID.Value))
                        .Select(x => x.ProjectHistoryProblemID)
                        .Distinct()
                        .Count();

                    if (totalProblems > 0)
                    {
                        dashboardData.Add(new
                        {
                            DepartmentID = dept.ID,
                            DepartmentName = dept.Name,
                            TotalProblems = totalProblems
                        });
                    }
                }

                // Sắp xếp các phòng ban có sự cố nhiều nhất lên đầu
                var sortedData = dashboardData.OrderByDescending(x => (int)((dynamic)x).TotalProblems).ToList();

                return Ok(ApiResponseFactory.Success(sortedData, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-dashboard-month")]
        public IActionResult GetDashboardMonth(int? projectId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                // 1. Lọc danh sách Phát sinh theo tiêu chí
                var query = _historyproblemRepo.GetAll().Where(x => x.IsDeleted != true).AsQueryable();

                if (projectId.HasValue && projectId > 0)
                {
                    query = query.Where(x => x.ProjectID == projectId.Value);
                }
                if (fromDate.HasValue)
                {
                    query = query.Where(x => x.DateProblem >= fromDate.Value.Date);
                }
                if (toDate.HasValue)
                {
                    query = query.Where(x => x.DateProblem < toDate.Value.Date.AddDays(1));
                }

                var problems = query.ToList();

                // 2. Gom nhóm đếm số lượng sự cố theo Năm và Tháng
                var dashboardData = problems
                    .Where(x => x.DateProblem.HasValue)
                    .GroupBy(x => new { x.DateProblem.Value.Year, x.DateProblem.Value.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        TotalProblems = g.Count()
                    })
                    // Sắp xếp theo trình tự thời gian tăng dần để Line Chart vẽ tịnh tiến
                    .OrderBy(x => x.Year).ThenBy(x => x.Month)
                    .ToList();

                return Ok(ApiResponseFactory.Success(dashboardData, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-dashboard-status")]
        public IActionResult GetDashboardStatus(int? projectId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                // 1. Lọc danh sách Phát sinh
                var query = _historyproblemRepo.GetAll().Where(x => x.IsDeleted != true).AsQueryable();

                if (projectId.HasValue && projectId > 0)
                {
                    query = query.Where(x => x.ProjectID == projectId.Value);
                }
                if (fromDate.HasValue)
                {
                    query = query.Where(x => x.DateProblem >= fromDate.Value.Date);
                }
                if (toDate.HasValue)
                {
                    query = query.Where(x => x.DateProblem < toDate.Value.Date.AddDays(1));
                }

                var problems = query.ToList();

                // 2. Gom nhóm theo trạng thái
                var dashboardData = problems
                    .Where(x => x.StatusProblem.HasValue)
                    .GroupBy(x => x.StatusProblem)
                    .Select(g => new
                    {
                        Status = g.Key.Value,
                        StatusName = g.Key.Value == 1 ? "Chờ xử lý" :
                                     g.Key.Value == 2 ? "Đang xử lý" :
                                     g.Key.Value == 3 ? "Đã xử lý" : "Khác",
                        TotalProblems = g.Count()
                    })
                    .ToList();

                return Ok(ApiResponseFactory.Success(dashboardData, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion Dashboard API

        #region SendMail

        [HttpPost("send-email-problem")]
        public async Task<IActionResult> SendEmailProblem([FromBody] SendEmailProblemRequest request)
        {
            try
            {
                var problem = _historyproblemRepo.GetByID(request.ProblemId);
                if (problem == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy thông tin sự cố/phát sinh!"));

                // Lấy thông tin dự án để hiển thị ở tiêu đề
                var project = _projectRepo.GetByID(problem.ProjectID ?? 0);
                string projectName = project != null ? project.ProjectName : "Không xác định";
                string projectCode = project != null ? project.ProjectCode : "Không xác định";

                // Sử dụng HashSet để lưu danh sách ID nhân viên (loại bỏ tự động các ID trùng lặp)
                var employeeIdsToEmail = new HashSet<int>();

                // 1. Thêm Người Tạo
                if (problem.CreatorID.HasValue) employeeIdsToEmail.Add(problem.CreatorID.Value);

                // 2. Thêm Người Thực Hiện
                if (problem.PerformerID.HasValue) employeeIdsToEmail.Add(problem.PerformerID.Value);

                // 3. Thêm danh sách Người Tiếp Nhận từ bảng Link
                var receiverLinks = _projectHistoryProblemReceiverLinkRepo
                    .GetAll(x => x.ProjectHistoryProblemID == request.ProblemId && x.IsDeleted != true)
                    .ToList();

                foreach (var link in receiverLinks)
                {
                    if (link.ReceiverID.HasValue)
                    {
                        employeeIdsToEmail.Add(link.ReceiverID.Value);
                    }
                }

                if (!employeeIdsToEmail.Any())
                {
                    return Ok(ApiResponseFactory.Success(null, "Không có người nhận mail nào liên quan đến phát sinh này."));
                }

                // Truy vấn bảng Employee để lấy danh sách Email Công ty
                var employees = _employeeRepo.GetAll(e => employeeIdsToEmail.Contains(e.ID)).ToList();
                var emails = employees
                    .Select(e => e.EmailCongTy)
                    .Where(e => !string.IsNullOrWhiteSpace(e))
                    .Distinct()
                    .ToList();

                if (!emails.Any())
                {
                    return Ok(ApiResponseFactory.Success(null, "Các nhân viên liên quan hiện chưa có địa chỉ Email Công Ty trong hệ thống."));
                }

                string issueLogTypeText = problem.IssueLogType switch
                {
                    1 => "Khách hàng",
                    2 => "Nội bộ",
                    3 => "Nhà cung cấp",
                    _ => ""
                };

                string statusProblemText = problem.StatusProblem switch
                {
                    1 => "Chờ xử lý",
                    2 => "Đang xử lý",
                    3 => "Đã xử lý",
                    _ => ""
                };

                string priorityText = problem.PriorityLevel switch
                {
                    1 => "Thấp",
                    2 => "Trung bình",
                    3 => "Cao",
                    4 => "Rất cao",
                    _ => ""
                };

                // Gom danh sách Email: Lấy người đầu tiên gán cho Email To, phần còn lại gán vào Email CC
                string emailTo = emails.First();
                string emailCc = emails.Count > 1 ? string.Join(",", emails.Skip(1)) : "";

                //string emailTo = "tuananh.ng011004@gmail.com";
                //string emailCc = "nhubinh2104@gmail.com";

                // Xây dựng Tiêu đề và Nội dung HTML
                string subject = $"THÔNG BÁO PHÁT SINH DỰ ÁN {projectCode} - {projectName}".ToUpper();
                string body = $@"
            <div style='font-family: Arial, sans-serif; line-height: 1.6;'>
                <h2 style='color: #d9534f;'>THÔNG BÁO CÓ PHÁT SINH DỰ ÁN MỚI</h2>
                <p><strong>Dự án:</strong>{projectCode} - {projectName}</p>
                <p><strong>Ngày phát sinh:</strong> {(problem.DateProblem.HasValue ? problem.DateProblem.Value.ToString("dd/MM/yyyy") : "")}</p>
                <p><strong>Mức độ nghiêm trọng:</strong> {priorityText}</p>
                <p><strong>Loại phát sinh:</strong> {issueLogTypeText}</p>
                <p><strong>Trạng thái xử lý:</strong> {statusProblemText}</p>
                <p><strong>Nội dung sự cố:</strong> {problem.ContentError}</p>
                <p><strong>Nguyên nhân:</strong> {problem.Reason}</p>
                <p><strong>Phương án xử lý:</strong> {problem.Remedies}</p>
                <p><strong>PIC:</strong> {problem.PIC}</p>
                <hr/>
                <p><small>Đây là email thông báo tự động từ hệ thống R-ERP, vui lòng không phản hồi lại email này.</small></p>
            </div>";

                // Gửi mail
                await _emailHelper.SendAsync(emailTo, subject, body, true, emailCc);

                return Ok(ApiResponseFactory.Success(new
                {
                    SentTo = emailTo,
                    SentCc = emailCc,
                    Subject = subject
                }, "Gửi email thông báo phát sinh thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi hệ thống khi gửi email: {ex.Message}"));
            }
        }

        #endregion SendMail

        #region GetLog

        [HttpGet("get-log")]
        public IActionResult GetLog(int projectHistoryProblemID)
        {
            try
            {
                var logs = _projectHistoryProblemLogRepo
                    .GetAll(x => x.ProjectHistoryProblemID == projectHistoryProblemID && x.IsDeleted != true)
                    .OrderByDescending(x => x.CreatedDate)
                    .Select(x => new
                    {
                        x.ID,
                        x.ProjectHistoryProblemID,
                        x.TypeLog,
                        x.ContentLog,
                        x.CreatedBy,
                        x.CreatedDate
                    })
                    .ToList();

                return Ok(ApiResponseFactory.Success(logs, "Lấy log thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion GetLog

        public class ApproveRequest
        {
            public int Id { get; set; }
            public string Role { get; set; }
            public bool Approve { get; set; }
        }

        public class SendEmailProblemRequest
        {
            public int ProblemId { get; set; }
        }
    }
}