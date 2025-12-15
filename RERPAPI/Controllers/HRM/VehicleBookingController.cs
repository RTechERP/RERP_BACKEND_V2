using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM.Vehicle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.HRM
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VehicleBookingController : ControllerBase
    {
        private readonly VehicleBookingManagementRepo _vehicleBookingManagementRepo;
        private readonly EmployeeRepo _employeeRepo;
        private readonly ConfigSystemRepo _configSystemRepo;
        private readonly ProjectRepo _projectRepo;
        private readonly EmployeeApproveRepo  _employeeApproveRepo;
        private readonly VehicleBookingFileRepo _vehicleBookingFileRepo;
        private readonly ProvinceRepo _provinceRepo;

        public VehicleBookingController(
            VehicleBookingManagementRepo vehicleBookingManagementRepo,
            EmployeeRepo employeeRepo,
            ConfigSystemRepo configSystemRepo,
            ProjectRepo projectRepo,
            EmployeeApproveRepo employeeApproveRepo,
            VehicleBookingFileRepo vehicleBookingFileRepo,
            ProvinceRepo provinceRepo
            )
        {
            _vehicleBookingManagementRepo = vehicleBookingManagementRepo;
            _employeeRepo = employeeRepo;
            _configSystemRepo = configSystemRepo;
            _projectRepo = projectRepo;
            _employeeApproveRepo = employeeApproveRepo;
            _vehicleBookingFileRepo = vehicleBookingFileRepo;
            _provinceRepo = provinceRepo;
        }

        [HttpGet("get-all")]
        public IActionResult GetAll(DateTime dateStart, DateTime dateEnd, int category, int status, int employeeId, int driverEmployeeId, string keyword = "")
        {
            try
            {
                keyword = string.IsNullOrEmpty(keyword) ? "" : keyword.Trim();
                var result = SQLHelper<dynamic>.ProcedureToList("spGetVehicleBookingManagement",
                                new string[] { "@StartDate", "@EndDate", "@Keyword", "@Category", "@Status", "@EmployeeID", "@DriverEmployeeID" },
                                new object[] { dateStart, dateEnd, keyword, category, status, employeeId, driverEmployeeId });

                var data = SQLHelper<dynamic>.GetListData(result, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-employees")]
        public IActionResult LoadEmployees()
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

        [HttpGet("get-employees-by-id")]
        public IActionResult LoadEmployeeByID(int employeeId)
        {
            try
            {
                var list = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployee",
                                 new string[] { "@Keyword", "@Status" },
                                 new object[] { "", -1 });
                var employee = list.FirstOrDefault(e => e.ID == employeeId);
                return Ok(ApiResponseFactory.Success(employee, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        

    }
}
