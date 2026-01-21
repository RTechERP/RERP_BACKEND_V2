using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeAttendanceRepo : GenericRepo<EmployeeAttendance>
    {
        private readonly EmployeeRepo _employeeRepo;

        public EmployeeAttendanceRepo(CurrentUser currentUser,EmployeeRepo employeeRepo) : base(currentUser)
        {
            _employeeRepo = employeeRepo;   
        }

        /// <summary>
        /// Kiểm tra số lượng bản ghi Attendance tồn tại trong khoảng ngày & phòng ban.
        /// </summary>
        public int CheckExisting(DateTime dateStart, DateTime dateEnd, int departmentId)
        {
            var ds = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 0);
            var de = new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 23, 59, 59);

            // Lấy danh sách IDChamCongMoi của nhân viên thuộc phòng ban (hoặc tất cả nếu Dept=0)
            var employeeIds = _employeeRepo
                .GetAll(x => departmentId == 0 || x.DepartmentID == departmentId)
                .Select(x => x.IDChamCongMoi)
                .ToList();

            if (employeeIds == null || employeeIds.Count == 0)
                return 0;

            // Đếm attendance trong khoảng ngày của các ID này
            int count = GetAll(ea =>
                ea.AttendanceDate >= ds &&
                ea.AttendanceDate <= de &&
                employeeIds.Contains(ea.IDChamCongMoi)
            ).Count();

            return count;
        }
    }
}
