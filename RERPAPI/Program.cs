using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RERPAPI.Controllers.ESL;
using RERPAPI.IRepo;
using RERPAPI.Middleware;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;
using RERPAPI.Repo.GenericEntity.Asset;
using RERPAPI.Repo.GenericEntity.BBNV;
using RERPAPI.Repo.GenericEntity.DocumentManager;
using RERPAPI.Repo.GenericEntity.Duan.MeetingMinutes;
using RERPAPI.Repo.GenericEntity.ESL;
using RERPAPI.Repo.GenericEntity.Film;
using RERPAPI.Repo.GenericEntity.GeneralCatetogy;
using RERPAPI.Repo.GenericEntity.GeneralCatetogy.JobRequirements;
using RERPAPI.Repo.GenericEntity.GeneralCatetogy.PaymentOrders;
using RERPAPI.Repo.GenericEntity.HRM;
using RERPAPI.Repo.GenericEntity.HRM.DepartmentRequire;
using RERPAPI.Repo.GenericEntity.HRM.FlightBooking;
using RERPAPI.Repo.GenericEntity.HRM.HotelBooking;
using RERPAPI.Repo.GenericEntity.HRM.HRRecruitmentInterviewAssessment;
using RERPAPI.Repo.GenericEntity.HRM.ProductProtectiveGear;
using RERPAPI.Repo.GenericEntity.HRM.Vehicle;

using RERPAPI.Repo.GenericEntity.HRRecruitmentExamRepo;
using RERPAPI.Repo.GenericEntity.KPISale;
using RERPAPI.Repo.GenericEntity.MakerTrainingFirm;
using RERPAPI.Repo.GenericEntity.MeetingMinutesRepo;
using RERPAPI.Repo.GenericEntity.Project;
using RERPAPI.Repo.GenericEntity.Systems;
using RERPAPI.Repo.GenericEntity.TB;
using RERPAPI.Repo.GenericEntity.Technical;
using RERPAPI.Repo.GenericEntity.Technical.KPI;
using RERPAPI.Repo.GenericEntity.Warehouses.AGV;
using RERPAPI.SendService;
using RTCApi.Repo.GenericRepo;
using Serilog;
using Serilog.Events;
using System.Text;
using tusdotnet;
using tusdotnet.Helpers;
using tusdotnet.Models;
using tusdotnet.Models.Configuration;
using tusdotnet.Models.Expiration;
using tusdotnet.Stores;
using RERPAPI.Repo.GenericEntity.HRM.Visa;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName);
});

builder.Services.AddHttpContextAccessor();

#region Injection Repositories and Services

builder.Services.AddScoped<IUserPermissionService, UserPermissionService>();
builder.Services.AddScoped<RTCContext>();
builder.Services.AddScoped<RoleConfig>();
builder.Services.AddScoped<IFirebaseNotificationService, FirebaseNotificationService>();

builder.Services.AddScoped<EmployeeOnLeaveRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericEntity.AddressStockRepo>();
builder.Services.AddScoped<AccountingContractRepo>();
builder.Services.AddScoped<AccountingContractLogRepo>();
builder.Services.AddScoped<AccountingContractFileRepo>();
builder.Services.AddScoped<AccountingContractTypeRepo>();
builder.Services.AddScoped<BillDocumentExportLogRepo>();
builder.Services.AddScoped<BillDocumentImportLogRepo>();
builder.Services.AddScoped<EmployeeNightShiftRepo>();
builder.Services.AddScoped<BillDocumentImportRepo>();
builder.Services.AddScoped<EmployeeBussinessFileRepo>();
builder.Services.AddScoped<BillExportDetailSerialNumberModulaLocationRepo>();
builder.Services.AddScoped<BillExportDetailSerialNumberRepo>();
builder.Services.AddScoped<BillExportDetailTechnicalRepo>();
builder.Services.AddScoped<BonusRuleIndexRepo>();
    builder.Services.AddScoped<VehicleBookingManagementRepo>();
    builder.Services.AddScoped<VehicleRentalRequestRepo>();

    builder.Services.AddScoped<VehicleBookingFileRepo>();
