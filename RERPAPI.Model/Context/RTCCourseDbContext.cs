using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Entities.RTCCourse;

namespace RERPAPI.Model.Context;

public partial class RTCCourseDbContext : DbContext
{
    public RTCCourseDbContext(DbContextOptions<RTCCourseDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ConfigSystem> ConfigSystems { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseAnswer> CourseAnswers { get; set; }

    public virtual DbSet<CourseCatalog> CourseCatalogs { get; set; }

    public virtual DbSet<CourseCatalogType> CourseCatalogTypes { get; set; }

    public virtual DbSet<CourseExam> CourseExams { get; set; }

    public virtual DbSet<CourseExamEvaluate> CourseExamEvaluates { get; set; }

    public virtual DbSet<CourseExamPractice> CourseExamPractices { get; set; }

    public virtual DbSet<CourseExamResult> CourseExamResults { get; set; }

    public virtual DbSet<CourseExamResultDetail> CourseExamResultDetails { get; set; }

    public virtual DbSet<CourseFile> CourseFiles { get; set; }

    public virtual DbSet<CourseLesson> CourseLessons { get; set; }

    public virtual DbSet<CourseLessonHistory> CourseLessonHistories { get; set; }

    public virtual DbSet<CourseQuestion> CourseQuestions { get; set; }

    public virtual DbSet<CourseRightAnswer> CourseRightAnswers { get; set; }

    public virtual DbSet<CourseType> CourseTypes { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConfigSystem>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__ConfigSy__3214EC2786C53E80");

            entity.ToTable("ConfigSystem");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.KeyName).HasMaxLength(100);
            entity.Property(e => e.KeyValue1).HasMaxLength(200);
            entity.Property(e => e.KeyValue10).HasMaxLength(200);
            entity.Property(e => e.KeyValue2).HasMaxLength(200);
            entity.Property(e => e.KeyValue3).HasMaxLength(200);
            entity.Property(e => e.KeyValue4).HasMaxLength(200);
            entity.Property(e => e.KeyValue5).HasMaxLength(200);
            entity.Property(e => e.KeyValue6).HasMaxLength(200);
            entity.Property(e => e.KeyValue7).HasMaxLength(200);
            entity.Property(e => e.KeyValue8).HasMaxLength(200);
            entity.Property(e => e.KeyValue9).HasMaxLength(200);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Course__3214EC27D0F3F0C7");

            entity.ToTable("Course");

            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Instructor).HasMaxLength(200);
            entity.Property(e => e.LeadTime).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NameCourse).HasMaxLength(300);
            entity.Property(e => e.QuestionCount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QuestionDuration).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseAnswer>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__CourseAn__3214EC27AA15EF34");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseCatalog>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__CourseCa__3214EC27792EF0E6");

            entity.ToTable("CourseCatalog");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseCatalogType>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__CourseCa__3214EC27C707AA9E");

            entity.ToTable("CourseCatalogType");

            entity.Property(e => e.CourseCatalogTypeCode)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CourseCatalogTypeName).HasMaxLength(550);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseExam>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__CourseEx__3214EC2785017680");

            entity.ToTable("CourseExam");

            entity.Property(e => e.CodeExam)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Goal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseExamEvaluate>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__CourseEx__3214EC27B68FCEFF");

            entity.ToTable("CourseExamEvaluate");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateCompleted).HasColumnType("datetime");
            entity.Property(e => e.DateEvaluate).HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.Point).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseExamPractice>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__CourseEx__3214EC277610CE72");

            entity.ToTable("CourseExamPractice");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateEnd).HasColumnType("datetime");
            entity.Property(e => e.DateStart).HasColumnType("datetime");
            entity.Property(e => e.PracticePoints).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseExamResult>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__CourseEx__3214EC278C4A1CF6");

            entity.ToTable("CourseExamResult");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.PercentageCorrect).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PracticePoints).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseExamResultDetail>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__CourseEx__3214EC278CB786B6");

            entity.ToTable("CourseExamResultDetail");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseFile>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__CourseFi__3214EC27ED22B6A2");

            entity.ToTable("CourseFile");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseLesson>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__CourseLe__3214EC27826C96A7");

            entity.ToTable("CourseLesson");

            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.LessonTitle).HasMaxLength(400);
            entity.Property(e => e.RequiredWatchedPercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseLessonHistory>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__CourseLe__3214EC275E202016");

            entity.ToTable("CourseLessonHistory");

            entity.Property(e => e.ViewDate).HasColumnType("datetime");
            entity.Property(e => e.WatchedPercent).HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<CourseQuestion>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__CourseQu__3214EC277C3803E0");

            entity.ToTable("CourseQuestion");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseRightAnswer>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__CourseRi__3214EC27A46ADA13");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseType>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__CourseTy__3214EC27AACDB440");

            entity.ToTable("CourseType");

            entity.Property(e => e.CourseTypeCode)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CourseTypeName).HasMaxLength(550);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Employee__3214EC27042D00D3");

            entity.ToTable("Employee");

            entity.Property(e => e.AnCa).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.AnhCBNV).HasColumnType("ntext");
            entity.Property(e => e.BHXH).HasMaxLength(550);
            entity.Property(e => e.BHYT).HasMaxLength(550);
            entity.Property(e => e.BankAccount).HasMaxLength(550);
            entity.Property(e => e.BirthOfDate).HasColumnType("datetime");
            entity.Property(e => e.CMTND).HasMaxLength(550);
            entity.Property(e => e.ChuyenCan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Code).HasMaxLength(550);
            entity.Property(e => e.CodeOld).HasMaxLength(550);
            entity.Property(e => e.Communication).HasMaxLength(550);
            entity.Property(e => e.CreatedBy).HasMaxLength(550);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DanToc).HasMaxLength(550);
            entity.Property(e => e.DcTamTru).HasMaxLength(550);
            entity.Property(e => e.DcThuongTru).HasMaxLength(550);
            entity.Property(e => e.DiaDiemLamViec).HasMaxLength(550);
            entity.Property(e => e.DienThoai).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DuongDcTamTru).HasMaxLength(150);
            entity.Property(e => e.DuongDcThuongTru).HasMaxLength(150);
            entity.Property(e => e.DvBHXH).HasMaxLength(550);
            entity.Property(e => e.Email).HasMaxLength(550);
            entity.Property(e => e.EmailCaNhan).HasMaxLength(550);
            entity.Property(e => e.EmailCom).HasMaxLength(550);
            entity.Property(e => e.EmailCongTy).HasMaxLength(550);
            entity.Property(e => e.EndWorking).HasColumnType("datetime");
            entity.Property(e => e.FullName).HasMaxLength(550);
            entity.Property(e => e.GiamTruBanThan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.HandPhone).HasMaxLength(550);
            entity.Property(e => e.HomeAddress).HasMaxLength(550);
            entity.Property(e => e.IDChamCongCu).HasMaxLength(550);
            entity.Property(e => e.IDChamCongMoi).HasMaxLength(550);
            entity.Property(e => e.ImagePath).HasMaxLength(550);
            entity.Property(e => e.JobDescription).HasMaxLength(550);
            entity.Property(e => e.Khac).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LuongCoBan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LuongThuViec).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MST).HasMaxLength(550);
            entity.Property(e => e.MoiQuanHe).HasMaxLength(150);
            entity.Property(e => e.MoiQuanHe2).HasMaxLength(250);
            entity.Property(e => e.MucDongBHXHHienTai).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NgayBatDauBHXH).HasColumnType("datetime");
            entity.Property(e => e.NgayBatDauBHXHCty).HasColumnType("datetime");
            entity.Property(e => e.NgayBatDauHD).HasColumnType("datetime");
            entity.Property(e => e.NgayBatDauHDXDTH).HasColumnType("datetime");
            entity.Property(e => e.NgayBatDauThuViec).HasColumnType("datetime");
            entity.Property(e => e.NgayCap).HasColumnType("datetime");
            entity.Property(e => e.NgayHieuLucHDKXDTH).HasColumnType("datetime");
            entity.Property(e => e.NgayKetThucBHXH).HasColumnType("datetime");
            entity.Property(e => e.NgayKetThucHD).HasColumnType("datetime");
            entity.Property(e => e.NgayKetThucHDXDTH).HasColumnType("datetime");
            entity.Property(e => e.NgayKetThucThuViec).HasColumnType("datetime");
            entity.Property(e => e.NguoiLienHeKhiCan).HasMaxLength(150);
            entity.Property(e => e.NguoiLienHeKhiCan2).HasMaxLength(250);
            entity.Property(e => e.NhaO).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NoiCap).HasMaxLength(150);
            entity.Property(e => e.NoiSinh).HasMaxLength(550);
            entity.Property(e => e.PassExpireDate).HasColumnType("datetime");
            entity.Property(e => e.PhuongDcTamTru).HasMaxLength(150);
            entity.Property(e => e.PhuongDcThuongTru).HasMaxLength(150);
            entity.Property(e => e.Position).HasMaxLength(550);
            entity.Property(e => e.PostalCode).HasMaxLength(550);
            entity.Property(e => e.Qualifications).HasMaxLength(550);
            entity.Property(e => e.QuanDcTamTru).HasMaxLength(150);
            entity.Property(e => e.QuanDcThuongTru).HasMaxLength(150);
            entity.Property(e => e.QuocTich).HasMaxLength(550);
            entity.Property(e => e.Resident).HasMaxLength(550);
            entity.Property(e => e.SDTCaNhan).HasMaxLength(50);
            entity.Property(e => e.SDTCongTy).HasMaxLength(50);
            entity.Property(e => e.SDTNguoiThan).HasMaxLength(50);
            entity.Property(e => e.SDTNguoiThan2).HasMaxLength(150);
            entity.Property(e => e.STKChuyenLuong)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SoCMTND).HasMaxLength(250);
            entity.Property(e => e.SoHD).HasMaxLength(150);
            entity.Property(e => e.SoHDKXDTH).HasMaxLength(100);
            entity.Property(e => e.SoHDTV).HasMaxLength(100);
            entity.Property(e => e.SoHDXDTH).HasMaxLength(100);
            entity.Property(e => e.SoNhaDcTamTru).HasMaxLength(150);
            entity.Property(e => e.SoNhaDcThuongTru).HasMaxLength(150);
            entity.Property(e => e.SoSoBHXH)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.StartWorking).HasColumnType("datetime");
            entity.Property(e => e.Telephone).HasMaxLength(550);
            entity.Property(e => e.TinhDcTamTru).HasMaxLength(150);
            entity.Property(e => e.TinhDcThuongTru).HasMaxLength(150);
            entity.Property(e => e.TinhTrangKyHD).HasMaxLength(150);
            entity.Property(e => e.TonGiao).HasMaxLength(550);
            entity.Property(e => e.TongLuong).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TongPhuCap).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangPhuc).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(550);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserZaloID)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.XangXe).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Users__3214EC2783EB53AE");

            entity.Property(e => e.BHXH).HasMaxLength(250);
            entity.Property(e => e.BHYT).HasMaxLength(250);
            entity.Property(e => e.BankAccount).HasMaxLength(250);
            entity.Property(e => e.BirthOfDate).HasColumnType("datetime");
            entity.Property(e => e.CMTND).HasMaxLength(250);
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Communication).HasMaxLength(100);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(250);
            entity.Property(e => e.EmailCom).HasMaxLength(250);
            entity.Property(e => e.FullName).HasMaxLength(250);
            entity.Property(e => e.HandPhone).HasMaxLength(100);
            entity.Property(e => e.HomeAddress).HasMaxLength(100);
            entity.Property(e => e.ImagePath).HasColumnType("ntext");
            entity.Property(e => e.JobDescription).HasMaxLength(200);
            entity.Property(e => e.LoginName).HasMaxLength(50);
            entity.Property(e => e.MST).HasMaxLength(250);
            entity.Property(e => e.Organization)
                .HasMaxLength(550)
                .HasComment("Đơn vị công tác");
            entity.Property(e => e.PassExpireDate).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(250);
            entity.Property(e => e.PinPassword).HasMaxLength(255);
            entity.Property(e => e.Position)
                .HasMaxLength(550)
                .HasComment("Vị trí");
            entity.Property(e => e.PostalCode).HasMaxLength(50);
            entity.Property(e => e.Qualifications).HasMaxLength(250);
            entity.Property(e => e.ReferralSource)
                .HasMaxLength(550)
                .HasComment("Nguồn giới thiệu");
            entity.Property(e => e.Resident).HasMaxLength(100);
            entity.Property(e => e.StartWorking).HasColumnType("datetime");
            entity.Property(e => e.Telephone).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    private partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}