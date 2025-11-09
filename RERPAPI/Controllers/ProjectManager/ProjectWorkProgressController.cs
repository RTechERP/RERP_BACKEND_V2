using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.ProjectManager
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectWorkPropressController : ControllerBase
    {
        #region Khai báo biến
        ProjectRepo projectRepo;
        CustomerRepo customerRepo;
        public ProjectWorkPropressController(ProjectRepo projectRepo, CustomerRepo customerRepo)
        {
            this.projectRepo = projectRepo;
            this.customerRepo = customerRepo;
        }
        #endregion

        #region Lấy danh sách tiến độ công việc
        [HttpGet("get-work-propress/{projectId}")]
        public async Task<IActionResult> getWorkPropress(int projectId)
        {
            try
            {
                string customerCode = "";
                var dt = SQLHelper<object>.ProcedureToList("[spGetTienDoCongViec]", new string[] { "@ProjectID" }, new object[] { projectId });
                if(projectId > 0)
                {
                    int customerId = projectRepo.GetByID(projectId).CustomerID;
                    if(customerId > 0)
                    {
                        customerCode = customerRepo.GetByID(customerId).CustomerCode;
                    }
                }
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(dt, 0),
                    data1 = SQLHelper<object>.GetListData(dt, 1),
                    data2 = SQLHelper<object>.GetListData(dt, 2),
                    data3 = customerCode
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        #endregion

    }
}