builder.Services.AddScoped<BillExportTechDetailSerialRepo>();
builder.Services.AddScoped<BillImportDetailRepo>();
builder.Services.AddScoped<BillImportDetailSerialNumberModulaLocationRepo>();
builder.Services.AddScoped<BillImportDetailSerialNumberRepo>();
builder.Services.AddScoped<BillImportLogRepo>();
builder.Services.AddScoped<BillImportRepo>();
builder.Services.AddScoped<EmployeeOvertimeFileRepo>();
builder.Services.AddScoped<BillImportTechDetailSerialRepo>();
builder.Services.AddScoped<BusinessFieldLinkRepo>();
builder.Services.AddScoped<BusinessFieldRepo>();
builder.Services.AddScoped<ConfigSystemRepo>();
builder.Services.AddScoped<OfficeSupplyRequestsDetailRepo>();
builder.Services.AddScoped<CurrencyRepo>();
builder.Services.AddScoped<CustomerContactRepo>();
builder.Services.AddScoped<CustomerEmployeeRepo>();
builder.Services.AddScoped<CustomerPartsRepo>();
builder.Services.AddScoped<CustomerRepo>();
builder.Services.AddScoped<CourseCatalogRepo>();
builder.Services.AddScoped<CustomerSpecializationRepo>();
builder.Services.AddScoped<DailyReportTechnicalRepo>();
builder.Services.AddScoped<TeamEmployeeProjectRepo>();
builder.Services.AddScoped<DailyReportSaleRepo>();
builder.Services.AddScoped<DailyReportSaleAdminRepo>();
builder.Services.AddScoped<DocumentTypeRepo>();
builder.Services.AddScoped<DocumentRepo>();
builder.Services.AddScoped<DocumentFileRepo>();
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
builder.Services.AddScoped<EmployeePayrollDeductionRepo>();
builder.Services.AddScoped<EmployeeDeductionTypeRepo>();
builder.Services.AddScoped<EmployeeFoodOrderRepo>();
builder.Services.AddScoped<EmployeeNoFingerprintRepo>();
builder.Services.AddScoped<EmployeeOnLeaveMasterRepo>();
builder.Services.AddScoped<EmployeeOnLeaveRepo>();
builder.Services.AddScoped<EmployeeOnLeavePhaseRepo>();
builder.Services.AddScoped<EmployeeOverTimeRepo>();
builder.Services.AddScoped<EmployeeProjectTypeRepo>();
builder.Services.AddScoped<EmployeePurchaseRepo>();
builder.Services.AddScoped<EmployeeRepo>();
builder.Services.AddScoped<EmployeeScheduleWorkRepo>();
builder.Services.AddScoped<EmployeeSendEmailRepo>();
builder.Services.AddScoped<EmployeeStatusRepo>();
builder.Services.AddScoped<EmployeeTeamRepo>();
builder.Services.AddScoped<EmployeeTeamSaleRepo>();
builder.Services.AddScoped<EmployeeTeamSaleLinkRepo>();
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
builder.Services.AddScoped<GroupSalesUserRepo>();
builder.Services.AddScoped<HistoryDeleteBillRepo>();
builder.Services.AddScoped<HistoryProductRTCRepo>();
builder.Services.AddScoped<HolidayRepo>();
builder.Services.AddScoped<HistoryMoneyPORepo>();
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
builder.Services.AddScoped<KPIEvaluationFactorRepo>();
builder.Services.AddScoped<KPIEmployeeTeamRepo>();
builder.Services.AddScoped<KPIEvaluationRepo>();
builder.Services.AddScoped<KPIErrorTypeRepo>();
builder.Services.AddScoped<KPIErrorRepo>();
builder.Services.AddScoped<KPIErrorFineAmountRepo>();
builder.Services.AddScoped<KPIErrorEmployeeFileRepo>();
builder.Services.AddScoped<KPIErrorEmployeeRepo>();
builder.Services.AddScoped<KPICriteriaDetailRepo>();
builder.Services.AddScoped<KPICriterionRepo>();
builder.Services.AddScoped<KPIPositionEmployeeRepo>();
builder.Services.AddScoped<KPIPositionRepo>();
builder.Services.AddScoped<KPIExamRepo>();
builder.Services.AddScoped<KPIExamPositionRepo>();
builder.Services.AddScoped<KPISpecializationTypeRepo>();
builder.Services.AddScoped<LocationRepo>();
builder.Services.AddScoped<LoginManagerRepo>();
builder.Services.AddScoped<MainIndexRepo>();
builder.Services.AddScoped<MeetingMinutesFileRepo>();
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
builder.Services.AddScoped<PONCCHistoryRepo>();
builder.Services.AddScoped<PositionContractRepo>();
builder.Services.AddScoped<PositionInternalRepo>();
//builder.Services.AddScoped<ProductGroupRTCRepo>();
//builder.Services.AddScoped<JobRequirementRecommendRepo>();
//builder.Services.AddScoped<JobRequirementRecommendDetailRepo>();
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
builder.Services.AddScoped<ProjectTypeDepartmentRepo>();
builder.Services.AddScoped<ProjectUserRepo>();
builder.Services.AddScoped<ProjectGateRepo>();
builder.Services.AddScoped<ProjectGateStepRepo>();
builder.Services.AddScoped<ProjectGateStepTemplateRepo>();
builder.Services.AddScoped<ProjectGateStepLinkRepo>();
builder.Services.AddScoped<ProjectGateStepWorkerRepo>();
builder.Services.AddScoped<ProjectGateDepartmentRepo>();
builder.Services.AddScoped<ProjectGateStepPositionRepo>();
builder.Services.AddScoped<ProjectGateStepCheckListRepo>();
builder.Services.AddScoped<ProjectGateStepCheckListLinkRepo>();
builder.Services.AddScoped<ProjectGateStepCheckListDetailRepo>();
builder.Services.AddScoped<ProjectGateStepFileRepo>();
builder.Services.AddScoped<ProjectWorkerTypeRepo>();
builder.Services.AddScoped<ProjectWorkerVersionRepo>();
builder.Services.AddScoped<ProvinceRepo>();
builder.Services.AddScoped<QuotationKHDetailRepo>();
builder.Services.AddScoped<QuotationKHRepo>();
builder.Services.AddScoped<ReportTypeRepo>();
builder.Services.AddScoped<RegisterIdeaTypeRepo>();
builder.Services.AddScoped<RegisterIdeaScoreRepo>();
builder.Services.AddScoped<RegisterIdeaFileRepo>();
builder.Services.AddScoped<RegisterIdeaDetailRepo>();
builder.Services.AddScoped<RequestInvoiceDetailRepo>();
builder.Services.AddScoped<RegisterIdeaRepo>();
builder.Services.AddScoped<RequestInvoiceStatusRepo>();
builder.Services.AddScoped<RequestInvoiceStatusLinkRepo>();
builder.Services.AddScoped<RequestInvoiceFileRepo>();
builder.Services.AddScoped<RequestInvoiceRepo>();
builder.Services.AddScoped<RulePayRepo>();
builder.Services.AddScoped<StatusWorkingProcessRepo>();
builder.Services.AddScoped<SupplierSaleContactRepo>();
builder.Services.AddScoped<SupplierSaleRepo>();
builder.Services.AddScoped<SealRegulationsRepo>();
builder.Services.AddScoped<SaleUserTypeRepo>();
builder.Services.AddScoped<SalesPerformanceRankingRepo>();
builder.Services.AddScoped<StatusRequestInvoiceLinkRepo>();
builder.Services.AddScoped<TrackingMarksFileRepo>();
builder.Services.AddScoped<TrackingMarksTaxCompanyRepo>();
builder.Services.AddScoped<TrackingMarksSealRepo>();
builder.Services.AddScoped<TrackingMarksRepo>();
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
builder.Services.AddScoped<PinResetTokenRepo>();
builder.Services.AddScoped<NotifyRepo>();

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

