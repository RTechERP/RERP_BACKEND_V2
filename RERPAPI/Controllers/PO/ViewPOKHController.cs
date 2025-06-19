using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers.PO
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewPOKHController : ControllerBase
    {
        GroupSaleRepo _groupSaleRepo = new GroupSaleRepo();
        MainIndexRepo _mainIndexRepo = new MainIndexRepo();
        CustomerRepo _customerViewPOKHRepo = new CustomerRepo();
        EmployeeTeamSaleRepo _employeeTeamSaleRepo = new EmployeeTeamSaleRepo();
        EmployeeRepo _employeeRepo = new EmployeeRepo();
        POKHDetailRepo _pokhDetailRepo = new POKHDetailRepo();
        [HttpGet("get-viewpokh")]
        public IActionResult Get(DateTime dateTimeS, DateTime dateTimeE, int employeeTeamSaleID, int userID, int poType,int status, int customerID, string keyword = "")
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToDynamicLists("spGetViewPOKHDetail", 
                         new string[] { "@DateStart", "@DateEnd", "@EmployeeTeamSaleID", "@UserID", "@POType", "@Status", "@CustomerID", "@Keyword" }, 
                         new object[] { dateTimeS , dateTimeE , employeeTeamSaleID, userID, poType, status, customerID, keyword });

                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<dynamic>.GetListData(list, 0)
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpGet("get-user")]
        public IActionResult LoadUser()
        {
            try
            {
                List<Employee> list = _employeeRepo.GetAll().Where(x => x.UserID != 0).ToList();
                return Ok(new
                {
                    status = 1,
                    data = list
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpGet("get-customer")]
        public IActionResult LoadCustomer()
        {
            try
            {
                var list = _customerViewPOKHRepo.GetAll()
                           .Where(x => x.IsDeleted != true).OrderByDescending(x => x.CreatedDate).ToList();
                return Ok(new
                {
                    status = 1,
                    data = list
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpGet("get-groupsale")]
        public IActionResult LoadGroupSale()
        {
            try
            {
                var list = _groupSaleRepo.GetAll().Select(x => new { x.ID, x.GroupSalesName }).ToList();
                return Ok(new
                {
                    status = 1,
                    data = list
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpGet("get-employee-team-sale")]
        public IActionResult LoadEmployeeTeamSale()
        {
            try
            {
                var list = _employeeTeamSaleRepo.GetAll().Where(x=>x.ParentID == 0).ToList();
                return Ok(new
                {
                    status = 1,
                    data = list
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpGet("get-mainindex")]
        public IActionResult LoadMainIndex()
        {
            try
            {
                List<int> ids = new List<int> { 8, 9, 10, 11, 12, 13 };
                var list = _mainIndexRepo.GetAll().Where(x => ids.Contains(x.ID)).ToList();
                return Ok(new
                {
                    status = 1,
                    data = list
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpPost("save-data")]
        public IActionResult SavePOKHDetail([FromBody] List<POKHDetail> model)
        {
            try
            {
                foreach (var item in model)
                {
                    _pokhDetailRepo.UpdateFieldsByID(item.ID, item);
                }
                return Ok(new
                {
                    status = 1,
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
    }
}
