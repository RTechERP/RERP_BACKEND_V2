using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewPOKHController : ControllerBase
    {
        GroupSaleRepo _groupSaleRepo = new GroupSaleRepo();
        [HttpGet("LoadViewPOKH")]
        public IActionResult Get(DateTime dateTimeS, DateTime dateTimeE, int employeeTeamSaleID, int userID, int poType,int status, int customerID, string keyword)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToDynamicLists("spGetViewPOKHDetail", 
                         new string[] { "@DateStart", "@DateEnd", "@EmployeeTeamSaleID", "@UserID", "@POType", "@Status", "@CustomerID", "@Keyword" }, 
                         new object[] { dateTimeS, dateTimeE, employeeTeamSaleID, userID, poType, status, customerID, keyword });

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
        [HttpGet("LoadUser")]
        public IActionResult LoadUser()
        {
            try
            {
                var list = SQLHelper<Employee>.FindByExpression(new Expression("UserID", 0, "<>"));
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
        [HttpGet("LoadCustomer")]
        public IActionResult LoadCustomer()
        {
            try
            {
                var list = SQLHelper<Customer>.FindAll()
                           .Where(x => x.ID != 1).OrderByDescending(x => x.CreatedDate).Select(x => new { x.ID, x.CustomerName }).ToList();
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
        [HttpGet("LoadGroupSale")]
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
        [HttpGet("LoadEmployeeTeamSale")]
        public IActionResult LoadEmployeeTeamSale()
        {
            try
            {
                var list = SQLHelper<EmployeeTeamSale>.FindAll().Where(x=>x.ParentID == 0).ToList();
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
        [HttpGet("LoadMainIndex")]
        public IActionResult LoadTeamIndex()
        {
            try
            {
                List<int> ids = new List<int> { 8, 9, 10, 11, 12, 13 };
                var list = SQLHelper<MainIndex>.FindAll().Where(x => ids.Contains(x.ID)).ToList();
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
    }
}
