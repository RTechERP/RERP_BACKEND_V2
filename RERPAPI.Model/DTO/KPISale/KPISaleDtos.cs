namespace RERPAPI.Model.DTO.KPISale
{
    public class KPISaleCalculateRequest
    {
        public int EmployeeID { get; set; }
        public int PeriodID { get; set; }
        public int TemplateID { get; set; }
        public int? DepartmentID { get; set; }
        public bool SaveSnapshot { get; set; } = true;
    }

    public class KPISaleCalculateResult
    {
        public int KpiIndexID { get; set; }
        public string IndexCode { get; set; } = "";
        public string IndexName { get; set; } = "";
        public decimal GoalValue { get; set; }
        public decimal ResultValue { get; set; }
        public decimal AchievedPercent { get; set; }
        public decimal WeightPercent { get; set; }
        public decimal FinalScore { get; set; }
        public string UnitType { get; set; } = "";
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
        public List<object> Conditions { get; set; } = new();
        public List<KPISaleFilterGroupNode> Children { get; set; } = new();
    }
}