builder.Services.AddScoped<FormAndFunctionRepo>();
builder.Services.AddScoped<FormAndFunctionGroupRepo>();
builder.Services.AddScoped<UserGroupRepo>();
builder.Services.AddScoped<UserGroupLinkRepo>();
builder.Services.AddScoped<UserGroupRightDistributionRepo>();

builder.Services.AddScoped<CategoriesRepo>();
builder.Services.AddScoped<ProductRTCRepo>();
builder.Services.AddScoped<ProductsRepo>();

builder.Services.AddScoped<TSAllocationAssetPersonalDetailRepo>();
builder.Services.AddScoped<FilmManagementRepo>();

builder.Services.AddScoped<FilmManagementDetailRepo>();

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
builder.Services.AddScoped<FlightBookingManagementRepo>();
builder.Services.AddScoped<FlightBookingProposalRepo>();
builder.Services.AddScoped<FlightBookingPassengerRepo>();
builder.Services.AddScoped<HotelBookingManagementRepo>();
builder.Services.AddScoped<HotelBookingProposalRepo>();
builder.Services.AddScoped<HotelBookingEmployeeRepo>();

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
builder.Services.AddScoped<MeetingMinutesFileRepo>();
builder.Services.AddScoped<MeetingTypeRepo>();
builder.Services.AddScoped<ProjectHistoryProblemRepo>();
builder.Services.AddScoped<ProjectHistoryProblemDetailRepo>();
builder.Services.AddScoped<ProjectManagerRepo>();

builder.Services.AddScoped<BillExportDetailSerialNumberRepo>();
builder.Services.AddScoped<BillExportDetailTechnicalRepo>();
builder.Services.AddScoped<BillExportTechnicalRepo>();
builder.Services.AddScoped<BillImportDetailSerialNumberRepo>();
builder.Services.AddScoped<BillImportTechnicalDetailRepo>();
builder.Services.AddScoped<BillImportTechDetailSerialRepo>();
builder.Services.AddScoped<BillImportTechnicalRepo>();
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
builder.Services.AddScoped<SupplierSaleLinkRepo>();
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
builder.Services.AddScoped<ProjectWorkerRepo>();
builder.Services.AddScoped<ProjectRequestFileRepo>();
builder.Services.AddScoped<FollowProjectBaseDetailRepo>();

builder.Services.AddScoped<TaxCompanyRepo>();
builder.Services.AddScoped<ProjectPartlistPurchaseRequestTypeRepo>();
builder.Services.AddScoped<ProjectPartlistPriceRequestTypeRepo>();
builder.Services.AddScoped<ProjectPartlistPriceRequestNoteRepo>();
builder.Services.AddScoped<InventoryStockRepo>();

#endregion Injection Repositories and Services

#region EmployeePayroll

builder.Services.AddScoped<EmployeePayrollRepo>();
builder.Services.AddScoped<EmployeePayrollDetailRepo>();
builder.Services.AddScoped<EmployeePayrollBonusDeuctionRepo>();

#endregion EmployeePayroll

#region Yêu cần mua hàng

builder.Services.AddScoped<SupplierRepo>();
builder.Services.AddScoped<ProjectTypeAssignRepo>();
builder.Services.AddScoped<InventoryProjectProductSaleLinkRepo>();

#endregion Yêu cần mua hàng

#region Yêu Cầu QC

builder.Services.AddScoped<BillImportQCRepo>();
builder.Services.AddScoped<BillImportQCDetailRepo>();
builder.Services.AddScoped<BillImportQCDetailFilesRepo>();

#endregion Yêu Cầu QC

#region Kho AGV

