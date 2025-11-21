using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private DepartmentRepo _departmentRepo;
        private EmployeeRepo _employeeRepo;
        public DepartmentController(DepartmentRepo departmentRepo, EmployeeRepo employeeRepo)
        {
            _departmentRepo = departmentRepo;
            _employeeRepo = employeeRepo;
        }   

        [HttpGet("get-all")]
        [RequiresPermission("N2,N1")]
        public IActionResult GetAll()
        {
            try
            {
                List<Department> departments = _departmentRepo.GetAll().OrderBy(x => x.STT).ToList();
                return Ok(ApiResponseFactory.Success(departments, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

        [HttpGet("deleted")]
        [RequiresPermission("N2,N1")]
        public IActionResult DeleteDepartment(int id)
        {
            try
            {
                Department? department = _departmentRepo.GetByID(id);
                if (department == null )
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Phòng ban không tồn tại"));
                }
                bool isUsed = _employeeRepo.GetAll(x => x.DepartmentID == id).Any();
                if (isUsed)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Phòng ban đã được sử dụng, không thể xóa"));
                }
                _departmentRepo.Update(department);
                return Ok(ApiResponseFactory.Success(null, "Xóa phòng ban thành công"));


            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save")]
        [RequiresPermission("N2,N1")]
        public async Task<IActionResult> savedepartment([FromBody] Department obj)
        {
            try
            {
                if(!_departmentRepo.Validate(obj, out String error))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, error));
                }
                obj.Code = obj.Code.Trim();
                obj.Name = obj.Name.Trim();
                if (obj.ID <= 0) await _departmentRepo.CreateAsync(obj);
                else _departmentRepo.Update(obj);

                return Ok(ApiResponseFactory.Success(null, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

    }
}
