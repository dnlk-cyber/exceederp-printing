using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintingBindingStyle : TrackUserSettingOperation
    {
        [Key]
        public int PrintingBindingStyleId { get; set; }
        [Display(Name ="Binding Style Name")]
        public string BindingStyleName { get; set; }
        [Display(Name ="Unit Cost")]
        public decimal UnitCost { get; set; }
        public string Remark { get; set; } 
    }
}
