namespace RERPAPI.Model.DTO.KPISale
{
    public class KPISaleCalculateRequest
    {
        public int EmployeeID { get; set; }
        public int PeriodID { get; set; }
        public int TemplateID { get; set; }
        public int? DepartmentID { get; set; }
        public bool SaveSnapshot { get; set; } = true;
        public List<KPISaleReportAdjustmentInputDto> ReportAdjustments { get; set; } = new();
    }

    public class KPISaleTeamCalculateRequest
    {
        public int? TeamID { get; set; }
        public List<int> EmployeeIDs { get; set; } = new();
        public int PeriodID { get; set; }
        public int TemplateID { get; set; }
        public int? DepartmentID { get; set; }
        public bool SaveSnapshot { get; set; } = true;
        /// <summary>Khi true: tính lại data cá nhân cho từng employee và lưu snapshot.</summary>
        public bool RecalcPerEmployee { get; set; } = false;
        public List<KPISaleReportAdjustmentInputDto> ReportAdjustments { get; set; } = new();
    }

    /// <summary>Request sao chép toàn bộ chỉ tiêu từ một mẫu nguồn sang mẫu đích (đều là mẫu có sẵn).</summary>
    public class KPISaleCopyTemplateRequest
    {
        /// <summary>ID mẫu nguồn cần sao chép các chỉ tiêu.</summary>
        public int SourceTemplateID { get; set; }

        /// <summary>ID mẫu đích (bắt buộc phải &gt; 0).</summary>
        public int TargetTemplateID { get; set; }

        /// <summary>Có copy các chỉ tiêu hay không.</summary>
        public bool CopyIndexes { get; set; } = true;

        /// <summary>Bao gồm các chỉ tiêu đang IsActive = false.</summary>
        public bool IncludeInactiveIndexes { get; set; } = true;

        /// <summary>Có copy các ánh xạ (mapping) và bộ lọc (filter) của chỉ tiêu hay không.</summary>
        public bool CopyMappings { get; set; } = true;

        /// <summary>ID user hiện tại (phục vụ audit). Có thể bỏ trống.</summary>
        public int? CurrentUserID { get; set; }
    }

    /// <summary>Kết quả trả về cho API copy template.</summary>
    public class KPISaleCopyTemplateResponse
    {
        /// <summary>ID mẫu đích sau khi copy.</summary>
        public int TargetTemplateID { get; set; }

        /// <summary>Tên mẫu đích.</summary>
        public string TargetTemplateName { get; set; } = "";

        /// <summary>Số chỉ tiêu đã copy.</summary>
        public int CopiedIndexCount { get; set; }

        /// <summary>Số ánh xạ đã copy.</summary>
        public int CopiedMappingCount { get; set; }

        /// <summary>Danh sách ID chỉ tiêu mới tạo.</summary>
        public List<int> NewIndexIDs { get; set; } = new();
    }

    public class KPISaleTeamDto
    {
        public int ID { get; set; }
        public string TeamCode { get; set; } = "";
        public string TeamName { get; set; } = "";
        public string? Description { get; set; }
        public int? LeaderEmployeeID { get; set; }
        public string? LeaderEmployeeName { get; set; }
        public bool IsActive { get; set; }
        public List<int> EmployeeIDs { get; set; } = new();
    }

    public class KPISaleTeamUpsertRequest
    {
        public int? ID { get; set; }
        public string TeamCode { get; set; } = "";
        public string TeamName { get; set; } = "";
        public string? Description { get; set; }
        public int? LeaderEmployeeID { get; set; }
        public List<KPISaleTeamMemberItem> EmployeeIDs { get; set; } = new();
    }

    public class KPISaleTeamMemberItem
    {
        public int EmployeeId { get; set; }
        public bool IsAdmin { get; set; } = false;
        public bool IsPM { get; set; } = false;
    }

    public class KPISaleTotalPerformanceDto
    {
        public long ID { get; set; }
        public int? EmployeeID { get; set; }
        public int? PeriodID { get; set; }
        public int? TemplateID { get; set; }
        public decimal? FinalScore { get; set; }
        public DateTime? CalculatedDate { get; set; }
    }

    public class KPISaleCalculateResponse
    {
        public List<KPISaleCalculateResult> Items { get; set; } = new();
        public KPISaleTotalPerformanceDto? TotalPerformance { get; set; }
    }

    public class KPISaleReportAdjustmentInputDto
    {
        public int KpiIndexID { get; set; }
        public int? ReportScoreAdjustmentType { get; set; }
        public decimal? ReportScoreValue { get; set; }
    }

