using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.Project
{
    [ApiController]
    [Route("api/project-type-department")]
    [Authorize]
    public class ProjectTypeDepartmentController : ControllerBase
    {
        private readonly ProjectTypeDepartmentRepo _projectTypeDepartmentRepo;


        public ProjectTypeDepartmentController(ProjectTypeDepartmentRepo projectTypeDepartmentRepo)
        {
            _projectTypeDepartmentRepo = projectTypeDepartmentRepo;

        }
        //Lấy kiểu dự án theo phòng ban
        [HttpGet("get-by-department/{departmentId}")]
        public IActionResult GetByDepartment(int departmentId)
        {
            try
            {
                var data = SQLHelper<ProjectTypeDTO>.ProcedureToListModel(
                    "spGetProjectTypeByDepartment",
                    new string[] { "@DepartmentID" },
                    new object[] { departmentId }
                );

                return Ok(ApiResponseFactory.Success(data));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //Lấy tất cả kiểu dự án và phòng ban liên kết
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                var data = _projectTypeDepartmentRepo.GetAll(x => x.IsDeleted == false).ToList();
                return Ok(ApiResponseFactory.Success(data));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //Lưu link kiểu dự án theo phòng ban
        [HttpPost("save-by-department")]
        public async Task<IActionResult> SaveByDepartment([FromBody] SaveProjectTypeDepartmentDto request)
        {
            try
            {
                if (request == null || request.DepartmentID <= 0)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));
                }

                var currentUsername = User.Identity?.Name ?? "system";
                var now = DateTime.Now;

                // Lấy các liên kết hiện tại
                var existingLinks = _projectTypeDepartmentRepo.GetAll(x => x.DepartmentID == request.DepartmentID && x.IsDeleted == false)
                    .ToList();

                var incomingTypeIds = request.ProjectTypeIDs ?? new List<int>();

                // 1. Xử lý các liên kết bị xóa
                foreach (var link in existingLinks)
                {
                    if (!incomingTypeIds.Contains(link.ProjectTypeID ?? 0))
                    {
                        link.IsDeleted = true;
                        link.UpdatedBy = currentUsername;
                        link.UpdatedDate = now;
                        await _projectTypeDepartmentRepo.UpdateAsync(link);
                    }
                }
                // 2. Xử lý các liên kết được thêm mới
                var existingTypeIds = existingLinks.Select(x => x.ProjectTypeID ?? 0).ToList();
                foreach (var typeId in incomingTypeIds)
                {
                    if (!existingTypeIds.Contains(typeId))
                    {
                        // Kiểm tra xem trước đó đã có bản ghi IsDeleted = true chưa để reuse, tránh trùng PK
                        var deletedLink = _projectTypeDepartmentRepo.GetAll(x => x.DepartmentID == request.DepartmentID && x.ProjectTypeID == typeId && x.IsDeleted == true).FirstOrDefault();
                        if (deletedLink != null)
                        {
                            deletedLink.IsDeleted = false;
                            deletedLink.UpdatedBy = currentUsername;
                            deletedLink.UpdatedDate = now;
                            await _projectTypeDepartmentRepo.UpdateAsync(deletedLink);
                        }
                        else
                        {
                            var newLink = new ProjectTypeDepartment
                            {
                                DepartmentID = request.DepartmentID,
                                ProjectTypeID = typeId,
                                IsDeleted = false,
                                CreatedBy = currentUsername,
                                CreatedDate = now,
                                UpdatedBy = currentUsername,
                                UpdatedDate = now
                            };
                            await _projectTypeDepartmentRepo.CreateAsync(newLink);
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //Xóa link kiểu dự án khỏi phòng ban
        [HttpDelete("delete-link/{departmentId}/{projectTypeId}")]
        public async Task<IActionResult> DeleteLink(int departmentId, int projectTypeId)
        {
            try
            {
                var link = _projectTypeDepartmentRepo.GetAll(x => x.DepartmentID == departmentId && x.ProjectTypeID == projectTypeId && x.IsDeleted == false).FirstOrDefault();
                if (link != null)
                {
                    var currentUsername = User.Identity?.Name ?? "system";
                    link.IsDeleted = true;
                    link.UpdatedBy = currentUsername;
                    link.UpdatedDate = DateTime.Now;
                    await _projectTypeDepartmentRepo.UpdateAsync(link);
                }
                return Ok(ApiResponseFactory.Success(null, "Xóa liên kết thành công"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
