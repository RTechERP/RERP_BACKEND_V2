using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RERPAPI.IRepo;
using RERPAPI.Middleware;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Repo;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;
using RERPAPI.Repo.GenericEntity.Asset;
using RERPAPI.Repo.GenericEntity.BBNV;
using RERPAPI.Repo.GenericEntity.Duan.MeetingMinutes;
using RERPAPI.Repo.GenericEntity.HRM;
using RERPAPI.Repo.GenericEntity.HRM.Vehicle;
using RERPAPI.Repo.GenericEntity.Project;
using RERPAPI.Repo.GenericEntity.Systems;
using RERPAPI.Repo.GenericEntity.TB;
using RERPAPI.Repo.GenericEntity.Technical;
using RTCApi.Repo.GenericRepo;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserPermissionService, UserPermissionService>();
builder.Services.AddScoped<RTCContext>();
builder.Services.AddScoped<EmployeeOnLeaveRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericEntity.AddressStockRepo>();
builder.Services.AddScoped<BillDocumentExportLogRepo>();
builder.Services.AddScoped<BillDocumentImportLogRepo>();
builder.Services.AddScoped<BillDocumentImportRepo>();
builder.Services.AddScoped<BillExportDetailSerialNumberModulaLocationRepo>();
builder.Services.AddScoped<BillExportDetailSerialNumberRepo>();
builder.Services.AddScoped<BillExportDetailTechnicalRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericEntity.BillExportTechDetailSerialRepo>();
builder.Services.AddScoped<BillImportDetailRepo>();
builder.Services.AddScoped<BillImportDetailSerialNumberModulaLocationRepo>();
builder.Services.AddScoped<BillImportDetailSerialNumberRepo>();
builder.Services.AddScoped<BillImportLogRepo>();
builder.Services.AddScoped<BillImportRepo>();
builder.Services.AddScoped<BillImportTechDetailSerialRepo>();
builder.Services.AddScoped<BusinessFieldLinkRepo>();
builder.Services.AddScoped<BusinessFieldRepo>();
builder.Services.AddScoped<ConfigSystemRepo>();
builder.Services.AddScoped<CurrencyRepo>();
builder.Services.AddScoped<CustomerContactRepo>();
builder.Services.AddScoped<CustomerEmployeeRepo>();
builder.Services.AddScoped<CustomerPartsRepo>();
builder.Services.AddScoped<CustomerRepo>();
builder.Services.AddScoped<CustomerSpecializationRepo>();
builder.Services.AddScoped<DailyReportTechnicalRepo>();
builder.Services.AddScoped<DepartmentRepo>();
builder.Services.AddScoped<DocumentImportPONCCRepo>();
builder.Services.AddScoped<DocumentImportRepo>();
builder.Services.AddScoped<EmployeeApproveRepo>();
builder.Services.AddScoped<EmployeeApprovedRepo>();
builder.Services.AddScoped<EmployeeAttendanceRepo>();
builder.Services.AddScoped<EmployeeBussinessRepo>();
builder.Services.AddScoped<EmployeeContractRepo>();
builder.Services.AddScoped<EmployeeContractTypeRepo>();
builder.Services.AddScoped<EmployeeCurricularRepo>();
builder.Services.AddScoped<EmployeeEarlyLateRepo>();
builder.Services.AddScoped<EmployeeEducationLevelRepo>();
builder.Services.AddScoped<EmployeeErrorRepo>();
builder.Services.AddScoped<EmployeeFoodOrderRepo>();
builder.Services.AddScoped<EmployeeNoFingerprintRepo>();
builder.Services.AddScoped<EmployeeOnLeaveMasterRepo>();
builder.Services.AddScoped<EmployeeOnLeaveRepo>();
builder.Services.AddScoped<EmployeeOverTimeRepo>();
builder.Services.AddScoped<EmployeeProjectTypeRepo>();
builder.Services.AddScoped<EmployeePurchaseRepo>();
builder.Services.AddScoped<EmployeeRepo>();
builder.Services.AddScoped<EmployeeScheduleWorkRepo>();
builder.Services.AddScoped<EmployeeSendEmailRepo>();
builder.Services.AddScoped<EmployeeStatusRepo>();
builder.Services.AddScoped<EmployeeTeamRepo>();
builder.Services.AddScoped<EmployeeTeamSaleRepo>();
builder.Services.AddScoped<EmployeeTypeBussinessRepo>();
builder.Services.AddScoped<EmployeeTypeOverTimeRepo>();
builder.Services.AddScoped<EmployeeVehicleBussinessRepo>();
builder.Services.AddScoped<EmployeeWorkingProcessRepo>();
builder.Services.AddScoped<FirmBaseRepo>();
builder.Services.AddScoped<FirmRepo>();
builder.Services.AddScoped<FollowProjectBaseRepo>();
builder.Services.AddScoped<FollowProjectRepo>();
builder.Services.AddScoped<GroupFileRepo>();
builder.Services.AddScoped<GroupSaleRepo>();
builder.Services.AddScoped<HistoryDeleteBillRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericEntity.HistoryProductRTCRepo>();
builder.Services.AddScoped<HolidayRepo>();
builder.Services.AddScoped<InventoryProjectRepo>();
builder.Services.AddScoped<InventoryRepo>();
builder.Services.AddScoped<IssueCauseRepo>();
builder.Services.AddScoped<IssueSolutionCauseLinkRepo>();
builder.Services.AddScoped<IssueSolutionDocumentRepo>();
builder.Services.AddScoped<IssueSolutionLogRepo>();
builder.Services.AddScoped<IssueLogSolutionRepo>();
builder.Services.AddScoped<IssueSolutionStatusLinkRepo>();
builder.Services.AddScoped<IssueSolutionStatusRepo>();
builder.Services.AddScoped<KPIEmployeeTeamLinkRepo>();
builder.Services.AddScoped<KPIEmployeeTeamRepo>();
builder.Services.AddScoped<KPIEvaluationRepo>();
builder.Services.AddScoped<LocationRepo>();
builder.Services.AddScoped<LoginManagerRepo>();
builder.Services.AddScoped<MainIndexRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericEntity.MeetingMinutesFileRepo>();
builder.Services.AddScoped<MenuRepo>();
builder.Services.AddScoped<ModulaLocationDetailRepo>();
builder.Services.AddScoped<ModulaLocationRepo>();
builder.Services.AddScoped<OfficeSupplyRepo>();
builder.Services.AddScoped<OfficeSupplyRequestsRepo>();
builder.Services.AddScoped<OfficeSupplyUnitRepo>();
builder.Services.AddScoped<POKHDetailMoneyRepo>();
builder.Services.AddScoped<POKHDetailRepo>();
builder.Services.AddScoped<POKHFilesRepo>();
builder.Services.AddScoped<POKHHistoryRepo>();
builder.Services.AddScoped<POKHRepo>();
builder.Services.AddScoped<PONCCDetailLogRepo>();
builder.Services.AddScoped<PONCCDetailRepo>();
builder.Services.AddScoped<PONCCDetailRequestBuyRepo>();
builder.Services.AddScoped<PONCCRepo>();
builder.Services.AddScoped<PONCCRulePayRepo>();
builder.Services.AddScoped<PositionContractRepo>();
builder.Services.AddScoped<PositionInternalRepo>();
builder.Services.AddScoped<ProductGroupRTCRepo>();
builder.Services.AddScoped<ProductGroupRepo>();
builder.Services.AddScoped<ProductGroupWareHouseRepo>();
builder.Services.AddScoped<ProductLocationRepo>();
builder.Services.AddScoped<ProductRTCQRCodeRepo>();
builder.Services.AddScoped<ProductRTCRepo>();
builder.Services.AddScoped<ProductSaleRepo>();
builder.Services.AddScoped<ProductsSaleRepo>();
builder.Services.AddScoped<ProjectCostRepo>();
builder.Services.AddScoped<ProjectCurrentSituationRepo>();
builder.Services.AddScoped<ProjectEmployeeRepo>();
builder.Services.AddScoped<ProjectPartListRepo>();
builder.Services.AddScoped<ProjectPartlistPriceRequestRepo>();
builder.Services.AddScoped<ProjectPartlistPurchaseRequestRepo>();
builder.Services.AddScoped<ProjectPartlistVersionRepo>();
builder.Services.AddScoped<ProjectPersonalPriotityRepo>();
builder.Services.AddScoped<ProjectPriorityLinkRepo>();
builder.Services.AddScoped<ProjectPriorityRepo>();
builder.Services.AddScoped<ProjectRepo>();
builder.Services.AddScoped<ProjectRequestRepo>();
builder.Services.AddScoped<ProjectSolutionFileRepo>();
builder.Services.AddScoped<ProjectSolutionRepo>();
builder.Services.AddScoped<ProjectStatusDetailRepo>();
builder.Services.AddScoped<ProjectStatusLogRepo>();
builder.Services.AddScoped<ProjectStatusRepo>();
builder.Services.AddScoped<ProjectSurveyDetailRepo>();
builder.Services.AddScoped<ProjectSurveyFileRepo>();
builder.Services.AddScoped<ProjectSurveyRepo>();
builder.Services.AddScoped<ProjectTreeFolderRepo>();
builder.Services.AddScoped<ProjectTypeBaseRepo>();
builder.Services.AddScoped<ProjectTypeLinkRepo>();
builder.Services.AddScoped<ProjectTypeRepo>();
builder.Services.AddScoped<ProjectUserRepo>();
builder.Services.AddScoped<ProjectWorkerTypeRepo>();
builder.Services.AddScoped<ProjectWorkerVersionRepo>();
builder.Services.AddScoped<ProvinceRepo>();
builder.Services.AddScoped<QuotationKHDetailRepo>();
builder.Services.AddScoped<QuotationKHRepo>();
builder.Services.AddScoped<RequestInvoiceDetailRepo>();
builder.Services.AddScoped<RequestInvoiceFileRepo>();
builder.Services.AddScoped<RequestInvoiceRepo>();
builder.Services.AddScoped<RulePayRepo>();
builder.Services.AddScoped<StatusWorkingProcessRepo>();
builder.Services.AddScoped<SupplierSaleContactRepo>();
builder.Services.AddScoped<SupplierSaleRepo>();
builder.Services.AddScoped<TeamRepo>();
builder.Services.AddScoped<TradePriceDetailRepo>();
builder.Services.AddScoped<TradePriceRepo>();
builder.Services.AddScoped<TrainingRegistrationApprovedFlowRepo>();
builder.Services.AddScoped<TrainingRegistrationApprovedRepo>();
builder.Services.AddScoped<TrainingRegistrationCategoryRepo>();
builder.Services.AddScoped<TrainingRegistrationDetailRepo>();
builder.Services.AddScoped<TrainingRegistrationFileRepo>();
builder.Services.AddScoped<TrainingRegistrationRepo>();
builder.Services.AddScoped<UnitCountKTRepo>();
builder.Services.AddScoped<UnitCountRepo>();
builder.Services.AddScoped<UserRepo>();
builder.Services.AddScoped<UserTeamLinkRepo>();
builder.Services.AddScoped<UserTeamRepo>();
builder.Services.AddScoped<WarehouseRepo>();
builder.Services.AddScoped<WeekPlanRepo>();
builder.Services.AddScoped<vUserGroupLinksRepo>();

