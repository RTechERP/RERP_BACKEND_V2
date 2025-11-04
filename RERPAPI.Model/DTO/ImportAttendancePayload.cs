namespace RERPAPI.Model.DTO
{
    public class ImportAttendancePayload
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int DepartmentId { get; set; }      // 0 = tất cả
        public bool Overwrite { get; set; } = true;
        public List<Dictionary<string, object>> Rows { get; set; } = new();
    }


}
