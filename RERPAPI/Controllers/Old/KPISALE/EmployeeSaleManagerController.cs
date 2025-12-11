using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.KPISALE
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeSaleManagerController : ControllerBase
    {

        private readonly EmployeeTeamSaleRepo _employeeTeamSaleRepo;
        private readonly EmployeeTeamSaleLinkRepo _employeeTeamSaleLinkRepo;
        public EmployeeSaleManagerController(EmployeeTeamSaleRepo employeeTeamSaleRepo, EmployeeTeamSaleLinkRepo employeeTeamSaleLinkRepo)
        {
            _employeeTeamSaleRepo = employeeTeamSaleRepo;
            _employeeTeamSaleLinkRepo = employeeTeamSaleLinkRepo;
        }
        [HttpGet("get-groupsale")]
        public IActionResult loadGroupSale()
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetEmployeeTeamSale",null,null);

                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-employeesale")]
        public IActionResult loadEmployeeSale(int selectedId)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetEmployeebyTeamSale", new string[] { "@EmployeeTeamSaleID" }, new object[] { selectedId });

                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("load-group-stt")]
        public IActionResult loadGroupStt()
        {
            try
            {
                List<EmployeeTeamSale> data = _employeeTeamSaleRepo.GetAll(x => x.IsDeleted != 1).ToList();
                int stt = data.Max(s => s.STT) ?? 0;
                return Ok(ApiResponseFactory.Success(stt, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-employee-team-sale")]
        public IActionResult saveEmployeeTeamSale( EmployeeTeamSale model )
        {
            try
            {
                var existsTeam = _employeeTeamSaleRepo.GetAll(x => x.Code == model.Code && x.ID != model.ID && x.IsDeleted == 0).FirstOrDefault();
                if(existsTeam != null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null,"Mã Team đã tồn tại!"));
                }   

                if(model.ID > 0)
                {
                    _employeeTeamSaleRepo.Update(model);
                }
                else
                {
                    _employeeTeamSaleRepo.Create(model);
                }
                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-employee-detail")]
        public IActionResult getEmployeeDetail()
        {
            try
            {
                var list = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployeeByKPIPositionDepartmentID", new string[] { "@DepartmentID" }, new object[] { 0 });

                //var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-employee-detail")]
        public IActionResult saveEmployeeDetail(SaveEmployeeDetailDTO request)
        {
            try
            {
                List<int> itemToRemove = new List<int>();
                foreach(int item in request.EmployeeIds)
                {
                    var existingRecords = _employeeTeamSaleLinkRepo.GetAll(x => x.EmployeeID == item && x.EmployeeTeamSaleID == request.EmployeeTeamSaleId);
                    if (existingRecords.Count > 0)
                    {
                        itemToRemove.Add(item);
                    } 
                }    
                foreach(int id in itemToRemove)
                {
                    request.EmployeeIds.Remove(id);
                }    
                foreach(int item in request.EmployeeIds)
                {
                    EmployeeTeamSaleLink newModel = new EmployeeTeamSaleLink()
                    {
                        EmployeeID = item,
                        EmployeeTeamSaleID = request.EmployeeTeamSaleId
                    };
                    _employeeTeamSaleLinkRepo.Create(newModel);
                }    
                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        public class SaveEmployeeDetailDTO
        {
            public List<int> EmployeeIds { get; set; }
            public int EmployeeTeamSaleId { get; set; }
        }

    }
}