// Project sub-namespace repos
builder.Services.AddScoped<ProjectItemFileRepo>();
builder.Services.AddScoped<ProjectItemRepo>();
builder.Services.AddScoped<ProjectItemProblemRepo>();

// ProjectAGV repos (same base namespace)
//builder.Services.AddScoped<ProjectDocumentsRepo>(); // falls under RERPAPI.Repo.GenericEntity
//builder.Services.AddScoped<ProjectIssuesRepo>();    // falls under RERPAPI.Repo.GenericEntity

builder.Services.AddScoped<VisitFactoryRepo>();
builder.Services.AddScoped<VisitFactoryDetailRepo>();
builder.Services.AddScoped<VisitGuestTypeRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericEntity.MeetingTypeRepo>();

builder.Services.AddScoped<FormAndFunctionRepo>();
builder.Services.AddScoped<UserGroupRepo>();

builder.Services.AddScoped<CategoriesRepo>();
builder.Services.AddScoped<ProductRTCRepo>();
builder.Services.AddScoped<ProductsRepo>();


builder.Services.AddScoped<TSAllocationAssetPersonalDetailRepo>();
builder.Services.AddScoped<TSAllocationAssetPersonalRepo>();
builder.Services.AddScoped<TSAllocationEvictionAssetRepo>();
builder.Services.AddScoped<TSAssetAllocationDetailRepo>();
builder.Services.AddScoped<TSAssetAllocationRepo>();
builder.Services.AddScoped<TSAssetManagementPersonalRepo>();
builder.Services.AddScoped<TSAssetManagementRepo>();
builder.Services.AddScoped<TSAssetRecoveryDetailRepo>();
builder.Services.AddScoped<TSAssetRecoveryRepo>();
builder.Services.AddScoped<TSAssetTransferDetailRepo>();
builder.Services.AddScoped<TSAssetTransferRepo>();
builder.Services.AddScoped<TSLiQuidationAssetRepo>();
builder.Services.AddScoped<TSLostReportAssetRepo>();
builder.Services.AddScoped<TSRecoveryAssetPersonalDetailRepo>();
builder.Services.AddScoped<TSRecoveryAssetPersonalRepo>();
builder.Services.AddScoped<TSRepairAssetRepo>();
builder.Services.AddScoped<TSReportBrokenAssetRepo>();
builder.Services.AddScoped<TSSourceAssetsRepo>();
builder.Services.AddScoped<TSStatusAssetRepo>();
builder.Services.AddScoped<TSTypeAssetPersonalRepo>();
builder.Services.AddScoped<TTypeAssetsRepo>();
builder.Services.AddScoped<UnitRepo>();

