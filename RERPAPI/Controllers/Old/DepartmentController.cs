using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        DepartmentRepo _departmentRepo = new DepartmentRepo();

        [HttpGet("get-all")]
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

        [HttpGet("{id}")]
        public IActionResult DeleteDepartment(int id)
        {
            try
            {
                var department = _departmentRepo.GetByID(id);
                List<Employee> checkList = SQLHelper<Employee>.FindByAttribute("DepartmentID", id).ToList();
                if (checkList.Count > 0)
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Phòng ban này đang được sử dụng nên không thể xóa được!\'"
                    });
                }
                if (department != null)
                {
                    _departmentRepo.Delete(department.ID);
                    return Ok(new
                    {
                        status = 1,
                        message = "Xóa phòng ban thành công."
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        status = 0,
                        message = "Phòng ban không tồn tại"
                    });
                }
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

    }
}
