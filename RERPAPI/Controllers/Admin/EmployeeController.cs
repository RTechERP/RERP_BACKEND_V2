using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.Admin
{
    [Route("api/admin/employees")]
    [ApiController]
    public class EmployeeController : BaseController
    {
        private readonly IUserRepo _userRepo;
        private readonly IUserGroupDetailRepo _userGroupDetailRepo;

        public EmployeeController(
            IUserRepo userRepo,
            IUserGroupDetailRepo userGroupDetailRepo)
        {
            _userRepo = userRepo;
            _userGroupDetailRepo = userGroupDetailRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string search = "", [FromQuery] string department = "", [FromQuery] bool? isActive = null)
        {
            try
            {
                var result = await _userRepo.GetEmployeesAsync(page, pageSize, search, department, isActive);
                return ApiResponseFactory.Success(result);
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Error(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            try
            {
                var employee = await _userRepo.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    return ApiResponseFactory.Error("Nhân viên không tồn tại");
                }
                return ApiResponseFactory.Success(employee);
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Error(ex.Message);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchEmployees([FromQuery] string keyword = "", [FromQuery] int limit = 20)
        {
            try
            {
                var result = await _userRepo.SearchEmployeesAsync(keyword, limit);
                return ApiResponseFactory.Success(result);
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Error(ex.Message);
            }
        }

        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments()
        {
            try
            {
                var result = await _userRepo.GetDepartmentsAsync();
                return ApiResponseFactory.Success(result);
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Error(ex.Message);
            }
        }

        [HttpGet("available-for-group/{groupId}")]
        public async Task<IActionResult> GetAvailableEmployeesForGroup(int groupId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string search = "", [FromQuery] string department = "")
        {
            try
            {
                var result = await _userRepo.GetAvailableEmployeesForGroupAsync(groupId, page, pageSize, search, department);
                return ApiResponseFactory.Success(result);
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Error(ex.Message);
            }
        }

        [HttpGet("in-group/{groupId}")]
        public async Task<IActionResult> GetEmployeesInGroup(int groupId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string search = "")
        {
            try
            {
                var result = await _userRepo.GetEmployeesInGroupAsync(groupId, page, pageSize, search);
                return ApiResponseFactory.Success(result);
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Error(ex.Message);
            }
        }

        [HttpGet("not-in-group/{groupId}")]
        public async Task<IActionResult> GetEmployeesNotInGroup(int groupId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string search = "", [FromQuery] string department = "")
        {
            try
            {
                var result = await _userRepo.GetEmployeesNotInGroupAsync(groupId, page, pageSize, search, department);
                return ApiResponseFactory.Success(result);
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Error(ex.Message);
            }
        }

        [HttpGet("cbb")]
        public async Task<IActionResult> GetEmployeesForComboBox([FromQuery] string search = "", [FromQuery] int limit = 50)
        {
            try
            {
                var result = await _userRepo.GetActiveEmployeesForComboBoxAsync(search, limit);
                return ApiResponseFactory.Success(result);
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Error(ex.Message);
            }
        }

        [HttpGet("cbb-by-department/{department}")]
        public async Task<IActionResult> GetEmployeesByDepartmentForComboBox(string department, [FromQuery] string search = "", [FromQuery] int limit = 50)
        {
            try
            {
                var result = await _userRepo.GetEmployeesByDepartmentForComboBoxAsync(department, search, limit);
                return ApiResponseFactory.Success(result);
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Error(ex.Message);
            }
        }

        [HttpGet("{id}/groups")]
        public async Task<IActionResult> GetEmployeeGroups(int id)
        {
            try
            {
                var employee = await _userRepo.GetByIdAsync(id);
                if (employee == null)
                {
                    return ApiResponseFactory.Error("Nhân viên không tồn tại");
                }

                var result = await _userGroupDetailRepo.GetUserGroupsAsync(id);
                return ApiResponseFactory.Success(result);
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Error(ex.Message);
            }
        }

        [HttpGet("{id}/permissions")]
        public async Task<IActionResult> GetEmployeePermissions(int id)
        {
            try
            {
                var employee = await _userRepo.GetByIdAsync(id);
                if (employee == null)
                {
                    return ApiResponseFactory.Error("Nhân viên không tồn tại");
                }

                var result = await _userRepo.GetEmployeePermissionsAsync(id);
                return ApiResponseFactory.Success(result);
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Error(ex.Message);
            }
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetEmployeeStatistics()
        {
            try
            {
                var result = await _userRepo.GetEmployeeStatisticsAsync();
                return ApiResponseFactory.Success(result);
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Error(ex.Message);
            }
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportEmployees([FromQuery] string search = "", [FromQuery] string department = "", [FromQuery] bool? isActive = null)
        {
            try
            {
                var result = await _userRepo.ExportEmployeesAsync(search, department, isActive);
                return ApiResponseFactory.Success(result);
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Error(ex.Message);
            }
        }
    }
}