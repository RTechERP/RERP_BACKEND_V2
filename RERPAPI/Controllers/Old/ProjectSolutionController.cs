using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyAuthorize]
    public class ProjectSolutionController : ControllerBase
    {
        private ProjectSolutionRepo _projectSolutionRepo;
        public ProjectSolutionController(ProjectSolutionRepo projectSolutionRepo)
        {
            _projectSolutionRepo = projectSolutionRepo;
        }   

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(int projectID)
        {
            try
            {
                var dtProjectSolutions = SQLHelper<object>.ProcedureToList("spGetProjectSolution",
                    new string[] { "@ProjectID" },
                    new object[] { projectID });
                var projectSolutions = dtProjectSolutions?.FirstOrDefault();
                return Ok(ApiResponseFactory.Success(projectSolutions,""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex,ex.Message));
            }
        }

        //[HttpPost("save-data")]
        //public async Task<IActionResult> SaveData([FromBody] ProjectSolution request)
        //{
        //    try
        //    {
        //        if(!_projectSolutionRepo.Validate(request, out string message))
        //        {
        //                return BadRequest(ApiResponseFactory.Fail(null, message));
        //        }
        //        if (request.ID > 0)
        //        {
        //            await _projectSolutionRepo.UpdateAsync(request);
        //            return Ok(ApiResponseFactory.Success(
        //                request,
        //                ""
        //            ));
        //        }
        //        else
        //        {
        //            await _projectSolutionRepo.CreateAsync(request);
        //            return Ok(ApiResponseFactory.Success(request
        //                ,
        //                ""
        //            ));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, "Lỗi khi lưu giải pháp"));
        //    }
        //}
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] ProjectSolutionDTO request)
        {
            try
            {
                string message = "";


                // XỬ LÝ DUYỆT (NẾU CÓ THÔNG TIN DUYỆT)
                if (request.ApproveStatus.HasValue && request.IsApproveAction.HasValue)
                {
                    if (!_projectSolutionRepo.ValidateApprove(request.ID, request.IsApproveAction.Value, request.ApproveStatus.Value, out message))
                        return BadRequest(ApiResponseFactory.Fail(null, message));

                    var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                    var currentUser = ObjectMapper.GetCurrentUser(claims);
                    var solution = await _projectSolutionRepo.GetByIDAsync(request.ID);

                    //if (solution == null) return NotFound();

                    if (request.ApproveStatus == 1)
                    {
                        solution.IsApprovedPrice = request.IsApproveAction;
                        solution.EmployeeApprovedPriceID = currentUser.EmployeeID;
                    }
                    else if (request.ApproveStatus == 2)
                    {
                        solution.IsApprovedPO = request.IsApproveAction;
                        solution.EmployeeApprovedPOID = currentUser.EmployeeID;
                    }

                    await _projectSolutionRepo.UpdateAsync(solution);
                    return Ok(ApiResponseFactory.Success(solution, "Duyệt thành công"));
                }
                if (!_projectSolutionRepo.Validate(request, out message))
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                // XỬ LÝ LƯU DỮ LIỆU THÔNG THƯỜNG
                if (request.ID > 0)
                {
                    await _projectSolutionRepo.UpdateAsync(request);
                    return Ok(ApiResponseFactory.Success(request, "Cập nhật thành công"));
                }
                else
                {
                    await _projectSolutionRepo.CreateAsync(request);
                    return Ok(ApiResponseFactory.Success(request, "Tạo mới thành công"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Lỗi khi xử lý dữ liệu"));
            }
        }
        [HttpGet("get-solution-code")]
        public async Task<IActionResult> GetSolutionCode(int projectRequestId)
        {
            try
            {
                string code = _projectSolutionRepo.GetSolutionCode(projectRequestId);
                return Ok(ApiResponseFactory.Success(code, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Lỗi khi lấy mã giải pháp"));
            }
        }
        //[HttpPost("approve")]
        //public async Task<IActionResult> ApproveSolution(int id, bool isApproved, int status)
        //{

        //    if (!_projectSolutionRepo.ValidateApprove(id, isApproved, status, out string message))
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(null, message));
        //    }
        //    var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
        //    var currentUser = ObjectMapper.GetCurrentUser(claims);
        //    //if (currentUser == null)
        //    //{
        //    //    return Unauthorized(ApiResponseFactory.Fail(null, "Bạn không có quyền thực hiện thao tác này"));
        //    //}
        //    var solution = await _projectSolutionRepo.GetByIDAsync(id);
        //    if (solution == null) return NotFound();

        //    if (status == 1)
        //    {
        //        solution.IsApprovedPrice = isApproved;
        //        solution.EmployeeApprovedPriceID = currentUser.EmployeeID;
        //    }
        //    else if (status == 2)
        //    {
        //        solution.IsApprovedPO = isApproved;
        //        solution.EmployeeApprovedPOID = currentUser.EmployeeID;
        //    }

        //   await _projectSolutionRepo.UpdateAsync(solution);
        //    return Ok(ApiResponseFactory.Success(solution, "Duyệt thành công"));
        //}
    }
}