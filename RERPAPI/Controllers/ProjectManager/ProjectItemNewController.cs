using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.Asset;
using RERPAPI.Model.DTO.Project;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.Asset;
using RERPAPI.Repo.GenericEntity.Project;
using System.Net.WebSockets;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectItemNewController : ControllerBase
    {
        ProjectItemProblemRepo _projectItemProblemRepo;
        ProjectItemRepo _projectItemRepo;
        ProjectItemFileRepo _projectItemFileRepo;
        public ProjectItemNewController(ProjectItemProblemRepo projectItemProblemRepo, ProjectItemRepo projectItemRepo, ProjectItemFileRepo projectItemFileRepo)
        {
            _projectItemProblemRepo = projectItemProblemRepo;
            _projectItemRepo = projectItemRepo;
            _projectItemFileRepo = projectItemFileRepo;
        }
        [Authorize]
        //Hàm check truy cập cho hạng mục công việc 
        [HttpGet("get-project-employee-permission")]
        public IActionResult GetProjectEmployeePermission([FromQuery] int? projectID, int? employeeID)
        {
            try
            {
                var rowNum = SQLHelper<dynamic>.ProcedureToList(
                "spGetProjectEmployeePermisstion",
                new string[] { "@ProjectID", "@EmployeeID" },
                new object[] { projectID, employeeID } );
                var data = SQLHelper<dynamic>.GetListData(rowNum, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //hàm lấy list hạng mục công việc từ ProjectID
        [HttpGet("get-project-item")]
        public IActionResult GetProjectItem([FromQuery] int projectID)
        {
            try
            {
                var projectItem = SQLHelper<dynamic>.ProcedureToList("spGetProjectItem",
                new string[] { "@ProjectID" },
                new object[] { projectID});
                var projectItemData = SQLHelper<dynamic>.GetListData(projectItem, 0);
                return Ok(ApiResponseFactory.Success(projectItemData, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //Hàm lấy mã hạng mục công việc
       // [Authorize]
        [HttpGet("get-project-item-code")]
        public IActionResult GetProjectItemCode([FromQuery] int projectId)
        {
            try
            {
                string newCode = _projectItemRepo.GenerateProjectItemCode(projectId);
                var data = newCode;
                return Ok(ApiResponseFactory.Success(newCode, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }
        // Hàm upload file
        [HttpPost("upload")]
        public IActionResult Upload(IFormFile file)
        {
            try
            {
                int statusCode = 0;
                string fileName = "";
                string message = "Upload file thất bại!";
                if (file != null)
                {               
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                    var fileExtension = Path.GetExtension(file.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return BadRequest(new
                        {
                            status = 0,
                            Message = "Chỉ được upload file ảnh (jpg, jpeg, png, gif, bmp)"
                        });
                    }
                    string path = "D:\\LeTheAnh\\Image\\Upload\\Mau";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using (FileStream fileStream = System.IO.File.Create(path + file.FileName))
                    {
                        file.CopyTo(fileStream);
                        fileStream.Flush();

                            statusCode = 1;
                            fileName = file.FileName;
                            message = "Upload File thành công!";
                    }
                }
                else
                {
                    statusCode = 0;
                    message = "Không có file được gửi lên.";
                }
            
                return Ok(ApiResponseFactory.Success(new { statusCode, fileName, message }, "Upload file thành công"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //Hàm lưu dữ liệu
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] ProjectItemFullDTO projectItem)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
                bool isTBP =  currentUser.EmployeeID == 54;
                bool isPBP = (currentUser.PositionCode == "CV57" || currentUser.PositionCode == "CV28");
                //Lưu hạng mục công việc nếu có

                if (projectItem.projectItems != null)
                {
                    if(currentUser.IsAdmin)
                    foreach (var item in projectItem.projectItems)
                    {
                            // Nếu có IsDeleted = true thì check quyền trước
                            if (item.IsDeleted == true)
                            {
                                if (!(isTBP || isPBP || currentUser.IsAdmin))
                                {
                                    return BadRequest(ApiResponseFactory.Fail(null, "Bạn không có quyền xóa hạng mục"));
                                }
                            }
                            if (item.ID <= 0)
                            await _projectItemRepo.CreateAsync(item);
                        else
                            _projectItemRepo.Update(item);
                    }
                }
                // Lưu phát sinh của hạng mục nếu có
                if (projectItem.projectItemProblem != null)
                {
                    if (projectItem.projectItemProblem.ID <= 0)
                        await _projectItemProblemRepo.CreateAsync(projectItem.projectItemProblem);
                    else
                        _projectItemProblemRepo.Update(projectItem.projectItemProblem);
                }
                //Lưu projectFile nếu có
                if (projectItem.ProjectItemFile != null)
                {
                    if (projectItem.ProjectItemFile.ID <= 0)
                        await _projectItemFileRepo.CreateAsync(projectItem.ProjectItemFile);
                    else
                        _projectItemFileRepo.Update(projectItem.ProjectItemFile);
                }
                return Ok(ApiResponseFactory.Success(1,"Lưu thành công"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }
    }
}
