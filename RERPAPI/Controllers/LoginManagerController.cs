using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Security.Cryptography;

namespace RERPAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginManagerController : ControllerBase
    {
        LoginManagerRepo loginManagerRepo = new LoginManagerRepo();
        EmployeeRepo employeeRepo = new EmployeeRepo();
        [HttpGet("{id}")]
        public IActionResult GetLoginInfo(int id)
        {
            try
            {
                var loginInfo = loginManagerRepo.GetByID(id);
                if (loginInfo != null)
                {
                    return Ok(new
                    {
                        status = 1,
                        data = loginInfo
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        status = 0,
                        message = "User not found"
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
        [HttpPost]  
        public async Task<IActionResult> AddLoginInfo([FromBody] LoginInfoDTO loginInfo)
        {
            try
            {
                if (string.IsNullOrEmpty(loginInfo.Code))
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Chưa có thông tin nhân viên"
                    });
                }
                User user = new User();
                Employee employee = new Employee();

                if(loginInfo.UserID > 0)
                {
                    user = loginManagerRepo.GetByID(loginInfo.UserID);
                    if(user == null)
                    {
                        return BadRequest(new
                        {
                            status = 0,
                            message = "Không tìm thấy thông tin người dùng"
                        });
                    }
                    employee = (SQLHelper<Employee>.FindByAttribute("UserID", loginInfo.UserID.ToString())).FirstOrDefault();
                    if (employee == null)
                    {
                        return BadRequest(new
                        {
                            status = 0,
                            message = "Không tìm thấy nhân viên liên kết với người dùng"
                        });
                    }
                }
                else
                {
                    // For new user, find employee by Code
                    employee = (SQLHelper<Employee>.FindByAttribute("Code", loginInfo.Code)).FirstOrDefault();
                    if (employee == null)
                    {
                        return BadRequest(new
                        {
                            status = 0,
                            message = "Không tìm thấy nhân viên với mã được cung cấp"
                        });
                    }
                }
                if (loginInfo.Status)
                {
                    // Validate login name and password
                    if (string.IsNullOrEmpty(loginInfo.LoginName) || string.IsNullOrEmpty(loginInfo.PasswordHash))
                    {
                        return BadRequest(new
                        {
                            status = 0,
                            message = "Tên đăng nhập và mật khẩu không được để trống"
                        });
                    }

                    user.LoginName = loginInfo.LoginName.Trim();
                    user.PasswordHash = MaHoaMD5.EncryptPassword(loginInfo.PasswordHash.Trim());
                    user.Status = 0;
                    user.FullName = loginInfo.FullName;
                    employee.Status = 0;
                    employee.EndWorking = null;
                }
                else
                {
                    user.Status = 1;
                    employee.Status = 1;
                }

                // Set TeamID
                user.TeamID = loginInfo.TeamID;

                // Update or insert records
                if (loginInfo.UserID > 0)
                {
                    loginManagerRepo.UpdateFieldsByID(user.ID, user);
                    employeeRepo.UpdateFieldsByID(employee.ID, employee);
                }
                else
                {
                    user.ID = await loginManagerRepo.CreateAsync(user);
                    employee.UserID = user.ID;
                    employeeRepo.UpdateFieldsByID(employee.ID, employee);
                }

                return Ok(new
                {
                    status = 1,
                    message = "Cập nhật thông tin đăng nhập thành công"
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

        
    }
}
