using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.Project;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectTechnologiesController : Controller
    {
        ProjectTechnologiesRepo _projectTechnologyRepo;

        public ProjectTechnologiesController(ProjectTechnologiesRepo projectTechnologyRepo)
        {
            _projectTechnologyRepo = projectTechnologyRepo;
        }

        [HttpGet("project-technology")]
        public async Task<IActionResult> GetProjectTechnology(int? projectTypeID)
        {
            try
            {
                var param = new
                {
                    ProjectTypeID = projectTypeID ?? 0,
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetProjectTechnologies", param);

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [RequiresPermission("N1,N13,N27")]
        [HttpPost("save-project-technology")]
        public async Task<IActionResult> SaveData([FromBody] ProjectTechnology projectTechnologies)
        {
            try
            {
                var exists = _projectTechnologyRepo.GetAll(x =>
                             x.ProjectTypeID == (projectTechnologies.ProjectTypeID ?? 0) &&
                             x.TechnologyName == projectTechnologies.TechnologyName &&
                             x.ID != projectTechnologies.ID && x.IsDeleted != true);
                if (exists.Count > 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null,
                        $"Công nghệ: [{projectTechnologies.TechnologyName}] đã tồn tại!"));
                }
                if (projectTechnologies.ID <= 0)
                {
                    await _projectTechnologyRepo.CreateAsync(projectTechnologies);
                }
                else
                {
                    await _projectTechnologyRepo.UpdateAsync(projectTechnologies);
                }

                return Ok(new
                {
                    status = 1,
                    message = "Lưu thành công."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N13,N27")]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] List<int> ids)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                if (ids == null || ids.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn dự án để xóa"));
                foreach (var item in ids)
                {

                    var project = _projectTechnologyRepo.GetByID(item);
                    project.IsDeleted = true;
                    await _projectTechnologyRepo.UpdateAsync(project);

                }
                return Ok(ApiResponseFactory.Success(ids, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
