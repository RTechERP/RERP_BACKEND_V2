using MathNet.Numerics.Distributions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.Asset;
using RERPAPI.Model.DTO.Project;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.Project;
using RERPAPI.Repo.GenericEntity.Asset;
using RERPAPI.Repo.GenericEntity.Project;
using System.Net.WebSockets;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectItemNewController : ControllerBase
    {
        private readonly ProjectItemProblemRepo _projectItemProblemRepo;
        private readonly ProjectItemRepo _projectItemRepo;
        private readonly ProjectItemFileRepo _projectItemFileRepo;

        public ProjectItemNewController(
            ProjectItemProblemRepo projectItemProblemRepo,
            ProjectItemRepo projectItemRepo,
            ProjectItemFileRepo projectItemFileRepo
        )
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
                new object[] { projectID, employeeID });
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
                new object[] { projectID });
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
                bool isTBP = currentUser.EmployeeID == 54;
                bool isPBP = (currentUser.PositionCode == "CV57" || currentUser.PositionCode == "CV28");
                //Lưu hạng mục công việc nếu có

                if (projectItem.projectItems != null)
                {
                    if (currentUser.IsAdmin)
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


                return Ok(ApiResponseFactory.Success(1, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }



        #region hạng mục công việc cá nhân 
        //API lấy list hạng mục công việc cá nhân
        [HttpPost("get-project-item-person")]
        public IActionResult GetProjectItem(ProjectItemRequestParam request)
        {
            try
            {
                var projectItem = SQLHelper<dynamic>.ProcedureToList("spGetProjectItem",
                    new[] { "@ProjectID", "@UserID", "@Keyword", "@Status" },
                    new object[] { request.ProjectID, request.UserID, request.Keyword, request.Status });
                var rows = SQLHelper<dynamic>.GetListData(projectItem, 0);
                return Ok(ApiResponseFactory.Success(rows, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //load người giao việc
        [HttpGet("get-employee-request")]
        public IActionResult GetEmployeeRequest()
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                int employeeRequest = 0;
                ProjectItem lastItem = _projectItemRepo.GetAll().Where(x => x.UserID == currentUser.ID && x.EmployeeIDRequest > 0)
                                                      .OrderByDescending(x => x.ID).FirstOrDefault();
                if (lastItem != null)
                {
                    employeeRequest = lastItem.EmployeeIDRequest ?? 0;
                }
                var projectItem = SQLHelper<dynamic>.ProcedureToList("spGetEmployeeRequestProjectItem",
                    new string[] { },
                    new object[] { });
                var rows = SQLHelper<dynamic>.GetListData(projectItem, 0);
                return Ok(ApiResponseFactory.Success(new { employeeRequest, rows }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetDataByID(int projectItemID)
        {
            try
            {
                ProjectItem data = _projectItemRepo.GetByID(projectItemID);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch(Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-project-item-parent")]
        public async Task<IActionResult> GetDataParent(int projectID)
        {
            try
            {
                var data = _projectItemRepo.GetAll(x=>x.ParentID ==0 && x.IsDeleted == false && x.ProjectID ==projectID);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data-person")]
        public async Task<IActionResult> SaveDataPerson([FromBody] ProjectItemFullDTO projectItem)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
             
                //Lưu hạng mục công việc nếu có
                int projectID = 0;
                if (projectItem.projectItems != null)
                {
                    foreach (var item in projectItem.projectItems)
                    {
                        projectID = item.ProjectID ?? 0;

                        if (!_projectItemRepo.Validate(item, out string mesage))
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, mesage));
                        }
                        if (item.ID <= 0)
                        {
                            item.STT = _projectItemRepo.GetMaxSTT(item.ProjectID);
                            item.UserID = currentUser.ID;
                            item.ItemLate = 0;
                            _projectItemRepo.CalculateDays(item);
                            if (item.ActualEndDate.HasValue) item.IsApproved = 2;
                            await _projectItemRepo.CreateAsync(item);
                        }
                        else
                        {
                            item.ItemLate = 0;
                            if (item.ActualEndDate.HasValue && item.IsApproved < 2)
                                item.IsApproved = 2;
                            _projectItemRepo.CalculateDays(item);
                            await _projectItemRepo.UpdateAsync(item);
                        }
                    }
                    if (projectID > 0)
                    {
                        await _projectItemRepo.UpdatePercent(projectID);
                        await _projectItemRepo.UpdateLate(projectID);
                    }
                    // Lưu phát sinh của hạng mục nếu có
                    if (projectItem.projectItemProblem != null)
                    {
                        if (projectItem.projectItemProblem.ID <= 0)
                            await _projectItemProblemRepo.CreateAsync(projectItem.projectItemProblem);
                        else
                            _projectItemProblemRepo.Update(projectItem.projectItemProblem);
                    }
                }
                //Lưu projectFile nếu có
                if (projectItem.ProjectItemFile != null)
                {
                    if (projectItem.ProjectItemFile.ID <= 0)
                        await _projectItemFileRepo.CreateAsync(projectItem.ProjectItemFile);
                    else
                        _projectItemFileRepo.Update(projectItem.ProjectItemFile);
                }

                return Ok(ApiResponseFactory.Success(1, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
    }
}
