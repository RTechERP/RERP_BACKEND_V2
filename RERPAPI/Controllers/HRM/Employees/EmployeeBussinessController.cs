using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.Asset;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Model.Param.HRM;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Asset;
using RERPAPI.Repo.GenericEntity.GeneralCatetogy;
using RERPAPI.Repo.GenericEntity.HRM;
using System;

namespace RERPAPI.Controllers.HRM.Employees
{
    [ApiController]

    [Route("api/[controller]")]
    public class EmployeeBussinessController : ControllerBase
    {
        private EmployeeBussinessRepo _employeeBussinessRepo;
        EmployeeBussinessFileRepo _employeeBussinessFileRepo;
        EmployeeBussinessVehicleRepo _employeeBussinessVehicleRepo;

        public EmployeeBussinessController(EmployeeBussinessRepo employeeBussinessRepo, EmployeeBussinessFileRepo employeeBussinessFileRepo, EmployeeBussinessVehicleRepo employeeBussinessVehicleRepo)
        {
            _employeeBussinessRepo = employeeBussinessRepo;
            _employeeBussinessFileRepo = employeeBussinessFileRepo;
            _employeeBussinessVehicleRepo = employeeBussinessVehicleRepo;
        }

        [RequiresPermission("N1,N2")]
        [HttpPost]
        public IActionResult getEmployeeBussiness(EmployeeBussinessParam param)
        {
            try
            {
                var arrParamName = new string[] { "@PageNumber", "@PageSize", "@StartDate", "@EndDate", "@Keyword", "@DepartmentID", "@IDApprovedTP", "@Status" };
                var arrParamValue = new object[] { param.pageNumber, param.pageSize, param.dateStart, param.dateEnd, param.keyWord, param.departmentId, param.idApprovedTp, param.status };
                var employeeBussiness = SQLHelper<object>.ProcedureToList("spGetEmployeeBussiness", arrParamName, arrParamValue);

                var result = SQLHelper<object>.GetListData(employeeBussiness, 0);

                return Ok(new
                {
                    status = 1,
                    data = result
                });

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
    
        [HttpPost("get-employee-bussinesss-person")]
        public IActionResult GetEmployeeBussinessPerson(EmployeeBussinessInWebRequestParam param)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
                var firstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var lastDay = firstDay.AddMonths(1).AddDays(-1);
                var arrParamName = new string[] { "@DateStart", "@DateEnd", "@Keyword", "@EmployeeID", "@IsApproved", "@Type", "@VehicleID", "@NotCheckIn" };
                var arrParamValue = new object[] { param.DateStart ?? firstDay, param.DateEnd ?? lastDay, param.Keyword ?? "",currentUser.EmployeeID, param.IsApproved ?? 0, param.Type??0, param.VehicleID??0, param.NotCheckIn??-1};
                    var employeeBussiness = SQLHelper<object>.ProcedureToList("spGetEmployeeBussinessInWeb", arrParamName, arrParamValue);

                var result = SQLHelper<object>.GetListData(employeeBussiness, 0);

                return Ok(new
                {
                    status = 1,
                    data = result
                });

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpPost("get-employee-bussiness-person")]
        public IActionResult GetEmployeeBussinessPerson(EmployeeBussinessParam param)
        {
            try
            {
                var arrParamName = new string[] { "@PageNumber", "@PageSize", "@StartDate", "@EndDate", "@Keyword", "@DepartmentID", "@IDApprovedTP", "@Status" };
                var arrParamValue = new object[] { param.pageNumber, param.pageSize, param.dateStart, param.dateEnd, param.keyWord, param.departmentId, param.idApprovedTp, param.status };
                var employeeBussiness = SQLHelper<object>.ProcedureToList("spGetEmployeeBussiness", arrParamName, arrParamValue);

                var result = SQLHelper<object>.GetListData(employeeBussiness, 0);
                // var TotalPage = SQLHelper<object>.GetListData(employeeBussiness, 1);
                var summary = result
        .GroupBy(x => (string)x.FullName)
        .Select(g => new
        {
            FullName = g.Key,
            TotalCost = g.Sum(r =>
            {
                object costObj = r.Cost;

                if (costObj == null || costObj == DBNull.Value)
                    return 0m;

                return (decimal)costObj;
            })
        })
        .ToList();
                return Ok(ApiResponseFactory.Success(new { result, summary }, "Lấy dữ liệu thành công"));


            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("list-summary-employee-bussiness")]

        public IActionResult ListSummaryEmployeeOnleavePerson(EmployeeBussinessSummaryParam request)
        {
            try
            {
                var employeeOnLeaveSummary = SQLHelper<object>.ProcedureToList("spGetEmployeeOnLeaveInWeb", new string[] { "@Keyword", "@DateStart", "@DateEnd", "@IsApproved", "@Type", "@DepartmentID", "@EmployeeID", "VehicleID", "NotCheckIn" },
               new object[] { request.Keyword ?? "", request.DateStart, request.DateEnd, request.IsApproved, request.Type, request.DepartmentID ?? 0, request.EmployeeID ?? 0, request.VehicleID, request.NotCheckIn });

                var data = SQLHelper<object>.GetListData(employeeOnLeaveSummary, 0);
                var TotalPages = SQLHelper<object>.GetListData(employeeOnLeaveSummary, 1);
                return Ok(ApiResponseFactory.Success(new { data, TotalPages }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [RequiresPermission("N1,N2")]
        [HttpPost("get-work-management")]
        public IActionResult GetWorkManagement([FromBody] EmployeeNightShiftSummaryRequestParam request)
        {
            try
            {
                var arrParamName = new string[] { "@Month", "@Year", "@DepartmentID", "@EmployeeID", "@Keyword" };
                var arrParamValue = new object[] { request.Month, request.Year, request.DepartmentID, request.EmployeeID, request.KeyWord ?? "" };
                var workManagement = SQLHelper<object>.ProcedureToList("spGetEmployeeBussinessByMonth", arrParamName, arrParamValue);
                var vehicleEarly = SQLHelper<object>.ProcedureToList("spGetEmployeeBussinessVehicle", arrParamName, arrParamValue);
                var workData = SQLHelper<object>.GetListData(workManagement, 0);
                var earlyData = SQLHelper<object>.GetListData(vehicleEarly, 0);
                var vehicleData = SQLHelper<object>.GetListData(vehicleEarly, 1);
                return Ok(ApiResponseFactory.Success(new { workData, earlyData, vehicleData }, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2")]
        [HttpGet("detail")]
        public IActionResult GetEmployeeBussinessDetail(int employeeId, DateTime dayBussiness)
        {
            try
            {
                var arrParamName = new string[] { "@EmployeeID", "@DayBussiness" };
                var arrParamValue = new object[] { employeeId, dayBussiness };
                var employeeBussiness = SQLHelper<object>.ProcedureToList("spGetEmployeeBussinessDetail", arrParamName, arrParamValue);

                var result = SQLHelper<object>.GetListData(employeeBussiness, 0);
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
      
        [HttpGet("deleted")]
        public async Task<IActionResult> deleteEmployeeBussiness([FromQuery] List<int> listID)
        {
            try
            {
                if (listID.Count() > 0)
                {
                    foreach (var id in listID)
                    {
                        var employeeBussiness = _employeeBussinessRepo.GetByID(id);
                        if (employeeBussiness != null) await _employeeBussinessRepo.DeleteAsync(id);

                    }
                }
                return Ok(ApiResponseFactory.Success(null, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [RequiresPermission("N1,N2")]

        [HttpPost("save-data")]
        public async Task<IActionResult> saveEmployeeBussiness([FromBody] List<EmployeeBussiness> employeeBussiness)
        {
            try
            {
                if (employeeBussiness.Count() > 0)
                {
                    foreach (EmployeeBussiness item in employeeBussiness)
                    {
                        if (item.ID <= 0)
                        {
                            item.IsApproved = false;
                            item.IsApprovedBGD = false;
                            item.IsApprovedHR = false;
                            await _employeeBussinessRepo.CreateAsync(item);
                        }
                        else await _employeeBussinessRepo.UpdateAsync(item);
                    }
                }


                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1")]
        [HttpPost("save-approve-tbp")]
        public async Task<IActionResult> SaveApproveTBP([FromBody] List<EmployeeBussiness> employeeBussiness)
        {
            try
            {
                if (employeeBussiness.Count() > 0)
                {
                    foreach (EmployeeBussiness item in employeeBussiness)
                    {
                        if (item.ID <= 0)
                        {
                            item.IsApproved = false;
                            item.IsApprovedBGD = false;
                            item.IsApprovedHR = false;
                            await _employeeBussinessRepo.CreateAsync(item);
                        }
                        else await _employeeBussinessRepo.UpdateAsync(item);
                    }
                }


                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2")]
        [HttpPost("save-approve-hr")]
        public async Task<IActionResult> SaveApproveHR([FromBody] List<EmployeeBussiness> employeeBussiness)
        {
            try
            {
                if (employeeBussiness.Count() > 0)
                {
                    foreach (EmployeeBussiness item in employeeBussiness)
                    {
                        if (item.ID <= 0)
                        {
                            item.IsApproved = false;
                            item.IsApprovedBGD = false;
                            item.IsApprovedHR = false;
                            await _employeeBussinessRepo.CreateAsync(item);
                        }
                        else await _employeeBussinessRepo.UpdateAsync(item);
                    }
                }


                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data-employee")]
        public async Task<IActionResult> SaveDataEmployee([FromBody] EmployeeBussinessDTO dto)
                {
            try
            {
                if (dto == null) { return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." }); }
                if (dto.employeeBussiness != null)
                {
                    if (dto.employeeBussiness.ID <= 0)
                        await _employeeBussinessRepo.CreateAsync(dto.employeeBussiness);
                    else
                        await _employeeBussinessRepo.UpdateAsync(dto.employeeBussiness);
                }
                if (dto.employeeBussinessFiles != null)
                {
                    if (dto.employeeBussinessFiles.ID <= 0)
                    {
                        dto.employeeBussinessFiles.EmployeeBussinessID = dto.employeeBussiness.ID;
                        await _employeeBussinessFileRepo.CreateAsync(dto.employeeBussinessFiles);
                    }    
                    else
                        await _employeeBussinessFileRepo.UpdateAsync(dto.employeeBussinessFiles);
                }
                if (dto.employeeBussinessVehicle != null)
                {
                    if (dto.employeeBussinessVehicle.ID <= 0)
                    {
                      
                        await _employeeBussinessVehicleRepo.CreateAsync(dto.employeeBussinessVehicle);
                    }
                    else
                        await _employeeBussinessVehicleRepo.UpdateAsync(dto.employeeBussinessVehicle);
                }
                return Ok(ApiResponseFactory.Success(dto, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
       
            [HttpPost("save-file")]
            public async Task<IActionResult> SaveFile([FromBody] EmployeeBussinessFile employeeBussinessFile)
            {
                try
                {
                    if (employeeBussinessFile.ID <= 0)
                    {
                 
                        await _employeeBussinessFileRepo.CreateAsync(employeeBussinessFile);
                    }
                    else await _employeeBussinessFileRepo.UpdateAsync(employeeBussinessFile);
                    return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
                }
                catch (Exception ex)
                {
                    return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
                }
            }
       
        [HttpGet("get-file-by-id")]
        public IActionResult GetFileByID(int bussinessID)
        {
            try
            {
                var file = _employeeBussinessFileRepo.GetAll(x => x.EmployeeBussinessID == bussinessID&&x.IsDeleted!=true);
                return Ok(ApiResponseFactory.Success(file, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-employee-buissiness-vehicle")]
        public IActionResult GetEmployeeBussinessVehicle (int id)
        {
            try
            {
                var arrParamName = new string[] { "@IDBussiness" };
                var arrParamValue = new object[] { id };
                var employeeBussinessVehicle = SQLHelper<object>.ProcedureToList("spGetBussinessVehicle", arrParamName, arrParamValue);

                var result = SQLHelper<object>.GetListData(employeeBussinessVehicle, 0);
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-by-id")]
        public IActionResult GetByID(int id)
        {
            try
            {
                var employeeBussiness = _employeeBussinessRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(employeeBussiness, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }

}