builder.Services.AddScoped<RERPAPI.Repo.GenericEntity.HRM.EmployeeChamCongDetailRepo>();
builder.Services.AddScoped<EmployeeChamCongMasterRepo>();
builder.Services.AddScoped<EmployeeChucVuHDRepo>();
builder.Services.AddScoped<EmployeeWFHRepo>();

builder.Services.AddScoped<ProposeVehicleRepairDetailRepo>();
builder.Services.AddScoped<ProposeVehicleRepairRepo>();
builder.Services.AddScoped<VehicleCategoryRepo>();
builder.Services.AddScoped<VehicleManagementRepo>();
builder.Services.AddScoped<VehicleRepairHistoryFileRepo>();
builder.Services.AddScoped<VehicleRepairHistoryRepo>();
builder.Services.AddScoped<VehicleRepairRepo>();
builder.Services.AddScoped<VehicleRepairTypeRepo>();

builder.Services.AddScoped<HandoverApproveRepo>();
builder.Services.AddScoped<HandoverAssetManagementRepo>();
builder.Services.AddScoped<HandoverFinanceRepo>();
builder.Services.AddScoped<HandoverReceiverRepo>();
builder.Services.AddScoped<HandoverRepo>();
builder.Services.AddScoped<HandoverSubordinateRepo>();
builder.Services.AddScoped<HandoverWarehouseAssetRepo>();
builder.Services.AddScoped<HandoverWorkRepo>();

