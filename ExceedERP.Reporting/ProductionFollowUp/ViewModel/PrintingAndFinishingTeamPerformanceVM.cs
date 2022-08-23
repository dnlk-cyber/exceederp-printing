using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.ProductionFollowUp.ViewModel
{
    public class ProductionPerformanceVM : Organization
    {
        public string From { get; set; }
        public string TotalClient { get; set; }
        public string DeliveryFrom { get; set; }
        public string DeliveryTo { get; set; }
        public string CancelledDelivery { get; set; }
        public string To { get; set; }
        public decimal LastWeekPendingJobsBirr { get; set; }
        public decimal LastWeekPendingJobsCent { get; set; }
        public decimal CurrentWeekJobsBirr { get; set; }
        public decimal CurrentWeekJobsCent { get; set; }
        public decimal WeeklyProductionPlanBirr { get; set; }
        public decimal WeeklyProductionPlanCent { get; set; }
        public decimal WeeklyProductionPerformanceBirr { get; set; }
        public decimal WeeklyProductionPerformanceCent { get; set; }
        public decimal TotalBirr { get; set; }
        public decimal TotalCent { get; set; }
        public decimal PendingPlanBirr { get; set; }
        public decimal PendingPlanCent { get; set; }
        public decimal Percent { get; set; }

        public string CreatedBy { get; set; }
        public string ApprovedBy { get; set; }
        public List<PrintingAndFinishingTeamPerformanceItem> PrintingAndFinishingTeamPerformanceItems { get; set; }

    }

    public class PrintingAndFinishingTeamPerformanceItem
    {
     
        public string JobCode { get; set; }
        public string JobType { get; set; }
        public decimal LastWeekPendingJobs { get; set; }
        public decimal CurrentWeekOrderedJobs { get; set; }
        public decimal Total { get; set; }
        public string Unit { get; set; }
        public decimal WeeklyPlan { get; set; }
        public decimal WeeklyPerformance { get; set; }
        public decimal ProductionInProgress { get; set; }
        public decimal Percent { get; set; }
    }
}
