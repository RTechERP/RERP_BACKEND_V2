using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class VehicleBookingManagement
{
    public int ID { get; set; }

    public int? VehicleManagementID { get; set; }

    public int? STT { get; set; }

    public int? EmployeeID { get; set; }

    public string? BookerVehicles { get; set; }

    public string? DepartureAddress { get; set; }

    public DateTime? DepartureDate { get; set; }

    /// <summary>
    /// 1:&apos;Đăng ký đi&apos;
    /// 2:&apos;Đăng ký giao hàng&apos;
    /// 3:&apos;Xếp xe về&apos;
    /// 4:&apos;Chủ động phương tiện&apos;
    /// 5:&apos;Đăng ký về&apos;
    /// 6:&apos;Đăng ký lấy hàng&apos;
    /// 
    /// </summary>
    public int? Category { get; set; }

    public int? Status { get; set; }

    public string? CompanyNameArrives { get; set; }

    public string? Province { get; set; }

    public string? SpecificDestinationAddress { get; set; }

    public DateTime? TimeNeedPresent { get; set; }

    public DateTime? TimeReturn { get; set; }

    public string? NameVehicleCharge { get; set; }

    public string? LicensePlate { get; set; }

    public string? DriverName { get; set; }

    public string? DriverPhoneNumber { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? PhoneNumber { get; set; }

    public int? PassengerEmployeeID { get; set; }

    public string? PassengerCode { get; set; }

    public string? PassengerName { get; set; }

    public string? PassengerDepartment { get; set; }

    public string? PassengerPhoneNumber { get; set; }

    public string? Note { get; set; }

    public int? ReceiverEmployeeID { get; set; }

    public string? ReceiverCode { get; set; }

    public string? ReceiverName { get; set; }

    public string? ReceiverPhoneNumber { get; set; }

    public string? PackageName { get; set; }

    public string? DeliverName { get; set; }

    public string? DeliverPhoneNumber { get; set; }

    public bool? IsApprovedTBP { get; set; }

    public string? DecilineApprove { get; set; }

    public string? ReasonDeciline { get; set; }

    public string? ProblemArises { get; set; }

    public bool? IsProblemArises { get; set; }

    public bool? IsCancel { get; set; }

    public bool? IsSend { get; set; }

    public int? ApprovedTBP { get; set; }

    public bool? IsNotifiled { get; set; }

    public int? ParentID { get; set; }

    /// <summary>
    /// 1:VP HN;2:VP BN;3:VP HP;4:VP HCM;5: Khác
    /// </summary>
    public int? DepartureAddressStatus { get; set; }

    public int? DepartureAddressStatusActual { get; set; }

    public string? DepartureAddressActual { get; set; }

    public DateTime? DepartureDateActual { get; set; }

    public int? ProjectID { get; set; }

    public decimal? VehicleMoney { get; set; }

    public string? PackageSize { get; set; }

    public string? PackageWeight { get; set; }

    public int? PackageQuantity { get; set; }

    /// <summary>
    /// Loại phương tiện (1: Oto, xe máy....; 2: Máy bay)
    /// </summary>
    public int VehicleType { get; set; }
}
