using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Entities;

namespace RERPAPI.Model.Context;

public partial class RTCContext : DbContext
{
    public RTCContext(DbContextOptions<RTCContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AGVBillDocumentExport> AGVBillDocumentExports { get; set; }

    public virtual DbSet<AGVBillDocumentExportLog> AGVBillDocumentExportLogs { get; set; }

    public virtual DbSet<AGVBillDocumentImport> AGVBillDocumentImports { get; set; }

    public virtual DbSet<AGVBillDocumentImportLog> AGVBillDocumentImportLogs { get; set; }

    public virtual DbSet<AGVBillExport> AGVBillExports { get; set; }

    public virtual DbSet<AGVBillExportDetail> AGVBillExportDetails { get; set; }

    public virtual DbSet<AGVBillExportDetailSerial> AGVBillExportDetailSerials { get; set; }

    public virtual DbSet<AGVBillExportLog> AGVBillExportLogs { get; set; }

    public virtual DbSet<AGVBillImport> AGVBillImports { get; set; }

    public virtual DbSet<AGVBillImportDetail> AGVBillImportDetails { get; set; }

    public virtual DbSet<AGVBillImportDetailSerial> AGVBillImportDetailSerials { get; set; }

    public virtual DbSet<AGVBillImportDetailSerialNumber> AGVBillImportDetailSerialNumbers { get; set; }

    public virtual DbSet<AGVBillImportLog> AGVBillImportLogs { get; set; }

    public virtual DbSet<AGVHistoryProduct> AGVHistoryProducts { get; set; }

    public virtual DbSet<AGVHistoryProductLog> AGVHistoryProductLogs { get; set; }

    public virtual DbSet<AGVInventoryDemo> AGVInventoryDemos { get; set; }

    public virtual DbSet<AGVLocation> AGVLocations { get; set; }

    public virtual DbSet<AGVProduct> AGVProducts { get; set; }

    public virtual DbSet<AGVProductGroup> AGVProductGroups { get; set; }

    public virtual DbSet<AGVProductQRCode> AGVProductQRCodes { get; set; }

    public virtual DbSet<AccountingBill> AccountingBills { get; set; }

    public virtual DbSet<AccountingBillApproved> AccountingBillApproveds { get; set; }

    public virtual DbSet<AccountingContract> AccountingContracts { get; set; }

    public virtual DbSet<AccountingContractFile> AccountingContractFiles { get; set; }

    public virtual DbSet<AccountingContractLog> AccountingContractLogs { get; set; }

    public virtual DbSet<AccountingContractType> AccountingContractTypes { get; set; }

    public virtual DbSet<ActionHistory> ActionHistories { get; set; }

    public virtual DbSet<AddressStock> AddressStocks { get; set; }

    public virtual DbSet<AdminMarketing> AdminMarketings { get; set; }

    public virtual DbSet<AdminMarketingDetail> AdminMarketingDetails { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<BillDocumentExport> BillDocumentExports { get; set; }

    public virtual DbSet<BillDocumentExportLog> BillDocumentExportLogs { get; set; }

    public virtual DbSet<BillDocumentExportTechnical> BillDocumentExportTechnicals { get; set; }

    public virtual DbSet<BillDocumentExportTechnicalLog> BillDocumentExportTechnicalLogs { get; set; }

    public virtual DbSet<BillDocumentImport> BillDocumentImports { get; set; }

    public virtual DbSet<BillDocumentImportLog> BillDocumentImportLogs { get; set; }

    public virtual DbSet<BillDocumentImportTechnical> BillDocumentImportTechnicals { get; set; }

    public virtual DbSet<BillDocumentImportTechnicalLog> BillDocumentImportTechnicalLogs { get; set; }

    public virtual DbSet<BillExport> BillExports { get; set; }

    public virtual DbSet<BillExportAcountant> BillExportAcountants { get; set; }

    public virtual DbSet<BillExportAcountantDetail> BillExportAcountantDetails { get; set; }

    public virtual DbSet<BillExportDetail> BillExportDetails { get; set; }

    public virtual DbSet<BillExportDetailSerialNumber> BillExportDetailSerialNumbers { get; set; }

    public virtual DbSet<BillExportDetailSerialNumberModulaLocation> BillExportDetailSerialNumberModulaLocations { get; set; }

    public virtual DbSet<BillExportDetailTechnical> BillExportDetailTechnicals { get; set; }

    public virtual DbSet<BillExportLog> BillExportLogs { get; set; }

    public virtual DbSet<BillExportTechDetailSerial> BillExportTechDetailSerials { get; set; }

    public virtual DbSet<BillExportTechnical> BillExportTechnicals { get; set; }

    public virtual DbSet<BillExportTechnicalLog> BillExportTechnicalLogs { get; set; }

    public virtual DbSet<BillFilm> BillFilms { get; set; }

    public virtual DbSet<BillFilmDetail> BillFilmDetails { get; set; }

    public virtual DbSet<BillImport> BillImports { get; set; }

    public virtual DbSet<BillImportDetail> BillImportDetails { get; set; }

    public virtual DbSet<BillImportDetailSerialNumber> BillImportDetailSerialNumbers { get; set; }

    public virtual DbSet<BillImportDetailSerialNumberModulaLocation> BillImportDetailSerialNumberModulaLocations { get; set; }

    public virtual DbSet<BillImportDetailTechnical> BillImportDetailTechnicals { get; set; }

    public virtual DbSet<BillImportLog> BillImportLogs { get; set; }

    public virtual DbSet<BillImportQC> BillImportQCs { get; set; }

    public virtual DbSet<BillImportQCDetail> BillImportQCDetails { get; set; }

    public virtual DbSet<BillImportTechDetailSerial> BillImportTechDetailSerials { get; set; }

    public virtual DbSet<BillImportTechnical> BillImportTechnicals { get; set; }

    public virtual DbSet<BillImportTechnicalLog> BillImportTechnicalLogs { get; set; }

    public virtual DbSet<BillSale> BillSales { get; set; }

    public virtual DbSet<BonusRule> BonusRules { get; set; }

    public virtual DbSet<BonusRuleIndex> BonusRuleIndices { get; set; }

    public virtual DbSet<BookingRoom> BookingRooms { get; set; }

    public virtual DbSet<BookingRoomLog> BookingRoomLogs { get; set; }

    public virtual DbSet<BusinessField> BusinessFields { get; set; }

    public virtual DbSet<BusinessFieldLink> BusinessFieldLinks { get; set; }

    public virtual DbSet<ChangeLogStore> ChangeLogStores { get; set; }

    public virtual DbSet<ConfigPrice> ConfigPrices { get; set; }

    public virtual DbSet<ConfigSystem> ConfigSystems { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseAnswer> CourseAnswers { get; set; }

    public virtual DbSet<CourseCatalog> CourseCatalogs { get; set; }

    public virtual DbSet<CourseCatalogProjectType> CourseCatalogProjectTypes { get; set; }

    public virtual DbSet<CourseExam> CourseExams { get; set; }

    public virtual DbSet<CourseExamEvaluate> CourseExamEvaluates { get; set; }

    public virtual DbSet<CourseExamPractice> CourseExamPractices { get; set; }

    public virtual DbSet<CourseExamResult> CourseExamResults { get; set; }

    public virtual DbSet<CourseExamResultDetail> CourseExamResultDetails { get; set; }

    public virtual DbSet<CourseFile> CourseFiles { get; set; }

    public virtual DbSet<CourseLesson> CourseLessons { get; set; }

    public virtual DbSet<CourseLessonHistory> CourseLessonHistories { get; set; }

    public virtual DbSet<CourseLessonLog> CourseLessonLogs { get; set; }

    public virtual DbSet<CourseQuestion> CourseQuestions { get; set; }

    public virtual DbSet<CourseRightAnswer> CourseRightAnswers { get; set; }

    public virtual DbSet<CourseType> CourseTypes { get; set; }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerBase> CustomerBases { get; set; }

    public virtual DbSet<CustomerBaseContact> CustomerBaseContacts { get; set; }

    public virtual DbSet<CustomerContact> CustomerContacts { get; set; }

    public virtual DbSet<CustomerEmployee> CustomerEmployees { get; set; }

    public virtual DbSet<CustomerPart> CustomerParts { get; set; }

    public virtual DbSet<CustomerSpecialization> CustomerSpecializations { get; set; }

    public virtual DbSet<DailyReportHR> DailyReportHRs { get; set; }

    public virtual DbSet<DailyReportSale> DailyReportSales { get; set; }

    public virtual DbSet<DailyReportSaleAdmin> DailyReportSaleAdmins { get; set; }

    public virtual DbSet<DailyReportTechnical> DailyReportTechnicals { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<DocumentExport> DocumentExports { get; set; }

    public virtual DbSet<DocumentFile> DocumentFiles { get; set; }

    public virtual DbSet<DocumentImport> DocumentImports { get; set; }

    public virtual DbSet<DocumentImportPONCC> DocumentImportPONCCs { get; set; }

    public virtual DbSet<DocumentSale> DocumentSales { get; set; }

    public virtual DbSet<DocumentType> DocumentTypes { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeApprove> EmployeeApproves { get; set; }

    public virtual DbSet<EmployeeAttendance> EmployeeAttendances { get; set; }

    public virtual DbSet<EmployeeBussiness> EmployeeBussinesses { get; set; }

    public virtual DbSet<EmployeeBussinessVehicle> EmployeeBussinessVehicles { get; set; }

    public virtual DbSet<EmployeeChamCongDetail> EmployeeChamCongDetails { get; set; }

    public virtual DbSet<EmployeeChamCongMaster> EmployeeChamCongMasters { get; set; }

    public virtual DbSet<EmployeeChucVu> EmployeeChucVus { get; set; }

    public virtual DbSet<EmployeeChucVuHD> EmployeeChucVuHDs { get; set; }

    public virtual DbSet<EmployeeCollectMoney> EmployeeCollectMoneys { get; set; }

    public virtual DbSet<EmployeeCompensatoryLeave> EmployeeCompensatoryLeaves { get; set; }

    public virtual DbSet<EmployeeContract> EmployeeContracts { get; set; }

    public virtual DbSet<EmployeeCurricular> EmployeeCurriculars { get; set; }

    public virtual DbSet<EmployeeEarlyLate> EmployeeEarlyLates { get; set; }

    public virtual DbSet<EmployeeEducationLevel> EmployeeEducationLevels { get; set; }

    public virtual DbSet<EmployeeError> EmployeeErrors { get; set; }

    public virtual DbSet<EmployeeFamily> EmployeeFamilies { get; set; }

    public virtual DbSet<EmployeeFingerprint> EmployeeFingerprints { get; set; }

    public virtual DbSet<EmployeeFingerprintMaster> EmployeeFingerprintMasters { get; set; }

    public virtual DbSet<EmployeeFoodOrder> EmployeeFoodOrders { get; set; }

    public virtual DbSet<EmployeeLoaiHDLD> EmployeeLoaiHDLDs { get; set; }

    public virtual DbSet<EmployeeLuckyNumber> EmployeeLuckyNumbers { get; set; }

    public virtual DbSet<EmployeeNighShift> EmployeeNighShifts { get; set; }

    public virtual DbSet<EmployeeNoFingerprint> EmployeeNoFingerprints { get; set; }

    public virtual DbSet<EmployeeOnLeave> EmployeeOnLeaves { get; set; }

    public virtual DbSet<EmployeeOnLeaveMaster> EmployeeOnLeaveMasters { get; set; }

    public virtual DbSet<EmployeeOvertime> EmployeeOvertimes { get; set; }

    public virtual DbSet<EmployeeOvertimeProjectItem> EmployeeOvertimeProjectItems { get; set; }

    public virtual DbSet<EmployeePOContact> EmployeePOContacts { get; set; }

    public virtual DbSet<EmployeePayroll> EmployeePayrolls { get; set; }

    public virtual DbSet<EmployeePayrollBonusDeuction> EmployeePayrollBonusDeuctions { get; set; }

    public virtual DbSet<EmployeePayrollDetail> EmployeePayrollDetails { get; set; }

    public virtual DbSet<EmployeeProjectType> EmployeeProjectTypes { get; set; }

    public virtual DbSet<EmployeePurchase> EmployeePurchases { get; set; }

    public virtual DbSet<EmployeeRegisterWork> EmployeeRegisterWorks { get; set; }

    public virtual DbSet<EmployeeSalaryAdvance> EmployeeSalaryAdvances { get; set; }

    public virtual DbSet<EmployeeScheduleWork> EmployeeScheduleWorks { get; set; }

    public virtual DbSet<EmployeeSendEmail> EmployeeSendEmails { get; set; }

    public virtual DbSet<EmployeeSettingMoney> EmployeeSettingMoneys { get; set; }

    public virtual DbSet<EmployeeStatus> EmployeeStatuses { get; set; }

    public virtual DbSet<EmployeeTeam> EmployeeTeams { get; set; }

    public virtual DbSet<EmployeeTeamSale> EmployeeTeamSales { get; set; }

    public virtual DbSet<EmployeeTeamSaleLink> EmployeeTeamSaleLinks { get; set; }

    public virtual DbSet<EmployeeTinhTrangHonNhan> EmployeeTinhTrangHonNhans { get; set; }

    public virtual DbSet<EmployeeTypeBussiness> EmployeeTypeBussinesses { get; set; }

    public virtual DbSet<EmployeeTypeOvertime> EmployeeTypeOvertimes { get; set; }

    public virtual DbSet<EmployeeVehicleBussiness> EmployeeVehicleBussinesses { get; set; }

    public virtual DbSet<EmployeeWFH> EmployeeWFHs { get; set; }

    public virtual DbSet<EmployeeWorkingProcess> EmployeeWorkingProcesses { get; set; }

    public virtual DbSet<ExamCategory> ExamCategories { get; set; }

    public virtual DbSet<ExamListTest> ExamListTests { get; set; }

    public virtual DbSet<ExamQuestion> ExamQuestions { get; set; }

    public virtual DbSet<ExamQuestionBank> ExamQuestionBanks { get; set; }

    public virtual DbSet<ExamQuestionGroup> ExamQuestionGroups { get; set; }

    public virtual DbSet<ExamQuestionListTest> ExamQuestionListTests { get; set; }

    public virtual DbSet<ExamQuestionType> ExamQuestionTypes { get; set; }

    public virtual DbSet<ExamResult> ExamResults { get; set; }

    public virtual DbSet<ExamResultAnswerDetail> ExamResultAnswerDetails { get; set; }

    public virtual DbSet<ExamResultDetail> ExamResultDetails { get; set; }

    public virtual DbSet<ExamTestResult> ExamTestResults { get; set; }

    public virtual DbSet<ExamTestResultMaster> ExamTestResultMasters { get; set; }

    public virtual DbSet<ExamTypeTest> ExamTypeTests { get; set; }

    public virtual DbSet<FcmToken> FcmTokens { get; set; }

    public virtual DbSet<FilmManagement> FilmManagements { get; set; }

    public virtual DbSet<FilmManagementDetail> FilmManagementDetails { get; set; }

    public virtual DbSet<Fingerprint> Fingerprints { get; set; }

    public virtual DbSet<Firm> Firms { get; set; }

    public virtual DbSet<FirmBase> FirmBases { get; set; }

    public virtual DbSet<FollowProject> FollowProjects { get; set; }

    public virtual DbSet<FollowProjectBase> FollowProjectBases { get; set; }

    public virtual DbSet<FollowProjectBaseDetail> FollowProjectBaseDetails { get; set; }

    public virtual DbSet<FollowProjectDetail> FollowProjectDetails { get; set; }

    public virtual DbSet<FormAndFunction> FormAndFunctions { get; set; }

    public virtual DbSet<FormAndFunctionGroup> FormAndFunctionGroups { get; set; }

    public virtual DbSet<Goal> Goals { get; set; }

    public virtual DbSet<GroupFile> GroupFiles { get; set; }

    public virtual DbSet<GroupProductSale> GroupProductSales { get; set; }

    public virtual DbSet<GroupSale> GroupSales { get; set; }

    public virtual DbSet<GroupSalesUser> GroupSalesUsers { get; set; }

    public virtual DbSet<HandoverMinute> HandoverMinutes { get; set; }

    public virtual DbSet<HandoverMinutesDetail> HandoverMinutesDetails { get; set; }

    public virtual DbSet<HistoryDeleteBill> HistoryDeleteBills { get; set; }

    public virtual DbSet<HistoryError> HistoryErrors { get; set; }

    public virtual DbSet<HistoryKPISale> HistoryKPISales { get; set; }

    public virtual DbSet<HistoryMoneyPO> HistoryMoneyPOs { get; set; }

    public virtual DbSet<HistoryProductRTC> HistoryProductRTCs { get; set; }

    public virtual DbSet<HistoryProductRTCLog> HistoryProductRTCLogs { get; set; }

    public virtual DbSet<Holiday> Holidays { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<InventoryDemo> InventoryDemos { get; set; }

    public virtual DbSet<InventoryProject> InventoryProjects { get; set; }

    public virtual DbSet<InventoryProjectExport> InventoryProjectExports { get; set; }

    public virtual DbSet<InventoryStock> InventoryStocks { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceLink> InvoiceLinks { get; set; }

    public virtual DbSet<JobRequirement> JobRequirements { get; set; }

    public virtual DbSet<JobRequirementApproved> JobRequirementApproveds { get; set; }

    public virtual DbSet<JobRequirementComment> JobRequirementComments { get; set; }

    public virtual DbSet<JobRequirementDetail> JobRequirementDetails { get; set; }

    public virtual DbSet<JobRequirementFile> JobRequirementFiles { get; set; }

    public virtual DbSet<JobRequirementLog> JobRequirementLogs { get; set; }

    public virtual DbSet<KPICriteriaDetail> KPICriteriaDetails { get; set; }

    public virtual DbSet<KPICriterion> KPICriteria { get; set; }

    public virtual DbSet<KPIDetail> KPIDetails { get; set; }

    public virtual DbSet<KPIDetailUser> KPIDetailUsers { get; set; }

    public virtual DbSet<KPIEmployeePoint> KPIEmployeePoints { get; set; }

    public virtual DbSet<KPIEmployeePointDetail> KPIEmployeePointDetails { get; set; }

    public virtual DbSet<KPIEmployeeTeam> KPIEmployeeTeams { get; set; }

    public virtual DbSet<KPIEmployeeTeamLink> KPIEmployeeTeamLinks { get; set; }

    public virtual DbSet<KPIError> KPIErrors { get; set; }

    public virtual DbSet<KPIErrorEmployee> KPIErrorEmployees { get; set; }

    public virtual DbSet<KPIErrorEmployeeFile> KPIErrorEmployeeFiles { get; set; }

    public virtual DbSet<KPIErrorFineAmount> KPIErrorFineAmounts { get; set; }

    public virtual DbSet<KPIErrorType> KPIErrorTypes { get; set; }

    public virtual DbSet<KPIEvaluation> KPIEvaluations { get; set; }

    public virtual DbSet<KPIEvaluationError> KPIEvaluationErrors { get; set; }

    public virtual DbSet<KPIEvaluationFactor> KPIEvaluationFactors { get; set; }

    public virtual DbSet<KPIEvaluationPoint> KPIEvaluationPoints { get; set; }

    public virtual DbSet<KPIEvaluationRule> KPIEvaluationRules { get; set; }

    public virtual DbSet<KPIEvaluationRuleDetail> KPIEvaluationRuleDetails { get; set; }

    public virtual DbSet<KPIExam> KPIExams { get; set; }

    public virtual DbSet<KPIExamPosition> KPIExamPositions { get; set; }

    public virtual DbSet<KPIPosition> KPIPositions { get; set; }

    public virtual DbSet<KPIPositionEmployee> KPIPositionEmployees { get; set; }

    public virtual DbSet<KPISession> KPISessions { get; set; }

    public virtual DbSet<KPISpecializationType> KPISpecializationTypes { get; set; }

    public virtual DbSet<KPISumarize> KPISumarizes { get; set; }

    public virtual DbSet<KPISumaryEvaluation> KPISumaryEvaluations { get; set; }

    public virtual DbSet<ListCost> ListCosts { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<MainIndex> MainIndices { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<MeetingMinute> MeetingMinutes { get; set; }

    public virtual DbSet<MeetingMinutesAttendance> MeetingMinutesAttendances { get; set; }

    public virtual DbSet<MeetingMinutesDetail> MeetingMinutesDetails { get; set; }

    public virtual DbSet<MeetingMinutesFile> MeetingMinutesFiles { get; set; }

    public virtual DbSet<MeetingType> MeetingTypes { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<MenuEmployeeLink> MenuEmployeeLinks { get; set; }

    public virtual DbSet<ModulaLocation> ModulaLocations { get; set; }

    public virtual DbSet<ModulaLocationDetail> ModulaLocationDetails { get; set; }

    public virtual DbSet<Notify> Notifies { get; set; }

    public virtual DbSet<Number> Numbers { get; set; }

    public virtual DbSet<OfficeSupply> OfficeSupplies { get; set; }

    public virtual DbSet<OfficeSupplyRequest> OfficeSupplyRequests { get; set; }

    public virtual DbSet<OfficeSupplyRequest1> OfficeSupplyRequests1 { get; set; }

    public virtual DbSet<OfficeSupplyRequestsDetail> OfficeSupplyRequestsDetails { get; set; }

    public virtual DbSet<OfficeSupplyUnit> OfficeSupplyUnits { get; set; }

    public virtual DbSet<OrganizationalChart> OrganizationalCharts { get; set; }

    public virtual DbSet<OrganizationalChartDetail> OrganizationalChartDetails { get; set; }

    public virtual DbSet<POKH> POKHs { get; set; }

    public virtual DbSet<POKHDetail> POKHDetails { get; set; }

    public virtual DbSet<POKHDetailMoney> POKHDetailMoneys { get; set; }

    public virtual DbSet<POKHFile> POKHFiles { get; set; }

    public virtual DbSet<POKHHistory> POKHHistories { get; set; }

    public virtual DbSet<PONCC> PONCCs { get; set; }

    public virtual DbSet<PONCCDetail> PONCCDetails { get; set; }

    public virtual DbSet<PONCCDetailLog> PONCCDetailLogs { get; set; }

    public virtual DbSet<PONCCDetailRequestBuy> PONCCDetailRequestBuys { get; set; }

    public virtual DbSet<PONCCHistory> PONCCHistories { get; set; }

    public virtual DbSet<PONCCRulePay> PONCCRulePays { get; set; }

    public virtual DbSet<Part> Parts { get; set; }

    public virtual DbSet<PartGroup> PartGroups { get; set; }

    public virtual DbSet<PartSummaryDetail> PartSummaryDetails { get; set; }

    public virtual DbSet<PaymentOrder> PaymentOrders { get; set; }

    public virtual DbSet<PaymentOrderCustomer> PaymentOrderCustomers { get; set; }

    public virtual DbSet<PaymentOrderDetail> PaymentOrderDetails { get; set; }

    public virtual DbSet<PaymentOrderDetailUserTeamSale> PaymentOrderDetailUserTeamSales { get; set; }

    public virtual DbSet<PaymentOrderFile> PaymentOrderFiles { get; set; }

    public virtual DbSet<PaymentOrderFileBankSlip> PaymentOrderFileBankSlips { get; set; }

    public virtual DbSet<PaymentOrderLog> PaymentOrderLogs { get; set; }

    public virtual DbSet<PaymentOrderOrderType> PaymentOrderOrderTypes { get; set; }

    public virtual DbSet<PaymentOrderType> PaymentOrderTypes { get; set; }

    public virtual DbSet<PaymentOrderTypeDocument> PaymentOrderTypeDocuments { get; set; }

    public virtual DbSet<PaymentOrderTypeOrder> PaymentOrderTypeOrders { get; set; }

    public virtual DbSet<PercentMainIndexUser> PercentMainIndexUsers { get; set; }

    public virtual DbSet<PriceCheck> PriceChecks { get; set; }

    public virtual DbSet<ProductFilm> ProductFilms { get; set; }

    public virtual DbSet<ProductGroup> ProductGroups { get; set; }

    public virtual DbSet<ProductGroupRTC> ProductGroupRTCs { get; set; }

    public virtual DbSet<ProductGroupWarehouse> ProductGroupWarehouses { get; set; }

    public virtual DbSet<ProductKhachHang> ProductKhachHangs { get; set; }

    public virtual DbSet<ProductLocation> ProductLocations { get; set; }

    public virtual DbSet<ProductRTC> ProductRTCs { get; set; }

    public virtual DbSet<ProductRTCQRCode> ProductRTCQRCodes { get; set; }

    public virtual DbSet<ProductSale> ProductSales { get; set; }

    public virtual DbSet<ProductWorking> ProductWorkings { get; set; }

    public virtual DbSet<ProductWorkingAudit> ProductWorkingAudits { get; set; }

    public virtual DbSet<ProductivityIndex> ProductivityIndices { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectCost> ProjectCosts { get; set; }

    public virtual DbSet<ProjectCurrentSituation> ProjectCurrentSituations { get; set; }

    public virtual DbSet<ProjectDetail> ProjectDetails { get; set; }

    public virtual DbSet<ProjectEmployee> ProjectEmployees { get; set; }

    public virtual DbSet<ProjectFile> ProjectFiles { get; set; }

    public virtual DbSet<ProjectHistoryProblem> ProjectHistoryProblems { get; set; }

    public virtual DbSet<ProjectHistoryProblemDetail> ProjectHistoryProblemDetails { get; set; }

    public virtual DbSet<ProjectItem> ProjectItems { get; set; }

    public virtual DbSet<ProjectItemDetail> ProjectItemDetails { get; set; }

    public virtual DbSet<ProjectItemFile> ProjectItemFiles { get; set; }

    public virtual DbSet<ProjectItemLog> ProjectItemLogs { get; set; }

    public virtual DbSet<ProjectItemProblem> ProjectItemProblems { get; set; }

    public virtual DbSet<ProjectMachinePrice> ProjectMachinePrices { get; set; }

    public virtual DbSet<ProjectMachinePriceDetail> ProjectMachinePriceDetails { get; set; }

    public virtual DbSet<ProjectPartList> ProjectPartLists { get; set; }

    public virtual DbSet<ProjectPartListType> ProjectPartListTypes { get; set; }

    public virtual DbSet<ProjectPartListVersion> ProjectPartListVersions { get; set; }

    public virtual DbSet<ProjectPartlistPriceRequest> ProjectPartlistPriceRequests { get; set; }

    public virtual DbSet<ProjectPartlistPriceRequestHistory> ProjectPartlistPriceRequestHistories { get; set; }

    public virtual DbSet<ProjectPartlistPurchaseRequest> ProjectPartlistPurchaseRequests { get; set; }

    public virtual DbSet<ProjectPartlistPurchaseRequestNote> ProjectPartlistPurchaseRequestNotes { get; set; }

    public virtual DbSet<ProjectPersonalPriotity> ProjectPersonalPriotities { get; set; }

    public virtual DbSet<ProjectPriority> ProjectPriorities { get; set; }

    public virtual DbSet<ProjectPriorityLink> ProjectPriorityLinks { get; set; }

    public virtual DbSet<ProjectRequest> ProjectRequests { get; set; }

    public virtual DbSet<ProjectRequestFile> ProjectRequestFiles { get; set; }

    public virtual DbSet<ProjectSolution> ProjectSolutions { get; set; }

    public virtual DbSet<ProjectSolutionFile> ProjectSolutionFiles { get; set; }

    public virtual DbSet<ProjectStatus> ProjectStatuses { get; set; }

    public virtual DbSet<ProjectStatusBase> ProjectStatusBases { get; set; }

    public virtual DbSet<ProjectStatusDetail> ProjectStatusDetails { get; set; }

    public virtual DbSet<ProjectStatusLog> ProjectStatusLogs { get; set; }

    public virtual DbSet<ProjectSurvey> ProjectSurveys { get; set; }

    public virtual DbSet<ProjectSurveyDetail> ProjectSurveyDetails { get; set; }

    public virtual DbSet<ProjectSurveyFile> ProjectSurveyFiles { get; set; }

    public virtual DbSet<ProjectTreeFolder> ProjectTreeFolders { get; set; }

    public virtual DbSet<ProjectType> ProjectTypes { get; set; }

    public virtual DbSet<ProjectTypeAssign> ProjectTypeAssigns { get; set; }

    public virtual DbSet<ProjectTypeBase> ProjectTypeBases { get; set; }

    public virtual DbSet<ProjectTypeDetail> ProjectTypeDetails { get; set; }

    public virtual DbSet<ProjectTypeLink> ProjectTypeLinks { get; set; }

    public virtual DbSet<ProjectUser> ProjectUsers { get; set; }

    public virtual DbSet<ProjectWorker> ProjectWorkers { get; set; }

    public virtual DbSet<ProjectWorkerType> ProjectWorkerTypes { get; set; }

    public virtual DbSet<ProjectWorkerVersion> ProjectWorkerVersions { get; set; }

    public virtual DbSet<Province> Provinces { get; set; }

    public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }

    public virtual DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

    public virtual DbSet<Quotation> Quotations { get; set; }

    public virtual DbSet<QuotationDetail> QuotationDetails { get; set; }

    public virtual DbSet<QuotationKH> QuotationKHs { get; set; }

    public virtual DbSet<QuotationKHDetail> QuotationKHDetails { get; set; }

    public virtual DbSet<QuotationNCC> QuotationNCCs { get; set; }

    public virtual DbSet<QuotationNCCDetail> QuotationNCCDetails { get; set; }

    public virtual DbSet<QuotationTermLink> QuotationTermLinks { get; set; }

    public virtual DbSet<RegisterContract> RegisterContracts { get; set; }

    public virtual DbSet<RegisterIdea> RegisterIdeas { get; set; }

    public virtual DbSet<RegisterIdeaDetail> RegisterIdeaDetails { get; set; }

    public virtual DbSet<RegisterIdeaFile> RegisterIdeaFiles { get; set; }

    public virtual DbSet<RegisterIdeaScore> RegisterIdeaScores { get; set; }

    public virtual DbSet<RegisterIdeaType> RegisterIdeaTypes { get; set; }

    public virtual DbSet<RegisterOT> RegisterOTs { get; set; }

    public virtual DbSet<ReportIndex> ReportIndices { get; set; }

    public virtual DbSet<ReportPurchase> ReportPurchases { get; set; }

    public virtual DbSet<ReportType> ReportTypes { get; set; }

    public virtual DbSet<RequestBuy> RequestBuys { get; set; }

    public virtual DbSet<RequestBuyDetail> RequestBuyDetails { get; set; }

    public virtual DbSet<RequestBuyRTC> RequestBuyRTCs { get; set; }

    public virtual DbSet<RequestBuyRTCTTDH> RequestBuyRTCTTDHs { get; set; }

    public virtual DbSet<RequestBuySale> RequestBuySales { get; set; }

    public virtual DbSet<RequestBuySaleDetail> RequestBuySaleDetails { get; set; }

    public virtual DbSet<RequestExport> RequestExports { get; set; }

    public virtual DbSet<RequestExportDetail> RequestExportDetails { get; set; }

    public virtual DbSet<RequestImport> RequestImports { get; set; }

    public virtual DbSet<RequestImportDetail> RequestImportDetails { get; set; }

    public virtual DbSet<RequestInvoice> RequestInvoices { get; set; }

    public virtual DbSet<RequestInvoiceDetail> RequestInvoiceDetails { get; set; }

    public virtual DbSet<RequestInvoiceFile> RequestInvoiceFiles { get; set; }

    public virtual DbSet<RequestPaidPO> RequestPaidPOs { get; set; }

    public virtual DbSet<RequestPrice> RequestPrices { get; set; }

    public virtual DbSet<RequestPriceDetail> RequestPriceDetails { get; set; }

    public virtual DbSet<RequestPriceNotify> RequestPriceNotifies { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RulePay> RulePays { get; set; }

    public virtual DbSet<SALE> SALEs { get; set; }

    public virtual DbSet<SaleUserType> SaleUserTypes { get; set; }

    public virtual DbSet<SalesPerformanceRanking> SalesPerformanceRankings { get; set; }

    public virtual DbSet<SealRegulation> SealRegulations { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<StockLocation> StockLocations { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<SupplierContact> SupplierContacts { get; set; }

    public virtual DbSet<SupplierSale> SupplierSales { get; set; }

    public virtual DbSet<SupplierSaleContact> SupplierSaleContacts { get; set; }

    public virtual DbSet<TSAllocationEvictionAsset> TSAllocationEvictionAssets { get; set; }

    public virtual DbSet<TSAsset> TSAssets { get; set; }

    public virtual DbSet<TSAssetAllocation> TSAssetAllocations { get; set; }

    public virtual DbSet<TSAssetAllocationDetail> TSAssetAllocationDetails { get; set; }

    public virtual DbSet<TSAssetManagement> TSAssetManagements { get; set; }

    public virtual DbSet<TSAssetRecovery> TSAssetRecoveries { get; set; }

    public virtual DbSet<TSAssetRecoveryDetail> TSAssetRecoveryDetails { get; set; }

    public virtual DbSet<TSCalendarPeriodAsset> TSCalendarPeriodAssets { get; set; }

    public virtual DbSet<TSLiQuidationAsset> TSLiQuidationAssets { get; set; }

    public virtual DbSet<TSLostReportAsset> TSLostReportAssets { get; set; }

    public virtual DbSet<TSPeriodAsset> TSPeriodAssets { get; set; }

    public virtual DbSet<TSRepairAsset> TSRepairAssets { get; set; }

    public virtual DbSet<TSReportBrokenAsset> TSReportBrokenAssets { get; set; }

    public virtual DbSet<TSSourceAsset> TSSourceAssets { get; set; }

    public virtual DbSet<TSStatusAsset> TSStatusAssets { get; set; }

    public virtual DbSet<TSTranferAsset> TSTranferAssets { get; set; }

    public virtual DbSet<TSTranferAssetDetail> TSTranferAssetDetails { get; set; }

    public virtual DbSet<TaxCompany> TaxCompanies { get; set; }

    public virtual DbSet<TaxDepartment> TaxDepartments { get; set; }

    public virtual DbSet<TaxEmployee> TaxEmployees { get; set; }

    public virtual DbSet<TaxEmployeeContract> TaxEmployeeContracts { get; set; }

    public virtual DbSet<TaxEmployeePosition> TaxEmployeePositions { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<TermCondition> TermConditions { get; set; }

    public virtual DbSet<Test> Tests { get; set; }

    public virtual DbSet<TrackingMark> TrackingMarks { get; set; }

    public virtual DbSet<TrackingMarksFile> TrackingMarksFiles { get; set; }

    public virtual DbSet<TrackingMarksLog> TrackingMarksLogs { get; set; }

    public virtual DbSet<TrackingMarksSeal> TrackingMarksSeals { get; set; }

    public virtual DbSet<TrackingMarksTaxCompany> TrackingMarksTaxCompanies { get; set; }

    public virtual DbSet<TradePrice> TradePrices { get; set; }

    public virtual DbSet<TradePriceDetail> TradePriceDetails { get; set; }

    public virtual DbSet<TrainingRegistration> TrainingRegistrations { get; set; }

    public virtual DbSet<TrainingRegistrationApproved> TrainingRegistrationApproveds { get; set; }

    public virtual DbSet<TrainingRegistrationApprovedFlow> TrainingRegistrationApprovedFlows { get; set; }

    public virtual DbSet<TrainingRegistrationCategory> TrainingRegistrationCategories { get; set; }

    public virtual DbSet<TrainingRegistrationDetail> TrainingRegistrationDetails { get; set; }

    public virtual DbSet<TrainingRegistrationFile> TrainingRegistrationFiles { get; set; }

    public virtual DbSet<UnitCount> UnitCounts { get; set; }

    public virtual DbSet<UnitCountKT> UnitCountKTs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserGroup> UserGroups { get; set; }

    public virtual DbSet<UserGroupLink> UserGroupLinks { get; set; }

    public virtual DbSet<UserGroupRightDistribution> UserGroupRightDistributions { get; set; }

    public virtual DbSet<UserRightDistribution> UserRightDistributions { get; set; }

    public virtual DbSet<UserTeam> UserTeams { get; set; }

    public virtual DbSet<UserTeamLink> UserTeamLinks { get; set; }

    public virtual DbSet<UserTeamSale> UserTeamSales { get; set; }

    public virtual DbSet<VehicleBookingFile> VehicleBookingFiles { get; set; }

    public virtual DbSet<VehicleBookingManagement> VehicleBookingManagements { get; set; }

    public virtual DbSet<VehicleCategory> VehicleCategories { get; set; }

    public virtual DbSet<VehicleManagement> VehicleManagements { get; set; }

    public virtual DbSet<ViewDetailKPIPurchase> ViewDetailKPIPurchases { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    public virtual DbSet<WeekPlan> WeekPlans { get; set; }

    public virtual DbSet<WorkPlan> WorkPlans { get; set; }

    public virtual DbSet<WorkPlanDetail> WorkPlanDetails { get; set; }

    public virtual DbSet<vUser> vUsers { get; set; }

    public virtual DbSet<vUserGroupLink> vUserGroupLinks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AGVBillDocumentExport>(entity =>
        {
            entity.ToTable("AGVBillDocumentExport", "agv");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.LogDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<AGVBillDocumentExportLog>(entity =>
        {
            entity.ToTable("AGVBillDocumentExportLog", "agv");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.LogDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<AGVBillDocumentImport>(entity =>
        {
            entity.ToTable("AGVBillDocumentImport", "agv");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateReceive).HasColumnType("datetime");
            entity.Property(e => e.LogDate).HasColumnType("datetime");
            entity.Property(e => e.ReasonCancel).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<AGVBillDocumentImportLog>(entity =>
        {
            entity.ToTable("AGVBillDocumentImportLog", "agv");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.LogDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<AGVBillExport>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__AGVBillExpo__3214EC2747739EC3");

            entity.ToTable("AGVBillExport", "agv");

            entity.Property(e => e.Addres).HasMaxLength(100);
            entity.Property(e => e.BillType).HasComment("0. Trả ,1. Cho mượn,2. Tặng / Bán,3. Mất,4. Bảo hành,5. Xuất dự án,6. Hỏng,7. Xuất kho");
            entity.Property(e => e.Code)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.Deliver).HasMaxLength(100);
            entity.Property(e => e.ExpectedDate).HasColumnType("datetime");
            entity.Property(e => e.Image).HasMaxLength(100);
            entity.Property(e => e.Note).HasMaxLength(250);
            entity.Property(e => e.ProjectName).HasMaxLength(550);
            entity.Property(e => e.Receiver).HasMaxLength(100);
            entity.Property(e => e.SupplierName).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.WarehouseType).HasMaxLength(100);
        });

        modelBuilder.Entity<AGVBillExportDetail>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__AGVBillExpo__3214EC27B6AD5F3B");

            entity.ToTable("AGVBillExportDetail", "agv");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Internalcode).HasMaxLength(100);
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalQuantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitName).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<AGVBillExportDetailSerial>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_AGVBillExportTechDetailSerial");

            entity.ToTable("AGVBillExportDetailSerial", "agv");

            entity.Property(e => e.SerialNumber).HasMaxLength(50);
        });

        modelBuilder.Entity<AGVBillExportLog>(entity =>
        {
            entity.ToTable("AGVBillExportLog", "agv");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateStatus).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<AGVBillImport>(entity =>
        {
            entity.ToTable("AGVBillImport", "agv");

            entity.Property(e => e.BillCode)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateRequestImport).HasColumnType("datetime");
            entity.Property(e => e.Deliver).HasMaxLength(150);
            entity.Property(e => e.Image).HasMaxLength(100);
            entity.Property(e => e.Receiver).HasMaxLength(150);
            entity.Property(e => e.Suplier).HasMaxLength(500);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.WarehouseType).HasMaxLength(100);
        });

        modelBuilder.Entity<AGVBillImportDetail>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__AGVBillImportDetail");

            entity.ToTable("AGVBillImportDetail", "agv");

            entity.Property(e => e.BillCodePO).HasMaxLength(150);
            entity.Property(e => e.COFormE).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateSomeBill).HasColumnType("datetime");
            entity.Property(e => e.DeadlineReturnNCC).HasColumnType("datetime");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.InternalCode).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProjectCode).HasMaxLength(100);
            entity.Property(e => e.ProjectName).HasMaxLength(100);
            entity.Property(e => e.QtyRequest).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SomeBill).HasMaxLength(250);
            entity.Property(e => e.TaxReduction).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalQuantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitName).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<AGVBillImportDetailSerial>(entity =>
        {
            entity.ToTable("AGVBillImportDetailSerial", "agv");

            entity.Property(e => e.SerialNumber).HasMaxLength(50);
        });

        modelBuilder.Entity<AGVBillImportDetailSerialNumber>(entity =>
        {
            entity.ToTable("AGVBillImportDetailSerialNumber", "agv");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.SerialNumber).HasMaxLength(250);
            entity.Property(e => e.SerialNumberAGV).HasMaxLength(250);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<AGVBillImportLog>(entity =>
        {
            entity.ToTable("AGVBillImportLog", "agv");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateStatus).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<AGVHistoryProduct>(entity =>
        {
            entity.ToTable("AGVHistoryProduct", "agv");

            entity.Property(e => e.AGVProductQRCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateBorrow)
                .HasComment("Ngày mượn thiết bị")
                .HasColumnType("datetime");
            entity.Property(e => e.DateReturn)
                .HasComment("Ngày trả đồ")
                .HasColumnType("datetime");
            entity.Property(e => e.DateReturnExpected).HasColumnType("datetime");
            entity.Property(e => e.EmployeeID).HasComment("ID người mượn");
            entity.Property(e => e.Note).HasComment("chú thích");
            entity.Property(e => e.NumberBorrow)
                .HasComment("Số lượng mượn")
                .HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Project)
                .HasMaxLength(550)
                .HasComment("Dự án sử dụng thiết bị");
            entity.Property(e => e.Status).HasComment("0: Đã trả; 1: Đang mượn; 2: Thiết bị đã mất;3: Thiết bị hỏng;4: Đăng ký trả;5: Quá hạn;6: Sắp hết hạn;7: Đăng kí mượn; 8: Đăng ký gia hạn");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<AGVHistoryProductLog>(entity =>
        {
            entity.ToTable("AGVHistoryProductLog", "agv");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateReturnExpected).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<AGVInventoryDemo>(entity =>
        {
            entity.ToTable("AGVInventoryDemo", "agv");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<AGVLocation>(entity =>
        {
            entity.ToTable("AGVLocation", "agv");

            entity.Property(e => e.AGVLocationCode).HasMaxLength(250);
            entity.Property(e => e.AGVLocationName).HasMaxLength(250);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<AGVProduct>(entity =>
        {
            entity.ToTable("AGVProduct", "agv");

            entity.Property(e => e.AGVProductCode).HasMaxLength(150);
            entity.Property(e => e.AGVProductGroupID).HasComment("Nhóm sản phẩm lấy từ bảng AGVProductGroup");
            entity.Property(e => e.AGVProductLocationID).HasComment("Vị trí sản phẩm lấy từ AGVProductLocation");
            entity.Property(e => e.AddressBox).HasMaxLength(150);
            entity.Property(e => e.CodeHCM)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CurrentIntensityMax).HasMaxLength(150);
            entity.Property(e => e.DataInterface).HasMaxLength(150);
            entity.Property(e => e.FNo).HasMaxLength(150);
            entity.Property(e => e.FirmID).HasComment("Hãng lấy từ bảng Firm");
            entity.Property(e => e.FocalLength).HasMaxLength(150);
            entity.Property(e => e.InputValue).HasMaxLength(150);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.LampColor).HasMaxLength(150);
            entity.Property(e => e.LampPower).HasMaxLength(150);
            entity.Property(e => e.LampType).HasMaxLength(150);
            entity.Property(e => e.LampWattage).HasMaxLength(150);
            entity.Property(e => e.LensMount).HasMaxLength(150);
            entity.Property(e => e.LocationImg).HasMaxLength(150);
            entity.Property(e => e.MOD).HasMaxLength(150);
            entity.Property(e => e.Magnification).HasMaxLength(150);
            entity.Property(e => e.Maker)
                .HasMaxLength(150)
                .HasComment("Hãng");
            entity.Property(e => e.MonoColor).HasMaxLength(150);
            entity.Property(e => e.Note).HasMaxLength(550);
            entity.Property(e => e.Number)
                .HasComment("Số lượng tổng")
                .HasColumnType("decimal(18, 0)");
            entity.Property(e => e.NumberInStore).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.OutputValue).HasMaxLength(150);
            entity.Property(e => e.PartNumber).HasMaxLength(150);
            entity.Property(e => e.PixelSize).HasMaxLength(150);
            entity.Property(e => e.ProductCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ProductName).HasMaxLength(550);
            entity.Property(e => e.Resolution).HasMaxLength(150);
            entity.Property(e => e.SensorSize).HasMaxLength(150);
            entity.Property(e => e.SensorSizeMax).HasMaxLength(150);
            entity.Property(e => e.Serial).HasMaxLength(150);
            entity.Property(e => e.SerialNumber).HasMaxLength(150);
            entity.Property(e => e.ShutterMode).HasMaxLength(150);
            entity.Property(e => e.Size).HasMaxLength(150);
            entity.Property(e => e.Status).HasComment("1: Đang giặt");
            entity.Property(e => e.StatusProduct).HasComment("Trạng thái sản phẩm (1: hiện có, 0: không có)");
            entity.Property(e => e.UnitCountID).HasComment("Đơn vị tính lấy từ bảng UnitCount");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.WD).HasMaxLength(150);
        });

        modelBuilder.Entity<AGVProductGroup>(entity =>
        {
            entity.ToTable("AGVProductGroup", "agv");

            entity.Property(e => e.AGVProductGroupName).HasMaxLength(150);
            entity.Property(e => e.AGVProductGroupNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<AGVProductQRCode>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_ProductAGVQRCode");

            entity.ToTable("AGVProductQRCode", "agv");

            entity.Property(e => e.AGVProductQRCode1)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AGVProductQRCode");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.PartNumber).HasMaxLength(150);
            entity.Property(e => e.Serial).HasMaxLength(150);
            entity.Property(e => e.SerialNumber).HasMaxLength(150);
            entity.Property(e => e.Status).HasComment("1:Trong Kho,2:Đang mượn,3:Đã Xuất Kho");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<AccountingBill>(entity =>
        {
            entity.ToTable("AccountingBill");

            entity.Property(e => e.BillDate).HasColumnType("datetime");
            entity.Property(e => e.BillNumber).HasMaxLength(250);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DeliverTaxDate).HasColumnType("datetime");
            entity.Property(e => e.DeliverTaxStatus).HasComment("Bàn giao cho BP thuế; 1: đã xác nhân, 0: chưa xác nhận");
            entity.Property(e => e.EmployeeStatus).HasComment("Xác nhận của Pur; 1: đã xác nhân, 0: chưa xác nhận");
            entity.Property(e => e.TotalMoney).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<AccountingBillApproved>(entity =>
        {
            entity.ToTable("AccountingBillApproved");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<AccountingContract>(entity =>
        {
            entity.ToTable("AccountingContract");

            entity.Property(e => e.Company).HasComment("1:RTC; 2:MVI; 3: APR; 4:YONKO");
            entity.Property(e => e.ContractGroup).HasComment("Loại HĐ chính (1:Hợp đồng mua vào; 2:Hợp đồng bán ra)");
            entity.Property(e => e.ContractNumber).HasMaxLength(550);
            entity.Property(e => e.ContractValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateContract).HasColumnType("datetime");
            entity.Property(e => e.DateExpired).HasColumnType("datetime");
            entity.Property(e => e.DateInput).HasColumnType("datetime");
            entity.Property(e => e.DateIsApprovedGroup)
                .HasComment("Ngày duyệt trên nhóm")
                .HasColumnType("datetime");
            entity.Property(e => e.DateReceived)
                .HasComment("Ngày nhận hồ sơ gốc")
                .HasColumnType("datetime");
            entity.Property(e => e.IsReceivedContract).HasComment("1: đã nhận hđ gốc; 0: chưa nhận");
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<AccountingContractFile>(entity =>
        {
            entity.ToTable("AccountingContractFile");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<AccountingContractLog>(entity =>
        {
            entity.ToTable("AccountingContractLog");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateLog).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<AccountingContractType>(entity =>
        {
            entity.ToTable("AccountingContractType");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsContractValue).HasComment("0: Không có giá trị; 1:Có giá trị");
            entity.Property(e => e.TypeCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ActionHistory>(entity =>
        {
            entity.ToTable("ActionHistory");

            entity.Property(e => e.Action).HasMaxLength(500);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        modelBuilder.Entity<AddressStock>(entity =>
        {
            entity.ToTable("AddressStock");

            entity.Property(e => e.Address).HasMaxLength(550);
        });

        modelBuilder.Entity<AdminMarketing>(entity =>
        {
            entity.ToTable("AdminMarketing");

            entity.Property(e => e.CompletionRate).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.KPI).HasMaxLength(250);
        });

        modelBuilder.Entity<AdminMarketingDetail>(entity =>
        {
            entity.ToTable("AdminMarketingDetail");

            entity.Property(e => e.CompletionRate).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PercentActual).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("AuditLog");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Action).HasComment("1: Thêm, 2: Sửa, 3: Xóa");
            entity.Property(e => e.ActionDate).HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(250);
            entity.Property(e => e.TableName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        modelBuilder.Entity<BillDocumentExport>(entity =>
        {
            entity.ToTable("BillDocumentExport");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.LogDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BillDocumentExportLog>(entity =>
        {
            entity.ToTable("BillDocumentExportLog");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.LogDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BillDocumentExportTechnical>(entity =>
        {
            entity.ToTable("BillDocumentExportTechnical");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.LogDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BillDocumentExportTechnicalLog>(entity =>
        {
            entity.ToTable("BillDocumentExportTechnicalLog");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.LogDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BillDocumentImport>(entity =>
        {
            entity.ToTable("BillDocumentImport");

            entity.HasIndex(e => e.BillImportID, "Index_BillDocumentImport_BillImportID");

            entity.HasIndex(e => e.DocumentImportID, "Index_BillDocumentImport_DocumentImportID");

            entity.HasIndex(e => e.DocumentStatus, "Index_BillDocumentImport_DocumentStatus");

            entity.HasIndex(e => e.LogDate, "Index_BillDocumentImport_LogDate");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.LogDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BillDocumentImportLog>(entity =>
        {
            entity.ToTable("BillDocumentImportLog");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.LogDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BillDocumentImportTechnical>(entity =>
        {
            entity.ToTable("BillDocumentImportTechnical");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateReceive).HasColumnType("datetime");
            entity.Property(e => e.LogDate).HasColumnType("datetime");
            entity.Property(e => e.ReasonCancel).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BillDocumentImportTechnicalLog>(entity =>
        {
            entity.ToTable("BillDocumentImportTechnicalLog");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.LogDate).HasColumnType("datetime");
            entity.Property(e => e.ReasonCancel).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BillExport>(entity =>
        {
            entity.ToTable("BillExport");

            entity.HasIndex(e => new { e.WarehouseID, e.IsDeleted }, "IX_BillExport_WarehouseID_IsDeleted");

            entity.HasIndex(e => e.AddressStockID, "Index_BillExport_AddressStockID");

            entity.HasIndex(e => e.BillDocumentExportType, "Index_BillExport_BillDocumentExportType");

            entity.HasIndex(e => e.Code, "Index_BillExport_Code");

            entity.HasIndex(e => e.CreatDate, "Index_BillExport_CreatDate");

            entity.HasIndex(e => e.CustomerID, "Index_BillExport_CustomerID");

            entity.HasIndex(e => e.GroupID, "Index_BillExport_GroupID");

            entity.HasIndex(e => e.IsApproved, "Index_BillExport_IsApproved");

            entity.HasIndex(e => e.IsPrepared, "Index_BillExport_IsPrepared");

            entity.HasIndex(e => e.IsReceived, "Index_BillExport_IsReceived");

            entity.HasIndex(e => e.KhoTypeID, "Index_BillExport_KhoTypeID");

            entity.HasIndex(e => e.RequestDate, "Index_BillExport_RequestDate");

            entity.HasIndex(e => e.SenderID, "Index_BillExport_SenderID");

            entity.HasIndex(e => e.Status, "Index_BillExport_Status");

            entity.HasIndex(e => e.SupplierID, "Index_BillExport_SupplierID");

            entity.HasIndex(e => e.TypeBill, "Index_BillExport_TypeBill");

            entity.HasIndex(e => e.UserID, "Index_BillExport_UserID");

            entity.HasIndex(e => e.WarehouseID, "Index_BillExport_WarehouseID");

            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.GroupID).HasMaxLength(50);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.PreparedDate).HasColumnType("datetime");
            entity.Property(e => e.RequestDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasComment("0. Mượn\r\n1. Tồn kho\r\n2. Đã xuất kho\r\n3. Chia trước\r\n4. Phiếu mượn nội bộ\r\n5. Xuất trả NCC\r\n6. Yêu cầu xuất kho");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.WarehouseType)
                .HasMaxLength(250)
                .HasComment("loại kho");
        });

        modelBuilder.Entity<BillExportAcountant>(entity =>
        {
            entity.ToTable("BillExportAcountant");

            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.InvoiceNumber).HasMaxLength(150);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BillExportAcountantDetail>(entity =>
        {
            entity.ToTable("BillExportAcountantDetail");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.GroupExport).HasMaxLength(350);
            entity.Property(e => e.IntoMoney).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IntoMoneyWithoutVat).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.InvoiceNumber).HasMaxLength(150);
            entity.Property(e => e.Note).HasMaxLength(550);
            entity.Property(e => e.ProductFullName).HasMaxLength(250);
            entity.Property(e => e.ProjectName).HasMaxLength(250);
            entity.Property(e => e.Qty).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.TotalIntoMoney).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalQty).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.VAT).HasColumnType("decimal(18, 3)");
        });

        modelBuilder.Entity<BillExportDetail>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_BillXuatDetail");

            entity.ToTable("BillExportDetail");

            entity.HasIndex(e => e.BillID, "Index_BillExportDetail_BillID");

            entity.HasIndex(e => e.POKHDetailID, "Index_BillExportDetail_POKHDetailID");

            entity.HasIndex(e => e.POKHID, "Index_BillExportDetail_POKHID");

            entity.HasIndex(e => e.ProductID, "Index_BillExportDetail_ProductID");

            entity.HasIndex(e => e.ProjectID, "Index_BillExportDetail_ProjectID");

            entity.HasIndex(e => e.ProjectPartListID, "Index_BillExportDetail_ProjectPartListID");

            entity.HasIndex(e => e.TradePriceDetailID, "Index_BillExportDetail_TradePriceDetailID");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ExpectReturnDate).HasColumnType("datetime");
            entity.Property(e => e.GroupExport).HasMaxLength(350);
            entity.Property(e => e.InvoiceNumber).HasMaxLength(150);
            entity.Property(e => e.ProductFullName)
                .HasMaxLength(250)
                .HasComment("tên sản phẩm");
            entity.Property(e => e.ProductID).HasComment("Id master");
            entity.Property(e => e.ProjectName)
                .HasMaxLength(250)
                .HasComment("tên dự án");
            entity.Property(e => e.Qty)
                .HasComment("số lượng")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SerialNumber).HasMaxLength(50);
            entity.Property(e => e.Specifications).HasMaxLength(550);
            entity.Property(e => e.TotalInventory).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalQty).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BillExportDetailSerialNumber>(entity =>
        {
            entity.ToTable("BillExportDetailSerialNumber");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.SerialNumber).HasMaxLength(250);
            entity.Property(e => e.SerialNumberRTC).HasMaxLength(250);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BillExportDetailSerialNumberModulaLocation>(entity =>
        {
            entity.ToTable("BillExportDetailSerialNumberModulaLocation");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BillExportDetailTechnical>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__BillExpo__3214EC27B6AD5F3B");

            entity.ToTable("BillExportDetailTechnical");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Internalcode).HasMaxLength(100);
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalQuantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitName).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BillExportLog>(entity =>
        {
            entity.ToTable("BillExportLog");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateStatus).HasColumnType("datetime");
            entity.Property(e => e.StatusBill).HasComment("1: Nhận chứng từ hoặc đã nhận bill hoặc đã duyệt; 0: Chưa nhận hoặc là huỷ duyệt...");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BillExportTechDetailSerial>(entity =>
        {
            entity.ToTable("BillExportTechDetailSerial");

            entity.Property(e => e.SerialNumber).HasMaxLength(50);
        });

        modelBuilder.Entity<BillExportTechnical>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__BillExpo__3214EC2747739EC3");

            entity.ToTable("BillExportTechnical");

            entity.HasIndex(e => e.Code, "BillExportTechnical_Code_Index");

            entity.Property(e => e.Addres).HasMaxLength(100);
            entity.Property(e => e.BillType).HasComment("0. Trả\r\n1. Cho mượn\r\n2. Tặng / Bán\r\n3. Mất\r\n4. Bảo hành\r\n5. Xuất dự án\r\n6. Hỏng\r\n7. Xuất kho");
            entity.Property(e => e.Code)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.Deliver).HasMaxLength(100);
            entity.Property(e => e.ExpectedDate).HasColumnType("datetime");
            entity.Property(e => e.Image).HasMaxLength(100);
            entity.Property(e => e.Note).HasMaxLength(250);
            entity.Property(e => e.ProjectName).HasMaxLength(550);
            entity.Property(e => e.Receiver).HasMaxLength(100);
            entity.Property(e => e.SupplierName).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.WarehouseType).HasMaxLength(100);
        });

        modelBuilder.Entity<BillExportTechnicalLog>(entity =>
        {
            entity.ToTable("BillExportTechnicalLog");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateStatus).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BillFilm>(entity =>
        {
            entity.ToTable("BillFilm");

            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CreatDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(100);
        });

        modelBuilder.Entity<BillFilmDetail>(entity =>
        {
            entity.ToTable("BillFilmDetail");
        });

        modelBuilder.Entity<BillImport>(entity =>
        {
            entity.ToTable("BillImport");

            entity.HasIndex(e => new { e.WarehouseID, e.BillTypeNew }, "IX_BillImport_WarehouseID_BillTypeNew");

            entity.HasIndex(e => e.BillImportCode, "Index_BillImport_BillImportCode");

            entity.HasIndex(e => e.BillTypeNew, "Index_BillImport_BillTypeNew");

            entity.HasIndex(e => e.CreatDate, "Index_BillImport_CreatDate");

            entity.HasIndex(e => e.DeliverID, "Index_BillImport_DeliverID");

            entity.HasIndex(e => e.GroupID, "Index_BillImport_GroupID");

            entity.HasIndex(e => e.KhoTypeID, "Index_BillImport_KhoTypeID");

            entity.HasIndex(e => e.ReciverID, "Index_BillImport_ReciverID");

            entity.HasIndex(e => e.RulePayID, "Index_BillImport_RulePayID");

            entity.HasIndex(e => e.Status, "Index_BillImport_Status");

            entity.HasIndex(e => e.SupplierID, "Index_BillImport_SupplierID");

            entity.HasIndex(e => e.WarehouseID, "Index_BillImport_WarehouseID");

            entity.Property(e => e.BillDocumentImportType).HasComment("1:Hoàn thành; 2:Chưa hoàn thành");
            entity.Property(e => e.BillImportCode).HasMaxLength(150);
            entity.Property(e => e.BillType).HasComment("Loại phiếu: 1: Phiếu trả, 0: phiếu nhập bình thường");
            entity.Property(e => e.BillTypeNew).HasComment("0: Phiếu nhập\r\n1: Phiếu trả\r\n2: Phiếu trả nội bộ\r\n3: Phiếu mượn NCC\r\n4: Yêu cầu nhập kho\r\n");
            entity.Property(e => e.CreatDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateRequestImport).HasColumnType("datetime");
            entity.Property(e => e.Deliver)
                .HasMaxLength(150)
                .HasComment("người giao");
            entity.Property(e => e.GroupID).HasMaxLength(150);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.KhoType)
                .HasMaxLength(150)
                .HasComment("Kho: 1:Kho sale, 2: kho dự án, 0: tất cả");
            entity.Property(e => e.Reciver)
                .HasMaxLength(150)
                .HasComment("Người nhận");
            entity.Property(e => e.Status).HasComment("Trạng thái, 1:Duyệt, 0: Chưa duyệt");
            entity.Property(e => e.Suplier).HasMaxLength(150);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BillImportDetail>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_BillImportDetails");

            entity.ToTable("BillImportDetail", tb => tb.HasComment("Danh sách thiết bị nhập"));

            entity.HasIndex(e => e.BillImportID, "IX_BillImportDetail_BillImportID");

            entity.HasIndex(e => e.BillExportDetailID, "Index_BillImportDetail_BillExportDetailID");

            entity.HasIndex(e => e.BillImportID, "Index_BillImportDetail_BillImportID");

            entity.HasIndex(e => e.PONCCDetailID, "Index_BillImportDetail_PONCCDetailID");

            entity.HasIndex(e => e.ProductID, "Index_BillImportDetail_ProductID");

            entity.HasIndex(e => e.ProjectID, "Index_BillImportDetail_ProjectID");

            entity.HasIndex(e => e.ProjectPartListID, "Index_BillImportDetail_ProjectPartListID");

            entity.Property(e => e.BillCodePO).HasMaxLength(550);
            entity.Property(e => e.BillImportID).HasComment("Mã master nhập");
            entity.Property(e => e.CodeMaPhieuMuon).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateSomeBill).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Qty).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QtyRequest).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ReturnedStatus).HasComment("1: Đã trả, 2: Chưa trả");
            entity.Property(e => e.SerialNumber).HasMaxLength(50);
            entity.Property(e => e.SomeBill)
                .HasMaxLength(550)
                .HasComment("sô hóa đơn");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalQty).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BillImportDetailSerialNumber>(entity =>
        {
            entity.ToTable("BillImportDetailSerialNumber");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.SerialNumber).HasMaxLength(250);
            entity.Property(e => e.SerialNumberRTC).HasMaxLength(250);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BillImportDetailSerialNumberModulaLocation>(entity =>
        {
            entity.ToTable("BillImportDetailSerialNumberModulaLocation");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BillImportDetailTechnical>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__BillImpo__3214EC27418E7CFE");

            entity.ToTable("BillImportDetailTechnical");

            entity.Property(e => e.BillCodePO).HasMaxLength(150);
            entity.Property(e => e.COFormE).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateSomeBill).HasColumnType("datetime");
            entity.Property(e => e.DeadlineReturnNCC).HasColumnType("datetime");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.InternalCode).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProjectCode).HasMaxLength(100);
            entity.Property(e => e.ProjectName).HasMaxLength(100);
            entity.Property(e => e.QtyRequest).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SomeBill).HasMaxLength(250);
            entity.Property(e => e.TaxReduction).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalQuantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitName).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BillImportLog>(entity =>
        {
            entity.ToTable("BillImportLog");

            entity.HasIndex(e => e.BillImportID, "Index_BillImportLog_BillImportID");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateStatus).HasColumnType("datetime");
            entity.Property(e => e.StatusBill).HasComment("1: Nhận chứng từ hoặc đã nhận bill hoặc đã duyệt; 0: Chưa nhận hoặc là huỷ duyệt...");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BillImportQC>(entity =>
        {
            entity.ToTable("BillImportQC");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Dealine).HasColumnType("datetime");
            entity.Property(e => e.EmployeeRequestID).HasComment("Nhân viên yêu cầu");
            entity.Property(e => e.RequestDateQC)
                .HasComment("Ngày yêu cầu QC")
                .HasColumnType("datetime");
            entity.Property(e => e.RequestImportCode)
                .HasMaxLength(150)
                .HasComment("Mã billimport");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BillImportQCDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("BillImportQCDetail");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ID).ValueGeneratedOnAdd();
            entity.Property(e => e.Status).HasComment("1.OK 2.NG");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BillImportTechDetailSerial>(entity =>
        {
            entity.ToTable("BillImportTechDetailSerial");

            entity.Property(e => e.SerialNumber).HasMaxLength(50);
        });

        modelBuilder.Entity<BillImportTechnical>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__BillImpo__3214EC2763CB0784");

            entity.ToTable("BillImportTechnical");

            entity.Property(e => e.BillCode)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateRequestImport).HasColumnType("datetime");
            entity.Property(e => e.Deliver).HasMaxLength(150);
            entity.Property(e => e.Image).HasMaxLength(100);
            entity.Property(e => e.Receiver).HasMaxLength(150);
            entity.Property(e => e.Suplier).HasMaxLength(500);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.WarehouseType).HasMaxLength(100);
        });

        modelBuilder.Entity<BillImportTechnicalLog>(entity =>
        {
            entity.ToTable("BillImportTechnicalLog");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateStatus).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BillSale>(entity =>
        {
            entity.ToTable("BillSale");

            entity.Property(e => e.BillCode).HasMaxLength(50);
            entity.Property(e => e.ProductName).HasMaxLength(150);
        });

        modelBuilder.Entity<BonusRule>(entity =>
        {
            entity.ToTable("BonusRule");

            entity.Property(e => e.CompareMAX).HasComment("0: dấu <, 1: dấu <=");
            entity.Property(e => e.Max).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Min).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PercentRule).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Value).HasColumnType("decimal(18, 1)");
        });

        modelBuilder.Entity<BonusRuleIndex>(entity =>
        {
            entity.ToTable("BonusRuleIndex");

            entity.Property(e => e.PercentBQMS).HasColumnType("decimal(18, 5)");
            entity.Property(e => e.PercentBase).HasColumnType("decimal(18, 5)");
            entity.Property(e => e.PercentBonus).HasColumnType("decimal(18, 5)");
            entity.Property(e => e.PercentMachine).HasColumnType("decimal(18, 5)");
            entity.Property(e => e.PercentVision).HasColumnType("decimal(18, 5)");
        });

        modelBuilder.Entity<BookingRoom>(entity =>
        {
            entity.ToTable("BookingRoom");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateRegister).HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.IsApproved).HasComment("0:Chưa duyệt; 1:đã duyệt");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BookingRoomLog>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_MeetingRoomLog");

            entity.ToTable("BookingRoomLog");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateLog).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BusinessField>(entity =>
        {
            entity.ToTable("BusinessField");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<BusinessFieldLink>(entity =>
        {
            entity.ToTable("BusinessFieldLink");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ChangeLogStore>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__ChangeLo__5E5486484A8310C6");

            entity.ToTable("ChangeLogStore");

            entity.Property(e => e.DatabaseName).HasMaxLength(256);
            entity.Property(e => e.EventDate).HasColumnType("datetime");
            entity.Property(e => e.EventType).HasMaxLength(50);
            entity.Property(e => e.LoginName).HasMaxLength(256);
            entity.Property(e => e.ObjectName).HasMaxLength(256);
            entity.Property(e => e.ObjectType).HasMaxLength(25);
        });

        modelBuilder.Entity<ConfigPrice>(entity =>
        {
            entity.ToTable("ConfigPrice");

            entity.Property(e => e.BankCharges).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CustomFees).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Declaration).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Exchange).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NumberOfTransactions).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TransportFee).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ConfigSystem>(entity =>
        {
            entity.ToTable("ConfigSystem");

            entity.Property(e => e.ConfigType).HasComment("1: Tiền tệ,2: Cấu hình mail");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.KeyName).HasMaxLength(100);
            entity.Property(e => e.KeyValue).HasDefaultValue("");
            entity.Property(e => e.KeyValue1)
                .HasMaxLength(200)
                .HasDefaultValue("");
            entity.Property(e => e.KeyValue10)
                .HasMaxLength(200)
                .HasDefaultValue("");
            entity.Property(e => e.KeyValue2)
                .HasMaxLength(200)
                .HasDefaultValue("");
            entity.Property(e => e.KeyValue3)
                .HasMaxLength(200)
                .HasDefaultValue("");
            entity.Property(e => e.KeyValue4)
                .HasMaxLength(200)
                .HasDefaultValue("");
            entity.Property(e => e.KeyValue5)
                .HasMaxLength(200)
                .HasDefaultValue("");
            entity.Property(e => e.KeyValue6)
                .HasMaxLength(200)
                .HasDefaultValue("");
            entity.Property(e => e.KeyValue7)
                .HasMaxLength(200)
                .HasDefaultValue("");
            entity.Property(e => e.KeyValue8)
                .HasMaxLength(200)
                .HasDefaultValue("");
            entity.Property(e => e.KeyValue9)
                .HasMaxLength(200)
                .HasDefaultValue("");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Course>(entity =>
        {
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
            entity.HasKey(e => e.ID).HasName("PK_CourseAnswer");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseCatalog>(entity =>
        {
            entity.ToTable("CourseCatalog");

            entity.Property(e => e.CatalogType).HasComment("1:Cơ bản; 2:Nâng cao");
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

        modelBuilder.Entity<CourseCatalogProjectType>(entity =>
        {
            entity.ToTable("CourseCatalogProjectType");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseExam>(entity =>
        {
            entity.ToTable("CourseExam");

            entity.Property(e => e.CodeExam)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ExamType).HasComment("1: trắc nghiệm; 2: Thực hành");
            entity.Property(e => e.Goal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseExamEvaluate>(entity =>
        {
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
            entity.ToTable("CourseExamPractice");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateEnd).HasColumnType("datetime");
            entity.Property(e => e.DateStart).HasColumnType("datetime");
            entity.Property(e => e.Evaluate).HasComment("1; đánh giá Đạt; 0:Không đạt");
            entity.Property(e => e.PracticePoints).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseExamResult>(entity =>
        {
            entity.ToTable("CourseExamResult");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.PercentageCorrect).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PracticePoints).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status).HasComment("0: Chưa hoàn thành; 1:Hoàn thành");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseExamResultDetail>(entity =>
        {
            entity.ToTable("CourseExamResultDetail");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseFile>(entity =>
        {
            entity.ToTable("CourseFile");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.NameFile).HasMaxLength(150);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseLesson>(entity =>
        {
            entity.ToTable("CourseLesson");

            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.LessonTitle).HasMaxLength(400);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.VideoURL)
                .HasMaxLength(300)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CourseLessonHistory>(entity =>
        {
            entity.ToTable("CourseLessonHistory");

            entity.Property(e => e.ViewDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseLessonLog>(entity =>
        {
            entity.ToTable("CourseLessonLog");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateLog).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseQuestion>(entity =>
        {
            entity.ToTable("CourseQuestion");

            entity.Property(e => e.CheckInput).HasComment("1: có 1 đáp án đúng; 2: Có nhiều đáp án đúng");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseRightAnswer>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseType>(entity =>
        {
            entity.ToTable("CourseType");

            entity.Property(e => e.CourseTypeCode)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CourseTypeName).HasMaxLength(550);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsLearnInTurn).HasComment("1: Học lần lượt, 0: ko cần học lần lượt");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.ToTable("Currency");

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CurrencyRate).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CurrencyRateOfficialQuota).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CurrencyRateUnofficialQuota).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DateExpried).HasColumnType("datetime");
            entity.Property(e => e.DateExpriedOfficialQuota).HasColumnType("datetime");
            entity.Property(e => e.DateExpriedUnofficialQuota).HasColumnType("datetime");
            entity.Property(e => e.DateStart).HasColumnType("datetime");
            entity.Property(e => e.MinUnit).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customer", tb => tb.HasComment("Khách hàng"));

            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.AdressStock).HasMaxLength(500);
            entity.Property(e => e.CheckVoucher).HasMaxLength(500);
            entity.Property(e => e.ClosingDateDebt).HasColumnType("datetime");
            entity.Property(e => e.ContactEmail).HasMaxLength(100);
            entity.Property(e => e.ContactName).HasMaxLength(100);
            entity.Property(e => e.ContactNote).HasMaxLength(300);
            entity.Property(e => e.ContactPhone).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CustomerCode).HasMaxLength(30);
            entity.Property(e => e.CustomerDetails).HasMaxLength(500);
            entity.Property(e => e.CustomerName).HasMaxLength(200);
            entity.Property(e => e.CustomerShortName).HasMaxLength(200);
            entity.Property(e => e.CustomerType).HasComment("0: other, 1: nhà máy, 2: thương mại");
            entity.Property(e => e.Debt).HasMaxLength(500);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.HardCopyVoucher).HasMaxLength(500);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.NoteDelivery).HasMaxLength(500);
            entity.Property(e => e.NoteVoucher).HasMaxLength(500);
            entity.Property(e => e.Phone).HasMaxLength(100);
            entity.Property(e => e.ProductDetails).HasMaxLength(500);
            entity.Property(e => e.Province).HasMaxLength(50);
            entity.Property(e => e.StatusDisable).HasDefaultValue(0);
            entity.Property(e => e.TaxCode).HasMaxLength(500);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CustomerBase>(entity =>
        {
            entity.ToTable("CustomerBase");

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CustomerCode).HasMaxLength(150);
            entity.Property(e => e.CustomerName).HasMaxLength(150);
            entity.Property(e => e.KCN).HasMaxLength(150);
            entity.Property(e => e.Note).HasMaxLength(250);
            entity.Property(e => e.ProductName).HasMaxLength(150);
            entity.Property(e => e.Province).HasMaxLength(150);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CustomerBaseContact>(entity =>
        {
            entity.ToTable("CustomerBaseContact");

            entity.Property(e => e.ContactEmail).HasMaxLength(150);
            entity.Property(e => e.ContactName).HasMaxLength(150);
            entity.Property(e => e.ContactPhone).HasMaxLength(150);
            entity.Property(e => e.CustomerPosition).HasMaxLength(150);
            entity.Property(e => e.Note).HasMaxLength(150);
        });

        modelBuilder.Entity<CustomerContact>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_CustomerDetail2");

            entity.ToTable("CustomerContact");

            entity.Property(e => e.ContactEmail).HasMaxLength(250);
            entity.Property(e => e.ContactName).HasMaxLength(150);
            entity.Property(e => e.ContactPhone).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CustomerPart).HasMaxLength(250);
            entity.Property(e => e.CustomerPosition).HasMaxLength(250);
            entity.Property(e => e.CustomerTeam).HasMaxLength(250);
        });

        modelBuilder.Entity<CustomerEmployee>(entity =>
        {
            entity.ToTable("CustomerEmployee");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CustomerPart>(entity =>
        {
            entity.ToTable("CustomerPart");

            entity.Property(e => e.PartCode).HasMaxLength(50);
            entity.Property(e => e.PartName).HasMaxLength(250);
        });

        modelBuilder.Entity<CustomerSpecialization>(entity =>
        {
            entity.ToTable("CustomerSpecialization");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<DailyReportHR>(entity =>
        {
            entity.ToTable("DailyReportHR");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateReport).HasColumnType("datetime");
            entity.Property(e => e.KmNumber)
                .HasComment("Số km")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Percentage)
                .HasComment("tỷ lệ năng suất trung bình / năng xuất thực tế")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PerformanceActual)
                .HasComment("TimeActual / Quantity (năng suất thực tế)")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Propose).HasComment("Kiến nghị / đề xuất");
            entity.Property(e => e.Quantity).HasComment("Kết quả thực hiện cắt phim");
            entity.Property(e => e.TimeActual)
                .HasComment("Thời gian thực hiện")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalLate).HasComment("Số cuốc xe muộn so với lịch đặt xe");
            entity.Property(e => e.TotalTimeLate)
                .HasComment("Tống số phút chậm")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<DailyReportSale>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_FollowKH");

            entity.ToTable("DailyReportSale");

            entity.Property(e => e.Confirm).HasDefaultValue(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateEnd).HasColumnType("datetime");
            entity.Property(e => e.DateStart).HasColumnType("datetime");
            entity.Property(e => e.DeleteFlag).HasDefaultValue(0);
            entity.Property(e => e.ProductOfCustomer).HasMaxLength(550);
            entity.Property(e => e.RequestOfCustomer).HasMaxLength(550);
        });

        modelBuilder.Entity<DailyReportSaleAdmin>(entity =>
        {
            entity.ToTable("DailyReportSaleAdmin");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateReport).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<DailyReportTechnical>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__DailyRep__3214EC27DF30369E");

            entity.ToTable("DailyReportTechnical");

            entity.Property(e => e.Confirm).HasDefaultValue(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DeleteFlag).HasDefaultValue(0);
            entity.Property(e => e.TotalHourOT).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalHours).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Department");

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.Email).HasMaxLength(250);
            entity.Property(e => e.IsShowHotline).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.ToTable("Document");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateEffective).HasColumnType("datetime");
            entity.Property(e => e.DatePromulgate).HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.NameDocument).HasMaxLength(200);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<DocumentExport>(entity =>
        {
            entity.ToTable("DocumentExport");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<DocumentFile>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_DoccumentFile");

            entity.ToTable("DocumentFile");

            entity.Property(e => e.FileName).HasMaxLength(200);
            entity.Property(e => e.FileNameOrigin).HasMaxLength(200);
            entity.Property(e => e.FilePath)
                .HasMaxLength(300)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DocumentImport>(entity =>
        {
            entity.ToTable("DocumentImport");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DocumentImportCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DocumentImportName).HasMaxLength(200);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<DocumentImportPONCC>(entity =>
        {
            entity.ToTable("DocumentImportPONCC");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateAdditional).HasColumnType("datetime");
            entity.Property(e => e.DateRecive).HasColumnType("datetime");
            entity.Property(e => e.Status).HasComment("1:Nhận; 2:Huỷ nhận;3:Khum có");
            entity.Property(e => e.StatusHR).HasComment("1:Nhận; 2:Huỷ nhận;3:Khum có");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<DocumentSale>(entity =>
        {
            entity.ToTable("DocumentSale");

            entity.Property(e => e.BillID).HasComment("ID của phiếu nhập hoặc xuất");
            entity.Property(e => e.BillType).HasComment("1: phiếu nhập; 2:Phiếu xuất");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(200);
            entity.Property(e => e.FileNameOrigin).HasMaxLength(200);
            entity.Property(e => e.FilePath)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<DocumentType>(entity =>
        {
            entity.ToTable("DocumentType");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
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
            entity.Property(e => e.IsSetupFunction).HasDefaultValue(false);
            entity.Property(e => e.JobDescription).HasMaxLength(550);
            entity.Property(e => e.Khac).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Leader).HasDefaultValue(0);
            entity.Property(e => e.LuongCoBan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LuongThuViec).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MST).HasMaxLength(550);
            entity.Property(e => e.MoiQuanHe).HasMaxLength(150);
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
            entity.Property(e => e.Status).HasDefaultValue(0);
            entity.Property(e => e.Telephone).HasMaxLength(550);
            entity.Property(e => e.TinhDcTamTru).HasMaxLength(150);
            entity.Property(e => e.TinhDcThuongTru).HasMaxLength(150);
            entity.Property(e => e.TinhTrangKyHD)
                .HasMaxLength(150)
                .HasComment("1: ");
            entity.Property(e => e.TonGiao).HasMaxLength(550);
            entity.Property(e => e.TongLuong).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TongPhuCap).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangPhuc).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(550);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserZaloID).HasMaxLength(250);
            entity.Property(e => e.XangXe).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<EmployeeApprove>(entity =>
        {
            entity.ToTable("EmployeeApprove");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Type).HasComment("1: TBP duyệt; 2: leader dự án");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeAttendance>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Employee__3214EC2774B3BC97");

            entity.ToTable("EmployeeAttendance");

            entity.Property(e => e.AttendanceDate).HasColumnType("datetime");
            entity.Property(e => e.CheckIn)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CheckInDate).HasColumnType("datetime");
            entity.Property(e => e.CheckOut)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CheckOutDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DayWeek).HasMaxLength(100);
            entity.Property(e => e.IDChamCongMoi).HasMaxLength(550);
            entity.Property(e => e.Interval)
                .HasMaxLength(50)
                .HasComment("Khoảng thời gian trong ngày(0h - 24h)");
            entity.Property(e => e.Note).HasMaxLength(550);
            entity.Property(e => e.TimeEarly).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TimeLate).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalDay).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalHour).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeBussiness>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_EmployeBussiness");

            entity.ToTable("EmployeeBussiness");

            entity.Property(e => e.ApprovedID).HasComment("Trưởng phòng duyệt");
            entity.Property(e => e.CostBussiness).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.CostOvernight).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CostVehicle).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.CostWorkEarly).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateApprovedBGD).HasColumnType("datetime");
            entity.Property(e => e.DayBussiness).HasColumnType("datetime");
            entity.Property(e => e.DecilineApprove).HasComment("2: Không đồng ý duyệt; 1: Có đồng ý duyệt");
            entity.Property(e => e.Location).HasMaxLength(550);
            entity.Property(e => e.NotChekIn).HasComment("true: Không chấm công ở văn phòng");
            entity.Property(e => e.OvernightType).HasComment("1:Phụ cấp ăn tối từ sau 20h; 2:Phụ cấp ăn tối theo loại công tác");
            entity.Property(e => e.ProvinceID).HasDefaultValue(0);
            entity.Property(e => e.Reason).HasMaxLength(550);
            entity.Property(e => e.TotalMoney).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TypeBusiness).HasComment("Loại công tác: 1.Công tác ngày; 2.Công tác đêm; 3. Công tác gần (10km - 30km); 4.Công tác xa");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeBussinessVehicle>(entity =>
        {
            entity.ToTable("EmployeeBussinessVehicle");

            entity.Property(e => e.Cost).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.VehicleName).HasMaxLength(500);
        });

        modelBuilder.Entity<EmployeeChamCongDetail>(entity =>
        {
            entity.ToTable("EmployeeChamCongDetail");

            entity.HasIndex(e => e.DayFinger, "Indexx_EmployeeChamCongDetail_DayFinger");

            entity.HasIndex(e => e.EmployeeID, "Indexx_EmployeeChamCongDetail_EmployeeID");

            entity.HasIndex(e => e.IDChamCong, "Indexx_EmployeeChamCongDetail_IDChamCong");

            entity.HasIndex(e => e.MasterID, "Indexx_EmployeeChamCongDetail_MasterID");

            entity.Property(e => e.CheckIn).HasColumnType("datetime");
            entity.Property(e => e.CheckOut).HasColumnType("datetime");
            entity.Property(e => e.CostBussiness).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CostWorkEarly).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DayFinger).HasColumnType("datetime");
            entity.Property(e => e.FoodOrderText)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.IDChamCong)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NotCheckin).HasComment("Không chấm công tại Vp khi đi công tác");
            entity.Property(e => e.OnLeaveDay).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalDay).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalDayFinger)
                .HasComment("Ngày công theo bảng chấm vân tay")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalDayText).HasMaxLength(50);
            entity.Property(e => e.TotalDayWFH).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalLunchText)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TotalNightShift).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalTimeOT).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.WorkTime).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<EmployeeChamCongMaster>(entity =>
        {
            entity.ToTable("EmployeeChamCongMaster");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(500);
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.TimeType).HasComment("Kiểu thời gian (0,theo buồi) (1,theo ngày)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeChucVu>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_ChucVu");

            entity.ToTable("EmployeeChucVu");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeChucVuHD>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_ChucVuHD");

            entity.ToTable("EmployeeChucVuHD");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeCollectMoney>(entity =>
        {
            entity.ToTable("EmployeeCollectMoney");

            entity.Property(e => e.CollectDay).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Error).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Fund).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Notes).HasMaxLength(550);
            entity.Property(e => e.Party).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TotalMoney).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeCompensatoryLeave>(entity =>
        {
            entity.ToTable("EmployeeCompensatoryLeave");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateValue).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeContract>(entity =>
        {
            entity.ToTable("EmployeeContract");

            entity.Property(e => e.ContractNumber)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateEnd)
                .HasComment("Ngày kết thúc hợp đồng")
                .HasColumnType("datetime");
            entity.Property(e => e.DateSign).HasColumnType("datetime");
            entity.Property(e => e.DateStart)
                .HasComment("Ngày bắt đầu hợp đồng")
                .HasColumnType("datetime");
            entity.Property(e => e.StatusSign).HasComment("1: chưa ký; 2: đã ký");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeCurricular>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_Curricular");

            entity.ToTable("EmployeeCurricular");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CurricularCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CurricularName).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeEarlyLate>(entity =>
        {
            entity.ToTable("EmployeeEarlyLate");

            entity.Property(e => e.ApprovedID).HasComment("Người duyệt (Từ bảng Employee)");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateEnd).HasColumnType("datetime");
            entity.Property(e => e.DateRegister).HasColumnType("datetime");
            entity.Property(e => e.DateStart).HasColumnType("datetime");
            entity.Property(e => e.DecilineApprove).HasComment("2: Không đồng ý duyệt; 1: Có đồng ý duyệt");
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.TimeRegister).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Type).HasComment("1: Đi muộn việc cá nhân; 2: Về sớm việc cá nhân;  3: Về sớm việc công ty; 4:Đi muộn việc  công ty; ");
            entity.Property(e => e.Unit).HasComment("Đơn vị thời gian. 1:Giờ, 2: phút");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeEducationLevel>(entity =>
        {
            entity.ToTable("EmployeeEducationLevel");

            entity.Property(e => e.Classification).HasComment("Xếp loại (1:Giỏi; 2: Khá, 3: Trung bình)");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Major).HasMaxLength(550);
            entity.Property(e => e.RankType).HasComment("1:Đại học (ĐH); 2: Cao đẳng (CĐ); 3: Trung cấp (TC)");
            entity.Property(e => e.SchoolName).HasMaxLength(550);
            entity.Property(e => e.TrainType).HasComment("Loại hình đào tạo (1. Chính quy;2. Liên thông)");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeError>(entity =>
        {
            entity.ToTable("EmployeeError");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateError).HasColumnType("datetime");
            entity.Property(e => e.Money).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeFamily>(entity =>
        {
            entity.ToTable("EmployeeFamily");

            entity.Property(e => e.CMND).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DanToc).HasMaxLength(150);
            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.GioiTinh).HasComment("values(0,Nam) ,(1,Nữ),(2,Khác)");
            entity.Property(e => e.NgaySinh).HasColumnType("datetime");
            entity.Property(e => e.NoiKhaiSinh).HasMaxLength(200);
            entity.Property(e => e.QuanHe).HasMaxLength(50);
            entity.Property(e => e.QuocTich).HasMaxLength(50);
            entity.Property(e => e.SoBHXH).HasMaxLength(50);
            entity.Property(e => e.TenNguoiThan).HasMaxLength(500);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeFingerprint>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_EFingerprint");

            entity.ToTable("EmployeeFingerprint");

            entity.Property(e => e.CheckIn).HasColumnType("datetime");
            entity.Property(e => e.CheckOut).HasColumnType("datetime");
            entity.Property(e => e.Day).HasColumnType("datetime");
            entity.Property(e => e.DayOfWeek).HasMaxLength(100);
            entity.Property(e => e.IDChamCong)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Note).HasMaxLength(550);
            entity.Property(e => e.Organization).HasMaxLength(250);
            entity.Property(e => e.Period).HasMaxLength(550);
            entity.Property(e => e.SumEarly)
                .HasComment("lấy giá trị tính theo phút")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SumLate)
                .HasComment("Lấy giá trị tính theo phút")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TimeReal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalTime)
                .HasComment("Lấy giá trị tính theo giờ")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.WorkTime).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<EmployeeFingerprintMaster>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_FingerprintMaster");

            entity.ToTable("EmployeeFingerprintMaster");

            entity.Property(e => e.Note).HasMaxLength(250);
        });

