using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.KPISALE
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DailyReportSaleController : ControllerBase
    {
        private readonly DailyReportSaleRepo _dailyReportSaleRepo;
        private readonly ProjectRepo _projectRepo;
        private readonly CustomerRepo _customerRepo;
        private readonly EmployeeTeamSaleRepo _employeeTeamSaleRepo;
        private readonly GroupSaleRepo _groupSaleRepo;
        public DailyReportSaleController(DailyReportSaleRepo dailyReportSaleRepo, ProjectRepo projectRepo, CustomerRepo customerRepo, GroupSaleRepo groupSaleRepo)
        {
            _dailyReportSaleRepo = dailyReportSaleRepo;
            _projectRepo = projectRepo;
            _customerRepo = customerRepo;
            _groupSaleRepo = groupSaleRepo;
        }

        [HttpGet("get-projects")]
        public IActionResult GetProjects() {
            try
            {
                var result = _projectRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-employee-team-sale")]
        public IActionResult GetEmployeeTeamSale()
        {
            try
            {
                var result = _employeeTeamSaleRepo.GetAll(x => x.IsDeleted != 1 && x.ParentID == 0);

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-group-sale")]
        public IActionResult GetGroupSale(int userId)
        {
            try
            {
                var result = SQLHelper<dynamic>.ProcedureToList("spGetGroupSalesByUserID",
                                new string[] { "@UserID" },
                                new object[] { "" });
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-customers")]
        public IActionResult GetCustomers()
        {
            try
            {
                var result = _customerRepo.GetAll(x => x.IsDeleted != true).Select(e => new
                {
                    CustomerName = e.CustomerName,
                    ID = e.ID,
                });

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}
