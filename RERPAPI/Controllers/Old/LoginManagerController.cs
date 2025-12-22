using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LoginManagerController : ControllerBase
    {
        LoginManagerRepo loginManagerRepo;
        EmployeeRepo employeeRepo;
        public LoginManagerController(LoginManagerRepo loginManagerRepo, EmployeeRepo employeeRepo)
        {
            this.loginManagerRepo = loginManagerRepo;
            this.employeeRepo = employeeRepo;
        }
        [HttpGet("{id}")]
        [RequiresPermission("N1,N2,N60")]
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
        [RequiresPermission("N1,N2,N60")]
        public async Task<IActionResult> AddLoginInfo([FromBody] LoginInfoDTO loginInfo)
        {
            try
            {
                //var existingLoginName = SQLHelper<User>.FindByAttribute("LoginName", loginInfo.LoginName.Trim());
                //if(existingLoginName != null)
                //{
                //    return BadRequest(new
                //    {
                //        status = 0,
                //        message = "Tên đăng nhập đã tồn tại"
                //    });
                //}
                if (string.IsNullOrEmpty(loginInfo.Code))
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Chưa có thông tin nhân viên"
                    });
                }
                User user = loginManagerRepo.GetAll(x => x.LoginName.ToLower() == loginInfo.LoginName.ToLower() && x.Status != 1).FirstOrDefault()??new Model.Entities.User();
                Employee employee = employeeRepo.GetAll(x=> x.Code.ToLower() == loginInfo.Code && x.FullName.ToLower() == loginInfo.FullName.ToLower()).FirstOrDefault();


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
                    user.TeamID = loginInfo.TeamID;
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
                if (loginInfo.UserID > 0 && user.ID > 0 && employee.ID > 0)
                {
                    await loginManagerRepo.UpdateAsync(user);
                    await employeeRepo.UpdateAsync(employee);
                }
                else
                {
                    await loginManagerRepo.CreateAsync(user);
                    employee.UserID = user.ID;
                    await employeeRepo.UpdateAsync(employee);
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
