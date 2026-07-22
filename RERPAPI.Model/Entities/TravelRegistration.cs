using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Entities;

public partial class TravelRegistration
{
    public int ID { get; set; }

    public int EmployeeID { get; set; }

    public string EmployeeCode { get; set; } = null!;

    public string EmployeeName { get; set; } = null!;

    public string? Department { get; set; }

    public string? PositionName { get; set; }

    public DateOnly? BirthDay { get; set; }

    public int? Age { get; set; }

    public decimal? Height { get; set; }

    public string? Gender { get; set; }

        /// <summary>
        /// CBNV, Vợ, Chồng, Con...
        /// </summary>
    public string? Relationship { get; set; }

    public string? Address { get; set; }

    public string? CCCD { get; set; }

    public DateTime? CCCDIssueDate { get; set; }

    public string? CCCDIssuePlace { get; set; }

    public string? PhoneNumber { get; set; }

    public string? DepartureLocation { get; set; }

        /// <summary>
        /// 0: Chưa xác nhận
        /// 1: Đã xác nhận
        /// 2: Không tham gia
        /// </summary>
        public int ConfirmStatus { get; set; }

    public DateTime? ConfirmDate { get; set; }

    public string? ConfirmBy { get; set; }

    public bool IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int OwnerEmployeeID { get; set; }

    public int? ConfirmStatus { get; set; }
}
