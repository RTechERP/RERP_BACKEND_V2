using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeePayrollDetail
{
    public int ID { get; set; }

    public int? PayrollID { get; set; }

    public int? STT { get; set; }

    public int? EmployeeID { get; set; }

    public decimal? BasicSalary { get; set; }

    /// <summary>
    /// Tổng công tiêu chuẩn
    /// </summary>
    public decimal? TotalWorkday { get; set; }

    /// <summary>
    /// Tổng công được tính (Bao gồm công thực tế và phép)
    /// </summary>
    public decimal? TotalMerit { get; set; }

    /// <summary>
    /// Tổng lương theo ngày công 
    /// </summary>
    public decimal? TotalSalaryByDay { get; set; }

    /// <summary>
    /// Tính tiền công 1h
    /// </summary>
    public decimal? SalaryOneHour { get; set; }

    /// <summary>
    ///  Số giờ làm thêm ngày thường  
    /// </summary>
    public decimal? OT_Hour_WD { get; set; }

    /// <summary>
    /// Số tiền làm thêm giờ (Số giờ * 1,5 * Tiền công 1h)
    /// </summary>
    public decimal? OT_Money_WD { get; set; }

    /// <summary>
    /// Số giờ làm thêm ngày thứ 7, CN
    /// </summary>
    public decimal? OT_Hour_WK { get; set; }

    /// <summary>
    /// Số tiền làm thêm giờ chiều T7, ngày CN (Số giờ * 2 * Công 1h)
    /// </summary>
    public decimal? OT_Money_WK { get; set; }

    /// <summary>
    /// Số giờ làm thêm ngày Lễ Tết
    /// </summary>
    public decimal? OT_Hour_HD { get; set; }

    /// <summary>
    /// Số tiền làm thêm giờ ngày Lễ Tết (Số giờ * 3 * Công 1h)
    /// </summary>
    public decimal? OT_Money_HD { get; set; }

    /// <summary>
    /// Phụ cấp chuyên cần 
    /// thực nhận
    /// </summary>
    public decimal? RealIndustry { get; set; }

    /// <summary>
    /// Phụ cấp cơm ca
    /// </summary>
    public decimal? AllowanceMeal { get; set; }

    /// <summary>
    /// Phụ cấp làm thêm trước 7H15 
    /// </summary>
    public decimal? Allowance_OT_Early { get; set; }

    /// <summary>
    /// Tiền công tác phí
    /// </summary>
    public decimal? BussinessMoney { get; set; }

    /// <summary>
    /// Tiền công làm đêm
    /// </summary>
    public decimal? NightShiftMoney { get; set; }

    /// <summary>
    /// Chi phí phương tiện công tác
    /// </summary>
    public decimal? CostVehicleBussiness { get; set; }

    /// <summary>
    /// &quot;Thưởng 
    /// KPIs/doanh số&quot;
    /// </summary>
    public decimal? Bonus { get; set; }

    /// <summary>
    /// khoản công khác
    /// </summary>
    public decimal? Other { get; set; }

    /// <summary>
    /// &quot;Tổng thu nhập 
    /// thực tế&quot;
    /// </summary>
    public decimal? RealSalary { get; set; }

    /// <summary>
    /// BHXH, BHYT, BHTN (10,5%)
    /// </summary>
    public decimal? Insurances { get; set; }

    /// <summary>
    /// Công đoàn (1% * lương đóng bảo hiểm)
    /// </summary>
    public decimal? UnionFees { get; set; }

    /// <summary>
    /// Ứng trước lương
    /// </summary>
    public decimal? AdvancePayment { get; set; }

    /// <summary>
    /// Thu hộ phòng ban
    /// </summary>
    public decimal? DepartmentalFees { get; set; }

    /// <summary>
    /// Tiền gửi xe ô tô
    /// </summary>
    public decimal? ParkingMoney { get; set; }

    /// <summary>
    /// Phạt 5S
    /// </summary>
    public decimal? Punish5S { get; set; }

    /// <summary>
    /// Các khoản phải trừ khác
    /// </summary>
    public decimal? OtherDeduction { get; set; }

    /// <summary>
    /// Thực lĩnh
    /// </summary>
    public decimal? ActualAmountReceived { get; set; }

    /// <summary>
    /// Ký
    /// </summary>
    public bool? Sign { get; set; }

    /// <summary>
    /// Nhận tiền mặt(true là có, false là không)
    /// </summary>
    public bool? GetCash { get; set; }

    /// <summary>
    /// 1: hiển thị trên web cho nhân viên xem; 0: Không show trên web
    /// </summary>
    public bool? IsPublish { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public int? MealUse { get; set; }

    public string? TaxCompanyName { get; set; }

    public string? StatusContract { get; set; }

    /// <summary>
    /// Lương làm thêm (- thuế)
    /// </summary>
    public decimal? TaxSalaryOT { get; set; }

    /// <summary>
    /// phụ cấp cơm ca (- thuế)
    /// </summary>
    public decimal? TaxSalaryMeal { get; set; }

    /// <summary>
    /// phụ cấp điện thoại (- thuế)
    /// </summary>
    public decimal? TaxSalaryPhone { get; set; }

    /// <summary>
    /// Giảm trừ bản thân (- thuế)
    /// </summary>
    public decimal? TaxPersonalDeduction { get; set; }

    /// <summary>
    /// Giảm trừ người phụ thuộc (- thuế)
    /// </summary>
    public decimal? TaxDependentsDeduction { get; set; }

    /// <summary>
    /// Thu thập tính thuế
    /// </summary>
    public decimal? TaxAbleIncome { get; set; }

    /// <summary>
    /// Khấu trừ thuế TNCN
    /// </summary>
    public decimal? TaxDeduction { get; set; }
}