        modelBuilder.Entity<EmployeeFoodOrder>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__FoodOrde__4EAA494328980842");

            entity.ToTable("EmployeeFoodOrder");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateOrder).HasColumnType("datetime");
            entity.Property(e => e.IsApproved).HasDefaultValue(false);
            entity.Property(e => e.Location).HasComment("1: VP Hà nội, 2: Đan phượng");
            entity.Property(e => e.ReasonDeciline).HasMaxLength(150);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeLoaiHDLD>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_LoaiHDLD");

            entity.ToTable("EmployeeLoaiHDLD");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeLuckyNumber>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Employee__3214EC275603858C");

            entity.ToTable("EmployeeLuckyNumber");

            entity.HasIndex(e => e.LuckyNumber, "UQ__Employee__F83B7661035A5636").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(250);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmployeeCode).HasMaxLength(150);
            entity.Property(e => e.EmployeeName).HasMaxLength(550);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(250);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeNighShift>(entity =>
        {
            entity.ToTable("EmployeeNighShift");

            entity.Property(e => e.BreaksTime).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateEnd).HasColumnType("datetime");
            entity.Property(e => e.DateRegister).HasColumnType("datetime");
            entity.Property(e => e.DateStart).HasColumnType("datetime");
            entity.Property(e => e.IsApprovedHR).HasComment("0: Chờ duyệt; 1: Duyệt");
            entity.Property(e => e.IsApprovedTBP).HasComment("0: Chờ duyệt; 1: Duyệt; 2:Không duyệt");
            entity.Property(e => e.Location).HasMaxLength(550);
            entity.Property(e => e.TotalHours).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.WorkTime).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<EmployeeNoFingerprint>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_NoFingerprint");

            entity.ToTable("EmployeeNoFingerprint");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DayWork).HasColumnType("datetime");
            entity.Property(e => e.DecilineApprove).HasComment("2: Không đồng ý duyệt; 1: Có đồng ý duyệt");
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.Type).HasComment("1:Quên buổi sáng; 2:Quên buổi chiều;3:Quên chấm công do đi công tác");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeOnLeave>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__OnLeave__3214EC277D276814");

            entity.ToTable("EmployeeOnLeave");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateApprovedBGD).HasColumnType("datetime");
            entity.Property(e => e.DateCancel).HasColumnType("datetime");
            entity.Property(e => e.DecilineApprove).HasComment("2: Không đồng ý duyệt; 1: Có đồng ý duyệt");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.TimeOnLeave).HasComment("1: Nghỉ buổi sáng; 2:Nghỉ buổi chiều; 3:Nghỉ cả ngày");
            entity.Property(e => e.TotalDay).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalTime).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Type).HasComment("1: Nghỉ không lương; 2: Nghỉ phép");
            entity.Property(e => e.TypeIsReal).HasComment("1: Nghỉ không lương; 2: Nghỉ phép");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeOnLeaveMaster>(entity =>
        {
            entity.ToTable("EmployeeOnLeaveMaster");

            entity.Property(e => e.TotalDayInYear)
                .HasComment("Số ngày phép trong năm")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalDayNoOnLeave)
                .HasComment("Số ngày nghỉ không phép")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalDayOnLeave)
                .HasComment("Số ngày đã nghỉ phép ")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalDayRemain)
                .HasComment("Số ngày phép còn lại")
                .HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<EmployeeOvertime>(entity =>
        {
            entity.ToTable("EmployeeOvertime");

            entity.Property(e => e.CostOvernight).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CostOvertime).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateApprovedBGD).HasColumnType("datetime");
            entity.Property(e => e.DateRegister).HasColumnType("datetime");
            entity.Property(e => e.DecilineApprove).HasComment("2: Không đồng ý duyệt; 1: Có đồng ý duyệt");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.Location).HasComment("1:Văn phòng; 2;Địa điểm công tác");
            entity.Property(e => e.TimeReality).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TimeStart).HasColumnType("datetime");
            entity.Property(e => e.TotalTime)
                .HasComment(" = Ratio * TimeReality")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeOvertimeProjectItem>(entity =>
        {
            entity.ToTable("EmployeeOvertimeProjectItem");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeePOContact>(entity =>
        {
            entity.ToTable("EmployeePOContact");

            entity.Property(e => e.Email).HasMaxLength(500);
            entity.Property(e => e.Phone)
                .HasMaxLength(520)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EmployeePayroll>(entity =>
        {
            entity.ToTable("EmployeePayroll");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(500);
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeePayrollBonusDeuction>(entity =>
        {
            entity.ToTable("EmployeePayrollBonusDeuction");

            entity.Property(e => e.BHXH)
                .HasComment("Mức đóng bảo hiểm xã hội")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Insurances)
                .HasComment("Mức thu BHXH")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.KPIBonus)
                .HasComment("Thưởng  KPIs / doanh số")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OtherBonus)
                .HasComment("Thưởng khác")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OtherDeduction)
                .HasComment("Khoản trừ khác")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ParkingMoney)
                .HasComment("Gửi xe ô tô")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Punish5S)
                .HasComment("Phạt 5s")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SalaryAdvance)
                .HasComment("Tạm ứng lương")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalWorkDay)
                .HasComment("Tổng ngày công được hưởng")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

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

        modelBuilder.Entity<EmployeeProjectType>(entity =>
        {
            entity.ToTable("EmployeeProjectType");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeePurchase>(entity =>
        {
            entity.ToTable("EmployeePurchase");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(550);
            entity.Property(e => e.Telephone).HasMaxLength(150);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeRegisterWork>(entity =>
        {
            entity.ToTable("EmployeeRegisterWork");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateValue).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeSalaryAdvance>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_SalaryAdvance");

            entity.ToTable("EmployeeSalaryAdvance");

            entity.Property(e => e.ApprovedHR).HasComment("Trưởng phòng nhân sự");
            entity.Property(e => e.ApprovedKT).HasComment("Trưởng phòng kế toán");
            entity.Property(e => e.ApprovedTP).HasComment("Trưởng phòng Kỹ thuật");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DatePayed).HasColumnType("datetime");
            entity.Property(e => e.DateRequest).HasColumnType("datetime");
            entity.Property(e => e.DecilineApprove).HasComment("2: Không đồng ý duyệt; 1: Có đồng ý duyệt");
            entity.Property(e => e.IsApproved_HR).HasComment("Trường phòng nhân sự đồng ý");
            entity.Property(e => e.IsApproved_KT).HasComment("Trưởng phòng kế toán đồng ý");
            entity.Property(e => e.IsApproved_TP).HasComment("trưởng phòng or người quản lý trực tiếp đồng ý");
            entity.Property(e => e.Money).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Note).HasMaxLength(550);
            entity.Property(e => e.Reason).HasMaxLength(250);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeScheduleWork>(entity =>
        {
            entity.ToTable("EmployeeScheduleWork");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateValue).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeSendEmail>(entity =>
        {
            entity.ToTable("EmployeeSendEmail");

            entity.Property(e => e.DateSend).HasColumnType("datetime");
            entity.Property(e => e.EmailCC).HasMaxLength(500);
            entity.Property(e => e.EmailTo).HasMaxLength(500);
            entity.Property(e => e.TableInfor)
                .HasMaxLength(250)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EmployeeSettingMoney>(entity =>
        {
            entity.ToTable("EmployeeSettingMoney");

            entity.Property(e => e.MoneyCTG).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.MoneyCTX).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.MoneyOT).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.MoneyQuaDem).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<EmployeeStatus>(entity =>
        {
            entity.ToTable("EmployeeStatus");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.StatusCode).HasMaxLength(50);
            entity.Property(e => e.StatusName).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeTeam>(entity =>
        {
            entity.ToTable("EmployeeTeam");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeTeamSale>(entity =>
        {
            entity.ToTable("EmployeeTeamSale");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeTeamSaleLink>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_EmployeeTeamSaleLink_1");

            entity.ToTable("EmployeeTeamSaleLink");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeTinhTrangHonNhan>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_TinhTrangHonNhan");

            entity.ToTable("EmployeeTinhTrangHonNhan");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeTypeBussiness>(entity =>
        {
            entity.ToTable("EmployeeTypeBussiness");

            entity.Property(e => e.Cost).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TypeCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TypeName).HasMaxLength(150);
        });

        modelBuilder.Entity<EmployeeTypeOvertime>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_EmployeeType");

            entity.ToTable("EmployeeTypeOvertime");

            entity.Property(e => e.Cost).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Ratio).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Type).HasMaxLength(150);
            entity.Property(e => e.TypeCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeVehicleBussiness>(entity =>
        {
            entity.ToTable("EmployeeVehicleBussiness");

            entity.Property(e => e.Cost).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.EditCost).HasComment("True: Cho phép người khai báo công tác sửa chi phí đi lại, False: Không cho phép");
            entity.Property(e => e.VehicleCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.VehicleName).HasMaxLength(100);
        });

        modelBuilder.Entity<EmployeeWFH>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_WFH");

            entity.ToTable("EmployeeWFH");

            entity.Property(e => e.ContentWork).HasMaxLength(550);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateApprovedBGD).HasColumnType("datetime");
            entity.Property(e => e.DateWFH).HasColumnType("datetime");
            entity.Property(e => e.DecilineApprove).HasComment("2: Không đồng ý duyệt; 1: Có đồng ý duyệt");
            entity.Property(e => e.EvaluateResults).HasMaxLength(550);
            entity.Property(e => e.Note).HasMaxLength(550);
            entity.Property(e => e.Reason).HasMaxLength(550);
            entity.Property(e => e.TimeWFH).HasComment("1: Buổi sáng; 2:Buổi chiều, 3: Cả ngày");
            entity.Property(e => e.TotalDay).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeWorkingProcess>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_WorkingProcess");

            entity.ToTable("EmployeeWorkingProcess");

            entity.Property(e => e.BasicSalary).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DecisionDay).HasColumnType("datetime");
            entity.Property(e => e.DecisionNumber).HasMaxLength(50);
            entity.Property(e => e.Diligence).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.Gasoline).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.House).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Insurance).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.Other).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Phone).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ProbationarySalary).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Rank).HasMaxLength(150);
            entity.Property(e => e.ShiftEat).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Skin).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Step).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ExamCategory>(entity =>
        {
            entity.ToTable("ExamCategory");

            entity.Property(e => e.CatCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CatName).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ExamListTest>(entity =>
        {
            entity.ToTable("ExamListTest");

            entity.Property(e => e.CodeTest).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.NameTest).HasMaxLength(50);
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ExamQuestion>(entity =>
        {
            entity.ToTable("ExamQuestion");

            entity.Property(e => e.CorrectAnswer)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Image)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ExamQuestionBank>(entity =>
        {
            entity.ToTable("ExamQuestionBank");

            entity.Property(e => e.CorrectAnswer)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Image)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ExamQuestionGroup>(entity =>
        {
            entity.ToTable("ExamQuestionGroup");

            entity.Property(e => e.GroupCode).HasMaxLength(50);
            entity.Property(e => e.GroupName).HasMaxLength(50);
        });

        modelBuilder.Entity<ExamQuestionListTest>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_QuestionOfExam");

            entity.ToTable("ExamQuestionListTest");
        });

        modelBuilder.Entity<ExamQuestionType>(entity =>
        {
            entity.ToTable("ExamQuestionType");

            entity.Property(e => e.ScoreRating).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TypeCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<ExamResult>(entity =>
        {
            entity.ToTable("ExamResult");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Season).HasComment("Quý");
            entity.Property(e => e.TestType).HasComment("1=Vision, 2=Điện, 3=PM, 4=Nội Quy");
            entity.Property(e => e.TotalMarks).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ExamResultAnswerDetail>(entity =>
        {
            entity.ToTable("ExamResultAnswerDetail");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(150)
                .IsFixedLength();
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(150)
                .IsFixedLength();
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ExamResultDetail>(entity =>
        {
            entity.ToTable("ExamResultDetail");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ExamTestResult>(entity =>
        {
            entity.ToTable("ExamTestResult");

            entity.Property(e => e.CandidateName).HasMaxLength(150);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ResultChose)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdateBy).HasMaxLength(150);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ExamTestResultMaster>(entity =>
        {
            entity.ToTable("ExamTestResultMaster");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.TotalMarks).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdateBy).HasMaxLength(150);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ExamTypeTest>(entity =>
        {
            entity.ToTable("ExamTypeTest");

            entity.Property(e => e.TypeCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<FcmToken>(entity =>
        {
            entity.Property(e => e.Token).IsUnicode(false);
        });

        modelBuilder.Entity<FilmManagement>(entity =>
        {
            entity.ToTable("FilmManagement");

            entity.Property(e => e.Code)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.RequestResult).HasComment("1: Có yêu cầu bắt buộc nhật kết quả; 0: Ko bắt buộc");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<FilmManagementDetail>(entity =>
        {
            entity.ToTable("FilmManagementDetail");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.PerformanceAVG).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitID).HasComment("Lấy từ UnitCount");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Fingerprint>(entity =>
        {
            entity.ToTable("Fingerprint");

            entity.Property(e => e.CheckIn).HasColumnType("datetime");
            entity.Property(e => e.CheckOut).HasColumnType("datetime");
            entity.Property(e => e.DayOfWeek).HasMaxLength(50);
            entity.Property(e => e.IDChamCong)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Note).HasMaxLength(550);
            entity.Property(e => e.Organization).HasMaxLength(250);
            entity.Property(e => e.SumEarly).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SumLate).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Firm>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_ProductFirm");

            entity.ToTable("Firm");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FirmCode).HasMaxLength(250);
            entity.Property(e => e.FirmName).HasMaxLength(250);
            entity.Property(e => e.FirmType).HasComment("1: Hãng kho Sale; 2: Hãng kho Demo");
            entity.Property(e => e.IsDelete).HasDefaultValue(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<FirmBase>(entity =>
        {
            entity.ToTable("FirmBase");

            entity.Property(e => e.FirmCode).HasMaxLength(250);
            entity.Property(e => e.FirmName).HasMaxLength(250);
        });

        modelBuilder.Entity<FollowProject>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_FollowProject_1");

            entity.ToTable("FollowProject");

            entity.Property(e => e.BankCharges).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.CustomFees).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CustomerQuotation).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Declaration).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Exchange).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NumberOfTransactions).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.POCode).HasMaxLength(200);
            entity.Property(e => e.Project).HasMaxLength(250);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Tax)
                .HasComment("Thuế VAT")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalBankCharges)
                .HasComment("Tổng phí ngân hàng")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalCostIncludingVAT)
                .HasComment("tổng chi phí bao gồm VAT")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalCostWithoutVAT)
                .HasComment("tổng chi phí không có VAT")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalCustomFees)
                .HasComment("tổng chi phí hải quan")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalCustomerQuotation)
                .HasComment("tổng báo giá khách hàng bảo gồm VAT")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalTransportFee)
                .HasComment("Tổng chi phí vận chuyển")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TransportFee).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<FollowProjectBase>(entity =>
        {
            entity.ToTable("FollowProjectBase");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateDonePM).HasColumnType("datetime");
            entity.Property(e => e.DateDoneSale).HasColumnType("datetime");
            entity.Property(e => e.DateWillDoPM).HasColumnType("datetime");
            entity.Property(e => e.DateWillDoSale).HasColumnType("datetime");
            entity.Property(e => e.ExpectedDate)
                .HasComment("Ngày dự kiến thực hiện")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpectedPODate)
                .HasComment("dk ngày PO")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpectedPlanDate)
                .HasComment("dự kiến ngày lên phương án")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpectedProjectEndDate)
                .HasComment("dk ngày kết thúc dự án")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpectedQuotationDate)
                .HasComment("dk ngày báo giá")
                .HasColumnType("datetime");
            entity.Property(e => e.FirmBaseID).HasComment("hãng");
            entity.Property(e => e.ImplementationDate)
                .HasComment("ngày thực hiện gần nhất")
                .HasColumnType("datetime");
            entity.Property(e => e.PossibilityPO).HasComment("khả năng có PO");
            entity.Property(e => e.ProjectContactName)
                .HasMaxLength(250)
                .HasComment("người phụ trách chính");
            entity.Property(e => e.ProjectStartDate)
                .HasComment("ngày bắt đầu dự án")
                .HasColumnType("datetime");
            entity.Property(e => e.ProjectStatusBaseID).HasComment("trạng thái dự án");
            entity.Property(e => e.ProjectTypeBaseID).HasComment("loại dự án");
            entity.Property(e => e.RealityPODate)
                .HasComment("tt ngày po")
                .HasColumnType("datetime");
            entity.Property(e => e.RealityPlanDate)
                .HasComment("thực tế ngày lên phương án")
                .HasColumnType("datetime");
            entity.Property(e => e.RealityProjectEndDate)
                .HasComment("tt ngày kết thúc dự án")
                .HasColumnType("datetime");
            entity.Property(e => e.RealityQuotationDate)
                .HasComment("thực tế ngày báo giá")
                .HasColumnType("datetime");
            entity.Property(e => e.TotalWithoutVAT)
                .HasComment("tổng giá chưa VAT")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.WorkDone).HasComment("việc đã làm");
            entity.Property(e => e.WorkWillDo).HasComment("việc sẽ làm");
        });

        modelBuilder.Entity<FollowProjectBaseDetail>(entity =>
        {
            entity.ToTable("FollowProjectBaseDetail");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ExpectedDate).HasColumnType("datetime");
            entity.Property(e => e.ImplementationDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<FollowProjectDetail>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_FollowProject");

            entity.ToTable("FollowProjectDetail");

            entity.Property(e => e.ArrivalDate)
                .HasComment("ngày hàng về")
                .HasColumnType("datetime");
            entity.Property(e => e.BankCharges)
                .HasComment("phí ngân hàng /1 tờ điện")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Bill).HasMaxLength(50);
            entity.Property(e => e.CostIncludingVATDetail).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CostWithoutVATDetail).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CustomFees)
                .HasComment("chí phí hải quan")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Debt).HasComment("Công nợ Nhà cung cấp");
            entity.Property(e => e.Declaration)
                .HasComment("Số tờ khai")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DeliveryRequestedDate)
                .HasComment("ngày yêu cầu giao hàng")
                .HasColumnType("datetime");
            entity.Property(e => e.Exchange).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImportTax)
                .HasComment("thuế nhập khẩu (%)")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImportTaxVND)
                .HasComment("Thuế nhập khẩu 1/pcs (vnd)")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.InsuranceFees)
                .HasComment("Chi phí bảo hiểm")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NewRow).HasDefaultValue(0);
            entity.Property(e => e.Note)
                .HasMaxLength(150)
                .HasComment("0:Pending;1:Finish; 2:Chưa giao đủ");
            entity.Property(e => e.NoteDelivery).HasMaxLength(450);
            entity.Property(e => e.NumberOfTransactions)
                .HasComment("Số lần điện")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OderDate)
                .HasComment("ngày đặt hàng")
                .HasColumnType("datetime");
            entity.Property(e => e.OldPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PODate)
                .HasComment("Ngày PO")
                .HasColumnType("datetime");
            entity.Property(e => e.PONo).HasMaxLength(150);
            entity.Property(e => e.Partner).HasMaxLength(50);
            entity.Property(e => e.PayDate)
                .HasComment("Ngày thanh toán")
                .HasColumnType("datetime");
            entity.Property(e => e.ProductID).HasComment("tên sản phẩm");
            entity.Property(e => e.Progress).HasComment("1: Đã nhận hàng, 2: đã nhận hóa đơn, , 3:đã thanh toán, 4: đã nhập kho");
            entity.Property(e => e.ProjectModel)
                .HasMaxLength(250)
                .HasComment("model dự án");
            entity.Property(e => e.Qty).HasComment("số lượng");
            entity.Property(e => e.QtyCustomer).HasComment("Số lượng khách hàng đặt");
            entity.Property(e => e.ShipmentDate)
                .HasComment("ngày ship hàng")
                .HasColumnType("datetime");
            entity.Property(e => e.StandardModel)
                .HasMaxLength(250)
                .HasComment("model chuẩn");
            entity.Property(e => e.Status).HasComment("1: Finish, 0: Pending");
            entity.Property(e => e.TaxDetail).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalBankCharges)
                .HasComment("Tổng chi phí ngân hàng")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalCustomfees)
                .HasComment("Tổng chi phí hải quan")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalImportTax)
                .HasComment("tổng thuế nhập khẩu")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPriceUSD).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPriceVND).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TransportFee)
                .HasComment("Phí vận chuyển")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPriceUSD).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPriceVND).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<FormAndFunction>(entity =>
        {
            entity.ToTable("FormAndFunction");

            entity.Property(e => e.Code).HasMaxLength(150);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.Name).HasMaxLength(300);
            entity.Property(e => e.ShortcutKey).HasMaxLength(20);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<FormAndFunctionGroup>(entity =>
        {
            entity.ToTable("FormAndFunctionGroup");

            entity.Property(e => e.Code).HasMaxLength(500);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(500);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Goal>(entity =>
        {
            entity.ToTable("Goal");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Goal0).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Goal1).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Goal2).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<GroupFile>(entity =>
        {
            entity.ToTable("GroupFile");

            entity.Property(e => e.GroupFileCode).HasMaxLength(250);
            entity.Property(e => e.GroupFileName).HasMaxLength(250);
        });

        modelBuilder.Entity<GroupProductSale>(entity =>
        {
            entity.ToTable("GroupProductSale");

            entity.Property(e => e.GroupName).HasMaxLength(150);
        });

        modelBuilder.Entity<GroupSale>(entity =>
        {
            entity.Property(e => e.GroupSalesCode).HasMaxLength(150);
            entity.Property(e => e.GroupSalesName).HasMaxLength(150);
            entity.Property(e => e.MainIndexID).HasMaxLength(150);
        });

        modelBuilder.Entity<GroupSalesUser>(entity =>
        {
            entity.ToTable("GroupSalesUser");

            entity.Property(e => e.Note).HasMaxLength(50);
        });

        modelBuilder.Entity<HandoverMinute>(entity =>
        {
            entity.Property(e => e.AdminWarehouseID).HasComment("thủ kho (Lấy từ ID Employee)");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CustomerAddress).HasMaxLength(550);
            entity.Property(e => e.CustomerContact).HasMaxLength(550);
            entity.Property(e => e.CustomerPhone)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DateMinutes).HasColumnType("datetime");
            entity.Property(e => e.Receiver)
                .HasMaxLength(550)
                .HasComment("Người nhận");
            entity.Property(e => e.ReceiverPhone)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<HandoverMinutesDetail>(entity =>
        {
            entity.ToTable("HandoverMinutesDetail");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DeliveryStatus).HasComment("1: đã nhận đủ; 2: thiếu");
            entity.Property(e => e.Guarantee).HasMaxLength(50);
            entity.Property(e => e.ProductStatus).HasComment("1: Hàng mới");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<HistoryDeleteBill>(entity =>
        {
            entity.ToTable("HistoryDeleteBill");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DeleteDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(550);
            entity.Property(e => e.TypeBill).HasMaxLength(350);
        });

        modelBuilder.Entity<HistoryError>(entity =>
        {
            entity.ToTable("HistoryError");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DescriptionError).HasColumnType("ntext");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<HistoryKPISale>(entity =>
        {
            entity.ToTable("HistoryKPISale");

            entity.Property(e => e.ACCP).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.ACCP0).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.ACCP1).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.ACCP2).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Goal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Goal0).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Goal1).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Goal2).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PercentIndex).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Result).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Result0).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Result1).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Result2).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<HistoryMoneyPO>(entity =>
        {
            entity.ToTable("HistoryMoneyPO");

            entity.Property(e => e.BankName).HasMaxLength(50);
            entity.Property(e => e.InvoiceNo).HasMaxLength(50);
            entity.Property(e => e.Money).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MoneyDate).HasColumnType("datetime");
            entity.Property(e => e.MoneyVAT).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Note).HasMaxLength(250);
            entity.Property(e => e.VAT).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<HistoryProductRTC>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__HistoryP__3214EC2786D54999");

            entity.ToTable("HistoryProductRTC");

            entity.HasIndex(e => e.BillExportTechnicalID, "HistoryProductRTC_BillExportTechnicalID_Index");

            entity.HasIndex(e => e.DateBorrow, "HistoryProductRTC_DateBorrow_Index");

            entity.HasIndex(e => e.IsDelete, "HistoryProductRTC_IsDelete_Index");

            entity.HasIndex(e => e.PeopleID, "HistoryProductRTC_PeopleID_Index");

            entity.HasIndex(e => e.ProductRTCID, "HistoryProductRTC_ProductRTCID_Index");

            entity.HasIndex(e => e.ProductRTCQRCodeID, "HistoryProductRTC_ProductRTCQRCodeID_Index");

            entity.HasIndex(e => e.Status, "HistoryProductRTC_Status_Index");

            entity.HasIndex(e => e.WarehouseID, "HistoryProductRTC_WarehouseID_Index");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateBorrow)
                .HasComment("Ngày mượn thiết bị")
                .HasColumnType("datetime");
            entity.Property(e => e.DateReturn)
                .HasComment("Ngày trả đồ")
                .HasColumnType("datetime");
            entity.Property(e => e.DateReturnExpected).HasColumnType("datetime");
            entity.Property(e => e.Note).HasComment("chú thích");
            entity.Property(e => e.NumberBorrow)
                .HasComment("Số lượng mượn")
                .HasColumnType("decimal(18, 0)");
            entity.Property(e => e.PeopleID).HasComment("ID người mượn");
            entity.Property(e => e.ProductRTCQRCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Project)
                .HasMaxLength(550)
                .HasComment("Dự án sử dụng thiết bị");
            entity.Property(e => e.Status).HasComment("0: Đã trả; 1: Đang mượn; 2: Thiết bị đã mất;3: Thiết bị hỏng;4: Đăng ký trả;5: Quá hạn;6: Sắp hết hạn;7: Đăng kí mượn; 8: Đăng ký gia hạn");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<HistoryProductRTCLog>(entity =>
        {
            entity.ToTable("HistoryProductRTCLog");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateReturnExpected).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Holiday>(entity =>
        {
            entity.ToTable("Holiday");

            entity.Property(e => e.DayValue)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.HolidayCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.HolidayName).HasMaxLength(150);
            entity.Property(e => e.Note).HasMaxLength(550);
            entity.Property(e => e.TypeHoliday).HasComment("1: Nghỉ có hưởng lương, 2: Nghỉ không hưởng lương");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.ToTable("Inventory");

            entity.HasIndex(e => e.IsStock, "Index_Inventory_IsStock");

            entity.HasIndex(e => e.ProductSaleID, "Index_Inventory_ProductSaleID");

            entity.HasIndex(e => e.WarehouseID, "Index_Inventory_WarehouseID");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Export).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Import).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MinQuantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalQuantityFirst).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalQuantityLast).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<InventoryDemo>(entity =>
        {
            entity.ToTable("InventoryDemo");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<InventoryProject>(entity =>
        {
            entity.ToTable("InventoryProject");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmployeeID).HasComment("Người giữ");
            entity.Property(e => e.Note).HasMaxLength(550);
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<InventoryProjectExport>(entity =>
        {
            entity.ToTable("InventoryProjectExport");

            entity.HasIndex(e => new { e.BillExportDetailID, e.IsDeleted }, "IX_InventoryProjectExport_BillExportDetailID_IsDeleted");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<InventoryStock>(entity =>
        {
            entity.ToTable("InventoryStock");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.ToTable("Invoice");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.InvoiceDate).HasColumnType("datetime");
            entity.Property(e => e.InvoiceNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<InvoiceLink>(entity =>
        {
            entity.ToTable("InvoiceLink");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<JobRequirement>(entity =>
        {
            entity.ToTable("JobRequirement");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateApprovedBGD).HasColumnType("datetime");
            entity.Property(e => e.DateApprovedHR).HasColumnType("datetime");
            entity.Property(e => e.DateApprovedTBP).HasColumnType("datetime");
            entity.Property(e => e.DateRequest).HasColumnType("datetime");
            entity.Property(e => e.DeadlineRequest).HasColumnType("datetime");
            entity.Property(e => e.NumberRequest)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<JobRequirementApproved>(entity =>
        {
            entity.ToTable("JobRequirementApproved");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateApproved).HasColumnType("datetime");
            entity.Property(e => e.IsApproved).HasComment("1:Duyệt,2:huỷ duyệt;");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<JobRequirementComment>(entity =>
        {
            entity.ToTable("JobRequirementComment");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateComment).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<JobRequirementDetail>(entity =>
        {
            entity.ToTable("JobRequirementDetail");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<JobRequirementFile>(entity =>
        {
            entity.ToTable("JobRequirementFile");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<JobRequirementLog>(entity =>
        {
            entity.ToTable("JobRequirementLog");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateLog).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<KPICriteriaDetail>(entity =>
        {
            entity.ToTable("KPICriteriaDetail");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<KPICriterion>(entity =>
        {
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CriteriaCode).HasMaxLength(150);
            entity.Property(e => e.CriteriaName).HasMaxLength(250);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<KPIDetail>(entity =>
        {
            entity.ToTable("KPIDetail");

            entity.Property(e => e.KPI).HasMaxLength(150);
            entity.Property(e => e.Note).HasMaxLength(150);
        });

        modelBuilder.Entity<KPIDetailUser>(entity =>
        {
            entity.ToTable("KPIDetailUser");

            entity.Property(e => e.PercentKPI).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<KPIEmployeePoint>(entity =>
        {
            entity.ToTable("KPIEmployeePoint");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.TotalPercent).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPercentActual).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<KPIEmployeePointDetail>(entity =>
        {
            entity.ToTable("KPIEmployeePointDetail");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FirstMonth).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PercentBonus).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PercentRemaining).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SecondMonth).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ThirdMonth).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<KPIEmployeeTeam>(entity =>
        {
            entity.ToTable("KPIEmployeeTeam");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsFixedLength();
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<KPIEmployeeTeamLink>(entity =>
        {
            entity.ToTable("KPIEmployeeTeamLink");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<KPIError>(entity =>
        {
            entity.ToTable("KPIError");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Monney).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Unit).HasComment("1: Lần");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<KPIErrorEmployee>(entity =>
        {
            entity.ToTable("KPIErrorEmployee");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ErrorDate).HasColumnType("datetime");
            entity.Property(e => e.TotalMoney).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<KPIErrorEmployeeFile>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("KPIErrorEmployeeFile");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(550);
            entity.Property(e => e.ID).ValueGeneratedOnAdd();
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<KPIErrorFineAmount>(entity =>
        {
            entity.ToTable("KPIErrorFineAmount");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.TotalMoneyError).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<KPIErrorType>(entity =>
        {
            entity.ToTable("KPIErrorType");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<KPIEvaluation>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_KPIEvaluationError");

            entity.ToTable("KPIEvaluation");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EvaluationCode).HasMaxLength(250);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<KPIEvaluationError>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_KPIErrorRuleDetails");

            entity.ToTable("KPIEvaluationError");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<KPIEvaluationFactor>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EvaluationType).HasComment("1: ĐÁNH GIÁ KỸ NĂNG\r\n, 2: CHUYÊN MÔN");
            entity.Property(e => e.KPIExamID).HasComment("Năm đánh giá");
            entity.Property(e => e.STT).HasMaxLength(250);
            entity.Property(e => e.SpecializationType).HasComment("1: Kỹ năng; 2: PLC, Robot; 3: VISION; 4: SOFTWARE");
            entity.Property(e => e.StandardPoint).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Unit).HasMaxLength(250);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.VerificationToolsContent).HasComment("Phương tiện xác minh tiêu chí");
        });

        modelBuilder.Entity<KPIEvaluationPoint>(entity =>
        {
            entity.ToTable("KPIEvaluationPoint");

            entity.Property(e => e.BGDCoefficient).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BGDEvaluation).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BGDPoint).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BGDPointInput)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmployeeCoefficient).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EmployeeEvaluation).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EmployeePoint).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status).HasComment("1: Hoàn thành; 0: Chưa hoàn thành");
            entity.Property(e => e.TBPCoefficient).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TBPEvaluation).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TBPPoint).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TBPPointInput)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<KPIEvaluationRule>(entity =>
        {
            entity.ToTable("KPIEvaluationRule");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.RuleCode).HasMaxLength(50);
            entity.Property(e => e.RuleName).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<KPIEvaluationRuleDetail>(entity =>
        {
            entity.ToTable("KPIEvaluationRuleDetail");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FormulaCode).HasMaxLength(250);
            entity.Property(e => e.MaxPercent).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MaxPercentageAdjustment).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PercentageAdjustment).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.STT).HasMaxLength(150);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<KPIExam>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_KPIExam_1");

            entity.ToTable("KPIExam");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Deadline).HasColumnType("datetime");
            entity.Property(e => e.ExamCode)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ExamName).HasMaxLength(250);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<KPIExamPosition>(entity =>
        {
            entity.ToTable("KPIExamPosition");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<KPIPosition>(entity =>
        {
            entity.ToTable("KPIPosition");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.PositionCode).HasMaxLength(250);
            entity.Property(e => e.PositionName).HasMaxLength(250);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<KPIPositionEmployee>(entity =>
        {
            entity.ToTable("KPIPositionEmployee");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<KPISession>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_KPIExam");

            entity.ToTable("KPISession");

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(250);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<KPISpecializationType>(entity =>
        {
            entity.ToTable("KPISpecializationType");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<KPISumarize>(entity =>
        {
            entity.ToTable("KPISumarize");

            entity.Property(e => e.AttitudeTowardsCustomers).HasComment("Tinh thần làm việc");
            entity.Property(e => e.FiveSRegulatedProcedures).HasComment("5S quy trình, quy định");
            entity.Property(e => e.LossEquipment).HasComment("Làm mất thiết bị");
            entity.Property(e => e.PLCExpertisePoints)
                .HasComment("Điểm chuyên môn PLC")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PrepareGoodsReport).HasComment("Chuẩn bị hàng và báo cáo công việc");
            entity.Property(e => e.SkillPoints)
                .HasComment("Điểm kỹ năng")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SoftwareExpertisePoints)
                .HasComment("Điểm chuyên môn Software")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TimeHours).HasComment("Thời gian, giờ giấc");
            entity.Property(e => e.VisionExpertisePoints)
                .HasComment("Điểm chuyên môn Vision")
                .HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<KPISumaryEvaluation>(entity =>
        {
            entity.ToTable("KPISumaryEvaluation");

            entity.Property(e => e.BGDPoint).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EmployeePoint).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TBPPoint).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<ListCost>(entity =>
        {
            entity.ToTable("ListCost");

            entity.Property(e => e.CostCode).HasMaxLength(250);
            entity.Property(e => e.CostName).HasMaxLength(250);
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.ToTable("Location", tb => tb.HasComment("bảng vị trí"));

            entity.Property(e => e.LocationCode)
                .HasMaxLength(250)
                .HasComment("mã vị trí");
            entity.Property(e => e.LocationName)
                .HasMaxLength(250)
                .HasComment("tên vị trí");
        });

        modelBuilder.Entity<MainIndex>(entity =>
        {
            entity.ToTable("MainIndex");

            entity.Property(e => e.ACCP).HasColumnType("decimal(18, 5)");
            entity.Property(e => e.ACCP0).HasColumnType("decimal(18, 5)");
            entity.Property(e => e.ACCP1).HasColumnType("decimal(18, 5)");
            entity.Property(e => e.ACCP2).HasColumnType("decimal(18, 5)");
            entity.Property(e => e.Goal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Goal0).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Goal1).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Goal2).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MainIndex1)
                .HasMaxLength(500)
                .HasColumnName("MainIndex");
            entity.Property(e => e.Result).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Result0).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Result1).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Result2).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.ToTable("Manufacturer");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ManufacturerCode).HasMaxLength(100);
            entity.Property(e => e.ManufacturerName).HasMaxLength(100);
            entity.Property(e => e.Note).HasMaxLength(250);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<MeetingMinute>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateEnd).HasColumnType("datetime");
            entity.Property(e => e.DateStart).HasColumnType("datetime");
            entity.Property(e => e.Place).HasMaxLength(550);
            entity.Property(e => e.Title).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<MeetingMinutesAttendance>(entity =>
        {
            entity.ToTable("MeetingMinutesAttendance");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CustomerName).HasMaxLength(250);
            entity.Property(e => e.EmailCustomer).HasMaxLength(250);
            entity.Property(e => e.FullName).HasMaxLength(550);
            entity.Property(e => e.PhoneNumber).HasMaxLength(250);
            entity.Property(e => e.Section).HasMaxLength(250);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<MeetingMinutesDetail>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CustomerName).HasMaxLength(250);
            entity.Property(e => e.PhoneNumber).HasMaxLength(250);
            entity.Property(e => e.PlanDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<MeetingMinutesFile>(entity =>
        {
            entity.ToTable("MeetingMinutesFile");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<MeetingType>(entity =>
        {
            entity.ToTable("MeetingType");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.GroupID).HasComment("1: Nội bộ; 2: Khách hàng");
            entity.Property(e => e.TypeCode).HasMaxLength(150);
            entity.Property(e => e.TypeName).HasMaxLength(250);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.ToTable("Menu");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ImageName).HasMaxLength(550);
            entity.Property(e => e.MenuName).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<MenuEmployeeLink>(entity =>
        {
            entity.ToTable("MenuEmployeeLink");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ModulaLocation>(entity =>
        {
            entity.ToTable("ModulaLocation");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Height).HasComment("Đơn vị chiều dài theo mm");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.Width).HasComment("Đơn vị chiều rộng theo mm");
        });

        modelBuilder.Entity<ModulaLocationDetail>(entity =>
        {
            entity.ToTable("ModulaLocationDetail");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Height).HasComment("Đơn vị chiều dài theo mm");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.Width).HasComment("Đơn vị chiều rộng theo mm");
        });

        modelBuilder.Entity<Notify>(entity =>
        {
            entity.ToTable("Notify");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DepartmentID).HasComment("Phòng ban nhận thông báo này");
            entity.Property(e => e.EmployeeID).HasComment("Người nhận thông báo này");
            entity.Property(e => e.NotifyStatus).HasComment("1:Chưa gửi;2:Đã gửi");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Number>(entity =>
        {
            entity.HasNoKey();
        });

        modelBuilder.Entity<OfficeSupply>(entity =>
        {
            entity.ToTable("OfficeSupply");

            entity.Property(e => e.CodeNCC).HasMaxLength(50);
            entity.Property(e => e.CodeRTC).HasMaxLength(50);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<OfficeSupplyRequest>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__OfficeSu__3214EC278960C21B");

            entity.ToTable("OfficeSupplyRequest");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateApproved).HasColumnType("datetime");
            entity.Property(e => e.DateRequest).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<OfficeSupplyRequest1>(entity =>
        {
            entity.ToTable("OfficeSupplyRequests");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateAdminApproved).HasColumnType("datetime");
            entity.Property(e => e.DateApproved).HasColumnType("datetime");
            entity.Property(e => e.DateRequest).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<OfficeSupplyRequestsDetail>(entity =>
        {
            entity.ToTable("OfficeSupplyRequestsDetail");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<OfficeSupplyUnit>(entity =>
        {
            entity.ToTable("OfficeSupplyUnit");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<OrganizationalChart>(entity =>
        {
            entity.ToTable("OrganizationalChart");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmployeeID).HasComment("ID leader");
            entity.Property(e => e.Name).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<OrganizationalChartDetail>(entity =>
        {
            entity.ToTable("OrganizationalChartDetail");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<POKH>(entity =>
        {
            entity.ToTable("POKH");

            entity.HasIndex(e => e.CurrencyID, "Index_POKH_CurrencyID");

            entity.HasIndex(e => e.CustomerID, "Index_POKH_CustomerID");

            entity.HasIndex(e => e.DealerID, "Index_POKH_DealerID");

            entity.HasIndex(e => e.EndUserID, "Index_POKH_EndUserID");

            entity.HasIndex(e => e.FollowProjectID, "Index_POKH_FollowProjectID");

            entity.HasIndex(e => e.GroupID, "Index_POKH_GroupID");

            entity.HasIndex(e => e.POCode, "Index_POKH_POCode");

            entity.HasIndex(e => e.PONumber, "Index_POKH_PONumber");

            entity.HasIndex(e => e.POType, "Index_POKH_POType");

            entity.HasIndex(e => e.PartID, "Index_POKH_PartID");

            entity.HasIndex(e => e.ProjectID, "Index_POKH_ProjectID");

            entity.HasIndex(e => e.QuotationID, "Index_POKH_QuotationID");

            entity.HasIndex(e => e.ReceivedDatePO, "Index_POKH_ReceivedDatePO");

            entity.HasIndex(e => e.Status, "Index_POKH_Status");

            entity.HasIndex(e => e.UserID, "Index_POKH_UserID");

            entity.HasIndex(e => e.WarehouseID, "Index_POKH_WarehouseID");

            entity.Property(e => e.BillCode)
                .HasMaxLength(250)
                .HasComment("mã hóa đơn");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DeliveryStatus).HasComment("tình trạng tiến độ giao hàng: 1 Chưa giao, 2 : Giao 1 phần, 3: Đã giao");
            entity.Property(e => e.EndDate)
                .HasComment("ngày kết thúc giao hàng")
                .HasColumnType("datetime");
            entity.Property(e => e.EndUser).HasMaxLength(550);
            entity.Property(e => e.ExportStatus).HasComment("tình trạng xuất kho");
            entity.Property(e => e.GroupID).HasMaxLength(150);
            entity.Property(e => e.ImportStatus).HasComment("tình trạng nhập kho");
            entity.Property(e => e.IsBill)
                .HasDefaultValue(false)
                .HasComment(" Tình trạng hoá đơn: 0: Chưa có hoá đơn, 1: Đã có hoá đơn");
            entity.Property(e => e.Note).HasMaxLength(250);
            entity.Property(e => e.POCode).HasMaxLength(250);
            entity.Property(e => e.PONumber).HasMaxLength(250);
            entity.Property(e => e.POType).HasComment("0: Prescale 1: PCB 2: VISION 3: Other");
            entity.Property(e => e.PaymentStatus).HasComment("Tình trạng thanh toán: 1: Chưa thanh toán, 2: Thanh toán 1 phần, 3: Đã thanh toán");
            entity.Property(e => e.ProjectID).HasComment("dự án");
            entity.Property(e => e.ReceiveMoney).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ReceivedDatePO).HasColumnType("datetime");
            entity.Property(e => e.StartDate)
                .HasComment("ngày bắt đầu")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasComment("0:Chưa giao , chưa thanh toán - 1:Đã giao, đã thanh toán -  2: Chưa giao,đã thanh toán - 3: Đã giao, nhưng thanh toán - 4:Đã thanh toán, GH chưa xuất hóa đơn");
            entity.Property(e => e.TotalMoneyKoVAT).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalMoneyPO)
                .HasComment("tổng tiền nhận PO")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserName)
                .HasMaxLength(250)
                .HasComment("người phụ trách");
        });

        modelBuilder.Entity<POKHDetail>(entity =>
        {
            entity.ToTable("POKHDetail");

            entity.HasIndex(e => e.CurrencyID, "Index_POKHDetail_CurrencyID");

            entity.HasIndex(e => e.KHID, "Index_POKHDetail_KHID");

            entity.HasIndex(e => e.POKHID, "Index_POKHDetail_POKHID");

            entity.HasIndex(e => e.ParentID, "Index_POKHDetail_ParentID");

            entity.HasIndex(e => e.ProductID, "Index_POKHDetail_ProductID");

            entity.HasIndex(e => e.QuotationDetailID, "Index_POKHDetail_QuotationDetailID");

            entity.Property(e => e.ActualDeliveryDate)
                .HasComment("Giao hàng thực tế")
                .HasColumnType("datetime");
            entity.Property(e => e.BillDate).HasColumnType("datetime");
            entity.Property(e => e.BillNumber).HasMaxLength(250);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DeliveryRequestedDate).HasColumnType("datetime");
            entity.Property(e => e.EstimatedPay).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.FilmSize).HasMaxLength(250);
            entity.Property(e => e.GroupPO).HasMaxLength(550);
            entity.Property(e => e.IndexPO).HasMaxLength(150);
            entity.Property(e => e.IntoMoney).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsOder).HasDefaultValue(false);
            entity.Property(e => e.NetUnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NewRow).HasDefaultValue(0);
            entity.Property(e => e.Note).HasMaxLength(250);
            entity.Property(e => e.PayDate).HasColumnType("datetime");
            entity.Property(e => e.RecivedMoneyDate).HasColumnType("datetime");
            entity.Property(e => e.TT).HasMaxLength(50);
            entity.Property(e => e.TotalPriceIncludeVAT).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserReceiver).HasMaxLength(500);
            entity.Property(e => e.VAT).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<POKHDetailMoney>(entity =>
        {
            entity.ToTable("POKHDetailMoney");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.MoneyUser).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PercentUser).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ReceiveMoney).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<POKHFile>(entity =>
        {
            entity.ToTable("POKHFile");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<POKHHistory>(entity =>
        {
            entity.ToTable("POKHHistory");

            entity.Property(e => e.BillDate).HasColumnType("datetime");
            entity.Property(e => e.BillNumber).HasMaxLength(250);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CustomerCode).HasMaxLength(250);
            entity.Property(e => e.DeliverDate).HasColumnType("datetime");
            entity.Property(e => e.IndexCode).HasMaxLength(250);
            entity.Property(e => e.Model).HasMaxLength(250);
            entity.Property(e => e.NetPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PODate).HasColumnType("datetime");
            entity.Property(e => e.PONumber).HasMaxLength(250);
            entity.Property(e => e.POTypeCode).HasMaxLength(250);
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.ProductCode).HasMaxLength(250);
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QuantityDeliver).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QuantityPending).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPriceVAT).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Unit).HasMaxLength(250);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.VAT).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<PONCC>(entity =>
        {
            entity.ToTable("PONCC");

            entity.HasIndex(e => e.EmployeeID, "Index_PONCC_EmployeeID");

            entity.HasIndex(e => e.Status, "Index_PONCC_Status");

            entity.HasIndex(e => e.SupplierSaleID, "Index_PONCC_SupplierSaleID");

            entity.HasIndex(e => e.RequestDate, "NonClusteredIndex-20240913-163239");

            entity.Property(e => e.AccountNumber)
                .HasMaxLength(50)
                .HasComment("Số tài khoản");
            entity.Property(e => e.AccountNumberSupplier).HasMaxLength(550);
            entity.Property(e => e.AddressDelivery).HasMaxLength(500);
            entity.Property(e => e.BankCharge).HasMaxLength(550);
            entity.Property(e => e.BankSupplier).HasMaxLength(550);
            entity.Property(e => e.BankingFee).HasMaxLength(500);
            entity.Property(e => e.BillCode)
                .HasMaxLength(250)
                .HasComment("mã hóa đơn");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CurrencyRate).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.DeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.DeliveryTime).HasComment("Số ngày giao hàng");
            entity.Property(e => e.Email).HasMaxLength(250);
            entity.Property(e => e.EmployeeID).HasComment("Nhân viên mua hàng");
            entity.Property(e => e.ExpectedDate).HasColumnType("datetime");
            entity.Property(e => e.FedexAccount).HasMaxLength(50);
            entity.Property(e => e.GroupID).HasMaxLength(150);
            entity.Property(e => e.IsApproved).HasComment("trạng thái duyệt");
            entity.Property(e => e.Note)
                .HasMaxLength(500)
                .HasComment("Ghi chú");
            entity.Property(e => e.OrderTargets).HasMaxLength(550);
            entity.Property(e => e.OriginItem).HasMaxLength(500);
            entity.Property(e => e.POCode)
                .HasMaxLength(250)
                .HasComment("mã PO");
            entity.Property(e => e.POType).HasComment("0: PO Thương mại; 1: PO mượn");
            entity.Property(e => e.Phone).HasMaxLength(150);
            entity.Property(e => e.ReceivedDatePO)
                .HasComment("ngày tạo PO")
                .HasColumnType("datetime");
            entity.Property(e => e.RequestDate)
                .HasComment("ngày yêu cầu giao hàng")
                .HasColumnType("datetime");
            entity.Property(e => e.RuleIncoterm).HasMaxLength(500);
            entity.Property(e => e.RulePay).HasMaxLength(500);
            entity.Property(e => e.ShippingPoint).HasMaxLength(550);
            entity.Property(e => e.Status).HasComment("Tình trạng đơn hàng(Đã giao ,Chưa giao,....)");
            entity.Property(e => e.SupplierVoucher).HasMaxLength(500);
            entity.Property(e => e.TotalMoneyPO)
                .HasComment("tổng tiền PO")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserNCC)
                .HasMaxLength(250)
                .HasComment("nhà cc");
            entity.Property(e => e.UserName)
                .HasMaxLength(250)
                .HasComment("người liên hệ");
        });

        modelBuilder.Entity<PONCCDetail>(entity =>
        {
            entity.ToTable("PONCCDetail");

            entity.HasIndex(e => e.PONCCID, "Index_PONCCDetail_PONCCID");

            entity.HasIndex(e => e.ProductRTCID, "Index_PONCCDetail_ProductRTCID");

            entity.HasIndex(e => e.ProductSaleID, "Index_PONCCDetail_ProductSaleID");

            entity.HasIndex(e => e.ProjectID, "Index_PONCCDetail_ProjectID");

            entity.HasIndex(e => e.ProjectPartListID, "Index_PONCCDetail_ProjectPartListID");

            entity.HasIndex(e => e.ProjectPartlistPurchaseRequestID, "Index_PONCCDetail_ProjectPartlistPurchaseRequestID");

            entity.Property(e => e.ActualDate)
                .HasComment("ngày giao hàng thực tế")
                .HasColumnType("datetime");
            entity.Property(e => e.BiddingPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CodeBill).HasMaxLength(250);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CurrencyExchange).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DateReturnEstimated).HasColumnType("datetime");
            entity.Property(e => e.DeadlineDelivery).HasColumnType("datetime");
            entity.Property(e => e.Discount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountPercent).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ExpectedDate).HasColumnType("datetime");
            entity.Property(e => e.FeeShip).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IntoMoney).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NameBill).HasMaxLength(250);
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.OrderDate)
                .HasComment("ngày yêu cầu giao hàng")
                .HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PriceHistory).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PriceSale).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductCodeOfSupplier).HasMaxLength(500);
            entity.Property(e => e.ProductType).HasComment("0: Phi mậu dịch; 1: Hàng thương mại; 2: Tạp nhập tái xuất");
            entity.Property(e => e.ProfitRate).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QtyReal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QtyRequest).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RequestDate)
                .HasComment("ngày yêu cầu giao hàng")
                .HasColumnType("datetime");
            entity.Property(e => e.Soluongcon).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status).HasComment("(0,chưa hoàn thành,1 hoàn thành)");
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.VAT)
                .HasComment("thuế giá trị gia tăng (%)")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.VATMoney).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<PONCCDetailLog>(entity =>
        {
            entity.ToTable("PONCCDetailLog");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateLog).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PONCCDetailRequestBuy>(entity =>
        {
            entity.ToTable("PONCCDetailRequestBuy");

            entity.HasIndex(e => e.ProjectPartlistPurchaseRequestID, "idx_ProjectPartlistPurchaseRequestID_PONCCDetailRequestBuy");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PONCCHistory>(entity =>
        {
            entity.ToTable("PONCCHistory");

            entity.Property(e => e.BiddingPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BillCode).IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CurrencyRate).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DeadlineDelivery).HasColumnType("datetime");
            entity.Property(e => e.DeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.FeeShip).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MinQuantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.POCode).IsUnicode(false);
            entity.Property(e => e.PriceHistory).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PriceSale).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductNewCode).IsUnicode(false);
            entity.Property(e => e.QtyRequest).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QuantityRemain).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QuantityReturn).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RequestDate).HasColumnType("datetime");
            entity.Property(e => e.TotalMoneyChangePO).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalQuantityLast).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPriceVAT).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.VAT).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<PONCCRulePay>(entity =>
        {
            entity.ToTable("PONCCRulePay");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Part>(entity =>
        {
            entity.ToTable("Part");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasComment("Có được sử dụng hay không");
            entity.Property(e => e.ManufacturerID).HasComment("Thuộc hãng nào");
            entity.Property(e => e.PartCode)
                .HasMaxLength(150)
                .HasComment("Mã vật tư thiết bị theo hãng cung cấp");
            entity.Property(e => e.PartCodeRTC)
                .HasMaxLength(150)
                .HasComment("Mã vật tư thiết bị theo RTC");
            entity.Property(e => e.PartGroupID).HasComment("Thuộc nhóm vật tư nào");
            entity.Property(e => e.PartName)
                .HasMaxLength(250)
                .HasComment("Tên vật tư thiết bị");
            entity.Property(e => e.PartNameRTC).HasMaxLength(250);
            entity.Property(e => e.Price)
                .HasComment("Giá gần nhất")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status).HasComment("Trạng thái vật tư: 1: đang sử dụng,0: ngừng sử dụng");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PartGroup>(entity =>
        {
            entity.ToTable("PartGroup");

            entity.Property(e => e.Note).HasMaxLength(250);
            entity.Property(e => e.PartGroupCode).HasMaxLength(50);
            entity.Property(e => e.PartGroupName).HasMaxLength(250);
        });

        modelBuilder.Entity<PartSummaryDetail>(entity =>
        {
            entity.ToTable("PartSummaryDetail");

            entity.Property(e => e.AskPriceID).HasComment("Người hỏi giá");
            entity.Property(e => e.BankCost)
                .HasComment("Chi phí ngân hàng")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.BuyPersonID).HasComment("Người phụ trách mua");
            entity.Property(e => e.ContactEmail)
                .HasMaxLength(150)
                .HasComment("Email người liên hệ nhà cung cấp");
            entity.Property(e => e.ContactName)
                .HasMaxLength(150)
                .HasComment("Tên người liên hệ nhà cung cấp");
            entity.Property(e => e.ContactPhone)
                .HasMaxLength(150)
                .HasComment("Điện thoại người liên hệ nhà cung cấp");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CurrencyUnit)
                .HasMaxLength(50)
                .HasComment("Đơn vị tiền tệ");
            entity.Property(e => e.CustomsCost)
                .HasComment("Tổng tiền chi phí hải quan")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.DateReplyPrice)
                .HasComment("Thời gian hỏi xong giá")
                .HasColumnType("datetime");
            entity.Property(e => e.DateRequestPrice)
                .HasComment("Ngày yêu cầu hỏi giá từ sale")
                .HasColumnType("datetime");
            entity.Property(e => e.DeliveryCost)
                .HasComment("Chi phí bốc dỡ vận chuyển")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.FinishTotalPrice)
                .HasComment("Tổng tiền cuối cùng kết thúc hỏi giá")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.ManufacturerID).HasComment("ID hãng sản xuất");
            entity.Property(e => e.MaxDay)
                .HasComment("Ngày hàng về dự kiến lâu nhất lúc hỏi giá")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.MinDay)
                .HasComment("Ngày hàng về dự kiến nhanh nhất lúc hỏi giá")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.POCustomerID).HasComment("PO khách hàng");
            entity.Property(e => e.POSupplierID).HasComment("PO nhà cung cấp");
            entity.Property(e => e.ParentID).HasComment("Vật tư thiết bị cha");
            entity.Property(e => e.PartCode)
                .HasMaxLength(50)
                .HasComment("Mã vật tư thiết bị");
            entity.Property(e => e.PartCodeRTC)
                .HasMaxLength(50)
                .HasComment("Mã vật tư hàng hóa theo RTC");
            entity.Property(e => e.PartName)
                .HasMaxLength(150)
                .HasComment("Tên vật tư thiết bị");
            entity.Property(e => e.PartNameRTC)
                .HasMaxLength(150)
                .HasComment("Tên vật tư hàng hóa theo RTC");
            entity.Property(e => e.PeriodExpected)
                .HasMaxLength(150)
                .HasComment("Khoảng thời gian hàng về dự kiến lúc hỏi giá");
            entity.Property(e => e.Price)
                .HasComment("Giá vật tư tiền việt")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.PriceCurrency)
                .HasComment("Đơn giá vật tư theo ngoại tệ")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.PriceQuotation).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.PriceVAT)
                .HasComment("Đơn giá sau VAT")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.ProjectID).HasComment("Dự án");
            entity.Property(e => e.ProjectModuleID).HasComment("Thuộc thiết bị sản xuất nào trong dự án");
            entity.Property(e => e.Qty)
                .HasComment("Số lượng")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.QuotationID).HasComment("Báo giá");
            entity.Property(e => e.RequestID).HasComment("ID yêu cầu hỏi giá");
            entity.Property(e => e.SaleID).HasComment("Sale phụ trách");
            entity.Property(e => e.SupplierID).HasComment("Nhà cung cấp");
            entity.Property(e => e.TotalPrice)
                .HasComment("Tổng tiền")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalPriceCurrency)
                .HasComment("Tổng tiền ngoại tệ")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalPriceQuotation).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalPriceVAT)
                .HasComment("Tổng tiền sau VAT")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasComment("Đơn vị tính");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.VAT)
                .HasComment("vat")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.VATCurrencyCost)
                .HasComment("Tổng tiền chi phí nhập khẩu")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.VATCurrencyPercent)
                .HasComment("Thuế nhập khẩu (%)")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.VATCurrencyPrice)
                .HasComment("Chi phí thuể nhập khẩu trên một con vật tư")
                .HasColumnType("decimal(18, 4)");
        });

        modelBuilder.Entity<PaymentOrder>(entity =>
        {
            entity.ToTable("PaymentOrder");

            entity.HasIndex(e => e.Code, "Index_PaymentOrder_Code");

            entity.HasIndex(e => e.CustomerID, "Index_PaymentOrder_CustomerID");

            entity.HasIndex(e => e.DateOrder, "Index_PaymentOrder_DateOrder");

            entity.HasIndex(e => e.EmployeeID, "Index_PaymentOrder_EmployeeID");

            entity.HasIndex(e => e.IsDelete, "Index_PaymentOrder_IsDelete");

            entity.HasIndex(e => e.IsSpecialOrder, "Index_PaymentOrder_IsSpecialOrder");

            entity.HasIndex(e => e.PONCCID, "Index_PaymentOrder_PONCCID");

            entity.HasIndex(e => e.PaymentOrderTypeID, "Index_PaymentOrder_PaymentOrderTypeID");

            entity.HasIndex(e => e.ProjectID, "Index_PaymentOrder_ProjectID");

            entity.HasIndex(e => e.SupplierSaleID, "Index_PaymentOrder_SupplierSaleID");

            entity.HasIndex(e => e.TypeOrder, "Index_PaymentOrder_TypeOrder");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateOrder)
                .HasComment("Ngày làm đề nghị")
                .HasColumnType("datetime");
            entity.Property(e => e.DatePayment)
                .HasComment("Ngày quyết toán đối với đề nghị tạm ứng")
                .HasColumnType("datetime");
            entity.Property(e => e.DeadlinePayment).HasColumnType("datetime");
            entity.Property(e => e.TotalMoney).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TypeBankTransfer).HasComment("1:Chuyển khoản RTC; 2:Chuyển khoản MVI;3:Chuyển khoản APR;4:Chuyển khoản Yonko;5:Chuyển khoản cá nhân");
            entity.Property(e => e.TypeOrder).HasComment("Loại đề nghị (1:Đề nghị tạm ứng; 2:Đề nghị thanh toán/quyết toán)");
            entity.Property(e => e.TypePayment).HasComment("Loại thanh toán(1:Chuyển khoản; 2:Tiền mặt)");
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PaymentOrderCustomer>(entity =>
        {
            entity.ToTable("PaymentOrderCustomer");

            entity.HasIndex(e => e.CustomerID, "Index_PaymentOrderCustomer_CustomerID");

            entity.HasIndex(e => e.PaymentOrderID, "Index_PaymentOrderCustomer_PaymentOrderID");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PaymentOrderDetail>(entity =>
        {
            entity.ToTable("PaymentOrderDetail");

            entity.HasIndex(e => e.EmployeeID, "Index_PaymentOrderDetail_EmployeeID");

            entity.HasIndex(e => e.ParentID, "Index_PaymentOrderDetail_ParentID");

            entity.HasIndex(e => e.PaymentMethods, "Index_PaymentOrderDetail_PaymentMethods");

            entity.HasIndex(e => e.PaymentOrderID, "Index_PaymentOrderDetail_PaymentOrderID");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentPercentage).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.STT)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TotalMoney).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPaymentAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PaymentOrderDetailUserTeamSale>(entity =>
        {
            entity.ToTable("PaymentOrderDetailUserTeamSale");

            entity.HasIndex(e => e.PaymentOrderDetailID, "Index_PaymentOrderDetailUserTeamSale_PaymentOrderDetailID");

            entity.HasIndex(e => e.UserTeamSaleID, "Index_PaymentOrderDetailUserTeamSale_UserTeamSaleID");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PaymentOrderFile>(entity =>
        {
            entity.ToTable("PaymentOrderFile");

            entity.HasIndex(e => e.PaymentOrderID, "Index_PaymentOrderFile_PaymentOrderID");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PaymentOrderFileBankSlip>(entity =>
        {
            entity.ToTable("PaymentOrderFileBankSlip");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PaymentOrderLog>(entity =>
        {
            entity.ToTable("PaymentOrderLog");

            entity.HasIndex(e => e.DateApproved, "Index_PaymentOrderLog_DateApproved");

            entity.HasIndex(e => e.EmployeeApproveActualID, "Index_PaymentOrderLog_EmployeeApproveActualID");

            entity.HasIndex(e => e.EmployeeID, "Index_PaymentOrderLog_EmployeeID");

            entity.HasIndex(e => e.IsApproved, "Index_PaymentOrderLog_IsApproved");

            entity.HasIndex(e => e.Step, "Index_PaymentOrderLog_Step");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateApproved).HasColumnType("datetime");
            entity.Property(e => e.EmployeeApproveActualID).HasComment("Người duyệt thực tế");
            entity.Property(e => e.EmployeeID).HasComment("Người được chỉ định duyệt");
            entity.Property(e => e.IsApproved).HasComment("0:Chờ duyêt; 1:Đã duyệt; 2:Không duyệt");
            entity.Property(e => e.IsRequestAppendFileAC).HasComment("Kế toán yc bổ sung (1: Yc bổ sung file)");
            entity.Property(e => e.IsRequestAppendFileHR).HasComment("HR yc bổ sung (1: Yc bổ sung file)");
            entity.Property(e => e.ReasonRequestAppendFileAC)
                .HasMaxLength(550)
                .HasComment("Lý do Kế toán yc bổ sung file");
            entity.Property(e => e.ReasonRequestAppendFileHR)
                .HasMaxLength(550)
                .HasComment("Lý do HR yc bổ sung file");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PaymentOrderOrderType>(entity =>
        {
            entity.ToTable("PaymentOrderOrderType");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PaymentOrderType>(entity =>
        {
            entity.ToTable("PaymentOrderType");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.TypeCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TypeName).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PaymentOrderTypeDocument>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_PaymentOderTypeDocument");

            entity.ToTable("PaymentOrderTypeDocument");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.TypeDocumentID).HasComment("1: PO, 2: Hóa đơn");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PaymentOrderTypeOrder>(entity =>
        {
            entity.ToTable("PaymentOrderTypeOrder");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.TypeOrderID).HasComment("1: Đề nghị tạm ứng, 2: Đề nghị thanh toán/quyết toán");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PercentMainIndexUser>(entity =>
        {
            entity.ToTable("PercentMainIndexUser");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.PercentIndex).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PriceCheck>(entity =>
        {
            entity.ToTable("PriceCheck");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.LeadTime).HasMaxLength(50);
            entity.Property(e => e.Note).HasMaxLength(550);
            entity.Property(e => e.ProductCode).HasMaxLength(550);
            entity.Property(e => e.ProductName).HasMaxLength(250);
            entity.Property(e => e.Qty).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBY).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProductFilm>(entity =>
        {
            entity.ToTable("ProductFilm");

            entity.Property(e => e.Area).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Height).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.InventoryNumber).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.PcsPerBox).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Width).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<ProductGroup>(entity =>
        {
            entity.ToTable("ProductGroup");

            entity.Property(e => e.ProductGroupID).HasMaxLength(50);
            entity.Property(e => e.ProductGroupName).HasMaxLength(250);
        });

        modelBuilder.Entity<ProductGroupRTC>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__ProductG__3213E83F5C0DC233");

            entity.ToTable("ProductGroupRTC");

            entity.Property(e => e.ProductGroupName)
                .HasMaxLength(150)
                .HasComment("Tên nhóm thiết bị");
            entity.Property(e => e.ProductGroupNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("Mã nhóm thiết bị");
        });

        modelBuilder.Entity<ProductGroupWarehouse>(entity =>
        {
            entity.ToTable("ProductGroupWarehouse");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProductKhachHang>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_ProductKhachHang1");

            entity.ToTable("ProductKhachHang");

            entity.Property(e => e.DiaChi).HasMaxLength(500);
            entity.Property(e => e.MaKhachHang).HasMaxLength(255);
            entity.Property(e => e.TenKhachHang).HasMaxLength(500);
            entity.Property(e => e.TenKiHieu).HasMaxLength(255);
            entity.Property(e => e.Type).HasMaxLength(255);
        });

        modelBuilder.Entity<ProductLocation>(entity =>
        {
            entity.ToTable("ProductLocation");

            entity.Property(e => e.CoordinatesX).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CoordinatesY).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.LocationCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LocationName).HasMaxLength(250);
            entity.Property(e => e.LocationType).HasComment("1: Tủ mũ & quần áo; 2: Tủ giày");
            entity.Property(e => e.OldLocationName).HasMaxLength(250);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProductRTC>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__CAMERABA__3214EC27D0C3C825");

            entity.ToTable("ProductRTC");

            entity.HasIndex(e => e.AddressBox, "ProductRTC_AddressBox_Index");

            entity.HasIndex(e => e.NumberInStore, "ProductRTC_NumberInStore_Index");

            entity.HasIndex(e => e.PartNumber, "ProductRTC_PartNumber_Index");

            entity.HasIndex(e => e.ProductCodeRTC, "ProductRTC_ProductCodeRTC_Index");

            entity.HasIndex(e => e.ProductCode, "ProductRTC_ProductCode_Index");

            entity.HasIndex(e => e.ProductGroupRTCID, "ProductRTC_ProductGroupRTCID_Index");

            entity.HasIndex(e => e.ProductLocationID, "ProductRTC_ProductLocationID_Index");

            entity.HasIndex(e => e.SerialNumber, "ProductRTC_SerialNumber_Index");

            entity.HasIndex(e => e.Serial, "ProductRTC_Serial_Index");

            entity.HasIndex(e => e.ProductCode, "ProductRTC_idx_ProductCode");

            entity.HasIndex(e => e.FirmID, "idx_FirmID_ProductRTC");

            entity.HasIndex(e => e.IsDelete, "idx_IsDelete_ProductRTC");

            entity.HasIndex(e => e.UnitCountID, "idx_UnitCountID_ProductRTC");

            entity.Property(e => e.AddressBox).HasMaxLength(150);
            entity.Property(e => e.CodeHCM)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CurrentIntensityMax).HasMaxLength(150);
            entity.Property(e => e.DataInterface).HasMaxLength(150);
            entity.Property(e => e.FNo).HasMaxLength(150);
            entity.Property(e => e.InputValue).HasMaxLength(150);
            entity.Property(e => e.LampColor).HasMaxLength(150);
            entity.Property(e => e.LampPower).HasMaxLength(150);
            entity.Property(e => e.LampType).HasMaxLength(150);
            entity.Property(e => e.LampWattage).HasMaxLength(150);
            entity.Property(e => e.LensMount).HasMaxLength(150);
            entity.Property(e => e.LocationImg).HasMaxLength(150);
            entity.Property(e => e.MOD).HasMaxLength(150);
            entity.Property(e => e.Maker)
                .HasMaxLength(150)
                .HasComment("Hãng");
            entity.Property(e => e.MonoColor).HasMaxLength(150);
            entity.Property(e => e.Number)
                .HasComment("Số lượng tổng")
                .HasColumnType("decimal(18, 0)");
            entity.Property(e => e.NumberInStore).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.OutputValue).HasMaxLength(150);
            entity.Property(e => e.PartNumber).HasMaxLength(150);
            entity.Property(e => e.PixelSize).HasMaxLength(150);
            entity.Property(e => e.ProductCode)
                .HasMaxLength(150)
                .HasComment("Mã thiết bị");
            entity.Property(e => e.ProductCodeRTC).HasMaxLength(150);
            entity.Property(e => e.ProductName)
                .HasMaxLength(550)
                .HasComment("Tên thiết bị");
            entity.Property(e => e.Resolution).HasMaxLength(150);
            entity.Property(e => e.SensorSize).HasMaxLength(150);
            entity.Property(e => e.SensorSizeMax).HasMaxLength(150);
            entity.Property(e => e.Serial).HasMaxLength(150);
            entity.Property(e => e.SerialNumber).HasMaxLength(150);
            entity.Property(e => e.ShutterMode).HasMaxLength(150);
            entity.Property(e => e.Size).HasMaxLength(150);
            entity.Property(e => e.Status).HasComment("1: Đang giặt");
            entity.Property(e => e.StatusProduct).HasComment("Trạng thái sản phẩm (1: hiện có, 0: không có)");
            entity.Property(e => e.WD).HasMaxLength(150);
        });

        modelBuilder.Entity<ProductRTCQRCode>(entity =>
        {
            entity.ToTable("ProductRTCQRCode");

            entity.HasIndex(e => e.ProductQRCode, "ProductRTCQRCode_ProductQRCode_Index");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.PartNumber).HasMaxLength(150);
            entity.Property(e => e.ProductQRCode)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Serial).HasMaxLength(150);
            entity.Property(e => e.SerialNumber).HasMaxLength(150);
            entity.Property(e => e.Status).HasComment("1:Trong Kho,2:Đang mượn,3:Đã Xuất Kho");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProductSale>(entity =>
        {
            entity.ToTable("ProductSale");

            entity.HasIndex(e => e.AddressBox, "Index_ProductSale_AddressBox");

            entity.HasIndex(e => e.LocationID, "Index_ProductSale_LocationID");

            entity.HasIndex(e => e.Maker, "Index_ProductSale_Maker");

            entity.HasIndex(e => e.ProductCode, "Index_ProductSale_ProductCode");

            entity.HasIndex(e => e.ProductGroupID, "Index_ProductSale_ProductGroupID");

            entity.HasIndex(e => e.ProductNewCode, "Index_ProductSale_ProductNewCode");

            entity.Property(e => e.AddressBox).HasMaxLength(150);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Export).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.FirmID).HasDefaultValue(0);
            entity.Property(e => e.Import).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.ItemType).HasMaxLength(50);
            entity.Property(e => e.Maker).HasMaxLength(50);
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.NumberInStoreCuoiKy).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.NumberInStoreDauky).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.ProductCode).HasMaxLength(200);
            entity.Property(e => e.ProductNewCode).HasMaxLength(250);
            entity.Property(e => e.SupplierName).HasMaxLength(500);
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProductWorking>(entity =>
        {
            entity.ToTable("ProductWorking");

            entity.Property(e => e.CheckValueType).HasComment("1: Kiểm tra theo kiểu dữ liệu số, 2: kiểm tra theo kiểu dữ liệu ký tự");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.MaxValue)
                .HasComment("Giá trị tiêu chuẩn lớn nhất")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.MinValue)
                .HasComment("Giá trị tiêu chuẩn nhỏ nhất")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.PeriodValue).HasComment("Giá trị tiêu chuẩn theo khoảng lấy lên từ min, max value");
            entity.Property(e => e.Port).HasMaxLength(50);
            entity.Property(e => e.ProductStepCode).HasMaxLength(50);
            entity.Property(e => e.ReasonChange).HasMaxLength(250);
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasComment("Đơn vị tính");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.ValueType).HasComment("Kiểu kiểm tra 0:check mark, 1: Điền giá trị");
            entity.Property(e => e.ValueTypeName).HasMaxLength(150);
            entity.Property(e => e.WorkingName).HasMaxLength(150);
        });

        modelBuilder.Entity<ProductWorkingAudit>(entity =>
        {
            entity.ToTable("ProductWorkingAudit");

            entity.Property(e => e.CheckValueType).HasComment("1: Kiểm tra theo kiểu dữ liệu số, 2: kiểm tra theo kiểu dữ liệu ký tự");
            entity.Property(e => e.CheckValueTypeNew).HasComment("1: Kiểm tra theo kiểu dữ liệu số, 2: kiểm tra theo kiểu dữ liệu ký tự");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.MaxValue)
                .HasComment("Giá trị tiêu chuẩn lớn nhất")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.MaxValueNew)
                .HasComment("Giá trị tiêu chuẩn lớn nhất")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.MinValue)
                .HasComment("Giá trị tiêu chuẩn nhỏ nhất")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.MinValueNew).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.PeriodValue).HasComment("Giá trị tiêu chuẩn theo khoảng lấy lên từ min, max value");
            entity.Property(e => e.PeriodValueNew).HasComment("Giá trị tiêu chuẩn theo khoảng lấy lên từ min, max value");
            entity.Property(e => e.ProductStepCode).HasMaxLength(50);
            entity.Property(e => e.ProductStepCodeNew).HasMaxLength(50);
            entity.Property(e => e.ReasonChange).HasMaxLength(250);
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasComment("Đơn vị tính");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserApproved).HasMaxLength(50);
            entity.Property(e => e.ValueType).HasComment("Kiểu kiểm tra 0:check mark, 1: Điền giá trị");
            entity.Property(e => e.ValueTypeName).HasMaxLength(150);
            entity.Property(e => e.ValueTypeNew).HasComment("Kiểu kiểm tra 0:check mark, 1: Điền giá trị");
            entity.Property(e => e.WorkingName).HasMaxLength(150);
            entity.Property(e => e.WorkingNameNew).HasMaxLength(150);
        });

        modelBuilder.Entity<ProductivityIndex>(entity =>
        {
            entity.ToTable("ProductivityIndex");

            entity.Property(e => e.CallDSKHBigAccount).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.CallDSKHOther).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.DemoTest).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.NewAccount).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.NumberCompanyBuy).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.NumberPO).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.NumberQuatation).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.POAmountPCB).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.POAmountPrescaleMinorAcc).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.POBigAccountFilm).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.POBigAccountVisionID).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.POOther).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.POVisionIDMinorAcc).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.SaleBigAccountVisionID).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.SaleOther).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.SalePCB).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.SalePrescaleBigAccount).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.SaleVisionID).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.SalesMinorAccPrescale).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.TotalPOAmount).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.TotalPerformance).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.VisitCustomerBigAccount).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.VisitCustomerOther).HasColumnType("decimal(18, 1)");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("Project");

            entity.HasIndex(e => e.BusinessFieldID, "Index_Project_BusinessFieldID");

            entity.HasIndex(e => e.ContactID, "Index_Project_ContactID");

            entity.HasIndex(e => e.CreatedDate, "Index_Project_CreatedDate");

            entity.HasIndex(e => e.CustomerID, "Index_Project_CustomerID");

            entity.HasIndex(e => e.EndUser, "Index_Project_EndUser");

            entity.HasIndex(e => e.ProjectManager, "Index_Project_ProjectManager");

            entity.HasIndex(e => e.ProjectType, "Index_Project_ProjectType");

            entity.HasIndex(e => e.UserID, "Index_Project_UserID");

            entity.HasIndex(e => e.UserTechnicalID, "Index_Project_UserTechnicalID");

            entity.Property(e => e.ActualDateEnd).HasColumnType("datetime");
            entity.Property(e => e.ActualDateStart).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CustomerID).HasComment("Khách hàng");
            entity.Property(e => e.EU).HasMaxLength(255);
            entity.Property(e => e.EndUser).HasComment("Link id khách hàng");
            entity.Property(e => e.Note).HasComment("Ghi chú");
            entity.Property(e => e.PO).HasMaxLength(150);
            entity.Property(e => e.PODate).HasColumnType("datetime");
            entity.Property(e => e.PlanDateEnd).HasColumnType("datetime");
            entity.Property(e => e.PlanDateStart).HasColumnType("datetime");
            entity.Property(e => e.Priotity)
                .HasComment("Mức độ ưu tiên")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProjectCode)
                .HasMaxLength(50)
                .HasComment("Mã dự án");
            entity.Property(e => e.ProjectName)
                .HasMaxLength(200)
                .HasComment("Tên dự án");
            entity.Property(e => e.ProjectShortName).HasMaxLength(50);
            entity.Property(e => e.ProjectStatus).HasComment("0: Chưa hoàn thành, 1: Hoàn thành");
            entity.Property(e => e.TypeProject).HasComment("1: Dự án; 2: Thương mại; 3:Phim");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserID).HasComment("Người phụ trách chính");
        });

        modelBuilder.Entity<ProjectCost>(entity =>
        {
            entity.ToTable("ProjectCost");

            entity.Property(e => e.Money).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<ProjectCurrentSituation>(entity =>
        {
            entity.ToTable("ProjectCurrentSituation");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateSituation).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectDetail>(entity =>
        {
            entity.ToTable("ProjectDetail");

            entity.Property(e => e.FileName).HasMaxLength(250);
            entity.Property(e => e.Note).HasMaxLength(550);
            entity.Property(e => e.PathFull).HasMaxLength(250);
            entity.Property(e => e.PathShort).HasMaxLength(250);
        });

        modelBuilder.Entity<ProjectEmployee>(entity =>
        {
            entity.ToTable("ProjectEmployee");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ReceiverID).HasComment("Người nhận bàn giao");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectFile>(entity =>
        {
            entity.ToTable("ProjectFile");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FileType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("Loại file theo đuôi mở rộng");
            entity.Property(e => e.FileTypeFolder).HasComment("loại file theo thư mục (1: Video;2: Image)");
            entity.Property(e => e.Size)
                .HasComment("Kích thước file tính bằng bytes")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectHistoryProblem>(entity =>
        {
            entity.ToTable("ProjectHistoryProblem");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateImplementation).HasColumnType("datetime");
            entity.Property(e => e.DateProblem).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectHistoryProblemDetail>(entity =>
        {
            entity.ToTable("ProjectHistoryProblemDetail");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectItem>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__ProjectI__3214EC273B316A27");

            entity.ToTable("ProjectItem");

            entity.Property(e => e.ActualEndDate).HasColumnType("datetime");
            entity.Property(e => e.ActualStartDate).HasColumnType("datetime");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmployeeIDRequest).HasComment("Người giao công việc");
            entity.Property(e => e.EmployeeRequestID).HasComment("lưu ID người yêu cầu lấy từ bảng Employee, nếu  = 0 thì là tên KH");
            entity.Property(e => e.EmployeeRequestName)
                .HasMaxLength(550)
                .HasComment("lưu tên người yêu cầu lấy từ bảng Employee, nếu  = 0 thì là tên KH");
            entity.Property(e => e.IsApproved).HasComment("0: Chờ duyệt kế hoạch; 1:Leader duyệt kế hoạch; 2:Chờ duyệt thực tế; 3:Leader Duyệt thực tế");
            entity.Property(e => e.ItemLate).HasComment("1:Hạng mục quá hạn,\r\n0: Hạng mục đúng hạn");
            entity.Property(e => e.PercentItem).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.PercentageActual).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PlanEndDate).HasColumnType("datetime");
            entity.Property(e => e.PlanStartDate).HasColumnType("datetime");
            entity.Property(e => e.STT)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TimeSpan).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.TotalDayActual).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalDayPlan).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDateActual)
                .HasComment("Ngày update kết thúc thực tế")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedDateReasonLate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectItemDetail>(entity =>
        {
            entity.ToTable("ProjectItemDetail");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectItemFile>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__ProjectI__3214EC27EAD30F8F");

            entity.Property(e => e.CreatedBy).HasMaxLength(30);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(200);
            entity.Property(e => e.UpdatedBy).HasMaxLength(30);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectItemLog>(entity =>
        {
            entity.ToTable("ProjectItemLog");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateEnd).HasColumnType("datetime");
            entity.Property(e => e.DateStart).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectItemProblem>(entity =>
        {
            entity.ToTable("ProjectItemProblem");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectMachinePrice>(entity =>
        {
            entity.ToTable("ProjectMachinePrice");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(150)
                .HasComment("Người tạo");
            entity.Property(e => e.CreatedDate)
                .HasComment("Ngày tạo")
                .HasColumnType("datetime");
            entity.Property(e => e.DatePrice)
                .HasComment("Ngày thanh toán")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(150)
                .HasComment("Người update");
            entity.Property(e => e.UpdatedDate)
                .HasComment("Ngày update")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectMachinePriceDetail>(entity =>
        {
            entity.ToTable("ProjectMachinePriceDetail");

            entity.Property(e => e.AmountSpent)
                .HasComment("Số tiền chi")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CodeGroup)
                .HasMaxLength(550)
                .IsUnicode(false);
            entity.Property(e => e.Coefficient)
                .HasComment("Hệ số")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ContentPrice)
                .HasMaxLength(550)
                .HasComment("Nội dung");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DependentObject)
                .HasMaxLength(550)
                .HasComment("Đối tượng phụ trách");
            entity.Property(e => e.EstimateCost)
                .HasComment("Chi phí dự toán")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NameGroup).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectPartList>(entity =>
        {
            entity.ToTable("ProjectPartList");

            entity.HasIndex(e => e.EmployeeID, "Index_ProjectPartList_EmployeeID");

            entity.HasIndex(e => e.IsApprovedPurchase, "Index_ProjectPartList_IsApprovedPurchase");

            entity.HasIndex(e => e.IsApprovedTBP, "Index_ProjectPartList_IsApprovedTBP");

            entity.HasIndex(e => e.IsProblem, "Index_ProjectPartList_IsProblem");

            entity.HasIndex(e => e.ParentID, "Index_ProjectPartList_ParentID");

            entity.HasIndex(e => e.ProductCode, "Index_ProjectPartList_ProductCode");

            entity.HasIndex(e => e.ProjectID, "Index_ProjectPartList_ProjectID");

            entity.HasIndex(e => e.ProjectPartListTypeID, "Index_ProjectPartList_ProjectPartListTypeID");

            entity.HasIndex(e => e.ProjectTypeID, "Index_ProjectPartList_ProjectTypeID");

            entity.HasIndex(e => e.Status, "Index_ProjectPartList_Status");

            entity.HasIndex(e => e.StatusPriceRequest, "Index_ProjectPartList_StatusPriceRequest");

            entity.HasIndex(e => e.SupplierSaleID, "Index_ProjectPartList_SupplierSaleID");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateApprovedNewCode).HasColumnType("datetime");
            entity.Property(e => e.DatePriceQuote).HasColumnType("datetime");
            entity.Property(e => e.DatePriceRequest).HasColumnType("datetime");
            entity.Property(e => e.DeadlinePriceRequest).HasColumnType("datetime");
            entity.Property(e => e.ExpectedReturnDate).HasColumnType("datetime");
            entity.Property(e => e.LeadTime).HasMaxLength(100);
            entity.Property(e => e.NCCFinal).HasMaxLength(750);
            entity.Property(e => e.OrderCode)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PriceOrder).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductCode)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.QtyFull).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QtyMin).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QtyReturned).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QuantityReturn).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RequestDate).HasColumnType("datetime");
            entity.Property(e => e.ReturnDate).HasColumnType("datetime");
            entity.Property(e => e.SpecialCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TT)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TotalPriceOrder).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.VAT).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<ProjectPartListType>(entity =>
        {
            entity.ToTable("ProjectPartListType");

            entity.Property(e => e.Code)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(150);
        });

        modelBuilder.Entity<ProjectPartListVersion>(entity =>
        {
            entity.ToTable("ProjectPartListVersion");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ReasonDeleted).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectPartlistPriceRequest>(entity =>
        {
            entity.ToTable("ProjectPartlistPriceRequest");

            entity.HasIndex(e => e.IsDeleted, "idx_IsDeleted_ProjectPartlistPriceRequest");

            entity.HasIndex(e => e.ProjectPartListID, "idx_ProjectPartListID_ProjectPartlistPriceRequest");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CurrencyRate).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DateExpected).HasColumnType("datetime");
            entity.Property(e => e.DatePriceQuote).HasColumnType("datetime");
            entity.Property(e => e.DateRequest).HasColumnType("datetime");
            entity.Property(e => e.Deadline).HasColumnType("datetime");
            entity.Property(e => e.HistoryPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LeadTime).HasMaxLength(100);
            entity.Property(e => e.Maker).HasMaxLength(150);
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.StatusRequest).HasComment("1:Yêu cầu báo giá; 2:Đã báo giá");
            entity.Property(e => e.TotaMoneyVAT).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalImportPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPriceExchange).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.UnitFactoryExportPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitImportPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.VAT).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<ProjectPartlistPriceRequestHistory>(entity =>
        {
            entity.ToTable("ProjectPartlistPriceRequestHistory");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CurrencyRate).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DatePriceQuote).HasColumnType("datetime");
            entity.Property(e => e.HistoryPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LeadTime).HasMaxLength(100);
            entity.Property(e => e.TotalImportPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPriceExchange).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.UnitFactoryExportPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitImportPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectPartlistPurchaseRequest>(entity =>
        {
            entity.ToTable("ProjectPartlistPurchaseRequest");

            entity.HasIndex(e => e.CreatedDate, "Index_ProjectPartlistPurchaseRequest_CreatedDate");

            entity.HasIndex(e => e.EmployeeID, "Index_ProjectPartlistPurchaseRequest_EmployeeID");

            entity.HasIndex(e => e.IsApprovedBGD, "Index_ProjectPartlistPurchaseRequest_IsApprovedBGD");

            entity.HasIndex(e => e.IsApprovedTBP, "Index_ProjectPartlistPurchaseRequest_IsApprovedTBP");

            entity.HasIndex(e => e.IsCommercialProduct, "Index_ProjectPartlistPurchaseRequest_IsCommercialProduct");

            entity.HasIndex(e => e.IsRequestApproved, "Index_ProjectPartlistPurchaseRequest_IsRequestApproved");

            entity.HasIndex(e => e.JobRequirementID, "Index_ProjectPartlistPurchaseRequest_JobRequirementID");

            entity.HasIndex(e => e.POKHDetailID, "Index_ProjectPartlistPurchaseRequest_POKHDetailID");

            entity.HasIndex(e => e.ProjectPartListID, "Index_ProjectPartlistPurchaseRequest_ProjectPartListID");

            entity.HasIndex(e => e.SupplierSaleID, "Index_ProjectPartlistPurchaseRequest_SupplierSaleID");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CurrencyRate).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DateApprovedBGD).HasColumnType("datetime");
            entity.Property(e => e.DateApprovedTBP).HasColumnType("datetime");
            entity.Property(e => e.DateEstimate)
                .HasComment("Ngày dự kiến hàng về")
                .HasColumnType("datetime");
            entity.Property(e => e.DateOrder).HasColumnType("datetime");
            entity.Property(e => e.DateReceive).HasColumnType("datetime");
            entity.Property(e => e.DateRequest).HasColumnType("datetime");
            entity.Property(e => e.DateReturnActual)
                .HasComment("Ngày hàng về thực tế")
                .HasColumnType("datetime");
            entity.Property(e => e.DateReturnEstimated).HasColumnType("datetime");
            entity.Property(e => e.DateReturnExpected)
                .HasComment("Ngày hàng về mong đợi (Deadline)")
                .HasColumnType("datetime");
            entity.Property(e => e.HistoryPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LeadTime).HasMaxLength(100);
            entity.Property(e => e.Maker).HasMaxLength(250);
            entity.Property(e => e.NoteHR).HasMaxLength(550);
            entity.Property(e => e.OriginQuantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.StatusRequest).HasComment("1:Y/c mua hàng;2:Huỷ Y/c mua; 3: Đã đặt hàng; 4: Đang về; 5:Đã về; 6:Không đặt hàng");
            entity.Property(e => e.TargetPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TicketType).HasComment("0: yêu cầu mua; 1: Yêu cầu mượn");
            entity.Property(e => e.TotaMoneyVAT).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalImportPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPriceExchange).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitFactoryExportPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitImportPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitMoney).HasMaxLength(50);
            entity.Property(e => e.UnitName).HasMaxLength(150);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.VAT).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<ProjectPartlistPurchaseRequestNote>(entity =>
        {
            entity.ToTable("ProjectPartlistPurchaseRequestNote");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectPersonalPriotity>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ProjectPersonalPriotity");

            entity.Property(e => e.ID).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<ProjectPriority>(entity =>
        {
            entity.ToTable("ProjectPriority");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Priority).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Rate).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectPriorityLink>(entity =>
        {
            entity.ToTable("ProjectPriorityLink");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectRequest>(entity =>
        {
            entity.ToTable("ProjectRequest");

            entity.Property(e => e.CodeRequest)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateRequest).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectRequestFile>(entity =>
        {
            entity.ToTable("ProjectRequestFile");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Extension)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FileName).HasMaxLength(550);
            entity.Property(e => e.FileNameOrigin).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectSolution>(entity =>
        {
            entity.ToTable("ProjectSolution");

            entity.Property(e => e.CodeSolution)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateSolution).HasColumnType("datetime");
            entity.Property(e => e.PriceReportDeadline).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectSolutionFile>(entity =>
        {
            entity.ToTable("ProjectSolutionFile");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Extension)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FileNameOrigin).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectStatus>(entity =>
        {
            entity.ToTable("ProjectStatus");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.StatusName).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectStatusBase>(entity =>
        {
            entity.ToTable("ProjectStatusBase");

            entity.Property(e => e.ProjectStatusCode).HasMaxLength(250);
            entity.Property(e => e.ProjectStatusName).HasMaxLength(250);
        });

        modelBuilder.Entity<ProjectStatusDetail>(entity =>
        {
            entity.ToTable("ProjectStatusDetail");

            entity.Property(e => e.ActualEndDate).HasColumnType("datetime");
            entity.Property(e => e.ActualStartDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EstimatedEndDate)
                .HasComment("Ngày kết thúc dự kiến")
                .HasColumnType("datetime");
            entity.Property(e => e.EstimatedStartDate)
                .HasComment("Ngày bắt đầu dự kiến")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectStatusLog>(entity =>
        {
            entity.ToTable("ProjectStatusLog");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateLog).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectSurvey>(entity =>
        {
            entity.ToTable("ProjectSurvey");

            entity.Property(e => e.Address).HasMaxLength(550);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateEnd).HasColumnType("datetime");
            entity.Property(e => e.DateStart).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(550);
            entity.Property(e => e.EmployeeID).HasComment("Nhân viên Sale");
            entity.Property(e => e.Note).HasMaxLength(550);
            entity.Property(e => e.PIC).HasMaxLength(550);
            entity.Property(e => e.PhoneNumber).HasMaxLength(150);
            entity.Property(e => e.ReasonUrgent).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectSurveyDetail>(entity =>
        {
            entity.ToTable("ProjectSurveyDetail");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateSurvey).HasColumnType("datetime");
            entity.Property(e => e.EmployeeID).HasComment("Nhân viên kỹ thuật khảo sát");
            entity.Property(e => e.Note).HasMaxLength(550);
            entity.Property(e => e.ReasonCancel).HasMaxLength(550);
            entity.Property(e => e.Status).HasComment("1: Duyệt; 2: Hủy duyệt");
            entity.Property(e => e.SurveySession).HasComment("Buổi khảo sát (1: Buổi sáng; 2: Buổi chiều)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectSurveyFile>(entity =>
        {
            entity.ToTable("ProjectSurveyFile");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectTreeFolder>(entity =>
        {
            entity.ToTable("ProjectTreeFolder");

            entity.Property(e => e.FolderName).HasMaxLength(50);
        });

        modelBuilder.Entity<ProjectType>(entity =>
        {
            entity.ToTable("ProjectType");

            entity.Property(e => e.ProjectTypeCode).HasMaxLength(250);
            entity.Property(e => e.ProjectTypeName).HasMaxLength(250);
            entity.Property(e => e.RootFolder).HasMaxLength(550);
        });

        modelBuilder.Entity<ProjectTypeAssign>(entity =>
        {
            entity.ToTable("ProjectTypeAssign");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectTypeBase>(entity =>
        {
            entity.ToTable("ProjectTypeBase");

            entity.Property(e => e.ProjectTypeCode).HasMaxLength(250);
            entity.Property(e => e.ProjectTypeName).HasMaxLength(250);
        });

        modelBuilder.Entity<ProjectTypeDetail>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__ProjectT__3214EC27C74705FF");

            entity.ToTable("ProjectTypeDetail");

            entity.Property(e => e.ProjectTypeNameChild).HasMaxLength(100);
        });

        modelBuilder.Entity<ProjectTypeLink>(entity =>
        {
            entity.ToTable("ProjectTypeLink");

            entity.HasIndex(e => e.LeaderID, "Index_ProjectTypeLink_LeaderID");

            entity.HasIndex(e => e.ProjectID, "Index_ProjectTypeLink_ProjectID");

            entity.HasIndex(e => e.ProjectTypeID, "Index_ProjectTypeLink_ProjectTypeID");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectUser>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__ProjectU__3214EC2781C639C1");

            entity.ToTable("ProjectUser");
        });

        modelBuilder.Entity<ProjectWorker>(entity =>
        {
            entity.ToTable("ProjectWorker");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.NumberOfDay).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TT)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalWorkforce).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectWorkerType>(entity =>
        {
            entity.ToTable("ProjectWorkerType");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(150);
        });

        modelBuilder.Entity<ProjectWorkerVersion>(entity =>
        {
            entity.ToTable("ProjectWorkerVersion");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Province>(entity =>
        {
            entity.ToTable("Province");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ProvinceCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ProvinceName).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.ToTable("PurchaseOrder");

            entity.Property(e => e.ApprovedDate).HasColumnType("datetime");
            entity.Property(e => e.BuyPersonID).HasComment("Người phụ trách mua");
            entity.Property(e => e.ContactEmail)
                .HasMaxLength(250)
                .HasComment("Địa chỉ email người liên hệ nhà cung cấp");
            entity.Property(e => e.ContactName)
                .HasMaxLength(150)
                .HasComment("Tên người liên hệ nhà cung cấp");
            entity.Property(e => e.ContactPhone)
                .HasMaxLength(100)
                .HasComment("Điện thoại người liên hệ nhà cung cấp");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateAndDelivery).HasComment("Ngày giao hàng và vận chuyển");
            entity.Property(e => e.FinishPrice)
                .HasComment("Tổng tiền PO có VAT")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.PODate)
                .HasComment("Ngày phát sinh PO")
                .HasColumnType("datetime");
            entity.Property(e => e.Payment).HasComment("Quy tắc thanh toán");
            entity.Property(e => e.PeriodExpected)
                .HasMaxLength(150)
                .HasComment("Khoảng thời gian hàng về dự kiến");
            entity.Property(e => e.PurchaseOrderCode)
                .HasMaxLength(150)
                .HasComment("Số PO");
            entity.Property(e => e.SupplierID).HasComment("Nhà cung cấp");
            entity.Property(e => e.TotalPrice)
                .HasComment("Tổng tiền PO chưa VAT")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalVAT)
                .HasComment("Tổng tiền VAT")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PurchaseOrderDetail>(entity =>
        {
            entity.ToTable("PurchaseOrderDetail");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DeliveryCost).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.FinishPrice).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.ManufacturerCode).HasMaxLength(50);
            entity.Property(e => e.PartCode).HasMaxLength(150);
            entity.Property(e => e.PartCodeRTC).HasMaxLength(150);
            entity.Property(e => e.PartName).HasMaxLength(250);
            entity.Property(e => e.PartNameRTC).HasMaxLength(250);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Qty).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.VAT).HasColumnType("decimal(18, 4)");
        });

        modelBuilder.Entity<Quotation>(entity =>
        {
            entity.ToTable("Quotation", tb => tb.HasComment("Báo giá"));

            entity.Property(e => e.Adjusting).HasMaxLength(250);
            entity.Property(e => e.BankCost)
                .HasComment("Chi phí ngân hàng")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.ContactEmail)
                .HasMaxLength(250)
                .HasComment("Địa chỉ email người liên hệ");
            entity.Property(e => e.ContactName)
                .HasMaxLength(150)
                .HasComment("Tên người liên hệ");
            entity.Property(e => e.ContactPhone)
                .HasMaxLength(100)
                .HasComment("Điện thoại người liên hệ");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CustomClearance).HasMaxLength(250);
            entity.Property(e => e.CustomerID).HasComment("Khách hàng");
            entity.Property(e => e.CustomsCost)
                .HasComment("Tổng tiền chi phí hải quan")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.DeliveryCost)
                .HasComment("Chi phí bốc dỡ vận chuyển")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.DeliveryFees).HasMaxLength(250);
            entity.Property(e => e.DeliveryPeriod).HasMaxLength(150);
            entity.Property(e => e.IsApproved)
                .HasDefaultValue(false)
                .HasComment("Đã được duyệt hay chưa");
            entity.Property(e => e.IsDelete).HasDefaultValue(false);
            entity.Property(e => e.Note).HasMaxLength(250);
            entity.Property(e => e.POCode).HasMaxLength(50);
            entity.Property(e => e.Payment).HasMaxLength(250);
            entity.Property(e => e.PlaceDelivery).HasMaxLength(250);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PricePS)
                .HasComment("Đơn giá / 1set")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PricePSVAT)
                .HasComment("Đơn giá / 1set")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PriceVAT).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PriceVT).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Qty).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QtySet)
                .HasComment("Số lượng set")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QuotationCode)
                .HasMaxLength(50)
                .HasComment("Số báo giá");
            entity.Property(e => e.QuotationDate)
                .HasComment("Ngày báo giá")
                .HasColumnType("datetime");
            entity.Property(e => e.QuotationStatus).HasComment("0: Chưa duyệt,1:Đã duyệt ,2:Đã báo khách,3:Pending,4:Đã có PO");
            entity.Property(e => e.QuotationType).HasComment("0: báo giá bình thường, 1: Báo giá hàng nhập khẩu(cognex)");
            entity.Property(e => e.Rate).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SaleID).HasComment("Nhân viên sale phụ trách");
            entity.Property(e => e.TotalPrice)
                .HasComment("Tổng tiền báo giá")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPriceVAT)
                .HasComment("Tổng tiền báo giá")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalVT).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.VAT).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Validity).HasMaxLength(250);
            entity.Property(e => e.Warranty).HasMaxLength(250);
        });

        modelBuilder.Entity<QuotationDetail>(entity =>
        {
            entity.ToTable("QuotationDetail", tb => tb.HasComment("Báo giá chi tiết"));

            entity.Property(e => e.BankCost)
                .HasComment("Chi phí ngân hàng")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.ContactEmail)
                .HasMaxLength(150)
                .HasComment("Email người liên hệ");
            entity.Property(e => e.ContactName)
                .HasMaxLength(150)
                .HasComment("Tên người liên hệ");
            entity.Property(e => e.ContactPhone)
                .HasMaxLength(150)
                .HasComment("Điện thoại người liên hệ");
            entity.Property(e => e.ContactWebsite).HasComment("Website liên hệ");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CurrencyRate).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.CurrencyUnit)
                .HasMaxLength(50)
                .HasComment("Đơn vị tiền tệ");
            entity.Property(e => e.CustomsCost)
                .HasComment("Tổng tiền chi phí hải quan")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Delivery).HasMaxLength(250);
            entity.Property(e => e.DeliveryCost)
                .HasComment("Chi phí bốc dỡ vận chuyển")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.FinishPrice)
                .HasComment("Đơn giá sau chi phí")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.FinishTotalPrice)
                .HasComment("Tổng tiền cuối cùng sau chi phí")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.PartCode).HasMaxLength(150);
            entity.Property(e => e.PartCodeRTC).HasMaxLength(150);
            entity.Property(e => e.PartName).HasMaxLength(250);
            entity.Property(e => e.PartNameRTC).HasMaxLength(250);
            entity.Property(e => e.PeriodExpected)
                .HasMaxLength(150)
                .HasComment("Khoảng thời gian hàng về dự kiến");
            entity.Property(e => e.Price)
                .HasComment("Đơn giá báo khách")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PriceCurrency)
                .HasComment("Đơn giá vật tư theo ngoại tệ")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.PriceOld)
                .HasComment("Đơn giá báo cũ")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PricePS).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.PriceVAT)
                .HasComment("Tiền vat")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.PriceVT).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Qty)
                .HasComment("Tổng số lượng")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QtyPS)
                .HasComment("Số lượng / 1 set")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QtySet)
                .HasComment("Số set")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.STT).HasMaxLength(10);
            entity.Property(e => e.SupplierID).HasComment("ID nhà cung cấp");
            entity.Property(e => e.TaxImporPrice)
                .HasComment("Chi phí thuể nhập khẩu trên một con vật tư")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TaxImporTotal)
                .HasComment("Tổng tiền chi phí nhập khẩu")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TaxImportPercent)
                .HasComment("Thuế nhập khẩu (%)")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalPrice)
                .HasComment("Tổng tiền báo khách")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPriceCurrency)
                .HasComment("Tổng tiền ngoại tệ")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalVAT)
                .HasComment("Tổng tiền VAT")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalVT).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.VAT)
                .HasComment("vat")
                .HasColumnType("decimal(18, 4)");
        });

        modelBuilder.Entity<QuotationKH>(entity =>
        {
            entity.ToTable("QuotationKH");

            entity.Property(e => e.ComMoney).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Commission).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Company).HasMaxLength(50);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Explanation).HasMaxLength(150);
            entity.Property(e => e.POCode).HasMaxLength(150);
            entity.Property(e => e.QuotationCode).HasMaxLength(150);
            entity.Property(e => e.QuotationDate).HasColumnType("datetime");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserID).HasComment("ID tài khoản đăng nhập");
            entity.Property(e => e.UserName).HasComment("Người phụ trách");
            entity.Property(e => e.Version).HasMaxLength(150);
        });

        modelBuilder.Entity<QuotationKHDetail>(entity =>
        {
            entity.ToTable("QuotationKHDetail");

            entity.Property(e => e.GiaNet).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Image).HasMaxLength(150);
            entity.Property(e => e.InternalCode).HasMaxLength(150);
            entity.Property(e => e.InternalName).HasMaxLength(150);
            entity.Property(e => e.IntoMoney).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsPO).HasDefaultValue(0);
            entity.Property(e => e.Maker).HasMaxLength(50);
            entity.Property(e => e.Note).HasMaxLength(150);
            entity.Property(e => e.ProductCode).HasMaxLength(250);
            entity.Property(e => e.ProductName).HasMaxLength(350);
            entity.Property(e => e.ProductNewCode).HasMaxLength(250);
            entity.Property(e => e.Qty).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPriceImport).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TypeOfPrice).HasMaxLength(50);
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPriceImport).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<QuotationNCC>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_QuoteNCC");

            entity.ToTable("QuotationNCC");

            entity.Property(e => e.ContactName).HasMaxLength(250);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.GroupID).HasMaxLength(150);
            entity.Property(e => e.Phone).HasMaxLength(250);
            entity.Property(e => e.QuoteCode).HasMaxLength(250);
            entity.Property(e => e.QuoteDate).HasColumnType("datetime");
            entity.Property(e => e.UserNCC).HasMaxLength(550);
            entity.Property(e => e.UserName).HasMaxLength(550);
        });

        modelBuilder.Entity<QuotationNCCDetail>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_QuoteNCCDetail");

            entity.ToTable("QuotationNCCDetail");

            entity.Property(e => e.IntendTime).HasMaxLength(250);
        });

        modelBuilder.Entity<QuotationTermLink>(entity =>
        {
            entity.ToTable("QuotationTermLink");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<RegisterContract>(entity =>
        {
            entity.ToTable("RegisterContract");

            entity.Property(e => e.ContractTypeID).HasComment("1: Sao y, 2: Gốc, 3: Treo");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateApproved).HasColumnType("datetime");
            entity.Property(e => e.FolderPath).HasMaxLength(550);
            entity.Property(e => e.RegistedDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasComment("0: Chưa nhận,1: Đã nhận, 2: Hủy");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<RegisterIdea>(entity =>
        {
            entity.ToTable("RegisterIdea");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateApproved).HasColumnType("datetime");
            entity.Property(e => e.DateApprovedTBP).HasColumnType("datetime");
            entity.Property(e => e.DateRegister).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<RegisterIdeaDetail>(entity =>
        {
            entity.ToTable("RegisterIdeaDetail");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateEnd).HasColumnType("datetime");
            entity.Property(e => e.DateStart).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<RegisterIdeaFile>(entity =>
        {
            entity.ToTable("RegisterIdeaFile");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<RegisterIdeaScore>(entity =>
        {
            entity.ToTable("RegisterIdeaScore");

            entity.Property(e => e.ApprovedID).HasComment("ID TBP phòng ban liên quan");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateApproved)
                .HasComment("Ngày TBP phòng ban liên quan duyệt/ chấm điểm")
                .HasColumnType("datetime");
            entity.Property(e => e.DateApprovedBGD).HasColumnType("datetime");
            entity.Property(e => e.DateApprovedTBP).HasColumnType("datetime");
            entity.Property(e => e.IsApproved).HasComment("TBP phòng ban liên quan duyệt");
            entity.Property(e => e.Score).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ScoreRating).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<RegisterIdeaType>(entity =>
        {
            entity.ToTable("RegisterIdeaType");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.RegisterTypeCode).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<RegisterOT>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Register__3214EC2730EAAE05");

            entity.ToTable("RegisterOT");

            entity.Property(e => e.Confirm).HasDefaultValue(false);
            entity.Property(e => e.CostON).HasDefaultValue(0.0);
            entity.Property(e => e.CostVehicle).HasDefaultValue(0.0);
            entity.Property(e => e.CostsOT).HasDefaultValue(0.0);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.NotCheckInOffice).HasDefaultValue(false);
            entity.Property(e => e.Overnight).HasDefaultValue(false);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.TypeGOB).HasDefaultValue(false);
            entity.Property(e => e.TypeOT).HasDefaultValue(false);
            entity.Property(e => e.Vehicle).HasDefaultValue(0);

            entity.HasOne(d => d.Customer).WithMany(p => p.RegisterOTs)
                .HasForeignKey(d => d.CustomerID)
                .HasConstraintName("FK__RegisterO__Custo__49C4C20E");

            entity.HasOne(d => d.UserOTNavigation).WithMany(p => p.RegisterOTs)
                .HasForeignKey(d => d.UserOT)
                .HasConstraintName("FK__RegisterO__UserO__45F4312A");
        });

        modelBuilder.Entity<ReportIndex>(entity =>
        {
            entity.ToTable("ReportIndex");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ReportPurchase>(entity =>
        {
            entity.ToTable("ReportPurchase");
        });

        modelBuilder.Entity<ReportType>(entity =>
        {
            entity.ToTable("ReportType");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ReportTypeName).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<RequestBuy>(entity =>
        {
            entity.ToTable("RequestBuy");

            entity.Property(e => e.ApprovedDate)
                .HasComment("Ngày duyệt")
                .HasColumnType("datetime");
            entity.Property(e => e.ApprovedID).HasComment("Người duyệt");
            entity.Property(e => e.BankCost)
                .HasComment("Chi phí ngân hàng")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate)
                .HasComment("")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerID).HasComment("Khách hàng");
            entity.Property(e => e.CustomsCost)
                .HasComment("Chi phí hải quan")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.DeadLine)
                .HasComment("Hạn hỏi giá")
                .HasColumnType("datetime");
            entity.Property(e => e.DeliveryCost)
                .HasComment("Chi phí vận chuyển")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.DepartmentID).HasComment("Phòng ban");
            entity.Property(e => e.IsApproved)
                .HasDefaultValue(false)
                .HasComment("Đã được duyệt hay chưa");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.IsImport).HasComment("Là hàng nhập khẩu(vd: cognex)");
            entity.Property(e => e.Note)
                .HasMaxLength(250)
                .HasComment("Ghi chú");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.ProjectID).HasComment("Dự án");
            entity.Property(e => e.Purpose)
                .HasMaxLength(250)
                .HasComment("Mục đích hỏi giá chung cho dự án, khách hàng nào, ai");
            entity.Property(e => e.QtySet).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.RequestBuyCode)
                .HasMaxLength(50)
                .HasComment("Mã yêu cầu hỏi giá");
            entity.Property(e => e.RequestBuyStatus).HasComment("Trạng thái YC: 0:Chưa thực hiện, 1: Đang thực hiện, 2: Đã hoàn thành");
            entity.Property(e => e.RequestPersonID).HasComment("Người yêu cầu");
            entity.Property(e => e.RequestType).HasComment("Loại hỏi giá: 1: thương mại, 2: sản xuất");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<RequestBuyDetail>(entity =>
        {
            entity.ToTable("RequestBuyDetail");

            entity.Property(e => e.BankCost)
                .HasComment("Chi phí ngân hàng")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.ContactEmail)
                .HasMaxLength(150)
                .HasComment("Email người liên hệ");
            entity.Property(e => e.ContactName)
                .HasMaxLength(150)
                .HasComment("Tên người liên hệ");
            entity.Property(e => e.ContactPhone)
                .HasMaxLength(150)
                .HasComment("Điện thoại người liên hệ");
            entity.Property(e => e.ContactWebsite).HasComment("Website liên hệ");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CurrencyRate).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.CurrencyUnit)
                .HasMaxLength(50)
                .HasComment("Đơn vị tiền tệ");
            entity.Property(e => e.CustomsCost)
                .HasComment("Tổng tiền chi phí hải quan")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.DeliveryCost)
                .HasComment("Chi phí bốc dỡ vận chuyển")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.FinishPrice)
                .HasComment("Đơn giá sau chi phí")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.FinishTotalPrice)
                .HasComment("Tổng tiền cuối cùng kết thúc hỏi giá")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.ManufacturerCode)
                .HasMaxLength(50)
                .HasComment("Mã hãng sản xuất");
            entity.Property(e => e.ManufacturerID).HasComment("ID hãng sản xuất");
            entity.Property(e => e.MaxDay)
                .HasComment("Ngày hàng về dự kiến lâu nhất")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.MinDay)
                .HasComment("Ngày hàng về dự kiến nhanh nhất lúc")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.MonitorID).HasComment("Người phụ trách");
            entity.Property(e => e.PartCode)
                .HasMaxLength(250)
                .HasComment("Mã vật tư thiết bị");
            entity.Property(e => e.PartCodeRTC)
                .HasMaxLength(50)
                .HasComment("Mã vật tư hàng hóa theo RTC");
            entity.Property(e => e.PartID).HasComment("ID vật tư, thiết bị, hàng hóa cần báo giá");
            entity.Property(e => e.PartName)
                .HasMaxLength(250)
                .HasComment("Tên vật tư thiết bị");
            entity.Property(e => e.PartNameRTC)
                .HasMaxLength(150)
                .HasComment("Tên vật tư hàng hóa theo RTC");
            entity.Property(e => e.PeriodExpected)
                .HasMaxLength(150)
                .HasComment("Khoảng thời gian hàng về dự kiến");
            entity.Property(e => e.Price)
                .HasComment("đơn giá vật tư nhập")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.PriceCurrency)
                .HasComment("Đơn giá vật tư theo ngoại tệ")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.PricePS).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.PriceVAT)
                .HasComment("Tiền vat")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Qty)
                .HasComment("Số lượng")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.QtyPS).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.QtySet).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Status).HasComment("Trạng thái: 0: chờ giá, 1: đã có giá");
            entity.Property(e => e.SupplierID).HasComment("ID nhà cung cấp");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(250)
                .HasComment("Tên nhà cung cấp");
            entity.Property(e => e.SupplierReplyDate)
                .HasComment("Ngày nhà cung cấp báo giá")
                .HasColumnType("datetime");
            entity.Property(e => e.TaxImporPrice)
                .HasComment("Chi phí thuể nhập khẩu trên một con vật tư")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TaxImporTotal)
                .HasComment("Tổng tiền chi phí nhập khẩu")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TaxImportPercent)
                .HasComment("Thuế nhập khẩu (%)")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalPrice)
                .HasComment("Tổng tiền nhập hàng")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalPriceCurrency)
                .HasComment("Tổng tiền ngoại tệ")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalVAT)
                .HasComment("Tổng tiền VAT")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasComment("Đơn vị tính");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.VAT)
                .HasComment("vat")
                .HasColumnType("decimal(18, 4)");
        });

        modelBuilder.Entity<RequestBuyRTC>(entity =>
        {
            entity.ToTable("RequestBuyRTC");

            entity.Property(e => e.CongNo)
                .HasMaxLength(500)
                .HasComment("Công nợ");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DonGiaNhap)
                .HasComment("Đơn Giá nhập")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GhiChu)
                .HasMaxLength(500)
                .HasComment("Ghi chú");
            entity.Property(e => e.GuestCode_).HasMaxLength(50);
            entity.Property(e => e.HanTT)
                .HasComment("hạn tt")
                .HasColumnType("datetime");
            entity.Property(e => e.HoaDon)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("Hoá đơn/Số tờ khai");
            entity.Property(e => e.MaSPMua).HasMaxLength(50);
            entity.Property(e => e.NgayDatHang)
                .HasComment("Ngày đặt hàng")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayDuKienVe)
                .HasComment("Ngày dự kiến hàng về")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayNhanYeuCau)
                .HasComment("Ngày nhận yêu cầu")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayVeThucTe)
                .HasComment("Ngày về thực tế")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayYeuCauGiao)
                .HasComment("Ngày yêu cầu giao")
                .HasColumnType("datetime");
            entity.Property(e => e.PriceSale).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductCode_).HasMaxLength(50);
            entity.Property(e => e.ProductName_).HasMaxLength(500);
            entity.Property(e => e.SupplierID).HasComment("Nhà cung cấp");
            entity.Property(e => e.TenSPMua).HasMaxLength(500);
            entity.Property(e => e.ThanhTien)
                .HasComment("Thành tiền")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TinhTrangDonHang).HasComment("values (0,chưa về) (1,đã về)");
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.Vat).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<RequestBuyRTCTTDH>(entity =>
        {
            entity.ToTable("RequestBuyRTCTTDH");

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(500);
        });

        modelBuilder.Entity<RequestBuySale>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_RequestBuySale_1");

            entity.ToTable("RequestBuySale");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.POCode).HasMaxLength(150);
            entity.Property(e => e.Project).HasMaxLength(150);
            entity.Property(e => e.RequestCode).HasMaxLength(150);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<RequestBuySaleDetail>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_RequestBuySale");

            entity.ToTable("RequestBuySaleDetail");

            entity.Property(e => e.ContactName).HasMaxLength(150);
            entity.Property(e => e.DeadLine).HasColumnType("datetime");
            entity.Property(e => e.IntoMoney).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Note).HasMaxLength(150);
            entity.Property(e => e.Qty).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QtyBuy).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TargetUse).HasMaxLength(150);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<RequestExport>(entity =>
        {
            entity.ToTable("RequestExport");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ExportDate).HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(550);
            entity.Property(e => e.RequestCode).HasMaxLength(250);
            entity.Property(e => e.RequestName).HasMaxLength(250);
            entity.Property(e => e.UserExport).HasMaxLength(250);
            entity.Property(e => e.UserRequest).HasMaxLength(250);
        });

        modelBuilder.Entity<RequestExportDetail>(entity =>
        {
            entity.ToTable("RequestExportDetail");

            entity.Property(e => e.Note).HasMaxLength(550);
            entity.Property(e => e.Project).HasMaxLength(250);
            entity.Property(e => e.Warehouse)
                .HasMaxLength(250)
                .HasComment("kho");
        });

        modelBuilder.Entity<RequestImport>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_ExportRequest");

            entity.ToTable("RequestImport");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ImportCode).HasMaxLength(150);
            entity.Property(e => e.ImportDate).HasColumnType("datetime");
            entity.Property(e => e.Importer).HasMaxLength(150);
            entity.Property(e => e.Note).HasMaxLength(150);
            entity.Property(e => e.Requester).HasMaxLength(150);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<RequestImportDetail>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_ExportRequestDetail");

            entity.ToTable("RequestImportDetail");

            entity.Property(e => e.IntoMoney).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Maker).HasMaxLength(150);
            entity.Property(e => e.Note).HasMaxLength(150);
            entity.Property(e => e.POSuplier).HasMaxLength(150);
            entity.Property(e => e.Pay).HasMaxLength(150);
            entity.Property(e => e.ProductCode).HasMaxLength(150);
            entity.Property(e => e.ProductName).HasMaxLength(150);
            entity.Property(e => e.Project).HasMaxLength(150);
            entity.Property(e => e.Qty).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.RequestCode).HasMaxLength(150);
            entity.Property(e => e.Suplier).HasMaxLength(150);
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.WareHouse).HasMaxLength(150);
        });

        modelBuilder.Entity<RequestInvoice>(entity =>
        {
            entity.ToTable("RequestInvoice");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateRequest).HasColumnType("datetime");
            entity.Property(e => e.DealineUrgency).HasColumnType("datetime");
            entity.Property(e => e.EmployeeRequestID).HasComment("ID của người gửi yêu cầu (Lấy từ bảng employee)");
            entity.Property(e => e.Note).HasMaxLength(550);
            entity.Property(e => e.ReceriverID).HasComment("ID của người nhận yêu cầu (Lấy từ bảng employee)");
            entity.Property(e => e.Status).HasComment("1: YC xuất HĐ, 2: Đã xuất nháp, 3: Đã phát hành hóa đơn");
            entity.Property(e => e.TaxCompanyID).HasComment("Công ty bán");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<RequestInvoiceDetail>(entity =>
        {
            entity.ToTable("RequestInvoiceDetail");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.InvoiceDate).HasColumnType("datetime");
            entity.Property(e => e.InvoiceNumber)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Note).HasMaxLength(550);
            entity.Property(e => e.ProductByProject).HasMaxLength(550);
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Specifications).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<RequestInvoiceFile>(entity =>
        {
            entity.ToTable("RequestInvoiceFile");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<RequestPaidPO>(entity =>
        {
            entity.ToTable("RequestPaidPO");

            entity.Property(e => e.DatePaid).HasColumnType("datetime");
            entity.Property(e => e.DatePaidExpected).HasColumnType("datetime");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 4)");
        });

        modelBuilder.Entity<RequestPrice>(entity =>
        {
            entity.ToTable("RequestPrice", tb => tb.HasComment("Yêu cầu hỏi giá"));

            entity.Property(e => e.BankCost)
                .HasComment("Chi phí ngân hàng")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate)
                .HasComment("")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerID).HasComment("Khách hàng");
            entity.Property(e => e.CustomsCost)
                .HasComment("Chi phí hải quan")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.DeadLine)
                .HasComment("Hạn hỏi giá")
                .HasColumnType("datetime");
            entity.Property(e => e.DeliveryCost)
                .HasComment("Chi phí vận chuyển")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.DepartmentID).HasComment("Phòng ban");
            entity.Property(e => e.IsApproved)
                .HasDefaultValue(false)
                .HasComment("Đã được duyệt hay chưa");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.IsImport).HasComment("Là hàng nhập khẩu(vd: cognex)");
            entity.Property(e => e.IsShowNotify).HasComment("Đã show notify khi tạo mới chưa");
            entity.Property(e => e.Note)
                .HasMaxLength(250)
                .HasComment("Ghi chú");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.ProjectID).HasComment("Dự án");
            entity.Property(e => e.Purpose)
                .HasMaxLength(250)
                .HasComment("Mục đích hỏi giá chung cho dự án, khách hàng nào, ai");
            entity.Property(e => e.QtySet).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.RequestPersonID).HasComment("Người yêu cầu hỏi giá");
            entity.Property(e => e.RequestPriceCode)
                .HasMaxLength(50)
                .HasComment("Mã yêu cầu hỏi giá");
            entity.Property(e => e.RequestStatus).HasComment("Trạng thái YC: 0:Chưa thực hiện, 1: Đang thực hiện, 2: Đã hoàn thành");
            entity.Property(e => e.RequestType).HasComment("Loại hỏi giá: 1: thương mại, 2: sản xuất");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<RequestPriceDetail>(entity =>
        {
            entity.ToTable("RequestPriceDetail", tb => tb.HasComment("Yêu cầu hỏi giá chi tiết"));

            entity.Property(e => e.AskDate).HasColumnType("datetime");
            entity.Property(e => e.AskPriceID).HasComment("Người hỏi giá, phụ trách");
            entity.Property(e => e.BankCost)
                .HasComment("Chi phí ngân hàng")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.ContactEmail)
                .HasMaxLength(150)
                .HasComment("Email người liên hệ");
            entity.Property(e => e.ContactName)
                .HasMaxLength(150)
                .HasComment("Tên người liên hệ");
            entity.Property(e => e.ContactPhone)
                .HasMaxLength(150)
                .HasComment("Điện thoại người liên hệ");
            entity.Property(e => e.ContactWebsite).HasComment("Website liên hệ");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CurrencyRate).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.CurrencyUnit)
                .HasMaxLength(50)
                .HasComment("Đơn vị tiền tệ");
            entity.Property(e => e.CustomerName).HasMaxLength(250);
            entity.Property(e => e.CustomsCost)
                .HasComment("Tổng tiền chi phí hải quan")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.DeadLine)
                .HasComment("ngày deadline")
                .HasColumnType("datetime");
            entity.Property(e => e.DeliveryCost)
                .HasComment("Chi phí bốc dỡ vận chuyển")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.FileName)
                .HasMaxLength(550)
                .HasComment("add file theo định dạng bất kì");
            entity.Property(e => e.FinishPrice)
                .HasComment("Đơn giá sau chi phí")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.FinishTotalPrice)
                .HasComment("Tổng tiền cuối cùng kết thúc hỏi giá")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Link)
                .HasMaxLength(550)
                .HasComment("Lick sản phẩm khi được lấy trên mạng");
            entity.Property(e => e.Manufacturer)
                .HasMaxLength(250)
                .HasComment("ID hãng sản xuất");
            entity.Property(e => e.MaxDay)
                .HasComment("Ngày hàng về dự kiến lâu nhất lúc hỏi giá")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.MinDay)
                .HasComment("Ngày hàng về dự kiến nhanh nhất lúc hỏi giá")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Note).HasMaxLength(550);
            entity.Property(e => e.NoteSlowCheck).HasMaxLength(250);
            entity.Property(e => e.PartCode)
                .HasMaxLength(250)
                .HasComment("Mã vật tư thiết bị");
            entity.Property(e => e.PartID).HasComment("ID vật tư, thiết bị, hàng hóa cần báo giá");
            entity.Property(e => e.PartName)
                .HasMaxLength(250)
                .HasComment("Tên vật tư thiết bị");
            entity.Property(e => e.PeriodExpected)
                .HasMaxLength(150)
                .HasComment("Khoảng thời gian hàng về dự kiến");
            entity.Property(e => e.Price)
                .HasComment("đơn giá vật tư nhập")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.PriceCurrency)
                .HasComment("Đơn giá vật tư theo ngoại tệ")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.PricePS).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.PriceVAT)
                .HasComment("Tiền vat")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Priority).HasComment("độ ưu tiên");
            entity.Property(e => e.ProjectName).HasMaxLength(250);
            entity.Property(e => e.Qty)
                .HasComment("Số lượng")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.QtyPS).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.QtySet).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Status)
                .HasMaxLength(500)
                .HasComment("Trạng thái hỏi giá: 0: chờ giá, 1: đã có giá");
            entity.Property(e => e.SupplierID).HasComment("ID nhà cung cấp");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(250)
                .HasComment("Tên nhà cung cấp");
            entity.Property(e => e.SupplierReplyDate)
                .HasComment("Ngày nhà cung cấp báo giá")
                .HasColumnType("datetime");
            entity.Property(e => e.TaxImporPrice)
                .HasComment("Chi phí thuể nhập khẩu trên một con vật tư")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TaxImporTotal)
                .HasComment("Tổng tiền chi phí nhập khẩu")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TaxImportPercent)
                .HasComment("Thuế nhập khẩu (%)")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalPrice)
                .HasComment("Tổng tiền nhập hàng")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalPriceCurrency)
                .HasComment("Tổng tiền ngoại tệ")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalVAT)
                .HasComment("Tổng tiền VAT")
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasComment("Đơn vị tính");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.VAT)
                .HasComment("vat")
                .HasColumnType("decimal(18, 4)");
        });

        modelBuilder.Entity<RequestPriceNotify>(entity =>
        {
            entity.ToTable("RequestPriceNotify");

            entity.Property(e => e.IsShow).HasDefaultValue(false);
            entity.Property(e => e.RequestPriceCode).HasMaxLength(50);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Role__3214EC2726CDFDE4");

            entity.ToTable("Role");

            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<RulePay>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("RulePay");

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ID).ValueGeneratedOnAdd();
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<SALE>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_Table_1");

            entity.ToTable("SALE");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.POCustomer).HasMaxLength(250);
            entity.Property(e => e.Sale1)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("Sale");
            entity.Property(e => e.SaleDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<SaleUserType>(entity =>
        {
            entity.ToTable("SaleUserType");

            entity.Property(e => e.PercentBonus).HasColumnType("decimal(18, 5)");
            entity.Property(e => e.SaleUserTypeCode).HasMaxLength(150);
            entity.Property(e => e.SaleUserTypeName).HasMaxLength(150);
        });

        modelBuilder.Entity<SalesPerformanceRanking>(entity =>
        {
            entity.ToTable("SalesPerformanceRanking");

            entity.Property(e => e.BonusAcc).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BonusAdd).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BonusRank).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BonusSales).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Coefficient).HasColumnType("decimal(18, 1)");
            entity.Property(e => e.Performance).HasColumnType("decimal(18, 5)");
            entity.Property(e => e.PerformanceOld).HasColumnType("decimal(18, 5)");
            entity.Property(e => e.TotalBonus).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalSale).HasColumnType("decimal(18, 5)");
        });

        modelBuilder.Entity<SealRegulation>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.SealCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SealName).HasMaxLength(250);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.ToTable("Stock");

            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StockCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StockName).HasMaxLength(50);
        });

        modelBuilder.Entity<StockLocation>(entity =>
        {
            entity.ToTable("StockLocation");

            entity.Property(e => e.StockLocationCode).HasMaxLength(50);
            entity.Property(e => e.StockLocationName).HasMaxLength(50);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("Supplier", tb => tb.HasComment("Nhà cung cấp"));

            entity.Property(e => e.Address).UseCollation("Vietnamese_CI_AS");
            entity.Property(e => e.Advantages).HasComment("Ưu điểm");
            entity.Property(e => e.BankAcount)
                .HasMaxLength(250)
                .UseCollation("Vietnamese_CI_AS");
            entity.Property(e => e.BankAcountName).UseCollation("Vietnamese_CI_AS");
            entity.Property(e => e.BankName).UseCollation("Vietnamese_CI_AS");
            entity.Property(e => e.ContactEmail).HasMaxLength(250);
            entity.Property(e => e.ContactName).HasMaxLength(100);
            entity.Property(e => e.ContactPhone).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DebtLimit)
                .HasComment("Hạn mức công nợ")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Defect).HasComment("Nhược điểm");
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.MST)
                .HasMaxLength(250)
                .UseCollation("Vietnamese_CI_AS");
            entity.Property(e => e.MainProduct).HasComment("Sản phẩm chính");
            entity.Property(e => e.Manufactures).HasComment("Hãng cung cấp");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasComment("Ghi chú");
            entity.Property(e => e.Office).UseCollation("Vietnamese_CI_AS");
            entity.Property(e => e.Phone).HasMaxLength(100);
            entity.Property(e => e.SkypeID).HasMaxLength(100);
            entity.Property(e => e.SupplierCode).HasMaxLength(50);
            entity.Property(e => e.SupplierName).HasMaxLength(200);
            entity.Property(e => e.SupplierShortName).HasMaxLength(200);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<SupplierContact>(entity =>
        {
            entity.ToTable("SupplierContact");

            entity.Property(e => e.ContactEmail).HasMaxLength(250);
            entity.Property(e => e.ContactName).HasMaxLength(150);
            entity.Property(e => e.ContactPhone).HasMaxLength(150);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ProductSale).HasComment("Sản phẩm phụ trách");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<SupplierSale>(entity =>
        {
            entity.ToTable("SupplierSale");

            entity.Property(e => e.AddressDelivery).HasMaxLength(550);
            entity.Property(e => e.AddressNCC).HasMaxLength(500);
            entity.Property(e => e.BankCharge).HasMaxLength(550);
            entity.Property(e => e.Brand).HasMaxLength(250);
            entity.Property(e => e.CodeNCC).HasMaxLength(500);
            entity.Property(e => e.Company).HasComment("1:RTC\r\n2:MVI\r\n3:APR\r\n4:YONKO");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Debt).HasMaxLength(350);
            entity.Property(e => e.Description).HasMaxLength(550);
            entity.Property(e => e.FedexAccount).HasMaxLength(550);
            entity.Property(e => e.MaNhom).HasMaxLength(250);
            entity.Property(e => e.MaSoThue).HasMaxLength(100);
            entity.Property(e => e.NVPhuTrach).HasMaxLength(550);
            entity.Property(e => e.NameNCC).HasMaxLength(500);
            entity.Property(e => e.NgayUpdate).HasColumnType("datetime");
            entity.Property(e => e.OrdererNCC).HasMaxLength(500);
            entity.Property(e => e.OriginItem).HasMaxLength(550);
            entity.Property(e => e.PhoneNCC).HasMaxLength(50);
            entity.Property(e => e.RuleIncoterm).HasMaxLength(550);
            entity.Property(e => e.ShortNameSupplier).HasMaxLength(550);
            entity.Property(e => e.SoTK).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<SupplierSaleContact>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_SupplierSaleContact_1");

            entity.ToTable("SupplierSaleContact");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Describe).HasMaxLength(550);
            entity.Property(e => e.SupplierEmail).HasMaxLength(250);
            entity.Property(e => e.SupplierName).HasMaxLength(550);
            entity.Property(e => e.SupplierPhone).HasMaxLength(500);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TSAllocationEvictionAsset>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_TSAllocationAsset");

            entity.ToTable("TSAllocationEvictionAsset", tb => tb.HasComment("Khoa"));

            entity.Property(e => e.AssetManagementID).HasComment("ID của grvMaster");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateAllocation)
                .HasComment("Ngày cấp phát, thu hồi,  tài sản cho nhân viên")
                .HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Trạng thái của sản phẩm");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TSAsset>(entity =>
        {
            entity.ToTable("TSAsset", tb => tb.HasComment("Khoa"));

            entity.Property(e => e.AssetCode).HasMaxLength(50);
            entity.Property(e => e.AssetType).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TSAssetAllocation>(entity =>
        {
            entity.ToTable("TSAssetAllocation");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateAllocation).HasColumnType("datetime");
            entity.Property(e => e.DateApproveAccountant).HasColumnType("datetime");
            entity.Property(e => e.DateApprovedHR).HasColumnType("datetime");
            entity.Property(e => e.DateApprovedPersonalProperty).HasColumnType("datetime");
            entity.Property(e => e.Status).HasComment("0: chưa duyệt; 1: Đã duyệt");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TSAssetAllocationDetail>(entity =>
        {
            entity.ToTable("TSAssetAllocationDetail");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TSAssetManagement>(entity =>
        {
            entity.ToTable("TSAssetManagement", tb => tb.HasComment("Khoa"));

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateBuy).HasColumnType("datetime");
            entity.Property(e => e.DateEffect)
                .HasComment("Thời gian áp dụng")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeID).HasComment("Trưởng phòng quản lý tài sản");
            entity.Property(e => e.Insurance)
                .HasComment("Thời gian bảo hành")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.OfficeActiveStatus).HasComment("1: Chưa active; 2: Đã active; 3: Crack");
            entity.Property(e => e.SourceID).HasComment("Nguồn gốc tài sản");
            entity.Property(e => e.SpecificationsAsset).HasComment("quy cách tài sản");
            entity.Property(e => e.SupplierID).HasComment("nhà cung cấp tài sản");
            entity.Property(e => e.TSCodeNCC)
                .HasMaxLength(550)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.WindowActiveStatus).HasComment("1: Chưa active; 2: Đã active; 3: Crack");
        });

        modelBuilder.Entity<TSAssetRecovery>(entity =>
        {
            entity.ToTable("TSAssetRecovery");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateApproveAccountant).HasColumnType("datetime");
            entity.Property(e => e.DateApprovedHR).HasColumnType("datetime");
            entity.Property(e => e.DateApprovedPersonalProperty).HasColumnType("datetime");
            entity.Property(e => e.DateRecovery).HasColumnType("datetime");
            entity.Property(e => e.Status).HasComment("0: chưa duyệt; 1: Đã duyệt");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TSAssetRecoveryDetail>(entity =>
        {
            entity.ToTable("TSAssetRecoveryDetail");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TSCalendarPeriodAsset>(entity =>
        {
            entity.ToTable("TSCalendarPeriodAsset");

            entity.Property(e => e.CodePeriod).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.NamePeriod).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TSLiQuidationAsset>(entity =>
        {
            entity.ToTable("TSLiQuidationAsset");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateLiquidation).HasColumnType("datetime");
            entity.Property(e => e.DateSuggest)
                .HasComment("Ngày đề nghị thanh lý")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeID).HasComment("Người phê duyệt thanh lý tài sản");
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TSLostReportAsset>(entity =>
        {
            entity.ToTable("TSLostReportAsset", tb => tb.HasComment("Khoa"));

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateLostReport)
                .HasComment("Ngày báo mất tài sản")
                .HasColumnType("datetime");
            entity.Property(e => e.Reason)
                .HasMaxLength(500)
                .HasComment("Lý do báo mất tài sản");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TSPeriodAsset>(entity =>
        {
            entity.ToTable("TSPeriodAsset", tb => tb.HasComment("Khoa"));

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateMaintenanceNearest)
                .HasComment("Ngày bảo dưỡng gần nhất")
                .HasColumnType("datetime");
            entity.Property(e => e.DateMaintenanceNext)
                .HasComment("Ngày bảo dưỡng tiếp theo")
                .HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.PeriodMaintenance).HasComment("Chu kỳ bảo dưỡng");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TSRepairAsset>(entity =>
        {
            entity.ToTable("TSRepairAsset", tb => tb.HasComment("Khoa"));

            entity.Property(e => e.ActualCosts).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ContentRepair).HasMaxLength(500);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateEndRepair).HasColumnType("datetime");
            entity.Property(e => e.DateRepair)
                .HasComment("Ngày sửa")
                .HasColumnType("datetime");
            entity.Property(e => e.DateReuse)
                .HasComment("Ngày đưa vào sử dụng lại")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpectedCost)
                .HasComment("Chi phí dự kiến")
                .HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasComment("Tên đơn vị sửa chữa, bảo dưỡng");
            entity.Property(e => e.Reason)
                .HasMaxLength(500)
                .HasComment("Lý do sửa");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TSReportBrokenAsset>(entity =>
        {
            entity.ToTable("TSReportBrokenAsset");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateReportBroken)
                .HasComment("Ngày báo hỏng tài sản")
                .HasColumnType("datetime");
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TSSourceAsset>(entity =>
        {
            entity.ToTable("TSSourceAsset", tb => tb.HasComment("Khoa"));

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.SourceCode).HasMaxLength(50);
            entity.Property(e => e.SourceName).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TSStatusAsset>(entity =>
        {
            entity.ToTable("TSStatusAsset", tb => tb.HasComment("Khoa"));

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TSTranferAsset>(entity =>
        {
            entity.ToTable("TSTranferAsset", tb => tb.HasComment("Khoa"));

            entity.Property(e => e.CodeReport)
                .HasMaxLength(50)
                .HasComment("Mã biên bản điều chuyển");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateApproveAccountant).HasColumnType("datetime");
            entity.Property(e => e.DateApprovedHR).HasColumnType("datetime");
            entity.Property(e => e.DateApprovedPersonalProperty).HasColumnType("datetime");
            entity.Property(e => e.DeliverID).HasComment("ID người giao tài sản lấy từ bảng employee");
            entity.Property(e => e.FromDepartmentID).HasComment("ID lấy từ Department (chuyển từ phòng)");
            entity.Property(e => e.Reason)
                .HasMaxLength(500)
                .HasComment("Lý do điều chuyển");
            entity.Property(e => e.ReceiverID).HasComment("ID người nhận lấy từ bảng employee");
            entity.Property(e => e.ToDepartmentID).HasComment("ID lấy từ DepartmentID (Phòng được nhận)");
            entity.Property(e => e.TranferDate)
                .HasComment("Ngày điều chuyển")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TSTranferAssetDetail>(entity =>
        {
            entity.ToTable("TSTranferAssetDetail");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TaxCompany>(entity =>
        {
            entity.ToTable("TaxCompany");

            entity.Property(e => e.Address).HasMaxLength(550);
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Director).HasMaxLength(250);
            entity.Property(e => e.FullName).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Position).HasMaxLength(250);
            entity.Property(e => e.TaxCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TaxDepartment>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_DepartmentTax");

            entity.ToTable("TaxDepartment");

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.Email).HasMaxLength(250);
            entity.Property(e => e.Hotline).HasMaxLength(50);
            entity.Property(e => e.IsShowHotline).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.PId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TaxEmployee>(entity =>
        {
            entity.ToTable("TaxEmployee");

            entity.Property(e => e.AnCa).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.AnhCBNV).HasColumnType("ntext");
            entity.Property(e => e.BHXH).HasMaxLength(550);
            entity.Property(e => e.BHYT).HasMaxLength(550);
            entity.Property(e => e.BankAccount).HasMaxLength(550);
            entity.Property(e => e.BirthOfDate).HasColumnType("datetime");
            entity.Property(e => e.CMTND).HasMaxLength(550);
            entity.Property(e => e.ChuyenCan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Code).HasMaxLength(550);
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
            entity.Property(e => e.IsSetupFunction).HasDefaultValue(false);
            entity.Property(e => e.JobDescription).HasMaxLength(550);
            entity.Property(e => e.Khac).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Leader).HasDefaultValue(0);
            entity.Property(e => e.LuongCoBan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LuongThuViec).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MST).HasMaxLength(550);
            entity.Property(e => e.MoiQuanHe).HasMaxLength(150);
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
            entity.Property(e => e.Status).HasDefaultValue(0);
            entity.Property(e => e.Telephone).HasMaxLength(550);
            entity.Property(e => e.TinhDcTamTru).HasMaxLength(150);
            entity.Property(e => e.TinhDcThuongTru).HasMaxLength(150);
            entity.Property(e => e.TinhTrangKyHD)
                .HasMaxLength(150)
                .HasComment("1: ");
            entity.Property(e => e.TonGiao).HasMaxLength(550);
            entity.Property(e => e.TongLuong).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TongPhuCap).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangPhuc).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(550);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.XangXe).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<TaxEmployeeContract>(entity =>
        {
            entity.ToTable("TaxEmployeeContract");

            entity.Property(e => e.ContractNumber)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateEnd)
                .HasComment("Ngày kết thúc hợp đồng")
                .HasColumnType("datetime");
            entity.Property(e => e.DateSign).HasColumnType("datetime");
            entity.Property(e => e.DateStart)
                .HasComment("Ngày bắt đầu hợp đồng")
                .HasColumnType("datetime");
            entity.Property(e => e.StatusSign).HasComment("1: chưa ký; 2: đã ký");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TaxEmployeePosition>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_EmployeePositionTax");

            entity.ToTable("TaxEmployeePosition");

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Team__3214EC27D79E1E62");

            entity.ToTable("Team");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TermCondition>(entity =>
        {
            entity.ToTable("TermCondition");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.ToTable("Test");

            entity.Property(e => e.Code).HasMaxLength(550);
            entity.Property(e => e.Name).HasMaxLength(350);
        });

        modelBuilder.Entity<TrackingMark>(entity =>
        {
            entity.Property(e => e.ApprovedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Deadline).HasColumnType("datetime");
            entity.Property(e => e.DocumentName).HasMaxLength(250);
            entity.Property(e => e.ExpectDateComplete).HasColumnType("datetime");
            entity.Property(e => e.RegisterDate).HasColumnType("datetime");
            entity.Property(e => e.SignDatedActual).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TrackingMarksFile>(entity =>
        {
            entity.ToTable("TrackingMarksFile");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TrackingMarksLog>(entity =>
        {
            entity.ToTable("TrackingMarksLog");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateLog).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TrackingMarksSeal>(entity =>
        {
            entity.ToTable("TrackingMarksSeal");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TrackingMarksTaxCompany>(entity =>
        {
            entity.ToTable("TrackingMarksTaxCompany");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TradePrice>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_TradePrice_1");

            entity.ToTable("TradePrice");

            entity.Property(e => e.BGDApprovedDate).HasColumnType("datetime");
            entity.Property(e => e.COM).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CurrencyRate).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LeaderApprovedDate).HasColumnType("datetime");
            entity.Property(e => e.RateCOM).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SaleApprovedDate).HasColumnType("datetime");
            entity.Property(e => e.UnitPriceDelivery).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPriceRTCVision).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TradePriceDetail>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_TradePrice");

            entity.ToTable("TradePriceDetail");

            entity.Property(e => e.BankCharge).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CMPerSet).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CurrencyRate).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CustomFees).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.FeeShipPcs).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Maker).HasMaxLength(150);
            entity.Property(e => e.Margin).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OrtherFees).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductCodeCustomer).HasMaxLength(150);
            entity.Property(e => e.Profit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProfitPercent).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProtectiveTariff).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProtectiveTariffPerPcs).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.STT).HasMaxLength(150);
            entity.Property(e => e.TotalImportPriceIncludeFees).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalImportPriceUSD).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalImportPriceVND).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPriceExpectCustomer).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPriceLabor).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPriceRTCVision).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalProtectiveTariff).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Unit).HasMaxLength(150);
            entity.Property(e => e.UnitImportPriceUSD).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitImportPriceVND).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPriceExpectCustomer).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPriceIncludeFees).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPricePerCOM).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TrainingRegistration>(entity =>
        {
            entity.ToTable("TrainingRegistration");

            entity.Property(e => e.CompletionAssessment)
                .HasMaxLength(550)
                .HasComment("Đánh giá mức độ hoàn thành");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateEnd).HasColumnType("datetime");
            entity.Property(e => e.DateRegister).HasColumnType("datetime");
            entity.Property(e => e.DateStart).HasColumnType("datetime");
            entity.Property(e => e.EmployeeID).HasComment("Người đăng ký lấy từ bảng Employee");
            entity.Property(e => e.IsCertification).HasComment("1: Có cấp chứng chỉ; 0: Không cấp");
            entity.Property(e => e.Purpose).HasMaxLength(550);
            entity.Property(e => e.SessionDuration).HasComment("Thời lượng 1 buổi (Phút)");
            entity.Property(e => e.SessionsPerCourse).HasComment("Số buổi/khóa");
            entity.Property(e => e.TrainingType).HasComment("1: Đào tạo nội bộ; 2: Đào tạo ngoài");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TrainingRegistrationApproved>(entity =>
        {
            entity.ToTable("TrainingRegistrationApproved");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateApproved).HasColumnType("datetime");
            entity.Property(e => e.EmployeeApprovedActualID).HasComment("Người duyệt thực tế");
            entity.Property(e => e.EmployeeApprovedID).HasComment("Người duyệt mặc định");
            entity.Property(e => e.Note).HasMaxLength(550);
            entity.Property(e => e.StatusApproved).HasComment("Trạng thái: 1: Đã duyệt; 2 Hủy duyệt...");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TrainingRegistrationApprovedFlow>(entity =>
        {
            entity.ToTable("TrainingRegistrationApprovedFlow");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(520);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TrainingRegistrationCategory>(entity =>
        {
            entity.ToTable("TrainingRegistrationCategory");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(250);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TrainingRegistrationDetail>(entity =>
        {
            entity.ToTable("TrainingRegistrationDetail");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DescriptionDetail).HasMaxLength(550);
            entity.Property(e => e.Note).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TrainingRegistrationFile>(entity =>
        {
            entity.ToTable("TrainingRegistrationFile");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(550);
            entity.Property(e => e.OriginName).HasMaxLength(550);
            entity.Property(e => e.OriginPath).HasMaxLength(550);
            entity.Property(e => e.ServerPath).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<UnitCount>(entity =>
        {
            entity.ToTable("UnitCount");

            entity.Property(e => e.UnitCode).HasMaxLength(250);
            entity.Property(e => e.UnitName).HasMaxLength(250);
        });

        modelBuilder.Entity<UnitCountKT>(entity =>
        {
            entity.ToTable("UnitCountKT");

            entity.Property(e => e.UnitCountCode).HasMaxLength(150);
            entity.Property(e => e.UnitCountName).HasMaxLength(150);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.LoginName, "Index_Users_LoginName");

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
            entity.Property(e => e.IsAdmin).HasDefaultValue(false);
            entity.Property(e => e.IsSetupFunction).HasDefaultValue(false);
            entity.Property(e => e.JobDescription).HasMaxLength(200);
            entity.Property(e => e.Leader).HasDefaultValue(0);
            entity.Property(e => e.LoginName).HasMaxLength(50);
            entity.Property(e => e.MST).HasMaxLength(250);
            entity.Property(e => e.PassExpireDate).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(250);
            entity.Property(e => e.Position).HasMaxLength(50);
            entity.Property(e => e.PostalCode).HasMaxLength(50);
            entity.Property(e => e.Qualifications).HasMaxLength(250);
            entity.Property(e => e.Resident).HasMaxLength(100);
            entity.Property(e => e.StartWorking).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasDefaultValue(0)
                .HasComment("Trạng thái hoạt động 0: hoạt động, 1: ngừng hoạt động");
            entity.Property(e => e.Telephone).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<UserGroup>(entity =>
        {
            entity.ToTable("UserGroup");

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<UserGroupLink>(entity =>
        {
            entity.ToTable("UserGroupLink");
        });

        modelBuilder.Entity<UserGroupRightDistribution>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_UserGroupRightDistribution_1");

            entity.ToTable("UserGroupRightDistribution");
        });

        modelBuilder.Entity<UserRightDistribution>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("UserRightDistribution");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.ID).ValueGeneratedOnAdd();
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<UserTeam>(entity =>
        {
            entity.ToTable("UserTeam");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<UserTeamLink>(entity =>
        {
            entity.ToTable("UserTeamLink");
        });

        modelBuilder.Entity<UserTeamSale>(entity =>
        {
            entity.ToTable("UserTeamSale");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(250);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<VehicleBookingFile>(entity =>
        {
            entity.ToTable("VehicleBookingFile");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<VehicleBookingManagement>(entity =>
        {
            entity.ToTable("VehicleBookingManagement");

            entity.Property(e => e.BookerVehicles).HasMaxLength(500);
            entity.Property(e => e.Category).HasComment("1:'Đăng ký đi'\r\n2:'Đăng ký giao hàng'\r\n3:'Xếp xe về'\r\n4:'Chủ động phương tiện'\r\n5:'Đăng ký về'\r\n6:'Đăng ký lấy hàng'\r\n");
            entity.Property(e => e.CompanyNameArrives).HasMaxLength(500);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DecilineApprove).HasMaxLength(500);
            entity.Property(e => e.DeliverName).HasMaxLength(500);
            entity.Property(e => e.DeliverPhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DepartureAddress).HasMaxLength(500);
            entity.Property(e => e.DepartureAddressActual).HasMaxLength(500);
            entity.Property(e => e.DepartureAddressStatus).HasComment("1:VP HN;2:VP BN;3:VP HP;4:VP HCM;5: Khác");
            entity.Property(e => e.DepartureDate).HasColumnType("datetime");
            entity.Property(e => e.DepartureDateActual).HasColumnType("datetime");
            entity.Property(e => e.DriverName).HasMaxLength(500);
            entity.Property(e => e.DriverPhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LicensePlate)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NameVehicleCharge).HasMaxLength(500);
            entity.Property(e => e.PackageName).HasMaxLength(500);
            entity.Property(e => e.PackageSize).HasMaxLength(250);
            entity.Property(e => e.PackageWeight).HasMaxLength(250);
            entity.Property(e => e.PassengerCode).HasMaxLength(100);
            entity.Property(e => e.PassengerDepartment).HasMaxLength(100);
            entity.Property(e => e.PassengerName).HasMaxLength(500);
            entity.Property(e => e.PassengerPhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ProblemArises).HasMaxLength(500);
            entity.Property(e => e.Province).HasMaxLength(100);
            entity.Property(e => e.ReasonDeciline).HasMaxLength(500);
            entity.Property(e => e.ReceiverCode).HasMaxLength(100);
            entity.Property(e => e.ReceiverName).HasMaxLength(500);
            entity.Property(e => e.ReceiverPhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SpecificDestinationAddress).HasMaxLength(500);
            entity.Property(e => e.TimeNeedPresent).HasColumnType("datetime");
            entity.Property(e => e.TimeReturn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.VehicleMoney).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.VehicleType).HasComment("Loại phương tiện (1: Oto, xe máy....; 2: Máy bay)");
        });

        modelBuilder.Entity<VehicleCategory>(entity =>
        {
            entity.ToTable("VehicleCategory");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<VehicleManagement>(entity =>
        {
            entity.ToTable("VehicleManagement");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DriverName).HasMaxLength(500);
            entity.Property(e => e.LicensePlate).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.VehicleName).HasMaxLength(500);
        });

        modelBuilder.Entity<ViewDetailKPIPurchase>(entity =>
        {
            entity.ToTable("ViewDetailKPIPurchase");

            entity.Property(e => e.WeekText).HasMaxLength(150);
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.ToTable("Warehouse");

            entity.Property(e => e.WarehouseCode).HasMaxLength(50);
            entity.Property(e => e.WarehouseName).HasMaxLength(250);
        });

        modelBuilder.Entity<WeekPlan>(entity =>
        {
            entity.ToTable("WeekPlan");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DatePlan).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<WorkPlan>(entity =>
        {
            entity.ToTable("WorkPlan");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.Location).HasMaxLength(150);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<WorkPlanDetail>(entity =>
        {
            entity.ToTable("WorkPlanDetail");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateDay).HasColumnType("datetime");
            entity.Property(e => e.Location).HasMaxLength(550);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.WorkCode)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vUser>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vUser");

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
            entity.Property(e => e.DepartmentCode).HasMaxLength(50);
            entity.Property(e => e.DepartmentName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(250);
            entity.Property(e => e.EmailCom).HasMaxLength(250);
            entity.Property(e => e.FullName).HasMaxLength(250);
            entity.Property(e => e.HandPhone).HasMaxLength(100);
            entity.Property(e => e.HomeAddress).HasMaxLength(100);
            entity.Property(e => e.ImagePath).HasColumnType("ntext");
            entity.Property(e => e.JobDescription).HasMaxLength(200);
            entity.Property(e => e.LoginName).HasMaxLength(50);
            entity.Property(e => e.MST).HasMaxLength(250);
            entity.Property(e => e.PassExpireDate).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(250);
            entity.Property(e => e.Position).HasMaxLength(50);
            entity.Property(e => e.PostalCode).HasMaxLength(50);
            entity.Property(e => e.Qualifications).HasMaxLength(250);
            entity.Property(e => e.Resident).HasMaxLength(100);
            entity.Property(e => e.StartWorking).HasColumnType("datetime");
            entity.Property(e => e.Telephone).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<vUserGroupLink>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vUserGroupLink");

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(250);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.UserCode).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
