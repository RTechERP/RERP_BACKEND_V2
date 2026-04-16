using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class EmployeeOnLeavePhaseRepo : GenericRepo<EmployeeOnLeavePhase>
    {
        public EmployeeOnLeaveRepo _employeeOnLeaveRepo;
        private CurrentUser _currentUser;
        private readonly vUserGroupLinksRepo _vUserGroupLinksRepo;
        public EmployeeOnLeavePhaseRepo(CurrentUser currentUser, EmployeeOnLeaveRepo employeeOnLeaveRepo, vUserGroupLinksRepo vUserGroupLinksRepo) : base(currentUser)
        {
            _employeeOnLeaveRepo = employeeOnLeaveRepo;
            _currentUser = currentUser;
            _vUserGroupLinksRepo = vUserGroupLinksRepo;
        }

        public string GeneratePhaseCode(int employeeID, int year, int month)
        {
            var employeeCode = db.Employees.Where(x => x.ID == employeeID).Select(x => x.Code).FirstOrDefault() ?? "EMP";
            var prefix = $"{year}_{month}_{employeeCode}_";

            // Tìm mã code đợt trước đó
            var count = db.EmployeeOnLeavePhases
                .Where(x => x.EmployeeID == employeeID && x.DateRegister.HasValue && x.DateRegister.Value.Year == year && x.DateRegister.Value.Month == month && (x.IsDeleted == false || x.IsDeleted == null))
                .Count();

            return prefix + (count + 1);
        }

        public async Task<EmployeeOnLeaveMultiDTO> GetMultiByID(int id)
        {
            var phase = await GetByIDAsync(id);
            if (phase == null) return null;

            var details = db.EmployeeOnLeaves
                .Where(x => x.EmployeeOnLeavePhaseID == id && (x.DeleteFlag == false || x.DeleteFlag == null))
                .ToList();

            return new EmployeeOnLeaveMultiDTO
            {
                Phase = phase,
                Details = details
            };
        }

        public async Task<int> SaveMultiPhase(EmployeeOnLeaveMultiDTO dto)
        {
            try
            {
                // Generate code or Update Phase
                if (dto.Phase.ID <= 0)
                {
                    var year = dto.Phase.DateRegister?.Year ?? DateTime.Now.Year;
                    var month = dto.Phase.DateRegister?.Month ?? DateTime.Now.Month;
                    dto.Phase.Code = GeneratePhaseCode(dto.Phase.EmployeeID ?? 0, year, month);
                    await CreateAsync(dto.Phase);
                }
                else
                {
                    await UpdateAsync(dto.Phase);
                }

                if (dto.IsPartialUpdate != true)
                {
                    var existingDetailIDs = db.EmployeeOnLeaves
                        .Where(x => x.EmployeeOnLeavePhaseID == dto.Phase.ID && (x.DeleteFlag == false || x.DeleteFlag == null))
                        .Select(x => x.ID)
                        .ToList();

                    var incomingDetailIDs = dto.Details.Where(x => x.ID > 0).Select(x => x.ID).ToList();

                    // Soft-delete records that are no longer in the incoming payload
                    var toDelete = existingDetailIDs.Except(incomingDetailIDs).ToList();
                    foreach (var id in toDelete)
                    {
                        var record = await _employeeOnLeaveRepo.GetByIDAsync(id);
                        if (record != null)
                        {
                            record.DeleteFlag = true;
                            await _employeeOnLeaveRepo.UpdateAsync(record);
                        }
                    }
                }

                foreach (var detail in dto.Details)
                {
                    // Always reset approval status when saving/updating from this method
                    detail.IsApprovedTP = false;
                    detail.IsApprovedHR = false;
                    detail.ApprovedHR = 0;

                    if (detail.ID <= 0)
                    {
                        detail.EmployeeID = dto.Phase.EmployeeID;
                        detail.EmployeeOnLeavePhaseID = dto.Phase.ID;
                        await _employeeOnLeaveRepo.CreateAsync(detail);
                    }
                    else
                    {
                        detail.EmployeeID = dto.Phase.EmployeeID;
                        detail.EmployeeOnLeavePhaseID = dto.Phase.ID;
                        await _employeeOnLeaveRepo.UpdateAsync(detail);
                    }
                }

                return dto.Phase.ID;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public APIResponse Validate(EmployeeOnLeaveMultiDTO dto)
        {
            try
            {
                var response = ApiResponseFactory.Success(null, "");
                if (dto.Details == null || dto.Details.Count == 0)
                {
                    return ApiResponseFactory.Fail(null, "Vui lòng thêm chi tiết ngày nghỉ.");
                }
                var vUserHR = _vUserGroupLinksRepo
                          .GetAll()
                          .FirstOrDefault(x =>
                           (x.Code == "N1" || x.Code == "N2") &&
                           x.UserID == _currentUser.ID);
                bool isSpecialPermission = _currentUser.IsAdmin==true||vUserHR!=null;
                // 1. Validate Overlaps against Database
                foreach (var detail in dto.Details)
                {
                    if (!detail.StartDate.HasValue) continue;

                    var checkDate = detail.StartDate.Value.Date;
                    var query = db.EmployeeOnLeaves
                        .Where(x => x.EmployeeID == dto.Phase.EmployeeID
                            && x.StartDate.HasValue
                            && x.StartDate.Value.Date == checkDate
                            && x.DeleteFlag != true);
                    // If editing, exclude current phase details
                    if (dto.Phase.ID > 0)
                    {
                        query = query.Where(x => x.EmployeeOnLeavePhaseID != dto.Phase.ID);
                    }

                    var existing = query.ToList();

                    foreach (var ex in existing)
                    {
                        bool isOverlap = false;
                        if (detail.TimeOnLeave == 3) isOverlap = true;
                        else if (ex.TimeOnLeave == 3) isOverlap = true;
                        else if (detail.TimeOnLeave == ex.TimeOnLeave) isOverlap = true;

                        if (isOverlap)
                        {
                            response = ApiResponseFactory.Fail(null, $"Nhân viên đã có đăng ký nghỉ vào {(ex.TimeOnLeave == 1 ? "buổi sáng" : ex.TimeOnLeave == 2 ? "buổi chiều" : "cả ngày")} ngày {checkDate.ToString("dd/MM/yyyy")}. Vui lòng kiểm tra lại.");
                            break;
                        }
                    }

                    if (response.status == 0) break;
                }

                // 2. Validate số ngày phép còn lại
                if (response.status == 1)
                {
                    var startOfYear = new DateTime(DateTime.Now.Year, 1, 1);
                    var totalRequested = dto.Details.Where(x => x.TypeIsReal == 2).Sum(x => x.TotalDay ?? 0);

                    if (totalRequested > 0)
                    {
                        var summary = SQLHelper<dynamic>.ProcedureToList("spGetEmployeeOnLeaveInWeb",
                            new string[] { "@DateStart", "@EmployeeID" },
                            new object[] { startOfYear, dto.Phase.EmployeeID ?? 0 });

                        if (summary.Count > 0 && !isSpecialPermission)
                        {
                            var data = SQLHelper<dynamic>.GetListData(summary, 0);
                            var firstRecord = data.FirstOrDefault();
                            if (firstRecord != null)
                            {
                                decimal remain = (decimal)(firstRecord.TotalDayRemain ?? 0);
                                if (totalRequested > remain)
                                {
                                    response = ApiResponseFactory.Fail(null,
                                        $"Số ngày nghỉ phép đăng ký ({totalRequested}) vượt quá số ngày phép còn lại ({remain}).");
                                }
                            }
                        }
                    }
                }
                // 3. Validate Registration Deadline (After 19:00 for tomorrow)
                if (response.status == 1 && !isSpecialPermission)
                {
                    var now = DateTime.Now;
                    var today = DateTime.Today;
                    var tomorrow = today.AddDays(1);

                    // Check đăng ký ngày hiện tại
                    bool hasTodayRegistration = dto.Details.Any(d =>
                        d.StartDate.HasValue && d.StartDate.Value.Date == today);

                    if (hasTodayRegistration)
                    {
                        response = ApiResponseFactory.Fail(null,
                            "Không thể đăng ký nghỉ phép cho ngày hiện tại. Vui lòng kiểm tra lại.");
                        return response;
                    }

                    // Check sau 19h không cho đăng ký ngày mai
                    if (now.Hour >= 19)
                    {
                        bool hasTomorrowRegistration = dto.Details.Any(d =>
                            d.StartDate.HasValue && d.StartDate.Value.Date == tomorrow);

                        if (hasTomorrowRegistration)
                        {
                            response = ApiResponseFactory.Fail(null,
                                "Sau 19:00 không thể đăng ký nghỉ phép cho ngày hôm sau. Vui lòng kiếm tra lại.");
                        }
                    }
                }

                // 4. Validate Edit Permissions (Approved or Past Time)
                if (response.status == 1 && dto.Phase.ID > 0 && !isSpecialPermission)
                {
                    var existingDetails = db.EmployeeOnLeaves
                        .Where(x => x.EmployeeOnLeavePhaseID == dto.Phase.ID && (x.DeleteFlag != true))
                        .ToList();

                    if (existingDetails.Any(x => x.IsApprovedTP == true))
                    {
                        return ApiResponseFactory.Fail(null, "Đăng ký nghỉ đã được TBP duyệt. Không thể sửa!");
                    }

                    if (existingDetails.Any(x => x.StartDate.HasValue && x.StartDate.Value < DateTime.Now))
                    {
                        return ApiResponseFactory.Fail(null, "Đã quá thời gian xin nghỉ. Không thể sửa!");
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Fail(ex, ex.Message);
            }
        }


    

    }
}
