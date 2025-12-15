using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.Handover;
using RERPAPI.Model.Param.HRM.DepartmentRequired;
using RERPAPI.Repo.GenericEntity.BBNV;
using RERPAPI.Repo.GenericEntity.GeneralCatetogy.JobRequirements;
using RERPAPI.Repo.GenericEntity.HRM.DepartmentRequire;

namespace RERPAPI.Controllers.HRM
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecommendSupplierController : ControllerBase
    {
        JobRequirementRepo _jobRepo;

        DepartmentRequiredRepo _departmentrequired;

        //DepartmentRequiredApprovalsRepo _departmentrequiredapprovals;

        HCNSProposalsRepo _hcnsproposals;

        private IConfiguration _configuration;

        public RecommendSupplierController(JobRequirementRepo jobRepo, DepartmentRequiredRepo departmentrequired, HCNSProposalsRepo hcnsproposals, IConfiguration configuration)
        {
            _jobRepo = jobRepo;
            _departmentrequired = departmentrequired;
            //_departmentrequiredapprovals = departmentrequiredapprovals;
            _hcnsproposals = hcnsproposals;
            _configuration = configuration;
        }

        [HttpPost("get-department-required-data")]
        public IActionResult GetHandoverData([FromBody] DepartmentRequiredRequestParam request)
        {
            try
            {
                var departmentRequired = SQLHelper<dynamic>.ProcedureToList("spGetDepartmentRequired",
                new string[] { "@JobRequirementID", "@EmployeeID", "@DepartmentID", "@Keyword", "@DateStart", "@DateEnd" },
                new object[] { request.JobRequirementID, request.EmployeeID, request.DepartmentID, request.Keyword, request.DateStart, request.DateEnd }
                );

                var HCNSProPosal = SQLHelper<dynamic>.ProcedureToList("spGetHCNSProposal",
                    new string[] { "@JobRequirementID", "@DepartmentRequiredID", "@DateStart", "@DateEnd" },
                    new object[] { request.JobRequirementID, request.DepartmentRequiredID ,request.DateStart, request.DateEnd }
                );

                var departmentRequiredData = SQLHelper<dynamic>.GetListData(departmentRequired, 0);
                var HCNSProPosalData = SQLHelper<dynamic>.GetListData(HCNSProPosal, 0);

                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        departmentRequiredData,
                        HCNSProPosalData,
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data-department-required")]
        public async Task<IActionResult> SaveData([FromBody] RecommendSupplierDTO dto)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                //if (dto == null || dto.JobRequirementID == null)
                //{
                //    return BadRequest(new { status = 0, message = "Dữ liệu không hợp lệ" });
                //}

                int JobRequirementID = dto.JobRequirementID;
                int DepartmentRequiredID = 0;
     

                // Phòng ban yêu cầu
                if (dto.DepartmentRequired != null)
                {
                    var maxSttReceiver = _departmentrequired.GetAll()
                        .Where(x => x.JobRequirementID == JobRequirementID)
                        .Select(x => x.STT ?? 0)
                        .DefaultIfEmpty(0)
                        .Max();
                    int sttEmployeeCounter = maxSttReceiver;

                    foreach (var employeeRequired in dto.DepartmentRequired)
                    {

                        employeeRequired.JobRequirementID = JobRequirementID;

                        var existing = _departmentrequired.GetAll()
                   .FirstOrDefault(x => x.JobRequirementID == JobRequirementID && x.ID == employeeRequired.ID);

                        if (existing == null || employeeRequired.ID <= 0)
                        {
                            sttEmployeeCounter++;
                            employeeRequired.STT = sttEmployeeCounter;
                            await _departmentrequired.CreateAsync(employeeRequired);
                            DepartmentRequiredID = employeeRequired.ID;

                        }

                        else
                            _departmentrequired.Update(employeeRequired);
                        DepartmentRequiredID = employeeRequired.ID;


                    }
                }

                // HCNS đề xuất
                if (dto.HCNSProposal != null && dto.HCNSProposal.Any())
                {
                    var maxSttProposal = _hcnsproposals.GetAll()
                        .Where(x => x.JobRequirementID == JobRequirementID)
                        .Select(x => x.STT ?? 0)
                        .DefaultIfEmpty(0)
                        .Max();
                    int sttHCNSProposal = maxSttProposal;
                    foreach (var itemProposal in dto.HCNSProposal)
                    {

                        itemProposal.JobRequirementID = JobRequirementID;
                       
                        var existing = _hcnsproposals.GetAll()
                .FirstOrDefault(x => x.JobRequirementID == JobRequirementID && x.ID == itemProposal.ID);

                        if (existing == null || itemProposal.ID <= 0)
                        {
                            sttHCNSProposal++;
                            itemProposal.STT = sttHCNSProposal;
                            itemProposal.CreatedDate = DateTime.Now;
                            itemProposal.DepartmentRequiredID = DepartmentRequiredID;
                            await _hcnsproposals.CreateAsync(itemProposal);
                        }

                        else
                            itemProposal.DepartmentRequiredID = existing.DepartmentRequiredID;
                        _hcnsproposals.Update(itemProposal);
     
                    }
                }
                if (dto.DeletedCommend.Count > 0)
                {
                    foreach (var item in dto.DeletedCommend)
                    {
                        HCNSProposal model = _hcnsproposals.GetByID(item);
                        model.IsDeleted = true;
                        await _hcnsproposals.UpdateAsync(model);
                    }
                }
                return Ok(new
                {
                    status = 1,
                    message = "Lưu thành công",
                    id = JobRequirementID,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}
