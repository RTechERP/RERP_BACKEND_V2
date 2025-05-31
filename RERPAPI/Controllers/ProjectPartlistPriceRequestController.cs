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
        ProjectSolutionRepo projectSolutionRepo = new ProjectSolutionRepo();

        [HttpGet("getallProjectParListPriceRequest")]
        public async Task<IActionResult> GetAll( DateTime dateStart,DateTime dateEnd,int statusRequest, int projectId,string? keyword,
            int isDeleted,int projectTypeID, int poKHID,int isCommercialProduct = -1,int page = 1, int size = 25)
        {
            if (projectTypeID < 0) isCommercialProduct = 1;
            else poKHID = 0;

            // Gọi stored procedure với tham số phân trang
            List<List<dynamic>> dtPriceRequest = SQLHelper<dynamic>.ProcedureToList("spGetProjectPartlistPriceRequest_New",
                                                                          new string[] {
                                                                  "@DateStart", "@DateEnd", "@StatusRequest", "@ProjectID", "@Keyword", "@IsDeleted",
                                                                  "@ProjectTypeID", "@IsCommercialProduct", "@POKHID", "@PageNumber", "@PageSize"
                                                                          },
                                                                          new object[] {
                                                                  dateStart, dateEnd, statusRequest, projectId, keyword, isDeleted,
                                                                  projectTypeID, isCommercialProduct, poKHID, page, size
                                                                          }
                                                                        );


            int totalRecords = dtPriceRequest[1][0].Total;
            int totalPages = (int)Math.Ceiling((double)totalRecords / size);


            return Ok(new
            {
                status = 1,
                data = new
                {
                    dtData = dtPriceRequest[0],
                    totalPages = totalPages
                }
            });
        }
        [HttpGet("getType")]
        public async Task<IActionResult> GetAllTypebyEmployeeID(int employeeID)
        {

            List<List<dynamic>> dtType = SQLHelper<dynamic>.ProcedureToList("spGetProjectTypeAssignByEmployeeID",
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
            var dtEmployee = SQLHelper<dynamic>.ProcedureToList("spGetEmployee", new string[] { "@Status" }, new object[] { 0 });
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
                .OrderBy(p => p.ID)
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
        public async Task<IActionResult> SaveData([FromBody] List<ProjectPartlistPriceRequest> dto)
        {
            try
            {
                if (dto != null && dto.Any())
                {
                    foreach (var item in dto)
                    {
                        if (item.ID > 0)
                        {
                            requestRepo.UpdateFieldsByID(item.ID, item);
                        }
                        else
                        {
                            requestRepo.Create(item);
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
        [HttpPost("download")]
        public async Task<IActionResult> DownloadFile([FromBody] DownloadProjectPartlistPriceRequestDTO request)
        {
            var project = projectRepo.GetByID(request.ProjectId);
            if (project == null || project.CreatedDate == null)
                return BadRequest(new
                {
                    status=0, 
                    message= "Không tim thấy dự án!"
                });

            var solutions = SQLHelper<ProjectSolution>.ProcedureToList("spGetProjectSolutionByProjectPartListID",
                                                            new string[] { "@ProjectPartListID" }, new object[] { request.PartListId });

            if (solutions.Count <= 0)
                return BadRequest(new
                {
                    status = 0,
                    message = "Không tìm thấy giải pháp dự án!"
                });
            var solution = SQLHelper<dynamic>.GetListData(solutions, 0).FirstOrDefault();
            var year = project.CreatedDate.Value.Year;
            var pathPattern = $"{year}/{project.ProjectCode.Trim()}/THIETKE.Co/{solution?.CodeSolution.Trim()}/2D/GC/DH";
            var fileName = $"{request.ProductCode}.pdf";

            var fileUrl = $"http://14.232.152.154:8083/api/project/{pathPattern}/{fileName}";

            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(fileUrl);

                if (!response.IsSuccessStatusCode)
                    return NotFound($"Không tìm thấy file: {fileUrl}");

                var stream = await response.Content.ReadAsStreamAsync();
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status=0,
                    message=ex.Message,
                    error=ex.ToString()
                });
            }
        }

    }
}
