using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.DTO.ESL;
using RERPAPI.Model.Entities.ESL;
using RERPAPI.Repo.GenericEntity.ESL;
using RERPAPI.Repo.GenericEntity;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using RERPAPI.Model.Common;

namespace RERPAPI.Controllers.ESL
{
    [Route("api/[controller]")]
    [ApiController]
    public class ESLRegistrationController : ControllerBase
    {
        private readonly ESLTestTableRegistrationRepo _registrationRepo;
        private readonly ESLTestTableRegistrationDetailRepo _detailRepo;
        private readonly ESLTestTableRegistrationLogRepo _logRepo;
        private readonly ESLTestTableRepo _testTableRepo;
        private readonly EmployeeRepo _employeeRepo;
        private readonly ESLConfigRepo _configRepo;
        private readonly IESLBindService _eslBindService;

        public ESLRegistrationController(
            ESLTestTableRegistrationRepo registrationRepo,
            ESLTestTableRegistrationDetailRepo detailRepo,
            ESLTestTableRegistrationLogRepo logRepo,
            ESLTestTableRepo testTableRepo,
            EmployeeRepo employeeRepo,
            ESLConfigRepo configRepo,
            IESLBindService eslBindService)
        {
            _registrationRepo = registrationRepo;
            _detailRepo = detailRepo;
            _logRepo = logRepo;
            _testTableRepo = testTableRepo;
            _employeeRepo = employeeRepo;
            _configRepo = configRepo;
            _eslBindService = eslBindService;
        }

        [HttpGet("getall")]
        public IActionResult GetAll(string keyword = "", int? status = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var masters = _registrationRepo.GetAll().ToList();
                var details = _detailRepo.GetAll().ToList();
                var tables = _testTableRepo.GetAll().ToList();
                var employees = _employeeRepo.GetAll().ToList();

                var list = new List<ESLRegistrationListDto>();

                foreach (var master in masters)
                {
                    // Find the "current" or "latest" detail for this master
                    // Order by No desc, if there's a pending one (status 0), it usually takes precedence in display, or just take the max No
                    var latestDetail = details.Where(d => d.RegistrationID == master.ID).OrderByDescending(d => d.No).FirstOrDefault();
                    if (latestDetail == null) continue;

                    var table = tables.FirstOrDefault(t => t.ID == master.TestTableID);
                    var owner = employees.FirstOrDefault(e => e.ID == latestDetail.OwnerID);
                    var approver = employees.FirstOrDefault(e => e.ID == latestDetail.ApproverID);

                    // Apply filters on the detail
                    if (status.HasValue && latestDetail.Status != status.Value) continue;
                    if (startDate.HasValue && latestDetail.EndDate < startDate.Value) continue;
                    if (endDate.HasValue && latestDetail.StartDate > endDate.Value) continue;

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        var kw = keyword.ToLower();
                        if (!(master.RegistrationCode?.ToLower().Contains(kw) == true ||
                              master.RegistrationContent?.ToLower().Contains(kw) == true ||
                              master.ProjectCode?.ToLower().Contains(kw) == true))
                        {
                            continue;
                        }
                    }

                    list.Add(new ESLRegistrationListDto
                    {
                        RegistrationID = master.ID,
                        RegistrationCode = master.RegistrationCode,
                        TestTableID = master.TestTableID,
                        TestTableName = table?.TestTableName ?? "",
                        Barcode = table?.Barcode ?? "",
                        TableSide = table?.TableSide ?? 1,
                        ProjectCode = master.ProjectCode,
                        RegistrationContent = master.RegistrationContent,

                        DetailID = latestDetail.ID,
                        Type = latestDetail.Type,
                        No = latestDetail.No,
                        OwnerID = latestDetail.OwnerID,
                        OwnerName = owner?.FullName ?? "",
                        ApproverID = latestDetail.ApproverID,
                        ApproverName = approver?.FullName ?? "",
                        StartDate = latestDetail.StartDate,
                        EndDate = latestDetail.EndDate,
                        ActualReturnDate = latestDetail.ActualReturnDate,
                        Status = latestDetail.Status,
                        
                        CreatedDate = master.CreatedDate
                    });
                }