builder.Services.AddScoped<MeetingMinuteRepo>();
builder.Services.AddScoped<MeetingMinutesAttendanceRepo>();
builder.Services.AddScoped<MeetingMinutesDetailRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericEntity.MeetingMinutesFileRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericEntity.MeetingTypeRepo>();
builder.Services.AddScoped<ProjectHistoryProblemRepo>();
builder.Services.AddScoped<ProjectManagerRepo>();

builder.Services.AddScoped<BillExportDetailSerialNumberRepo>();
builder.Services.AddScoped<BillExportDetailTechnicalRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericEntity.BillExportTechDetailSerialRepo>();
builder.Services.AddScoped<BillExportTechnicalRepo>();
builder.Services.AddScoped<BillImportDetailSerialNumberRepo>();
builder.Services.AddScoped<BillImportTechnicalDetailRepo>();
builder.Services.AddScoped<BillImportTechDetailSerialRepo>();
builder.Services.AddScoped<BillImportTechnicalRepo>();
builder.Services.AddScoped<HistoryDeleteBillRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericEntity.HistoryProductRTCRepo>();
builder.Services.AddScoped<InventoryDemoRepo>();
builder.Services.AddScoped<KPIEvaluationErrorRepo>();
builder.Services.AddScoped<ProductRTCQRCodeRepo>();

builder.Services.AddScoped<RERPAPI.Repo.GenericEntity.AddressStockRepo>();
builder.Services.AddScoped<BillDocumentExportRepo>();
builder.Services.AddScoped<BillExportDetailRepo>();
builder.Services.AddScoped<BillExportLogRepo>();
builder.Services.AddScoped<BillExportRepo>();
builder.Services.AddScoped<DocumentExportRepo>();
builder.Services.AddScoped<InventoryProjectExportRepo>();
builder.Services.AddScoped<InvoiceLinkRepo>();
builder.Services.AddScoped<SupplierSaleRepo>();
builder.Services.AddScoped<SupplierSaleContactRepo>();
builder.Services.AddScoped<ProjectFieldRepo>();


