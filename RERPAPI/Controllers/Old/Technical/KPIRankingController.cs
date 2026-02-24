using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.Technical
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KPIRankingController : ControllerBase
    {
        private readonly DepartmentRepo _departmentRepo;
        public KPIRankingController(DepartmentRepo departmentRepo)
        {
            _departmentRepo = departmentRepo;
        }

        [HttpGet("get-department")]
        public IActionResult GetDepartment()
        {
            try
            {
                var data = _departmentRepo
                    .GetAll(x => x.IsDeleted != true)
                    .OrderBy(x => x.STT);

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-data")]
        public async Task<IActionResult> GetData(int year, int quarter, int departmentId, int kpiLevel)
        {
            try
            {
                var param = new
                {  
                    Year = year,
                    Quarter = quarter,
                    DepartmentID = departmentId,
                    KpiLevel = kpiLevel
                };
                var dataStore = await SqlDapper<object>.ProcedureToListAsync("spGetKPIRanking", param);


                return Ok(ApiResponseFactory.Success(dataStore, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
