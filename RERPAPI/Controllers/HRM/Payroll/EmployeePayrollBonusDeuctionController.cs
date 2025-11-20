using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeePayrollBonusDeuctionController : Controller
    {
        EmployeePayrollBonusDeuctionRepo _employeePayrollBonusDeuctionRepo;
        EmployeeRepo _employeeRepo;
        public EmployeePayrollBonusDeuctionController(EmployeePayrollBonusDeuctionRepo employeePayrollBonusDeuctionRepo, EmployeeRepo employeeRepo)
        {
            _employeePayrollBonusDeuctionRepo = employeePayrollBonusDeuctionRepo;
            _employeeRepo = employeeRepo;
        }
        [HttpGet("employee-payroll-bonus-deduction")]
        public async Task<IActionResult> getemployeepayrollbonusdeduction(int? year, int? month, int page, int size, int departmentID, int employeeID, string? keyword)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetEmployeePayrollBonusDeduction",
                    new string[] { "@Year", "@Month", "@PageNumber", "@PageSize", "@DepartmentID", "@EmployeeID", "@Keyword" },
                    new object[] { year, month, page, size, departmentID, employeeID, keyword ?? "" });
                var arrTotalPage = SQLHelper<object>.GetListData(data, 1);


                var result = new
                {
                    data = SQLHelper<object>.GetListData(data, 0),
                    totalPage = arrTotalPage[0]?.TotalPage ?? 0
                };

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("employee-payroll-bonus-deduction-delete-id")]
        public async Task<IActionResult> employeepayrollbonusdeductiondeleteid(int ID)
        {
            try
            {
                EmployeePayrollBonusDeuction model = _employeePayrollBonusDeuctionRepo.GetByID(ID);
                model.IsDeleted = true;
                _employeePayrollBonusDeuctionRepo.UpdateAsync(model);
                return Ok(ApiResponseFactory.Success("", ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("check-employee-payroll-bonus-deduction")]
        public async Task<IActionResult> checkemployeepayrollbonusdeduction(int? year, int? month, int? employeeId)
        {
            try
            {
                EmployeePayrollBonusDeuction model = _employeePayrollBonusDeuctionRepo
                    .GetAll(x => x.YearValue == year && x.MonthValue == month && x.EmployeeID == employeeId && x.IsDeleted != true)
                    .FirstOrDefault();
                return Ok(ApiResponseFactory.Success(model, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-employee-payroll-bonus-deduction")]
        public async Task<IActionResult> saveemployeepayrollbonusdeuction([FromBody] EmployeePayrollBonusDeuction obj)
        {
            try
            {
                if (obj.ID <= 0)
                {
                    await _employeePayrollBonusDeuctionRepo.CreateAsync(obj);
                }
                else
                {
                    _employeePayrollBonusDeuctionRepo.Update(obj);
                }

                return Ok(ApiResponseFactory.Success(1, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("import-excel-payroll-bonusde-duction")]
        public IActionResult importexcelpayrollbonusdeduction([FromBody] List<Dictionary<string, object>> payrollbonusdeduction, [FromQuery] int month, int year)
        {
            try
            {
                if (payrollbonusdeduction == null || payrollbonusdeduction.Count == 0)
                    return BadRequest(new { status = 0, message = "Payload rỗng." });

                int created = 0, updated = 0;
                var errors = new List<object>();
                foreach (var row in payrollbonusdeduction)
                {
                    try
                    {
                        string? Code = row["Code"]?.ToString();
                        var employee = _employeeRepo.GetAll(c => c.Code == Code);
                        EmployeePayrollBonusDeuction employeePayrollBonusDeuction = new EmployeePayrollBonusDeuction();
                        if (employee.Any())
                        {
                            var lstExistFromDB = _employeePayrollBonusDeuctionRepo.GetAll(c =>
                                c.EmployeeID == employee.First().ID &&
                               c.YearValue == year && c.MonthValue == month);

                            if (lstExistFromDB.Any())
                            {
                                employeePayrollBonusDeuction = lstExistFromDB.First();
                            }
                            employeePayrollBonusDeuction.EmployeeID = employee.First().ID;
                            employeePayrollBonusDeuction.TotalWorkDay = decimal.Parse(row["TotalWorkDay"]?.ToString());
                            employeePayrollBonusDeuction.MonthValue = month;
                            employeePayrollBonusDeuction.YearValue = year;

                            // Các khoản thưởng
                            employeePayrollBonusDeuction.KPIBonus = decimal.Parse(row["KPIBonus"]?.ToString());
                            employeePayrollBonusDeuction.OtherBonus = decimal.Parse(row["OtherBonus"]?.ToString());

                            // Các khoản trừ
                            employeePayrollBonusDeuction.Insurances = decimal.Parse(row["Insurances"]?.ToString());
                            employeePayrollBonusDeuction.ParkingMoney = decimal.Parse(row["ParkingMoney"]?.ToString());
                            employeePayrollBonusDeuction.Punish5S = decimal.Parse(row["Punish5S"]?.ToString());
                            employeePayrollBonusDeuction.OtherDeduction = decimal.Parse(row["OtherDeduction"]?.ToString());
                            employeePayrollBonusDeuction.SalaryAdvance = decimal.Parse(row["SalaryAdvance"]?.ToString());
                            employeePayrollBonusDeuction.Note = row["Note"]?.ToString();
                            // Lưu
                            if (lstExistFromDB.Any())
                            {
                                _employeePayrollBonusDeuctionRepo.Update(employeePayrollBonusDeuction);
                                updated++;
                            }
                            else
                            {
                                _employeePayrollBonusDeuctionRepo.Create(employeePayrollBonusDeuction);
                                created++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add(new { message = ex.Message });
                    }
                }

                var result = new
                {
                    created = created,
                    updated = updated,
                    skipped = errors.Count,
                    errors = errors
                };

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

    }
}
