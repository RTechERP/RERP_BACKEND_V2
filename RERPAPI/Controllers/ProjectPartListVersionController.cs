using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Linq.Expressions;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectPartListVersionController : ControllerBase
    {
        private ProjectPartlistVersionRepo _projectPartlistVersionRepo = new ProjectPartlistVersionRepo();
        ProjectPartListRepo _projectPartListRepo = new ProjectPartListRepo();
        [HttpGet("get-all")]
        public IActionResult GetAll(int projectSolutionId, bool isPO)
        {
            try
            {
                int statusVersion = 1;
                if (isPO)
                {
                    statusVersion = 2;
                }
                var projectPartListVersions = SQLHelper<dynamic>.ProcedureToList(
                    "spGetProjectPartListVersion",
                    new string[] { "@ProjectSolutionID", "@StatusVersion" },
                    new object[] { projectSolutionId, statusVersion });
                return Ok(ApiResponseFactory.Success(
                    SQLHelper<object>.GetListData(projectPartListVersions, 0),
                    ""
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex,ex.Message));
            }
        }
        //[HttpPost("save-data")]
        //public async Task<IActionResult> SaveData([FromBody] ProjectPartListVersion request)
        //{
        //    try
        //    {
        //        string message = "";
        //        if (!_projectPartlistVersionRepo.Validate(request, out message))
        //        {
        //            return BadRequest(ApiResponseFactory.Fail(null, message));
        //        }
        //        if (request.ID > 0)
        //        {
        //            await _projectPartlistVersionRepo.UpdateAsync(request);
        //        }
        //        else
        //        {
        //            await _projectPartlistVersionRepo.CreateAsync(request);
        //        }
        //        if (request.IsActive == false)
        //        {
        //            var myDict = new Dictionary<Expression<Func<ProjectPartList, object>>, object>
        //                    {
        //                        { x => x.IsApprovedTBP, 0 },
        //                        { x => x.IsApprovedPurchase, 0 }
        //                    };
        //          await _projectPartListRepo.UpdateFieldByAttributeAsync(x => x.ProjectPartListVersionID == request.ID, myDict);
        //        }
        //        if(request.IsDeleted==true)
        //        {
        //            var myDict = new Dictionary<Expression<Func<ProjectPartList, object>>, object>
        //                    {
        //                        { x => x.IsDeleted, 1 },
        //                        { x => x.ReasonDeleted, request.ReasonDeleted}
        //                    };
        //            await _projectPartListRepo.UpdateFieldByAttributeAsync(x => x.ProjectPartListVersionID == request.ID, myDict);
        //        }
        //        return Ok(ApiResponseFactory.Success(request,""));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, "Lỗi khi lưu phiên bản danh sách phần"));
        //    }
        //}
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] ProjectPartListVersionDTO requestDTO)
        {
            try
            {
                string message = "";

                var request = (ProjectPartListVersion)requestDTO;
                if (requestDTO.IsApproveAction.HasValue)
                {
                    if (!_projectPartlistVersionRepo.ValidateApprove(request, requestDTO.IsApproveAction.Value, out message))
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, message));
                    }

                    var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                    var currentUser = ObjectMapper.GetCurrentUser(claims);

                    request.IsApproved = requestDTO.IsApproveAction;
                    request.ApprovedID = currentUser?.EmployeeID;
                }

                // Regular validation
                if (!_projectPartlistVersionRepo.Validate(request, out message))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }

                if (request.ID > 0)
                {
                    await _projectPartlistVersionRepo.UpdateAsync(request);
                }
                else
                {
                    await _projectPartlistVersionRepo.CreateAsync(request);
                }

                if (request.IsActive == false)
                {
                    var myDict = new Dictionary<Expression<Func<ProjectPartList, object>>, object>
                        {
                            { x => x.IsApprovedTBP, 0 },
                            { x => x.IsApprovedPurchase, 0 }
                        };
                    await _projectPartListRepo.UpdateFieldByAttributeAsync(x => x.ProjectPartListVersionID == request.ID, myDict);
                }

                if (request.IsDeleted == true)
                {
                    var myDict = new Dictionary<Expression<Func<ProjectPartList, object>>, object>
                        {
                            { x => x.IsDeleted, 1 },
                            { x => x.ReasonDeleted, request.ReasonDeleted}
                        };
                    await _projectPartListRepo.UpdateFieldByAttributeAsync(x => x.ProjectPartListVersionID == request.ID, myDict);
                }

                return Ok(ApiResponseFactory.Success(request, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Lỗi khi lưu phiên bản "));
            }
        }
    }
}