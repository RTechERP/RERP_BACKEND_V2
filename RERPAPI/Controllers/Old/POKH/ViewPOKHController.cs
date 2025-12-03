using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers.Old.POKH
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ViewPOKHController : ControllerBase
    {
        private readonly GroupSaleRepo _groupSaleRepo;
        private readonly MainIndexRepo _mainIndexRepo;
        private readonly CustomerRepo _customerViewPOKHRepo;
        private readonly EmployeeTeamSaleRepo _employeeTeamSaleRepo;
        private readonly EmployeeRepo _employeeRepo;
        private readonly POKHDetailRepo _pokhDetailRepo;
        private readonly RequestInvoiceDetailRepo _requestInvoiceDetailRepo;
        public ViewPOKHController(
            GroupSaleRepo groupSaleRepo,
            MainIndexRepo mainIndexRepo,
            CustomerRepo customerViewPOKHRepo,
            EmployeeTeamSaleRepo employeeTeamSaleRepo,
            EmployeeRepo employeeRepo,
            POKHDetailRepo pokhDetailRepo,
            RequestInvoiceDetailRepo requestInvoiceDetailRepo
            )
        {
            _groupSaleRepo = groupSaleRepo;
            _mainIndexRepo = mainIndexRepo;
            _customerViewPOKHRepo = customerViewPOKHRepo;
            _employeeTeamSaleRepo = employeeTeamSaleRepo;
            _employeeRepo = employeeRepo;
            _pokhDetailRepo = pokhDetailRepo;
            _requestInvoiceDetailRepo = requestInvoiceDetailRepo;
        }
        [HttpGet("get-viewpokh")]
        public IActionResult Get(DateTime dateTimeS, DateTime dateTimeE, int employeeTeamSaleID, int userID, int poType, int status, int customerID,int warehouseId, string keyword = "")
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetViewPOKHDetail", 
                         new string[] { "@DateStart", "@DateEnd", "@EmployeeTeamSaleID", "@UserID", "@POType", "@Status", "@CustomerID", "@Keyword", "@WarehouseID" }, 
                         new object[] { dateTimeS , dateTimeE , employeeTeamSaleID, userID, poType, status, customerID, keyword, warehouseId });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                var dataExport = SQLHelper<dynamic>.GetListData(list, 1);
                var dataInvoice = SQLHelper<dynamic>.GetListData(list, 2);

                return Ok(ApiResponseFactory.Success(new { data, dataExport, dataInvoice}, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-user")]
        public IActionResult LoadUser()
        {
            try
            {
                var list = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployee",
                                 new string[] { "@Keyword", "@Status" },
                                 new object[] { "", 0 });
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-employee-by-teamsale")]
        public IActionResult LoadEmployeeByTeamSale(int teamId)
        {
            try
            {
                var list = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployeeByTeamSale",
                                 new string[] { "@EmployeeTeamSaleID" },
                                 new object[] { teamId });
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-customer")]
        public IActionResult LoadCustomer()
        {
            try
            {
                var list = _customerViewPOKHRepo.GetAll()
                           .Where(x => x.IsDeleted != true).OrderByDescending(x => x.CreatedDate).ToList();
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-groupsale")]
        public IActionResult LoadGroupSale()
        {
            try
            {
                var list = _groupSaleRepo.GetAll().Select(x => new { x.ID, x.GroupSalesName }).ToList();
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-employee-team-sale")]
        public IActionResult LoadEmployeeTeamSale()
        {
            try
            {
                var list = _employeeTeamSaleRepo.GetAll().Where(x=>x.ParentID == 0).ToList();
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-mainindex")]
        public IActionResult LoadMainIndex()
        {
            try
            {
                List<int> ids = new List<int> { 8, 9, 10, 11, 12, 13 };
                var list = _mainIndexRepo.GetAll().Where(x => ids.Contains(x.ID)).ToList();
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data")]
        public IActionResult SavePOKHDetail(SaveViewPOKHDTO dto)
        {
            try
            {
                foreach (var item in dto.pokhDetails)
                {
                    _pokhDetailRepo.Update(item);
                }

                if (dto.requestInvoiceDetails != null)
                {
                    foreach (var inv in dto.requestInvoiceDetails)
                    {
                        _requestInvoiceDetailRepo.Update(inv);
                    }
                }
                return Ok(ApiResponseFactory.Success(null, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