builder.Services.AddScoped<HRHiringRequestRepo>();
builder.Services.AddScoped<HRHiringAppearanceLinkRepo>();
builder.Services.AddScoped<HRHiringRequestApproveLinkRepo>();
builder.Services.AddScoped<HRHiringRequestCommunicationLinkRepo>();
builder.Services.AddScoped<HRHiringRequestComputerLevelLinkRepo>();
builder.Services.AddScoped<HRHiringRequestEducationLinkRepo>();
builder.Services.AddScoped<HRHiringRequestExperienceLinkRepo>();
builder.Services.AddScoped<HRHiringRequestGenderLinkRepo>();
builder.Services.AddScoped<HRHiringRequestHealthLinkRepo>();
builder.Services.AddScoped<HRHiringRequestLanguageLinkRepo>();

// BillExportTechnicalRepo in RTCApi namespace (used by Old Technical controller)
builder.Services.AddScoped<BillExportTechnicalRepo>();

builder.Services.AddScoped<CurrentUser>(provider =>
{

    var context = provider.GetRequiredService<IHttpContextAccessor>().HttpContext;
    var claims = context?.User.Claims.ToDictionary(x => x.Type, x => x.Value);
    CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
    return currentUser;

});

//Config connect database
Config.ConnectionString = builder.Configuration.GetValue<string>("ConnectionString") ?? "";
builder.Services.AddDbContext<RTCContext>(o => o.UseSqlServer(Config.ConnectionString));

builder.Services.AddMvc().AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = null);


//Config CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCors", builder =>
    {

        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});





//Config FormOption

//builder.Services.Configure<IISServerOptions>(options =>
//{
//    options.MaxRequestBodySize = int.MaxValue;
//});

//builder.Services.Configure<FormOptions>(opt =>
//{
//    // Kích thước tối đa mỗi phần form (field/file) 
//    opt.MultipartBodyLengthLimit = Int32.MaxValue;

//    // Nếu file < 1 MB thì vẫn buffer hết trong RAM trước khi viết ra
//    opt.MemoryBufferThreshold = 1 * 1024 * 1024;
//    // (Tuỳ chọn) nếu có rất nhiều fields, tăng số fields tối đa
//    opt.ValueCountLimit = 1000;

//    // (Tuỳ chọn) tăng độ dài tối đa tên key/value nếu cần
//    opt.ValueLengthLimit = 64 * 1024;
//});


// Load JWT settings
var jwtSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSection);
var jwtSettings = jwtSection.Get<JwtSettings>() ?? new JwtSettings();

builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                        NameClaimType = "sub" // Để Middleware lấy đúng UserID
                    };
                });
builder.Services.AddAuthentication();


//Config session
builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromMinutes(jwtSettings.ExpireMinutes);
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
    opt.Cookie.Name = "r-erp";
});



builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true; // Chuyển tất cả URL thành chữ thường
});


builder.Services.Configure<ModulaConfig>(builder.Configuration.GetSection("ModulaConfig"));

// Nếu bạn muốn inject trực tiếp:
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<ModulaConfig>>().Value);

builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<ModulaConfig>();

    return new PersistentTcpClientService(
        config.IpAddress,
        config.Port,
        connectTimeoutMs: 3000,
        sendTimeoutMs: 3000,
        receiveTimeoutMs: 3000,
        maxReconnectAttempts: 3,
        reconnectDelayMs: 1000
    );
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseCors("MyCors");
app.UseAuthorization();
//app.UseAuthentication();
app.UseSession();
app.UseMiddleware<DynamicAuthorizationMiddleware>();

app.MapControllers();


//Config static file

app.Use(async (context, next) =>
{
    context.Request.Path = context.Request.Path.Value?.ToLower();
    await next();
});


app.UseStaticFiles();
List<PathStaticFile> staticFiles = builder.Configuration.GetSection("PathStaticFiles").Get<List<PathStaticFile>>() ?? new List<PathStaticFile>();
foreach (var item in staticFiles)
{
    app.UseStaticFiles(new StaticFileOptions()
    {
        FileProvider = new PhysicalFileProvider(item.PathFull),
        RequestPath = new PathString($"/api/share/{item.PathName.Trim().ToLower()}")
    });


    app.UseDirectoryBrowser(new DirectoryBrowserOptions
    {
        FileProvider = new PhysicalFileProvider(item.PathFull),
        RequestPath = new PathString($"/api/share/{item.PathName.Trim().ToLower()}")
    });
}

app.Run();