                return Ok(ApiResponseFactory.Success(list.OrderByDescending(x => x.RegistrationID).ToList()));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("check-conflict")]
        public IActionResult CheckConflict([FromBody] ESLCheckConflictRequest request)
        {
            try
            {
                // Find all masters for this table
                var masters = _registrationRepo.GetAll(x => x.TestTableID == request.TestTableId).Select(x => x.ID).ToList();

                // Find active details for these masters
                var activeDetails = _detailRepo.GetAll(d => masters.Contains(d.RegistrationID) && d.Status != 2);

                if (request.ExcludeDetailId.HasValue)
                {
                    activeDetails = activeDetails.Where(d => d.ID != request.ExcludeDetailId.Value).ToList();
                }

                bool hasConflict = activeDetails.Any(d => {
                    // Effective end date is ActualReturnDate if returned, else EndDate
                    var effectiveEnd = d.ActualReturnDate ?? d.EndDate;
                    return request.StartDate <= effectiveEnd && request.EndDate >= d.StartDate;
                });

                if (hasConflict)
                    return Ok(ApiResponseFactory.Fail(null, "Thời gian đăng ký bị trùng lặp với lượt đăng ký khác."));

                return Ok(ApiResponseFactory.Success(null, "Không trùng lặp."));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save")]
        public IActionResult Save([FromBody] ESLRegistrationRequest req)
        {
            try
            {
                var testTable = _testTableRepo.GetByID(req.TestTableID);
                if (testTable == null || testTable.ID == 0) return Ok(ApiResponseFactory.Fail(null, "Bàn test không tồn tại"));

                var master = new ESLTestTableRegistration
                {
                    RegistrationCode = $"DK-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}",
                    TestTableID = req.TestTableID,
                    StartDate = req.StartDate,
                    ProjectCode = req.ProjectCode,
                    RegistrationContent = req.RegistrationContent,
                    CreatedDate = DateTime.Now
                };

                _registrationRepo.Create(master);

                var detail = new ESLTestTableRegistrationDetail
                {
                    RegistrationID = master.ID,
                    No = 1,
                    Type = 1, // Đăng ký
                    StartDate = req.StartDate,
                    EndDate = req.EndDate,
                    OwnerID = req.OwnerID,
                    ApproverID = req.ApproverID,
                    Status = 0, // Chờ duyệt
                    CreatedDate = DateTime.Now
                };

                _detailRepo.Create(detail);

                _logRepo.Create(new ESLTestTableRegistrationLog
                {
                    RegistrationID = master.ID, Action = "CREATE", ActionBy = req.OwnerID, ActionDate = DateTime.Now, NewStatus = 0
                });

                return Ok(ApiResponseFactory.Success(null, "Tạo đăng ký thành công"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("extend-handover")]
        public IActionResult ExtendOrHandover([FromBody] ESLExtendHandoverRequest req)
        {
            try
            {
                var details = _detailRepo.GetAll(d => d.RegistrationID == req.RegistrationID).ToList();
                if (details.Count == 0) return Ok(ApiResponseFactory.Fail(null, "Không tìm thấy đăng ký"));
                if (details.Count >= 3) return Ok(ApiResponseFactory.Fail(null, "Chỉ được gia hạn / bàn giao tối đa 2 lần (Tổng 3 Detail)"));

                var latest = details.OrderByDescending(d => d.No).First();
                if (latest.Status != 1) return Ok(ApiResponseFactory.Fail(null, "Lượt đăng ký trước đó chưa được duyệt hoặc đã bị từ chối."));
                if (latest.ActualReturnDate != null) return Ok(ApiResponseFactory.Fail(null, "Bàn đã được trả, không thể gia hạn / bàn giao."));

                var newDetail = new ESLTestTableRegistrationDetail
                {
                    RegistrationID = req.RegistrationID,
                    No = latest.No + 1,
                    Type = req.Type, // 2 = Gia hạn, 3 = Bàn giao
                    StartDate = latest.EndDate,
                    EndDate = latest.EndDate.AddDays(7), // Max 7 days
                    OwnerID = req.OwnerID,
                    ApproverID = req.ApproverID,
                    Status = 0, // Chờ duyệt
                    CreatedDate = DateTime.Now
                };

                _detailRepo.Create(newDetail);

                _logRepo.Create(new ESLTestTableRegistrationLog
                {
                    RegistrationID = req.RegistrationID, Action = req.Type == 2 ? "EXTEND" : "HANDOVER", ActionBy = req.OwnerID, ActionDate = DateTime.Now, NewStatus = 0
                });

                return Ok(ApiResponseFactory.Success(null, "Gửi yêu cầu thành công, đang chờ duyệt."));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("return")]
        public async Task<IActionResult> ReturnTable([FromBody] ESLReturnRequest request)
        {
            try
            {
                var details = _detailRepo.GetAll(d => d.RegistrationID == request.RegistrationID).ToList();
                if (details.Count == 0) return Ok(ApiResponseFactory.Fail(null, "Không tìm thấy đăng ký"));
                
                var latest = details.OrderByDescending(d => d.No).First();
                if (latest.Status != 1) return Ok(ApiResponseFactory.Fail(null, "Không có lượt duyệt nào để trả."));
                if (latest.ActualReturnDate != null) return Ok(ApiResponseFactory.Fail(null, "Bàn này đã được trả trước đó."));

                latest.ActualReturnDate = DateTime.Now;
                _detailRepo.Update(latest);

                var master = _registrationRepo.GetByID(request.RegistrationID);
                var table = _testTableRepo.GetByID(master.TestTableID);
                var bindResponse = await SyncESLProductAsync(table.Barcode);

                _logRepo.Create(new ESLTestTableRegistrationLog
                {
                    RegistrationID = request.RegistrationID, Action = "RETURN_AND_BIND", ActionBy = request.ReturnBy, ActionDate = DateTime.Now, NewStatus = 4,
                    APIResponse = System.Text.Json.JsonSerializer.Serialize(bindResponse)
                });

                return Ok(ApiResponseFactory.Success(null, "Trả bàn thành công và đã cập nhật ESL"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("approve")]
        public async Task<IActionResult> Approve([FromBody] ESLApproveRequest request)
        {
            try
            {
                var detail = _detailRepo.GetByID(request.DetailId);
                if (detail == null) return Ok(ApiResponseFactory.Fail(null, "Bản ghi không tồn tại"));
                if (detail.Status != 0) return Ok(ApiResponseFactory.Fail(null, "Trạng thái không hợp lệ"));

                detail.ApproveDate = DateTime.Now;
                detail.ApproveNote = request.Note;
                
                if (!request.IsApproved)
                {
                    detail.Status = 2; // Từ chối
                    _detailRepo.Update(detail);
                    
                    _logRepo.Create(new ESLTestTableRegistrationLog
                    {
                        RegistrationID = detail.RegistrationID, Action = "REJECT", ActionBy = request.ApproverId, Note = request.Note, NewStatus = 2
                    });

                    return Ok(ApiResponseFactory.Success(null, "Đã từ chối"));
                }

                detail.Status = 1; // Đã duyệt
                _detailRepo.Update(detail);

                var master = _registrationRepo.GetByID(detail.RegistrationID);
                var table = _testTableRepo.GetByID(master.TestTableID);
                var bindResponse = await SyncESLProductAsync(table.Barcode);

                _logRepo.Create(new ESLTestTableRegistrationLog
                {
                    RegistrationID = detail.RegistrationID, Action = "APPROVE_AND_BIND", ActionBy = request.ApproverId, Note = request.Note, NewStatus = 1,
                    APIResponse = System.Text.Json.JsonSerializer.Serialize(bindResponse)
                });

                return Ok(ApiResponseFactory.Success(null, "Đã duyệt và gửi lệnh Bind ESL"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("pending")]
        public IActionResult GetPendingApproval(int approverId)
        {
            try
            {
                var pendingDetails = _detailRepo.GetAll(x => x.Status == 0 && x.ApproverID == approverId).ToList();
                var masters = _registrationRepo.GetAll().ToList();
                var tables = _testTableRepo.GetAll().ToList();
                var employees = _employeeRepo.GetAll().ToList();

                var list = new List<ESLRegistrationListDto>();

                foreach(var detail in pendingDetails)
                {
                    var master = masters.FirstOrDefault(m => m.ID == detail.RegistrationID);
                    if(master == null) continue;

                    var table = tables.FirstOrDefault(t => t.ID == master.TestTableID);
                    var owner = employees.FirstOrDefault(e => e.ID == detail.OwnerID);
                    var approver = employees.FirstOrDefault(e => e.ID == detail.ApproverID);

                    list.Add(new ESLRegistrationListDto
                    {
                        RegistrationID = master.ID,
                        RegistrationCode = master.RegistrationCode,
                        TestTableID = master.TestTableID,
                        TestTableName = table?.TestTableName ?? "",
                        Barcode = table?.Barcode ?? "",
                        TableSide = table?.TableSide ?? 1,
                        ProjectCode = master.ProjectCode,
                        RegistrationContent = master.RegistrationContent,

                        DetailID = detail.ID,
                        Type = detail.Type,
                        No = detail.No,
                        OwnerID = detail.OwnerID,
                        OwnerName = owner?.FullName ?? "",
                        ApproverID = detail.ApproverID,
                        ApproverName = approver?.FullName ?? "",
                        StartDate = detail.StartDate,
                        EndDate = detail.EndDate,
                        ActualReturnDate = detail.ActualReturnDate,
                        Status = detail.Status,
                        
                        CreatedDate = master.CreatedDate
                    });
                }

                return Ok(ApiResponseFactory.Success(list.OrderByDescending(x => x.DetailID).ToList()));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-details")]
        public IActionResult GetDetails(int registrationId)
        {
            try
            {
                var details = _detailRepo.GetAll(d => d.RegistrationID == registrationId).OrderBy(d => d.No).ToList();
                var employees = _employeeRepo.GetAll().ToList();
                var result = new List<object>();

                foreach (var detail in details)
                {
                    var owner = employees.FirstOrDefault(e => e.ID == detail.OwnerID);
                    var approver = employees.FirstOrDefault(e => e.ID == detail.ApproverID);

                    result.Add(new
                    {
                        detail.ID,
                        detail.RegistrationID,
                        detail.No,
                        detail.Type,
                        detail.StartDate,
                        detail.EndDate,
                        detail.ActualReturnDate,
                        detail.OwnerID,
                        OwnerName = owner?.FullName ?? "",
                        detail.ApproverID,
                        ApproverName = approver?.FullName ?? "",
                        detail.Status,
                        detail.ApproveDate,
                        detail.ApproveNote,
                        detail.CreatedDate
                    });
                }

                return Ok(ApiResponseFactory.Success(result));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        private async Task<ESLBindResponse> SyncESLProductAsync(string barcode)
        {
            var tables = _testTableRepo.GetAll(x => x.Barcode == barcode).ToList();
            if(!tables.Any()) return new ESLBindResponse { code = 404, message = "Không tìm thấy bàn test theo Barcode" };

            var tableIds = tables.Select(t => t.ID).ToList();
            
            // Find all active masters for these tables. An active master has a latest detail with Status = 1 and ActualReturnDate = null and EndDate >= Today
            var activeMasters = new List<ESLTestTableRegistration>();
            var activeDetails = new Dictionary<int, ESLTestTableRegistrationDetail>();

            var allMasters = _registrationRepo.GetAll(x => tableIds.Contains(x.TestTableID)).ToList();
            var allDetails = _detailRepo.GetAll().ToList();

            foreach(var m in allMasters)
            {
                var latest = allDetails.Where(d => d.RegistrationID == m.ID).OrderByDescending(d => d.No).FirstOrDefault();
                if (latest != null && latest.Status == 1 && latest.ActualReturnDate == null && latest.EndDate >= DateTime.Today)
                {
                    activeMasters.Add(m);
                    activeDetails[m.ID] = latest;
                }
            }

            var payload = new Dictionary<string, object>();
            
            var configs = _configRepo.GetAll();
            payload["store_code"] = configs.Find(x => x.ConfigKey == "ESL_STORE_CODE")?.ConfigValue ?? "rtc01";
            payload["is_base64"] = configs.Find(x => x.ConfigKey == "ESL_IS_BASE64")?.ConfigValue ?? "0";
            payload["sign"] = configs.Find(x => x.ConfigKey == "ESL_SIGN")?.ConfigValue ?? "80805d794841f1b4";
            payload["pc"] = barcode;
            
            var table1 = tables.FirstOrDefault(x => x.TableSide == 1);
            var table2 = tables.FirstOrDefault(x => x.TableSide == 2);
            
            payload["pn"] = table1?.TestTableName?.Replace(" - Mặt 1", "") ?? table2?.TestTableName?.Replace(" - Mặt 2", "") ?? "Bàn Test";
            payload["extend"] = new object();

            void FillSide(int side, ESLTestTableRegistration reg, ESLTestTableRegistrationDetail det)
            {
                string empReg = "", empApp = "";
                if(det != null) {
                    var rE = _employeeRepo.GetByID(det.OwnerID);
                    var aE = _employeeRepo.GetByID(det.ApproverID);
                    empReg = rE?.FullName ?? "";
                    empApp = aE?.FullName ?? "";
                }

                int offset = side == 1 ? 0 : 14; 
                
                payload[$"f{1 + offset}"] = reg != null ? (side == 1 ? (table1?.TestTableName ?? "Bàn 1") : (table2?.TestTableName ?? "Bàn 2")) : "";
                payload[$"f{2 + offset}"] = reg?.ProjectCode ?? "";
                payload[$"f{3 + offset}"] = reg?.RegistrationContent ?? "";
                payload[$"f{4 + offset}"] = det != null ? det.StartDate.ToString("dd/MM/yyyy") : "";
                payload[$"f{5 + offset}"] = det != null ? det.EndDate.ToString("dd/MM/yyyy") : "";
                payload[$"f{6 + offset}"] = empReg;
                payload[$"f{7 + offset}"] = empApp;
                
                for(int i = 8; i <= (side == 1 ? 14 : 13); i++) {
                    payload[$"f{i + offset}"] = "";
                }
            }

            var reg1 = activeMasters.FirstOrDefault(x => table1 != null && x.TestTableID == table1.ID);
            var det1 = reg1 != null ? activeDetails[reg1.ID] : null;

            var reg2 = activeMasters.FirstOrDefault(x => table2 != null && x.TestTableID == table2.ID);
            var det2 = reg2 != null ? activeDetails[reg2.ID] : null;

            FillSide(1, reg1, det1);
            FillSide(2, reg2, det2);

            return await _eslBindService.UpdateProductAsync(payload);
        }
    }
}
