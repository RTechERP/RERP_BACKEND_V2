using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace RERPAPI.Controllers.Old
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillImportQCController : ControllerBase
    {
        List<int> lsEmployeeID = new List<int>();
        List<int> lsLeaderID = new List<int>();
        List<string> lsEmailCC = new List<string>();

        EmployeeRepo _employeeRepo;
        BillImportQCRepo _billImportQCRepo;
        BillImportQCDetailRepo _billImportQCDetailRepo;
        ProjectRepo _projectRepo;
        ProductSaleRepo _productSaleRepo;
        BillImportQCDetailFilesRepo _billImportQCDetailFilesRepo;
        ConfigSystemRepo _configSystemRepo;
        public BillImportQCController(
            BillImportQCDetailRepo billImportQCDetailRepo,
            BillImportQCRepo billImportQCRepo,
            ProjectRepo projectRepo,
            ProductSaleRepo productSaleRepo,
            BillImportQCDetailFilesRepo billImportQCDetailFilesRepo,
            EmployeeRepo employeeRepo
            ,
            ConfigSystemRepo configSystemRepo)
        {
            _billImportQCRepo = billImportQCRepo;
            _billImportQCDetailRepo = billImportQCDetailRepo;
            _projectRepo = projectRepo;
            _productSaleRepo = productSaleRepo;
            _billImportQCDetailFilesRepo = billImportQCDetailFilesRepo;
            _employeeRepo = employeeRepo;
            _configSystemRepo = configSystemRepo;
        }

        [HttpGet("projects")]
        public IActionResult LoadProjects()
        {
            try
            {
                var projects = _projectRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(projects, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("leaders")]
        public IActionResult LoadLeader()
        {
            try
            {
                var dt = SQLHelper<dynamic>.ProcedureToList(
                    "spGetEmployeeApprove",
                    new string[]
                    {
                        "@Type"
                    },
                    new object[]
                    {
                        2
                    }
                );

                var data = SQLHelper<dynamic>.GetListData(dt, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("product-sale")]
        public IActionResult LoadProductSale()
        {
            try
            {
                var data = _productSaleRepo.GetAll().Where(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("bill-import-qc")]
        [RequiresPermission("")]
        public IActionResult LoadBillImportQc(int billImportRequestId)
        {
            try
            {
                var data = _billImportQCRepo.GetByID(billImportRequestId);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("files")]
        [RequiresPermission("")]
        public IActionResult LoadFiles(int billImportQCDetailId)
        {
            try
            {
                var dt = SQLHelper<dynamic>.ProcedureToList(
                    "spGetBillImportQCDetailFiles",
                    new string[]
                    {
                        "@BillImportQCDetailID"
                    },
                    new object[]
                    {
                        billImportQCDetailId
                    }
                );

                var fileCheckSheets = SQLHelper<dynamic>.GetListData(dt, 0);//FILE CHECK SHEET
                var fileTestReports = SQLHelper<dynamic>.GetListData(dt, 1); //File test report;

                var data = new
                {
                    FileCheckSheets = fileCheckSheets,
                    FileTestReports = fileTestReports
                };

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("bill-number")]
        [RequiresPermission("")]
        public IActionResult LoadBillnumber()
        {
            try
            {
                string billNumber = "";
                int code = 0;

                string month = TextUtils.ToString(DateTime.Now.ToString("MM"));
                string day = TextUtils.ToString(DateTime.Now.ToString("dd"));
                string year = TextUtils.ToString(DateTime.Now.Year).Substring(2);
                string date = year + month + day;

                var now = DateTime.Now;

                var bill = _billImportQCRepo.GetAll()
                    .Where(x =>
                        x.CreatedDate.HasValue &&
                        x.CreatedDate.Value.Year == now.Year &&
                        x.CreatedDate.Value.Month == now.Month &&
                        x.CreatedDate.Value.Day == now.Day &&
                        x.IsDeleted == false
                    )
                    .OrderByDescending(x => x.ID)
                    .FirstOrDefault();

                string requestImportCode = "";
                if (bill != null) requestImportCode = bill.RequestImportCode;

                if (requestImportCode.Contains("YCQC"))
                {
                    requestImportCode = requestImportCode.Substring(4);
                }

                if (requestImportCode == "")
                {
                    billNumber = "YCQC" + date + "001";
                }
                else
                {
                    code = Convert.ToInt32(requestImportCode.Substring(requestImportCode.Length - 3));
                }

                if (code == 0)
                {
                    billNumber = "YCQC" + date + "001";
                }
                else
                {
                    string dem = TextUtils.ToString(code + 1);
                    for (int i = 0; dem.Length < 3; i++)
                    {
                        dem = "0" + dem;
                    }

                    billNumber = "YCQC" + date + TextUtils.ToString(dem);
                }

                return Ok(ApiResponseFactory.Success(billNumber, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("data-master")]
        [RequiresPermission("")]
        public IActionResult GetDataMaster(DateTime dateStart, DateTime dateEnd, int employeeRequestId, string? keyword)
        {
            try
            {
                dateStart = dateStart.Date;
                dateEnd = dateEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                // Gọi stored procedure
                var dt = SQLHelper<dynamic>.ProcedureToList(
                    "spGetBillImportRequestQC",
                    new string[]
                    {
                        "@DateStart", "@DateEnd", "@EmployeeRequestID", "@Keyword"
                    },
                    new object[]
                    {
                        dateStart, dateEnd, employeeRequestId, keyword ?? ""
                    }
                );

                var data = SQLHelper<dynamic>.GetListData(dt, 0);

                return Ok(ApiResponseFactory.Success(data, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("data-detail")]
        [RequiresPermission("")]
        public IActionResult GetDataDetail(int billImportRequestId)
        {
            try
            {
                //Thêm trường CheckSheet và Report trong spGetBillImportRequestQCDetail
                var dt = SQLHelper<object>.ProcedureToList("spGetBillImportRequestQCDetail",
                    new string[] { "@BillImportRequestID" },
                    new object[] { billImportRequestId });

                var data = SQLHelper<object>.GetListData(dt, 0);

                foreach (dynamic item in data)
                {
                    int detailId = Convert.ToInt32(item.ID);

                    var files = _billImportQCDetailFilesRepo.GetAll(x => x.BillImportQCDetailID == detailId);

                    if (files != null && files.Count > 0)
                    {
                        item.CheckSheet = string.Join(";",
                            files.Where(x => x.FileType == 1)
                                 .Select(x => x.FileName));

                        item.Report = string.Join(";",
                            files.Where(x => x.FileType == 2)
                                 .Select(x => x.FileName));
                    }
                    else
                    {
                        item.CheckSheet = string.Empty;
                        item.Report = string.Empty;
                    }
                }

                return Ok(ApiResponseFactory.Success(data, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("deleted-bill-import-qc")]
        [RequiresPermission("")]
        public async Task<IActionResult> DeletedBillImportQC([FromBody] List<BillImportQC> billImportQCs)
        {
            var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
            var currentUser = ObjectMapper.GetCurrentUser(claims);

            try
            {
                if (billImportQCs.Count() > 0)
                {
                    foreach (BillImportQC billImportQC in billImportQCs)
                    {
                        // Thêm khóa cho bảng BillImportQCDetail
                        List<BillImportQCDetail> lsDetail = _billImportQCDetailRepo.GetAll().Where(x => x.BillImportQCID == billImportQC.ID).ToList();
                        if (lsDetail.Any(x => x.Status != 0) && !(currentUser.IsAdmin && currentUser.EmployeeID <= 0))
                        {
                            return BadRequest(ApiResponseFactory.Fail(
                                null,
                                $"Sản phẩm phiếu {billImportQC.RequestImportCode} đã được yêu cầu QC. Bạn không thể xóa!"
                                ));
                        }

                        foreach (BillImportQCDetail detail in lsDetail)
                        {
                            detail.IsDeleted = true;
                            detail.UpdatedBy = currentUser.Code;
                            detail.UpdatedDate = DateTime.Now;
                            await _billImportQCDetailRepo.UpdateAsync(detail);
                        }

                        billImportQC.IsDeleted = true;
                        billImportQC.UpdatedBy = currentUser.Code;
                        billImportQC.UpdatedDate = DateTime.Now;
                        await _billImportQCRepo.UpdateAsync(billImportQC);
                    }
                }

                return Ok(ApiResponseFactory.Success(0, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-bill-import-qc")]
        [DisableRequestSizeLimit]
        [RequiresPermission("")]
        public async Task<IActionResult> SaveBillImportQC()
        {
            try
            {
                bool isNew = false;
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };

                #region Lấy dữ liệu từ formdata
                var form = await Request.ReadFormAsync();
                var dto = new BillImportQCDTO();

                if (form.TryGetValue("billImportQC", out var header))
                {
                    dto.billImportQC = JsonSerializer.Deserialize<BillImportQC>(
                        header!,
                        jsonOptions
                    );
                }

                if (form.TryGetValue("billImportQCDetails", out var details))
                {
                    dto.billImportQCDetails = JsonSerializer.Deserialize<List<BillImportQCDetail>>(
                        details!,
                        jsonOptions
                    );
                }

                dto.DeletedDetailIds = _billImportQCRepo.DeserializeList<int>(form, "DeletedDetailIds");
                dto.DeletedCheckSheetFileIds = _billImportQCRepo.DeserializeList<int>(form, "DeletedCheckSheetFileIds");
                dto.DeletedReportFileIds = _billImportQCRepo.DeserializeList<int>(form, "DeletedReportFileIds");

                dto.CheckSheetFiles = _billImportQCRepo.ExtractFiles(form, "CheckSheetFiles", 1);
                dto.ReportFiles = _billImportQCRepo.ExtractFiles(form, "ReportFiles", 2);
                //return Ok(ApiResponseFactory.Success(null, ""));
                #endregion

                #region Xử lý dữ liệu Master
                if (dto.billImportQC.ID > 0)
                {
                    dto.billImportQC.UpdatedDate = DateTime.Now;
                    dto.billImportQC.UpdatedBy = currentUser.LoginName;
                    await _billImportQCRepo.UpdateAsync(dto.billImportQC);
                }
                else
                {
                    isNew = true;
                    await _billImportQCRepo.CreateAsync(dto.billImportQC);
                }
                #endregion

                #region xử lý dữ liệu detail
                if (dto.billImportQCDetails.Count() > 0)
                {
                    foreach (var detail in dto.billImportQCDetails)
                    {
                        var checkSheetFiles = dto.CheckSheetFiles.Where(x => x.BillImportQCDetailId == detail.ID).ToList();
                        var reportFiles = dto.ReportFiles.Where(x => x.BillImportQCDetailId == detail.ID).ToList();

                        if (!lsLeaderID.Contains(Convert.ToInt32(detail.LeaderTechID)) && Convert.ToInt32(detail.LeaderTechID) > 0)
                            lsLeaderID.Add(Convert.ToInt32(detail.LeaderTechID));

                        if (!lsEmployeeID.Contains(Convert.ToInt32(detail.EmployeeTechID)) && Convert.ToInt32(detail.EmployeeTechID) > 0)
                            lsEmployeeID.Add(Convert.ToInt32(detail.EmployeeTechID));

                        if (detail.ID > 0)
                        {
                            detail.UpdatedDate = DateTime.Now;
                            detail.UpdatedBy = currentUser.LoginName;
                            await _billImportQCDetailRepo.UpdateAsync(detail);
                        }
                        else
                        {
                            detail.ID = 0;
                            // detail.BillImportDetailID = TextUtils.ToInt(grvDetails.GetRowCellValue(i, colBillImportDetailID));
                            detail.BillImportQCID = dto.billImportQC.ID;
                            if (detail.Status == 0) detail.Status = 3;
                            detail.IsDeleted = false;
                            await _billImportQCDetailRepo.CreateAsync(detail);
                        }

                        #region Xử lý upload file
                        if (dto.billImportQC.EmployeeRequestID != currentUser.EmployeeID && !currentUser.IsAdmin) continue;

                        var uploadPath = _configSystemRepo.GetUploadPathByKey("BillImportQCDetailFiles");
                        if (string.IsNullOrWhiteSpace(uploadPath))
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy cấu hình đường dẫn cho key: BillImportQCDetailFiles"));
                        }

                        var project = _projectRepo.GetByID(Convert.ToInt32(detail.ProjectID));

                        int year = project.CreatedDate.HasValue ? project.CreatedDate.Value.Year : DateTime.Now.Year;
                        string projectCode = string.IsNullOrWhiteSpace(project.ProjectCode) ? "" : project.ProjectCode.Trim();

                        string pathPattern = $@"{year}\{projectCode}\TaiLieuChung\QC\{dto.billImportQC.RequestImportCode}";
                        string pathUpload = Path.Combine(uploadPath, pathPattern);

                        var client = new HttpClient();

                        if (checkSheetFiles != null && checkSheetFiles.Any())
                        {
                            await _billImportQCDetailFilesRepo.UploadFiles(checkSheetFiles, detail.ID, pathUpload);
                        }

                        if (reportFiles != null && reportFiles.Any())
                        {
                            await _billImportQCDetailFilesRepo.UploadFiles(reportFiles, detail.ID, pathUpload);
                        }
                        #endregion
                    }
                }
                #endregion

                #region Xử lý xóa file
                if (dto.DeletedCheckSheetFileIds != null && dto.DeletedCheckSheetFileIds.Count() > 0)
                {
                    var url = $"http://113.190.234.64:8083/api/Home/removefile?path=";
                    var client = new HttpClient();
                    foreach (int id in dto.DeletedCheckSheetFileIds)
                    {
                        if (id > 0)
                        {
                            var billImportQCDetailFile = _billImportQCDetailFilesRepo.GetByID(id);
                            url += $@"{billImportQCDetailFile.ServerPath}\{billImportQCDetailFile.FileName}";
                            var result = await client.GetAsync(url);

                            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                await _billImportQCDetailFilesRepo.DeleteAsync(id);
                            }
                        }
                    }

                    foreach (int id in dto.DeletedReportFileIds)
                    {
                        if (id > 0)
                        {
                            var billImportQCDetailFile = _billImportQCDetailFilesRepo.GetByID(id);
                            url += $@"{billImportQCDetailFile.ServerPath}\{billImportQCDetailFile.FileName}";
                            var result = await client.GetAsync(url);

                            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                await _billImportQCDetailFilesRepo.DeleteAsync(id);
                            }
                        }
                    }
                }
                #endregion

                #region Xử lý xóa detail
                if (dto.DeletedDetailIds != null && dto.DeletedDetailIds.Count() > 0)
                {
                    foreach (int id in dto.DeletedDetailIds)
                    {
                        if (id > 0) await _billImportQCDetailRepo.DeleteAsync(id);
                    }
                }
                #endregion

                #region Xử lý gửi mail
                if (isNew)
                {
                    string emailCCs = "";
                    string emRequestName = _employeeRepo.GetByID(Convert.ToInt32(dto.billImportQC.EmployeeRequestID)).FullName;
                    foreach (int id in lsEmployeeID)
                    {
                        string emailCC = _employeeRepo.GetByID(id).EmailCongTy;
                        if (!lsEmailCC.Contains(emailCC) && emailCC != "") lsEmailCC.Add(emailCC);
                    }

                    if (lsEmailCC.Count > 0) emailCCs = string.Join(";", lsEmailCC);

                    foreach (int leaderID in lsLeaderID)
                    {
                        List<BillImportQCDetail> dtr = dto.billImportQCDetails.Where(x => x.LeaderTechID == leaderID).ToList();

                        string emailEmRequest = _employeeRepo.GetByID(leaderID).EmailCongTy;
                        _billImportQCRepo.SetInforEmail(emailCCs, emailEmRequest, leaderID, dtr, emRequestName, (DateTime)dto.billImportQC.Dealine);
                    }
                }
                #endregion

                return Ok(ApiResponseFactory.Success(dto.billImportQC.ID, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