builder.Services.AddScoped<AGVProductRepo>();
builder.Services.AddScoped<AGVProductGroupRepo>();
builder.Services.AddScoped<AGVProductGroupLinkRepo>();
builder.Services.AddScoped<AGVBillImportRepo>();
builder.Services.AddScoped<AGVBillImportDetailRepo>();
builder.Services.AddScoped<AGVBillExportRepo>();
builder.Services.AddScoped<AGVBillExportDetailRepo>();
builder.Services.AddScoped<AGVInventoryDemoRepo>();
builder.Services.AddScoped<AGVHistoryProductRepo>();

#endregion Kho AGV

#region YCCV

builder.Services.AddScoped<JobRequirementRepo>();
builder.Services.AddScoped<JobRequirementDetailRepo>();
builder.Services.AddScoped<DepartmentRequiredRepo>();
builder.Services.AddScoped<HCNSProposalsRepo>();

#endregion YCCV

builder.Services.AddScoped<BillExportTechnicalRepo>();
builder.Services.AddScoped<EmployeeBussinessVehicleRepo>();
builder.Services.AddScoped<TaxCompanyRepo>();
builder.Services.AddScoped<HistoryErrorRepo>();
builder.Services.AddScoped<HistoryProductRTCLogRepo>();
builder.Services.AddScoped<BillImportTechnicalLogRepo>();
builder.Services.AddScoped<BillImportDetailTechnicalRepo>();
builder.Services.AddScoped<BillDocumentImportTechnicalRepo>();
builder.Services.AddScoped<BillDocumentImportTechnicalLogRepo>();
builder.Services.AddScoped<BillExportTechnicalLogRepo>();
builder.Services.AddScoped<RegisterContractRepo>();
builder.Services.AddScoped<BookingRoomRepo>();
builder.Services.AddScoped<PhasedAllocationPersonDetailRepo>();
builder.Services.AddScoped<JobRequirementFileRepo>();
builder.Services.AddScoped<JobRequirementApprovedRepo>();
builder.Services.AddScoped<JobRequirementCommentRepo>();
builder.Services.AddScoped<OrganizationalChartDetailRepo>();
builder.Services.AddScoped<OrganizationalChartRepo>();
builder.Services.AddScoped<NewsletterTypeRepo>();
builder.Services.AddScoped<NewsletterRepo>();
builder.Services.AddScoped<NewsletterFileRepo>();
builder.Services.AddScoped<CourseCatalogRepo>();
builder.Services.AddScoped<CourseCatalogProjectTypeRepo>();
builder.Services.AddScoped<KPIPositionTypeRepo>();
builder.Services.AddScoped<CourseRepo>();
builder.Services.AddScoped<CourseRegisterIdeaRepo>();
builder.Services.AddScoped<CourseLessonRepo>();
builder.Services.AddScoped<CourseFilesRepo>();
builder.Services.AddScoped<CourseExamRepo>();
builder.Services.AddScoped<CourseLessonHistoryRepo>();
builder.Services.AddScoped<CourseExamResultRepo>();
builder.Services.AddScoped<CourseExamResultDetailRepo>();
builder.Services.AddScoped<CourseQuestionRepo>();
builder.Services.AddScoped<CourseRightAnswerRepo>();
builder.Services.AddScoped<CourseExamEvaluateRepo>();
builder.Services.AddScoped<CourseAnswerRepo>();
builder.Services.AddScoped<CourseExamPracticeRepo>();
builder.Services.AddScoped<ExamResultRepo>();
builder.Services.AddScoped<ExamResultDetailRepo>();
builder.Services.AddScoped<ExamResultAnswerDetailRepo>();

builder.Services.AddScoped<InventoryProjectProductSaleLinkRepo>();
builder.Services.AddScoped<HandoverPersonalAssetRepo>();
builder.Services.AddScoped<UpdateVersionRepo>();

builder.Services.AddScoped<ProjectTaskRepo>();
builder.Services.AddScoped<ProjectTaskGroupRepo>();
builder.Services.AddScoped<ProjectTaskChecklistRepo>();
builder.Services.AddScoped<ProjectTaskEmailBandRepo>();
builder.Services.AddScoped<ProjectTaskAttendanceRepo>();
builder.Services.AddScoped<ProjectTaskAttachmentRepo>();
builder.Services.AddScoped<ProjectTaskAdditionalRepo>();
builder.Services.AddScoped<ProjectTaskSettingRepo>();

builder.Services.AddScoped<SendEmailReceiveProjectTaskClass>();

builder.Services.AddScoped<FollowProjectBaseDetailRepo>();
builder.Services.AddScoped<DailyReportAccountingRepo>();

builder.Services.AddScoped<ProductSaleGroupWarehouseLinkRepo>();
builder.Services.AddScoped<FiveSRatingDetailRepo>();
builder.Services.AddScoped<FiveSRuleErrorRepo>();
builder.Services.AddScoped<FiveSErrorRepo>();
builder.Services.AddScoped<FiveSRatingRepo>();
builder.Services.AddScoped<FiveSDepartmentRepo>();
builder.Services.AddScoped<FiveSRatingTicketRepo>();
builder.Services.AddScoped<FiveSBonusMinusRepo>();

