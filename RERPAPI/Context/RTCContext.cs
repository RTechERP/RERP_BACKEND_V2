using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RERPAPI.Entities;

namespace RERPAPI.Context;

public partial class RTCContext : DbContext
{
    public RTCContext(DbContextOptions<RTCContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EmployeePayrollDetail> EmployeePayrollDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmployeePayrollDetail>(entity =>
        {
            entity.ToTable("EmployeePayrollDetail");

            entity.Property(e => e.ActualAmountReceived)
                .HasComment("Thực lĩnh")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.AdvancePayment)
                .HasComment("Ứng trước lương")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.AllowanceMeal)
                .HasComment("Phụ cấp cơm ca")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Allowance_OT_Early)
                .HasComment("Phụ cấp làm thêm trước 7H15 ")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BasicSalary).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Bonus)
                .HasComment("\"Thưởng \r\nKPIs/doanh số\"")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BussinessMoney)
                .HasComment("Tiền công tác phí")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CostVehicleBussiness)
                .HasComment("Chi phí phương tiện công tác")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DepartmentalFees)
                .HasComment("Thu hộ phòng ban")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GetCash).HasComment("Nhận tiền mặt(true là có, false là không)");
            entity.Property(e => e.Insurances)
                .HasComment("BHXH, BHYT, BHTN (10,5%)")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsPublish).HasComment("1: hiển thị trên web cho nhân viên xem; 0: Không show trên web");
            entity.Property(e => e.NightShiftMoney)
                .HasComment("Tiền công làm đêm")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Note)
                .HasMaxLength(1500)
                .HasComment("Ghi chú");
            entity.Property(e => e.OT_Hour_HD)
                .HasComment("Số giờ làm thêm ngày Lễ Tết")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OT_Hour_WD)
                .HasComment(" Số giờ làm thêm ngày thường  ")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OT_Hour_WK)
                .HasComment("Số giờ làm thêm ngày thứ 7, CN")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OT_Money_HD)
                .HasComment("Số tiền làm thêm giờ ngày Lễ Tết (Số giờ * 3 * Công 1h)")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OT_Money_WD)
                .HasComment("Số tiền làm thêm giờ (Số giờ * 1,5 * Tiền công 1h)")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OT_Money_WK)
                .HasComment("Số tiền làm thêm giờ chiều T7, ngày CN (Số giờ * 2 * Công 1h)")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Other)
                .HasComment("khoản công khác")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OtherDeduction)
                .HasComment("Các khoản phải trừ khác")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ParkingMoney)
                .HasComment("Tiền gửi xe ô tô")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Punish5S)
                .HasComment("Phạt 5S")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RealIndustry)
                .HasComment("Phụ cấp chuyên cần \r\nthực nhận")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RealSalary)
                .HasComment("\"Tổng thu nhập \r\nthực tế\"")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SalaryOneHour)
                .HasComment("Tính tiền công 1h")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Sign).HasComment("Ký");
            entity.Property(e => e.TotalMerit)
                .HasComment("Tổng công được tính (Bao gồm công thực tế và phép)")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalSalaryByDay)
                .HasComment("Tổng lương theo ngày công ")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalWorkday)
                .HasComment("Tổng công tiêu chuẩn")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnionFees)
                .HasComment("Công đoàn (1% * lương đóng bảo hiểm)")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
