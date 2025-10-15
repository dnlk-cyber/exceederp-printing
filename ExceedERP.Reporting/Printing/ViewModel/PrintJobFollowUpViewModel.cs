using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Printing.ViewModel
{
    public class PrintJobFollowUpViewModel
    {
        public string JobOrderNo { get; set; }
        public string CustomerName { get; set; }
        public string JobType { get; set; }
        public string JobDescription { get; set; }
        public string PaperForText { get; set; }
        public string PaperForCover { get; set; }
        public int TextNoColor { get; set; }
        public int CoverNoColor { get; set; }
        public int Page { get; set; }
        public decimal Quantity { get; set; }
        public string BindingType { get; set; }
    }
    public class JobFollowUpTaskViweModel
    {
        public string ProcessCategory { get; set; }
        public string Activity { get; set; }
        public string MachineType { get; set; }
        public string AssingedTo { get; set; }
        public string Unit { get; set; }
        public decimal PlannedOutPut { get; set; }
        public decimal EstimatedHour { get; set; }
        public List<DailyFollowUpDetailViewModel> FollowUpDetails { get; set; }
    }

    public class DailyFollowUpDetailViewModel
    {
        public string OperatorName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Unit { get; set; }
        public decimal ActualOutput { get; set; }
        public double TimeSpent { get; set; }

    }

    public class TaskIssueViewModel
    {
        public string IssueType { get; set; }
        public string Description { get; set; }
        public DateTime ReportedTime { get; set; }
        public DateTime ResolvedTime { get; set; }
        public string Resolution { get; set; }
    }
    public class LogBookViewModel
    {
        public string ActivityType { get; set; }
        public int TimeSpent { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }

    }
    public class WastageViewModel
    {
        public decimal Amount { get; set; }
        public string Unit { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }

    }

    public class MachineMaintenanceViewModel
    {
        public string Reason { get; set; }
        public DateTime IssueReportedTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string TechnicianName { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }

    }
}
