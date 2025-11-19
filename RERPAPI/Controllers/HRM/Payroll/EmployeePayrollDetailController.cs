using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeePayrollDetailController : ControllerBase
    {
        EmployeePayrollDetailRepo _employeePayrollDetailRepo;
        EmployeePayrollRepo _employeePayrollRepo;
        EmployeeRepo _employeeRepo;

        public EmployeePayrollDetailController(EmployeePayrollDetailRepo employeePayrollDetailRepo, EmployeePayrollRepo employeePayrollRepo, EmployeeRepo employeeRepo)
        {
            _employeePayrollDetailRepo = employeePayrollDetailRepo;
            _employeePayrollRepo = employeePayrollRepo;
            _employeeRepo = employeeRepo;
        }

        [HttpGet("employee-payroll-detail")]
        public async Task<IActionResult> getemployeepayrolldetail(int? year, int? month, int departmentID, int employeeID, string? keyword)
        {
            try
            {
                var employeePayrollDetails = SQLHelper<object>.ProcedureToList("spGetEmployeePayrollDetail",
                    new string[] { "@Year", "@Month", "@DepartmentID", "@EmployeeID", "@Keyword" },
                    new object[] { year, month, departmentID, employeeID, keyword ?? "" });

                var data = SQLHelper<object>.GetListData(employeePayrollDetails, 0);
                var totalWorkday = SQLHelper<object>.GetListData(employeePayrollDetails, 1)
                    .Select(c => c?.TotalWorkday)
                    .Where(x => x != null)
                    .Distinct()
                    .FirstOrDefault() ?? 0;

                var payrollId = _employeePayrollRepo.GetAll(x => x._Year == year && x._Month == month).FirstOrDefault()?.ID;
                var result = new
                {
                    data = data,
                    totalWorkday = totalWorkday,
                    payrollId = payrollId
                };
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("employee-payroll-detail-by-id")]
        public async Task<IActionResult> getemployeepayrolldetailbyid(int ID)
        {
            try
            {
                var data = _employeePayrollDetailRepo.GetByID(ID);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpGet("update-employee-payroll-detail")]
        public async Task<IActionResult> getupdateemployeepayrolldetail(int payrollID, int year, int month, int employeeID, string loginName, int type)
        {
            try
            {
                if (type == 2)
                {
                    List<EmployeePayrollDetail> employeePayrollDetails = _employeePayrollDetailRepo.GetAll(x => x.PayrollID == payrollID);
                    if (employeePayrollDetails.Count() > 0) _employeePayrollDetailRepo.DeleteRange(employeePayrollDetails);
                    employeeID = 0;
                }

                SQLHelper<object>.ExcuteProcedure("spInsertIntoEmployeePayrollDetail",
                       new string[] { "@PayrollID", "@Year", "@Month", "@EmployeeID", "@LoginName" },
                       new object[] { payrollID, year, month, employeeID, loginName });

                return Ok(ApiResponseFactory.Success(1, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("publish-employee-payroll")]
        public async Task<IActionResult> publishemployeepayroll(bool isPublish, int[] listID)
        {
            try
            {
                if (listID.Count() > 0)
                {
                    List<EmployeePayrollDetail> employeePayrollDetails = _employeePayrollDetailRepo.GetAll().Where(x => listID.Contains(x.ID)).ToList();
                    if (employeePayrollDetails.Count() > 0)
                    {
                        foreach (EmployeePayrollDetail item in employeePayrollDetails)
                        {
                            item.IsPublish = isPublish;
                            _employeePayrollDetailRepo.UpdateAsync(item);
                        }
                    }
                }
                return Ok(ApiResponseFactory.Success(1, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("import-excel-payroll-report")]
        public IActionResult importexcelpayrollreport([FromBody] List<Dictionary<string, object>> payrollreport, [FromQuery] int PayrollID)
        {
            try
            {
                if (payrollreport == null || payrollreport.Count == 0)
                    return BadRequest(new { status = 0, message = "Payload rỗng." });

                int created = 0, updated = 0;
                var errors = new List<object>();
                foreach (var row in payrollreport)
                {
                    try
                    {
                        string code = row["Code"]?.ToString();
                        decimal? BasicSalary = row["BasicSalary"] != null
                            ? Convert.ToDecimal(row["BasicSalary"].ToString())
                            : null;


                        EmployeePayrollDetail employeePayrollDetail = new EmployeePayrollDetail();
                        var employee = _employeeRepo.GetAll(c => c.Code == code);

                        if (employee.Any())
                        {
                            var lstExistFromDB = _employeePayrollDetailRepo.GetAll(c =>
                                c.EmployeeID == employee.First().ID &&
                                c.BasicSalary == BasicSalary &&
                                c.PayrollID == PayrollID);

                            if (lstExistFromDB.Any())
                            {
                                employeePayrollDetail = lstExistFromDB.First();
                            }

                            // Boolean
                            employeePayrollDetail.IsPublish = row["IsPublish"]?.ToString().ToLower() == "true";
                            employeePayrollDetail.Sign = row["Sign"]?.ToString().ToLower() == "true";

                            // Cơ bản
                            employeePayrollDetail.STT = row["STT"] != null ? Convert.ToInt32(row["STT"].ToString()) : 0;

                            employeePayrollDetail.EmployeeID = employee.First().ID;
                            employeePayrollDetail.PayrollID = PayrollID;

                            // Lương cơ bản
                            employeePayrollDetail.BasicSalary = row["BasicSalary"] != null ? Convert.ToDecimal(row["BasicSalary"].ToString()) : 0;
                            employeePayrollDetail.TotalMerit = row["TotalMerit"] != null ? Convert.ToDecimal(row["TotalMerit"].ToString()) : 0;
                            employeePayrollDetail.TotalSalaryByDay = row["TotalSalaryByDay"] != null ? Convert.ToDecimal(row["TotalSalaryByDay"].ToString()) : 0;
                            employeePayrollDetail.SalaryOneHour = row["SalaryOneHour"] != null ? Convert.ToDecimal(row["SalaryOneHour"].ToString()) : 0;



                            // Làm thêm
                            employeePayrollDetail.OT_Hour_WD = row["OT_Hour_WD"] != null ? Convert.ToDecimal(row["OT_Hour_WD"].ToString()) : 0;
                            employeePayrollDetail.OT_Money_WD = row["OT_Money_WD"] != null ? Convert.ToDecimal(row["OT_Money_WD"].ToString()) : 0;

                            employeePayrollDetail.OT_Hour_WK = row["OT_Hour_WK"] != null ? Convert.ToDecimal(row["OT_Hour_WK"].ToString()) : 0;
                            employeePayrollDetail.OT_Money_WK = row["OT_Money_WK"] != null ? Convert.ToDecimal(row["OT_Money_WK"].ToString()) : 0;

                            employeePayrollDetail.OT_Hour_HD = row["OT_Hour_HD"] != null ? Convert.ToDecimal(row["OT_Hour_HD"].ToString()) : 0;
                            employeePayrollDetail.OT_Money_HD = row["OT_Money_HD"] != null ? Convert.ToDecimal(row["OT_Money_HD"].ToString()) : 0;




                            // Phụ cấp
                            //employeePayrollDetail.ReferenceIndustry = decimal.Parse(row.GetString("Phụ cấp chuyên cần tham chiếu"));
                            employeePayrollDetail.RealIndustry = row["RealIndustry"] != null ? Convert.ToDecimal(row["RealIndustry"].ToString()) : 0;
                            employeePayrollDetail.AllowanceMeal = row["AllowanceMeal"] != null ? Convert.ToDecimal(row["AllowanceMeal"].ToString()) : 0;
                            employeePayrollDetail.Allowance_OT_Early = row["Allowance_OT_Early"] != null ? Convert.ToDecimal(row["Allowance_OT_Early"].ToString()) : 0;



                            //employeePayrollDetail.TotalAllowance = decimal.Parse(row.GetString("Tổng tiền PC"));

                            // Các khoản cộng khác
                            employeePayrollDetail.BussinessMoney = row["BussinessMoney"] != null ? Convert.ToDecimal(row["BussinessMoney"].ToString()) : 0;
                            employeePayrollDetail.NightShiftMoney = row["NightShiftMoney"] != null ? Convert.ToDecimal(row["NightShiftMoney"].ToString()) : 0;
                            employeePayrollDetail.CostVehicleBussiness = row["CostVehicleBussiness"] != null ? Convert.ToDecimal(row["CostVehicleBussiness"].ToString()) : 0;
                            employeePayrollDetail.Bonus = row["Bonus"] != null ? Convert.ToDecimal(row["Bonus"].ToString()) : 0;
                            employeePayrollDetail.Other = row["Other"] != null ? Convert.ToDecimal(row["Other"].ToString()) : 0;



                            //employeePayrollDetail.TotalBonus = decimal.Parse(row.GetString("Tổng cộng"));

                            // Tổng thu nhập
                            employeePayrollDetail.RealSalary = row["RealSalary"] != null ? Convert.ToDecimal(row["RealSalary"].ToString()) : 0;



                            // Các khoản phải trừ
                            //employeePayrollDetail.SocialInsurance = decimal.Parse(row.GetString("Mức đóng"));
                            employeePayrollDetail.Insurances = row["Insurances"] != null ? Convert.ToDecimal(row["Insurances"].ToString()) : 0;
                            employeePayrollDetail.UnionFees = row["UnionFees"] != null ? Convert.ToDecimal(row["UnionFees"].ToString()) : 0;
                            employeePayrollDetail.AdvancePayment = row["AdvancePayment"] != null ? Convert.ToDecimal(row["AdvancePayment"].ToString()) : 0;
                            employeePayrollDetail.DepartmentalFees = row["DepartmentalFees"] != null ? Convert.ToDecimal(row["DepartmentalFees"].ToString()) : 0;
                            employeePayrollDetail.ParkingMoney = row["ParkingMoney"] != null ? Convert.ToDecimal(row["ParkingMoney"].ToString()) : 0;
                            employeePayrollDetail.Punish5S = row["Punish5S"] != null ? Convert.ToDecimal(row["Punish5S"].ToString()) : 0;
                            employeePayrollDetail.MealUse = row["MealUse"] != null ? Convert.ToInt32(row["MealUse"].ToString()) : 0;
                            employeePayrollDetail.OtherDeduction = row["OtherDeduction"] != null ? Convert.ToDecimal(row["OtherDeduction"].ToString()) : 0;


                            //employeePayrollDetail.TotalDeduction = decimal.Parse(row.GetString("Tổng cộng các khoản phải trừ"));

                            // Thực lĩnh
                            employeePayrollDetail.ActualAmountReceived = row["ActualAmountReceived"] != null ? Convert.ToDecimal(row["ActualAmountReceived"].ToString()) : 0;


                            // Ghi chú
                            employeePayrollDetail.Note = row["Note"]?.ToString();

                            // Lưu
                            if (lstExistFromDB.Any())
                            {
                                _employeePayrollDetailRepo.Update(employeePayrollDetail);
                                updated++;
                            }
                            else
                            {
                                _employeePayrollDetailRepo.Create(employeePayrollDetail);
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

        [HttpPost("save-employee-payroll-detail")]
        public async Task<IActionResult> saveemployeepayrolldetail([FromBody] EmployeePayrollDetail obj)
        {
            try
            {
                if (obj.ID <= 0)
                {
                    await _employeePayrollDetailRepo.CreateAsync(obj);
                }
                else
                {
                    _employeePayrollDetailRepo.Update(obj);
                }

                return Ok(ApiResponseFactory.Success(1, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }
    }
}
