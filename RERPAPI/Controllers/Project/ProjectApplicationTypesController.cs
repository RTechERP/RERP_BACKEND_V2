using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.Project;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectApplicationTypesController : Controller
    {
        ProjectApplicationTypesRepo _projectApplicationType;

        public ProjectApplicationTypesController(ProjectApplicationTypesRepo projectApplicationTypeRepo)
        {
            _projectApplicationType = projectApplicationTypeRepo;
        }

        [HttpGet("project-application-type")]
        public async Task<IActionResult> GetProjectApplicationTypes( int? projectTypeID)
        {
            try
            {
                var param = new
                {
                    ProjectTypeID = projectTypeID ?? 0,
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetProjectApplicationTypes", param);

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //[RequiresPermission("N1,N2,N34,N89")]
        [HttpPost("save-project-application-type")]
        public async Task<IActionResult> SaveData([FromBody] ProjectApplicationType projectApplicationType)
        {
            try
            {

                if (projectApplicationType.ID <= 0)
                {
                    await _projectApplicationType.CreateAsync(projectApplicationType);
                }
                else
                {
                    _projectApplicationType.UpdateAsync(projectApplicationType);
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


        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] List<int> ids)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                if (ids == null || ids.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn loại ứng dụng dự án để xóa"));
                foreach (var item in ids)
                {

                    var project = _projectApplicationType.GetByID(item);
                    project.IsDeleted = true;
                    await _projectApplicationType.UpdateAsync(project);

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
