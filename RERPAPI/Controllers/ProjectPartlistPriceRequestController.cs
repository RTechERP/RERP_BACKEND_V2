using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
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
    [ApiKeyAuthorize]
    public class ProjectPartlistPriceRequestController : ControllerBase
    {
        #region Khai báo repository
        ProjectRepo projectRepo = new ProjectRepo();
        POKHRepo pOKHRepo = new POKHRepo();
        ProjectPartlistPriceRequestRepo requestRepo = new ProjectPartlistPriceRequestRepo();
        ProductSaleRepo productSaleRepo = new ProductSaleRepo();
        CurrencyRepo currencyRepo = new CurrencyRepo();
        SupplierSaleRepo supplierSaleRepo = new SupplierSaleRepo();
        ProjectSolutionRepo projectSolutionRepo = new ProjectSolutionRepo();
        #endregion
        #region Lấy tất cả yêu cầu báo giá
        /// <summary>
        /// lấy tất cả yêu cầu báo giá
        /// </summary>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <param name="statusRequest"> 0:tất cả;1:Yêu cầu báo giá;2:Đã báo giá;3:Đã hoàn thành</param>
        /// <param name="projectId"></param>
        /// <param name="keyword"></param>
        /// <param name="isDeleted"></param>
        /// <param name="projectTypeID"></param>
        /// <param name="poKHID"></param>
        /// <param name="isCommercialProduct"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet("get-all-project-parList-price-request")]
        public async Task<IActionResult> GetAll( DateTime dateStart,DateTime dateEnd,int statusRequest, int projectId,string? keyword,
            int isDeleted,int projectTypeID, int poKHID,int isCommercialProduct = -1,int page = 1, int size = 25)
        {
            if (projectTypeID < 0) isCommercialProduct = 1;
            else poKHID = 0;

            
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
        #endregion
        [HttpGet("get-type")]
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
        [HttpGet("get-all-projects")]
        public async Task<IActionResult> GetAllProjects()
        {
            List<Project> lstProjects = projectRepo.GetAll();
            return Ok(new { status = 1, data = lstProjects });
        }
        [HttpGet("get-all-employee")]
        public async Task<IActionResult> GetAllEmployee()
        {
            var dtEmployee = SQLHelper<dynamic>.ProcedureToList("spGetEmployee", new string[] { "@Status" }, new object[] { 0 });
            return Ok(new { status = 1, data = new { dtEmployee = dtEmployee[0] } });
        }
        [HttpGet("get-po-code")]
        public async Task<IActionResult> GetPoCode()
        {
            var dtPOKH = pOKHRepo.GetAll();
            return Ok(new { status = 1, data = dtPOKH });
        }
        [HttpGet("get-product-sale")]
        public async Task<IActionResult> GetProductSale()
        {
            var data = productSaleRepo.GetAll().ToList();

            return Ok(new
            {
                status = 1,
                data = data,
            });
        }
        [HttpGet("get-currency")]
        public async Task<IActionResult> GetCurrency()
        {
            List<Currency> currencies = currencyRepo.GetAll();
            return Ok(new { status = 1, data = currencies });
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] List<ProjectPartlistPriceRequest> projectPartlistPriceRequest)
        {
            try
            {
                if (projectPartlistPriceRequest != null && projectPartlistPriceRequest.Any())
                {
                    foreach (var item in projectPartlistPriceRequest)
                    {
                        if (item.ID > 0)
                        {
                            requestRepo.Update(item);
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

        [HttpGet("get-supplier-sale")]
        public async Task<IActionResult> GetSupplierSale()
        {
            List<SupplierSale> lst = supplierSaleRepo.GetAll().ToList();
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
            //sửa
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
