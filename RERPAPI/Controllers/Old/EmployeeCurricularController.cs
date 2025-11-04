using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using OfficeOpenXml;
using System.Data;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeCurricularController : ControllerBase
    {
        private readonly EmployeeCurricularRepo _employeeCurricularRepo;
        private readonly EmployeeRepo _employeeRepo;

        public EmployeeCurricularController()
        {
            _employeeCurricularRepo = new EmployeeCurricularRepo();
            _employeeRepo = new EmployeeRepo();
        }

        [HttpGet("")]
        public IActionResult GetEmployeeCurricular(int month = 0, int year = 0, int departmentId = 0, int employeeId = 0)
        {
            try
            {
                // Sử dụng SQLHelper cho stored procedure như trong projectSumary.txt
                var result = SQLHelper<object>.ProcedureToList("spGetEmployeeCurricular",
                    new string[] { "@Month", "@Year", "@DepartmentID", "@EmployeeID" },
                    new object[] { month == 0 ? DateTime.Now.Month : month, 
                                  year == 0 ? DateTime.Now.Year : year, 
                                  departmentId, employeeId });

                var data = SQLHelper<object>.GetListData(result, 0);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveEmployeeCurricular([FromBody] EmployeeCurricularDTO model)
        {
            try
            {
                if (model.ID > 0)
                {
                    // Update existing record
                    var existing = _employeeCurricularRepo.GetByID(model.ID);
                    if (existing != null)
                    {
                        existing.CurricularCode = model.CurricularCode;
                        existing.CurricularName = model.CurricularName;
                        existing.CurricularDay = model.CurricularDay;
                        existing.CurricularMonth = model.CurricularMonth;
                        existing.CurricularYear = model.CurricularYear;
                        existing.EmployeeID = model.EmployeeID;
                        existing.Note = model.Note;
                        existing.UpdatedBy = model.UpdatedBy;
                        existing.UpdatedDate = DateTime.Now;
                        
                        await _employeeCurricularRepo.UpdateAsync(existing);
                    }
                }
                else
                {
                    // Check if record already exists
                    var existingRecord = _employeeCurricularRepo.GetAll()
                        .FirstOrDefault(x => x.EmployeeID == model.EmployeeID &&
                                           x.CurricularDay == model.CurricularDay &&
                                           x.CurricularMonth == model.CurricularMonth &&
                                           x.CurricularYear == model.CurricularYear);

                    if (existingRecord != null)
                    {
                        // Update existing record
                        existingRecord.CurricularCode = model.CurricularCode;
                        existingRecord.CurricularName = model.CurricularName;
                        existingRecord.Note = model.Note;
                        existingRecord.UpdatedBy = model.UpdatedBy;
                        existingRecord.UpdatedDate = DateTime.Now;
                        
                        await _employeeCurricularRepo.UpdateAsync(existingRecord);
                    }
                    else
                    {
                        // Create new record
                        var newRecord = new EmployeeCurricular
                        {
                            CurricularCode = model.CurricularCode,
                            CurricularName = model.CurricularName,
                            CurricularDay = model.CurricularDay,
                            CurricularMonth = model.CurricularMonth,
                            CurricularYear = model.CurricularYear,
                            EmployeeID = model.EmployeeID,
                            Note = model.Note,
                            CreatedBy = model.CreatedBy,
                            CreatedDate = DateTime.Now
                        };
                        
                        await _employeeCurricularRepo.CreateAsync(newRecord);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("check")]
        public IActionResult CheckEmployeeCurricular(int employeeId, int curricularDay, int curricularMonth, int curricularYear)
        {
            try
            {
                // Sử dụng SQLHelper.FindByExpression như trong projectSumary.txt
                var employee = SQLHelper<EmployeeCurricular>.FindByAttribute("Code", employeeId.ToString()).FirstOrDefault();
                if (employee != null)
                {
                    var exists = _employeeCurricularRepo.GetAll()
                        .Any(x => x.EmployeeID == employeeId && 
                                 x.CurricularDay == curricularDay &&
                                 x.CurricularMonth == curricularMonth &&
                                 x.CurricularYear == curricularYear);

                    return Ok(ApiResponseFactory.Success(new { exists }, exists ? "Dữ liệu đã tồn tại!" : "Dữ liệu chưa tồn tại!"));
                }
                
                return Ok(ApiResponseFactory.Success(new { exists = false }, "Nhân viên không tồn tại!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportFromExcel(IFormFile file, string sheetName = "")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn file Excel!"));
                }

                var importResults = new List<object>();
                var errorList = new List<string>();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet;
                        
                        if (string.IsNullOrEmpty(sheetName))
                        {
                            worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        }
                        else
                        {
                            worksheet = package.Workbook.Worksheets[sheetName];
                        }

                        if (worksheet == null)
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy sheet dữ liệu!"));
                        }

                        var rowCount = worksheet.Dimension?.Rows ?? 0;
                        
                        // Assuming first row is header, start from row 2
                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                // Read data from Excel columns
                                var employeeCode = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                                var curricularCode = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                                var curricularName = worksheet.Cells[row, 3].Value?.ToString()?.Trim();
                                var curricularDateStr = worksheet.Cells[row, 4].Value?.ToString()?.Trim();

                                if (string.IsNullOrEmpty(employeeCode) || string.IsNullOrEmpty(curricularDateStr))
                                {
                                    continue; // Skip empty rows
                                }

                                // Find employee by code
                                var employee = _employeeRepo.GetAll()
                                    .FirstOrDefault(x => x.Code?.Trim().ToUpper() == employeeCode.ToUpper());

                                if (employee == null)
                                {
                                    errorList.Add($"Dòng {row}: Không tìm thấy nhân viên với mã '{employeeCode}'");
                                    continue;
                                }

                                // Parse date
                                if (!DateTime.TryParse(curricularDateStr, out DateTime curricularDate))
                                {
                                    errorList.Add($"Dòng {row}: Định dạng ngày không hợp lệ '{curricularDateStr}'");
                                    continue;
                                }

                                // Check if record already exists
                                var existingRecord = _employeeCurricularRepo.GetAll()
                                    .FirstOrDefault(x => x.EmployeeID == employee.ID &&
                                                       x.CurricularDay == curricularDate.Day &&
                                                       x.CurricularMonth == curricularDate.Month &&
                                                       x.CurricularYear == curricularDate.Year);

                                if (existingRecord != null)
                                {
                                    // Update existing record
                                    existingRecord.CurricularCode = curricularCode;
                                    existingRecord.CurricularName = curricularName;
                                    existingRecord.UpdatedDate = DateTime.Now;
                                    existingRecord.UpdatedBy = "Excel Import";
                                    
                                    await _employeeCurricularRepo.UpdateAsync(existingRecord);
                                    importResults.Add(new { Row = row, Action = "Updated", EmployeeCode = employeeCode });
                                }
                                else
                                {
                                    // Create new record
                                    var newRecord = new EmployeeCurricular
                                    {
                                        EmployeeID = employee.ID,
                                        CurricularCode = curricularCode,
                                        CurricularName = curricularName,
                                        CurricularDay = curricularDate.Day,
                                        CurricularMonth = curricularDate.Month,
                                        CurricularYear = curricularDate.Year,
                                        CreatedDate = DateTime.Now,
                                        CreatedBy = "Excel Import"
                                    };
                                    
                                    await _employeeCurricularRepo.CreateAsync(newRecord);
                                    importResults.Add(new { Row = row, Action = "Created", EmployeeCode = employeeCode });
                                }
                            }
                            catch (Exception ex)
                            {
                                errorList.Add($"Dòng {row}: {ex.Message}");
                            }
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(new 
                { 
                    ImportedCount = importResults.Count,
                    ErrorCount = errorList.Count,
                    Results = importResults,
                    Errors = errorList
                }, "Import hoàn thành!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-excel-sheets")]
        public async Task<IActionResult> GetExcelSheets(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn file Excel!"));
                }

                var sheetNames = new List<string>();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        foreach (var worksheet in package.Workbook.Worksheets)
                        {
                            sheetNames.Add(worksheet.Name);
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(sheetNames, "Lấy danh sách sheet thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeCurricular(int id)
        {
            try
            {
                var record = _employeeCurricularRepo.GetByID(id);
                if (record == null)
                {
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy dữ liệu!"));
                }

                await _employeeCurricularRepo.DeleteAsync(id);
                return Ok(ApiResponseFactory.Success(null, "Xóa dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}