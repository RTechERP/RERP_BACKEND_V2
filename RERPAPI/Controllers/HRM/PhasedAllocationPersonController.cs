using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
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
        public IActionResult GetAll(int year, int month, int typeAllocation = 0, int statusAllocation = 0, string keyword = "")
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetPhasedAllocationPerson",
                    new string[] { "@Year", "@Month", "@TypeAllocation", "@StatusAllocation", "@Keyword" },
                    new object[] { year, month, typeAllocation, statusAllocation, keyword });
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
                if (phased.IsDeleted != true)
                {
                    var isExistCode = _phasedRepo.GetAll(x => x.Code.Trim().ToUpper() == phased.Code.Trim().ToUpper() && x.IsDeleted != true && x.ID != phased.ID).Any();
                    if (isExistCode)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, $"Mã cấp phát [{phased.Code}] đã tồn tại!"));
                    }
                }

                if (phased.ID <= 0)
                {

                    await _phasedRepo.CreateAsync(phased);
                }
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
                int phasedAllocationPersonID = 0;
                int sl = 0;
                foreach (var item in details)
                {
                    if (!string.IsNullOrWhiteSpace(item.EmployeeCode))
                    {
                        var employee = _employeeRepo.GetAll(x => x.Code == item.EmployeeCode && x.Status != 1).FirstOrDefault() ?? new Employee();
                        if (employee.ID <= 0)
                        {
                            employeeFails.Add($"Mã nhân viên [{item.EmployeeCode}] không tồn tại hoặc đã bị xóa!");
                            continue;
                        }
                        var detail = _phasedDetailRepo.GetAll(x => x.PhasedAllocationPersonID == item.PhasedAllocationPersonID && x.EmployeeID == employee.ID).FirstOrDefault() ?? new PhasedAllocationPersonDetail();
                        if (detail.ID <= 0)
                        {
                          var result =   await _phasedDetailRepo.CreateAsync(item);
                            if (result > 0)
                            {
                                sl++;
                            }
                        }

                        else
                        {
                            detail.DateReceive = DateTime.Now;
                            detail.UpdatedDate = DateTime.Now;
                            detail.StatusReceive = 1;
                            var result = await _phasedDetailRepo.UpdateAsync(detail);
                            if (result > 0)
                            {
                                sl++;
                            }
                        }
                    }
                    else
                    {
                        if (item.ID <= 0) continue;

                        var detail = _phasedDetailRepo.GetByID(item.ID);
                        if (detail == null) continue;

                        if (item.StatusReceive == 1 && detail.StatusReceive != 1)
                        {
                            detail.DateReceive = DateTime.Now;
                        }
                        detail.IsDeleted = item.IsDeleted;
                        detail.StatusReceive = item.StatusReceive;
                        detail.UpdatedDate = DateTime.Now;

                        var result = await _phasedDetailRepo.UpdateAsync(detail);
                        if (result > 0)
                        {
                            sl++;
                        }
                    }
                    phasedAllocationPersonID = (int)item.PhasedAllocationPersonID;
                }

                if (phasedAllocationPersonID != 0)
                {
                    List<PhasedAllocationPersonDetail> lstDetail = _phasedDetailRepo.GetAll(p => p.PhasedAllocationPersonID == phasedAllocationPersonID && !p.IsDeleted.Value);
                    PhasedAllocationPerson allocationPerson = _phasedRepo.GetByID(phasedAllocationPersonID);
                    if (lstDetail.Count(p => p.StatusReceive == 1) == lstDetail.Count)
                    {
                        allocationPerson.StatusAllocation = 1;
                        var result = await _phasedRepo.UpdateAsync(allocationPerson);
                        if (result > 0)
                        {
                            sl++;
                        }
                    }
                }

                //Kiểm tra all update lại trạng thái phiếu
                string message = "Cập nhập thành công!";
                if (employeeFails.Count() > 0)
                {
                    message = string.Join("\n", employeeFails);
                }
                if (sl > 0)
                {
                    return Ok(ApiResponseFactory.Success(details, message));
                }
                else
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Lưu thất bại"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
