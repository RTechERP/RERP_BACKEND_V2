using Microsoft.AspNetCore.Mvc;
using RERPAPI.Middleware;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM;
using RERPAPI.Repo.GenericEntity.HRM.Vehicle;
using System.Reflection;
using static RERPAPI.Model.DTO.ApproveTPDTO;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApproveController : ControllerBase
    {

        private readonly RTCContext _context;
        private readonly IConfiguration _configuration;

        private readonly EmployeeOnLeaveRepo _employeeOnleaveRepo;              // Type 1
        private readonly EmployeeEarlyLateRepo _employeeEarlyLateRepo;            // Type 2
        private readonly EmployeeOverTimeRepo _employeeOTRepo;                    // Type 3
        private readonly EmployeeBussinessRepo _employeeBussinessRepo;            // Type 4
        private readonly EmployeeWFHRepo _wfhRepo;                                // Type 5
        private readonly EmployeeNoFingerprintRepo _employeeNoFingerprintRepo;    // Type 6
                                                                                  //  private readonly EmployeeSalaryAdvanceRepo _employeeSalaryAdvance;            // Type 7
        private readonly EmployeeNightShiftRepo _employeeNightShiftRepo;          // Type 8
        private readonly VehicleBookingManagementRepo _vehicleBookingManagementRepo; // Type 9

        private readonly vUserGroupLinksRepo _vUserGroupLinksRepo;
        private readonly RoleConfig _roleConfig;
        private readonly EmployeeOnLeaveRepo _onLeaveRepo;
        private readonly ConfigSystemRepo _configSystemRepo;
        private readonly Dictionary<ApproveType, Func<ApproveItemParam, bool, Task>> _approveMapTP;
        private readonly Dictionary<ApproveType, Func<ApproveItemParam, Task>> _approveMapSenior;
        private readonly Dictionary<ApproveType, Func<ApproveItemParam, bool, Task>> _approveMapBGD;

        public ApproveController(
      RTCContext context,
      IConfiguration configuration,
      EmployeeOnLeaveRepo employeeOnleaveRepo,
      EmployeeEarlyLateRepo employeeEarlyLateRepo,
      EmployeeOverTimeRepo employeeOTRepo,
      EmployeeBussinessRepo employeeBussinessRepo,
      EmployeeWFHRepo wfhRepo,
      EmployeeNoFingerprintRepo employeeNoFingerprintRepo,
      EmployeeNightShiftRepo employeeNightShiftRepo,
      VehicleBookingManagementRepo vehicleBookingManagementRepo,
      vUserGroupLinksRepo vUserGroupLinksRepo,
      RoleConfig roleConfig,
      EmployeeOnLeaveRepo onLeaveRepo,
      ConfigSystemRepo configSystemRepo
  )
        {
            _context = context;
            _configuration = configuration;

            _employeeOnleaveRepo = employeeOnleaveRepo;
            _employeeEarlyLateRepo = employeeEarlyLateRepo;
            _employeeOTRepo = employeeOTRepo;
            _employeeBussinessRepo = employeeBussinessRepo;
            _wfhRepo = wfhRepo;
            _employeeNoFingerprintRepo = employeeNoFingerprintRepo;
            _employeeNightShiftRepo = employeeNightShiftRepo;
            _vehicleBookingManagementRepo = vehicleBookingManagementRepo;
            _vUserGroupLinksRepo = vUserGroupLinksRepo;
            _roleConfig = roleConfig;
            _onLeaveRepo = onLeaveRepo;
            _configSystemRepo = configSystemRepo;

            // ===== TP =====
            _approveMapTP = new()
            {
                [ApproveType.Onleave] = (item, ok) =>
                    _employeeOnleaveRepo.UpdateAsync(MapApproveTP<EmployeeOnLeave>(item, ok)),

                [ApproveType.EarlyLate] = (item, ok) =>
                    _employeeEarlyLateRepo.UpdateAsync(MapApproveTP<EmployeeEarlyLate>(item, ok)),

                [ApproveType.OT] = (item, ok) =>
                    _employeeOTRepo.UpdateAsync(MapApproveTP<EmployeeOvertime>(item, ok)),

                [ApproveType.Bussiness] = (item, ok) =>
                    _employeeBussinessRepo.UpdateAsync(MapApproveTP<EmployeeBussiness>(item, ok)),

                [ApproveType.WFH] = (item, ok) =>
                    _wfhRepo.UpdateAsync(MapApproveTP<EmployeeWFH>(item, ok)),

                [ApproveType.NoFingerprint] = (item, ok) =>
                    _employeeNoFingerprintRepo.UpdateAsync(MapApproveTP<EmployeeNoFingerprint>(item, ok)),

                [ApproveType.NightShift] = (item, ok) =>
                    _employeeNightShiftRepo.UpdateAsync(MapApproveTP<EmployeeNighShift>(item, ok)),

                [ApproveType.VehicleBooking] = (item, ok) =>
                    _vehicleBookingManagementRepo.UpdateAsync(
                        MapApproveTP<VehicleBookingManagement>(item, ok))
            };

            // ===== SENIOR =====
            _approveMapSenior = new()
            {
                [ApproveType.Onleave] = item =>
                    _employeeOnleaveRepo.UpdateAsync(MapApproveSenior<EmployeeOnLeave>(item)),

                [ApproveType.EarlyLate] = item =>
                    _employeeEarlyLateRepo.UpdateAsync(MapApproveSenior<EmployeeEarlyLate>(item)),

                [ApproveType.OT] = item =>
                    _employeeOTRepo.UpdateAsync(MapApproveSenior<EmployeeOvertime>(item)),

                [ApproveType.Bussiness] = item =>
                    _employeeBussinessRepo.UpdateAsync(MapApproveSenior<EmployeeBussiness>(item)),

                [ApproveType.WFH] = item =>
                    _wfhRepo.UpdateAsync(MapApproveSenior<EmployeeWFH>(item)),

                [ApproveType.NoFingerprint] = item =>
                    _employeeNoFingerprintRepo.UpdateAsync(MapApproveSenior<EmployeeNoFingerprint>(item)),

                [ApproveType.NightShift] = item =>
                    _employeeNightShiftRepo.UpdateAsync(MapApproveSenior<EmployeeNighShift>(item)),

                [ApproveType.VehicleBooking] = item =>
                    _vehicleBookingManagementRepo.UpdateAsync(
                        MapApproveSenior<VehicleBookingManagement>(item))
            };

            // ===== BGD =====
            _approveMapBGD = new()
            {
                [ApproveType.Onleave] = (item, isApproved) =>
                    _employeeOnleaveRepo.UpdateAsync(MapApproveBGD<EmployeeOnLeave>(item, isApproved)),

                [ApproveType.EarlyLate] = (item, isApproved) =>
                    _employeeEarlyLateRepo.UpdateAsync(MapApproveBGD<EmployeeEarlyLate>(item, isApproved)),

                [ApproveType.OT] = (item, isApproved) =>
                    _employeeOTRepo.UpdateAsync(MapApproveBGD<EmployeeOvertime>(item, isApproved)),

                [ApproveType.Bussiness] = (item, isApproved) =>
                    _employeeBussinessRepo.UpdateAsync(MapApproveBGD<EmployeeBussiness>(item, isApproved)),

                [ApproveType.WFH] = (item, isApproved) =>
                    _wfhRepo.UpdateAsync(MapApproveBGD<EmployeeWFH>(item, isApproved)),

                [ApproveType.NoFingerprint] = (item, isApproved) =>
                    _employeeNoFingerprintRepo.UpdateAsync(MapApproveBGD<EmployeeNoFingerprint>(item, isApproved)),

                [ApproveType.NightShift] = (item, isApproved) =>
                    _employeeNightShiftRepo.UpdateAsync(MapApproveBGD<EmployeeNighShift>(item, isApproved)),

                [ApproveType.VehicleBooking] = (item, isApproved) =>
                    _vehicleBookingManagementRepo.UpdateAsync(MapApproveBGD<VehicleBookingManagement>(item, isApproved))
            };

        }


        private static T MapApproveSenior<T>(ApproveItemParam item) where T : class, new()
        {
            var e = new T();
            var type = typeof(T);
            typeof(T).GetProperty("ID")?.SetValue(e, item.ID);
            SetApproveValue(e, "IsSeniorApproved", item.IsSeniorApproved);
                

            return e;
        }
        private static T MapApproveTP<T>(ApproveItemParam item, bool isApproved)where T : class, new()
        {
            var e = new T();
            var type = typeof(T);
            if (!string.IsNullOrWhiteSpace(item.ValueDecilineApprove)&& int.TryParse(item.ValueDecilineApprove, out var decilineApprove))
            {
                type.GetProperty("DecilineApprove")?.SetValue(e, decilineApprove);
            }
            if (!string.IsNullOrWhiteSpace(item.ReasonDeciline))
            {
                type.GetProperty("ReasonDeciline")?.SetValue(e, item.ReasonDeciline);
            }
            if (!string.IsNullOrWhiteSpace(item.EvaluateResults)&&item.TType==5)
            {
                type.GetProperty("EvaluateResults")?.SetValue(e, item.EvaluateResults);
            }
            type.GetProperty("ID")?.SetValue(e, item.ID);
            SetApproveEither(e, isApproved);
            return e;
        }
        private static T MapApproveBGD<T>(ApproveItemParam item, bool isApproved)
      where T : class, new()
        {
            var e = new T();
            var type = typeof(T);

            type.GetProperty("ID")?.SetValue(e, item.ID);
            SetApproveValue(e, "IsApprovedBGD", isApproved);

            return e;
        }
        private static void SetApproveValue<T>(T entity, string propName, bool? value)
        {
            if (value == null) return;

            var prop = typeof(T).GetProperty(propName);
            if (prop == null) return;

            if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
                prop.SetValue(entity, value);
            else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                prop.SetValue(entity, value.Value ? 1 : 0);
        }
        private static void SetApproveEither<T>(T entity, bool value)
        {
            SetApproveValue(entity, "IsApproved", value);
            SetApproveValue(entity, "IsApprovedTP", value);
        }

        private string? ValidateBgd(ApproveItemParam item, bool isApproved)
        {
            if ((item.ID ?? 0) <= 0)
                return "ID không hợp lệ.";

            if (item.IsApprovedHR != true)
                return $"Nhân viên [{item.FullName}] chưa được HR duyệt.";




            return null;
        }

        private string? ValidateSenior(ApproveItemParam item, bool isApproved)
        {
            if ((item.ID ?? 0) <= 0)
                return "ID không hợp lệ.";

            //// TableName có thể null nhưng string.Equals static handle được null, nên giữ nguyên cũng ok
            //if (!string.Equals(item.TableName, "EmployeeOvertime", StringComparison.OrdinalIgnoreCase))
            //    return "Senior chỉ được duyệt cho đăng ký làm thêm (EmployeeOvertime).";

            if (item.IsApprovedBGD == true)
                return $"Nhân viên [{item.FullName}] đã được BGĐ duyệt.";



            //// Hủy duyệt mà chưa từng Senior duyệt
            //if (isApproved==false && (item.IsSeniorApproved != true))
            //    return $"Nhân viên [{item.FullName}] chưa được Senior duyệt, không thể hủy duyệt.";

            return null;
        }
        private string? ValidateTBP(ApproveItemParam item, bool isApproved)
        {
            if ((item.ID ?? 0) <= 0)
                return "ID không hợp lệ.";

            if (item.DeleteFlag ?? false)
                return $"Nhân viên [{item.FullName}] đã tự xoá khai báo, không thể duyệt / hủy duyệt.";



            if ((item.IsCancelRegister ?? 0) > 0)
                return $"Nhân viên [{item.FullName}] đã đăng ký hủy, không thể duyệt / hủy duyệt.";

            if (!isApproved && item.IsApprovedHR == true)
                return $"Nhân viên [{item.FullName}] đã được HR duyệt.";

            if (!isApproved && item.IsApprovedBGD == true)
                return $"Nhân viên [{item.FullName}] đã được BGĐ duyệt.";

            return null;
        }

        public enum ApproveType
        {
            Onleave = 1,
            EarlyLate = 2,
            OT = 3,
            Bussiness = 4,
            WFH = 5,
            NoFingerprint = 6,
            SalaryAdvance = 7,
            NightShift = 8,
            VehicleBooking = 9
        }

        [HttpPost("approve-tbp-new")]
        public async Task<IActionResult> ApproveTBPNew([FromBody] ApproveRequestParam request)
        {
            if (request?.Items == null || request.Items.Count == 0)
                return BadRequest(ApiResponseFactory.Fail(null, "Danh sách phê duyệt không được để trống"));
            string approved = request.IsApproved == true ? "Duyệt" : "Hủy duyệt";
            var notProcessed = new List<NotProcessedApprovalItem>();
            var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
            var currentUser = ObjectMapper.GetCurrentUser(claims);

            foreach (var item in request.Items)
            {
                try
                {

                    var error = ValidateTBP(item, request.IsApproved ?? false);
                    if (error != null)
                    {
                        notProcessed.Add(new() { Item = item, Reason = error });
                        continue;
                    }
                    var type = (ApproveType)(item.TType ?? 0);

                    if (!_approveMapTP.TryGetValue(type, out var approveAction))
                    {
                        notProcessed.Add(new()
                        {
                            Item = item,
                            Reason = "Loại phê duyệt không hợp lệ."
                        });
                        continue;
                    }

                    await approveAction(item, request.IsApproved ?? false);
                }
                catch (Exception ex)
                {
                    notProcessed.Add(new()
                    {
                        Item = item,
                        Reason = ex.Message
                    });
                }
            }

            return Ok(ApiResponseFactory.Success(
                notProcessed,
                notProcessed.Count == 0
                    ? $"{approved} thành công."
                    : $"{approved} thành công, bỏ qua {notProcessed.Count} bản ghi."
            ));
        }
        [HttpPost("approve-bgd-new")]
        public async Task<IActionResult> ApproveBGDNew([FromBody] ApproveRequestParam request)
        {
            if (request?.Items == null || request.Items.Count == 0)
                return BadRequest(ApiResponseFactory.Fail(null, "Danh sách phê duyệt không được để trống"));
            string approved = request.IsApproved == true ? "Duyệt" : "Hủy duyệt";
            var notProcessed = new List<NotProcessedApprovalItem>();
            var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
            var currentUser = ObjectMapper.GetCurrentUser(claims);

            foreach (var item in request.Items)
            {
                try
                {
                    var error = ValidateBgd(item, request.IsApproved ?? false);
                    if (error != null)
                    {
                        notProcessed.Add(new() { Item = item, Reason = error });
                        continue;
                    }
                    var type = (ApproveType)(item.TType ?? 0);

                    if (!_approveMapBGD.TryGetValue(type, out var approveAction))
                    {
                        notProcessed.Add(new()
                        {
                            Item = item,
                            Reason = "Loại phê duyệt không hợp lệ."
                        });
                        continue;
                    }

                    await approveAction(item, request.IsApproved ?? false);

                }
                catch (Exception ex)
                {
                    notProcessed.Add(new()
                    {
                        Item = item,
                        Reason = ex.Message
                    });
                }
            }

            return Ok(ApiResponseFactory.Success(
                notProcessed,
                notProcessed.Count == 0
                       ? $"{approved} thành công."
                    : $"{approved} thành công, bỏ qua {notProcessed.Count} bản ghi."
            ));
        }

        [HttpPost("approve-senior-new")]
        public async Task<IActionResult> ApproveSenior([FromBody] ApproveRequestParam request)
        {
            if (request?.Items == null || request.Items.Count == 0)
                return BadRequest(ApiResponseFactory.Fail(null, "Danh sách phê duyệt không được để trống"));
            string approved = request.IsApproved == true ? "Duyệt" : "Hủy duyệt";
            var notProcessed = new List<NotProcessedApprovalItem>();
            var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
            var currentUser = ObjectMapper.GetCurrentUser(claims);

            foreach (var item in request.Items)
            {
                try
                {
                    var error = ValidateSenior(item, request.IsApproved ?? false);
                    if (error != null)
                    {
                        notProcessed.Add(new() { Item = item, Reason = error });
                        continue;
                    }
                    var type = (ApproveType)(item.TType ?? 0);

                    if (!_approveMapSenior.TryGetValue(type, out var approveAction))
                    {
                        notProcessed.Add(new()
                        {
                            Item = item,
                            Reason = "Loại phê duyệt không hợp lệ."
                        });
                        continue;
                    }

                    await approveAction(item);
                }
                catch (Exception ex)
                {
                    notProcessed.Add(new()
                    {
                        Item = item,
                        Reason = ex.Message
                    });
                }
            }

            return Ok(ApiResponseFactory.Success(
                notProcessed,
                notProcessed.Count == 0
                ? $"{approved} thành công."
        : $"{approved} thành công, bỏ qua {notProcessed.Count} bản ghi."
            ));
        }
    }
}

