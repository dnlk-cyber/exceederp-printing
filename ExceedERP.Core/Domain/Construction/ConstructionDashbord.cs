namespace ExceedERP.Core.Domain.Construction
{
    //public class ContructionDashboardDictionary
    //{
    //    [Key]
    //    public int ContructionDashboardDictionaryId { get; set; }
    //    public string Category { get; set; }
    //    public string Name { get; set; }
    //    public string Dimesion { get; set; }
    //    public decimal Value { get; set; }
    //}
    public class ConstructionProjectTypeData
    {
        public string ProjectType { get; set; }
        public int NoOfProject { get; set; }
    }
    public class ProjectPlanVsAchivementStatus
    {
        public decimal PlannedCost { get; set; }
        public decimal ActualCost { get; set; }
        public decimal VariationCost { get; set; }
        public string ProjectName { get; set; }
    }
    public class ProjectCostStatus
    {
        public decimal TotalContractAmount { get; set; }
        public decimal ActualCost { get; set; }
        public decimal RemainingAmount { get; set; }
        public string ProjectName { get; set; }
    }
    public class ProjectTimeStatus
    {
        public decimal TotalContractTime { get; set; }
        public decimal TimeElapsed { get; set; }
        public decimal RemainingTime { get; set; }
        public string ProjectName { get; set; }
    }
    public class ProjectPaymentAmount
    {
        public decimal TotalContractAmount { get; set; }

        public decimal PaymentAmountTodate { get; set; }
        public decimal RemainingAmount { get; set; }
        public string ProjectName { get; set; }

    }
}
