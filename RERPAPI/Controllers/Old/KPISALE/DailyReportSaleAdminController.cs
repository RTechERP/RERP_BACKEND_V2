using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RERPAPI.Controllers.Old.KPISALE
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DailyReportSaleAdminController : ControllerBase
    {
        private readonly CustomerRepo _customerRepo;
        private readonly ReportTypeRepo _reportTypeRepo;
        private readonly ProjectRepo _projectRepo;
        private readonly DailyReportSaleAdminRepo _dailyReportSaleAdminRepo;
        public DailyReportSaleAdminController(CustomerRepo customerRepo, ReportTypeRepo reportTypeRepo, ProjectRepo projectRepo, DailyReportSaleAdminRepo dailyReportSaleAdminRepo, ReportTypeRepo reportType)
        {
            _customerRepo = customerRepo;
            _reportTypeRepo = reportTypeRepo;
            _projectRepo = projectRepo;
            _dailyReportSaleAdminRepo = dailyReportSaleAdminRepo;
            _reportTypeRepo = reportType;
        }
        [HttpGet("load-data")]
        public IActionResult Get(DateTime dateStart, DateTime dateEnd, int customerId, int userId, string keyword = "" )
        {
            try
            {
                var list = SQLHelper<dynamic>.ProcedureToList("SPGetDailyReportAdmin", 
                    new string[] { "@TimeStart", "@TimeEnd", "@CustomerID", "@EmployeeID", "@ID", "@KeyWord" }, 
                    new object[] { dateStart, dateEnd, customerId, userId, 0 , keyword });

                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-user")]
        public IActionResult LoadUser()
        {
            try
            {
                var list = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployee",
                                 new string[] { "@Keyword", "@Status" },
                                 new object[] { "", 0 });
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-customers")]
        public IActionResult LoadCustomers()
        {
            try
            {
                var data = _customerRepo.GetAll(x => x.IsDeleted != true );
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-details")]
        public IActionResult LoadDailyReportSaleAdminDetail(int id)
        {
            try
            {
                var list = SQLHelper<dynamic>.ProcedureToList("SPDailyReportSaleAdminGetByID",
                    new string[] { "@ID" },
                    new object[] { id });

                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-reporttypes")]

        public IActionResult GetReportTypes()
        {
            try
            {
                var list = _reportTypeRepo.GetAll();
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-projects")]

        public IActionResult GetProjects()
        {
            try
            {
                var list = _projectRepo.GetAll(x => x.IsDeleted != true).OrderByDescending(x=>x.ID).ToList();
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> Save(DailyReportSaleAdminDTO dto)
        {
            try
            {
                foreach(var item in dto.request)
                {
                    DailyReportSaleAdmin model = item.ID > 0 ? await _dailyReportSaleAdminRepo.GetByIDAsync(item.ID) : new DailyReportSaleAdmin();
                    model.ID = item.ID;
                    model.PlanNextDay = item.PlanNextDay;
                    model.Problem = item.Problem;
                    model.ProblemSolve = item.ProblemSolve;
                    model.ReportContent = item.ReportContent;
                    model.Result = item.Result;
                    model.EmployeeID = item.EmployeeID;
                    model.EmployeeRequestID = item.EmployeeRequestID;
                    model.CustomerID = item.CustomerID;
                    model.ReportTypeID = item.ReportTypeID;
                    model.DateReport = item.DateReport;
                    model.ProjectID = item.ProjectID;
                    if (item.ID > 0)
                    {
                        await _dailyReportSaleAdminRepo.UpdateAsync(item);
                    }
                    else
                    {
                        await _dailyReportSaleAdminRepo.CreateAsync(item);
                    }
                }    

                foreach (int idDels in dto.IdsDel)
                {
                    if(idDels > 0)
                    {
                        await _dailyReportSaleAdminRepo.DeleteAsync(idDels);
                    }
                }    
                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("delete")]
        public IActionResult Delete (int id)
        {
                try
                {
                    _dailyReportSaleAdminRepo.Delete(id);
                    return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
                }
                catch (Exception ex)
                {
                    return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
                }
        }

        [HttpPost("save-reporttype")]
        public IActionResult SaveReportType(ReportType model)
        {
            try
            {
                var exist = _reportTypeRepo.GetAll(x => x.ReportTypeName.ToLower().Trim() == model.ReportTypeName.ToLower().Trim());
                if (exist != null && exist.Count > 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, ""));
                }
                ReportType data = new ReportType();
                data.ReportTypeName = model.ReportTypeName;
                _reportTypeRepo.Create(data);
                return Ok(ApiResponseFactory.Success("", "Lưu thành công"));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        public class DailyReportSaleAdminDTO
        {
            public List<DailyReportSaleAdmin> request { get; set; }
            public List<int> IdsDel { get; set; }
        }
    }
}