    public class KPISaleCalculateResult
    {
        public int KpiIndexID { get; set; }
        public int? ParentID { get; set; }
        public int? EmployeeID { get; set; }
        public int? TeamID { get; set; }
        public int? PeriodID { get; set; }
        public string? PeriodCode { get; set; }
        public DateTime? CalculatedDate { get; set; }
        public string IndexCode { get; set; } = "";
        public string IndexName { get; set; } = "";
        public string IndexType { get; set; } = "DETAIL";
        public decimal GoalValue { get; set; }
        public decimal ResultValue { get; set; }
        public decimal AchievedPercent { get; set; }
        public decimal WeightPercent { get; set; }
        public decimal FinalScore { get; set; }
        public string UnitType { get; set; } = "";
        public int? ReportScoreAdjustmentType { get; set; }
        public decimal? ReportScoreValue { get; set; }
        public int SortOrder { get; set; }
        public bool IsMainIndex { get; set; }
        public bool IsBold { get; set; }
    }

    public class KPISaleTargetUpsertRequest
    {
        public int ID { get; set; }
        public int EmployeeID { get; set; }
        public int PeriodID { get; set; }
        public int KpiIndexID { get; set; }
        public decimal GoalValue { get; set; }
        public decimal? WeightPercent { get; set; }
        public decimal? ProposedGoalValue { get; set; }
        public string? ApprovalStatus { get; set; }
    }

    public class KPISaleTargetProposeRequest
    {
        public int ID { get; set; }
        public decimal ProposedGoalValue { get; set; }
        public string? Note { get; set; }
    }

    public class KPISaleTargetRejectRequest
    {
        public string? Reason { get; set; }
    }

    public class AutoCreateTargetRequest
    {
        public int EmployeeID { get; set; }
        public int TemplateID { get; set; }
        public int PeriodID { get; set; }
    }

    public class KPISaleTargetUpdateWeightRequest
    {
        public decimal? WeightPercent { get; set; }
    }

    public class KPISaleEmployeeTemplateUpsertRequest
    {
        public int? ID { get; set; }
        public int EmployeeID { get; set; }
        public int TemplateID { get; set; }

        /// <summary>
        /// Loại kỳ: "Quarter" hoặc "Month" (mặc định "Month" nếu null)
        /// </summary>
        public string? PeriodType { get; set; }

        /// <summary>
        /// Mã kỳ: "2026-Q1" hoặc "2026-03"
        /// </summary>
        public string? PeriodValue { get; set; }

        public bool? IsActive { get; set; } = true;
        public string? Note { get; set; }
    }

    public class KPISaleEmployeeTemplateResponse
    {
        public int ID { get; set; }
        public int EmployeeID { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public string? DepartmentName { get; set; }
        public int TemplateID { get; set; }
        public string? TemplateCode { get; set; }
        public string? TemplateName { get; set; }
        public string? PeriodType { get; set; }
        public string? PeriodValue { get; set; }
        public int? PeriodID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? AssignedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? AssignedBy { get; set; }
        public string? Note { get; set; }
    }

    public class KPISaleFilterTreeResult
    {
        public object? Mapping { get; set; }
        public List<KPISaleFilterGroupNode> Groups { get; set; } = new();
    }

    public class KPISaleFilterGroupNode
    {
        public int ID { get; set; }
        public int MappingID { get; set; }
        public int? ParentGroupID { get; set; }
        public string LogicOperator { get; set; } = "";
        public int SortOrder { get; set; }
        public List<KPISaleFilterConditionDto> Conditions { get; set; } = new();
        public List<KPISaleFilterGroupNode> Children { get; set; } = new();
    }

    public class KPISaleFilterConditionDto
    {
        public int ID { get; set; }
        public int FilterGroupID { get; set; }
        public string ColumnName { get; set; } = null!;
        public string Operator { get; set; } = null!;
        public string ValueType { get; set; } = null!;
        public string? Value1 { get; set; }
        public string? Value2 { get; set; }
        public string DataType { get; set; } = null!;
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public string? Value1Display { get; set; }
        public string? Value2Display { get; set; }
    }

