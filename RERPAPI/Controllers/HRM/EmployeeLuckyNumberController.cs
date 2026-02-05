using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.HPSF;
using RERPAPI.Attributes;
using RERPAPI.Middleware;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
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
        private readonly ConfigSystemRepo _configSystemRepo;

        private EmployeeLuckyNumberRepo _employeeLucky;

        public EmployeeLuckyNumberController(IConfiguration configuration, CurrentUser currentUser, ConfigSystemRepo configSystemRepo, EmployeeLuckyNumberRepo employeeLucky)
        {
            _configuration = configuration;
            _currentUser = currentUser;
            _configSystemRepo = configSystemRepo;
            _employeeLucky = employeeLucky;
            _configSystemRepo = configSystemRepo;
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
                int year = DateTime.Now.Year;
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
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                _currentUser = ObjectMapper.GetCurrentUser(claims);

                employeeID = 0;
                if (!_currentUser.Permissions.Contains("N34")) employeeID = _currentUser.EmployeeID;

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

        [HttpPost("save-data")]
        [Authorize]
        [RequiresPermission("N34")]
        public async Task<IActionResult> SaveData([FromBody] List<EmployeeLuckyNumber> employeeLuckys)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                _currentUser = ObjectMapper.GetCurrentUser(claims);
                int records = 0;

                foreach (var employeeLucky in employeeLuckys)
                {
                    var employee = _employeeLucky.GetAll(x => x.YearValue == employeeLucky.YearValue
                                                        && x.EmployeeID == employeeLucky.EmployeeID
                                                        //&& (x.LuckyNumber == employeeLucky.LuckyNumber || employeeLucky.LuckyNumber == 0)
                                                        )
                                            .FirstOrDefault() ?? new EmployeeLuckyNumber();
                    if (employee.ID <= 0)
                    {
                        records += await _employeeLucky.CreateAsync(employeeLucky);
                    }
                    else
                    {
                        employee.EmployeeID = employeeLucky.EmployeeID;
                        employee.EmployeeCode = employeeLucky.EmployeeCode;
                        employee.EmployeeName = employeeLucky.EmployeeName;
                        employee.PhoneNumber = employeeLucky.PhoneNumber;
                        employee.YearValue = employeeLucky.YearValue;
                        //employee.LuckyNumber = employeeLucky.LuckyNumber;
                        employee.IsChampion = employeeLucky.IsChampion;
                        employee.ImageName = employeeLucky.ImageName;
                        
                        records += await _employeeLucky.UpdateAsync(employee);
                    }
                }

                if (records > 0) return Ok(ApiResponseFactory.Success(employeeLuckys, "Câp nhật thành công!"));
                else return BadRequest(ApiResponseFactory.Fail(null, "Câp nhật thất bại!", employeeLuckys));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("get-random-number")]
        [Authorize]
        public async Task<IActionResult> GetRandomNumberAsync([FromBody] EmployeeLuckyNumber employeeLucky)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                _currentUser = ObjectMapper.GetCurrentUser(claims);

                employeeLucky.YearValue = DateTime.Now.Year;

                var luckyNumbers = _employeeLucky.GetAll(x => x.YearValue == employeeLucky.YearValue);
                int minValue = _configuration.GetValue<int>("MinValue");
                int maxValue = _configuration.GetValue<int>("MaxValue");
                //int minValue = 1;
                //int maxValue = luckyNumbers.Count();

                EmployeeLuckyNumber luckyNumber = luckyNumbers.FirstOrDefault(x => x.EmployeeID == _currentUser.EmployeeID) ?? new EmployeeLuckyNumber();
                //if (luckyNumber.LuckyNumber > 0) //nếu đã có số
                //{
                //    return Ok(ApiResponseFactory.Success(new
                //    {
                //        randomNumber = luckyNumber.LuckyNumber,
                //        minValue,
                //        maxValue
                //    }, $"Số may măn của bạn là {luckyNumber.LuckyNumber}."));
                //}
                //else//Nếu chưa có số thì random 1 số

                luckyNumber.LuckyNumber = luckyNumber.LuckyNumber ?? 0;
                if (luckyNumber.ID > 0 && luckyNumber.LuckyNumber <= 0)
                {
                    //Get 1 list số đã có
                    List<int> numbers = luckyNumbers.Select(x => Convert.ToInt32(x.LuckyNumber)).ToList();

                    Random random = new Random();
                    int randomNumber = 0;
                    do
                    {
                        randomNumber = random.Next(minValue, maxValue);
                    }
                    while (numbers.Contains(randomNumber)); // Kiểm tra số có trong danh sách không

                    //randomNumber = 10;

                    //luckyNumber.EmployeeID = _currentUser.EmployeeID;
                    //luckyNumber.EmployeeCode = _currentUser.Code;
                    //luckyNumber.EmployeeName = _currentUser.FullName;
                    //luckyNumber.PhoneNumber = employeeLucky.PhoneNumber;
                    //luckyNumber.YearValue = employeeLucky.YearValue;
                    luckyNumber.LuckyNumber = randomNumber;
                    luckyNumber.IsChampion = false;
                    //luckyNumber.ImageName = employeeLucky.ImageName;

                    var record = await _employeeLucky.UpdateAsync(luckyNumber);
                    if (record >= 1)
                    {
                        return Ok(ApiResponseFactory.Success(new
                        {
                            randomNumber = luckyNumber.LuckyNumber,
                            minValue,
                            maxValue
                        }, $"Số may măn của bạn là {randomNumber}."));
                    }
                    else
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, $"Lấy số thất bại. Vui lòng nhận lại số!"));
                    }
                }
                else
                {
                    string message = $"Số may măn của bạn là {luckyNumber.LuckyNumber}.";
                    if (luckyNumber.ID <= 0) message = "Bạn không nằm trong danh sách quay số năm nay!";
                    return Ok(ApiResponseFactory.Success(new
                    {
                        randomNumber = luckyNumber.LuckyNumber,
                        minValue,
                        maxValue
                    }, message));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("upload-avatar")]
        [Authorize]
        public async Task<IActionResult> UploadFileAvatar()
        {
            try
            {
                //_currentUser = HttpContext.Session.GetObject<CurrentUser>(_configuration.GetValue<string>("SessionKey") ?? "");

                var form = await Request.ReadFormAsync();
                var employeeLuckyID = TextUtils.ToInt32(form["EmployeeLuckyNumberID"]);
                var phoneNumber = TextUtils.ToString(form["PhoneNumber"]);
                var files = Request.Form.Files;

                // Lấy đường dẫn từ ConfigSystem
                var pathServer = _configSystemRepo.GetUploadPathByKey("EmployeeLuckyNumber");
                if (string.IsNullOrWhiteSpace(pathServer))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy cấu hình đường dẫn cho key: EmployeeLuckyNumber"));
                }

                var employeeLucky = _employeeLucky.GetByID(employeeLuckyID);
                string pathUpload = Path.Combine(pathServer, $"{DateTime.Now.Year}");

                int records = 0;
                string imageName = "";
                if (files.Count() > 0)
                {
                    foreach (var file in files)
                    {
                        string fileName = $"{employeeLucky.EmployeeCode}_{file.FileName}";
                        var result = await FileHelper.UploadFile(file, pathUpload, fileName);

                        if (result.status == 1)
                        {
                            employeeLucky.ImageName = fileName;
                            employeeLucky.PhoneNumber = phoneNumber;
                            records += await _employeeLucky.UpdateAsync(employeeLucky);
                        }
                    }
                }
                else
                {
                    employeeLucky.PhoneNumber = phoneNumber;
                    records = await _employeeLucky.UpdateAsync(employeeLucky);
                }

                if (records > 0) return Ok(ApiResponseFactory.Success(null, "Cập nhật thành công!"));
                else return BadRequest(ApiResponseFactory.Fail(null, "Cập nhật thất bại. Vui lòng thử lại!"));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}