builder.Services.AddScoped<HRRecruitmentInterviewAssessmentFormRepo>();
builder.Services.AddScoped<HRRecruitmentApplicationFormRepo>();
builder.Services.AddScoped<HRRecruitmentApproveRepo>();
builder.Services.AddScoped<JobPerfomanceEvaluationApproveRepo>();
builder.Services.AddScoped<JobPerfomanceEvaluationNewRepo>();
builder.Services.AddScoped<JobPerfomanceEvaluationNewLogRepo>();

builder.Services.AddScoped<ProjectHistoryProblemProjectItemLinkRepo>();
builder.Services.AddScoped<ProjectHistoryProblemPartListLinkRepo>();
builder.Services.AddScoped<ProjectHistoryProblemReceiverLinkRepo>();
builder.Services.AddScoped<ProjectHistoryProblemWorkerLinkRepo>();
builder.Services.AddScoped<ProjectHistoryProblemFileRepo>();
builder.Services.AddScoped<ProjectHistoryProblemLogRepo>();
builder.Services.AddScoped<DrawingRepo>();
builder.Services.AddScoped<DrawingLogRepo>();
builder.Services.AddScoped<JobRequirementRecommendRepo>();
builder.Services.AddScoped<JobRequirementRecommendDetailRepo>();
builder.Services.AddScoped<AppMobileVersionRepo>();

#region khóa học

builder.Services.AddScoped<CoureTypeRepo>();
builder.Services.AddScoped<CourseKPIEmployeeTeamLinkRepo>();
builder.Services.AddScoped<CourseKPIEmployeeTeamMapRepo>();
builder.Services.AddScoped<CourseKPIEmployeeTeamRepo>();

#endregion khóa học

#region Tủ đồ bảo hộ

builder.Services.AddScoped<ProductGroupRTCRepo>();

#endregion Tủ đồ bảo hộ

#region Kế hoạch tuần

builder.Services.AddScoped<WorkPlanRepo>();
builder.Services.AddScoped<WorkPlanDetailRepo>();

#endregion Kế hoạch tuần

#region Đề nghị thanh toán

builder.Services.AddScoped<PaymentOrderRepo>();
builder.Services.AddScoped<PaymentOrderDetailRepo>();
builder.Services.AddScoped<PaymentOrderLogRepo>();
builder.Services.AddScoped<PaymentOrderApproveFollowRepo>();
builder.Services.AddScoped<PaymentOrderTypeRepo>();
builder.Services.AddScoped<PaymentOrderFileRepo>();
builder.Services.AddScoped<PaymentOrderFileBankSlipRepo>();
builder.Services.AddScoped<PaymentOrderTypeRepo>();
builder.Services.AddScoped<PaymentOrderDetailUserTeamSaleRepo>();
builder.Services.AddScoped<PaymentOrderPORepo>();

#endregion Đề nghị thanh toán

builder.Services.AddScoped<DailyReportHRRepo>();
builder.Services.AddScoped<DailyReportLXCP>();
builder.Services.AddScoped<DailyReportMarketingFileRepo>();
builder.Services.AddScoped<EconomicContractRepo>();
builder.Services.AddScoped<EconomicContractFileRepo>();
builder.Services.AddScoped<EconomicContractTypeRepo>();
builder.Services.AddScoped<EconomicContractTermRepo>();

builder.Services.AddScoped<PhasedAllocationPersonRepo>();
builder.Services.AddScoped<PhasedAllocationPersonDetailRepo>();

builder.Services.AddScoped<MenuAppRepo>();
builder.Services.AddScoped<MenuAppUserGroupLinkRepo>();
builder.Services.AddScoped<ProjectPartListPurchaseRequestApproveLogRepo>();
builder.Services.AddScoped<EmployeeLuckyNumberRepo>();
builder.Services.AddScoped<ProductGroupLinkRepo>();

//phần lĩnh vực và công nghệ dự án
builder.Services.AddScoped<ProjectApplicationTypesRepo>();
builder.Services.AddScoped<ProjectTechnologiesRepo>();
builder.Services.AddScoped<ProjectTypeApplicationLinkRepo>();
builder.Services.AddScoped<ProjectTypeTechnologyLinkRepo>();
builder.Services.AddScoped<CustomerIndustriesRepo>();
// mobile
builder.Services.AddScoped<FcmTokenRepo>();
builder.Services.AddScoped<NotificationTypeLinkRepo>();
builder.Services.AddScoped<NotificationTypeRepo>();
builder.Services.AddScoped<JobRequirementLogRepo>();
//builder.Services.AddScoped<AssetLogRepo>();

#region KPI Tech

builder.Services.AddScoped<KPISaleRepo>();
builder.Services.AddScoped<KPIEvaluationPointRepo>();
builder.Services.AddScoped<KPISessionRepo>();
builder.Services.AddScoped<KPIEmployeePointRepo>();
builder.Services.AddScoped<KPIPositionRepo>();
builder.Services.AddScoped<KPIEvaluationRuleRepo>();
builder.Services.AddScoped<KPIPositionEmployeeRepo>();
builder.Services.AddScoped<KPIEmployeePointDetailRepo>();

builder.Services.AddScoped<ProjectTaskChecklist>();
builder.Services.AddScoped<ProjectTaskEmployeeRepo>();
builder.Services.AddScoped<ProjectTaskApproveRepo>();
builder.Services.AddScoped<ProjectTaskLogRepo>();
builder.Services.AddScoped<ProjectTaskTypeRepo>();

