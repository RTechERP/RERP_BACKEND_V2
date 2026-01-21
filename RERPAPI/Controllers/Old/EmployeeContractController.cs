using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using Xceed.Words.NET;

namespace RERPAPI.Controllers.Old
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeContractController : ControllerBase
    {
        private EmployeeContractRepo _employeeContractRepo;
        private readonly IWebHostEnvironment _environment;
        public EmployeeContractController(EmployeeContractRepo employeeContractRepo, IWebHostEnvironment environment)
        {
            _employeeContractRepo = employeeContractRepo;
            _environment = environment;
        }




        //[HttpGet]
        //public IActionResult GetAll()
        //{
        //    try
        //    {
        //        List<EmployeeContract> employeeContracts = employeeContractRepo.GetAll();
        //        return Ok(new
        //        {
        //            status = 1,
        //            data = employeeContracts
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new
        //        {
        //            status = 0,
        //            message = ex.Message,
        //            error = ex.ToString()
        //        });
        //    }
        //}


        [HttpGet]
        [RequiresPermission("N1,N2,N60")]
        public IActionResult GetEmployeeContract(int employeeID, int? employeeContractTypeID, string? filterText)
        {
            try
            {
                filterText = string.IsNullOrWhiteSpace(filterText) ? "" : filterText;
                var employeeContracts = SQLHelper<object>.ProcedureToList("spGetEmployeeContract",
                                       new string[] { "@EmployeeID", "@LoaiHDLDID", "@FilterText" },
                                                          new object[] { employeeID, employeeContractTypeID, filterText ?? "" });
                //return Ok(new
                //{
                //    status = 1,
                //    data = SQLHelper<object>.GetListData(employeeContracts, 0)
                //});
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(employeeContracts, 0), ""));

            }
            catch (Exception ex)
            {
                //return BadRequest(new
                //{
                //    status = 0,
                //    message = ex.Message,
                //    error = ex.ToString()
                //});

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("{id}")]
        [RequiresPermission("N1,N2,N60")]
        public IActionResult GetEmployeeContractByID(int id)
        {
            try
            {
                var employeeContract = _employeeContractRepo.GetByID(id);
                //return Ok(new
                //{
                //    status = 1,
                //    data = employeeContract
                //});

                return Ok(ApiResponseFactory.Success(employeeContract, ""));
            }
            catch (Exception ex)
            {
                //return BadRequest(new
                //{
                //    status = 0,
                //    message = ex.Message,
                //    error = ex.ToString()
                //});

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost]
        [RequiresPermission("N1,N2,N60")]
        public async Task<IActionResult> SaveEmployeeContract([FromBody] EmployeeContract employeeContract)
        {
            try
            {
                if (employeeContract.ID <= 0) await _employeeContractRepo.CreateAsync(employeeContract);
                else await _employeeContractRepo.UpdateAsync(employeeContract);

                //return Ok(new
                //{
                //    status = 1,
                //    data = employeeContract,
                //    message = "Lưu thành công"
                //});
                return Ok(ApiResponseFactory.Success(employeeContract, ""));

            }
            catch (Exception ex)
            {
                //return BadRequest(new
                //{
                //    status = 0,
                //    message = ex.Message,
                //    error = ex.ToString()
                //});

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }



        [HttpGet("{id}/print-contract")]
        [RequiresPermission("N1,N2,N60")]
        public async Task<IActionResult> PrintContractEmployee(int id)
        {
            try
            {
                var printContract = SQLHelper<object>.ProcedureToList("spGetEmployeeContractWord",
                                                                        new string[] { "@ID" },
                                                                        new object[] { id });
                if (printContract == null || printContract.Count == 0)
                {
                    //return NotFound(new
                    //{
                    //    status = 0,
                    //    message = "Không tìm thấy thông tin hợp đồng"
                    //});

                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy thông tin hợp đồng"));
                }

                //return Ok(new
                //{
                //    status = 1,
                //    data = SQLHelper<object>.GetListData(printContract, 0)
                //});

                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(printContract, 0), ""));
            }
            catch (Exception ex)
            {
                //return BadRequest(new
                //{
                //    status = 0,
                //    message = ex.Message,
                //    error = ex.ToString()
                //});

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("generate")]
        [RequiresPermission("N1,N2,N60")]
        public IActionResult GenerateContract([FromBody] ContractDTO data)
        {
            try
            {
                // Determine template path and output filename based on LoaiHDLDID
                string templatePath;
                string outputFileName;
                var contractNumber = data.ContractNumber?.Replace("/", "") ?? "Contract";

                if (data.EmployeeLoaiHDLDID == 1) // HĐTV
                {
                    templatePath = Path.Combine(_environment.WebRootPath, "templates", "(Mau)_HDTV.docx");
                    outputFileName = $"{contractNumber}.docx";
                }
                else if (data.EmployeeLoaiHDLDID == 4) // HĐLĐ 12T
                {
                    templatePath = Path.Combine(_environment.WebRootPath, "templates", "(Mau)_HDLD.docx");
                    outputFileName = $"{contractNumber}_12T.docx";
                }
                else // HĐLĐ không xác định thời hạn
                {
                    templatePath = Path.Combine(_environment.WebRootPath, "templates", "(Mau)_HDLD.docx");
                    outputFileName = $"{contractNumber}.docx";
                }

                if (!System.IO.File.Exists(templatePath))
                {
                    //return NotFound(new { message = "Không tìm thấy file template" });
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy file template"));
                }

                using (var outputStream = new MemoryStream())
                {
                    // Read template
                    byte[] templateBytes;
                    using (var fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        templateBytes = new byte[fileStream.Length];
                        fileStream.Read(templateBytes, 0, templateBytes.Length);
                    }

                    // Load and process document
                    using (var templateStream = new MemoryStream(templateBytes))
                    using (var doc = DocX.Load(templateStream))
                    {
                        // Replace placeholders
                        doc.ReplaceText("#ContractNumber", data.ContractNumber ?? "");
                        doc.ReplaceText("#DateContract", data.DateContract ?? "");
                        doc.ReplaceText("#FullName", data.FullName ?? "");
                        doc.ReplaceText("#DateOfBirth", data.DateOfBirth ?? "");
                        doc.ReplaceText("#CCCD_CMND", data.CCCD_CMND ?? "");
                        doc.ReplaceText("#IssuedBy", data.IssuedBy ?? "");
                        doc.ReplaceText("#Address", data.Address ?? "");
                        doc.ReplaceText("#PhoneNumber", data.PhoneNumber ?? "");
                        doc.ReplaceText("#Sex", data.Sex ?? "");
                        doc.ReplaceText("#Nationality", data.Nationality ?? "");
                        doc.ReplaceText("#DateRange", data.DateRange ?? "");
                        doc.ReplaceText("#ContractType", data.ContractType ?? "");
                        doc.ReplaceText("#ContractDuration", data.ContractDuration ?? "");
                        doc.ReplaceText("#Position", data.Position ?? "");
                        doc.ReplaceText("#Department", data.Department ?? "");
                        doc.ReplaceText("#Salary", data.Salary ?? "");
                        doc.ReplaceText("#NotificationDate", data.NotificationDate ?? "");

                        // ===== THÔNG TIN CÔNG TY =====
                        doc.ReplaceText("#CompanyNameHeader",
                            (data.CompanyName ?? "").Replace("\n", "\r\n").ToUpper());
                        doc.ReplaceText("#COMPANYCODE", data.COMPANYCODE ?? "");
                        doc.ReplaceText("#CompanyName", data.CompanyName ?? "");
                        doc.ReplaceText("#UPPERCOMPANYNAME", (data.CompanyName ?? "").ToUpper());
                        doc.ReplaceText("#TaxCodeCom", data.TaxCodeCom ?? "");
                        doc.ReplaceText("#AddCom", data.AddressCom ?? "");
                        doc.ReplaceText("#PhoneCom", data.PhoneNumberCom ?? "");
                        doc.ReplaceText("#DirectorCom", data.DirectorCom ?? "");
                        doc.ReplaceText("#PosCom", data.PositionCom ?? "");


                        // Save document
                        doc.SaveAs(outputStream);
                    }

                    var fileBytes = outputStream.ToArray();
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", outputFileName);
                }
            }
            catch (IOException ex)
            {
                //return StatusCode(500, new { message = $"Lỗi truy cập file: {ex.Message}" });
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
            catch (Exception ex)
            {
                //return StatusCode(500, new { message = $"Lỗi xử lý: {ex.Message}" });
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
