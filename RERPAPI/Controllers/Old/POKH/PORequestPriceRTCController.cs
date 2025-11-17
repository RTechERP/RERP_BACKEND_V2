using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.POKH
{
    [Route("api/[controller]")]
    [ApiController]
    public class PORequestPriceRTCController : ControllerBase
    {
        private readonly ProjectPartlistPriceRequestRepo _projectPartlistPriceRequests;
        public PORequestPriceRTCController(

            ProjectPartlistPriceRequestRepo projectPartlistPriceRequests
            )
        {
            _projectPartlistPriceRequests = projectPartlistPriceRequests;
        }

        [HttpGet("get-details")]
        public IActionResult loadDetailUser(int id)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetPOKHDetail", new string[] { "@ID", "@IDDetail" }, new object[] { id, 0 });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> Save(List<ProjectPartlistPriceRequest> models)
        {
            try
            {
                if (models == null || models.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách yêu cầu báo giá trống"));
                foreach (var i in models)
                {
                    var oldRequests = _projectPartlistPriceRequests.GetAll(x => x.POKHDetailID == i.POKHDetailID && x.IsDeleted == false).ToList();
                    if (oldRequests.Any())
                    {
                        foreach (var req in oldRequests)
                        {
                            req.IsDeleted = true;
                            req.UpdatedDate = DateTime.Now;
                            await _projectPartlistPriceRequests.UpdateAsync(req);
                        }
                    }
                    var newRequest = new ProjectPartlistPriceRequest
                    {
                        DateRequest = i.DateRequest,
                        EmployeeID = i.EmployeeID,
                        Deadline = i.Deadline,
                        ProductCode = i.ProductCode,
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        StatusRequest = 1,
                        IsCommercialProduct = true,
                        POKHDetailID = i.POKHDetailID
                    };
                    await _projectPartlistPriceRequests.CreateAsync(newRequest);
                }
                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
