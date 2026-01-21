using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.KPISALE
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DailyReportSaleController : ControllerBase
    {
        private readonly DailyReportSaleRepo _dailyReportSaleRepo;
        private readonly ProjectRepo _projectRepo;
        private readonly CustomerRepo _customerRepo;
        private readonly EmployeeTeamSaleRepo _employeeTeamSaleRepo;
        private readonly EmployeeTeamSaleLinkRepo _employeeTeamSaleLinkRepo;
        private readonly GroupSaleRepo _groupSaleRepo;
        private readonly FirmBaseRepo _firmBaseRepo;
        private readonly ProjectTypeBaseRepo _projectTypeBaseRepo;
        private readonly ProjectStatusRepo _projectStatusRepo;
        private readonly CustomerContactRepo _customerContactRepo;
        private readonly FollowProjectBaseRepo _followProjectBaseRepo;
        private readonly CustomerPartsRepo _customerPartRepo;
        private readonly ProjectStatusLogRepo _projectStatusLogRepo;

        public DailyReportSaleController(DailyReportSaleRepo dailyReportSaleRepo,CustomerPartsRepo customerPartsRepo , ProjectRepo projectRepo, CustomerRepo customerRepo, GroupSaleRepo groupSaleRepo, EmployeeTeamSaleRepo employeeTeamSaleRepo, FirmBaseRepo firmBaseRepo, ProjectTypeBaseRepo projectTypeBaseRepo, ProjectStatusRepo projectStatusRepo, CustomerContactRepo customerContactRepo, FollowProjectBaseRepo followProjectBaseRepo, ProjectStatusLogRepo projectStatusLogRepo, EmployeeTeamSaleLinkRepo employeeTeamSaleLinkRepo)
        {
            _dailyReportSaleRepo = dailyReportSaleRepo;
            _projectRepo = projectRepo;
            _customerRepo = customerRepo;
            _groupSaleRepo = groupSaleRepo;
            _employeeTeamSaleRepo = employeeTeamSaleRepo;
            _firmBaseRepo = firmBaseRepo;
            _projectTypeBaseRepo = projectTypeBaseRepo;
            _projectStatusRepo = projectStatusRepo;
            _customerContactRepo = customerContactRepo;
            _followProjectBaseRepo = followProjectBaseRepo;
            _projectStatusLogRepo = projectStatusLogRepo;
            _customerPartRepo = customerPartsRepo;
            _employeeTeamSaleLinkRepo = employeeTeamSaleLinkRepo;
        }

        [HttpGet("get-data")]
        public IActionResult GetDailyReportSale(int page, int size, DateTime dateStart, DateTime dateEnd, int customerId, int userId, int teamId, int projectId, int employeeTeamSaleId, string? filterText = "", int groupType = -1)
        {
            try
            {
                var result = SQLHelper<dynamic>.ProcedureToList("spGetDailyReportSale",
                                new string[] { "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@FilterText", "@CustomerID", "@UserID", "@GroupType", "@Team", "@ProjectID", "@EmployeeTeamSaleID" },
                                new object[] { page, size, dateStart, dateEnd, filterText, customerId, userId, groupType, teamId, projectId, employeeTeamSaleId });
                var data = SQLHelper<dynamic>.GetListData(result, 0);
                var totalPage = SQLHelper<dynamic>.GetListData(result, 1); 
                return Ok(ApiResponseFactory.Success(new {data, totalPage}, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-by-id")]
        public IActionResult GetByID(int id)
        {
            try
            {
                var data = _dailyReportSaleRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-projects")]
        public IActionResult GetProjects() {
            try
            {
                var result = _projectRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-employee-team-sale")]
        public IActionResult GetEmployeeTeamSale()
        {
            try
            {
                var result = _employeeTeamSaleRepo.GetAll(x => x.IsDeleted != 1 && x.ParentID == 0);


                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-group-sale")]
        public IActionResult GetGroupSale(int userId)
        {
            try
            {
                var result = SQLHelper<dynamic>.ProcedureToList("spGetGroupSalesByUserID",
                                new string[] { "@UserID" },
                                new object[] { "" });
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-customerpart")]
        public IActionResult GetCustomerPart(int customerId)
        {
            try
            {
                var result = SQLHelper<dynamic>.ProcedureToList("spGetCustomerPart",
                                new string[] { "@ID" },
                                new object[] { customerId });
                var data = SQLHelper<dynamic>.GetListData(result, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-mainindex")]
        public IActionResult GetMainIndex()
        {
            try
            {
                var result = SQLHelper<dynamic>.ProcedureToList("spGetMainIndex",
                                new string[] { "@Type" },
                                new object[] { 2 });
                var data = SQLHelper<dynamic>.GetListData(result, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-customers")]
        public IActionResult GetCustomers()
        {
            try
            {
                var result = _customerRepo.GetAll(x => x.IsDeleted != true).Select(e => new
                {
                    CustomerName = e.CustomerName,
                    CustomerCode = e.CustomerCode,
                    ID = e.ID,
                });

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-firmbase")]
        public IActionResult GetFirmBase()
        {
            try
            {
                var result = _firmBaseRepo.GetAll();
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-projecttypebase")]
        public IActionResult GetProjectTypeBase()
        {
            try
            {
                var result = _projectTypeBaseRepo.GetAll();
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-projectstatus")]
        public IActionResult GetProjectStatus()
        {
            try
            {
                var result = _projectStatusRepo.GetAll();
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-customercontact")]
        public IActionResult GetCustomerContact(int customerId)
        {
            try
            {
                var result = _customerContactRepo.GetAll(x => x.CustomerID == customerId && x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-teamsale-by-employee")]
        public IActionResult GetTeamSaleByEmployee(int employeeId)
        {
            try
            {
                var result = (
                    from el in _employeeTeamSaleLinkRepo.GetAll(x =>
                            x.EmployeeID == employeeId)
                    join ets in _employeeTeamSaleRepo.GetAll(x =>
                            x.IsDeleted != 1 && x.ParentID == 0)
                        on el.EmployeeTeamSaleID equals ets.ID
                    select new
                    {
                        TeamSaleID = ets.ID,
                        TeamSaleName = ets.Name
                    }
                ).Distinct().FirstOrDefault();

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> Save(DailyReportSaleDTO dto)
        {
            try
            {
                DailyReportSale model = dto.ID > 0 ? _dailyReportSaleRepo.GetByID(dto.ID) : new DailyReportSale();
                model.UserID = dto.userId;
                model.DateEnd = dto.dateEnd;
                model.BigAccount = dto.bigAccount;
                model.CustomerID = dto.customerId;
                model.ContacID = dto.contactId;
                model.Content = dto.content;
                model.Result = dto.result;
                model.ProblemBacklog = dto.problemBacklog;
                model.PlanNext = dto.planNext;
                //model.Note = dto.note; // bị ẩn
                model.GroupType = dto.groupTypeId;
                model.Month = dto.dateEnd?.Month;
                model.Year = DateTime.Now.Year;
                model.EndUser = dto.partId;
                model.DateStart = dto.dateStart;
                //model.RequestOfCustomer = dto.requestofcustomer //bị ẩn
                model.ProductOfCustomer = dto.productOfCustomer;
                model.ProjectID = dto.projectId;
                model.FirmBaseID = dto.firmId;
                model.ProjectTypeBaseID = dto.projectTypeId;
                model.SaleOpportunity = dto.saleOpportunity;
                model.WarehouseID = dto.warehouseId;
                
                if(dto.ID > 0)
                {
                    await _dailyReportSaleRepo.UpdateAsync(model);
                }
                else
                {
                    await _dailyReportSaleRepo.CreateAsync(model);
                }

                RERPAPI.Model.Entities.Project project = await _projectRepo.GetByIDAsync(dto.projectId); 
                FollowProjectBase followProjectBase = _followProjectBaseRepo.GetAll(x => x.ProjectID == project.ID).OrderByDescending(x => x.ID).FirstOrDefault() ?? new FollowProjectBase();
                followProjectBase.ProjectID = project.ID;
                followProjectBase.CustomerBaseID = dto.customerId;
                followProjectBase.EndUserID = project.EndUser;
                //followProjectBase.ProjectStatusBaseID = project.ProjectStatus;
                followProjectBase.ProjectStartDate = project.CreatedDate;
                followProjectBase.WarehouseID = dto.warehouseId;
                followProjectBase.FirmBaseID = model.FirmBaseID;
                followProjectBase.ProjectTypeBaseID = model.ProjectTypeBaseID;
                followProjectBase.ProjectStatusBaseID = dto.projectStatusBaseId;

                if(followProjectBase.ID > 0)
                {
                    await _followProjectBaseRepo.UpdateAsync(followProjectBase);
                }
                else
                {
                    await _followProjectBaseRepo.CreateAsync(followProjectBase);
                }
                FollowProjectBaseDetail detail = new FollowProjectBaseDetail()
                {
                    FollowProjectBaseID = followProjectBase.ID,
                    ProjectID = project.ID,
                    UserID = dto.userId,
                    ImplementationDate = dto.dateStart,
                    ExpectedDate = dto.dateEnd,
                    WorkDone = dto.content.Trim(),
                    WorkWillDo = dto.planNext.Trim(),
                    Results = dto.result.Trim(),
                    ProblemBacklog = dto.problemBacklog.Trim(),
                }; 

                //Updateproject 
                if(project.ID > 0)
                {
                    project.ProjectStatus = dto.projectStatusBaseId;
                    await _projectRepo.UpdateAsync(project);

                    if(dto.projectStatusOld != project.ProjectStatus)
                    {
                        ProjectStatusLog statuslog = new ProjectStatusLog()
                        {
                            ProjectID = project.ID,
                            ProjectStatusID = project.ProjectStatus,
                            EmployeeID = dto.employeeId ?? 0,
                            DateLog = dto.dateStatusLog,
                        };
                        await _projectStatusLogRepo.CreateAsync(statuslog);
                    }    
                } 
                    
                return Ok(ApiResponseFactory.Success(null, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-latest-daily-report-sale")]
        public IActionResult GetLatestDailyReportSale(int userId, int projectId)
        {
            try
            {
                if (userId <= 0 || projectId <= 0)
                {
                    return Ok(ApiResponseFactory.Success(new { ProductOfCustomer = "" }, ""));
                }

                // Tìm DailyReportSale mới nhất theo UserID và ProjectID
                var dailyReportSale = _dailyReportSaleRepo.GetAll(x => x.UserID == userId && x.ProjectID == projectId)
                    .OrderByDescending(x => x.ID)
                    .FirstOrDefault();

                if (dailyReportSale == null)
                {
                    return Ok(ApiResponseFactory.Success(new { ProductOfCustomer = "" }, ""));
                }

                return Ok(ApiResponseFactory.Success(new
                {
                    ProductOfCustomer = dailyReportSale.ProductOfCustomer ?? ""
                }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-latest-follow-project-base")]
        public IActionResult GetLatestFollowProjectBase(int projectId)
        {
            try
            {
                if (projectId <= 0)
                {
                    return Ok(ApiResponseFactory.Success(new
                    {
                        FirmBaseID = 0,
                        ProjectTypeBaseID = 0
                    }, ""));
                }

                // Tìm FollowProjectBase mới nhất theo ProjectID
                var followProjectBase = _followProjectBaseRepo.GetAll(x => x.ProjectID == projectId)
                    .OrderByDescending(x => x.ID)
                    .FirstOrDefault();

                if (followProjectBase == null)
                {
                    return Ok(ApiResponseFactory.Success(new
                    {
                        FirmBaseID = 0,
                        ProjectTypeBaseID = 0
                    }, ""));
                }

                return Ok(ApiResponseFactory.Success(new
                {
                    FirmBaseID = followProjectBase.FirmBaseID ?? 0,
                    ProjectTypeBaseID = followProjectBase.ProjectTypeBaseID ?? 0
                }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("delete")]
        public IActionResult Delete(int id)
        {
            try
            {
                var model = _dailyReportSaleRepo.GetByID(id);
                model.DeleteFlag = 1;
                _dailyReportSaleRepo.Update(model);
                return Ok(ApiResponseFactory.Success("", ""));

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

                //var users = _userRepo.GetAll().ToList();
                var dataUser = SQLHelper<dynamic>.ProcedureToList(
                "spGetEmployee",
                new string[] { "@Status" },
                new object[] { 0 });
                var users = SQLHelper<dynamic>.GetListData(dataUser, 0);

                var projects = _projectRepo.GetAll().ToList();
                var firmBases = _firmBaseRepo.GetAll().ToList();
                var projectTypeBases = _projectTypeBaseRepo.GetAll().ToList();
                var customers = _customerRepo.GetAll().ToList();
                var contacts = _customerContactRepo.GetAll().ToList();
                var mainIndexesData = SQLHelper<dynamic>.ProcedureToList(
                "spGetMainIndex",
                new string[] { "@Type" },
                new object[] { 2 }).ToList();
                var mainIndexes = SQLHelper<dynamic>.GetListData(mainIndexesData, 0);

                var customerParts = _customerPartRepo.GetAll().ToList();

                int rowNumber = 1;
                foreach (var row in data)
                {
                    rowNumber++;
                    try
                    {
                        var model = new DailyReportSale();

                        // F1: Ngày thực hiện gần nhất
                        model.DateStart = ParseDate(GetString(row, "Ngày thực hiện"));

                        // F2: Ngày dự kiến thực hiện  
                        model.DateEnd = ParseDate(GetString(row, "Ngày dự kiến"));

                        // F3: Người phụ trách (lookup từ FullName)
                        var fullName = GetString(row, "Người phụ trách");
                        var userRow = users.FirstOrDefault(u => u.FullName == fullName);
                        model.UserID = userRow?.UserID ?? 0;

                        // F4: Mã dự án (lookup từ ProjectCode)
                        var projectCode = GetString(row, "Mã dự án");
                        var project = projects.FirstOrDefault(p => p.ProjectCode == projectCode);
                        model.ProjectID = project?.ID ?? 0;

                        // F6: Hãng (lookup từ FirmName)
                        var firmName = GetString(row, "Hãng");
                        var firmRow = firmBases.FirstOrDefault(f => f.FirmName == firmName);
                        model.FirmBaseID = firmRow?.ID ?? 0;

                        // F7: Loại dự án (lookup từ ProjectTypeName)
                        var projectTypeName = GetString(row, "Loại dự án");
                        var projectTypeRow = projectTypeBases.FirstOrDefault(pt => pt.ProjectTypeName == projectTypeName);
                        model.ProjectTypeBaseID = projectTypeRow?.ID ?? 0;

                        // F8: Khách hàng (lookup từ CustomerName)
                        var customerName = GetString(row, "Khách hàng");
                        var customerRow = customers.FirstOrDefault(c => c.CustomerName == customerName);
                        model.CustomerID = customerRow?.ID ?? 0;

                        // F9: Mã KH (dùng cho EndUser)
                        var customerCode = GetString(row, "Mã KH");

                        // F10: Sản phẩm của KH
                        model.ProductOfCustomer = GetString(row, "Sản phẩm KH");

                        // F11: Người liên hệ - contactname
                        var contactName = GetString(row, "Người liên hệ");
                        var contactRow = contacts.FirstOrDefault(c => c.ContactName == contactName);
                        model.ContacID = contactRow?.ID ?? 0;

                        // F12: Loại nhóm (lookup từ MainIndex)
                        var mainIndexName = GetString(row, "Loại nhóm");
                        var mainIndexRow = mainIndexes.FirstOrDefault(m => m.MainIndex == mainIndexName);
                        model.GroupType = mainIndexRow?.ID ?? 0;

                        // F13: Việc đã làm
                        model.Content = GetString(row, "Việc đã làm");

                        // F14: Kết quả
                        model.Result = GetString(row, "Kết quả");

                        // F15: Vấn đề tồn đọng
                        model.ProblemBacklog = GetString(row, "Vấn đề tồn đọng");

                        // F16: Kế hoạch tiếp theo
                        model.PlanNext = GetString(row, "Kế hoạch tiếp theo");

                        // F17: End User (lookup từ PartCode + CustomerCode)
                        var partCode = GetString(row, "End User");
                        var endUserRow = customerParts.FirstOrDefault(p =>
                            p.PartCode == partCode);
                        model.EndUser = endUserRow?.ID;

                        // F18: Big Account
                        var bigAccount = GetString(row, "Big Account").Trim().ToLower();
                        model.BigAccount = bigAccount == "x" || bigAccount == "có";

                        // F19: Cơ hội bán hàng 
                        var saleOpportunity = GetString(row, "Cơ hội bán hàng").Trim().ToLower();
                        model.SaleOpportunity = saleOpportunity == "x" || saleOpportunity == "có";

                        // Các trường bổ sung
                        model.Month = model.DateStart?.Month;
                        model.Year = DateTime.Now.Year;

                        await _dailyReportSaleRepo.CreateAsync(model);
                        created++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Dòng {rowNumber}: {ex.Message}");
                        skipped++;
                    }
                }

                return Ok(ApiResponseFactory.Success(new { created, updated = 0, skipped, errors },
                    $"Import hoàn tất: Tạo mới {created}, Bỏ qua {skipped}"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-project-status")]
        public IActionResult SaveProjectStatus(ProjectStatus model)
        {
            try
            {
                var exist = _projectStatusRepo.GetAll(x => x.StatusName.ToLower().Trim() == model.StatusName.ToLower().Trim());
                if(exist != null && exist.Count > 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null,""));
                }
                ProjectStatus data = new ProjectStatus();
                data.STT = model.STT;
                data.StatusName = model.StatusName;
                _projectStatusRepo.Create(data);
                return Ok(ApiResponseFactory.Success("", "Lưu thành công"));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        private string GetString(Dictionary<string, object> row, string key)
        {
            return row.TryGetValue(key, out var value) ? value?.ToString()?.Trim() ?? "" : "";
        }

        private DateTime? ParseDate(string dateStr)
        {
            if (string.IsNullOrEmpty(dateStr)) return null;

            // dd/MM/yyyy
            if (DateTime.TryParseExact(dateStr, "dd/MM/yyyy", null,
                System.Globalization.DateTimeStyles.None, out var date1))
                return date1;

            // ISO format
            if (DateTime.TryParse(dateStr, out var date2))
                return date2;

            return null;
        }

    }
}
