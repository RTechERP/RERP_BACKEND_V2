using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class PhasedAllocationPersonDetailDTO : PhasedAllocationPersonDetail
    {
        public string EmployeeCode { get; set; } = string.Empty;
    }
}