using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.Record.Chart;
using RERPAPI.Middleware;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.GeneralCatetogy.JobRequirements;
using RERPAPI.Repo.GenericEntity.HRM;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.GeneralCategory
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class JobRequirementController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private CurrentUser _currentUser;
        private readonly JobRequirementRepo _jobRepo;
        private readonly JobRequirementDetailRepo _detailRepo;
        private JobRequirementFileRepo _fileRepo;
        private JobRequirementApprovedRepo _approvedRepo;
        private EmployeeRepo _employeeRepo;
        private JobRequirementCommentRepo _commentRepo;
        private vUserGroupLinksRepo _vUserGroupLinksRepo;



        public JobRequirementController(IConfiguration configuration, CurrentUser currentUser, JobRequirementRepo jobRepo, JobRequirementDetailRepo detailRepo, JobRequirementFileRepo jobRequirementFileRepo, JobRequirementApprovedRepo approvedRepo, EmployeeRepo employeeRepo, JobRequirementCommentRepo commentRepo, vUserGroupLinksRepo userLinkRepo)
        {
            _configuration = configuration;
            _currentUser = currentUser;
            _jobRepo = jobRepo;
            _detailRepo = detailRepo;
            _fileRepo = jobRequirementFileRepo;
            _approvedRepo = approvedRepo;
            _employeeRepo = employeeRepo;
            _commentRepo = commentRepo;
            _vUserGroupLinksRepo = userLinkRepo;
        }


        [HttpPost("")]
        public IActionResult GetAll([FromBody] JobRequirementParam param)
        {
            try
            {
                param.DateStart = new DateTime(param.DateStart.Year, param.DateStart.Month, param.DateStart.Day, 0, 0, 0);
                param.DateEnd = new DateTime(param.DateEnd.Year, param.DateEnd.Month, param.DateEnd.Day, 23, 59, 59);
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                var vUserViewAllYCCV = _vUserGroupLinksRepo.GetAll().FirstOrDefault(x => (
                                                           x.Code == "N1"
                                                        || x.Code == "N2"
                                                        || x.Code == "N32"
                                                        || x.Code == "N34"
                                                        || x.Code == "N57"
                                                        || x.Code == "N58"
                                                        || x.Code == "N59"
                                                        || x.Code == "N56") &&
                                                           x.UserID == currentUser.ID);
                int employeeID;
                if (vUserViewAllYCCV != null)
                {
                    employeeID = param.EmployeeID;
                }
                else
                {
                    employeeID = currentUser.EmployeeID;
                }
                var data = SQLHelper<object>.ProcedureToList("spGetJobRequirement   ",
                                                            new string[] { "@DateStart", "@DateEnd", "@Request", "@EmployeeId", "@Step", "@DepartmentId", "@ApprovedTBPID" },
                                                            new object[] { param.DateStart, param.DateEnd, param.Request, employeeID, param.Step, param.DepartmentID, param.ApprovedTBPID });

                var jobs = SQLHelper<object>.GetListData(data, 0);

                return Ok(ApiResponseFactory.Success(jobs, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpGet("details/{jobRequirementID}")]
        public IActionResult GetDetails(int jobRequirementID)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetJobRequirementDetail",
                                                            new string[] { "@JobRequirementID" },
                                                            new object[] { jobRequirementID });

                var details = SQLHelper<object>.GetListData(data, 0);
                var approves = SQLHelper<object>.GetListData(data, 1);
                var files = SQLHelper<object>.GetListData(data, 2);
                var detailsCategory = SQLHelper<object>.GetListData(data, 4);

                return Ok(ApiResponseFactory.Success(new
                {
                    details,
                    approves,
                    files,
                    detailsCategory
                }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetByID(int id)
        {
            try
            {
                var job = _jobRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(job, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] JobRequirementDTO job)
        {
            try
            {
                //  _currentUser = HttpContext.Session.GetObject<CurrentUser>(_configuration.GetValue<string>("SessionKey"));
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                //Update master
                bool isNew = job.ID <= 0;
                if (isNew)
                {
                    job.NumberRequest = "";
                    job.EmployeeID = job.EmployeeID;
                    int currentYear = DateTime.Now.Year;
                    var list = _jobRepo.GetAll(x => x.DateRequest.Value.Year == currentYear);
                    job.NumberRequest = $"{list.Count + 1}.{currentYear}.PYC-RTC";
                    var result = await _jobRepo.CreateAsync(job);
                   
                }
                else if (job.EmployeeID != currentUser.EmployeeID)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Bạn không thể sửa phiếu của người khác"));
                }
                else await _jobRepo.UpdateAsync(job);
                await _approvedRepo.CreateJobRequirementApproved(job.ApprovedTBPID ?? 0, job);
                // Thêm detail
                foreach (var item in job.JobRequirementDetails)
                {
                    item.JobRequirementID = job.ID;
                    if (item.ID <= 0) await _detailRepo.CreateAsync(item);
                    else await _detailRepo.UpdateAsync(item);
                }
                //thêm file
                foreach (var item in job.JobRequirementFiles)
                {
                    item.JobRequirementID = job.ID;
                    if (item.ID <= 0) await _fileRepo.CreateAsync(item);
                    else await _fileRepo.UpdateAsync(item);
                }
                if (isNew)
                {
                    _jobRepo.SendMail(job);
                }
                return Ok(ApiResponseFactory.Success(job, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] List<int> ids)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                if (ids == null || ids.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn yccv để xóa"));
                foreach (var item in ids)
                {

                    var jobRe = _jobRepo.GetByID(item);
                    var jobReApprove = _approvedRepo.GetAll(x => x.JobRequirementID == jobRe.ID && x.IsApproved == 1 && x.Step != 1);
                    if (jobReApprove.Any())
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, $"Phiếu yêu cầu công việc [{jobRe.NumberRequest}] đã được duyệt, không thể xóa"));
                    }
                    else if (jobRe.EmployeeID != currentUser.EmployeeID)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, $"Bạn không thể xóa phiếu yêu cầu công việc của người khác"));
                    }
                    jobRe.IsDeleted = true;
                    await _jobRepo.UpdateAsync(jobRe);
                }
                return Ok(ApiResponseFactory.Success(ids, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("approve")]
        public async Task<IActionResult> Approve([FromBody] List<JobRequirementApproveDTO> list)
        {
            var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
            var currentUser = ObjectMapper.GetCurrentUser(claims);
            var result = new List<object>();
            foreach (var param in list)
            {
                try
                {
                    var job = _jobRepo.GetByID(param.JobRequirementID);
                    if (job == null)
                    {
                        result.Add(new { param.JobRequirementID, Success = false, Message = "Phiếu không tồn tại" });
                        continue;
                    }

                    var approves = _approvedRepo
                        .GetAll(x => x.JobRequirementID == job.ID)
                        .OrderBy(x => x.Step)
                        .ToList();

                    var currentApprove = approves.FirstOrDefault(x => x.Step == param.Step);
                    if (currentApprove == null)
                    {
                        result.Add(new { job.ID, Success = false, Message = "Không tìm thấy bước duyệt" });
                        continue;
                    }

                    // check step trước
                    if (param.Step > 1)
                    {
                        var prevStep = approves.First(x => x.Step == param.Step - 1);
                        if (prevStep.IsApproved != 1)
                        {
                            result.Add(new { job.ID, Success = false, Message = $" Số yêu cầu {job.NumberRequest} chưa được  duyệt, không thể duyệt" });
                            continue;
                        }
                    }

                    // check đã duyệt chưa
                    if (currentApprove.IsApproved == 1 && param.Status == 1)
                    {
                        result.Add(new { job.ID, Success = false, Message = $"Số yêu cầu {job.NumberRequest} đã duyệt, không thể duyệt lại" });
                        continue;
                    }

                    // phân quyền step đặc biệt (VD TBP)
                    if (param.Step == 2 &&
                        job.ApprovedTBPID != currentUser.EmployeeID &&
                        !currentUser.IsAdmin)
                    {
                        result.Add(new { job.ID, Success = false, Message = "Bạn không phải TBP nên không có quyền duyệt" });
                        continue;
                    }

                    // xử lý duyệt / huỷ
                    if (param.Status == 2)
                    {
                        if (string.IsNullOrEmpty(param.ReasonCancel))
                        {
                            result.Add(new { job.ID, Success = false, Message = "Vui lòng nhập lý do huỷ" });
                            continue;
                        }

                        currentApprove.IsApproved = 2;
                        currentApprove.ReasonCancel = param.ReasonCancel;
                    }
                    else
                    {
                        currentApprove.IsApproved = 1;
                    }

                    currentApprove.DateApproved = DateTime.Now;
                    currentApprove.ApprovedActualID = currentUser.EmployeeID;

                    await _approvedRepo.UpdateAsync(currentApprove);

                    result.Add(new { job.ID, Success = true });
                }
                catch (Exception ex)
                {
                    result.Add(new { param.JobRequirementID, Success = false, Message = ex.Message });
                }
            }

            return Ok(ApiResponseFactory.Success(result, "Xử lý duyệt hoàn tất"));
        }
        [HttpPost("save-request-bgd-approve")]
        public async Task<IActionResult> SaveRequestBGDApprove([FromBody] JobRequirement model)
        {
            try
            {
                if (model != null)
                {
                    string isRequestText = model.IsRequestBGDApproved ?? false ? "yêu cầu" : "huỷ yêu cầu";
                    if (model.ID > 0)
                    {
                        await _jobRepo.UpdateAsync(model);
                    }
                }

                return Ok(ApiResponseFactory.Success(model, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-comment")]
        public async Task<IActionResult> SaveComment([FromBody] JobRequirement model)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                JobRequirementComment comment = new JobRequirementComment();
                comment.JobRequirementID = model.ID;
                comment.DateComment = DateTime.Now;
                comment.EmployeeID = currentUser.EmployeeID;
                comment.CommentContent = model.Note;
                await _commentRepo.CreateAsync(comment);
                return Ok(ApiResponseFactory.Success(model, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("get-project-partlist-purchase-request")]
        public IActionResult GetProjectPartlistPurchaseRequest([FromBody] JobProjectPartlistPurchaseRequestParam request)
        {
            try
            {
                var ds = request.DateStart.AddHours(00).AddMinutes(00).AddSeconds(00); // 00:00:00
                var de = request.DateEnd.AddHours(23).AddMinutes(59).AddSeconds(59); // 23:59:59
                var data = SQLHelper<object>.ProcedureToList("spGetProjectPartlistPurchaseRequest_New_Khanh",
                                                             new string[] { "@DateStart", "@DateEnd", "@Keyword", "@JobRequirementID" },
                                                             new object[] { ds, de, request.KeyWord ?? "", request.JobRequirementID });
                var dataList = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(dataList, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("get-summary-jobrequirement")]
        public IActionResult GetSummaryJobRequireMent([FromBody] SummaryJobrequirementRequestParam request)
        {
            try
            {
                var ds = request.DateStart.AddHours(00).AddMinutes(00).AddSeconds(00); // 00:00:00
                var de = request.DateEnd.AddHours(23).AddMinutes(59).AddSeconds(59); // 23:59:59
                var data = SQLHelper<object>.ProcedureToList("spGetSumarizeJobrequirement",
                                                             new string[] { "@DateStart", "@DateEnd", "@Request", "@EmployeeId", "@Step", "@DepartmentId" },
                                                             new object[] { ds, de, request.request ?? "", request.EmployeeID ?? 0, request.Step ?? 0, request.DepartmentID });
                var dataList = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(dataList, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-all")]
        public IActionResult GetAllJobRequirement()
        {
            try
            {
                var job = _jobRepo.GetAll(x => x.IsDeleted != true);

                return Ok(ApiResponseFactory.Success(job, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
