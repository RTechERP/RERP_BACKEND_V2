namespace RERPAPI.Model.DTO
{
    /// <summary>
    /// DTO for manual single employee attendance upsert (add/edit).
    /// </summary>
    public class UpsertSingleAttendanceRequest
    {
        /// <summary>
        /// Record ID (0 = Create new, > 0 = Update existing)
        /// </summary>
        public int ID { get; set; } = 0;

        /// <summary>
        /// Employee Code (Employee.Code)
        /// </summary>
        public string Code { get; set; } = "";

        /// <summary>
        /// Attendance date
        /// </summary>
        public DateTime AttendanceDate { get; set; }

        /// <summary>
        /// Day of week (e.g. "Thứ Hai", "Monday")
        /// </summary>
        public string DayWeek { get; set; } = "";

        /// <summary>
        /// Check-In time (HH:mm format)
        /// </summary>
        public string CheckIn { get; set; } = "";

        /// <summary>
        /// Check-Out time (HH:mm format)
        /// </summary>
        public string CheckOut { get; set; } = "";
    }
}