builder.Services.AddScoped<KPIEvaluationRuleDetailRepo>();
builder.Services.AddScoped<KPIExamRepo>();
builder.Services.AddScoped<KPISumaryEvaluationRepo>();
builder.Services.AddScoped<KPIEvaluationLogRepo>(); 
#endregion

#region Yêu cầu tuyển dụng

builder.Services.AddScoped<HRRecruitmentCandidateLogRepo>();
builder.Services.AddScoped<HRRecruitmentCandidateRepo>();
builder.Services.AddScoped<HRHiringCandidateInformationFormWorkingExperienceRepo>();
builder.Services.AddScoped<HRHiringCandidateInformationEmergencyContactRepo>();
builder.Services.AddScoped<HRHiringCandidateInformationFormOtherCertificateRepo>();
builder.Services.AddScoped<HRHiringCandidateInformationFormEducationRepo>();
builder.Services.AddScoped<HRHiringCandidateInformationFormRepo>();
builder.Services.AddScoped<HRRecruitmentCandidateRepo>();
builder.Services.AddScoped<HRHiringCandidateInformationFormForeignLanguageSkillsRepo>();
builder.Services.AddScoped<HRHiringCandidateInformationFormRecruitmentInfoRepo>();

#endregion Yêu cầu tuyển dụng

#region đề thi ứng tuyển

builder.Services.AddScoped<HRRecruitmentExamRepo>();
builder.Services.AddScoped<HRRecruitmentQuestionRepo>();
builder.Services.AddScoped<HRRecruitmentAnswersRepo>();
builder.Services.AddScoped<HRRecruitmentRightAnswearsRepo>();
builder.Services.AddScoped<HRRecruitmentExamResultRepo>();
builder.Services.AddScoped<HRRecruitmentQuestionImageRepo>();
builder.Services.AddScoped<HRRecruitmentExamResultDetailRepo>();
builder.Services.AddScoped<HRRecruitmentExamResultImageRepo>();
builder.Services.AddScoped<HiringRequestExamRepo>();
builder.Services.AddScoped<HRRecruitmentExamEvaluationFileRepo>();

#endregion đề thi ứng tuyển

#region RabbitService

//builder.Services.AddSingleton<RabbitMqConnection>();
//builder.Services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();
//builder.Services.AddHostedService<EmailConsumer>();

builder.Services.AddScoped<EmailHelper>();

#endregion RabbitService

builder.Services.AddScoped<HistoryBorrowSaleLogRepo>();
builder.Services.AddScoped<CommercialPriceRequestRepo>();
builder.Services.AddScoped<PaymentOrderLogApprovedRepo>();
builder.Services.AddScoped<CurrencyConfigRepo>();
builder.Services.AddScoped<BillImportSaleLogRepo>();
builder.Services.AddScoped<BankListRepo>();
builder.Services.AddScoped<ProjectPartlistPurchaseRequestNoteRepo>();
builder.Services.AddScoped<PollFormRepo>();
builder.Services.AddScoped<PollSectionRepo>();
builder.Services.AddScoped<PollQuestionRepo>();
builder.Services.AddScoped<PollQuestionOptionRepo>();
builder.Services.AddScoped<PollResponseRepo>();
builder.Services.AddScoped<PollResponseAnswerRepo>();
builder.Services.AddScoped<PollBranchingRuleEvaluator>();
builder.Services.AddScoped<MakerTrainingRepo>();
builder.Services.AddScoped<MakerTrainingEmployeeLinkRepo>();
builder.Services.AddScoped<MakerTrainingDocumentRepo>();
builder.Services.AddScoped<MakerTrainingTypeRepo>();
builder.Services.AddScoped<PerformanceCriteriaRepo>();
builder.Services.AddScoped<EmployeeAttendanceNewRepo>();
builder.Services.AddScoped<ProjectTaskWorkRepo>();
builder.Services.AddScoped<ProjectTaskStatusRepo>();
builder.Services.AddScoped<PaymentOrderOrderTypeRepo>();
builder.Services.AddScoped<ConfigNotificationKeyRepo>();
builder.Services.AddScoped<ConfigNotificationKeyLinkRepo>();
builder.Services.AddScoped<ProjectGateRepo>();
builder.Services.AddScoped<ProductGroupRTCLinkRepo>();
builder.Services.AddScoped<MakerTrainingDepartmentLinkRepo>();
builder.Services.AddScoped<MakerTrainingVideoLinkRepo>();

builder.Services.AddScoped<ESLTestTableRepo>();
builder.Services.AddScoped<ESLTestTableRegistrationRepo>();
builder.Services.AddScoped<ESLTestTableRegistrationLogRepo>();
builder.Services.AddScoped<ESLTestTableRegistrationDetailRepo>();
builder.Services.AddScoped<ESLConfigRepo>();

builder.Services.AddScoped<VehicleRentalRequestRepo>();
builder.Services.AddScoped<BusinessVisaRequestRepo>();


