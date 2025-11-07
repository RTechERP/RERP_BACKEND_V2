using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using System.Data;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace RERPAPI.Controllers.KhoBaseManager
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowProjectBaseController : ControllerBase
    {
        private readonly ProjectRepo _projectRepo = new ProjectRepo();
        private readonly UserRepo _userRepo = new UserRepo();
        private readonly CustomerRepo _customerRepo = new CustomerRepo();
        private readonly FollowProjectBaseRepo _followProjectBaseRepo = new FollowProjectBaseRepo();
        private readonly ProjectStatusLogRepo _projectStatusLogRepo = new ProjectStatusLogRepo();
        private readonly FirmBaseRepo _firmBaseRepo = new FirmBaseRepo();
        private readonly ProjectTypeBaseRepo _projectTypeBaseRepo = new ProjectTypeBaseRepo();
        private readonly FollowProjectRepo _followProjectRepo = new FollowProjectRepo();
        private readonly ProjectStatusRepo _projectStatusRepo = new ProjectStatusRepo();
        // Danh sách follow project base
        [HttpGet("getfollowprojectbase")]
        public async Task<IActionResult> getfollowprojectbase([FromQuery] FollowProjectBaseFilterParam param)
        {
            try
            {
            var data = SQLHelper<object>.ProcedureToList(
            "spGetFollowProjectBase",
                new string[]
                {
                    "@PageNumber", "@PageSize", "@DateStart", "@DateEnd",
                    "@FilterText", "@User", "@CustomerID", "@PM",
                    "@WarehouseID", "@GroupSaleID"
                },
                new object[]
                {
                    param.page, param.size, param.dateStart, param.dateEnd,
                    param.filterText, param.user, param.customerID, param.pm,
                    param.warehouseID, param.groupSaleID
                }
            );
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(data, 0),
                    totalPage = SQLHelper<object>.GetListData(data, 0).Select(c => c?.TotalPage).Where(x => x != null).Distinct().FirstOrDefault() ?? 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Danh sách group sale suser 
        [HttpGet("getfollowprojectbasedetail")]
        public async Task<IActionResult> getfollowprojectbasedetail(int followProjectBaseID, int projectID)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList(
            "spGetFollowProjectBaseDetail",
            new string[]
            {
                "@FollowProjectBaseID", "@ProjectID"
            },
            new object[]
            {
                followProjectBaseID, projectID
            }
        );
                return Ok(new
                {
                    status = 1,
                    dataSale = SQLHelper<object>.GetListData(data, 0),
                    dataPM = SQLHelper<object>.GetListData(data, 1),
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // check exist firmbase
        [HttpGet("getcheckexistfirmbase")]
        public async Task<IActionResult> getcheckexistfirmbase(string firmBaseCode)
        {
            try
            {
                List<FirmBase> lstFirmBase = _firmBaseRepo.GetAll(c => c.FirmCode.ToLower() == firmBaseCode.ToLower());
                bool isExist = lstFirmBase.Count == 0 ? false : true;
                return Ok(new
                {
                    status = 1,
                    isExist = isExist
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // check exist project type base
        [HttpGet("getcheckexistprojecttypebase")]
        public async Task<IActionResult> getcheckexistprojecttypebase(string projectTypeBaseCode)
        {
            try
            {
                List<ProjectTypeBase> lstProjectTypeBase = _projectTypeBaseRepo.GetAll(c => c.ProjectTypeCode.ToLower() == projectTypeBaseCode.ToLower());
                bool isExist = lstProjectTypeBase.Count == 0 ? false : true;
                return Ok(new
                {
                    status = 1,
                    isExist = isExist
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Danh sách group sale suser 
        [HttpGet("getgroupsalesuser")]
        public async Task<IActionResult> getgroupsalesuser(int groupID, int teamID)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList(
            "spGetEmployeeManager",
            new string[]
            {
                "@group", "@teamID"
            },
            new object[]
            {
                groupID, teamID
            }
        );
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(data, 0),
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Danh sách PM 
        [HttpGet("getpm")]
        public async Task<IActionResult> getpm()
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetEmployeeForProject",
                                                     new string[] { },
                                                     new object[] { });
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(data, 0),
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Danh sách projects 
        [HttpGet("getprojects")]
        public async Task<IActionResult> getprojects()
        {
            try
            {
                //var data = SQLHelper<object>.Select("SELECT *,ProjectCode +'_'+ProjectName as ProjectFullName FROM Project");
                var data = _projectRepo.GetAll()
                    .Select(p => new
                    {
                        p.ID,
                        p.ProjectCode,
                        p.ProjectName,
                        ProjectFullName = p.ProjectCode + "_" + p.ProjectName
                    }).ToList();
                return Ok(ApiResponseFactory.Success( data , ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Danh sách user 
        [HttpGet("getusers")]
        public async Task<IActionResult> getusers()
        {
            try
            {
                //var data = SQLHelper<object>.Select("SELECT * FROM Users");
                var data = _userRepo.GetAll();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Danh sách customer 
        [HttpGet("getcustomers")]
        public async Task<IActionResult> getcustomers()
        {
            try
            {
                //var data = SQLHelper<object>.Select("SELECT * FROM Customer WHERE IsDeleted <> 1");
                var data = _customerRepo.GetAll(c => c.IsDeleted != true);
                return Ok(new
                {
                    status = 1,
                    data = data,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Danh sách getprojectstatus 
        [HttpGet("getprojectstatus")]
        public async Task<IActionResult> getprojectstatus()
        {
            try
            {
                //var data = SQLHelper<object>.Select("SELECT * FROM ProjectStatus ORDER BY STT ASC");
                var data = _projectStatusRepo.GetAll().OrderBy(c => c.STT).ToList();
                return Ok(new
                {
                    status = 1,
                    data = data,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Danh sách getprojectstatus 
        [HttpGet("getprojectbyid")]
        public async Task<IActionResult> getprojectbyid(int id)
        {
            try
            {
                var data = _projectRepo.GetByID(id);
                return Ok(new
                {
                    status = 1,
                    data = data,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Danh sách FirmBase 
        [HttpGet("getfirmbase")]
        public async Task<IActionResult> getfirmbase()
        {
            try
            {
                //var data = SQLHelper<object>.Select("SELECT * FROM FirmBase");
                var data = _firmBaseRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = data,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Danh sách ProjectTypeBase 
        [HttpGet("getprojecttypebase")]
        public async Task<IActionResult> getprojecttypebase()
        {
            try
            {
                //var data = SQLHelper<object>.Select("SELECT * FROM ProjectTypeBase");
                var data = _projectTypeBaseRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = data,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Danh sách ProjectTypeBase 
        [HttpGet("getupdateproject")]
        public async Task<IActionResult> getupdateproject(int ProjectStatusBaseID, int ProjectID, string LoginName)
        {
            try
            {
                //var data = SQLHelper<object>.ExcuteScalar($"UPDATE Project SET ProjectStatus = {ProjectStatusBaseID},UpdatedBy = '{LoginName}',UpdatedDate = '{DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss")}' WHERE ID = {ProjectID}");
                var project = new RERPAPI.Model.Entities.Project
                {
                    ID = ProjectID,
                    ProjectStatus = ProjectStatusBaseID,
                    UpdatedBy = LoginName,
                    UpdatedDate = DateTime.Now
                };

                var data = await _projectRepo.UpdateAsync(project);

                return Ok(new
                {
                    status = 1,
                    data = data,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Danh sách khách hàng 
        [HttpGet("getcustomerbase")]
        public async Task<IActionResult> getcustomerbase()
        {
            try
            {
                //var data = SQLHelper<object>.Select("SELECT * FROM Customer");
                var data = _customerRepo.GetAll().Where(x => x.IsDeleted != true);
                return Ok(new
                {
                    status = 1,
                    data = data,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Danh sách employee 
        [HttpGet("getemployee")]
        public async Task<IActionResult> getemployee(int status)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetEmployee",
                                                    new string[] { "@Status" },
                                                    new object[] { status });
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(data, 0)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("savefollowprojectbase")]
        public async Task<IActionResult> savefollowprojectbase([FromBody] FollowProjectBase obj)
        {
            try
            {
                if (obj.ID <= 0)
                {
                    await _followProjectBaseRepo.CreateAsync(obj);
                }
                else
                {
                    _followProjectBaseRepo.Update(obj);
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
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }
        [HttpPost("saveprojectstatuslog")]
        public async Task<IActionResult> saveprojectstatuslog([FromBody] ProjectStatusLog obj)
        {
            try
            {
                if (obj.ID <= 0)
                {
                    await _projectStatusLogRepo.CreateAsync(obj);
                }
                else
                {
                    _projectStatusLogRepo.Update(obj);
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
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }
        // lưu firm base
        [HttpPost("savefirmbase")]
        public async Task<IActionResult> savefirmbase([FromBody] FirmBase obj)
        {
            try
            {
                if (obj.ID <= 0)
                {
                    await _firmBaseRepo.CreateAsync(obj);
                }
                else
                {
                    _firmBaseRepo.Update(obj);
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
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }
        // lưu loại dự án base
        [HttpPost("saveprojecttypebase")]
        public async Task<IActionResult> saveprojecttypebase([FromBody] ProjectTypeBase obj)
        {
            try
            {
                if (obj.ID <= 0)
                {
                    await _projectTypeBaseRepo.CreateAsync(obj);
                }
                else
                {
                    _projectTypeBaseRepo.Update(obj);
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
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

        [HttpGet("exportfollowprojectbase")]
        public IActionResult ExportFollowProjectBase(
                                                    int followProjectBaseID = 0,
                                                    int projectID = 0,
                                                    int userID = 0,
                                                    int customerID = 0,
                                                    int pm = 0,
                                                    int warehouseID = 1,
                                                    string filterText = "",
                                                    string fileNameElement = "")
        {
            try
            {
                // 1. Lấy dữ liệu từ stored procedure với SQLHelper
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetFollowProjectBaseExport",
                    new string[] { "@FollowProjectBaseID", "@ProjectID", "@UserID", "@CustomerID", "@PM", "@WarehouseID", "@FilterText" },
                    new object[] { followProjectBaseID, projectID, userID, customerID, pm, warehouseID, filterText });

                // Lấy result set đầu tiên (dữ liệu chính)
                var data = SQLHelper<dynamic>.GetListData(list, 0);

                // Nếu cần result set thứ 2 (ví dụ: data Sale/PM)
                // var data1 = SQLHelper<dynamic>.GetListData(list, 1);

                if (data == null || data.Count == 0)
                {
                    return BadRequest(new { message = "Không có dữ liệu để xuất!" });
                }

                // 2. Tạo workbook với ClosedXML
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Follow Dự Án");

                    // 3. Thêm header
                    var headers = new string[]
                    {
                        "Mã dự án", "Tên dự án", "Sale phụ trách", "PM", "Đối tác(KH)", "End User",
                        "Trạng thái", "Ngày bắt đầu", "Loại dự án", "Hãng",
                        "Khả năng có PO",
                        "Dự Kiến: Ngày lên PA", "Dự Kiến: Ngày báo giá", "Dự Kiến: Ngày PO", "Dự Kiến: Ngày kết thúc",
                        "Thực tế: Ngày lên PA", "Thực tế: Ngày báo giá", "Thực tế: Ngày PO", "Thực tế: Ngày kết thúc",
                        "Tổng báo giá chưa VAT", "Người phụ trách chính", "Ghi chú",
                        "Sale: Ngày đã làm", "Sale: Việc đã làm", "Sale: Ngày sẽ làm", "Sale: Việc sẽ làm",
                        "PM: Ngày đã làm", "PM: Việc đã làm", "PM: Ngày sẽ làm", "PM: Việc sẽ làm"
                    };

                    // Style cho header
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = worksheet.Cell(1, i + 1);
                        cell.Value = headers[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell.Style.Alignment.WrapText = true;

                    }
                    worksheet.Row(1).AdjustToContents();

                    // 4. Thêm dữ liệu - QUAN TRỌNG: Dùng dynamic thay vì DataRow
                    int row = 2;
                    foreach (dynamic item in data)
                    {
                        worksheet.Cell(row, 1).Value = GetDynamicValue(item, "ProjectCode");
                        worksheet.Cell(row, 2).Value = GetDynamicValue(item, "ProjectName");
                        worksheet.Cell(row, 3).Value = GetDynamicValue(item, "FullName");
                        worksheet.Cell(row, 4).Value = GetDynamicValue(item, "ProjectManager");
                        worksheet.Cell(row, 5).Value = GetDynamicValue(item, "CustomerName");
                        worksheet.Cell(row, 6).Value = GetDynamicValue(item, "EndUser");
                        worksheet.Cell(row, 7).Value = GetDynamicValue(item, "ProjectStatusName");

                        // Ngày bắt đầu
                        SetDateCellDynamic(worksheet, row, 8, item, "ProjectStartDate");

                        worksheet.Cell(row, 9).Value = GetDynamicValue(item, "ProjectTypeName");
                        worksheet.Cell(row, 10).Value = GetDynamicValue(item, "FirmName");
                        worksheet.Cell(row, 11).Value = GetDynamicValue(item, "PossibilityPO");

                        // Dự kiến (Expected)
                        SetDateCellDynamic(worksheet, row, 12, item, "ExpectedPlanDate");
                        SetDateCellDynamic(worksheet, row, 13, item, "ExpectedQuotationDate");
                        SetDateCellDynamic(worksheet, row, 14, item, "ExpectedPODate");
                        SetDateCellDynamic(worksheet, row, 15, item, "ExpectedProjectEndDate");

                        // Thực tế (Reality)
                        SetDateCellDynamic(worksheet, row, 16, item, "RealityPlanDate");
                        SetDateCellDynamic(worksheet, row, 17, item, "RealityQuotationDate");
                        SetDateCellDynamic(worksheet, row, 18, item, "RealityPODate");
                        SetDateCellDynamic(worksheet, row, 19, item, "RealityProjectEndDate");

                        // Follow dự án
                        SetNumericCellDynamic(worksheet, row, 20, item, "TotalWithoutVAT");
                        worksheet.Cell(row, 21).Value = GetDynamicValue(item, "ProjectContactName");
                        worksheet.Cell(row, 22).Value = GetDynamicValue(item, "Note");
                        worksheet.Cell(row, 22).Style.Alignment.WrapText = true;

                        // Sale báo cáo
                        SetDateCellDynamic(worksheet, row, 23, item, "ImplementationDateSale");
                        worksheet.Cell(row, 24).Value = GetDynamicValue(item, "WorkDoneSale");
                        worksheet.Cell(row, 24).Style.Alignment.WrapText = true;
                        SetDateCellDynamic(worksheet, row, 25, item, "ExpectedDateSale");
                        worksheet.Cell(row, 26).Value = GetDynamicValue(item, "WorkWillDoSale");
                        worksheet.Cell(row, 26).Style.Alignment.WrapText = true;

                        // PM báo cáo
                        SetDateCellDynamic(worksheet, row, 27, item, "ImplementationDatePM");
                        worksheet.Cell(row, 28).Value = GetDynamicValue(item, "WorkDonePM");
                        worksheet.Cell(row, 28).Style.Alignment.WrapText = true;
                        SetDateCellDynamic(worksheet, row, 29, item, "ExpectedDatePM");
                        worksheet.Cell(row, 30).Value = GetDynamicValue(item, "WorkWillDoPM");
                        worksheet.Cell(row, 30).Style.Alignment.WrapText = true;

                        row++;
                    }

                    // === GROUP VÀ MERGE THEO MÃ DỰ ÁN ===

                    // Các cột cần merge (theo thứ tự cột bạn tạo header)
                    int[] mergeCols = { 1, 2, 11, 20, 21, 22 };

                    // Nhóm dữ liệu theo mã dự án
                    var groups = data
                        .GroupBy(x => GetDynamicValue(x, "ProjectCode"))
                        .ToList();

                    // Dòng bắt đầu dữ liệu (bỏ qua header)
                    int currentRow = 2;

                    foreach (var group in groups)
                    {
                        int groupStart = currentRow;
                        int groupCount = group.Count();
                        int groupEnd = groupStart + groupCount - 1;

                        // Với mỗi cột cần merge
                        foreach (int colIndex in mergeCols)
                        {
                            var range = worksheet.Range(groupStart, colIndex, groupEnd, colIndex);
                            if (groupCount > 1)
                            {
                                range.Merge();
                                range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            }
                        }

                        // Di chuyển row pointer sang nhóm kế tiếp
                        currentRow += groupCount;
                    }


                    // 5. Format columns
                    worksheet.Columns().AdjustToContents();
                    foreach (var column in worksheet.Columns())
                    {
                        if (column.Width > 50) column.Width = 50;
                        if (column.Width < 10) column.Width = 10;
                    }

                    // 6. Freeze header row
                    worksheet.SheetView.FreezeRows(1);

                    // 7. Add AutoFilter
                    worksheet.RangeUsed().SetAutoFilter();

                    // 8. Tạo tên file
                    string fileName = $"FollowProject_{fileNameElement}_{DateTime.Now:ddMMyy}.xlsx";

                    // 9. Save to stream và return
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Position = 0;

                        return File(stream.ToArray(),
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi xuất Excel: " + ex.Message, stack = ex.StackTrace });
            }
        }

        // ===== HELPER METHODS - Thêm vào controller =====

        /// <summary>
        /// Lấy giá trị từ dynamic object
        /// </summary>
        private string GetDynamicValue(dynamic item, string propertyName)
        {
            try
            {
                // Thử truy cập như Dictionary
                if (item is IDictionary<string, object> dict)
                {
                    return dict.ContainsKey(propertyName) ? dict[propertyName]?.ToString() ?? "" : "";
                }

                // Thử reflection cho dynamic object
                var type = item.GetType();
                var property = type.GetProperty(propertyName);
                if (property != null)
                {
                    return property.GetValue(item)?.ToString() ?? "";
                }

                return "";
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Set date cell từ dynamic object
        /// </summary>
        private void SetDateCellDynamic(IXLWorksheet worksheet, int row, int col, dynamic item, string propertyName)
        {
            try
            {
                object value = null;

                if (item is IDictionary<string, object> dict)
                {
                    value = dict.ContainsKey(propertyName) ? dict[propertyName] : null;
                }
                else
                {
                    var type = item.GetType();
                    var property = type.GetProperty(propertyName);
                    value = property?.GetValue(item);
                }

                if (value != null && value != DBNull.Value)
                {
                    if (value is DateTime date)
                    {
                        var cell = worksheet.Cell(row, col);
                        cell.Value = date;
                        cell.Style.DateFormat.Format = "dd/mm/yyyy";
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }
                    else if (DateTime.TryParse(value.ToString(), out DateTime parsedDate))
                    {
                        var cell = worksheet.Cell(row, col);
                        cell.Value = parsedDate;
                        cell.Style.DateFormat.Format = "dd/mm/yyyy";
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }
                }
            }
            catch
            {
                // Ignore error
            }
        }

        /// <summary>
        /// Set numeric cell từ dynamic object
        /// </summary>
        private void SetNumericCellDynamic(IXLWorksheet worksheet, int row, int col, dynamic item, string propertyName)
        {
            try
            {
                object value = null;

                if (item is IDictionary<string, object> dict)
                {
                    value = dict.ContainsKey(propertyName) ? dict[propertyName] : null;
                }
                else
                {
                    var type = item.GetType();
                    var property = type.GetProperty(propertyName);
                    value = property?.GetValue(item);
                }

                if (value != null && value != DBNull.Value)
                {
                    if (decimal.TryParse(value.ToString(), out decimal number))
                    {
                        var cell = worksheet.Cell(row, col);
                        cell.Value = number;
                        cell.Style.NumberFormat.Format = "#,##0";
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    }
                }
            }
            catch
            {
                // Ignore error
            }
        }


        [HttpPost("importexcel")]
        public IActionResult ImportExcel([FromBody] List<Dictionary<string, object>> projects)
        {
            if (projects == null || projects.Count == 0)
                return BadRequest(new { status = 0, message = "Payload rỗng." });


            int created = 0, updated = 0;
            var errors = new List<object>();
            foreach (var row in projects)
            {
                try
                {
                    FollowProjectBase followProjectBase = new FollowProjectBase();
                    // Lấy giá trị theo key
                    string ProjectCode = row.GetString("Mã dự án");
                    string ProjectName = row.GetString("Tên dự án");
                    string FullName = row.GetString("Sale phụ trách");
                    //string ProjectManager = row.GetString("PM");
                    string CustomerName = row.GetString("Đối tác(KH)");
                    string EndUser = row.GetString("End User");
                    string ProjectStatusName = row.GetString("Trạng thái");
                    DateTime? ProjectStartDate = row.GetNullableDate("Ngày bắt đầu");
                    string ProjectTypeName = row.GetString("Loại dự án");
                    string FirmName = row.GetString("Hãng");
                    string PossibilityPO = row.GetString("Khả năng có PO");
                    DateTime? ExpectedPlanDate = row.GetNullableDate("Ngày lên phương án");
                    DateTime? ExpectedQuotationDate = row.GetNullableDate("Ngày báo giá");
                    DateTime? ExpectedPODate = row.GetNullableDate("Ngày PO");
                    DateTime? ExpectedProjectEndDate = row.GetNullableDate("Ngày kết thúc dự án");
                    DateTime? RealityPlanDate = row.GetNullableDate("Ngày lên phương án_1");
                    DateTime? RealityQuotationDate = row.GetNullableDate("Ngày báo giá_1");
                    DateTime? RealityPODate = row.GetNullableDate("Ngày PO_1");
                    DateTime? RealityProjectEndDate = row.GetNullableDate("Ngày kết thúc dự án_1");
                    string? TotalWithoutVAT = row.GetString("Tổng báo giá chưa VAT");
                    string ProjectContactName = row.GetString("Người phụ trách chính");
                    string Note = row.GetString("Ghi chú");

                    if (string.IsNullOrWhiteSpace(ProjectCode) || string.IsNullOrWhiteSpace(ProjectName))
                        throw new Exception("Thiếu 'Mã dự án' hoặc 'Tên dự án'.");

                    List<RERPAPI.Model.Entities.Project> project = _projectRepo.GetAll(c => c.ProjectName == ProjectName && c.ProjectCode == ProjectCode);
                    if (project.Count > 0)
                    {
                        //int projectID = getIDFromDb("Project", "ProjectName", ProjectName, 0);
                        int projectID = _projectRepo.GetAll(c => c.ProjectName == ProjectName).Select(c => c.ID).FirstOrDefault();
                        // gán lại follow project nếu đã tồn tại
                        List<FollowProjectBase> followProjects = _followProjectBaseRepo.GetAll(c => c.ProjectID == projectID);
                        if (followProjects.Count > 0)
                        {
                            followProjectBase = followProjects.FirstOrDefault();
                        }
                        followProjectBase.ProjectID = projectID; // ID dự án
                        //followProjectBase.UserID = getIDFromDb("Users", "FullName", FullName??"", 0); // Sale phụ trách
                        followProjectBase.UserID = _userRepo.GetAll(u => u.FullName == FullName).Select(u => u.ID).FirstOrDefault(); // Sale phụ trách
                        //followProjectBase.CustomerBaseID = getIDFromDb("CustomerBase", "CustomerName", CustomerName??"", 0); // Đối tác(KH)
                        followProjectBase.CustomerBaseID = _customerRepo.GetAll(c => c.CustomerName == CustomerName).Select(c => c.ID).FirstOrDefault(); // Đối tác(KH)
                        //followProjectBase.EndUserID = getIDFromDb("CustomerBase", "CustomerName", EndUser??"", 0);//End User
                        followProjectBase.EndUserID = _customerRepo.GetAll(c => c.CustomerName == EndUser).Select(c => c.ID).FirstOrDefault();//End User
                        //followProjectBase.ProjectStatusBaseID = getIDFromDb("ProjectStatus", "StatusName", ProjectStatusName??"", 0);// trạng thái dự án (khác winform, winform trong sp lấy là projectstatus mà lúc nhập excel lại đối chiếu với projectstatusbase
                        followProjectBase.ProjectStatusBaseID = _projectStatusRepo.GetAll(c => c.StatusName == ProjectStatusName).Select(c => c.ID).FirstOrDefault();// trạng thái dự án (khác winform, winform trong sp lấy là projectstatus mà lúc nhập excel lại đối chiếu với projectstatusbase
                        //followProjectBase.ProjectTypeBaseID = getIDFromDb("ProjectTypeBase", "ProjectTypeName", ProjectTypeName??"", 0);// loại dự án
                        followProjectBase.ProjectTypeBaseID = _projectTypeBaseRepo.GetAll(c => c.ProjectTypeName == ProjectTypeName).Select(c => c.ID).FirstOrDefault();// loại dự án
                        //followProjectBase.FirmBaseID = getIDFromDb("FirmBase", "FirmName", FirmName??"", 0);// hãng
                        followProjectBase.FirmBaseID = _firmBaseRepo.GetAll(c => c.FirmName == FirmName).Select(c => c.ID).FirstOrDefault();// hãng
                        followProjectBase.PossibilityPO = PossibilityPO; // khả năng có PO
                        followProjectBase.ExpectedPlanDate = ExpectedPlanDate; // dự kiến ngày lên phương án
                        followProjectBase.ExpectedPODate = ExpectedPODate; //dự kiến ngày po
                        followProjectBase.ExpectedProjectEndDate = ExpectedProjectEndDate; // dự kiến ngày kết thúc dự án
                        followProjectBase.ExpectedQuotationDate = ExpectedQuotationDate;// dự kiến ngày báo giá
                        followProjectBase.RealityPlanDate = RealityPlanDate;// thực tế ngày lên phương án
                        followProjectBase.RealityQuotationDate = RealityQuotationDate;// thực tế ngày báo giá
                        followProjectBase.RealityPODate = RealityPODate;// thực tế ngày po
                        followProjectBase.RealityProjectEndDate = RealityProjectEndDate;// thực tế ngày kết thúc dự án
                        followProjectBase.TotalWithoutVAT = decimal.Parse(TotalWithoutVAT);// tổng giá chưa Vat
                        followProjectBase.ProjectContactName = ProjectContactName;// người phụ trách chính
                        followProjectBase.Note = Note;// ghi chú
                                                      // --- Lưu ---
                        if (followProjects.Any())
                        {
                            _followProjectBaseRepo.Update(followProjectBase);
                            updated++;
                        }
                        else
                        {
                            _followProjectBaseRepo.Create(followProjectBase);
                            created++;
                        }
                    }


                    //toSave.Add(entity);
                }
                catch (Exception ex)
                {
                    errors.Add(new { message = ex.Message });
                }
            }

            return Ok(new
            {
                status = 1,
                created,
                updated,
                skipped = errors.Count,
                errors
            });
        }
        //private static int getIDFromDb(string dataBase, string columnName, string value, int result)
        //{
        //    //int result;
        //    var dt = SQLHelper<object>.Select($"Select * From {dataBase} Where {columnName.Trim()} = N'{value.Trim()}' ");
        //    if (dt.Count > 0)
        //    {
        //        result = int.Parse(((Dictionary<string, object>)dt[0]).GetString("ID"));
        //    }
        //    return result;

        //}

    }
}
static class ImportExtensions
{
    public static string GetString(this Dictionary<string, object> row, string key)
    {
        if (row == null)
            return null;
        if (!row.TryGetValue(key, out var val) || val == null)
            return null;
        var s = val.ToString()?.Trim();
        return string.IsNullOrEmpty(s) ? null : s;
    }

    public static DateTime? GetNullableDate(this Dictionary<string, object> row, string key)
    {
        if (row == null)
            return null;
        if (!row.TryGetValue(key, out var val) || val == null)
            return null;

        var str = val.ToString();

        // ISO string
        if (DateTime.TryParse(str, CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var iso))
            return iso;

        // dd/MM/yyyy
        if (DateTime.TryParseExact(str, new[] { "dd/MM/yyyy", "d/M/yyyy" },
            CultureInfo.InvariantCulture, DateTimeStyles.None, out var dmy))
            return dmy;

        // yyyy-MM-dd
        if (DateTime.TryParseExact(str, "yyyy-MM-dd",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out var ymd))
            return ymd;

        // Excel serial number
        if (double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var serial))
        {
            var epoch = new DateTime(1899, 12, 30);
            return epoch.AddDays(serial);
        }

        return null;
    }
}
