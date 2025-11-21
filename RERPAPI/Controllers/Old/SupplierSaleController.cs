using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.DocumentManager;
using ZXing;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SupplierSaleController : ControllerBase
    {
        SupplierRepo _supplierRepo;
        SupplierSaleContactRepo _supplierSaleContactRepo;
        SupplierSaleRepo _supplierSaleRepo;
        RulePayRepo _rulePayRepo;
        TaxCompanyRepo _taxCompanyRepo;
        EmployeeRepo _employeeRepo;
        DepartmentRepo _departmentRepo;
        DocumentFileRepo _documentFileRepo;
        DocumentTypeRepo _documentTypeRepo;
        DocumentRepo _documentRepo;
        ConfigSystemRepo _configSystemRepo;


        public SupplierSaleController(
            SupplierRepo supplierRepo,
            SupplierSaleContactRepo supplierSaleContactRepo,
            SupplierSaleRepo supplierSaleRepo,
            RulePayRepo rulePayRepo,
            TaxCompanyRepo taxCompanyRepo,
            EmployeeRepo employeeRepo,
            DepartmentRepo departmentRepo,
            DocumentFileRepo documentFileRepo,
            DocumentTypeRepo documentTypeRepo,
            DocumentRepo documentRepo,
            ConfigSystemRepo configSystemRepo
        )
        {
            _supplierRepo = supplierRepo;
            _supplierSaleContactRepo = supplierSaleContactRepo;
            _supplierSaleRepo = supplierSaleRepo;
            _rulePayRepo = rulePayRepo;
            _taxCompanyRepo = taxCompanyRepo;
            _employeeRepo = employeeRepo;
            _departmentRepo = departmentRepo;
            _documentFileRepo = documentFileRepo;
            _documentTypeRepo = documentTypeRepo;
            _documentRepo = documentRepo;
            _configSystemRepo = configSystemRepo;
        }
        #region Get
        // Danh sách supplier
        [HttpGet("supplier-sale")]
        [RequiresPermission("N27,N33,N35,N1,N36")]
        public async Task<IActionResult> getSupplierSale(string? keyword, int page, int size)
        {
            try
            {
                var saleSupplier = SQLHelper<object>.ProcedureToList("spFindSupplierNCC",
                    new string[] { "@Find", "@PageNumber", "@PageSize" },
                    new object[] { keyword ?? "", page, size });

                var result = new
                {
                    data = SQLHelper<object>.GetListData(saleSupplier, 0),
                    totalPage = SQLHelper<object>.GetListData(saleSupplier, 1).FirstOrDefault().TotalPage ?? 1
                };

                return Ok(ApiResponseFactory.Success(result, null));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("supplier-sale-contact")]
        [RequiresPermission("N27,N33,N52,N53,N35,N1")]
        public async Task<IActionResult> getSupplierSaleContact(int supplierID)
        {
            try
            {
                var data = _supplierSaleContactRepo.GetAll()
                    .Where(c => c.SupplierID == supplierID)
                    .OrderByDescending(c => c.ID);

                return Ok(ApiResponseFactory.Success(data, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        


        #endregion
        #region Method Post
        [HttpPost("supplier-sale")]
        [RequiresPermission("N27,N33,N35,N1")]
        public async Task<IActionResult> savesuppliersale([FromBody] SupplierSale supplierSale)
        {
            try
            {
                if (supplierSale != null && supplierSale.IsDeleted == true && supplierSale.ID > 0)
                {
                    SupplierSale sup = _supplierSaleRepo.GetByID(supplierSale.ID);
                    sup.IsDeleted = true;
                    await _supplierSaleRepo.UpdateAsync(sup);
                    return Ok(ApiResponseFactory.Success(null, null));
                }

                if (!_supplierSaleRepo.Validate(supplierSale, out string message))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }

                if (supplierSale.ID <= 0)
                {
                    if (supplierSale.EmployeeID == 0)
                    {
                        supplierSale.NVPhuTrach = "";
                    }
                    else
                    {
                        if (supplierSale.EmployeeID != null && supplierSale.EmployeeID > 0)
                        {
                            string NVPhuTrach = _employeeRepo.GetByID((int)supplierSale.EmployeeID).FullName;
                            supplierSale.NVPhuTrach = NVPhuTrach;
                        }
                    }
                    await _supplierSaleRepo.CreateAsync(supplierSale);
                }
                else
                {
                    if (supplierSale.EmployeeID != 0 && supplierSale.EmployeeID != null)
                    {
                        string NVPhuTrach = _employeeRepo.GetByID((int)supplierSale.EmployeeID).FullName;
                        supplierSale.NVPhuTrach = NVPhuTrach;
                    }

                    await _supplierSaleRepo.UpdateAsync(supplierSale);
                }

                return Ok(ApiResponseFactory.Success(supplierSale.ID, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

        [HttpPost("supplier-sale-contact")]
        [RequiresPermission("N27,N33,N35,N1")]
        public async Task<IActionResult> savesuppliersalecontact([FromBody] SupplierSaleContact supplierSaleContact)
        {
            try
            {
                if (supplierSaleContact.ID <= 0)
                {
                    await _supplierSaleContactRepo.CreateAsync(supplierSaleContact);
                }
                else
                {
                    _supplierSaleContactRepo.Update(supplierSaleContact);
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
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
        #endregion

        #region API chưa sử dụng
        [HttpPost("savedocument")]
        public async Task<IActionResult> savedocument([FromBody] Model.Entities.Document document)
        {
            try
            {
                if (document.ID <= 0)
                {
                    document.GroupType = 2;
                    await _documentRepo.CreateAsync(document);
                }
                else
                {
                    _documentRepo.Update(document);
                }

                return Ok(new
                {
                    status = 1,
                    data = ""
                })
                ;
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
        [HttpPost("savedocumentfile")]
        public async Task<IActionResult> savedocumentfile([FromBody] Model.Entities.DocumentFile documentfile)
        {
            try
            {
                if (documentfile.ID <= 0)
                {
                    await _documentFileRepo.CreateAsync(documentfile);
                }
                else
                {
                    _documentFileRepo.Update(documentfile);
                }

                return Ok(new
                {
                    status = 1,
                    data = ""
                })
                ;
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
        [HttpPost("uploaddocumentfiles")]
        public async Task<IActionResult> uploaddocumentfiles([FromForm] string departmentCode, [FromForm] string documentType, [FromForm] int documentID, [FromForm] List<IFormFile> files)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(departmentCode))
                    departmentCode += @"\";
                string pathServer = _configSystemRepo.GetAll(c => c.KeyName.Trim() == "PathDocumenSale").FirstOrDefault().KeyValue;
                string pathPattern = $@"{departmentCode}{documentType}";
                string pathUpload = Path.Combine(pathServer, pathPattern);
                int totalSuccess = 0;

                List<ProjectSurveyFile> listFiles = new List<ProjectSurveyFile>();
                foreach (var file in files)
                {

                    FileInfo fileInfo = new FileInfo(file.FileName);
                    string fileName = fileInfo.Name;
                    string duoifile = Path.GetExtension(file.FileName);
                    fileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{DateTime.Now.ToString("ddMMyyHHmmss")}{duoifile}";
                    string filePath = Path.Combine(pathUpload, fileName);

                    if (file.Length < 0)
                        continue;
                    using (var client = new HttpClient())
                    {
                        using (var fileStream = file.OpenReadStream())
                        {
                            var content = new MultipartFormDataContent
                            {
                                { new StreamContent(fileStream), "file", fileName}
                            };

                            string url = $"http://14.232.152.154:8083/api/Home/uploadfile?path={pathUpload}";
                            var response = await client.PostAsync(url, content);
                            string contentresponse = await response.Content.ReadAsStringAsync();

                            JObject json = JObject.Parse(contentresponse);
                            int status = json.Value<int?>("status") ?? 0;
                            if (status == 1)
                            {
                                DocumentFile modelFile = new DocumentFile
                                {
                                    FileName = fileName,
                                    FileNameOrigin = file.FileName,
                                    FilePath = file.FileName,
                                    DocumentID = documentID
                                };
                                var result = await _documentFileRepo.CreateAsync(modelFile);

                                totalSuccess++;

                            }
                        }
                    }

                }

                return Ok(ApiResponseFactory.Success(1, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("getdocumenttype")]
        public async Task<IActionResult> getdocumenttype()
        {
            try
            {
                var data = _documentTypeRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = data.ToList()
                });
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

        [HttpGet("getsalesupplierbyid")]
        public async Task<IActionResult> getsalesupplierbyid(int supplierID)
        {
            try
            {
                var data = _supplierSaleRepo.GetAll().FirstOrDefault(c => c.ID == supplierID);
                return Ok(new
                {
                    status = 1,
                    data = data
                });
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

        [HttpGet("getrulepay")]
        public async Task<IActionResult> getrulepay()
        {
            try
            {
                var data = _rulePayRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = data.ToList()
                });
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
        [HttpGet("getdepartment")]
        public async Task<IActionResult> getdepartment()
        {
            try
            {
                var data = _departmentRepo.GetAll().OrderBy(c => c.STT).ToList();
                return Ok(new
                {
                    status = 1,
                    data = data
                });
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
        [HttpGet("getdocumentsaleadmin/{departmentID:int}")]
        public async Task<IActionResult> getdocumentsaleadmin([FromRoute] int departmentID)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetDocumentSaleAdmin", new string[] { "@GroupType", "@DepartmentID" }, new object[] { 2, departmentID });
                return Ok(new
                {
                    status = 1,
                    data = data
                });
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

        [HttpGet("gettaxcompany")]
        public async Task<IActionResult> gettaxcompany()
        {
            try
            {
                var data = _taxCompanyRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = data.ToList()
                });
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
        [HttpGet("getdocumentfile/{documentID}")]
        public async Task<IActionResult> getdocumentfile(int documentID)
        {
            try
            {
                var data = _documentFileRepo.GetAll().Where(c => c.DocumentID == documentID && c.IsDeleted == false);
                return Ok(new
                {
                    status = 1,
                    data = data.ToList()
                });
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
        [HttpGet("getdocument/{documentID}")]
        public async Task<IActionResult> getdocument(int documentID)
        {
            try
            {
                var data = _documentRepo.GetByID(documentID);
                return Ok(new
                {
                    status = 1,
                    data = data
                });
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
        [HttpPost("downloaddocumentfile")]
        public async Task<IActionResult> downloaddocumentfile([FromForm] int[] arrDocumentFileID, [FromForm] string documentType, [FromForm] string departmentCode)
        {
            try
            {
                string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string pathDownload = Path.Combine(userFolder, "Downloads", $"FORM_BIEU_CHUNG/{departmentCode}/{documentType}");

                if (!Directory.Exists(pathDownload))
                {
                    Directory.CreateDirectory(pathDownload);
                }
                string pathPattern = Path.Combine($"{departmentCode}/{documentType}");
                List<string> lst = new List<string>();

                foreach (int id in arrDocumentFileID)
                {
                    string fileName = _documentFileRepo.GetByID(id).FileName;
                    string folderDownload = Path.Combine(pathDownload, fileName);
                    string url = $"http://14.232.152.154:8083/api/formadminsale/{pathPattern}/{fileName}";
                    lst.Add(url);
                }
                return Ok(new
                {
                    status = 1,
                    data = lst.ToList()
                });
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


        [HttpGet("getcheckexistdocument")]
        public async Task<IActionResult> getcheckexistdocument(string code)
        {
            try
            {
                var existDocument = _documentRepo.GetAll(c => c.GroupType == 2 && c.IsDeleted == false && c.Code.Trim().ToLower() == code.Trim().ToLower());
                if (existDocument.Count == 0)
                {
                    return Ok(new
                    {
                        status = 1,
                        message = "Không có mã văn bản nào bị trùng lặp!"
                    });

                }
                return Ok(new
                {
                    status = 0,
                    message = "Đã tồn mã văn bản trên hệ thống!"
                });
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
        #endregion
    }
}