builder.Services.AddHttpClient<IESLBindService, ESLBindService>();
builder.Services.AddScoped<KPISaleApprovalRepo>();
builder.Services.AddScoped<KPISaleApprovalLogRepo>();
builder.Services.AddScoped<KPISaleTeamMemberRepo>();
builder.Services.AddScoped<KPISalePeroidRepo>();

builder.Services.AddScoped<ProjectGateRepo>();
builder.Services.AddScoped<ProjectGateStepRepo>();
builder.Services.AddScoped<ProjectGateStepTemplateRepo>();
builder.Services.AddScoped<ProjectGateStepLinkRepo>();
builder.Services.AddScoped<ProjectGateStepWorkerRepo>();
builder.Services.AddScoped<ProjectGateDepartmentRepo>();
builder.Services.AddScoped<ProjectGateStepPositionRepo>();
builder.Services.AddScoped<ProjectGateStepCheckListRepo>();
builder.Services.AddScoped<ProjectGateStepCheckListLinkRepo>();
builder.Services.AddScoped<ProjectGateStepCheckListDetailRepo>();
builder.Services.AddScoped<ProjectGateStepFileRepo>();
#region DI LOG

builder.Services.AddScoped<POKHLogRepo>();
builder.Services.AddScoped<RequestInvoiceLogRepo>();
builder.Services.AddScoped<BillExportSaleLogRepo>();
builder.Services.AddScoped<PONCCLogRepo>();
builder.Services.AddScoped<BillImportTechnicalAuditLogRepo>();
builder.Services.AddScoped<AssetLogRepo>();
builder.Services.AddScoped<BillExportTechnicalAuditLogRepo>();
builder.Services.AddScoped<AssetAllocationLogRepo>();
builder.Services.AddScoped<ProjectPartlistPurchaseRequestLogRepo>();
builder.Services.AddScoped<ProjectPartListPriceRequestLogRepo>();
builder.Services.AddScoped<ProjectPartListLogRepo>();
builder.Services.AddScoped<JobPerfomanceEvaluationNewLogRepo>();

#endregion DI LOG

//Config connect databaseCourse
Config.ConnectionStringCourse = builder.Configuration.GetValue<string>("ConnectionStringCourse") ?? "";
builder.Services.AddDbContext<RTCCourseDbContext>(o => o.UseSqlServer(Config.ConnectionStringCourse));

#region DI Khoá học web

builder.Services.AddScoped<RERPAPI.Repo.GenericCourseEntity.CourseCatalogRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericCourseEntity.CourseAnswersRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericCourseEntity.CourseExamEvaluateRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericCourseEntity.CourseExamRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericCourseEntity.CourseExamResultDetailRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericCourseEntity.CourseExamResultRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericCourseEntity.CourseFileRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericCourseEntity.CourseLessonHistoryRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericCourseEntity.CourseLessonRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericCourseEntity.CourseQuestionRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericCourseEntity.CourseRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericCourseEntity.CourseRightAnswersRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericCourseEntity.CourseTypeRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericCourseEntity.ConfigSystemRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericCourseEntity.UserRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericCourseEntity.EmployeeRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericCourseEntity.CourseExamPracticeRepo>();
builder.Services.AddScoped<RERPAPI.Repo.GenericCourseEntity.CourseCatalogTypeRepo>();
builder.Services.AddScoped<HistoryProductPriceRequestRepo>();

#endregion DI Khoá học web

builder.Services.AddScoped<CurrentUser>(provider =>
{
    var context = provider.GetRequiredService<IHttpContextAccessor>().HttpContext;
    var claims = context?.User?.Claims?.ToDictionary(x => x.Type, x => x.Value) ?? new Dictionary<string, string>();
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
               .AllowAnyHeader()
               .WithExposedHeaders(CorsHelper.GetExposedHeaders()); // config cors tus dotnet
        ;
    });
});
// Chỉ khởi tạo 1 lần duy nhất khi chạy server
//FirebaseApp.Create(new AppOptions()
//{
//    Credential = GoogleCredential.FromFile("firebase-adminsdk.json") // Thay bằng đường dẫn thực tế
//});

builder.Services.AddSingleton<SseService>();

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
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);

// Load Candidate JWT settings
var candidateJwtSection = builder.Configuration.GetSection("CandidateJwtSettings");
builder.Services.Configure<CandidateJwtSettings>(candidateJwtSection);
var candidateJwtSettings = candidateJwtSection.Get<CandidateJwtSettings>() ?? new CandidateJwtSettings();
builder.Services.AddSingleton(candidateJwtSettings);

builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuers = new[] { jwtSettings.Issuer, candidateJwtSettings.Issuer },
                        ValidAudiences = new[] { jwtSettings.Audience, candidateJwtSettings.Audience },
                        IssuerSigningKeys = new[]
                        {
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(candidateJwtSettings.SecretKey))
                        },
                        NameClaimType = "sub" // Để Middleware lấy đúng UserID
                    };
                });
builder.Services.AddAuthentication();

//Get SmtpSetting
var smtpSettings = builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.Configure<SmtpSettingsHr>(builder.Configuration.GetSection("SmtpSettingsHr"));
builder.Services.Configure<SmtpSettingsHrm>(builder.Configuration.GetSection("SmtpSettingsHrm"));

