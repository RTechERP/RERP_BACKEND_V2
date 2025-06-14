using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        DepartmentRepo departmentRepo = new DepartmentRepo();

        [HttpGet]
        public IActionResult GetAll()
        {

            try
            {
                List<Department> departments = departmentRepo.GetAll().Where(x => x.IsDeleted == false).OrderBy(x => x.STT).ToList();

                return Ok(new
                {
                    status = 1,
                    data = departments
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }

        }


        [HttpGet("{id}")]
        public IActionResult GetDepartmentById(int id)
        {
            try
            {
                var department = departmentRepo.GetByID(id);
                if (department == null)
                {
                    return NotFound(new
                    {
                        status = 0,
                        message = "Department not found"
                    });
                }
                return Ok(new
                {
                    status = 1,
                    data = department
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
        [HttpPost]
        public async Task<IActionResult> SaveDepartment([FromBody] Department department)
        {
            try
            {
                List<Department> departments = departmentRepo.GetAll().Where(x => x.IsDeleted == false).ToList();
                department.STT = departments.Count + 1;
                department.CreatedDate = DateTime.Now;
                department.UpdatedDate = DateTime.Now;
                if (departments.Any(d => d.Code == department.Code && d.ID != department.ID))
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Mã phòng ban đã tồn tại"
                    });
                }
                if (department.ID <= 0)
                {
                    await departmentRepo.CreateAsync(department);
                } else
                {
                    departmentRepo.UpdateFieldsByID(department.ID, department);
                }
                return Ok(new
                {
                    status = 1,
                    data = department,
                    message = "Lưu phòng ban thành công."
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



        [HttpDelete("{id}")]
        public IActionResult DeleteDepartment(int id)
        {
            try
            {
                var department = departmentRepo.GetByID(id);
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
                    departmentRepo.Delete(department.ID);
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
