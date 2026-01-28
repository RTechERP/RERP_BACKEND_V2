using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.HRM;
using SixLabors.ImageSharp;
using System.Threading.Tasks;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RERPAPI.Controllers.HRM
{
    [Route("api/[controller]")]
    [ApiController]

    public class EmployeeLuckyNumberController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private CurrentUser _currentUser;

        private EmployeeLuckyNumberRepo _employeeLucky;

        public EmployeeLuckyNumberController(IConfiguration configuration, CurrentUser currentUser, EmployeeLuckyNumberRepo employeeLucky)
        {
            _configuration = configuration;
            _currentUser = currentUser;
            _employeeLucky = employeeLucky;

        }


        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetByID(int id)
        {
            try
            {
                var employeeLucky = _employeeLucky.GetByID(id);
                return Ok(ApiResponseFactory.Success(employeeLucky, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("number/{number}")]
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

        [HttpGet("")]
        [Authorize]
        public async Task<IActionResult> GetAllAsync(int? year, int? departmentID, int? employeeID, string? keyword)
        {
            try
            {
                var param = new
                {
                    Year = year ?? new DateTime().Year,
                    DepartmentID = departmentID ?? 0,
                    EmployeeID = employeeID ?? 0,
                    Keyword = keyword ?? ""
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetEmployeeLuckyNumber", param);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("save-data")]
        [Authorize]
        public async Task<IActionResult> SaveData([FromBody] EmployeeLuckyNumber employeeLucky)
        {
            try
            {
                var employee = _employeeLucky.GetAll(x => x.YearValue == employeeLucky.YearValue
                                                        && x.EmployeeID == employeeLucky.EmployeeID
                                                        && (x.LuckyNumber == employeeLucky.LuckyNumber || employeeLucky.LuckyNumber == 0))
                                            .FirstOrDefault() ?? new EmployeeLuckyNumber();
                int record = 0;
                if (employee.ID <= 0)
                {
                    record = await _employeeLucky.CreateAsync(employeeLucky);
                }
                else
                {
                    employee.EmployeeID = employeeLucky.EmployeeID;
                    employee.EmployeeCode = employeeLucky.EmployeeCode;
                    employee.EmployeeName = employeeLucky.EmployeeName;
                    employee.PhoneNumber = employeeLucky.PhoneNumber;
                    employee.YearValue = employeeLucky.YearValue;
                    employee.LuckyNumber = employeeLucky.LuckyNumber;
                    employee.IsChampion = employeeLucky.IsChampion;
                    employee.ImageName = employeeLucky.ImageName;
                    record = await _employeeLucky.UpdateAsync(employee);
                }

                if (record > 0) return Ok(ApiResponseFactory.Success(employeeLucky, "Câp nhật thành công!"));
                else return BadRequest(ApiResponseFactory.Fail(null, "Câp nhật thất bại!", employeeLucky));


            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpGet("get-random-number")]
        [Authorize]
        public IActionResult GetRandomNumber(int year)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                _currentUser = ObjectMapper.GetCurrentUser(claims);

                var luckyNumbers = _employeeLucky.GetAll(x => x.YearValue == year);
                //int minValue = _configuration.GetValue<int>("MinValue");
                int minValue = 1;
                int maxValue = luckyNumbers.Count();

                EmployeeLuckyNumber luckyNumber = luckyNumbers.FirstOrDefault(x => x.EmployeeID == _currentUser.EmployeeID) ?? new EmployeeLuckyNumber();
                if (luckyNumber.LuckyNumber > 0) //nếu đã có số
                {
                    return Ok(ApiResponseFactory.Success(new
                    {
                        luckyNumber,
                        minValue,
                        maxValue
                    }, $"Số may măn của bạn là {luckyNumber.LuckyNumber}."));
                }
                else//Nếu chưa có số thì random 1 số
                {
                    //Get 1 list số đã có
                    List<int> numbers = luckyNumbers.Select(x => Convert.ToInt32(x.LuckyNumber)).ToList();

                    Random random = new Random();
                    int randomNumber = 0;
                    do
                    {
                        randomNumber = random.Next(minValue, maxValue); // Tạo số ngẫu nhiên từ 1 đến 200
                    }
                    while (numbers.Contains(randomNumber)); // Kiểm tra số có trong danh sách không

                    return Ok(ApiResponseFactory.Success(new
                    {
                        randomNumber,
                        minValue,
                        maxValue
                    }, $"Số may măn của bạn là {randomNumber}."));
                }

            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}
