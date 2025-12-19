using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.HRM
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class PhasedAllocationPersonController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private CurrentUser _currentUser;

        private readonly PhasedAllocationPersonRepo _phasedRepo;
        private readonly PhasedAllocationPersonDetailRepo _phasedDetailRepo;
        private readonly EmployeeRepo _employeeRepo;

        public PhasedAllocationPersonController(IConfiguration configuration, CurrentUser currentUser, PhasedAllocationPersonRepo phasedRepo, PhasedAllocationPersonDetailRepo phasedDetailRepo, EmployeeRepo employeeRepo)
        {
            _configuration = configuration;
            _currentUser = currentUser;
            _phasedRepo = phasedRepo;
            _phasedDetailRepo = phasedDetailRepo;
            _employeeRepo = employeeRepo;
        }

        [HttpGet("")]
        public IActionResult GetAll(int year, int month)
        {
            try
            {
            
                var data = SQLHelper<object>.ProcedureToList("spGetPhasedAllocationPerson", new string[] { "@Year", "@Month" }, new object[] { year, month });
                var phaseds = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(phaseds, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("detail/{phasedId}")]
        public IActionResult GetDetail(int phasedId)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetPhasedAllocationPersonDetail", new string[] { "@PhasedAllocationPersonID" }, new object[] { phasedId });
                var phasedDetails = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(phasedDetails, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] PhasedAllocationPerson phased)
        {
            try
            {
                if (phased.ID <= 0) await _phasedRepo.CreateAsync(phased);
                else await _phasedRepo.UpdateAsync(phased);
                return Ok(ApiResponseFactory.Success(phased, "Cập nhập thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("detail/save-data")]
        public async Task<IActionResult> SaveDataDetail([FromBody] List<PhasedAllocationPersonDetailDTO> details)
        {
            try
            {
                List<string> employeeFails = new List<string>();
                foreach (var item in details)
                {
                    var employee = _employeeRepo.GetAll(x => x.Code == item.EmployeeCode && x.Status != 1).FirstOrDefault() ?? new Employee();
                    if (employee.ID <= 0)
                    {
                        employeeFails.Add($"Mã nhân viên [{item.EmployeeCode}] không tồn tại hoặc đã bị xóa!");
                        continue;
                    }
                    var detai = _phasedDetailRepo.GetAll(x => x.PhasedAllocationPersonID == item.PhasedAllocationPersonID && x.EmployeeID == employee.ID).FirstOrDefault() ?? new PhasedAllocationPersonDetail();
                    if (detai.ID <= 0) await _phasedDetailRepo.CreateAsync(item);
                    else
                    {
                        detai.DateReceive = DateTime.Now;
                        detai.StatusReceive = 1;
                        detai.UpdatedDate = DateTime.Now;

                        await _phasedDetailRepo.UpdateAsync(detai);
                    }

                }

                string message = "Cập nhập thành công!";
                if (employeeFails.Count() >0)
                {
                    message = string.Join("\n", employeeFails);
                }
                return Ok(ApiResponseFactory.Success(details, message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
