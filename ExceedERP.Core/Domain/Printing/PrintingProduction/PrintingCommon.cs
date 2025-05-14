using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing.PrintingProduction
{
    public enum OperationStatus
    {
        NotStarted = 0,
        InProgress = 1,
        Completed = 2
    }
    public class PrintingCommon
    {
        public OperationStatus OperationStatus { get; set; } = OperationStatus.NotStarted;

        public bool IsPrePressChecked { get; set; }
        public bool IsPressChecked { get; set; }
        public bool IsPostPressChecked { get; set; }
        public string PrePressCheckedBy { get; set; }
        public string PressCheckedBy { get; set; }
        public string PostPressCheckedBy { get; set; }
        public DateTime? PrePressCheckedTime { get; set; }
        public DateTime? PressCheckedTime { get; set; }
        public DateTime? PostPressCheckedTime { get; set; }

        public bool IsPrePressApproved { get; set; }
        public bool IsPressApproved { get; set; }
        public bool IsPostPressApproved { get; set; }
        public string PrePressApprovedBy { get; set; }
        public string PressApprovedBy { get; set; }
        public string PostPressApprovedBy { get; set; }
        public DateTime? PrePressApprovedTime { get; set; }
        public DateTime? PressApprovedTime { get; set; }
        public DateTime? PostPressApprovedTime { get; set; }
    }
}
