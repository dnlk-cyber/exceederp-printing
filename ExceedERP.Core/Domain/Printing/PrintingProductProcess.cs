using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing
{
    // here we map jobs to a specific printing process category
    
    public class PrintingProductProcess : TrackUserSettingOperation
    {
        [Key]
        public int PrintingProductProcessId { get; set; }
        [Display(Name ="Job Type")]
        public int JobTypeId { get; set; }

        [Display(Name ="Printing Process")]
        public int PrintingProcessId { get; set; }
        [Display(Name ="Printing Process Category")]
        public PrintingProcessCategory PrintingProcessCategory { get; set; }
        [Display(Name ="Process Name")]
        public string ProcessName { get; set; }
        [Display(Name ="Hrs Req")]
        public double HoursRequired { get; set; }
        [Display(Name ="Qty to Done")]
        public double QtyToDone { get; set; }
    }
}
