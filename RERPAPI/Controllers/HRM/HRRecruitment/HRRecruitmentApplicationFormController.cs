using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RERPAPI.Attributes;
using RERPAPI.Middleware;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.HRM
{
    [Route("api/[controller]")]
    [ApiController]
    public class HRRecruitmentApplicationFormController : ControllerBase
    {
        private readonly EmployeeChucVuHDRepo _employeeChucVuHDRepo;
        private readonly HRHiringCandidateInformationFormWorkingExperienceRepo _hRHiringCandidateInformationFormWorkingExperienceRepo;
        private readonly HRHiringCandidateInformationFormOtherCertificateRepo _hRHiringCandidateInformationFormOtherCertificateRepo;
        private readonly HRHiringCandidateInformationFormEducationRepo _hRHiringCandidateInformationFormEducationRepo;
        private readonly HRHiringCandidateInformationFormRepo _hRHiringCandidateInformationFormRepo;
        private readonly HRHiringCandidateInformationEmergencyContactRepo _hRHiringCandidateInformationEmergencyContactRepo;
        private readonly HRHiringCandidateInformationFormForeignLanguageSkillsRepo _hRHiringCandidateInformationFormForeignLanguageSkillsRepo;
        private readonly HRHiringCandidateInformationFormRecruitmentInfoRepo _hRHiringCandidateInformationFormRecruitmentInfoRepo;
        private readonly JwtSettings _jwtSettings;

        public HRRecruitmentApplicationFormController(
            EmployeeChucVuHDRepo employeeChucVuHDRepo, 
            HRHiringCandidateInformationFormWorkingExperienceRepo hRHiringCandidateInformationFormWorkingExperienceRepo, 
            HRHiringCandidateInformationFormOtherCertificateRepo hRHiringCandidateInformationFormOtherCertificateRepo, 
            HRHiringCandidateInformationFormEducationRepo hRHiringCandidateInformationFormEducationRepo, 
            HRHiringCandidateInformationFormRepo hRHiringCandidateInformationFormRepo,
            HRHiringCandidateInformationEmergencyContactRepo hRHiringCandidateInformationEmergencyContactRepo,
            HRHiringCandidateInformationFormForeignLanguageSkillsRepo hRHiringCandidateInformationFormForeignLanguageSkillsRepo,
            HRHiringCandidateInformationFormRecruitmentInfoRepo hRHiringCandidateInformationFormRecruitmentInfoRepo,
            JwtSettings jwtSettings)
        {
            _employeeChucVuHDRepo = employeeChucVuHDRepo;
            _hRHiringCandidateInformationFormWorkingExperienceRepo = hRHiringCandidateInformationFormWorkingExperienceRepo;
            _hRHiringCandidateInformationFormOtherCertificateRepo = hRHiringCandidateInformationFormOtherCertificateRepo;
            _hRHiringCandidateInformationFormEducationRepo = hRHiringCandidateInformationFormEducationRepo;
            _hRHiringCandidateInformationFormRepo = hRHiringCandidateInformationFormRepo;
            _hRHiringCandidateInformationEmergencyContactRepo = hRHiringCandidateInformationEmergencyContactRepo;
            _hRHiringCandidateInformationFormForeignLanguageSkillsRepo = hRHiringCandidateInformationFormForeignLanguageSkillsRepo;
            _hRHiringCandidateInformationFormRecruitmentInfoRepo = hRHiringCandidateInformationFormRecruitmentInfoRepo;
            _jwtSettings = jwtSettings;
        }

        //API lấy danh sách chức vụ ứng tuyển
        [HttpGet("get-all-chuc-vu")]
        public IActionResult GetAll()
        {
            try
            {
                var data = _employeeChucVuHDRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
                        //API lấy danh sách tờ khai 
                        [HttpGet("get-all-application-form")]
                        public IActionResult GetAllApplicationForm(int chucVuID, string? filterText)
                        {
                            try
                            {
                           //     var data = _hRHiringCandidateInformationFormRepo.GetAll(x => x.IsDeleted != true);
                var applicationForm = SQLHelper<dynamic>.ProcedureToList(
                                   "spGetHRCandidateApplicationForm",
                                   new[] { "@ChucVuHDID", "@FilterText" },
                                   new object[] { chucVuID, filterText });
                var dataList = SQLHelper<dynamic>.GetListData(applicationForm, 0);

                return Ok(ApiResponseFactory.Success(dataList, ""));
                            }
                            catch (Exception ex)
                            {
                                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
                            }           
                        }
                        //API lấy danh sách tờ khai 
                        [HttpGet("get-all-application-form-detail")]
                        public IActionResult GetAllApplicationFormDetail(int hRRecruitmentCandidateID)
                        {
                            try
                            {

                                var candidate = SQLHelper<dynamic>.ProcedureToList(
                                   "spGetHRRecruitmentApplicationForm",
                                   new[] { "@HRRecruitmentCandidateID" },
                                   new object[] { hRRecruitmentCandidateID });
                                //Lấy thông tin ứng viên
                                var applicationForm = SQLHelper<dynamic>.GetListData(candidate, 0);
                                var workingExperiences = SQLHelper<dynamic>.GetListData(candidate, 1);
                                var otherCertificates = SQLHelper<dynamic>.GetListData(candidate, 2);
                                var educations = SQLHelper<dynamic>.GetListData(candidate, 3);
                                var emergencyContacts = SQLHelper<dynamic>.GetListData(candidate, 4);
                                var foreignLanguageSkills = SQLHelper<dynamic>.GetListData(candidate, 5);
                                var recruitmentInfo = SQLHelper<dynamic>.GetListData(candidate, 6);

                                return Ok(ApiResponseFactory.Success(new
                                {
                                    applicationForm,
                                    workingExperiences,
                                    otherCertificates,
                                    educations,
                                    emergencyContacts,
                                    foreignLanguageSkills,
                                    recruitmentInfo
                                }, "Lấy dữ liệu thành công"));
                            }

                            catch (Exception ex)
                            {
                                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
                            }
                        }

        
            //Xóa form thông tin ứng viên 
            [HttpGet("delete-application-form")]
            public async Task<IActionResult> DeleteApplicationForm([FromQuery]  
        List<int> ids)
            {
                try
                {
                    int count = 0;

                    foreach (var id in ids)
                    {
                        var application = _hRHiringCandidateInformationFormRepo.GetByID(id);
                        if (application != null)
                        {
                            application.IsDeleted = true;
                            count += await _hRHiringCandidateInformationFormRepo.UpdateAsync(application);
                        }
                    }

                    if (count > 0)
                    {
                        return Ok(ApiResponseFactory.Success(count, $"Xóa thành công {count} bản ghi"));
                    }
                    else
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Không có bản ghi nào được xóa"));
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
                }
            }
        //API đăng nhập ứng viên
        [HttpPost("login-candidate")]
        public IActionResult LoginCandidate([FromBody] HRRecruitmentCandidate user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Password))
                {
                    return Unauthorized(ApiResponseFactory.Fail(null, "Vui lòng nhập Tên đăng nhập và Mật khẩu!"));
                }

                //1. Check user
                string loginName = user.UserName ?? "";
                string password = user.Password??"";
                //password = user.PasswordHash;
                var login = SQLHelper<object>.ProcedureToList("spLoginCandidate", new string[] { "@LoginName", "@Password" }, new object[] { loginName, password });
                var hasUsers = SQLHelper<object>.GetListData(login, 0);

                if (hasUsers.Count <= 0 || hasUsers[0].ID <= 0)
                {
                    return Unauthorized(ApiResponseFactory.Fail(null, "Sai tên đăng nhập hoặc mật khẩu!"));
                }

                var hasUser = SQLHelper<object>.GetListData(login, 0)[0];

                //2. Tạo Claims
                var claims = new List<Claim>()
                    {
                        new Claim(JwtRegisteredClaimNames.Sub,hasUser.ID.ToString()),
                        new Claim(JwtRegisteredClaimNames.UniqueName,hasUser.FullName ?? ""),
                    };

                var dictionary = (IDictionary<string, object>)hasUser;
                foreach (var item in dictionary)
                {
                    if (item.Key.ToLower() == "passwordhash") continue;

                    // Sửa: Đổi tên claim 'id' thành 'candidateid' để phân biệt với UserID của hệ thống
                    string claimKey = item.Key.ToLower() == "id" ? "candidateid" : item.Key.ToLower();
                    var claim = new Claim(claimKey, item.Value?.ToString() ?? "");
                    claims.Add(claim);
                }


                //3. Tạo token
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims.ToArray(),
                    expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);


                return Ok(new
                {
                    access_token = tokenString,
                    expires = token.ValidTo.AddHours(+7)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message + "\n"));
            }
        }
        //Get Current candidate
        //[ApiKeyAuthorize]
        [HttpGet("current-candidate")]
        public IActionResult GetCurrentUser()
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentCandidate(claims);
                return Ok(ApiResponseFactory.Success(currentUser, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-form")]
        public async Task<IActionResult> SaveForm([FromBody] HRRecruitmentApplicationFullDTO data)
        {
            try
            {
                if (data == null || data.HRRecruitmentApplicationForm == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu"));
                }

                // Lấy thông tin ứng viên từ claims
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentCandidate = ObjectMapper.GetCurrentCandidate(claims);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                // Phải có ít nhất một thông tin đăng nhập (Ứng viên hoặc Nhân viên hệ thống)
                if ((currentCandidate == null || currentCandidate.ID == 0) && (currentUser == null || currentUser.ID == 0))
                {
                    return Unauthorized(ApiResponseFactory.Fail(null, "Không tìm thấy thông tin đăng nhập (Vui lòng đăng nhập lại)"));
                }

                // 0. Validate data
                var validate = _hRHiringCandidateInformationFormRepo.Validate(data);
                if (validate.status == 0)
                {
                    return BadRequest(validate);
                }

                // 1. Lưu tờ khai chính
                var mainForm = data.HRRecruitmentApplicationForm;

                // Nếu là ứng viên đang đăng nhập, gán ID ứng viên để đảm bảo tính an toàn dữ liệu
                // Nếu là nhân viên hệ thống (HR), giữ nguyên ID từ payload (hoặc check quyền ở đây)
                if (currentCandidate != null && currentCandidate.ID > 0)
                {
                    mainForm.HRRecruitmentCandidateID = currentCandidate.ID;
                }
                else if (mainForm.HRRecruitmentCandidateID <= 0)
                {
                     return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy ID ứng viên trong dữ liệu gửi lên"));
                }

                if (mainForm.ID > 0)
                {
                    await _hRHiringCandidateInformationFormRepo.UpdateAsync(mainForm);
                }
                else
                {
                    await _hRHiringCandidateInformationFormRepo.CreateAsync(mainForm);
                }

                int parentId = mainForm.ID;

                // 2. Lưu Kinh nghiệp làm việc
                if (data.WorkingExperiences != null)
                {
                    foreach (var item in data.WorkingExperiences)
                    {
                        item.HRRecruitmentApplicationFormID = parentId;
                        if (item.ID > 0) await _hRHiringCandidateInformationFormWorkingExperienceRepo.UpdateAsync(item);
                        else await _hRHiringCandidateInformationFormWorkingExperienceRepo.CreateAsync(item);
                    }
                }

                // 3. Lưu Chứng chỉ khác
                if (data.OtherCertificates != null)
                {
                    foreach (var item in data.OtherCertificates)
                    {
                        item.HRRecruitmentApplicationFormID = parentId;
                        if (item.ID > 0) await _hRHiringCandidateInformationFormOtherCertificateRepo.UpdateAsync(item);
                        else await _hRHiringCandidateInformationFormOtherCertificateRepo.CreateAsync(item);
                    }
                }

                // 4. Lưu Thông tin học vấn
                if (data.Educations != null)
                {
                    foreach (var item in data.Educations)
                    {
                        item.HRHiringCandidateInformationFormID = parentId; // Lưu ý: Tên cột là HRHiringCandidateInformationFormID
                        if (item.ID > 0) await _hRHiringCandidateInformationFormEducationRepo.UpdateAsync(item);
                        else await _hRHiringCandidateInformationFormEducationRepo.CreateAsync(item);
                    }
                }

                // 5. Lưu Người liên hệ khẩn cấp
                if (data.EmergencyContacts != null)
                {
                    foreach (var item in data.EmergencyContacts)
                    {
                        item.HRRecruitmentApplicationFormID = parentId;
                        if (item.ID > 0) await _hRHiringCandidateInformationEmergencyContactRepo.UpdateAsync(item);
                        else await _hRHiringCandidateInformationEmergencyContactRepo.CreateAsync(item);
                    }
                }

                // 6. Lưu Ngoại ngữ
                if (data.ForeignLanguageSkills != null)
                {
                    foreach (var item in data.ForeignLanguageSkills)
                    {
                        item.HRHiringCandidateInformationFormID = parentId; // Lưu ý: Tên cột là HRHiringCandidateInformationFormID
                        if (item.ID > 0) await _hRHiringCandidateInformationFormForeignLanguageSkillsRepo.UpdateAsync(item);
                        else await _hRHiringCandidateInformationFormForeignLanguageSkillsRepo.CreateAsync(item);
                    }
                }

                // 7. Lưu Thông tin tuyển dụng (Dạng single object)
                if (data.RecruitmentInfo != null)
                {
                    var item = data.RecruitmentInfo;
                    item.HRRecruitmentApplicationFormID = parentId;
                    if (item.ID > 0) await _hRHiringCandidateInformationFormRecruitmentInfoRepo.UpdateAsync(item);
                    else await _hRHiringCandidateInformationFormRecruitmentInfoRepo.CreateAsync(item);
                }

                return Ok(ApiResponseFactory.Success(data, "Lưu tờ khai thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-form-auto")]
        public async Task<IActionResult> SaveFormAuto([FromBody] HRRecruitmentApplicationFullDTO data)
        {
            try
            {
                if (data == null || data.HRRecruitmentApplicationForm == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu"));
                }

                // Lấy thông tin ứng viên từ claims
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentCandidate = ObjectMapper.GetCurrentCandidate(claims);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                if ((currentCandidate == null || currentCandidate.ID == 0) && (currentUser == null || currentUser.ID == 0))
                {
                    return Unauthorized(ApiResponseFactory.Fail(null, "Không tìm thấy thông tin đăng nhập (Vui lòng đăng nhập lại)"));
                }
                // 1. Lưu tờ khai chính
                var mainForm = data.HRRecruitmentApplicationForm;

                if (currentCandidate != null && currentCandidate.ID > 0)
                {
                    mainForm.HRRecruitmentCandidateID = currentCandidate.ID;
                }
                else if (mainForm.HRRecruitmentCandidateID <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy ID ứng viên trong dữ liệu gửi lên"));
                }

                if (mainForm.ID > 0)
                {
                    await _hRHiringCandidateInformationFormRepo.UpdateAsync(mainForm);
                }
                else
                {
                    await _hRHiringCandidateInformationFormRepo.CreateAsync(mainForm);
                }

                int parentId = mainForm.ID;

                // 2. Lưu Kinh nghiệp làm việc
                if (data.WorkingExperiences != null)
                {
                    foreach (var item in data.WorkingExperiences)
                    {
                        item.HRRecruitmentApplicationFormID = parentId;
                        if (item.ID > 0) await _hRHiringCandidateInformationFormWorkingExperienceRepo.UpdateAsync(item);
                        else await _hRHiringCandidateInformationFormWorkingExperienceRepo.CreateAsync(item);
                    }
                }

                // 3. Lưu Chứng chỉ khác
                if (data.OtherCertificates != null)
                {
                    foreach (var item in data.OtherCertificates)
                    {
                        item.HRRecruitmentApplicationFormID = parentId;
                        if (item.ID > 0) await _hRHiringCandidateInformationFormOtherCertificateRepo.UpdateAsync(item);
                        else await _hRHiringCandidateInformationFormOtherCertificateRepo.CreateAsync(item);
                    }
                }

                // 4. Lưu Thông tin học vấn
                if (data.Educations != null)
                {
                    foreach (var item in data.Educations)
                    {
                        item.HRHiringCandidateInformationFormID = parentId; // Lưu ý: Tên cột là HRHiringCandidateInformationFormID
                        if (item.ID > 0) await _hRHiringCandidateInformationFormEducationRepo.UpdateAsync(item);
                        else await _hRHiringCandidateInformationFormEducationRepo.CreateAsync(item);
                    }
                }

                // 5. Lưu Người liên hệ khẩn cấp
                if (data.EmergencyContacts != null)
                {
                    foreach (var item in data.EmergencyContacts)
                    {
                        item.HRRecruitmentApplicationFormID = parentId;
                        if (item.ID > 0) await _hRHiringCandidateInformationEmergencyContactRepo.UpdateAsync(item);
                        else await _hRHiringCandidateInformationEmergencyContactRepo.CreateAsync(item);
                    }
                }

                // 6. Lưu Ngoại ngữ
                if (data.ForeignLanguageSkills != null)
                {
                    foreach (var item in data.ForeignLanguageSkills)
                    {
                        item.HRHiringCandidateInformationFormID = parentId; // Lưu ý: Tên cột là HRHiringCandidateInformationFormID
                        if (item.ID > 0) await _hRHiringCandidateInformationFormForeignLanguageSkillsRepo.UpdateAsync(item);
                        else await _hRHiringCandidateInformationFormForeignLanguageSkillsRepo.CreateAsync(item);
                    }
                }

                // 7. Lưu Thông tin tuyển dụng (Dạng single object)
                if (data.RecruitmentInfo != null)
                {
                    var item = data.RecruitmentInfo;
                    item.HRRecruitmentApplicationFormID = parentId;
                    if (item.ID > 0) await _hRHiringCandidateInformationFormRecruitmentInfoRepo.UpdateAsync(item);
                    else await _hRHiringCandidateInformationFormRecruitmentInfoRepo.CreateAsync(item);
                }

                return Ok(ApiResponseFactory.Success(data, "Lưu tờ khai thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //lấy master hợp đồng
        //[HttpGet("get-candidate-infomation")]
        //public IActionResult GetCandidateInfomation(int hRRecruitmentCandidateID)
        //{
        //    try
        //    {
        //        var candidate = SQLHelper<dynamic>.ProcedureToList(
        //           "spGetHRRecruitmentApplicationForm",
        //           new[] { "@HRRecruitmentCandidateID" },
        //           new object[] { hRRecruitmentCandidateID } );
        //        var dataList = SQLHelper<dynamic>.GetListData(candidate, 0);
        //        return Ok(ApiResponseFactory.Success(dataList, "Lấy dữ liệu thành công"));
        //    }

        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}

        ///Lấy thông tin ứng viên
        [HttpGet("get-candidate-infomation")]
        public IActionResult GetCandidateInfomation(int hRRecruitmentCandidateID)
                {
            try
            {
                
                var candidate = SQLHelper<dynamic>.ProcedureToList(
                   "spGetHRRecruitmentApplicationForm",
                   new[] { "@HRRecruitmentCandidateID" },
                   new object[] { hRRecruitmentCandidateID });
                //Lấy thông tin ứng viên
                var applicationForm = SQLHelper<dynamic>.GetListData(candidate, 0);
                var workingExperiences = SQLHelper<dynamic>.GetListData(candidate, 1);
                var otherCertificates = SQLHelper<dynamic>.GetListData(candidate, 2);
                var educations = SQLHelper<dynamic>.GetListData(candidate, 3);
                var emergencyContacts = SQLHelper<dynamic>.GetListData(candidate, 4);
                var foreignLanguageSkills = SQLHelper<dynamic>.GetListData(candidate, 5);
                var recruitmentInfo = SQLHelper<dynamic>.GetListData(candidate, 6);

                return Ok(ApiResponseFactory.Success(new {applicationForm,
                    workingExperiences,
                    otherCertificates,
                    educations,
                    emergencyContacts,
                    foreignLanguageSkills,
                    recruitmentInfo
                }, "Lấy dữ liệu thành công"));
            }

            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
