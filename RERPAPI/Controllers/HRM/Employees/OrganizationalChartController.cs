using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.Record.Chart;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.Asset;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Asset;

namespace RERPAPI.Controllers.HRM.Employees
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationalChartController : ControllerBase
    {
        private OrganizationalChartDetailRepo _organizationalChartDetailRepo;
        private OrganizationalChartRepo _organizationalChartRepo;

        public OrganizationalChartController(OrganizationalChartDetailRepo organizationalChartDetailRepo,
            OrganizationalChartRepo organizationalChartRepo)
        {
            _organizationalChartDetailRepo = organizationalChartDetailRepo;
            _organizationalChartRepo = organizationalChartRepo;
        }

        [HttpGet("get-organization-chart")]
        //[RequiresPermission("N2,N23,N34,N1,N52,N80")]
        public IActionResult GetOrganizationalChart()
        {
            try
            {
                var dt = SQLHelper<object>.ProcedureToList("spGetOrganizationalChart", new string[] { "@TaxCompanyID", "@DepartmentID" }, new object[] { 0, 0 });
                var result = SQLHelper<object>.GetListData(dt, 0);

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-organization-chart-detail")]
        //[RequiresPermission("N2,N23,N34,N1,N52,N80")]
        public IActionResult GetOrganizationalChartDetail(int id)
        {
            try
            {
                var dt = SQLHelper<object>.ProcedureToList("spGetOrganizationalChartDetail", new string[] { "@ID" }, new object[] { id });
                var result = SQLHelper<object>.GetListData(dt, 0);

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] OrganizationalChartDTO dto)
        {
            try
            {
                var failedEmployees = new List<OrganizationalChartDetail>();
                if (dto.organizationalCharts != null && dto.organizationalCharts.Any())
                {
                    foreach (var item in dto.organizationalCharts)
                    {
                        if (item.ID <= 0)
                            await _organizationalChartRepo.CreateAsync(item);
                        else
                            await _organizationalChartRepo.UpdateAsync(item);
                    }
                }


                if (dto.organizationalChartDetails != null && dto.organizationalChartDetails.Any())
                {
                    foreach (var item in dto.organizationalChartDetails)
                    {
                        var employeeExist = _organizationalChartDetailRepo
                            .GetAll(x => x.OrganizationalChartID == item.OrganizationalChartID
                                      && x.EmployeeID == item.EmployeeID);

                        if (employeeExist.Any())
                        {
                            failedEmployees.Add(item);
                            continue;
                        }

                        if (item.ID <= 0)
                            await _organizationalChartDetailRepo.CreateAsync(item);
                        else
                            await _organizationalChartDetailRepo.UpdateAsync(item);
                    }
                }

                return Ok(ApiResponseFactory.Success( failedEmployees, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}
