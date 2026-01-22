using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.HRM;

namespace RERPAPI.Controllers.HRM
{
    [Route("api/[controller]")]
    [ApiController]

    public class EmployeeLuckyNumberController : ControllerBase
    {
        private EmployeeLuckyNumberRepo _employeeLucky;

        public EmployeeLuckyNumberController(EmployeeLuckyNumberRepo employeeLucky)
        {
            _employeeLucky = employeeLucky;
        }


        [HttpGet("{number}")]
        public IActionResult GetByNumber(int number)
        {
            try
            {
                int year = 2025;
                var employeeLucky = _employeeLucky.GetAll(x => x.YearValue == year
                                                            && x.LuckyNumber == number)
                                                  .FirstOrDefault() ?? new EmployeeLuckyNumber();
                return Ok(ApiResponseFactory.Success(employeeLucky, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
