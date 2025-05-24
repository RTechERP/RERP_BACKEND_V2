using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Data;
using System.Dynamic;
using System.Linq.Expressions;
using System.Text.Json;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectPartlistPriceRequestController : ControllerBase
    {
        ProjectRepo projectRepo = new ProjectRepo();
        POKHRepo pOKHRepo = new POKHRepo();
        ProjectPartlistPriceRequestRepo requestRepo = new ProjectPartlistPriceRequestRepo();
        ProductSaleRepo productSaleRepo = new ProductSaleRepo();
        CurrencyRepo currencyRepo = new CurrencyRepo();
        SupplierSaleRepo supplierSaleRepo = new SupplierSaleRepo();

        [HttpGet("getallProjectParListPriceRequest")]
        public async Task<IActionResult> GetAll(DateTime dateStart, DateTime dateEnd, int statusRequest, int projectId, string? keyword, int isDeleted,int projectTypeID, int poKHID, int isCommercialProduct = -1)
        {
            if (projectTypeID < 0) isCommercialProduct = 1;
            else poKHID = 0;
            List<List<dynamic>> dtPriceRequest = SQLHelper<dynamic>.ProcedureToDynamicLists("spGetProjectPartlistPriceRequest_New",
                    new string[] { "@DateStart", "@DateEnd", "@StatusRequest", "@ProjectID", "@Keyword", "@IsDeleted", "@ProjectTypeID", "@IsCommercialProduct", "@POKHID" },
                    new object[] { dateStart, dateEnd, statusRequest, projectId, keyword, isDeleted, projectTypeID, isCommercialProduct, poKHID });
            
            return Ok(new { status = 1, data = new {
                                                    dtData = dtPriceRequest[0]
            } });
        }
        [HttpGet("getType")]
        public async Task<IActionResult> GetAllTypebyEmployeeID(int employeeID)
        {

            List<List<dynamic>> dtType = SQLHelper<dynamic>.ProcedureToDynamicLists("spGetProjectTypeAssignByEmployeeID",
                                                        new string[] { "@EmployeeID" },
                                                        new object[] { employeeID });
            return Ok(new
            {
                status = 0,
                data = new
                {
                    dtType = dtType[0]
                }
            });
        }
        [HttpGet("getAllProjects")]
        public async Task<IActionResult> GetAllProjects()
        {
            List<Project> lstProjects = projectRepo.GetAll();
            return Ok(new { status = 1, data = lstProjects });
        }
        [HttpGet("getAllEmployee")]
        public async Task<IActionResult> GetAllEmployee()
        {
            var dtEmployee = SQLHelper<dynamic>.ProcedureToDynamicLists("spGetEmployee", new string[] { "@Status" }, new object[] { 0 });
            return Ok(new { status = 1, data = new { dtEmployee = dtEmployee[0] } });
        }
        [HttpGet("getPoCode")]
        public async Task<IActionResult> GetPoCode()
        {
            var dtPOKH = pOKHRepo.GetAll();
            return Ok(new { status = 1, data = dtPOKH });
        }
        [HttpGet("getProductSale")]
        public async Task<IActionResult> GetProductSale(int page = 1, int pageSize = 20)
        {
            var query = productSaleRepo.GetAll().AsQueryable();

            var totalRecords = query.Count();

            var pagedData = query
                .OrderBy(p=>p.ID)   
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new
            {
                status = 1,
                data = pagedData,
                totalRecords = totalRecords,
                page = page,
              pageSize = pageSize
            });
        }
        [HttpGet("getCurrency")]
        public async Task<IActionResult> GetCurrency()
        {
            List<Currency> currencies = currencyRepo.GetAll();
            return Ok(new { status = 1, data = currencies });
        }
        [HttpPost("saveData")]
        public async Task<IActionResult> SaveData([FromBody] ProjectPartlistPriceRequestDTO dto)
        {
            try
            {
                if (dto.lstModel != null && dto.lstModel.Any())
                {
                    foreach (var item in dto.lstModel)
                    {
                        if (item.ID > 0)
                        {
                            var existing = requestRepo.GetByID(item.ID);
                            if (existing != null)
                            {
                                existing.DateRequest = item.DateRequest;
                                existing.EmployeeID = item.EmployeeID;
                                existing.Deadline = item.Deadline;
                                existing.ProductCode = item.ProductCode;
                                existing.ProductName = item.ProductName;
                                existing.Maker = item.Maker;
                                existing.Note = item.Note;
                                existing.Unit = item.Unit;
                                existing.Quantity = item.Quantity;
                                existing.StatusRequest = 1;
                                existing.IsCommercialProduct = true;

                                await requestRepo.UpdateAsync(existing);
                            }
                        }
                        else
                        {
                            var newModel = new ProjectPartlistPriceRequest
                            {
                                DateRequest = item.DateRequest,
                                EmployeeID = item.EmployeeID,
                                Deadline = item.Deadline,
                                ProductCode = item.ProductCode,
                                ProductName = item.ProductName,
                                Maker = item.Maker,
                                Note = item.Note,
                                Unit = item.Unit,
                                Quantity = item.Quantity,
                                StatusRequest = 1,
                                IsCommercialProduct = true
                            };
                            requestRepo.Create(newModel);
                        }
                    }
                }

                if (dto.lstID != null && dto.lstID.Any())
                {
                    foreach (var id in dto.lstID)
                    {
                        var request = requestRepo.GetByID(id);
                        if (request != null)
                        {
                            request.IsDeleted = true;
                            request.UpdatedDate = DateTime.Now;
                            await requestRepo.UpdateAsync(request);
                        }
                    }
                }

                return Ok(new { status = 1 });
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

        [HttpGet("getSupplierSale")]
        public async Task<IActionResult> GetSupplierSale()
        {
            List<SupplierSale> lst = supplierSaleRepo.GetAll().OrderByDescending(x => x.ID).Take(10).ToList();
            return Ok(new { status = 0, data = lst });
        }
        [HttpPost("saveDataPriceRequest")]
        public async Task<IActionResult> SaveDataPriceRequest([FromBody] List<ProjectPartlistPriceRequest> requestList)
        {
            try
            {
                if (requestList == null || !requestList.Any())
                {
                    return BadRequest(new { status = 0, message = "Danh sách yêu cầu trống." });
                }

                foreach (var item in requestList)
                {
                    var existing = requestRepo.GetByID(item.ID);
                    if (existing == null)
                    {
                        return NotFound(new
                        {
                            status = 0,
                            message = $"Không tìm thấy sản phẩm với ID = {item.ID} trong cơ sở dữ liệu."
                        });
                    }
                    existing.EmployeeID = item.EmployeeID;
                    existing.Deadline = item.Deadline;
                    existing.Note = item.Note;
                    existing.Unit = item.Unit;
                    existing.Quantity = item.Quantity;
                    existing.TotalPrice = item.TotalPrice;
                    existing.UnitPrice = item.UnitPrice;
                    existing.VAT = item.VAT;
                    existing.TotaMoneyVAT = item.TotaMoneyVAT;
                    existing.CurrencyID = item.CurrencyID;
                    existing.CurrencyRate = item.CurrencyRate;
                    existing.IsCheckPrice = item.IsCheckPrice;
                    existing.SupplierSaleID = item.SupplierSaleID;
                    existing.UpdatedDate = DateTime.Now;
                    existing.UpdatedBy = item.UpdatedBy;

                    await requestRepo.UpdateAsync(existing);
                }
                return Ok(new { status = 1, message = "Cập nhật thành công." });
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
    }
}
