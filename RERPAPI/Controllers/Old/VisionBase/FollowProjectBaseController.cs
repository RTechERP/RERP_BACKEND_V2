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
