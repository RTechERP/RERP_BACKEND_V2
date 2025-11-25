using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectCurrentSituationController : ControllerBase
    {
        private readonly ProjectCurrentSituationRepo _projectCurrentSituationRepo;
        public ProjectCurrentSituationController(
          ProjectCurrentSituationRepo projectCurrentSituation)
        {
             _projectCurrentSituationRepo=projectCurrentSituation;
        }

        [HttpGet("get-data")]
        public async Task<IActionResult> GetData(int projectID)
        {
            try
            {
                var projectCurrentSituation = SQLHelper<object>.ProcedureToList("spGetProjectCurrentSituation",
                    new string[] { "@ProjectID" }, new object[] { projectID });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(projectCurrentSituation,0), "Lấy dữu liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] Model.Entities.ProjectCurrentSituation item)
        {
            try
            {
                if (!_projectCurrentSituationRepo.Validate(item, out string message))
                {
                    return Ok(new { status = 2, message = message });
                }
                item.DateSituation = DateTime.Now;
                if (item.ID > 0)
                {
                    await _projectCurrentSituationRepo.UpdateAsync(item);
                }
                else
                {
                    await _projectCurrentSituationRepo.CreateAsync(item);
                }
                return Ok(ApiResponseFactory.Success(null, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

    }
}
