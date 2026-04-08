using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectTaskTypeController : ControllerBase
    {
        private readonly ProjectTaskTypeRepo _projectTaskTypeRepo;
        public ProjectTaskTypeController(ProjectTaskTypeRepo projectTaskTypeRepo)
        {
            _projectTaskTypeRepo = projectTaskTypeRepo;
        }

        [HttpGet]
        [RequiresPermission("N90")]
        public IActionResult GetProjectTaskType()
        {
            try
            {
                var result = _projectTaskTypeRepo.GetAll(x => !x.IsDeleted ?? true);
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to get project task type."));
            }
        }
        
        [HttpPost]
        [RequiresPermission("N90")]
        public async Task<IActionResult> CreateProjectTaskType([FromBody] ProjectTaskType projectTaskType)
        {
            try
            {
                if(string.IsNullOrEmpty(projectTaskType.Code) )
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "MÃ CODE KHÔNG ĐƯỢC ĐỂ TRỐNG, VUI LÒNG NHẬP MÃ CODE !"));
                }
                if( int.TryParse(projectTaskType.Code, out int code))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "MÃ CODE PHẢI LÀ CHỮ HOẶC CHỮ VÀ SỐ VUI LÒNG NHẬP MÃ CODE !"));
                }
                var exitCode = _projectTaskTypeRepo.GetAll(x => (!x.IsDeleted ?? true) && (x.Code.ToUpper().Equals(projectTaskType.Code.ToUpper()))).FirstOrDefault();
                if (projectTaskType.ID > 0)
                {
                    if(exitCode != null && exitCode.ID > 0 && exitCode.ID != projectTaskType.ID)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "MÃ CODE ĐÃ TỒN TẠI, VUI LÒNG NHẬP MÃ CODE MỚI !"));
                    }
                    var existing = await _projectTaskTypeRepo.GetByIDAsync(projectTaskType.ID);
                    existing.TypeName = projectTaskType.TypeName;
                    existing.Code = projectTaskType.Code.Replace(" ", "").ToUpper();
                    existing.IsDeleted = projectTaskType.IsDeleted;
                    existing.DepartmentID = projectTaskType.DepartmentID ?? 0;
                    existing.Color = projectTaskType.Color;
                    existing.UpdatedDate = DateTime.Now;
                    if (await _projectTaskTypeRepo.UpdateAsync(existing) > 0)
                    {
                        return Ok(ApiResponseFactory.Success(existing, "Project task type updated successfully."));
                    }
                }
                else
                {
                    if(exitCode != null && exitCode.ID > 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "MÃ CODE ĐÃ TỒN TẠI, VUI LÒNG NHẬP MÃ CODE MỚI !"));
                    }
                    projectTaskType.Code = projectTaskType.Code.Replace(" ","").ToUpper();
                    if (await _projectTaskTypeRepo.CreateAsync(projectTaskType) > 0)
                    {
                        return Ok(ApiResponseFactory.Success(projectTaskType, "Project task type created successfully."));
                    }
                }
                return BadRequest(ApiResponseFactory.Fail(null, "Project task type created fail."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to create project task type."));
            }
        }
    }
}
