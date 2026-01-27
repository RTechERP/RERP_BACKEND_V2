using Azure.Core;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;
using NPOI.Util;
using OfficeOpenXml;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.Handover;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Asset;
using RERPAPI.Repo.GenericEntity.BBNV;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HandoverController : ControllerBase
    {
        private readonly DepartmentRepo _tsDepartment;
        private readonly EmployeeChucVuHDRepo _positionRepo;
        private readonly HandoverRepo _handoverRepo;
        private readonly HandoverReceiverRepo _handoverReceiverRepo;
        private readonly HandoverWorkRepo _handoverWorkRepo;
        private readonly HandoverWarehouseAssetRepo _handoverWarehouseAssetRepo;
        private readonly HandoverAssetManagementRepo _handoverAssetManagementRepo;
        private readonly HandoverFinanceRepo _handoverFinanceRepo;
        private readonly HandoverSubordinateRepo _handoverSubordinateRepo;
        private readonly HandoverApproveRepo _approveRepo;
        private readonly vUserGroupLinksRepo _vUserGroupLinksRepo;
        private readonly TSAssetManagementRepo _tsAssetManagementRepo;
        private IConfiguration _configuration;

        public HandoverController(DepartmentRepo tsDepartment, EmployeeChucVuHDRepo positionRepo, HandoverRepo handoverRepo, HandoverReceiverRepo handoverReceiverRepo, HandoverWorkRepo handoverWorkRepo, HandoverWarehouseAssetRepo handoverWarehouseAssetRepo, HandoverAssetManagementRepo handoverAssetManagementRepo, HandoverFinanceRepo handoverFinanceRepo, HandoverSubordinateRepo handoverSubordinateRepo, HandoverApproveRepo approveRepo, vUserGroupLinksRepo vUserGroupLinksRepo, TSAssetManagementRepo tsAssetManagementRepo, IConfiguration configuration)
        {
            _tsDepartment = tsDepartment;
            _positionRepo = positionRepo;
            _handoverRepo = handoverRepo;
            _handoverReceiverRepo = handoverReceiverRepo;
            _handoverWorkRepo = handoverWorkRepo;
            _handoverWarehouseAssetRepo = handoverWarehouseAssetRepo;
            _handoverAssetManagementRepo = handoverAssetManagementRepo;
            _handoverFinanceRepo = handoverFinanceRepo;
            _handoverSubordinateRepo = handoverSubordinateRepo;
            _approveRepo = approveRepo;
            _vUserGroupLinksRepo = vUserGroupLinksRepo;
            _tsAssetManagementRepo = tsAssetManagementRepo;
            _configuration = configuration;
        }

        [HttpPost("get-handover")]
        public IActionResult GetHandover([FromBody] HandoverRequestParam handoverrequest)
                {
            var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
            CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

            handoverrequest.DateStart = new DateTime(handoverrequest.DateStart.Year, handoverrequest.DateStart.Month, handoverrequest.DateStart.Day, 0, 0, 0);
            handoverrequest.DateEnd = new DateTime(handoverrequest.DateEnd.Year, handoverrequest.DateEnd.Month, handoverrequest.DateEnd.Day, 23, 59, 59);

            var vUserHR = _vUserGroupLinksRepo
                .GetAll()
                .FirstOrDefault(x => (x.Code == "N23" || x.Code == "N1" || x.Code == "N34") &&
          x.UserID == currentUser.ID);

            int employeeID;
            if (vUserHR != null)
            {
                employeeID = handoverrequest.EmployeeID;
            }
            else
            {
                employeeID = currentUser.EmployeeID;
            }



            //int employeeID = 0;
            //int approverID = 0;
            //if (vUserHR != null)
            //{
            //    handoverrequest.LeaderID = 0;
            //    handoverrequest.EmployeeID = 0;
            //}
            //else
            //{
            //    handoverrequest.EmployeeID = currentUser.EmployeeID;

            //}
            //var vUserTBP = _vUserGroupLinksRepo
            //   .GetAll()
            //   .FirstOrDefault(x => x.Code == "N32" && x.UserID == currentUser.ID);
            //if (vUserTBP != null)
            //{
            //    //approverID = handoverrequest.ApproverID;
            //    //handoverrequest.ApproverID = currentUser.EmployeeID;
            //}

            var handover = SQLHelper<dynamic>.ProcedureToList("spGetHandover",
                new string[] { "@DepartmentID", "@EmployeeID", "@Keyword", "@LeaderID", "@DateStart", "@DateEnd", "@ApproverID" },
                new object[] { handoverrequest.DepartmentID, employeeID, handoverrequest.Keyword, handoverrequest.LeaderID, handoverrequest.DateStart, handoverrequest.DateEnd, handoverrequest.ApproverID }
                );
            try
            {
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        asset = SQLHelper<dynamic>.GetListData(handover, 0),
                        total = SQLHelper<dynamic>.GetListData(handover, 1)
                    }
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()

                });
            }
        }
        [HttpPost("get-handover-data")]
        public IActionResult GetHandoverData([FromBody] HandoverDataRequestParam request)
        {
            try
            {
                var handoverReceiver = SQLHelper<dynamic>.ProcedureToList("spGetHandoverReceiver ",
                   new string[] { "@HandoverID" },
                   new object[] { request.HandoverID }
               );

                var handoverWork = SQLHelper<dynamic>.ProcedureToList("spGetHandoverWork",
                    new string[] { "@HandoverID" },
                    new object[] { request.HandoverID }
                );

                    var handoverWarehouseAsset = SQLHelper<dynamic>.ProcedureToList("spGetHandoverWarehouseAsset",
                  new string[] { "@PageNumber", "@PageSize", "@DateBegin", "@DateEnd", "@ProductGroupID", "@ReturnStatus", "@FilterText", "@WareHouseID", "@EmployeeID" },
                  new object[] { request.PageNumber, request.PageSize, request.DateBegin, request.DateEnd, request.ProductGroupID, request.ReturnStatus, request.FilterText, request.WareHouseID, request.EmployeeID }
                );
                var handoverAssetManagement = SQLHelper<dynamic>.ProcedureToList("spGetTSAsset",
                    new string[] { "@LeaderID", "@HandoverID" },
                    new object[] { request.LeaderID, request.HandoverID }
                );
                var handoverFinances = SQLHelper<dynamic>.ProcedureToList("spGetHandoverFinances",
                    new string[] { "@HandoverID" },
                    new object[] { request.HandoverID }
                );
                var handoverSubordinates = SQLHelper<dynamic>.ProcedureToList("spGetHandoverSubordinates",
                  new string[] { "@LeaderID" },
                  new object[] { request.LeaderID }
              );
                var handoverApprove = SQLHelper<dynamic>.ProcedureToList("spGetHandoverApprove ",
                   new string[] { "@HandoverID" },
                   new object[] { request.HandoverID }
               );
                var HandoverWork = SQLHelper<dynamic>.GetListData(handoverWork, 0);
                var HandoverWarehouseAsset = SQLHelper<dynamic>.GetListData(handoverWarehouseAsset, 0);
                var HandoverAssetManagement = SQLHelper<dynamic>.GetListData(handoverAssetManagement, 0);
                var HandoverFinance = SQLHelper<dynamic>.GetListData(handoverFinances, 0);
                var HandoverSubordinate = SQLHelper<dynamic>.GetListData(handoverSubordinates, 0);
                var HandoverReceiver = SQLHelper<dynamic>.GetListData(handoverReceiver, 0);
                var HandoverApprove = SQLHelper<dynamic>.GetListData(handoverApprove, 0);



                // Trả về dữ liệu tổng hợp
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        HandoverReceiver,
                        HandoverWork,
                        HandoverWarehouseAsset,
                        HandoverAssetManagement,
                        HandoverFinance,
                        HandoverSubordinate,
                        HandoverApprove
                    }
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        [HttpGet("get-departments")]

        public IActionResult GetDepartment()
        {
            try
            {
                var datadepartment = _tsDepartment.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = datadepartment
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

        [HttpGet("get-position")]

        public IActionResult GetPosition()
        {
            try
            {
                var dataposition = _positionRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = dataposition
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


        [HttpPost("get-employees")]

        public IActionResult GetEmployee([FromBody] AllEmployeeRequestParam employeerequest)
        {
            try
            {

                var employees = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployee",
                                                new string[] { "@Status" },
                                                new object[] { 0 });
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        asset = employees

                    }

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

        [HttpGet("get-all-employees")]

        public IActionResult GetListEmployee()
        {
            try
            {

                var employees = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployee",
                                                new string[] { "@Status" },
                                                new object[] { 0 });

                return Ok(new
                {
                    status = 1,
                    data = employees,

                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {

                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }


        [HttpGet("get-handover/{id}")]
        public IActionResult getHandover(int id)
        {
            try
            {
                Handover result = _handoverRepo.GetByID(id);
                return Ok(new
                {
                    status = 1,
                    data = result
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

        private async Task UpdateEmployeeForAssets(List<HandoverAssetManagement> assets, int newEmployeeId)
        {
            if (assets == null || !assets.Any()) return;

            foreach (var item in assets)
            {
                var asset = _tsAssetManagementRepo
                    .GetAll()
                    .FirstOrDefault(x => x.TSAssetCode == item.TSAssetCode);

                if (asset != null)
                {
                    asset.EmployeeID = newEmployeeId; // nhân viên mượn
                    asset.UpdatedDate = DateTime.Now;
                    await _tsAssetManagementRepo.UpdateAsync(asset);
                }
            }
        }


        [HttpPost("save-data-handover")]
        public async Task<IActionResult> SaveData([FromBody] HandoverDTO dto)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                if (dto == null || dto.Handover == null)
                {
                    return BadRequest(new { status = 0, message = "Dữ liệu không hợp lệ" });
                }

                int handoverID = 0;



                // Master
                if (dto.Handover.ID <= 0)

                {
                    var maxStt = _handoverRepo.GetAll()
                        .Select(x => x.STT ?? 0)
                        .DefaultIfEmpty(0)
                        .Max();
                    dto.Handover.STT = maxStt + 1;

                    string prefix = "MB_RTC";
                    string today = DateTime.Now.ToString("yyyyMMdd");

                    var last = _handoverRepo
                        .GetAll(x => x.Code.StartsWith(prefix + "_" + today))
                        .OrderByDescending(x => x.CreatedDate)
                        .FirstOrDefault();

                    int nextNumber = 1;
                    if (last != null && !string.IsNullOrEmpty(last.Code))
                    {
                        var parts = last.Code.Split('.');
                        if (parts.Length > 1 && int.TryParse(parts[1], out int lastNumber))
                        {
                            nextNumber = lastNumber + 1;
                        }
                    }

                    dto.Handover.Code = $"{prefix}_{today}.{nextNumber:D3}";
                    if (dto == null || dto.Handover == null)
                    {
                        return BadRequest(new { status = 0, message = "Dữ liệu không hợp lệ" });
                    }


                    dto.Handover.CreatedBy = currentUser.LoginName;
                    dto.Handover.UpdatedBy = currentUser.LoginName;
                    await _handoverRepo.CreateAsync(dto.Handover);
                    handoverID = dto.Handover.ID; // repo sẽ gán ID sau khi insert
                }
                else
                {

                    dto.Handover.UpdatedBy = currentUser.LoginName;
                    _handoverRepo.Update(dto.Handover);
                    handoverID = dto.Handover.ID;
                }

                // Người nhận bàn giao
                if (dto.HandoverReceiver != null && dto.HandoverReceiver.Any())
                {
                    var maxSttReceiver = _handoverReceiverRepo.GetAll()
                        .Where(x => x.HandoverID == handoverID)
                        .Select(x => x.STT ?? 0)
                        .DefaultIfEmpty(0)
                        .Max();
                    int sttEmployeeCounter = maxSttReceiver;

                    foreach (var employeeReceiver in dto.HandoverReceiver)
                    {
                        employeeReceiver.HandoverID = handoverID;

                        var existing = _handoverReceiverRepo.GetAll()
                   .FirstOrDefault(x => x.HandoverID == handoverID && x.ID == employeeReceiver.ID);

                        if (existing == null || employeeReceiver.ID <= 0)
                        {
                            sttEmployeeCounter++;
                            employeeReceiver.STT = sttEmployeeCounter;
                            await _handoverReceiverRepo.CreateAsync(employeeReceiver);
                        }

                        else
                            _handoverReceiverRepo.Update(employeeReceiver);
                    }
                }
                if (dto.DeletedHandoverReceiver.Count > 0)
                {
                    foreach (var item in dto.DeletedHandoverReceiver)
                    {
                        HandoverReceiver receivermodel = _handoverReceiverRepo.GetByID(item);
                        receivermodel.IsDeleted = true;
                        await _handoverReceiverRepo.UpdateAsync(receivermodel);
                    }
                }

                // Công việc bàn giao
                if (dto.HandoverWork != null && dto.HandoverWork.Any())
                {
                    var maxSttWork = _handoverWorkRepo.GetAll()
                        .Where(x => x.HandoverID == handoverID)
                        .Select(x => x.STT ?? 0)
                        .DefaultIfEmpty(0)
                        .Max();
                    int sttHandoverWork = maxSttWork;
                    foreach (var itemWork in dto.HandoverWork)
                    {
                        itemWork.HandoverID = handoverID;
                        var existing = _handoverWorkRepo.GetAll()
                .FirstOrDefault(x => x.HandoverID == handoverID && x.ID == itemWork.ID);

                        if (existing == null || itemWork.ID <= 0)
                        {
                            sttHandoverWork++;
                            itemWork.STT = sttHandoverWork;
                            itemWork.CreatedDate = DateTime.Now;
                            await _handoverWorkRepo.CreateAsync(itemWork);
                        }

                        else
                            _handoverWorkRepo.Update(itemWork);
                    }
                }
                if (dto.DeletedWork.Count > 0)
                {
                    foreach (var item in dto.DeletedWork)
                    {
                        HandoverWork model = _handoverWorkRepo.GetByID(item);
                        model.IsDeleted = true;
                        await _handoverWorkRepo.UpdateAsync(model);
                    }
                }

                // Tài sản bàn giao
                if (dto.HandoverAssetManagement != null && dto.HandoverAssetManagement.Any())
                {
                    var maxSttAsset = _handoverAssetManagementRepo.GetAll()
                        .Where(x => x.HandoverID == handoverID)
                        .Select(x => x.STT ?? 0)
                        .DefaultIfEmpty(0)
                        .Max();
                    int sttHandoverAsset = maxSttAsset;
                    foreach (var itemAsset in dto.HandoverAssetManagement)
                    {
                        itemAsset.HandoverID = handoverID;
                        var existing = _handoverAssetManagementRepo.GetAll()
               .FirstOrDefault(x => x.HandoverID == handoverID && x.TSAssetCode == itemAsset.TSAssetCode);


                        if (existing == null || itemAsset.ID <= 0)
                        {

                            sttHandoverAsset++;
                            itemAsset.STT = sttHandoverAsset;
                            itemAsset.CreatedDate = DateTime.Now;
                            await _handoverAssetManagementRepo.CreateAsync(itemAsset);
                        }

                        else
                        {
                            itemAsset.EmployeeID = itemAsset.EmployeeID;

                            _handoverAssetManagementRepo.Update(itemAsset);
                        }

                    }
                }

                // Tài sản kho bàn giao
                if (dto.HandoverWarehouseAsset != null && dto.HandoverWarehouseAsset.Any())
                {
                    var maxSttWarehouseAsset = _handoverWarehouseAssetRepo.GetAll()
                        .Where(x => x.HandoverID == handoverID)
                        .Select(x => x.STT ?? 0)
                        .DefaultIfEmpty(0)
                        .Max();
                    int sttHandoverWarehouseAsset = maxSttWarehouseAsset;
                    foreach (var itemWarehouseAsset in dto.HandoverWarehouseAsset)
                    {
                        itemWarehouseAsset.HandoverID = handoverID;
                        var existing = _handoverWarehouseAssetRepo.GetAll()
                        .FirstOrDefault(x => x.HandoverID == handoverID && x.ID == itemWarehouseAsset.ID);

                        if (existing == null || itemWarehouseAsset.ID <= 0)
                        {
                            sttHandoverWarehouseAsset++;
                            itemWarehouseAsset.STT = sttHandoverWarehouseAsset;
                            itemWarehouseAsset.CreatedDate = DateTime.Now;
                            await _handoverWarehouseAssetRepo.CreateAsync(itemWarehouseAsset);
                        }

                        else
                            _handoverWarehouseAssetRepo.Update(itemWarehouseAsset);
                    }
                }

                // Tài chính bàn giao
                if (dto.HandoverFinance != null && dto.HandoverFinance.Any())
                {
                    var maxSttFinance = _handoverFinanceRepo.GetAll()
                        .Where(x => x.HandoverID == handoverID)
                        .Select(x => x.STT ?? 0)
                        .DefaultIfEmpty(0)
                        .Max();
                    int sttHandoverFinance = maxSttFinance;
                    foreach (var itemFinance in dto.HandoverFinance)
                    {
                        itemFinance.HandoverID = handoverID;
                        var existing = _handoverFinanceRepo.GetAll()
                        .FirstOrDefault(x => x.HandoverID == handoverID && x.ID == itemFinance.ID);

                        if (existing == null || itemFinance.ID <= 0)
                        {
                            sttHandoverFinance++;
                            itemFinance.STT = sttHandoverFinance;
                            itemFinance.CreatedDate = DateTime.Now;
                            await _handoverFinanceRepo.CreateAsync(itemFinance);
                        }

                        else
                            _handoverFinanceRepo.Update(itemFinance);
                    }
                }
                if (dto.DeletedFinance.Count > 0)
                {
                    foreach (var item in dto.DeletedFinance)
                    {
                        HandoverFinance model = _handoverFinanceRepo.GetByID(item);
                        model.IsDeleted = true;
                        await _handoverFinanceRepo.UpdateAsync(model);
                    }
                }

                // Nhân viên trực thuộc bàn giao
                if (dto.HandoverSubordinate != null && dto.HandoverSubordinate.Any())
                {
                    var maxSttSub = _handoverSubordinateRepo.GetAll()
                        .Where(x => x.HandoverID == handoverID)
                        .Select(x => x.STT ?? 0)
                        .DefaultIfEmpty(0)
                        .Max();
                    int sttHandoverSub = maxSttSub;
                    foreach (var itemSub in dto.HandoverSubordinate)
                    {
                        itemSub.HandoverID = handoverID;
                        var existing = _handoverSubordinateRepo.GetAll()
                        .FirstOrDefault(x => x.HandoverID == handoverID && x.ID == itemSub.ID);

                        if (itemSub.ID <= 0)
                        {
                            sttHandoverSub++;
                            itemSub.STT = sttHandoverSub;
                            itemSub.CreatedDate = DateTime.Now;
                            await _handoverSubordinateRepo.CreateAsync(itemSub);
                        }

                        else
                            _handoverSubordinateRepo.Update(itemSub);
                    }
                }

                //Duyệt
                if (dto.HandoverApprove != null && dto.HandoverApprove.Any())
                {
                    var maxSttSub = _approveRepo.GetAll()
                        .Where(x => x.HandoverID == handoverID)
                        .Select(x => x.STT ?? 0)
                        .DefaultIfEmpty(0)
                        .Max();
                    int sttHandoverApprove = maxSttSub;
                    foreach (var itemApprove in dto.HandoverApprove)
                    {
                        itemApprove.HandoverID = handoverID;
                        var existing = _handoverSubordinateRepo.GetAll()
                        .FirstOrDefault(x => x.HandoverID == handoverID && x.ID == itemApprove.ID);

                        if (itemApprove.ID <= 0)
                        {
                            sttHandoverApprove++;
                            itemApprove.STT = sttHandoverApprove;
                            itemApprove.CreatedDate = DateTime.Now;
                            await _approveRepo.CreateAsync(itemApprove);
                        }

                        else
                            _approveRepo.Update(itemApprove);
                    }
                }


                return Ok(new
                {
                    status = 1,
                    message = "Lưu thành công",
                    id = handoverID,
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

        [HttpPost("upload")]
        public IActionResult Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { status = 0, Message = "Không có file được gửi lên." });
                }

                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xlsx", ".png", ".jpg" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { status = 0, Message = "Chỉ được upload file tài liệu (pdf, doc, docx, xlsx, png, jpg)" });
                }

                //string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                string uploadsFolder = Path.Combine("\\\\192.168.1.190\\Software\\Test", "BBBG");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                string fullPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                return Ok(new
                {
                    status = 1,
                    FileName = file.FileName,             // tên gốc
                    FilePath = $"/uploads/{uniqueFileName}", // đường dẫn public
                    FileNameUnique = uniqueFileName       // giữ lại để sau này insert DB
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 0, Message = $"Upload file thất bại! ({ex.Message})" });
            }
        }
        private void CopyRowStyle(IXLWorksheet sheet, int sourceRow, int targetRow)
        {
            var lastCol = sheet.LastColumnUsed().ColumnNumber();

            for (int col = 1; col <= lastCol; col++)
            {
                sheet.Cell(targetRow, col).Style = sheet.Cell(sourceRow, col).Style;
            }
        }
        [HttpGet("export-excel/{id}")]
        public IActionResult ExportExcel(int id)
        {
            try
            {
                DateTime start = new DateTime(2000, 1, 1);
                DateTime end = new DateTime(2099, 12, 31);
                List<List<dynamic>> handoverList = SQLHelper<dynamic>.ProcedureToList(
                    "spGetHandover",
                    new string[] { "@DateStart", "@DateEnd", "@DepartmentID", "@EmployeeID", "@KeyWord" },
                    new object[] { TextUtils.MinDate, TextUtils.MaxDate, 0, 0, "" }
                );
                var handoverData = SQLHelper<dynamic>.GetListData(handoverList, 0).FirstOrDefault(h => h.ID == id);
                int leaderID = handoverData?.LeaderID ?? 0;
                int employeeID = handoverData?.EmployeeID ?? 0;
                // Lấy chi tiết biên bản (danh sách người nhận bàn giao)
                List<List<dynamic>> detailReceiverList = SQLHelper<dynamic>.ProcedureToList(
                    "spGetHandoverReceiver",
                    new string[] { "@HandoverID" },
                    new object[] { id }
                );
                //Lấy danh sách công việc bàn giao
                List<List<dynamic>> detailWorkList = SQLHelper<dynamic>.ProcedureToList(
                  "spGetHandoverWork",
                  new string[] { "@HandoverID" },
                  new object[] { id }
                );
                //Lấy danh sách tài sản cấp phát bàn giao
                List<List<dynamic>> detailAssetList = SQLHelper<dynamic>.ProcedureToList(
                  "spGetTSAsset",
                  new string[] { "@LeaderID", "@HandoverID" },
                  new object[] { leaderID ,id}
                );
                //Lấy danh sách tài sản kho bàn giao
                List<List<dynamic>> detailAssetWarehouseList = SQLHelper<dynamic>.ProcedureToList(
                  "spGetHandoverWarehouseAsset",
                  new string[] { "@PageNumber", "@PageSize", "@DateBegin", "@DateEnd", "@ProductGroupID", "@ReturnStatus", "@FilterText", "@WareHouseID", "@EmployeeID" },
                  new object[] { 1, 9999, new DateTime(2022, 9, 1), DateTime.Now, 0, 0, "", 1, employeeID }
                );
                //Lấy danh sách công nợ bàn giao
                List<List<dynamic>> detailFinanceList = SQLHelper<dynamic>.ProcedureToList(
                  "spGetHandoverFinances",
                  new string[] { "@HandoverID" },
                  new object[] { id }
                );
                //Lấy danh sách nhân viên trực thuộc bàn giao
                List<List<dynamic>> detailSubList = SQLHelper<dynamic>.ProcedureToList(
                  "spGetHandoverSubordinates",
                  new string[] { "@LeaderID" },
                  new object[] { leaderID }
                );
                //Lấy danh Người duyệt bàn giao
                List<List<dynamic>> detailApproveList = SQLHelper<dynamic>.ProcedureToList(
                  "spGetHandoverApprove",
                  new string[] { "@HandoverID" },
                  new object[] { id }
                );
                var receiverData = SQLHelper<dynamic>.GetListData(detailReceiverList, 0);
                var workData = SQLHelper<dynamic>.GetListData(detailWorkList, 0);
                var assetData = SQLHelper<dynamic>.GetListData(detailAssetList, 0);
                var warehouseAssetData = SQLHelper<dynamic>.GetListData(detailAssetWarehouseList, 0);
                var financeData = SQLHelper<dynamic>.GetListData(detailFinanceList, 0);
                var subData = SQLHelper<dynamic>.GetListData(detailSubList, 0);
                var approveData = SQLHelper<dynamic>.GetListData(detailApproveList, 0);


                ExcelPackage.License.SetNonCommercialOrganization("RTC");

                //string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "BienBanBanGiao.xlsx");

                var templateFolder = _configuration.GetValue<string>("PathTemplate");

                if (string.IsNullOrWhiteSpace(templateFolder))
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy đường dẫn thư mục {templateFolder} trên sever!!"));
                // Đường dẫn mẫu Excel
                //    string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "BanGiaoNghiViec.xlsx");
                string templatePath = Path.Combine(templateFolder, "ExportExcel", "HandoverTemplate.xlsx");
                if (!System.IO.File.Exists(templatePath))
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy File mẫu!"));
                if (!System.IO.File.Exists(templatePath))
                {
                    throw new Exception("Không tìm thấy file mẫu Excel");
                }

                // Mở file Excel mẫu
                using (var workbook = new XLWorkbook(templatePath))
                {
                    // Sử dụng sheet đúng theo template: "Sheet3 (2)"
                    var sheet = workbook.Worksheet("Sheet1");

                    // Điền thông tin header
                    DateTime dateMinutes = handoverData.HandoverDate != null ? Convert.ToDateTime(handoverData.HandoverDate) : DateTime.Now;
                    DateTime startWorkingDate = handoverData.StartWorking != null ? Convert.ToDateTime(handoverData.StartWorking) : DateTime.MinValue;

                    // Row 1: Điền mã biên bản vào L1 (hoặc K1/L1 dựa trên template, giả sử L1)
                    if (!string.IsNullOrEmpty(handoverData.Code?.ToString()))
                    {
                        var code = handoverData.Code.ToString();
                        // Thêm tiền tố nếu chưa có
                        if (!code.StartsWith("MB/RTC"))
                            code = $"MB/RTC-{code}";

                        sheet.Cell("J1").Value = code;
                    }


                    // Row 2: Điền ngày tháng năm vào A2 (xây dựng chuỗi đầy đủ)
                    string dateStr = $"Hôm nay ngày {dateMinutes.Day} / {dateMinutes.Month} / {dateMinutes.Year} , tại Văn phòng Công ty Cổ phần RTC Technology Việt Nam. Chúng tôi gồm:";
                    sheet.Cell("A2").Value = dateStr;

                    // I. Bên bàn giao
                    // Row 4: Họ và tên ở A4 (thay thế phần dots)
                    string fullName = handoverData.FullName?.ToString() ?? "";
                    sheet.Cell("A4").Value = $"Họ và tên: {fullName}";

                    // Chức vụ ở G4 (sau "Chức vụ: " ở F4)
                    string tenChucVu = handoverData.TenChucVu?.ToString() ?? "";
                    sheet.Cell("F4").Value = tenChucVu;

                    // Row 5: Bộ phận ở A5 (thay thế phần dots)
                    string departmentName = handoverData.DepartmentName?.ToString() ?? "";
                    sheet.Cell("A5").Value = $"Bộ phận: {departmentName}";

                    // Ngày vào làm việc ở F5 (thay thế placeholders)
                    if (startWorkingDate != DateTime.MinValue)
                    {
                        string startDateStr = $"Ngày vào làm việc: {startWorkingDate.Day} / {startWorkingDate.Month} / {startWorkingDate.Year}";
                        sheet.Cell("E5").Value = startDateStr;
                    }
                    int receiverStartRow = 8;
                    int maxTemplateRows = 3; // template có sẵn 3 dòng Bộ phận

                    for (int i = 0; i < maxTemplateRows; i++)
                    {
                        int rowIdx = receiverStartRow + i;

                        if (i < receiverData.Count)
                        {
                            var rowData = (IDictionary<string, object>)receiverData[i];

                            string receiverFullName =
                                rowData.ContainsKey("FullName")
                                    ? rowData["FullName"]?.ToString() ?? ""
                                    : "";

                            string receiverDepartment =
                                rowData.ContainsKey("Name")
                                    ? rowData["Name"]?.ToString() ?? ""
                                    : "";

                            // Họ và tên (chỉ dòng đầu có số thứ tự)
                            sheet.Cell(rowIdx, 1).Value =
                                i == 0
                                    ? $"1. Họ và tên: {receiverFullName}"
                                    : $"{i + 1}. Họ và tên: {receiverFullName}";

                            // Bộ phận
                            sheet.Cell(rowIdx, 6).Value =
                                $"Bộ phận: {receiverDepartment}";

                            // Style
                            for (int j = 1; j <= 12; j++)
                            {
                                var cell = sheet.Cell(rowIdx, j);
                                cell.Style.Alignment.WrapText = true;
                                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            }
                        }
                        else
                        {
                            // 🔥 CLEAR DÒNG THỪA (BỘ PHẬN RỖNG)
                            sheet.Row(rowIdx).Clear();
                        }
                    }

                    // =========================
                    // III.1 CÔNG VIỆC BÀN GIAO (FULL WRAP + HEIGHT)
                    // =========================
                    int workStartRow = 16;
                    int maxWorkRows = 5;

                    if (workData.Count > maxWorkRows)
                    {
                        sheet.Row(workStartRow + maxWorkRows - 1)
                             .InsertRowsBelow(workData.Count - maxWorkRows);
                    }

                    for (int i = 0; i < workData.Count; i++)
                    {
                        var rowData = (IDictionary<string, object>)workData[i];
                        int rowIdx = workStartRow + i;

                        sheet.Cell(rowIdx, 1).Value = i + 1;

                        // =========================
                        // NỘI DUNG CÔNG VIỆC (B)
                        // =========================
                        string contentWork =
                            rowData.ContainsKey("ContentWork")
                                ? rowData["ContentWork"]?.ToString() ?? ""
                                : "";

                        var contentCell = sheet.Cell(rowIdx, 2);
                        contentCell.Value = contentWork;

                        var contentRange = contentCell.MergedRange()
                            ?? sheet.Range(rowIdx, 2, rowIdx, 2);

                        contentRange.Style.Alignment.WrapText = true;
                        contentRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        contentRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        int contentLines =
                            (int)Math.Ceiling((double)Math.Max(contentWork.Length, 1) / 40);

                        // =========================
                        // TÌNH TRẠNG
                        // =========================
                        sheet.Cell(rowIdx, 4).Value =
                            rowData.ContainsKey("StatusText") ? rowData["StatusText"]?.ToString() ?? "" : "";

                        // =========================
                        // TẦN SUẤT
                        // =========================
                        sheet.Cell(rowIdx, 6).Value =
                            rowData.ContainsKey("Frequency") ? rowData["Frequency"]?.ToString() ?? "" : "";

                        // =========================
                        // FILE CỨNG (G)
                        // =========================
                        string fileCung =
                            rowData.ContainsKey("FileCung")
                                ? rowData["FileCung"]?.ToString() ?? ""
                                : "";

                        var fileCungCell = sheet.Cell(rowIdx, 7);
                        fileCungCell.Value = fileCung;

                        var fileCungRange = fileCungCell.MergedRange()
                            ?? sheet.Range(rowIdx, 7, rowIdx, 7);

                        fileCungRange.Style.Alignment.WrapText = true;
                        fileCungRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        fileCungRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        int fileCungLines =
                            (int)Math.Ceiling((double)Math.Max(fileCung.Length, 1) / 25);

                        // =========================
                        // FILE MỀM (H)
                        // =========================
                        string fileMem =
                            rowData.ContainsKey("FileName")
                                ? rowData["FileName"]?.ToString() ?? ""
                                : "";

                        var fileMemCell = sheet.Cell(rowIdx, 8);
                        fileMemCell.Value = fileMem;

                        var fileMemRange = fileMemCell.MergedRange()
                            ?? sheet.Range(rowIdx, 8, rowIdx, 8);

                        fileMemRange.Style.Alignment.WrapText = true;
                        fileMemRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        fileMemRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        int fileMemLines =
                            (int)Math.Ceiling((double)Math.Max(fileMem.Length, 1) / 25);

                        // =========================
                        // NGƯỜI NHẬN (I)
                        // =========================
                        string receiverName =
                            rowData.ContainsKey("FullName")
                                ? rowData["FullName"]?.ToString() ?? ""
                                : "";

                        var receiverCell = sheet.Cell(rowIdx, 9);
                        receiverCell.Value = receiverName;

                        var receiverRange = receiverCell.MergedRange()
                            ?? sheet.Range(rowIdx, 9, rowIdx, 9);

                        receiverRange.Style.Alignment.WrapText = true;
                        receiverRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        receiverRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        int receiverLines =
                            (int)Math.Ceiling((double)Math.Max(receiverName.Length, 1) / 20);

                        // =========================
                        // KÝ NHẬN
                        // =========================
                        sheet.Cell(rowIdx, 10).Value =
                            rowData.ContainsKey("IsSigned") && rowData["IsSigned"] is bool signed
                                ? (signed ? "✓" : "✗")
                                : "";

                        // =========================
                        // 🔥 HEIGHT = MAX CỦA TẤT CẢ TEXT CỘT
                        // =========================
                        int finalLines = Math.Max(
                            Math.Max(contentLines, receiverLines),
                            Math.Max(fileCungLines, fileMemLines)
                        );

                        sheet.Row(rowIdx).Height = Math.Max(
                            18,
                            finalLines * 18
                        );
                    }


                    // Clear thừa
                    //for (int i = workData.Count; i < maxWorkRows; i++)
                    //{
                    //    int rowIdx = workStartRow + i;
                    //    for (int j = 1; j <= 12; j++) sheet.Cell(rowIdx, j).Clear();
                    //}

                    // =========================
                    // III.2 TÀI SẢN HCNS (FIX AUTO WRAP + HEIGHT)
                    // =========================
                    int assetStartRow = 23;
                    int maxAssetRows = 5;
                    int templateRow = assetStartRow + maxAssetRows - 1;

                    // Lưu template row
                    var templateRange = sheet.Range(
                        templateRow,
                        1,
                        templateRow,
                        sheet.LastColumnUsed().ColumnNumber()
                    );

                    if (assetData.Count > maxAssetRows)
                    {
                        int rowsToInsert = assetData.Count - maxAssetRows;

                        sheet.Row(templateRow).InsertRowsBelow(rowsToInsert);

                        for (int i = 1; i <= rowsToInsert; i++)
                        {
                            templateRange.CopyTo(
                                sheet.Range(
                                    templateRow + i,
                                    1,
                                    templateRow + i,
                                    sheet.LastColumnUsed().ColumnNumber()
                                )
                            );
                        }
                    }

                    for (int i = 0; i < assetData.Count; i++)
                    {
                        var rowData = (IDictionary<string, object>)assetData[i];
                        int rowIdx = assetStartRow + i;

                        string assetName = rowData.ContainsKey("TSAssetName")
                            ? rowData["TSAssetName"]?.ToString() ?? ""
                            : "";

                        // STT
                        sheet.Cell(rowIdx, 1).Value = i + 1;

                        // =========================
                        // TÊN TÀI SẢN (MERGED CELL)
                        // =========================
                        var nameCell = sheet.Cell(rowIdx, 2);
                        nameCell.Value = assetName;

                        var nameRange = nameCell.MergedRange() ?? sheet.Range(rowIdx, 2, rowIdx, 2);
                        nameRange.Style.Alignment.WrapText = true;
                        nameRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        nameRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        // 👉 AUTO HEIGHT THỦ CÔNG (BẮT BUỘC VỚI MERGE)
                        int charPerLine = 35; // tùy width cột B
                        int lineCount = (int)Math.Ceiling((double)(assetName.Length == 0 ? 1 : assetName.Length) / charPerLine);
                        sheet.Row(rowIdx).Height = Math.Max(18, lineCount * 18);

                        // =========================
                        // CÁC CỘT CÒN LẠI
                        // =========================
                        sheet.Cell(rowIdx, 4).Value =
                            rowData.ContainsKey("TSCodeNCC") ? rowData["TSCodeNCC"]?.ToString() ?? "" : "";

                        var qtyCell = sheet.Cell(rowIdx, 5);
                        qtyCell.Value =
                            rowData.ContainsKey("Quantity") ? rowData["Quantity"]?.ToString() ?? "1" : "1";

                        var qtyRange = qtyCell.MergedRange() ?? sheet.Range(rowIdx, 5, rowIdx, 5);
                        qtyRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        qtyRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        qtyRange.Style.Alignment.WrapText = false;

                        sheet.Cell(rowIdx, 6).Value =
                            rowData.ContainsKey("UnitName") ? rowData["UnitName"]?.ToString() ?? "" : "";

                        sheet.Cell(rowIdx, 7).Value =
                            rowData.ContainsKey("Status") ? rowData["Status"]?.ToString() ?? "" : "";

                        string receiverName =
       rowData.ContainsKey("ReceiverName")
           ? rowData["ReceiverName"]?.ToString() ?? ""
           : "";

                        // ÉP CHIA DÒNG: Nguyễn Tuấn | Nha
                        if (!string.IsNullOrEmpty(receiverName))
                        {
                            var parts = receiverName.Split(' ');
                            if (parts.Length >= 3)
                            {
                                receiverName =
                                    string.Join(" ", parts.Take(2)) +
                                    Environment.NewLine +
                                    string.Join(" ", parts.Skip(2));
                            }
                        }

                        var receiverCell = sheet.Cell(rowIdx, 10);
                        receiverCell.Value = receiverName;

                        var receiverRange = receiverCell.MergedRange()
                            ?? sheet.Range(rowIdx, 10, rowIdx, 10);

                        receiverRange.Style.Alignment.WrapText = true;
                        receiverRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        receiverRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;


                        int receiverLineCount = receiverName.Split('\n').Length;

                        sheet.Row(rowIdx).Height = Math.Max(
                            sheet.Row(rowIdx).Height,
                            receiverLineCount * 18   // mỗi dòng ~18px
                        );
                        sheet.Cell(rowIdx, 11).Value =
                            rowData.ContainsKey("IsSigned") && rowData["IsSigned"] is bool signed
                                ? (signed ? "✓" : "✗")
                                : "";
                    }

                    // Clear thừa (nếu ít hơn, clear data cells but keep fixed labels)
                    //for (int i = assetData.Count; i < maxAssetRows; i++)
                    //{
                    //    int rowIdx = assetStartRow + i;
                    //    // Clear data columns: D,E,F,G,J,K but keep A,B if fixed
                    //    sheet.Cell(rowIdx, 4).Clear();
                    //    sheet.Cell(rowIdx, 5).Clear();
                    //    sheet.Cell(rowIdx, 6).Clear();
                    //    sheet.Cell(rowIdx, 7).Clear();
                    //    sheet.Cell(rowIdx, 10).Clear();
                    //    sheet.Cell(rowIdx, 11).Clear();
                    //}

                    // III.3 Tài sản kho (rows 38-42+)
                    int warehouseStartRow = 30;
                    int maxWarehouseRows = 5;

                    if (warehouseAssetData.Count > maxWarehouseRows)
                    {
                        sheet.Row(warehouseStartRow + maxWarehouseRows - 1).InsertRowsBelow(warehouseAssetData.Count - maxWarehouseRows);
                    }

                    for (int i = 0; i < warehouseAssetData.Count; i++)
                    {
                        var rowData = (IDictionary<string, object>)warehouseAssetData[i];
                        int rowIdx = warehouseStartRow + i;

                        // A: STT, B: Tên tài sản, D: SL, E: Đơn vị tính, F: Tình trạng, I: Người nhận, J: Ký nhận
                        sheet.Cell(rowIdx, 1).Value = i + 1;
                        sheet.Cell(rowIdx, 2).Value = rowData.ContainsKey("ProductName") ? rowData["ProductName"]?.ToString() ?? "" : "";
                        sheet.Cell(rowIdx, 4).Value = rowData.ContainsKey("BorrowQty") ? rowData["BorrowQty"]?.ToString() ?? "" : "";
                        sheet.Cell(rowIdx, 5).Value = rowData.ContainsKey("Unit") ? rowData["Unit"]?.ToString() ?? "" : "";
                        sheet.Cell(rowIdx, 6).Value = rowData.ContainsKey("ReturnedStatusText") ? rowData["ReturnedStatusText"]?.ToString() ?? "" : "";
                        sheet.Cell(rowIdx, 10).Value = rowData.ContainsKey("ReceiverName") ? rowData["ReceiverName"]?.ToString() ?? "" : "";
                        sheet.Cell(rowIdx, 11).Value = rowData.ContainsKey("IsSigned") && rowData["IsSigned"] is bool signed ? (signed ? "✓" : "✗") : "";


                        // Căn chỉnh
                        for (int j = 1; j <= 12; j++)
                        {
                            var cell = sheet.Cell(rowIdx, j);
                            if (cell.Value.IsText)
                            {
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                cell.Style.Alignment.WrapText = true;
                            }
                        }
                    }

                    // Clear thừa
                    //for (int i = warehouseAssetData.Count; i < maxWarehouseRows; i++)
                    //{
                    //    int rowIdx = warehouseStartRow + i;
                    //    for (int j = 1; j <= 12; j++) sheet.Cell(rowIdx, j).Clear();
                    //}

                    // III.4 Tài chính (rows 45-49+)
                    int financeStartRow = 37;
                    int maxFinanceRows = 5;

                    if (financeData.Count > maxFinanceRows)
                    {
                        sheet.Row(financeStartRow + maxFinanceRows - 1).InsertRowsBelow(financeData.Count - maxFinanceRows);
                    }

                    for (int i = 0; i < financeData.Count; i++)
                    {
                        var rowData = (IDictionary<string, object>)financeData[i];
                        int rowIdx = financeStartRow + i;

                        // A: STT, B: Tồn tại về tài chính, F: Kế toán theo dõi, I: Kế toán trưởng
                        sheet.Cell(rowIdx, 1).Value = i + 1;
                        sheet.Cell(rowIdx, 2).Value = rowData.ContainsKey("DebtType") ? rowData["DebtType"]?.ToString() ?? "" : "";
                        sheet.Cell(rowIdx, 6).Value = rowData.ContainsKey("FullName") ? rowData["FullName"]?.ToString() ?? "" : "";
                        sheet.Cell(rowIdx, 9).Value = rowData.ContainsKey("Accountant") ? rowData["Accountant"]?.ToString() ?? "" : "";

                        // Căn chỉnh
                        for (int j = 1; j <= 12; j++)
                        {
                            var cell = sheet.Cell(rowIdx, j);
                            if (cell.Value.IsText)
                            {
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                cell.Style.Alignment.WrapText = true;
                            }
                        }
                    }

                    // Clear thừa
                    //for (int i = financeData.Count; i < maxFinanceRows; i++)
                    //{
                    //    int rowIdx = financeStartRow + i;
                    //    for (int j = 1; j <= 12; j++) sheet.Cell(rowIdx, j).Clear();
                    //}

                    // III.5 Nhân sự trực thuộc (rows 51-54+)
                    int subStartRow = 43;
                    int maxSubRows = 4;

                    if (subData.Count > maxSubRows)
                    {
                        sheet.Row(subStartRow + maxSubRows - 1).InsertRowsBelow(subData.Count - maxSubRows);
                    }

                    for (int i = 0; i < subData.Count; i++)
                    {
                        var rowData = (IDictionary<string, object>)subData[i];
                        int rowIdx = subStartRow + i;

                        // A: STT, B: Vị trí, D: SL, E: Người đảm nhận, G: Người nhận bàn giao, I: Ký nhận
                        sheet.Cell(rowIdx, 1).Value = i + 1;
                        sheet.Cell(rowIdx, 2).Value = rowData.ContainsKey("SubordinateFullName") ? rowData["SubordinateFullName"]?.ToString() ?? "" : "";
                        sheet.Cell(rowIdx, 4).Value = rowData.ContainsKey("Name") ? rowData["Name"]?.ToString() ?? "" : "";
                        sheet.Cell(rowIdx, 5).Value = rowData.ContainsKey("AssigneeFullName") ? rowData["AssigneeFullName"]?.ToString() ?? "" : "";
                        sheet.Cell(rowIdx, 7).Value = rowData.ContainsKey("ReceiverFullName") ? rowData["ReceiverFullName"]?.ToString() ?? "" : "";
                        sheet.Cell(rowIdx, 9).Value = rowData.ContainsKey("KyNhan") ? rowData["KyNhan"]?.ToString() ?? "" : "";

                        // Căn chỉnh
                        for (int j = 1; j <= 12; j++)
                        {
                            var cell = sheet.Cell(rowIdx, j);
                            if (cell.Value.IsText)
                            {
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                cell.Style.Alignment.WrapText = true;
                            }
                        }
                    }

                    // Clear thừa
                    //for (int i = subData.Count; i < maxSubRows; i++)
                    //{
                    //    int rowIdx = subStartRow + i;
                    //    for (int j = 1; j <= 12; j++) sheet.Cell(rowIdx, j).Clear();
                    //}

                    // Phần chữ ký (row 57 dưới labels row 56)
                    // B57: Tên người bàn giao (ghi rõ họ tên)
                    // Người bàn giao ở B50
                    //sheet.Cell("B52").Value = handoverData.FullName?.ToString() ?? "";
                    // =========================
                    // MERGE + ĐIỀN CHỮ KÝ
                    // =========================
                    int signRow = 61; // dòng chữ ký

                    if (approveData != null && approveData.Any())
                    {
                        foreach (var item in approveData)
                        {
                            var dict = (IDictionary<string, object>)item;

                            int approveLevel =
                                dict.ContainsKey("ApproveLevel") && dict["ApproveLevel"] != null
                                    ? Convert.ToInt32(dict["ApproveLevel"])
                                    : -1;

                            string employeeName =
                                dict.ContainsKey("EmployeeName")
                                    ? dict["EmployeeName"]?.ToString    () ?? ""
                                    : "";

                            if (string.IsNullOrWhiteSpace(employeeName))
                                continue;

                            IXLRange signRange = null;

                            switch (approveLevel)
                            {
                                case 0:
                                    // NGƯỜI BÀN GIAO → B:C
                                    signRange = sheet.Range(signRow, 2, signRow, 3);
                                    break;

                                case 1:
                                    // TRƯỞNG BỘ PHẬN → E:F:G
                                    signRange = sheet.Range(signRow, 5, signRow, 7);
                                    break;

                                case 2:
                                    // TRƯỞNG PHÒNG HCNS → I:J
                                    signRange = sheet.Range(signRow, 9, signRow, 10);
                                    break;
                            }

                            if (signRange == null) continue;

                            // ===== MERGE =====
                            if (!signRange.IsMerged())
                                signRange.Merge();

                            // ===== GÁN GIÁ TRỊ =====
                            signRange.Value = employeeName;

                            // ===== FORMAT =====
                            signRange.Style.Font.Bold = true;
                            signRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            signRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            signRange.Style.Alignment.WrapText = true;

                            // ===== FIX HEIGHT (TÊN 2 DÒNG KHÔNG BỊ CẮT) =====
                            int lineCount = employeeName.Split(' ').Length >= 3 ? 2 : 1;

                            sheet.Row(signRow).Height = Math.Max(
                                sheet.Row(signRow).Height,
                                lineCount * 20
                            );
                        }
                    }





                    // Lưu file vào stream
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Position = 0;
                        var fileName = $"{handoverData.Code?.ToString() ?? "BanGiaoNghiViec"}_{DateTime.Now:yyyy-MM-dd}.xlsx";
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("approved")]
        public async Task<IActionResult> HandoverApproved([FromBody] List<HandoverApprove> actionApproveds)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                if (actionApproveds == null || !actionApproveds.Any())
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu duyệt."));

                var handoverId = actionApproveds.First().HandoverID;
                if (handoverId == null || handoverId == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Thiếu HandoverID."));

                var approves = _approveRepo.GetAll(x => x.HandoverID == handoverId)
                                           .OrderBy(x => x.STT)
                                           .ToList();

                if (!approves.Any())
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        var newApprove = new HandoverApprove
                        {
                            HandoverID = handoverId.Value,
                            STT = i,
                            ApproveLevel = i, // ApproveLevel 1,2,3
                            ApproveStatus = 0,
                            Note = "",
                            CreatedBy = currentUser.LoginName,
                            UpdatedBy = currentUser.LoginName
                        };
                        await _approveRepo.CreateAsync(newApprove);
                        approves.Add(newApprove);
                    }
                }

                // mapping STT → chức danh
                string GetRoleTitle(int stt) => stt switch
                {
                    1 => "Người bàn giao",
                    2 => "Trưởng bộ phận",
                    3 => "Trưởng phòng HCNS",
                    _ => $"Cấp STT {stt}"
                };

                var sortedActions = actionApproveds.OrderBy(x => x.STT).ToList();

                foreach (var action in sortedActions)
                {
                    if (action.STT == null)
                        return BadRequest(ApiResponseFactory.Fail(null, "Thiếu STT."));

                    int currentSTT = action.STT.Value;
                    int approveStatus = action.ApproveStatus ?? 0;

                    if (approveStatus != 1 && approveStatus != 2)
                        return BadRequest(ApiResponseFactory.Fail(null, "ApproveStatus không hợp lệ. (1: Duyệt, 2: Hủy duyệt)"));

                    var currentApprove = approves.FirstOrDefault(x => x.STT == currentSTT);
                    if (currentApprove == null)
                        return NotFound(ApiResponseFactory.Fail(null, $"Không tìm thấy cấp duyệt STT={currentSTT}."));

                    if (approveStatus == 1)
                    {
                        if (currentSTT > 1)
                        {
                            var prev = approves.FirstOrDefault(x => x.STT == currentSTT - 1);
                            if (prev == null || prev.ApproveStatus != 1)
                            {
                                string prevRole = GetRoleTitle(currentSTT - 1);
                                return BadRequest(ApiResponseFactory.Fail(null, $"{prevRole} chưa duyệt."));
                            }
                        }

                        currentApprove.ApproveStatus = 1;
                        currentApprove.Note = action.Note ?? "";
                        currentApprove.UpdatedDate = DateTime.Now;

                        currentApprove.ApproveDate = DateTime.Now;
                        currentApprove.ApproverID = currentUser.EmployeeID;
                        //currentApprove.RejectReason = action.RejectReason ?? "";

                        await _approveRepo.UpdateAsync(currentApprove);
                    }
                    else if (approveStatus == 2)
                    {
                        var next = approves.FirstOrDefault(x => x.STT == currentSTT + 1);
                        if (next != null && next.ApproveStatus == 1)
                        {
                            string nextRole = GetRoleTitle(currentSTT + 1);
                            return BadRequest(ApiResponseFactory.Fail(null, $"Không thể hủy duyệt vì {nextRole} đã duyệt."));
                        }

                        currentApprove.ApproveStatus = 2;
                        currentApprove.Note = action.Note ?? "";
                        currentApprove.UpdatedDate = DateTime.Now;

                        currentApprove.ApproveDate = DateTime.Now;
                        currentApprove.ApproverID = currentUser.EmployeeID;
                        currentApprove.RejectReason = action.RejectReason ?? "";
                        await _approveRepo.UpdateAsync(currentApprove);
                    }

                    //var handover = _handoverRepo.GetByID(handoverId.Value);
                    //if (handover != null)
                    //{
                    //    handover.IsApprove = approves.All(x => x.ApproveStatus == 1);
                    //    handover.UpdatedDate = DateTime.Now;
                    //    await _handoverRepo.UpdateAsync(handover);
                    //}

                    var handover = _handoverRepo.GetByID(handoverId.Value);
                    if (handover != null)
                    {
                        bool isAllApproved = approves.All(x => x.ApproveStatus == 1);

                        // nếu vừa chuyển sang trạng thái duyệt hết
                        if (isAllApproved && handover.IsApprove != true)
                        {
                            await UpdateEmployeeForAssets(handoverId.Value);
                        }

                        handover.IsApprove = isAllApproved;
                        handover.UpdatedDate = DateTime.Now;
                        await _handoverRepo.UpdateAsync(handover);
                    }

                }

                return Ok(ApiResponseFactory.Success(null, "Duyệt theo trình tự thành công!"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseFactory.Fail(null, ex.Message));
            }
        }

        private async Task UpdateEmployeeForAssets(int handoverId)
        {
            var assets = _handoverAssetManagementRepo.GetAll()
                .Where(x => x.HandoverID == handoverId && x.IsDeleted == false)
                .ToList();

            foreach (var item in assets)
            {
                if (item.EmployeeID == null) continue;

                var asset = _tsAssetManagementRepo.GetAll()
                    .FirstOrDefault(x => x.TSCodeNCC == item.TSAssetCode && x.IsDeleted == false);

                if (asset != null)
                {
                    asset.EmployeeID = item.EmployeeID;
                    asset.UpdatedDate = DateTime.Now;
                    await _tsAssetManagementRepo.UpdateAsync(asset);
                }
            }
        }




    }
}
