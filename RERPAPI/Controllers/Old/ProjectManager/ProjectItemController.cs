using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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

namespace RERPAPI.Controllers.Old.ProjectManager
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyAuthorize]
    public class ProjectItemController : ControllerBase
    {
        ProjectItemProblemRepo _projectItemProblemRepo = new ProjectItemProblemRepo();
        ProjectItemRepo _projectItemRepo = new ProjectItemRepo();
        ProjectItemFileRepo _projectItemFileRepo = new ProjectItemFileRepo();

        //API lấy list hạng mục công việc 
        [ApiKeyAuthorize]
        [HttpPost("get-project-item")]
        public IActionResult GetProjectItem(ProjectItemRequestParam request)
        {
            try
            {
                var projectItem = SQLHelper<dynamic>.ProcedureToList("spGetProjectItem",
                    new[] { "DateStart", "DateEnd", "@ProjectID", "@UserID", "@Keyword", "@Status" },
                    new object[] { request.DateStart, request.DateEnd, request.ProjectID, request.UserID, request.Keyword, request.Status });
                var rows = SQLHelper<dynamic>.GetListData(projectItem, 0);
                return Ok(ApiResponseFactory.Success(rows, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //Hàm lấy mã hạng mục công việc
        [ApiKeyAuthorize]
        [HttpGet("get-project-item-code")]
        public IActionResult GetProjectItemCode([FromQuery] int projectId)
        {
            try
            {
                string newCode = _projectItemRepo.GenerateProjectItemCode(projectId);
                var data = newCode;
                return Ok(ApiResponseFactory.Success(newCode, "Lấy mã thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }
        // API tính lại và cập nhật giá trị ItemLate cho tất cả ProjectItem của 1 Project.
        //private void RecalcItemLateInternal(int projectId)
        //{
        //    try
        //    {
        //        var today = DateTime.Now;
        //        var items = _projectItemRepo.GetAll(x => x.ProjectID == projectId && !x.IsDeleted);
        //        foreach (var it in items)
        //        {
        //            var v = _projectItemRepo.CalcItemLate(it.PlanStartDate, it.PlanEndDate, it.ActualStartDate, it.ActualEndDate, today);
        //            it.ItemLate = v;

        //            _projectItemRepo.Update(it);
        //        }
        //    }
        //    catch(Exception ex)

        //    {
        //        throw new Exception("Lỗi"+ex);
        //    }


        //}

        //Hàm lưu dữ liệu
        [ApiKeyAuthorize]
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] ProjectItemFullDTO dto)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                bool isTBP = currentUser.EmployeeID == 54;
                bool isPBP = currentUser.PositionCode == "CV57" || currentUser.PositionCode == "CV28";
                // 1) Hạng mục
                if (dto.projectItems != null)
                {
                    foreach (var item in dto.projectItems)
                    {
                        ProjectItem? existing = item.ID > 0 ? _projectItemRepo.GetByID(item.ID) : null;
                        int approved = existing?.IsApproved ?? 0;

                        // Xóa mềm
                        if (item.IsDeleted == true && item.ID != 0)
                        {
                            if (!(currentUser.IsAdmin || isTBP || isPBP))
                                return BadRequest(ApiResponseFactory.Fail(null, "Bạn không có quyền xóa hạng mục"));
                            if (approved > 0)
                                return BadRequest(ApiResponseFactory.Fail(null, "Hạng mục đã duyệt không thể xóa"));

                            if (existing != null)
                            {
                                existing.IsDeleted = true;
                                _projectItemRepo.Update(existing);
                            }
                            continue;
                        }

                        // Thêm / Sửa
                        if (item.ID <= 0)
                        {
                            string validateMsg;
                            if (!_projectItemRepo.Validate(item, out validateMsg))
                            {
                                return BadRequest(ApiResponseFactory.Fail(null, validateMsg));
                            }

                            item.IsDeleted = false;
                            await _projectItemRepo.CreateAsync(item);
                        } 
                        else
                        {
                            string validateMsg;
                            if (!_projectItemRepo.Validate(item, out validateMsg))
                            {
                                return BadRequest(ApiResponseFactory.Fail(null, validateMsg));
                            }
                            if (!currentUser.IsAdmin && approved > 0)
                                return BadRequest( ApiResponseFactory.Fail(null, "Hạng mục đã duyệt không thể sửa"));
                            _projectItemRepo.Update(item);
                        }
                    }
                }

                // 2) Phát sinh
                if (dto.projectItemProblem != null)
                {
                    if (dto.projectItemProblem.ID <= 0)
                    {
                        await _projectItemProblemRepo.CreateAsync(dto.projectItemProblem);
                    }
                    else
                    {
                        _projectItemProblemRepo.Update(dto.projectItemProblem);
                    }
                }
                //// 3) File--Tạm thời chưa cần
                //if (dto.ProjectItemFile != null)
                //{
                //    if (dto.ProjectItemFile.ID <= 0)
                //    {
                //        await _projectItemFileRepo.CreateAsync(dto.ProjectItemFile);
                //    }
                //    else
                //    {
                //        _projectItemFileRepo.Update(dto.ProjectItemFile);
                //    }
                //}

                // 4) Tính lại % theo TotalDayPlan cho toàn bộ Project
                int projectId = dto.projectItems?.FirstOrDefault()?.ProjectID ?? 0;
                if (projectId > 0)
                {
                    var items = _projectItemRepo.GetAll(x => x.ProjectID == projectId && x.IsDeleted == false);
                    decimal total = items.Sum(x => x.TotalDayPlan ?? 0m);
                    foreach (var it in items)
                    {
                        var plan = it.TotalDayPlan ?? 0m;

                        it.PercentItem = total > 0 ? plan / total * 100m : 0m;

                        _projectItemRepo.Update(it);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu hạng mục thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
