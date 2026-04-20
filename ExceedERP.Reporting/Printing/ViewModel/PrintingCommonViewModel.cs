using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Printing.ViewModel
{
    public class PrintingCommonViewModel
    {
        public string CreatedBy { get; set; }
        public Image CreatedBySignature { get; set; }
        public DateTime? DateCreated { get; set; } 
        public string CheckedBy { get; set; }
        public Image CheckedBySignature { get; set; }
        public DateTime? CheckedDate { get; set; }
        public string ApprovedBy { get; set; }
        public Image ApprovedBySignature { get; set; } 
        public DateTime? ApprovedDate { get; set; }
        //
        public string PrePressCheckedBy { get; set; }
        public string PressCheckedBy { get; set; }
        public string PostPressCheckedBy { get; set; }
        public DateTime? PrePressCheckedTime { get; set; }
        public DateTime? PressCheckedTime { get; set; }
        public DateTime? PostPressCheckedTime { get; set; }
        public string PrePressApprovedBy { get; set; }
        public string PressApprovedBy { get; set; }
        public string PostPressApprovedBy { get; set; }
        public DateTime? PrePressApprovedTime { get; set; }
        public DateTime? PressApprovedTime { get; set; }
        public DateTime? PostPressApprovedTime { get; set; }


        public Image PrePressCreatedBySignature { get; set; }
        public Image PrePressApprovedBySignature { get; set; }
        public Image PresPressCheckedBySignature { get; set; }

        public Image PressCreatedBySignature { get; set; }
        public Image PressApprovedBySignature { get; set; }
        public Image PressCheckedBySignature { get; set; }

        public Image PostPressCreatedBySignature { get; set; }
        public Image PostPressApprovedBySignature { get; set; }
        public Image PostPressCheckedBySignature { get; set; } 

        public string PostPressClosedBy { get; set; }
        public DateTime? PostPressDateClosed { get; set; }
        public Image PostPressClosedBySigniture { get; set; }
    }
}
