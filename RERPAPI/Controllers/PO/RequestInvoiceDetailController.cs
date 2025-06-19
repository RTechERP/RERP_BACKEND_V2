using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Repo.GenericEntity;
using System.Net.WebSockets;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers.PO
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestInvoiceDetailController : ControllerBase
    {
        EmployeeRepo _employeeRepo = new EmployeeRepo();
        ProductSaleRepo _productSaleRepo = new ProductSaleRepo();
        ProjectRepo _projectRepo = new ProjectRepo();

        [HttpGet("get-employee")]
        public IActionResult GetEmployee()
        {
            try
            {
                var data = _employeeRepo.GetAll().Where(x => x.Status == 0 && x.FullName != "").ToList();
                return Ok(new
                {
                    status = 1,
                    data,
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
        [HttpGet("get-productsale")]
        public IActionResult GetProductSale()
        {
            try
            {
                var data = _productSaleRepo.GetAll().ToList();
                return Ok(new
                {
                    status = 1,
                    data,
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
        [HttpGet("get-project")]
        public IActionResult GetProject()
        {
            try
            {
                var data = _projectRepo.GetAll().OrderByDescending(x=>x.CreatedDate).ToList();
                return Ok(new
                {
                    status = 1,
                    data,
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
        //[HttpPost("save-data")]
        //public IActionResult Save()
        //{
        //    try
        //    {
        //        var data = _projectRepo.GetAll().ToList();
        //        return Ok(new
        //        {
        //            status = 1,
        //            data = data,
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new
        //        {
        //            status = 0,
        //            message = ex.Message,
        //            error = ex.ToString()
        //        });
        //    }
        //}
    }
}