    public class KPISaleReportMetricConfigUpsertRequest
    {
        public int ID { get; set; }
        public int? TemplateID { get; set; }
        public int? KpiIndexID { get; set; }
        public string? MetricCode { get; set; }
        public string? MetricName { get; set; }
        public string? Description { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsRequired { get; set; }
        public string? InputType { get; set; }
        public string? QuarterFormulaType { get; set; }
        public string? QuarterFormulaConfig { get; set; }
        public bool? IncludeInTotalPerformance { get; set; }
        public decimal? TotalWeight { get; set; }
        public string? TotalFormulaRole { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public int? DecimalPlaces { get; set; }
        public string? NotePlaceholder { get; set; }
        public int? PeriodID { get; set; }
        public List<int> EmployeeIDs { get; set; } = new();
    }

    public class KPISaleReportMetricAssignmentDto
    {
        public int ID { get; set; }
        public int? MetricConfigID { get; set; }
        public int? EmployeeID { get; set; }
        public int? PeriodID { get; set; }
        public bool IsActive { get; set; }
    }

    public class KPISaleReportMetricInputUpsertDto
    {
        public int? MetricConfigID { get; set; }
        public decimal? Month1Percent { get; set; }
        public decimal? Month2Percent { get; set; }
        public decimal? Month3Percent { get; set; }
        public string? Note { get; set; }
    }

    public class KPISaleReportMonthlyInputSaveRequest
    {
        public int? EmployeeID { get; set; }
        public int? PeriodID { get; set; }
        public int? TemplateID { get; set; }
        public List<KPISaleReportMetricInputUpsertDto> Items { get; set; } = new();
    }

    public class KPISaleReportMetricCalculatedDto
    {
        public int MetricConfigID { get; set; }
        public int? KpiIndexID { get; set; }
        public string MetricCode { get; set; } = "";
        public string MetricName { get; set; } = "";
        public string InputType { get; set; } = "PERCENT";
        public bool IsRequired { get; set; }
        public bool IncludeInTotalPerformance { get; set; }
        public decimal? TotalWeight { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public int DecimalPlaces { get; set; }
        public string? NotePlaceholder { get; set; }
        public decimal? Month1Percent { get; set; }
        public decimal? Month2Percent { get; set; }
        public decimal? Month3Percent { get; set; }
        public decimal? QuarterPercent { get; set; }
        public decimal? ContributionMonth1 { get; set; }
        public decimal? ContributionMonth2 { get; set; }
        public decimal? ContributionMonth3 { get; set; }
        public decimal? ContributionQuarter { get; set; }
        public string? Note { get; set; }
        public string DataStatus { get; set; } = "DRAFT";
    }

    public class KPISaleReportPerformanceSummaryDto
    {
        public decimal TotalPerformanceMonth1 { get; set; }
        public decimal TotalPerformanceMonth2 { get; set; }
        public decimal TotalPerformanceMonth3 { get; set; }
        public decimal TotalPerformanceQuarter { get; set; }
    }

    public class KPISaleReportMetricsResponseDto
    {
        public int? EmployeeID { get; set; }
        public int? PeriodID { get; set; }
        public int? TemplateID { get; set; }
        public List<KPISaleReportMetricCalculatedDto> Items { get; set; } = new();
        public KPISaleReportPerformanceSummaryDto Summary { get; set; } = new();
        public List<int> AssignedMetricConfigIDs { get; set; } = new();
    }

    // ===== KPI Summary (monthly breakdown + quarterly) =====

    public class KpiSummaryRequest
    {
        public int EmployeeID { get; set; }
        public int QuarterPeriodID { get; set; }
        public int TemplateID { get; set; }
    }

    public class KpiSummaryResponse
    {
        public int QuarterPeriodID { get; set; }
        public string QuarterCode { get; set; } = "";
        public string QuarterName { get; set; } = "";
        public List<PeriodInfoDto> Periods { get; set; } = new();
        public List<KpiSummaryRowDto> Items { get; set; } = new();
        public KpiSummaryPerformanceDto Summary { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }

    public class PeriodInfoDto
    {
        public int PeriodID { get; set; }
        public string PeriodCode { get; set; } = "";
        public string PeriodName { get; set; } = "";
        public string PeriodType { get; set; } = "";
        public int SortOrder { get; set; }
    }

    public class KpiSummaryRowDto
    {
        public int IndexID { get; set; }
        public int? ParentID { get; set; }
        public string IndexCode { get; set; } = "";
        public string IndexName { get; set; } = "";
        public string IndexType { get; set; } = "";
        public decimal WeightPercent { get; set; }
        public bool IsBold { get; set; }
        public int SortOrder { get; set; }
        public int Depth { get; set; }
        public bool HasChildren { get; set; }

        public List<KpiSummaryValueDto> MonthlyValues { get; set; } = new();
        public KpiSummaryValueDto QuarterValue { get; set; } = new();

        public int ReportScoreAdjustmentType { get; set; }
        public decimal ReportScoreValue { get; set; }
    }

    public class KpiSummaryValueDto
    {
        public decimal Goal { get; set; }
        public decimal Result { get; set; }
        public decimal Score { get; set; }
        public decimal AchievedPercent { get; set; }
    }

    public class KpiSummaryPerformanceDto
    {
        public decimal Month1Score { get; set; }
        public decimal Month2Score { get; set; }
        public decimal Month3Score { get; set; }
        public decimal QuarterScore { get; set; }
    }

    // ============== KPI Reward & Ranking DTOs ==============

    public class KPISaleRankingCalculateRequest
    {
        public int PeriodId { get; set; }
        public int TemplateId { get; set; }
        public string? TeamCode { get; set; }
    }

    public class KPISaleRewardConfigDto
    {
        public int ID { get; set; }
        public string? ConfigCode { get; set; }
        public string? ConfigName { get; set; }
        public string? EmployeeType { get; set; }
        public int? TemplateId { get; set; }
        public decimal? RewardRate { get; set; }
        public decimal? Rank1BonusAmount { get; set; }
        public decimal? NewAccountBonusAmount { get; set; }
        public int? NewAccountKpiIndexId { get; set; }
        public string? NewAccountKpiIndexCode { get; set; }
        public string? NewAccountKpiIndexName { get; set; }
        public int? SalesAmountKpiIndexId { get; set; }
        public string? SalesAmountKpiIndexCode { get; set; }
        public string? SalesAmountKpiIndexName { get; set; }
        public int? RevenueKpiIndexId { get; set; }
        public string? RevenueKpiIndexCode { get; set; }
        public string? RevenueKpiIndexName { get; set; }
        public bool? IsActive { get; set; }
    }

    public class KPISaleRewardCoefficientDto
    {
        public int ID { get; set; }
        public int? ConfigId { get; set; }
        public string? EmployeeType { get; set; }
        public decimal? MinPerformance { get; set; }
        public decimal? MaxPerformance { get; set; }
        public decimal? Coefficient { get; set; }
        public int? Priority { get; set; }
        public bool? IsActive { get; set; }
    }

    public class KPISaleRankingResultDto
    {
        public int ID { get; set; }
        public int? EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public int? PeriodId { get; set; }
        public int? TemplateId { get; set; }
        public string? TeamCode { get; set; }
        public string? PositionType { get; set; }
        public decimal TotalSalesAmount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AchievementPercent { get; set; }
        public decimal Coefficient { get; set; }
        public int? Ranking { get; set; }
        public decimal SalesBonusAmount { get; set; }
        public decimal RankingBonusAmount { get; set; }
        public int? NewAccountCount { get; set; }
        public decimal NewAccountBonus { get; set; }
        public decimal OtherBonus { get; set; }
        public decimal TotalBonus { get; set; }
        public bool? IsCalculated { get; set; }
        public DateTime? CalculatedDate { get; set; }
    }

    public class KPISaleRankingSummaryDto
    {
        public int EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public string? TeamCode { get; set; }
        public string? PositionType { get; set; }
        public decimal TotalSalesAmount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AchievementPercent { get; set; }
        public decimal Coefficient { get; set; }
        public int? Rank { get; set; }
        public decimal SalesBonusAmount { get; set; }
        public decimal RankingBonusAmount { get; set; }
        public int NewAccountCount { get; set; }
        public decimal NewAccountBonus { get; set; }
        public decimal OtherBonus { get; set; }
        public decimal TotalBonus { get; set; }
    }

    public class KPISaleEmployeeRewardMappingDto
    {
        public int ID { get; set; }
        public int? EmployeeId { get; set; }
        public int? RewardConfigId { get; set; }
        public string? PositionType { get; set; }
        public string? TeamCode { get; set; }
        public string? ProjectIds { get; set; }
        public bool? IsActive { get; set; }
        public DateOnly? EffectiveFromDate { get; set; }
        public DateOnly? EffectiveToDate { get; set; }
    }

    // ===== KPI Sale Team Template DTOs =====
    public class KPISaleTeamTemplateUpsertRequest
    {
        public int? ID { get; set; }
        public int TeamID { get; set; }
        public int TemplateID { get; set; }

        /// <summary>
        /// Chỉ "Quarter" — gán mẫu theo quý
        /// </summary>
        public string PeriodType { get; set; } = "Quarter";

        /// <summary>
        /// Mã quý, ví dụ: "2026-Q1"
        /// </summary>
        public string PeriodValue { get; set; } = "";

        public bool? IsActive { get; set; } = true;
        public string? Note { get; set; }
    }

    public class KPISaleTeamTemplateResponse
    {
        public int ID { get; set; }
        public int TeamID { get; set; }
        public string? TeamCode { get; set; }
        public string? TeamName { get; set; }
        public int TemplateID { get; set; }
        public string? TemplateCode { get; set; }
        public string? TemplateName { get; set; }
        public string PeriodType { get; set; } = "Quarter";
        public string? PeriodValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? AssignedDate { get; set; }
        public string? AssignedBy { get; set; }
        public string? Note { get; set; }
    }
}
