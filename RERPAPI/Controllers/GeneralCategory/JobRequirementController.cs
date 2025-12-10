using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Middleware;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity.GeneralCatetogy.JobRequirements;
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

        public JobRequirementController(IConfiguration configuration, CurrentUser currentUser, JobRequirementRepo jobRepo, JobRequirementDetailRepo detailRepo)
        {
            _configuration = configuration;
            _currentUser = currentUser;
            _jobRepo = jobRepo;
            _detailRepo = detailRepo;
        }


        [HttpPost("")]
        public IActionResult GetAll([FromBody] JobRequirementParam param)
        {
            try
            {
                param.DateStart = new DateTime(param.DateStart.Year, param.DateStart.Month, param.DateStart.Day, 0, 0, 0);
                param.DateEnd = new DateTime(param.DateEnd.Year, param.DateEnd.Month, param.DateEnd.Day, 23, 59, 59);

                var data = SQLHelper<object>.ProcedureToList("spGetJobRequirement",
                                                            new string[] { "@DateStart", "@DateEnd", "@Request", "@EmployeeId", "@Step", "@DepartmentId", "@ApprovedTBPID" },
                                                            new object[] { param.DateStart, param.DateEnd, param.Keyword, param.EmployeeID, param.Step, param.DepartmentID, param.ApprovedTBPID });

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


        [HttpGet("save-data")]
        public async Task<IActionResult> SaveData([FromBody] JobRequirementDTO job)
        {
            try
            {
                _currentUser = HttpContext.Session.GetObject<CurrentUser>(_configuration.GetValue<string>("SessionKey"));

                //Update master
                if (job.ID <= 0)
                {
                    job.NumberRequest = "";
                    job.EmployeeID = _currentUser.EmployeeID;

                    await _jobRepo.CreateAsync(job);
                }
                else await _jobRepo.UpdateAsync(job);

                //Update detail
                foreach (var item in job.JobRequirementDetails)
                {
                    item.JobRequirementID = job.ID;
                    if (item.ID <= 0) await _detailRepo.CreateAsync(item);
                    else await _detailRepo.UpdateAsync(item);
                }

                return Ok(ApiResponseFactory.Success(job, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
