using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing.PrintingProduction
{
    public enum PrintingOperationStatus
    {
        Started = 0,
        PrePressInProgress = 1,
        PrePressCompleted = 2,
        PressInProgress = 3,
        PressCompleted = 4,
        PostPressInProgress = 5,
        PostPressCompleted = 6,
        Finished = 7,
        Shipped = 8,
        Reverted = 9
    }
    public class PrintingCommon : Operations
    { 
        public PrintingOperationStatus PrintingOperationStatus { get; set; }

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
        public bool IsClosed { get; set; } 
    }
}
