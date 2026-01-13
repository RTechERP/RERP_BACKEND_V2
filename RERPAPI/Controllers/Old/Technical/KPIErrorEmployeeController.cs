using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.Technical
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KPIErrorEmployeeController : ControllerBase
    {
        private readonly DepartmentRepo _departmentRepo;
        private readonly KPIErrorTypeRepo _kpiErrorTypeRepo;
        private readonly KPIErrorEmployeeRepo _kpiErrorEmployeeRepo;
        private readonly KPIErrorEmployeeFileRepo _kpiErrorEmployeeFileRepo;
        private readonly UserTeamRepo _userTeamRepo;
        private readonly ConfigSystemRepo _configSystemRepo;
        private readonly KPIErrorRepo _kpiErrorRepo;
        private readonly EmployeeRepo _employeeRepo;
        public KPIErrorEmployeeController(DepartmentRepo departmentRepo, KPIErrorTypeRepo kpiErrorTypeRepo, KPIErrorEmployeeRepo kpiErrorEmployeeRepo, KPIErrorEmployeeFileRepo kpiErrorEmployeeFileRepo, UserTeamRepo userTeamRepo, ConfigSystemRepo configSystemRepo, KPIErrorRepo kpiErrorRepo, EmployeeRepo employeeRepo)
        {
            _departmentRepo = departmentRepo;
            _kpiErrorTypeRepo = kpiErrorTypeRepo;
            _kpiErrorEmployeeRepo = kpiErrorEmployeeRepo;
            _kpiErrorEmployeeFileRepo = kpiErrorEmployeeFileRepo;
            _userTeamRepo = userTeamRepo;
            _configSystemRepo = configSystemRepo;
            _kpiErrorRepo = kpiErrorRepo;
            _employeeRepo = employeeRepo;
        }

        [HttpGet("get-department")]
        public IActionResult GetDepartment()
        {
            try
            {
                var data = _departmentRepo.GetAll(x => x.IsDeleted != true).OrderBy(x => x.STT);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-employees")]
        public IActionResult LoadUser()
        {
            try
            {
                var list = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployee",
                                 new string[] {"@Status" },
                                 new object[] { 0 });
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-kpierror")]
        public IActionResult GetKPIError(int typeID)
        {
            try
            {
                var dataKpiError = SQLHelper<object>.ProcedureToList("spGetKPIError",
                                                new string[] { "TypeID" },
                                                new object[] { typeID });
                var data = SQLHelper<object>.GetListData(dataKpiError, 0);

                return Ok(ApiResponseFactory.Success(data, ""));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-kpierror-by-department")]
        public IActionResult GetKPIErrorByDepartment(int departmentID)
        {
            try
            {
                var dataKpiError = SQLHelper<object>.ProcedureToList("spGetKPIError",
                                                new string[] { "@DepartmentID" },
                                                new object[] { departmentID });
                var data = SQLHelper<object>.GetListData(dataKpiError, 0);

                return Ok(ApiResponseFactory.Success(data, ""));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-kpi-error-type")]
        public IActionResult GetKPIErrorType()
        {
            try
            {
                var data = _kpiErrorTypeRepo.GetAll(x => x.IsDelete != true);

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("load-data")]  
        public IActionResult GetKPIErrorEmployeeData(DateTime startDate, DateTime endDate, int kpiErrorID, int employeeID, int typeID, int departmentID, string keywords = "")
        {
            try
            {
                DateTime dateStartLocal = startDate.ToLocalTime();
                DateTime dateEndLocal = endDate.ToLocalTime();
                var dataKpiError = SQLHelper<object>.ProcedureToList("spGetKPIErrorEmployee",
                                                new string[] { "@StartDate", "@EndDate", "@KPIErrorID", "@EmployeeID", "@Keyword", "@TypeID", "@DepartmentID" },
                                                new object[] { dateStartLocal, dateEndLocal, kpiErrorID, employeeID, keywords, typeID, departmentID });
                var data = SQLHelper<object>.GetListData(dataKpiError, 0);

                return Ok(ApiResponseFactory.Success(data, ""));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("load-data-file")]
        public IActionResult GetDataFile(int kpiErrorEmployeeID)
        {
            try
            {
                var dataFile = _kpiErrorEmployeeFileRepo.GetAll(x => x.KPIErrorEmployeeID == kpiErrorEmployeeID);

                return Ok(ApiResponseFactory.Success(dataFile, ""));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data-employee-error")]
        public IActionResult SaveData([FromBody] KPIErrorEmployee model)
        {
            try
            {
                if (model.EmployeeID <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn nhân viên vi phạm"));
                }
                if (model.KPIErrorID <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn mã lỗi vi phạm"));
                }

                if(model.ID > 0)
                {
                    _kpiErrorEmployeeRepo.Update(model);
                }
                else
                {
                    _kpiErrorEmployeeRepo.Create(model);
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-kpi-error-employee-by-id")]
        public IActionResult GetKPIErrorEmployeeByID(int id)
        {
            try
            {
                var data = _kpiErrorEmployeeRepo.GetByID(id);

                return Ok(ApiResponseFactory.Success(data, ""));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("auto-add")]
        public IActionResult AutoAddKPIError([FromBody] AutoAddKPIErrorRequest request)
        {
            try
            {
                var userTeams = _userTeamRepo.GetAll();
                string teamIds = string.Join(";", userTeams.Select(x => x.ID));

                DateTime dateStart = new DateTime(
                    request.StartDate.Year,
                    request.StartDate.Month,
                    request.StartDate.Day,
                    0, 0, 0
                );

                DateTime dateEndInput = new DateTime(
                    request.EndDate.Year,
                    request.EndDate.Month,
                    request.EndDate.Day,
                    23, 59, 59
                );

                DateTime dateEndNow = new DateTime(
                    DateTime.Now.AddDays(-1).Year,
                    DateTime.Now.AddDays(-1).Month,
                    DateTime.Now.AddDays(-1).Day,
                    23, 59, 59
                );

                DateTime dateEnd = dateEndNow.Date < dateEndInput.Date
                    ? dateEndNow
                    : dateEndInput;

                var dataStore = SQLHelper<object>.ProcedureToList(
                    "spExportToExcelDRT",
                    new string[] { "DateStart", "DateEnd", "TeamID" },
                    new object[] { dateStart, dateEnd, teamIds }
                );

                var data = SQLHelper<object>.GetListData(dataStore, 1);

                List<KPIErrorEmployee> insertList = new();

                foreach (var item in data)
                {
                    var row = item as IDictionary<string, object>;

                    if (row == null) continue;

                    int employeeId = Convert.ToInt32(row["EmployeeID"]);
                    string dayValue = row["DayValue"]?.ToString()?.ToLower() ?? "";
                    DateTime errorDate = Convert.ToDateTime(row["AllDates"]);

                    int kpiErrorId = dayValue == "xm" ? 3 : 1;

                    bool existed = _kpiErrorEmployeeRepo.GetAll(x =>
                        x.KPIErrorID == kpiErrorId &&
                        x.EmployeeID == employeeId &&
                        x.ErrorDate.HasValue &&
                        x.ErrorDate.Value.Year == errorDate.Year &&
                        x.ErrorDate.Value.Month == errorDate.Month &&
                        x.ErrorDate.Value.Day == errorDate.Day &&
                        x.IsDelete != true
                    ).Any();

                    if (existed) continue;

                    insertList.Add(new KPIErrorEmployee
                    {
                        KPIErrorID = kpiErrorId,
                        EmployeeID = employeeId,
                        ErrorDate = errorDate,
                        ErrorNumber = 1,
                        Note = "",
                        TotalMoney = 10000,
                        IsDelete = false,
                        CreatedDate = DateTime.Now
                    });
                }

                if (insertList.Any())
                {
                    _kpiErrorEmployeeRepo.CreateRange(insertList);
                }

                return Ok(ApiResponseFactory.Success(new
                {
                    Inserted = insertList.Count
                }, "Cập nhật thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete-kpi-error-employee")]
        public IActionResult DeleteKPIErrorEmployee([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách ID không hợp lệ"));
                }

                foreach (var id in ids)
                {
                    var entity = _kpiErrorEmployeeRepo.GetByID(id);
                    if (entity == null) continue;

                    entity.IsDelete = true;
                    _kpiErrorEmployeeRepo.Update(entity);
                }

                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("upload")]
        [DisableRequestSizeLimit]
        //[RequiresPermission("N27,N36,N1,N31")]

        public async Task<IActionResult> Upload(int kpiErrorEmployeeId, int fileType)
        {
            try
            {
                var form = await Request.ReadFormAsync();
                var key = form["key"].ToString();
                var files = form.Files;

                // Kiểm tra input
                if (string.IsNullOrWhiteSpace(key))
                    return BadRequest(ApiResponseFactory.Fail(null, "Key không được để trống!"));

                if (files == null || files.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách file không được để trống!"));

                var ke = _kpiErrorEmployeeRepo.GetByID(kpiErrorEmployeeId);
                if (ke == null)
                    throw new Exception("kpiErrorEmployee not found");

                var uploadPath = _configSystemRepo.GetUploadPathByKey(key);
                if (string.IsNullOrWhiteSpace(uploadPath))
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy cấu hình đường dẫn cho key: {key}"));

                var subPathRaw = form["subPath"].ToString()?.Trim() ?? "";
                string targetFolder = uploadPath;
                if (!string.IsNullOrWhiteSpace(subPathRaw))
                {
                    var separator = Path.DirectorySeparatorChar;
                    var segments = subPathRaw
                        .Replace('/', separator)
                        .Replace('\\', separator)
                        .Split(separator, StringSplitOptions.RemoveEmptyEntries)
                        .Select(seg =>
                        {
                            var invalidChars = Path.GetInvalidFileNameChars();
                            var cleaned = new string(seg.Where(c => !invalidChars.Contains(c)).ToArray());
                            cleaned = cleaned.Replace("..", "").Trim();
                            return cleaned;
                        })
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToArray();

                    if (segments.Length > 0)
                        targetFolder = Path.Combine(uploadPath, Path.Combine(segments));
                }
                else
                {
                    targetFolder = Path.Combine(uploadPath, $"NB{ke.ID}");
                }

                if (!Directory.Exists(targetFolder))
                    Directory.CreateDirectory(targetFolder);

                var processedFile = new List<KPIErrorEmployeeFile>();

                foreach (var file in files)
                {
                    if (file.Length <= 0) continue;

                    // Tạo tên file unique để tránh trùng lặp
                    var fileExtension = Path.GetExtension(file.FileName);
                    var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                    var uniqueFileName = $"{originalFileName}{fileExtension}";
                    var fullPath = Path.Combine(targetFolder, uniqueFileName);

                    // Lưu file trực tiếp vào targetFolder (không tạo file tạm khác)
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var filePO = new KPIErrorEmployeeFile
                    {
                        KPIErrorEmployeeID = ke.ID,
                        FileName = uniqueFileName,
                        OriginPath = targetFolder,
                        ServerPath = targetFolder,
                        CreatedBy = User.Identity?.Name ?? "System",
                        CreatedDate = DateTime.Now,
                        UpdatedBy = User.Identity?.Name ?? "System",
                        UpdatedDate = DateTime.Now
                    };

                    await _kpiErrorEmployeeFileRepo.CreateAsync(filePO);
                    processedFile.Add(filePO);
                }

                return Ok(ApiResponseFactory.Success(processedFile, $"{processedFile.Count} tệp đã được tải lên thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi upload file: {ex.Message}"));
            }
        }



        [HttpPost("delete-file")]
        public IActionResult DeleteFile([FromBody] List<int> fileIds)
        {
            if (fileIds == null || !fileIds.Any())
                throw new Exception("Danh sách file ID không được trống");

            try
            {
                var results = new List<object>();

                foreach (var fileId in fileIds)
                {
                    var file = _kpiErrorEmployeeFileRepo.GetByID(fileId);

                    // Cập nhật database
                    //file.IsDeleted = true;
                    //file.UpdatedBy = User.Identity?.Name ?? "System";
                    //file.UpdatedDate = DateTime.UtcNow;
                    //_kpiErrorEmployeeFileRepo.Update(file);
                    _kpiErrorEmployeeFileRepo.Delete(fileId);

                    // Xóa file vật lý
                    var physicalPath = Path.Combine(file.ServerPath, file.FileName);
                    if (System.IO.File.Exists(physicalPath))
                        System.IO.File.Delete(physicalPath);

                    results.Add(new { fileId, success = true, message = "Xóa thành công" });
                }

                return Ok(ApiResponseFactory.Success(results, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportExcel([FromBody] List<Dictionary<string, object>> data)
        {
            try
            {
                int created = 0;
                int skipped = 0;
                var errors = new List<string>();

                var employees = _employeeRepo.GetAll().ToList();
                var kpiErrors = _kpiErrorRepo.GetAll().ToList();

                int rowNumber = 1;

                foreach (var row in data)
                {
                    rowNumber++;
                    try
                    {
                        int stt = GetInt(row, "F1");
                        if (stt <= 0)
                        {
                            skipped++;
                            continue;
                        }

                        string employeeCode = GetString(row, "F2").Trim();

                        string kpiErrorCode = GetString(row, "F4").Trim();

                        DateTime? errorDate = ParseDate(GetString(row, "F5"));

                        int errorNumber = GetInt(row, "F6");

                        decimal totalMoney = GetDecimal(row, "F7");

                        string note = GetString(row, "F8");

                        var employee = employees.FirstOrDefault(e => e.Code == employeeCode);
                        var kpiError = kpiErrors.FirstOrDefault(k => k.Code == kpiErrorCode);

                        if (employee == null || kpiError == null || !errorDate.HasValue)
                        {
                            skipped++;
                            continue;
                        }

                        var model = new KPIErrorEmployee
                        {
                            STT = 0,
                            EmployeeID = employee.ID,
                            KPIErrorID = kpiError.ID,
                            ErrorDate = errorDate,
                            ErrorNumber = errorNumber,
                            TotalMoney = totalMoney,
                            Note = note
                        };

                        //SQLHelper<KPIErrorEmployeeModel>.Insert(model);
                        await _kpiErrorEmployeeRepo.CreateAsync(model);
                        created++;
                    }
                    catch (Exception ex)
                    {
                        skipped++;
                        errors.Add($"Dòng {rowNumber}: {ex.Message}");
                    }
                }

                return Ok(ApiResponseFactory.Success(
                    new { created, skipped, errors },
                    $"Import hoàn tất: Tạo {created}, Bỏ qua {skipped}"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        public class AutoAddKPIErrorRequest
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        private string GetString(Dictionary<string, object> row, string key)
        {
            return row.ContainsKey(key) ? row[key]?.ToString() ?? "" : "";
        }

        private int GetInt(Dictionary<string, object> row, string key)
        {
            int.TryParse(GetString(row, key), out int result);
            return result;
        }

        private decimal GetDecimal(Dictionary<string, object> row, string key)
        {
            decimal.TryParse(GetString(row, key), out decimal result);
            return result;
        }

        private DateTime? ParseDate(string value)
        {
            if (DateTime.TryParse(value, out var date))
                return date;
            return null;
        }

    }
}
