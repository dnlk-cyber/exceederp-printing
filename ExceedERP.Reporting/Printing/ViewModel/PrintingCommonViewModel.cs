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
        
    }
}
