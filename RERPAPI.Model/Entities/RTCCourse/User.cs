namespace RERPAPI.Model.Entities.RTCCourse;

public partial class User
{
    public int ID { get; set; }

    public string? Code { get; set; }

    public string? LoginName { get; set; }

    public string? PasswordHash { get; set; }

    public string? FullName { get; set; }

    public DateTime? BirthOfDate { get; set; }

    public int? Sex { get; set; }

    public string? Qualifications { get; set; }

    public string? BankAccount { get; set; }

    public string? BHYT { get; set; }

    public string? MST { get; set; }

    public string? BHXH { get; set; }

    public string? CMTND { get; set; }

    public string? JobDescription { get; set; }

    public string? Telephone { get; set; }

    public string? HandPhone { get; set; }

    public string? HomeAddress { get; set; }

    public string? Resident { get; set; }

    public string? PostalCode { get; set; }

    public int? DepartmentID { get; set; }

    public int? Status { get; set; }

    public string? Communication { get; set; }

    public DateTime? PassExpireDate { get; set; }

    public bool? IsCashier { get; set; }

    public int? CashierNo { get; set; }

    public string? EmailCom { get; set; }

    public string? Email { get; set; }

    public DateTime? StartWorking { get; set; }

    public int? UserGroupID { get; set; }

    public int? UserGroupSXID { get; set; }

    public int? MainViewID { get; set; }

    public bool? IsSetupFunction { get; set; }

    public string? ImagePath { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsAdmin { get; set; }

    public int? isAdminSale { get; set; }

    public int? RoleID { get; set; }

    public int? TeamID { get; set; }

    public int? Leader { get; set; }

    public string? PinPassword { get; set; }

    /// <summary>
    /// Đơn vị công tác
    /// </summary>
    public string? Organization { get; set; }

    /// <summary>
    /// Vị trí
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// Nguồn giới thiệu
    /// </summary>
    public string? ReferralSource { get; set; }
}