//Get list static file
builder.Services.Configure<List<PathStaticFile>>(builder.Configuration.GetSection("PathStaticFiles"));

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

var roleConfigSection = builder.Configuration.GetSection("RoleConfig");

// Bind vào IOptions<RoleConfig>
builder.Services.Configure<RoleConfig>(roleConfigSection);

// Đăng ký singleton để inject trực tiếp RoleConfig
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<RoleConfig>>().Value);
builder.Services.Configure<ModulaConfig>(builder.Configuration.GetSection("ModulaConfig"));

// Nếu bạn muốn inject trực tiếp:
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<ModulaConfig>>().Value);
//builder.Services.AddHostedService<PersistentTcpClientService>();
//builder.Services.AddSingleton(sp =>
//{
//    var config = sp.GetRequiredService<ModulaConfig>();

//    return new PersistentTcpClientService(
//        config.IpAddress,
//        config.Port,
//        connectTimeoutMs: 3000,
//        sendTimeoutMs: 3000,
//        receiveTimeoutMs: 3000,
//        maxReconnectAttempts: 3,
//        reconnectDelayMs: 1000
//    );
//});

//Add logger
// 👉 cấu hình Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .WriteTo.Async(a => a.File(
         @"D:\RERPLogs\api\log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss:fff} [{Level}] {Message}{NewLine}{Exception}"
    ))
    .CreateLogger();

// 👉 replace logger mặc định
builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate =
        "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

    options.GetLevel = (httpContext, elapsed, ex) =>
    {
        if (ex != null) return LogEventLevel.Error;
        if (httpContext.Response.StatusCode >= 500) return LogEventLevel.Error;
        if (elapsed > 3000) return LogEventLevel.Warning;

        return LogEventLevel.Information;
    };
});

app.UseHttpsRedirection();
//Config static file
app.UseStaticFiles();

app.Use(async (context, next) =>
{
    context.Request.Path = context.Request.Path.Value?.ToLower();
    await next();
});

app.UseRouting();
app.UseCors("MyCors");
app.UseAuthentication();
app.UseMiddleware<DynamicAuthorizationMiddleware>();
app.UseAuthorization();
app.UseSession();

app.MapControllers();


app.UseStaticFiles();
List<PathStaticFile> staticFiles = builder.Configuration.GetSection("PathStaticFiles").Get<List<PathStaticFile>>() ?? new List<PathStaticFile>();

foreach (var item in staticFiles)
{
    var pathName = item.PathName.Trim().ToLower();
    var requestPath = pathName == "upload" 
        ? "/api/upload" 
        : $"/api/share/{pathName}";

    app.UseStaticFiles(new StaticFileOptions()
    {
        FileProvider = new PhysicalFileProvider(item.PathFull),
        RequestPath = new PathString(requestPath)
    });

    //app.UseDirectoryBrowser(new DirectoryBrowserOptions
    //{
    //    FileProvider = new PhysicalFileProvider(item.PathFull),
    //    RequestPath = new PathString(requestPath)
    //});
}
var tusStore = new TusDiskStore(Directory.GetCurrentDirectory());
// config Tus dotnet
app.UseTus(httpContext => new DefaultTusConfiguration
{
    Store = tusStore, // đường dẫn lưu temp file ( file chunk)

    UrlPath = "/api/tus/upload-video", // path gọi api
    Expiration = new AbsoluteExpiration(TimeSpan.FromHours(24)), // xóa upload không hoàn thành sau 24h

    Events = new Events
    {
        OnFileCompleteAsync = async ctx =>
        {
            var file = await ctx.GetFileAsync();
            if (file == null) return;

            var metadata = await file.GetMetadataAsync(ctx.CancellationToken);

            var fileName = metadata.ContainsKey("filename")
                ? metadata["filename"].GetString(Encoding.UTF8)
                : $"{file.Id}.bin";

            //var destDir = @"\\192.168.1.190\Software\Test\UPLOADFILE\CourseLesson\Videos\";
            //Directory.CreateDirectory(destDir);
            //var destPath = Path.Combine(destDir, fileName);

            var pathServer = metadata["pathServer"].GetString(Encoding.UTF8);
            var destPath = Path.Combine(pathServer, fileName);
            Directory.CreateDirectory(pathServer);
            await using (var source = await file.GetContentAsync(ctx.CancellationToken))
            {
                await using (var target = System.IO.File.Create(destPath))
                {
                    await source.CopyToAsync(target);
                }
            }
        }
    }
});
//app.Use(async (context, next) =>
//{
//    var sw = System.Diagnostics.Stopwatch.StartNew();

//    try
//    {
//        await next();
//    }
//    finally
//    {
//        sw.Stop();

//        if (sw.ElapsedMilliseconds > 3000)
//        {
//            var logger = context.RequestServices
//                .GetRequiredService<ILoggerFactory>()
//                .CreateLogger("SlowRequest");

//            logger.LogWarning(
//                "Slow request: {Method} {Path}{QueryString} took {Elapsed}ms, StatusCode={StatusCode}",
//                context.Request.Method,
//                context.Request.Path,
//                context.Request.QueryString,
//                sw.ElapsedMilliseconds,
//                context.Response.StatusCode
//            );
//        }
//    }
//});
//app.UseSerilogRequestLogging(); // log request

app.Run();