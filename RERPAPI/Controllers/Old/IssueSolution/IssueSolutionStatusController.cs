using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.IssueSolution
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssueSolutionStatusController : ControllerBase
    {
        private readonly IssueSolutionStatusRepo _issueSolutionStatusRepo = new IssueSolutionStatusRepo();

        [HttpGet()]
        public IActionResult GetAllIssueSolutionStatus()
        {
            try
            {
                var listIssueSolutionStatus = _issueSolutionStatusRepo.GetAll().Where(x => x.IsDeleted != true).ToList();
                return Ok(ApiResponseFactory.Success(listIssueSolutionStatus, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-detail")]
        public IActionResult GetIssueStatusDetail(int id)
        {
            try
            {
                var issueSolutionStatus = _issueSolutionStatusRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(issueSolutionStatus, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("save")]
        public async Task<IActionResult> Save(IssueSolutionStatus model)
        {
            try
            {
                if (model.ID <= 0)
                {
                    await _issueSolutionStatusRepo.CreateAsync(model);
                }
                else
                {
                    await _issueSolutionStatusRepo.UpdateAsync(model);
                }

                return Ok(ApiResponseFactory.Success(null, "Success"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
